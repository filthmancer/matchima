using UnityEngine;
using System.Collections;

public class Ravenous : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Ravenous", GameData.Colour(GENUS.STR), false),
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
			Tile targ = TileMaster.RandomTile;
			while(targ == _Tile) targ = TileMaster.RandomTile;

			int val = targ.GetValue();
			_Tile.AddValue(val);
			targ.DestroyThyself();

			yield return new WaitForSeconds(Time.deltaTime * 10);
		}
		yield break;
	}
}
