using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Collector : Tile {
	public string Type;
	private int damage = 1;
	public override StCon [] Description
	{
		get{

			return new StCon[]{
				new StCon("Collects all " + 
							(Genus != GENUS.ALL ? GameData.ResourceLong(Genus) : "") + 
							" " + Type + " tiles.", GameData.Colour(Genus)),
				new StCon("Deals ", Color.white, false),
				new StCon(damage + " damage", GameData.Colour(Genus))
			};
		}
	}

	public override void Update()
	{
		base.Update();
		if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, GameData.Colour(Genus), 0.6F);
	}


	public override IEnumerator BeforeMatch(bool original)
	{
		if(isMatching) yield break;
		isMatching = true;
		float part_time = 0.3F;

		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();
		List<GameObject> particles = new List<GameObject>();

		SPECIES spec = TileMaster.Types[Type];

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				Tile tile = _tiles[x,y];
				if(tile == null) continue;
				if(tile.isMatching) continue;
				if(tile.IsType(spec) && tile.IsGenus(Genus, (Genus==GENUS.ALL))) 
				{
					to_collect.Add(tile);
				}
			}
		}
		
		foreach(Tile child in to_collect)
		{
			GameObject part;
			if(child == null) continue;
			if(child.Type.isEnemy)
			{
				child.InitStats.TurnDamage += damage;
				part = EffectManager.instance.PlayEffect(this.transform, Effect.Force, "", GameData.instance.GetGENUSColour(Genus));
				MoveToPoint mp = part.GetComponent<MoveToPoint>();
				mp.enabled = true;
				mp.SetTarget(child.transform.position);
				mp.SetPath(0.3F, 0.02F);
				mp.SetMethod(() => {
						if(child != null) EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
					});
				}
			if(child.isMatching) continue;

			child.SetState(TileState.Selected, true);
			part = EffectManager.instance.PlayEffect(child.transform, Effect.Shiny, "", GameData.instance.GetGENUSColour(child.Genus));
			part.transform.parent = child.transform;
			yield return new WaitForSeconds(Time.deltaTime * 2);
		}

		//yield return new WaitForSeconds(GameData.GameSpeed(part_time));
		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.RemoveTileToMatch(this);
		to_collect.Add(this);
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield break;
	}
}
