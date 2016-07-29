using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHole : Tile {

	public bool StoppedUp = false;
	public int radius = 1;
	bool antigrav = false;
	public GameObject Particles;

	public override StCon [] Description
	{
		get{
			return new StCon[] {
				new StCon("Absorbs tile in " + radius, GameData.Colour(Genus),true, 40)};
		}
	}

	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		if(isMatching) yield break;
		isMatching = true;

		yield return StartCoroutine(SuckUp());

			
	}

	IEnumerator SuckUp()
	{
		while(!TileMaster.AllLanded) yield return null;
		radius = 0;
		int scale = 10;
		int val = Stats.Value;
		while(val > 0)
		{
			val -= scale;
			scale*=10;
			radius++;
		}

		GameObject part = Instantiate(Particles);
		part.transform.position = this.transform.position;
		part.transform.parent = this.transform;
		float emitsize = 8f * radius;
		part.GetComponent<ParticleSystem>().startSize = emitsize;
		part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);
		part.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);
		part.transform.localScale = Vector3.one * radius;

		List<Tile> tiles = new List<Tile>();
		int xx = Point.Base[0], yy = Point.Base[1];
		int dist = 0;
		for(int x = -radius; x <= radius; x++)
		{
			for(int y = -radius; y <= radius; y++)
			{
				dist = Mathf.Abs(x) + Mathf.Abs(y);
				if((x != 0 || y != 0) && dist <= radius)
				{
					int [] tile = new int [] {xx + x, yy + y};
					if(tile[0] >= TileMaster.Tiles.GetLength(0) || tile[0] < 0) continue;
					if(tile[1] >= TileMaster.Tiles.GetLength(1) || tile[1] < 0) continue;
					if(TileMaster.Tiles[tile[0], tile[1]]) 
					{
						if(!TileMaster.Tiles[tile[0], tile[1]].isMatching && 
							TileMaster.Tiles[tile[0], tile[1]].Point.Scale <= Point.Scale)// &&
						   //!TileMaster.Tiles[tile[0], tile[1]].IsState(TileState.Selected))
						{
							PlayerControl.instance.RemoveTileToMatch(TileMaster.Tiles[tile[0], tile[1]]);
							if(TileMaster.Tiles[tile[0],tile[1]].IsType("blackhole"))
							{
								AddEffect("AntiGravity", -1, "4");
								antigrav= true;
								PlayerControl.instance.RemoveTileToMatch(this);
							}
							TileMaster.Tiles[tile[0], tile[1]].SetState(TileState.Selected);
							tiles.Add(TileMaster.Tiles[tile[0], tile[1]]);
						}
					}	
				}
			}
		}
		if(tiles.Count == 0) yield break;
		yield return new WaitForSeconds(0.1F);
		for(int i = 0; i < tiles.Count; i++)
		{
			tiles[i].isMatching = true;
			Vector3 pos = transform.position + (GameData.RandomVector*1.4F);
			/*MoveToPoint mini = TileMaster.instance.CreateMiniTile(tiles[i].transform.position, this.transform, tiles[i].Inner);
			//mini.Target =  target;
			mini.SetMethod(() =>{
					this.AddValue(tiles[i].Stats.Value);
					tiles[i].DestroyThyself();
				}
			);*/
		}

		yield return new WaitForSeconds(0.25F);

		val = 0;
		//foreach(Tile child in tiles)
		//{
		//	val += child.Stats.Value;
		//	child.DestroyThyself();
		//}
		//AddValue(val);
		yield return new WaitForSeconds(0.2F);
		Reset();

		isMatching = false;
		TileMaster.instance.ResetTiles();
		//TileMaster.instance.SetFillGrid(true);
		yield break;
	}
}
