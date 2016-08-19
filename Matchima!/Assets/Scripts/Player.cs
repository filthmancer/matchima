using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

#region Variables
	public static Player instance;
	public static Ops Options
	{
		get {return Player.instance._Options;}
	}
	public static int RequiredMatchNumber = 3;
	public static bool loaded = false;
	public static Stat Stats;

	public static List<Item> StashItems
	{
		get {
			return instance.ThisTurn_items;
		}
	}

	public static bool NewItems
	{
		get
		{
			foreach (Item child in StashItems)
			{
				if (!child.Seen) return true;
			}
			return false;
		}
	}


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
		get {
			List<Slot> final = new List<Slot>();
			foreach (Class child in Classes)
			{
				if (child == null) continue;
				foreach (Slot sl in child._Slots)
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
			foreach (Class child in Classes)
			{
				if (child == null) continue;
				foreach (Slot sl in child.AllMods)
				{
					if (sl is Ability) final.Add(sl as Ability);
				}
				foreach (Slot sl in child._Slots)
				{
					if (sl is Ability) final.Add(sl as Ability);
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
			foreach (Class child in Classes)
			{
				if (child == null) continue;
				foreach (Slot sl in child.AllMods)
				{
					if (sl is Item) final.Add(sl as Item);
				}
				foreach (Slot sl in child._Slots)
				{
					if (sl is Item) final.Add(sl as Item);
				}
			}
			return final.ToArray();
		}
	}

	public int [] MeterThisTurn = new int[7];

	public EquipmentContainer Equipment;
	public List<Item> ThisTurn_items = new List<Item>();
	public List<UpgradeGroup> ThisTurn_upgrades = new List<UpgradeGroup>();
	public List<ClassEffect> _Status = new List<ClassEffect>();

	[SerializeField]
	private LevelContainer _Level;
	public static LevelContainer Level{get{return instance._Level;}}

	private Unlock [] _Unlocks = new Unlock[]
	{
		new Unlock("quickmode", 2, "Quick Crawl Mode"),
		new Unlock("charselect", 2, "Character Selection"),
		new Unlock("farmer", 3, "The Farmer", false),
		new Unlock("endlessmode", 5, "Endless Mode"),
		new Unlock("squire", 6, "The Squire", false),
		new Unlock("warden", 7, "The Warden", false),
		new Unlock("merchant", 8, "The Merchant", false)
	};
	public Unlock [] Unlocks{get{return _Unlocks;}}
	
	float idle_quote_chance = 0.02F;
	bool idle_quote = false;
	float idle_time = 0.0F;
#endregion

#region Generic Functions
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

		if (PlayerControl.instance.TimeWithoutInput > 20.0F)
		{
			idle_time -= Time.deltaTime;
			if (idle_time <= 0.0F)
			{
				idle_time = Random.Range(7.5F, 15.0F);
				int randa = Random.Range(0, 4);
				bool info = Random.value > 0.5F;
				if (Classes[randa] != null)
					StartCoroutine(UIManager.instance.Quote((info ? Classes[randa].Quotes.Info : Classes[randa].Quotes.Idle)));
			}
		}
		else if (PlayerControl.instance.TimeWithoutInput == 0.0F) idle_time = 0.0F;

	}
#endregion

#region Event-Based Functions
	public StatContainer GetResourceFromGENUS(GENUS ab)
	{
		switch (ab)
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

	public void CompleteRegen()
	{
		int heal = 0;
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			int [] regen = child.Stats.CompleteRegen();
			heal += regen[0];
			child.AddToMeter(regen[1]);
		}
		
		Stats.Heal(heal);
	}

	public void OnTileMatch(Tile t)
	{

	}

	public void OnTileDestroy(Tile t)
	{
		//if(t is Enemy)
		//{
		//	Enemy e = t as Enemy;
		//	foreach(Class child in Classes)
		//	{
		//		child.AddExp(e);
		//	}
		//}
	}

	public void OnTileCollect(Tile t)
	{
		//if(t is Enemy)
		//{
		//	Enemy e = t as Enemy;
		//	foreach(Class child in Classes)
		//	{
		//		child.AddExp(e);
		//	}
		//}
	}

	public int CompleteClasses()
	{
		foreach (Class child in Classes)
		{
			if (child == null) continue;
			child.Complete();
		}
		return 0;
	}

	public void OnHit(params Tile[] attackers)
	{
		int hit =  0;
		for (int i = 0; i < attackers.Length; i++)
		{
			hit += attackers[i].GetAttack();
		}

		foreach (Item child in Items)
		{
			hit = child.OnHit(hit, attackers);
		}
		foreach(Class child in Classes)
		{
			if(child == null) continue;
			hit = child.OnHit(hit,attackers);
		}

		Stats.Hit(hit, attackers);
	}

	public void CheckForBestCombo(int[] combo)
	{
		int final = 0;
		for (int i = 0; i < combo.Length; i++)
		{
			final += combo[i];
		}
		if (final > BestCombo)
		{
			BestCombo = final;
		}
	}

#endregion

#region Matching Functions
	public IEnumerator BeforeMatch(List<Tile> tiles)
	{
		foreach (Class child in Classes)
		{
			if(child == null) continue;
			yield return StartCoroutine(child.BeforeMatch(tiles));
		}

		yield break;
	}

	public IEnumerator AfterMatch()
	{
		PlayerControl.instance.finalTiles.Clear();
		PlayerControl.instance.selectedTiles.Clear();
		CompleteRegen();
		CompleteHealth();
		PlayerControl.matchingTile = null;

		TileMaster.instance.SetFillGrid(true);
		while (!TileMaster.AllLanded)	yield return null;
		UIManager.instance.SetBonuses(GameManager.instance.GetBonuses(GameManager.ComboSize));
		UIManager.instance.StartTimer();
		while (UIManager.instance.IsShowingMeters) yield return null;
		yield break;
	}

	public IEnumerator BeginTurn()
	{
		foreach (Class child in Classes)
		{
			if (child == null) continue;
			yield return StartCoroutine(child.BeginTurn());

			for (int i = 0; i < child._Status.Count; i++)
			{
				if (child._Status[i].CheckDuration())
				{
					Destroy(child._Status[i].gameObject);
					child._Status.RemoveAt(i);
				}
			}

			//yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
			UIManager.instance.SetClassButtons(false);
		}


		UIManager.instance.SetClassButtons(false);
		for (int i = 0; i < _Status.Count; i++)
		{
			if (_Status[i].CheckDuration())
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


		//if(ThisTurn_items.Count > 0)
		//{
		//	UIManager.Objects.BotGear.SetTween(0, true);
		//	UIManager.instance.ItemUI.gameObject.SetActive(true);
		//	UIManager.instance.current_class = null;
		//	UIManager.instance.ScreenAlert.SetTween(0, true);
		//	UIManager.instance.ShowItemUI(ThisTurn_items.ToArray());
		//	while(UIManager.ItemUI_active)
		//	{
		//		yield return null;
		//	}
		//	UIManager.instance.ScreenAlert.SetTween(0, false);
		//}

		//UIManager.instance.WaveAlert.SetTween(0,false);

		if (ThisTurn_upgrades.Count > 0)
		{
			foreach (UpgradeGroup child in ThisTurn_upgrades)
			{
				if (child.Upgrades.Length > 1)
				{
					UIManager.instance.OpenBoonUI(child);
					while (UIManager.BoonUI_active)
					{
						yield return null;
					}
				}
				else
				{
					child.Target.GetUpgrade(child.Upgrades[0]);
					yield return new WaitForSeconds(0.1F);
				}
				foreach (ClassUpgrade up in child.Upgrades)
				{
					up.Value = 1;
				}
			}
		}

		UIManager.instance.ItemUI.gameObject.SetActive(false);

		PlayerControl.instance.canMatch = true;

		//ThisTurn_items.Clear();
		ThisTurn_upgrades.Clear();

		Stats.PrevTurnKills = 0;
		InitStats.PrevTurnKills = 0;
		CompleteMatch = true;
		//MeterThisTurn = new int[4];
		yield return null;
	}

	public IEnumerator EndTurn()
	{
		foreach (Class child in Classes)
		{
			if (child == null) continue;
			yield return StartCoroutine(child.EndTurn());
		}
		ResetStats();
		Turns ++;
		yield return null;
	}
#endregion

	/*public void CheckForBoons()
	{
		StartCoroutine(CheckForBoonsRoutine());
	}

	public IEnumerator CheckForBoonsRoutine()
	{
		foreach (Class child in Classes)
		{
			if (child == null) continue;
			yield return StartCoroutine(child.CheckForBoon());
		}
		yield return null;
	}*/

	bool showingpickup = false;
	public void PickupItem(Item i)
	{
		StartCoroutine(_PickupItem(i));
	}

	IEnumerator _PickupItem(Item i)
	{
		while (showingpickup) yield return null;
		showingpickup = true;
		(UIManager.Objects.MiddleGear[2] as UIObjTweener).SetTween(1, true);
		UIManager.Objects.MiddleGear[2][0].SetActive(false);
		UIManager.Objects.MiddleGear[2].Txt[0].text = "FOUND\n" + i.Name.Value;
		UIManager.Objects.MiddleGear[2].Txt[1].text = "Added to Stash";

		yield return new WaitForSeconds(GameData.GameSpeed(2.0F));
		(UIManager.Objects.MiddleGear[2] as UIObjTweener).SetTween(1, false);
		yield return new WaitForSeconds(GameData.GameSpeed(0.45F));
		UIManager.Objects.MiddleGear[2][0].SetActive(true);
		UIManager.Objects.MiddleGear[2].Txt[0].text = "";
		UIManager.Objects.MiddleGear[2].Txt[1].text = "";
		UIManager.Objects.TopRightButton.Txt[2].enabled = true;
		ThisTurn_items.Add(i);
		showingpickup = false;
	}

	public void PickupUpgrade(UpgradeGroup c)
	{
		ThisTurn_upgrades.Add(c);
	}

	public IEnumerator CheckHealth()
	{
		for (int i = 0; i < Classes.Length; i++)
		{
			if (Classes[i] != null) Classes[i].CheckHealth();
		}

		if (Stats._Health < Stats._HealthMax / 5 && Stats._Health > 0)
		{
			int randa = Random.Range(0, 4);
			if(Classes[randa] != null)
			StartCoroutine(UIManager.instance.Quote(Classes[randa].Quotes.Danger));
		}

		if (Stats._Health <= 0)
		{
			yield return StartCoroutine(DeathRoll());
		}
		yield return null;
	}

	IEnumerator DeathRoll()
	{
		//Find all living characters
		List<int> living_chars = new List<int>();
		for (int i = 0; i < 4; i++)
		{
			if(Classes[i] == null) continue;
			if (!Classes[i].isKilled) living_chars.Add(i);
		}

		if (living_chars.Count == 0)
		{
			Stats.isKilled = true;
			yield break;
		}

		//Target Char to be killed
		int target = living_chars[Random.Range(0, living_chars.Count)];

		//ROLL LUCK STAT OF TARGET TO SEE IF THEY DIE
		float luck_chance = (float) Classes[target].Stats.Luck / (GameManager.Difficulty * 3.2F);
		luck_chance += Classes[target].Stats.DeathSaveChance;

		bool roll = Random.value > luck_chance;
		yield return StartCoroutine(UIManager.instance.ShowDeathIcon(Classes[target], roll));
		Stats.Heal(50);
		if (roll)
		{
			//StartCoroutine(UIManager.instance.Quote(Classes[target].Quotes.Death));

			if (living_chars.Count == 1)
			{
				Stats.isKilled = true;
			}
		}
		ResetStats();
	}

	public List<Bonus> CheckForBonus(GENUS g)
	{
		List<Bonus> bonuses = new List<Bonus>();
		for (int i = 0; i < Equipment.Length; i++)
		{
			if (Equipment[i] == null) continue;
			Bonus b = Equipment[i].CheckBonus();
			if (b != null) bonuses.Add(b);
		}
		return bonuses;
	}

	public void AddStatus(int i, string StatusName, int Duration, params string [] args)
	{
		ClassEffect e = (ClassEffect) Instantiate(GameData.instance.GetTileEffectByName(StatusName));
		e.GetArgs(Duration, args);
		if (i == -1)
		{
			foreach (ClassEffect child in _Status)
			{
				if (child.Name == e.Name)
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
			foreach (ClassEffect child in Classes[i]._Status)
			{
				if (child.Name == e.Name)
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

	public IEnumerator AddClassToSlot(int slot, Class c)
	{
		Classes[slot] = Instantiate(c);
		Classes[slot].transform.parent = this.transform;
		Classes[slot].Genus = (GENUS) slot;
		Classes[slot].Index = slot;
		Classes[slot].StartClass();

		ResetStats();
		
		yield return StartCoroutine(UIManager.instance.AddClass(Classes[slot], slot));
	}

	public void Reset()
	{
		loaded = false;
		foreach (Class child in Classes)
		{
			if (child == null) continue;
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
		if (c == null)
		{
			Classes = new Class[_Classes.Length];
			for (int i = 0; i < _Classes.Length; i++)
			{
				if (_Classes[i] == null) continue;

				Classes[i] = (Class) Instantiate(_Classes[i]);
				Classes[i].transform.parent = this.transform;
				Classes[i].Genus = (GENUS)i;
				Classes[i].Index = i;
				Classes[i].StartClass();
				yield return null;
			}
		}
		else
		{
			Classes = new Class[c.Length];
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i] == null) continue;
				Classes[i] = c[i];
				Classes[i].transform.parent = this.transform;
				Classes[i].Genus = (GENUS)i;
				Classes[i].Index = i;
				Classes[i].StartClass();
				yield return null;
			}
		}

		/*for(int i = 0; i < Classes.Length; i++)
		{
			if(Classes[i] !=null)
			{

				//yield return null;
			}
		}*/
		ResetStats();

		Stats._Health = Stats._HealthMax;
		//yield return new WaitForSeconds(0.1F);


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
		for (int i = 0; i < Classes.Length; i++)
		{
			if (Classes[i] != null)
			{
				Classes[i].Reset();
				if (!Classes[i].isKilled)
				{
					Stats.AddStats(Classes[i].Stats);
					finalattack += Classes[i].Stats._Attack;
					//finalhealth += Classes[i].Stats._Health;
				}
			}
		}

		for (int i = 0; i < _Status.Count; i++)
		{
			Stats.AddStats(_Status[i].CheckStats());
			_Status[i].StatusEffect();
		}

		Stats._Health = (int) Mathf.Clamp(Stats._HealthMax * ratio, 0, Stats._HealthMax);
		Stats._Armour = armour;
		Stats._Attack = finalattack;

		Stats.MapSize.x = Mathf.Clamp(Stats.MapSize.x, 0, 4);
		Stats.MapSize.y = Mathf.Clamp(Stats.MapSize.y, 0, 4);

		Stats.MatchNumberModifier = Mathf.Clamp(Stats.MatchNumberModifier, -2, 100);;

		if (!Stats.isKilled && TileMaster.GridSetup)
		{
			Vector2 finalMap = TileMaster.instance.MapSize_Default
			                   + Stats.MapSize;
			TileMaster.instance.IncreaseGridTo(finalMap);
		}

		RequiredMatchNumber = Mathf.Clamp(3 + Stats.MatchNumberModifier, 1, 10);
		ResetChances();
	}

	public void ResetChances()
	{
		TileMaster.instance.ResetChances();
		foreach (TileChance t in Stats.TileChances)
		{
			TileMaster.instance.IncreaseChance(t.Genus, t.Type, t.Chance);
			if (t.Value > 0)
			{
				SPECIES s = TileMaster.Types[t.Type];
				if(t.Genus == string.Empty)
				{
					s._ValueAdded.y += t.Value;
				}
				else
				{
					GenusInfo g = s[t.Genus];
					g.ValueAdded.y += (t.Value);
				}
				
			}
		}

		if (GameManager.Wave != null) GameManager.Wave.GetChances();

		Spawner2.GetSpawnables(TileMaster.Types);
	}

	public int [] ActiveDamage(int damage, Tile [] selected)
	{
		int[] indiv_damage = new int[selected.Length];
		for (int i = 0; i < selected.Length; i++)
		{
			if (selected[i].Type.isEnemy) indiv_damage[i] = damage;
			else indiv_damage[i] = 0;
		}

		foreach (Ability child in Abilities)
		{
			if (child == null) continue;
			child.DamageIndicator(ref indiv_damage, selected);
		}

		foreach (Item child in Items) {
			if (child == null) continue;
			child.DamageIndicator(ref indiv_damage, selected);
		}

		return indiv_damage;
	}

#region Player Level/Unlocks
	public IEnumerator AddXP(int xp)
	{
		int rate = 1;
		float rate_soft = 1.0F;
		int current = 0;
		while(current < xp)
		{
			int actual = Mathf.Clamp(rate, 0, xp-current);
			current += actual;
			
			rate_soft *= 1.05F;
			rate = (int)rate_soft;
			if(_Level.AddXP(actual))
			{
				MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear[1][2].transform.position, "Level Up!", 60);
				rate = 1;
				rate_soft = 1.0F;
				UIManager.instance.UpdatePlayerLvl();
				yield return new WaitForSeconds(0.05F);
			}
			UIManager.instance.UpdatePlayerLvl();
			yield return null;
		}
		yield return null;
	}


	public void SetLevelInfo(int lvl, int xp)
	{
		if(lvl < 1) lvl = 1;
	 	_Level.Level = lvl;
	 	_Level.XP_Current = xp;
	 	_Level.XP_RequiredArray_num = lvl-1;
	 	_Level.XP_Required = Level.XP_RequiredArray[lvl-1];
	 	while(_Level.AddXP(0))
	 	{

	 	}
	}

	public bool GetUnlock(string s){
		string fin = s.ToLower();
		for(int i = 0; i < Unlocks.Length; i++)
		{
			if(string.Equals(Unlocks[i].Name, fin)) return Unlocks[i].Value;
		}
		return false;
	}

#endregion

#region Attack/Spell Values
	public static int AttackValue
	{
		get{return (int) ((float)Stats.GetAttack() * (1.0F + AddedAttackPower));}
	}
	public static int SpellValue
	{
		get{return (int) ((float)Stats.GetSpell() * (1.0F + AddedSpellPower));}
	}
	
	public static float AttackPower
	{
		get{return AddedAttackPower + Stats.GetAttackPower();}
	}
	
	public static float SpellPower
	{
		get{return AddedSpellPower + Stats.GetSpellPower();}
	}
	public static float AddedAttackPower = 0;
	public static float AddedSpellPower  = 0;

	public int [] GetAttackValues(Tile [] selected)
	{
		//GET ATTACK POWER
		AddedAttackPower = 0;
		AddedSpellPower = 0;
		for(int i = 0; i < selected.Length; i++)
		{
			AddedAttackPower += selected[i].Stats.AttackPower;
			AddedSpellPower += selected[i].Stats.SpellPower;
		}

		//CHECK PLAYER, CLASSES, ITEMS FOR BONUS ATTACK
		int [] final = new int[selected.Length];
		final = ActiveDamage(AttackValue, selected);

		return final;
	}
#endregion
}

//Options
[System.Serializable]
public class Ops
{
	public Vector2 Resolution;
	public bool ViewTileStats;
	private float game_speed = 1.0F;
	public float GameSpeed
	{
		get {return game_speed;}
		set {game_speed = Mathf.Clamp(value, 0.0F, 2.0F);}
	}

	public KeyCode ViewTileStatsKey = KeyCode.LeftShift;

	public KeyCode GravityUp = KeyCode.UpArrow, GravityDown = KeyCode.DownArrow, GravityLeft = KeyCode.LeftArrow, GravityRight = KeyCode.RightArrow;
	//public bool HealthFromResource = false;
	public bool RealHP = false;
	public bool ShowNumbers = false;
	public Ops_Story StorySet = Ops_Story.Default;

	public bool PowerupAlerted = false;
}

public enum Ops_Story
{
	Default, AlwaysShow, NeverShow
}

//Item Information
[System.Serializable]
public class EquipmentContainer {
	public Item Helm;
	public Item Chest;
	public Item Weapon;
	public Item Shield;
	public Item Boots;

	[HideInInspector]
	public int Length = 5;

	public Item this[int i]
	{
		get {
			switch (i)
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

		set {
			switch (i)
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
		switch (i)
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

//Player Global Leveling System
[System.Serializable]
public class LevelContainer
{
	public int Level = 1;
	public int XP_Current;
	public int XP_Required;

	public int [] XP_RequiredArray = new int[]
	{
		100,
		250,
		400,
		950,
		1500,
		3500,
		7000,
		18000
	};
	public Color [] Level_Colors;
	public int XP_RequiredArray_num = 0;

	public float XP_Ratio {get{return (float)XP_Current/(float)XP_Required;}}
	public LevelContainer()
	{
		Level = 1;
		XP_Current = 0;
		XP_Required = XP_RequiredArray[0];
		XP_RequiredArray_num = 0;
	}

	public bool AddXP(int num)
	{
		XP_Current = Mathf.Clamp(XP_Current + num, 0, XP_Required);
		if(XP_Current == XP_Required)
		{
			Level++;
			XP_RequiredArray_num++;
			XP_Required = XP_RequiredArray[XP_RequiredArray_num];
			XP_Current = 0;
			return true;
		}
		return false;
	}

	public Color LevelColor
	{
		get{
			int num = 0;
			if(Level < 5) num = 0;
			else if(Level < 10) num = 1;
			else if(Level < 20)	num = 2;
			num = Mathf.Clamp(num, 0, Level_Colors.Length-1);
			return Level_Colors[num];
		}
	}

	public Color GetColor(int lvl)
	{
		int num = 0;
		if(lvl < 5) num = 0;
		else if(lvl < 10) num = 1;
		else if(lvl < 20)	num = 2;
		num = Mathf.Clamp(num, 0, Level_Colors.Length-1);
		return Level_Colors[num];
	}
}

//Player Global Unlocks
public class Unlock
{
	public string Name;
	public int Level_Required;

	public string GetTitle()
	{
		if(showtitle) return _Title;
		return (showtitle || Value) ? _Title : "???";
	}

	private string _Title;
	private bool showtitle = true;
	public Unlock(string n, int lvl, string title, bool showt = true)
	{
		Name = n;
		Level_Required = lvl;
		_Title = title;
		showtitle = showt;
	}
	public bool Value
	{
		get{return Player.Level.Level >= Level_Required;}
	} 
}