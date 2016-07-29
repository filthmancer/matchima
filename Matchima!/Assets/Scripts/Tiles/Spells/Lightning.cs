using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lightning : Tile {

	private int LDamage = 10;
	private int added_damage;
	private int final_damage
	{
		get{
			return (int) (LDamage + added_damage) * Stats.Value;
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
		CameraUtility.instance.ScreenShake((float)Stats.Value/5, Time.deltaTime* 15);
		AudioSource aud = PlayAudio("cast");
		aud.GetComponent<DestroyTimer>().Timer = 5.0F;
		yield return new WaitForSeconds(GameData.GameSpeed(0.35F));

		List<GENUS> onScreen = new List<GENUS>();
		List<float> onScreen_chances = new List<float>();
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				Tile tile = _tiles[x,y];
				if(tile.Genus == this.Genus) continue;
				bool add = true;
				for(int i = 0; i < onScreen.Count; i++)
				{
					if(tile.Genus == onScreen[i])
					{
						add = false;
						onScreen_chances[i] += 0.1F;
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

		int collect = ChanceEngine.Index(onScreen_chances.ToArray());
		GENUS col = onScreen[collect];
		particles = new List<GameObject>();
		Tile target = TileMaster.Tiles[Random.Range(0,TileMaster.Grid.Size[0]), Random.Range(0,TileMaster.Grid.Size[1])];
		while(target.Genus != col)
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

		yield return new WaitForSeconds(GameData.GameSpeed(0.35F));

		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
		particles.Clear();

		foreach(Tile child in to_collect)
		{
			if(child.Type.isEnemy)
			{
				child.InitStats.TurnDamage += final_damage;
				child.PlayAudio("Hit");
				EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
			} 
			if(child.IsType("","Altar"))
			{
				TileMaster.instance.ReplaceTile(child.Point.Base[0], child.Point.Base[1], TileMaster.Types["Scarecrow"]);
			}
			if(child.IsType("", "chicken"))
			{
				TileMaster.instance.ReplaceTile(child.Point.Base[0], child.Point.Base[1], TileMaster.Types["health"]);
			}			
		}

		//yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
			
		yield break;
	}

}