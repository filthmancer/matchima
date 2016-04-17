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
				new StCon("Collects all of a\nrandom tile type"),
				new StCon("Deals ", Color.white, false),
				new StCon(final_damage + " damage", GameData.Colour(Genus))
			};
		}
	}

	
	public override IEnumerator BeforeMatch()
	{
		if(isMatching) yield break;
		isMatching = true;
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<TileInfo> onScreen = new List<TileInfo>();
		List<float> onScreen_chances = new List<float>();
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				Tile tile = _tiles[x,y];
				bool add = true;
				for(int i = 0; i < onScreen.Count; i++)
				{
					if(tile.IsType(onScreen[i]))
					{
						add = false;
						onScreen_chances[i] += 0.1F;
						break;
					}
				}
				if(add)
				{
					onScreen.Add(tile.Info);
					onScreen_chances.Add(0.1F);
				}
			}
		}

		int collect = ChanceEngine.Index(onScreen_chances.ToArray());
		TileInfo col = onScreen[collect];
		particles = new List<GameObject>();
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(0); y++)
			{
				Tile tile = _tiles[x,y];
				if(tile.IsType(col))
				{
					tile.SetState(TileState.Selected, true);
					to_collect.Add(tile);
					GameObject part = EffectManager.instance.PlayEffect(tile.transform, Effect.Lightning);
					particles.Add(part);
				}
			}
		}
		yield return new WaitForSeconds(1.0F);

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
			} 
			else if(child.IsType("","Altar"))
			{
				TileMaster.instance.ReplaceTile(child.Point.Base[0], child.Point.Base[1], TileMaster.Types["Scarecrow"]);
			}					
		}

		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.AddTilesToMatch(to_collect.ToArray());
			
		yield break;
	}
}