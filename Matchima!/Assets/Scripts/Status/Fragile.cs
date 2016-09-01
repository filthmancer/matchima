using UnityEngine;
using System.Collections;

public class Fragile : TileEffect {

	public int [] initial_point;
	bool after_first = false;
	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Fragile", GameData.Colour(GENUS.WIS), false),
				new StCon(DurationString)
			};
		}
	}

	public override void Setup(Tile t)
	{
		base.Setup(t);
		initial_point = _Tile.Point.Base;
	}

	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0)
		{
			if(!after_first) 
			{
				initial_point = _Tile.Point.Base;
				after_first = true;
				yield break;
			}

			//print(_Tile.Point.Base[0] + ":" + _Tile.Point.Base[1] + "-" + initial_point[0] + ":" + initial_point[1]);
			if(_Tile.Point.Base[0] != initial_point[0] || _Tile.Point.Base[1] != initial_point[1])
			{
				UIManager.instance.MiniAlert(_Tile.transform.position, "BREAK!", 95, GameData.Colour(_Tile.Genus), 0.3F);
				yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
				_Tile.DestroyThyself();
			}
		}
	}
}
