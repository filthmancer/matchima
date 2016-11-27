using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stairs : Tile {

	public int ZoneIndex = 0;
	public bool TravelDown = true;
	public bool Doorway = false;
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

	public override void Setup(GridInfo g, int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(g, x,y,scale,inf, value_inc);
		ZoneIndex = Random.Range(0,4);
		InitStats.Hits = 1;
		Doorway = false;
	}

	public List<Tile> TakeTiles;
	public override IEnumerator BeforeMatch(Tile Controller)
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
			if(tilesafter && !child.Controllable)
			{
				TakeTiles.Add(child);
				MoveToPoint p = TileMaster.instance.CreateMiniTile(child.transform.position, this.transform, child);
				child.DestroyThyself();
				yield return new WaitForSeconds(GameData.GameSpeed(0.08F));
			}
		}

		for(int i = 0; i < TileMaster.Controllers.Length; i++)
		{
			//if(TakeTiles.Contains(TileMaster.Controllers[i])) continue;
			//TakeTiles.Add(TileMaster.Controllers[i]);
			MoveToPoint p = TileMaster.instance.CreateMiniTile(TileMaster.Controllers[i].transform.position, this.transform, TileMaster.Controllers[i]);
			TileMaster.Controllers[i].gameObject.SetActive(false);
			yield return new WaitForSeconds(GameData.GameSpeed(0.08F));
		}

		yield return new WaitForSeconds(GameData.GameSpeed(0.3F));

		TileMaster.instance.AddTravelTiles(TakeTiles.ToArray());
		PlayerControl.instance.selectedTiles.Clear();
		PlayerControl.instance.finalTiles.Clear();
		Player.instance.CompleteMatch = false;
		GameManager.OverrideMatch = true;

		if(Doorway) yield return StartCoroutine(TileMaster.instance.MoveToRoom(Direction, null, Genus));
		else yield return StartCoroutine(TileMaster.instance.MoveToLevel());
		
		yield return new WaitForSeconds(GameData.GameSpeed(0.5F));
		Destroy(powerup);

		//GameManager.instance.AdvanceZoneMap(IndexFinal);
		
		yield return null;
	}


	IntVector Direction;
	public void SetDirection(IntVector n)
	{
		Direction = n;
	}
}
