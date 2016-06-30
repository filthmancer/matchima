using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum SwipeDir{
	Up,Down,Left,Right
}


public class PlayerControl : MonoBehaviour {
	public static PlayerControl instance;
	public static Vector3 InputPos;
	public static bool HoldingSlot = false;
	public static UIButton HeldButton = null, SwapButton = null;
	public static ParticleSystem TouchParticle;
	//[HideInInspector]
	public Tile focusTile;
	public static Tile matchingTile;
	public float TimeSinceLastMatch = 0.0F;
	public float TimeWithoutInput = 0.0F;
	public float currentCombo;
	public List<Tile> selectedTiles = new List<Tile>();
	public List<Tile> finalTiles = new List<Tile>();
	[HideInInspector]
	public List<Tile> newTiles = new List<Tile>();

	public bool isMatching = false;
	public bool canMatch = true;

	[HideInInspector]
	public bool AttackMatch = false;
	[HideInInspector]
	public int AttackValue = 0;

	public Line _Line;

	public static int MatchCount
	{
		get{
			return PlayerControl.instance.selectedTiles.Count;
		}
	}

	private float tooltip_time = 0.78F, tooltip_current = 0.0F;
	private bool tooltip_showing = false;

	bool swiping, swipeSent;
	Vector2 last_position;

	public LineRenderer [] InnerLine, OuterLine;
	private Vector3 linepos;
	[SerializeField]
	private AudioSource sound_hover;

	public float ComboBonus
	{
		get
		{ 
			return 1.0F + (Mathf.Floor(selectedTiles.Count / Player.Stats.ComboCounter) * Player.Stats.ComboBonus);
		}
	}

	void Awake()
	{
		if(instance == null) instance = this;
		else if(instance != this) Destroy(this.gameObject);
	}

	void Start () {
		//_Line.SetPoints(Vector3.zero, -Vector3.one* 5);
		//_Line.DrawLightning();

		//InnerLine.sortingLayerID = Params._render.sortingLayerID;
		//OuterLine.sortingLayerID = Params._render.sortingLayerID;
		for(int x = 0; x < InnerLine.Length; x++)
		{
			InnerLine[x].sortingOrder = 1;
			OuterLine[x].sortingOrder = 1;
			InnerLine[x].SetWidth(0.2F, 0.2F);
			OuterLine[x].SetWidth(0.05F, 0.05F);
		}

		
		
	}

	void Update () {
		CheckTouch();
		if(GameManager.instance.isPaused) return;
		if(GameManager.instance.EnemyTurn) return;

		if(TimeSinceLastMatch < 30.0F) TimeSinceLastMatch += Time.deltaTime;
		if(TimeWithoutInput < 30.0F) TimeWithoutInput += Time.deltaTime;

		CheckInput();
		currentCombo = ComboBonus;
		if(TouchParticle != null) TouchParticle.transform.position = InputPos;
		if(focusTile != null && !HoldingSlot)
		{
			bool input = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

			if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || (PlayerControl.matchingTile == null && input)) 
			{
				isMatching = true;
				matchingTile = focusTile;
			}

			CheckCheats();
		}

		if(!canMatch) return;

		if(focusTile != null && !GameManager.instance.EnemyTurn && !UIManager.InMenu && selectedTiles.Count > 0)
		{
			for(int x = 0; x < InnerLine.Length; x++)
			{
				List<Vector3> finalpoints = new List<Vector3>();
				for(int i = 0; i < selectedTiles.Count;i++)
				{
					Tile child = selectedTiles[i];
					if(i == selectedTiles.Count - 1)
					{
						float softdist = 1.4F;
						float stretch = 1.0F;
						float str_decay = 1.0F;

						float dist = Vector3.Distance(InputPos, child.transform.position);
						Vector3 vel = InputPos - child.transform.position;
						Vector3 final = child.transform.position + vel.normalized * softdist;

						if(dist > softdist)
						{
							stretch = dist/softdist - (dist-softdist)/(dist-softdist);
							linepos = final;
						}
						else
						{
							linepos = InputPos;
						}
					}
					else
					{
						linepos = selectedTiles[i+1].transform.position;
					}
					Vector3 finalpoint = child.transform.position;
					float power = (0.01F + MatchCount * 0.005F) * (x + 1);
					Vector3 [] points = LightningLine(finalpoint, linepos, 5, power);
					finalpoints.AddRange(points);
				}
				InnerLine[x].SetVertexCount(finalpoints.Count);
				OuterLine[x].SetVertexCount(finalpoints.Count);
				for(int i = 0; i < finalpoints.Count; i++)
				{
					InnerLine[x].SetPosition(i, finalpoints[i]);
					OuterLine[x].SetPosition(i, finalpoints[i] + Vector3.back);
				}
				Color innercol = GameData.instance.GetGENUSColour(selectedTiles[0].Genus) * 0.9F;
				innercol = Color.Lerp(innercol, Color.white, x*0.3F);
				InnerLine[x].enabled = true;
				InnerLine[x].SetColors(innercol, innercol);

				Color outercol = Color.white;
				outercol = Color.Lerp(outercol, innercol, x*0.3F);
				OuterLine[x].enabled = true;
				OuterLine[x].SetColors(outercol, outercol);
			}
			
			
			
		}

		if(matchingTile != null)//PlayerControl.matchingTile != null)
		{
			if(PlayerControl.matchingTile.Genus == GENUS.ALL)
			{
				foreach(Tile child in selectedTiles)
				{
					if(child.Genus != GENUS.ALL) PlayerControl.matchingTile = child;
				}
			}
			AttackValue = Player.Stats._Attack;
			AttackMatch = false;
			foreach(Tile child in selectedTiles)
			{
				//If the match is an attack
				//if(child.Type.isEnemy) AttackMatch = true;

				//Sword Damage
				if(!child.Type.isEnemy) 
				{
					AttackValue += Player.Stats._Attack * child.Stats.Attack;
				}
			}
			//Adding Combo bonus multiplier to damage
			AttackValue = (int)((float)AttackValue * ComboBonus);

			//Adding ability, item and other values to attack
			List<int> finaldamage = Player.instance.ActiveDamage(AttackValue, selectedTiles);

			for(int i = 0; i < selectedTiles.Count; i++)
			{
				if(selectedTiles[i].Type.isEnemy)
				{
					selectedTiles[i].SetDamageWarning(finaldamage[i]);
				}
			}

			/*foreach(Tile child in selectedTiles)
			{
				//How much damage will be dealt to enemies
				if(child.Type.isEnemy) 
				{
					child.SetDamageWarning(AttackValue);
				}
			}*/
		}
		else 
		{
			AttackMatch = false;
			AttackValue = 0;
		}
	}

	public Vector3 [] LightningLine(Vector3 start, Vector3 end, int segments, float power)
	{
		Vector3 [] final = new Vector3[segments];
		Vector3 velocity = start - end;
		velocity.Normalize();
		Vector3 last = start;
		for(int i = 0; i < segments; i++)
		{
			final[i] = Vector3.Lerp(last, end, (float)i/segments);
			final[i] += Utility.RandomVectorInclusive(1, 1, 0) * power;

			last = final[i];
		}
		return final;
	}


	public void SwapSlots()
	{
		UISlotButton Held = HeldButton as UISlotButton;
		UISlotButton Swap = SwapButton as UISlotButton;
		if(Held != null && Swap != null)
		{
			Slot a = Held.slot;
			Slot b = Swap.slot;
			print(a);
			if(Held.Parent != null)
			{
				StartCoroutine(Held.Parent.GetSlotRoutine(b, Held.Index));
			}
			if(Swap.Parent != null && Swap is UISlotButton) 
			{
				StartCoroutine((Swap as UISlotButton).Parent.GetSlotRoutine(a, Swap.Index));
			}

			Swap.Setup(a);
			Held.Setup(b);
			b.Drag = DragType.None;
			a.Drag = DragType.Hold;
		}
		
		//if(a != null) a.Drag = DragType.None;
		//if(b != null) b.Drag = DragType.None;
		//if(b== null && HeldButton.Parent == null) Destroy(HeldButton.gameObject);
		//if(a== null && SwapButton.Parent == null) Destroy(SwapButton.gameObject);
		HoldingSlot = false;
		HeldButton = null;
		SwapButton = null;
	}

	public void CheckTouch()
	{
		Ray cursor = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(cursor.origin, cursor.direction*1000);

		Plane baseplane = new Plane(Vector3.back, Vector3.zero);
		float distance;
		baseplane.Raycast(cursor, out distance);
		InputPos = cursor.GetPoint(distance);
	}

	public void CheckInput()
	{
		if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) TimeWithoutInput = 0.0F;
		if(Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) 
		{
			CheckMatch();
			return;
		}
		if(!Input.GetMouseButton(0) && !Input.GetKey(KeyCode.Space))
		{
			if(TouchParticle != null) Destroy(TouchParticle.gameObject);
			if(sound_hover != null) sound_hover.enabled = false;
			if(focusTile != null) 
			{
				tooltip_current = 0.0F;
				tooltip_showing = false;
				UIManager.instance.ShowGearTooltip(false);
				focusTile.Reset(true);
				ResetSelected();
			}
			else
			{
				tooltip_current = 0.0F;
				tooltip_showing = false;
			}
			return;
		}

		Ray cursor = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(cursor, out hit, Mathf.Infinity))
		{
			Tile nt = hit.transform.gameObject.GetComponent<Tile>();
			if(nt != null && focusTile != nt) 
			{
				if(LastSelected() != null)
				{
					int d;
					int [] closest = LastSelected().Point.Closest(nt.Point.Base, out d);
					int [] vel = Utility.IntNormal(closest, nt.Point.Base);
					int [] final = new int [] { closest[0] + vel[0], closest[1] + vel[1]};
					focusTile = TileMaster.Tiles[final[0], final[1]];
				}
				else focusTile = nt;
				if(sound_hover == null)
				{
					PlayerControl.instance.sound_hover = AudioManager.instance.PlayClip(this.transform, AudioManager.instance.Tiles_Default, "hold_1");
					if(sound_hover != null)
					{
						PlayerControl.instance.sound_hover.GetComponent<DestroyTimer>().enabled = false;
						PlayerControl.instance.sound_hover.loop = true;
					}
					
				}
				else sound_hover.enabled = true;
				tooltip_current = 0.0F;
				tooltip_showing = false;
				//UIManager.instance.ShowTooltip(false);
			}
			else if(nt != null && focusTile == nt)
			{
				UIManager.instance.ShowGearTooltip(true, focusTile);

				/*if(tooltip_current > tooltip_time && ! tooltip_showing)
				{
					
					//UIManager.instance.ShowSimpleTooltip(true, focusTile.transform, focusTile._Name, focusTile.FullDescription);
					tooltip_showing = true;
				}
				else if(!tooltip_showing) tooltip_current += Time.deltaTime;*/
			}
			else
			{
				focusTile = null;
				if(sound_hover != null) sound_hover.enabled = false;
				//UIManager.instance.ShowTooltip(false);
				tooltip_current = 0.0F;
				tooltip_showing = false;
			}
		}
	}

	public void BackTo(Tile t)
	{
		int point = 0;
		bool move_back = false;

		for(int i = 0; i < selectedTiles.Count-1; i++)
		{
			if(selectedTiles[i] == t)
			{
				if(i == selectedTiles.Count-1) return;
				else 
				{
					point = i;
					move_back = true;
					break;
				}
			}
		}

		if(move_back)
		{
			int range = (selectedTiles.Count - 1) - point;
			if(range <= 0) return;
			foreach(Tile child in selectedTiles)
			{
				child.Reset(true);
			}

			selectedTiles.RemoveRange(point+1, range);
			PlayerControl.matchingTile = selectedTiles[point];

			//AudioManager.instance.PlayClipOn(this.transform, "Player", "Release");
			for(int i = 0; i < selectedTiles.Count; i++)
			{
				if(PlayerControl.matchingTile.Genus == GENUS.ALL)
				{
					if(selectedTiles[i].Genus != GENUS.ALL) PlayerControl.matchingTile = selectedTiles[i];
				}
				selectedTiles[i].SetState(TileState.Selected);
				if(i+1 <= selectedTiles.Count - 1) selectedTiles[i].SetArrow(selectedTiles[i+1], null, true);
			}
			//selectedTiles[0].arrowIn.enabled = false;
		}
	}

	public void GetSelectedTile(Tile t)
	{
		selectedTiles.Add(t);
	}

	public void CheckMatch()
	{
		if(focusTile == null) 
		{
			return;
		}

		for(int x = 0; x < InnerLine.Length; x++)
		{
			InnerLine[x].enabled = false;
			OuterLine[x].enabled = false;
		}
		

		bool match = true;
		if(selectedTiles == null || selectedTiles.Count < Player.RequiredMatchNumber) match = false;

	
		if(selectedTiles!= null && selectedTiles.Count > 0)
		{
			foreach(Tile child in selectedTiles)
			{
				if(child == null) continue;
				child.Reset(false);
				child.SetArrow(null, null, false);
				child.originalMatch = true;
			}
		}

		if(match)
		{
			//AudioManager.instance.PlayClip(this.transform, AudioManager.instance.Tiles_Default, "complete_1");
			//finalTiles.AddRange(selectedTiles);
			GameManager.instance.GetTurn();
			TimeSinceLastMatch = 0.0F;
		}
		else
		{
			isMatching = false;
			TileMaster.instance.ResetTiles(true);
			selectedTiles.Clear();
			selectedTiles = new List<Tile>();
			PlayerControl.matchingTile = null;
		}
	}

	public void CheckCheats()
	{
		if(Input.GetKeyDown(KeyCode.H)) focusTile.Info.Shift = ShiftType.None;
		if(Input.GetKeyDown(KeyCode.J)) TileMaster.instance.ReplaceTile(focusTile.Point.Base[0], focusTile.Point.Base[1], TileMaster.Types["cross"], GENUS.ALL, 6);
		if(Input.GetKeyDown(KeyCode.K)) TileMaster.instance.ReplaceTile(focusTile.Point.Base[0], focusTile.Point.Base[1], TileMaster.Types["cross"], GENUS.ALL, 1);
		if(Input.GetKeyDown(KeyCode.Plus)) 
		{
			Player.Stats.MapSize += new Vector2(0,1);
			if(TileMaster.instance.MapSize != Player.Stats.MapSize)
			{
				TileMaster.instance.MapSize = Player.Stats.MapSize;
				TileMaster.instance.GenerateGrid(null, 0.2F);
			}
		}
		if(Input.GetKeyDown(KeyCode.Minus)) 
		{
			Player.Stats.MapSize += new Vector2(1,0);
			if(TileMaster.instance.MapSize != Player.Stats.MapSize)
			{
				TileMaster.instance.MapSize = Player.Stats.MapSize;
				TileMaster.instance.GenerateGrid(null, 0.2F);
			}
		}
	}

	public Tile LastSelected()
	{
		if(selectedTiles.Count == 0) return null;
		else return selectedTiles[selectedTiles.Count-1];
	}

	public Tile SecondLastSelected()
	{
		if(selectedTiles.Count <= 1) return null;
		else return selectedTiles[selectedTiles.Count-2];
	}

	public bool IsSelected(Tile t)
	{
		foreach(Tile child in selectedTiles)
		{
			if(child == t) return true;
		}
		return false;
	}

	public bool IsInMatch(Tile t)
	{
		foreach(Tile child in finalTiles)
		{
			if(child == t) return true;
		}
		return false;
	}


	public void AddTilesToSelected(params Tile [] _newtiles)
	{
		foreach(Tile child in _newtiles)
		{
			bool add = true;
			foreach(Tile tile in selectedTiles)
			{
				if(child == tile) 
				{
					add = false;
					break;
				}
			}
			if(!add) continue;
			selectedTiles.Add(child);
			//finalTiles.Add(child);
		}
	}

	public void AddTilesToFinal(params Tile [] _newtiles)
	{
		foreach(Tile child in _newtiles)
		{
			bool add = true;
			foreach(Tile tile in finalTiles)
			{
				if(child == tile) 
				{
					add = false;
					break;
				}
			}
			if(!add) continue;
			finalTiles.Add(child);
			//finalTiles.Add(child);
		}
	}

	public void RemoveTilesToMatch(List<Tile> oldtiles)
	{
		foreach(Tile child in oldtiles)
		{
			for(int i = 0; i < finalTiles.Count; i++){
				if(finalTiles[i] == child)
				{
					finalTiles.RemoveAt(i);
				}
			}
		}
	}

	public void RemoveTileToMatch(Tile child)
	{
		for(int i = 0; i < finalTiles.Count; i++){
			if(finalTiles[i] == child)
			{
				finalTiles.RemoveAt(i);
			}
		}
	}

	public void ResetSelected()
	{
		foreach(Tile child in selectedTiles)
		{
			if(child != null)	child.Reset(true);
		}
		selectedTiles.Clear();
		for(int i = 0; i < InnerLine.Length; i++)
		{
			InnerLine[i].enabled = false;
			OuterLine[i].enabled = false;
		}
		focusTile = null;
		matchingTile = null;
	}

	
}
