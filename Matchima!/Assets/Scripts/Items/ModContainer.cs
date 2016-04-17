using UnityEngine;
using System.Collections;
using System;

public class ModContainer : MonoBehaviour {

	public ItemModContainer ItemMods =  new ItemModContainer(
	 new ItemBracket("Basic", ItemType.Basic, 1.0F,
		new ItemGenusBracket("Red",
			new Upgrade(12, 0.5F,  5,"Hearty",  "Max HP", 5, ScaleType.GRADIENT) //REFLECT - CURRENTLY HEALTH MAX
			),
		new ItemGenusBracket("Blue",
			new Upgrade(8,  1F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.5F), //ATTACK
			new Upgrade(9,  0.45F, 1,"Spiked", "Spikes", 1, ScaleType.GRADIENT, 0.5F) //SPIKES
			),
		new ItemGenusBracket("Green",
			new Upgrade(24, 0.65F, 2,"Wise", "Max MP", 2, ScaleType.GRADIENT, 1) //RES MAX
			),
		new ItemGenusBracket("Yellow",
			new Upgrade(24, 0.65F, 2,"Wise", "Max MP", 2, ScaleType.GRADIENT, 1) //RES MAX
			)	
		//new Upgrade(11, 0.0F, 1, "Attack",1, ScaleType.GRADIENT), //POISON - CURRENTLY ATTACK
		//new Upgrade(13, 0.0F, 1, "Hunter",1, ScaleType.GRADIENT), //HUNTER
		//new Upgrade(14, 0.0F, 1, "Healer",1, ScaleType.GRADIENT) //HEALER
		//new Upgrade(15, 0.3F,  15, 1, ItemType.Basic) //HARVESTER
	),


	  new ItemBracket("Masterwork", ItemType.Masterwork, 0.12F,
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
	),

	// ItemBracket Unique = new ItemBracket("Unique", ItemType.Unique, 0.0F, null
	//			),

	 new ItemBracket("Crude", ItemType.Crude, 0.7F,
		new ItemGenusBracket("Red",
			new Upgrade(12, 1.0F, 1,"Hearty", "Max HP", 1, ScaleType.GRADIENT, 2), //HEALTH MAX
			new Upgrade(7,  0.5F, 1,"Healing", "HP Regen",1, ScaleType.RANK) //REGEN
			),
		new ItemGenusBracket("Blue",
			new Upgrade(24, 0.65F, 1,"Wise", "Max MP", 1, ScaleType.GRADIENT, 1), //RES MAX
			new Upgrade(6,  0.35F, 1,"Vampiric", "HP Leech",1, ScaleType.RANK) //LEECH
			),
		new ItemGenusBracket("Green",
			new Upgrade(24, 0.65F, 1,"Wise", "Max MP", 1, ScaleType.GRADIENT, 1), //RES MAX
			new Upgrade(4, 0.1F, 1,"Draining", "MP Leech", 1, ScaleType.GRADIENT) //RES LEECH
			),
		new ItemGenusBracket("Yellow",
			new Upgrade(12, 1.0F, 1,"Hearty", "Max HP", 1, ScaleType.GRADIENT, 2), //HEALTH MAX
			new Upgrade(5, 0.1F, 1,"Learning", "MP Regen", 1, ScaleType.GRADIENT) //RES REGEN
			)
		//new Upgrade(0, 1.0F, 5, 5, ItemType.Crude, "DEX", 5), //STR
		//new Upgrade(1, 1.0F, 5, 5, ItemType.Crude, "WIS", 5), //DEX
		//new Upgrade(2, 1.0F, 5, 5, ItemType.Crude, "CHA", 5), //WIS
		//new Upgrade(3, 1.0F, 5, 5, ItemType.Crude, "STR", 5), //CHA
	),

	 new ItemBracket("Developers", ItemType.Developers, 0.22F,
		new ItemGenusBracket("Red",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travelers", "Map Y", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(25, 0.3F, 0.05F,"Spotters", "% Red Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(29, 0.15F, 0.05F,"Bombers", "% Bomb Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
		),
		new ItemGenusBracket("Blue",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(26, 0.3F, 0.05F,"Spotters", "% Blue Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(30, 0.15F, 0.05F,"Warriors", "% Sword Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
		),
		new ItemGenusBracket("Green",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(27, 0.3F, 0.05F,"Spotters", "% Green Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(33, 0.15F, 0.05F,"Mages", "% Arcane Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
		),
		new ItemGenusBracket("Yellow",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.RANK, 0.2F), //MAP SIZE
			new Upgrade(28, 0.3F, 0.05F,"Spotters", "% Yellow Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(32, 0.15F, 0.05F,"Clerics", "% Cross Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
		)
	)
	);

	public static UpgradeNew [] Basic = new UpgradeNew[0];

	public static UpgradeNew [] Generator = new UpgradeNew[] 
	{
		new UpgradeNew("Mage's", " chance of Arcane", 0.15F, ScaleType.GRADIENT,
			(Stat s, float value) => {
				s.TileChances.Add(new TileChance("", "arcane", 0.05F * value));}
			),
		new UpgradeNew("Cleric's", " chance of Cross", 0.15F, ScaleType.GRADIENT,
			(Stat s, float value) => {
				s.TileChances.Add(new TileChance("", "cross", 0.05F * value));
				}
			)
	};

	public static UpgradeNew [] Shift = new UpgradeNew[0];

	public static UpgradeNew [] Unstable = new UpgradeNew[0];

	public static UpgradeNew [] Primal = new UpgradeNew[0];

	public static UpgradeNew [] Elegant = new UpgradeNew[0];
	
	public static UpgradeNew [] Developers = new UpgradeNew []
	{
		/*new UpgradeNew(0.1F,	1.00F,	"Voyagers",		"Map X",			1, ScaleType.RANK, 0.2F), //MAP SIZE
		new UpgradeNew(0.1F,	1.00F,	"Travellers",	"Map Y",			1, ScaleType.RANK, 0.2F), //MAP SIZE
		new UpgradeNew(0.3F,	0.05F,	"Spotters",		"% Yellow Tiles",	5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.15F,	0.05F,	"Clerics",		"% Cross Tiles",	5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.3F,	0.05F,	"Spotters",		"% Green Tiles",	5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.15F,	0.05F,	"Mages",		"% Arcane Tiles",	5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.3F,	0.05F,	"Spotters",		"% Blue Tiles",		5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.15F,	0.05F,	"Warriors",		"% Sword Tiles",	5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.3F,	0.05F,	"Spotters",		"% Red Tiles",		5, ScaleType.GRADIENT), //RES CHANCE
		new UpgradeNew(0.15F,	0.05F,	"Bombers",		"% Bomb Tiles",		5, ScaleType.GRADIENT), //RES CHANCE
		*/
		new UpgradeNew("Cook's", "Map X", 0.1F, ScaleType.RANK, 
			(Stat s, float value) => {
				s.MapSize.x += (int) (1 * value);}
			),
		new UpgradeNew("Magellan's", "Map Y", 0.1F, ScaleType.RANK, 
			(Stat s, float value) => {
				s.MapSize.y += (int) (1 * value);}
			)
	};

}

[System.Serializable]
public class ItemModContainer
{
	public ItemBracket [] Brackets;
	public ItemModContainer(params ItemBracket [] _brackets)
	{
		Brackets = _brackets;
	}
}
[System.Serializable]
public class ItemBracket
{
	public string Title;
	public ItemType Type;
	public ItemGenusBracket [] Genus;
	public Upgrade [] Upgrades;

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
	public ItemBracket(string _title, ItemType _type, float _chance, params Upgrade [] u)
	{
		Title = _title;
		Type = _type;
		Chance = _chance;
		Upgrades = u;
	}

	public ItemBracket(string _title, ItemType _type, float _chance, params ItemGenusBracket [] b)
	{
		Title = _title;
		Type = _type;
		Chance = _chance;
		Genus = b;
		foreach(ItemGenusBracket child in Genus)
		{
			child.SetTypes(Type);
		}
	}


	public Upgrade GetUpgrade(GENUS g = GENUS.NONE)
	{
		if(g == GENUS.NONE)
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
public class ItemGenusBracket
{
	public string Name;
	public Upgrade [] Upgrades;
	public ItemGenusBracket(string _title, params Upgrade [] u)
	{
		Name = _title;
		Upgrades = u;
	}

	public void SetTypes(ItemType t)
	{
		foreach(Upgrade child in Upgrades)
		{
			child.type = t;
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
	Crude,
	Masterwork,
	Developers,
	Unique
}

public enum ScaleType
{
	STATIC,
	RANK,
	GRADIENT
}

[System.Serializable]
public class UpgradeNew
{
	public string Title
	{
		get{
			string pref = "+";
			if(Player._Options.ShowNumbers) pref += (Points*Rate);
			return pref + " " + Suffix;
		}
	}
	public string Suffix, Info;

	public float chance;
	public ItemType type;
	public ScaleType scaletype;
	public float scalerate;

	public float value;
	public int price;

	public int Points = 0;
	public float Rate = 1.0F;

	public Action<Stat, float> UpgradeMethod;
	

	public UpgradeNew(float _chance, float _scalevalue, string _prefix, string _suffix,  float _rate, ScaleType _scaletype = ScaleType.RANK, float _scalerate = 1)
	{
		Suffix = _suffix;
		//Prefix = _prefix;
		chance = _chance;
		value = _scalevalue;

		Rate = _rate;
		Points = 0;
		scaletype = _scaletype;
		scalerate = _scalerate;
	}

	public UpgradeNew(string _title, string _suffix, float _chance, ScaleType _sc, Action<Stat, float> _method)
	{

	}

	public UpgradeNew(UpgradeNew u)
	{
		Suffix = u.Suffix;
		//Prefix = u.Prefix;
		chance = u.chance;
		value = u.value;
		
		type = u.type;

		Points = u.Points;
		Rate = u.Rate;
		scaletype = u.scaletype;
		scalerate = u.scalerate;
	}
}

[System.Serializable]
public class Upgrade
{
	public string Title
	{
		get{
			string pref = "+";
			if(Player._Options.ShowNumbers) pref += (Points*Rate);
			return pref + " " + Suffix;
		}
	}
	public string Suffix;
	public int index;
	public int subindex;
	public float chance;
	public ItemType type;
	public ScaleType scaletype;
	public float scalerate;
	public float value;
	public int price;

	public int Points = 0;
	public float Rate = 1.0F;
	public string Prefix;

	public Upgrade(int _index, float _chance, float _scalevalue, string _prefix, string _suffix,  float _rate, ScaleType _scaletype = ScaleType.RANK, float _scalerate = 1)
	{
		Suffix = _suffix;
		Prefix = _prefix;
		index = _index;
		chance = _chance;
		value = _scalevalue;

		Rate = _rate;
		Points = 0;
		scaletype = _scaletype;
		scalerate = _scalerate;
	}

	public Upgrade(Upgrade u)
	{
		Suffix = u.Suffix;
		Prefix = u.Prefix;
		index = u.index;
		subindex = u.subindex;
		chance = u.chance;
		value = u.value;
		//price = u.price;
		type = u.type;

		Points = u.Points;
		Rate = u.Rate;
		scaletype = u.scaletype;
		scalerate = u.scalerate;
	}
}
