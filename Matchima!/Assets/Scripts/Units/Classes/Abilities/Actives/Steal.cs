using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steal : Ability {

	public string TileType;
	public string ResourceType;

	private int upgrade_res_inc = 0;

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Matching", Color.white, false));
			All.Add(new StCon(TileType, Color.white, false));
			All.Add(new StCon(" gives " + ResourceType, Color.white));
			return All.ToArray();
		}
	}


	public override void Start()
	{
		base.Start();
		if(TileType == string.Empty || ResourceType == string.Empty) 
		{ 
			Debug.LogError("Swap skill parameters not loaded!");
		 	return;
		}
		Description_Basic = "Matching " + TileType + " tiles this turn gives " + ResourceType; 
	}

	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;

		activated = true;
	}

	public override IEnumerator AfterMatch()
	{
		if(!activated) yield break;

		List<Tile> _tiles = PlayerControl.instance.selectedTiles;

		int resource = 0;

		foreach(Tile child in _tiles)
		{
			if(child.IsType(TileType))
			{
				int final_res = 1 + upgrade_res_inc + (int) StatBonus();
				resource += final_res;
			}
		}

		switch(ResourceType)
		{
			case "Yellow":
			Player.Classes[3].AddToMeter(resource);
			break;
			case "Blue":
			Player.Classes[1].AddToMeter(resource);
			break;
			case "Red":
			Player.Classes[0].AddToMeter(resource);
			break;
			case "Green":
			Player.Classes[2].AddToMeter(resource);
			break;
		}
		cooldown_time = cooldown;
		activated = false;
		yield break;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Steal new_steal = (Steal) new_ab;
		TileType = new_steal.TileType;
		ResourceType = new_steal.ResourceType;
		Start();
	}

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

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		TileType = _input.args[1];
		ResourceType = _input.args[4];

	}

}