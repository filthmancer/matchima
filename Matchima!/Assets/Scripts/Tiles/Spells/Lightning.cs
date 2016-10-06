using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lightning : Tile {

	private int final_damage
	{
		get{
			return (int) (10 * Stats.Value) + (int) Player.SpellValue;
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

		GameObject part = EffectManager.instance.PlayEffect(transform, Effect.Lightning);
		CameraUtility.instance.ScreenShake((float)Stats.Value/10, Time.deltaTime* 15);
		AudioSource aud = PlayAudio("cast", 0.75F);
		aud.GetComponent<DestroyTimer>().Timer = 1.0F;
		yield return new WaitForSeconds(GameData.GameSpeed(0.35F));

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
		int collect = ChanceEngine.Index(onScreen_chances.ToArray());
		GENUS col = onScreen[collect];
		particles = new List<GameObject>();
		Tile target = TileMaster.Tiles[Random.Range(0,TileMaster.Grid.Size[0]), Random.Range(0,TileMaster.Grid.Size[1])];
		while(target == null || target.Genus != col)
		{
			target = TileMaster.Tiles[Random.Range(0,TileMaster.Grid.Size[0]), Random.Range(0,TileMaster.Grid.Size[1])];
		}
		target.SetState(TileState.Selected, true);
		
		part = EffectManager.instance.PlayEffect(target.transform, Effect.Lightning);
		particles.Add(part);
		CameraUtility.instance.ScreenShake((float)Stats.Value/5, Time.deltaTime* 15);
		MatchContainer m = TileMaster.instance.FloodCheck(target, true);
		yield return null;

		foreach(Tile t in m.Tiles)
		{
			to_collect.Add(t);
			t.SetState(TileState.Selected, true);
			yield return null;
		}

		yield return new WaitForSeconds(GameData.GameSpeed(0.2F));

		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
		particles.Clear();

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
			
		yield return new WaitForSeconds( GameData.GameSpeed(0.2F));
	}

}