using UnityEngine;
using System.Collections;

public class Thief : Class {

	TileChance attack;
	private Slot manapower;
	private int _currentmanapower = 100;
	public override void StartClass () {
		
		attack = new TileChance();
		attack.Genus = "Alpha";
		attack.Type = "sword";
		attack.Chance = 0.15F;
		InitStats.TileChances.Add(attack);

	/*	ClassUpgrade b = new ClassUpgrade((int val) => {attack.Chance += 0.02F * val;});
		b.BaseAmount = 2;
		b.Name = "Sword Tiles";
		b.ShortName = "SWRD";
		b.Description = " chance of Sword Tiles";
		b.Prefix = "+";
		b.Suffix = "%";	
		b.Rarity = Rarity.Uncommon;
		AddUpgrades(new ClassUpgrade[] {b});*/

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
		base.StartClass();
	}


	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = 1;//Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["force"], g, 1, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["bomb"], g, 1, points);	
			break;
			case 2:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["sword"], GENUS.ALL, 1, points);	
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["arcane"], g, 1, points);
				(TileMaster.Tiles[x,y] as Arcane).SetArgs("Genus", "", "", "Sword");
			break;
			case 4:

			break;
		}
		
	}

	public override void ManaPower(int lvl)
	{
		if(manapower != null)
		{
			Destroy(manapower.gameObject);
			AllMods.Remove(manapower);
			//_currentmanapower = lvl;
		}
		switch(lvl)
		{
			case 1:
				manapower = AddMod("Swap", "sword", "");
				
			break;
			case 2:
				manapower = AddMod("Backstab", "1.5");
			//	manapower[1] = AddMod("Slash", "1", "X");
			break;
			case 3:
				manapower = AddMod("Slash", "1", "X");
			break;
		}
	}

}
