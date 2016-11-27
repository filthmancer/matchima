using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maturing : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Maturing", GameData.Colour(_Tile.Genus), false)
			};
		}
	}

	public int AddedValue = 1;
	public int SpawnPerTurn = 1;
	public override void GetArgs(int _duration, params string [] args)
	{
		base.GetArgs(_duration, args);
		SpawnPerTurn = GameData.StringToInt(args[0]);
		AddedValue = GameData.StringToInt(args[1]);
	}

	bool setupturn = false;
	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.AfterTurnEffect = true;
		setupturn = true;
	}

	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0 && !setupturn)
		{
			Tile [] nbours = _Tile.Point.GetNeighbours(true);
			List<Tile> final = new List<Tile>();
		
			foreach(Tile child in nbours)
			{
				if(!child.IsGenus(GENUS.OMG)) 
					final.Add(child);
			}

			for(int i = 0; i < SpawnPerTurn; i++)
			{
				if(final.Count <= 0) continue;

				
				int targ_num = Random.Range(0, final.Count);
				final[targ_num].AddValue(AddedValue);
				
				final.RemoveAt(targ_num);
				yield return new WaitForSeconds(Time.deltaTime*2);
			}
		}
		else setupturn = false;
	}
}
