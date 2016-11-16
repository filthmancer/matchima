﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization; 
using System.IO;
using System;


public class GameData : MonoBehaviour {
#region Variables
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

	public Class [] Classes;
	public AbilityContainer [] Abilities, ClassAbilities;

	public Powerup [] Powerups;

	public Ability [] AbilityPrefabs;
	public Ability [] TeamAbilities;
	public ItemNameContainer ItemNames;
	public static ItemNameContainer _Items;

	public static Wave [] _Waves;
	public static Ability [] _Abilities;
	public static Status [] _Status;
	public ItemInfo [] _Icons;

	public ZoneMapContainer StoryModeMap, EndlessModeMap, DeepModeMap;
	public Zone [] Zones;
	public bool ModeUnlocked_Endless, ModeUnlocked_Quick, ModeUnlocked_Deep;

	public GameObject WaveParent;
	public GameObject AbilityParent;
	public GameObject TileEffectParent;

	private float abilities_allchance = 0.0F;

	public static bool loading_assets = false;
	public static bool loaded_assets = false;
	public static bool ChestsFromEnemies = true;
	public static bool GetBonuses = true;
	public static bool FullVersion;

	public bool PrintLogs;

#endregion

#region Generics
	public static Vector3 RandomVector
	{
		get{
			return new Vector3(UnityEngine.Random.value - UnityEngine.Random.value, UnityEngine.Random.value - UnityEngine.Random.value, UnityEngine.Random.value - UnityEngine.Random.value);
		}
	}

	public static float DeltaSeconds(float amt)
	{
		return (Time.deltaTime * Application.targetFrameRate) / amt;
	}

	public static void Log(string s, Type t = null)
	{
		if(!GameData.instance.PrintLogs) return;
		if(t != null) Debug.Log(t + " : " + s);
		else Debug.Log(s);
	}

	public static float GameSpeed(float f, float ratio = 1.0F)
	{
		return f / (Player.Options.GameSpeed/ratio) * Time.deltaTime * 60 * 1.3F;
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

	public GameObject ActionCaster(Transform parent, Tile target, Action a)
	{
		target.SetState(TileState.Selected, true);
		GameObject initpart = EffectManager.instance.PlayEffect(parent, Effect.Spell);
		MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
		mp.SetTarget(target.transform.position);
		mp.SetPath(25.0F, 0.3F);
		mp.Target_Tile = target;
		mp.SetThreshold(0.1F);
		mp.SetMethod(a);
		return initpart;
	}

	public static IEnumerator DeltaWait(float time)
	{
		float final = 0.0F;
		while(final < time)
		{
			final += Time.deltaTime;
			yield return null;
		}
		yield break;
	}
#endregion

#region Tile/Type Info
	public Sprite [] GetTileSprite(string s)
	{
		return TileMaster.Types.SpriteOf(s);
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
		g = g.ToLower();
		switch(g)
		{
			case "str":
			return GENUS.STR;
			case "dex":
			return GENUS.DEX;
			case "wis":
			return GENUS.WIS;
			case "cha":
			return GENUS.CHA;
			case "red":
			return GENUS.STR;
			case "blue":
			return GENUS.DEX;
			case "green":
			return GENUS.WIS;
			case "yellow":
			return GENUS.CHA;
			case "alpha":
			return GENUS.ALL;
			case "all":
			Debug.Log("WRONG GENUS INFO");
			return GENUS.ALL;
			case "omega":
			return GENUS.OMG;
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
#endregion
	
#region Waves/Zones
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

	public Zone GetZone(string name)
	{
		for(int i = 0; i < Zones.Length; i++)
		{
			if(Zones[i]._Name == name) return Zones[i];
		}
		return null;
	}

	public Zone GetZoneRandom()
	{
		return Zones[UnityEngine.Random.Range(0, Zones.Length)];
	}

	public ZoneMapContainer GenerateEndlessMode()
	{
		return GenerateZoneMap(new Vector2[]{new Vector2(2,2), new Vector2(2,2)});
	}

	public ZoneMapContainer GenerateZoneMap(Vector2 [] zonenum)
	{
		ZoneMapContainer final = new ZoneMapContainer();
		ZoneBracket [] br = new ZoneBracket[zonenum.Length];
		for(int i = 0; i < zonenum.Length; i++)
		{
			int targ = (int)UnityEngine.Random.Range(zonenum[i].x, zonenum[i].y);
			br[i] = new ZoneBracket(targ);
			List<Zone> choices = GetRandomZones(i);
			if(i > 0)
			{
				for(int x = 0; x < choices.Count; x++)
				{
					if(br[i-1].Choices.Contains(choices[x])) choices.RemoveAt(x);
				}
			}
			
			for(int z = 0; z < targ; z++)
			{
				int rand = UnityEngine.Random.Range(0,choices.Count);
				br[i].Choices[z] = choices[rand];
				if(choices.Count > 1) choices.RemoveAt(rand);
			}
		}
		final.Brackets = br;
		return final;
	}

	public ZoneMapContainer GenerateZoneMap(ZoneMapBracketData [] brackets)
	{
		ZoneMapContainer final = new ZoneMapContainer();
		ZoneBracket [] br = new ZoneBracket[brackets.Length];
		for(int i = 0; i < brackets.Length; i++)
		{
			br[i] = new ZoneBracket(brackets[i].Zone.Length);
			for(int x = 0; x < brackets[i].Zone.Length; x++)
			{
				br[i].Choices[x] = GetZone(brackets[i].Zone[x].Name);
			}
		}
		final.Brackets = br;
		return final;
	}

	public List<Zone> GetRandomZones(int depth)
	{
		List<Zone> final = new List<Zone>();

		for(int i = 0; i < Zones.Length; i++)
		{
			if(!Zones[i].UseInGeneration) continue;
			if(Zones[i].GenerationDepths.x > depth || Zones[i].GenerationDepths.y < depth) continue;
			final.Add(Zones[i]);
		}
		return final;
	}
#endregion

#region Load/Save
	public void LoadAssets()
	{
		loading_assets = true;
		StartCoroutine(LoadAssets_Routine());
	}

	//Save State

	public void Save()
	{
		//bool finished_save = false;
		XmlDocument file = new XmlDocument();
		
		XmlElement data = (XmlElement) file.AppendChild(file.CreateElement("PlayerData"));
		//data.InnerText = "" + AppVersion;
		XmlElement build = (XmlElement) data.AppendChild(file.CreateElement("Build"));
		build.InnerText = "" + AppVersion;

		XmlElement lvl = (XmlElement) data.AppendChild(file.CreateElement("PlayerLevel"));
		lvl.InnerText = "" + Player.Level.Level;
		XmlElement xp = (XmlElement) data.AppendChild(file.CreateElement("PlayerXP"));
		xp.InnerText = "" + Player.Level.XP_Current;
		
		XmlSerializer SerializerObj = new XmlSerializer(typeof(PlayerData));
			
		// Create a new file stream to write the serialized object to a file

		string filepos = Application.persistentDataPath + "/PlayerData.xml";
		/*if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			filepos = GetIOSPath();
		}*/	

		FileStream WriteFileStream = File.Create(filepos);

		SerializerObj.Serialize(WriteFileStream, file);
		 
		// Cleanup
		WriteFileStream.Close();

		//finished_save = true;
		Debug.Log("Generated data at " + filepos);	
		//UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 5, "SAVED", 150, Color.white, 2.4F);
		//UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 3, "L" + Player.Level.Level, 150, Color.white, 2.4F);	
		//UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down,  filepos, 35, Color.white, 2.4F);
	}

	public void Load()
	{
		#if !UNITY_WEBPLAYER
		
		XmlDocument xmldoc = new XmlDocument (); 
		string datapath = Application.persistentDataPath + "/PlayerData.xml";
		
		if(System.IO.File.Exists(datapath))
		{
			xmldoc.LoadXml (System.IO.File.ReadAllText(datapath));
			XmlNode root = xmldoc.ChildNodes[1];

			XmlNode build = root.ChildNodes[0];
			int level = StringToInt(root.ChildNodes[1].InnerText);
			int xp = StringToInt(root.ChildNodes[2].InnerText);
			Player.instance.SetLevelInfo(level, xp);

			//if(StringToInt(build.InnerText) != AppVersion)
			//{
			//	Save();
			//	Load();
			//	return;
			//}

			Debug.Log("Loaded data at " + datapath);
			
		}
		else 
		{
			Save();
		}
		#elif UNITY_WEBPLAYER
		LoadAbilities();
		#endif
	}
#endregion

#region Loading Data
	public IEnumerator LoadInitialData()
	{
		Load();
		LoadUnlocks();
		
		LoadPowerups();
		
		StartCoroutine(AudioManager.instance.LoadAudioInit());
		yield return null;
		yield return StartCoroutine(UIManager.instance.UnloadGameUI());
		yield return null;

		yield return StartCoroutine(UIManager.Menu.LoadMenu());
	}

	public void LoadUnlocks()
	{
		//MODES
		ModeUnlocked_Quick = Player.instance.GetUnlock("quickmode");
		ModeUnlocked_Endless = Player.instance.GetUnlock("endlessmode");
		ModeUnlocked_Deep = Player.instance.GetUnlock("deepmode");

		FullVersion = ZPlayerPrefs.GetInt("FullVersion") == 1;
		LoadClasses();
	}

	public void LoadModes()
	{
		ModeUnlocked_Quick = Player.instance.GetUnlock("quickmode");
		ModeUnlocked_Endless = Player.instance.GetUnlock("endlessmode");
		ModeUnlocked_Deep = Player.instance.GetUnlock("deepmode");
	}
	
	public void LoadAbilities()
	{
		return;
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
	
	/*public void LoadClass(int num, string name, bool un, int lvl)
	{
		//if(Classes[num].Name != name) 
		//{
		//	Debug.LogError("LOADED INTO WRONG CLASS");
		//	return;
		//}
		//Classes[num].Unlocked = un;
		//Classes[num].Level = lvl;
	}*/
	
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
	string [] classes_list = new string []
	{
		"barbarian",
		"bard",
		"rogue",
		"wizard",
		"farmer",
		"warden",
		"witchdoctor",
		"gyromancer"
	};

	public void LoadClasses()
	{
		int num = 0;
		string path_init = "classes";
		List<Class> final = new List<Class>();
		AudioManager.instance.Classes = new AudioGroup[classes_list.Length];
		AudioManager.instance.Class_Default = AudioManager.GenerateGroup(path_init, "default");
		for(int i = 0; i < classes_list.Length; i++)
		{
			string path = path_init + "/" + classes_list[i];

			string prefpath = path + "/" + classes_list[i] + "_prefab";
			UnityEngine.Object cobj = Resources.Load(prefpath);
			if(cobj == null) continue;

			Class cfin = (cobj as GameObject).GetComponent<Class>();
			bool thisclassunlocked = Player.instance.GetUnlock(classes_list[i]);
			cfin.Unlocked = thisclassunlocked;

			final.Add(cfin);
		
			AudioGroup audiogroup = AudioManager.GenerateGroup(path_init, classes_list[i]);
			AudioManager.instance.Classes[i] = audiogroup;

			num++;
		}
		print("Loaded " + num + " classes");
		final = final.OrderBy(o=>!o.Unlocked).ToList();
		Classes = final.ToArray();
	}

	string [] powerups_list = new string []
	{
		"heal",
		"firestorm",
		"throwknives",
		"lullaby",
		"colorswap",
		"calldown",
		"createbombs"
	};

	public void LoadPowerups()
	{
		int num = 0;
		string path_init = "powerups";
		List<Powerup> final = new List<Powerup>();

		for(int i = 0; i < powerups_list.Length; i++)
		{
			string path = path_init + "/" + powerups_list[i];
			string prefpath = path + "/" + powerups_list[i] + "_prefab";
			UnityEngine.Object pobj = Resources.Load(prefpath);
			if(pobj == null) continue;
			Powerup pfin = (pobj as GameObject).GetComponent<Powerup>();
			final.Add(pfin);
			num ++;
		}
		print("Loaded " + num + " powerups");
		Powerups = final.ToArray();
	}

	public IEnumerator LoadAssets_Routine()
	{
		_Items = ItemNames;		

		TileModel = (GameObject) Resources.Load("TileModel");

		UnityEngine.Object[] textures = Resources.LoadAll("Icons");
		_Icons = new ItemInfo[textures.Length];

		_Abilities = new Ability[AbilityParent.transform.childCount];

		AbilityParent.SetActive(false);

		_Status = new Status[TileEffectParent.transform.childCount];
		for(int i = 0; i < TileEffectParent.transform.childCount; i++)
		{
			_Status[i] = TileEffectParent.transform.GetChild(i).GetComponent<Status>();
			//_Abilities[i].Index = i;
			yield return null;
		}

		TileEffectParent.SetActive(false);

		yield return null;

		//yield return StartCoroutine(LoadZones());
		//yield return StartCoroutine(LoadWaves());
		yield return StartCoroutine(AudioManager.instance.LoadAudio("Tiles"));
		yield return StartCoroutine(TileMaster.Types.LoadSprites("Tiles"));
		yield return StartCoroutine(TileMaster.Types.LoadPrefabs());

		print("FINISHED LOADING");
		loaded_assets = true;
		
		yield return null;
	}

	string [] zonenames = new string []
	{
		"library",
		"gateway",
		"ruins",
		"catacombs",
		"sewers",
		"outpost",
		"gromspit",
		"tunnels"
	};

	IEnumerator LoadZones()
	{
		print("LOADING ZONES");
		List<Zone> final = new List<Zone>();

		for(int i = 0; i < zonenames.Length; i++)
		{
			string path = "zones/" + zonenames[i];

			GameObject pref = Resources.Load(path + "/prefab") as GameObject;
			Zone pref_zone = pref.GetComponent<Zone>();
			final.Add(pref_zone);
		}
		Zones = final.ToArray();
		print("Found " + Zones.Length + " Zones");
		yield return null;
	}

	IEnumerator LoadWaves()
	{

		yield return null;
	}
#endregion

	public Powerup GetPowerup(string name, Class c = null)
	{
		Powerup obj = null;
		foreach(Powerup child in Powerups)
		{
			if(child.Name == name) obj = child;
		}

		if(obj == null) return null;
		Powerup final = (Powerup) Instantiate(obj);
		if(c != null)
		{
			final.transform.parent = c.transform;
			final.Setup(c);
		}
		return final;
	}

#region Shop

	public void SetFullVersion(bool active)
	{
		ZPlayerPrefs.SetInt("FullVersion", active ? 1 : 0);
		FullVersion = active;
		Debug.Log("Bought Full Version");
	}
	public void RollSkin()
	{
		Debug.Log("Bought Skin");
	}

	public void AddDeathCounter()
	{
		Debug.Log("Bought Death Counter");
	}

	public void RemoveDeathCounter()
	{

	}
#endregion
#region Other/Discard
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
			if(name == string.Empty) return Classes[UnityEngine.Random.Range(0, 5)];
			foreach(Class child in Classes)
			{
				if(name == child.Name || name == child.Info.ShortName)
				{
					return child;
				}
			}
			return null;
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

		public string [] TuteTips = new string [] 
		{
			""
		};

		public string StoryTip = "Four adventurers break into the forbidden undercity to explore and gather the precious 'Mana' that seeps from below...";

#endregion
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
	public string Level;
	public string XP;

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

public class ChoiceComplete
{
	public int ChoiceCount;
	public int ChoicePicked = 0;
	public bool Result;
	public ChoiceComplete(int num)
	{
		Result = false;
		ChoiceCount = num;
	}

	public void SetChoice(int num)
	{
		Result = true;
		ChoicePicked = num;
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

[System.Serializable]
public class ZoneMapContainer
{
	public ZoneBracket [] Brackets;
	public int Current = 0;
	public int Next{get{return Current +1;}}
	public ZoneBracket CurrentBracket
	{
		get{return Brackets[Current];}
	}
	public ZoneBracket NextBracket{get{
		int index = Mathf.Clamp(Current+1, 0, Brackets.Length-1);
		return Brackets[index];
		}}
	public ZoneBracket LastBracket{get{
		int index = Mathf.Clamp(Current-1, 0, Brackets.Length-1);
		return Brackets[index];
		}}

	public int Length{get{return Brackets.Length;}}
	public ZoneBracket this[int v]
	{
		get{return Brackets[v];}
	}

	public ZoneMapContainer(int b = 0)
	{
		Brackets = new ZoneBracket[b];
		Current = b;
	}

	public ZoneMapContainer(ZoneMapContainer z)
	{
		Brackets = z.Brackets;
		Current = z.Current;
	}

	public bool Progress()
	{
		Current ++;
		return Current < Length;
	}
}

[System.Serializable]
public class ZoneBracket
{
	public Zone [] Choices;
	public int Length {get{return Choices.Length;}}
	public Zone this[int v]
	{
		get{return Choices[v];}
		set{Choices[v] = value;}
	}

	public ZoneBracket(int z = 0)
	{
		Choices = new Zone[z];
	}
}

