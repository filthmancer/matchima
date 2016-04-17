using UnityEngine;
using System.Collections;

public class Merchant : Class {

	public override void StartClass () {
		
		ClassUpgrade a = new ClassUpgrade((int val) => {InitStats.CostDecrease += 0.01F * val;});
		a.Name = "Costs";
		a.BaseAmount = 1;
		a.Prefix = "-";
		a.Suffix = "%";
		a.Rarity = Rarity.Common;

		ClassUpgrade b = new ClassUpgrade((int val) => {InitStats.ComboBonus += 0.02F * val;});
		b.BaseAmount = 2;
		b.Name = "Combo Bonus";
		b.Prefix = "+";
		b.Suffix = "%";	
		b.Rarity = Rarity.Uncommon;

		ClassUpgrade c = new ClassUpgrade((int val) => {InitStats._Attack += 1 * val;});
		c.BaseAmount = 1;
		c.Name = "Attack";
		c.Prefix = "+";
		c.Rarity = Rarity.Common;

		ClassUpgrade manaregen = new ClassUpgrade((int val) => {InitStats.ManaRegen += 1 * val;});
		manaregen.Name = "MP Regen";
		manaregen.BaseAmount = 1;
		manaregen.Prefix = "+";
		manaregen.Rarity = Rarity.Uncommon;

		AddUpgrades(new ClassUpgrade[] {a,b, manaregen});
		base.StartClass();
	}
}
