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
			_Tile.InitStats.isAlerted = true;
			MiniAlertUI m = UIManager.instance.MiniAlert(_Tile.transform.position, " !", 150, GameData.instance.BadColour);

			yield return new WaitForSeconds(GameData.GameSpeed(0.05F));
		}
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.InitStats.isAlerted = false;
	}
}
