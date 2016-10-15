using UnityEngine;
using System.Collections;

public class Warden : Class {

	public override void StartClass () {

		TileChance bomb = new TileChance();
		bomb.Genus = GameData.ResourceLong(Genus);
		bomb.Type = "flame";
		bomb.Chance = 0.15F;
		InitStats.TileChances.Add(bomb);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.1F;
		InitStats.TileChances.Add(health);


	//Charming guards
		//TileMaster.Types["guard"].Effects.Add(new TileEffectInfo("Charm", -1));

		PowerupSpell = GameData.instance.GetPowerup("Calldown", this);

		base.StartClass();	
	}

		public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
				new Upgrade(1,0, "% chance\n of Health", 0.4F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade(1,1, "% chance\n of Bomb", 0.4F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "bomb", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade(1,2, "% chance\n of flame", 0.4F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "flame", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade(1,3, "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 1.0F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance += 0.05F + (value * 0.03F);
						}, 3, 5),
				new Upgrade(1,4, " Tile Per Match", 1.0F, ScaleType.RANK, 0.0F,
					(Stat s, float value) =>
					{
						s.MatchNumberModifier -= 1;
						}, -1, -1)
			};
		}
	}

	public override Upgrade [] Curses
	{
		get
		{
 			return new Upgrade [] 
 			{
 				new Upgrade(-2,0, " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10)
 			};
		}
	}

}
