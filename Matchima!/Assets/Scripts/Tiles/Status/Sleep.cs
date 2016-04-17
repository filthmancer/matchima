using UnityEngine;
using System.Collections;

public class Sleep : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Sleeping ", GameData.Colour(GENUS.DEX), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			_Tile.InitStats.isAlerted = false;
		}
		else _Tile.InitStats.isAlerted = true;
		return Duration == 0;
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.InitStats.isAlerted = false;
	}
}
