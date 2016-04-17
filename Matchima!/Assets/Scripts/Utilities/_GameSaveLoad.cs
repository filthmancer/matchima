using UnityEngine; 
using System.Collections; 
using System.Collections.Generic; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 
 
public class _GameSaveLoad: MonoBehaviour { 

   // An example where the encoding can be found is at 
   // http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
   // We will just use the KISS method and cheat a little and use 
   // the examples from the web page since they are fully described 
 
   // This is our local private members 
 
   bool _ShouldSave, _ShouldLoad,_SwitchSave,_SwitchLoad; 
   string _FileLocation,_FileName; 
   UserData myData; 
   string _data; 
 
   Vector3 VPosition; 
 
   // When the EGO is instansiated the Start will trigger 
   // so we setup our initial values for our local members 
   void Start () { 
      // We setup our rectangles for our messages 
 
      // Where we want to save and load to and from 
      _FileLocation=Application.persistentDataPath; 
      _FileName="SaveData.xml"; 
 
      // we need soemthing to store the information into 
      myData=new UserData(); 
   } 
 
   void Update () {} 


 



   /* The following metods came from the referenced URL */ 
   string UTF8ByteArrayToString(byte[] characters) 
   {      
      UTF8Encoding encoding = new UTF8Encoding(); 
      string constructedString = encoding.GetString(characters); 
      return (constructedString); 
   } 
 
   byte[] StringToUTF8ByteArray(string pXmlString) 
   { 
      UTF8Encoding encoding = new UTF8Encoding(); 
      byte[] byteArray = encoding.GetBytes(pXmlString); 
      return byteArray; 
   } 
 
   // Here we serialize our UserData object of myData 
   string SerializeObject(object pObject) 
   { 
      string XmlizedString = null; 
      MemoryStream memoryStream = new MemoryStream(); 
      XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      xs.Serialize(xmlTextWriter, pObject); 
      memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
      XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
      return XmlizedString; 
   } 
 
   // Here we deserialize it back into its original form 
   object DeserializeObject(string pXmlizedString) 
   { 
      XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
      MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
      return xs.Deserialize(memoryStream); 
   } 
 
   // Finally our save and load methods for the file itself 
   void CreateXML() 
   { 
   	  #if !UNITY_WEBPLAYER
      StreamWriter writer; 
      FileInfo t = new FileInfo(_FileLocation+"/"+ _FileName); 
      if(!t.Exists) 
      { 
         writer = t.CreateText(); 
      } 
      else 
      { 
         t.Delete(); 
         writer = t.CreateText(); 
      } 
      writer.Write(_data); 
      writer.Close(); 
      Debug.Log("File written."); 
      #endif
   } 

   	public string GetIOSPath()
	{
		string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
		path = path.Substring(0,path.LastIndexOf('/'));
		return path + "/Documents";
	}
 
   void LoadXML() 
   { 
      StreamReader r = File.OpenText(_FileLocation+"/"+ _FileName); 
      string _info = r.ReadToEnd(); 
      r.Close(); 
      _data=_info; 
      Debug.Log("File Read"); 
   } 

   //*************************************************** 
   // Saving The Player... 
   // **************************************************    
   public void Save()
   {
		Debug.Log("Saving to: "+_FileLocation); 
		
		//myData._iUser.ClassName = Player.instance.ClassName;
		myData._iUser.Difficulty = GameManager.Difficulty;
		myData._iUser.Turns = Player.instance.Turns;
		myData._iUser.BestCombo = Player.instance.BestCombo;

		myData._iUser.ClassData = StatToStatData(Player.instance.InitStats);
		myData._iUser.ClassData._Health = Player.Stats._Health;
		myData._iUser.Tens = GameManager.instance.Tens;
		myData._iUser.Hunds = GameManager.instance.Hunds;
		myData._iUser.Thous = GameManager.instance.Thous;

		
		myData._iUser.ItemData = new ItemContainerData[5];
		for(int i = 0; i < 5; i++)
		{
			if(Player.instance.Equipment[i] == null) continue;
			myData._iUser.ItemData[i].Name = Player.instance.Equipment[i].name;
			myData._iUser.ItemData[i].slot = Player.instance.Equipment[i].slot;
			myData._iUser.ItemData[i].value = Player.instance.Equipment[i].value;
			myData._iUser.ItemData[i].price = Player.instance.Equipment[i].price;
			myData._iUser.ItemData[i].Equipped = Player.instance.Equipment[i].Equipped;
			
			myData._iUser.ItemData[i].Type = (int)Player.instance.Equipment[i].Type;
			List<UpgradeData> upgrades = new List<UpgradeData>();
			foreach(Upgrade child in Player.instance.Equipment[i].AllUpgrades)
			{
				UpgradeData d;
				d.index = child.index;
				d.subindex = child.subindex;
				d.chance = child.chance;
				d.value = child.value;
				d.price = child.price;
				d.Type = (int)child.type;
				upgrades.Add(d);
			}
			myData._iUser.ItemData[i].AllUpgrades = upgrades.ToArray();
			
			//myData._iUser.ItemData[i].stats = StatToStatData(Player.instance.Equipment[i].stats);
			//myData._iUser.ItemData[i].Effect = Player.instance.Equipment[i].Effect;
		}
		myData._iUser.AbilityData = new AbilityContainerData[6];
		for(int i = 0; i < 6; i++)
		{
			myData._iUser.AbilityData[i].hasAbility = (Player.Abilities[i] != null);
			if(!myData._iUser.AbilityData[i].hasAbility)continue;
			myData._iUser.AbilityData[i].Name = Player.Abilities[i].name;
			myData._iUser.AbilityData[i].Level = Player.Abilities[i].UpgradeLevel;
			myData._iUser.AbilityData[i].AbilityScript = Player.Abilities[i].GetType().ToString();
			myData._iUser.AbilityData[i].Cooldown = Player.Abilities[i]._defaultCooldown;
			myData._iUser.AbilityData[i].Cost = Player.Abilities[i]._defaultCost;
			//myData._iUser.AbilityData[i].CostType = (int)Player.Abilities[i].CostType;
			myData._iUser.AbilityData[i].StatType = (int)Player.Abilities[i].GENUS;
			myData._iUser.AbilityData[i].StatMultiplier = (int)Player.Abilities[i].GENUSMultiplier;
			myData._iUser.AbilityData[i].Input = new string[Player.Abilities[i]._input.args.Length];

			for(int o = 0; o < myData._iUser.AbilityData[i].Input.Length; o++)
			{
				myData._iUser.AbilityData[i].Input[o] = Player.Abilities[i]._input.args[o];
			} 

			myData._iUser.AbilityData[i].Output = new string[Player.Abilities[i]._output.args.Length];
			for(int o = 0; o < myData._iUser.AbilityData[i].Output.Length; o++)
			{
				myData._iUser.AbilityData[i].Output[o] = Player.Abilities[i]._output.args[o];
			} 
		}

		myData._iUser.Rows = new RowData[TileMaster.Tiles.GetLength(0)];
		for(int xx = 0; xx < TileMaster.Tiles.GetLength(0); xx++)
		{
			myData._iUser.Rows[xx].GenusIndex = new int[TileMaster.Tiles.GetLength(1)];
			myData._iUser.Rows[xx].SpeciesIndex = new int[TileMaster.Tiles.GetLength(1)];
			for(int yy = 0; yy < TileMaster.Tiles.GetLength(1); yy++)
			{
				Tile t = TileMaster.Tiles[xx,yy];
				myData._iUser.Rows[xx].GenusIndex[yy] = (int)t.Genus;
				myData._iUser.Rows[xx].SpeciesIndex[yy] = t.Type.Index;
			}
		}

		/*if(GameManager.instance._Wave != null)
		{
			myData._iUser.Wave.hasWave = true;
			Wave w = GameManager.instance._Wave;
			myData._iUser.Wave.Index = w.Index;
			myData._iUser.Wave.Current = w.Current;
			myData._iUser.Wave.Total = w.Required;
			myData._iUser.Wave.Effects = w.Effects.FXActive;
			myData._iUser.Wave.EffectsAmount = w.Effects.FXAmount;
		}
		else myData._iUser.Wave.hasWave = false;*/
		

		
		//myData._iUser.Grid = TileMaster.Grid;
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		Debug.Log("saved"); 
   }

  	StatData StatToStatData(Stat s)
   {
   		StatData _s;
		
		_s.Level              = s.Level;
		_s.Class_Type         = (int)s.Class_Type;
		_s.Shift         = (int)s.Shift;
		_s._Health             = s._Health;
		_s._HealthMax          = s._HealthMax;
		_s._Armour             = s._Armour;
		
		
		_s.MapSizeX           = s.MapSize.x;
		_s.MapSizeY           = s.MapSize.y;
		_s.TurnsPerDifficulty = s.TurnsPerDifficulty;
		_s.TurnsToWave        = s.TurnsToWave;
		_s.ComboCounter       = s.ComboCounter;
		_s.ComboBonus         = s.ComboBonus;
		_s._Attack             = s._Attack;
		_s.AttackRate         = s.AttackRate;
		_s.Regen              = s.Regen;
		_s.Leech              = s.Leech;
		_s.Spikes             = s.Spikes;
		_s.Poison             = s.Poison;
		_s.PoisonTime         = s.PoisonTime;
		_s.CooldownDecrease   = s.CooldownDecrease;
		_s.CostDecrease       = s.CostDecrease;
		_s.Presence           = s.Presence;		
		_s.isKilled           = s.isKilled;
		
		_s.PrevTurnKills      = s.PrevTurnKills;
		_s.HealThisTurn       = s.HealThisTurn;
		_s.DmgThisTurn        = s.DmgThisTurn;
		
		_s.OverflowMulti      = s.OverflowMulti;
		_s.AllColourMulti     = s.AllColourMulti;
	
   		_s.ContainerData = new StatContainerData[4];
		for(int i = 0; i < 4; i++)
		{
			_s.ContainerData[i].StatCurrent = s[i].StatCurrent;
			_s.ContainerData[i].StatGain = s[i].StatGain;
			_s.ContainerData[i].StatLeech = s[i].StatLeech;
			_s.ContainerData[i].StatRegen = s[i].StatRegen;
			//_s.ContainerData[i].ResCurrent = s[i].ResCurrent;
			//_s.ContainerData[i].ResMax = s[i].ResMax;
			_s.ContainerData[i].ResMultiplier = s[i].ResMultiplier;
			//_s.ContainerData[i].ResGain = s[i].ResGain;
			_s.ContainerData[i].ResLeech = s[i].ResLeech;
			_s.ContainerData[i].ResRegen = s[i].ResRegen;
			_s.ContainerData[i].ThisTurn = s[i].ThisTurn;
			//_s.ContainerData[i].ResMax_soft = s[i].ResMax_soft;
		}
		return _s;
   }

   Stat DataToStat(StatData s)
   {
   		Stat _s = new Stat();
		
		_s.Level = s.Level;
		_s.Class_Type = (GENUS)s.Class_Type;
		_s._Health = s._Health;
		_s._HealthMax = s._HealthMax;
		_s._Armour = s._Armour;
	
		_s.Shift = (ShiftType) s.Shift;
		_s.MapSize = new Vector2(s.MapSizeX, s.MapSizeY);
		_s.TurnsPerDifficulty = s.TurnsPerDifficulty;
		_s.TurnsToWave = s.TurnsToWave;
		_s.ComboCounter = s.ComboCounter;
		_s.ComboBonus = s.ComboBonus;
		_s._Attack = s._Attack;
		_s.AttackRate = s.AttackRate;
		_s.Regen = s.Regen;
		_s.Leech = s.Leech;
		_s.Spikes = s.Spikes;
		_s.Poison = s.Poison;
		_s.PoisonTime = s.PoisonTime;
		_s.CooldownDecrease = s.CooldownDecrease;
		_s.CostDecrease = s.CostDecrease;
		_s.Presence = s.Presence;
		_s.OverflowMulti = s.OverflowMulti;
		_s.AllColourMulti = s.AllColourMulti;
	
		_s.isKilled = s.isKilled;
		
		_s.PrevTurnKills = s.PrevTurnKills;
		_s.HealThisTurn = s.HealThisTurn;
		_s.DmgThisTurn = s.DmgThisTurn;

		for(int i = 0; i < 4; i++)
		{
			_s[i].StatCurrent   = s.ContainerData[i].StatCurrent;
			_s[i].StatGain      = s.ContainerData[i].StatGain;
			_s[i].StatLeech     = s.ContainerData[i].StatLeech;
			_s[i].StatRegen     = s.ContainerData[i].StatRegen;
			//_s[i].ResCurrent    = s.ContainerData[i].ResCurrent;
			//_s[i].ResMax        = s.ContainerData[i].ResMax;
			_s[i].ResMultiplier = s.ContainerData[i].ResMultiplier;
			//_s[i].ResGain       = s.ContainerData[i].ResGain;
			_s[i].ResLeech      = s.ContainerData[i].ResLeech;
			_s[i].ResRegen      = s.ContainerData[i].ResRegen;
			_s[i].ThisTurn      = s.ContainerData[i].ThisTurn;
			//_s[i].ResMax_soft   = s.ContainerData[i].ResMax_soft;
		}
		return _s;
   }

   Item DataToItem (ItemContainerData d)
   {
   		if(!d.Equipped) return null;
   		GameObject new_item = (GameObject) Instantiate(GameData.instance.Item, Vector3.zero, Quaternion.identity);
   		Item ii = new_item.GetComponent<Item>();
		ii.name = d.Name;
		ii.slot = d.slot;
		ii.value = d.value;
		ii.price = d.price;
		ii.Equipped = d.Equipped;
		
		ii.Type = (ItemType)d.Type;
		ii.AllUpgrades = new List<Upgrade>();
		for(int i = 0; i < d.AllUpgrades.Length; i++)
		{
			/*Upgrade u = new Upgrade(d.AllUpgrades[i].index,
									d.AllUpgrades[i].chance,
									d.AllUpgrades[i].price,
									d.AllUpgrades[i].value,
									(ItemType) d.AllUpgrades[i].Type);
			u.subindex = d.AllUpgrades[i].subindex;
			ii.AllUpgrades.Add(u);*/
		}
 		
 		//ii.stats = DataToStat(d.stats);
 		//myData._iUser.ItemData[i].Effect = Player.instance.Equipment[i].Effect;

		ii.SetStats();
		return ii;
   }

   AbilityContainer DataToAbility(AbilityContainerData con)
   {
   		AbilityContainer ab = new AbilityContainer();
   		ab.Name = con.Name;
   		ab.Level = con.Level;
   		ab.AbilityScript = con.AbilityScript;
   		ab.StatType = GameData.GENUSToString((GENUS)con.StatType);
   		ab.StatMultiplier = con.StatMultiplier;
   		ab.CooldownMin = con.Cooldown;
   		ab.CooldownMax = con.Cooldown;
   		//ab.CostMin = con.Cost[con.CostType];
   		//ab.CostMax = con.Cost[con.CostType];
   		ab.CostChance = 1.0F;
   		//ab.CostType = GameData.GENUSToString((GENUS)con.CostType);
   		ab.Input = new ContainerData[1];
   		ab.Input[0] = new ContainerData(con.Input);
   		ab.Output = new ContainerData[1];
   		ab.Output[0] = new ContainerData(con.Output);
   		return ab;
   }

   //*************************************************** 
   // Loading The Player... 
   // **************************************************     
   public string Load()
   {
   	  // Load our UserData into myData 
      LoadXML(); 
      if(_data.ToString() != "") 
      { 
        // notice how I use a reference to type (UserData) here, you need this 
        // so that the returned object is converted into the correct type 
        myData = (UserData)DeserializeObject(_data); 
       	GameManager.Difficulty         = myData._iUser.Difficulty;
		Player.instance.Turns              = myData._iUser.Turns;
		Player.instance.BestCombo          = myData._iUser.BestCombo;

		GameManager.instance.Tens = myData._iUser.Tens;
		GameManager.instance.Hunds = myData._iUser.Hunds;
		GameManager.instance.Thous = myData._iUser.Thous;

		// set the players position to the data we loaded 
		Stat newStats = DataToStat(myData._iUser.ClassData);
		Player.instance.InitStats = new Stat(newStats, false);

        //newAbilities = myData._iUser.abilities;
        Player.instance.Equipment.Helm = DataToItem(myData._iUser.ItemData[0]);
        if(Player.instance.Equipment.Helm != null)
        	Player.instance.Equipment.Helm.transform.parent = Player.instance.transform;

        Player.instance.Equipment.Chest = DataToItem(myData._iUser.ItemData[1]);
        if(Player.instance.Equipment.Chest != null)
        	Player.instance.Equipment.Chest.transform.parent = Player.instance.transform;

        Player.instance.Equipment.Weapon = DataToItem(myData._iUser.ItemData[2]);
        if(Player.instance.Equipment.Weapon != null)
        	Player.instance.Equipment.Weapon.transform.parent = Player.instance.transform;

        Player.instance.Equipment.Shield = DataToItem(myData._iUser.ItemData[3]);
        if(Player.instance.Equipment.Shield != null)
        	Player.instance.Equipment.Shield.transform.parent = Player.instance.transform;

        Player.instance.Equipment.Boots = DataToItem(myData._iUser.ItemData[4]);
        if(Player.instance.Equipment.Boots != null)
        	Player.instance.Equipment.Boots.transform.parent = Player.instance.transform;

        for(int i = 0; i < 6; i++)
        {
        	if(!myData._iUser.AbilityData[i].hasAbility) continue;
        	AbilityContainer ab = DataToAbility(myData._iUser.AbilityData[i]);
        	Ability a = (Ability)Instantiate(GameData.instance.GetAbilityByName(ab.AbilityScript));
        	a.Setup(ab,0,0);
        	Player.instance.AddAbility(a, i);
        }

        GridInfo level = new GridInfo();
        Vector2 size = new Vector2(myData._iUser.Rows.Length, myData._iUser.Rows[0].GenusIndex.Length);
        level.SetUp(size);
        for(int xx = 0; xx < myData._iUser.Rows.Length; xx++)
        {
        	for(int yy = 0; yy < myData._iUser.Rows[0].GenusIndex.Length; yy++)
        	{
        		TileInfo sp = new TileInfo( TileMaster.Types[myData._iUser.Rows[xx].SpeciesIndex[yy]], 
        									(GENUS) myData._iUser.Rows[xx].GenusIndex[yy]);
        		level.SetPointInfo(xx,yy, sp);
        	}
        }

        TileMaster.instance.LevelToLoad(level);
        /*if(myData._iUser.Wave.hasWave)
        {
			GameManager.instance._Wave = Instantiate(GameData._Waves[myData._iUser.Wave.Index]);
			GameManager.instance._Wave.Current = myData._iUser.Wave.Current;
			GameManager.instance._Wave.Required = myData._iUser.Wave.Total;
			GameManager.instance._Wave.GetEffects(myData._iUser.Wave.Effects, myData._iUser.Wave.EffectsAmount);
			GameManager.instance._Wave.transform.parent = GameManager.instance.transform;
			GameManager.instance.WaveActive = true;
        }*/

        //Spawner2.GetSpawnables(TileMaster.Types, GameManager.instance._Wave);
        Player.instance.ResetStats();
        Player.instance.ResetChances();
        Player.Stats._Health = myData._iUser.ClassData._Health;

		//Player.Stats.Red.ResCurrent = Player.instance.InitStats.Red.ResCurrent;
		//Player.Stats.Blue.ResCurrent = Player.instance.InitStats.Blue.ResCurrent;
		//Player.Stats.Green.ResCurrent = Player.instance.InitStats.Green.ResCurrent;
		//Player.Stats.Yellow.ResCurrent = Player.instance.InitStats.Yellow.ResCurrent;

        Debug.Log("loaded player"); 
        return  "";//myData._iUser.ClassName;
      }
      return "";
   }

} 
 
// UserData is our custom class that holds our defined objects we want to store in XML format 
 public class UserData 
 { 
    // We have to define a default instance of the structure 
   public Data _iUser; 
    // Default constructor doesn't really do anything at the moment 
   public UserData() { } 
 
   // Anything we want to store in the XML file, we define it here 
   public struct Data 
   { 
		//public string ClassName;
		public float Difficulty;
		public int Turns;
		public int BestCombo;
		public StatData ClassData;
		public ItemContainerData[] ItemData;
		public int Tens, Hunds, Thous;
		public AbilityContainerData [] AbilityData;
		
		public RowData [] Rows;
		public WaveData Wave;
   } 
}

public struct RowData
{
	public int [] GenusIndex;
	public int [] SpeciesIndex;
}
public struct WaveData
{
	public bool hasWave;
	public int Index;
	public int Current, Total;
	public bool [] Effects;
	public int [] EffectsAmount;
}

   public struct StatData
   {
     	public int Level;
		public int Class_Type;
		public int _Health, _HealthMax;
		public int _Armour;
			
		public float MapSizeX, MapSizeY;
		public int TurnsPerDifficulty;
		public int TurnsToWave;
		public int ComboCounter;
		public float ComboBonus;
		public int _Attack;
		public float AttackRate;
		public int Regen;
		public int Leech;
		public int Spikes;
		public int Poison;
		public int PoisonTime;
		public float CooldownDecrease;
		public float CostDecrease;

		public int Presence;
		
		public bool isKilled;
		
		public int PrevTurnKills;
		public int HealThisTurn, DmgThisTurn;

		public int Shift;
		public StatContainerData [] ContainerData;
		public float OverflowMulti;//0.4F;
		public float AllColourMulti;//1.2F;
   }

    public struct StatContainerData
   {
   		public int StatCurrent;
		public int StatGain;
	
		public int StatLeech;
		public int StatRegen;
	
		//public int ResCurrent;
		//public int ResMax;
		public float ResMultiplier;
	
		//public float ResGain;
		
		public int ResLeech;
		public int ResRegen;
	
		public int ThisTurn;
	
		//public float ResMax_soft;
   }

   public struct ItemContainerData
   {
   		public string Name;

		public int slot;
		public int value;
		public int price;
		public bool Equipped;
		public StatData stats;
		public AbilityContainerData Effect;
		public int Type;
		public UpgradeData [] AllUpgrades;
   }

   public struct UpgradeData
   {
   		public int index;
		public int subindex;
		public float chance;
		public float value;
		public int price;
		public int Type;
   }

   public struct AbilityContainerData
   {
   		public bool hasAbility;
   		public string Name;
		public string ShortName;
		public string Description;
		public int Level;
	
		public string AbilityScript;
	
		public int Cooldown;
		public int [] Cost;
		//public int CostType;
	
		public int StatType;
		public int StatMultiplier;
		public string [] Input;
		public string [] Output;
   }

