using UnityEngine;
using System.Collections;

public class Farmer : Class {

	public override void StartClass () {

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.08F;
		InitStats.TileChances.Add(health);

		TileChance lightning = new TileChance();
		lightning.Genus = GameData.ResourceLong(Genus);
		lightning.Type = "lightning";
		lightning.Chance = 0.07F;
		InitStats.TileChances.Add(lightning);

		PowerupSpell = GameData.instance.GetPowerup("Colour Swap", this);

		base.StartClass();
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
							
				new Upgrade("Soldier's", "% chance\n of Health", 0.1F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade("Bombers's", "% chance\n of Lightning", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "lightning", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of Arcane", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "arcane", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of flame", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "flame", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of Beam", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "cross", 0.1F + 0.03F * value));}, 3, 10
					)
			};
		}
	}

	public override Upgrade [] Curses
	{
		get
		{
 			return new Upgrade [] 
 			{
 				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10),
 				new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 0.12F, (Stat s, float val) => {s._Attack -= 1 + (int)val;}, -1, -1)
 			};
		}
	}
}
