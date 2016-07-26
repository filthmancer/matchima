using UnityEngine;
using System.Collections;

public class Stoneform : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Stoneform", GameData.Colour(GENUS.OMG), false),
				new StCon(DurationString)
			};
		}
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.AfterTurnEffect = true;
		initial_genus = _Tile.Genus;
		_Tile.ChangeGenus(GENUS.OMG);
	}

	public override void GetArgs(int _duration, params string [] args)
	{
		Duration = _duration;
		StoneformTime = GameData.StringToInt(args[0]);
		StoneformDelay = GameData.StringToInt(args[1]);
	}

	public int StoneformTime;
	public int StoneformDelay = 1;

	private int StoneformTime_current;
	private GENUS initial_genus;

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
			
			if(StoneformTime_current >= StoneformTime)
			{
				StoneformTime_current = 0;
				_Tile.ChangeGenus(initial_genus);
				yield return new WaitForSeconds(Time.deltaTime * 10);
			}
			else 
			{
				StoneformTime_current ++;
				if(_Tile.Genus != GENUS.OMG)
				{
					_Tile.ChangeGenus(GENUS.OMG);
					yield return new WaitForSeconds(Time.deltaTime * 10);
				}
			}
		}
		yield break;
	}
}
