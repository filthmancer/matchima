﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;


public class Tile : MonoBehaviour {

	public string Name;
	public virtual StCon _Name {
		get{return new StCon(Info._TypeName, GameData.Colour(Genus));}
	}
	public int x{get{return Point.Base[0];}}
	public int y{get{return Point.Base[1];}}

	public StCon [] BaseDescription
	{
		get{
			List<StCon> basic = new List<StCon>();
			if(Stats.Resource != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[0] + " "  + GameData.ResourceLong(Genus) + " Mana", GameData.Colour(Genus)));
			if(Stats.Heal != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[1] + "% Health", GameData.Colour(GENUS.STR)));
			if(Stats.Armour != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[2] + " Armour", GameData.Colour(GENUS.DEX)));
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
			if(Genus == GENUS.OMG) final.Add(new StCon("Cannot be matched", Color.grey));
			final.AddRange(BaseDescription);
			if(Description != null)	final.AddRange(Description);
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
	public SPECIES Type{
		get{return Info._Type;}
	}
	[HideInInspector]
	public TileInfo Info;

	public Sprite Inner
	{
		get{return Params._render.sprite;}
	}
	public Sprite Outer
	{
		get{return Params._border.sprite;}
	}
	public TileState state;
	[HideInInspector]
	public TilePointContainer Point;

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
				new TileUpgrade(0.05F, 5, () => {InitStats.Hits += 1;}),
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

       
	}

	public void Setup(int x, int y)
	{
		//Setup(x,y, _Scale, Info, InitStats.Value-1);

		Point = new TilePointContainer(x,y,_Scale, this);

		if(!Info.ShiftOverride) InitStats.Shift = Player.Stats.Shift;
		else InitStats.Shift = Info.Shift;
		transform.name = Info.Name + " | " + Point.Base[0] + ":" + Point.Base[1];
		CheckStats();
	}

	public virtual void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		Point = new TilePointContainer(x,y,scale, this);

		Info = new TileInfo(inf);

		if(Params != null)
		{
			def = Params._render.material.color;
			targetColor = def;
			//_anim = GetComponent<Animator>();
			//Params.lineIn.sortingLayerID = Params._render.sortingLayerID;
			//Params.lineOut.sortingLayerID = Params._render.sortingLayerID;
			//Params.lineIn.sortingOrder = 1;
			//Params.lineOut.sortingOrder = 1;
			//Params.lineIn.SetWidth(0.3F, 0.3F);
			//Params.lineOut.SetWidth(0.1F, 0.1F);
		}
		

		UnlockedFromGrid = false;
		isMatching = false;
		Destroyed = false;
		marked = false;
		originalMatch = false;
		state_override = false;
		GetComponent<SphereCollider>().enabled = true;
		for(int i = 0; i < Effects.Count; i++)
		{
			TileEffect t = Effects[i];
			Effects.RemoveAt(i);
			Destroy(t.gameObject);
		}

		defaultScale = 1.1F * scale;
		targetScale = defaultScale;
		_Scale = scale;

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
		if(InitStats.Hits == 0) InitStats.Hits = 1;
		AddUpgrades(val);
		
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
		SetSprite();

		if(GameManager.instance.EnemyTurn) SetState(TileState.Locked);
		else SetState(TileState.Idle);
	}


	// Update is called once per frame
	public virtual void Update () {
		if(Destroyed || UnlockedFromGrid) return;
		if(Stats.Shift != ShiftType.None && !Destroyed)
		{
			Velocity();
		}
		transform.name = Info.Name + " | " + Point.Base[0] + ":" + Point.Base[1];
		if(GameManager.inStartMenu) 
		{
			if(Player.loaded && UIManager.loaded) Destroy(this.gameObject); 
			if(transform.position.y < -10) Destroy(this.gameObject);
			return;
		}
		if(TileMaster.Tiles.GetLength(0) <= Point.Base[0] || TileMaster.Tiles.GetLength(1) <= Point.Base[1]) return;
		if(TileMaster.Tiles[Point.Base[0], Point.Base[1]] != this && TileMaster.Tiles[Point.Base[0], Point.Base[1]] != null && !Destroyed) 
		{
			//TileMaster.instance.SetFillGrid(true);
			//DestroyThyself();
		}

		if(GameManager.instance.EnemyTurn && !IsState(TileState.Selected)) SetState(TileState.Locked);
		if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, targetColor, 0.6F);
		if(Params._border != null) Params._border.color = Color.Lerp(Params._border.color, targetColor, 0.6F);
		if(Params.HitCounter != null) Params.HitCounter.SetActive(Stats.Hits > 1);
		if(Params.HitCounterText != null) Params.HitCounterText.text = "" + Stats.Hits;
			
		if(!isMatching) transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5);
		Params._shiny.enabled = IsState(TileState.Selected);

		
		if(!IsState(TileState.Selected))
		{
			//Params.lineIn.enabled = false;
			//Params.lineOut.enabled = false;	
			linepos = PlayerControl.InputPos;
			if(Player.Options.ViewTileStats) SetCounter("" + Stats.Value);
			else if(!GameManager.instance.EnemyTurn) SetCounter("");

			if(GameManager.instance.EnemyTurn) return;

			if(PlayerControl.instance.focusTile == this && !PlayerControl.HoldingSlot)
			{
				if(IsGenus(PlayerControl.matchingTile) || PlayerControl.matchingTile == null)
				{
					if(Application.isEditor)
					{
						SetState(TileState.Hover);	
					} 

					if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) 
					{
						Tile lastselected = PlayerControl.instance.LastSelected();
						int [] diff;
						if(lastselected!= null && lastselected != this)
						{
							lastselected.SetArrow(this);
						}
						if(lastselected == null || TileMaster.instance.AreNeighbours(lastselected, this, out diff))
						{	
							SetState(TileState.Selected);
							PlayerControl.instance.GetSelectedTile(this);
							AudioManager.instance.PlayClipOn(this.transform, "Player", "Touch");
						}
						else SetState(TileState.Idle);	
					}		
				}
				else SetState(TileState.Locked);
			}
			else if(PlayerControl.matchingTile != null && !IsGenus(PlayerControl.matchingTile))
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
			else  
			{
				SetState(TileState.Idle);
			}
		}	
		else if(IsState(TileState.Selected))
		{
			if(PlayerControl.instance.focusTile == this && PlayerControl.instance.SecondLastSelected() == this)	
			{
				PlayerControl.instance.BackTo(this);
				//Params.lineIn.enabled = false;
				//Params.lineOut.enabled = false;			
			}
			else if(PlayerControl.instance.SecondLastSelected() == this && PlayerControl.instance.LastSelected() != null)
			{
				LineTarget = PlayerControl.instance.LastSelected();
			}
			if(PlayerControl.instance.LastSelected() == this && PlayerControl.instance.focusTile != null && !GameManager.instance.EnemyTurn && !UIManager.InMenu)
			{
				float softdist = 1.4F * Point.Scale;
				float stretch = 1.0F;
				float str_decay = 1.0F;

				float dist = Vector3.Distance(PlayerControl.InputPos, transform.position);
				Vector3 vel = PlayerControl.InputPos - transform.position;
				Vector3 final = transform.position + vel.normalized * softdist;

				if(dist > softdist)
				{
					stretch = dist/softdist - (dist-softdist)/(dist-softdist);
					//Params._render.transform.position = Vector3.Lerp(Point.targetPos, final, 0.02F);
					linepos = final;
				}
				else
				{
					linepos = PlayerControl.InputPos;
					//Params._render.transform.position = Vector3.Lerp(Point.targetPos, transform.position + vel, 0.02F);
				}
				/*Vector3 [] points = LightningLine(this.transform.position, linepos, 5, 0.01F + PlayerControl.MatchCount * 0.005F);
				for(int i = 0; i < points.Length; i++)
				{
					Params.lineIn.SetPosition(i, points[i]);
					Params.lineOut.SetPosition(i, points[i] + Vector3.back);
				}

				Params.lineIn.enabled = true;
				Params.lineIn.SetColors(GameData.instance.GetGENUSColour(Genus) * 0.9F, GameData.instance.GetGENUSColour(Genus) * 0.9F);

				Params.lineOut.enabled = true;
				Params.lineOut.SetColors(Color.white, Color.white);
				*/
				
			}
			else if(GameManager.instance.EnemyTurn || UIManager.InMenu)
			{
				//Params.lineIn.enabled = false;
				//Params.lineOut.enabled = false;
			}
			else 
			{
				if(LineTarget == null) 
				{
					//Params.lineIn.enabled = false;
					//Params.lineOut.enabled = false;
					return;
				}

				/*Vector3 [] points = LightningLine(this.transform.position, LineTarget.transform.position, 5, 0.01F + PlayerControl.MatchCount * 0.005F);
				for(int i = 0; i < points.Length; i++)
				{
					Params.lineIn.SetPosition(i, points[i]);
					Params.lineOut.SetPosition(i, points[i] + Vector3.back);
				}

				Params.lineIn.enabled = true;
				Params.lineIn.SetColors(GameData.instance.GetGENUSColour(Genus) * 0.9F, GameData.instance.GetGENUSColour(Genus) * 0.9F);

				Params.lineOut.enabled = true;
				Params.lineOut.SetColors(Color.white, Color.white);
				*/
			}
		}		
	}

	void LateUpdate()
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), Time.deltaTime * 8);
		Params.counter.transform.rotation = Quaternion.Euler(Vector3.zero);
		Params.otherWarning.transform.rotation = Quaternion.Euler(Vector3.zero);
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

		Ray velRay = new Ray(transform.position, velocity);
		RaycastHit hit;

		if(Physics.Raycast(transform.position, -velocity, out hit, (speed + collide_radius) * Time.deltaTime * 80))
		{
			Tile hit_tile = hit.transform.gameObject.GetComponent<Tile>();

			if(hit_tile != null)
			{
				Vector3 clamppos = transform.position;
				if(Stats.Shift == ShiftType.Down && hit_tile.Point.Base[1] < Point.Base[1]) 
				{
					clamppos.y = Mathf.Clamp(clamppos.y, hit_tile.transform.position.y + collide_radius * 2, 100);
				}
				else if(Stats.Shift == ShiftType.Up && hit_tile.Point.Base[1] > Point.Base[1])
				{
					clamppos.y = Mathf.Clamp(clamppos.y, -100, hit_tile.transform.position.y - collide_radius * 2);
				}
				if(Stats.Shift == ShiftType.Left && hit_tile.Point.Base[0] < Point.Base[0]) 
				{
					clamppos.x = Mathf.Clamp(clamppos.x, hit_tile.transform.position.x + collide_radius * 2, 100);
				}
				else if(Stats.Shift == ShiftType.Right && hit_tile.Point.Base[0] > Point.Base[0])
				{
					clamppos.x = Mathf.Clamp(clamppos.x, -100, hit_tile.transform.position.x - collide_radius * 2);
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
		{
			float finalspeed = speed - gravity;
			speed = Mathf.Clamp(finalspeed, speed_max_falling, speed_max_rising);
		}
		if(!GameManager.inStartMenu && TileMaster.Grid != null)
		{
			if(Stats.Shift == ShiftType.Down && transform.position.y <= Point.targetPos.y - (speed*Time.deltaTime)) 
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
				OnLand();
			}
			else if(Stats.Shift == ShiftType.Up && transform.position.y >= Point.targetPos.y + (speed*Time.deltaTime)) 
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				
				transform.position = new Vector3(transform.position.x, Point.targetPos.y, transform.position.z);
				OnLand();
			}
			else if(Stats.Shift == ShiftType.Right && transform.position.x >= Point.targetPos.x + (speed * Time.deltaTime))
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				
				transform.position = new Vector3(Point.targetPos.x, transform.position.y, transform.position.z);
				OnLand();
			}
			else if(Stats.Shift == ShiftType.Left && transform.position.x <= Point.targetPos.x - (speed * Time.deltaTime))
			{
				velocity = Vector3.zero;
				speed = 0.0F;
				
				transform.position = new Vector3(Point.targetPos.x, transform.position.y, transform.position.z);
				OnLand();
			}
			else 
			{
				if(speed >= 0.0F) speed = -0.1F;
				transform.localScale = new Vector3(defaultScale+speed/90, defaultScale,defaultScale);
				isFalling = true;
			}
		}

		transform.position += velocity * speed * Time.deltaTime;
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
			targetColor = def * 0.65F;
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

	public virtual IEnumerator BeforeMatch(bool original)
	{
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
		InitStats.Hits -= 1;
		CheckStats();
		AudioManager.instance.PlayClipOn(this.transform, "Player", "Match");
		if(Stats.Hits <= 0)
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
			EffectManager.instance.PlayEffect(this.transform,Effect.Attack);
		}
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


		isFalling = false;
		if(!IsState(TileState.Selected) && !IsState(TileState.Locked))  {
			SetState(TileState.Idle);
		}
	}

	public virtual void CollectThyself(bool destroy)
	{
		if(this == null) return;

		if(destroy)
		{
			TileMaster.instance.AddVelocityToColumn(Point.Base[0], Point.Base[1], 0.2F + Stats.Value * 0.5F);
			Destroyed = true;
			GetComponent<SphereCollider>().enabled = false;
			
		}
		
		TileMaster.instance.CollectTile(this, destroy);
	}

	public virtual void DestroyThyself(bool collapse = false)
	{
		if(this == null) return;

		//TileMaster.instance.AddVelocityToColumn(Point.Base[0], Point.Base[1], 0.2F + Stats.Value * 0.5F);
		Destroyed = true;
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
			float life = 1.0F;
	
			float sidevel = UnityEngine.Random.value > 0.5F ? UnityEngine.Random.value * 0.1F : -UnityEngine.Random.value * 0.1F;
			
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

	public virtual IEnumerator Animate(string type, float time = 0.0F)
	{
		if(_anim == null) yield break;
		_anim.SetTrigger(type);

		if(time != 0.0F) yield return new WaitForSeconds(time);
		else yield return null;
	}

	public virtual bool CanAttack() {return Type.isEnemy && Stats.Attack > 0;}
	public virtual int GetAttack() {return Mathf.Max(Stats.Attack, 0);}
	public virtual int GetHealth() {return Mathf.Max(Stats.Hits,0);}
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

			Vector3 [] points = LightningLine(this.transform.position, nextTile.transform.position, 4, 0.1F);

			for(int i = 0; i < points.Length; i++)
			{
				Params.lineIn.SetPosition(i, points[i]);
				Params.lineOut.SetPosition(i, points[i] + Vector3.back);
			}
			//Params.lineIn.SetPosition(0, this.transform.position);
			//Params.lineIn.SetPosition(1, nextTile.transform.position);
			Params.lineIn.SetColors(GameData.instance.GetGENUSColour(Genus), GameData.instance.GetGENUSColour(nextTile.Genus));

			Params.lineOut.SetColors(Color.white, Color.white);
			//Params.lineOut.SetPosition(0, this.transform.position + Vector3.back);
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

	public bool IsType(string s)
	{
		return Type.IsType(s);
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
		//if(!IsState(TileState.Locked) && 
		state_override = false;
		originalMatch = false;
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

	public virtual void SetValue(int val)
	{
		Stats.Value = val;
	}

	public virtual void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);

		int diff = (int) InitStats.value_soft - InitStats.Value;
		if(diff != 0)
		{
			InitStats.Value = (int) InitStats.value_soft;
			CheckStats();
			UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + Stats.Value, 75, Color.white, 0.8F,0.00F);
			
			Animate("Alert");
			SetSprite();
		}
	}

	public void CheckStats()
	{
		Stats = new TileStat(InitStats);
		for(int i = 0; i < Effects.Count; i++)
		{
			if(Effects[i] == null) Effects.RemoveAt(i);
			else Stats.Add(Effects[i].CheckStats(), false);
		}
	}

	public virtual void BeforeTurn()
	{

	}

	public virtual IEnumerator BeforeTurnRoutine(){

		yield break;
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
		for(int i = 0; i < Effects.Count; i++)
		{
			if(Effects[i] == null) Effects.RemoveAt(i);
			else if(Effects[i].CheckDuration()) 
			{
				GameObject old = Effects[i].gameObject;
				Effects.RemoveAt(i);
				Destroy(old);
			}
		}
		CheckStats();
	}

	public virtual IEnumerator AfterTurnRoutine()
	{
		Reset();
		InitStats.TurnDamage = 0;
		InitStats.Lifetime ++;
		if(InitStats.Lifetime >= 1) 
		{
			InitStats.isNew = false;
		}
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
		CheckStats();
		
		yield break;
	}

	public virtual void SetSprite()
	{
		int sprite = Stats.Value / 5;
		SetBorder(Info.Outer);
		if(Info.Inner.Length > 0)
		{
			if(sprite > Info.Inner.Length - 1) sprite = Info.Inner.Length - 1;
		}
		else return;//sprite = 0;

		SetRender(Info.Inner[sprite]);
		
		if(Params._shiny != null && Params._render != null) Params._shiny.sprite = Params._render.sprite;
		//transform.position = Point.targetPos;
		Params.transform.position = transform.position;
		Params._render.transform.localPosition = Vector3.zero;
	}

	public  void SetBorder(Sprite border)
	{
		if(Params._border != null) Params._border.sprite = border;
	}
	public  void SetRender(Sprite render)
	{
		if(Params._render!= null) Params._render.sprite = render;
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
		e.transform.position = this.transform.position;
		e.transform.parent = this.transform;
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
		e.transform.position = this.transform.position;
		e.transform.parent = this.transform;
		Effects.Add(e);
		CheckStats();
		return e;
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
		Info.ChangeGenus(g);
		SetSprite();
	}

	public void MoveToGridPoint(int x, int y, float arc = 0.0F)
	{
		Vector3 newpoint = TileMaster.Grid[x,y].position;
		UnlockedFromGrid = true;

		MoveToPoint mp = this.gameObject.AddComponent<MoveToPoint>();
		mp.SetTarget(newpoint);
		mp.SetPath(0.13F, arc);
		mp.SetThreshold(0.1F);
		mp.DontDestroy = true;

		mp.SetMethod(() => {
			transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
			Params.transform.position = transform.position;
			Params._render.transform.localPosition = Vector3.zero;
			UnlockedFromGrid = false;
			
		});
		TileMaster.Grid[x, y]._Tile = this;
		Setup(x,y);
	}

}


public enum TileState{
	Idle,
	Hover,
	Selected,
	Locked,
	Falling
}

[System.Serializable]
public class TileStat
{
	public string Name;
	public int Value        = 0;
	[HideInInspector]
	public float value_soft = 0;

	public int Resource  = 0;
	public int Heal 	 = 0;
	public int Armour 	 = 0;
	
	public int Hits      = 0;
	public int Attack    = 0;
	public int Lifetime  = 0;
	public int Deathtime = 0;
	
	public bool isNew       = true;
	public bool isFrozen    = false;
	public bool isBroken    = false;
	public bool isAlerted   = false;
	public bool isAlly      = false;
	public int  AllyAttackType = 0;

	public int DOT = 0;
	public int TurnDamage = 0;
	public ShiftType Shift;

	public void Add(TileStat t, bool _override = true)
	{
		if(t == null) return;
		Value     += t.Value;

		Resource  += t.Resource;
		Heal 	  += t.Heal;
		Armour 	  += t.Armour;
		Hits      += t.Hits;
		Attack    += t.Attack;
		Lifetime  += t.Lifetime;
		Deathtime += t.Deathtime;
		DOT 	  += t.DOT;
		TurnDamage += t.TurnDamage;

		if(_override)
		{
			isFrozen = t.isFrozen;
			isBroken = t.isBroken;
			isAlerted = t.isAlerted;
			isAlly 	= t.isAlly;
			AllyAttackType = t.AllyAttackType;
			isNew = t.isNew;
		}
		else
		{
			if(t.isFrozen) isFrozen    = true;
			if(t.isBroken) isBroken    = true;
			//if(!t.isAlerted) isAlerted = false;
			if(t.isAlly)	
			{
				isAlly = true;
				AllyAttackType = 0;
			}
		}
		
	}

	public TileStat(TileStat t = null)
	{
		if(t != null)
		{
			Value     = t.Value;

			Resource  = t.Resource;
			Heal 	  = t.Heal;
			Armour 	  = t.Armour;
			Hits      = t.Hits;
			Attack    = t.Attack;
			Lifetime  = t.Lifetime;
			Deathtime = t.Deathtime;
			DOT 	  = t.DOT;
			TurnDamage = t.TurnDamage;
			isFrozen = t.isFrozen;
			isBroken = t.isBroken;
			isAlerted = t.isAlerted;
			isAlly 	= t.isAlly;
			AllyAttackType = t.AllyAttackType;
			isNew = t.isNew;
			Shift = t.Shift;
		}
	}

	public int [] GetValues()
	{
		return new int [] {(int) (Resource * Value), (int)(Heal * Value), (int)(Armour * Value)};
	}
}

[System.Serializable]
public class TilePointContainer
{
	public int BaseX, BaseY;
	public List<int> AllX, AllY;
	public int Scale;


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

	public TilePointContainer(int x, int y, int sc, Tile p)
	{
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
		if(TileMaster.Tiles == null || TileMaster.Grid == null) 
		{
			return;
		}

		TopX = BaseX + (Scale - 1);
		TopY = BaseY + (Scale - 1);
		
		for(int xx = 0; xx < Scale; xx++)
		{
			for(int yy = 0; yy < Scale; yy++)
			{
				if(TileMaster.Tiles.GetLength(0) <= BaseX + xx || TileMaster.Tiles.GetLength(1) <= BaseY + yy) continue;
				AllX.Add(BaseX + xx);
				AllY.Add(BaseY + yy);
				TileMaster.Tiles[BaseX+xx, BaseY+yy] = parent;
			}
			
		}
		int end = AllX.Count - 1;

		Vector3 bottomleft = TileMaster.Grid.GetPoint(BaseX, BaseY);
		Vector3 topright = TileMaster.Grid.GetPoint(TopX, TopY);
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

	public Tile [] GetNeighbours(bool diags = false)
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
				
				if(a != null && !final.Contains(a) && a != parent) final.Add(a);
				if(b != null && !final.Contains(b) && b != parent) final.Add(b);
				if(c != null && !final.Contains(c) && c != parent) final.Add(c);
				if(d != null && !final.Contains(d) && d != parent) final.Add(d);

				if(diags)
				{
					Tile e = TileMaster.instance.GetTile(AllX[xx]+1, AllY[yy]+1);
					Tile f = TileMaster.instance.GetTile(AllX[xx]-1, AllY[yy]-1);
					if(e != null && !final.Contains(e) && e != parent) final.Add(e);
					if(f != null && !final.Contains(f) && f != parent) final.Add(f);

					Tile g = TileMaster.instance.GetTile(AllX[xx]+1, AllY[yy]-1);
					Tile h = TileMaster.instance.GetTile(AllX[xx]-1, AllY[yy]+1);
					if(g != null && !final.Contains(g) && g != parent) final.Add(g);
					if(h != null && !final.Contains(h) && h != parent) final.Add(h);
				}
				
			}
		}
		return final.ToArray();
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