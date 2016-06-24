using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ModType
{
	Boon,
	Curse
}
public class Ability : Slot {

	public int Damage = 0;
	protected int final_damage = 0;
	public int _Level = 1;

	public ModType Type;
	Ability_UpgradeInfo CostUp, CoolUp;

	public UIObj MinigameObj;

	public override StCon [] BaseDescription
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon(Description_Basic, Color.white));
			All.AddRange(CoolDesc);
			All.AddRange(CostDesc);
			return All.ToArray();
		}
	}


	public StCon [] CostDesc
	{
		get{
			List<StCon> All = new List<StCon>();
			for(int i = 0; i < cost.Length; i++)
			{
				if(cost[i] <= 0) continue;
				string s = "Costs " + cost[i] + " " + GameData.Resource((GENUS)i);
				if(Player.Stats.CostDecrease > 0)
				s += "(-" + (int)(Player.Stats.CostDecrease * 100) + "%)";
				if(CostUp != null) s += CostUp.Info;

				StCon Base = new StCon(s, GameData.Colour((GENUS)i));
				All.Add(Base);
			}
			return All.ToArray();
		}
	}

	public StCon [] CoolDesc{
		get
		{
			List<StCon> All = new List<StCon>();
			if(cooldown <= 0) return All.ToArray();
			string s = "Cooldown " + cooldown;
			if(Player.Stats.CooldownDecrease > 0)
			s += " (-" + (int)(Player.Stats.CooldownDecrease * 100) + "%)";
			if(CoolUp != null) s += " " + CoolUp.Info;

			All.Add(new StCon(s, GameData.Colour(GENUS.PRP)));
			return All.ToArray();
		}
		
	}

	public override string Level
	{
		get{return "Lvl " + _Level;}
	}


	public void Upgrade()
	{
		UpgradeLevel ++;
		int rand = UnityEngine.Random.Range(0, Upgrades.Count);
		Upgrades[rand].Upgrade();
	}
	
	public List<Ability_UpgradeInfo> Upgrades = new List<Ability_UpgradeInfo>();
	public int UpgradeLevel = 1;
	public int UpgradeCost
	{
		get{
			return UpgradeLevel * 25;
		}
	}
	public string UpgradeInfo
	{
		get
		{
			string s = "\n\n";
			for(int i = 2; i < Upgrades.Count; i++)
			{
				if(Upgrades[i].Level > 0) s += Upgrades[i].Info + "\n";
			}
			return s;
		}
	}

	public bool RelativeStats;
	public bool passive;

	
	[HideInInspector]
	public GENUS GENUS;
	[HideInInspector]
	public float GENUSMultiplier = 1.0F;


	public int [] cost = new int[4];

	[HideInInspector]
	public int _defaultCooldown;
	[HideInInspector]
	public int [] _defaultCost = new int[4];
	[HideInInspector]
	public float _cooldownModifier = 1.0F, _costModifier = 1.0F;
	[HideInInspector]
	public int addedCooldown = 0;
	[HideInInspector]
	public int [] addedCost = new int[4];
	[HideInInspector]
	public float addedCooldownPercent;
	[HideInInspector]
	public float [] addedCostPercent = new float[4];

	public GameObject Particle;

	public override bool IsDraggable
	{
		get{
			return Drag != DragType.None && CanAfford();
		}
	}

	[HideInInspector]
	public ContainerData _input, _output;



	public virtual float StrengthFactor	 	{get{return 1 + (float)Parent.Stats.Strength / 100.0F;}}
	public virtual float DexterityFactor	{get{return 1 + (float)Parent.Stats.Dexterity / 100.0F;}}
	public virtual float WisdomFactor		{get{return 1 + (float)Parent.Stats.Wisdom / 100.0F;}}
	public virtual float CharismaFactor		{get{return 1 + (float)Parent.Stats.Charisma / 100.0F;}}
	public virtual int MagicFactor			{get{return Parent.Stats.MagicPower;}}

	// Use this for initialization
	public override void Start () {
		if(UpgradeLevel > 1)
		{
			int num = UpgradeLevel-1;
			UpgradeLevel = 1;
			for(int i = 0; i < num; i++)
			{
				Upgrade();
			}
		}
		Drag = DragType.Cast;
	}
	
	// Update is called once per frame
	public override void Update () {
		if(!initialized) return;
		if(cooldown_time < 0) cooldown_time = 0;

		float final_cooldown = 1.0F * _cooldownModifier;
		final_cooldown *= (1.0F - Player.Stats.CooldownDecrease);
		float final_cost = 1.0F * _costModifier;
		final_cost *= (1.0F - Player.Stats.CostDecrease);

		
		if(_defaultCooldown > 0)
		{
			cooldown = (int)(_defaultCooldown * final_cooldown);
			cooldown += (int) (cooldown * addedCooldownPercent);
			cooldown += addedCooldown;
			if(cooldown < 1) cooldown = 1;

		}

		for(int i = 0; i < _defaultCost.Length; i++)
		{
			cost[i] = (int) (_defaultCost[i] * final_cost);
			cost[i] += (int)(cost[i] * addedCostPercent[i]);
			cost[i] += addedCost[i];
			if(cost[i] < 0) cost[i] = 0;
		}
		
	}


	public override IEnumerator AfterTurn()
	{
		addedCooldownPercent = 0.0F;
		addedCostPercent = new float [4];
		addedCost = new int[4];
		addedCooldown = 0;
		if(passive) yield break;
		if(cooldown_time > 0) cooldown_time -= 1;
		yield return null;
	}



	public virtual void OnTileCollect(Tile t)
	{

	}
	public virtual void OnTileDestroy(Tile t)
	{

	}

	public virtual Bonus CheckBonus(GENUS g)
	{
		return null;
	}

	public virtual void Destroy()
	{
		Destroy(this.gameObject);
	}

	public float StatBonus()
	{
		if(GENUSMultiplier == 1.0F) return 0.0F;
		else return (Player.Stats.GetGENUSStat(GENUS) / GENUSMultiplier);
	}


	public override void Activate()
	{
		activated = true;	
	}

	public virtual void SetStatsRandom()
	{
		
	}

	protected IEnumerator Shift()
	{
		yield return new WaitForSeconds(Time.deltaTime * 5);
		//TileMaster.instance.StartShiftingTiles(Player.Stats.Shift);
		cooldown_time = cooldown;
		activated = false;
	}


	public virtual void Setup(Ability new_ab)
	{
		Name_Basic = new_ab.Name_Basic;
		_defaultCooldown = new_ab.cooldown;
		_defaultCost = new_ab._defaultCost;

		GENUS = new_ab.GENUS;
		Colour = GameData.Colour(GENUS);
		GENUSMultiplier = new_ab.GENUSMultiplier;
		Upgrades = new_ab.Upgrades;

		initialized = true;
	}

	public override void Init(int i, int lvl = 1)
	{
		_Level = lvl;
		Setup();
		Index = i;
	}

	protected override void Setup()
	{
		if(cost == null) 
		{
			cost = new int [4];
			_defaultCost = new int [4];
		}
		//_defaultCost = cost;
		_defaultCooldown = cooldown;
		if(RelativeStats)
		{
			if(Parent != null) GENUS = Parent.Genus;
		}
		Sprite s = GameData.instance.GetIconByName(IconString);
		if(s!= null) Icon = s;
		Colour = GameData.Colour(GENUS);
		initialized = true;
	}

	

	public virtual void Setup(AbilityContainer con, int? inp = null, int? outp = null)
	{
		Name_Basic = con.Name;
		_defaultCooldown = UnityEngine.Random.Range(con.CooldownMin, con.CooldownMax);
			

		_defaultCost = new int[4];
		cost = new int[4];
		//if(UnityEngine.Random.value < con.CostChance)
//		{
//			_defaultCost[(int)CostType] = UnityEngine.Random.Range(con.CostMin, con.CostMax);
//			cost[(int)CostType] = _defaultCost[(int)CostType];
//			if(con.CostReducesCooldown) _defaultCooldown -= (_defaultCost[(int)CostType]/4);
//		}
		//else _defaultCost = 0;

		

		if(con.StatType != "Input" && con.StatType != "Output")	GENUS = GameData.StringToGENUS(con.StatType);
		GENUSMultiplier = con.StatMultiplier;

		Description_Basic = con.Description;
		cooldown = _defaultCooldown;
		Icon = GameData.instance.GetIconByName(con.Icon);

		CoolUp = new Ability_UpgradeInfo(0, 5, "(-", "%)", 
											Color.green, () => {_cooldownModifier = Mathf.Clamp(_cooldownModifier - 0.05F, 0.0F, 5.0F);});
		Upgrades.Add(CoolUp);

		//if(_defaultCost[(int)CostType] > 0) 
//		{
//			CostUp = new Ability_UpgradeInfo(0, 5, "(-", "% cost)", 
//												Color.green, () => {_costModifier = Mathf.Clamp(_costModifier - 0.05F, 0.0F, 5.0F);});
//			Upgrades.Add(CostUp);
//		}
		
	}


	protected bool CanAfford()
	{
		for(int i = 0; i < cost.Length; i++)
		{
			if(cost[i] == 0) continue;
			if(Player.Classes[i].Meter >= cost[i])
			{
				Player.Classes[i].Meter -= cost[i];
			}
			else
			{
				UIManager.Objects.StartWarning();
				//StartCoroutine(UIManager.Objects.CostWarning());//Debug.Log("Cant Afford!");
				return false;
			}
		}
		
		return true;
	}

	public ContainerData GetContainerData(AbilityContainer con)
	{
		ContainerData con_data = null;
		float allchance = 0.0F;
		foreach(ContainerData child in con.Input)
		{
			float c = GameData.StringToFloat(child.args[1]);
			allchance += c;
		}
		float value = UnityEngine.Random.value * allchance;
		float current_value = 0.0F;
		for(int i = 0; i < con.Input.Length; i++)
		{
			float c = GameData.StringToFloat(con.Input[i].args[1]);
			if(value > current_value && value < current_value + c)
			{
				con_data = con.Input[i];
				break;
			}
			current_value += c;
		}
		if(con_data == null) Debug.LogError("Could not load Ability Container Data!");
		return con_data;
	}

	public ContainerData GetOutputData(AbilityContainer con)
	{
		ContainerData con_data = null;
		float allchance = 0.0F;
		foreach(ContainerData child in con.Output)
		{
			float c = GameData.StringToFloat(child.args[1]);
			allchance += c;
		}
		float value = UnityEngine.Random.value * allchance;
		float current_value = 0.0F;
		for(int i = 0; i < con.Output.Length; i++)
		{
			float c = GameData.StringToFloat(con.Output[i].args[1]);
			if(value > current_value && value < current_value + c)
			{
				con_data = con.Output[i];
				break;
			}
			current_value += c;
		}
		if(con_data == null) Debug.LogError("Could not load Ability Container Data!");
		return con_data;
	}

	public string UpgradeInfoColoured(int lvl, string s)
	{
		string final = "Lvl " + (lvl+1) + ": ";
		if(UpgradeLevel > lvl) final += "<color=white>";
		else final += "<color=#808080ff>";
		final += s;
		
		final += "</color>";
		return final;
	}

	public void UpgradeCooldown()
	{
		_defaultCooldown -= 1;
	}

	public void UpgradeValue()
	{
		//_defaultCooldown -= 1;
	}

}


public class Ability_UpgradeInfo
{
	public string Info{
		get{
			if(Current == 0) return "";
			return Prefix + Current + Suffix;
		}
	}
	public string Suffix, Prefix;
	public int Level = 0;
	public int Current = 0;
	public int Rate = 0;
	public Action Up;
	public Color col;

	public Ability_UpgradeInfo(int cur, int r, string p, string s, Color c, Action method)
	{
		Current = cur;
		Rate = r;
		Prefix = p;
		Suffix = s;
		col = c;
		Up = method;
	}

	public void Upgrade()
	{
		Level ++;
		Current += Rate;
		Up();
	}
}

