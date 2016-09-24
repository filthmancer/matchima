using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wizard : Class {

	// Use this for initialization
	public override void StartClass () {

		TileChance bomb = new TileChance();
		bomb.Genus = GameData.ResourceLong(Genus);
		bomb.Type = "arcane";
		bomb.Chance = 0.15F;
		InitStats.TileChances.Add(bomb);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.08F;
		InitStats.TileChances.Add(health);

		PowerupSpell = GameData.instance.GetPowerup("Firestorm", this);

		base.StartClass();	
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
				new Upgrade("Sharp", " Spell", 0.7F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._Spell += 1 + (int)val;}, 1, 1),
				new Upgrade("Wise", "% Spell Power", 1.0F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.SpellPower += 0.2F * val;}, 20),
				new Upgrade("Healing", " MP Regen", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.MeterRegen += 1 + (int) val;}),
				new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 0.3F, 
					(Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),
				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s._HealthMax += 20 + (int)val*5;}, 5, 20),

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
					),

				new Upgrade("Cook's", " Map X", 0.2F, ScaleType.RANK, 0.4F,
							(Stat s, float value) => {
								s.MapSize.x += 1 + (int) (1 * value);},1,1
							),
				new Upgrade("Magellan's", " Map Y", 0.4F, ScaleType.RANK, 0.4F,
					(Stat s, float value) => {
						s.MapSize.y += 1 + (int) (1 * value);},1,1
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