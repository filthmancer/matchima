using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum MenuState
{
	StartScreen,
	MainMenu,
	Options,
	Character
}

public class UIMenu : UIObj {

	public UIObj ClassMenu;
	public UIObj PauseMenu;
	public UIObj OptionsMenu;
	public UIObj HelpMenu;
	public UIObj DefaultMenu;

	public UIObj ClassButton;
	public UIObj ResumeGame;
	public UIObj NewGame;
	public UIObj ModeButton;

	public UIObj Difficulty;
	public UIObj LevelUp;
	public UIObj AlertObj;
	public UIObj PipObj;
	public UIObj HealthPipParent, AttackPipParent, MagicPipParent;
	public UIObj HelpBasic, HelpMana, HelpItems;

	public UIClassSelect ClassPrefab;
	public List<UIClassSelect> ClassObjects;

	public RectTransform ClassParent;

	public TextMeshProUGUI ResumeGameInfo;

	public string [] DiffText;

	public UIObj [] SlotButtons;
	public int? TargetSlot = null;

	private List<UIObj> class_buttons = new List<UIObj>();
	private List<string> class_names = new List<string>();
	private List<string> class_short_names = new List<string>();
	private UIObj target_obj;

	private int selected_class = 0;
	private Vector3 ClassParent_pos = Vector3.zero;

	private List<GameObject> tiles = new List<GameObject>();
	private float time_to_tile = 0.6F;
	private float time = 0.0F;



	public MenuState State = MenuState.StartScreen;

	void Start () {
		UIManager.ShowClassButtons(false);
		UIManager.ShowWaveButtons(false);
		UIManager.Objects.MiddleGear.SetActive(false);
		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,true);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,true);
		UIManager.Objects.BotGear.Child[0].SetActive(false);
		UIManager.Objects.TopGear.SetRotate(true, Vector3.forward * Time.deltaTime * 4);
		UIManager.Objects.BotGear.SetRotate(true, Vector3.back * Time.deltaTime * 4);
		UIManager.ShowWaveButtons(false);

		
		int wedge_num = 8;
		for(int i = 0; i < wedge_num; i++)
		{
			Class child = GameData.instance.Classes[i];
			UIClassSelect obj = (UIClassSelect) Instantiate(ClassPrefab);
			obj.transform.SetParent(UIManager.Objects.BotGear[3].GetChild(0).transform);
			
			UIManager.Objects.BotGear[3].GetChild(0).AddChild(obj);
			obj.Setup(child);
			obj.transform.rotation = Quaternion.Euler(0,0,360/wedge_num * i);

		}
		UIManager.Objects.BotGear[3].GetChild(0).SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(State == MenuState.StartScreen)
		{
			if(Input.GetMouseButtonDown(0))
			{
				MainMenu();
			}
		}
		
		//Tokens.text = PlayerPrefs.GetInt("AllTokens") + " Tokens";

		if(GameManager.inStartMenu)
		{
			//if(time > time_to_tile)
			//{
			//	TileInfo spec = TileMaster.Types.RandomType();
			//	Tile new_tile = Instantiate(spec._Type.Prefab).GetComponent<Tile>();
			//	//new_tile.Setup(new int [] {0,0}, spec);
			//	new_tile.Setup(0,0,1,spec);
			//	new_tile.transform.position = new Vector3(Random.Range(-3.5F,3.5F), 10, 0);
			//	new_tile.speed_max_falling = -7.0F;
			//	new_tile.Params._border.sprite = spec.Outer;
			//	
			//	tiles.Add(new_tile.gameObject);
			//	time = 0.0F;
			//	time_to_tile = Random.Range(0.8F, 2F);
			//}
			//else time += Time.deltaTime;
		}
	}

	public void Reset()
	{
		for(int i = 0; i < UIManager.ClassButtons.Length; i++)
		{
			UIManager.ClassButtons[i].TweenClass(false);
		}
		
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(2, false);
		UIManager.Objects.BotGear[3][0].SetActive(false);
		UIManager.Objects.MiddleGear.SetActive(true);
	}
	
	public void MainMenu()
	{
		State = MenuState.MainMenu;

		Reset();
		UIManager.Objects.TopGear.SetRotate(true, Vector3.forward * Time.deltaTime * 2);
		UIManager.Objects.BotGear.SetRotate(true, Vector3.forward * Time.deltaTime * 2);
	
		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1, false);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1, false);
		UIManager.ShowClassButtons(true);
		UIManager.WaveButtons[0].SetActive(true);
		UIManager.WaveButtons[0].Txt[0].text = "New";
		UIManager.WaveButtons[0].Img[1].color = GameData.Colour(GENUS.STR);
		if(PlayerPrefs.GetInt("Resume") == 1) 
		{
			UIManager.WaveButtons[1].SetActive(true);
			UIManager.WaveButtons[1].Txt[0].text = "Resume";
			UIManager.WaveButtons[1].Img[1].color = GameData.Colour(GENUS.DEX);
			//ResumeGameInfo.text = PlayerPrefs.GetString("Name") + "\nTurn: " + PlayerPrefs.GetInt("Turns");
		}
		else UIManager.WaveButtons[1].SetActive(false);
		
		UIManager.Objects.TopGear.Txt[0].text = "";
		UIManager.Objects.BotGear.Txt[0].text = "";
		UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);
		UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
		() => {StartGame();});
		UIManager.Objects.MiddleGear[0].GetChild(1).AddAction(UIAction.MouseUp,
		() => {ChangeMode();});
		UIManager.Objects.MiddleGear[0].GetChild(2).AddAction(UIAction.MouseUp,
		() => {ChangeDifficulty();});
	}

	public void HeroMenu(int x)
	{
		if(State != MenuState.Character)
		{
			State = MenuState.Character;
			(UIManager.Objects.BotGear as UIObjTweener).SetTween(2, true);
			UIManager.Objects.BotGear[3][0].SetActive(true);
			UIManager.Objects.TopGear[1][0].SetActive(true);
			UIManager.Objects.TopGear[1][0].Txt[0].text = "Exit";
			UIManager.Objects.TopGear[1][0].ClearActions();
			UIManager.Objects.TopGear[1][0].AddAction(UIAction.MouseUp,
			() =>{
				MainMenu();
				});
			UIManager.Objects.TopGear[1][1].SetActive(false);
			UIManager.Objects.TopGear[1][2].SetActive(false);
			
			int wedge_num = 8;
			for(int i = 0; i < UIManager.Objects.BotGear[3][0].Length; i++)
			{
				Class child = GameData.instance.Classes[i];
				(UIManager.Objects.BotGear[3][0].GetChild(i) as UIClassSelect).ClearActions();
				(UIManager.Objects.BotGear[3][0].GetChild(i) as UIClassSelect).Setup(child); 
			}
		}
		else
		{
			if(x == 100) return;
			SetTargetSlot(x);
		}
	}


	public void StartGame()
	{
		switch(GameManager.instance.Mode)
		{
			case GameMode.Endless:
	
			break;
			case GameMode.Story:
			Player.instance._Classes[0] = GameData.instance.GetClass("Barbarian");
			Player.instance._Classes[1] = GameData.instance.GetClass("Rogue");
			Player.instance._Classes[2] = GameData.instance.GetClass("Wizard");
			Player.instance._Classes[3] = GameData.instance.GetClass("Bard");
			break;
		}

		NewGameActivate();
	}



	public void SetTargetSlot(int i)
	{
		if(TargetSlot == i)
		{
			UIManager.ClassButtons[TargetSlot.Value].TweenClass(false);
			TargetSlot = null;
			return;
		}
		else
		{
			if(TargetSlot != null) UIManager.ClassButtons[TargetSlot.Value].TweenClass(false);
			TargetSlot = i;
			if(TargetSlot != null) UIManager.ClassButtons[TargetSlot.Value].TweenClass(true);
		} 
		
	}

	public void SetTargetClass(UIClassSelect c)
	{
		//If targetslot is null, get the first empty slot
		bool set_from_null = false;
		if(TargetSlot == null)
		{
			for(int i = 0; i < Player.instance._Classes.Length; i++)
			{
				if(Player.instance._Classes[i] == null)
				{
					TargetSlot = i;
					set_from_null = true;
					break;
				}
			}
			if(TargetSlot == null) return;
		}
		
		//Set the targeted slot class
		Player.instance._Classes[TargetSlot.Value] = c._class;
		(UIManager.Objects.BotGear[1][TargetSlot.Value][0] as UIClassButton).Setup(c._class);

		ChangeMode(GameMode.Endless);
		//If targetslot was initally null, set back to null
		if(set_from_null) TargetSlot = null;
	}

	public void ChangeDifficulty()
	{
		GameManager.instance.DifficultyMode = GameManager.instance.DifficultyMode + 1;
		if(GameManager.instance.DifficultyMode > (DiffMode)2) GameManager.instance.DifficultyMode = 0;
		UIManager.Objects.MiddleGear[0].GetChild(2).Txt[0].text = "" + GameManager.instance.DifficultyMode;
	}

	public void ChangeMode(GameMode m = GameMode.None)
	{
		if(m == GameMode.None)
		{
			m = GameManager.instance.Mode;
			if(m == GameMode.Story) m = GameMode.Endless;
			else if(m == GameMode.Endless) m = GameMode.Story;
		}

		GameManager.instance.Mode = m;

		UIManager.Objects.MiddleGear[0].GetChild(1).Txt[0].text = "" + GameManager.instance.Mode;
		UIManager.Objects.MiddleGear[0].GetChild(0).BooleanObjColor(m == GameMode.Story);
	}


	public void BackToDefault()
	{
		DefaultMenu.SetActive(true);
		ClassMenu.SetActive(false);
		OptionsMenu.SetActive(false);
		HelpMenu.SetActive(false);
	}

	public void NewGameActivate()
	{
		UIManager.Objects.TopGear.SetRotate(false);
		UIManager.Objects.BotGear.SetRotate(false);
		UIManager.Objects.MiddleGear.SetActive(false);
		UIManager.Objects.BotGear.Child[0].SetActive(true);
		bool alert = false;
		for(int i = 0; i < Player.instance._Classes.Length; i++)
		{
			if(Player.instance._Classes[i] == null)
			{
				alert = true;
				break;
			}
		}
		if(alert)
		{
			StartCoroutine(SetAlert());
			return;
		}
		DefaultMenu.SetActive(false);
		ClassMenu.SetActive(false);
		GameManager.instance.LoadGame(false);
	}

	public void CustomStartActivate()
	{
		//HeroMenu();
		ClassMenu.SetActive(true);
		DefaultMenu.SetActive(false);
	}



	public void ResumeGameActivate()
	{
		DefaultMenu.SetActive(false);
		GameManager.instance.LoadGame(true);
	}

	public void OptionsActivate()
	{
		OptionsMenu.SetActive(true);
		ResetOptions();
		DefaultMenu.SetActive(false);
	}

	public void ResetOptions()
	{
		OptionsMenu["RealNumbers"].BooleanObjColor(Player.Options.ShowNumbers);
		OptionsMenu["RealHP"].BooleanObjColor(Player.Options.RealHP);
		OptionsMenu["Intros"].BooleanObjColor(Player.Options.ShowIntroWaves);
		OptionsMenu["Story"].BooleanObjColor(!Player.Options.SkipAllStory);
	}

	public void HelpActivate()
	{
		HelpMenu.SetActive(true);
		DefaultMenu.SetActive(false);
	}

	public void TutorialActivate()
	{
		DefaultMenu.SetActive(false);
		Player.instance._Classes[0] = GameData.instance.GetClass("Barbarian");
		//Player.instance._Classes[1] = GameData.instance.GetClass("Rogue");
		//Player.instance._Classes[2] = GameData.instance.GetClass("Wizard");
		//Player.instance._Classes[3] = GameData.instance.GetClass("Bard");
		GameManager.TuteActive = true;
		DefaultMenu.SetActive(false);
		ClassMenu.SetActive(false);
		GameManager.instance.LoadGame(false);
	}

	public IEnumerator SetAlert()
	{
		AlertObj.SetActive(true);
		yield return new WaitForSeconds(0.3F);
		AlertObj.SetActive(false);
	}

	public void GetPips(ClassInfo Info)
	{
		for(int i = 0 ; i < HealthPipParent.transform.childCount; i++)
		{
			Destroy(HealthPipParent.transform.GetChild(i).gameObject);
		}
		for(int i = 0 ; i < AttackPipParent.transform.childCount; i++)
		{
			Destroy(AttackPipParent.transform.GetChild(i).gameObject);
		}
		for(int i = 0 ; i < MagicPipParent.transform.childCount; i++)
		{
			Destroy(MagicPipParent.transform.GetChild(i).gameObject);
		}

		for(int i = 0 ; i < Info.HealthRating; i++)
		{
			UIObj newpip = (UIObj) Instantiate(PipObj);
			newpip.transform.parent = HealthPipParent.transform;
			newpip.transform.localScale = Vector3.one;
			newpip._Image.color = GameData.Colour(GENUS.STR);
		}

		for(int i = 0 ; i < Info.AttackRating; i++)
		{
			UIObj newpip = (UIObj) Instantiate(PipObj);
			newpip.transform.parent = AttackPipParent.transform;
			newpip.transform.localScale = Vector3.one;
			newpip._Image.color = GameData.Colour(GENUS.DEX);
		}
		for(int i = 0 ; i < Info.MagicRating; i++)
		{
			UIObj newpip = (UIObj) Instantiate(PipObj);
			newpip.transform.parent = MagicPipParent.transform;
			newpip.transform.localScale = Vector3.one;
			newpip._Image.color = GameData.Colour(GENUS.WIS);
		}
	}

	public void SetOption(string s)
	{
		switch(s)
		{
			case "RealHP":
			Player.Options.RealHP = !Player.Options.RealHP;
			break;
			case "RealNumbers":
			Player.Options.ShowNumbers = !Player.Options.ShowNumbers;
			break;
			case "Intros":
			Player.Options.ShowIntroWaves = !Player.Options.ShowIntroWaves;
			break;
			case "Story":
			Player.Options.SkipAllStory = !Player.Options.SkipAllStory;
			break;
		}
		ResetOptions();
	}

	public void ShowHelp(int i)
	{
		switch(i)
		{
			case 0:
				HelpBasic.SetActive(null);
			break;
			case 1:
				HelpMana.SetActive(null);
			break;
			case 2:
				HelpItems.SetActive(null);
			break;
		}
	}


}
