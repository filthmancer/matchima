using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public static Player instance;
	public static Ops _Options
	{
		get{return Player.instance.Options;}
	}
	public static int RequiredMatchNumber = 3;
	public static bool loaded = false;
	public static Stat Stats;

	public bool CompleteMatch = true;
	public int Turns = 0;
	public int BestCombo = 0;
	public bool retired;
	public Ops Options;
	public Stat InitStats, StatTemp;

	public Class [] _Classes = new Class [4];
	public static Class [] Classes;

	public static Slot [] Slots
	{
		get{
			List<Slot> final = new List<Slot>();
			foreach(Class child in Classes)
			{
				if(child == null) continue;
				foreach(Slot sl in child._Slots)
				{
					final.Add(sl);
				}
			}
			return final.ToArray();
		}
	}
	public static Ability [] Abilities
	{
		get{
			List<Ability> final = new List<Ability>();
			foreach(Class child in Classes)
			{
				if(child == null) continue;
				foreach(Slot sl in child._Slots)
				{
					if(sl is Ability) final.Add(sl as Ability);
				}
			}
			return final.ToArray();
		}
	}

	public static Item [] Items
	{
		get
		{
			List<Item> final = new List<Item>();
			foreach(Class child in Classes)
			{
				if(child == null) continue;
				foreach(Slot sl in child._Slots)
				{
					if(sl is Item) final.Add(sl as Item);
				}
			}
			return final.ToArray();
		}
	}
	public int AbilityCount
	{
		get{
			int a = 0;
			for(int i = 0; i < Abilities.Length; i++)
			{
				if(Abilities[i] != null) a++;
			}
			return a;
		}
	}

	public int BoughtAbilityCount
	{
		get{
			int a = 0;
			for(int i = 0; i < Stats.AbilitySlots; i++)
			{
				//if(abilities[i] != null) a++;
			}
			return a;
		}
	}

	public EquipmentContainer Equipment;
	public List<Item> ThisTurn_items = new List<Item>();
	public List<UpgradeGroup> ThisTurn_upgrades = new List<UpgradeGroup>();
	public List<ClassEffect> _Status = new List<ClassEffect>();


	float idle_quote_chance = 0.02F;
	bool idle_quote = false;
	float idle_time = .0F;
	void Awake()
	{
		instance = this;
		Stats = new Stat(InitStats);
	}

	void Start () {
		TileMaster.instance.MapSize = Stats.MapSize;
	}
	
	void Update () {
		StatTemp = Stats;

		if(PlayerControl.instance.TimeWithoutInput > 20.0F)
		{
			idle_time -= Time.deltaTime;
			if(idle_time <= 0.0F)
			{
				idle_time = Random.Range(7.5F, 15.0F);
				int randa = Random.Range(0,4);
				bool info = Random.value > 0.5F;
				if(Classes[randa] != null) 
					StartCoroutine(UIManager.instance.Quote((info ? Classes[randa].Quotes.Info : Classes[randa].Quotes.Idle)));
			}
		}
		else if(PlayerControl.instance.TimeWithoutInput == 0.0F) idle_time = 0.0F;

	}

	public TileTypes GetTileTypes()
	{
		return null;//_class.Types;
	}


	public StatContainer GetResourceFromGENUS(GENUS ab)
	{
		switch(ab)
		{
			case GENUS.STR:
			return Classes[0].Stats.Red;
			case GENUS.DEX:
			return Classes[1].Stats.Blue;
			case GENUS.WIS:
			return Classes[2].Stats.Green;
			case GENUS.CHA:
			return Classes[3].Stats.Yellow;
		}
		return null;
	}

	public void OnTileDestroy(Tile t)
	{
		foreach(Ability child in Abilities)
		{
			if(child == null) continue;
			child.OnTileDestroy(t);
		}
	}

	public void OnTileCollect(Tile t)
	{
		foreach(Ability child in Abilities)
		{
			if(child == null) continue;
			child.OnTileCollect(t);
		}
	}

	public IEnumerator BeforeMatch(List<Tile> tiles)
	{
		//bool match = true;
		QueuedSpells.Clear();
		foreach(Ability child in Abilities)
		{
			if(child == null || !child.BeforeMatchEffect) continue;
			yield return StartCoroutine(child.BeforeMatch(tiles));
			//if(!child.BeforeMatch(tiles)) match = false;
		}
		
		for(int i = 0; i < tiles.Count; i++)
		{
			if(tiles[i] == null || !tiles[i].BeforeMatchEffect) continue;
			yield return StartCoroutine(tiles[i].BeforeMatch());
		}


		yield break;
	}

	public void CompleteHealth()
	{
		Stats.CompleteHealth();
		ResetStats();
	}

	public void OnMatch(Tile t)
	{
		if(t is Enemy)
		{
			Enemy e = t as Enemy;
		}
	}

	public int CompleteClasses()
	{
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			child.Complete();
		}
		return 0;
	}

	public IEnumerator AfterMatch()
	{
		float time = 0.0F;
		foreach(Ability child in Abilities)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.AfterMatch());
			//time += child.AfterMatch();
		}
		yield break;
		//yield return new WaitForSeconds(time);
	}


	public IEnumerator BeginTurn()
	{
		foreach(Ability child in Abilities)
		{
			if(child == null) continue;
			if(!child.initialized) child.initialized = true;
			child.BeforeTurn();
		}
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.BeginTurn());
			for(int i = 0; i < child._Status.Count; i++)
			{
				if(child._Status[i].CheckDuration()) 
				{
					Destroy(child._Status[i].gameObject);
					child._Status.RemoveAt(i);
				}
			}
		}

		for(int i = 0; i < _Status.Count; i++)
		{
			if(_Status[i].CheckDuration()) 
			{
				Destroy(_Status[i].gameObject);
				_Status.RemoveAt(i);
			}
		}

		//if(Turns == 0)
		//{
		//	int randa = Random.Range(0,4);
		//	StartCoroutine(UIManager.instance.Quote(Classes[randa].Quotes.Start));
		//	if(Random.value > 0.6F)
		//	{
		//		int randb = Random.Range(0,3);
		//		if(randb == randa) randb++;
		//		StartCoroutine(UIManager.instance.Quote(Classes[randb].Quotes.Start));
		//	}
		//}

		//Tutorial();


		if(ThisTurn_items.Count > 0)
		{
			UIManager.Objects.ShowObj(UIManager.Objects.BigUI,true);
			UIManager.Objects.ShowObj(UIManager.Objects.ClassUpgradeUI,false);
			UIManager.Objects.LevelUpMenu.SetActive(false);
			UIManager.instance.ItemUI.gameObject.SetActive(true);
			UIManager.instance.current_class = null;
			//UIManager.instance.current_item = child;
			UIManager.instance.ShowItemUI(ThisTurn_items.ToArray());

			while(UIManager.ItemUI_active) 
			{
				yield return null;
			}
		}

		if(ThisTurn_upgrades.Count > 0)
		{
			foreach(UpgradeGroup child in ThisTurn_upgrades)
			{
				if(child.Upgrades.Length > 1)
				{
					UIManager.instance.OpenBoonUI(child);
					while(UIManager.BoonUI_active)	
					{
						yield return null;
					}
				}
				else
				{
					child.Target.GetUpgrade(child.Upgrades[0]);
					yield return new WaitForSeconds(0.1F);
				}
				foreach(ClassUpgrade up in child.Upgrades)
				{
					up.Value = 1;
				}
			}
		}

		UIManager.Objects.ShowObj(UIManager.Objects.BigUI, false);
		UIManager.instance.ItemUI.gameObject.SetActive(false);

		UIManager.Objects.LevelUpMenu.SetActive(false);
		
		PlayerControl.instance.canMatch = true;

		ThisTurn_items.Clear();
		ThisTurn_upgrades.Clear();

		Stats.PrevTurnKills = 0;
		InitStats.PrevTurnKills = 0;
		CompleteMatch = true;



		yield return null;
	}

	public bool EndTurn()
	{
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			child.EndTurn();
		}
		ResetStats();
		Turns ++;
		return Turns % (int)Stats.AttackRate == 0;
	}

	public void CheckForBoons()
	{
		StartCoroutine(CheckForBoonsRoutine());
	}

	public IEnumerator CheckForBoonsRoutine()
	{
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.CheckForBoon());
		}
		yield return null;
	}

	public void PickupItem(Item i)
	{
		ThisTurn_items.Add(i);
	}

	public void PickupUpgrade(UpgradeGroup c)
	{
		ThisTurn_upgrades.Add(c);
	}

	public void CheckHealth()
	{
		for(int i = 0; i < Classes.Length; i++)
		{
			if(Classes[i] != null) Classes[i].CheckHealth();
		}

		if(Stats._Health < Stats._HealthMax / 5)
		{
			int randa = Random.Range(0,4);
			StartCoroutine(UIManager.instance.Quote(Classes[randa].Quotes.Danger));
		}
		if(Stats._Health <= 0) 
		{
			int randa = Random.Range(0,4);
			StartCoroutine(UIManager.instance.Quote(Classes[randa].Quotes.Death));
			Stats.isKilled = true;
		}

	}

	public List<Bonus> CheckForBonus(GENUS g)
	{
		List<Bonus> bonuses = new List<Bonus>();
		foreach(Ability child in Abilities)
		{
			if(child == null) continue;
			Bonus b = child.CheckBonus(g);
			if(b != null) bonuses.Add(b);
		}
		for(int i = 0; i < Equipment.Length; i++)
		{
			if(Equipment[i] == null) continue;
			Bonus b = Equipment[i].CheckBonus();
			if(b != null) bonuses.Add(b);
		}
		return bonuses;
	}

	public void AddAbility(Ability type, int? num = null)
	{
		bool upgrade = false;

		if(!upgrade)
		{
			GameObject ActiveObj = (type.gameObject);
			ActiveObj.transform.parent = this.transform;

			if(num.HasValue)
			{
				//abilities[(int)num]  = ActiveObj.GetComponent<Ability>();
				ActiveObj.name = "Ability " + (int) num + ": " + ActiveObj.GetComponent<Ability>().name;
			}
			else 
			{
				for(int i = 0; i < Stats.AbilitySlots; i++)
				{
					//if(abilities[i] == null) 
					//{
					//	abilities[i] = ActiveObj.GetComponent<Ability>();
					//	ActiveObj.name = "Ability " + i + ": " + ActiveObj.GetComponent<Ability>().name;
					//	UIManager.instance.AddAbility(ActiveObj.GetComponent<Ability>());
					//	break;
					//}
				}
			}
			
			//ActiveObj.GetComponent<Ability>().Init();
			if(type.passive) ActiveObj.GetComponent<Ability>().Activate();
			//foreach(Ability child in abilities)
			//{
			//	if(child == null) continue;
			//	child.AfterTurnB();
			//}
		}
		
	}
	public int RemoveAbility(Ability type)
	{
		//for(int i = 0; i < abilities.Length; i++)
		//{
		//	if(abilities[i] == null) continue;
		//	if(abilities[i] == type)
		//	{
		//		UIManager.instance.AbilityButtons[i].Remove();
		//		abilities[i] = null;
		//		return i;
		//	}
		//}
		return 100;
	}

	public bool FreeAbilitySlot()
	{
		//for(int i = 0; i < Stats.AbilitySlots; i++)
		//{ 
		//	if(abilities[i] == null) return true;
		//}
		return false;
	}

	public void AddStatus(int i, string StatusName, int Duration, params string [] args)
	{
		ClassEffect e = (ClassEffect) Instantiate(GameData.instance.GetTileEffectByName(StatusName));
		e.GetArgs(Duration, args);
		if(i == -1) 
		{
			_Status.Add(e);
			e.transform.parent = this.transform;
		}
		else 
		{
			Classes[i]._Status.Add(e);
			e.transform.parent = Classes[i].transform;
		}
	}

	public void AddClass(ClassContainer con)
	{
		//_container = con;
		//_class = (Class) Instantiate(con.Prefab);
		//_class.transform.parent = this.transform;
		//InitStats = new Stat(_class.Stats);
		//Stats = new Stat(_class.Stats);
		//Stats.ApplyStatInc();
		//
		//StartCoroutine(LoadClass(_class));
	}

	public void Reset()
	{
		loaded = false;
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			Destroy(child.gameObject);
		}
		_Classes = new Class[4];
		Classes = new Class[4];
		Stats.isKilled = false;
	}

	public void Load()
	{
		StartCoroutine(LoadClasses());
	}

	IEnumerator LoadClasses()
	{
		Classes = new Class[_Classes.Length];
		for(int i = 0; i < _Classes.Length; i++)
		{
			if(_Classes[i] == null) continue;

			Classes[i] = (Class) Instantiate(_Classes[i]);
			Classes[i].transform.parent = this.transform;
		}
		
		for(int i = 0; i < Classes.Length; i++)
		{
			if(Classes[i] !=null) 
			{
				Classes[i].Genus = (GENUS)i;
				Classes[i].Index = i;
				Classes[i].StartClass();
			}
		}
		ResetStats();

		Stats._Health = Stats._HealthMax;
		yield return new WaitForSeconds(0.1F);
		loaded = true;

		yield return null;
	}


	public void ResumeClass(ClassContainer con)
	{
		//_container = con;
		//_class = (Class) Instantiate(con.Prefab);
		//_class.transform.parent = this.transform;
		//ResetStats();
		//StartCoroutine(LoadClass(_class));
	}

	public void AddItem(Item i)
	{
		int slot = i.slot;

		if(Equipment[slot] != null) Destroy(Equipment[slot].gameObject);
		Equipment[slot] = i;
		i.Equipped = true;
		i.DeformatText();
		i.transform.parent = this.transform;

		ResetStats();
	}


	public void ResetStats()
	{
		float ratio = (float) Stats._Health / (float) Stats._HealthMax;
		int armour = Stats._Armour;
		Stats.SetStats(InitStats, false);
		
		int finalattack = 0;
		//int finalhealth = 0;
		for(int i = 0; i < Classes.Length; i++)
		{
			if(Classes[i] != null) 
			{
				Classes[i].Reset();
				if(!Classes[i].isKilled)
				{
					Stats.AddStats(Classes[i].Stats);
					finalattack += Classes[i].Stats._Attack;
					//finalhealth += Classes[i].Stats._Health;
				}
			}	
		}

		for(int i = 0; i < _Status.Count; i++)
		{
			_Status[i].StatusEffect();
		}

		Stats._Health = (int) Mathf.Clamp(Stats._HealthMax * ratio, 0, Stats._HealthMax);
		Stats._Armour = armour;
		Stats._Attack = finalattack;



		Stats.MapSize.x = Mathf.Clamp(Stats.MapSize.x, 3, 20);
		Stats.MapSize.y = Mathf.Clamp(Stats.MapSize.y, 3, 20);
		
		if(TileMaster.instance.MapSize != Stats.MapSize)
		{
			//TileMaster.instance.MapSize = Stats.MapSize;
			TileMaster.instance.IncreaseGridTo(Stats.MapSize);
		}

		RequiredMatchNumber = Mathf.Clamp(3 + Stats.MatchNumberModifier, 1, 10);
		ResetChances();
	}

	public void ResetChances()
	{
		TileMaster.instance.ResetChances();
		foreach(TileChance t in Stats.TileChances)
		{
			TileMaster.instance.IncreaseChance(t.Genus,t.Type, t.Chance);
			if(t.Value > 0)
			{
				SPECIES s = TileMaster.Types[t.Type];
				GenusInfo g = s[t.Genus];
				g.ValueAdded.y += (t.Value);
			}
		}

		for(int i = 0; i < GameManager.instance._Wave.Length; i++)
		{
			if(GameManager.instance._Wave[i] != null)
			{
				foreach(WaveTile w in GameManager.instance._Wave[i].Tiles)
				{
					TileMaster.instance.IncreaseChance(w.Genus, w.Species, w.Chance);
					if(w.Value.y > 0)
					{
						SPECIES s = TileMaster.Types[w.Species];
						GenusInfo g = s[w.Genus];
						g.ValueAdded.Add(w.Value);
					}
				}
			}
			
		}
		
		Spawner2.GetSpawnables(TileMaster.Types);
	}

	public List<int> ActiveDamage(int damage, List<Tile> selected)
	{
		List<int> indiv_damage = new List<int>();
		for(int i = 0; i < selected.Count; i++)
		{
			indiv_damage.Add(damage);
		}

			foreach(Ability child in Abilities)
			{
				if(child == null) continue;
				child.DamageIndicator(ref indiv_damage, selected);
			}

		return indiv_damage;
	}

	public void CheckForBestCombo(int[] combo)
	{
		int final = 0;
		for(int i = 0; i < combo.Length; i++)
		{
			final += combo[i];
		}
		if(final > BestCombo) 
		{
			BestCombo = final;
		}
	}


	public void LevelUp()
	{
		Stats.Level += 1;

		InitStats.LevelUp();

		ResetStats();
	}

	public string [] GetLevelUpInfo()
	{
		string [] info = new string [10];
		info[0] = ("LEVEL:  " + Stats.Level + "\n");
		return info;
	}

	public Ability GetClassAbility()
	{
		return null;
	}

	public static List<Vector2> QueuedSpells = new List<Vector2>();
	public static void QueueSpell(int x, int y)
	{
		QueuedSpells.Add(new Vector2(x,y));
	}
	public static bool QueuedSpell(int x, int y)
	{
		foreach(Vector2 child in QueuedSpells)
		{
			if(child.x == x && child.y == y) return true;
		}
		return false;
	}

	public void Tutorial()
	{
		QuoteGroup tute = null;
		switch(Turns)
		{
			case 0:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Swipe 3 tiles to Match!",  Classes[2], true, 1F);
			break;
			case 1:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Mana tiles fill mana pools.", Classes[1],true, 1F );
			tute.AddQuote("Health tiles fill the health bar.", Classes[1], true, 1F);
			break;
			case 3:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Enemies incoming!", Classes[0], true, 1F);
			tute.AddQuote("Enemy tiles deal damage to your health.",Classes[0], true, 1F);
			tute.AddQuote("Match enemies to attack them back!", Classes[0], true, 1f);
			break;
			case 5:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Enemies sleep for one turn on appearing", Classes[1], true, 1f);
			tute.AddQuote("Keep their numbers low. The more there are, the more damage you'll take!", Classes[1], true, 1F);
			break;
			case 8:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Tiles each have different values and effects", Classes[3], true, 1F);
			tute.AddQuote("Hold down on a tile to see its Info.", Classes[3], true, 1F);
			break;
			case 11:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("When you fill a mana pool, it creates a special tile.", Classes[2], true, 1f);
			tute.AddQuote("Tiles can have many different effects! Read their Info and experiment!", Classes[2], true, 1f);
			break;
		}

		if(tute != null) StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
	}
}

[System.Serializable]
public class Ops
{
	public Vector2 Resolution;
	public bool ViewTileStats;
	private float game_speed = 1.0F;
	public float GameSpeed
	{
		get{return game_speed;}
		set{game_speed = Mathf.Clamp(value, 0.0F, 2.0F);}
	}

	public KeyCode ViewTileStatsKey = KeyCode.LeftShift;

	public KeyCode GravityUp = KeyCode.UpArrow, GravityDown = KeyCode.DownArrow, GravityLeft = KeyCode.LeftArrow, GravityRight = KeyCode.RightArrow;
	//public bool HealthFromResource = false;
	public bool HPBasedOnHits = false;
	public bool ShowNumbers = false;
}

[System.Serializable]
public class EquipmentContainer{
	public Item Helm;
	public Item Chest;
	public Item Weapon;
	public Item Shield;
	public Item Boots;

	[HideInInspector]
	public int Length = 5;

	public Item this[int i]
	{
		get{
			switch(i)
			{
				case 0:
				return Helm;
				case 1:
				return Chest;
				case 2:
				return Weapon;
				case 3:
				return Shield;
				case 4:
				return Boots;
			}
			return null;
		}

		set{
			switch(i)
			{
				case 0:
				Helm = value;
				break;
				case 1:
				Chest = value;
				break;
				case 2:
				Weapon = value;
				break;
				case 3:
				Shield = value;
				break;
				case 4:
				Boots = value;
				break;
			}
		}
	}

	public string TypeOf(int i)
	{
		switch(i)
		{
			case 0:
			return "Helm";
			case 1:
			return "Chest";
			case 2:
			return "Weapon";
			case 3:
			return "Shield";
			case 4:
			return "Boots";
		}
		return "ERROR";
	}
}