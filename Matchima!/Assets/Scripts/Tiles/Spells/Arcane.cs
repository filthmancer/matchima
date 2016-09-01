using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arcane : Tile {

	public string InputGenus, InputType;
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
			return (1 * (Stats.Value)) + (int)Player.SpellValue;
		}
	}

	public GameObject ArcaneParticle;
	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Attacks " + TilesCollected + " enemy tiles", Color.white,true, 40),
				new StCon("Deals ", Color.white, false, 40),
				new StCon(final_damage+"", GameData.Colour(GENUS.WIS), false, 40),
				new StCon(" damage", Color.white, true, 40)
			};
		}
	}
	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc)
	{
		base.Setup(x, y, scale, inf, value_inc);	
	}

	public override void SetArgs(params string [] args)
	{
		
	}


	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		float part_time = 0.2F;
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
			mp.SetPath(28.0F, 0.2F);
			part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);

			float dist = Vector3.Distance(child.transform.position, this.transform.position);
			//mp.Speed = 0.1F + 0.05F * dist;
			//part_time = 0.2F + (0.03F * dist);
			mp.SetTileMethod(child, (Tile c) =>
			{
				child.SetState(TileState.Selected, true);
				/*if(EndType == string.Empty)
				{
					c.ChangeGenus(Genus);	
					c.AddValue(EndValueAdded);
				}
				else if(EndGenus == string.Empty)
				{
					TileMaster.instance.ReplaceTile(c, TileMaster.Types[EndType], c.Genus, 1, EndValueAdded);
				}*/
				//else 
				//TileMaster.instance.ReplaceTile(c, TileMaster.Types[EndType], TileMaster.Genus[EndGenus], 1, EndValueAdded);
				c.ChangeGenus(Genus);
				EffectManager.instance.PlayEffect(c.transform, Effect.Replace, GameData.instance.GetGENUSColour(c.Genus));	
			});

			yield return new WaitForSeconds(part_time);
			
		}

		if(TileMaster.instance.EnemiesOnScreen == 0 || TileMaster.Enemies.Length == 0) yield break;
		int check = 0;
		bool checkforallies = false;
		int check_maxsize = TileMaster.Grid.Size[0]*TileMaster.Grid.Size[1];
		while(to_collect.Count < TilesCollected)
		{
			Tile c = TileMaster.Enemies[Random.Range(0, TileMaster.Enemies.Length)];
			if(c != this && 
				c.Type.isEnemy && (!c.Type.isAlly || checkforallies))
			{
				
				to_collect.Add(c);	
				//PlayerControl.instance.RemoveTileToMatch(c);

				GameObject part = Instantiate(ArcaneParticle);
				part.transform.position = this.transform.position;
				MoveToPoint mp = part.GetComponent<MoveToPoint>();
				mp.SetTarget(c.transform.position);
				mp.SetPath(28.0F, 0.2F);
				mp.DontDestroy = false;

				part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);
				float dist = Vector3.Distance(c.transform.position, this.transform.position);
				///mp.Speed = 0.1F + 0.05F * dist;
				//part_time = 0.2F;// + (0.03F * dist);

				mp.SetTileMethod(c, (Tile child) =>
				{
					child.SetState(TileState.Selected, true);
					child.InitStats.Hits -= final_damage;
					//child.InitStats.TurnDamage += final_damage;
					PlayerControl.instance.AddTilesToSelected(child);

					Vector3 pos = TileMaster.Grid.GetPoint(child.Point.Point(0)) + Vector3.down * 0.3F;
					MiniAlertUI hit = UIManager.instance.DamageAlert(pos, final_damage);

					CameraUtility.instance.ScreenShake(0.26F + 0.02F * final_damage,  GameData.GameSpeed(0.06F));
					EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
					
				});

				yield return new WaitForSeconds(part_time);
			}
			else 
			{
				check++;
				if(check > check_maxsize/2) checkforallies = true;
			}
			if(check > check_maxsize) break;
			
		}
		
		yield return null;
	}

}
