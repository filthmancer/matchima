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
      Debug.Log("File written to " + _FileLocation+"/"+ _FileName); 
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
		myData._iUser.Health = Player.Stats._Health;


		myData._iUser.ClassData = new ClassData[4];
		for(int i = 0; i < 4; i++)
		{
			myData._iUser.ClassData[i].Name = Player.Classes[i].Name;
			myData._iUser.ClassData[i].Init = StatToData(Player.Classes[i].InitStats);
			if(Player.Classes[i]._Slots.Length > 0)
			{
				myData._iUser.ClassData[i].Mods = new ModContainerData[Player.Classes[i]._Slots.Length];
				for(int m = 0; m < Player.Classes[i]._Slots.Length; m++)
				{
					if(Player.Classes[i]._Slots[m] == null) continue;
					myData._iUser.ClassData[i].Item[m] = ItemToData(Player.Classes[i]._Slots[m] as Item);
				}
			}

			if(Player.Classes[i].AllMods.Count > 0)
			{
				myData._iUser.ClassData[i].Mods = new ModContainerData[Player.Classes[i].AllMods.Count];
				for(int m = 0; m < Player.Classes[i].AllMods.Count; m++)
				{
					myData._iUser.ClassData[i].Mods[m] = ModToData(Player.Classes[i].AllMods[m]);
				}
			}
		}

		myData._iUser.Rows = new RowData[TileMaster.Tiles.GetLength(0)];
		for(int xx = 0; xx < TileMaster.Tiles.GetLength(0); xx++)
		{
			myData._iUser.Rows[xx].GenusIndex = new int[TileMaster.Tiles.GetLength(1)];
			myData._iUser.Rows[xx].SpeciesIndex = new int[TileMaster.Tiles.GetLength(1)];
			myData._iUser.Rows[xx].ValueIndex = new int[TileMaster.Tiles.GetLength(1)];
			myData._iUser.Rows[xx].ScaleIndex = new int[TileMaster.Tiles.GetLength(1)];
			for(int yy = 0; yy < TileMaster.Tiles.GetLength(1); yy++)
			{
				Tile t = TileMaster.Tiles[xx,yy];
				myData._iUser.Rows[xx].GenusIndex[yy] = (int)t.Genus;
				myData._iUser.Rows[xx].SpeciesIndex[yy] = t.Type.Index;
				myData._iUser.Rows[xx].ValueIndex[yy] = t.Stats.Value;
				myData._iUser.Rows[xx].ScaleIndex[yy] = t.Point.Scale;
			}
		}

		if(GameManager.instance._Wave != null)
		{
      myData._iUser.Wave.HasWave = true;
      Wave w = GameManager.instance._Wave;
			myData._iUser.Wave.Active = w.Active;
      myData._iUser.Wave.Index = w.Index;

      myData._iUser.Wave.Current = new int [w.AllSlots.Length];
      myData._iUser.Wave.Timer = new int [w.AllSlots.Length];
      myData._iUser.Wave.Slot = new bool [w.AllSlots.Length];
			for(int i = 0; i < w.AllSlots.Length; i++)
      {
        if(w.AllSlots[i] == null) 
        {
          continue;
        }
        myData._iUser.Wave.Slot[i] = true;
        myData._iUser.Wave.Current[i] = w.AllSlots[i].Current;
        myData._iUser.Wave.Timer[i] = w.AllSlots[i].Timer;
      }
		}
		

		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		Debug.Log("saved"); 
   }


   Item DataToItem (ItemContainerData d)
   {
   		GameObject new_item = (GameObject) Instantiate(GameData.instance.Item, Vector3.zero, Quaternion.identity);
   		Item ii = new_item.GetComponent<Item>();
		
		  ii.Name_Basic = d.SlotData.Name;
		  ii.IconString = d.SlotData.IconString;
		  ii.Index = d.SlotData.Index;
		  ii.cooldown = d.SlotData.Cooldown;
  
		  ii.Type = (ItemType)d.Type;
		  ii.Type = (ItemType) d.Type;
		  ii.ScaleGenus = (GENUS)d.ScaleGenus;
		  ii.ScaleRate = d.ScaleRate;
  
		  ii.SetStats();
		  return ii;
   }

   ItemContainerData ItemToData(Item s)
   {
   		ItemContainerData i;
   		i.SlotData = SlotToData(s as Slot);
   		i.Type = (int)s.Type;
   		i.ScaleGenus = (int)s.ScaleGenus;
   		i.ScaleRate = s.ScaleRate;

   		return i;
   }

   ModContainerData ModToData(Slot s)
   {
   		ModContainerData m;
   		m.SlotData = SlotToData(s);

   		return m;
   }

   Slot DataToMod(ModContainerData m)
   {
   		Slot s = null;
   		return s;
   }

   SlotContainerData SlotToData(Slot s)
   {
   		SlotContainerData d;

   		d.Name = s.Name_Basic;
   		d.IconString = s.IconString;
   		d.Index = s.Index;
   		d.Cooldown = s.cooldown;
   		return d;
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
   public Class [] Load()
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
  
		  Class [] Classes = new Class[myData._iUser.ClassData.Length];
  
		  for(int i = 0; i < myData._iUser.ClassData.Length; i++)
		  {
		  	ClassData d = myData._iUser.ClassData[i];
		  	Classes[i] = (Class) Instantiate(GameData.instance.GetClass(d.Name));
		  	Classes[i].InitStats = DataToStat(d.Init);
  
		  	if(d.Item != null && d.Item.Length > 0)
		  	{
		  		Classes[i]._Slots = new Slot[d.Item.Length];
		  		for(int m = 0; m < d.Item.Length; m++)
		  		{
		  			Classes[i]._Slots[m] = DataToItem(d.Item[m]);
		  		}
		  	}
  
  
		  	if(d.Mods != null && d.Mods.Length > 0)
		  	{
		  		Classes[i].AllMods = new List<Slot>();
		  		for(int m = 0; m < d.Mods.Length; m++)
		  		{
		  			Classes[i].AllMods.Add(DataToMod(d.Mods[m]));
		  		}
		  	}
		  	
		  }
		

      GridInfo level = new GridInfo();
      Vector2 size = new Vector2(myData._iUser.Rows.Length, myData._iUser.Rows[0].GenusIndex.Length);
      level.SetUp(size);
      for(int xx = 0; xx < myData._iUser.Rows.Length; xx++)
      {
      	for(int yy = 0; yy < myData._iUser.Rows[0].GenusIndex.Length; yy++)
      	{
      		TileInfo sp = new TileInfo(TileMaster.Types[myData._iUser.Rows[xx].SpeciesIndex[yy]], 
      									(GENUS) myData._iUser.Rows[xx].GenusIndex[yy]);
      		sp.FinalValue = new IntVector(myData._iUser.Rows[xx].ValueIndex[yy]);
      		sp.Scale = myData._iUser.Rows[xx].ScaleIndex[yy];
      		level.SetPointInfo(xx,yy, sp);
      	}
      }

      TileMaster.instance.LevelToLoad(level);
      if(myData._iUser.Wave.HasWave)
      {
        WaveGroup g = null;
        if((GameMode)myData._iUser.GameMode == GameMode.Story)
        {
          g = GameManager.instance.StoryMode;
        }
        else if((GameMode)myData._iUser.GameMode == GameMode.Endless)
        {
          g = GameManager.instance.EndlessMode;
        }
			   GameManager.instance._Wave = Instantiate(g[myData._iUser.Wave.Index]);
			   GameManager.instance._Wave.transform.parent = GameManager.instance.transform;
         for(int i = 0; i < GameManager.instance._Wave.Length; i++)
         {
          if(!myData._iUser.Wave.Slot[i]) continue;
          GameManager.instance._Wave[i].Current = myData._iUser.Wave.Current[i];
          GameManager.instance._Wave[i].Timer = myData._iUser.Wave.Timer[i];
         }
      }

      Spawner2.GetSpawnables(TileMaster.Types, GameManager.instance._Wave);
      //Player.instance.ResetStats();
      //Player.instance.ResetChances();
      Player.Stats._Health = myData._iUser.Health;

        Debug.Log("loaded player"); 
        return Classes;
      }
      return null;
   }

    StatData StatToData(Stat s)
    {
      StatData _s;
   		
		  _s.Level               = s.Level;
		  _s.Class_Type          = (int)s.Class_Type;
		  _s.Shift               = (int)s.Shift;
		  _s._Health             = s._Health;
		  _s._HealthMax          = s._HealthMax;
		  _s._MeterMax           = s.MeterMax;
		  _s.MeterDecay_Global   = s.MeterDecay_Global;
		  
		  _s._Armour             = s._Armour;
		  _s._ArmourMax          = s._ArmourMax;
		  _s.ArmourReductionRate = s.ArmourReductionRate;
		  
		  _s.MatchNumberMod      = s.MatchNumberModifier;
		  _s.MapSizeX            = s.MapSize.x;
		  _s.MapSizeY            = s.MapSize.y;
		  
		  _s.ComboCounter        = s.ComboCounter;
		  _s.ComboBonus          = s.ComboBonus;
		  
		  _s._Attack             = s._Attack;
		  _s.AttackRate          = s.AttackRate;
		  _s.MagicPower          = s.MagicPower;
		  
		  _s.HealthRegen         = s.HealthRegen;
		  _s.HealthLeech         = s.HealthLeech;
		  _s.MeterRegen          = s.MeterRegen;
		  _s.MeterLeech          = s.MeterLeech;
		  
		  _s.Spikes              = s.Spikes;
		  
		  _s.CooldownDecrease    = s.CooldownDecrease;
		  _s.CostDecrease        = s.CostDecrease;
		  _s.ValueInc            = s.ValueInc;
		  _s.Presence            = s.Presence;		
		  _s.isKilled            = s.isKilled;
		  
		  _s.PrevTurnKills       = s.PrevTurnKills;
		  _s.HealThisTurn        = s.HealThisTurn;
		  _s.DmgThisTurn         = s.DmgThisTurn;
   		
   	
      	_s.ContainerData = new StatContainerData[4];
   		for(int i = 0; i < 4; i++)
   		{
   			_s.ContainerData[i].StatCurrent = s[i].StatCurrent;
   			_s.ContainerData[i].StatGain = s[i].StatGain;
   			_s.ContainerData[i].StatLeech = s[i].StatLeech;
   			_s.ContainerData[i].StatRegen = s[i].StatRegen;

   			_s.ContainerData[i].ResMultiplier = s[i].ResMultiplier;
   			_s.ContainerData[i].ResLeech = s[i].ResLeech;
   			_s.ContainerData[i].ResRegen = s[i].ResRegen;
   			_s.ContainerData[i].ThisTurn = s[i].ThisTurn;
   		}
   		return _s;
    }

    Stat DataToStat(StatData s)
    {
    	Stat _s = new Stat();
   	   		
   		_s.Level               = s.Level;
   		_s.Class_Type          = (GENUS)s.Class_Type;
   		_s.Shift               = (ShiftType)s.Shift;
   		_s._Health             = s._Health;
   		_s._HealthMax          = s._HealthMax;
   		_s.MeterMax            = s._MeterMax;
   		_s.MeterDecay_Global   = s.MeterDecay_Global;
   		
   		_s._Armour             = s._Armour;
   		_s._ArmourMax          = s._ArmourMax;
   		_s.ArmourReductionRate = s.ArmourReductionRate;
   		
   		_s.MatchNumberModifier  = s.MatchNumberMod;
   		_s.MapSize.x 			= s.MapSizeX;
   		_s.MapSize.y 			= s.MapSizeY;
   		
   		_s.ComboCounter        = s.ComboCounter;
   		_s.ComboBonus          = s.ComboBonus;
   		
   		_s._Attack             = s._Attack;
   		_s.AttackRate          = s.AttackRate;
   		_s.MagicPower          = s.MagicPower;
   		
   		_s.HealthRegen         = s.HealthRegen;
   		_s.HealthLeech         = s.HealthLeech;
   		_s.MeterRegen          = s.MeterRegen;
   		_s.MeterLeech          = s.MeterLeech;
   		
   		_s.Spikes              = s.Spikes;
   		
   		_s.CooldownDecrease    = s.CooldownDecrease;
   		_s.CostDecrease        = s.CostDecrease;
   		_s.ValueInc            = s.ValueInc;
   		_s.Presence            = s.Presence;		
   		_s.isKilled            = s.isKilled;
   		
   		_s.PrevTurnKills       = s.PrevTurnKills;
   		_s.HealThisTurn        = s.HealThisTurn;
   		_s.DmgThisTurn         = s.DmgThisTurn;
   		
 
   		for(int i = 0; i < 4; i++)
   		{
   			_s[i].StatCurrent   = s.ContainerData[i].StatCurrent;
   			_s[i].StatGain      = s.ContainerData[i].StatGain;
   			_s[i].StatLeech     = s.ContainerData[i].StatLeech;
   			_s[i].StatRegen     = s.ContainerData[i].StatRegen;
   			_s[i].ResMultiplier = s.ContainerData[i].ResMultiplier;
   			_s[i].ResLeech      = s.ContainerData[i].ResLeech;
   			_s[i].ResRegen      = s.ContainerData[i].ResRegen;
   			_s[i].ThisTurn      = s.ContainerData[i].ThisTurn;
   		}
   		return _s;
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
    public int Health;
    public int GameMode;

		public ClassData [] ClassData;
		
		public RowData [] Rows;
		public WaveData Wave;
  } 
}

public struct RowData
{
	public int [] GenusIndex;
	public int [] SpeciesIndex;
	public int [] ValueIndex;
	public int [] ScaleIndex;
}
public struct WaveData
{
  public bool HasWave;
	public bool Active;
	public int [] Current;
  public int [] Timer;
  public int Index;
  public bool [] Slot;
}

public struct ClassData
{
	public string Name;
	public StatData Init;
	public ItemContainerData [] Item;
	public ModContainerData [] Mods;
}
   public struct StatData
   {
     	public int Level;
		public int Class_Type;
		public int _Health, _HealthMax;
		public int _MeterMax;
		public float MeterDecay_Global;

		public int _Armour;
		public int _ArmourMax;
		public float ArmourReductionRate;
			
		public float MapSizeX, MapSizeY;

		public int ComboCounter;
		public float ComboBonus;
		public int MatchNumberMod;

		public int _Attack;
		public float AttackRate;
		public int MagicPower;

		public int HealthRegen, HealthLeech;
		public int MeterRegen, MeterLeech;

		public int Spikes;

		public float CooldownDecrease;
		public float CostDecrease;
		public int ValueInc;

		public int Presence;
		
		public bool isKilled;
		
		public int PrevTurnKills;
		public int HealThisTurn, DmgThisTurn;

		public int Shift;
		public StatContainerData [] ContainerData;
		//public TileChanceData [] TileChances;
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
   		public SlotContainerData SlotData;
   		public int Type;
   		public int ScaleGenus;
   		public float ScaleRate;
   		//public UpgradeData [] Upgrades;
   }

   public struct SlotContainerData
   {
   	   	public string Name;
   	   	public string IconString;
   	   	public int Index;
   	   	public int Cooldown;
   }

   public struct ModContainerData
   {
	   public SlotContainerData SlotData;
   }

   public struct UpgradeData
   {
   		public int [] index;
   		public string Prefix, Suffix;
		public float chance;
		public int ItemType;
		public int ScaleType;
		public float scalerate;
		public int Points_total;
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

   public struct TileChanceData
   {
   		public string Genus, Type;
   		public float Chance;
   		public int Value;
   }

