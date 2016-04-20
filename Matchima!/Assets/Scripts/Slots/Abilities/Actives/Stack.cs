using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack : Ability {

	Ability_UpgradeInfo ValueUp, ExtraTurns;
	public int upgrade_extra_turns = 0;
	public int upgrade_value = 0;
	int extra_turns;
	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(ValueUp.Info, GameData.Colour(GENUS.WIS)),
								new StCon(ExtraTurns.Info, GameData.Colour(GENUS.WIS))};
		}
	}


	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("New tiles will be ", Color.white, false));
			All.Add(new StCon(tileType, Color.white));
			return All.ToArray();
		}
	}

	public string genus, species;
	public string tileType
	{
		get{return genus + " " + species;}
	}
	private float upgrade_ignore_cd = 0.0F;

	public override void Start()
	{
		base.Start();
		Description_Basic = "New tiles will be " + tileType + " tiles";
		BeforeMatchEffect = true;
	}


	public override void Update()
	{
		base.Update();
		Description_Basic = "New tiles will be " + tileType + " tiles";
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;
		cooldown_time = cooldown;
	}

	public override IEnumerator BeforeMatch(List<Tile> tiles)
	{
		if(activated)
		{
			for(int i = 0; i < tiles.Count; i++)
			{
				TileMaster.instance.QueueTile(TileMaster.Types[species], TileMaster.Genus[genus], upgrade_value);
			}
		}
		yield break;
	}

	public override void AfterTurnA()
	{
		base.AfterTurnA();
		if(activated)
		{
			if(extra_turns > 0)
			{
				extra_turns -= 1;
			}
			else 
			{
				activated = false;
				extra_turns = upgrade_extra_turns;
				if(Random.value > upgrade_ignore_cd) cooldown_time = cooldown;
			}
		}
	}

	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Stack stack = (Stack) new_ab;
		genus = stack.genus;
		species = stack.species;
		Start();
	}

	protected override void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
			genus = GameData.ResourceLong(Parent.Genus);
		}
	}

	//Input
	//Args 0: Title
	//Args 1: Chance
	//Args 2: GENUS
	//Args 3: Genus
	//Args 4: Species
	public override void Setup(AbilityContainer con, int? _in, int? _out)
	{
		base.Setup(con, _in, _out);
		string suffix = "S";
		

		_input = null;
		if(_in.HasValue)
		{
			_input = con.Input[(int)_in];
		} 
		else
		{
			_input = GetContainerData(con);
		}


		genus = _input.args[3];
		species = _input.args[4];

		suffix = _input.args[0];
		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		name = suffix;

		ExtraTurns = new Ability_UpgradeInfo(0,1, "", "extra turns", Color.green, () => {upgrade_extra_turns += 1;});
		Upgrades.Add(ExtraTurns);
		ValueUp = new Ability_UpgradeInfo(0, 1, "+", " Value", Color.green, () => {upgrade_value += 1;});
		Upgrades.Add(ValueUp);
	}

}