using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColourSwap : Powerup {

	public GENUS [] PreferredColours;
	public UIObjtk [] ColCards;

	public GENUS TargetGenus;
	public List<GENUS> CardChoices;

	int [] choicenum = new int [] {2, 3, 3};
	int [] baddies = new int [] {1, 1, 2};
	float [] speed = new float [] {3.5F, 4.5F, 6.5F};
	protected override IEnumerator Minigame(int Level)
	{
		GameManager.instance.paused = true;
		yield return StartCoroutine(PowerupStartup());

		choice_set = false;
		choice_selected = 0;
		int good = choicenum[Level-1];
		int bad = baddies[Level-1];
		int choicetotal = good + bad;
		ColCards = new UIObjtk[choicetotal];
		CardChoices = new List<GENUS>();
		Vector3 [] tpoints = new Vector3[choicetotal];

		while(CardChoices.Count < good)
		{
			GENUS c = GetFinal();
			if(!CardChoices.Contains(c)) CardChoices.Add(c);
		}
		for(int b = 0; b < bad; b++)
		{
			CardChoices.Add(GENUS.OMG);
		}
		

		for(int i = 0; i < CardChoices.Count; i++)
		{
			tpoints[i] = UIManager.Objects.MiddleGear.transform.position + Vector3.down + Vector3.right;
			tpoints[i] += Vector3.Lerp(Vector3.left * (CardChoices.Count), Vector3.right*CardChoices.Count, (float)i/(float)choicetotal) * CameraUtility.OrthoFactor;

			ColCards[i] = (UIObjtk) CreateMinigameObj(0);
			ColCards[i].GetComponent<RectTransform>().sizeDelta = new Vector2(300,300);

			string spec = (i == CardChoices.Count - bad ? "grunt" : "resource");
			ColCards[i].Imgtk[0].SetSprite(TileMaster.Types[spec].Atlas, GameData.ResourceLong(CardChoices[i]));
			ColCards[i].Imgtk[1].SetSprite(TileMaster.Genus.Frames, GameData.ResourceLong(CardChoices[i]));
			LinkCard(ColCards[i], i);
			ColCards[i].transform.position = tpoints[i];
			yield return StartCoroutine(GameData.DeltaWait(0.25F));
		}
		yield return StartCoroutine(GameData.DeltaWait(0.1F));

		bool isRotating = true;
		bool spritechanged = false;
		float curr_rot = 0;
		float rot_speed = 290;
		while(isRotating)
		{
			for(int i = 0; i < ColCards.Length; i++)
			{
				ColCards[i].transform.rotation = Quaternion.Euler(0,curr_rot, 0);
			}
			curr_rot += Time.deltaTime * rot_speed;
			if(curr_rot >= 90 && !spritechanged) 
			{
				spritechanged = true;
				rot_speed = -rot_speed;
				for(int i = 0; i < ColCards.Length; i++)
				{
					ColCards[i].Imgtk[0].SetSprite(TileMaster.Types["resource"].Atlas, "Omega");
					ColCards[i].Imgtk[1].SetSprite(TileMaster.Genus.Frames, "Omega");
				}
			}
			else if(curr_rot <= 0 && spritechanged) isRotating = false;
			yield return null;
		}

		yield return StartCoroutine(GameData.DeltaWait(0.15F));

		bool isShuffling = true;
		int shuffle_loops = 3;
		List<Vector3> oldtargs = new List<Vector3>();
		oldtargs.AddRange(tpoints);
		for(int i = 0; i < shuffle_loops; i++)
		{
		//Set new points for the cards
			Vector3 [] newtargs = new Vector3[tpoints.Length];
			for(int v = 0; v < newtargs.Length; v++)
			{
				int t = Random.Range(0, oldtargs.Count);
				while(t == v) t = Random.Range(0, oldtargs.Count);
				newtargs[v] = oldtargs[t];
				oldtargs.RemoveAt(t);
			}

			oldtargs.Clear();
			oldtargs.AddRange(tpoints);

		//Shuffle the cards to new points
			isShuffling = true;
			float curr_shuff = 0.0F;
			float speed_shuff = speed[Level-1];
			while(isShuffling)
			{
				curr_shuff+= Time.deltaTime * speed_shuff;
				for(int c = 0; c < ColCards.Length; c++)
				{
					ColCards[c].transform.position = Vector3.Lerp(oldtargs[c], newtargs[c], curr_shuff);
				}
				if(curr_shuff >= 1.0F) isShuffling = false;
				yield return null;
			}
			oldtargs.Clear();
			oldtargs.AddRange(newtargs);
			yield return StartCoroutine(GameData.DeltaWait(0.15F));
		}
		

		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Pick the\ntarget colour!", 140, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!choice_set)
		{
			yield return null;
		}
		m.PoolDestroy();

		bool isRevealing  = true;
		curr_rot = 0;
		spritechanged = false;
		rot_speed = Mathf.Abs(rot_speed);

		while(isRevealing)
		{
			ColCards[choice_selected].transform.rotation = Quaternion.Euler(0, curr_rot, 0);

			curr_rot += Time.deltaTime * rot_speed;
			if(curr_rot >= 90 && !spritechanged) 
			{
				spritechanged = true;
				rot_speed = -rot_speed;
				string spec = (choice_selected == CardChoices.Count - bad ? "grunt" : "resource");
				ColCards[choice_selected].Imgtk[0].SetSprite(TileMaster.Types[spec].Atlas, GameData.ResourceLong(CardChoices[choice_selected]));
				ColCards[choice_selected].Imgtk[1].SetSprite(TileMaster.Genus.Frames, GameData.ResourceLong(CardChoices[choice_selected]));
			}
			else if(curr_rot <= 0 && spritechanged) isRevealing = false;
			yield return null;
		}

		yield return StartCoroutine(GameData.DeltaWait(0.15F));

		bool fail = choice_selected == CardChoices.Count - bad;
	//FINAL GENUS
		GENUS final = CardChoices[choice_selected];
		string title = fail ? "FAIL" : GameData.ResourceLong(final);

		UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, title, 140, GameData.Colour(final), 0.8F, 0.25F);
		isRevealing  = true;
		curr_rot = 0;
		spritechanged = false;
		rot_speed = Mathf.Abs(rot_speed);

		while(isRevealing)
		{
			for(int i = 0; i < ColCards.Length; i++)
			{
				if(i == choice_selected) continue;
				ColCards[i].transform.rotation = Quaternion.Euler(0, curr_rot, 0);
			}
			curr_rot += Time.deltaTime * rot_speed;
			if(curr_rot >= 90 && !spritechanged) 
			{
				spritechanged = true;
				rot_speed = -rot_speed;
				for(int i = 0; i < ColCards.Length; i++)
				{
					string spec = (i == CardChoices.Count - bad ? "grunt" : "resource");
					ColCards[i].Imgtk[0].SetSprite(TileMaster.Types[spec].Atlas, GameData.ResourceLong(CardChoices[i]));
					ColCards[i].Imgtk[1].SetSprite(TileMaster.Genus.Frames, GameData.ResourceLong(CardChoices[i]));
				}
			}
			else if(curr_rot <= 0 && spritechanged) isRevealing = false;
			yield return null;
		}
		yield return StartCoroutine(GameData.DeltaWait(0.25F));
		for(int i = 0; i < ColCards.Length; i++)
		{
			Destroy(ColCards[i].gameObject);
		}
		UIManager.instance.ScreenAlert.SetTween(0, false);

		if(final != GENUS.OMG) 	yield return StartCoroutine(Cast(Level, final));

		yield return new WaitForSeconds(GameData.GameSpeed(0.5F));
	}


	void LinkCard(UIObj obj, int num)
	{
		obj.AddAction(UIAction.MouseUp, () =>{SelectChoice(num);});
	}

	bool choice_set = false;
	int choice_selected = 0;
	void SelectChoice(int num)
	{
		choice_selected = num;
		choice_set = true;
	}

	IEnumerator Cast(int Level, GENUS final)
	{
		List<GENUS> prev = new List<GENUS>();

		for(int col = 0; col < Level; col++)
		{
			GENUS [] targets = GetTargets(final, prev);
			if(targets.Length == 0) break;
			GENUS StartGenus = targets[Random.Range(0, targets.Length)];

			if(col == 0)
			{
				StartGenus = Parent.Genus;
			}
			
			prev.Add(StartGenus);
			Tile [,] _tiles = TileMaster.Tiles;
			for(int x = 0; x < _tiles.GetLength(0); x++)
			{
				for(int y = 0; y < _tiles.GetLength(1); y++)
				{
					if(_tiles[x,y] == null) continue;
					if(_tiles[x,y].IsGenus(StartGenus, false, false)) 
					{
						int old_value = _tiles[x,y].Stats.Value;
						_tiles[x,y].ChangeGenus(final);
						EffectManager.instance.PlayEffect(_tiles[x,y].transform, "replace", GameData.instance.GetGENUSColour(_tiles[x,y].Genus));
						
					}
				}
			}
		}
		yield return null;
	}

	public GENUS [] GetTargets(GENUS fin, List<GENUS> previous)
	{
		List<GENUS> init = new List<GENUS>();
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			if(!previous.Contains(Player.Classes[i].Genus) && Player.Classes[i].Genus != fin)  init.Add(Player.Classes[i].Genus);
		}

		List<GENUS> final = new List<GENUS>();
		for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				if(final.Contains(TileMaster.Tiles[x,y].Genus)) continue;
				for(int a = 0; a < init.Count; a++)
				{
					if(TileMaster.Tiles[x,y].Genus == init[a]) 
					{
						final.Add(init[a]);
						break;
					}
				}
			}
		}
		return final.ToArray();
	}

	public GENUS GetFinal()
	{
		List<GENUS> final = new List<GENUS>();
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			if(Player.Classes[i] != Parent)  final.Add(Player.Classes[i].Genus);
		}
		return final[Random.Range(0, final.Count)];
	}
}
