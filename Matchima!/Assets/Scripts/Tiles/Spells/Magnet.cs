using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Magnet : Tile {
	public int radius = 1;

	public bool IsMagnetic(Tile t)
	{
		return (t.Genus == Genus);
	}

	public override IEnumerator AfterTurnRoutine()
	{
		List<Tile> sameGenus = new List<Tile>();
		List<Tile> nbours = new List<Tile>();
		for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
			{
				if(TileMaster.Tiles[x,y] == this) continue;
				if(TileMaster.Tiles[x,y].IsType( "Magnet")) continue;
				int dist = Mathf.Abs(Point.Base[0]-x) + Mathf.Abs(Point.Base[1] - y);
				if(dist < 2) continue;

				if(IsMagnetic(TileMaster.Tiles[x,y]))
				{
					sameGenus.Add(TileMaster.Tiles[x,y]);
				}
				
			}
		}

		int xx = Point.Base[0], yy = Point.Base[1];
		//for(int x = -radius; x <= radius; x++)
		//{
		//	for(int y = -radius; y <= radius; y++)
		//	{
		//		
		//		if((x != 0 || y != 0) && x != y && x != -y)
		//		{
		//			int [] tile = new int [] {xx + x, yy + y};
		//			if(tile[0] >= TileMaster.Tiles.GetLength(0) || tile[0] < 0) continue;
		//			if(tile[1] >= TileMaster.Tiles.GetLength(1) || tile[1] < 0) continue;
		//			if(TileMaster.Tiles[tile[0], tile[1]]) 
		//			{
		//				if(!TileMaster.Tiles[tile[0], tile[1]].isMatching)
		//				{
		//					if(IsMagnetic(TileMaster.Tiles[tile[0],tile[1]])) continue;
		//					//TileMaster.Tiles[tile[0], tile[1]].SetState(TileState.Selected); 
		//				}
		//			}	
		//		}
		//	}
		//}

		for(int x = 1; x <= radius; x++)
		{
			for(int y = 1; y <= radius; y++)
			{
				for(int h = -1; h <= 1; h++)
				{
					for(int v = -1; v <= 1; v ++)
					{
						int fx = x*h;
						int fy = y*v;
						if((fx != 0 || fy != 0) && (Mathf.Abs(fx) + Mathf.Abs(fy)) <= radius)
						{
							int [] tile = new int[]{xx + fx, yy + fy};
							if(tile[0] >= TileMaster.Tiles.GetLength(0) || tile[0] < 0) continue;
							if(tile[1] >= TileMaster.Tiles.GetLength(1) || tile[1] < 0) continue;
							if(TileMaster.Tiles[tile[0], tile[1]]) 
							{
								if(!TileMaster.Tiles[tile[0], tile[1]].isMatching)
								{
									if(IsMagnetic(TileMaster.Tiles[tile[0],tile[1]])) continue;
									//TileMaster.Tiles[tile[0], tile[1]].SetState(TileState.Selected);
									nbours.Add(TileMaster.Tiles[tile[0], tile[1]]);
								}
							}
						}	
					}
				}
			}
		}

		if(nbours.Count == 0 || sameGenus.Count == 0)
		{
			yield break;
		}
		for(int i = 0; i < nbours.Count; i++)
		{
			if(sameGenus.Count == 0) break;
			int randG = Random.Range(0, sameGenus.Count);
			Tile a = sameGenus[randG];
			Tile b = nbours[0];

			if(a == null || b == null) continue;

			TileMaster.instance.CreateMiniTile(a.transform.position, b.transform, a.Inner, 0.5F);
			TileMaster.instance.CreateMiniTile(b.transform.position, a.transform, b.Inner, 0.5F);
			yield return new WaitForSeconds(0.5F);

			TileMaster.instance.ReplaceTile(a.Point.Base[0], a.Point.Base[1], b.Type, b.Genus);
			TileMaster.instance.ReplaceTile(b.Point.Base[0], b.Point.Base[1], a.Type, a.Genus);
			yield return new WaitForSeconds(0.1F);

			sameGenus.RemoveAt(randG);
			nbours.RemoveAt(0);
		}
		yield break;
	}
}
