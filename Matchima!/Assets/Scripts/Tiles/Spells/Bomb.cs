using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomb : Tile {
	public int radius
	{
		get
		{
			CheckStats();
			return 1 + (Stats.Value/4);
		}
	}
	public GameObject Particles;
	private int BombDamage
	{
		get{
			return (2 * Stats.Value) + (int)Player.SpellValue;
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Collects in " + radius + " radius.", Color.white,true, 40),
				new StCon("Deals ", Color.white, false, 40),
				new StCon(BombDamage + " damage to enemy tiles", GameData.Colour(Genus),true, 40)
			};
		}
	}


	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale, inf, value_inc);
	}


	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		if(isMatching) yield break;
		isMatching = true;		
		PlayAudio("cast");
		
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

		GameObject p = Instantiate(Particles);
		p.transform.position = this.transform.position;
		p.GetComponent<MoveToPoint>().enabled = false;
		foreach(Tile child in to_collect)
		{
			p = Instantiate(Particles);
			p.transform.position = child.transform.position;
			p.GetComponent<MoveToPoint>().enabled = false;
		}

		CameraUtility.instance.ScreenShake((float)Stats.Value/5,  GameData.GameSpeed(0.4F));
		yield return new WaitForSeconds( GameData.GameSpeed(0.2F));

		for(int i = 0; i < to_collect.Count; i++)
		{
			if(to_collect[i].IsType("Chicken"))
			{
				TileMaster.instance.ReplaceTile(to_collect[i].Point.Base[0], to_collect[i].Point.Base[1], TileMaster.Types["Health"]);
				TileMaster.Tiles[to_collect[i].Point.Base[0], to_collect[i].Point.Base[1]].AddValue(to_collect[i].Stats.Value * 10);
				to_collect.RemoveAt(i);
			}
			if(to_collect[i].IsType("Enemy")) 
			{
				Vector3 pos = TileMaster.Grid.GetPoint(to_collect[i].Point.Point(0)) + Vector3.down * 0.3F;
				MiniAlertUI hit = UIManager.instance.DamageAlert(pos, BombDamage);

				to_collect[i].InitStats.Hits -= BombDamage;
				to_collect[i].PlayAudio("hit");
				EffectManager.instance.PlayEffect(to_collect[i].transform,Effect.Attack);
			}
		}
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield return new WaitForSeconds( GameData.GameSpeed(0.2F));
	}
}