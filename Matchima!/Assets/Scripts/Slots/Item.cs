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
			if(Player.Options.ShowNumbers) pref += " (" + ScaleRate.ToString("0.00") + ")";
			final.Add(new StCon(pref, GameData.Colour(ScaleGenus)));
			foreach(Upgrade child in AllUpgrades)
			{
				Color c = GameData.ItemColour(child.type);
				if(Parent != null) final.Add(new StCon(child.Title, c));
				else final.Add(new StCon(child.Title_NoParent, c));
			}
			return final.ToArray();
		}
	}

	public int value = 0;
	public bool Equipped;
	public Ability Effect;
	public ItemType Type;

	public GENUS ScaleGenus;
	public float ScaleRate = 1.0F;
	public string ScaleRank
	{
		get{
			if(ScaleRate < 0.8F) return "D";
			else if (ScaleRate < 2.0F) return "C";
			else if (ScaleRate < 3.4F) return "B";
			else if (ScaleRate < 4.4F) return "A";
			else if (ScaleRate < 6.0F) return "S";
			else return "SS";
		}
	}

	private static float ScalePointPerDiff = 0.007F;
	private static int MAX_UPGRADES = 4;
	private static int MAX_VALUE = 30;
	private static int MAX_UNIQUE = 1;
	private static float MaxScalePoint = 6.2F;

	public string ScaleString
	{
		get
		{
			return ScaleRank;
		}
	}

	public int ScaleRankInt(float rate)
	{
			if(rate < 0.8F) return 1;
			else if (rate < 2.0F) return 2;
			else if (rate < 3.4F) return 3;
			else if (rate < 4.4F) return 4;
			else if (rate < 6.0F) return 5;
			else return 6;
	}


	public List<Upgrade> AllUpgrades = new List<Upgrade>();
	private string prefix = "";
	public bool Seen;
	//private int prefix_value = 0;


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
			float v = (float)added_val * 0.14F;
			v += UnityEngine.Random.Range(0, ScalePointPerDiff * GameManager.Difficulty);
			v += Random.value/3.0F;
			v = (v * Random.Range(0.4F, 1.4F));

			v = Mathf.Clamp(v, 0.0F, MaxScalePoint);
			
			ScaleRate = v;
			ScaleGenus = scalegenus;
			if(ScaleGenus == GENUS.NONE || ScaleGenus == GENUS.ALL || ScaleGenus == GENUS.PRP)
			{
				ScaleGenus = (GENUS) Utility.RandomInt(3);
			}
			GetItem((int)v);
		}
		else
		{
			Name_Basic = prev.Name_Basic;
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
		int maxupgrades = 1;
		float upchance = (float) GameManager.Difficulty/100.0F;
		while(Random.value < upchance)
		{
			maxupgrades++;
			upchance*=0.35F;
		}
		int upcount = (int)Mathf.Clamp(maxupgrades, 1, MAX_UPGRADES);

		ScaleRate /= upcount;

		while(upcount > 0)
		{
			Upgrade u = null;
			float chance = Random.value * ModContainer.AllChance;
			float current = 0.0F;
			for(int i = 0; i < ModContainer.Brackets.Length; i++)
			{
				if(chance >= current && chance < current + ModContainer.Brackets[i].Chance)
				{
					GENUS g = (Random.value > 0.92F ? GENUS.NONE : ScaleGenus);
					u = new Upgrade(ModContainer.Brackets[i].GetUpgrade(g));
					break;
				}
				current += ModContainer.Brackets[i].Chance;
			}

			//Upgrade prev = null;
			//foreach(Upgrade child in AllUpgrades){if(child != null && child.Index == u.Index) prev = child;}
			//if(prev == null) 
			//{
				AllUpgrades.Add(u);
				//Upgrade(u);
				upcount --;
			//}
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
				rate = (float)ScaleRankInt(ScaleRate * child.scalerate);
				break;
			}
			if(rate < 1.0F) rate = 1.0F;
			child.Up(final, rate);	
			
		}
		return final;
	}

	protected void GetInfo()
	{
		ItemInfo i = GameData._Items.GenerateInfo(Random.Range(0,5));

		prefix = AllUpgrades[Random.Range(0, AllUpgrades.Count)].Prefix;
		string r = prefix + " " + i._Name;
		Icon = i._Sprite;
		Name_Basic = r;
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