using UnityEngine;
using System.Collections;

public class DamageInc : ClassEffect {
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

	public override Stat CheckStats()
	{
		Stat s = new Stat();
		s._Attack = Damage;
		return s;
	}
}
