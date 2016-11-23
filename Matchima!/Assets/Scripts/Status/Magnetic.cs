using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Magnetic : TileEffect {

	public string Species;
	public string Genus;
	public int PerTurn = 0;

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Attracts " + Genus + " " + Species, GameData.Colour(GENUS.DEX), false),
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

	public override void GetArgs(int _duration, params string [] args)
	{
		base.GetArgs(_duration, args);
		Genus = args[0];
		Species = args[1];
		PerTurn = GameData.StringToInt(args[2]);
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
			while(_Tile.isAttacking) yield return null;

			

			List<Tile> emptypoints= new List<Tile>();
			emptypoints.AddRange(_Tile.Point.GetNeighbours(false));

			for(int i = 0; i < emptypoints.Count; i++)
			{
				if(!emptypoints[i].IsType("resource"))
				{
					emptypoints.RemoveAt(i);
					i--;
				}
			}

			if(emptypoints.Count == 0) yield break;
			Tile targ = TileMaster.RandomTileOfType(Species);
			if(targ == null) yield break;
			EffectManager.instance.PlayEffect(_Tile.transform, "shout");

			yield return new WaitForSeconds(Time.deltaTime * 15);

			for(int i = 0; i < PerTurn; i++)
			{
				if(emptypoints.Count == 0) continue;
				targ = TileMaster.RandomTileOfType(Species);
				if(targ == null) break;
				if(targ.HasFlag("Magnetic")) continue;
				targ.AddFlag("Magnetic");
				int rand = Random.Range(0, emptypoints.Count);
				yield return StartCoroutine(targ.MoveToPoint(emptypoints[rand].x, emptypoints[rand].y, true, 19, 0.3F));
				emptypoints.RemoveAt(rand);
				yield return new WaitForSeconds(Time.deltaTime *4);
			}

			yield return new WaitForSeconds(Time.deltaTime * 20);
		}
		yield break;
	}
}
