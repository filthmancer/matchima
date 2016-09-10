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

	public UIObj ClassButton;

	public UIObj AlertObj;
	public UIObj PipObj;

	public UIClassSelect ClassPrefab;
	public List<UIClassSelect> ClassObjects;

	public RectTransform ClassParent;

	public TextMeshProUGUI ResumeGameInfo;
	public tk2dSpriteAnimator LogoAnimation;

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
	private int top_division_last = 0;

	private bool wedges_created;
	bool loaded = false;

	public IEnumerator LoadMenu()
	{
		State = MenuState.StartScreen;
		UIManager.Objects.BotGear[3].SetActive(true);
		
		if(!wedges_created)
		{
			wedges_created = true;
			int wedge_num = 8;
			for(int i = 0; i < wedge_num; i++)
			{
				if(GameData.instance.Classes.Length <= i) break;
				Class child = GameData.instance.Classes[i];
				UIClassSelect obj = (UIClassSelect) Instantiate(ClassPrefab);
				UIManager.Objects.BotGear[3].GetChild(0).AddChild(obj);
				obj.transform.SetParent(UIManager.Objects.BotGear[3].GetChild(0).transform);
				obj.Setup(child);
				obj.transform.rotation = Quaternion.Euler(0,0,360 - (360/wedge_num * i));
			}
		}

		for(int n = 0; n < UIManager.ClassButtons.Length; n++)
		{
			UIManager.ClassButtons.GetClass(n)._Sprite.enabled = true;
			UIManager.ClassButtons.GetClass(n)._Sprite.color = Color.white;
			UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = false;
			UIManager.ClassButtons.GetClass(n).Death.enabled = false;
		}
		
		UIManager.Objects.BotGear[3].GetChild(1).transform.SetAsLastSibling();
		UIManager.Objects.BotGear[3].GetChild(1).ClearActions();
		UIManager.Objects.BotGear[3].GetChild(1).AddAction(UIAction.MouseDown,
			() => {
				(UIManager.Objects.BotGear[3][0]).isPressed = true;
				(UIManager.Objects.BotGear[3][0] as UIGear).Drag = true;
				});
		UIManager.Objects.BotGear[3].GetChild(1).AddAction(UIAction.MouseUp,
			() => {
				(UIManager.Objects.BotGear[3][0]).isPressed = false;
				(UIManager.Objects.BotGear[3][0] as UIGear).Drag = false;
				});

		UIManager.Objects.BotGear[4].SetActive(false);
		UIManager.Objects.BotGear[3].GetChild(0).SetActive(false);

		UIManager.Objects.BotGear[3][2].transform.SetAsLastSibling();
		UIManager.Objects.BotGear[3][2].ClearActions();
		UIManager.Objects.BotGear[3][2].AddAction(UIAction.MouseUp,
			() => {
				
				bool filled = true;
				for(int i = 0; i < Player.instance._Classes.Length; i++)
				{
					if(Player.instance._Classes[i] == null)
					{
						PlayerPrefs.SetString("PrevClass_" + i, string.Empty);
						filled = false;
					}
					else PlayerPrefs.SetString("PrevClass_" + i, Player.instance._Classes[i].Name);
				}
				PlayerPrefs.SetInt("PrevClass", filled ? 1 : 0);
				MainMenu(false);
		});

		UIManager.instance.Health.Txt[0].text = "";
		UIManager.instance.Health.Txt[1].text = "";
		UIManager.instance.PlayerHealth[0].gameObject.SetActive(false);
		UIManager.instance.PlayerHealth[1].gameObject.SetActive(false);
		UIManager.Objects.BotGear[3].SetActive(false);

		
		yield return null;
		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,false);

		yield return null;
		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,true);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,true);
		yield return new WaitForSeconds(Time.deltaTime * 2);
		LogoAnimation.gameObject.SetActive(true);
		LogoAnimation.SetFrame(0);
		LogoAnimation.Play("Title Animation");
		yield return new WaitForSeconds(Time.deltaTime * 30);
		UIManager.Objects.BotGear.Txt[0].text = "TOUCH TO START";

		loaded = true;

		UIManager.Objects.BotGear[1].Img[0].enabled = false;

		if(PlayerPrefs.GetInt("Resume") == 1) 
		{	
			PlayerPrefs.SetInt("PrevMode", 1);
		}

		bool activated = false;
		UIManager.Objects.BotGear.ClearActions();
		UIManager.Objects.TopGear.ClearActions();
		UIManager.Objects.MiddleGear[0][2].ClearActions();
		UIManager.Objects.MiddleGear[0][2].AddAction(UIAction.MouseUp, ()=>{activated = true;});

		while(!activated)
		{
			yield return null;
		}

		yield return new WaitForSeconds(Time.deltaTime * 10);
		MainMenu(true);
		UIManager.Objects.TopGear.MoveToDivision(PlayerPrefs.GetInt("PrevMode"));
	}

	public void NewGameActivate()
	{
		
		UIManager.Objects.MiddleGear[0].SetActive(false);
		UIManager.Objects.MiddleGear.Img[0].enabled = false;
		UIManager.Objects.TopGear[2].SetActive(false);
		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,false);
		
		UIManager.Objects.TopGear[1][0].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][1].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][2].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][3].Txt[0].enabled = false;
		UIManager.Objects.BotGear[3].SetActive(false);
		UIManager.Objects.TopGear.isFlashing = false;
		UIManager.Objects.BotGear.isFlashing = false;
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;	
		(UIManager.Objects.BotGear as UIGear).SetTween(3, false);
		UIManager.Objects.BotGear[1].ClearActions();
		UIManager.Objects.BotGear[1].Img[0].enabled = false;

		StartCoroutine(GameManager.instance.LoadGame(false));
	}

	public void ResumeGameActivate()
	{
		Reset();
		(UIManager.Objects.MiddleGear["resume"] as UIObjTweener).SetTween(0, false);
		UIManager.Objects.MiddleGear[0].SetActive(false);
		UIManager.Objects.MiddleGear.Img[0].enabled = false;
		UIManager.Objects.TopGear[2].SetActive(false);
		UIManager.Objects.TopGear.Txt[0].text = "RESUMING\nGAME";
		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,false);
		

		UIManager.Objects.BotGear[3].SetActive(false);

		(UIManager.Objects.BotGear as UIGear).SetTween(3, false);
		UIManager.Objects.BotGear[1].ClearActions();
		UIManager.Objects.BotGear[1].Img[0].enabled = false;
		StartCoroutine(GameManager.instance.LoadGame(true));
	}
	
	public void Reset()
	{
		LogoAnimation.gameObject.SetActive(false);
		for(int i = 0; i < UIManager.ClassButtons.Length; i++)
		{
			UIManager.ClassButtons.GetClass(i).TweenClass(false);
		}
		
		UIManager.Objects.MiddleGear.Img[0].enabled = true;
		//UIManager.Objects.MiddleGear["resume"].SetActive(false);
		UIManager.Objects.TopGear.Txt[0].text = "";
		UIManager.Objects.MiddleGear.Txt[0].text = "";
		UIManager.Objects.BotGear.Txt[0].text = "";	
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(2, false);
		UIManager.Objects.BotGear[3][0].SetActive(true);
		UIManager.Objects.MiddleGear.SetActive(true);

		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,false);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,false);
		(UIManager.Objects.BotGear as UIGear).SetTween(3, false);
		
		(UIManager.Objects.TopGear as UIGear).SetRotate(false);
		(UIManager.Objects.BotGear as UIGear).SetRotate(false);
		UIManager.Objects.BotGear[0].SetActive(true);
		UIManager.Objects.BotGear[3].SetActive(false);
		//UIManager.Objects.TopGear[2].SetActive(true);
		UIManager.Objects.TopGear.Txt[0].text = "";

		UIManager.Objects.BotGear.ClearActions();
		UIManager.Objects.TopGear.ClearActions();
		UIManager.Objects.MiddleGear.ClearActions();

		UIManager.Objects.BotGear.DivisionActions.Clear();
		UIManager.Objects.TopGear.DivisionActions.Clear();
		UIManager.Objects.MiddleGear.DivisionActions.Clear();

		UIManager.Objects.TopGear.isFlashing = false;
		UIManager.Objects.BotGear.isFlashing = false;
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;
		UIManager.Objects.TopGear[1][0].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][1].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][2].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][3].Txt[0].enabled = false;

		
	}
	
	public void MainMenu(bool resume)
	{
		
		State = MenuState.MainMenu;
		Reset();

		UIManager.Objects.TopGear[1][0].Txt[0].enabled = true;
		UIManager.Objects.TopGear[1][1].Txt[0].enabled = true;
		UIManager.Objects.TopGear[1][2].Txt[0].enabled = true;
		UIManager.Objects.TopGear[1][3].Txt[0].enabled = true;

		UIManager.Objects.TopLeftButton.ClearActions();
		UIManager.Objects.TopLeftButton.AddAction(UIAction.MouseUp, () =>
		{
			UIManager.instance.ShowOptions();
		});

		UIManager.Objects.MiddleGear[3].ClearChildActions();
		UIManager.Objects.MiddleGear[3][0].AddAction(UIAction.MouseUp, ()=>
		{
			GameManager.instance.SaveAndQuit();
		});

		UIManager.Objects.MiddleGear[3][1].AddAction(UIAction.MouseUp, ()=>
		{
			GameManager.instance.Retire();
		});

		UIManager.Objects.MiddleGear[3][2].AddAction(UIAction.MouseUp, ()=>
		{
			AudioManager.PlaySFX = !AudioManager.PlaySFX;
			UIManager.instance.RefreshOptions();
		});

		UIManager.Objects.MiddleGear[3][3].AddAction(UIAction.MouseUp, ()=>
		{

			AudioManager.PlayMusic = !AudioManager.PlayMusic;
			UIManager.instance.RefreshOptions();
		});

		UIManager.Objects.MiddleGear[3][4].AddAction(UIAction.MouseUp, ()=>
		{
			Player.Options.CycleGameSpeed();
			UIManager.instance.RefreshOptions();
		});
		UIManager.Objects.MiddleGear["advancedops"].ClearActions();
		UIManager.Objects.MiddleGear["advancedops"].AddAction(UIAction.MouseUp, () =>
		{
			if(!GameData.FullVersion) UIManager.instance.ShowFullVersionAlert();
		});

		UIManager.Objects.TopRightButton.ClearActions();
		UIManager.Objects.TopRightButton.AddAction(UIAction.MouseUp, () =>
		{
			UIManager.instance.ShowPlayerLvl();
		});

		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, true);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,true);
		UIManager.Objects.TopRightButton.Txt[0].text = "Level";
		UIManager.Objects.TopRightButton.Txt[1].text = ""+Player.Level.Level;
		
		
		UIManager.Objects.TopGear.AddAction(UIAction.MouseDown,
			() => {UIManager.Objects.TopGear.Drag = true;});
		UIManager.Objects.TopGear.AddAction(UIAction.MouseUp,
			() => {UIManager.Objects.TopGear.Drag = false;});


		//UIManager.Objects.BotGear.AddAction(UIAction.MouseUp,
		//	() => {HeroMenu(0);});

		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1, false);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1, false);
		UIManager.ShowClassButtons(true);

		UIManager.Objects.MiddleGear.SetActive(true);
		UIManager.Objects.MiddleGear[0][0].Img[0].color = GameData.instance.GoodColour;
		(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
		(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);

		UIManager.Objects.BotGear[1].Img[0].enabled = true;

		UIManager.Objects.BotGear[3].SetActive(true);
		UIManager.Objects.BotGear[3][0].SetActive(true);
		(UIManager.Objects.BotGear[3][0] as UIGear).Drag = false;

		UIManager.Objects.TopGear[1][0].Txt[0].text = "STORY";
		UIManager.Objects.TopGear[1][1].Txt[0].text = "QUICK CRAWL";
		UIManager.Objects.TopGear[1][2].Txt[0].text = "RESUME";
		UIManager.Objects.TopGear[1][3].Txt[0].text = "ENDLESS";

		UIManager.Objects.TopGear.DoDivisionLerpActions = true;
		UIManager.Objects.TopGear.DivisionActions.Clear();
		UIManager.Objects.TopGear.DivisionActions.Add((int i) =>
		{
			GetMiddleGearInfo(i);
		});

		
		UIManager.Objects.TopGear.MoveToDivision(PlayerPrefs.GetInt("PrevMode"));
		GetMiddleGearInfo(PlayerPrefs.GetInt("PrevMode"));
		
		UIManager.Objects.TopGear.Txt[0].text = "";
		UIManager.Objects.BotGear.Txt[0].text = "";	

		UIManager.Objects.TopGear.FlashArrows();
		UIManager.Objects.BotGear.FlashArrows();
	}

	public void HeroMenu(int x)
	{
		if(!Player.instance.GetUnlock("charselect")) return;
		top_division_last = UIManager.Objects.TopGear.LastDivision;
		if(State != MenuState.Character)
		{
			State = MenuState.Character;
			UIManager.Objects.MiddleGear.SetActive(false);
			
			(UIManager.Objects.BotGear as UIObjTweener).SetTween(2, true);
			UIManager.Objects.BotGear[3][0].SetActive(true);
			UIManager.Objects.BotGear[3][0].Img[1].gameObject.SetActive(false);
			UIManager.Objects.BotGear.isFlashing = false;

			UIManager.Objects.TopGear[1][0].Txt[0].text = "CUSTOM";
			UIManager.Objects.TopGear[1][1].Txt[0].text = "PREVIOUS";
			UIManager.Objects.TopGear[1][2].Txt[0].text = "SAVED";
			UIManager.Objects.TopGear[1][3].Txt[0].text = "TEAM OF THE WEEK";

			if(PlayerPrefs.GetInt("PrevClass") == 1)
			{
				UIManager.Objects.TopGear.MoveToDivision(1);
				ReadClasses("PrevClass_");
				SetClassUI();
			}
			else if(PlayerPrefs.GetInt("SavedClass") == 1)
			{
				UIManager.Objects.TopGear.MoveToDivision(2);
				ReadClasses("SavedClass_");
				SetClassUI();

			}
			else UIManager.Objects.TopGear.MoveToDivision(0);

			(UIManager.Objects.BotGear[3][0] as UIGear).FlashArrows();	

			UIManager.Objects.BotGear[1].Img[0].enabled = false;
			
			(UIManager.Objects.BotGear[3][0] as UIGear).Drag = true;
			
			int wedge_num = 8;
			for(int i = 0; i < UIManager.Objects.BotGear[3][0].Length; i++)
			{
				Class child = GameData.instance.Classes[i];
				(UIManager.Objects.BotGear[3][0].GetChild(i) as UIClassSelect).ClearActions();
				(UIManager.Objects.BotGear[3][0].GetChild(i) as UIClassSelect).Setup(child); 
			}
			UIManager.Objects.TopGear.DivisionActions.Clear();
			UIManager.Objects.TopGear.DivisionActions.Add((int i) =>
			{
				GetHeroMenu_Info(i);
			});
			//StartCoroutine(ClearBot());
		}
		else
		{
			if(x == 100) return;
			SetTargetSlot(x);
		}
	}

	IEnumerator ClearBot()
	{
		yield return null;
		UIManager.Objects.BotGear.ClearActions();
		yield return null;
	}

	private void GetHeroMenu_Info(int i)
	{
		switch(i)
		{
			case 0:
			print("Custom Team");
			break;
			case 1:
			print("Saved");
			break;
			case 2:
			print("Challenge");
			break;
			case 3:
			print("Team of the Week");
			break;
		}
	}


	public void StartGame(GameMode g)
	{
		GameManager.instance.Mode = g;
		switch(GameManager.instance.Mode)
		{
			case GameMode.Endless:
			for(int i = 0; i < Player.instance._Classes.Length; i++)
			{
				if(Player.instance._Classes[i] == null)
				{
					HeroMenu(0);
					return;
				}
			}
			Reset();
			UIManager.Objects.BotGear[4].Txt[0].text = "";
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nENDLESS";
			break;
			case GameMode.Story:
			Player.instance._Classes[0] = null;//GameData.instance.GetClass("Barbarian");
			Player.instance._Classes[1] = null;//GameData.instance.GetClass("Rogue");
			Player.instance._Classes[2] = null;//GameData.instance.GetClass("Wizard");
			Player.instance._Classes[3] = null;//GameData.instance.GetClass("Bard");
			Reset();
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nSTORY";
			UIManager.Objects.BotGear[4].Txt[0].text = GameData.instance.StoryTip;
			break;
			case GameMode.Quick:
			for(int i = 0; i < Player.instance._Classes.Length; i++)
			{
				if(Player.instance._Classes[i] == null)
				{
					HeroMenu(0);
					return;
				}
			}
			Reset();
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nQUICK CRAWL";
			UIManager.Objects.BotGear[4].Txt[0].text = "";
			break;
		}
		(UIManager.Objects.MiddleGear["resume"] as UIObjTweener).SetTween(0,false);
		//NewGameActivate();

		UIManager.Objects.MiddleGear[0].SetActive(false);
		UIManager.Objects.MiddleGear.Img[0].enabled = false;
		UIManager.Objects.TopGear[2].SetActive(false);
		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,false);
		
		UIManager.Objects.TopGear[1][0].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][1].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][2].Txt[0].enabled = false;
		UIManager.Objects.TopGear[1][3].Txt[0].enabled = false;
		UIManager.Objects.BotGear[3].SetActive(false);
		UIManager.Objects.TopGear.isFlashing = false;
		UIManager.Objects.BotGear.isFlashing = false;
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;	
		(UIManager.Objects.BotGear as UIGear).SetTween(3, false);
		UIManager.Objects.BotGear[1].ClearActions();
		UIManager.Objects.BotGear[1].Img[0].enabled = false;
		
		StartCoroutine(GameManager.instance.LoadGame(false));
	}

	public Sprite NoHeroInSlot;
	public void GetMiddleGearInfo(int i)
	{
		if(i == 4) i = 0;
		UIManager.Objects.MiddleGear.AddSpin(6);
		bool unlocked = false;
		//top_division_last = UIManager.Objects.TopGear.LastDivision;
		//Reset();
		if(i!= 1) PlayerPrefs.SetInt("PrevMode", i);
		UIGear MidGear = UIManager.Objects.MiddleGear as UIGear;
		switch(i)
		{
			case 0: // STORY
			top_division_last = 0;
			UIManager.Objects.MiddleGear[0].Txt[0].text = "";
			//"FOUR ADVENTURERS BREAK INTO THE FORBIDDEN UNDERCITY TO EXPLORE AND GATHER THE PRECIOUS 'MANA' THAT SEEPS FROM BELOW";
			(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
			(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);

			UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();
			UIManager.Objects.MiddleGear[0].GetChild(1).AddAction(UIAction.MouseUp,
			() => {ChangeDifficulty();});

			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);
			UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
			() => {
				StartGame(GameMode.Story);
				});
			(UIManager.Objects.BotGear as UIGear).SetTween(3, true);

			

			UIManager.ClassButtons.GetClass(0)._Sprite.sprite = GameData.instance.GetClass("Barbarian").Icon;
			UIManager.ClassButtons.GetClass(1)._Sprite.sprite = GameData.instance.GetClass("Rogue").Icon;
			UIManager.ClassButtons.GetClass(2)._Sprite.sprite = GameData.instance.GetClass("Wizard").Icon;
			UIManager.ClassButtons.GetClass(3)._Sprite.sprite = GameData.instance.GetClass("Bard").Icon;

			for(int n = 0; n < UIManager.ClassButtons.Length; n++)
			{
				UIManager.ClassButtons.GetClass(n)._Sprite.enabled = true;
				UIManager.ClassButtons.GetClass(n)._Sprite.color = Color.white;
				UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = false;
			}

			break;

			case 1: // Resume

			
			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);
			UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
			() => {
				ResumeGameActivate();
				});

			MidGear[0].Txt[0].text = "RESUME GAME";
			(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
			(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);
			(UIManager.Objects.BotGear as UIGear).SetTween(3, true);

			ReadClasses("PrevClass_");
			SetClassUI();
			
			//ShowSettingsUI(true);
			break;

			case 2:  // Endless
			unlocked = GameData.instance.ModeUnlocked_Endless;
			top_division_last = 2;
			UIManager.Objects.MiddleGear[0].Txt[0].text = unlocked ? 
			"ENDLESSLY EXPLORE THE UNDERCITY, DELVING EVER DEEPER" : "LOCKED";
			
			UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();
			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);

			if(unlocked)
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);
				UIManager.Objects.MiddleGear[0].GetChild(1).AddAction(UIAction.MouseUp,
				() => {ChangeDifficulty();});
				UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
				() => {
					StartGame(GameMode.Endless);
				});
			}
			else
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
			}
			
			(UIManager.Objects.BotGear as UIGear).SetTween(3, !Player.instance.GetUnlock("charselect"));

			ReadClasses("PrevClass_");
			SetClassUI();
			break;

			case 3:  // QUICK CRAWL
			unlocked = GameData.instance.ModeUnlocked_Quick;
			top_division_last = 3;
			UIManager.Objects.MiddleGear[0].Txt[0].text = unlocked ? 
			"EXPLORE A GENERATED DUNGEON" : "Locked";

			UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();
			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);

			if(unlocked)
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);
				UIManager.Objects.MiddleGear[0].GetChild(1).AddAction(UIAction.MouseUp,
				() => {ChangeDifficulty();});
				UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
				() => {
					StartGame(GameMode.Quick);
					});
			}
			else 
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
			}
			
			(UIManager.Objects.BotGear as UIGear).SetTween(3, !Player.instance.GetUnlock("charselect"));
			ReadClasses("PrevClass_");
			SetClassUI();

			break;
		}
	}

	public void ReadClasses(string type)
	{
		for(int n = 0; n < Player.instance._Classes.Length; n++)
		{
			Class c = GameData.instance.GetClass(PlayerPrefs.GetString(type + n));
			if(c)
			{
				UIManager.ClassButtons.GetClass(n)._Sprite.enabled = true;
				UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = false;
				UIManager.ClassButtons.GetClass(n)._Sprite.sprite = c.Icon;
				Player.instance._Classes[n] = c;
			}
		}
		
	}

	public void SetClassUI()
	{
		for(int n = 0; n < UIManager.ClassButtons.Length; n++)
		{
			
			if(Player.instance._Classes[n] != null)
			{
				UIManager.ClassButtons.GetClass(n)._Sprite.sprite = Player.instance._Classes[n].Icon;
				UIManager.ClassButtons.GetClass(n)._Sprite.enabled = true;
				UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = false;
			}
			else 
			{
				UIManager.ClassButtons.GetClass(n)._Sprite.enabled = false;
				UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = true;
				UIManager.ClassButtons.GetClass(n)._SpriteMask.sprite = NoHeroInSlot;
			}
		}
	}

	public void SetTargetSlot(int i)
	{
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;	
		if(TargetSlot == i)
		{
			UIManager.ClassButtons.GetClass(TargetSlot.Value).TweenClass(false);
			TargetSlot = null;
			return;
		}
		else
		{
			if(TargetSlot != null) UIManager.ClassButtons.GetClass(TargetSlot.Value).TweenClass(false);
			TargetSlot = i;
			if(TargetSlot != null) UIManager.ClassButtons.GetClass(TargetSlot.Value).TweenClass(true);
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
		UIManager.ClassButtons.GetClass(TargetSlot.Value).Setup(c._class);
		UIManager.ClassButtons.GetClass(TargetSlot.Value).QuickPopup(0.1F);
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;	
		UIManager.ClassButtons.GetClass(TargetSlot.Value).TweenClass(false);
		//If targetslot was initally null, set back to null
		if(set_from_null) TargetSlot = null;
	}

	public void ShowSettingsUI(bool open)
	{
		Color targ;
		UIGear Options = UIManager.Objects.MiddleGear[0] as UIGear;
			Options[3].SetActive(open);
			if(!open) return;

			Options[3][0].Txt[0].text = "Real\nHP";
			 targ = Player.Options.RealHP ? GameData.instance.GoodColour : GameData.instance.BadColour;
			Options[3][0].SetInitCol(targ);
			Options[3][0].Img[0].color = targ;

			Options[3][0].ClearActions();
			Options[3][0].AddAction(UIAction.MouseUp, () =>
			{
				Player.Options.RealHP = !Player.Options.RealHP;	
				ShowSettingsUI(true);
			});

			Options[3][1].Txt[0].text = "Show\nNumbers";
			 targ = Player.Options.ShowNumbers ? GameData.instance.GoodColour : GameData.instance.BadColour;
			Options[3][1].SetInitCol(targ);
			Options[3][1].Img[0].color = targ;
			
			Options[3][1].ClearActions();
			Options[3][1].AddAction(UIAction.MouseUp, () =>
			{
				Player.Options.ShowNumbers = !Player.Options.ShowNumbers;	
				ShowSettingsUI(true);
			});


			string title = Player.Options.ShowStory == Ops_Story.Default ? "DEFAULT\nSTORY" : 
						Player.Options.ShowStory == Ops_Story.AlwaysShow ? "ALWAYS \nSHOW \nSTORY" :
						"NEVER\nSHOW\nSTORY";
			 targ = Player.Options.ShowStory == Ops_Story.Default ? GameData.Colour(GENUS.DEX) : 
						Player.Options.ShowStory == Ops_Story.AlwaysShow ? GameData.Colour(GENUS.WIS) :
						GameData.Colour(GENUS.STR);

			Options[3][2].Txt[0].text = title;									
			Options[3][2].SetInitCol(targ);
			Options[3][2].Img[0].color = targ;
			Options[3][2].ClearActions();
			Options[3][2].AddAction(UIAction.MouseUp, () =>
			{
				switch(Player.Options.ShowStory)
				{
					case Ops_Story.Default:
						Player.Options.ShowStory = Ops_Story.AlwaysShow;
					break;
					case Ops_Story.AlwaysShow:
						Player.Options.ShowStory = Ops_Story.NeverShow;
					break;
					case Ops_Story.NeverShow:
						Player.Options.ShowStory = Ops_Story.Default;
					break;
				}	
				ShowSettingsUI(true);
			});


			Options[3][3].SetActive(false);
			Options[3][3].Txt[0].text = "Real HP";
			Options[3][3].Img[0].color = Player.Options.RealHP ? GameData.instance.GoodColour : GameData.instance.BadColour;
			Options[3][3].ClearActions();
			Options[3][3].AddAction(UIAction.MouseUp, () =>
			{
				Player.Options.RealHP = !Player.Options.RealHP;	
			});
	}

	public void ChangeDifficulty()
	{
		GameManager.instance.DifficultyMode = GameManager.instance.DifficultyMode + 1;
		if(GameManager.instance.DifficultyMode > (DiffMode)2) GameManager.instance.DifficultyMode = 0;
		UIManager.Objects.MiddleGear[0].GetChild(1).Txt[0].text = "" + GameManager.instance.DifficultyMode;


		switch(GameManager.instance.DifficultyMode)
		{
			case DiffMode.Easy:
				UIManager.Objects.MiddleGear[0][1].SetInitCol(GameData.Colour(GENUS.WIS));
			break;
			case DiffMode.Okay:
				UIManager.Objects.MiddleGear[0][1].SetInitCol(GameData.Colour(GENUS.CHA));
			break;
			case DiffMode.Hard:
				UIManager.Objects.MiddleGear[0][1].SetInitCol(GameData.Colour(GENUS.STR));
			break;
		}
		
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
	}

	public void ResetOptions()
	{
		//OptionsMenu["RealNumbers"].BooleanObjColor(Player.Options.ShowNumbers);
		//OptionsMenu["RealHP"].BooleanObjColor(Player.Options.RealHP);
		//OptionsMenu["Intros"].BooleanObjColor(Player.Options.ShowIntroWaves);
		//OptionsMenu["Story"].BooleanObjColor(!Player.Options.SkipAllStory);
	}

	public void TutorialActivate()
	{
		//Player.instance._Classes[0] = GameData.instance.GetClass("Barbarian");
		//Player.instance._Classes[1] = GameData.instance.GetClass("Rogue");
		//Player.instance._Classes[2] = GameData.instance.GetClass("Wizard");
		//Player.instance._Classes[3] = GameData.instance.GetClass("Bard");
		GameManager.TuteActive = true;
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
		/*for(int i = 0 ; i < HealthPipParent.transform.childCount; i++)
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
		}*/
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
		}
		ResetOptions();
	}

	public void ShowHelp(int i)
	{
		/*switch(i)
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
		}*/
	}
}