using UnityEngine;
using System.Collections;

public class Warden : Class {

	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["arcane"], GENUS.RAND, 1, points);
				(TileMaster.Tiles[x,y] as Arcane).InputGenus = "Genus";
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["lightning"], GENUS.RAND, 1, points);	
			break;
			case 2:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["lens"], GENUS.ALL, 1, points);	
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["cross"], GENUS.RAND, 1, points);	
			break;
			case 4:

			break;
		}
		
	}

	public override void StartClass () {

		TileChance bomb = new TileChance();
		bomb.Genus = GameData.ResourceLong(Genus);
		bomb.Type = "bomb";
		bomb.Chance = 0.2F;
		InitStats.TileChances.Add(bomb);


	//Charming guards
		//TileMaster.Types["guard"].Effects.Add(new TileEffectInfo("Charm", -1));

		PowerupSpell = GameData.instance.GetPowerup("Calldown", this);

		base.StartClass();	
	}

}
