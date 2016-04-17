using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Harp : Tile {

	public GameObject Particles;
	public int StunDuration
	{
		get
		{
			CheckStats();
			return 1 + (Stats.Value/3);
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Stuns all enemies for " + StunDuration + " turns", GameData.Colour(Genus))};
		}
	}

	public override IEnumerator BeforeMatch()
	{
		if(isMatching) yield break;
		isMatching = true;

		GameObject p = Instantiate(Particles);
		p.transform.position = this.transform.position;

		float part_time = 0.6F;
		List<Tile> to_collect = new List<Tile>();
		Tile [,] _tiles = TileMaster.Tiles;
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				if(_tiles[x,y] == null) continue;
				if(_tiles[x,y].Type.isEnemy) 
				{
					to_collect.Add(_tiles[x,y]);
				}
			}
		}

		GameObject new_part = (GameObject) Instantiate(Particles);
		new_part.transform.position = transform.position;
		new_part.transform.parent = transform;

		yield return new WaitForSeconds(GameData.GameSpeed(part_time));
		if(to_collect.Count > 0)
		{
			foreach(Tile child in to_collect)
			{
				if(child != null)
				{
					child.SetState(TileState.Selected, true);
					TileEffect eff = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName("Sleep"));
					eff.GetArgs(StunDuration);
					child.AddEffect(eff);
				}
			}
		}

		yield break;
	}


	public override void DestroyThyself(bool collapse = false)
	{
		if(!collapse) Match(1);
		else base.DestroyThyself(collapse);
	}
}