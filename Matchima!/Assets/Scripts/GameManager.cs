using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;

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
	public Scumworks Scum;

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
						GrowthRate_Hard = 0.26F;

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
	public Zone EndlessMode;


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
	public int ResumeGameHealth;
	public int [] ResumeGameMeter;
	public int ResumeZoneIndex;
	public int ResumeWaveIndex;
	public int ResumeZoneCurrent;
	public int ResumeWaveCurrent;
	public string ResumeWaveName;
	public bool ResumeWaveHasEntered;

	[HideInInspector]
	public int ComboFactor_RepeatedCombos;
	public float ComboFactor_ComboTotalSize;
#endregion

#region Generics/Cheats
	void OnApplicationQuit()
		{
			//GameData.instance.Save();
			//PlayerPrefs.SetInt("PlayerLevel", Player.Level.Level);
			//PlayerPrefs.SetInt("PlayerXP", Player.Level.XP_Current);
			if(gameStart) 
			{
				PlayerLoader.Save();
				PlayerPrefs.SetInt("Resume", gameStart ? 1 : 0);
				PlayerPrefs.SetString("Name", Player.Classes[0].Name);
				PlayerPrefs.SetInt("Turns", Player.instance.Turns);
			}
		}
	
		void OnApplicationPause()
		{
			//GameData.instance.Save();
			//PlayerPrefs.SetInt("PlayerLevel", Player.Level.Level);
			//PlayerPrefs.SetInt("PlayerXP", Player.Level.XP_Current);
			if(gameStart)
			{
				PlayerLoader.Save();
				PlayerPrefs.SetInt("Resume", gameStart ? 1 : 0);
				PlayerPrefs.SetString("Name", Player.Classes[0].Name);
				PlayerPrefs.SetInt("Turns", Player.instance.Turns);
			}
			PlayerPrefs.Save();
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
			
			if(!Application.isMobilePlatform)
			{
				Screen.SetResolution(525,830, false);
				Application.targetFrameRate = 60;
			}
			else 
			{
				Application.targetFrameRate = 45;
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
				TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["lightning"], GENUS.ALL,1, 1);
				//Player.instance.ResetLevel();
				
				/*UIManager.Objects.DeathIcon.transform.position = UIManager.ClassButtons[1].transform.position + Vector3.up * 5;
				UIManager.Objects.DeathIcon.gameObject.SetActive(true);
				UIManager.Objects.DeathIcon.SetFrame(0);
				UIManager.Objects.DeathIcon.Play("PlayDeath");*/
				break;
				case 4: //V
				
				//GetTurn();
				//PlayerControl.instance.focusTile.AddEffect("Charm", 5, "2", "1");
				Player.Stats.Hit(50);
				foreach(Class child in Player.Classes)
				{
					child.isKilled = true;
				}
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
				//Juice.instance.JuiceIt(Juice.instance.LandFromAbove, TileMaster.Tiles[0,1].transform, 0.6F, 1.0F);
				StartCoroutine(Player.instance.AddXP(500));
				//UIManager.instance.UpdatePlayerLvl();
				//StartCoroutine(Player.instance.AddXP(5000));
				//CameraUtility.instance.ScreenShake(0.6F, 1.1F);
				break;
				case 9: //S
				TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["arcane"], GENUS.ALL,1, 1);
				//TileMaster.instance.Ripple(TileMaster.Grid.Tiles[3,3]);
				break;
			}
		}
#endregion

#region Start Conditions
	public IEnumerator LoadGame(bool resume, bool show_starter = false)
	{
		Class [] c = null;
		if(resume)
		{
			_ResumeGame = true;
			c = PlayerLoader.Load();
			if(c == null) yield break;
		}
		inStartMenu = false;

		UIManager.instance.SetLoadScreen(true);
		
		if(show_starter) StartCoroutine(ShowStarterAd());
		TileMaster.instance.MapSize = new Vector2(1,1);
		Player.instance.Load(c);
		yield return new WaitForSeconds(0.1F);
		AudioManager.instance.GetZoneMusic();
		yield return StartCoroutine(UIManager.instance.LoadGameUI());
		yield return StartCoroutine(GameData.instance.LoadAssets_Routine());

		/*while(!Advertisement.IsReady())
	  	{
	  		yield return null;
	  	}*/

		(UIManager.instance.AdAlertMini as UIObjTweener).SetTween(0, false);
		(UIManager.instance.AdAlertMini[0] as UIObjTweener).SetTween(0, false);
		//(UIManager.instance.AdAlert[0] as UIObjTweener).SetTween(0, false);
		yield return new WaitForSeconds(0.1F);
		UIManager.Objects.TopGear.Txt[0].text = "Touch to begin";
		
		bool press_start = false;
		while(!press_start)
		{
			if(Input.GetMouseButton(0)) press_start = true;
			yield return null;
		}

		gameStart = true;
		
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

	IEnumerator ShowStarterAd()
	{
		UIObjTweener alerter = UIManager.instance.AdAlertMini as UIObjTweener;
		alerter.SetTween(0, false);

		UIObjTweener adflash = alerter[0] as UIObjTweener;
		adflash.SetTween(0,false);
		while(!Advertisement.IsReady())
	  	{
	  		yield return null;
	  	}

	  	yield return new WaitForSeconds(1.65F);

	  	alerter.Txt[0].text = "Watch an ad for 3 Bonus Tiles?";
	  //	alerter.SetActive(true);
	 	 alerter.SetTween(0, true);
		
		
		adflash.ClearActions();
		adflash.AddAction(UIAction.MouseUp, () => {
			Scum.ShowStarterAd();
			//(alerter[0] as UIObjTweener).SetTween(0, false);
			(UIManager.instance.AdAlertMini as UIObjTweener).SetTween(0, false);
			(UIManager.instance.AdAlertMini[0] as UIObjTweener).SetTween(0, false);
			});
		yield return StartCoroutine(GameData.DeltaWait(0.95F));
		adflash.SetTween(0, true);
		yield return null;
	}

	public IEnumerator ResumeGame()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;

		UIManager.instance.GenerateZoneMap();
		UIManager.instance.SetLoadScreen(false);
		Zone z = ZoneMap.CurrentBracket[ResumeZoneIndex];
		yield return StartCoroutine(_EnterZone(z, ResumeWaveIndex, ResumeWaveName));

		yield return null;
		
		Zone.SetCurrent(ResumeZoneCurrent);
		Wave.Current = ResumeWaveCurrent;
		Wave.HasEntered = ResumeWaveHasEntered;

		Player.Stats._Health = ResumeGameHealth;

		for(int i = 0; i < Player.Classes.Length; i++)
		{
			Player.Classes[i].AddToMeter(ResumeGameMeter[i]);
		}
		ResumeGameMeter = new int [0];
		ResumeGameHealth = 0;
		ResumeWaveCurrent = 0;
		ResumeWaveIndex = 0;
		ResumeZoneCurrent = 0;
		ResumeZoneIndex = 0;
		ResumeWaveName = "";
	}

	public void ResetFactors()
	{
		Difficulty = (DifficultyMode == DiffMode.Easy ? 1.1F : 2.1F);
		GameData.ChestsFromEnemies = true;
		Player.instance.Turns = 0;
	}

	public void PlayStoryMode()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;
		Player.Options.PowerupAlerted = false;
		UIManager.instance.SetLoadScreen(false);

		ZoneMap = GameData.instance.StoryModeMap;


		
		UIManager.instance.GenerateZoneMap();
		StartCoroutine(StartGameEnterZone());
	}

	public void PlayEndlessMode()
	{
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;
		Player.Options.PowerupAlerted = false;
		UIManager.instance.SetLoadScreen(false);

		ZoneMap = GameData.instance.EndlessModeMap;
		
	
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
			length = 4;
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

	

		UIManager.instance.GenerateZoneMap();
		StartCoroutine(StartGameEnterZone());
	}

	private bool QueueReward;
	private int QueueRewardNum = 3;
	public void SetQueueReward(bool active, int num)
	{
		QueueReward = active;
		QueueRewardNum = num;
	}
	IEnumerator StartGameEnterZone()
	{
		UIManager.instance.SetHealthNotifier(true);
		UIManager.Objects.TopGear.Txt[0].text = "";
		ResetFactors();

		while(GameManager.instance.paused) yield return null;
		yield return StartCoroutine(_EnterZone(ZoneMap.CurrentBracket[0], 0));
		
		ShownEndAd = false;
		if(QueueReward)
		{
			List<Tile> targs = new List<Tile>();
			for(int i = 0; i < QueueRewardNum; i++)
			{
				Tile t = TileMaster.RandomTile;
				while(targs.Contains(t)) t = TileMaster.RandomTile;
				targs.Add(t);

				CastReward(t);
			}
		}
	}

	private void CastReward(Tile t)
	{
		GameData.instance.ActionCaster(UIManager.Objects.TopGear.transform, t, () => {
					TileMaster.instance.ReplaceTile(t, TileMaster.Types["chest"], GENUS.RAND,1, 5);
				});
	}
#endregion

#region End Conditions/Reset
	public IEnumerator Reset(bool finished_game)
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
		
		if(GameData.FullVersion)
		{
			ChoiceComplete comp = new ChoiceComplete(2);
			UIManager.Objects.MiddleGear.SetActive(false);
			StartCoroutine(UIManager.instance.Alert(0.2F, "Like the game?", "Support us by\nbuying the full version!", "", true, 70, null, comp));
			while(!comp.Result) yield return null;
			UIManager.Objects.MiddleGear.SetActive(true);
			if(comp.ChoicePicked == 0) 
			{
				UIManager.Objects.TopGear.MoveToDivision(1);
				UIManager.Menu.GetMiddleGearInfo(1);
			}
			
		}
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
		UIManager.instance.ShowOptions();
		StartCoroutine(RetireRoutine());
	}

	IEnumerator RetireRoutine()
	{
		bool? retire_actual = null;
		ChoiceComplete comp = new ChoiceComplete(2);
		StartCoroutine(UIManager.instance.Alert(0.2F, null, new StCon[]{new StCon("Are you sure you\nwant to retire?")}, null,
												true, null, comp));

		while(!retire_actual.HasValue) 
		{	
			if(comp.Result) retire_actual = comp.ChoicePicked == 0 ? true : false;
			yield return null;
		}
		if(!retire_actual.Value) yield break;
		EndType = End_Type.Retire;
		//UIManager.instance.ShowOptions();
		//Player.Stats.isKilled = true;
		Player.instance.retired = true;
		paused = true;
		yield return StartCoroutine(EndGame(EndType));
	}

	public void SaveAndQuit()
	{
		EndType = End_Type.SaveQuit;
		Player.instance.Reset();
		GameManager.instance.gameStart = false;
		GameData.instance.Save();
		Application.LoadLevel(0);
	}

	public bool ShownEndAd = false;
	private int XP_total = 0;
	IEnumerator EndGame(End_Type e)
	{
		bool enderad = (e == End_Type.Defeat);
		if(enderad && !Player.Stats.isKilled) yield break;
		UIManager.instance.CloseZoneUI();
		
		int [] xp = CalculateXP(e);
		XP_total = xp[2];

		yield return new WaitForSeconds(0.25F);
		yield return StartCoroutine(UIManager.instance.ShowKillUI(e, xp));
		if(enderad && !ShownEndAd)
		{
			ShownEndAd = true;
			yield return StartCoroutine(ShowEnderAd2());
		}
	}



	public IEnumerator ExitGame()
	{
		TileMaster.instance.ClearGrid(true);
		if(Wave) Wave.OnWaveDestroy();

		yield return StartCoroutine(Player.instance.AddXP(XP_total));
		XP_total = 0;
		yield return StartCoroutine(Reset(true));
	}

	int [] CalculateXP(End_Type e)
	{
		int xp_diff_rate = 4;
		int xp_depth_rate = 20;
		int xp_turns_rate = 5;

		int avg_turns_per_floor = 10;

		int difficulty = (int) (Difficulty * (int) DifficultyMode);

		int depth = GameManager.Floor;
		int turns = Player.instance.Turns;
		if(turns == 0) turns = 1;
		
		//XP multiplier from ending type
		float multi = 1.0F;
			switch(e)
			{
				case End_Type.Victory:
				multi = 2.0F;
				break;
				case End_Type.Defeat:
				multi = 1.7F;
				break;
				case End_Type.Retire:
				multi = 1.0F;
				break;
			}

		int [] final = new int[3];
	//The base XP amount, taken from difficulty
		final[0] = (int) (difficulty * xp_diff_rate * multi);


	//XP from depth
		float floorxp = GameManager.Floor * xp_depth_rate * multi;
		final[1] = final[0] + (int)floorxp;

	//XP gained from position on average turns scale
		int avg_turns_xp = GameManager.Floor * avg_turns_per_floor;
		int actual_turns_xp = GameManager.Floor * Player.instance.Turns;

	//Caluculate turn xp by subtracting the actual turn number from the average turn number at that depth
		int final_turns_xp = Mathf.Clamp(avg_turns_xp-actual_turns_xp, 0, 100);
		float turnxp = final_turns_xp * xp_turns_rate * multi;
		final[2] = final[1] + (int)turnxp;

		return final;
	}

	IEnumerator ShowEnderAd2()
	{
		EnderAd_showing = false;
		UIObjTweener alerter = UIManager.instance.AdAlertMini as UIObjTweener;
		alerter.SetTween(0, false);

		UIObjTweener adflash = alerter[0] as UIObjTweener;
		adflash.SetTween(0,false);
		while(!Advertisement.IsReady())
		{
			yield return null;
		}

		alerter.Txt[0].text = "Watch an ad\nto cheat death?";
		alerter.SetTween(0, true);
		
		yield return StartCoroutine(GameData.DeltaWait(0.85F));
		
		adflash.ClearActions();
		adflash.AddAction(UIAction.MouseUp, () => {
			Scum.ShowEnderAd();
			EnderAd_showing = true;
			(UIManager.instance.AdAlertMini as UIObjTweener).SetTween(0, false);
			(UIManager.instance.AdAlertMini[0] as UIObjTweener).SetTween(0, false);
			});
		adflash.SetTween(0, true);
		yield return null;
	}

	
	IEnumerator ShowEnderAd()
	{
		EnderAd_showing = false;
		UIObjTweener adflash = UIManager.instance.AdAlert[0] as UIObjTweener;
		adflash.SetTween(0, false);

		yield return new WaitForSeconds(0.2F);
		while(!Advertisement.IsReady())
	  	{
	  		yield return null;
	  	}
		
	  	float EndTimer = 3.4F;

		adflash.ClearActions();
		adflash.AddAction(UIAction.MouseUp, () => {
			Scum.ShowEnderAd();
			EnderAd_showing = true;
			//(UIManager.Objects.MiddleGear[0][4] as UIObjTweener).SetTween(0, false);
			//UIManager.Objects.TopGear.Txt[0].text = "Loading";
			UIManager.instance.AdAlert.SetActive(false);
			});
		
		
		UIManager.instance.AdAlert.Txt[0].text = "Watch an ad\nto cheat death?";
		UIManager.instance.AdAlert.SetActive(true);

		yield return StartCoroutine(GameData.DeltaWait(0.85F));
		adflash.SetTween(0, true);
		adflash.Txt[0].text = "OK";
		int EndTimer_curr = (int) EndTimer;

		yield break;

		MiniAlertUI time = UIManager.instance.MiniAlert(adflash.Txt[0].transform.position, "" + EndTimer_curr,
														170, Color.white, EndTimer, 0.0F);
		time.AddJuice(Juice.instance.BounceB, 0.3F);

		
		while((EndTimer -= Time.deltaTime) > 0.0F || EnderAd_showing)
		{
			
			if((int) EndTimer != EndTimer_curr)
			{
				time.AddJuice(Juice.instance.BounceB, 0.3F);
				EndTimer_curr = (int)EndTimer;
			}
			time.text = "" + (int) EndTimer_curr;
			yield return null;
		}

		adflash.SetTween(0, false);
		UIManager.instance.AdAlert.SetActive(false);
		
		yield return StartCoroutine(GameData.DeltaWait(0.25F));
	}

	bool EnderAd_showing = false;

	public void SetDeathReward(bool active){

		int num = active ? 4 : 2;
		string title = active ? "MASS REVIVE!" : "PARTIAL REVIVE!";
		UIManager.instance.ScreenAlert.SetTween(0, false);
		UIManager.instance.KillUI.Hide();
		UIManager.instance.AdAlert.SetActive(false);
		EnderAd_showing = false;
		UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, title, 180, Color.white, 0.6F, 0.1F, true);
		for(int i = 0; i < num; i++)
		{
			Player.Classes[i].isKilled = false;
		}
		Player.instance.ResetStats();
		Player.Stats.isKilled = false;
		Player.Stats._Health = Player.Stats._HealthMax;
		//EndType = End_Type.Defeat;
		paused = false;
		gameStart = true;
	}
#endregion

#region Waves/Zones

	public void AdvanceZoneMap(int choice)
	{
		UIManager.instance.CloseZoneUI();
		bool end = ZoneMap.Progress();
		if(end) EnterZone(ZoneMap.CurrentBracket[choice]);		
		else Victory();
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
		CurrentZone.gameObject.name = target.gameObject.name;
		CurrentZone.transform.parent = this.transform;
		if(CurrentWave != null) 
		{
			Destroy(CurrentWave.gameObject);
			CurrentWave = null;
		}

		yield return StartCoroutine(CurrentZone.Enter());
		yield return StartCoroutine(TileMaster.instance.CreateTravelTiles());

		yield return StartCoroutine(_GetWave(w));

		yield return StartCoroutine(TileMaster.instance.BeforeTurn());
	}

	IEnumerator _EnterZone(Zone z, int wavenum, string name = "")
	{
		if(CurrentZone != null) Destroy(CurrentZone.gameObject);

		CurrentZone = (Zone) Instantiate(z);
		CurrentZone.gameObject.name = z.gameObject.name;
		CurrentZone.transform.parent = this.transform;
		
		if(CurrentWave != null) 
		{
			Destroy(CurrentWave.gameObject);
			CurrentWave = null;
		}
		yield return StartCoroutine(CurrentZone.Enter());
		CurrentZone.SetCurrent(wavenum);
		if(name != string.Empty) yield return StartCoroutine(_GetWave(CurrentZone.GetWaveByName(name)));
		else yield return StartCoroutine(_GetWave());
	}

	public void EscapeZone()
	{	
		bool end = ZoneMap.Progress();
		if(end) UIManager.instance.ShowZoneUI(true);		
		else Victory();
		/*if(ZoneMap.NextBracket == ZoneMap.CurrentBracket)
		{
			AdvanceZoneMap(0);
		}
		else 
		{
			List<GENUS> prevstairs = new List<GENUS>();
			for(int i = 0; i < ZoneMap.NextBracket.Length; i++)
			{
				GENUS g = (GENUS) Random.Range(0,3);
				while(prevstairs.Contains(g)) g = (GENUS) Random.Range(0,3);
				prevstairs.Add(g);
				Tile t = TileMaster.instance.ReplaceTile(TileMaster.RandomResTile, TileMaster.Types["stairs"],g);
				(t as Stairs).ZoneIndex = i;
			}
			
		}*/
		
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
		Player.instance.CompleteMatch = true;
		EnemyTurn = true;
		TileMaster.instance.SetFillGrid(false);
		UIManager.instance.current_class = null;
		UIManager.instance.SetClassButtons(false);
		UIManager.instance.ShowGearTooltip(false);
		
		UIManager.Objects.BotGear.SetToState(0);
		UIManager.Objects.TopGear.SetToState(0);
		UIManager.instance.MoveTopGear(0);

		yield return StartCoroutine(BeforeMatchRoutine());
		bool all_of_resource = false;
		
		//Debug.Log("BEFORE MATCH");
		if(Player.instance.CompleteMatch) 
		{
			yield return StartCoroutine(MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		}
		else 
		{
			EnemyTurn = false;
			yield break;
		}

		
		//Debug.Log("MATCH");

		yield return StartCoroutine(Player.instance.AfterMatch());
		yield return StartCoroutine(Player.instance.EndTurn());
		//Debug.Log("AFTER MATCH");

		//UIManager.Objects.BotGear.SetTween(3, false);
		
		yield return StartCoroutine(TileMaster.instance.BeforeTurn());
		//Debug.Log("BEFORE TURN");
	/* ENEMY TURN *////////////////////////////////////////////////////
		if(CurrentWave) yield return StartCoroutine(CurrentWave.BeginTurn());
		if(Player.instance.Turns % (int)Player.Stats.AttackRate == 0 && TileMaster.instance.EnemiesOnScreen > 0)
		{
			yield return StartCoroutine(EnemyTurnRoutine());
		}

		//Debug.Log("TURN");
	
		while(TileMaster.EnemiesAttacking()) yield return null;
		TileMaster.instance.ResetTiles(true);
		//UIManager.instance.CashMeterPoints();
		yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		
		yield return StartCoroutine(TileMaster.instance.AfterTurn());

		//Debug.Log("AFTER TURN");
		yield return StartCoroutine(CompleteTurnRoutine());
		yield return StartCoroutine(Player.instance.CheckHealth());	
		if(CurrentWave) yield return StartCoroutine(CurrentWave.AfterTurn());

		if(CurrentWave && CurrentWave.Ended && !Player.Stats.isKilled)
		{
			yield return StartCoroutine(_GetWave());
		}
		
		UIManager.Objects.BotGear.SetToState(0);
		yield return StartCoroutine(Player.instance.BeginTurn());
		
		TileMaster.instance.ResetTiles(true);
		UIManager.instance.ResetTopGear();
		paused = false;
		yield return null;
	}


	IEnumerator EnemyTurnRoutine()
	{
		float per_column = 0.06F;
		List<Tile> total_attackers = new List<Tile>();
		int total_damage = 0;
		List<Tile> column;

		List<Tile> allied_attackers = new List<Tile>();
	
		yield return new WaitForSeconds(GameData.GameSpeed(0.08F));
		UIManager.Objects.BotGear.SetToState(3);
		UIManager.Objects.TopGear.SetToState(0);

		//ENEMY ATTACKERS
		for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
		{
			column = new List<Tile>();
			for(int y = 0; y < TileMaster.Grid.Size[1];y++)
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
						column.Add(tile);
					}
					
				}
			}

			for(int i = 0; i < column.Count; i++)
			{
				if(column[i] == null || !column[i].gameObject.activeSelf || column[i].Destroyed) continue;

				column[i].SetState(TileState.Selected);
				column[i].OnAttack();
				total_damage += column[i].GetAttack();

				column[i].AttackPlayer();
				yield return StartCoroutine(column[i].Animate("Attack", 0.05F));
			}

			//total_attackers.AddRange(column);
			if(column.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(per_column));
		}
		/*if(total_attackers.Count > 0)
		{
			GameData.Log("Took " + total_damage + " damage from " + total_attackers.Count + " attackers");
			
		} */
		if(total_damage > 0) yield return new WaitForSeconds(GameData.GameSpeed(0.2F));

		//ALLIED ATTACKERS
		if(allied_attackers.Count > 0) 
		{
			TileMaster.instance.ResetTiles();
			yield return new WaitForSeconds(GameData.GameSpeed(0.21F));
		}

		foreach(Tile child in allied_attackers)
		{
			if(child == null) continue;

			child.SetState(TileState.Selected);
			child.OnAttack();

			Tile target = null;
			if(TileMaster.Enemies.Length == 0) continue;
			target = TileMaster.Enemies[Random.Range(0, TileMaster.Enemies.Length)];
			if(target == null) continue;
			else
			{
				child.AttackTile(target);
				yield return StartCoroutine(child.Animate("Attack", 0.05F));
				
			}
		}

		if(allied_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(0.14F));
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
			//List<Tile> beforematch = new List<Tile>();
			for (int i = 0; i < newTiles.Count; i++)
			{
				if (newTiles[i] == null) continue;
				if (newTiles[i].BeforeMatchEffect) yield return StartCoroutine(newTiles[i].BeforeMatch(false));
			}

			/*beforematch.Sort((a,b) => {return a.BeforeMatchPriority.CompareTo(b.BeforeMatchPriority);});
			beforematch.OrderBy(b => b.BeforeMatchPriority);
			for(int n = 0; n < beforematch.Count; n++)
			{
				yield return StartCoroutine(beforematch[n].BeforeMatch(false));
			}*/

			yield return StartCoroutine(Player.instance.BeforeMatch(newTiles));
			yield return null;
			newTiles.Clear();
			newTiles.AddRange(PlayerControl.instance.selectedTiles);
			yield return null;
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
		UIManager.Objects.BotGear.SetToState(0);
		UIManager.Objects.TopGear.SetToState(0);

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
		AudioManager.instance.ClearAlerts();

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

