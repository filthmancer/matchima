using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicTile : Ability {

	public int numberToChange;
	public int incValue = 5;

	public string StartType
	{get{return StartGenus + " " + StartSpecies;}}
	public string EndType
	{get{return EndGenus + " " + EndSpecies;}}

	public string StartGenus, StartSpecies;
	public string EndGenus, EndSpecies;

	private int _defaultNumberToChange;
	private int _upgradeNumberToChange;

	public Ability_UpgradeInfo NumberUpgrade;

	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(NumberUpgrade.Info, GameData.Colour(GENUS.WIS))};
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Changes " + numberToChange + " ", Color.white, false));
			All.Add(new StCon(StartType, Color.white, false));
			All.Add(new StCon(" to ", Color.white, false));
			All.Add(new StCon(EndType, Color.white, false));
			return All.ToArray();
		}
	}

	public override void Start()
	{
		base.Start();
		_defaultNumberToChange = numberToChange;
		Description_Basic = "Changes " + numberToChange + " " + StartType + " to " + EndType;
	}

	public override void Update()
	{
		base.Update();
		numberToChange = _defaultNumberToChange + _upgradeNumberToChange + (int) StatBonus();
		Description_Basic = "Changes " + numberToChange + " " + StartType + " to " + EndType;
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 2") + "\n";
				s+= UpgradeInfoColoured(2, "Inc. number of changed tiles by 1") + "\n";
				s+= UpgradeInfoColoured(3,  "Inc. value of changed tiles by 3") + "\n";
				return s;
		}
	}

	bool UPGRADEOLD2()
	{
		if(UpgradeLevel >= 4) return false;
		UpgradeLevel ++;
		switch(UpgradeLevel)
		{
			case 2:
			_defaultCooldown -= 2;
			if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
			break;
			case 3:
			_upgradeNumberToChange = 1;
			break;
			case 4:
			incValue += 3;
			break;
		}
		return true;
	}


	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;

		List<Tile> tilesCanChange = new List<Tile>();

		Tile [,] _tiles = TileMaster.Tiles;

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				if(_tiles[x,y].IsType(StartGenus, StartSpecies))
				{
					tilesCanChange.Add(_tiles[x,y]);
				}
			}
		}

		for(int i = 0; i < numberToChange; i++)
		{
			if(tilesCanChange.Count == 0) break;

			int rand = Random.Range(0, tilesCanChange.Count);
			int [] num = tilesCanChange[rand].Point.Base;

			Destroy(tilesCanChange[rand].gameObject);

			TileMaster.instance.ReplaceTile(num[0],num[1], TileMaster.Types[EndSpecies], TileMaster.Genus[EndGenus]);
			_tiles[num[0],num[1]].Stats.Value = incValue;
			tilesCanChange.RemoveAt(rand);
		}
		activated = false;
		cooldown_time = cooldown;
	}

	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);

		MagicTile magic = (MagicTile) new_ab;
		StartGenus = magic.StartGenus;
		StartSpecies = magic.StartSpecies;

		EndGenus = magic.EndGenus;
		EndSpecies = magic.EndSpecies;
		
		numberToChange = magic.numberToChange;
		incValue = magic.incValue;

		Start();
	}

	public void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
			EndGenus = GameData.ResourceLong(Parent.Genus);
		}
	}

	//Input
	//Arg 0: Title
	//Arg 1: Chance
	//Arg 2: GENUS
	//Arg 3: In Genus
	//Arg 4: In Species
	//Arg 5: Number To Change
	//Arg 6: New Value

	//Output
	//Arg 0: Title
	//Arg 1: Chance
	//Arg 2: Out Genus
	//Arg 3: Out Species
	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);

		_input = null;
		if(_in.HasValue)
		{
			_input = con.Input[(int)_in];
		} 
		else
		{
			_input = GetContainerData(con);
		}

		_output = null;
		if(_out.HasValue)
		{
			_output = con.Output[(int)_out];
		} 
		else
		{
			_output = GetOutputData(con);
		}

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		numberToChange = GameData.StringToInt(_input.args[5]);
		incValue = GameData.StringToInt(_input.args[6]);

		StartGenus = _input.args[3];
		StartSpecies = _input.args[4];
		EndGenus = _output.args[3];
		EndSpecies = _output.args[4];

		NumberUpgrade = new Ability_UpgradeInfo(0, 1, "+", " tiles", Color.green, () => {_upgradeNumberToChange += 1;});
		Upgrades.Add(NumberUpgrade);

	}
}