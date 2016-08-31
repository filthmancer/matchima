using UnityEngine;
using System.Collections;

public class Attack : Tile {

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("+" + Stats.AttackPower + " AP", GameData.Colour(GENUS.DEX),false, 40),
								new StCon("+" + Stats.SpellPower + " SP", GameData.Colour(GENUS.WIS), true, 40)};
		}
	}

	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc)
	{
		base.Setup(x, y, scale, inf, value_inc);
		InitStats.Attack = Stats.Value;
		CheckStats();
	}
}
