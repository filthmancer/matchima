using UnityEngine;
using System.Collections;

public class Charm : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Charmed ", GameData.Colour(GENUS.CHA), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			//_Tile.Stats.isAlly = true;
		}
		else _Tile.OnAlert();
		return Duration == 0;
	}

	public override void StatusEffect()
	{
		//_Tile.Stats.isAlly = true;
	}

	public override TileStat CheckStats()
	{
		TileStat s = new TileStat();
		s._Team = Team.Ally;
		return s;
	}
}
