using UnityEngine;
using System.Collections;

public class Attack : Tile {

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("+" + Stats.AttackPower + " Attack Power", GameData.Colour(GENUS.DEX),true, 40),
								new StCon("+" + Stats.SpellPower + " Spell Power", GameData.Colour(GENUS.WIS), true, 40)};
		}
	}

	public override void Setup(GridInfo g, int x, int y, int scale, TileInfo inf, int value_inc)
	{
		base.Setup(g, x, y, scale, inf, value_inc);
		InitStats.Attack = Stats.Value;
		CheckStats();
	}
}
