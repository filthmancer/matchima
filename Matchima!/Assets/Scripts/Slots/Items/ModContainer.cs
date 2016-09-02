using UnityEngine;
using System.Collections;
using System;

public class ModContainer : MonoBehaviour {

	public static string [] Wave_Prefixes = new string []
	{
		"Red Hot",
		"Glowing",
		"The Evil",
		"Hypothetical",
		"Broken",
		"Fruity",
		"Ever-Hungry"
	};

	public static string [] Wave_Titles = new string []
	{
		"Gnarr",
		"Rom",
		"Shiik",
		"Yiik",
		"Ptn",
		"Yth",
		"Rggn"
	};

	public static string [] Wave_Suffixes = new string []
	{
		"the Cold",
		"Shiny Devourer",
		"Evil Friend",
		"Sloveth",
		"Smith"
	};

	public static string [] GenerateWaveNameArray(bool pref, bool suff)
	{
		string prefix = "", suffix = "";
		string title = Wave_Titles[UnityEngine.Random.Range(0, Wave_Titles.Length)];
		if(pref) prefix = Wave_Prefixes[UnityEngine.Random.Range(0, Wave_Prefixes.Length)];
		if(suff) suffix = Wave_Suffixes[UnityEngine.Random.Range(0, Wave_Suffixes.Length)];
		return new string [] {prefix, title, suffix};
	}

	public static string GenerateWaveName(bool pref, bool suff)
	{
		string prefix = "", suffix = "";
		string title = Wave_Titles[UnityEngine.Random.Range(0, Wave_Titles.Length)];
		if(pref) prefix = Wave_Prefixes[UnityEngine.Random.Range(0, Wave_Prefixes.Length)];
		if(suff) suffix = Wave_Suffixes[UnityEngine.Random.Range(0, Wave_Suffixes.Length)];
		return prefix + "\n" + title + (suff ? ",\n":"\n") + suffix;
	}

	public static UpgradeBracket [] Brackets
	{
		get{
			return new UpgradeBracket[] {
				Basic, 
				Generator, 
				Shift, 
				Unstable,
				Primal,
				Elegant,
				Developers
			};
		}
	}

	public static UpgradeBracket [] Boons
	{
		get{
			return new UpgradeBracket[] {
				Basic, 
				Generator, 
				Shift, 
				Unstable,
				Primal,
				Elegant,
				Developers
			};
		}
	}


	public static float AllChance
	{
		get{
			return Basic.Chance +  
				Generator.Chance +  
				Shift.Chance +  
				Unstable.Chance + 
				Primal.Chance + 
				Elegant.Chance + 
				Developers.Chance;
		}
	}


	public static float BoonChance
	{
		get{
			return Basic.Chance +  
				Generator.Chance +  
				Shift.Chance +  
				Unstable.Chance + 
				Primal.Chance + 
				Elegant.Chance + 
				Developers.Chance;
		}
	}


	public static UpgradeBracket [] Curses
	{
		get
		{
			return new UpgradeBracket[]
			{
				Curse_Basic, 
				Curse_Generator, 
				Curse_Shift, 
				Curse_Unstable,
				Curse_Primal,
				Curse_Elegant,
				Curse_Developers
			};
		}
	}
	public static float CurseChance
	{
		get
		{
			return	Curse_Basic.Chance +  
			Curse_Generator.Chance +  
			Curse_Shift.Chance +  
			Curse_Unstable.Chance + 
			Curse_Primal.Chance + 
			Curse_Elegant.Chance + 
			Curse_Developers.Chance;
		}
	}


	//BOONS
	public static UpgradeBracket Basic = new UpgradeBracket(0, "Basic", ItemType.Basic, 1.9F, new GenusBracket []
	{
		new GenusBracket("Red",
			new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax += 10 + (int)val*5;}, 5, 10),
			new Upgrade("Healing", " HP Regen", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}, 1, 1),
			new Upgrade("Sharp", " Attack", 0.4F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),
			new Upgrade("Wise", "% Spell Power", 0.4F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.SpellPower += 0.2F * val;}, 20),
			new Upgrade("Healing", " MP Regen", 0.2F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.MeterRegen += 1 + (int) val;}) 
		),
		
		new GenusBracket("Blue",
			new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),
			new Upgrade("Spiked", " Spikes", 1.0F, ScaleType.GRADIENT,1.0F, (Stat s, float val) => {s.Spikes += 1 + (int)val;}, 1, 1),
			new Upgrade("Hearty", " Max HP", 0.4F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax += 10 + (int)val*5;}, 5, 10),
			new Upgrade("Healing", " HP Regen", 0.4F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}, 1, 1),
			new Upgrade("Wise", "% Spell Power", 0.4F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.SpellPower += 0.2F * val;}, 20),
			new Upgrade("Healing", " MP Regen", 0.2F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.MeterRegen += 1 + (int) val;})
		),
		new GenusBracket("Green",
			new Upgrade("Wise", "% Spell Power", 1.0F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.SpellPower += 0.2F * val;}, 20),
			new Upgrade("Hearty", " Max HP", 0.4F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax += 10 + (int)val * 5;}, 5, 10),
			new Upgrade("Healing", " HP Regen", 0.4F, ScaleType.GRADIENT, 0.8F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}),
			new Upgrade("Healing", " MP Regen", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.MeterRegen += 1 + (int) val;}),
			new Upgrade("Sharp", " Spell", 0.7F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._Spell += 1 + (int)val;}, 1, 1)
		),
		new GenusBracket("Yellow",
			new Upgrade("Spiked", " Spikes", 1.0F, ScaleType.GRADIENT,1.0F, (Stat s, float val) => {s.Spikes += 1 + (int)val;}, 1, 1),
			new Upgrade("Sharp", " Attack", 0.4F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),
			new Upgrade("Healing", " HP Regen", 0.4F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}, 1, 1),
			new Upgrade("Wise", "% Spell Power", 0.5F, ScaleType.GRADIENT, 0.5F, (Stat s, float val) => {s.SpellPower += 0.2F * val;}, 20),
			new Upgrade("Healing", " MP Regen", 0.2F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.MeterRegen += 1 + (int) val;})
		)
		
	});

	public static UpgradeBracket Generator = new UpgradeBracket(1, "Generator", ItemType.Generator, 0.75F, new GenusBracket[]
	{
		new GenusBracket("Red",
			new Upgrade("Bombers's", "% chance\n of Bomb", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance\n of Red Health", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance\n of Red Sword", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("red", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance\n of Red Armour", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("red", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Cleric's", "% chance\n of Red Beam", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Mages's", "% chance\n of Red Arcane", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "arcane", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Red Lightning", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "lightning", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Red Flame", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "flame", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Red lens", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "lens", 0.1F + 0.03F * value));}, 3, 10
				)
			),
		
		new GenusBracket("Blue",
			new Upgrade("Soldier's", "% chance\n of Sword", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance\n of Blue Health", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Cleric's", "% chance\n of Blue Beam", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance\n of Blue Armour", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("blue", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Mages's", "% chance\n of Blue Arcane", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "arcane", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Blue Bomb", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Blue Lightning", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "lightning", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Blue flame", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "flame", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Blue lens", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "lens", 0.1F + 0.03F * value));}, 3, 10
				)
		),
		new GenusBracket("Green",
			new Upgrade("Mage's", "% chance\n of Arcane", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "arcane", 0.1F + 0.03F * value));}
				),
			new Upgrade("Soldier's", "% chance\n of Green Health", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance\n of Green Sword", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance\n of Green Armour", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Cleric's", "% chance\n of Green Beam", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Green Bomb", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Green Lightning", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "lightning", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Green Flame", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "flame", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Green lens", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "lens", 0.1F + 0.03F * value));}, 3, 10
				)
		),
		new GenusBracket("Yellow",
			new Upgrade("Cleric's", "% chance\n of Beam", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance\n of Yellow Health", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance\n of Yellow Sword", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance\n of Yellow Armour", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Mages's", "% chance\n of Yellow Arcane", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "arcane", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Yellow Bomb", 1.0F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Yellow Lightning", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "lightning", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Yellow Flame", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "flame", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance\n of Yellow lens", 0.45F, ScaleType.GRADIENT, 1.0F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "lens", 0.1F + 0.03F * value));}, 3, 10
				)
		)
	});

	public static UpgradeBracket Shift = new UpgradeBracket(2, "Shift", ItemType.Shift, 0.7F, new GenusBracket [] {
		new GenusBracket("Red",

			new Upgrade("Bombers's", " Mana Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "mana", 0, 1 + (int)value));}, 1, 1),

			new Upgrade("Bombers's", " Lightning Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "lightning", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Arcane Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "arcane", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Bomb Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "bomb", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Beam Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "cross", 0, 1 + (int)value));}, 1, 1)
			
		),
		
		new GenusBracket("Blue",

			new Upgrade("Bombers's", " Mana Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "mana", 0, 1 + (int)value));}, 1, 1),

			new Upgrade("Bombers's", " Lightning Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "lightning", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Arcane Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "arcane", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Bomb Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "bomb", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Beam Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "cross", 0, 1 + (int)value));}, 1, 1)
			

		),
		new GenusBracket("Green",

			new Upgrade("Bombers's", " Mana Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "mana", 0, 1 + (int)value));}, 1, 1),

			new Upgrade("Bombers's", " Lightning Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "lightning", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Arcane Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "arcane", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Bomb Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "bomb", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Beam Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "cross", 0, 1 + (int)value));}, 1, 1)
			
		
		),
		new GenusBracket("Yellow",

			new Upgrade("Bombers's", " Mana Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "mana", 0, 1 + (int)value));}, 1, 1),

			new Upgrade("Bombers's", " Lightning Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "lightning", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Arcane Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "arcane", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Bomb Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "bomb", 0, 1 + (int)value));}, 1, 1),
			
			new Upgrade("Bombers's", " Beam Value", 1.0F, ScaleType.GRADIENT, 0.2F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "cross", 0, 1 + (int)value));}, 1, 1)
		)
	});

	public static UpgradeBracket Unstable = new UpgradeBracket(3, "Unstable", ItemType.Unstable, 0.4F, new GenusBracket [] {
		new GenusBracket("Red",
			new Upgrade("Lucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 1.0F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance += 0.05F + (value * 0.03F);
						}, 3, 5),
			new Upgrade("Strengthening", " Tile Per Match", 0.2F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{
					s.MatchNumberModifier -= 1;
					}, -1, -1)
			),
		
		new GenusBracket("Blue",
			new Upgrade("Lucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 1.0F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance += 0.05F + (value * 0.03F);
						}, 3, 5),
			new Upgrade("Strengthening", " Tile Per Match", 0.2F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{
					s.MatchNumberModifier -= 1;
					}, -1, -1)
		),
		new GenusBracket("Green",
			new Upgrade("Lucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 1.0F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance += 0.05F + (value * 0.03F);
						}, 3, 5),
			new Upgrade("Strengthening", " Tile Per Match", 0.2F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{
					s.MatchNumberModifier -= 1;
					}, -1, -1)
		),
		new GenusBracket("Yellow",
			new Upgrade("Lucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 1.0F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance += 0.05F + (value * 0.03F);
						}, 3, 5),
			new Upgrade("Strengthening", " Tile Per Match", 0.2F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{
					s.MatchNumberModifier -= 1;
					}, -1, -1)
		)
		});

	public static UpgradeBracket Primal = new UpgradeBracket(4, "Primal", ItemType.Primal, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});

	public static void CastTileChange(float chance, string in_species, string out_species)
	{
		foreach(Tile child in PlayerControl.instance.finalTiles)
		{
			if(!child.IsType(in_species)) continue;
			
			if(UnityEngine.Random.value < chance)
			{
				Tile e = TileMaster.Enemies[UnityEngine.Random.Range(0, TileMaster.Enemies.Length)];
				e.SetState(TileState.Selected, true);
				GameObject initpart = EffectManager.instance.PlayEffect(UIManager.ClassButtons[0].transform, Effect.Spell);
				MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
				charm.SetTarget(e.transform.position);
				charm.SetPath(0.6F, 0.3F);
				charm.Target_Tile = e;
				charm.SetThreshold(0.15F);
				charm.SetMethod(() =>
				{
					TileMaster.instance.ReplaceTile(child, TileMaster.Types[out_species]);
				});
				PlayerControl.instance.RemoveTileToMatch(child);
			}
		}
	}

	public static void CastTileEffect(float chance, string in_species, string effect)
	{
		foreach(Tile child in PlayerControl.instance.finalTiles)
		{
			if(!child.IsType(in_species)) continue;
			
			if(UnityEngine.Random.value < chance)
			{
				Tile e = TileMaster.Enemies[UnityEngine.Random.Range(0, TileMaster.Enemies.Length)];
				e.SetState(TileState.Selected, true);
				GameObject initpart = EffectManager.instance.PlayEffect(UIManager.ClassButtons[0].transform, Effect.Spell);
				MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
				charm.SetTarget(e.transform.position);
				charm.SetPath(0.6F, 0.3F);
				charm.Target_Tile = e;
				charm.SetThreshold(0.15F);
				charm.SetMethod(() =>
				{
					MiniAlertUI m = UIManager.instance.MiniAlert(charm.Target_Tile.Point.targetPos, effect +"!", 85, GameData.Colour(charm.Target_Tile.Genus), 1.2F, 0.1F);
					charm.Target_Tile.AddEffect(effect, -1);
				});
				PlayerControl.instance.RemoveTileToMatch(child);
			}
		}
	}
	public static UpgradeBracket Elegant = new UpgradeBracket(5, "Elegant", ItemType.Elegant, 0.35F, new GenusBracket [] {
		new GenusBracket("Red",
			new OnMatchUpgrade("Charming", "% Chance to\nChange Enemy to Bomb",
								1.0F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileChange(value, "enemy", "Bomb");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nSleep Enemy",
								0.8F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Sleep");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nCharm Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Charm");
				}	
			)
		),
		new GenusBracket("Blue",
			new OnMatchUpgrade("Charming", "% Chance to\nChange Enemy to Sword",
								1.0F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileChange(value, "enemy", "Sword");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nSleep Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Sleep");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nCharm Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Charm");
				}	
			)
		),
		new GenusBracket("Green",
			new OnMatchUpgrade("Charming", "% Chance to\nChange Enemy to Lightning",
								1.0F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileChange(value, "enemy", "lightning");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nSleep Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Sleep");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nCharm Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Charm");
				}	
			)
		),
		new GenusBracket("Yellow",
			new OnMatchUpgrade("Charming", "% Chance to\nChange Enemy to Chicken",
								1.0F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileChange(value, "enemy", "Chicken");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nSleep Enemy",
								0.4F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Sleep");
				}	
			),
			new OnMatchUpgrade("Charming", "% Chance to\nCharm Enemy",
								0.8F, ScaleType.GRADIENT, 1.5F,
				(Stat s, float value) => {
					CastTileEffect(value, "enemy", "Charm");
				}	
			)
		)
		});
	
	public static UpgradeBracket Developers = new UpgradeBracket(6, "Developer's", ItemType.Developers, 0.52F, new GenusBracket[]
	{
		new GenusBracket("Red",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.4F,
						(Stat s, float value) => {
							s.MapSize.x += 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.4F,
				(Stat s, float value) => {
					s.MapSize.y += 1 + (int) (1 * value);},1,1
					)
		),
		new GenusBracket("Blue",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.4F,
						(Stat s, float value) => {
							s.MapSize.x += 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.4F,
				(Stat s, float value) => {
					s.MapSize.y += 1 + (int) (1 * value);},1,1
				)
		),
		new GenusBracket("Green",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.4F,
						(Stat s, float value) => {
							s.MapSize.x += 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.4F,
				(Stat s, float value) => {
					s.MapSize.y += 1 + (int) (1 * value);},1,1
				)
		),
		new GenusBracket("Yellow",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.4F,
						(Stat s, float value) => {
							s.MapSize.x += 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.4F,
				(Stat s, float value) => {
					s.MapSize.y += 1 + (int) (1 * value);},1,1
				)
		)
		
	});

	//CURSES
	public static UpgradeBracket Curse_Basic = new UpgradeBracket(7,"Cursed", ItemType.Basic, 1.0F, new GenusBracket []
	{
		new GenusBracket("Red",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 0.6F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val;}, -1, -10)
			),
		
		new GenusBracket("Blue",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 0.12F, (Stat s, float val) => {s._Attack -= 1 + (int)val;}, -1, -1)
		
		),
		new GenusBracket("Green",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 0.6F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val;}, -1, -10)
		),
		new GenusBracket("Yellow",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 0.6F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val;}, -1, -10)
		)
		
	});

	public static UpgradeBracket Curse_Generator = new UpgradeBracket(8,"Demonic", ItemType.Generator, 0.35F, new GenusBracket[]
	{
		new GenusBracket("Red",
			new Upgrade("Demon's", "% chance\n of Red Grunts", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "grunt", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Demon's", "% chance\n of Chickens", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("omega", "altar", 0.1F + 0.03F * value));}, 3, 10
				)

			),
		
		new GenusBracket("Blue",
			new Upgrade("Soldier's", "% chance\n of Blue Minions", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("blue", "minion", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Demon's", "% chance\n of Chickens", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("omega", "altar", 0.1F + 0.03F * value));}, 3, 10
				)
		),
		new GenusBracket("Green",
			new Upgrade("Mage's", "% chance\n of Green Blobs", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "blob", 0.1F + 0.03F * value));}, 3,10
				),
			new Upgrade("Demon's", "% chance\n of Chickens", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("omega", "altar", 0.1F + 0.03F * value));}, 3, 10
				)
		),
		new GenusBracket("Yellow",
			new Upgrade("Cleric's", "% chance\n of Yellow Grunts", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "grunt", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Demon's", "% chance\n of Chickens", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("omega", "altar", 0.1F + 0.03F * value));}, 3, 10
				)
		)
	});

	public static UpgradeBracket Curse_Shift = new UpgradeBracket(9,"Mindless", ItemType.Shift, 0.56F, new GenusBracket [] {
		new GenusBracket("Red",
			new Upgrade("Weakening", " Tile Per Match",
						1.0F, ScaleType.RANK, 0.0F,
			(Stat s, float value) =>
			{
				s.MatchNumberModifier += 1;
			}, 1, 1),
			new Upgrade("Unlucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 0.5F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance -= 0.05F + (value * 0.03F);
						}, -3, -5)
		),

		
		new GenusBracket("Blue",
			new Upgrade("Weakening", " Tile Per Match", 1.0F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{
					s.MatchNumberModifier += 1;
					}, 1, 1),
			new Upgrade("Unlucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 0.5F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance -= 0.05F + (value * 0.03F);
						}, -3, -5)
				),
		new GenusBracket("Green",
			new Upgrade(
				"Weakening", " Tile Per Match", 1.0F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{
				s.MatchNumberModifier += 1;
				}, 1, 1),
			new Upgrade("Unlucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 0.5F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance -= 0.05F + (value * 0.03F);
						}, -3, -5)
			),
		new GenusBracket("Yellow",
			new Upgrade("Weakening", " Tile Per Match", 1.0F, ScaleType.RANK, 0.0F,
				(Stat s, float value) =>
				{s.MatchNumberModifier += 1;}, 1, 1),
			new Upgrade("Unlucky", "% Death Save Chance",
						1.0F, ScaleType.GRADIENT, 0.5F,
						(Stat s, float value) =>
						{
							s.DeathSaveChance -= 0.05F + (value * 0.03F);
						}, -3, -5)
		)
	});

	public static UpgradeBracket Curse_Unstable = new UpgradeBracket(10, "Unstable", ItemType.Unstable, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});

	public static UpgradeBracket Curse_Primal = new UpgradeBracket(11, "Primal", ItemType.Primal, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});

	public static UpgradeBracket Curse_Elegant = new UpgradeBracket(12, "Elegant", ItemType.Elegant, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});
	
	public static UpgradeBracket Curse_Developers = new UpgradeBracket(13, "Hacker's", ItemType.Developers, 0.35F, new GenusBracket[]
	{
		new GenusBracket("Red",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.7F,
						(Stat s, float value) => {
							s.MapSize.x -= 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.7F,
				(Stat s, float value) => {
					s.MapSize.y -= 1 + (int) (1 * value);},-1,-1
					)
		),
		new GenusBracket("Blue",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.7F,
						(Stat s, float value) => {
							s.MapSize.x -= 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.7F,
				(Stat s, float value) => {
					s.MapSize.y -= 1 + (int) (1 * value);},-1,-1
				)
		),
		new GenusBracket("Green",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.7F,
						(Stat s, float value) => {
							s.MapSize.x -= 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.7F,
				(Stat s, float value) => {
					s.MapSize.y -= 1 + (int) (1 * value);},-1,-1
				)
		),
		new GenusBracket("Yellow",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.7F,
						(Stat s, float value) => {
							s.MapSize.x -= 1 + (int) (1 * value);},1,1
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.7F,
				(Stat s, float value) => {
					s.MapSize.y -= 1 + (int) (1 * value);},-1,-1
				)
		)
		
	});

}

[System.Serializable]
public class UpgradeBracket
{
	public string Title;
	public int Index;
	public ItemType Type;
	public GenusBracket [] Genus;

	public float AllChances
	{
		get{
			float c = 0.0F;
			for(int i = 0; i < Genus.Length; i++)
			{
				c += Genus[i].AllChances;
			}
			return c;
		}
	}
	public float Chance;

	public UpgradeBracket(int _index, string _title, ItemType _type, float _chance, params GenusBracket [] b)
	{
		Index = _index;
		Title = _title;
		Type = _type;
		Chance = _chance;

		Genus = b;
		foreach(GenusBracket child in Genus)
		{
			child.SetTypes(Type, Index);
		}
	}


	public Upgrade GetUpgrade(GENUS g = GENUS.NONE)
	{
		if(g == GENUS.NONE || g == GENUS.ALL)
		{
			float chance = UnityEngine.Random.value * AllChances;
			float current = 0.0F;
			for(int i = 0; i < Genus.Length; i++)
			{
				for(int x = 0; x < Genus[i].Upgrades.Length; x++)
				{
					if(chance >= current && chance < current + Genus[i].Upgrades[x].chance)
					{
						return Genus[i].Upgrades[x];
					}
					current += Genus[i].Upgrades[x].chance;
				}
			}
			return null;
		}
		else
		{
			int i = (int)g;
			float chance = UnityEngine.Random.value * Genus[i].AllChances;
			float current = 0.0F;
			for(int x = 0; x < Genus[i].Upgrades.Length; x++)
			{
				if(chance >= current && chance < current + Genus[i].Upgrades[x].chance)
				{
					return Genus[i].Upgrades[x];
				}
				current += Genus[i].Upgrades[x].chance;
			}
		}
		return null;
		
	}
}

[System.Serializable]
public class GenusBracket
{
	public string Name;
	public Upgrade [] Upgrades;
	public GenusBracket(string _title, params Upgrade [] u)
	{
		Name = _title;
		Upgrades = u;
	}

	public void SetTypes(ItemType t, int index)
	{
		int i = 0;
		foreach(Upgrade child in Upgrades)
		{
			child.type = t;
			child.Index = new int []{index, i};
			i++;
		}
	}

	public float AllChances
	{
		get{
			float c = 0.0F;
			for(int i = 0; i < Upgrades.Length; i++)
			{
				c += Upgrades[i].chance;
			}
			return c;
		}
	}
}

public enum ItemType
{
	Basic, 
	Generator, 
	Shift, 
	Unstable,
	Primal,
	Elegant,
	Developers,
}

public enum ScaleType
{
	STATIC,
	RANK,
	GRADIENT,
	NONE
}


[System.Serializable]
public class Upgrade
{
	public string Title
	{
		get{
			string pref = Points > 0 ? "+" : "";
			if(Player.Options.ShowNumbers) pref += Points;
			return pref + Suffix;
		}
	}

	public string Title_NoParent
	{
		get
		{
			return "+" + Suffix;
		}
	}
	public int Points 
	{
		get{

			return (int)(RateFinal * Points_desc_mult + Points_desc_add);
		}
	}

	public int[] Index;

	public string Prefix;
	public string Suffix;
	public float chance;
	public ItemType type;
	public ScaleType scaletype;

	public float scalerate;
	public int Points_total = 0;
	public float Points_desc_mult;
	public int Points_desc_add;

	public Action <Stat, float> Method;
	public float _Rate;
	public float RateFinal
	{
		get
		{
			return _Rate * scalerate;
		}
	}

	public MethodType MethType = MethodType.OnReset;
	

	public Upgrade(string _prefix, string _suffix, float _chance, ScaleType _type, float _scalerate, Action<Stat, float> _method, 
		float desc_mult = 1, int desc_add = 0)
	{
		Init(_prefix, _suffix, _chance, _type, _scalerate, _method, desc_mult, desc_add);
	}

	public virtual void Init(string _prefix, string _suffix, float _chance, ScaleType _type, float _scalerate, Action<Stat, float> _method, 
		float desc_mult = 1, int desc_add = 0)
	{
		//PREFIX TITLE FOR ITEMS
			Prefix = _prefix;
		//ACTUAL INFO
			Suffix = _suffix;
		//CHANCE TO BE SELECTED (DEFAULT 1.0)
			chance = _chance;

		//SCALING STYLE
			scaletype = _type;
		//VALUE FOR GRADIENT/RANK SCALING
			scalerate =_scalerate;

		//TOTAL POINTS OF THIS UPGRADE
			Points_total = 0;
		//POINT VALUE MULTIPLIER FOR UI
			Points_desc_mult = desc_mult;
		//POINT VALUE ADDER FOR UI
			Points_desc_add = desc_add;

		//METHOD FOR ADDING UPGRADE
			Method = _method;
	}

	public Upgrade(Upgrade u)
	{
		Index = u.Index;
		Suffix = u.Suffix;
		Prefix = u.Prefix;
		chance = u.chance;
		type = u.type;

		Points_total = u.Points_total;
		Points_desc_mult = u.Points_desc_mult;
		Points_desc_add = u.Points_desc_add;
		scaletype = u.scaletype;
		scalerate = u.scalerate;

		Method = u.Method;
	}

	

	public Upgrade()
	{

	}

	public virtual void Up(Stat s, float val)
	{
		if(MethType != MethodType.OnReset) return;
		//Points_total = (int) val;
		if(Method != null) Method(s, val);
	}


	public void OnMatch(Stat s, float val)
	{
		if(MethType != MethodType.OnMatch) return;
		if(Method != null) Method(s, val);
	}

	public int OnPlayerHit(int hit, Tile[] attackers, Stat s, float val)
	{
		if(MethType != MethodType.OnPlayerHit) return hit;
		if(Method != null) Method(s,val);
		return hit;
	}


}

public enum MethodType
{
	OnReset,
	OnMatch,
	OnPlayerHit
}

public class OnMatchUpgrade : Upgrade
{

	public OnMatchUpgrade(string _prefix, string _suffix, float _chance, ScaleType _type, float _scalerate, Action<Stat, float> _method, 
		float desc_mult = 1, int desc_add = 0) : base(_prefix, _suffix, _chance, _type, _scalerate, _method, desc_mult, desc_add)
	{
		MethType = MethodType.OnMatch;
	}


	public override void Init(string _prefix, string _suffix, float _chance, ScaleType _type, float _scalerate, Action<Stat, float> _method, 
		float desc_mult = 1, int desc_add = 0)
	{
		base.Init(_prefix, _suffix, _chance, _type, _scalerate, _method, desc_mult, desc_add);
		
		
	}

	/*public OnMatchUpgrade(Upgrade u) : base(u)
	{
		//base.Upgrade(u);
		//(this as OnMatchUpgrade).MatchMethod = u.MatchMethod;
	}*/
}