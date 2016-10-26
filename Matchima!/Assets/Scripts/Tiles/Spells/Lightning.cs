using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lightning : Tile {

	private int final_damage
	{
		get{
			return  8 + (int)(2.0F * Stats.Value) + (int) Player.SpellValue;
		}
	}

	private int final_collected
	{
		get{
			return 1 + (int) ((float)Stats.Value/5.0F);
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Shoots lightning at a random tile", Color.white,true, 40),
				new StCon("Deals ", Color.white, false, 40),
				new StCon(final_damage+"", GameData.Colour(GENUS.WIS), false, 40),
				new StCon(" damage", Color.white, true, 40)
			};
		}
	}

	
	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		if(isMatching) yield break;
		isMatching = true;
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<GENUS> onScreen = new List<GENUS>();
		List<float> onScreen_chances = new List<float>();
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				Tile tile = _tiles[x,y];
				if(tile == null || tile.Genus == this.Genus) continue;
				//if(tile.isMatching || tile.Destroyed) continue;
				bool add = true;
				for(int i = 0; i < onScreen.Count; i++)
				{
					if(tile.Genus == onScreen[i])
					{
						add = false;
						onScreen_chances[i] += 0.2F;
						break;
					}
				}
				if(add)
				{
					onScreen.Add(tile.Genus);
					onScreen_chances.Add(0.1F);
				}
			}
		}

		if(onScreen_chances.Count == 0) yield break;

		//GameObject part = EffectManager.instance.PlayEffect(transform, Effect.Lightning);
		//CameraUtility.instance.ScreenShake((float)Stats.Value/10, Time.deltaTime* 15);
		AudioSource aud = PlayAudio("cast", 0.75F);
		if(aud) aud.GetComponent<DestroyTimer>().Timer = 1.0F;
		//yield return new WaitForSeconds(GameData.GameSpeed(0.35F));



		for(int i = 0; i < final_collected; i++)
		{
			int collect = ChanceEngine.Index(onScreen_chances.ToArray());
			GENUS col = onScreen[collect];
			particles = new List<GameObject>();
			Tile target = TileMaster.Tiles[Random.Range(0,TileMaster.Grid.Size[0]), Random.Range(0,TileMaster.Grid.Size[1])];
			while(target == null || target.Genus != col)
			{
				target = TileMaster.Tiles[Random.Range(0,TileMaster.Grid.Size[0]), Random.Range(0,TileMaster.Grid.Size[1])];
			}
			target.SetState(TileState.Selected, true);

			float part_time = GameData.GameSpeed(0.23F);
			float part_time_init = part_time;
			LineRenderer [] bolt = new LineRenderer [] {
				EffectManager.instance.PlayEffect(this.transform, Effect.Lightning).GetComponent<LineRenderer>(),
				EffectManager.instance.PlayEffect(this.transform, Effect.Lightning).GetComponent<LineRenderer>(),
				EffectManager.instance.PlayEffect(this.transform, Effect.Lightning).GetComponent<LineRenderer>(),
				EffectManager.instance.PlayEffect(this.transform, Effect.Lightning).GetComponent<LineRenderer>(),
				EffectManager.instance.PlayEffect(this.transform, Effect.Lightning).GetComponent<LineRenderer>()
			};
			
			
			Vector3 offset = new Vector3(0, 0, -0.3F);

			MatchContainer m = TileMaster.instance.FloodCheck(target, true);
			
			int curr = 0;
			float time_per_tile = GameData.GameSpeed(0.03F);
			float tile_waittime = 0.0F;
			while((part_time -= Time.deltaTime) > 0.0F)
			{
				for(int o = 0; o < bolt.Length; o++)
				{
					Vector3 [] points = PlayerControl.instance.LightningLine(this.transform.position + offset, target.transform.position + offset, 5, 0.4F);
					bolt[o].SetVertexCount(points.Length);
					Color c = Color.Lerp(Color.white, Color.blue, (float)o/bolt.Length);
					bolt[o].SetColors(c,c);
					for(int b = 0; b < points.Length; b++)
					{
						bolt[o].SetPosition(b, points[b]);
					}
				}

				if(m.Tiles.Count > curr && tile_waittime <= 0.0F)
				{
					CameraUtility.instance.ScreenShake(0.15F + (float)Stats.Value/10,  0.2F);
					to_collect.Add(m.Tiles[curr]);
					m.Tiles[curr].SetState(TileState.Selected, true);
					curr++;
					part_time += time_per_tile;
					tile_waittime = time_per_tile;
					yield return new WaitForSeconds(Time.deltaTime * 2); 
				}
				else tile_waittime -= Time.deltaTime;
				
				yield return null;
			}
			
			
			for(int o =0; o < bolt.Length; o++) bolt[o].GetComponent<ObjectPoolerReference>().Unspawn();
		}

		foreach(Tile child in to_collect)
		{
			if(child.Type.isEnemy)
			{
				Vector3 pos = TileMaster.Grid.GetPoint(child.Point.Point(0)) + Vector3.down * 0.3F;
				MiniAlertUI hit = UIManager.instance.DamageAlert(pos, final_damage);

				child.InitStats.Hits -= final_damage;
				child.PlayAudio("Hit");
				EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
			} 
			if(child.IsType("","Altar"))
			{
				TileMaster.instance.ReplaceTile(child.Point.Base[0], child.Point.Base[1], TileMaster.Types["blob"], GENUS.RAND, child.Point.Scale);
			}
			if(child.IsType("", "chicken"))
			{
				TileMaster.instance.ReplaceTile(child.Point.Base[0], child.Point.Base[1], TileMaster.Types["health"], GENUS.RAND, child.Point.Scale);
			}			
		}

		//yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		
		yield return new WaitForSeconds( GameData.GameSpeed(0.08F));
		//if(part) Destroy(part);
	}

}