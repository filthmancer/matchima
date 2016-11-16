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

#region Variables
	public UIObj ClassButton;

	public UIObj AlertObj;
	public UIObj PipObj;

	public UIClassSelect ClassPrefab;
	public List<UIClassSelect> ClassObjects;

	public RectTransform ClassParent;

	public TextMeshProUGUI ResumeGameInfo;
	public tk2dSpriteAnimator LogoAnimation;
	public UIObjTweener GoButton, TwoButton, ThreeButton;

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

#endregion

#region Loading
	public IEnumerator LoadMenu()
	{
		State = MenuState.StartScreen;
		UIManager.Objects.BotGear[3].SetActive(true);
		
		/*if(!wedges_created)
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
		}*/

		/*for(int n = 0; n < UIManager.CrewButtons.Length; n++)
		{
			UIClassButton b = UIManager.CrewButtons.GetClass(n);
			b._Sprite.enabled = true;
			b._Sprite.color = Color.white;
			b._SpriteMask.enabled = false;
			b.Death.enabled = false;
			b.Txt[0].text = "";
		}*/
		
		/*UIManager.Objects.BotGear[3].GetChild(1).transform.SetAsLastSibling();
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

		//UIManager.Objects.BotGear[3][2].transform.SetAsLastSibling();
		UIManager.Objects.BotGear[3][2].ClearActions();
		UIManager.Objects.BotGear[3][2].AddAction(UIAction.MouseUp,
			() => {
				
				SetPrevClasses();

				MainMenu(false);
		});
		UIManager.Objects.BotGear[3][2].SetActive(false);


		UIManager.Objects.BotGear[3][3].ClearActions();
		UIManager.Objects.BotGear[3][3].AddAction(UIAction.MouseUp,
			() => {
				(UIManager.Objects.BotGear[3][0] as UIGear).MoveLeft();
		});
		UIManager.Objects.BotGear[3][3].SetActive(false);

		UIManager.Objects.BotGear[3][4].ClearActions();
		UIManager.Objects.BotGear[3][4].AddAction(UIAction.MouseUp,
			() => {
				(UIManager.Objects.BotGear[3][0] as UIGear).MoveRight();
		});
		UIManager.Objects.BotGear[3][4].SetActive(false);

		UIManager.instance.SetHealthNotifier(false);*/
		
		UIManager.Objects.BotGear[3].SetActive(false);
		UIManager.Objects.TopGear[3].SetActive(false);
		UIManager.Objects.TopGear[4].SetActive(false);
		
		yield return null;
		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,false);

		yield return null;
		UIManager.Objects.TopGear.SetToState(1);
		UIManager.Objects.BotGear.SetToState(1);
		UIManager.instance.ShowGearTooltip(false);
		////(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,true);
		//(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,true);
		yield return new WaitForSeconds(Time.deltaTime * 2);


		LogoAnimation.gameObject.SetActive(true);
		LogoAnimation.SetFrame(0);
		LogoAnimation.Play("Title Animation");
		yield return new WaitForSeconds(Time.deltaTime * 30);


		AudioManager.instance.SetMusicClip(AudioManager.instance.HomeScreenMusic);
		//UIManager.Objects.BotGear.Txt[0].text = "TOUCH TO START";

		loaded = true;
		bool activated = false;

		GoButton.SetTween(0, true);
		GoButton.ClearActions();
		GoButton.AddAction(UIAction.MouseUp, () =>
		{
			StartGame(GameMode.Quick,1);
			});

		TwoButton.SetTween(0, true);
		TwoButton.ClearActions();
		TwoButton.AddAction(UIAction.MouseUp, () =>
		{
			StartGame(GameMode.Quick, 2);
			});

		ThreeButton.SetTween(0, true);
		ThreeButton.ClearActions();
		ThreeButton.AddAction(UIAction.MouseUp, () =>
		{
			StartGame(GameMode.Quick,3);
			});
		//UIManager.Objects.BotGear[1].Img[0].enabled = false;

		/*if(PlayerPrefs.GetInt("Resume") == 1) 
		{	
			if(GameData.FullVersion) 
			{
				(UIManager.Objects.MiddleGear["resume"] as UIObjTweener).SetTween(0, true);
				UIManager.Objects.MiddleGear["resume"].ClearActions();
				UIManager.Objects.MiddleGear["resume"].AddAction(UIAction.MouseUp, ()=>
				{
						ResumeGameActivate();
				});
			}
		}

		
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
		UIManager.Objects.TopGear.MoveToDivision(PlayerPrefs.GetInt("PrevMode"));*/
	}

	public void ResumeGameActivate()
	{
		Reset();
		UIManager.Objects.BotGear[4].Txt[0].text = "";
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
		StartCoroutine(GameManager.instance.LoadGame(true, true));
	}
	
	public void Reset()
	{
		LogoAnimation.gameObject.SetActive(false);
		/*for(int i = 0; i < UIManager.CrewButtons.Length; i++)
		{
			UIManager.CrewButtons.GetClass(i).TweenClass(false);
		}*/
		
		UIManager.Objects.MiddleGear.Img[0].enabled = true;
		//UIManager.Objects.MiddleGear["resume"].SetActive(false);
		UIManager.Objects.TopGear.Txt[0].text = "";
		UIManager.Objects.MiddleGear.Txt[0].text = "";
		UIManager.Objects.BotGear.Txt[0].text = "";	
		
		UIManager.Objects.BotGear[3][0].SetActive(true);
		UIManager.Objects.MiddleGear.SetActive(true);

		UIManager.Objects.TopGear.SetToState(0);
		UIManager.Objects.BotGear.SetToState(0);
		//(UIManager.Objects.TopGear as UIObjTweener).SetTween(1,false);
		//(UIManager.Objects.BotGear as UIObjTweener).SetTween(2, false);
		//(UIManager.Objects.BotGear as UIObjTweener).SetTween(1,false);
		//(UIManager.Objects.BotGear as UIGear).SetTween(3, false);
		
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

		UIManager.Objects.BotGear[3][2].SetActive(false);		
	}
	
	public void MainMenu(bool resume)
	{
		
		State = MenuState.MainMenu;
		Reset();

		UIManager.Objects.TopGear[1][0].Txt[0].enabled = true;
		UIManager.Objects.TopGear[1][1].Txt[0].enabled = true;
		UIManager.Objects.TopGear[1][2].Txt[0].enabled = true;
		UIManager.Objects.TopGear[1][3].Txt[0].enabled = true;

		UIManager.Objects.TopGear[3].SetActive(true);
		UIManager.Objects.TopGear[3].ClearActions();
		UIManager.Objects.TopGear[3].AddAction(UIAction.MouseUp, ()=>
		{	
			(UIManager.Objects.TopGear as UIGear).MoveRight();
			//print(MiddleGearInfo_current-1);
			GetMiddleGearInfo(MiddleGearInfo_current-1);
		});

		UIManager.Objects.TopGear[4].SetActive(true);
		UIManager.Objects.TopGear[4].ClearActions();
		UIManager.Objects.TopGear[4].AddAction(UIAction.MouseUp, ()=>
		{	
			(UIManager.Objects.TopGear as UIGear).MoveLeft();
			//print(MiddleGearInfo_current+1);
			GetMiddleGearInfo(MiddleGearInfo_current+1);
		});


		UIManager.Objects.TopLeftButton.ClearActions();
		UIManager.Objects.TopLeftButton.AddAction(UIAction.MouseUp, () =>
		{
			UIManager.instance.ShowOptions();
		});

		UIManager.Objects.MiddleGear[3].ClearChildActions();


		UIManager.Objects.MiddleGear[3][0].AddAction(UIAction.MouseUp, ()=>
		{
			GameManager.instance.Retire();
		});

		UIManager.Objects.MiddleGear[3][1].AddAction(UIAction.MouseUp, ()=>
		{
			AudioManager.instance.SetSFX();
			UIManager.instance.RefreshOptions();
		});

		UIManager.Objects.MiddleGear[3][2].AddAction(UIAction.MouseUp, ()=>
		{
			AudioManager.instance.SetMusic();
			UIManager.instance.RefreshOptions();
		});

		UIManager.Objects.MiddleGear["advancedops"].ClearActions();
		UIManager.Objects.MiddleGear["advancedops"].AddAction(UIAction.MouseUp, () =>
		{
			UIManager.instance.ShowAdvancedOptions();
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
		UIManager.Objects.TopGear.SetToState(0);
		//(UIManager.Objects.TopGear as UIObjTweener).SetTween(1, false);
		UIManager.Objects.BotGear.SetToState(0);
		//(UIManager.Objects.BotGear as UIObjTweener).SetTween(1, false);
		//UIManager.ShowCrewButtons(true);

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
		UIManager.Objects.TopGear[1][2].Txt[0].text = "SHOP";
		UIManager.Objects.TopGear[1][3].Txt[0].text = "OTHER CRAWLS";

		UIManager.Objects.TopGear.DoDivisionLerpActions = true;
		UIManager.Objects.TopGear.DivisionActions.Clear();
		UIManager.Objects.TopGear.DivisionActions.Add((int i) =>
		{
			PlayerPrefs.SetInt("FlashTop", 1);
			GetMiddleGearInfo(i);
		});

		
		UIManager.Objects.TopGear.MoveToDivision(PlayerPrefs.GetInt("PrevMode"));
		GetMiddleGearInfo(PlayerPrefs.GetInt("PrevMode"));
		
		UIManager.Objects.TopGear.Txt[0].text = "";
		UIManager.Objects.BotGear.Txt[0].text = "";	

		if(PlayerPrefs.GetInt("FlashTop") != 1) UIManager.Objects.TopGear.FlashArrows();
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
			
			UIManager.Objects.BotGear.SetToState(2);
			//(UIManager.Objects.BotGear as UIObjTweener).SetTween(2, true);
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

			if(PlayerPrefs.GetInt("FlashBot") != 1) (UIManager.Objects.BotGear[3][0] as UIGear).FlashArrows();	

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

			UIManager.Objects.BotGear[3][2].SetActive(true);
			UIManager.Objects.BotGear[3][3].SetActive(true);
			UIManager.Objects.BotGear[3][4].SetActive(true);

		}
		else
		{
			if(x == 100) return;
			SetTargetSlot(x);
		}
	}
#endregion
	
	private void GetHeroMenu_Info(int i)
	{
		PlayerPrefs.SetInt("FlashTop", 1);
		switch(i)
		{
			case 0:
			//print("Custom Team");
			break;
			case 1:
			//print("Saved");
			SetClasses(PlayerPrefs.GetString("SavedClass_0"), 
						PlayerPrefs.GetString("SavedClass_1"),
						PlayerPrefs.GetString("SavedClass_2"),
						PlayerPrefs.GetString("SavedClass_3"));
			break;
			case 2:
			//print("Team of the Week");
			SetClasses("Barbarian", "Rogue", "Rogue", "Wizard");
			
			break;
			case 3:
			//print("Previous");
			
			SetClasses(PlayerPrefs.GetString("PrevClass_0"), 
						PlayerPrefs.GetString("PrevClass_1"),
						PlayerPrefs.GetString("PrevClass_2"),
						PlayerPrefs.GetString("PrevClass_3"));
			break;
		}
	}

	public void SetPrevClasses()
	{
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
	}

	public void SetClasses(params string [] names)
	{
		for(int i = 0; i < Player.instance._Classes.Length; i++)
		{
			Player.instance._Classes[i] = GameData.instance.GetClass(names[i]);
		}
		SetClassUI();
	}


	public void StartGame(GameMode g, int pnum= 4)
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
			case GameMode.Deep:
			Reset();
			UIManager.Objects.BotGear[4].Txt[0].text = "";
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nTHE DEEP";
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

			for(int i = 0; i < pnum; i++)
			{
				Player.instance._Classes[i] = GameData.instance.GetClass("");
			}
			
			//Player.instance._Classes[1] = GameData.instance.GetClass("Rogue");
			//Player.instance._Classes[2] = GameData.instance.GetClass("Wizard");
			//Player.instance._Classes[3] = GameData.instance.GetClass("Bard");
			/*for(int i = 0; i < Player.instance._Classes.Length; i++)
			{
				if(Player.instance._Classes[i] == null)
				{
					HeroMenu(0);
					return;
				}
			}*/
			Reset();
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nQUICK CRAWL";
			UIManager.Objects.BotGear[4].Txt[0].text = "";
			break;
		}

		GoButton.SetTween(0, false);
		TwoButton.SetTween(0, false);
		ThreeButton.SetTween(0, false);
		/*(UIManager.Objects.MiddleGear["resume"] as UIObjTweener).SetTween(0,false);

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
		UIManager.Objects.BotGear[1].Img[0].enabled = false;*/
		
		StartCoroutine(GameManager.instance.LoadGame(false, GameManager.instance.Mode != GameMode.Story));
	}

	public Sprite NoHeroInSlot;
	public int MiddleGearInfo_current = 0;
	public void GetMiddleGearInfo(int i)
	{
		while(i >= 4) i -= 4;
		while(i < 0) i+=4;
		UIManager.Objects.MiddleGear.AddSpin(6);
		bool unlocked = false;
		
		if(i!= 1) PlayerPrefs.SetInt("PrevMode", i);
		UIGear MidGear = UIManager.Objects.MiddleGear as UIGear;
		UIManager.instance.ShowFullVersionAlert(false);
		UIManager.instance.SetHealthNotifier(false);
		MiddleGearInfo_current = i;
		switch(i)
		{
			case 0: // STORY
			top_division_last = 0;
			UIManager.Objects.MiddleGear[0].Txt[0].text = "LEARN THE ARTS OF MATCHING";
			(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
			(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
			(UIManager.Objects.MiddleGear[0][5] as UIObjTweener).SetTween(0, false);
			(UIManager.Objects.MiddleGear[0][6] as UIObjTweener).SetTween(0, false);

			UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();

			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);
			UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
			() => {
				StartGame(GameMode.Story);
				});
			(UIManager.Objects.BotGear as UIGear).SetTween(3, true);

			//UIManager.CrewButtons.GetClass(0)._Sprite.sprite = GameData.instance.GetClass("Barbarian").Icon;
			//UIManager.CrewButtons.GetClass(1)._Sprite.sprite = GameData.instance.GetClass("Rogue").Icon;
			//UIManager.CrewButtons.GetClass(2)._Sprite.sprite = GameData.instance.GetClass("Wizard").Icon;
			//UIManager.CrewButtons.GetClass(3)._Sprite.sprite = GameData.instance.GetClass("Bard").Icon;

			/*for(int n = 0; n < UIManager.CrewButtons.Length; n++)
			{
				UIManager.CrewButtons.GetClass(n)._Sprite.enabled = true;
				UIManager.CrewButtons.GetClass(n)._Sprite.color = Color.white;
				UIManager.CrewButtons.GetClass(n)._SpriteMask.enabled = false;
			}*/

			break;

			case 1: // Shop

			UIManager.instance.ShowFullVersionAlert(true);
			(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
			(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
			(UIManager.Objects.MiddleGear[0][5] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][6] as UIObjTweener).SetTween(0, false);

			UIManager.Objects.MiddleGear[0].Txt[0].text = "";
			(UIManager.Objects.BotGear as UIGear).SetTween(3, true);

			break;

			case 2:  // Other Modes
			bool unlocked_end = GameData.instance.ModeUnlocked_Endless;
			bool unlocked_deep = GameData.instance.ModeUnlocked_Deep;
			//unlocked = GameData.instance.ModeUnlocked_Endless;
			top_division_last = 2;
			UIManager.Objects.MiddleGear[0].Txt[0].text = (unlocked_end || unlocked_deep) ? 
			"" : "LOCKED";
			
			//UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();
			//UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);

			if(unlocked_end)
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][5] as UIObjTweener).SetTween(0, true);
				//(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);
				//UIManager.Objects.MiddleGear[0].GetChild(1).AddAction(UIAction.MouseUp,
				//() => {ChangeDifficulty();});
				UIManager.Objects.MiddleGear[0][5].ClearActions();
				UIManager.Objects.MiddleGear[0][5].AddAction(UIAction.MouseUp,
				() => {
					GameManager.instance.DifficultyMode = DiffMode.Okay;
					StartGame(GameMode.Endless);
				});
			}
			if(unlocked_deep)
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][6] as UIObjTweener).SetTween(0, true);
				//(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);
				//UIManager.Objects.MiddleGear[0].GetChild(1).AddAction(UIAction.MouseUp,
				//() => {ChangeDifficulty();});
				UIManager.Objects.MiddleGear[0][6].ClearActions();
				UIManager.Objects.MiddleGear[0][6].AddAction(UIAction.MouseUp,
				() => {
					GameManager.instance.DifficultyMode = DiffMode.Hard;
					StartGame(GameMode.Deep);
				});
			}
			if(!unlocked_deep && !unlocked_end)
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][5] as UIObjTweener).SetTween(0, false);
				(UIManager.Objects.MiddleGear[0][6] as UIObjTweener).SetTween(0, false);
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
			(UIManager.Objects.MiddleGear[0][5] as UIObjTweener).SetTween(0, false);
			(UIManager.Objects.MiddleGear[0][6] as UIObjTweener).SetTween(0, false);

			UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();
			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);

			if(unlocked)
			{
				(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
				(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);
				ChangeDifficulty(0);
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
				//UIManager.CrewButtons.GetClass(n)._Sprite.enabled = true;
				//UIManager.CrewButtons.GetClass(n)._SpriteMask.enabled = false;
				//UIManager.CrewButtons.GetClass(n)._Sprite.sprite = c.Icon;
				Player.instance._Classes[n] = c;
			}
		}
		
	}

	public void SetClassUI()
	{
		for(int n = 0; n < UIManager.CrewButtons.Length; n++)
		{
			if(Player.instance._Classes[n] != null)
			{
				//UIManager.CrewButtons.GetClass(n)._Sprite.sprite = Player.instance._Classes[n].Icon;
				//UIManager.CrewButtons.GetClass(n)._Sprite.enabled = true;
				//UIManager.CrewButtons.GetClass(n)._SpriteMask.enabled = false;
			}
			else 
			{
				//UIManager.CrewButtons.GetClass(n)._Sprite.enabled = false;
				//UIManager.CrewButtons.GetClass(n)._SpriteMask.enabled = true;
				//UIManager.CrewButtons.GetClass(n)._SpriteMask.sprite = NoHeroInSlot;
			}
		}
	}

	public void SetTargetSlot(int i)
	{
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;	
		PlayerPrefs.SetInt("FlashBot", 1);
		if(TargetSlot == i)
		{
		//	UIManager.CrewButtons.GetClass(TargetSlot.Value).TweenClass(false);
			TargetSlot = null;
			return;
		}
		else
		{
			//if(TargetSlot != null) UIManager.CrewButtons.GetClass(TargetSlot.Value).TweenClass(false);
			TargetSlot = i;
			//if(TargetSlot != null) UIManager.CrewButtons.GetClass(TargetSlot.Value).TweenClass(true);
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
		//UIManager.CrewButtons.GetClass(TargetSlot.Value).Setup(c._class);
		//UIManager.CrewButtons.GetClass(TargetSlot.Value).QuickPopup(0.1F);
		(UIManager.Objects.BotGear[3][0] as UIGear).isFlashing = false;	
		PlayerPrefs.SetInt("FlashBot", 1);
		GetHeroMenu_Info(0);
		UIManager.Objects.TopGear.MoveToDivision(0);
		//UIManager.CrewButtons.GetClass(TargetSlot.Value).TweenClass(false);
		//If targetslot was initally null, set back to null
		if(set_from_null) TargetSlot = null;
	}

	public void ChangeDifficulty(int rate = 1)
	{
		GameManager.instance.DifficultyMode = GameManager.instance.DifficultyMode + rate;
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

	public IEnumerator SetAlert()
	{
		AlertObj.SetActive(true);
		yield return new WaitForSeconds(0.3F);
		AlertObj.SetActive(false);
	}
}