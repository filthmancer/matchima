using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Swap : Ability {

	public string StartType
	{get{return StartGenus + " " + StartSpecies;}} 
	public string EndType
	{get{return EndGenus + " " + EndSpecies;}} 

	public string StartGenus, StartSpecies;
	public string EndGenus, EndSpecies;

	public int upgrade_value_inc = 0;

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
			All.Add(new StCon("Changes ", Color.white, false));
			All.Add(new StCon(StartType, Color.white, false));
			All.Add(new StCon(" to ", Color.white, false));
			All.Add(new StCon(EndType, Color.white));
			return All.ToArray();
		}
	}


	public override void Start()
	{
		base.Start();
		if(StartType == string.Empty || EndType == string.Empty) 
		{ 
			Debug.LogError("Swap skill parameters not loaded!");
		 	return;
		}
		Description_Basic = "Changes " + StartType + " tiles to " + EndType + " tiles";
	}


	public override void Update()
	{
		base.Update();
		Description_Basic = "Changes " + StartType + " tiles to " + EndType + " tiles";
	}

	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		
		activated = true;
		Tile [,] _tiles = TileMaster.Tiles;
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				if(_tiles[x,y] == null) continue;
				SPECIES sp = null;
				GENUS g = TileMaster.Types.GENUSOf(EndGenus);
				if(_tiles[x,y].IsType(StartGenus, StartSpecies)) 
				{

					int old_value = _tiles[x,y].Stats.Value;
					int final_value = old_value + upgrade_value_inc + (int) StatBonus();

					if(StartSpecies != string.Empty) sp = _tiles[x,y].Type;
					if(EndSpecies != string.Empty) sp = TileMaster.Types[EndSpecies];

					if(sp == null)
					{
						_tiles[x,y].ChangeGenus(g);
						EffectManager.instance.PlayEffect(_tiles[x,y].transform, Effect.Replace, "", GameData.instance.GetGENUSColour(_tiles[x,y].Genus));
					} 
					else
					{
						TileMaster.instance.ReplaceTile(x,y, sp, g);
						_tiles[x,y].InitStats.Value = final_value;
					}
				}
			}
		}
		cooldown_time = cooldown;
		activated = false;
	}

	protected override void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
		 	EndGenus = GameData.ResourceLong(Parent.Genus);

		}
		if(StartGenus == "Random")
	 	{
	 		GENUS g = (GENUS) Random.Range(0,4);
	 		StartGenus = GameData.ResourceLong(g);
	 		while(StartGenus == EndGenus)
	 		{
	 			g = (GENUS) Random.Range(0,4);
	 			StartGenus = GameData.ResourceLong(g);
	 		}
	 	}
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Swap new_swap = (Swap) new_ab;
		StartGenus = new_swap.StartGenus;
		StartSpecies = new_swap.StartSpecies;
		EndGenus = new_swap.EndGenus;
		EndSpecies = new_swap.EndSpecies;
		Start();
	}

	//Input
	//Arg 0: Title
	//Arg 1: Chance
	//Arg 2: Genus Type
	//Arg 3: Species Type

	//Output
	//Arg 0: Title
	//Arg 1: Chance
	//Arg 2: GENUS
	//Arg 3: Genus Type
	//Arg 4: Species Type

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		string prefix = "", suffix = "";
		
		_input = null;
		_output = null;
		if(_in.HasValue) _input = con.Input[(int)_in];
		else _input = GetContainerData(con);

		if(_out.HasValue) _output = con.Output[(int)_out];
		else _output = GetOutputData(con);

		StartGenus = _input.args[2];
		StartSpecies = _input.args[3];
		
		StartGenus = _input.args[2];
		StartSpecies = _input.args[3];
		
		EndGenus = StartGenus;
		EndSpecies = StartSpecies;

		while(EndGenus == StartGenus && EndSpecies == StartSpecies)
		{
			_output = GetOutputData(con);

			GENUS = (GENUS)GameData.StringToInt(_output.args[2]);
			EndGenus = _output.args[3];
			EndSpecies = _output.args[4];
		}

		prefix = _output.args[0];
		suffix = _input.args[0];


		if(prefix != "" && suffix != "") name = prefix + " " + suffix;
		ValueMult = new Ability_UpgradeInfo(0, 10, "+", " Value", Color.green, () => {upgrade_value_inc += 1;});
		Upgrades.Add(ValueMult);
	}
}