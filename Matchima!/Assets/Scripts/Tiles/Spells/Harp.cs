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
			return 2 + (Stats.Value);
		}
	}

	public int Radius
	{
		get{
			CheckStats();
			return 1 + (Stats.Value/5);
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Changes nearby tiles to " + GameData.ResourceLong(Genus), GameData.Colour(Genus),true, 40),
				new StCon("Charms enemy tiles", GameData.Colour(GENUS.CHA),true, 40)};
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
		PlayAudio("cast");
		TileMaster.instance.Ripple(this, to_collect, 2.1F*Stats.Value, GameData.GameSpeed(0.35F), 0.2F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.4F));
		if(to_collect.Count > 0)
		{
			foreach(Tile child in to_collect)
			{
				if(child != null)
				{
					
					//else
					//{
						child.ChangeGenus(Genus);
						//child.SetState(TileState.Selected, true);
						EffectManager.instance.PlayEffect(child.transform, Effect.Replace, "", GameData.instance.GetGENUSColour(child.Genus));	
						//c.AddValue(EndValueAdded);
					//}
					if(child.Type.isEnemy) 
					{					
						//child.SetState(TileState.Selected, true);
						MiniAlertUI m = UIManager.instance.MiniAlert(child.Point.targetPos, "Charmed!", 120, GameData.Colour(child.Genus), 0.3F, 0.1F);
						child.AddEffect("Charm", StunDuration);
					}
					
				}
			}
			yield return new WaitForSeconds(GameData.GameSpeed(0.2F));

		}
		
		yield break;
	}


	//public override void DestroyThyself(bool collapse = false)
	//{
	//	if(!collapse) Match(1);
	//	else base.DestroyThyself(collapse);
	//}
}