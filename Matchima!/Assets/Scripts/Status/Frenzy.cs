using UnityEngine;
using System.Collections;

public class Frenzy : TileEffect {

	public int Damage = 0;

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Frenzied", GameData.Colour(GENUS.STR), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		return Duration == 0;
	}

	public override void GetArgs(int _duration, params string [] args)
	{
		base.GetArgs(_duration, args);
		Damage = (int)GameData.StringToFloat(args[0]);
	}

	public override TileStat CheckStats()
	{
		TileStat s = new TileStat();
		s._Attack.Set(Damage);
		return s;
	}
}
