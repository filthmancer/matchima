using UnityEngine;
using System.Collections;

public class Sleep : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Sleeping ", GameData.Colour(GENUS.DEX), false),
				new StCon(DurationString)
			};
		}
	}


	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0)
		{
			_Tile.InitStats.isAlerted = false;
		}
		else 
		{
			_OnDestroy();

			yield return new WaitForSeconds(GameData.GameSpeed(0.09F));
		}
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.InitStats.isAlerted = false;
	}

	public override void _OnDestroy()
	{
		base._OnDestroy();
		_Tile.OnAlert();
		
	}
}
