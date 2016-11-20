using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Curse : TileEffect {
	public int AttackReduction;
	public int HealthReduction;

	public override StCon [] Description
	{
		get{
			List<StCon> st = new List<StCon>();
			if(AttackReduction > 0) st.Add(new StCon("-" + AttackReduction + " Attack " + DurationString, GameData.Colour(GENUS.STR),false));
			if(HealthReduction > 0) st.Add(new StCon("-" + HealthReduction + " Health " + DurationString, GameData.Colour(GENUS.STR),false));
			st.Add(new StCon(DurationString));

			return st.ToArray();
		}
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.Stats._Attack.Add(-AttackReduction);
		_Tile.Stats._Hits.Add(-HealthReduction);
	}

	public override void GetArgs(int _dur, params string [] args)
	{
		base.GetArgs(_dur,args);
		AttackReduction = GameData.StringToInt(args[0]);
		HealthReduction = GameData.StringToInt(args[1]);
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			_Tile.Stats._Attack.Add(-AttackReduction);
			_Tile.Stats._Hits.Add(-HealthReduction);
		}
		return Duration == 0;
	}
}
