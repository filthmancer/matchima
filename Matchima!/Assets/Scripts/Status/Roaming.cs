using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Roaming : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Roaming", GameData.Colour(GENUS.DEX), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		if(Duration > 0) Duration -= 1;
		
		return Duration == 0;
	}

	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.AfterTurnEffect = true;
	}

	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0)
		{
			if(_Tile.Destroyed) yield break;
			if(_Tile.HasEffect("Sleep")) yield break;
			while(!TileMaster.AllLanded) 
			{
				if(_Tile == null) yield break;
				if(_Tile.isMatching || _Tile.Destroyed) yield break;
				yield return null;
			}
			Tile [] nbours = _Tile.Point.GetNeighbours();
			List<Tile> final = new List<Tile>();
			foreach(Tile child in nbours)
			{
				if(child.Point.BaseY == 0) continue;
				if(child.Point.BaseY < _Tile.Point.BaseY) continue;
				if(!child.isMatching && child.Point.Scale == _Tile.Point.Scale) final.Add(child);
			}
			if(final.Count == 0) yield break;
			Tile target = final[Random.Range(0, final.Count)];
			TileMaster.instance.SwapTiles(target, _Tile);

			yield return new WaitForSeconds(Time.deltaTime * 10);
		}
		yield break;
	}

}
