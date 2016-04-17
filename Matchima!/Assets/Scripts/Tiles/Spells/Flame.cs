using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flame : Tile {

	public GameObject Particle;
	private int total_cycles
	{
		get
		{
			CheckStats();
			return (int)Mathf.Sqrt(Stats.Value);
		}
	}

	private int damage = 10;
	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Collects a " + total_cycles + "L cone", GameData.Colour(Genus)),
				new StCon("Deals ", Color.white, false),
				new StCon(damage + " damage", GameData.Colour(Genus))
			};
		}
	}


	public override IEnumerator BeforeMatch()
	{
		if(isMatching) yield break;
		isMatching = true;
		CheckStats();
		

	//FIND TILE AFTER THIS IN MATCH (OR BEFORE IF LAST TILE)
		Tile [] nbours = Point.GetNeighbours(false);
		Tile nexttile = nbours[Random.Range(0, nbours.Length)];

	//COLLECT ALL TILES ALONG VELOCITY BETWEEN THIS TILE AND NEXT TILE
		int d;
		int [] closest = Point.Closest(nexttile.Point.Base, out d);
		int [] vel = Utility.IntNormal(closest, nexttile.Point.Base);
		int [] cross_vel = new int[] {vel[0] != 0 ? 0 : 1, vel[1] != 0 ? 0 : 1};
		int [] final = new int [] { closest[0] + vel[0], closest[1] + vel[1]};

		List<Tile> to_collect = new List<Tile>();
		int x = final[0], y = final[1];
		int cycle = 0;
		while(x >= 0 && x < TileMaster.Grid.Size[0] && y >= 0 && y < TileMaster.Grid.Size[1])
		{
			if(cycle > total_cycles) break;
			if(!TileMaster.Tiles[x,y].isMatching || TileMaster.Tiles[x,y] == this)
			{
				FlameTile(TileMaster.Tiles[x,y]);
				to_collect.Add(TileMaster.Tiles[x,y]);	

				for(int i = 0; i < cycle; i++)
				{
					int factor = i + 1;
					int [] left = new int [] {x + cross_vel[0]*factor, y + cross_vel[1]*factor};
					int [] right = new int [] {x-cross_vel[0]*factor,y-cross_vel[1]*factor};
					if(TileMaster.instance.GetTile(left[0], left[1]) != null) 
					{
						FlameTile(TileMaster.instance.GetTile(left[0], left[1]));
						to_collect.Add(TileMaster.instance.GetTile(left[0], left[1]));
					}
					if(TileMaster.instance.GetTile(right[0], right[1]) != null) 
					{
						FlameTile(TileMaster.instance.GetTile(right[0], right[1]));
						to_collect.Add(TileMaster.instance.GetTile(right[0], right[1]));
					}
				}
				
				yield return new WaitForSeconds(Time.deltaTime * 5);
			}	
			cycle ++;
			x += vel[0];
			y += vel[1];
			yield return null;
		}

		if(to_collect.Count == 0) yield break;


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
			if(to_collect[i].Type.isEnemy)
			{
				to_collect[i].InitStats.TurnDamage += damage;
			}
		}

		PlayerControl.instance.AddTilesToMatch(to_collect.ToArray());

		yield return null;
	}

	void FlameTile(Tile t)
	{

		t.SetState(TileState.Selected, true);
		GameObject p = (GameObject)Instantiate(Particle);
		p.transform.position = t.transform.position;
		p.GetComponent<MoveToPoint>().enabled = false;
	}
}
