using UnityEngine;
using System.Collections;

public class Barbarian : Class {

	TileChance health;

	private Slot manapower;
	private int _currentmanapower;

	// Use this for initialization
	public override void StartClass () {
		
		/*ClassUpgrade a = new ClassUpgrade((int val) => {InitStats._HealthMax += 10 * val;});
		a.BaseAmount = 10;
		a.Rarity = Rarity.Common;
		a.Name = "Health Max";
		a.ShortName = "HP MAX";
		a.Description = " Maximum Health";
		a.Prefix = "+";*/

		health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.4F;
		InitStats.TileChances.Add(health);

		TileChance sword = new TileChance();
		sword.Genus = GameData.ResourceLong(Genus);
		sword.Type = "bomb";
		sword.Chance = 0.2F;
		InitStats.TileChances.Add(sword);

		base.StartClass();
	}



	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["arcane"], g, 1, points);
				(TileMaster.Tiles[x,y] as Swapper).SetArgs("", "Enemy", "Genus", "");
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["collector"], g, 1, points);	
				(TileMaster.Tiles[x,y] as Collector).Type = "Health";
			break;
			case 2:	
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["stacker"], g, 1, points);	
				(TileMaster.Tiles[x,y] as Stacker).Type = "Health";
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["armour"], g, 1, points*2);	
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
		//	_currentmanapower = lvl;
		}
		switch(lvl)
		{
			case 1:
				manapower = AddMod("Heal", "5");
			break;
			case 2:
				
			break;
			case 3:
				manapower = AddMod("Shield", "0.2");
			break;
		}
		
	}

}
