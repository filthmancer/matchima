using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplyEffectToTile : Ability {

	public List<string> args;
	public string EffectName;

	public string [] TileType				//Types of tile
	{
		get{
			string [] final = new string[TileGenus.Length];
			for(int i = 0; i < TileGenus.Length; i++)
			{
				final[i] = TileGenus[i] + " " + TileSpecies[i];
			}
			return final;
		}
	}

	public string AllTypes
	{
		get{
			string final = "";
			for(int i = 0; i < TileGenus.Length; i++)
			{
				final += TileGenus[i] + " " + TileSpecies[i];
			}
			return final;
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Applies " + EffectName + " to " + (ApplyToAll?"":""+TileNumber), Color.white, false));
			for(int i = 0; i < TileType.Length; i++)
			{
				All.Add(new StCon(TileType[i], Color.white, false));
			}	
			return All.ToArray();
		}
	}

	public string [] TileGenus;
	public string [] TileSpecies;
	public int TileNumber; 					//Number of tiles to apply the GENUS to
	public bool ApplyToAll;					//Apply to all instance of this tile?

	public int upgrade_value_inc = 0;
	public int Duration = 0;

	public override void Start()
	{
		base.Start();
		if(TileType.Length == 0) 
		{ 
			Debug.LogError("Swap skill parameters not loaded!");
		 	return;
		}
		Description_Basic = "Applies " + EffectName + " to " + (ApplyToAll ? "" : "" + TileNumber) + AllTypes + " tiles";
	}


	public override void Update()
	{
		base.Update();
		Description_Basic = "Applies " + EffectName + " to " + (ApplyToAll ? "" : "" + TileNumber) + AllTypes + " tiles";
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Reduces cooldown by 3") + "\n";
			s+= UpgradeInfoColoured(2,  "Inc. changed tile value by 1") + "\n";
			s+= UpgradeInfoColoured(3,  "Inc. changed tile value by 5") + "\n";
			return s;
		}
	}

	//bool UPGRADEOLD2()
	//{
	//	if(UpgradeLevel >= 4) return false;
	//	UpgradeLevel ++;
	//	switch(UpgradeLevel)
	//	{
	//		case 2:
	//		_defaultCooldown -= 3;
	//		if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
	//		break;
	//		case 3:
	//		upgrade_value_inc += 1;
	//		break;
	//		case 4:
	//		upgrade_value_inc += 5;
	//		break;
	//	}
	//	return true;
	//}

	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;

		StartCoroutine(ApplyGENUSRoutine());
	}

	public IEnumerator ApplyGENUSRoutine()
	{
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_apply = new List<Tile>();
		print(TileGenus[0] + " : " + TileSpecies[0]);
		for(int t = 0; t < TileType.Length; t++)
		{
			for(int x = 0; x < _tiles.GetLength(0); x++)
			{
				for(int y = 0; y < _tiles.GetLength(1); y++)
				{
					if(_tiles[x,y].IsType(TileGenus[t], TileSpecies[t])) 
					{
						to_apply.Add(_tiles[x,y]);
					}
				}
			}
		}
		
		if(!ApplyToAll)
		{
			List<Tile> final = new List<Tile>();
			for(int i = 0; i < TileNumber; i++)
			{
				int num = Random.Range(0, to_apply.Count);
				final.Add(to_apply[num]);
				to_apply.RemoveAt(num);
			}
			to_apply = final;
		}

		if(to_apply.Count == 0) 
		{
			TileMaster.instance.ResetTiles(true);
			cooldown_time = cooldown;
			activated = false;
			yield break;
		}
		foreach(Tile child in to_apply)
		{
			child.SetState(TileState.Selected, true);
			child.AddEffect(EffectName, Duration, args.ToArray());
		}
		
		yield return new WaitForSeconds(GameData.GameSpeed(0.35F));
		TileMaster.instance.ResetTiles(true);
		cooldown_time = cooldown;
		activated = false;
	}

	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		ApplyEffectToTile apply = (ApplyEffectToTile) new_ab;		
		TileGenus = apply.TileGenus;
		TileSpecies = apply.TileSpecies;
		Start();
	}


	//Input
	//Arg 0: Name
	//Arg 1: Chance
	//Arg 2: GENUS
	//Arg 3: ApplyToAll
	//Arg 4: TileNumber
	//Arg 5: Particle Name
	//Arg ??: TileGenus/TileSpecies

	//Output
	//Arg 0: Name
	//Arg 1: Chance
	//Arg 2: GENUS
	//Arg 3: GENUS(Buff) Type (Tile/Stat)
	//Arg ??: GENUS Values
	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		string prefix = "P";
		
		_input = null;
		_output = null;

		if(_in.HasValue) _input = con.Input[(int)_in];
		else _input = GetContainerData(con);

		if(_out.HasValue) _output = con.Output[(int)_out];
		else _output = GetOutputData(con);

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		ApplyToAll = _input.args[3] == "T";
		TileNumber = GameData.StringToInt(_input.args[4]);
		EffectName = _input.args[0];
		
		int num = 5;
		int types = (_input.args.Length - num)/2;
		TileGenus = new string[types];
		TileSpecies = new string[types];
		for(int i = 0; i < types; i++)
		{
			TileGenus[i] = _input.args[num];
			TileSpecies[i] = _input.args[num+1];
			num+=2;
		}


		EffectName = _output.args[0];
		Duration = GameData.StringToInt(_output.args[3]);
		args = new List<string>();
		for(int i = 4; i < _output.args.Length; i++)
		{
			args.Add(_output.args[i]);
		}
	}
}