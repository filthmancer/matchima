using UnityEngine;
using System.Collections;

public class Thief : Class {

	TileChance attack, enemy;
	public override void StartClass () {
		
		ClassUpgrade atk = new ClassUpgrade((int val) => {InitStats._Attack += (int) (1.0F * val);});
		atk.Name = "Attack";
		atk.ShortName = "ATK";
		atk.Description = " Attack";
		atk.BaseAmount = 1;
		atk.BaseRate = 0.3F;
		atk.Prefix = "+";
		atk.Rarity = Rarity.Uncommon;

		ClassUpgrade leech = new ClassUpgrade((int val) => {InitStats.Leech += 1 + (1 * (val/2));});
		leech.BaseAmount = 1;
		leech.Rarity = Rarity.Uncommon;
		leech.Name = "Health Leech";
		leech.ShortName = "HP LCH";
		leech.Description = " HP gained from\nattacking an enemy";
		leech.Prefix = "+";


		attack = new TileChance();
		attack.Genus = "Alpha";
		attack.Type = "sword";
		InitStats.TileChances.Add(attack);

		ClassUpgrade b = new ClassUpgrade((int val) => {attack.Chance += 0.02F * val;});
		b.BaseAmount = 2;
		b.Name = "Alpha Sword Tiles";
		b.ShortName = "SWRD";
		b.Description = " chance of Alpha Sword Tiles";
		b.Prefix = "+";
		b.Suffix = "%";	
		b.Rarity = Rarity.Uncommon;

		enemy = new TileChance();
		enemy.Type = "minion";
		InitStats.TileChances.Add(enemy);

		ClassUpgrade en = new ClassUpgrade((int val) => {enemy.Chance += 0.05F * val;});
		en.BaseAmount = 5;
		en.Name = " Minion Tiles";
		en.ShortName = "NMY";
		en.Description = " chance of Minion Tiles";
		en.Prefix = "+";
		en.Suffix = "%";	
		en.Rarity = Rarity.Rare;

		AddUpgrades(new ClassUpgrade[] {atk, b, leech, en});


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
}
