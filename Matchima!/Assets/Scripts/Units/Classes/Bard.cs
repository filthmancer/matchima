using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bard : Class {

	public override void StartClass () {

		TileChance harp = new TileChance();
		harp.Genus = GameData.ResourceLong(Genus);
		harp.Type = "harp";
		harp.Chance = 0.13F;
		InitStats.TileChances.Add(harp);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.1F;
		InitStats.TileChances.Add(health);

		PowerupSpell = GameData.instance.GetPowerup("Lullaby", this);

		base.StartClass();
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
				new Upgrade("Soldier's", "% chance\n of Health", 0.6F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of harp", 0.6F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "harp", 0.1F + 0.03F * value));}, 3, 10
					),

				new OnMatchUpgrade("Charming", "% Chance to\nCharm Enemy",
								0.4F, ScaleType.GRADIENT, 4.5F,
				(Stat s, float value) => {
					ModContainer.CastTileEffect(value, "enemy", "Charm");
				}),

				new OnMatchUpgrade("Sleepy", "% Chance to\nSleep Enemy",
								0.4F, ScaleType.GRADIENT, 4.5F,
				(Stat s, float value) => {
					ModContainer.CastTileEffect(value, "enemy", "Sleep");
				})
			};
		}
	}

	public override Upgrade [] Curses
	{
		get
		{
 			return new Upgrade [] 
 			{
 				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10)
 			};
		}
	}
}
