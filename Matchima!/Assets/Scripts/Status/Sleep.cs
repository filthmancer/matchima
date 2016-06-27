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

			yield return new WaitForSeconds(GameData.GameSpeed(0.05F));
		}
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.InitStats.isAlerted = false;
	}

	public override void _OnDestroy()
	{
		_Tile.InitStats.isAlerted = true;
		MiniAlertUI m = UIManager.instance.MiniAlert(_Tile.transform.position, "!", 180, Color.black);
		m.Txt[0].outlineColor = GameData.Colour(_Tile.Genus);
	}
}
