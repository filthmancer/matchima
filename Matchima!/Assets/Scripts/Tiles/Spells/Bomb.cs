using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomb : Tile {
	public int radius
	{
		get
		{
			CheckStats();
			return 1 + (Stats.Value/12);
		}
	}
	public GameObject Particles;
	public int BombDamage = 10;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Collects in " + radius + " radius."),
				new StCon("Deals ", Color.white, false),
				new StCon(BombDamage + " damage", GameData.Colour(Genus))
			};
		}
	}


	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale, inf, value_inc);
	}


	public override IEnumerator BeforeMatch()
	{
		if(isMatching) yield break;
		isMatching = true;		
		
		List<Tile> to_collect = new List<Tile>();
		int xx = Point.Base[0], yy = Point.Base[1];

		for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
			{
				int distX = Mathf.Abs(x - xx);
				int distY = Mathf.Abs(y - yy);
				if(distX + distY <= radius)
				{
					Tile tile = TileMaster.Tiles[x,y];
					TileMaster.Tiles[x,y].SetState(TileState.Selected, true);
					to_collect.Add(tile);
				}
			}
		}
		//for(int x = -radius; x <= radius; x++)
		//{
		//	for(int y = -radius; y <= radius; y++)
		//	{
		//		if(x != 0 || y != 0)
		//		{
		//			int [] tile = new int [] {xx + x, yy + y};
		//			if(tile[0] >= TileMaster.Tiles.GetLength(0) || tile[0] < 0) continue;
		//			if(tile[1] >= TileMaster.Tiles.GetLength(1) || tile[1] < 0) continue;
		//			if(TileMaster.Tiles[tile[0], tile[1]]) 
		//			{
		//				if(TileMaster.Tiles[tile[0],tile[1]].Type.isEnemy) 
		//				{
		//					TileMaster.Tiles[tile[0],tile[1]].InitStats.TurnDamage += BombDamage;
		//				}
		//				if(!TileMaster.Tiles[tile[0], tile[1]].isMatching)
		//				{
		//					TileMaster.Tiles[tile[0], tile[1]].SetState(TileState.Selected,true);
		//					to_collect.Add(TileMaster.Tiles[tile[0], tile[1]]);
		//				}
		//			}	
		//		}
		//	}
		//}
		
		GameObject p = Instantiate(Particles);
		p.transform.position = this.transform.position;
		p.GetComponent<MoveToPoint>().enabled = false;
		foreach(Tile child in to_collect)
		{
			p = Instantiate(Particles);
			p.transform.position = child.transform.position;
			p.GetComponent<MoveToPoint>().enabled = false;
		}

		yield return new WaitForSeconds(0.5F);

		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.RemoveTileToMatch(this);
		to_collect.Add(this);
		for(int i = 0; i < to_collect.Count; i++)
		{
			if(to_collect[i].IsType("Chicken"))
			{
				TileMaster.instance.ReplaceTile(to_collect[i].Point.Base[0], to_collect[i].Point.Base[1], TileMaster.Types["Health"]);
				TileMaster.Tiles[to_collect[i].Point.Base[0], to_collect[i].Point.Base[1]].AddValue(to_collect[i].Stats.Value * 10);
				to_collect.RemoveAt(i);
			}
		}
		PlayerControl.instance.AddTilesToMatch(to_collect.ToArray());
	}

	//public override IEnumerator AfterTurnRoutine()
	//{
	//	Reset();
	//	InitStats.Lifetime ++;
	//	if(InitStats.Lifetime >= 1) 
	//	{
	//		Stats.isNew = false;
	//	}

	//	yield break;

	//	//TRYING TO MAKE BOMBS MATCH IF THEY HAVE 2 BOMB NEIGHBOURS
	//	
	//	int exp = 0;
	//	Tile [] nb = new Tile[4];
	//	nb[0] = TileMaster.instance.GetTile(num[0], num[1]-1);
	//	nb[1] = TileMaster.instance.GetTile(num[0], num[1]+1);
	//	nb[2] = TileMaster.instance.GetTile(num[0]-1, num[1]);
	//	nb[3] = TileMaster.instance.GetTile(num[0]+1, num[1]);
	//	for(int i = 0; i < nb.Length; i++)
	//	{
	//		if(nb[i] == null) continue;
	//		if(nb[i].IsType("Bomb")) exp++;
	//	}
	//	if(exp >= 2)
	//	{
	//		Match(1);
	//		//StartCoroutine(BeforeMatch());
	//	}

	//}

//	public override bool Match(int resource) 
//	{
//		if(isMatching) return false;
//		isMatching = true;
//
//		GameObject p = Instantiate(Particles);
//		p.transform.position = this.transform.position;
//
//		int xx = num[0], yy = num[1];
//		for(int x = -radius; x <= radius; x++)
//		{
//			for(int y = -radius; y <= radius; y++)
//			{
				//
//				if(x != 0 || y != 0)
//				{
//					int [] tile = new int [] {xx + x, yy + y};
//					if(tile[0] >= TileMaster.Tiles.GetLength(0) || tile[0] < 0) continue;
//					if(tile[1] >= TileMaster.Tiles.GetLength(1) || tile[1] < 0) continue;
//					if(TileMaster.Tiles[tile[0], tile[1]]) 
//					{
//						if(!TileMaster.Tiles[tile[0], tile[1]].isMatching)
//						{
//							GameObject part = Instantiate(Particles);
//							part.transform.position = TileMaster.Tiles[tile[0], tile[1]].transform.position;
//							//part.transform.SetParent(TileMaster.Tiles[tile[0],tile[1]].transform);
//							TileMaster.Tiles[tile[0], tile[1]].SetState(TileState.Selected);
//							TileMaster.Tiles[tile[0], tile[1]].DestroyThyself(false);
//						}
//					}	
//				}
//			}
//		}
//		base.DestroyThyself(false);
//		return true;
//	}

	//public override void DestroyThyself(bool collect, bool collapse = false)
//	{
//		if(!collapse) Match(1);
//		else base.DestroyThyself(collect,false);
//	}
}