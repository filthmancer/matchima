using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;


public class Class : Unit {


	public virtual StCon _Name
	{
		get
		{
			return new StCon("The " + Name + "   LVL: " + Level, GameData.Colour(Genus));
		}
	}

	public virtual StCon [] _Desc
	{
		get
		{
			List<StCon> final = new List<StCon>();
			final.Add(new StCon(Meter + "/" + MeterMax + " Mana", GameData.Colour(Genus), true));
			final.Add(new StCon(mainStat.StatCurrent + " " + GameData.StatLong(Genus), GameData.Colour(Genus), true));
			
			final.Add(new StCon("+" + Stats.HealthMax + " HP  ", Color.white, false));
			final.Add(new StCon("+" + Stats.Attack + " ATK", Color.white, true));

			for(int i = 0; i < Upgrades.Length; i++)
			{
				bool newline = i % 2 == 0;
				final.Add(new StCon(Upgrades[i], newline));
			}
			
			foreach(ClassEffect child in _Status)
			{
				final.AddRange(child.Description);
			}
			//if(Stats.Regen > 0) final.Add(new StCon("+" + Stats.Regen + " HP Regen", Color.white, false));
			//if(Stats.Spikes > 0) final.Add(new StCon("+" + Stats.Spikes + " Spikes", Color.white, false));

			//final.AddRange(Upgrades);			
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
	public Slot [] _Boons;
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
	public bool LevelUpAlert, PulseAlert;
	public bool LevelUp;

	public StatContainer mainStat;

	[HideInInspector]
	public bool activated;

	public string MeterString
	{
		get{
			return GameData.PowerString(Meter);
		}
	}
	public int Meter, MeterMax;

	public int BonusLevelRate = 0;
	public int WaveLevelRate = 0;
	public int TurnLevelRate = 1;
	public Stat Stats;
	public List<ClassEffect> _Status = new List<ClassEffect>();

	private int MeterMax_init;
	private float MeterMax_soft;
	private float MeterGain = 0.2F;

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



	// Use this for initialization
	public virtual void Start () {
		
	}
	
	public virtual void StartClass()
	{
		MeterMax_init = MeterMax;
		int i =0;
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			child.Parent = this;
			child.Init(i);
			i++;
		}
		foreach(Slot child in _Boons)
		{
			if(child == null) continue;
			child.Parent = this;
			child.Init(i);
			i++;
		}
		//RollForBonuses();
		InitStats.Setup();
		Reset();
		Stats._Health = Stats._HealthMax;
		Quotes.Setup(this);

		GetBaseUpgrades();
		//print(AllCurses.Length);

		gameObject.name = Name + ": " + GameData.StatLong(Genus);
		
	}

	public virtual void Update()
	{
		if(time_from_last_pulse < 5.0F) time_from_last_pulse += Time.deltaTime;
	}

	public virtual float GetMeterRatio()
	{
		if(Meter == 0 || MeterMax == 0) return 0.0F;
		float f = Meter * 1.0F;
		return f/MeterMax;
	}

	public void UpgradeStat(int i)
	{
		if(LevelPoints <= 0) return;
		GENUS g = (GENUS) i;
		switch(g)
		{
			case GENUS.STR:
			InitStats.Strength += 1;
			break;
			case GENUS.DEX:
			InitStats.Dexterity += 1;
			break;
			case GENUS.WIS:
			InitStats.Wisdom += 1;
			break;
			case GENUS.CHA:
			InitStats.Charisma += 1;
			break;
		}
		Reset();
		LevelPoints -= 1;
	}


	public void UpgradeBonus(int i)
	{
		switch(i)
		{
			case 0:
			//BonusA.Upgrade(ref LevelPoints);
			break;
			case 1:
			//BonusB.Upgrade(ref LevelPoints);
			break;
		}
		Reset();
	}

	public void Reset()
	{
		float ratio = (float) Stats._Health / (float) Stats._HealthMax;
		int initres = Meter;
		int heal = Stats.HealThisTurn;

		InitStats.CheckStatInc();
		Stats = new Stat(InitStats);
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			if(child is Item) child.Drag = DragType.None;
			if(child.GetStats() != null) Stats.AddStats(child.GetStats());
		}
		foreach(Slot child in _Boons)
		{
			if(child == null) continue;
			if(child.GetStats() != null) Stats.AddStats(child.GetStats());
		}
		foreach(ClassEffect child in _Status)
		{
			if(child == null) continue;
			if(child.CheckStats()!= null) Stats.AddStats(child.CheckStats());
		}

		Stats.ApplyStatInc();
		Stats._Health = (int) Mathf.Clamp(Stats._HealthMax * ratio, 0, Stats._HealthMax);
		Stats.HealThisTurn = heal;
		Stats.Class_Type = Genus;
		mainStat = Stats.GetResourceFromGENUS(Genus);

		MeterMax_soft = MeterMax_init * (1.0F + (MeterGain * Level)) + mainStat.MeterInc + Stats.MeterMax;
		MeterMax = (int) MeterMax_soft;
		Meter = initres;
	}


	public virtual IEnumerator BeginTurn()
	{
		ManaThisTurn = 0;
		yield return null;
	}

	public virtual void EndTurn()
	{
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			child.AfterTurnA();
		}
		foreach(Slot child in _Slots)
		{
			if(child == null) continue;
			child.AfterTurnB();
		}

		foreach(Slot child in _Boons)
		{
			if(child == null) continue;
			child.AfterTurnA();
		}
		foreach(Slot child in _Boons)
		{
			if(child == null) continue;
			child.AfterTurnB();
		}
		
		AddToMeter(Stats.ManaRegen);
		Reset();
	}


	public IEnumerator CheckForBoon()
	{
		if(!LevelUp) yield break;

		int x = Utility.RandomInt(TileMaster.Tiles.GetLength(0));
		int y = Utility.RandomInt(TileMaster.Tiles.GetLength(1));

		//while(TileMaster.Tiles[x,y].Info._TypeName == "boon" || 
		//	  TileMaster.Tiles[x,y].Info._TypeName == "curse" || 
		//	  TileMaster.Tiles[x,y].Info._TypeName == "cocoon"||
		//	  TileMaster.Tiles[x,y].Info._TypeName == "chest")
		while((TileMaster.Tiles[x,y].Info._TypeName != "resource" && TileMaster.Tiles[x,y].Point.Scale > 1)  && Player.QueuedSpell(x,y))
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
		move.SetPath(0.9F, true, false, 0.25F);
		move.SetMethod( () => {
			if(UnityEngine.Random.value < 0.95F) GetSpellTile(x,y,Genus,LevelPoints);
			else GetSpellFizzle(x,y,Genus, LevelPoints);
			
			LevelPoints = 0;
		});
		
		//yield return StartCoroutine(GenerateLevelChoice());
		//TurnLevelRate = 1;
		HasLeveled = false;
		LevelUp = false;
		yield return new WaitForSeconds(Time.deltaTime * 20);
	}

	public virtual void GetSpellTile(int x, int y, GENUS g, int points)
	{
		TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["cocoon"], g, TileMaster.Tiles[x,y].Point.Scale, points);	
	}

	public virtual void GetSpellFizzle(int x, int y, GENUS g, int points)
	{
		MiniAlertUI m = UIManager.instance.MiniAlert(TileMaster.Tiles[x,y].Point.targetPos, "FIZZLE", 40, GameData.Colour(g), 1.2F, 0.3F);
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
		foreach(Slot child in _Boons)
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
		ManaThisTurn += res;
		if(res > 0) 
		{
			if(time_from_last_pulse > 1.3F)
			{
				UIManager.ClassButtons[Index].GetComponent<Animator>().SetTrigger("Pulse");
				time_from_last_pulse = 0.0F;			
			}
		}
		while(Meter >= MeterMax)
		{
			//LevelPoints ++;

		//NEW STYLE LEVELING
			TurnLevelRate = (int) (MeterMax / 25);
			LevelPoints += TurnLevelRate + BonusLevelRate + WaveLevelRate;
			LevelPoints += Stats.BoonIncrease;
			TurnLevelRate = 0;

			BonusLevelRate = 0;
			WaveLevelRate = 0;
			Level ++;
			LevelUp = true;
			LevelUpAlert = true;
			InitStats.LevelUp();
			Meter = (int)Mathf.Clamp(Meter - MeterMax,0,Mathf.Infinity);
			Player.instance.ResetStats();
			
			//Stats._Health = Stats._HealthMax;
			//Reset();
			//if(Level % 10 == 0) 
			//{
				//Ability a = (Ability) Instantiate(RollForUnlock());
				//GetSlot(a as Slot);
			//}
		}
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
			Quote abalert = new Quote(Name + type + s.Name.Value, false, 0.6F);
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
			Quote abalert = new Quote(Name + type + s.Name.Value, false, 0.6F);
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

	public IEnumerator GenerateLevelChoice()
	{
		//while(UIManager.InMenu) yield return null;
		UIManager.InMenu = true;

		ClassUpgrade [] choices = RollUpgrades(2, LevelPoints);
		//UIManager.instance.OpenBoonUI(this, choices);

		while(UIManager.BoonUI_active)
		{
			yield return null;
		}

		//yield return StartCoroutine(UIManager.instance.LevelChoiceRoutine(this, choices));
		//if(UIManager.LevelChoice.HasValue)
		//{
			//choices[UIManager.LevelChoice.Value].Upgrade();
		//}

		/*if(UIManager.LevelChoice.Value == -1)
		{
			LevelPoints = 0;
		}
		else
		{
			ClassUpgrade final = choices[UIManager.LevelChoice.Value];
			int loops = (final.IgnoreValue ? 1 : final.Value);

			final.Current += final.Value;
			yield return StartCoroutine(final.Upgrade);
				
			LevelPoints = 0;
			TurnLevelRate = 1;
		}*/

		UIManager.Objects.LevelUpMenu.SetActive(false);
		foreach(ClassUpgrade child in choices)
		{
			child.Value = 1;
		}
		UIManager.LevelChoice = null;
		Player.instance.ResetStats();
		UIManager.InMenu = false;
		

		HasLeveled = true;
		bool closeUI = true;
		foreach(Class child in Player.Classes)
		{
			if(!child.HasLeveled) closeUI = false;
		}


		if(closeUI)
		{
			UIManager.Objects.BigUI.SetActive(false);
			UIManager.Objects.LevelUpMenu.SetActive(false);
		}

		yield return null;

	}

	/*public void GenerateSlotUpgrades()
	{
		foreach(Ability child in _Unlocks)
		{
			ClassUpgrade spell = new ClassUpgrade((int val) => {GetSlotRoutine(child);});
			spell.BaseAmount = 1;
			spell.Rarity = Rarity.Uncommon;
			spell.Prefix = "SPELL: ";
			spell.Suffix = child.Name_Basic;
			AddUpgrades(spell);
		}
	}

	public void GenerateSlotUpgrade(Slot s)
	{
		ClassUpgrade spell = new ClassUpgrade(SlotUpgrade(s));

		spell.BaseAmount = 1;
		spell.Rarity = s.Rarity;
		spell.Prefix = "Lvl. ";
		spell.Name = s.Name_Basic;
		spell.IgnoreValue = true;
		spell.SlotUpgrade = true;
		spell.slotobj = s;
		//spell.Suffix = s.Name_Basic;
		AddUpgrades(spell);
	}*/

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