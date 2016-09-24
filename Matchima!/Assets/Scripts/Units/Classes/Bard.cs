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
				new Upgrade("Spiked", " Spikes", 1.0F, ScaleType.GRADIENT,1.0F, (Stat s, float val) => {s.Spikes += 1 + (int)val;}, 1, 1),
				new Upgrade("Healing", " MP Regen", 0.2F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.MeterRegen += 1 + (int) val;}),
				new Upgrade("Healing", " HP Regen", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}, 1, 1),
				new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 0.5F, 
					(Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),
				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s._HealthMax += 20 + (int)val*5;}, 5, 20),
				new Upgrade("Wise", "% Spell Power", 0.5F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.SpellPower += 0.2F * val;}, 20),

				new Upgrade("Soldier's", "% chance\n of Health", 0.6F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of harp", 0.6F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "harp", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade("Cook's", " Map X", 0.3F, ScaleType.RANK, 0.4F,
							(Stat s, float value) => {
								s.MapSize.x += 1 + (int) (1 * value);},1,1
							),
				new Upgrade("Magellan's", " Map Y", 0.3F, ScaleType.RANK, 0.4F,
					(Stat s, float value) => {
						s.MapSize.y += 1 + (int) (1 * value);},1,1
						),

				new OnMatchUpgrade("Charming", "% Chance to\nCharm Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					ModContainer.CastTileEffect(value, "enemy", "Charm");
				}),

				new OnMatchUpgrade("Sleepy", "% Chance to\nSleep Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					ModContainer.CastTileEffect(value, "enemy", "Sleep");
				}),
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
