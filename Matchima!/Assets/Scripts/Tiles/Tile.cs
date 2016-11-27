using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;


public class Tile : MonoBehaviour {

	public string Name;
	public virtual StCon _Name {
		get{
			string valpref = Stats.Value > 1 ? "+" + Stats.Value : "";
			return new StCon(valpref + " " + Info._TypeName, GameData.Colour(Genus), true, 60);}
	}
	
	public int x{get{return Point.Base[0];}}
	public int y{get{return Point.Base[1];}}
	public int GetValue(){CheckStats(); return Stats.Value;}

	public virtual StCon [] BaseDescription
	{
		get{
			List<StCon> basic = new List<StCon>();
			//if(Stats.Resource != 0)
			//basic.Add(new StCon("+" + Stats.GetValues()[0] + " Mana", GameData.Colour(Genus), false, 40));
			if(Stats.Heal != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[1] + "% Health", GameData.Colour(GENUS.STR), false, 40));
			if(Stats.Armour != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[2] + " Armour", GameData.Colour(GENUS.DEX), false, 40));
			return basic.ToArray();
		}
	}
	public virtual StCon [] Description
	{
		get{return null;}
	}
	public virtual StCon [] FullDescription
	{
		get{
			List<StCon> final = new List<StCon>();
			if(Genus == GENUS.OMG) final.Add(new StCon("Cannot be matched", Color.grey, false, 40));
			//final.AddRange(BaseDescription);
			if(Description != null)	final.AddRange(Description);
			if(EffectDescription != null) final.AddRange(EffectDescription);
			return final.ToArray();
		}
	}

	public virtual StCon [] EffectDescription
	{
		get
		{
			List<StCon> final = new List<StCon>();
			foreach(TileEffect child in Effects)
			{
				final.AddRange(child.Description);
			}
			return final.ToArray();
		}
	}

	[HideInInspector]
	public string DescriptionOverride;

	public TileStat InitStats;
	public TileStat Stats;

	public List<TileEffect> Effects = new List<TileEffect>();

	public GENUS Genus{
		get{return Info._GenusEnum;}
	}
	public virtual string GenusName
	{
		get{return GameData.ResourceLong(Genus);}
	}
	public SPECIES Type{
		get{return Info._Type;}
	}
	public string TypeName
	{
		get{return Info._TypeName;}
	}
	[HideInInspector]
	public TileInfo Info;

	public tk2dSpriteCollectionData Inner
	{
		get{return Params._render.Collection;}
	}
	public tk2dSpriteCollectionData Outer
	{
		get{return Params._border.Collection;}
	}
	public TileState state;
	[HideInInspector]
	public TilePointContainer Point;

	public bool Controllable;

	[HideInInspector]
	public bool isMatching;
	[HideInInspector]
	public bool isFalling;
	public bool BeforeMatchEffect;
	public bool BeforeTurnEffect, AfterTurnEffect;
	[HideInInspector]
	public bool AfterTurnCheck = false;
	[HideInInspector]
	public bool originalMatch = false;
	//[HideInInspector]
	public bool AttackedThisTurn;
	public TileParamContainer Params;

	[HideInInspector]
	public float collide_radius = 0.5F;
	[HideInInspector]
	public int matureTime = 0;

	private float land_time = 0.0F, land_time_end = 0.8F;

	protected Color def;
	public Color targetColor;
	private float gravity = 0.95F;
	protected Animator _anim;

	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float speed_max_falling = -18.5F;
	private float speed_max_rising = 10F;
	private float targetScale = 1.1F;
	private float defaultScale = 1.1f;

	private SphereCollider _collider;

	public bool NewVisuals = false;

	protected bool attacking;
	public bool isAttacking{
		get{return attacking;}
		set{attacking = value;}}


	Vector3 rotation = Vector3.zero;
	//[HideInInspector]
	public bool marked = false;

	//[HideInInspector]
	public bool Destroyed = false;
	public bool UnlockedFromGrid = false;
	private Vector3 linepos;
	[HideInInspector]
	public bool state_override = false;

	protected List<GameObject> particles;
	protected Transform _Transform;


	protected virtual TileUpgrade [] AddedUpgrades
	{
		get{
			return new TileUpgrade [0];
		}
	}

	protected virtual TileUpgrade [] BaseUpgrades
	{
		get
		{
			return new TileUpgrade []
			{
				new TileUpgrade(0.05F, 5, () => {InitStats._Hits.Max += 1;}),
				new TileUpgrade(1.0F, 1, () => {InitStats.Value += 1;}),
				new TileUpgrade(0.1F, 2, () => {InitStats.Resource +=1;})
			};
		}
	}

	protected TileUpgrade [] Upgrades
	{
		get
		{
			List<TileUpgrade> final = new List<TileUpgrade>();
			final.AddRange(BaseUpgrades);
			final.AddRange(AddedUpgrades);
			return final.ToArray();
		}
	}
	protected float UpgradeChanceTotal{
		get
		{
			float final = 0.0F;
			foreach(TileUpgrade child in Upgrades)
			{
				final += child.Chance;
			}
			return final;
		}
	}

	protected int _Scale = 1;
	protected Tile LineTarget;
	protected float LineTimer = 0.1F;

	// Use this for initialization
	public virtual void Start () {
       _Transform = this.transform;

	}

	public void Setup(GridInfo g, int x, int y)
	{
		//Setup(x,y, _Scale, Info, InitStats.Value-1);

		Point = new TilePointContainer(g, x,y,_Scale, this);
		_Transform = this.transform;
		_Transform.SetParent(g[x].Obj.transform);
		if(!Info.ShiftOverride) InitStats.Shift = Player.Stats.Shift;
		else InitStats.Shift = Info.Shift;
		transform.name = Info.Name + " | " + Point.Base[0] + ":" + Point.Base[1];
		CheckStats();
	}

	public virtual void Setup(GridInfo g, int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		Point = new TilePointContainer(g, x,y,scale, this);
		_Transform = this.transform;
		_Transform.SetParent(g[x].Obj.transform);
		Info = new TileInfo(inf);

		if(Params != null)
		{
			def = Color.white;//Inner.material.color;
			targetColor = def;
		}
		

		UnlockedFromGrid = false;
		isMatching = false;
		Destroyed = false;
		marked = false;
		originalMatch = false;
		state_override = false;
		_collider = GetComponent<SphereCollider>();
		//_collider.enabled = true;
		for(int i = 0; i < Effects.Count; i++)
		{
			Destroy(Effects[i].gameObject);
		}
		Effects = new List<TileEffect>();

		defaultScale = 1.1F * scale;
		targetScale = defaultScale;
		_Scale = scale;
		attacking = false;

		Name = Info.Name;
		if(!Info.ShiftOverride) InitStats.Shift = Player.Stats.Shift;
		else InitStats.Shift = Info.Shift;
		transform.name = Info.Name + " | " + Point.Base[0] + ":" + Point.Base[1];

		int val = value_inc;
		for(int i = 1; i < scale; i++)
		{
			val *= 2;
		}
		InitStats.Value = Info.Value;
		if(!Type.isEnemy) InitStats.Value += Player.Stats.ValueInc;
		if(InitStats._Hits.Max == 0) InitStats._Hits.Max = 1;

		InitStats._Hits.Current = InitStats._Hits.Max;
		AddUpgrades(val);

		InitStats.Lifetime = 0;
		InitStats.isNew = true;
		InitStats.value_soft = (float) InitStats.Value;

		if(Info.FinalEffects != null)
		{
			for(int i = 0; i < Info.FinalEffects.Count; i++)
			{
				AddEffect(Info.FinalEffects[i]);
			}
		}

		CheckStats();

		Stats._Hits.Current = Stats._Hits.Max;
		SetSprite();
		ClearActions();

		if(GameManager.instance.BotTeamTurn) SetState(TileState.Locked);
		else SetState(TileState.Idle);
	}

	public virtual void GetParams(params string [] args)
	{

	}

	int looseupdate_frame_check = 0;
	int looseupdate_frames = 5;
	// Update is called once per frame
	public virtual void Update () {
		if(Destroyed || UnlockedFromGrid) return;

		if(GameManager.inStartMenu) 
		{
			if(Player.loaded && UIManager.loaded) Destroy(this.gameObject); 
			if(transform.position.y < -10) Destroy(this.gameObject);
			return;
		}
		
		if(!Controllable && TileMaster.Grid == Point.Grid)
		{
			if(TileMaster.Tiles[Point.Base[0], Point.Base[1]] != this && TileMaster.Tiles[Point.Base[0], Point.Base[1]] != null && !Destroyed)
			{
				TileMaster.instance.SetFillGrid(true);
				DestroyThyself();
			}
			if(TileMaster.Grid[x,y].Empty && !GameManager.instance.isPaused)
			{
				//TileMaster.instance.SetFillGrid(true);
				DestroyThyself();
			}
		}
		else if(TileMaster.Grid != Point.Grid) return;

		if(TileMaster.Tiles.GetLength(0) <= Point.Base[0] || TileMaster.Tiles.GetLength(1) <= Point.Base[1]) return;

		if(Stats.Shift != ShiftType.None && !Destroyed)
		{
			Velocity();
		}
		
		if(PlayerControl.instance.TimeWithoutInput < 1.0F) looseupdate_frame_check = looseupdate_frames;
		if((looseupdate_frame_check++) >= looseupdate_frames)
		{
			looseupdate_frame_check = 0;

			transform.name = Info.Name + " | " + Point.Base[0] + ":" + Point.Base[1];
			if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, targetColor, 0.6F);
			if(Params._border != null) Params._border.color = Color.Lerp(Params._border.color, targetColor, 0.6F);
			
			if(Stats._Hits.Current > 1)
			{
				if(Params.HitCounter != null && !Params.HitCounter.activeSelf) Params.HitCounter.SetActive(true);
				if(Params.HitCounterText != null) Params.HitCounterText.text = "" + Stats._Hits.Current;
			}
			else if(Params.HitCounter != null && Params.HitCounter.activeSelf) Params.HitCounter.SetActive(false);
				
			if(!isMatching) transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5);
			Params._shiny.enabled = IsState(TileState.Selected);
		}
		
		if(!IsState(TileState.Selected))
		{
			if(GameManager.instance.BotTeamTurn) return;
			bool hascontrol = PlayerControl.instance.Controller != null;
			bool controlgenus = IsGenus(PlayerControl.instance.Controller_Genus);
			if(PlayerControl.instance.focusTile == this)
			{
				if(hascontrol)
				{
					if(controlgenus)
					{
						if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) 
						{
							Tile lastselected = PlayerControl.instance.LastSelected();
							int [] diff;
							if(lastselected!= null && lastselected != this)
							{
								lastselected.SetArrow(this);
							}
							if(lastselected == null || (lastselected != this && TileMaster.instance.AreNeighbours(lastselected, this, out diff)))
							{	
								SetState(TileState.Selected);
								PlayerControl.instance.AddTilesToSelected(this);
								PlayAudio("touch");
							}
							else SetState(TileState.Idle);	
						}
					}
					else
					{
						if(!IsState(TileState.Locked)) PlayAudio("touch");
						SetState(TileState.Locked);
					}	
				}
				else
				{
					SetState(TileState.Selected);
				}
				
			}
			else if(hascontrol && !controlgenus)
			{
				SetState(TileState.Locked);
			}
			else if(PlayerControl.instance.focusTile == this && PlayerControl.HoldingSlot) 
			{
				if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) 
				{

					SetState(TileState.Selected);
				}
			}
			else if(UIManager.InMenu)
			{
				SetState(TileState.Locked);
			}
			else  if(!IsState(TileState.Idle))
			{
				SetState(TileState.Idle);
			}
		}	
		else if(IsState(TileState.Selected))
		{
			if(PlayerControl.instance.focusTile == this && PlayerControl.instance.SecondLastSelected() == this)	
			{
				PlayerControl.instance.BackTo(this);		
			}
			else SetState(TileState.Idle);
		}		
	}

	public virtual AudioSource PlayAudio(string clip, float volume = 1.0F)
	{
		return AudioManager.instance.PlayTileAudio(this, clip, volume);
	}

	void LateUpdate()
	{
		_Transform.rotation = Quaternion.Slerp(_Transform.rotation, Quaternion.Euler(rotation), Time.deltaTime * 8);
	}

	
	public void Velocity()
	{
		Vector3 velocity = Vector3.zero;
		
		if(Stats.Shift == ShiftType.Up) 		
		{
			velocity = new Vector3(0, -1, 0);
			//rotation = new Vector3 (0,0,180);
		}
		else if(Stats.Shift == ShiftType.Down)
		{
		 	velocity = new Vector3(0, 1, 0);
		 	//rotation = new Vector3 (0,0,0);
		 }
		else if(Stats.Shift == ShiftType.Left)
		{
			velocity = new Vector3(1, 0, 0);
			//rotation = new Vector3 (0,0,-90);
		}
		else if(Stats.Shift == ShiftType.Right)
		{
		 	velocity = new Vector3(-1, 0, 0);
		 	//rotation = new Vector3 (0,0,90);
		}
		else if(Stats.Shift == ShiftType.None)
		{
			velocity = Vector3.zero;
			rotation = Vector3.zero;
			speed = 0;
			return;
		}

		/*Ray velRay = new Ray(_Transform.position, velocity);
		RaycastHit hit;

		if(Physics.Raycast(_Transform.position, -velocity, out hit, (speed + collide_radius) * Time.deltaTime * 80))
		{
			Tile hit_tile = hit._Transform.gameObject.GetComponent<Tile>();

			if(hit_tile != null)
			{
				Vector3 clamppos = _Transform.position;
				if(Stats.Shift == ShiftType.Down && hit_tile.Point.Base[1] < Point.Base[1]) 
				{
					clamppos.y = Mathf.Clamp(clamppos.y, hit_tile._Transform.position.y + collide_radius * 2, 100);
				}
				else if(Stats.Shift == ShiftType.Up && hit_tile.Point.Base[1] > Point.Base[1])
				{
					clamppos.y = Mathf.Clamp(clamppos.y, -100, hit_tile._Transform.position.y - collide_radius * 2);
				}
				if(Stats.Shift == ShiftType.Left && hit_tile.Point.Base[0] < Point.Base[0]) 
				{
					clamppos.x = Mathf.Clamp(clamppos.x, hit_tile._Transform.position.x + collide_radius * 2, 100);
				}
				else if(Stats.Shift == ShiftType.Right && hit_tile.Point.Base[0] > Point.Base[0])
				{
					clamppos.x = Mathf.Clamp(clamppos.x, -100, hit_tile._Transform.position.x - collide_radius * 2);
				}
				speed = hit_tile.speed;

			}
			else 
			{
				float finalspeed = speed - gravity;
				speed = Mathf.Clamp(finalspeed, speed_max_falling, speed_max_rising);
			}
		}
		else
		{*/
			float finalspeed = speed - gravity;
			speed = Mathf.Clamp(finalspeed, speed_max_falling, speed_max_rising);
		//}
		if(!GameManager.inStartMenu && TileMaster.Grid != null)
		{
			if(Stats.Shift == ShiftType.Down && _Transform.position.y <= Point.targetPos.y - (speed*Time.deltaTime)) 
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				_Transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, _Transform.position.z);
				OnLand();
			}
			else if(Stats.Shift == ShiftType.Up && _Transform.position.y >= Point.targetPos.y + (speed*Time.deltaTime)) 
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				
				_Transform.position = new Vector3(_Transform.position.x, Point.targetPos.y, _Transform.position.z);
				OnLand();
			}
			else if(Stats.Shift == ShiftType.Right && _Transform.position.x >= Point.targetPos.x + (speed * Time.deltaTime))
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				
				_Transform.position = new Vector3(Point.targetPos.x, _Transform.position.y, _Transform.position.z);
				OnLand();
			}
			else if(Stats.Shift == ShiftType.Left && _Transform.position.x <= Point.targetPos.x - (speed * Time.deltaTime))
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				
				_Transform.position = new Vector3(Point.targetPos.x, _Transform.position.y, _Transform.position.z);
				OnLand();
			}
			else 
			{
				if(speed >= 0.0F) speed = -0.1F;
				_Transform.localScale = new Vector3(defaultScale+speed/90, defaultScale,defaultScale);
				if(_collider.enabled) _collider.enabled = false;
				isFalling = true;
			}
		}

		if(speed != 0.0F) _Transform.position += velocity * speed * Time.deltaTime;
	}

	public void SetState(TileState _state, bool _override = false)
	{
		if(state == _state || (state_override && !_override)) return;
		state_override = _override;
		state = _state;
		switch(_state)
		{
			case TileState.Hover:
			targetScale = defaultScale * 1.2F;
			targetColor = def;
			break;
			case TileState.Idle:
			targetScale = defaultScale;
			targetColor = def;
			SetCounter("");
			//Params._render.transform.position = Point.targetPos;
			SetArrow(null, null, false);
			break;
			case TileState.Locked:
			targetScale = defaultScale;
			targetColor = Color.Lerp(def, Color.black, 0.65F);
			break;
			case TileState.Selected:
			targetScale = defaultScale * 1.2F;
			targetColor = def;
			SetArrow(null, PlayerControl.instance.LastSelected(), true);
			break;
		}	
	}

	public bool IsState(TileState _state)
	{
		return state == _state;
	}

	private List<string> Flags = new List<string>();
	public bool HasFlag(string s)
	{
		for(int i = 0; i < Flags.Count; i++)
		{
			if(string.Equals(Flags[i], s)) return true;
		}
		return false;
	}

	public void AddFlag(string s)
	{
		Flags.Add(s);
	}

	public void OnAlert()
	{
		AudioManager.instance.QueueAlert(this);
		//PlayAudio("alert");
		InitStats.isAlerted = true;
		EffectManager.instance.PlayEffect(transform, "alert", Color.white);	
		MiniAlertUI m = UIManager.instance.MiniAlert(transform.position + Vector3.down * 0.2F, "!", 230, Color.black, 0.4F, 0.07F);
		m.Txt[0].outlineColor = GameData.Colour(Genus);
	}

	[HideInInspector]
	public int BeforeMatchPriority = 0;
	public virtual IEnumerator BeforeMatch(Tile Controller)
	{
		if(this == null) yield break;//return false;
		if(Controllable) yield break; //return ControlMatch(0);

		//InitStats.TurnDamage += Controller.Stats.Attack;
		InitStats._Hits.Current -= 1;
		CheckStats();
		PlayAudio("match");
		if(Stats._Hits.Current <= 0)
		{
			isMatching = true;
			//Stats.Value *=  resource;
			CollectThyself(true);
			TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;		
		}
		else 
		{
			isMatching = false;
			CollectThyself(false);
			EffectManager.instance.PlayEffect(_Transform,Effect.Attack);
		}
		yield break;
	}

	public void AddUpgrades(int v)
	{
		while(v > 0)
		{
			float c = 0.0F;
			float f = UnityEngine.Random.value * UpgradeChanceTotal;
			for(int i = 0; i < Upgrades.Length; i++)
			{
				if(f > c && f < c + Upgrades[i].Chance)
				{
					//if(v > Upgrades[i].Points)
					//{
						Upgrades[i].Method();
						v -= Upgrades[i].Points;
					//}
					break;
				}
				c++;
			}
		}
	}

	public virtual bool Match(int resource)
	{
		if(this == null) return false;
		if(Controllable) return ControlMatch(resource);

		InitStats._Hits.Current -= 1;
		CheckStats();
		PlayAudio("match");
		if(Stats._Hits.Current <= 0)
		{
			isMatching = true;
			Stats.Value *=  resource;
			CollectThyself(true);
			TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;

			return true;			
		}
		else 
		{
			isMatching = false;

			CollectThyself(false);
			EffectManager.instance.PlayEffect(_Transform,Effect.Attack);
		}
		return false;
	}

	public virtual IEnumerator TakeTurnDamage()
	{
		CheckStats();
		int fullhit = 0;

		fullhit += Stats.TurnDamage;

		InitStats.TurnDamage = 0;
		InitStats._Hits.Current -= fullhit;
		CheckStats();

		yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		Player.instance.OnTileMatch(this);
		if(Stats._Hits.Current <= 0)
		{
			isMatching = true;
			//Player.Stats.PrevTurnKills ++;	
			//Stats.Value *= resource;
					
			CollectThyself(true);

			PlayAudio("death");
		}
		else 
		{
			//CollectThyself(false);
			isMatching = false;
		}
	}

	public virtual bool ControlMatch(int resource)
	{
		return false;
	}

	public virtual bool Tap()
	{
		return false;
	}

	public virtual void OnLand()
	{
		if(isFalling) 
		{
			land_time = 0.0F;			
		}
		
		if(!isFalling && land_time < land_time_end)
		{
			if(Genus != GENUS.OMG) 
			{
				Params._render.transform.localPosition += Juice._Squish.Position.Evaluate(land_time/land_time_end) * 0.7F;
				Vector3 finalscale = Params._render.transform.localScale;
				finalscale = Juice.instance.JuiceItNow(Juice._Squish.Scale, Params._render_defaultscale, land_time/land_time_end);
				finalscale.x = Mathf.Lerp(Params._render.transform.localScale.x, Params._render_defaultscale.x, Time.deltaTime *15);
				Params._render.transform.localScale = finalscale;
			}
			land_time += Time.deltaTime;
			if(land_time >= land_time_end) 
			{
				Params._render.transform.localPosition = Params._render_defaultpos;
				Params._render.transform.localScale = Params._render_defaultscale;
			}
		}
		else if(!isFalling) 
		{
			_collider.enabled = true;
			return;
		}

		_collider.enabled = true;
		isFalling = false;
		if(!IsState(TileState.Selected) && !IsState(TileState.Locked))  {
			SetState(TileState.Idle);
		}
	}

	public List<Action> DestroyActions = new List<Action>();
	public void AddAction(Action a)
	{
		DestroyActions.Add(a);
	}

	public void ClearActions()
	{
		DestroyActions.Clear();
	}

	public void ClearEffects()
	{
		for(int i = 0; i < Effects.Count; i++)
		{
			Destroy(Effects[i].gameObject);
		}
		Effects = new List<TileEffect>();
	}

	public virtual void CollectThyself(bool destroy)
	{
		if(this == null || !this.gameObject.activeSelf || this.Destroyed) return;

		if(destroy)
		{
			TileMaster.instance.AddVelocityToColumn(Point.Base[0], Point.Base[1], 0.2F + Stats.Value * 0.5F);
			Destroyed = true;
			for(int i = 0; i < DestroyActions.Count; i++)
			{
				DestroyActions[i]();
			}
			GetComponent<SphereCollider>().enabled = false;
		}
		TileMaster.instance.CollectTile(this, destroy);

		/*if(destroy)
		{
			TileMaster.
			DestroyThyself(false);
		}
		else
		{

			TileMaster.instance.CollectTile(this, destroy);
		}*/

		
	}

	public virtual void DestroyThyself(bool collapse = false)
	{
		if(this == null || !this.gameObject.activeSelf || this.Destroyed) return;

		TileMaster.instance.AddVelocityToColumn(Point.Base[0], Point.Base[1], 0.2F + Stats.Value * 0.5F);
		Destroyed = true;
		for(int i = 0; i < DestroyActions.Count; i++)
		{
			DestroyActions[i]();
		}
		GetComponent<SphereCollider>().enabled = false;
		StartCoroutine(Destroy_Thyself(collapse));
	}


	private IEnumerator Destroy_Thyself(bool collapse)
	{
		if(collapse)
		{
			bool dest = true;
			float gravity = 0.03F;
			float vel = -0.2F;
			float life = 0.6F;
	
			float sidevel = UnityEngine.Random.value > 0.5F ? UnityEngine.Random.value * 0.1F : -UnityEngine.Random.value * 0.1F;
			
			TileMaster.Grid[Point.Base[0], Point.Base[1]]._Tile = null;
			while(dest)
			{
				transform.position += (Vector3.left * sidevel) + Vector3.down * vel;
				vel += gravity;
		
				if(life < 0.0F) dest = false;
				else life-= Time.deltaTime;
				yield return null;
			}
		}
		TileMaster.instance.DestroyTile(this);
		yield return null;
	}

	public virtual void AddMana(GENUS g, int m)
	{
		
	}

	public virtual void AddHealth(GENUS g, int h)
	{
		if(Stats._Hits.Current < Stats.HitsMax)
		{
			InitStats._Hits.Current += h;
			CheckStats();
			Stats._Hits.Current = Mathf.Clamp(Stats._Hits.Current, 0, Stats.HitsMax);
		}
	}

	public virtual IEnumerator Animate(string type, float time = 0.0F)
	{
		if(_anim == null) yield break;
		_anim.SetTrigger(type);

		if(time != 0.0F) yield return new WaitForSeconds(time);
		else yield return null;
	}

	public virtual bool CanAttack() {return !AttackedThisTurn && Stats.Attack > 0;}
	public virtual bool CanBeAttacked() {return Genus != GENUS.OMG;}
	public virtual int GetAttack() {return Mathf.Max(Stats.Attack, 0);}
	public virtual int GetHealth() {return Mathf.Max(Stats._Hits.Current,0);}
	public virtual void Stun(int Stun){}

	public virtual void OnAttack(){}

	public void SetArrow(Tile nextTile = null, Tile prevTile = null, bool Active = true)
	{
		return;
		Params.lineOut.enabled = (Active && nextTile != null);
		Params.lineIn.enabled = (Active && nextTile != null);
		if(!Active)
		{
			return;
		}
		if(nextTile != null)
		{
			float dist = Vector3.Distance(nextTile.transform.position, transform.position);
			Vector3 vel = nextTile.transform.position - transform.position;

			Vector3 [] points = LightningLine(_Transform.position, nextTile.transform.position, 4, 0.1F);

			for(int i = 0; i < points.Length; i++)
			{
				Params.lineIn.SetPosition(i, points[i]);
				Params.lineOut.SetPosition(i, points[i] + Vector3.back);
			}
			//Params.lineIn.SetPosition(0, _Transform.position);
			//Params.lineIn.SetPosition(1, nextTile.transform.position);
			Params.lineIn.SetColors(GameData.instance.GetGENUSColour(Genus), GameData.instance.GetGENUSColour(nextTile.Genus));

			Params.lineOut.SetColors(Color.white, Color.white);
			//Params.lineOut.SetPosition(0, _Transform.position + Vector3.back);
			//Params.lineOut.SetPosition(1, nextTile.transform.position + Vector3.back);
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

	public virtual bool IsGenus(Tile t)
	{
		if(t == null) return false;
		if(Genus == GENUS.NONE || Genus == GENUS.OMG) return false;
		return Genus == t.Genus || Genus == GENUS.ALL || t.Genus == GENUS.ALL;
	}

	public virtual bool IsGenus(GENUS g, bool collect_all = true, bool allow_alpha = true)
	{
		if(Genus == GENUS.NONE || (Genus == GENUS.OMG && g != GENUS.OMG)) return false;
		return Genus == g || (collect_all && Genus == GENUS.ALL) || (allow_alpha && g == GENUS.ALL);
	}

	public bool IsType(Tile t)
	{
		return IsType(t.Info);
	}

	public bool IsType(TileInfo t)
	{
		return Genus == t._GenusEnum && Info._Type == t._Type;
	}

	public bool IsType(SPECIES s)
	{
		if(s == null || s == SPECIES.None) return true;
		return Info._Type == s;
	}

	public bool IsType(params string [] s)
	{
		bool genus = true;
		string asgenus = s[0].ToLower();
		GENUS g = TileMaster.Genus.GenusOf(asgenus);

		if(g != GENUS.NONE && Genus != g) genus = false;

		for(int i = 0; i < s.Length; i++)
		{
			if(Type.IsType(s[i])) return genus;
		}
		return false;
	}

	public bool IsType(string g, string s)
	{
		bool genus = false;
		
		if(g == string.Empty) genus = true;
		if(g == "Red" && Genus == GENUS.STR) genus = true;
		if(g == "Blue" && Genus == GENUS.DEX) genus = true;
		if(g == "Green" && Genus == GENUS.WIS) genus = true;
		if(g == "Yellow" && Genus == GENUS.CHA) genus = true;
		if(g == "Purple" && Genus == GENUS.PRP) genus = true;
		if((g == "All" || g == "Prism") && Genus == GENUS.ALL) genus = true;
		if(g == "Gray" && Genus == GENUS.OMG) genus = true;
		if(s == "Enemy" && Stats.isAlly) return false;
		return genus && IsType(s);
	}

	public virtual void SetArgs(params string [] args)
	{
		
	}

	public virtual void Reset(bool idle = false)
	{
		if(Params.counter != null)
		{
			Params.counter.text = "";
			Params.counter.color = Color.white;	
			Params.otherWarning.text = "";
		}
		if(Info != null && Point != null) transform.name = Info.Name + " | " + Point.Base[0] + ":" + Point.Base[1];
		state_override = false;
		originalMatch = false;
		AttackedThisTurn = false;
		Flags.Clear();
		//attacking = false;
		if(idle) SetState(TileState.Idle);
	}

	//For enemies to show possible damage inflicted
	public virtual void SetDamageWarning(int amt)
	{
	}

	public virtual void SetOtherWarning(string text)
	{
		Params.otherWarning.text = text;
	}

	public void SetCounter(string text, Color? col = null)
	{
		Params.counter.color = col ?? Color.white;
		Params.counter.text = text;
	}

	public void SetCounterText(string text)
	{
		Params.counter.text = text;
	}

	public int PointToInt(int [] diff)
	{
		if(diff[1] == -1)
		{
			switch(diff[0])
			{
				case -1:
				return 3;
				case 0:
				return 2;
				case 1:
				return 1;
			}
		}
		else if(diff[1] == 1)
		{
			switch(diff[0])
			{
				case -1:
				return 5;
				case 0:
				return 6;
				case 1:
				return 7;
			}
		}
		else
		{
			if(diff[0] == -1) return 4;
			else if(diff[0] == 1) return 8;
		}
		return 0;
	}

	public void AttackPlayer()
	{
		PlayAudio("attack");
		int final_attack = GetAttack();

		float init_size = UnityEngine.Random.Range(160, 200);
		float init_rotation = UnityEngine.Random.Range(-7,7);

		float info_time = 0.43F;
		float info_size = init_size + (final_attack * 2);
		float info_movespeed = 25;
		float info_finalscale = 0.75F;

		Vector3 pos = TileMaster.Grid.GetPoint(Point.Point(0));
		MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + final_attack, info_size, Color.black, info_time, 0.03F, false);
		m.Txt[0].outlineColor = GameData.Colour(Genus);
		m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
		MoveToPoint mini = m.GetComponent<MoveToPoint>();
		//mini.enabled = true;
		m.AddJuice(Juice.instance.BounceB, info_time);
		m.AddAction(() => {mini.enabled = true;});
		m.DestroyOnEnd = false;

		mini.SetTarget(UIManager.instance.Health.transform.position);
		mini.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
		mini.SetMethod(() =>{
				Player.instance.OnHit(this);
				//PlayAudio("hit");
				GameData.Log("Took " + final_attack + "damage from " + this);
			}
		);
	}

	public void AttackWaveUnit(Wave w)
	{
		float init_size = UnityEngine.Random.Range(160,200);
		float init_rotation = UnityEngine.Random.Range(-7,7);

		float info_time = 0.43F;
		float info_size = init_size + (GetAttack() * 2);
		float info_movespeed = 25;
		float info_finalscale = 0.75F;

		Vector3 pos = TileMaster.Grid.GetPoint(Point.Point(0));
		MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + GetAttack(), info_size, GameData.instance.BadColour, info_time, 0.03F, false);
		m.Txt[0].outlineColor = GameData.Colour(Genus);
		m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
		MoveToPoint mini = m.GetComponent<MoveToPoint>();
		//mini.enabled = true;
		m.AddJuice(Juice.instance.BounceB, info_time);
		m.AddAction(() => {mini.enabled = true;});
		m.DestroyOnEnd = false;

		mini.SetTarget(UIManager.Objects.TopGear[1][0][0].transform.position);
		mini.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
		mini.SetMethod(() =>{
				w.AddPoints(GetAttack());
				//AudioManager.instance.PlayClipOn(Player.instance.transform, "Player", "Hit");
			}
		);
	}

	public void AttackTile(Tile t)
	{
		PlayAudio("attack");
		//UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + GetAttack(), 95, Color.red, 0.8F,0.08F);
		attacking = true;
		t.SetState(TileState.Selected);

		float init_size = UnityEngine.Random.Range(190,210);
		float init_rotation = UnityEngine.Random.Range(-7,7);

		float info_time = 0.43F;
		float info_size = init_size + (GetAttack() * 2);
		float info_movespeed = 25;
		float info_finalscale = 0.75F;

		Vector3 pos = TileMaster.Grid.GetPoint(Point.Point(0));

		Color inside = Stats.isEnemy ? Color.black : Color.white;
		MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + GetAttack(), info_size, inside, info_time, 0.03F, false);
		m.Txt[0].outlineColor = GameData.Colour(Genus);
		m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
		MoveToPoint mini = m.GetComponent<MoveToPoint>();
		//mini.enabled = true;
		m.AddJuice(Juice.instance.BounceB, info_time);
		m.AddAction(() => {mini.enabled = true;});
		m.DestroyOnEnd = false;

		mini.SetTarget(t.transform.position);
		mini.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
		mini.SetMethod(() =>{
				attacking = false;
				if(t == null) return;
				if(t != null)
				{
					t.InitStats.TurnDamage += GetAttack();
					t.PlayAudio("hit");
					EffectManager.instance.PlayEffect(t.transform,Effect.Attack);
					StartCoroutine(t.TakeTurnDamage());
					//UIManager.instance.CashMeterPoints();
					GameData.Log(this +  " dealt " + GetAttack() + "damage to " + t);
				} 
			}
		);
		mini.DontDestroy = false;
	}

	public virtual IEnumerator AttackRoutine()
	{
		List<Tile> targ = new List<Tile>();
		targ.AddRange(Point.GetNeighbours(false, "hero"));

		for(int i = 0; i < targ.Count; i++) 
		{
			if(!targ[i].CanBeAttacked()) targ.RemoveAt(i);
		}

		if(targ.Count > 0)
		{
			Tile targ_final = targ[UnityEngine.Random.Range(0, targ.Count)];
			SetState(TileState.Selected);
			OnAttack();
			yield return StartCoroutine(Animate("Attack", 0.05F));
			AttackTile(targ_final);
		}
		yield return null;
	}


	public virtual void SetValue(int val)
	{
		Stats.Value = val;
	}

	public virtual void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);

		int diff = (int) InitStats.value_soft - InitStats.Value;
		if(diff > 0)
		{
			StartCoroutine(ValueAlert(diff));
		}
		else
		{
			InitStats.Value = (int) InitStats.value_soft;
			CheckStats();
			SetSprite();
		}
	}

	protected virtual IEnumerator ValueAlert(int diff)
	{
		SetState(TileState.Selected, true);
		Animate("Alert");
		MiniAlertUI m = UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "+" + Stats.Value, 150, GameData.Colour(Genus), 0.4F,0.00F);
		m.AddJuice(Juice.instance.Ripple.Scale, 0.4F);
		yield return new WaitForSeconds(0.4F);

		InitStats.Value = (int) InitStats.value_soft;
		CheckStats();
		SetSprite();
		Reset(true);
		yield return null;
	}

	public void CheckStats()
	{
		//float ratio = (float)Stats.Hits / (float)Stats.HitsMax;
		Stats = new TileStat(InitStats);

		for(int i = 0; i < Effects.Count; i++)
		{
			if(Effects[i] == null) Effects.RemoveAt(i);
			else Stats.Add(Effects[i].CheckStats(), false);
		}

		Stats.CheckIncreases();

		//Stats.Hits = Mathf.Clamp((int)(Stats.HitsMax * ratio), 0, Stats.HitsMax);

	}

	public virtual void BeforeTurn()
	{

	}

	public virtual IEnumerator BeforeTurnRoutine(){

		yield break;
	}

	public bool HasAfterTurnEffect()
	{
		return AfterTurnEffect || Effects.Count > 0;
	}

	public virtual void AfterTurn()
	{
		Reset();
		
		InitStats.TurnDamage = 0;
		InitStats.Lifetime ++;
		if(InitStats.Lifetime >= 1) 
		{
			InitStats.isNew = false;
		}


		/*for(int i = 0; i < Effects.Count; i++)
		{
			if(Effects[i] == null) Effects.RemoveAt(i);
			else if(Effects[i].CheckDuration()) 
			{
				GameObject old = Effects[i].gameObject;
				Effects.RemoveAt(i);
				Destroy(old);
			}
		}*/
		CheckStats();
	}

	public virtual IEnumerator AfterTurnRoutine()
	{
		for(int i = 0; i < Effects.Count; i++)
		{
			bool destroy = false;
			if(Effects[i] == null) Effects.RemoveAt(i);
			else if(Effects[i].CheckDuration()) 
			{
				destroy = true;
			}
			if(Effects[i] != null)
			{
				yield return StartCoroutine(Effects[i].StatusEffectRoutine());
				if(i < Effects.Count-1)	yield return new WaitForSeconds(Time.deltaTime * 15);
			}
			if(destroy)
			{
				GameObject old = Effects[i].gameObject;
				Effects.RemoveAt(i);
				Destroy(old);
			}

		}
		
		yield break;
	}

	public virtual void SetSprite()
	{
		int sprite = Stats.Value / 5;
		SetBorder(Info.Outer);
		SetRender(Info._GenusName);
		
		//transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
		Params.transform.position = transform.position;
		Params._render.transform.localPosition = Vector3.zero;
	}

	public  void SetBorder(int border)
	{
		if(NewVisuals) Params._border.gameObject.SetActive(false);
		else if(Params._border != null) 
		{
			Params._border.gameObject.SetActive(true);
			Params._border.SetSprite(TileMaster.Genus.Frames, border);
		}
	}
	public  void SetRender(string render)
	{
		if(Params._render != null && Info.Inner != null) 
		{
			tk2dSpriteDefinition id = Info.Inner.GetSpriteDefinition(render);
			if(id == null) render = "Alpha";
			Params._render.SetSprite(Info.Inner, render);
			if(NewVisuals) Params._render.scale = new Vector3(0.8F, 0.8F, 1.0F);
		}
	}

	public virtual TileEffect AddEffect(TileEffect init)
	{
		foreach(TileEffect child in Effects)
		{
			if(child.Name == init.Name)
			{
				child.Duration += init.Duration;
				return child;
			}
		}

		TileEffect e = init;
		e.Setup(this);
		e.transform.position = _Transform.position;
		e.transform.parent = _Transform;
		Effects.Add(e);
		CheckStats();
		return e;
	}

	public virtual TileEffect AddEffect(string name, int duration, params string [] args)
	{
		foreach(TileEffect child in Effects)
		{
			if(child.Name == name)
			{
				child.Duration += duration;
				return child;
			}
		}
		TileEffect e = (Status) (Instantiate(GameData.instance.GetTileEffectByName(name))) as TileEffect;
		e.GetArgs(duration, args);
		e.Setup(this);
		e.transform.position = _Transform.position;
		e.transform.parent = _Transform;
		Effects.Add(e);
		CheckStats();
		return e;
	}

	public virtual void RemoveEffect(string name)
	{
		for(int i = 0; i < Effects.Count; i++)
		{
			if(Effects[i].Name == name)
			{
				TileEffect e = Effects[i];
				Effects.RemoveAt(i);

				e._OnDestroy();
				Destroy(e.gameObject);
			}
		}
	}

	public virtual TileEffect AddEffect(TileEffectInfo inf)
	{
		return AddEffect(inf.Name, inf.Duration, inf.Args);
	}

	public bool HasEffect(string name)
	{
		foreach(TileEffect child in Effects)
		{
			if(child.Name == name)return true;
		}
		return false;
	}

	
	public void ChangeGenus(GENUS g)
	{
		EffectManager.instance.PlayEffect(transform, Effect.Replace, GameData.instance.GetGENUSColour(g));
		PlayAudio("replace", 0.6F);
		Info.ChangeGenus(g);
		SetSprite();
	}



	private MoveToPoint _movecomp;
	public MoveToPoint MoveComp
	{
		get{
			if(_movecomp == null) _movecomp = this.gameObject.AddComponent<MoveToPoint>();
			return _movecomp;
		}
	}
	public IEnumerator MoveToTile(Tile t, bool overtake, float speed = 15.0F, float arc = 0.0F)
	{
		bool complete = false;
		Vector3 newpoint = t.Point.targetPos;
		UnlockedFromGrid = true;

		MoveComp.Clear();
		MoveComp.SetTarget(newpoint);
		MoveComp.SetPath(speed, arc);
		MoveComp.SetThreshold(0.1F);
		MoveComp.DontDestroy = true;
		MoveComp.NoDestroy = true;

		MoveComp.SetMethod(() => {
			complete = true;
			
			if(overtake)
			{
				UnlockedFromGrid = false;
				TileMaster.Grid[x,y]._Tile = null;
				TileMaster.Grid[t.x, t.y]._Tile = this;
				Setup(t.Point.Grid, t.x, t.y);
				transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
				SetSprite();
			}
			
		});

		while(!complete) yield return null;
	}

	public virtual string InnerRender(){return Info._GenusName;}
	public virtual tk2dSpriteCollectionData InnerAtlas(){return Info.Inner;}

	public IEnumerator MoveToPoint(int _x, int _y, bool overtake, float speed = 6.0F, float arc = 0.0F)
	{
		//print("MOVING" + TileMaster.Grid[_x,_y]._Tile + ":" + this);
		bool complete = false;
		Vector3 newpoint = TileMaster.Grid[_x,_y].Pos;
		UnlockedFromGrid = true;

		MoveComp.Clear();
		MoveComp.SetTarget(newpoint);
		MoveComp.SetPath(speed, arc);
		MoveComp.SetThreshold(0.1F);
		MoveComp.DontDestroy = true;
		MoveComp.NoDestroy = true;


		MoveComp.SetMethod(() => {
			complete = true;
			
			if(overtake)
			{
				UnlockedFromGrid = false;
				TileMaster.Grid[x,y]._Tile = null;
				TileMaster.Grid[_x, _y]._Tile = this;
				Setup(TileMaster.Grid, _x, _y);
				transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
				SetSprite();
			}
			
		});

		while(!complete) 
		{
			yield return null;
		}
		
	}

	public void MoveToGridPoint(int x, int y, float arc = 0.0F)
	{
		Vector3 newpoint = TileMaster.Grid[x,y].Pos;
		UnlockedFromGrid = true;

		MoveComp.Clear();
		MoveComp.SetTarget(newpoint);
		MoveComp.SetPath(6.0F, arc);
		MoveComp.SetThreshold(0.1F);
		MoveComp.DontDestroy = true;
		MoveComp.NoDestroy = true;

		MoveComp.SetIntMethod((int [] num) => {
			TileMaster.Grid[num[0], num[1]]._Tile = this;
			Setup(TileMaster.Grid, num[0], num[1]);

			transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
			SetSprite();
			UnlockedFromGrid = false;
			
		}, x,y);
		
	}

	public bool Isolated
	{
		get
		{
			Tile [] nbours = Point.GetNeighbours(true);
			for(int i = 0; i < nbours.Length; i++)
			{
				if(IsGenus(nbours[i].Genus, false)) return false;
			}
			return true;
		}
	}

}


public enum TileState{
	Idle,
	Hover,
	Selected,
	Locked,
	Falling
}

public enum Team
{
	None, Enemy, Ally
}

[System.Serializable]
public class StatCon
{
	public int Current;
	public float Current_Soft;
	public int Min;
	public int Max;
	private float Max_Soft;

	public float ThisTurn;

	public float Gain;
	public float Leech;
	public float Regen;

	public float Multiplier = 0.0F;

	public string String
	{
		get
		{
			return GameData.PowerString(Current);	
		}
	}

	public float Ratio
	{
		get{return Current_Soft/Max_Soft;}
	}


	public StatCon(StatCon prev = null)
	{
		ThisTurn = 0;
		if(prev != null)
		{
			Current = prev.Current;
			Current_Soft = prev.Current_Soft;
			Min = prev.Min;
			Max = prev.Max;
			Max_Soft = prev.Max_Soft;

			Gain 	= prev.Gain;
			Leech = prev.Leech;
			Regen = prev.Regen;

			Multiplier = prev.Multiplier;

			Level_Required = prev.Level_Required;
			Level_Current = prev.Level_Current;
			Level_Multiplier = prev.Level_Multiplier;
			Level_Increase = prev.Level_Increase;
		}
	}

	public void Add(StatCon prev)
	{
		if(prev == null) return;
		Current += prev.Current;
		Current_Soft += prev.Current_Soft;
		Min += prev.Min;
		Max += prev.Max;
		Max_Soft += prev.Max_Soft;

		Gain 	+= prev.Gain;
		Leech += prev.Leech;
		Regen += prev.Regen;

		Multiplier += prev.Multiplier;
		ThisTurn += prev.ThisTurn;

		Level_Required += prev.Level_Required;
		Level_Current += prev.Level_Current;
		Level_Multiplier += prev.Level_Multiplier;
		Level_Increase += prev.Level_Increase;
	}

	public void Add(StatContainer prev)
	{
		Current += prev.StatCurrent;
		Current_Soft = (float)Current;
		//Min += prev.Min;
		
		Max_Soft += prev.StatCurrent;
		Max = (int)Max_Soft;

		Gain 	+= prev.StatGain;
		Leech += prev.StatLeech;
		Regen += prev.StatRegen;

		Multiplier += prev.ResMultiplier;
		ThisTurn += prev.ThisTurn;
	}

	public void Reset()
	{
		Current = 0;
		ThisTurn = 0;
		Multiplier = 1.0F;
		Min = 0;
		Max = 10;
		Max_Soft = 10.0F;
		Gain = 0;
		Leech = 0;
		Regen = 0;
	}

	public void Complete()
	{
		ThisTurn += Regen;

		Max_Soft += Gain;
		Max = (int) Max_Soft;

		int Max_actual = Max == 0 ? 99999 : Max;
		Current_Soft = Mathf.Clamp(Current_Soft + ThisTurn * (1.0F + Multiplier),
									Min, Max_actual);

		Current = (int)Current_Soft;

		ThisTurn = 0;
	}

	public int Level_Current = 0, Level_Required = 0;
	public float Level_Multiplier = 0.0F;
	public float Level_Increase = 0.0F;
	public int Level(int input)
	{
		int total = 0;
		Level_Current += input;
		if(Level_Required == 0) 
		{
			Level_Required = 10;
			Level_Increase = 1.0F;
			Level_Multiplier = 0.09F;
		}
		while(Level_Current >= Level_Required)
		{
			Level_Current = (int)Mathf.Clamp(Level_Current - Level_Required,
										0, Mathf.Infinity);
			Level_Required = (int)(Level_Required * (1.0F + Level_Multiplier));
			Debug.Log(Level_Increase);
			Max_Soft += Level_Increase;
			Max = (int) Max_Soft;
			Current_Soft += Level_Increase;
			Current = (int) Current_Soft;
			total ++;
		}
		return total;
	}

	public void Add(int num)
	{
		ThisTurn += num;
	}

	public void Set(int num)
	{
		Max_Soft = num;
		Max = (int)Max_Soft;
		Current = num;
	}

	public void Increase(int num)
	{
		if(num == 0) return;
		float ratio = (float) Current/(float) Max;
		Set(Max + num);
		Current = (int)(Max * ratio);
	}
}

[System.Serializable]
public class TileStat
{
	public string Name;
	public int Value        = 0;
	[HideInInspector]
	public float value_soft = 0;

	public int Resource    = 0;
	public int Heal        = 0;
	public int Armour      = 0;
	
	public int Hits
	{
		get{return _Hits.Current;}
		set{_Hits.Current = value;}
	}

	public int HitsMax
	{
		get{return _Hits.Max;}
		set{_Hits.Max = value;}
	}
	
	public int Attack
	{
		get{return _Attack.Current;}
		set{_Attack.Current = value;}
	}

	public int Spell
	{
		get{return _Spell.Current;}
		set{_Spell.Current = value;}
	}

	public int Movement
	{
		get{return _Movement.Current;}
		set{_Movement.Current = value;}
	}

	public int Lifetime
	{
		get{return _Lifetime.Current;}
		set{_Lifetime.Current = value;}
	}

	public int Deathtime
	{
		get{return _Lifetime.Max;}
		set{_Lifetime.Max = value;}
	}

	public StatCon _Lifetime;
	
	public int AttackPower = 0;
	public int SpellPower  = 0;
	public int TurnDamage = 0;
	
	public bool isNew       = true;
	public bool isAlerted   = false;

	public StatCon _Hits;
	public StatCon 	_Movement,
					_Attack,
					_Spell;
					
	public StatCon 	 _Strength,
					 _Dexterity,
					 _Charisma,
					 _Wisdom,
					 _Luck,
					 _Chaos;

	public int Length
	{
		get{
			return 6;
		}
	}
	public StatCon this[int value]
	{
		get{
			switch(value)
			{
				case 0:
				return _Strength;
				case 1:
				return _Dexterity;
				case 2:
				return _Wisdom;
				case 3:
				return _Charisma;
				case 4:
				return _Luck;
				case 5:
				return _Chaos;
			}
			return null;
		}

	}




	public Team _Team = Team.None;
	
	public bool isAlly
	{
		get{return _Team == Team.Ally;}
	}
	public bool isEnemy{get{return _Team == Team.Enemy;}}

	public ShiftType Shift;

	public void Add(TileStat t, bool _override = true)
	{
		if(t == null) return;
		Value     += t.Value;

		Resource    += t.Resource;
		Heal        += t.Heal;
		Armour      += t.Armour;
		
		_Hits.Add(t._Hits);
		_Attack.Add(t._Attack);
		_Spell.Add(t._Spell);
		_Movement.Add(t._Movement);

		_Lifetime.Add(t._Lifetime);
		
		AttackPower += t.AttackPower;
		SpellPower  += t.SpellPower;

		TurnDamage  += t.TurnDamage;

		if(_override)
		{
			//isFrozen = t.isFrozen;
			//isBroken = t.isBroken;
			isAlerted = t.isAlerted;
			_Team = t._Team;
		
			isNew = t.isNew;
		}
		else
		{
			//if(t.isFrozen) isFrozen    = true;
			//if(t.isBroken) isBroken    = true;
			if(t._Team != Team.None) _Team = t._Team;
			//if(!t.isAlerted) isAlerted = false;
		}
		
	}

	public TileStat(TileStat t = null)
	{
		if(t != null)
		{
			Value     = t.Value;

			_Hits      = new StatCon(t._Hits);

			Resource  = t.Resource;
			Heal 	  = t.Heal;
			Armour 	  = t.Armour;
			
			_Attack    = new StatCon(t._Attack);
			_Spell     = new StatCon(t._Spell);
			_Movement  = new StatCon(t._Movement);
			_Lifetime = new StatCon(t._Lifetime);

			_Dexterity = new StatCon(t._Dexterity);
			_Strength = new StatCon(t._Strength);
			_Charisma = new StatCon(t._Charisma);
			_Wisdom = new StatCon(t._Wisdom);
			_Luck = new StatCon(t._Luck);
			_Chaos = new StatCon(t._Chaos);

			AttackPower = t.AttackPower;
			SpellPower  = t.SpellPower;
			
			
			TurnDamage = t.TurnDamage;
			//isFrozen = t.isFrozen;
			//isBroken = t.isBroken;
			isAlerted = t.isAlerted;
			_Team = t._Team;
		
			isNew = t.isNew;
			Shift = t.Shift;
		}
	}

	public void CheckIncreases()
	{
		_Attack.Increase(_Dexterity.Current/3);
		_Spell.Increase(_Wisdom.Current/3);
		_Movement.Increase(_Charisma.Current/3);
		_Hits.Increase(_Strength.Current*2);
	}

	public int [] GetValues()
	{
		return new int [] {(int) (Resource * Value), (int)(Heal + (Heal > 0 ? (Value*2):0)), (int)(Armour + (Armour > 0 ? (Value*2):0))};
	}

}

[System.Serializable]
public class TilePointContainer
{
	public int BaseX, BaseY;
	public List<int> AllX, AllY;
	public int Scale;
	public GridInfo Grid;

	public Vector3 targetPos;
	public Tile parent;
	public int TopX, TopY;

	public int [] Point(int x)
	{
		return new int[]{AllX[x], AllY[x]};
	}

	public int [] Base{
		get{return new int [] {BaseX, BaseY};}
		set{BaseX = value[0]; 
			BaseY = value[1];
			SetPoints();}
	}

	public int [] Top
	{
		get{return new int [] {TopX, TopY};}
	}

	public int Length{get{return AllX.Count;}}

	public TilePointContainer(GridInfo g, int x, int y, int sc, Tile p)
	{
		Grid = g;
		BaseX = x;
		BaseY = y;
		Scale = sc;
		parent = p;
		SetPoints();
	}

	public void SetPoints()
	{
		AllX = new List<int>();
		AllY = new List<int>();
		targetPos = Vector3.zero;
		if(Grid == null)
		{
			return;
		}

		TopX = BaseX + (Scale - 1);
		TopY = BaseY + (Scale - 1);
		
		for(int xx = 0; xx < Scale; xx++)
		{
			for(int yy = 0; yy < Scale; yy++)
			{
				if(Grid.Size[0] <= BaseX + xx || Grid.Size[1] <= BaseY + yy) continue;
				AllX.Add(BaseX + xx);
				AllY.Add(BaseY + yy);
				Grid[BaseX+xx, BaseY+yy]._Tile = parent;
			}
			
		}
		int end = AllX.Count - 1;

		Vector3 bottomleft = Grid.GetPoint(BaseX, BaseY);
		Vector3 topright = Grid.GetPoint(TopX, TopY);
		if(topright == Vector3.zero) topright = bottomleft;

		//Debug.Log(BaseY + ":" + AllY[end] + ":" + TileMaster.Grid.Size[1]);
		targetPos = Vector3.Lerp(bottomleft, topright, 0.5F);

	}

	public bool Contains(int x, int y, bool mutual = true)
	{
		for(int xx = 0; xx <AllX.Count; xx++)
		{
			for(int yy = 0; yy < AllY.Count; yy++)
			{
				if(AllX[xx] == x && AllY[yy] == y) return true;
				if(!mutual)
				{
					if(AllX[xx] == x || AllY[yy] == y) return true;
				}
			}
		}
		return false;
	}

	public int [] Closest(int x, int y, out int shortest)
	{
		shortest = 100;
		int [] point = new int[2];
		for(int xx = 0; xx < AllX.Count; xx++)
		{
			for(int yy = 0; yy < AllY.Count; yy++)
			{
				int dist = Mathf.Abs(AllX[xx]-x) + Mathf.Abs(AllY[yy]-y);
				if(dist < shortest) 
				{
					point = new int [] {xx,yy};
					shortest = dist;
				}
			}
		}
		return point;
	}

	public int [] Closest(int [] num, out int shortest)
	{
		int x = num[0], y = num[1];
		shortest = 100;
		int [] point = new int[2];
		for(int xx = 0; xx < AllX.Count; xx++)
		{
			for(int yy = 0; yy < AllY.Count; yy++)
			{
				int dist = Mathf.Abs(AllX[xx]-x) + Mathf.Abs(AllY[yy]-y);
				if(dist < shortest) 
				{
					point = new int [] {AllX[xx],AllY[yy]};
					shortest = dist;
				}
			}
		}
		return point;
	}

	public Tile [] GetNeighbours(bool diags = false, string type = "")
	{
		List<Tile> final = new List<Tile>();
		for(int xx = 0; xx < AllX.Count; xx++)
		{
			for(int yy = 0; yy < AllY.Count; yy++)
			{
				Tile a = TileMaster.instance.GetTile(AllX[xx]-1, AllY[yy]);
				Tile b = TileMaster.instance.GetTile(AllX[xx]+1, AllY[yy]);
				Tile c = TileMaster.instance.GetTile(AllX[xx], AllY[yy]-1);
				Tile d = TileMaster.instance.GetTile(AllX[xx], AllY[yy]+1);
				
				if(a != null && !final.Contains(a) && a != parent && a.IsType(type)) final.Add(a);
				if(b != null && !final.Contains(b) && b != parent && b.IsType(type)) final.Add(b);
				if(c != null && !final.Contains(c) && c != parent && c.IsType(type)) final.Add(c);
				if(d != null && !final.Contains(d) && d != parent && d.IsType(type)) final.Add(d);

				if(diags)
				{
					Tile e = TileMaster.instance.GetTile(AllX[xx]+1, AllY[yy]+1);
					Tile f = TileMaster.instance.GetTile(AllX[xx]-1, AllY[yy]-1);
					if(e != null && !final.Contains(e) && e != parent && e.IsType(type)) final.Add(e);
					if(f != null && !final.Contains(f) && f != parent && f.IsType(type)) final.Add(f);

					Tile g = TileMaster.instance.GetTile(AllX[xx]+1, AllY[yy]-1);
					Tile h = TileMaster.instance.GetTile(AllX[xx]-1, AllY[yy]+1);
					if(g != null && !final.Contains(g) && g != parent && g.IsType(type)) final.Add(g);
					if(h != null && !final.Contains(h) && h != parent && h.IsType(type)) final.Add(h);
				}
				
			}
		}
		return final.ToArray();
	}

	public Tile GetNeighbour(int x, int y)
	{
		int dist = 0;
		int x_final = x * Scale;
		int y_final = y * Scale;
		int [] point = new int [] {BaseX+x_final, BaseY+y_final};

		if(point[0] < 0 || point[0] >= TileMaster.Grid.Size[0] ||
			point[1] < 0 || point[1] >= TileMaster.Grid.Size[1])
			return null;

		Tile targ = TileMaster.Tiles[point[0], point[1]];
		if(targ == parent)
		{
			int [] closest = Closest(point[0], point[1], out dist);

			return  TileMaster.Tiles[closest[0] + x, closest[1]+y];
		}
		else return targ;
	}



}


public class TileUpgrade
{
	public float Chance;
	public int Points;
	public Action Method;

	public TileUpgrade(float c, int p, Action m)
	{
		Chance = c;
		Points = p;
		Method = m;
	}
}