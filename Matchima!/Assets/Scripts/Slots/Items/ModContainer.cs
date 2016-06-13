using UnityEngine;
using System.Collections;
using System;

public class ModContainer : MonoBehaviour {


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

	public static UpgradeBracket Basic = new UpgradeBracket("Basic", ItemType.Basic, 1.0F, new GenusBracket []
	{
		new GenusBracket("Red",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 0.6F, (Stat s, float val) => {s._HealthMax += 10 + (int)val;}, 1, 10),
			new Upgrade("Healing", " HP Regen", 1.0F, ScaleType.GRADIENT, 0.15F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}, 1, 1)
			),
		
		new GenusBracket("Blue",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 0.12F, (Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1)
		
		),
		new GenusBracket("Green",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Addict's", "% Manapower Decay", 1.0F, ScaleType.GRADIENT, 0.08F,
						(Stat s, float val) => {s.MeterDecay_Global -= (int)val;}, 4)
		),
		new GenusBracket("Yellow",
			new Upgrade("Wise", " Max MP", 1.0F, ScaleType.GRADIENT, 0.4F, (Stat s, float val) => {s.MeterMax += 5 + (int)val;}, 1, 5),
			new Upgrade("Spiked", " Spikes", 1.0F, ScaleType.GRADIENT, 0.09F, (Stat s, float val) => {s.Spikes += 1 + (int)val;}, 1, 1)
		)
		
	});

	public static UpgradeBracket Generator = new UpgradeBracket("Generator", ItemType.Generator, 0.75F, new GenusBracket[]
	{
		new GenusBracket("Red",
			new Upgrade("Bombers's", "% chance of Bomb", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance of Red Health", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance of Red Sword", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("red", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance of Red Armour", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("red", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Cleric's", "% chance of Red Cross", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Mages's", "% chance of Red Arcane", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "arcane", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Red Lightning", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Red", "lightning", 0.1F + 0.03F * value));}, 3, 10
				)
			),
		
		new GenusBracket("Blue",
			new Upgrade("Soldier's", "% chance of Sword", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance of Blue Health", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Cleric's", "% chance of Blue Cross", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance of Blue Armour", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("blue", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Mages's", "% chance of Blue Arcane", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "arcane", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Blue Bomb", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Blue Lightning", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Blue", "lightning", 0.1F + 0.03F * value));}, 3, 10
				)
		),
		new GenusBracket("Green",
			new Upgrade("Mage's", "% chance of Arcane", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "arcane", 0.1F + 0.03F * value));}
				),
			new Upgrade("Soldier's", "% chance of Green Health", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance of Green Sword", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance of Green Armour", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("green", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Cleric's", "% chance of Green Cross", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Green Bomb", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Green Lightning", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Green", "lightning", 0.1F + 0.03F * value));}, 3, 10
				)
		),
		new GenusBracket("Yellow",
			new Upgrade("Cleric's", "% chance of Cross", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("", "cross", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance of Yellow Health", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "health", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Soldier's", "% chance of Yellow Sword", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "sword", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Smith's", "% chance of Yellow Armour", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("yellow", "armour", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Mages's", "% chance of Yellow Arcane", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "arcane", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Yellow Bomb", 1.0F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "bomb", 0.1F + 0.03F * value));}, 3, 10
				),
			new Upgrade("Bombers's", "% chance of Yellow Lightning", 0.45F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance("Yellow", "lightning", 0.1F + 0.03F * value));}, 3, 10
				)
		)
	});

	public static UpgradeBracket Shift = new UpgradeBracket("Shift", ItemType.Shift, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
	});

	public static UpgradeBracket Unstable = new UpgradeBracket("Unstable", ItemType.Unstable, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});

	public static UpgradeBracket Primal = new UpgradeBracket("Primal", ItemType.Primal, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});

	public static UpgradeBracket Elegant = new UpgradeBracket("Elegant", ItemType.Elegant, 0.0F, new GenusBracket [] {
		new GenusBracket("Red"

			),
		
		new GenusBracket("Blue"

		),
		new GenusBracket("Green"
		
		),
		new GenusBracket("Yellow"

		)
		});
	
	public static UpgradeBracket Developers = new UpgradeBracket("Developer's", ItemType.Developers, 0.25F, new GenusBracket[]
	{
		new GenusBracket("Red",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.5F,
						(Stat s, float value) => {
							s.MapSize.x += (int) (1 * value);}
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.5F,
				(Stat s, float value) => {
					s.MapSize.y += (int) (1 * value);}
					)
		),
		new GenusBracket("Blue",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.5F,
						(Stat s, float value) => {
							s.MapSize.x += (int) (1 * value);}
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.5F,
				(Stat s, float value) => {
					s.MapSize.y += (int) (1 * value);}
				)
		),
		new GenusBracket("Green",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.5F,
						(Stat s, float value) => {
							s.MapSize.x += (int) (1 * value);}
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.5F,
				(Stat s, float value) => {
					s.MapSize.y += (int) (1 * value);}
				)
		),
		new GenusBracket("Yellow",
			new Upgrade("Cook's", " Map X", 1.0F, ScaleType.RANK, 0.5F,
						(Stat s, float value) => {
							s.MapSize.x += (int) (1 * value);}
						),
			new Upgrade("Magellan's", " Map Y", 1.0F, ScaleType.RANK, 0.5F,
				(Stat s, float value) => {
					s.MapSize.y += (int) (1 * value);}
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

	public UpgradeBracket(string _title, ItemType _type, float _chance, params GenusBracket [] b)
	{
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
	GRADIENT
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
			return (int)(Points_total * Points_desc_mult) + Points_desc_add;
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
	

	public Upgrade(string _prefix, string _suffix, float _chance, ScaleType _type, float _scalerate, Action<Stat, float> _method, 
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

	public void Up(Stat s, float val)
	{
		Points_total = (int) val;
		Method(s, val);
	}
}


/*private ItemBracket Masterwork = new ItemBracket("Masterwork", ItemType.Masterwork, 0.07F,
		new ItemGenusBracket("Red",
			new Upgrade(21, 0.4F, 1, "Haunted", "Presence", 1, ScaleType.RANK), //Presence
			new Upgrade(17, 0.6F, 0.01F, "Hasting", "% Spell Cooldown", 1, ScaleType.GRADIENT) //CD DEC
			),
		new ItemGenusBracket("Blue",
			new Upgrade(16, 0.01F, 1, "Speedy", " Attack Rate", 1, ScaleType.STATIC), //ATK RATE
			new Upgrade(17, 0.6F, 0.01F, "Hasting", "% Spell Cooldown", 1, ScaleType.GRADIENT) //CD DEC
			),
		new ItemGenusBracket("Green",
			new Upgrade(17, 0.6F, 0.01F,"Hasting", "% Spell Cooldown", 1, ScaleType.GRADIENT), //CD DEC
			new Upgrade(18, 0.6F, 0.01F,"Cheap", "% Spell Cost", 1, ScaleType.GRADIENT) //COST DEC
			),
		new ItemGenusBracket("Yellow",
			new Upgrade(22, 0.3F, 0.1F,"Bountiful", "Combo Bonus", 0.1F, ScaleType.GRADIENT), //COMBO BONUS
			new Upgrade(23, 0.05F, 1,"Minimal", "Tile/Combo", -1, ScaleType.STATIC) //COMBO COUNTER
			)
			);

private ItemBracket Basic = new ItemBracket("Basic", ItemType.Basic, 0.8F,
		new ItemGenusBracket("Red",
			new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.07F), //ATTACK
			new Upgrade(12, 0.5F,  5,"Hearty",  "Max HP", 5, ScaleType.GRADIENT) //REFLECT - CURRENTLY HEALTH MAX
			),
		new ItemGenusBracket("Blue",
			new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT,0.07F), //ATTACK
			new Upgrade(9,  0.45F, 1,"Spiked", "Spikes", 1, ScaleType.GRADIENT, 0.1F) //SPIKES
			),
		new ItemGenusBracket("Green",
			new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.07F), //ATTACK
			new Upgrade(24, 0.65F, 2,"Wise", "Max MP", 2, ScaleType.GRADIENT, 1) //RES MAX
			),
		new ItemGenusBracket("Yellow",
			new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.07F), //ATTACK
			new Upgrade(24, 0.65F, 2,"Wise", "Max MP", 2, ScaleType.GRADIENT, 1) //RES MAX
			)	
			//new Upgrade(11, 0.0F, 1, "Attack",1, ScaleType.GRADIENT), //POISON - CURRENTLY ATTACK
			//new Upgrade(13, 0.0F, 1, "Hunter",1, ScaleType.GRADIENT), //HUNTER
			//new Upgrade(14, 0.0F, 1, "Healer",1, ScaleType.GRADIENT) //HEALER
			//new Upgrade(15, 0.3F,  15, 1, ItemType.Basic) //HARVESTER
			);

//private ItemBracket Unique = new ItemBracket("Unique", ItemType.Unique, 0.0F, null
//			);

private ItemBracket Crude = new ItemBracket("Crude", ItemType.Crude, 0.7F,
	new ItemGenusBracket("Red",
		new Upgrade(12, 1.0F, 1,"Hearty", "Max HP", 1, ScaleType.GRADIENT, 0.3F), //HEALTH MAX
		new Upgrade(7,  0.5F, 1,"Healing", "HP Regen",1, ScaleType.GRADIENT, 0.08F) //REGEN
		),
	new ItemGenusBracket("Blue",
		new Upgrade(12, 0.65F, 1,"Hearty", "Max HP", 1, ScaleType.GRADIENT, 0.3F), //RES MAX
		new Upgrade(6,  0.35F, 1,"Vampiric", "HP Leech",1, ScaleType.GRADIENT, 0.08F) //LEECH
		),
	new ItemGenusBracket("Green",
		new Upgrade(12, 0.65F, 1,"Hearty", "Max HP", 1, ScaleType.GRADIENT, 0.3F), //RES MAX
		new Upgrade(4, 0.1F, 1,"Draining", "MP Leech", 1, ScaleType.GRADIENT, 0.08F) //RES LEECH
		),
	new ItemGenusBracket("Yellow",
		new Upgrade(12, 1.0F, 1,"Hearty", "Max HP", 1, ScaleType.GRADIENT, 0.3F), //HEALTH MAX
		new Upgrade(5, 0.1F, 1,"Learning", "MP Regen", 1, ScaleType.GRADIENT, 0.08F) //RES REGEN
		)
			//new Upgrade(0, 1.0F, 5, 5, ItemType.Crude, "DEX", 5), //STR
			//new Upgrade(1, 1.0F, 5, 5, ItemType.Crude, "WIS", 5), //DEX
			//new Upgrade(2, 1.0F, 5, 5, ItemType.Crude, "CHA", 5), //WIS
			//new Upgrade(3, 1.0F, 5, 5, ItemType.Crude, "STR", 5), //CHA
	);

private ItemBracket Developers = new ItemBracket("Developers", ItemType.Developers, 0.27F,
	new ItemGenusBracket("Red",
		new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(20, 0.1F, 1,"Travelers", "Map Y", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(25, 0.3F, 0.01F,"Spotters", "% Red Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(29, 0.15F, 0.01F,"Bombers", "% Bomb Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(31, 0.15F, 0.01F,"Priests", "% Health Tiles", 1, ScaleType.GRADIENT, 0.3F) //RES CHANCE
		),
	new ItemGenusBracket("Blue",
		new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(26, 0.3F, 0.01F,"Spotters", "% Blue Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(30, 0.15F, 0.01F,"Warriors", "% Sword Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(31, 0.15F, 0.01F,"Priests", "% Health Tiles", 1, ScaleType.GRADIENT, 0.3F) //RES CHANCE
		),
	new ItemGenusBracket("Green",
		new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(27, 0.3F, 0.01F,"Spotters", "% Green Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(31, 0.15F, 0.01F,"Priests", "% Health Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(33, 0.15F, 0.01F,"Mages", "% Arcane Tiles", 1, ScaleType.GRADIENT, 0.3F) //RES CHANCE
		),
	new ItemGenusBracket("Yellow",
		new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.RANK, 1.0F), //MAP SIZE
		new Upgrade(28, 0.3F, 0.01F,"Spotters", "% Yellow Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(31, 0.15F, 0.01F,"Priests", "% Health Tiles", 1, ScaleType.GRADIENT, 0.3F), //RES CHANCE
		new Upgrade(32, 0.15F, 0.01F,"Clerics", "% Cross Tiles", 1, ScaleType.GRADIENT, 0.3F) //RES CHANCE
		)
	);*/

/*public void Upgrade(Upgrade up, Stat output, float rate)
{
	float finalrate = up.value * rate;
	switch(up.index)
	{
		case 0:
			output.Dexterity               += (int)(finalrate);
			if(output.Dexterity > prefix_value)
			{
				prefix_value = output.Dexterity/5;
				prefix       = "Rogues";
			} 
			Type = ItemType.Crude;
		break;
		case 1:
			output.Wisdom                  += (int)(finalrate);
			if(output.Wisdom > prefix_value)
			{
				prefix_value = output.Wisdom/5;
				prefix = "Scholars";
			} 
			Type = ItemType.Crude;
		break;
		case 2:
			output.Charisma                += (int)(finalrate);
			if(output.Charisma > prefix_value)
			{
				prefix_value = output.Charisma/5;
				prefix = "Mayors";
			} 
			Type = ItemType.Crude;
		break;
		case 3:
			output.Strength                += (int)(finalrate);
			if(output.Strength > prefix_value)
			{
				prefix_value = output.Strength/5;
				prefix = "Warriors";
			} 
			Type = ItemType.Crude;
		break;
		case 4:
			output[up.subindex].ResLeech += (int)(finalrate);
			if(output[up.subindex].ResLeech > prefix_value)
			{
				prefix_value = output[up.subindex].ResLeech;
				prefix = "Misers";
			} 
			Type = ItemType.Crude;
		break;
		case 5:
			output[up.subindex].ResRegen += (int)(finalrate);
			if(output[up.subindex].ResRegen > prefix_value)
			{
				prefix_value = output[up.subindex].ResRegen;
				prefix = "Investors";
			} 
			Type = ItemType.Crude;
		break;
		case 6:
			output.Leech                   += (int)(finalrate);
			if(output.Leech > prefix_value)
			{
				prefix_value = output.Leech;
				prefix = "Vampiric";
			} 
		break;
		case 7:
			output.Regen                   += (int)(finalrate);
			if(output.Regen > prefix_value)
			{
				prefix_value = output.Regen;
				prefix = "Heros";
			} 
		break;
		case 8:
			output._Attack                  += (int)(finalrate);
			if(output._Attack > prefix_value)
			{
				prefix_value = output._Attack;
				prefix = "Sharp";
			} 
		break;
		case 9:
			output.Spikes                  += (int)(finalrate);
			if(output.Spikes > prefix_value)
			{
				prefix_value = output.Spikes;
				prefix = "Spiky";
			} 
		break;
		case 10:
			output._HealthMax               += (int)(finalrate);
			if(output._HealthMax > prefix_value)
			{
				prefix_value = output._HealthMax;
				prefix = "Robust";
			} 
		break;
		case 11:
			output._Attack                  += (int)(finalrate);
			if(output._Attack > prefix_value)
			{
				prefix_value = output._Attack;
				prefix = "Sharp";
			} 
		break;
		case 12:
			output._HealthMax               += (int)(finalrate);
			if(output._HealthMax > prefix_value)
			{
				prefix_value = output._HealthMax;
				prefix = "Robust";
			} 
			Type = ItemType.Crude;
		break;
		case 13:
			//output.Hunter                  += (int)(finalrate);
			//if(output.Hunter > prefix_value)
			//{
			//	prefix_value = output.Hunter;
			//	prefix = "Hunters";
			//} 
		break;
		case 14:
			//output.Healer                  += (int)(finalrate);
			//if(output.Healer > prefix_value)
			//{
			//	prefix_value = output.Healer;
			//	prefix = "Healers";
			//} 
		break;
		case 15:
			//output.Harvester               += (int)(finalrate);
			//if(output.Harvester > prefix_value)
			//{
			//	prefix_value = output.Harvester;
			//	prefix = "Harvesters";
			//} 
		break;
		case 16:
			output.AttackRate              += (int)(finalrate);
			if(output.AttackRate > prefix_value)
			{
				prefix_value = (int)output.AttackRate;
				prefix = "Speedy";
			} 
		break;
		case 17:
			output.CooldownDecrease        += (float)(finalrate);
			if(output.CooldownDecrease > prefix_value)
			{
				prefix_value = (int)(output.CooldownDecrease * 10);
				prefix = "Hasty";
			} 
		break;
		case 18:
			output.CostDecrease            += (float)(finalrate);
			if(output.CostDecrease > prefix_value)
			{
				prefix_value = (int)(output.CostDecrease * 10);
				prefix = "Cheap";
			} 
		break;
		case 19:
			output.MapSize += new Vector2((int)finalrate, 0);
			if(output.MapSize.x + output.MapSize.y > prefix_value)
			{
				prefix_value = (int) (output.MapSize.x + output.MapSize.y);
				prefix = "Voyagers";
			} 
		break;
		case 20:

			output.MapSize += new Vector2(0,(int)finalrate);
			if(output.MapSize.x + output.MapSize.y > prefix_value)
			{
				prefix_value = (int) (output.MapSize.x + output.MapSize.y);
				prefix = "Voyagers";
			} 
		break;
		case 21:
			output.Presence     = Mathf.Clamp(output.Presence + (int)(finalrate), 1, 20);
			if(output.Presence > prefix_value)
			{
				prefix_value = output.Presence;
				prefix = "Foreboding";
			} 
		break;
		case 22:
			output.ComboBonus   += (float)(finalrate);
			if(output.ComboBonus > prefix_value)
			{
				prefix_value = (int)(output.ComboBonus * 10);
				prefix = "Bounty";
			} 
		break;
		case 23:
			output.ComboCounter += (int)(finalrate);
			if(output.ComboCounter > prefix_value)
			{
				prefix_value = output.ComboCounter;
				prefix = "Counting";
			} 
		break;
		case 24:
			output.MeterMax += (int)(finalrate);
			if(output.MeterMax > prefix_value)
			{
				prefix_value = output.MeterMax;
				prefix = "Storing";
			} 
			Type = ItemType.Crude;
		break;
		case 25:
			output.TileChances.Add(new TileChance("Red", "", finalrate));
			prefix = "Spotters";
			Type = ItemType.Developers;
		break;
		case 26:
			output.TileChances.Add(new TileChance("Blue", "", finalrate));
			prefix = "Spotters";
			Type = ItemType.Developers;
		break;
		case 27:
			output.TileChances.Add(new TileChance("Green", "", finalrate));
			prefix = "Spotters";
			Type = ItemType.Developers;
		break;
		case 28:
			output.TileChances.Add(new TileChance("Yellow", "", finalrate));
			prefix = "Spotters";
			Type = ItemType.Developers;
		break;
		case 29:
			output.TileChances.Add(new TileChance("", "Bomb", finalrate));
			prefix = "Bombers";
			Type = ItemType.Developers;
		break;
		case 30:
			output.TileChances.Add(new TileChance("", "Sword", finalrate));
			prefix = "Fighters";
			Type = ItemType.Developers;
		break;
		case 31:
			output.TileChances.Add(new TileChance("", "Health", finalrate));
			prefix = "Healers";
			Type = ItemType.Developers;
		break;
		case 32:
			output.TileChances.Add(new TileChance("Alpha", "Cross", finalrate));
			prefix = "Holy";
			Type = ItemType.Developers;
		break;
		case 33:
			output.TileChances.Add(new TileChance("", "arcane", finalrate));
			prefix = "Casters";
			Type = ItemType.Developers;
		break;
	}	
}*/