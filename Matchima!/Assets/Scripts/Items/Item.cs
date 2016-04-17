using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;




public class Item : Slot {
	public List<string> desc = new List<string>();

	public string Description
	{
		get{
			return ConvertStringArrayToString(desc.ToArray());
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> final = new List<StCon>();
			string pref = ScaleGenus + ": " + ScaleRank;
			if(Player._Options.ShowNumbers) pref += " (" + ScaleRate.ToString("0.00") + ")";
			final.Add(new StCon(pref, GameData.Colour(ScaleGenus)));
			foreach(Upgrade child in AllUpgrades)
			{
				Color c = GameData.ItemColour(child.type);
				final.Add(new StCon(child.Title, c));
			}
			return final.ToArray();
		}
	}

	public int slot;
	public int value = 0;
	public int price = 0;
	public bool Equipped;
	//public Stat stats;
	public Ability Effect;
	public ItemType Type;

	public GENUS ScaleGenus;
	public float ScaleRate = 1.0F;
	public string ScaleRank
	{
		get{
			if(ScaleRate < 0.2F) return "D";
			else if (ScaleRate < 0.4F) return "C";
			else if (ScaleRate < 0.8F) return "B";
			else if (ScaleRate < 1.6F) return "A";
			else if (ScaleRate < 2.4F) return "S";
			else return "SS";
		}
	}

	public string ScaleString
	{
		get
		{
			return ScaleRank;// + "\n(" + ScaleGenus + ")";
		}
	}

	public int ScaleRankInt(float rate)
	{
			if(rate < 0.2F) return 1;
			else if (rate < 0.4F) return 2;
			else if (rate < 0.8F) return 3;
			else if (rate < 1.6F) return 4;
			else if (rate < 2.4F) return 5;
			else return 6;
	}
	private float ScalePointPerDiff = 0.02F;

	private int MAX_UPGRADES = 4;
	private int MAX_VALUE = 30;
	private int MAX_UNIQUE = 1;

	private int BasePrice = 5;

	private ItemBracket Masterwork = new ItemBracket("Masterwork", ItemType.Masterwork, 0.12F,
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

	private ItemBracket Basic = new ItemBracket("Basic", ItemType.Basic, 5.0F,
			new ItemGenusBracket("Red",
				new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.2F), //ATTACK
				new Upgrade(12, 0.5F,  5,"Hearty",  "Max HP", 5, ScaleType.GRADIENT) //REFLECT - CURRENTLY HEALTH MAX
				),
			new ItemGenusBracket("Blue",
				new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT,0.2F), //ATTACK
				new Upgrade(9,  0.45F, 1,"Spiked", "Spikes", 1, ScaleType.GRADIENT, 0.5F) //SPIKES
				),
			new ItemGenusBracket("Green",
				new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.2F), //ATTACK
				new Upgrade(24, 0.65F, 2,"Wise", "Max MP", 2, ScaleType.GRADIENT, 1) //RES MAX
				),
			new ItemGenusBracket("Yellow",
				new Upgrade(8,  5F, 1,"Sharp",  "Attack", 1, ScaleType.GRADIENT, 0.2F), //ATTACK
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
		);

	private ItemBracket Developers = new ItemBracket("Developers", ItemType.Developers, 0.22F,
		new ItemGenusBracket("Red",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travelers", "Map Y", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(25, 0.3F, 0.05F,"Spotters", "% Red Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(29, 0.15F, 0.05F,"Bombers", "% Bomb Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
			),
		new ItemGenusBracket("Blue",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(26, 0.3F, 0.05F,"Spotters", "% Blue Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(30, 0.15F, 0.05F,"Warriors", "% Sword Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
			),
		new ItemGenusBracket("Green",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(27, 0.3F, 0.05F,"Spotters", "% Green Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(33, 0.15F, 0.05F,"Mages", "% Arcane Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
			),
		new ItemGenusBracket("Yellow",
			new Upgrade(19, 0.1F, 1,"Voyagers", "Map X", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(20, 0.1F, 1,"Travellers", "Map Y", 1, ScaleType.STATIC, 0.2F), //MAP SIZE
			new Upgrade(28, 0.3F, 0.05F,"Spotters", "% Yellow Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(31, 0.15F, 0.05F,"Priests", "% Health Tiles", 5, ScaleType.GRADIENT), //RES CHANCE
			new Upgrade(32, 0.15F, 0.05F,"Clerics", "% Cross Tiles", 5, ScaleType.GRADIENT) //RES CHANCE
			)
		);

	private ItemBracket [] Brackets
	{
		get{
			return new ItemBracket[] {
				Masterwork, 
			//	Unique, 
				Crude, 
				Basic, 
				Developers
			};
		}
	}
	private float AllChance
	{
		get{
			return  Masterwork.Chance + 
				//	Unique.Chance + 
					Crude.Chance + 
					Basic.Chance + 
					Developers.Chance;
		}
	}

	private Item prev_item;
	public List<Upgrade> AllUpgrades = new List<Upgrade>();


	private string prefix = "";
	private int prefix_value = 0;


	public virtual Bonus CheckBonus()
	{
		return null;
	}

	protected override void Setup()
	{
		initialized = true;
		float finalscalerate = ((float)Parent.Stats[(int)ScaleGenus].StatCurrent * ScaleRate);
	}



	public void SetStats(Item prev = null, int added_val = 0, GENUS scalegenus = GENUS.NONE)
	{
		Drag = DragType.None;
		if(prev == null)
		{
			slot = Random.Range(0,5);
			price = BasePrice;


			float v = (float)added_val * 0.225F;
			v += UnityEngine.Random.Range(0, ScalePointPerDiff * GameManager.Difficulty);
			v += Random.value/3.0F;
			v = (v * Random.Range(0.4F, 1.6F));

			v = Mathf.Clamp(v, 0.0F, 3.0F);
			
			ScaleRate = v;
			ScaleGenus = scalegenus;
			if(ScaleGenus == GENUS.NONE || ScaleGenus == GENUS.ALL || ScaleGenus == GENUS.PRP)
			{
				ScaleGenus = (GENUS) Utility.RandomInt(3);
			}

			//print(ScaleGenus + ":" + ScaleRate + ":" + ScaleRank);


			GetItem((int)v);
		}
		else
		{
			Name_Basic = prev.Name_Basic;
			slot = prev.slot;
			price = prev.price;
			prev_item = prev.prev_item;
			AllUpgrades = prev.AllUpgrades;
			Effect = prev.Effect;
			Type = prev.Type;
			ScaleGenus = prev.ScaleGenus;
			ScaleRate = prev.ScaleRate;
		}

		//Colour = GameData.ItemColour(Type);
		Colour = GameData.Colour(ScaleGenus);
		GetInfo();
		gameObject.name = Name_Basic;
	}

	public void GetItem(int value)
	{
		int maxupgrades = (int)Mathf.Clamp((ScalePointPerDiff*GameManager.Difficulty) - ScaleRate, 1, MAX_UPGRADES);
		int upcount = Random.Range(1, maxupgrades);

		while(upcount > 0)
		{
			Upgrade u = null;
			float chance = Random.value * AllChance;
			float current = 0.0F;
			for(int i = 0; i < Brackets.Length; i++)
			{
				if(chance >= current && chance < current + Brackets[i].Chance)
				{
					GENUS g = (Random.value > 0.85F ? GENUS.NONE : ScaleGenus);
					u = new Upgrade(Brackets[i].GetUpgrade(g));
					break;
				}
				current += Brackets[i].Chance;
			}

			Upgrade prev = null;
			foreach(Upgrade child in AllUpgrades){if(child.index == u.index) prev = child;}

			if(prev == null) 
			{
				AllUpgrades.Add(u);
				//Upgrade(u);
				upcount --;
			}
		}
	}

	public override Stat GetStats()
	{
		Stat final = new Stat();
		float finalscalerate = ((float)Parent.Stats[(int)ScaleGenus].StatCurrent * ScaleRate);
		foreach(Upgrade child in AllUpgrades)
		{
			float rate = 0.0F;
			switch(child.scaletype)
			{
				case ScaleType.STATIC:
				rate = 1;
				break;
				case ScaleType.GRADIENT:
				rate = (finalscalerate * child.scalerate);
				break;
				case ScaleType.RANK:
				rate = (float)ScaleRankInt(finalscalerate * child.scalerate);

				//print(rate);
				break;
			}

			child.Points = (int) rate;
			Upgrade(child, final, rate);	
			/*for(int i = 0; i < rate; i++)
			{
				
			}*/
			
		}
		return final;
	}



	public void Upgrade(Upgrade up, Stat output, float rate)
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
				print(finalrate);
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
	}

	protected void GetInfo()
	{
		ItemInfo i = GameData._Items.GenerateInfo(slot);

		prefix = AllUpgrades[Random.Range(0, AllUpgrades.Count)].Prefix;
		string r = prefix + " " + i._Name;
		Icon = i._Sprite;
		Name_Basic = r;
		//switch(Type)
		//{
		//	case ItemType.Basic:
		//	name = r;
		//	break;
		//	case ItemType.Crude:
		//	name = "<color=#46b0ff>" + r + "</color>";
		//	break;
		//	case ItemType.Masterwork:
		//	name = "<color=#cf46ff>" + r +"</color>";
		//	break;
		//	case ItemType.Unique:
		//	name = "<color=orange>" + r +"</color>";
		//	break;
		//	case ItemType.Developers:
		//	name = "<color=#66ff66>" + r + "</color>";
		//	break;
		//}
	}

	protected string GetDescByValue(int newvalue, int oldvalue = 0, string desc = "")
	{
		if(newvalue < oldvalue) return "\n-<color=red>" + newvalue + desc + " </color>";
		else if(newvalue > oldvalue) return "\n+<color=green>" + newvalue + desc + " </color>";
		else return "\n~" + newvalue + desc;
	}

	protected string GetDescByFloat(float newvalue, float oldvalue = 0, string desc = "", float mult = 1.0F)
	{
		if(newvalue < oldvalue) return "\n-<color=red>" + (newvalue * mult) + desc + " </color>";
		else if(newvalue > oldvalue) return "\n+<color=green>" + (newvalue * mult) + desc + " </color>";
		else return "\n~" + (newvalue * mult) + desc;
	}	

	public void DeformatText()
	{
		for(int i = 0; i < desc.Count; i++)
		{
			string nonRichTextDesc = Regex.Replace(desc[i], "<.*?>", string.Empty);
			desc[i] = nonRichTextDesc;
		}
	}

	static string ConvertStringArrayToString(string[] array)
    {
		//
		// Concatenate all the elements into a StringBuilder.
		//
		StringBuilder builder = new StringBuilder();
		foreach (string value in array)
		{
		    builder.Append(value);
		}
		return builder.ToString();
    }

}


[System.Serializable]
public class UpgradeString
{
	public string Title 
	{
		get{
			return Prefix + (Points*Rate) + Suffix;
		}
	}
	public string Prefix, Suffix;
	public Color Colour;
	public int Points;
	public float Rate;

	public UpgradeString(string _prefix, string _suffix, Color _col, float _rate)
	{
		Prefix = _prefix;
		Suffix = _suffix;
		Colour = _col;
		Rate = _rate;

	}
}