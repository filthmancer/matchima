using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Roaming : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Roaming", GameData.Colour(GENUS.DEX), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			Tile [] nbours = _Tile.Point.GetNeighbours();
			List<Tile> final = new List<Tile>();
			foreach(Tile child in nbours)
			{
				if(!child.isMatching && child.Point.Scale == _Tile.Point.Scale) final.Add(child);
			}

			Tile target = final[Random.Range(0, final.Count)];
			TileMaster.instance.SwapTiles(target, _Tile);
		}
		return Duration == 0;
	}
}
