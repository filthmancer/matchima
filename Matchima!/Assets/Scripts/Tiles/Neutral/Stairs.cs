using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stairs : Tile {

	public int ZoneIndex = 0;
	public bool TravelDown = true;
	private int IndexFinal
	{
		get
		{
			return (int) Mathf.Clamp(ZoneIndex, 0, (TravelDown ? GameManager.ZoneMap.NextBracket.Length-1 : GameManager.ZoneMap.LastBracket.Length-1));
		}
	}
	private Zone TargetZone
	{
		get{
			return (TravelDown ? GameManager.ZoneMap.NextBracket[IndexFinal] : GameManager.ZoneMap.LastBracket[IndexFinal]);
		}
		
	}
	public override StCon [] Description
	{
		get{
			return new StCon[] {
				new StCon("Travel to " + TargetZone.Name, GameData.Colour(Genus),true, 40)};
		}
	}

	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale,inf, value_inc);
		ZoneIndex = Random.Range(0,4);
		InitStats.Hits = 1;
	}

	public List<Tile> TakeTiles;
	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, "stairstravel", GameData.Colour(Genus));

		TakeTiles = new List<Tile>();
		bool tilesafter = false;
		foreach(Tile child in PlayerControl.instance.finalTiles)
		{
			child.isMatching = false;
			if(child == this)
			{
				tilesafter = true;
				continue;
			} 
			if(tilesafter)
			{
				TakeTiles.Add(child);
				MoveToPoint p = TileMaster.instance.CreateMiniTile(child.transform.position, this.transform, child);
				child.DestroyThyself();
				yield return new WaitForSeconds(GameData.GameSpeed(0.08F));
			}
		}

		yield return new WaitForSeconds(GameData.GameSpeed(0.3F));

		TileMaster.instance.AddTravelTiles(TakeTiles.ToArray());
		PlayerControl.instance.finalTiles.Clear();
		Player.instance.CompleteMatch = false;

		TileMaster.instance.ClearGrid(false);
		yield return new WaitForSeconds(GameData.GameSpeed(0.5F));
		Destroy(powerup);
		GameManager.instance.AdvanceZoneMap(IndexFinal);
		
		yield return null;
	}
}
