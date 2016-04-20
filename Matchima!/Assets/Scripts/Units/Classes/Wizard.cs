using UnityEngine;
using System.Collections;

public class Wizard : Class {

TileChance wiz_cross;
TileChance wiz_arcane;
	// Use this for initialization
	public override void StartClass () {
		
		ClassUpgrade a = new ClassUpgrade((int val) => {InitStats.CooldownDecrease += 0.01F * val;});
		a.Name = "Cooldowns";
		a.ShortName = "CD%";
		a.Description = " spell cooldowns";
		a.BaseAmount = 1;
		a.Prefix = "-";
		a.Suffix = "%";
		a.Rarity = Rarity.Common;

		ClassUpgrade b = new ClassUpgrade((int val) => {InitStats.MagicPower += 1 * val;});
		b.BaseAmount = 1;
		b.Name = "Spell Power";
		b.ShortName = "SP";
		b.Description = " Spell Power.\nIncreases strength of spells";
		b.Prefix = "+";
		b.Rarity = Rarity.Uncommon;

		wiz_cross = new TileChance();
		wiz_cross.Genus = "Alpha";
		wiz_cross.Type = "cross";
		InitStats.TileChances.Add(wiz_cross);

		ClassUpgrade c = new ClassUpgrade((int val) => {wiz_cross.Chance += 0.01F * val;});
		c.Name = "Alpha Cross Tiles";
		c.ShortName = "CRS";
		c.BaseAmount = 1;
		c.Description = " chance of Alpha Cross Tiles";
		c.Prefix = "+";
		c.Suffix = "%";
		c.Rarity = Rarity.Magic;

		wiz_arcane = new TileChance();
		wiz_arcane.Genus = GameData.ResourceLong(Genus);
		wiz_arcane.Type = "arcane";
		InitStats.TileChances.Add(wiz_arcane);

		ClassUpgrade wiz_arcane_up = new ClassUpgrade((int val) => {wiz_arcane.Chance += 0.02F * val;});
		wiz_arcane_up.Name = GameData.ResourceLong(Genus) + " Arcane Tiles";
		wiz_arcane_up.ShortName = GameData.Resource(Genus) + " ARC";
		wiz_arcane_up.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Arcane Tiles";
		wiz_arcane_up.BaseAmount = 2;
		wiz_arcane_up.Prefix = "+";
		wiz_arcane_up.Suffix = "%";
		wiz_arcane_up.Rarity = Rarity.Uncommon;

		ClassUpgrade manamax = new ClassUpgrade((int val) => {InitStats.MeterMax += 5 * val;});
		manamax.Name = "Mana Max";
		manamax.ShortName = "MP MAX";
		manamax.Description = " Maximum Mana";
		manamax.BaseAmount = 5;
		manamax.Prefix = "+";
		manamax.Rarity = Rarity.Common;


		AddUpgrades(new ClassUpgrade[] {a,b,c, wiz_arcane_up, manamax});
		base.StartClass();	
	}


	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["bomb"], g, 1, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["lightning"], g, 1, points);	
			break;
			case 2:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["flame"], g, 1, points);	
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["cross"], g, 1, points);	
			break;
			case 4:

			break;
		}
		
	}

}
