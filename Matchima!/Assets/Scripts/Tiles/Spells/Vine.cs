using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vine : Tile {

	public int radius = 1;
	public int wait_time = 2;

	private int time = 2;
	public override IEnumerator AfterTurnRoutine()
	{
		bool n = InitStats.isNew;
		base.AfterTurn();
		if(n) yield break;

		time ++;
		if(time < wait_time) yield break;
		else time = 0;

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		List<Tile> nbours = new List<Tile>();
		int xx = Point.Base[0], yy = Point.Base[1];
		for(int x = -radius; x <= radius; x++)
		{
			for(int y = -radius; y <= radius; y++)
			{
				if((x != 0 || y != 0) && x != y && x != -y)
				{
					int [] tile = new int [] {xx + x, yy + y};
					if(tile[0] >= TileMaster.Tiles.GetLength(0) || tile[0] < 0) continue;
					if(tile[1] >= TileMaster.Tiles.GetLength(1) || tile[1] < 0) continue;
					if(TileMaster.Tiles[tile[0], tile[1]]) 
					{
						if(!TileMaster.Tiles[tile[0], tile[1]].isMatching)
						{
							if(TileMaster.Tiles[tile[0],tile[1]].Genus != Genus) 
								nbours.Add(TileMaster.Tiles[tile[0], tile[1]]);
						}
					}	
				}
			}
		}
		if(nbours.Count == 0) 
		{
			TileMaster.instance.ResetTiles();
			yield break;
		}

		int rand = Random.Range(0, nbours.Count);
		Tile v = nbours[rand];

		SetState(TileState.Selected,true);
		yield return new WaitForSeconds(0.3F);
		v.SetState(TileState.Selected,true);
		yield return new WaitForSeconds(0.4F);
		TileMaster.instance.ReplaceTile(v.Point.Base[0],v.Point.Base[1], Type, Genus);
		TileMaster.instance.ResetTiles();
		yield break;
	}

}
