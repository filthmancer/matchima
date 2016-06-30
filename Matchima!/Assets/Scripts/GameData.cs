using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization; 
using System.IO;
using System;


public class GameData : MonoBehaviour {
	public static GameData instance;
	void Awake() {
		if(instance == null)
		{
			//DontDestroyOnLoad(transform.gameObject);
			instance = this;
		}
		else if(instance != this) 
		{
			instance.Invoke("Start",0.05F);
			Destroy(this.gameObject);
		}
	}

	public int AppVersion = 082;
	public GameObject Item;
	public static GameObject TileModel;
	public Color Strength, Dexterity, Wisdom, Charisma, Spell, Grey;
	public Color Combo;
	public ItemColours _ItemColours;
	public Color GoodColour, BadColour;
	public Color GoodColourFill, BadColourFill;
	public Color ShieldFull, ShieldEmpty;

	//public ClassContainer [] Classes;
	public Class [] Classes;
	//public ClassContainer EndlessMode;

	public AbilityContainer [] Abilities, ClassAbilities;

	public Ability [] AbilityPrefabs;
	public Ability [] TeamAbilities;
	public ItemNameContainer ItemNames;
	public static ItemNameContainer _Items;

	public static Wave [] _Waves;
	public static Ability [] _Abilities;
	public static Status [] _Status;
	public ItemInfo [] _Icons;

	public GameObject WaveParent;
	public GameObject AbilityParent;
	public GameObject TileEffectParent;

	private float abilities_allchance = 0.0F;

	public static bool loading_assets = false;
	public static bool loaded_assets = false;

	public bool PrintLogs;

	// Use this for initialization
	void Start () {
		LoadClasses();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadAssets()
	{
		loading_assets = true;
		StartCoroutine(LoadAssets_Routine());
	}


	public static Vector3 RandomVector
	{
		get{
			return new Vector3(UnityEngine.Random.value - UnityEngine.Random.value, UnityEngine.Random.value - UnityEngine.Random.value, UnityEngine.Random.value - UnityEngine.Random.value);
		}
	}

	public static void Log(string s, Type t = null)
	{
		if(!GameData.instance.PrintLogs) return;
		if(t != null) Debug.Log(t + " : " + s);
		else Debug.Log(s);
	}

	public static float GameSpeed(float f)
	{
		return f * Player.Options.GameSpeed * Time.deltaTime * 60;
	}

	public Sprite [] GetTileSprite(string s)
	{
		return TileMaster.Types.SpriteOf(s);
	}

	public static string Genus(GENUS g)
	{
		return GENUSToString(g);
	}

	public static Color Colour(GENUS g)
	{
		return GameData.instance.GetGENUSColour(g);
	}

	public Color GetGENUSColour(GENUS ab)
	{
		switch(ab)
		{
			case GENUS.STR:
			return Strength;
			case GENUS.DEX:
			return Dexterity;
			case GENUS.WIS:
			return Wisdom;
			case GENUS.CHA:
			return Charisma;
			case GENUS.PRP:
			return Spell;
			case GENUS.OMG:
			return Grey;
		}
		return Color.white;
	}

	public static Color ItemColour(ItemType t)
	{
		return GameData.instance.GetItemColour(t);
	}

	public Color GetItemColour(ItemType t)
	{
		switch(t)
		{
			case ItemType.Basic: 
			return _ItemColours.Basic;
			
			case ItemType.Generator: 
			return _ItemColours.Generator;
			
			case ItemType.Shift: 
			return _ItemColours.Shift;
			
			case ItemType.Unstable:
			return _ItemColours.Unstable;
			
			case ItemType.Primal:
			return _ItemColours.Primal;
			
			case ItemType.Elegant:
			return _ItemColours.Elegant;
			
			case ItemType.Developers:
			return _ItemColours.Developers;
			
		}
		return Color.white;
	}

	public Ability GetTeamAbility()
	{
		int r = TeamAbilities.Length;
		r = Utility.RandomInt(r);
		return TeamAbilities[r];
	}


	public Ability GetAbilityByName(string name)
	{

		for(int i = 0 ; i < _Abilities.Length; i++)
		{
			//print(name + ":" + _Abilities[i].name);
			if(_Abilities[i].name == name) return _Abilities[i];
		}

		return null;
	}

	public Status GetTileEffectByName(string name)
	{

		for(int i = 0 ; i < _Status.Length; i++)
		{
			//print(name + ":" + _Status[i].name);
			if(_Status[i].Name == name) return _Status[i];
		}

		return null;
	}

	public Slot GetMod(string name)
	{
		for(int i = 0; i < _Abilities.Length; i++)
		{
			if(_Abilities[i].name == name) return _Abilities[i] as Slot;
		}
		return null;
	}

	public static GENUS StringToGENUS(string g)
	{
		switch(g)
		{
			case "STR":
			return GENUS.STR;
			case "DEX":
			return GENUS.DEX;
			case "WIS":
			return GENUS.WIS;
			case "CHA":
			return GENUS.CHA;
			case "Red":
			return GENUS.STR;
			case "Blue":
			return GENUS.DEX;
			case "Green":
			return GENUS.WIS;
			case "Yellow":
			return GENUS.CHA;
		}
		return GENUS.STR;
	}

	public static string GENUSToString(GENUS g)
	{
		switch(g)
		{
			case GENUS.STR:
			return "STR";
			case GENUS.DEX:
			return "DEX";
			case GENUS.WIS:
			return "WIS";
			case GENUS.CHA:
			return "CHA";
			case GENUS.OMG:
			return "Omega";
			case GENUS.PRP:
			return "Purple";
			case GENUS.ALL:
			return "Alpha";
		}
		return "None";
	}

	public static string GENUSToResourceString(GENUS g)
	{
		switch(g)
		{
			case GENUS.STR:
			return "RED";
			case GENUS.DEX:
			return "BLU";
			case GENUS.WIS:
			return "GRN";
			case GENUS.CHA:
			return "YLW";
			case GENUS.PRP:
			return "PRP";
			case GENUS.ALL:
			return "APH";
			case GENUS.OMG:
			return "OMG";
		}
		return "None";
	}

	public static string Resource(GENUS g)
	{
		return GameData.GENUSToResourceString(g);
	}

	public static string ResourceLong(GENUS g)
	{
		switch(g)
		{
			case GENUS.STR:
			return "Red";
			case GENUS.DEX:
			return "Blue";
			case GENUS.WIS:
			return "Green";
			case GENUS.CHA:
			return "Yellow";
			case GENUS.PRP:
			return "Purple";
			case GENUS.ALL:
			return "Alpha";
			case GENUS.OMG:
			return "Omega";
		}
		return "None";
	}

	public static string StatLong(GENUS g)
	{
		switch(g)
		{
			case GENUS.STR:
			return "Strength";
			case GENUS.DEX:
			return "Dexterity";
			case GENUS.WIS:
			return "Wisdom";
			case GENUS.CHA:
			return "Charisma";
		}
		return "None";
	}

	public static string Stat(GENUS g)
	{
		switch(g)
		{
			case GENUS.STR:
			return "STR";
			case GENUS.DEX:
			return "DEX";
			case GENUS.WIS:
			return "WIS";
			case GENUS.CHA:
			return "CHA";
		}
		return "None";
	}

	public static bool GenusIsResource(GENUS g)
	{
		switch(g)
		{
			case GENUS.STR: return true;
			case GENUS.DEX: return true;
			case GENUS.WIS: return true;
			case GENUS.CHA: return true;
		}
		return false;
	}
	
	public static int StringToInt(string num)
	{
		try
		{
			return Convert.ToInt32(num);
		}
		catch
		{
			return -100;
		}
	}

	public static float StringToFloat(string num)
	{
		return Convert.ToSingle(num);
	}

	public static string PowerString(int num)
	{
		string suff = "";
		int suffnum = 0;

		int n = num;
		
		bool ispowering = true;
		while(ispowering)
		{
			if(n <= 1) break;
			if(n < 1000) ispowering = false;
			else 
			{
				n /= 1000;
				suffnum ++;
				suff = power_suff[suffnum];
			}
		}
		return n + suff;
	}

	static string [] power_suff = new string []
	{
		"",
		"k",
		"m",
		"b",
		"t"
	};

	public Wave GetRandomWave()
	{
		float allchance = 0.0F;
		List<Wave> r_waves = new List<Wave>();
		foreach(Wave child in _Waves)
		{
			if(child.RequiredDifficulty <= GameManager.Difficulty)
			{
				allchance += child.Chance;
				r_waves.Add(child);
			}
		}
		float rand = UnityEngine.Random.value * allchance;
		float currchance = 0.0F;

		foreach(Wave child in r_waves)
		{
			if(child.RequiredDifficulty > GameManager.Difficulty) continue;
			if(rand > currchance && rand <= currchance + child.Chance)
			{
				return child;
			}
			currchance += child.Chance;
		}
		return null;
	}

	public Wave GetWave(params string [] s)
	{
		return GetWaveByName(s[0]);
	}

	public Wave GetWaveByName(string s)
	{
		foreach(Wave child in _Waves)
		{
			if(child.Name == s) return child;
		}
		return null;
	}

	public Sprite GetIconByName(string s)
	{
		foreach(ItemInfo child in _Icons)
		{
			if(child._Name == s) return child._Sprite;
		}
		//Debug.LogError("NO ICON FOUND: " + s);
		return null;
	}


//SAVE STATE

	public void Save()
	{
		//bool finished_save = false;
		XmlDocument file = new XmlDocument();
		
		XmlElement data = (XmlElement) file.AppendChild(file.CreateElement("PlayerData"));
		//data.InnerText = "" + AppVersion;
		XmlElement build = (XmlElement) data.AppendChild(file.CreateElement("Build"));
		build.InnerText = "" + AppVersion;

		//XmlElement classes = (XmlElement) data.AppendChild(file.CreateElement("Classes"));
		/*foreach(ClassContainer child in Classes)
		{
			XmlElement title = (XmlElement) classes.AppendChild(file.CreateElement("Class"));

			XmlElement name = (XmlElement) title.AppendChild(file.CreateElement("Name"));
			name.InnerText = child.Name;
			XmlElement unlocked = (XmlElement) title.AppendChild(file.CreateElement("Unlocked"));
			unlocked.InnerText = child.Unlocked ? "T" : "F";
			XmlElement level = (XmlElement) title.AppendChild(file.CreateElement("Level"));
			level.InnerText =  "" + child.Level;
			XmlElement ptl = (XmlElement) title.AppendChild(file.CreateElement("LevelUpCost"));
			ptl.InnerText =  "" + child.LevelUpCost;
		}*/


		XmlSerializer SerializerObj = new XmlSerializer(typeof(PlayerData));
			
		// Create a new file stream to write the serialized object to a file

		string filepos = Application.persistentDataPath;
		/*if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			filepos = GetIOSPath();
		}*/	

		FileStream WriteFileStream = File.Create(filepos +  "/PlayerData" + ".xml");

		SerializerObj.Serialize(WriteFileStream, file);
		 
		// Cleanup
		WriteFileStream.Close();

		//finished_save = true;
		Debug.Log("Generated level at " + filepos +  "/PlayerData" + ".xml");	
		
	}

	public void Load()
	{
		#if !UNITY_WEBPLAYER
		
		XmlDocument xmldoc = new XmlDocument (); 
		string datapath = Application.persistentDataPath + "/" + "PlayerData" + ".xml";
		
		if(System.IO.File.Exists(datapath))
		{
			xmldoc.LoadXml (System.IO.File.ReadAllText(datapath));
			XmlNode root = xmldoc.ChildNodes[1];

			XmlNode build = root.ChildNodes[0];

			if(StringToInt(build.InnerText) != AppVersion)
			{
				Save();
				Load();
				return;
			}
			
		//Class Data
		//XmlNode classroot = root.ChildNodes[1];
		//for(int i = 0; i < classroot.SelectNodes("//Class").Count; i++)
		//{
		//	XmlNode c = classroot.SelectNodes("//Class")[i];
		//	string name = c.SelectNodes("Name")[0].InnerText;
		//	bool unlocked = c.SelectNodes("Unlocked")[0].InnerText == "T";
		//	int level = StringToInt(c.SelectNodes("Level")[0].InnerText);
		//	LoadClass(i, name, unlocked, level);
		//	
		//}
			//UIManager.Menu.CheckClassButtons();
			
		}
		else Save();
		#elif UNITY_WEBPLAYER
		LoadAbilities();
		#endif
	}

	public void LoadAbilities()
	{
		XmlDocument xmldoc = new XmlDocument ();
		//Ability Data
		List<AbilityContainer> UnlockedAbilities = new List<AbilityContainer>();
		List<AbilityContainer> UnlockedClassAbilities = new List<AbilityContainer>();

		TextAsset skills = (TextAsset) Resources.Load("SkillParameters");
		xmldoc.LoadXml(skills.text);

		XmlNode abilityroot = xmldoc.ChildNodes[0].ChildNodes[0];

		int ab_num = abilityroot.ChildNodes.Count;
		
		abilities_allchance = 0.0F;

		for(int i = 0; i < ab_num; i++)
		{
			XmlNode a = abilityroot.ChildNodes[i];
			AbilityContainer new_ab = GetAbilityFromXml(a);
			if(!new_ab.Unlocked) continue;
			UnlockedAbilities.Add(new AbilityContainer(new_ab));
			abilities_allchance += new_ab.Chance;
		}

		XmlNode class_abilityroot = xmldoc.ChildNodes[0].ChildNodes[1];

		for(int i = 0; i < class_abilityroot.SelectNodes("//Class").Count; i++)
		{
			XmlNode ca = class_abilityroot.SelectNodes("//Class")[i];
			string classname = ca.SelectNodes("Name")[0].InnerText;
			ClassContainer c_con = GetClassContainer(classname);
			if(c_con == null) {
				Debug.LogError("Could not load class abilities! " + classname);
				continue;
			}
			ab_num = ca.ChildNodes[1].SelectNodes("Ability").Count;

			c_con.Abilities = new AbilityContainer[ab_num];
			for(int a = 0; a < ab_num; a++)
			{
				XmlNode ab = ca.ChildNodes[1].ChildNodes[a];
				
				AbilityContainer new_ab = GetAbilityFromXml(ab);

				int unlock_lvl = StringToInt(ab.SelectNodes("UnlockLvl")[0].InnerText);
				if(new_ab.Unlocked || c_con.Level >= unlock_lvl)
				{
					new_ab.Unlocked = true;
					UnlockedClassAbilities.Add(new AbilityContainer(new_ab));
				} 
				c_con.Abilities[a] = new AbilityContainer(new_ab);
			}
		}
		Abilities = UnlockedAbilities.ToArray();
		ClassAbilities = UnlockedClassAbilities.ToArray();
	}

	public AbilityContainer GetAbilityFromXml(XmlNode a)
	{
		AbilityContainer new_ab = new AbilityContainer();

		string script = a.SelectNodes("Script")[0].InnerText;
		string name = a.SelectNodes("Name")[0].InnerText;
		string icon = a.SelectNodes("Icon")[0].InnerText;

		bool unlocked = (a.SelectNodes("Unlocked")[0].InnerText == "T");

		float chance = StringToFloat(a.SelectNodes("Chance")[0].InnerText);
		int cd_min = StringToInt(a.SelectNodes("Cooldown")[0].SelectNodes("RangeMin")[0].InnerText);
		int cd_max = StringToInt(a.SelectNodes("Cooldown")[0].SelectNodes("RangeMax")[0].InnerText);

		new_ab.Name = name;
		new_ab.Icon = icon;
		new_ab.AbilityScript = script;
		new_ab.Chance = chance;
		new_ab.Unlocked = unlocked;
		new_ab.CooldownMin = cd_min;
		new_ab.CooldownMax = cd_max;

		float cost_chance = StringToFloat(a.SelectNodes("CostChance")[0].InnerText);
		int cost_min = StringToInt(a.SelectNodes("Cost")[0].SelectNodes("RangeMin")[0].InnerText);
		int cost_max = StringToInt(a.SelectNodes("Cost")[0].SelectNodes("RangeMax")[0].InnerText);
		string costType = a.SelectNodes("CostType")[0].InnerText;
		bool cost_reduce = a.SelectNodes("ReduceCooldown")[0].InnerText == "T";

		string mod = a.SelectNodes("AbilityMod")[0].InnerText;
		int mod_mult = StringToInt(a.SelectNodes("AbilityModMult")[0].InnerText);

		new_ab.CostChance = cost_chance;
		new_ab.CostType = costType;
		new_ab.CostMin = cost_min;
		new_ab.CostMax = cost_max;
		new_ab.CostReducesCooldown = cost_reduce;
		new_ab.StatType = mod;
		new_ab.StatMultiplier = mod_mult;

		XmlNode starttypes = a.SelectNodes("Inputs")[0];
		int start_num = starttypes.SelectNodes("Input").Count;
		new_ab.Input = new ContainerData[start_num];
		for(int x = 0; x < start_num; x++)
		{
			XmlNode starttype = starttypes.SelectNodes("Input")[x];
			new_ab.Input[x] = new ContainerData(starttype.ChildNodes.Cast<XmlNode>().Select(node => node.InnerText).ToArray());		
		}

		XmlNode endtypes = a.SelectNodes("Outputs")[0];
		int end_num = endtypes.SelectNodes("Output").Count;
		new_ab.Output = new ContainerData[end_num];
		for(int x = 0; x < end_num; x++)
		{
			XmlNode endtype = endtypes.SelectNodes("Output")[x];
			new_ab.Output[x] = new ContainerData(endtype.ChildNodes.Cast<XmlNode>().Select(node => node.InnerText).ToArray());
		}

		return new_ab;
	}

	public void LoadClass(int num, string name, bool un, int lvl)
	{
		//if(Classes[num].Name != name) 
		//{
		//	Debug.LogError("LOADED INTO WRONG CLASS");
		//	return;
		//}
		//Classes[num].Unlocked = un;
		//Classes[num].Level = lvl;
	}

	public void LoadClassAbilities(Class c)
	{
		UnityEngine.Object [] class_ab = Resources.LoadAll("Abilities/" + c.Name);
		List<Ability> final = new List<Ability>();
		for(int i = 0; i < class_ab.Length; i++)
		{
			GameObject abobj = (GameObject) class_ab[i];
			final.Add((Ability) abobj.GetComponent<Slot>());
			//c.GenerateSlotUpgrade(abobj.GetComponent<Slot>());
		}

		UnityEngine.Object [] default_ab = Resources.LoadAll("Abilities/Default");
		
		for(int i = 0; i < default_ab.Length; i++)
		{
			GameObject abobj = (GameObject) default_ab[i];
			final.Add((Ability) abobj.GetComponent<Slot>());
			//c.GenerateSlotUpgrade(abobj.GetComponent<Slot>());
		}

		TeamAbilities = final.ToArray();
	}

	public string GetIOSPath()
	{
		string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
		path = path.Substring(0,path.LastIndexOf('/'));
		return path + "/Documents";
	}

	public Ability GetRandomAbility()
	{
		List<AbilityContainer> all_ab = new List<AbilityContainer>();
		all_ab.AddRange(Abilities);
				
		float allchance = 0.0F;
		foreach(AbilityContainer child in all_ab)
		{
			if(child.Unlocked) allchance += child.Chance;
		}

		float chance = UnityEngine.Random.value * allchance;
		float chance_current = 0.0F;
		AbilityContainer Container = null;

		for(int i = 0; i < all_ab.Count; i++)
		{
			if(!all_ab[i].Unlocked) continue;

			if(chance > chance_current && chance < chance_current + all_ab[i].Chance)
			{
				Container = all_ab[i];
				break;
			}
			chance_current += all_ab[i].Chance;
		}

		if(Container == null) 
		{
			Debug.LogError("Could not generate skill!");
			return null;
		}
		Ability ab = (Ability) Instantiate(GetAbilityByName(Container.AbilityScript));
		ab.Setup(Container);

		return ab;
	}

	public void LoadClasses()
	{
		int num = 0;
		UnityEngine.Object [] classes = Resources.LoadAll("Classes");
		Classes = new Class[classes.Length];
		for(int i = 0; i < classes.Length; i++)
		{
			GameObject cobj = classes[i] as GameObject;
			Classes[i] = cobj.GetComponent<Class>();
			if(Classes[i]) num++;
		}
		print("Loaded " + num + " classes");
		List<Class> final = new List<Class>();
		final.AddRange(Classes);
		final = final.OrderBy(o=>!o.Unlocked).ToList();
		Classes = final.ToArray();
	}


	IEnumerator LoadAssets_Routine()
	{
		_Items = ItemNames;
		//_Waves = new Wave[WaveParent.transform.childCount];
		//for(int i = 0; i < WaveParent.transform.childCount; i++)
		//{
		//	_Waves[i] = WaveParent.transform.GetChild(i).GetComponent<Wave>();
		//	_Waves[i].Index = i;
		//}

		TileModel = (GameObject) Resources.Load("TileModel");

		UnityEngine.Object[] textures = Resources.LoadAll("Icons");
		//Sprite [] textures = (Sprite[]) Resources.LoadAll("Icons");
		_Icons = new ItemInfo[textures.Length];
		for(int i = 0; i < _Icons.Length; i++)
		{
			_Icons[i] = new ItemInfo();
			_Icons[i]._Name = textures[i].name;
			Texture2D tex = (Texture2D) textures[i];
			Sprite newSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.one/2);
			_Icons[i]._Sprite = newSprite;
			if(i % 5 == 0) yield return null;
		}

		
		_Abilities = new Ability[AbilityParent.transform.childCount];
		for(int i = 0; i < AbilityParent.transform.childCount; i++)
		{
			_Abilities[i] = AbilityParent.transform.GetChild(i).GetComponent<Ability>();
			//_Abilities[i].Index = i;
		}
		AbilityParent.SetActive(false);

		_Status = new Status[TileEffectParent.transform.childCount];
		for(int i = 0; i < TileEffectParent.transform.childCount; i++)
		{
			_Status[i] = TileEffectParent.transform.GetChild(i).GetComponent<Status>();
			//_Abilities[i].Index = i;
		}

		TileEffectParent.SetActive(false);

		yield return null;
		//LoadAbilities();

		yield return StartCoroutine(AudioManager.instance.LoadAudio("Tiles"));
		yield return StartCoroutine(TileMaster.Types.LoadSprites("Tiles"));
		yield return StartCoroutine(TileMaster.Types.LoadPrefabs());

		print("FINISHED LOADING");
		loaded_assets = true;
		
		yield return null;
	}



//Get Class by Name
	public ClassContainer GetClassContainer(string name)
	{
		//foreach(ClassContainer child in Classes)
		//{
		//	if(name == child.Name || name == child.ShortName) 
		//	{
		//		return child;
		//	}
		//}
		return null;
	}

	public Class GetClass(string name)
	{
		foreach(Class child in Classes)
		{
			if(name == child.Name || name == child.Info.ShortName)
			{
				return child;
			}
		}
		return null;
	}

	public static float DeltaSeconds(float amt)
	{
		return (Time.deltaTime * Application.targetFrameRate) / amt;
	}

	public static Rarity ClosestRarity(int r)
	{
		if(r < 2) return Rarity.Common;
		else if(r >= 2 && r <= 6) return Rarity.Uncommon;
		else if(r >= 6 && r <= 12) return Rarity.Magic;
		else if(r >= 12 && r <= 24) return Rarity.Rare;
		else if(r >= 24 && r <= 50) return Rarity.Legendary;
		else return Rarity.Ultimate;
	}

}


public enum Rarity
{
	Common = 1,
	Uncommon = 2,
	Magic = 6,
	Rare = 12,
	Legendary = 24,
	Ultimate = 50,
	Impossible = 0
}


[Serializable, XmlRoot("PlayerData")]
public class PlayerData
{
	public string [] Classes { get; set; }
}

[System.Serializable]
public class ItemNameContainer
{
	public Infos Helm, Chest, Weapon, Shield, Boots;
	public string [] Prefix;

	public UniqueItem [] Unique;
	public UniqueItem GetRandomUnique()
	{
		return Unique[UnityEngine.Random.Range(0, Unique.Length)];
	}

	public ItemInfo GenerateInfo(int slot, string mod = "")
	{
		ItemInfo type = null;
		switch(slot)
		{
			case 0:
			type = Helm.RandomType;
			break;
			case 1:
			type = Chest.RandomType;
			break;
			case 2:
			type = Weapon.RandomType;
			break;
			case 3:
			type = Shield.RandomType;
			break;
			case 4:
			type = Boots.RandomType;
			break;
		}
		return type;
	}

	public string RandomPrefix
	{
		get{ return Prefix[UnityEngine.Random.Range(0, Prefix.Length)];}
	}
}

[System.Serializable]
public class UniqueItem
{
	public string Name;
	public string Description;
	public Stat Stats;
	public Ability Effect;
}

[System.Serializable]
public class Infos
{
	public ItemInfo [] Types;

	public ItemInfo RandomType
	{
		get { return Types[UnityEngine.Random.Range(0, Types.Length)];}
	}
}

[System.Serializable]
public class ItemInfo
{
	public string _Name;
	public Sprite _Sprite;
}

[System.Serializable]
public class ItemColours
{
	public Color Basic, 
	Generator, 
	Shift, 
	Unstable,
	Primal,
	Elegant,
	Developers;
}



public class StCon
{
	public string Value;
	public Color Colour;
	public bool NewLine = true;
	public float Size = 80;
	public StCon(string v, Color? c = null, bool line = true, float _size = 70)
	{
		Value = v;
		Colour = c ?? Color.white;
		NewLine = line;
		Size = _size;
	}

	public StCon (StCon prev, bool line = true)
	{
		Value = prev.Value;
		Colour = prev.Colour;
		Size = prev.Size;
		NewLine = line;
	}
}