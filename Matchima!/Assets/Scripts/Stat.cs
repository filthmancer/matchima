using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stat
{
	public int Level = 1;
	public GENUS Class_Type;

	public string Health
	{
		get{
			return GameData.PowerString(_Health);
		}
	}
	public string HealthMax
	{
		get
		{
			return GameData.PowerString(_HealthMax);
		}
	}

	public string Armour
	{
		get{
			return GameData.PowerString(_Armour);
		}
	}

	public string Attack
	{
		get{
			return GameData.PowerString(_Attack);
		}
	}

	public int _Health = 0, _HealthMax = 0;
	public int _Armour = 0;
	private int _ArmourMax = 999999;
	public float ArmourReductionRate = 0.0F;
	public StatContainer _Strength;
	public StatContainer _Dexterity;
	public StatContainer _Charisma;
	public StatContainer _Wisdom;

	public StatContainer this[int value]
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
			}
			return null;
		}

	}
	
	public ShiftType Shift;
	public Vector2 MapSize = new Vector2(0,0);

	public int TurnsPerDifficulty = 0;
	public int TurnsToWave = 0;

	public int ComboCounter = 0;
	public float ComboBonus = 0.0F;
	public int MatchNumberModifier = 0;

	public int _Attack = 0;
	public float AttackRate = 0;

	public int MagicPower = 0;

	public int Regen = 0;
	public int Leech = 0;
	public int Spikes = 0;
	public int Poison = 0;

	public int PoisonTime = 0;

	public float CooldownDecrease = 0.0F;
	public float CostDecrease = 0.0F;
	public int BoonIncrease = 0;

	public int Presence = 0;

	public float OverflowMulti = 0.0F;//0.4F;
	public float AllColourMulti = 0.0F;//1.2F;

	public int MeterMax = 0;
	public int ManaRegen = 0;

	[HideInInspector]
	public bool isKilled;

	public int AbilitySlots =0;
	public int PrevTurnKills = 0;
	public int HealThisTurn = 0, DmgThisTurn = 0;
	

	public List<TileChance> TileChances = new List<TileChance>();

	public int Strength {get{return _Strength.StatCurrent;}set{_Strength.StatCurrent = value;}} 
	public int Dexterity {get{return _Dexterity.StatCurrent;}set{_Dexterity.StatCurrent = value;}}
	public int Wisdom {get{return _Wisdom.StatCurrent;}set{_Wisdom.StatCurrent = value;}}
	public int Charisma {get{return _Charisma.StatCurrent;}set{_Charisma.StatCurrent = value;}}

	[HideInInspector]
	public List<StatContainer> AllStats;
	public StatContainer STR{get{return _Strength;}}
	public StatContainer DEX{get{return _Dexterity;}}
	public StatContainer WIS{get{return _Wisdom;}}
	public StatContainer CHA{get{return _Charisma;}}

	public StatContainer Red{get{return _Strength;}}
	public StatContainer Blue{get{return _Dexterity;}}
	public StatContainer Green{get{return _Wisdom;}}
	public StatContainer Yellow{get{return _Charisma;}}

	
	private Stat() {}
	public Stat (Stat prev = null, bool GENUSs = true) : base()
	{
		Shift = ShiftType.Down;
		if(prev != null)
		{
			SetStats(prev, GENUSs);
		}
		else 
		{
			_Strength          = new StatContainer();
			_Dexterity         = new StatContainer();
			_Wisdom            = new StatContainer();
			_Charisma          = new StatContainer();

		}
		AllStats = new List<StatContainer>();
		AllStats.Add(_Strength);
		AllStats.Add(_Dexterity);
		AllStats.Add(_Wisdom);
		AllStats.Add(_Charisma);
	}

	public void SetStats(Stat prev, bool GENUSs = true)
	{
		//if(GENUSs)
		//{
			Stat_HealthInc = prev.Stat_HealthInc;
			Stat_CoolInc = prev.Stat_CoolInc;
			Stat_AtkInc = prev.Stat_AtkInc;
			Stat_BoonInc = prev.Stat_BoonInc;
		//}

		_Strength          = new StatContainer(prev._Strength, GENUSs);
		_Dexterity         = new StatContainer(prev._Dexterity, GENUSs);
		_Wisdom            = new StatContainer(prev._Wisdom, GENUSs);
		_Charisma          = new StatContainer(prev._Charisma, GENUSs);

		Class_Type 		  = prev.Class_Type;
		Shift        = prev.Shift;
		MapSize 		  = prev.MapSize;

		TurnsPerDifficulty  = prev.TurnsPerDifficulty;
		TurnsToWave  		= prev.TurnsToWave;
		
		ComboCounter     = prev.ComboCounter;
		ComboBonus       = prev.ComboBonus;
		_Attack           = prev._Attack;
		AttackRate       = prev.AttackRate;
		Regen            = prev.Regen;
		Leech            = prev.Leech;
		
		Spikes           = prev.Spikes;
		Poison           = prev.Poison;
		CooldownDecrease = prev.CooldownDecrease;
		CostDecrease     = prev.CostDecrease;
		BoonIncrease     = prev.BoonIncrease;
		Presence         = prev.Presence;
		
		_HealthMax        = prev._HealthMax;
		MeterMax 		 = prev.MeterMax;
		ManaRegen 		= prev.ManaRegen;

		_Armour = prev._Armour;
		ArmourReductionRate = prev.ArmourReductionRate;
		
		OverflowMulti    = prev.OverflowMulti;
		AllColourMulti   = prev.AllColourMulti;

		AllStats = new List<StatContainer>();
		AllStats.Add(_Strength);
		AllStats.Add(_Dexterity);
		AllStats.Add(_Charisma);
		AllStats.Add(_Wisdom);
		TileChances.Clear();
		TileChances.AddRange(prev.TileChances);

		MatchNumberModifier = prev.MatchNumberModifier;

		//HealThisTurn = prev.HealThisTurn;
		//DmgThisTurn = prev.DmgThisTurn;
	}

	public void AddStats(Stat other)
	{
		Stat_HealthInc += other.Stat_HealthInc;
		Stat_CoolInc += other.Stat_CoolInc;
		Stat_AtkInc += other.Stat_AtkInc;
		Stat_BoonInc += other.Stat_BoonInc;

		_Strength.AddValues(other._Strength);
		_Dexterity.AddValues(other._Dexterity);
		_Wisdom.AddValues(other._Wisdom);
		_Charisma.AddValues(other._Charisma);

		ComboCounter      += other.ComboCounter;
		ComboBonus        += other.ComboBonus;
		MatchNumberModifier += other.MatchNumberModifier;

		_Attack            += other._Attack;
		AttackRate        += other.AttackRate;
		Regen 			  += other.Regen;
		Leech 			  += other.Leech;
		_Health 			  += other._Health;
		_HealthMax 		  += other._HealthMax;
		MeterMax 		  += other.MeterMax;
		ManaRegen 		+= other.ManaRegen;

		ArmourReductionRate += other.ArmourReductionRate;

		Spikes += other.Spikes;
		Poison += other.Poison;
		CooldownDecrease += other.CooldownDecrease;
		CostDecrease += other.CostDecrease;
		BoonIncrease += other.BoonIncrease;
		Presence += other.Presence;

		MapSize += other.MapSize;

		TileChances.AddRange(other.TileChances);
		OverflowMulti += other.OverflowMulti;
		AllColourMulti += other.AllColourMulti;

		//HealThisTurn += other.HealThisTurn;
		//DmgThisTurn += other.DmgThisTurn;
	}	

	public float Stat_HealthInc = 0, Stat_CoolInc = 0, Stat_AtkInc = 0, Stat_BoonInc = 0;
	public void ApplyStatInc()
	{
		BoonIncrease       += (int) Stat_BoonInc;
		_Attack           += (int)Stat_AtkInc;
		CooldownDecrease += Stat_CoolInc;
		_HealthMax        += (int)Stat_HealthInc;
	}

	public void CheckStatInc() {
		Stat_HealthInc = (float)Strength;
		Stat_CoolInc = (float)Wisdom / 300.0F;
		Stat_AtkInc = (float)Dexterity / 20.0F;
		Stat_BoonInc = (float)Charisma/30.0F;
	}
	
	public int GetGENUSStat(GENUS ab) {
		switch(ab)
		{
			case GENUS.STR:
			return Strength;
			case GENUS.DEX:
			return Dexterity;
			case GENUS.WIS:
			return Wisdom;
			case GENUS.CHA:
			return Charisma;
		}
		return 1;
	}

	public StatContainer GetResourceFromGENUS(GENUS ab)
	{
		switch(ab)
		{
			case GENUS.STR:
			return Red;
			case GENUS.DEX:
			return Blue;
			case GENUS.WIS:
			return Green;
			case GENUS.CHA:
			return Yellow;
		}
		return null;
	}

	public StatContainer GetResourceFromIndex(int index)
	{
		switch(index)
		{
			case 0:
			return Red;
			case 1:
			return Blue;
			case 2:
			return Green;
			case 3:
			return Yellow;
		}
		return null;
	}

	public StatContainer GetResourceFromName(string s)
	{
		if(s == "Red" || s == "Strength") return Red;
		if(s == "Blue" || s == "Dexterity") return Blue;
		if(s == "Yellow" || s == "Charisma") return Yellow;
		if(s == "Green" || s == "Wisdom") return Green;
		return null;
	}

	public void SetGENUSStat(GENUS ab, int num)
	{
		switch(ab)
		{
			case GENUS.STR:
			Strength = num;
			break;
			case GENUS.DEX:
			Dexterity = num;
			break;
			case GENUS.WIS:
			Wisdom = num;
			break;
			case GENUS.CHA:
			Charisma = num;
			break;
		}
	}

	public void Heal(int heal)
	{
		int healperc = (int) (_HealthMax * ((float)heal/100.0F));
		HealThisTurn = Mathf.Clamp(HealThisTurn + healperc, 0, 1000000);
		_Health = Mathf.Clamp(_Health + healperc, 0, _HealthMax);
	}

	public void AddArmour(int _armour)
	{
		_Armour = Mathf.Clamp(_Armour + _armour,0, 99999999);
		//UIManager.instance.UpdatePlayerUI();
	}

	public void Hit(int TurnHit, params Tile[] attackers)
	{
		GameData.Log("Incoming " + TurnHit + " damage");

		foreach(Ability child in Player.Abilities)
		{
			if(child == null) continue;
		 	TurnHit = child.OnHit(TurnHit, attackers);
		}

		float hit = (float) TurnHit;
		float armour = (float) _Armour;

		float armour_res = armour/(armour + 100 * hit);
		float final_hit = hit - (float) ( hit * armour_res);

		//Debug.Log(hit + " : " + final_hit + " -- " + armour_res);
		float armour_reduction = 0.0F;
		int total_attack = 0;

		if(attackers != null)
		{	
			foreach(Tile child in attackers)
			{
				if(Spikes > 0)
				{
					UIManager.instance.MiniAlert(child.transform.position, "SPIKES \n" + Spikes);
					child.InitStats.TurnDamage += Spikes;
					if(child.Match(0))
					{
						GameData.Log("Spiked enemy to death");
					}
				}
				total_attack += child.Stats.Attack;
				armour_reduction += (child.Stats.Attack * ArmourReductionRate)/100;
			}
		}

	//Armour reduction is calculated after other reductions... could be good to keep the game harder?
		//Debug.Log("ARMOUR BLOCK %: " + armour_res + ":" + final_hit + " - " + TurnHit +  "(DECAY: " + armour_reduction + ":" + total_attack + ")");
		if(_Armour > 0) _Armour = (int)Mathf.Clamp(_Armour - 1, 0, _ArmourMax);
		//if(_Armour > 0) _Armour = (int)Mathf.Clamp(_Armour - ((float)_Armour * armour_reduction), 0, _ArmourMax);

		if(final_hit <= 0)
		{
			final_hit = 1;
			//return;
		}
		_Health -= (int) final_hit;
		_Health = Mathf.Clamp(_Health, 0, _HealthMax);
		DmgThisTurn += (int) final_hit;

	}

	public float GetHealthRatio()
	{
		return (float)_Health / (float)_HealthMax;
	}


	public void CompleteRegen()
	{
		//Debug.Log(Regen);
		GameData.Log("Regen'd " + Regen + " health");
		HealThisTurn += Regen;
		foreach(StatContainer child in AllStats)
		{
			child.Regen();
		}
		
	}

	public void CompleteLeech(int num)
	{ 
		//Debug.Log(num + ":" + num * Leech);
		if(num == 0) return;
		GameData.Log("Leeched " + num * Leech + " health");

		HealThisTurn += (Leech * num);
		foreach(StatContainer child in AllStats)
		{
			child.Leech(num);
		}
	}

	public void CompleteHealth()
	{
		HealThisTurn = 0;
		DmgThisTurn = 0;
		int healperc = (int) (_HealthMax * ((float)HealThisTurn/100.0F));
		//Debug.Log("HEAL PERCENT " + healperc + ":" + HealThisTurn);

		int total = healperc - DmgThisTurn;
		int newhealth = Mathf.Clamp((_Health + healperc) - DmgThisTurn, 0, _HealthMax);

		if(total != 0 && newhealth != _Health)
		{
			Vector3 tpos = Vector3.right * 2.5F;
			UIManager.instance.MiniAlert(
				UIManager.instance.Health.transform.position + tpos, 
				(total > 0 ? "+":"") + total, 38, 
				total > 0 ? GameData.instance.GoodColour : GameData.instance.BadColour, 1.7F,
				total > 0 ? 0.01F: -0.01F);
		}

		_Health = newhealth;

		HealThisTurn = 0;
		DmgThisTurn = 0;
	}

	public void Setup()
	{
		_Strength.Setup();
		_Dexterity.Setup();
		_Wisdom.Setup();
		_Charisma.Setup();
	}

	public void LevelUp()
	{
		_Strength.LevelUp();
		_Dexterity.LevelUp();
		_Wisdom.LevelUp();
		_Charisma.LevelUp();
	}

	public string GetStatIncBonusString(GENUS type)
	{
		switch(type)
		{
			case GENUS.STR:
			return "+" + (int)Stat_HealthInc + " HP";
			case GENUS.DEX:
			return "+" + (int)Stat_AtkInc + " ATK";
			case GENUS.WIS:
			return "+" + (Stat_CoolInc*100).ToString("0") + "% CD";
			case GENUS.CHA:
			return "+" + Stat_BoonInc.ToString("0.0") + " COMBO";
		}
		return "ERROR";
	}


}

[System.Serializable]
public class StatContainer
{
	public int StatCurrent = 10;
	public int StatGain = 1;

	public int StatLeech = 0;
	public int StatRegen = 0;

	//public int ResCurrent = 0;
	//public int ResMax = 20;
	public float ResMultiplier = 1.0F;
	//public float ResGain = 1.05F;
	public int ResLeech = 0;
	public int ResRegen = 0;

	public int ThisTurn = 0;

	private int ToMult = 65;
	private int ToMeter = 8;
	public int MeterInc = 0;

	//public float ResMax_soft;

	public StatContainer(StatContainer prev = null, bool mult = false)
	{
		ThisTurn = 0;
		if(prev != null)
		{
			StatCurrent = prev.StatCurrent;
			StatGain = prev.StatGain;

			StatLeech = prev.StatLeech;
			StatRegen = prev.StatRegen;

			ResMultiplier = prev.ResMultiplier;
			MeterInc = prev.MeterInc;

			if(mult)
			{
				//MeterInc    += prev.StatCurrent / ToMeter;
				//ResMultiplier  += (float)(prev.StatCurrent) / ToMult;
			}
		}
		else 
		{
			StatCurrent = 0;
			StatGain = 0;
			StatLeech = 0;
			StatRegen = 0;
			MeterInc = 0;
			ResMultiplier = 0;
		}	
	}

	public void AddValues(StatContainer prev)
	{
		StatCurrent += prev.StatCurrent;
		StatGain += prev.StatGain;

		StatLeech += prev.StatLeech;
		StatRegen += prev.StatRegen;

	}

	public void LevelUp()
	{
		StatCurrent += StatGain;

		MeterInc = StatCurrent / ToMeter;
		ResMultiplier =  1.0F + (float)StatCurrent/(float)ToMult;
	}

	public void Setup()
	{
		ToMeter = 8;
		ToMult = 65;

		MeterInc = StatCurrent / ToMeter;
		ResMultiplier = 1.0F +(float)StatCurrent/(float)ToMult;
	}

	public void Regen()
	{
		StatCurrent += StatRegen;
		ThisTurn += ResRegen;

		//SHOW ALERT
	}

	public void Leech(int amt)
	{
		StatCurrent += amt * StatLeech;
		//ResCurrent = Mathf.Clamp(ResCurrent + (ResLeech * amt), 0, ResMax);

		//SHOW ALERT
	}
}

[System.Serializable]
public class TileChance
{
	public string Genus = "", Type = "";
	public float Chance = 0.0F;
	public int Value = 0;

	public TileChance(string g, string t, float c, int v = 0)
	{
		Genus = g;
		Type = t;
		Chance = c;
		Value = v;
	}

	public TileChance()
	{
		
	}
}