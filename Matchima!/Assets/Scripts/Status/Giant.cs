using UnityEngine;
using System.Collections;

public class Giant : TileEffect {

	public int HitsMultiplier = 10;
	public int ScaleIncrease = 1;
	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Giant ", GameData.Colour(GENUS.DEX), false),
				new StCon(DurationString)
			};
		}
	}



	public override void Setup(Tile t)
	{
		base.Setup(t);
		AudioManager.instance.PlayClipOn(t.transform, "Status", "GiantRoar");
		_Tile = TileMaster.instance.ReplaceTile(_Tile, TileMaster.Types[_Tile.Info._TypeName], _Tile.Genus, _Tile.Point.Scale + ScaleIncrease, 0);
		_Tile.InitStats.Hits *= HitsMultiplier;
		//_Tile.InitStats.isAlerted = false;
	}

	public override void _OnDestroy()
	{
		base._OnDestroy();
		TileMaster.instance.ReplaceTile(_Tile,  TileMaster.Types[_Tile.Info._TypeName], _Tile.Genus, _Tile.Point.Scale - ScaleIncrease, 0);
	}
}
