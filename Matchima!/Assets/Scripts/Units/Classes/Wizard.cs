﻿using UnityEngine;
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
				new Upgrade(1,0, "% chance\n of Health", 0.1F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade(1,1, "% chance\n of Lightning", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "lightning", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade(1,2, "% chance\n of Arcane", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "arcane", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade(1,3, "% chance\n of flame", 0.8F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "flame", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade(1,4, "% chance\n of Beam", 0.8F, ScaleType.GRADIENT, 1.0F,
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
 				new Upgrade(-2,0, " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10),
 				new Upgrade(-2,1, " Attack", 1.0F, ScaleType.GRADIENT, 0.12F, (Stat s, float val) => {s._Attack -= 1 + (int)val;}, -1, -1)
 			};
		}
	}
}