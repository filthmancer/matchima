using UnityEngine;
using System.Collections;

public class Barbarian : Class {

	TileChance health;

	private Slot manapower;
	private int _currentmanapower;


	// Use this for initialization
	public override void StartClass () {
	
		health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.25F;
		InitStats.TileChances.Add(health);

		PowerupSpell = GameData.instance.GetPowerup("Heal", this);

		base.StartClass();
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
				new Upgrade(1,0, " Max HP", 1.0F,
									 ScaleType.GRADIENT, 1.0F,
									 (Stat s, float val) => {s._HealthMax += 20 + (int)val*5;}, 5, 20),

				new Upgrade(1,1, "% chance\n of Health", 0.7F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade(1,2, " Tile Per Match", 0.2F, ScaleType.RANK, 0.0F,
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
 				new Upgrade(-2, 0, " Max HP", 0.2F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10),
 				new Upgrade(-2, 1, " Attack", 1.0F, ScaleType.GRADIENT, 0.12F, (Stat s, float val) => {s._Attack -= 1 + (int)val;}, -1, -1),

 				new Upgrade(-2, 2, "% chance\n of Grunts", 0.4F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "grunt", 0.1F + 0.03F * value));}, 3, 10
				),

 			};
		}
	}

}
