using UnityEngine;
using System.Collections;

public class Fragile : TileEffect {

	public int [] initial_point;
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



	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			if(_Tile.Point.Base[0] != initial_point[0] || _Tile.Point.Base[1] != initial_point[1])
			{
				_Tile.DestroyThyself();
			}
		}
		return Duration == 0;
	}

	public override void StatusEffect()
	{
		if(_Tile.Point.Base != initial_point)
		{
			_Tile.DestroyThyself();
		}
	}
}
