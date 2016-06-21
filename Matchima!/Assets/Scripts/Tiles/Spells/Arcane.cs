using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arcane : Tile {

	public string InputGenus, InputType;

	public string EndGenus;
	//{
	//	get
	//	{
	//		if(InputGenus == "Random") return GameData.ResourceLong((GENUS)Random.Range(0,4));
	//		else if(InputGenus == "RandomAll") return GameData.ResourceLong((GENUS)Random.Range(0,7));
	//		else if(InputGenus == "Genus") return GameData.ResourceLong(Genus);
	//		else return InputGenus;
	//	}
	//}

	public string EndType;
	//{
	//	get
	//	{
	//		if(InputType == "Random") return "Resource";
	//		else return InputType;
	//	}
	//}
	public int EndValueAdded = 0;
	public int TilesCollected
	{
		get
		{
			CheckStats();
			return 2 + Stats.Value/2;
		}
	}

	public int final_damage 
	{
		get
		{
			CheckStats();
			return 3 + (Stats.Value);
		}
	}

	public GameObject ArcaneParticle;
	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Changes " + TilesCollected + (TilesCollected > 1 ? " tiles" : " tile")
										 + " to " + EndGenus + " " + EndType)};
		}
	}
	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc)
	{
		base.Setup(x, y, scale, inf, value_inc);	
		EndGenus = GameData.ResourceLong(Genus);
	}

	public override void SetArgs(params string [] args)
	{
		InputGenus = args[0];
		InputType = args[1];
		EndGenus = args[2];
		EndType = args[3];

		if(InputGenus == "Random") InputGenus = GameData.ResourceLong((GENUS)Random.Range(0,4));
		else if(InputGenus == "RandomAll") InputGenus = GameData.ResourceLong((GENUS)Random.Range(0,6));

		if(EndGenus == "Random") EndGenus = GameData.ResourceLong((GENUS)Random.Range(0,4));
		else if(EndGenus == "RandomAll") EndGenus = GameData.ResourceLong((GENUS)Random.Range(0,6));
	}


	public override IEnumerator BeforeMatch(bool original)
	{
		float part_time = 0.6F;
		if(isMatching) yield break;

		isMatching = true;
		List<Tile> to_collect = new List<Tile>();
		int x = TileMaster.Tiles.GetLength(0);
		int y = TileMaster.Tiles.GetLength(1);

		for(int xx = 0; xx < x; xx ++)
		{
			for(int yy = 0; yy < y; yy++)
			{
				if(TileMaster.Tiles[xx,yy].IsGenus(GENUS.OMG, false) &&
					!TileMaster.Tiles[xx,yy].isMatching) to_collect.Add(TileMaster.Tiles[xx,yy]);
			}
		}

		
		while(to_collect.Count > TilesCollected)
		{
			to_collect.RemoveAt(Random.Range(0, to_collect.Count));
		}

		foreach(Tile child in to_collect)
		{
			
			PlayerControl.instance.RemoveTileToMatch(child);

			GameObject part = Instantiate(ArcaneParticle);
			part.transform.position = this.transform.position;

			MoveToPoint mp = part.GetComponent<MoveToPoint>();
			mp.SetTarget(child.transform.position);
			mp.SetPath(0.5F, 0.2F);
			part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);

			float dist = Vector3.Distance(child.transform.position, this.transform.position);
			mp.Speed = 0.1F + 0.05F * dist;
			part_time = 0.2F + (0.03F * dist);
			mp.SetTileMethod(child, (Tile c) =>
			{
				child.SetState(TileState.Selected, true);
				if(EndType == string.Empty)
				{
					c.ChangeGenus(Genus);	
					c.AddValue(EndValueAdded);
				}
				else if(EndGenus == string.Empty)
				{
					TileMaster.instance.ReplaceTile(c, TileMaster.Types[EndType], c.Genus, 1, EndValueAdded);
				}
				else TileMaster.instance.ReplaceTile(c, TileMaster.Types[EndType], TileMaster.Genus[EndGenus], 1, EndValueAdded);
				EffectManager.instance.PlayEffect(c.transform, Effect.Replace, "", GameData.instance.GetGENUSColour(c.Genus));	
			});

			yield return new WaitForSeconds(part_time);
			
		}

		if(TileMaster.instance.EnemiesOnScreen == 0) yield break;
		int check = 0;
		while(to_collect.Count < TilesCollected)
		{
			Tile c = TileMaster.Tiles[Utility.RandomInt(x), Utility.RandomInt(y)];
			if(c != this && 
				c.Type.isEnemy)
			{
				
				to_collect.Add(c);	
				//PlayerControl.instance.RemoveTileToMatch(c);

				GameObject part = Instantiate(ArcaneParticle);
				part.transform.position = this.transform.position;
				MoveToPoint mp = part.GetComponent<MoveToPoint>();
				mp.SetTarget(c.transform.position);
				mp.SetPath(0.5F, 0.2F);
				part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);

				float dist = Vector3.Distance(c.transform.position, this.transform.position);
				mp.Speed = 0.1F + 0.05F * dist;
				part_time = 0.2F;// + (0.03F * dist);

				mp.SetTileMethod(c, (Tile child) =>
				{
					child.SetState(TileState.Selected, true);
					child.InitStats.Hits -= final_damage;
					//child.InitStats.TurnDamage += final_damage;
					PlayerControl.instance.AddTilesToSelected(child);

					float init_rotation = Random.Range(-3,3);
					float info_time = 0.4F;
					float info_start_size = 100 + (final_damage*2);
					float info_movespeed = 0.25F;
					float info_finalscale = 0.65F;

					Vector3 pos = TileMaster.Grid.GetPoint(child.Point.Point(0)) + Vector3.down * 0.3F;
					MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + final_damage, info_start_size, GameData.Colour(Genus), info_time, 0.6F, false);
					m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
					m.SetVelocity(Utility.RandomVectorInclusive(0.2F) + (Vector3.up*0.4F));
					m.Gravity = true;
					m.AddJuice(Juice.instance.BounceB, info_time/0.8F);

					CameraUtility.instance.ScreenShake(0.26F + 0.02F * final_damage,  GameData.GameSpeed(0.06F));
					EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
					
				});

				yield return new WaitForSeconds(part_time);
			}
			//yield return null;
		}
		
		yield return null;
	}

}
