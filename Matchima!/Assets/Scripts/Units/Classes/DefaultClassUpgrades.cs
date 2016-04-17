using UnityEngine;
using System.Collections;
using System;

public class DefaultClassUpgrades : MonoBehaviour {

	public static ClassUpgrade [] DefaultBoons (Stat init, GENUS Genus)
	{
			//+1 GENUS STAT
				ClassUpgrade genusstat = new ClassUpgrade((int val) => {init.GetResourceFromGENUS(Genus).StatCurrent += 3 * val;});
				genusstat.BaseAmount = 3;
				genusstat.Rarity = Rarity.Common;
				genusstat.Name = GameData.StatLong(Genus);
				genusstat.ShortName = GameData.Stat(Genus);
				genusstat.Description = " points added\nto " + GameData.StatLong(Genus);
				genusstat.Prefix = "+";

			//+5 HEALTH
				ClassUpgrade health_a = new ClassUpgrade((int val) => {init._HealthMax += 10 * val;});
				health_a.BaseAmount = 10;
				health_a.Rarity = Rarity.Rare;
				health_a.Name = "Health Max";
				health_a.ShortName = "HP MAX";
				health_a.Description = " Maximum Health";
				health_a.Prefix = "+";

			//+5 MANA
				ClassUpgrade mana_a = new ClassUpgrade((int val) => {init.MeterMax += 5 * val;});
				mana_a.BaseAmount = 5;
				mana_a.Rarity = Rarity.Rare;
				mana_a.Name = "Meter Max";
				mana_a.ShortName = "MP MAX";
				mana_a.Description = " Maximum Mana";
				mana_a.Prefix = "+";

			//+1 MAP X
				ClassUpgrade mapx = new ClassUpgrade((int val) => {init.MapSize.x += 1 * val;});
				mapx.BaseAmount = 1;
				mapx.Rarity = Rarity.Legendary;
				mapx.Name = "Map X";
				mapx.ShortName = "MAP X";
				mapx.Description = " increase to\nGrid X Size";
				mapx.Prefix = "+";

			//+1 MAP Y
				ClassUpgrade mapy = new ClassUpgrade((int val) => {init.MapSize.y += 1* val;});
				mapy.BaseAmount = 1;
				mapy.Rarity = Rarity.Legendary;
				mapy.Name = "Map Y";
				mapy.ShortName = "MAP Y";
				mapy.Description = " increase to\nGrid Y Size";
				mapy.Prefix = "+";

			//-1 MATCH NUMBER MODIFIER
				ClassUpgrade matchmod = new ClassUpgrade((int val) => {init.MatchNumberModifier -= 1 * val;});
				matchmod.BaseAmount = 1;
				matchmod.Rarity = Rarity.Legendary;
				matchmod.Name = "Tiles/Match";
				matchmod.ShortName = "MATCH";
				matchmod.Description = " less tiles required\nfor a match";
				matchmod.Prefix = "-";

			//+2% GENUS TILE
				TileChance genus = new TileChance();
				genus.Genus = GameData.ResourceLong(Genus);
				init.TileChances.Add(genus);
				ClassUpgrade g = new ClassUpgrade((int val) => {genus.Chance += 0.02F * val;});
				g.BaseAmount = 2;
				g.Rarity = Rarity.Magic;
				g.Name = GameData.ResourceLong(Genus) + " Tiles";
				g.ShortName = GameData.Resource(Genus) + "%";
				g.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Tiles";
				g.Prefix = "+";
				g.Suffix = "%";

			//+1 GENUS RES TILE VALUE
				TileChance genusres = new TileChance();
				genusres.Genus = GameData.ResourceLong(Genus);
				genusres.Type = "resource";
				init.TileChances.Add(genusres);
				ClassUpgrade genusresvalue = new ClassUpgrade((int val) => {genusres.Value += 1 * val;});
				genusresvalue.BaseAmount = 1;
				genusresvalue.Rarity = Rarity.Rare;
				genusresvalue.Name = GameData.ResourceLong(Genus) + " Mana Value";
				genusresvalue.ShortName = GameData.Resource(Genus);
				genusresvalue.Description = " increase to\n" + GameData.ResourceLong(Genus) + " mana tile values";
				genusresvalue.Prefix = "+";
				genusresvalue.Suffix = "";

			//+2% BOMB TILE
				TileChance bomb = new TileChance();
				bomb.Type = "bomb";
				init.TileChances.Add(bomb);
				ClassUpgrade bomb_a = new ClassUpgrade((int val) => {bomb.Chance += 0.02F * val;});
				bomb_a.Name = "Bomb Tiles";
				bomb_a.ShortName = "BMB";
				bomb_a.Description = " chance of Bomb";
				bomb_a.BaseAmount = 2;
				bomb_a.Rarity = Rarity.Rare;
				bomb_a.Prefix = "+";
				bomb_a.Suffix = "%";

			//+2% ARMOUR TILE
				TileChance armour = new TileChance();
				armour.Genus = GameData.ResourceLong(Genus);
				armour.Type = "armour";
				init.TileChances.Add(armour);
				ClassUpgrade armour_a = new ClassUpgrade((int val) => {armour.Chance += 0.03F * val;});
				armour_a.Name = GameData.ResourceLong(Genus) + " Armour Tiles";
				armour_a.ShortName = "ARM";
				armour_a.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Armour";
				armour_a.BaseAmount = 3;
				armour_a.Rarity = Rarity.Magic;
				armour_a.Prefix = "+";
				armour_a.Suffix = "%";

			//+5% ALPHA HEALTH TILE
				TileChance aph_health = new TileChance();
				aph_health.Genus = "Alpha";
				aph_health.Type = "health";
				init.TileChances.Add(aph_health);
				ClassUpgrade aph_health_a = new ClassUpgrade((int val) => {aph_health.Chance += 0.05F * val;});
				aph_health_a.Name = "Alpha Health Tiles";
				aph_health_a.ShortName = "APH";
				aph_health_a.Description = " chance of Alpha Health";

				aph_health_a.BaseAmount = 5;
				aph_health_a.Rarity = Rarity.Rare;
				aph_health_a.Prefix = "+";
				aph_health_a.Suffix = "%";

			//+5% ALPHA RES TILE
				TileChance aph_res = new TileChance();
				aph_res.Genus = "Alpha";
				aph_res.Type = "resource";
				init.TileChances.Add(aph_res);
				ClassUpgrade aph_res_a = new ClassUpgrade((int val) => {aph_res.Chance += 0.05F * val;});
				aph_res_a.Name = "Alpha Mana Tiles";
				aph_res_a.Description = " chance of Alpha Mana";
				aph_res_a.ShortName = "APH";
				aph_res_a.BaseAmount = 5;
				aph_res_a.Rarity = Rarity.Legendary;
				aph_res_a.Prefix = "+";
				aph_res_a.Suffix = "%";

			//+1% CHEST TILE
				TileChance chest = new TileChance();
				chest.Type = "chest";
				init.TileChances.Add(chest);
				ClassUpgrade chest_a = new ClassUpgrade((int val) => {chest.Chance += 0.01F * val;});
				chest_a.Name = "Chest Tiles";
				chest_a.ShortName = "CHST";
				chest_a.Description = " chance of Chest";
				chest_a.BaseAmount = 1;
				chest_a.Rarity = Rarity.Legendary;
				chest_a.Prefix = "+";
				chest_a.Suffix = "%";

				TileChance boon = new TileChance();
				boon.Type = "boon";
				init.TileChances.Add(boon);
				ClassUpgrade boon_a = new ClassUpgrade((int val) => {boon.Chance += 0.01F * val;});
				boon_a.Name = "Boon Tiles";
				boon_a.ShortName = "BOON";
				boon_a.Description = " chance of Boon";
				boon_a.BaseAmount = 1;
				boon_a.Rarity = Rarity.Legendary;
				boon_a.Prefix = "+";
				boon_a.Suffix = "%";

			return new ClassUpgrade[]{ 
				mapx,
				mapy,
				matchmod,
				g,
				mana_a,
				genusresvalue,
				genusstat,
				bomb_a,
				armour_a,
				aph_res_a,
				aph_health_a,
				health_a,
				chest_a,
				boon_a				
			};
	}


	public static ClassUpgrade [] DefaultCurses(Stat init, GENUS Genus)
	{

		//+1 MAP X
			ClassUpgrade mapx = new ClassUpgrade((int val) => {init.MapSize.x -= 1 * val;});
			mapx.BaseAmount = 1;
			mapx.Rarity = Rarity.Legendary;
			mapx.Name = "Map X";
			mapx.ShortName = "MAP X";
			mapx.Description = " decrease to\nGrid X Size";
			mapx.Prefix = "-";

		//+1 MAP Y
			ClassUpgrade mapy = new ClassUpgrade((int val) => {init.MapSize.y -= 1* val;});
			mapy.BaseAmount = 1;
			mapy.Rarity = Rarity.Legendary;
			mapy.Name = "Map Y";
			mapy.ShortName = "MAP Y";
			mapy.Description = " decrease to\nGrid Y Size";
			mapy.Prefix = "-";

		//-5 HEALTH
			ClassUpgrade health_a = new ClassUpgrade((int val) => {init._HealthMax -= 1 * val;});
			health_a.BaseAmount = 1;
			health_a.Rarity = Rarity.Common;
			health_a.Name = "Health Max";
			health_a.ShortName = "HP MAX";
			health_a.Description = " Maximum Health";
			health_a.Prefix = "-";


		//5% CURSE TILES
			TileChance curse = new TileChance();
			curse.Type = "curse";
			init.TileChances.Add(curse);
			ClassUpgrade curse_a = new ClassUpgrade((int val) => {curse.Chance += 0.01F * val;});
			curse_a.Name = "Curse Tiles";
			curse_a.ShortName = "CRSE";
			curse_a.Description = " chance of Curse Tiles";
			curse_a.BaseAmount = 1;
			curse_a.Rarity = Rarity.Legendary;
			curse_a.Prefix = "+";
			curse_a.Suffix = "%";


		//5% MINION TILES
			TileChance minion_c = new TileChance();
			minion_c.Type = "minion";
			init.TileChances.Add(minion_c);
			ClassUpgrade minion = new ClassUpgrade((int val) => {minion_c.Chance += 0.05F * val;});
			minion.Name = "Minion Tiles";
			minion.ShortName = "MIN";
			minion.Description = " chance of Minion Tiles";
			minion.BaseAmount = 5;
			minion.Rarity = Rarity.Magic;
			minion.Prefix = "+";
			minion.Suffix = "%";

			
		
		return new ClassUpgrade [] 
		{
			mapx,
			mapy,
			health_a,
			curse_a,
			minion
		};
	}
}

[System.Serializable]
public class ClassUpgrade
{
	public string ShortName;
	public string Name = "UPGRADE";
	public string Description;
	public string Prefix, Suffix;

	public int BaseAmount;
	public float BaseRate = 1.0F;
	public Rarity Rarity;
	
	public string ValueString
	{
		get{
			return Prefix + (Value * BaseAmount) + Suffix;
		}
	}

	public StCon [] Tooltip
	{
		get
		{
			return new StCon [] {new StCon(ValueString + Description, Color.white)};
		}
	}

	public string CurrentString
	{
		get
		{
			return Prefix + (Current * BaseAmount) + Suffix;
		}
	}
	public bool IgnoreValue = false;
	public bool Coroutine = false;
	public void AddValue(int v) {Value += v;}
	public int Value = 1;
	public int Current;
	public bool SlotUpgrade;
	public Slot slotobj;
	//public delegate IEnumerator Upgrade();

	public IEnumerator Upgrade;
	public Action<int> _Action;

	public ClassUpgrade(IEnumerator method)
	{
		Upgrade = method;
	}

	public ClassUpgrade(Action<int> method)
	{
		_Action = method;
		Upgrade = Class.ActionUpgrade(this);
	}

	/*public bool Upgrade()
	{
		if(!IgnoreValue)
		{
			for(int i = 0; i < Value; i++)
			{
				Current ++;
				Up();
			}
		}
		else
		{
			Current ++;
			Up();
		}
		
			return true;
	}*/
	
}
