using UnityEngine;
using System.Collections;

public class CurseTile : Tile {

	private UpgradeGroup Upgrades;
	private ClassUpgrade FinalUpgrade;
	private int Upgrade_num = 1;

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Contains a Curse for " + Player.Classes[(int)Genus].Name, GameData.Colour(Genus))};
		}
	}

	public override bool Match(int resource)
	{
		if(isMatching) return true;
		isMatching = true;

		CheckStats();
		Stats.Value *=  resource;

	//CHANGE ITEM STATS BASED ON VALUE
		Class target = Player.Classes[(int)Genus];
		Upgrades = new UpgradeGroup(target, target.RollCurses(Upgrade_num, Stats.Value));

		bool canskip = true;
		for(int i = 0; i < Upgrades.Upgrades.Length; i++)
		{
			if(!Upgrades.Upgrades[i].SlotUpgrade) canskip = false;
		}
		Upgrades.CanSkip = canskip;

		Player.instance.PickupUpgrade(Upgrades);
		//FinalUpgrade = target.RollUpgrades(1, Stats.Value);
		//Player.instance.
		CollectThyself(true);
		TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
		return true;
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);

		int diff = (int) InitStats.value_soft - InitStats.Value;
		if(diff != 0)
		{
			InitStats.Value = (int) InitStats.value_soft;
			CheckStats();
			SetSprite();
		}
	}
}
