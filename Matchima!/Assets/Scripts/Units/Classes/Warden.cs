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
				new Upgrade("Soldier's", "% chance\n of Health", 0.4F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of Bomb", 0.4F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "bomb", 0.1F + 0.03F * value));}, 3, 10
					),
				new Upgrade("Bombers's", "% chance\n of flame", 0.4F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "flame", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade("Lucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 1.0F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance += 0.05F + (value * 0.03F);
						}, 3, 5),
				new Upgrade("Strengthening", " Tile Per Match", 1.0F, ScaleType.RANK, 0.0F,
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
 				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10)
 			};
		}
	}

}
