using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

public class Collect : Ability {
	public string genus, species;
	public string tileType
	{
		get{return genus + " " + species;}
	}

	public float upgrade_value_mult = 1.0F;
	private float _finalMult
	{
		get{
			return (1 + StatBonus()) * upgrade_value_mult;
		}
	}


	Ability_UpgradeInfo ValueMult;
	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(ValueMult.Info, GameData.Colour(GENUS.WIS))};
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Collects ", Color.white, false));
			All.Add(new StCon(genus + " " + species, Color.white));
			return All.ToArray();
		}
	}


	public override void Start()
	{
		base.Start();
		Description_Basic = "Collects " + tileType + " tiles";
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Collects " + tileType + " tiles";

	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 3") + "\n";
			s+= UpgradeInfoColoured(2, "Inc. value multiplier by 1") + "\n";
			s+= UpgradeInfoColoured(3, "Dec. cooldown by 4") + "\n";
			return s;
		}
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;
		StartCoroutine(CollectRoutine());
	}

	public IEnumerator CollectRoutine()
	{
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		float part_time = 0.3F;

		int [] resource = new int [6];
		int [] heal = new int [6];
		int [] armour = new int [6];
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();
		List<GameObject> particles = new List<GameObject>();
		GENUS matchGENUS = TileMaster.Genus[genus];
		SPECIES spec = TileMaster.Types[species];
		if(matchGENUS == GENUS.NONE) matchGENUS = GENUS.ALL;

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				Tile tile = _tiles[x,y];
				if(tile == null) continue;
				if(tile.IsType(spec) && tile.IsGenus(matchGENUS, false)) 
				{
					to_collect.Add(tile);
				}
			}
		}
		

		foreach(Tile child in to_collect)
		{
			if(child == null || child.isMatching) continue;
			child.SetState(TileState.Selected, true);
			GameObject part = EffectManager.instance.PlayEffect(child.transform, Effect.Shiny, "", GameData.instance.GetGENUSColour(child.Genus));
			part.transform.parent = child.transform;
			yield return new WaitForSeconds(Time.deltaTime * 2);
		}

		yield return new WaitForSeconds(GameData.GameSpeed(part_time));
		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		
		if(Player.instance.CompleteMatch)
		{
			foreach(Tile child in to_collect)
			{
				int [] values = child.Stats.GetValues();
				int v = 1;
				if(child.Type.isEnemy)
				{
					v = Damage;
				} 
				
				if(child.Match(v))
				{
					resource[(int)child.Genus] += values[0];
					heal[(int)child.Genus]   += values[1];
					armour[(int)child.Genus]   += values[2];
				}
			}
		}
		
		yield return StartCoroutine(Player.instance.AfterMatch());

		List<Bonus> bonuses = new List<Bonus>();
		float final_mult = (1 + StatBonus()) * upgrade_value_mult;
		bonuses.Add(new Bonus(final_mult , "ABL", "Bonus from " + name + " ability", GameData.instance.GetGENUSColour(GENUS.PRP)));
		
		GameManager.instance.CollectResources(resource, heal, armour, bonuses.ToArray());
		
		cooldown_time = cooldown;
		activated = false;
		yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());
		TileMaster.instance.ResetTiles();
		TileMaster.instance.SetFillGrid(true);
		yield break;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Collect collect = (Collect) new_ab;
		genus = collect.genus;
		species = collect.species;

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
	//Arg 0: Title
	//Arg 1: Chance
	//Arg 2: GENUS
	//Arg 3: Genus Type
	//Arg 4: Species Type
	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		string prefix = "P", suffix = "S";

		_input = null;
		_output = null;
		if(_in.HasValue) _input = con.Input[(int)_in];
		else _input = GetContainerData(con);

		if(_out.HasValue) _output = con.Output[(int)_out];
		else _output = GetOutputData(con);

		genus = _input.args[3];
		species = _input.args[4];
		
		suffix = _input.args[0];
		prefix = _output.args[0];
		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);

		
		name = prefix + " " + suffix;

		ValueMult = new Ability_UpgradeInfo(0, 10, "+", "% Bonus Value", Color.green, () => {upgrade_value_mult += 0.1F;});
		Upgrades.Add(ValueMult);
	}
}