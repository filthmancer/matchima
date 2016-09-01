using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Thief : Class {

	TileChance attack;
	private Slot manapower;
	private int _currentmanapower = 100;
	public override void StartClass () {

		TileChance attackgen = new TileChance();
		attackgen.Genus = "Alpha";
		attackgen.Type = "sword";
		attackgen.Chance = 0.1F;
		InitStats.TileChances.Add(attackgen);

		TileChance chest = new TileChance();
		chest.Genus = GameData.ResourceLong(Genus);
		chest.Type = "chest";
		chest.Chance = 0.01F;
		InitStats.TileChances.Add(chest);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.05F;
		InitStats.TileChances.Add(health);
	

		Barbarian barb = null;
		foreach(Class child in Player.Classes) {if(child is Barbarian) barb = child as Barbarian;}
		if(barb != null)
		{
			QuoteGroup barb_disgust = new QuoteGroup("Thief-Barb: Disgust");
			barb_disgust.AddQuote("Keep that brute away from me!", this, false, 2.5F);
			barb_disgust.AddQuote("You watch your tongue, shadow girl.", barb, false, 2.5F);
			//barb_disgust.Unlocked = () => {foreach(Class child in Player.Classes){if(child.Name == "Barbarian") return true;} return false;};
			Quotes.StartQuotes.Add(barb_disgust);
		}

		PowerupSpell = GameData.instance.GetPowerup("Throw Knives", this);

		base.StartClass();
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
				new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 1.0F, 
					(Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),
				new Upgrade("Wise", "%\n Attack Power", 0.4F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.AttackPower += 0.2F * val;}, 20),

				
				new Upgrade("Soldier's", "% chance of\ngreen Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade("Soldier's", "% chance of\nred Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("red", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade("Soldier's", "% chance of\nblue Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("blue", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade("Soldier's", "% chance of\nyellow Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade("Soldier's", "% chance of\nalpha Sword", 0.15F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("alpha", "sword", 0.04F + 0.03F * value));}, 3, 4
				),


				new Upgrade("Lucky", "% Death Save Chance", 0.6F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) =>
					{
						s.DeathSaveChance += 0.05F + (value * 0.03F);
					}, 3, 5),


				new Upgrade("Cook's", " Map X", 0.3F, ScaleType.RANK, 0.4F,
					(Stat s, float value) => {
						s.MapSize.x += 1 + (int) (1 * value);},1,1
					),
				new Upgrade("Magellan's", " Map Y", 0.3F, ScaleType.RANK, 0.4F,
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
 				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10)
 			};
		}
	}

}
