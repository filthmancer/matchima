using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DiffMode
{
	Easy,
	Normal,
	Hard,
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public static _GameSaveLoad PlayerLoader;

	public static bool debug = false;
	public static bool inStartMenu;

	public static float GlobalManaMult = 1.0F,
						GlobalHealthMult = 1.0F,
						GlobalArmourMult = 1.0F;
	public static float GrowthRate_Easy = 0.08F,
						GrowthRate_Normal = 0.18F,
						GrowthRate_Hard = 0.25F;

	public Juice _Juice;

	public float Difficulty_Growth{
		get{
			switch(DifficultyMode)
			{
				case DiffMode.Normal:
				return Player.instance.Turns/20.0F * GrowthRate_Normal;
				case DiffMode.Easy:
				return Player.instance.Turns/20.0F * GrowthRate_Easy;
				case DiffMode.Hard:
				return Player.instance.Turns/20.0F * GrowthRate_Hard;
			}
			return GrowthRate_Easy;
		}
	}
	public static float Difficulty = 1;
	public float _Difficulty = 0;

	public DiffMode DifficultyMode = DiffMode.Normal;
	public int TurnsToWin = 100;
	public bool gameStart = false;
	public bool paused = false;
	public bool EnemyTurn = false;

	public long RoundTokens = 0;
	public int Tens = 0, Hunds = 0, Thous = 0;
	private int OverflowThisTurn = 0;
	private int OverflowMulti = 2;


	private float OverflowHitMulti = 1.0F;
	private float AllOfTypeMulti = 1.2F;

	public Wave [] _Wave = new Wave[5];
	public int TimeToWave = 3;

	public bool LevelUp = false, WaveAlert = false;
	
	public bool isPaused
	{
		get{
			return !gameStart || paused;
		}
	}

	public static bool TuteActive = false;
	public Wave TuteWave;
	void OnApplicationQuit()
	{
		GameData.instance.Save();
		if(gameStart) PlayerLoader.Save();
		PlayerPrefs.SetInt("Resume", 0);
		//PlayerPrefs.SetInt("Resume", gameStart ? 1 : 0);
		//PlayerPrefs.SetString("Name", Player.instance.Class.Name);
		//PlayerPrefs.SetInt("Turns", Player.instance.Turns);
	}

	void OnApplicationPause()
	{
		if(!Application.isEditor)
		{
			GameData.instance.Save();
			if(gameStart) 
			{
				PlayerLoader.Save();
			}
			PlayerPrefs.SetInt("Resume", 0); //gameStart ? 1 : 0);
			//PlayerPrefs.SetString("Name", Player.instance.Class.Name);
			//PlayerPrefs.SetInt("Turns", Player.instance.Turns);
		} 
	}

	void Awake()
	{
		//if(Application.isMobilePlatform)
		//{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 48;
		//}
		  
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
		Difficulty = 0;
		inStartMenu = true;
		TuteActive = false;

		for(int i = 0; i < _Wave.Length; i++)
		{
			if(_Wave[i] != null) 
			{
				_Wave[i].Active = false;
				_Wave[i].ResetChances();
				Destroy(_Wave[i].gameObject);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		_Difficulty = Difficulty;
		if(Player.loaded && UIManager.loaded && !gameStart) 
		{
			if(!GameData.loading_assets) GameData.instance.LoadAssets();
			
			if(GameData.loaded_assets) StartGame();
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

		if(Input.GetKeyDown(KeyCode.U))			Player.Classes[0].AddToMeter(Player.Classes[0].MeterMax);
		if(Input.GetKeyDown(KeyCode.I))			Player.Classes[1].AddToMeter(Player.Classes[1].MeterMax);
		if(Input.GetKeyDown(KeyCode.O))			Player.Classes[2].AddToMeter(Player.Classes[2].MeterMax);
		if(Input.GetKeyDown(KeyCode.P))			Player.Classes[3].AddToMeter(Player.Classes[3].MeterMax);

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

	public void Retire()
	{
		UIManager.instance.ShowMenu(false);
		Player.Stats.isKilled = true;
		Player.instance.retired = true;
		paused = true;
		StartCoroutine(EndGame());
	}

	IEnumerator EndGame()
	{
		yield return new WaitForSeconds(0.3F);
		TileMaster.instance.ClearGrid(false);

		yield return new WaitForSeconds(0.3F);
		long AllTokens = 0;//PlayerPrefs.GetInt("AllTokens");

		UIManager.instance.ShowKillUI(AllTokens, Tens, Hunds,Thous);
		RoundTokens = Tens + (Hunds * 50) + (Thous * 500);
		PlayerPrefs.SetInt("AllTokens", (int)(AllTokens + RoundTokens));
	}

	public void Cheat(int i)
	{
		switch(i)
		{
			case 1: //Z
			Player.Stats._Health = Player.Stats._HealthMax;
			break;
			case 2: //X
			TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["flame"], GENUS.ALL, 1, 50);
			//UIManager.instance.ShowResourceUI();
			break;
			case 3: //c
			TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["cross"], GENUS.ALL,1, 4);
			break;
			case 4: //V
			//GetTurn();
			int [] point = PlayerControl.instance.focusTile.Point.Base;
			TileMaster.instance.ReplaceTile(PlayerControl.instance.focusTile, TileMaster.Types["ward"], GENUS.STR,1, 4);
			(TileMaster.Tiles[point[0], point[1]] as Ward).Buff = "Frenzy";
			TileEffect effect = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName("Fragile"));
			effect.GetArgs(-1);
			TileMaster.Tiles[point[0], point[1]].AddEffect(effect);

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
			TileMaster.instance.IncreaseGrid(1,0);
			break;
			case 8: //A
			CameraUtility.instance.ScreenShake(0.6F, 1.1F);
			break;
			case 9: //S
			TileMaster.instance.Ripple(TileMaster.Grid.Tiles[3,3]);
			break;
		}

		//print(Difficulty + Mathf.Exp(Difficulty_Growth) + ":" + Difficulty_Growth);
		//Difficulty += Mathf.Exp(Difficulty_Growth);
		//Player.instance.Turns += 25;
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


			Thous += hunds/100;
			Hunds += tens/10;
			Tens += ones;
		}
		else
		{
			int _tk = PlayerPrefs.GetInt("AllTokens");
			_tk += tokens;

			PlayerPrefs.SetInt("AllTokens", _tk);
		}
	}

	public void GetWave(Wave w, int slot)
	{
		if(_Wave[slot] != null) Destroy(_Wave[slot].gameObject);
		_Wave[slot] = Instantiate(w);
		_Wave[slot].Active = false;
		_Wave[slot].Timer = 3;

		if(_Wave[slot].Effects.LevelUpOnEnd) Difficulty += Mathf.Exp(Difficulty_Growth);
		//StartCoroutine(WaveStartRoutine(slot, alert));
	}

	public void ActivateWave(int slot, bool alert = true)
	{
		_Wave[slot].Randomise();
		_Wave[slot].transform.parent = this.transform;
		_Wave[slot].Current = _Wave[slot].Required;
		_Wave[slot].Active = true;
		WaveAlert = true;

		StartCoroutine(WaveStartRoutine(slot, alert));
	}

	IEnumerator WaveStartRoutine(int slot, bool alert)
	{
		PlayerControl.instance.ResetSelected();
		if(alert)
		{
			UIManager.instance.WaveAlert.SetActive(true);
			UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(true);
			yield return new WaitForSeconds(0.15F);

			yield return new WaitForSeconds(1.0F);
			
			UIManager.instance.WaveAlert.SetActive(false);
			UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(false);
			PlayerControl.instance.ResetSelected();
		}
		WaveAlert = false;
		_Wave[slot].OnStart(slot);
		yield return null;
	}

	public void WaveEnd(int slot)
	{
		StartCoroutine(WaveEndRoutine(slot));
	}

	IEnumerator WaveEndRoutine(int slot)
	{
		WaveAlert = true;
		GlobalManaMult = 1.0F;

		float wait_time = 1.3F;
		if(_Wave[slot].Effects.LevelUpOnEnd) 
		{
			WaveReward reward = GenerateWaveReward();
			UIManager.instance.WaveAlert.SetActive(true);
			UIManager.instance.WaveAlert.Img[1].gameObject.SetActive(true);
			if(reward != null)
			{
				UIManager.instance.WaveAlert.Img[2].gameObject.SetActive(true);
				UIManager.instance.WaveAlert.Txt[0].text = reward.Title;
				wait_time += 0.6F;
			}
		}
		yield return new WaitForSeconds(1.3F);
		_Wave[slot].Active = false;		
		TimeToWave = 3;//Random.Range((int)_Wave[slot].PointfieldEndTime.x, (int)_Wave[slot].PointfieldEndTime.y);
		
		if(_Wave[slot].Effects.LevelUpOnEnd)
		{
			UIManager.instance.WaveAlert.SetActive(false);
			UIManager.instance.WaveAlert.Img[1].gameObject.SetActive(false);
			UIManager.instance.WaveAlert.Img[2].gameObject.SetActive(false);
		}	
		WaveAlert = false;
		GameManager.instance._Wave[slot] = null;

		if(slot == 0)
		{
			GetWave(GameData.instance.GetRandomWave(), slot);
		}
		yield return StartCoroutine(CompleteTurnRoutine());
		yield return null;
	}

	public WaveReward GenerateWaveReward()
	{	
		if(Random.value > 1.0F) return null;

		WaveReward reward = new WaveReward();
		reward.value = Difficulty;
		float val = Random.value;
		if(val < 0.25F)
		{
			reward.value = (reward.value * 5);
			reward.Title = "+" + (int)reward.value + " mana";
			foreach(Class child in Player.Classes)
			{
				child.AddToMeter((int)reward.value);
			}
		}
		else if(val < 0.5F)
		{
			reward.value = 1.0F + reward.value / 100;
			reward.Title = "+" + reward.value.ToString("0.00") + "x mana values";
			GlobalManaMult = reward.value;
		}
		else if(val < 0.75F)
		{
			reward.value = Mathf.Clamp(reward.value / 3, 1, Mathf.Infinity);
			reward.Title = "+ Upgrade Value!";
			foreach(Class child in Player.Classes)
			{
				child.WaveLevelRate += (int) reward.value;
			}
		}
		else
		{
			reward.value = Mathf.Clamp(reward.value * 20, 1, Player.Stats._HealthMax);
			reward.Title = (int)reward.value + " HP HEAL";
			Player.Stats.Heal((int) reward.value);
		}
		return reward;
	}

	public class WaveReward
	{
		public string Title;
		public float value;
	}

	public ClassContainer CheckForClass(string name)
	{
		foreach(ClassContainer _class in GameData.instance.Classes)
		{
			if(_class.Name == name) 
			{
				return _class;
			}
		}
		if(name == GameData.instance.EndlessMode.Name) return GameData.instance.EndlessMode;
		Debug.LogError("CLASS COULD NOT BE FOUND");
		return null;
	}

	public void ResumeGame()
	{
		string _class = PlayerLoader.Load();
		UIManager.Menu.ClassName.text = _class;
		if(_class == "") return;
		ClassContainer c = CheckForClass(_class);
		TurnsToWin = c.TurnsToWin;
		Player.instance.ResumeClass(c);
		StartCoroutine(UIManager.instance.LoadUI());	
	}

	public void StartGame()
	{
		inStartMenu = false;
		gameStart = true;
		StartCoroutine(Player.instance.BeginTurn());
		RoundTokens = 0;
		TimeToWave = Player.Stats.TurnsToWave;
		UIManager.instance.LoadScreen.SetSpin(false);
		UIManager.instance.LoadScreen.SetActive(false);

		if(TuteActive) 
		{
			GetWave(TuteWave, 0);
			ActivateWave(0, false);
		}
		else GetWave(GameData.instance.GetRandomWave(), 0);
	}

	public void LoadClass(ClassContainer targetClass)
	{
		UIManager.instance.LoadScreen.SetActive(true);
		TurnsToWin = targetClass.TurnsToWin;
		Player.instance.AddClass(targetClass);
		StartCoroutine(UIManager.instance.LoadUI());
	}


	public void LoadGame()
	{
		UIManager.instance.LoadScreen.SetActive(true);
		UIManager.instance.LoadScreen.SetSpin(true);
		TurnsToWin = 100;
		Player.instance.Load();
		foreach(Class child in Player.Classes)
		{
			if(child == null) continue;
			GameData.instance.LoadClassAbilities(child);
		}
		
		StartCoroutine(UIManager.instance.LoadUI());
	}


	public void GetTurn()
	{
		StartCoroutine(Turn());
	}

	IEnumerator Turn()
	{

/* PLAYER TURN */////////////////////////////////////////////////////////////////////////////////////////////////////////////
		float after_match = 0.2F, before_match = 0.5F;
		int [] resource = new int [6];
		int [] health   = new int [6];
		int [] armour   = new int [6];
		int enemies_hit = 0;
		EnemyTurn = true;
		TileMaster.instance.SetFillGrid(false);
		UIManager.instance.current_class = null;
		UIManager.instance.SetClassButtons(false);

		bool all_of_resource = false;

		yield return StartCoroutine(Player.instance.BeforeMatch(PlayerControl.instance.selectedTiles));
		int rate = 3 + (int) ((Player.instance.Options.GameSpeed- 1) * 3), num = 0;
		if(Player.instance.CompleteMatch) 
		{
			foreach(Tile child in PlayerControl.instance.finalTiles)
			{
				//if(child.isMatching) continue;

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
					if(values[0] > 0)
					{
						//if(child.Type.isEnemy) added_res += Player.Stats.Hunter;
						//else added_res += Player.Stats.Harvester;
					}
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

				yield return new WaitForSeconds(Time.deltaTime * rate);
				num++;
				if(num % 4 == 0) rate = Mathf.Clamp(rate - 1, 1, 5);
			}
		}
		else EnemyTurn = false;

		if(TileMaster.FillGrid_Override) TileMaster.instance.SetFillGrid(true);

		yield return StartCoroutine(CollectResourcesRoutine(resource, health, armour));
		//CollectResources(resource, health, armour);
		

		yield return StartCoroutine(Player.instance.AfterMatch());

		Player.Stats.CompleteLeech(enemies_hit);
		Player.Stats.CompleteRegen();
		Player.instance.CompleteHealth();
		Player.instance.CheckHealth();	
		
		PlayerControl.matchingTile = null;
		Player.instance.CheckForBestCombo(resource);
	
/* ENEMY TURN */////////////////////////////////////////////////////////////////////////////////////////////////////////////

		yield return StartCoroutine(TileMaster.instance.BeforeTurn());

		for(int i = 0; i < _Wave.Length; i++)
		{
			if(_Wave[i] != null) 
			{
				if(_Wave[i].Active)
				{
					if(_Wave[i].Effects.HitByPresence) _Wave[i].AddPoints(-Player.Stats.Presence);
					_Wave[i].AddPoints(-_Wave[i].PointsPerTurn);
					_Wave[i].WaveCheck();
					//if(_Wave[i].Complete()) yield return new WaitForSeconds(GameData.GameSpeed(0.25F));
				}
				else
				{
					if(_Wave[i].Timer > 1)
					{
						_Wave[i].Timer --;
					}
					else if(_Wave[i].Timer == 1)
					{
						ActivateWave(i, true);
					}
				}
			}
		}
		
		if(Player.instance.EndTurn() && TileMaster.instance.EnemiesOnScreen > 0)
		{
			yield return new WaitForSeconds(0.2F);
			yield return StartCoroutine(EnemyTurnRoutine());
		}

		yield return StartCoroutine(TileMaster.instance.AfterTurn());
		
		for(int i = 0; i < _Wave.Length; i++)
		{
			if(_Wave[i] != null) 
			{
				if(_Wave[i].Active)
				{
					if(_Wave[i].Complete()) yield return new WaitForSeconds(GameData.GameSpeed(0.45F));
					if(_Wave[i].Current <= 0 && _Wave[i].Active)
					{
						WaveAlert = true;
						yield return StartCoroutine(WaveEndRoutine(i));
					}
				}
			}
		}
		
		TileMaster.instance.ResetTiles(true);
		yield return StartCoroutine(Player.instance.BeginTurn());
		yield return StartCoroutine(CompleteTurnRoutine());
		yield return null;
	}


	IEnumerator EnemyTurnRoutine()
	{
		float per_column = 0.18F;
		List<Tile> all_attackers = new List<Tile>();
		List<Tile> column_attackers;
		int damage = 0;

		for(int x = 0; x < TileMaster.instance.MapSize.x; x++)
		{
			column_attackers = new List<Tile>();
			for(int y = 0; y < TileMaster.instance.MapSize.y;y++)
			{
				Tile tile = TileMaster.Tiles[x,y];
				if(tile == null) continue;
				if(tile.CanAttack())
				{
					tile.OnAttack();
					damage += tile.GetAttack();
					column_attackers.Add(tile as Tile);
					tile.SetState(TileState.Selected);
				}
			}
			foreach(Tile child in column_attackers)
			{
				if(child == null) continue;
				MiniTile enemy = (MiniTile) Instantiate(TileMaster.instance.ResMiniTile);
				enemy._render.sprite = child.Info.Outer;
				enemy.transform.position = child.transform.position;
				enemy.SetTarget(UIManager.instance.HealthImg.transform, 0.2F, 0.0F);
				enemy.SetMethod(() =>{
						Player.Stats.Hit(child.GetAttack(), child);
					});
				yield return StartCoroutine(child.Animate("Attack", 0.05F));
			}

			all_attackers.AddRange(column_attackers);
			if(column_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(per_column));
		}
		//UIManager.instance.targetui_class = nexttarget;
		//Player.Stats.Hit(damage, all_attackers);
		yield return new WaitForSeconds(0.2F);
	}


	public IEnumerator BeginTurn()
	{
		yield return null;
	}

	public IEnumerator CompleteTurnRoutine()
	{
		yield return StartCoroutine(Player.instance.CheckForBoonsRoutine());
		Player.instance.CompleteHealth();
		Player.instance.CheckHealth();	
		Player.instance.ResetStats();

		PlayerControl.instance.canMatch = true;
		PlayerControl.instance.isMatching = false;
		PlayerControl.instance.focusTile = null;
		PlayerControl.instance.selectedTiles.Clear();
		PlayerControl.instance.finalTiles.Clear();
		PlayerControl.matchingTile = null;

		for(int i = 0; i < _Wave.Length; i++)
		{
			if(_Wave[i] != null) _Wave[i].OnTurn();
		}
		
		

		EnemyTurn = false;
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
		yield return new WaitForSeconds(0.4F);
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
		Player._Options.ViewTileStats = !Player._Options.ViewTileStats;
	}

	public void AddOverflow(int amt)
	{
		OverflowThisTurn += amt;
	}

	public void EnemyKilled(Enemy e)
	{
		for(int i = 0; i < _Wave.Length; i++)
		{
			if(_Wave[i] != null) _Wave[i].EnemyKilled(e);
		}
		
	}

	public void ToggleEasyGENUSe()
	{
		if(DifficultyMode == DiffMode.Normal) DifficultyMode = DiffMode.Easy;
		else DifficultyMode = DiffMode.Normal;
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

