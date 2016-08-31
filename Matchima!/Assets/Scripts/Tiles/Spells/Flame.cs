using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flame : Tile {

	public GameObject Particle;
	public GameObject TargetObj;

	private int total_cycles
	{
		get
		{
			CheckStats();
			return (int)Mathf.Sqrt(Stats.Value)*3;
		}
	}

	private int damage = 10;
	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Collects a cone of tiles", GameData.Colour(Genus), true, 40),
				new StCon("Deals ", Color.white, false, 40),
				new StCon(damage + " damage", GameData.Colour(Genus), true, 40)
			};
		}
	}


	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		if(isMatching) yield break;
		isMatching = true;
		CheckStats();
		

	//FIND TILE AFTER THIS IN MATCH (OR BEFORE IF LAST TILE)
		Tile [] nbours = Point.GetNeighbours(false);
		Tile nexttile = null;
		IntVector tile_pos = new IntVector(Point.BaseX, Point.BaseY);
		IntVector nexttile_pos = new IntVector(Point.BaseX, Point.BaseY);
		IntVector velocity = new IntVector(0,0);
		switch(Genus)
		{
		case GENUS.STR:
		velocity = new IntVector(-1,-1);
		break;
		case GENUS.DEX:
		velocity = new IntVector(1,-1);
		break;
		case GENUS.CHA:
		velocity = new IntVector(1,1);
		break;
		case GENUS.WIS:
		velocity = new IntVector(-1,1);
		break;
		}

		nexttile_pos = tile_pos + velocity;

		if(nexttile_pos.x >= TileMaster.Grid.Size[0] || nexttile_pos.x < 0)
		{
			velocity.x = - velocity.x;
			
		}

		if(nexttile_pos.y >= TileMaster.Grid.Size[1] || nexttile_pos.y < 0)
		{
			velocity.y = -velocity.y;
		}

		nexttile_pos = tile_pos + velocity;

		
		//nbours[Random.Range(0, nbours.Length)];

	//COLLECT ALL TILES ALONG VELOCITY BETWEEN THIS TILE AND NEXT TILE
		/*nexttile = TileMaster.Tiles[nexttile_pos.x, nexttile_pos.y];
		int d;
		int [] closest = Point.Closest(nexttile.Point.Base, out d);
		int [] vel = Utility.IntNormal(closest, nexttile.Point.Base);
		//int [] cross_vel = new int[] {vel[0] != 0 ? 0 : 1, vel[1] != 0 ? 0 : 1};
		int [] final = new int [] { closest[0] + vel[0], closest[1] + vel[1]};*/

		List<Tile> to_collect = new List<Tile>();
		//int x = final[0], y = final[1];
		int x = nexttile_pos.x, y = nexttile_pos.y;
		int cycle = 0;

		GameObject targ = (GameObject)Instantiate(TargetObj);
		targ.transform.position = TileMaster.Tiles[x,y].transform.position;

		while(cycle < total_cycles)
		{	
			if(x <= 0 || x >= TileMaster.Grid.Size[0]-1)
			{
				velocity.x = -velocity.x;
			}
			
			if(y <= 0 || y >= TileMaster.Grid.Size[1]-1)
			{
				velocity.y = -velocity.y;
			}

			if(!TileMaster.Tiles[x,y].isMatching && TileMaster.Tiles[x,y] != this)
			{
				FlameTile(TileMaster.Tiles[x,y]);
				to_collect.Add(TileMaster.Tiles[x,y]);	
				
				//yield return new WaitForSeconds(Time.deltaTime * 5);
			}

			if(cycle < total_cycles-1)
			{
				Vector3 initpos = TileMaster.Tiles[x,y].transform.position;
				Vector3 nextpos = TileMaster.Tiles[x+velocity.x, y+velocity.y].transform.position;
				bool isLerping = true;
				float rate = 0.0F;

				while(isLerping)
				{
					targ.transform.position = Vector3.Lerp(initpos, nextpos, rate);
					rate += Time.deltaTime * 10;
					if(rate >= 1.0F) isLerping = false;
					yield return null;
				}	
			}
			
			
			x += velocity.x;
			y += velocity.y;

			cycle ++;
			
			yield return null;
		}

		Destroy(targ.gameObject);

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

		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());

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
