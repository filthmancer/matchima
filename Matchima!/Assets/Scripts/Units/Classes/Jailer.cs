using UnityEngine;
using System.Collections;

public class Jailer : Class {

	// Use this for initialization
	public override void StartClass () {
		
		ClassUpgrade a = new ClassUpgrade((int val) => {InitStats._HealthMax += 5 * val;});
		a.BaseAmount = 5;
		a.Name = "Health: ";
		a.Prefix = "+";
		a.Rarity = Rarity.Common;
		
		AddUpgrades(new ClassUpgrade[] {a});
		base.StartClass();
	}
}
