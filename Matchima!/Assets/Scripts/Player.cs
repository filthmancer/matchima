using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public static Player instance;
	public static Ops Options
	{
		get{return Player.instance._Options;}
	}
	public static int RequiredMatchNumber = 3;
	public static bool loaded = false;
	public static Stat Stats;

	public bool CompleteMatch = true;
	public int Turns = 0;
	public int BestCombo = 0;
	public bool retired;
	public Ops _Options;
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
		get
		{
			List<Ability> final = new List<Ability>();
			foreach(Class child in Classes)
			{
				if(child == null) continue;
				foreach(Slot sl in child.AllMods)
				{
					if(sl is Ability) final.Add(sl as Ability);
				}
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
				foreach(Slot sl in child.AllMods)
				{
					if(sl is Item) final.Add(sl as Item);
				}
				foreach(Slot sl in child._Slots)
				{
					if(sl is Item) final.Add(sl as Item);
				}
			}
			return final.ToArray();
		}
	}

	public EquipmentContainer Equipment;
	public List<Item> ThisTurn_items = new List<Item>();
	public List<UpgradeGroup> ThisTurn_upgrades = new List<UpgradeGroup>();
	public List<ClassEffect> _Status = new List<ClassEffect>();


	float idle_quote_chance = 0.02F;
	bool idle_quote = false;
	float idle_time = 0.0F;
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

	public void CompleteHealth()
	{
		Stats.CompleteHealth();
		ResetStats();
	}

	public void OnTileMatch(Tile t)
	{
		
	}

	public void OnTileDestroy(Tile t)
	{
		if(t is Enemy)
		{
			Enemy e = t as Enemy;
			foreach(Class child in Classes)
			{
				child.AddExp(e);
			}
		}
	}

	public void OnTileCollect(Tile t)
	{
		if(t is Enemy)
		{
			Enemy e = t as Enemy;
			foreach(Class child in Classes)
			{
				child.AddExp(e);
			}
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

	public IEnumerator BeforeMatch(List<Tile> tiles, bool original = false)
	{
		PlayerControl.instance.AddTilesToFinal(tiles.ToArray());
		for(int i = 0; i < tiles.Count; i++)
		{
			if(tiles[i] == null) continue;
			if(tiles[i].BeforeMatchEffect) yield return StartCoroutine(tiles[i].BeforeMatch(original));
		}

		foreach(Class child in Classes)
		{
			yield return StartCoroutine(child.BeforeMatch(tiles));
		}

		yield break;
	}

	public IEnumerator AfterMatch()
	{
		yield break;
	}

	public IEnumerator BeginTurn()
	{
		//yield return new WaitForSeconds(0.1F);
		//foreach(Ability child in Abilities)
		//{
		//	if(child == null) continue;
		//	if(!child.initialized) child.initialized = true;
		//	child.BeforeTurn();
		//}
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
			
			//yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
			UIManager.instance.SetClassButtons(false);
		}

		
		UIManager.instance.SetClassButtons(false);
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
			UIManager.Objects.BotGear.SetTween(0, true);
			UIManager.instance.WaveAlert.SetTween(0,true);
			UIManager.instance.ItemUI.gameObject.SetActive(true);
			UIManager.instance.current_class = null;
			//UIManager.instance.current_item = child;
			UIManager.instance.ShowItemUI(ThisTurn_items.ToArray());

			while(UIManager.ItemUI_active) 
			{
				yield return null;
			}
		}
		UIManager.instance.WaveAlert.SetTween(0,false);

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

		UIManager.instance.ItemUI.gameObject.SetActive(false);
		
		PlayerControl.instance.canMatch = true;

		ThisTurn_items.Clear();
		ThisTurn_upgrades.Clear();

		Stats.PrevTurnKills = 0;
		InitStats.PrevTurnKills = 0;
		CompleteMatch = true;
		yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
		yield return null;
	}

	public IEnumerator EndTurn()
	{
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.EndTurn());
		}
		ResetStats();
		Turns ++;
		yield return null;
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
		for(int i = 0; i < Equipment.Length; i++)
		{
			if(Equipment[i] == null) continue;
			Bonus b = Equipment[i].CheckBonus();
			if(b != null) bonuses.Add(b);
		}
		return bonuses;
	}

	public void AddStatus(int i, string StatusName, int Duration, params string [] args)
	{
		ClassEffect e = (ClassEffect) Instantiate(GameData.instance.GetTileEffectByName(StatusName));
		e.GetArgs(Duration, args);
		if(i == -1) 
		{
			foreach(ClassEffect child in _Status)
			{
				if(child.Name == e.Name)
				{
					child.Duration += e.Duration;
					Destroy(e.gameObject);
					return;
				}
			}
			_Status.Add(e);
			e.transform.parent = this.transform;
		}
		else 
		{
			foreach(ClassEffect child in Classes[i]._Status)
			{
				if(child.Name == e.Name)
				{
					child.Duration += e.Duration;
					Destroy(e.gameObject);
					return;
				}
			}
			Classes[i]._Status.Add(e);
			e.Setup(Classes[i]);
			e.transform.parent = Classes[i].transform;
		}
	}

	public void AddClassToSlot(int slot, Class c)
	{
		Classes[slot] = Instantiate(c);
		Classes[slot].transform.parent = this.transform;
		Classes[slot].Genus = (GENUS) slot;
		Classes[slot].Index = slot;
		Classes[slot].StartClass();

		ResetStats();
		//print(UIManager.ClassButtons[slot]);
		//UIManager.ClassButtons[slot].Setup(Classes[slot]);
		UIManager.instance.AddClass(Classes[slot], slot);
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

	public void Load(Class [] classes = null)
	{
		StartCoroutine(LoadClasses(classes));
	}

	IEnumerator LoadClasses(Class [] c = null)
	{
		if(c == null)
		{
			Classes = new Class[_Classes.Length];
			for(int i = 0; i < _Classes.Length; i++)
			{
				if(_Classes[i] == null) continue;

				Classes[i] = (Class) Instantiate(_Classes[i]);
				Classes[i].transform.parent = this.transform;
			}
		}
		else 
		{
			Classes = new Class[c.Length];
			for(int i = 0; i < c.Length; i++)
			{
				if(c[i] == null) continue;
				Classes[i] = c[i];
				Classes[i].transform.parent = this.transform;
			}
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
			Stats.AddStats(_Status[i].CheckStats());
			_Status[i].StatusEffect();
		}

		Stats._Health = (int) Mathf.Clamp(Stats._HealthMax * ratio, 0, Stats._HealthMax);
		Stats._Armour = armour;
		Stats._Attack = finalattack;

		Stats.MapSize.x = Mathf.Clamp(Stats.MapSize.x, 3, 14);
		Stats.MapSize.y = Mathf.Clamp(Stats.MapSize.y, 3, 14);
		
		if(TileMaster.instance.MapSize != Stats.MapSize && !Stats.isKilled)
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

		if(GameManager.instance._Wave != null) GameManager.instance._Wave.GetChances();
		
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

		foreach(Item child in Items){
			if(child == null) continue;
			child.DamageIndicator(ref indiv_damage, selected);
		}

		return indiv_damage;
	}

	public void OnHit(params Tile[] attackers)
	{
		int hit =  0;
		for(int i = 0; i < attackers.Length; i++)
		{
		 	hit += attackers[i].GetAttack();
		}

		foreach(Item child in Items)
		{
			hit = child.OnHit(hit, attackers);
		}
		foreach(Ability child in Abilities)
		{
			hit = child.OnHit(hit, attackers);
		}
		
		Stats.Hit(hit, attackers);
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
	public bool RealHP = false;
	public bool ShowNumbers = false;
	public bool ShowIntroWaves;
	public bool SkipAllStory;
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