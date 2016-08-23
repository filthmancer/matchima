﻿using UnityEngine;
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
	Quick,
	None
}

public class GameManager : MonoBehaviour {
#region Variables
	public static GameManager instance;
	public static _GameSaveLoad PlayerLoader;

	public static bool debug = false;
	public static bool inStartMenu;

	public static int ComboSize;
	public static Zone Zone	{get{return instance.CurrentZone;}}
	public static int Floor	{get{return instance.CurrentFloorNum;}}
	public static int ZoneNum {get{return ZoneMap.Current + 1;}}
	public static Wave Wave{get{return instance.CurrentWave;}}
	public static ZoneMapContainer ZoneMap;

	public static Zone ZoneChoiceA, ZoneChoiceB;

	public static float GlobalManaMult = 1.0F,
						GlobalHealthMult = 1.0F,
						GlobalArmourMult = 1.0F;
	public static float GrowthRate_Easy = 0.13F,
						GrowthRate_Normal = 0.19F,
						GrowthRate_Hard = 0.30F;

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
					MeterDecayLvl1 = 0.0F,//1.12F, 
					MeterDecayLvl2 = 0.0F,//1.25F, 
					MeterDecayLvl3 = 0.0F;//1.20F;

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

	private float Difficulty_init = 1.1F;
	public static float Difficulty = 1;
	public float _Difficulty = 0;
	public DiffMode DifficultyMode = DiffMode.Okay;
	public GameMode Mode = GameMode.Story;

	public int CurrentFloorNum = 0;
	public int CurrentZoneNum = 0;
	public Zone CurrentZone;
	public Wave CurrentWave;
	public ZoneMapContainer CurrentZoneMap;


	public bool gameStart = false;
	public bool paused = false;
	public bool EnemyTurn = false;

	public long RoundTokens = 0;

	private int OverflowThisTurn = 0;
	private int OverflowMulti = 2;

	private float OverflowHitMulti = 1.0F;
	private float AllOfTypeMulti = 1.2F;

	public Zone QuickCrawlOverride;


	public bool LevelUp = false;
	
	public bool isPaused
	{
		get{
			return !gameStart || paused;
		}
	}
	public static bool TuteActive = false;


	
	public WaveGroup DefaultWaves;
	private float EnemyTurnTime = 0.0F;


	public bool _ResumeGame;
	public int ResumeZoneIndex;
	public int ResumeWaveIndex;
	public int ResumeWaveCurrent;

	[HideInInspector]
	public int ComboFactor_RepeatedCombos;
	public float ComboFactor_ComboTotalSize;
#endregion

#region Generics/Cheats
	void OnApplicationQuit()
		{
			GameData.instance.Save();
			PlayerPrefs.SetInt("PlayerLevel", Player.Level.Level);
			PlayerPrefs.SetInt("PlayerXP", Player.Level.XP_Current);
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
				PlayerPrefs.SetInt("PlayerLevel", Player.Level.Level);
				PlayerPrefs.SetInt("PlayerXP", Player.Level.XP_Current);
				if(!gameStart)  return;
				PlayerLoader.Save();
				PlayerPrefs.SetInt("Resume", gameStart ? 1 : 0);
				PlayerPrefs.SetString("Name", Player.Classes[0].Name);
				PlayerPrefs.SetInt("Turns", Player.instance.Turns);
			} 
		}
	
		void Awake()
		{
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
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 50;
			if(!Application.isMobilePlatform)
			{
				Screen.SetResolution(525,830, false);
			}
	
			paused = false;
			gameStart = false;
			_Juice = GetComponent<Juice>();
			PlayerLoader = GetComponent<_GameSaveLoad>();
			Difficulty = Difficulty_init;
	
			CurrentFloorNum = 0;
			CurrentZoneNum = 0;
			ZoneMap = CurrentZoneMap;
	
			inStartMenu = true;
			TuteActive = false;	
			StartCoroutine(GameData.instance.LoadInitialData());
		}
		
		// Update is called once per frame
		void Update () {
			_Difficulty = Difficulty;
	
			if(Input.GetKeyDown(KeyCode.Z))	Cheat(1);
			if(Input.GetKeyDown(KeyCode.X))	Cheat(2);
			if(Input.GetKeyDown(KeyCode.C)) Cheat(3);
			if(Input.GetKeyDown(KeyCode.V))	Cheat(4); 
			if(Input.GetKeyDown(KeyCode.B))	Cheat(5);
			if(Input.GetKeyDown(KeyCode.N))	Cheat(6);
			if(Input.GetKeyDown(KeyCode.M))	Cheat(7);
			if(Input.GetKeyDown(KeyCode.A))	Cheat(8);
			if(Input.GetKeyDown(KeyCode.S))	Cheat(9);
			CurrentZoneMap = ZoneMap;
			
	
			if(EnemyTurn)
			{
				if(EnemyTurnTime > 50) 
				{
					EnemyTurnTime = 0.0F;
					EnemyTurn = false;
				}
				else EnemyTurnTime += Time.deltaTime;
			}
	
	
			if(Input.GetKeyDown(KeyCode.U)) 
			{
				Player.Classes[0].AddToMeter(Player.Classes[0].MeterTop);
			}
			if(Input.GetKeyDown(KeyCode.I)) 
			{
				Player.Classes[1].AddToMeter(Player.Classes[1].MeterTop);
			}
			if(Input.GetKeyDown(KeyCode.O)) 
			{
				Player.Classes[2].AddToMeter(Player.Classes[2].MeterTop);
			}
			if(Input.GetKeyDown(KeyCode.P)) 
			{
				Player.Classes[3].AddToMeter(Player.Classes[3].MeterTop);
			}
	
			if(Input.GetKeyDown(KeyCode.H))	StartCoroutine(Player.Classes[0].Mutate(1));
			if(Input.GetKeyDown(KeyCode.J))	StartCoroutine(Player.Classes[1].Mutate(1));
			if(Input.GetKeyDown(KeyCode.K))	StartCoroutine(Player.Classes[2].Mutate(1));
			if(Input.GetKeyDown(KeyCode.L))	StartCoroutine(Player.Classes[3].Mutate(1));
	
			if(Input.GetKeyDown(KeyCode.F5)) PlayerControl.instance.focusTile.AddValue(5);
			if(Input.GetKeyDown(KeyCode.F10)) OpenInFileBrowser.Open(Application.persistentDataPath);
			if(Input.GetKeyDown(KeyCode.F12)) Player.Stats.Hit(10000);
	
			if(Input.GetKeyDown(KeyCode.Y)) paused = !paused;
	
	
			if(Player.Stats.isKilled && !isPaused)
			{
				Killed();
				return;
			}
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
				//EscapeZone();
				Wave.AddPoints(150);
				break;
				case 3: //c
				//TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["guard"], GENUS.DEX,1, 1);
				//StartCoroutine(Player.instance.AddXP(500));
				UIManager.Objects.DeathIcon.transform.position = UIManager.ClassButtons[1].transform.position + Vector3.up * 5;
				UIManager.Objects.DeathIcon.gameObject.SetActive(true);
				UIManager.Objects.DeathIcon.SetFrame(0);
				UIManager.Objects.DeathIcon.Play("PlayDeath");
				break;
				case 4: //V
				//GetTurn();
				//PlayerControl.instance.focusTile.AddEffect("Charm", 5, "2", "1");
				Player.Stats.Hit(50);
				/*foreach(Class child in Player.Classes)
				{
					child.isKilled = true;
				}*/
				break;
				case 5: //B
				PlayerControl.instance.focusTile.InitStats.TurnDamage += 1000;
				PlayerControl.instance.focusTile.Match(1000);
				break;
				case 6: //N
				Player.instance.InitStats.MapSize.y+=1;
				TileMaster.instance.IncreaseGrid(0,1);
				break;
				case 7: //M
				Player.instance.InitStats.MapSize.x+=1;
				Player.instance.ResetStats();
				break;
				case 8: //A
				PlayerPrefs.SetInt("PlayerLevel", 0);
				Player.Level.Level = 0;
				UIManager.instance.UpdatePlayerLvl();
				//CameraUtility.instance.ScreenShake(0.6F, 1.1F);
				break;
				case 9: //S
				TileMaster.instance.Ripple(TileMaster.Grid.Tiles[3,3], 1.7F, 0.5F, 0.35F);
				//TileMaster.instance.Ripple(TileMaster.Grid.Tiles[3,3]);
				break;
			}
		}
#endregion

#region Start Conditions
	public IEnumerator LoadGame(bool resume)
	{
		Class [] c = null;
		if(resume)
		{
			_ResumeGame = true;
			c = PlayerLoader.Load();
			if(c == null) yield break;
		}

		UIManager.instance.SetLoadScreen(true);
		TileMaster.instance.MapSize = new Vector2(1,1);

		Player.instance.Load(c);
		yield return StartCoroutine(UIManager.instance.LoadUI());

		yield return StartCoroutine(GameData.instance.LoadAssets_Routine());

		if(Application.isMobilePlatform)
		{
			//Player.Options.GameSpeed = 0.5F;
			//if (Time.frameCount % 60 == 0)
			//{
			//   System.GC.Collect();
			//}
		}

		UIManager.Objects.TopGear.Txt[0].text = "Touch to begin";
		bool press_start = false;
		while(!press_start)
		{
			if(Input.GetMouseButton(0)) press_start = true;
			yield return null;
		}

		gameStart = true;
		inStartMenu = false;
		Resources.UnloadUnusedAssets();

		if(_ResumeGame) 
		{
			yield return StartCoroutine(ResumeGame());
		}
		else
		{
			if(Mode == GameMode.Story) PlayStoryMode();
			else if (Mode == GameMode.Endless) PlayEndlessMode();
			else if (Mode == GameMode.Quick) PlayQuickMode();
		}
		yield return null;
	}
	public void LoadClass(ClassContainer targetClass)
	{
		StartCoroutine(UIManager.instance.LoadUI());
	}

	public IEnumerator ResumeGame()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;

		UIManager.instance.SetLoadScreen(false);
		Zone z = ZoneMap.CurrentBracket[ResumeZoneIndex];
		yield return StartCoroutine(_EnterZone(z, ResumeWaveIndex));

		yield return null;
		
		Zone.SetCurrent(ResumeWaveIndex);
		Wave.Current = ResumeWaveCurrent;

		ResumeWaveCurrent = 0;
		ResumeWaveIndex = 0;
		ResumeZoneIndex = 0;
	}


	public void PlayStoryMode()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;
		Player.Options.PowerupAlerted = false;
		UIManager.instance.SetLoadScreen(false);

		ZoneMap = GameData.instance.StoryModeMap;
		UIManager.Objects.TopGear.Txt[0].text = "";

		UIManager.instance.GenerateZoneMap();
		StartCoroutine(StartGameEnterZone());
	}

	public void PlayEndlessMode()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;
		Player.Options.PowerupAlerted = false;
		UIManager.instance.SetLoadScreen(false);

		ZoneMap = GameData.instance.GenerateEndlessMode();
		UIManager.Objects.TopGear.Txt[0].text = "";

		UIManager.instance.GenerateZoneMap();
		StartCoroutine(StartGameEnterZone());
	}

	public void PlayQuickMode()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;
		UIManager.instance.SetLoadScreen(false);
		Player.Options.PowerupAlerted = false;

		int length = 3;
		switch(DifficultyMode)
		{
			case DiffMode.Easy:
			length = 3;
			break;
			case DiffMode.Okay:
			length = 4;
			break;
			case DiffMode.Hard:
			length = 6;
			break;
		}
		Vector2 [] brackets = new Vector2[length];
		brackets[0] = new Vector2(1,1);
		for(int i = 1; i < length; i ++)
		{
			int min = (i>0 ? 2:1);
			int max = Random.Range(2,5);
			brackets[i] = new Vector2(min, max);
		}
		ZoneMap = GameData.instance.GenerateZoneMap(brackets);
		if(QuickCrawlOverride) ZoneMap[0][0] = QuickCrawlOverride;

		UIManager.Objects.TopGear.Txt[0].text = "";

		UIManager.instance.GenerateZoneMap();
		StartCoroutine(StartGameEnterZone());
	}

	IEnumerator StartGameEnterZone()
	{
		while(GameManager.instance.paused) yield return null;
		StartCoroutine(_EnterZone(ZoneMap.CurrentBracket[0], 0));
	}
#endregion

#region End Conditions/Reset
	public IEnumerator Reset()
	{
		yield return null;
		CurrentZone = null;
		CurrentWave = null;
		CurrentFloorNum = 0;
		gameStart = false;


		Player.instance.Reset();
		TileMaster.instance.Reset();
		yield return StartCoroutine(UIManager.instance.Reset());
		paused = false;
		
	}


	public End_Type EndType;
	public void Killed()
	{
		EndType = End_Type.Defeat;
		paused = true;
		gameStart = false;
		StartCoroutine(EndGame(EndType));
	}

	public void Victory()
	{
		EndType = End_Type.Victory;
		paused = true;
		gameStart = false;
		StartCoroutine(EndGame(EndType));
	}

	public void Retire()
	{
		EndType = End_Type.Retire;
		UIManager.instance.ShowOptions();
		Player.Stats.isKilled = true;
		Player.instance.retired = true;
		paused = true;
		StartCoroutine(EndGame(EndType));
	}

	public void SaveAndQuit()
	{
		EndType = End_Type.SaveQuit;
		Player.instance.Reset();
		GameManager.instance.gameStart = false;
		GameData.instance.Save();
		Application.LoadLevel(0);
	}

	IEnumerator EndGame(End_Type e)
	{

		int [] xp = CalculateXP();
		yield return new WaitForSeconds(0.1F);
		TileMaster.instance.ClearGrid(true);

		yield return new WaitForSeconds(0.3F);
		UIManager.instance.ShowKillUI(e, xp);
	}

	int [] CalculateXP()
	{
		int turns = Player.instance.Turns;
		if(turns == 0) turns = 1;

		int turns_per_floor = 10;

		int [] final = new int [2];
		final[0] = GameManager.Floor * 10;

		final[1] = GameManager.Floor * turns_per_floor;
		final[1] = Mathf.Clamp(final[1]/turns, 1, 100);
		final[1] *= final[0];
		return final;
	}
#endregion

#region Waves/Zones
	public void AdvanceZoneMap(int choice)
		{
			EnterZone(ZoneMap.CurrentBracket[choice]);		
		}
	
		public void EnterZone(Zone z = null, string name = null)
		{
			StartCoroutine(_EnterZone(z, name, null));
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
				target = GameData.instance.GetZone(name);
			}
			else if(ZoneMap != null)
			{
				target = ZoneMap[0][0];
			}
			else target = GameData.instance.GetZoneRandom();
	
			if(target == null) yield break;
			CurrentZone = (Zone) Instantiate(target);
			CurrentZone.transform.parent = this.transform;
			if(CurrentWave != null) 
			{
				Destroy(CurrentWave.gameObject);
				CurrentWave = null;
			}

			yield return StartCoroutine(CurrentZone.Enter());
			yield return StartCoroutine(_GetWave(w));
			
		}

		IEnumerator _EnterZone(Zone z, int wavenum)
		{
			if(CurrentZone != null) Destroy(CurrentZone.gameObject);

			CurrentZone = (Zone) Instantiate(z);
			CurrentZone.transform.parent = this.transform;
			
			if(CurrentWave != null) 
			{
				Destroy(CurrentWave.gameObject);
				CurrentWave = null;
			}
			print("ENTERING ZONE");
			yield return StartCoroutine(CurrentZone.Enter());
			CurrentZone.SetCurrent(wavenum);
			yield return StartCoroutine(_GetWave());
		}
	
		public void EscapeZone()
		{
			bool end = ZoneMap.Progress();
			if(end) UIManager.instance.ShowZoneUI(true);		
			else Victory();
		}
	
		public void GetWave(Wave w = null)
		{
			CurrentFloorNum ++;
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
	
			for(int i = 0; i < GameManager.Wave.Length; i++)
			{
				if(GameManager.Wave[i] != null && GameManager.Wave[i].Active && UIManager.Objects.TopGear[1].Length > i)
				{
					UIObj wavebutton = UIManager.WaveButtons[i];
					wavebutton.SetActive(true);
					wavebutton.Txt[0].text = "";	
					wavebutton.Img[1].transform.gameObject.SetActive(true);
					wavebutton.Img[1].enabled = true;	
					wavebutton.Img[0].enabled = true;
					//wavebutton.Img[0].sprite = GameManager.Wave[i].Inner;
					wavebutton.Img[0].color = Color.white;
					wavebutton.Img[2].enabled = true;
					//wavebutton.Img[2].sprite = GameManager.Wave[i].Outer;
					wavebutton.Img[2].color = Color.white;				
				}
				else 
				{
					UIManager.WaveButtons[i].SetActive(false);
				}
			}
			StartCoroutine(CurrentWave.Setup());
	
			Difficulty += Mathf.Exp(Difficulty_Growth);
			UIManager.Objects.TopRightButton.Txt[0].text = "" + GameManager.Floor;
			UIManager.Objects.TopRightButton.Txt[1].text = "" + GameManager.ZoneNum;
	
			CameraUtility.instance.MainLight.color = Color.Lerp(
				CameraUtility.instance.MainLight.color, UIManager.instance.BackingTint, Time.deltaTime * 5);
			UIManager.Objects.Walls.color = Color.Lerp(
				UIManager.Objects.Walls.color, UIManager.instance.WallTint, Time.deltaTime * 5);
		}
	
		IEnumerator _GetWave(Wave w = null)
		{
			CurrentFloorNum ++;
			if(w == null)
			{

				w = Zone.CheckZone();
			}
	
			if(CurrentWave != null && CurrentWave != w) Destroy(CurrentWave.gameObject);
			if(w == null)
			{
				EscapeZone();
				yield break;
			}
			CurrentWave = Instantiate(w);
			CurrentWave.transform.parent = this.transform;
			yield return StartCoroutine(CurrentWave.Setup());
			Difficulty += Mathf.Exp(Difficulty_Growth);
		}

		bool shown_trial;
		public IEnumerator CheckForTute()
		{
			if(CurrentWave.Name == GameData.instance.StoryModeMap[0][0].Name && !shown_trial)
			{
				shown_trial = true;
				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Mana Spells", "Complete the Mana Trial to cast a spell", "", true, 60));
			}
			yield return null;
		}
#endregion

#region Turn Loop
	public void GetTurn()
	{
		StartCoroutine(Turn());
	}

	IEnumerator Turn()
	{

	/* PLAYER TURN *///////////////////////////////////////////////////
		EnemyTurn = true;
		
		TileMaster.instance.SetFillGrid(false);
		UIManager.instance.current_class = null;
		UIManager.instance.SetClassButtons(false);
		UIManager.instance.ShowGearTooltip(false);
		
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, false);
		//UIManager.Objects.BotGear.SetTween(3, true);
		UIManager.instance.MoveTopGear(0);

		yield return StartCoroutine(BeforeMatchRoutine());
		bool all_of_resource = false;
		
		if(Player.instance.CompleteMatch) 
		{
			yield return StartCoroutine(MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		}
		else EnemyTurn = false;


		yield return StartCoroutine(Player.instance.AfterMatch());
		
		

		yield return StartCoroutine(Player.instance.EndTurn());
		//UIManager.Objects.BotGear.SetTween(3, false);
		yield return StartCoroutine(TileMaster.instance.BeforeTurn());
	/* ENEMY TURN *////////////////////////////////////////////////////
		yield return StartCoroutine(CurrentWave.BeginTurn());
		if(Player.instance.Turns % (int)Player.Stats.AttackRate == 0 && TileMaster.instance.EnemiesOnScreen > 0)
		{
			yield return StartCoroutine(EnemyTurnRoutine());
		}

		TileMaster.instance.ResetTiles(true);
		while(TileMaster.EnemiesAttacking()) yield return null;
		
		yield return StartCoroutine(TileMaster.instance.AfterTurn());
		yield return StartCoroutine(CompleteTurnRoutine());
		yield return StartCoroutine(Player.instance.CheckHealth());	
		yield return StartCoroutine(CurrentWave.AfterTurn());

		if(CurrentWave.Ended && !Player.Stats.isKilled)
		{
			yield return StartCoroutine(_GetWave());
		}
		
		UIManager.Objects.BotGear.SetTween(0, true);
		yield return StartCoroutine(Player.instance.BeginTurn());

		foreach(Class child in Player.Classes)
		{
			if(child == null) continue;
			if(child.MeterLvl > 0 && !Player.Options.PowerupAlerted)
			{
				Player.Options.PowerupAlerted = true;

				//UIManager.instance.ShowTuteAlert("A HERO HAS POWERED UP!\nTOUCH THE HERO'S ICON TO CAST A SPELL");
				break;
			}
		}
		
		TileMaster.instance.ResetTiles(true);

		UIManager.instance.ResetTopGear();
		yield return null;
	}


	IEnumerator EnemyTurnRoutine()
	{
		float per_column = 0.06F;
		List<Tile> total_attackers = new List<Tile>();
		int total_damage = 0;
		List<Tile> column_attackers;

		List<Tile> allied_attackers = new List<Tile>();
	
		yield return new WaitForSeconds(GameData.GameSpeed(0.08F));
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);

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
					tile.AttackedThisTurn = true;
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

				child.AttackPlayer();
				yield return StartCoroutine(child.Animate("Attack", 0.03F));
			}

			total_attackers.AddRange(column_attackers);
			if(column_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(per_column));
		}
		if(total_attackers.Count > 0)
		{
			GameData.Log("Took " + total_damage + " damage from " + total_attackers.Count + " attackers");
			yield return new WaitForSeconds(GameData.GameSpeed(0.08F));
		} 


		//ALLIED ATTACKERS
		if(allied_attackers.Count > 0) 
		{
			TileMaster.instance.ResetTiles();
			yield return new WaitForSeconds(GameData.GameSpeed(0.13F));
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
				child.AttackTile(target);
				yield return StartCoroutine(child.Animate("Attack", 0.05F));
			}
		}
		if(allied_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
	}
#endregion

#region Match Loops, Routines, Bonuses
	public IEnumerator BeforeMatchRoutine()
	{
		List<Tile> newTiles = new List<Tile>();
		newTiles.AddRange(PlayerControl.instance.selectedTiles);
		PlayerControl.instance.selectedTiles.Clear();
		ComboFactor_RepeatedCombos = 1; //Number of repeated combos made by tiles
		
		while(newTiles.Count > 0)
		{
			PlayerControl.instance.AddTilesToFinal(newTiles.ToArray());
			for (int i = 0; i < newTiles.Count; i++)
			{
				if (newTiles[i] == null) continue;
				if (newTiles[i].BeforeMatchEffect) yield return StartCoroutine(newTiles[i].BeforeMatch(false));
			}

			yield return StartCoroutine(Player.instance.BeforeMatch(newTiles));
			yield return null;
			newTiles.Clear();
			newTiles.AddRange(PlayerControl.instance.selectedTiles);
			PlayerControl.instance.selectedTiles.Clear();
			ComboFactor_RepeatedCombos++;
			//yield return new WaitForSeconds( GameData.GameSpeed(0.1F));
		}
	}

	public IEnumerator MatchRoutine(params Tile [] tiles)
	{
		int [] resource = new int [6];
		int [] health   = new int [6];
		int [] armour   = new int [6];
		int enemies_hit = 0;
		

		float rate = 0.07F, num = 0;

		for(int x = 0; x < tiles.Length; x++)
		{
			Tile child = tiles[x];
			if(child == null) continue;

			int added_res = 0, added_armour = 0;

			int v = 1;
			if(child.Type.isEnemy)
			{
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
					health[(int)child.Genus]   += values[1];
					armour[(int)child.Genus]   += values[2] + added_armour;
			}

			yield return new WaitForSeconds(GameData.GameSpeed(rate));
			num++;
			ComboSize++;
			
			if(num % 4 == 0) rate = Mathf.Clamp(rate - 0.01F, 0.0F, 1.0F);
		}
		 
		yield return StartCoroutine(CollectResourcesRoutine(resource, health, armour));
		
	}

	public IEnumerator CompleteTurnRoutine()
	{
		UIManager.instance.SetClassButtons(false);
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);

		/*UIManager.instance.SetBonuses(GetBonuses(ComboSize));
		UIManager.instance.StartTimer();
		while(UIManager.instance.IsShowingMeters) yield return null;*/

		//yield return StartCoroutine(Player.instance.CheckForBoonsRoutine());
		Player.instance.CompleteHealth();
		
		Player.instance.ResetStats();

		PlayerControl.instance.canMatch = true;
		PlayerControl.instance.isMatching = false;
		PlayerControl.instance.focusTile = null;
		PlayerControl.instance.selectedTiles.Clear();
		PlayerControl.instance.finalTiles.Clear();
		PlayerControl.matchingTile = null;

		////CameraUtility.SetTurnOffset(false);

		EnemyTurn = false;
		ComboSize = 0;
		yield return null;
	}

	public Bonus [] GetBonuses(int combo)
	{
	 	List<Bonus> final = new List<Bonus>();

	//Combo Bonus
	 	float multi = 1.0F;
	 	string title = "";
	 	Color col = GameData.Colour(GENUS.STR);
	 	if(combo >= 20)
	 	{
	 		title = "WOW!";
	 		multi = 2.2F;
	 		col = GameData.Colour(GENUS.PRP);
	 	}
	 	else if(combo >= 12)
	 	{
	 		title = "GREAT!";
	 		multi = 1.5F;
	 		col = GameData.Colour(GENUS.DEX);
	 	}
	 	else if(combo >= 6)
	 	{
	 		title = "GOOD!";
	 		multi = 1.2F;
	 		col = GameData.Colour(GENUS.WIS);
	 	}
	 	if(multi > 1.0F)
	 		final.Add(new Bonus(multi,title,multi.ToString("0.0") + "x", col));

		if(!Zone.isNew)
		{
			for(int i = 0; i < Player.Classes.Length; i++)
			{
				if(AllOfRes(i))
				{
					final.Add(new Bonus(
						Player.Stats.AllColourMulti,
						"ALL " + GameData.Resource(Player.Classes[i].Genus),
						Player.Stats.AllColourMulti.ToString("0.0") + "x",
						GameData.Colour((GENUS)i), i));
				}
				final.AddRange(Player.instance.CheckForBonus((GENUS) i));
			}
		}
		Zone.isNew = false;
	 	

	 	return final.ToArray();
	 	
	}

	public IEnumerator CollectResourcesRoutine(int [] resource, int [] health, int [] armour, Bonus [] added_bonuses = null, bool all_of_res = true)
	{
		UIManager.Objects.GetScoreWindow().Reset();
		
		for(int r = 0; r < resource.Length; r++)
		{
			if(resource[r] != 0 || health[r] != 0 || armour[r] != 0)
			{
				List<Bonus> bonuses = new List<Bonus>();

				/*if(GameData.GenusIsResource(matchGENUS))
				{
					bonuses.Add(new Bonus(Player.Classes[(int)matchGENUS].Stats.GetResourceFromGENUS(matchGENUS).ResMultiplier,
						GameData.GENUSToString(matchGENUS), 
						"Bonus from " + GameData.GENUSToString(matchGENUS) + " stat",
						GameData.instance.GetGENUSColour(matchGENUS)));
				}

				if(all_of_res) bonuses.Add(new Bonus(Player.Stats.AllColourMulti, "ALL", "Bonus for collecting all of a colour", GameData.instance.GetGENUSColour(matchGENUS)));*/
				
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


	public IEnumerator SplashBonus(int num)
	{
	//Combo Size Bonus Points
			float info_size = 120;
			string title = "";
			Color col = GameData.Colour(GENUS.STR);
			if(num >= 20)
			{
				title = "WOW!";
				info_size = 190;
				col = GameData.Colour(GENUS.PRP);
			}
			else if(num >= 10)
			{
				title = "GREAT!";
				info_size = 150;
				col = GameData.Colour(GENUS.DEX);
			}
			else if(num >= 5)
			{
				title = "GOOD!";
				info_size = 120;
				col = GameData.Colour(GENUS.WIS);
			}
			else yield break;

			float init_rotation = Random.Range(-4,4);
			float info_time = 0.95F;
			
			float info_movespeed = 0.27F;
			float info_finalscale = 0.5F;

			//Player.OnCombo(ComboSize);
			Vector3 pos = UIManager.Objects.BotGear.Img[2].transform.position + Vector3.down * 0.6F;
			pos += Utility.RandomVectorInclusive(1,0,0);
			MiniAlertUI m = UIManager.instance.MiniAlert(pos, title, info_size,col, info_time, 0.03F, false);

			m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
			//MoveToPoint mini = m.GetComponent<MoveToPoint>();
			m.AddJuice(Juice.instance.BounceB, info_time);
			//m.AddAction(() => {mini.enabled = true;});
			//m.DestroyOnEnd = false;

			//mini.SetTarget(UIManager.ClassButtons[(int)g].transform.position);
			//mini.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
			//mini.SetMethod(() =>{
			//		Player.Classes[(int)g].AddToMeter(5);
			//	}
			//);
		yield return new WaitForSeconds(GameData.GameSpeed(0.9F));


	//All of Colour Bonus
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			/*bool all_of_res = true;
			GENUS matchGENUS = Player.Classes[i].Genus;
			if(matchGENUS != GENUS.ALL)
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

			if(all_of_res)
			{
				init_rotation = Random.Range(-4,4);
				title = "ALL " + GameData.ResourceLong(matchGENUS) + "!";
				info_size = 130;
				col = GameData.Colour(matchGENUS);

				pos = UIManager.Objects.BotGear.Img[2].transform.position + Vector3.down * 0.6F;
				pos += (Vector3.left * 2) + (Vector3.right * i);
				m = UIManager.instance.MiniAlert(pos, title, info_size,col, info_time, 0.03F, false);

				m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
				m.AddJuice(Juice.instance.BounceB, info_time);

				//MoveToPoint mini = m.GetComponent<MoveToPoint>();
				//m.AddAction(() => {mini.enabled = true;});
				//m.DestroyOnEnd = false;
				//mini.SetTarget(UIManager.ClassButtons[i].transform.position);
				//mini.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
				//mini.SetMethod(() =>{
				//		Player.Classes[i].AddToMeter(5);
				//	}
				//);
				yield return new WaitForSeconds(GameData.GameSpeed(0.6F));
			}*/
			//yield return StartCoroutine(AllOfResRoutine(i));	
		}
	}

	public bool AllOfRes(int i)
	{
		if(Player.Classes[i] == null) return false;
		bool all_of_res = true;
		GENUS matchGENUS = Player.Classes[i].Genus;
		if(matchGENUS != GENUS.ALL)
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
		return all_of_res;
	}

	IEnumerator AllOfResRoutine(int i)
	{
		float info_size = 120;
		string title = "";
		Color col = GameData.Colour(GENUS.STR);
		float init_rotation = Random.Range(-4,4);
		float info_time = 0.95F;
		
		float info_movespeed = 0.27F;
		float info_finalscale = 0.5F;

		bool all_of_res = true;
		GENUS matchGENUS = Player.Classes[i].Genus;
		if(matchGENUS != GENUS.ALL)
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

		if(all_of_res)
		{
			init_rotation = Random.Range(-4,4);
			title = "ALL " + GameData.ResourceLong(matchGENUS) + "!";
			col = GameData.Colour(matchGENUS);

			Vector3 pos = UIManager.Objects.BotGear.Img[2].transform.position + Vector3.down * 0.6F;
			pos += (Vector3.left * 2) + (Vector3.right * i);
			MiniAlertUI m = UIManager.instance.MiniAlert(pos, title, info_size,col, info_time, 0.03F, false);

			m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
			m.AddJuice(Juice.instance.BounceB, info_time);

			MoveToPoint mini = m.GetComponent<MoveToPoint>();
			m.AddAction(() => {mini.enabled = true;});
			m.DestroyOnEnd = false;
			mini.SetTarget(UIManager.ClassButtons[i].transform.position);
			mini.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
			mini.SetMethod(() =>{
					Player.Classes[i].AddToMeter(5);
				}
			);
			yield return new WaitForSeconds(GameData.GameSpeed(0.6F));
		}
	}
#endregion

	public void ToggleTileInfo()
	{
		Player.Options.ViewTileStats = !Player.Options.ViewTileStats;
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

public enum End_Type
{
	Defeat, Victory, Retire, SaveQuit
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
	public int index;
	public Bonus(float mult, string name, string desc, Color _col, int _index = 5)
	{
		Name = name;
		Description = desc;
		Multiplier = mult;
		col = _col;
		index = _index;
	}
}

