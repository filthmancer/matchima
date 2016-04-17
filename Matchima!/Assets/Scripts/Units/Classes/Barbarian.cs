using UnityEngine;
using System.Collections;

public class Barbarian : Class {

	TileChance bomb2, health;
	// Use this for initialization
	public override void StartClass () {
		
		ClassUpgrade a = new ClassUpgrade((int val) => {InitStats._HealthMax += 10 * val;});
		a.BaseAmount = 10;
		a.Rarity = Rarity.Common;
		a.Name = "Health Max";
		a.ShortName = "HP MAX";
		a.Description = " Maximum Health";
		a.Prefix = "+";

		ClassUpgrade spike = new ClassUpgrade((int val) => {InitStats.Spikes += 1 * val;});
		spike.BaseAmount = 1;
		spike.Rarity = Rarity.Magic;
		spike.Name = "Spikes";
		spike.ShortName = "SPK";
		spike.Description = " Damage dealt to enemies\nwhen they attack";
		spike.Prefix = "+";


		ClassUpgrade hpregen = new ClassUpgrade((int val) => {InitStats.Regen += 1 + (1 * (val/2));});
		hpregen.BaseAmount = 1;
		hpregen.Rarity = Rarity.Uncommon;
		hpregen.Name = "Health Regen";
		hpregen.ShortName = "HP RGN";
		hpregen.Description = " HP gained every turn";
		hpregen.Prefix = "+";

		bomb2 = new TileChance();
		bomb2.Genus = GameData.ResourceLong(Genus);
		bomb2.Type = "bomb";
		InitStats.TileChances.Add(bomb2);

		ClassUpgrade c = new ClassUpgrade((int val) => {bomb2.Chance += 0.02F * val;});
		c.Name = GameData.ResourceLong(Genus) + " Bomb Tiles";
		c.ShortName = "BMB";
		c.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Bomb Tiles";
		c.BaseAmount = 2;
		c.Rarity = Rarity.Uncommon;
		c.Prefix = "+";
		c.Suffix = "%";


		health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		InitStats.TileChances.Add(health);

		ClassUpgrade d = new ClassUpgrade((int val) => {health.Chance += 0.02F * val;});
		d.Name = GameData.ResourceLong(Genus) + " Health Tiles";
		d.ShortName = GameData.Resource(Genus) + "HP";
		d.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Health Tiles";
		d.BaseAmount = 2;
		d.Rarity = Rarity.Common;
		d.Prefix = "+";
		d.Suffix = "%";

		AddUpgrades(new ClassUpgrade[] {a,c, d, spike, hpregen});
		base.StartClass();
	}

	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["swapper"], g, 1, points);
				(TileMaster.Tiles[x,y] as Swapper).SetArgs("", "Enemy", "Genus", "");

				//.OverrideStartGenus("");
				//(TileMaster.Tiles[x,y] as Swapper).StartSpecies = "Enemy";
				//(TileMaster.Tiles[x,y] as Swapper).EndGenus = "Genus";
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["collector"], GENUS.ALL, 1, points);	
				(TileMaster.Tiles[x,y] as Collector).Type = "Health";
			break;
			case 2:	
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["stacker"], g, 1, points);	
				(TileMaster.Tiles[x,y] as Stacker).Type = "Health";
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["armour"], GENUS.RAND, 1, points*2);	
			break;
			case 4:

			break;
		}
		
	}


}
