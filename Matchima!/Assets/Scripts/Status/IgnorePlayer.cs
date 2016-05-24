using UnityEngine;
using System.Collections;

public class IgnorePlayer : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Ignored", GameData.Colour(GENUS.OMG), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		return Duration == 0;
	}
}