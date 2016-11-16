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

	public override void GetTile(Tile t)
	{
		_Tile = t;
		_Tile.AddEffect("Spawning", -1, "alpha", "sword", "0.1");
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{			
				new Upgrade(1,0, "% chance of\ngreen Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade(1,1, "% chance of\nred Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("red", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade(1,2, "% chance of\nblue Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("blue", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade(1,3, "% chance of\nyellow Sword", 0.3F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "sword", 0.04F + 0.03F * value));}, 3, 4
				),
				new Upgrade(1,4, "% chance of\nalpha Sword", 0.15F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("alpha", "sword", 0.04F + 0.03F * value));}, 3, 4
				),


				new Upgrade(1,5, "% Death Save Chance", 0.6F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) =>
					{
						s.DeathSaveChance += 0.05F + (value * 0.03F);
					}, 3, 5)
			};
		}
	}

	public override Upgrade [] Curses
	{
		get
		{
 			return new Upgrade [] 
 			{
 				new Upgrade(-2,0, " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10)
 			};
		}
	}

}
