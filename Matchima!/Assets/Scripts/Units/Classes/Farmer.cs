using UnityEngine;
using System.Collections;

public class Farmer : Class {

	public override void StartClass () {

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.15F;
		InitStats.TileChances.Add(health);

		TileChance lightning = new TileChance();
		lightning.Genus = GameData.ResourceLong(Genus);
		lightning.Type = "lightning";
		lightning.Chance = 0.1F;
		InitStats.TileChances.Add(lightning);

		PowerupSpell = GameData.instance.GetPowerup("Colour Swap", this);

		base.StartClass();
	}


}
