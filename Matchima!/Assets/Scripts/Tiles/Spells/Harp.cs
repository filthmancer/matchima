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

	public int Radius
	{
		get{
			CheckStats();
			return 2 + (Stats.Value/20);
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Stuns all enemies for " + StunDuration + " turns", GameData.Colour(Genus))};
		}
	}

	public override IEnumerator BeforeMatch(bool original)
	{
		if(isMatching) yield break;
		isMatching = true;

		GameObject p = Instantiate(Particles);
		p.transform.position = this.transform.position;

		List<Tile> to_collect = new List<Tile>();
		int xx = Point.Base[0], yy = Point.Base[1];
		for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
			{
				if(TileMaster.Tiles[x,y] == null) continue;
				
				int distX = Mathf.Abs(x - xx);
				int distY = Mathf.Abs(y - yy);
				if(distX + distY <= Radius)
				{
					to_collect.Add(TileMaster.Tiles[x,y]);
				}
			}
		}

		GameObject new_part = (GameObject) Instantiate(Particles);
		new_part.transform.position = transform.position;
		new_part.transform.parent = transform;

		TileMaster.instance.Ripple(this, to_collect, 2.4F*Stats.Value, GameData.GameSpeed(0.5F), 0.2F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.45F));
		if(to_collect.Count > 0)
		{
			foreach(Tile child in to_collect)
			{
				if(child != null)
				{
					if(child.Type.isEnemy) 
					{					
						child.SetState(TileState.Selected, true);
						MiniAlertUI m = UIManager.instance.MiniAlert(child.Point.targetPos, "Sleep", 55, GameData.Colour(child.Genus), 1.2F, 0.1F);
						child.AddEffect("Sleep", StunDuration);
					}
					
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