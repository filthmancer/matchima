﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;


public class Class : Unit {


	public virtual StCon _Name
	{
		get
		{
			string name = "The " + Name;
			//if(MeterLvl > 0) name += "  M-POWER: " + MeterLvl;
			return new StCon(name, GameData.Colour(Genus));
		}
	}

	public virtual StCon [] _Desc
	{
		get
		{
			List<StCon> final = new List<StCon>();
			final.Add(new StCon(Meter + "/" + MeterTop + " Mana", GameData.Colour(Genus)));
			final.Add(new StCon("Lvl " + Level + " ("+ Exp_Current + "/" + Exp_Max + " xp)", GameData.Colour(GENUS.WIS)));

			for(int i = 0; i < Stats.Length; i++)
			{
				bool last = i==Stats.Length-1;
				final.Add(new StCon(Stats[i].StatCurrent+"", GameData.Colour((GENUS)i), last));
				if(!last) final.Add(new StCon(" /", Color.white, false));
			}
		//	final.Add(new StCon(mainStat.StatCurrent + " " + GameData.StatLong(Genus), GameData.Colour(Genus)));
			
			//final.Add(new StCon("+" + Stats.HealthMax + " HP  ", Color.white, false));
			//final.Add(new StCon("+" + Stats.Attack + " ATK", Color.white, true));

			foreach(Slot child in AllMods)
			{
				if(child != null) final.AddRange(child.Description_Tooltip);
			}

			//for(int i = 0; i < Upgrades.Length; i++)
			//{
			//	bool newline = i % 2 == 0;
			//	final.Add(new StCon(Upgrades[i], newline));
			//}
			
			foreach(ClassEffect child in _Status)
			{
				final.AddRange(child.Description);
			}
				
			return final.ToArray();
		}
	}

	public virtual StCon [] Upgrades
	{
		get
		{
			List<StCon> final = new List<StCon>();
			int num = 1;
			foreach(ClassUpgrade child in AllBoons)
			{
				if(child.Current == 0) continue;
				final.Add(new StCon(child.CurrentString + " " + child.Name, Color.white, (num % 3 == 0)));
				num++;
			}
			return final.ToArray();
		}
	}
	public Sprite Icon;
	public bool Unlocked = true;

	public ClassInfo Info;
	
	public Stat InitStats;
	public List<Slot> AllMods;
	public Slot InitMod;

	public Slot [] _Boons
	{
		get
		{
			List<Slot> final = new List<Slot>();
			foreach(Slot child in AllMods)
			{
				if((child is Ability) && (child as Ability).Type == ModType.Boon) final.Add(child);
			}
			return final.ToArray();
		}
	}
	public Slot [] _Curses
	{
		get
		{
			List<Slot> final = new List<Slot>();
			foreach(Slot child in AllMods)
			{
				if((child is Ability) && (child as Ability).Type == ModType.Curse) final.Add(child);
			}
			return final.ToArray();
		}
	}
	public Slot [] _Slots = new Slot[1];
	
	public ClassQuotes Quotes; 

	public bool isKilled = false;

	public ClassUpgrade [] AllBoons;
	public ClassUpgrade [] AllCurses;

	public int Level = 1;
	public int LevelPoints = 0;

	[HideInInspector]
	public int ManaThisTurn = 0;
	public bool CanCollectMana = true;

	[HideInInspector]
	public bool LevelUpAlert;

	public StatContainer mainStat;

	[HideInInspector]
	public bool activated;

	public string MeterString
	{
		get{
			return GameData.PowerString(Meter);
		}
	}
	public int Meter;
	public int _MeterMax;
	public int MeterLvl;


	public int MeterTop
	{
		get{
			if(MeterLvl+1 >= MeterMax_array.Length) return 1000;
			else if(MeterLvl+1 < 0) return MeterMax_array[0];
			else return MeterMax_array[MeterLvl+1];
		}
	}
	public int MeterBottom
	{
		get{
			return 0;
			//if(MeterLvl >= MeterMax_array.Length) return MeterMax_array[MeterMax_array.Length-1];
			//else if(MeterLvl < 0) return 0;
			//else return MeterMax_array[MeterLvl] - 5;
		}
	}

	[HideInInspector]
	public int [] MeterMax_array = new int[3];

	[HideInInspector]
	public int BonusLevelRate = 0,WaveLevelRate = 0, TurnLevelRate = 1;
	public Stat Stats;

	private float MeterMax_soft;
	private float MeterGain = 0.2F;

	protected bool ManaPowerActivate = false;
	protected bool ManaPowerActive = false;
	protected int MeterDecay = 1;
	protected float MeterDecay_soft = 1.0F;
	protected float [] MeterDecayInit = new float[]
	{
		0, 6, 7, 9
	};

	public GameObject ManaPowerParticle;

	private bool LowHealthWarning = true, DeathWarning = true;
	private float time_from_last_pulse = 0.0F;
	private IntVector [] combo_thresholds = new IntVector[] 
	{
		new IntVector(15, 1), //GREAT
		new IntVector(35, 4), //AMAZING
		new IntVector(60, 10), //IMPOSSIBLE
		new IntVector(100, 20), //MATCHIMADNESS
	};
	private int combo_biggest = 0;

	private bool HasLeveled = true;
	public int Exp_Max, Exp_Current;
	private float Exp_Max_soft;


	// Use this for initialization
	public virtual void Start () {
		
	}
	
	public virtual void StartClass()
	{
		Exp_Current = 0;
		Exp_Max = 50;
		Exp_Max_soft = 50.0F;
		
		Meter = 0;

		int i = 0;
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			child.Parent = this;
			child.Init(i++);
		}
		if(InitMod!= null)
		{
			InitMod.Parent = this;
			InitMod.Init(i++);
		}
		
		for(int m = 0; m < AllMods.Count; m++)
		{
			if(AllMods[m] == null)
			{
				AllMods.RemoveAt(m);
				m--;
				continue;
			}
			AllMods[m].Parent = this;
			AllMods[m].Init(m);
		}
		InitStats.Setup();
		Reset();
		Stats._Health = Stats._HealthMax;
		Quotes.Setup(this);
		gameObject.name = Name + ": " + GameData.StatLong(Genus);
	}

	public virtual void Update()
	{
		if(time_from_last_pulse < 5.0F) time_from_last_pulse += Time.deltaTime;
		if(ManaPowerParticle != null)	ManaPowerParticle.transform.position = UIManager.ClassButtons[(int)Genus].transform.position;
	}

	public virtual float GetMeterRatio()
	{
		if(Meter == 0 || MeterTop == 0) return 0.0F;
		float f = Meter * 1.0F;
		return (f-MeterBottom)/(MeterTop-MeterBottom);
	}



	public virtual void Reset()
	{
		float ratio = (float) Stats._Health / (float) Stats._HealthMax;
		//int initres = Meter;
		int heal = Stats.HealThisTurn;

		InitStats.CheckStatInc();
		Stats = new Stat(InitStats);
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			if(child is Item) child.Drag = DragType.None;
			if(child.GetStats() != null) Stats.AddStats(child.GetStats());
		}
		foreach(Slot child in AllMods)
		{
			if(child == null) continue;
			if(child.GetStats() != null) Stats.AddStats(child.GetStats());
		}
		foreach(ClassEffect child in _Status)
		{
			if(child == null) continue;
			if(child.CheckStats()!= null) Stats.AddStats(child.CheckStats());
			child.StatusEffect();
		}

		Stats.ApplyStatInc();
		Stats._Health = (int) Mathf.Clamp(Stats._HealthMax * ratio, 0, Stats._HealthMax);
		Stats.HealThisTurn = heal;
		Stats.Class_Type = Genus;
		mainStat = Stats.GetResourceFromGENUS(Genus);

		MeterMax_soft = _MeterMax * (1.0F + (MeterGain * Level)) + Stats.MeterMax;
		MeterMax_array = new int [] {0, (int)(MeterMax_soft), (int)(MeterMax_soft * 2F), (int)(MeterMax_soft * 4.8F)};
	}

	public virtual IEnumerator BeforeMatch(List<Tile> tiles)
	{
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.BeforeMatch(tiles));
		}
		foreach(Slot child in AllMods)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.BeforeMatch(tiles));
		}
		yield return null;
	}


	public virtual IEnumerator BeginTurn()
	{
		ManaThisTurn = 0;
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.BeforeTurn());
		}
		foreach(Slot child in AllMods)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.BeforeTurn());
		}

		AddToMeter(Stats.MeterRegen);
		MeterDecay_soft *= (GameManager.MeterDecay[MeterLvl] + Stats.MeterDecay[MeterLvl] + Stats.MeterDecay_Global);
		MeterDecay = (int)MeterDecay_soft;
		if(Meter < MeterDecay) Meter = 0;
		else AddToMeter(-MeterDecay);
			
		
		yield return StartCoroutine(CheckManaPower());
		Reset();
		yield return null;
	}

	public virtual IEnumerator EndTurn()
	{
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.AfterTurn());
		}
		foreach(Slot child in AllMods)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.AfterTurn());
		}
		Reset();
		yield return null;
	}

	public IEnumerator CheckManaPower()
	{
		if(Meter == 0 && MeterLvl > 0)
		{
			UIManager.ClassButtons[Index].ShowClass(true);
			MeterLvl = 0;
			MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons[(int)Genus].transform.position, "POWER\nDOWN", 75, GameData.Colour(Genus), 1.2F, 0.2F);
			yield return new WaitForSeconds(0.1F);
			MeterDecay_soft = MeterDecayInit[0];
			MeterDecay = (int) MeterDecay_soft;
			if(ManaPowerParticle != null) Destroy(ManaPowerParticle);
			ManaPower(0);
		}
		else
		{
			
			int newlvl = 0;
			for(int i = 0; i < MeterMax_array.Length; i++)
			{
				if(Meter >= MeterMax_array[i]) newlvl = i;
			}
			//print(Meter + ":" + newlvl + " - " + MeterMax_array[1]);
			if(MeterLvl < newlvl)
			{
				UIManager.instance.WaveAlert.SetTween(0,true);
				UIManager.ClassButtons[Index].ShowClass(true);
				yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
				
				GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Genus));
				powerup.transform.position = UIManager.ClassButtons[(int)Genus].transform.position;

				yield return new WaitForSeconds(GameData.GameSpeed(0.84F));
				UIManager.instance.WaveAlert.SetTween(0,false);
				Destroy(powerup);

				MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons[(int)Genus].transform.position, "POWER\nUP", 75, GameData.Colour(Genus), 1.2F, 0.2F);

				MeterLvl = newlvl;
				MeterDecay_soft = MeterDecayInit[MeterLvl];
				MeterDecay = (int) MeterDecay_soft;
				ManaPower(MeterLvl);

				Effect e = MeterLvl == 1 ? Effect.ManaPowerLvl1 : (MeterLvl == 2 ? Effect.ManaPowerLvl2 : Effect.ManaPowerLvl3);
				ParticleSystem part = EffectManager.instance.PlayEffect(this.transform, e, "", GameData.Colour(Genus)).GetComponent<ParticleSystem>();
				if(ManaPowerParticle != null) Destroy(ManaPowerParticle);
				ManaPowerParticle = part.gameObject;
				ManaPowerParticle.transform.position = UIManager.ClassButtons[(int)Genus].transform.position;
				yield return null;
			}
		}



		
		/*if(newlvl == 0 && MeterLvl != 0) 
		{
			MeterLvl = 0;
			MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons[(int)Genus].transform.position, "POWER\nDOWN", 75, GameData.Colour(Genus), 1.2F, 0.2F);
			yield return new WaitForSeconds(0.1F);
			Meter = 0;
			//MeterDecay_soft = MeterDecayInit[MeterLvl];
			//MeterDecay = (int)MeterDecay_soft;
			//ManaPower(MeterLvl);

			if(ManaPowerParticle != null) Destroy(ManaPowerParticle);
		}
		else if(newlvl != 0 && MeterLvl < newlvl)
		{
			MeterLvl = newlvl;
			MeterDecay_soft = MeterDecayInit[MeterLvl];
			MeterDecay = (int) MeterDecay_soft;
			ManaPower(MeterLvl);

			GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Genus));
			powerup.transform.position = UIManager.ClassButtons[(int)Genus].transform.position;
			UIManager.instance.WaveAlert.SetTween(0,true);
			yield return new WaitForSeconds(0.95F);
			UIManager.instance.WaveAlert.SetTween(0,false);
			Destroy(powerup);
			MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons[(int)Genus].transform.position, "POWER\nUP", 75, GameData.Colour(Genus), 1.2F, 0.2F);

			Effect e = MeterLvl == 1 ? Effect.ManaPowerLvl1 : (MeterLvl == 2 ? Effect.ManaPowerLvl2 : Effect.ManaPowerLvl3);
			ParticleSystem part = EffectManager.instance.PlayEffect(this.transform, e, "", GameData.Colour(Genus)).GetComponent<ParticleSystem>();
			part.startColor = GameData.Colour(Genus);

			if(ManaPowerParticle != null) Destroy(ManaPowerParticle);
			ManaPowerParticle = part.gameObject;
			ManaPowerParticle.transform.position = UIManager.ClassButtons[(int)Genus].transform.position;

			yield return null;
		}*/


		
	}


	public IEnumerator CheckForBoon()
	{
		yield break;

//OLD STYLE (SPELL TILES)
/*
		int x = Utility.RandomInt(TileMaster.Tiles.GetLength(0));
		int y = Utility.RandomInt(TileMaster.Tiles.GetLength(1));	
		//while(TileMaster.Tiles[x,y].Info._TypeName == "boon" || 
		//	  TileMaster.Tiles[x,y].Info._TypeName == "curse" || 
		//	  TileMaster.Tiles[x,y].Info._TypeName == "cocoon"||
		//	  TileMaster.Tiles[x,y].Info._TypeName == "chest")
		while(!TileMaster.Tiles[x,y].IsType("resource") && TileMaster.Tiles[x,y].Point.Scale > 1  && Player.QueuedSpell(x,y))
		{
			x = Utility.RandomInt(TileMaster.Tiles.GetLength(0));
			y = Utility.RandomInt(TileMaster.Tiles.GetLength(1));
		}

		Player.QueueSpell(x,y);
		ParticleSystem part = (ParticleSystem) Instantiate(EffectManager.instance.Particles.TouchParticle);
		part.startColor = GameData.Colour(Genus);
		part.transform.position = UIManager.ClassButtons[Index].transform.position;

		MoveToPoint move = part.GetComponent<MoveToPoint>();
		move.enabled = true;
		move.SetTarget(TileMaster.Tiles[x,y].transform.position);
		move.SetPath(0.3F, 0.25F);
		move.SetMethod( () => {
			if(UnityEngine.Random.value < 0.95F) GetSpellTile(x,y,Genus,LevelPoints);
			else GetSpellFizzle(x,y,Genus, LevelPoints);
			LevelPoints = 0;
		});
		
		//yield return StartCoroutine(GenerateLevelChoice());
		//TurnLevelRate = 1;
*/

//NEW STYLE (MANA POWER)
		//ManaPowerActive = true;
		//MeterDecay = ManaPowerDecay_init;
		//MeterDecay_soft = ManaPowerDecay_init;

		//ParticleSystem part = (ParticleSystem) Instantiate(EffectManager.instance.Particles.ManaPower);
		//part.startColor = GameData.Colour(Genus);
		//ManaPowerParticle = part.gameObject;

		HasLeveled = false;
		yield return new WaitForSeconds(Time.deltaTime * 20);
	}

	public virtual void GetSpellTile(int x, int y, GENUS g, int points)
	{
		TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["cocoon"], g, TileMaster.Tiles[x,y].Point.Scale, points);	
	}

	public virtual void GetSpellFizzle(int x, int y, GENUS g, int points)
	{
		MiniAlertUI m = UIManager.instance.MiniAlert(TileMaster.Tiles[x,y].Point.targetPos, "FIZZLE", 50, GameData.Colour(g), 1.2F, 0.3F);
		int num = UnityEngine.Random.Range(0,3);
		switch(num)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y,TileMaster.Types["chicken"], GENUS.OMG, TileMaster.Tiles[x,y].Point.Scale, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y,TileMaster.Types["altar"], GENUS.OMG, TileMaster.Tiles[x,y].Point.Scale, points);
			break;
			case 2:
				TileMaster.instance.ReplaceTile(x,y,TileMaster.Types["blob"], g, TileMaster.Tiles[x,y].Point.Scale, points);
			break;
		}	
	}

	public virtual bool UpdateClass()
	{
		//TileMaster.instance.SetFillGrid(true);
		return true;
	}

	public virtual void CheckHealth()
	{
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			child.CheckHealth();
		}
		foreach(Slot child in AllMods)
		{
			if(child == null) continue;
			child.CheckHealth();
		}


		if(Player.Stats._Health > Player.Stats._HealthMax/5)
		{
			OnSafeHealth();
		}
		else if(Player.Stats._Health < Player.Stats._HealthMax/5 && Player.Stats._Health > 0) 
		{
			OnLowHealth();
		}
		else if(Player.Stats._Health <= 0 && !isKilled)
		{
			isKilled = true;
			LevelPoints = 0;
			Meter = 0;
			//Player.instance.ResetStats();
			OnDeath();
		}
	}

	public void Add(int res)
	{
		if(isKilled) return;
	}

	public void Complete()
	{
		AddToMeter(ManaThisTurn);
	}

	public void AddToMeter(int res)
	{
		if(isKilled || !CanCollectMana)
		{
			ManaThisTurn = 0;
			return;
		}
		Meter = (int)Mathf.Clamp(Meter + res, 0, Mathf.Infinity);
		//ManaThisTurn += res;
		if(res > 0) 
		{
			if(time_from_last_pulse > 1.3F)
			{
				UIManager.ClassButtons[Index].GetComponent<Animator>().SetTrigger("Pulse");
				time_from_last_pulse = 0.0F;			
			}
		}
		//if(Meter >= MeterTop)//while(Meter >= MeterTop)
		//{
			//LevelPoints ++;
			//TurnLevelRate = (int) (MeterTop / 25);
			//LevelPoints += TurnLevelRate + BonusLevelRate + WaveLevelRate;
			//LevelPoints += Stats.BoonIncrease;
			//TurnLevelRate = 0;
			//BonusLevelRate = 0;
			//WaveLevelRate = 0;

		//NEW STYLE METER (MANA POWER)
			//MeterLvl ++;
			//MeterDecay_soft = MeterDecayInit[MeterLvl-1];
			//MeterDecay = (int) MeterDecay_soft;
			//MeterDecay_soft = GameManager.MeterDecay[MeterLvl-1] + Stats.MeterDecay[MeterLvl-1] + Stats.MeterDecay_Global;
			//MeterDecay = (int)MeterDecay;
			//Player.instance.ResetStats();
		//}
	}

	public virtual void ManaPower(int lvl)
	{

	}

	public void DestroyManaPowers()
	{
		for(int i = 0; i < AllMods.Count; i++)
		{
			if(AllMods[i].ManaPowerMod)
			{
				Destroy(AllMods[i].gameObject);
				AllMods.RemoveAt(i);
				
			}
		}
	}

	public void AddExp(Enemy e)
	{
		Exp_Current += e.Stats.Value;
		while(Exp_Current > Exp_Max)
		{
			Exp_Current -= Exp_Max;
			LevelUp();
			Exp_Max_soft *= 1.5F;
			Exp_Max = (int)Exp_Max_soft;
		}
	}

	public void LevelUp()
	{
		Level ++;
		LevelUpAlert = true;
		InitStats.LevelUp();
	}

	public Slot AddMod(string name, params string [] args)
	{
		foreach(Slot child in AllMods)
		{
			if(child.Name_Basic == name) return null;
		}

		Slot m = (Slot) Instantiate(GameData.instance.GetMod(name));
		m.SetArgs(args);
		m.Parent = this;
		m.transform.parent = this.transform;
		AllMods.Add(m);
		return m;
	}


	public IEnumerator WaitForSlotDeleteRoutine(Slot a)
	{
		string type = (a is Ability ? " learn " : " equip ");
		Quote alerta = new Quote(Name + " wants to" + type + a.Name.Value + ", but has no free slots", true, 0.2F);
		Quote alertb = new Quote("Clear a slot to make room for " + a.Name.Value + "?",  true, 0.2F);
		alertb.SlotButtons = true;
		alerta.Parent = this;
		alerta.ShowTail = false;
		alertb.Parent = this;
		alertb.ShowTail = false;

		yield return StartCoroutine(UIManager.instance.Alert(alerta, alertb));

		int num = alertb.answer.Value;
		if(num < _Slots.Length)
		{
			Destroy(_Slots[num].gameObject);
			GetSlot(a, num);
		}
		else Destroy(a.gameObject);

		yield return null;
	}

	public void GetSlot(Slot s, int? num = null)
	{
		if(s == null) return;

		if(!num.HasValue)
		{
			for(int i = 0; i < _Slots.Length; i ++)
			{
				if(_Slots[i] == null) 
				{
					num = i;
					break;
				}
			}
		}

		if(!num.HasValue)
		{
			StartCoroutine(WaitForSlotDeleteRoutine(s));
		}
		else
		{

			s.transform.parent = this.transform;
			_Slots[num.Value] = s;
			s.Parent = this;
			s.Init(num.Value);
			string type = (s is Ability ? " learned " : " equipped ");
			if(s is Item) s.Drag = DragType.None;
			Quote abalert = new Quote(Name + type + s.Name.Value, false, 1.5F);
			abalert.Parent = this;
			abalert.ShowTail = false;
			Reset();
			Player.instance.ResetStats();
			StartCoroutine(UIManager.instance.Quote(abalert));
		}
	}

	public IEnumerator GetSlotRoutine(Slot s, int? num = null)
	{
		if(s == null) yield break;
		if(!num.HasValue)
		{
			for(int i = 0; i < _Slots.Length; i ++)
			{
				if(_Slots[i] == null) 
				{
					num = i;
					break;
				}
			}
		}

		if(!num.HasValue)
		{
			yield return StartCoroutine(WaitForSlotDeleteRoutine(s));
		}
		else
		{
			s.transform.parent = this.transform;
			_Slots[num.Value] = s;
			s.Parent = this;
			s.Init(num.Value);
			string type = (s is Ability ? " learned " : " equipped ");
			Quote abalert = new Quote(Name + type + s.Name.Value, false, 1.5F);
			abalert.Parent = this;
			abalert.ShowTail = false;
			Reset();
			Player.instance.ResetStats();
			if(s is Item) s.Drag = DragType.None;
			StartCoroutine(UIManager.instance.Quote(abalert));
		}
		yield return null;
	}

	public virtual void OnDeath()
	{
		//if(DeathWarning)
		//{
		//	StartCoroutine(UIManager.instance.Quote(Quotes.Death));
		//	DeathWarning = false;
		//	LowHealthWarning = false;
		//}
	}

	public virtual void OnLowHealth()
	{
		//if(LowHealthWarning)
		//{
		//	StartCoroutine(UIManager.instance.Quote(Quotes.Danger));
		//	LowHealthWarning = false;
		//}
	}

	public void OnSafeHealth()
	{
		DeathWarning = true;
		LowHealthWarning = true;
	}

	public void OnCombo(int combo)
	{
		//BIGGEST COMBO
		if(combo > combo_thresholds[0].x && combo > combo_biggest)
		{
			int bonus =  (combo - combo_biggest)/2;
			if(bonus < 1) bonus = 1;
			BonusLevelRate += bonus;
			combo_biggest = combo;
			StartCoroutine(UIManager.instance.Quote(Quotes.GoodBonus));
			return;
		}

		//COMBO THRESHOLDS
		for(int i = 0; i < combo_thresholds.Length; i++)
		{
			if(combo < combo_thresholds[i].x) break;
			else
			{
				if(i+1 < combo_thresholds.Length)
				{
					if(combo > combo_thresholds[i+1].x) continue;
					else
					{
						BonusLevelRate += combo_thresholds[i].y;
						if(UnityEngine.Random.value > 0.8F)
						{
							StartCoroutine(UIManager.instance.Quote(Quotes.GoodBonus));
						}
					}
				}
				else
				{
					StartCoroutine(UIManager.instance.Quote(Quotes.GoodBonus));
					BonusLevelRate += (combo/combo_thresholds[i].x) * combo_thresholds[i].y;
				}
			}
		}

		
	}

	public ClassUpgrade [] RollForBonuses(int num, int rarity = 1)
	{
		List<ClassUpgrade> final = new List<ClassUpgrade>();
		List<ClassUpgrade> start = new List<ClassUpgrade>();
		start.AddRange(AllBoons);
		for(int i = 0; i < num; i++)
		{
			if(start.Count == 0) break;

			int rand = UnityEngine.Random.Range(0, start.Count);
			final.Add(start[rand]);
			start.RemoveAt(rand);
		}

		return final.ToArray();
	}

	public void AddUpgrades(params ClassUpgrade [] ups)
	{
		List<ClassUpgrade> final = new List<ClassUpgrade>();
		final.AddRange(AllBoons);
		final.AddRange(ups);
		AllBoons = final.ToArray();
	}

	public void AddCurses(params ClassUpgrade [] ups)
	{
		List<ClassUpgrade> final = new List<ClassUpgrade>();
		final.AddRange(AllCurses);
		final.AddRange(ups);
		AllCurses = final.ToArray();
	}

	public ClassUpgrade [] RollUpgrades(int num, int rarity = 1)
	{
		List<ClassUpgrade> final = new List<ClassUpgrade>();
		List<ClassUpgrade> start = new List<ClassUpgrade>();
		start.AddRange(AllBoons);
		int closest_rarity = (int) GameData.ClosestRarity(rarity);
		int curr_rarity = 0;
		int added_value = 0;
		for(int i = 0; i < num; i++)
		{
			if(start.Count == 0) break;
			ClassUpgrade u = null;
			int rand = 0;
			bool hasUpgrade = false;
			while(!hasUpgrade)
			{
				if(start.Count == 0) break;
				rand = UnityEngine.Random.Range(0, start.Count);
				curr_rarity = (int)start[rand].Rarity;

				if(curr_rarity > closest_rarity)
				{
					start.RemoveAt(rand);
					continue;
				}
				else if (start[rand].slotobj != null)
				{
					if(final.Count > 0) continue;
				}

				if((curr_rarity == closest_rarity))
				{
					u = start[rand];
					hasUpgrade = true;
					start.RemoveAt(rand);
					break;
				}
				else if(curr_rarity < closest_rarity)
				{
					int diff = closest_rarity/curr_rarity;
					u = start[rand];
					u.Value += (int) Mathf.Clamp(diff, 1, Mathf.Infinity);
					hasUpgrade = true;
					start.RemoveAt(rand);
					break;
				}
			}
			if(u!=null)
			{
				//print(closest_rarity + " -- " + u.Name + ": "+ u.Rarity + " ("+ curr_rarity +")" + " Value: " + u.Value);
				final.Add(u);
				
			}
		}

		return final.ToArray();
	}

	public ClassUpgrade [] RollCurses(int num, int rarity = 1)
	{
		List<ClassUpgrade> final = new List<ClassUpgrade>();
		List<ClassUpgrade> start = new List<ClassUpgrade>();
		start.AddRange(AllCurses);
		int closest_rarity = (int) GameData.ClosestRarity(rarity);
		int curr_rarity = 0;
		int added_value = 0;
		for(int i = 0; i < num; i++)
		{
			if(start.Count == 0) break;
			ClassUpgrade u = null;
			int rand = 0;
			bool hasUpgrade = false;
			while(!hasUpgrade)
			{
				if(start.Count == 0) break;
				rand = UnityEngine.Random.Range(0, start.Count);
				curr_rarity = (int)start[rand].Rarity;

				if(curr_rarity > closest_rarity)
				{
					start.RemoveAt(rand);
					continue;
				}
				else if (start[rand].slotobj != null)
				{
					if(final.Count > 0) continue;
				}

				if((curr_rarity == closest_rarity))
				{
					u = start[rand];
					hasUpgrade = true;
					start.RemoveAt(rand);
					break;
				}
				else if(curr_rarity < closest_rarity)
				{
					int diff = closest_rarity/curr_rarity;
					u = start[rand];
					u.Value += (int) Mathf.Clamp(diff, 1, Mathf.Infinity);
					hasUpgrade = true;
					start.RemoveAt(rand);
					break;
				}
			}
			if(u!=null)
			{
				//print(closest_rarity + " -- " + u.Name + ": "+ u.Rarity + " ("+ curr_rarity +")" + " Value: " + u.Value);
				final.Add(u);
				
			}
		}
		return final.ToArray();
	}

	public void GetUpgrade(ClassUpgrade c)
	{
		StartCoroutine(c.Upgrade);

		string type = " gained ";
		string value = (c.Value * c.BaseAmount) + c.Suffix + " " + c.Name;
		Quote abalert = new Quote(Name + type + value, false, 1.0F);
		abalert.Parent = this;
		abalert.ShowTail = false;
		StartCoroutine(UIManager.instance.Quote(abalert));

		UIManager.instance.HideBoonUI();

		Reset();
		Player.instance.ResetStats();
	}


	public IEnumerator SlotUpgrade(Slot s)
	{
		Slot final = (Slot) Instantiate(s);
		//final.Upgrade(val);
		yield return StartCoroutine(GetSlotRoutine(final));
	}

	public static IEnumerator ActionUpgrade(ClassUpgrade c)
	{
		c._Action(c.Value);
		yield return null;
	}

	TileChance genus, genusres, bomb, chest, armour;
	TileChance aph_health, aph_res;

	public void GetBaseUpgrades()
	{
		AddUpgrades(DefaultClassUpgrades.DefaultBoons(InitStats, Genus));
		AddCurses(DefaultClassUpgrades.DefaultCurses(InitStats, Genus));
		/*AddUpgrades();*/
	}
}

[System.Serializable]
public class ClassQuotes
{
	public List<QuoteGroup> StartQuotes;
	public List<QuoteGroup> GoodBonusQuotes;
	public List<QuoteGroup> DangerQuotes;
	public List<QuoteGroup> DeathQuotes;
	public List<QuoteGroup> IdleQuotes;
	public List<QuoteGroup> InfoQuotes;

	public List<QuoteGroup> Special;

	public static Quote DefaultStart = new Quote("I'm alive!");
	public static Quote DefaultBonus = new Quote("What a bonus, Mark!");
	public static Quote DefaultDanger = new Quote("This looks bad...");
	public static Quote DefaultDeath = new Quote("Ack, curses...");
	public static Quote DefaultIdle = new Quote("Hmm, hmm...");

	public Quote [] Start
	{
		get{
			if(StartQuotes.Count == 0) return new Quote[] {DefaultStart};
			int r = UnityEngine.Random.Range(0, StartQuotes.Count);
			return StartQuotes[r].ToArray();
		}
	}

	public Quote [] GoodBonus
	{
		get{
			if(GoodBonusQuotes.Count == 0) return new Quote[] {DefaultBonus};
			int r = UnityEngine.Random.Range(0, GoodBonusQuotes.Count);
			return GoodBonusQuotes[r].ToArray();
		}
	}

	public Quote [] Danger
	{
		get{
			if(DangerQuotes.Count == 0) return new Quote[] {DefaultDanger};
			int r = UnityEngine.Random.Range(0, DangerQuotes.Count);
			return DangerQuotes[r].ToArray();
		}
	}

	public Quote [] Death
	{
		get{
			if(DeathQuotes.Count == 0) return new Quote[] {DefaultDeath};
			int r = UnityEngine.Random.Range(0, DeathQuotes.Count);
			return DeathQuotes[r].ToArray();
		}
	}
	
	public Quote [] Idle
	{
		get{
			if(IdleQuotes.Count == 0) return new Quote[] {DefaultIdle};
			int r = UnityEngine.Random.Range(0, IdleQuotes.Count);
			return IdleQuotes[r].ToArray();
		}
	}

	public Quote [] Info
	{
		get{
			if(InfoQuotes.Count == 0) return new Quote[] {DefaultIdle};
			int r = UnityEngine.Random.Range(0, InfoQuotes.Count);
			return InfoQuotes[r].ToArray();
		}
	}

	public QuoteGroup GetSpecial(string name)
	{
		foreach(QuoteGroup child in Special)
		{
			if(name == child.Name) return child;
		}
		return null;
	}

	public void Setup(Class c)
	{
		foreach(QuoteGroup child in StartQuotes) {child.Setup(c);}
		foreach(QuoteGroup child in GoodBonusQuotes) {child.Setup(c);}
		foreach(QuoteGroup child in DangerQuotes) {child.Setup(c);}
		foreach(QuoteGroup child in DeathQuotes) {child.Setup(c);}

		foreach(QuoteGroup child in IdleQuotes) {child.Setup(c);}
		foreach(QuoteGroup child in InfoQuotes) {child.Setup(c);}
	}
}
[System.Serializable]
public class Quote{
	public string Text;
	public float WaitTime = 0.0F;
	[HideInInspector]
	public float WaitTime_init = 0.3F;

	public bool OverrideTouch, OverrideWait;
	public bool SlotButtons, YesNoButtons;
	public bool accepted;
	public int? answer = null;


	//[HideInInspector]
	public Unit Parent;
	//[HideInInspector]
	public bool ShowTail = true;

	string current_string;
	int current_int;
	float TickTime;
	public float TickTime_init = 0.03F;

	public Quote(string _text, bool _override = false, float wait = 0.8F, Unit p = null)
	{
		Text = _text;
		OverrideTouch = _override;
		WaitTime = wait;
		WaitTime_init = WaitTime;
		if(p != null)
		{
			Parent = p;
			ShowTail = true;
		}
	}

	public void Reset()
	{
		current_string = string.Empty;
		current_int = 0;
	}


	public bool CheckForAccept(TextMeshProUGUI target = null)
	{
		bool overtouch = true;
		if(OverrideTouch) overtouch = Input.GetMouseButton(0);
		if(OverrideWait)
		{
			if(Input.GetMouseButton(0)) return true;
		}
		if((SlotButtons || YesNoButtons)) overtouch = answer.HasValue;

		if(current_int < Text.Length)
		{
			if(WaitTime < WaitTime_init*0.8F && Input.GetMouseButton(0))
			{
				current_string = Text;
				target.text = current_string;
				current_int = Text.Length;
			}
			else
			{
				if(TickTime > 0.0F) TickTime -= Time.deltaTime;
				else 
				{
					current_string += Text[current_int];
					current_int++;
					if((current_int < Text.Length) && Text[current_int] == ' ')
					{
						current_string += Text[current_int];
						current_int++;
					}
					TickTime = TickTime_init;
					target.text = current_string;
				}
			}
			
		}

		bool waitime = false;
		if(WaitTime > 0.0F) WaitTime -= Time.deltaTime;
		else 
		{
			waitime = true;
			if(accepted) WaitTime = WaitTime_init;
		}



		
		accepted = overtouch && waitime;
		return accepted;
	}

}

[System.Serializable]
public class QuoteGroup{
	public string Name;
	public List<Quote> Quotes;

	public Func<bool> Unlocked;

	public Quote RandomQuote
	{
		get{
			return Quotes[UnityEngine.Random.Range(0, Quotes.Count)];
		}
	}

	public QuoteGroup(string n)
	{
		Name = n;
		Quotes = new List<Quote>();
	}

	public void AddQuote(string _text, Unit c, bool _override = false, float wait = 3.0F, float tick = 0.01F)
	{
		Quote q = new Quote(_text, _override, wait, c);
		q.TickTime_init = tick;
		Quotes.Add(q);
	}

	public void AddQuote(Quote q)
	{
		Quotes.Add(q);
	}

	public Quote [] ToArray()
	{
		return Quotes.ToArray();
	}

	public void Setup(Unit c)
	{
		foreach(Quote child in Quotes)
		{
			if(child.Parent == null) child.Parent = c;
			child.ShowTail = true;
			child.WaitTime_init = child.WaitTime;
		}
	}
}


[System.Serializable]
public class UpgradeGroup
{
	public ClassUpgrade [] Upgrades;
	public Class Target;
	public bool CanSkip = false;

	public UpgradeGroup(Class t, params ClassUpgrade [] u)
	{
		Upgrades = u;
		Target = t;
	}
}


[System.Serializable]
public class ClassInfo
{
	public int HealthRating;
	public int AttackRating;
	public int MagicRating;

	public string Name;
	public string ShortName;
	public string Description;
}