using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DiffMode
{
	Easy,
	Okay,
	Hard
}

public enum GameMode
{
	Story,
	Endless,
	None
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public static _GameSaveLoad PlayerLoader;

	public static bool debug = false;
	public static bool inStartMenu;

	public static int ComboSize;
	public static Zone Zone	{get{return instance.CurrentZone;}}
	public static int Floor	{get{return instance.CurrentFloor;}}
	public static int ZoneNum {get{return instance._ZoneNum;}}
	public static Wave Wave{get{return instance.CurrentWave;}}

	public static float GlobalManaMult = 1.0F,
						GlobalHealthMult = 1.0F,
						GlobalArmourMult = 1.0F;
	public static float GrowthRate_Easy = 0.19F,
						GrowthRate_Normal = 0.26F,
						GrowthRate_Hard = 0.40F;

	public static float [] MeterDecay
	{
		get
		{
			return new float [] {	GameManager.instance.MeterDecayLvl0,
									GameManager.instance.MeterDecayLvl1,
									GameManager.instance.MeterDecayLvl2, 
									GameManager.instance.MeterDecayLvl3};
		}
	}
	private float	MeterDecayLvl0 = 0.0F,
					MeterDecayLvl1 = 1.12F, 
					MeterDecayLvl2 = 1.25F, 
					MeterDecayLvl3 = 1.20F;

	public Juice _Juice;
	public AudioSource AudioObj;
	public AudioClip MatchA, TouchA;
	public AudioClip ReleaseA;

	public float Difficulty_Growth{
		get{
			switch(DifficultyMode)
			{
				case DiffMode.Okay:
				return Player.instance.Turns/20.0F * GrowthRate_Normal;
				case DiffMode.Easy:
				return Player.instance.Turns/20.0F * GrowthRate_Easy;
				case DiffMode.Hard:
				return Player.instance.Turns/20.0F * GrowthRate_Hard;
			}
			return Player.instance.Turns/20.0F * GrowthRate_Easy;
		}
	}

	private float Difficulty_init = 2.5F;
	public static float Difficulty = 1;
	public float _Difficulty = 0;
	public DiffMode DifficultyMode = DiffMode.Okay;
	public GameMode Mode = GameMode.Story;

	public int CurrentFloor = 0;
	public Zone CurrentZone;
	public Wave CurrentWave;
	private int _ZoneNum = 0;

	public bool gameStart = false;
	public bool paused = false;
	public bool EnemyTurn = false;

	public long RoundTokens = 0;

	private int OverflowThisTurn = 0;
	private int OverflowMulti = 2;

	private float OverflowHitMulti = 1.0F;
	private float AllOfTypeMulti = 1.2F;


	public bool LevelUp = false;
	
	public bool isPaused
	{
		get{
			return !gameStart || paused;
		}
	}
	public static bool TuteActive = false;

	public Zone StoryMode;
	public Zone [] Zones;
	
	public WaveGroup DefaultWaves;
	private float EnemyTurnTime = 0.0F;

	[HideInInspector]
	public Zone ResumeZone = null;
	[HideInInspector]
	public Wave ResumeWave = null;

	void OnApplicationQuit()
	{
		GameData.instance.Save();
		//PlayerPrefs.SetInt("Resume", 0);
		if(!gameStart) return;
		PlayerLoader.Save();
		PlayerPrefs.SetInt("Resume", gameStart ? 1 : 0);
		PlayerPrefs.SetString("Name", Player.Classes[0].Name);
		PlayerPrefs.SetInt("Turns", Player.instance.Turns);
	}

	void OnApplicationPause()
	{
		if(!Application.isEditor)
		{
			GameData.instance.Save();
			if(gameStart) 
			{
				PlayerLoader.Save();
				PlayerPrefs.SetInt("Resume", gameStart ? 1 : 0);
				PlayerPrefs.SetString("Name", Player.Classes[0].Name);
				PlayerPrefs.SetInt("Turns", Player.instance.Turns);
			}
			
		} 
	}

	void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		
		  
		if(instance == null)
		{
			DontDestroyOnLoad(transform.gameObject);
			instance = this;
		}
		else if(instance != this) 
		{
			instance.Invoke("Start",0.05F);
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		paused = false;
		gameStart = false;
		_Juice = GetComponent<Juice>();
		PlayerLoader = GetComponent<_GameSaveLoad>();
		GameData.instance.Load();
		ResumeZone = null;
		Difficulty = Difficulty_init;

		CurrentFloor = 0;
		_ZoneNum = 0;

		inStartMenu = true;
		TuteActive = false;	
	}
	
	// Update is called once per frame
	void Update () {
		_Difficulty = Difficulty;
		if(Player.loaded && UIManager.loaded && !gameStart && !Player.Stats.isKilled)
		{
			if(!GameData.loading_assets) GameData.instance.LoadAssets();
			if(Application.isMobilePlatform)
			{
				//Player.Options.GameSpeed = 0.8F;
			}
			if(GameData.loaded_assets)
			{
				if(ResumeZone != null) 
				{
					ResumeGame();
				}
				else
				{
					if(Mode == GameMode.Story) PlayStoryMode();
					else if (Mode == GameMode.Endless) PlayEndlessMode();
				}
			}
		}

		if(EnemyTurn)
		{
			if(EnemyTurnTime > 50) 
			{
				EnemyTurnTime = 0.0F;
				EnemyTurn = false;
			}
			else EnemyTurnTime += Time.deltaTime;
		}

		if(Input.GetKeyDown(KeyCode.Z)) 
		{
			Cheat(1);
		}
		if(Input.GetKeyDown(KeyCode.X)) 
		{
			Cheat(2);
		}
		if(Input.GetKeyDown(KeyCode.C)) 
		{
			//PlayerLoader.Save();
			Cheat(3);
		}
		if(Input.GetKeyDown(KeyCode.V))
		{
			Cheat(4);
		} 
		if(Input.GetKeyDown(KeyCode.B))
		{
			Cheat(5);
		}
		if(Input.GetKeyDown(KeyCode.N))
		{
			Cheat(6);
		}
		if(Input.GetKeyDown(KeyCode.M))
		{
			Cheat(7);
		}
		if(Input.GetKeyDown(KeyCode.A))
		{
			Cheat(8);
		}
		if(Input.GetKeyDown(KeyCode.S))
		{
			Cheat(9);
		}

		if(Input.GetKeyDown(KeyCode.U))			Player.Classes[0].AddToMeter(Player.Classes[0].MeterTop);
		if(Input.GetKeyDown(KeyCode.I))			Player.Classes[1].AddToMeter(Player.Classes[1].MeterTop);
		if(Input.GetKeyDown(KeyCode.O))			Player.Classes[2].AddToMeter(Player.Classes[2].MeterTop);
		if(Input.GetKeyDown(KeyCode.P))			Player.Classes[3].AddToMeter(Player.Classes[3].MeterTop);

		if(Input.GetKeyDown(KeyCode.H))			Player.Classes[0].LevelUp();
		if(Input.GetKeyDown(KeyCode.J))			Player.Classes[1].LevelUp();
		if(Input.GetKeyDown(KeyCode.K))			Player.Classes[2].LevelUp();
		if(Input.GetKeyDown(KeyCode.L))			Player.Classes[3].LevelUp();

		if(Input.GetKeyDown(KeyCode.F5)) PlayerControl.instance.focusTile.AddValue(5);
		if(Input.GetKeyDown(KeyCode.F10)) OpenInFileBrowser.Open(Application.persistentDataPath);
		if(Input.GetKeyDown(KeyCode.F6)) AddTokens(10000);
		if(Input.GetKeyDown(KeyCode.F12)) Player.Stats.Hit(10000);

		if(Input.GetKeyDown(KeyCode.P)) paused = !paused;


		if(Player.Stats.isKilled && !isPaused)
		{
			paused = true;
			gameStart = false;
			StartCoroutine(EndGame());
			return;
		}
	}

	public void Reset()
	{
		CurrentZone = null;
		CurrentWave = null;
		CurrentFloor = 0;
	}

	public void Retire()
	{
		UIManager.instance.ShowOptions();
		Player.Stats.isKilled = true;
		Player.instance.retired = true;
		paused = true;
		StartCoroutine(EndGame());
	}

	public void SaveAndQuit()
	{
		Player.instance.Reset();
		GameManager.instance.gameStart = false;
		GameData.instance.Save();
		Application.LoadLevel(0);
	}

	IEnumerator EndGame()
	{
		yield return new WaitForSeconds(0.3F);
		TileMaster.instance.ClearGrid(true);

		yield return new WaitForSeconds(0.3F);
		UIManager.instance.ShowKillUI(0, 0,0,0);
	}

	bool camopen;
	public void Cheat(int i)
	{
		switch(i)
		{
			case 1: //Z
			print(Difficulty + Mathf.Exp(Difficulty_Growth) + ":" + Difficulty_Growth);
			Difficulty += Mathf.Exp(Difficulty_Growth);
			Player.instance.Turns += 25;
			Player.Stats._Health = Player.Stats._HealthMax;
			break;
			case 2: //X
			(UIManager.Objects.MiddleGear[3] as UIObjTweener).SetTween(0);
			//GetWave(GameData.instance.GetRandomWave(), 2);
			//CameraUtility.SetTurnOffset(camopen);
			//EscapeZone();
		//	TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["chest"], GENUS.ALL, 2, 1);
			//UIManager.instance.ShowResourceUI();
			break;
			case 3: //c
			TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["lightning"], GENUS.DEX,1, 1);
			break;
			case 4: //V
			//GetTurn();
			int [] point = PlayerControl.instance.focusTile.Point.Base;
			TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["lightning"], GENUS.STR,1, 50);
			//(TileMaster.Tiles[point[0], point[1]] as Ward).Buff = "Frenzy";
			//TileEffect effect = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName("Fragile"));
			//effect.GetArgs(-1);
			//TileMaster.Tiles[point[0], point[1]].AddEffect(effect);

			break;
			case 5: //B
			PlayerControl.instance.focusTile.InitStats.TurnDamage += 1000;
			PlayerControl.instance.focusTile.Match(1000);
			break;
			case 6: //N
			Player.instance.InitStats.MapSize.y-=1;
			TileMaster.instance.IncreaseGrid(0,-1);
			break;
			case 7: //M
			Player.instance.InitStats.MapSize.x+=1;
			Player.instance.ResetStats();
			break;
			case 8: //A
			CameraUtility.instance.ScreenShake(0.6F, 1.1F);
			break;
			case 9: //S
			TileMaster.instance.Ripple(TileMaster.Grid.Tiles[3,3], 1.7F, 0.5F, 0.35F);
			//TileMaster.instance.Ripple(TileMaster.Grid.Tiles[3,3]);
			break;
		}

		
	}

	public void AddTokens(int tokens)
	{
		if(tokens == 0) return; 
		if(gameStart) 
		{
			int all = tokens;
			int d = 10;

			int ones = all % d - all % (d/10);
		all -= ones;
		d *= 10;


		int tens = all % d - all % (d/10);
		all -= tens;
		d *= 10;

		int hunds = all % d - all % (d/10);
		all -= hunds;

		}
		else
		{
			int _tk = PlayerPrefs.GetInt("AllTokens");
			_tk += tokens;

			PlayerPrefs.SetInt("AllTokens", _tk);
		}
	}



	public Zone GetZone(string name)
	{
		for(int i = 0; i < Zones.Length; i++)
		{
			if(Zones[i]._Name == name) return Zones[i];
		}
		return null;
	}

	public void EnterZone(Zone z = null, string name = null)
	{
		if(CurrentZone != null) Destroy(CurrentZone.gameObject);
		Zone target = null;

		if(z != null)
		{
			target = z;
		}
		else if(name != null) 
		{
			target = GetZone(name);
		}
		else target = Zones[Random.Range(0,Zones.Length)];

		if(target == null) return;

		_ZoneNum ++;
		CurrentZone = (Zone) Instantiate(target);
		CurrentZone.transform.parent = this.transform;
		CurrentZone.Randomise();

		UIManager.instance.BackingTint = CurrentZone.Tint;
		UIManager.instance.WallTint = CurrentZone.WallTint;
		TileMaster.instance.MapSize_Default = CurrentZone.GetMapSize();
		Player.instance.ResetStats();
		StartCoroutine(UIManager.instance.Alert(1.2F, false, CurrentZone.Name));
		
	}

	IEnumerator _EnterZone(Zone z = null, string name = null, Wave w = null)
	{
		if(CurrentZone != null) Destroy(CurrentZone.gameObject);
		Zone target = null;

		if(z != null)
		{
			target = z;
		}
		else if(name != null) 
		{
			target = GetZone(name);
		}
		else target = Zones[Random.Range(0,Zones.Length)];

		if(target == null) yield break;
		_ZoneNum ++;
		CurrentZone = (Zone) Instantiate(target);
		CurrentZone.transform.parent = this.transform;
		CurrentZone.Randomise();

		UIManager.instance.BackingTint = CurrentZone.Tint;
		UIManager.instance.WallTint = CurrentZone.WallTint;
		TileMaster.instance.MapSize_Default = CurrentZone.GetMapSize();
		Player.instance.ResetStats();
		yield return StartCoroutine(UIManager.instance.Alert(1.2F, false, CurrentZone.Name, null, "Entered"));
		yield return StartCoroutine(_GetWave(w));
	}

	public void EscapeZone()
	{
		StartCoroutine(_EscapeZone());
	}

	IEnumerator _EscapeZone()
	{
		string oldname = CurrentZone.Name;
		
		UIManager.instance.ShowZoneMenu();
		UIManager.Objects.MiddleGear[1].Txt[0].text = "Escaped " + CurrentZone.Name;

		yield return null;
	 	//yield return StartCoroutine(UIManager.instance.Alert(1.2F, false, "Escaped " + oldname));
	 	//yield return StartCoroutine(_EnterZone());
	}

	public void GetWave(Wave w = null)
	{
		CurrentFloor ++;
		if(w == null)
		{
			w = Zone.CheckZone();
			/*
			if(Mode == GameMode.Story) w = Zone.CheckZone();
			else if(Mode == GameMode.Endless) 
			{
				if(Random.value > 0.5F) w = DefaultWaves.GetWaveRandom();
				else w = CurrentZone.GetWaveRandom();
			}*/
		}

		if(CurrentWave != null && CurrentWave != w) Destroy(CurrentWave.gameObject);
		CurrentWave = Instantiate(w);
		CurrentWave.transform.parent = this.transform;
		StartCoroutine(CurrentWave.Setup());

		Difficulty += Mathf.Exp(Difficulty_Growth);
	
	}

	IEnumerator _GetWave(Wave w = null)
	{
		CurrentFloor ++;
		if(w == null)
		{
			w = Zone.CheckZone();
			/*
			if(Mode == GameMode.Story) w = Zone.CheckZone();
			else if(Mode == GameMode.Endless) 
			{
				if(Random.value > 0.5F) w = DefaultWaves.GetWaveRandom();
				else w = CurrentZone.GetWaveRandom();
			}*/
		}

		if(CurrentWave != null && CurrentWave != w) Destroy(CurrentWave.gameObject);
		CurrentWave = Instantiate(w);
		CurrentWave.transform.parent = this.transform;
		yield return StartCoroutine(CurrentWave.Setup());

		Difficulty += Mathf.Exp(Difficulty_Growth);
		
	}

	public void LoadGame(bool resume)
	{

		Class [] c = null;
		if(resume)
		{
			c = PlayerLoader.Load();
			if(c == null) return;
		}

		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,true);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,true);

		(UIManager.Objects.TopGear as UIGear).SetRotate(true, new Vector3(0,0,1));
		(UIManager.Objects.BotGear as UIGear).SetRotate(true, new Vector3(0,0,-1));

		TileMaster.instance.MapSize = new Vector2(1,1);

		Player.instance.Load(c);
		StartCoroutine(UIManager.instance.LoadUI());
	}

	public void PlayStoryMode()
	{
		inStartMenu = false;
		gameStart = true;
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;

		ClearUI();
		StartCoroutine(_EnterZone(StoryMode));
		//GetWave();
	}

	public void PlayEndlessMode()
	{
		inStartMenu = false;
		gameStart = true;
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;

		ClearUI();
		UIManager.Objects.TopGear.Txt[0].text = "";
		StartCoroutine(_EnterZone());
	}

	public void ResumeGame()
	{
		inStartMenu = false;
		gameStart = true;
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;

		ClearUI();
		StartCoroutine(_EnterZone(ResumeZone, null, ResumeWave));
	}

	public void LoadClass(ClassContainer targetClass)
	{
		UIManager.instance.LoadScreen.SetActive(true);
		StartCoroutine(UIManager.instance.LoadUI());
	}

	void ClearUI()
	{
		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,false);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,false);
		(UIManager.Objects.TopGear as UIGear).SetRotate(false);
		(UIManager.Objects.BotGear as UIGear).SetRotate(false);
		UIManager.Objects.BotGear[0].SetActive(true);
		//UIManager.Objects.TopGear[2].SetActive(true);
		UIManager.Objects.TopGear.Txt[0].text = "";
	}



	public void GetTurn()
	{
		StartCoroutine(Turn());
	}

	IEnumerator Turn()
	{

/* PLAYER TURN */////////////////////////////////////////////////////////////////////////////////////////////////////////////

		
		EnemyTurn = true;
		TileMaster.instance.SetFillGrid(false);
		UIManager.instance.current_class = null;
		UIManager.instance.SetClassButtons(false);
		UIManager.instance.ShowTooltip(false);

		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		yield return new WaitForSeconds(GameData.GameSpeed(0.03F));
		////CameraUtility.SetTurnOffset(false);
		yield return new WaitForSeconds(GameData.GameSpeed(0.03f));

		yield return StartCoroutine(BeforeMatchRoutine());
		bool all_of_resource = false;
		
		if(Player.instance.CompleteMatch) 
		{
			yield return StartCoroutine(MatchRoutine(PlayerControl.instance.finalTiles));
		}
		else EnemyTurn = false;

		if(TileMaster.FillGrid_Override) TileMaster.instance.SetFillGrid(true);

		yield return StartCoroutine(Player.instance.AfterMatch());

		//Player.Stats.CompleteLeech(enemies_hit);
		Player.Stats.CompleteRegen();
		Player.instance.CompleteHealth();
		Player.instance.CheckHealth();	
		
		PlayerControl.matchingTile = null;
		PlayerControl.instance.finalTiles.Clear();
		//Player.instance.CheckForBestCombo(resource);

		yield return StartCoroutine(Player.instance.EndTurn());
/* ENEMY TURN */////////////////////////////////////////////////////////////////////////////////////////////////////////////
		

		yield return StartCoroutine(TileMaster.instance.BeforeTurn());
		yield return StartCoroutine(CurrentWave.BeginTurn());
		
		if(Player.instance.Turns % (int)Player.Stats.AttackRate == 0 && TileMaster.instance.EnemiesOnScreen > 0)
		{
			yield return StartCoroutine(EnemyTurnRoutine());
		}
		yield return StartCoroutine(TileMaster.instance.AfterTurn());
		yield return StartCoroutine(Player.instance.BeginTurn());
		yield return StartCoroutine(CompleteTurnRoutine());
		TileMaster.instance.ResetTiles(true);

		yield return StartCoroutine(CurrentWave.AfterTurn());
		
		if(CurrentWave.Ended && !Player.Stats.isKilled)
		{
			GetWave();
		}
		yield return null;
	}


	IEnumerator EnemyTurnRoutine()
	{
		float per_column = 0.15F;
		List<Tile> total_attackers = new List<Tile>();
		int total_damage = 0;
		List<Tile> column_attackers;

		List<Tile> allied_attackers = new List<Tile>();
	

		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);
		yield return new WaitForSeconds(GameData.GameSpeed(0.02F));
		////CameraUtility.SetTurnOffset(true);
		yield return new WaitForSeconds(GameData.GameSpeed(0.15F));


	//ENEMY ATTACKERS
		for(int x = 0; x < TileMaster.instance.MapSize.x; x++)
		{
			column_attackers = new List<Tile>();
			for(int y = 0; y < TileMaster.instance.MapSize.y;y++)
			{
				Tile tile = TileMaster.Tiles[x,y];
				if(tile == null) continue;
				if(tile.CanAttack())
				{
					if(tile.Stats.isAlly)
					{
						allied_attackers.Add(tile);
					}
					else
					{
						
						column_attackers.Add(tile);
					}
					
				}
			}
			foreach(Tile child in column_attackers)
			{
				if(child == null) continue;

				child.SetState(TileState.Selected);
				child.OnAttack();
				total_damage += child.GetAttack();

				Vector3 pos = child.transform.position + (GameData.RandomVector*1.4F);
				MoveToPoint mini = TileMaster.instance.CreateMiniTile(pos,UIManager.instance.Health.transform, child.Info.Outer);
				mini.SetPath(0.3F, 0.25F, 0.0F, 0.1F);
				mini.SetMethod(() =>{
						Player.instance.OnHit(child);
						AudioManager.instance.PlayClipOn(Player.instance.transform, "Player", "Hit");
						GameData.Log("Took " + child.GetAttack() + "damage from " + child);
					}
				);
				yield return StartCoroutine(child.Animate("Attack", 0.05F));
			}

			total_attackers.AddRange(column_attackers);
			if(column_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(per_column));
		}
		if(total_attackers.Count > 0)
		{
			GameData.Log("Took " + total_damage + " damage from " + total_attackers.Count + " attackers");
			yield return new WaitForSeconds(GameData.GameSpeed(0.05F));
		} 


	//ALLIED ATTACKERS
		if(allied_attackers.Count > 0) 
		{
			TileMaster.instance.ResetTiles();
			yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		}
		foreach(Tile child in allied_attackers)
		{
			if(child == null) continue;

			child.SetState(TileState.Selected);
			child.OnAttack();

			Tile target = null;
			for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
			{
				for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
				{
					if(TileMaster.Tiles[x,y].Type.isEnemy)
					{
						if(TileMaster.Tiles[x,y] == null) continue;
						if(TileMaster.Tiles[x,y] == child) continue;
						if(TileMaster.Tiles[x,y].Type.isAlly) continue;
					
						target = TileMaster.Tiles[x,y];
						break;
					}
				}
				if(target!= null) break;
			}
			
			if(target != null)
			{
				Vector3 pos = child.transform.position + (GameData.RandomVector*1.4F);
				MoveToPoint mini = TileMaster.instance.CreateMiniTile(pos, target.transform, child.Info.Outer);
				mini.SetPath(0.2F, 0.5F, 0.0F, 0.1F);
				mini.SetMethod(() =>{
					if(target == null) return;
					if(target != null) AudioManager.instance.PlayClipOn(target.transform, "Enemy", "Hit");
						target.SetState(TileState.Selected);
						target.InitStats.TurnDamage += child.GetAttack();
						target.Match(0);
						GameData.Log(child +  " dealt " + child.GetAttack() + "damage to " + target);
					});
				yield return StartCoroutine(child.Animate("Attack", 0.05F));
			}
		}
		if(allied_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
	}

	public IEnumerator BeforeMatchRoutine()
	{
		List<Tile> newTiles = new List<Tile>();
		newTiles.AddRange(PlayerControl.instance.selectedTiles);
		PlayerControl.instance.selectedTiles.Clear();

		int combo_factor = 0; //Number of repeated combos made by tiles
		while(newTiles.Count > 0)
		{
			yield return StartCoroutine(Player.instance.BeforeMatch(newTiles, combo_factor==0));
			newTiles.Clear();
			newTiles.AddRange(PlayerControl.instance.selectedTiles);
			PlayerControl.instance.selectedTiles.Clear();
			combo_factor++;
			//yield return new WaitForSeconds( GameData.GameSpeed(0.1F));
		}
	}

	public IEnumerator MatchRoutine(List<Tile> tiles)
	{
		int [] resource = new int [6];
		int [] health   = new int [6];
		int [] armour   = new int [6];
		int enemies_hit = 0;

		float rate = 0.07F, num = 0;
		for(int x = 0; x < tiles.Count; x++)
		{
			Tile child = tiles[x];
			if(child == null) continue;

			int added_res = 0, added_armour = 0;

			int v = 1;
			if(child.Type.isEnemy)
			{
				v = PlayerControl.instance.AttackValue;
				enemies_hit ++;
			} 
			
			if(child.Match(v))
			{
				int [] values = child.Stats.GetValues();
				if(child.Genus == GENUS.NONE || child.Genus == GENUS.OMG) continue;
				if(child.Genus == GENUS.ALL)
				{
					for(int i = 0; i < 4; i++)
					{
						resource[i] += values[0] + added_res;
						health[i] += values[1];
						armour[i] += values[2];
					}
				}
					resource[(int)child.Genus] += values[0] + added_res;
					health[(int)child.Genus]   += values[1]; //(values[1] > 0 ? Player.Stats.Healer : 0);
					armour[(int)child.Genus]   += values[2] + added_armour;
			}

			yield return new WaitForSeconds(GameData.GameSpeed(rate));
			num++;
			ComboSize++;
			if(num % 4 == 0) rate = Mathf.Clamp(rate - 0.01F, 0.0F, 1.0F);
		}
		yield return StartCoroutine(CollectResourcesRoutine(resource, health, armour));
	}

	public IEnumerator BeginTurn()
	{
		yield return null;
	}

	public IEnumerator CompleteTurnRoutine()
	{
		//yield return StartCoroutine(Player.instance.CheckForBoonsRoutine());
		Player.instance.CompleteHealth();
		Player.instance.CheckHealth();	
		Player.instance.ResetStats();

		PlayerControl.instance.canMatch = true;
		PlayerControl.instance.isMatching = false;
		PlayerControl.instance.focusTile = null;
		PlayerControl.instance.selectedTiles.Clear();
		PlayerControl.instance.finalTiles.Clear();
		PlayerControl.matchingTile = null;

		UIManager.instance.SetClassButtons(false);
		
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		////CameraUtility.SetTurnOffset(false);

		EnemyTurn = false;
		ComboSize = 0;
		yield return null;
	}

	public void CollectResources(int [] resource, int [] health, int [] armour, Bonus [] added_bonuses = null, bool all_of_res = true)
	{
		UIManager.Objects.GetScoreWindow().Reset();
		for(int r = 0; r < resource.Length; r++)
		{
			GENUS matchGENUS = (GENUS)r;
			if(all_of_res && matchGENUS != GENUS.ALL && matchGENUS != GENUS.PRP)
			{
				for(int x = 0; x < TileMaster.Tiles.GetLength(0); x ++)
				{
					for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
					{
						Tile t = TileMaster.Tiles[x,y];
						if(t)
						{
							if(t.Genus == matchGENUS && !t.Stats.isNew) 
							{
								all_of_res = false;
							}
						}
					}
				}
			}
			if(matchGENUS == GENUS.ALL || matchGENUS == GENUS.PRP) all_of_res = false;
			

			if(resource[r] != 0 || health[r] != 0 || armour[r] != 0)
			{
				List<Bonus> bonuses = new List<Bonus>();

				if(GameData.GenusIsResource(matchGENUS))
				{
					bonuses.Add(new Bonus(Player.Classes[(int)matchGENUS].Stats.GetResourceFromGENUS(matchGENUS).ResMultiplier,
						GameData.GENUSToString(matchGENUS), 
						"Bonus from " + GameData.GENUSToString(matchGENUS) + " stat",
						GameData.instance.GetGENUSColour(matchGENUS)));
				}

				if(all_of_res) bonuses.Add(new Bonus(Player.Stats.AllColourMulti, "ALL", "Bonus for collecting all of a colour", GameData.instance.GetGENUSColour(matchGENUS)));
				
				if(PlayerControl.instance.ComboBonus > 1.0F) bonuses.Add(new Bonus(PlayerControl.instance.ComboBonus, 
					"COMBO", 
					"Combo bonus:\n+" + Player.Stats.ComboBonus + " per " + Player.Stats.ComboCounter + " tiles",
					GameData.instance.Combo));

				bonuses.AddRange(Player.instance.CheckForBonus((GENUS) r));
				if(added_bonuses != null) bonuses.AddRange(added_bonuses);	
//

//				StartCoroutine(UIManager.Objects.GetScoreWindow().AddScore(matchGENUS, Player.Classes[r], resource[r], health[r], armour[r], bonuses.ToArray()));//

//				ResourceBonus(ref resource[r], ref health[r], ref armour[r], bonuses.ToArray());		//

//				
//				if(resource[r]    != 0) {
//					Player.Classes[r].Add(resource[r]);
//					//Player.Stats.AddResourceOfGENUS(matchGENUS, resource[r]);
//					//AddTokens(resource[r]);
//				}
//				if(health[r] != 0) {
//					Player.Stats.Heal(health[r]);
//					//AddTokens(health[r]/5);
//				}
//				if(armour[r] != 0) {
//					Player.Stats.AddArmour(armour[r]);
//					//AddTokens(armour[r]);
//				}

				int final_res = resource[r];
				int final_health = health[r];
				int final_armour = armour[r];
				ResourceBonus(ref final_res, ref final_health, ref final_armour, bonuses.ToArray());	
				if(r >= 4) return;	

				if(final_res    != 0) {
					//Player.Classes[r].StartCoroutine(Player.Classes[r].AddRoutine(final_res, bonuses.ToArray()));
					//Player.Classes[r].Add(final_res);
					//Player.Stats.AddResourceOfGENUS(matchGENUS, final_res);
					//AddTokens(final_res);
				}
				if(final_health != 0) {
					//Player.Classes[r].Stats.Heal(final_health);
					//AddTokens(final_health/5);
				}
				if(final_armour != 0) {
					//Player.Classes[r].Stats.AddArmour(final_armour);
					//AddTokens(armour[r]);
				}

				StartCoroutine(UIManager.Objects.GetScoreWindow().AddScore(matchGENUS, Player.Classes[r], resource[r], health[r], armour[r], bonuses.ToArray()));

			}
		}
	}

	public IEnumerator CollectResourcesRoutine(int [] resource, int [] health, int [] armour, Bonus [] added_bonuses = null, bool all_of_res = true)
	{
		UIManager.Objects.GetScoreWindow().Reset();
		for(int r = 0; r < resource.Length; r++)
		{
			if(resource[r] != 0 || health[r] != 0 || armour[r] != 0)
			{
				GENUS matchGENUS = (GENUS)r;
				if(all_of_res && matchGENUS != GENUS.ALL && matchGENUS != GENUS.PRP)
				{
					for(int x = 0; x < TileMaster.Tiles.GetLength(0); x ++)
					{
						for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
						{
							Tile t = TileMaster.Tiles[x,y];
							if(t)
							{
								//if(t.Type.Genus == matchGENUS) print(t.Stats.isNew);
								if(t.Genus == matchGENUS && !t.Stats.isNew) 
								{
									all_of_res = false;
								}
							}
						}
					}
				}
				if(matchGENUS == GENUS.ALL || matchGENUS == GENUS.PRP) all_of_res = false;
				//print(all_of_res + ":" + matchGENUS);

			
				List<Bonus> bonuses = new List<Bonus>();

				if(GameData.GenusIsResource(matchGENUS))
				{
					bonuses.Add(new Bonus(Player.Classes[(int)matchGENUS].Stats.GetResourceFromGENUS(matchGENUS).ResMultiplier,
						GameData.GENUSToString(matchGENUS), 
						"Bonus from " + GameData.GENUSToString(matchGENUS) + " stat",
						GameData.instance.GetGENUSColour(matchGENUS)));
				}

				if(all_of_res) bonuses.Add(new Bonus(Player.Stats.AllColourMulti, "ALL", "Bonus for collecting all of a colour", GameData.instance.GetGENUSColour(matchGENUS)));
				
				if(PlayerControl.instance.ComboBonus > 1.0F) bonuses.Add(new Bonus(PlayerControl.instance.ComboBonus, 
					"COMBO", 
					"Combo bonus:\n+" + Player.Stats.ComboBonus + " per " + Player.Stats.ComboCounter + " tiles",
					GameData.instance.Combo));

				bonuses.AddRange(Player.instance.CheckForBonus((GENUS) r));
				if(added_bonuses != null) bonuses.AddRange(added_bonuses);	

				int final_res = (int) (resource[r] * GlobalManaMult);
				int final_health = (int) (health[r] * GlobalHealthMult);
				int final_armour = (int) (armour[r] * GlobalArmourMult);

				ResourceBonus(ref final_res, ref final_health, ref final_armour, bonuses.ToArray());	
				if(r >= 4) yield break;	
				Player.Classes[r].OnCombo(final_res);
				if(final_res    > 0) {

					//Player.Classes[r].StartCoroutine(Player.Classes[r].AddRoutine(final_res, bonuses.ToArray()));
					//Player.Classes[r].Add(final_res);
					//Player.Stats.AddResourceOfGENUS(matchGENUS, final_res);
					//AddTokens(final_res);
				}
				if(final_health != 0) {
					//Player.Classes[r].Stats.Heal(final_health);
					//AddTokens(final_health/5);
				}
				if(final_armour != 0) {
					//Player.Classes[r].Stats.AddArmour(final_armour);
					//AddTokens(armour[r]);
				}

				StartCoroutine(UIManager.Objects.GetScoreWindow().AddScore(matchGENUS, Player.Classes[r], resource[r], health[r], armour[r], bonuses.ToArray()));

			}
		}

		yield break;

		bool collecting_bonuses = true;
		while(collecting_bonuses)
		{
			bool complete = true;
			for(int i = 0; i < UIManager.Objects.GetScoreWindow().BonusComplete.Length; i++)
			{
				if(!UIManager.Objects.GetScoreWindow().BonusComplete[i]) complete = false;
			}
			if(complete) collecting_bonuses = false;
			yield return null;
		}
		yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
	}



	public void ResourceBonus(ref int res, ref int health, ref int armour, Bonus [] bonuses)
	{
		if(res <= 0 && health <= 0 && armour <= 0)
		{
			Debug.LogError("RES ERROR");
			return;
		}

			//StartCoroutine(UIManager.Objects.GetScoreWindow().ShowScore(res, health, armour, bonuses));
			
			for(int b = 0; b < bonuses.Length; b++)
			{
				if(bonuses[b] != null) 
				{	
					res = (int) (res * bonuses[b].Multiplier);
					health = (int) (health * bonuses[b].Multiplier);
					armour = (int) (armour * bonuses[b].Multiplier);
				}
			}
	}

	public void ToggleTileInfo()
	{
		Player.Options.ViewTileStats = !Player.Options.ViewTileStats;
	}

	public void AddOverflow(int amt)
	{
		OverflowThisTurn += amt;
	}

	public void EnemyKilled(Enemy e)
	{
		CurrentWave.EnemyKilled(e);
	}

	public void ToggleEasyGENUSe()
	{
		if(DifficultyMode == DiffMode.Okay) DifficultyMode = DiffMode.Easy;
		else DifficultyMode = DiffMode.Okay;
	}
}

[System.Serializable]
public class ClassContainer
{
	public string Name, ShortName;
	public string Description;
	public Class Prefab;

	public bool Unlocked;
	public int UnlockPoints;
	public bool ShowTute;
	public bool NoAccess;

	public int Level = 1, MaxLevel = 5;

	public bool LevelupOnWin = true;
	public int LevelUpCost{
		get
		{
			switch(Level)
			{
				case 1:
				return 2000;
				case 2:
				return 4000;
				case 3:
				return 8000;
				case 4:
				return 16000;
			}
			return 2000;
		}
	}
	public int TurnsToWin = 100;
	public DiffMode Difficulty = DiffMode.Easy;

	public AbilityContainer [] Abilities;

	public bool LevelUp(){
		if(Level >= MaxLevel) return false;
		Level = Mathf.Clamp(Level + 1, 0, MaxLevel);
		return true;
	}

	public bool LevelUpOnWin()
	{
		Level += 1;
		return true;
	}
}

[System.Serializable]
public class AbilityContainer
{
	public string Name;
	public string ShortName = "ABL";
	public string Icon;
	public string Description;
	public int Level = 1;
	public int UpgradeLevel = 1;
	public bool Unlocked;

	public float Chance = 1.0F;
	public string AbilityScript;

	public int CooldownMin = 1, CooldownMax = 1;
	public int CostMin = 0, CostMax = 0;
	public string CostType;
	public float CostChance = 0.0F;

	public string StatType;
	public int StatMultiplier = 0;
	public ContainerData [] Input;
	public ContainerData [] Output;

	public bool CostReducesCooldown = false;

	public AbilityContainer(AbilityContainer old = null)
	{
		if(old == null) return;

		Name                = old.Name;
		ShortName           = old.ShortName;
		Icon                = old.Icon;
		Description         = old.Description;
		Unlocked            = old.Unlocked;
		Level               = old.Level;
		UpgradeLevel        = old.UpgradeLevel;
		Chance              = old.Chance;
		AbilityScript       = old.AbilityScript;
		StatType            = old.StatType;
		StatMultiplier      = old.StatMultiplier;
		CooldownMin         = old.CooldownMin;
		CooldownMax         = old.CooldownMax;
		CostMin             = old.CostMin;
		CostMax             = old.CostMax;
		CostType            = old.CostType;
		CostChance          = old.CostChance;
		Input               = old.Input;
		Output              = old.Output;
		CostReducesCooldown = old.CostReducesCooldown;
	}
}

[System.Serializable]
public class ContainerData
{
	public string Title;
	public string [] args;

	public ContainerData(string [] _args)
	{
		if(_args == null) return;
		args = _args;
		if(_args.Length < 0) Title = _args[0];
	}
}

public class Bonus
{
	public float Multiplier = 1.0F;
	public string Name, Description;
	public Color col;
	public Bonus(float mult, string name, string desc, Color _col)
	{
		Name = name;
		Description = desc;
		Multiplier = mult;
		col = _col;
	}
}

