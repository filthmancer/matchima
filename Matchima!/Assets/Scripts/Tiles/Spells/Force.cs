using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Force : Tile {

	private int damage = 5;

	private int final_damage
	{get{
		CheckStats();
		return Stats.Value * damage;
	}}

	public override StCon [] Description
	{
		get{

			return new StCon[]{
				new StCon("Deals ", Color.white, false),
				new StCon(final_damage + " damage", GameData.Colour(Genus))
			};
		}
	}

	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		CheckStats();
		float part_time = 0.2F;

		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();
		List<GameObject> particles = new List<GameObject>();

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				Tile tile = _tiles[x,y];
	
				if(tile.Type.isEnemy) 
				{
					tile.SetState(TileState.Selected, true);
					to_collect.Add(tile);
				}
			}
		}

		if(to_collect.Count == 0) yield break;


		foreach(Tile child in to_collect)
		{
			GameObject part = EffectManager.instance.PlayEffect(this.transform, Effect.Force, "", GameData.instance.GetGENUSColour(Genus));
			MoveToPoint mp = part.GetComponent<MoveToPoint>();
			mp.enabled = true;
			mp.SetTarget(child.transform.position);
			mp.SetPath(0.3F, 0.02F);
			mp.SetMethod(() => {
					EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
				});

			particles.Add(part);
			yield return new WaitForSeconds(Time.deltaTime * 15);
		}

		yield return new WaitForSeconds(GameData.GameSpeed(part_time));

		foreach(Tile child in to_collect)
		{
			child.InitStats.TurnDamage += final_damage;
		}
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		
		//cooldown_time = cooldown;
		//activated = false;

		
		yield break;
	}
}
