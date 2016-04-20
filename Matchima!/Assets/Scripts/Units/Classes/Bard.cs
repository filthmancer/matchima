using UnityEngine;
using System.Collections;

public class Bard : Class {

	public override void StartClass () {

		//-1% ARMOUR DECAY
		ClassUpgrade armour = new ClassUpgrade((int val) => {InitStats.ArmourReductionRate -= 0.01F * val;});
		armour.BaseAmount = 1;
		armour.Rarity = Rarity.Common;
		armour.Name = "Armour Decay";
		armour.ShortName = "ARMD";
		armour.Description = " rate of armour breakage";
		armour.Prefix = "-";
		armour.Suffix = "%";

		//+1 SPELL POWER
		ClassUpgrade spellpower = new ClassUpgrade((int val) => {InitStats.MagicPower += 1 * val;});
		spellpower.BaseAmount = 1;
		spellpower.Name = "Spell Power";
		spellpower.ShortName = "SP";
		spellpower.Description = " Spell Power.\nIncreases strength of spells";
		spellpower.Prefix = "+";
		spellpower.Rarity = Rarity.Uncommon;


		//+3% GENUS HARP TILES
		TileChance harp = new TileChance();
		harp.Genus = GameData.ResourceLong(Genus);
		harp.Type = "harp";
		InitStats.TileChances.Add(harp);

		ClassUpgrade _harp = new ClassUpgrade((int val) => {harp.Chance += 0.01F * val;});
		_harp.BaseAmount = 1;
		_harp.Name = GameData.ResourceLong(Genus) + " Harp Tiles";
		_harp.ShortName = "HARP";
		_harp.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Harp Tiles";
		_harp.Prefix = "+";
		_harp.Suffix = "%";	
		_harp.Rarity = Rarity.Common;

		//+5 MANA
		ClassUpgrade mana_a = new ClassUpgrade((int val) => {InitStats.MeterMax += 5 * val;});
		mana_a.BaseAmount = 5;
		mana_a.Rarity = Rarity.Magic;
		mana_a.Name = "Mana Max";
		mana_a.ShortName = "MP MAX";
		mana_a.Description = " Maximum Mana";
		mana_a.Prefix = "+";

		//+1 MANA REGEN
		ClassUpgrade mana_regen = new ClassUpgrade((int val) => {InitStats.ManaRegen += 1 * val;});
		mana_regen.BaseAmount = 1;
		mana_regen.Rarity = Rarity.Rare;
		mana_regen.Name = "Mana Regen";
		mana_regen.ShortName = "MP RGN";
		mana_regen.Description = " Mana gained every turn";
		mana_regen.Prefix = "+";

		AddUpgrades(new ClassUpgrade[] {armour, spellpower});
		base.StartClass();
	}


	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["harp"], g, 1, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["collector"], g, 1, points);	
				(TileMaster.Tiles[x,y] as Collector).Type = "";
			break;
			case 2:

				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["ward"], g, 1, points);	
				TileMaster.Tiles[x,y].SetArgs("Healing", "1", "2", "5");
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["arcane"], g, 1, points/2);	
				TileMaster.Tiles[x,y].SetArgs("Genus", "resource", "", "Harp");
			break;
			case 4:

			break;
		}
		
	}
}
