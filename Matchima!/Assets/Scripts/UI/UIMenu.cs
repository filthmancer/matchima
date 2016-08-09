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

	bool loaded = false;

	public IEnumerator LoadMenu()
	{
		State = MenuState.StartScreen;
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

		UIManager.Objects.BotGear[3].GetChild(1).transform.SetAsLastSibling();
		UIManager.Objects.BotGear[3].GetChild(0).AddAction(UIAction.MouseDown,
			() => {(UIManager.Objects.BotGear[3][0] as UIGear).Drag = true;});
		UIManager.Objects.BotGear[3].GetChild(0).AddAction(UIAction.MouseUp,
			() => {(UIManager.Objects.BotGear[3][0] as UIGear).Drag = false;});
		UIManager.Objects.BotGear[3].GetChild(0).SetActive(false);

		UIManager.Objects.BotGear[3].GetChild(2).transform.SetAsLastSibling();
		UIManager.Objects.BotGear[3].GetChild(2).AddAction(UIAction.MouseUp,
			() => {MainMenu(false);});
		
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
			UIObj res = UIManager.Objects.MiddleGear["resume"];	
			res.AddAction(UIAction.MouseUp, ()=>
			{
				ResumeGameActivate();
			});
			(res as UIObjTweener).SetTween(0, true);

		}
		bool activated = false;
		UIManager.Objects.BotGear.AddAction(UIAction.MouseUp, ()=>{activated = true;});
		UIManager.Objects.TopGear.AddAction(UIAction.MouseUp, ()=>{activated = true;});
		UIManager.Objects.MiddleGear.AddAction(UIAction.MouseUp, ()=>{activated = true;});

		while(!activated)
		{
			yield return null;
		}

		yield return new WaitForSeconds(Time.deltaTime * 10);
		MainMenu(true);
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
		GameManager.instance.LoadGame(false);
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
		GameManager.instance.LoadGame(true);
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
			() => {(UIManager.Objects.TopGear as UIGear).Drag = true;});
		UIManager.Objects.TopGear.AddAction(UIAction.MouseUp,
			() => {(UIManager.Objects.TopGear as UIGear).Drag = false;});

		(UIManager.Objects.TopGear as UIObjTweener).SetTween(1, false);
		(UIManager.Objects.BotGear as UIObjTweener).SetTween(1, false);
		UIManager.ShowClassButtons(true);

		UIManager.Objects.MiddleGear.SetActive(true);
		UIManager.Objects.MiddleGear[0][0].Img[0].color = GameData.instance.GoodColour;
		(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
		(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, true);

		UIManager.Objects.BotGear[1].Img[0].enabled = true;

		(UIManager.Objects.BotGear[3][0] as UIGear).Drag = false;

		UIManager.Objects.TopGear[1][0].Txt[0].text = "STORY";
		UIManager.Objects.TopGear[1][1].Txt[0].text = "QUICK CRAWL";
		UIManager.Objects.TopGear[1][2].Txt[0].text = "SETTINGS";
		UIManager.Objects.TopGear[1][3].Txt[0].text = "ENDLESS";

		UIManager.Objects.TopGear.DoDivisionLerpActions = true;
		UIManager.Objects.TopGear.DivisionActions.Clear();
		UIManager.Objects.TopGear.DivisionActions.Add((int i) =>
		{
			GetMiddleGearInfo(i);
		});

		
		UIManager.Objects.TopGear.MoveToDivision(top_division_last);
		GetMiddleGearInfo(top_division_last);
		UIManager.Objects.BotGear[3][0].SetActive(true);
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
			UIManager.Objects.TopGear[1][1].Txt[0].text = "SAVED";
			UIManager.Objects.TopGear[1][2].Txt[0].text = "CHALLENGE";
			UIManager.Objects.TopGear[1][3].Txt[0].text = "TEAM OF THE WEEK";
			UIManager.Objects.TopGear.MoveToDivision(0);
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
		}
		else
		{
			if(x == 100) return;
			SetTargetSlot(x);
		}
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
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nENDLESS";
			break;
			case GameMode.Story:
			Player.instance._Classes[0] = GameData.instance.GetClass("Barbarian");
			Player.instance._Classes[1] = GameData.instance.GetClass("Rogue");
			Player.instance._Classes[2] = GameData.instance.GetClass("Wizard");
			Player.instance._Classes[3] = GameData.instance.GetClass("Bard");
			Reset();
			UIManager.Objects.TopGear.Txt[0].text = "LOADING\nSTORY";
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
			break;
		}
		(UIManager.Objects.MiddleGear["resume"] as UIObjTweener).SetTween(0,false);
		NewGameActivate();
	}

	public Sprite NoHeroInSlot;
	public void GetMiddleGearInfo(int i)
	{
		if(i == 4) i = 0;
		UIManager.Objects.MiddleGear.AddSpin(6);
		bool unlocked = false;
		//top_division_last = UIManager.Objects.TopGear.LastDivision;
		//Reset();
		UIGear MidGear = UIManager.Objects.MiddleGear as UIGear;
		switch(i)
		{
			case 0: // STORY
			top_division_last = 0;
			ShowSettingsUI(false);
			UIManager.Objects.MiddleGear[0].Txt[0].text = 
			"FOUR ADVENTURERS BREAK INTO THE FORBIDDEN UNDERCITY TO EXPLORE AND GATHER THE PRECIOUS 'MANA' THAT SEEPS FROM BELOW";
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

			case 1: // Settings

			
			MidGear[0].Txt[0].text = "";
			(MidGear[0][0] as UIObjTweener).SetTween(0, false);
			(MidGear[0][1] as UIObjTweener).SetTween(0, false);
			(UIManager.Objects.BotGear as UIGear).SetTween(3, true);
			
			ShowSettingsUI(true);



			/*UIManager.Objects.MiddleGear[0].Txt[0].text = 
			"RESUME GAME\n" + 
			"Turn: " + PlayerPrefs.GetInt("Turns");
			(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, true);
			(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
			UIManager.Objects.MiddleGear[0].GetChild(1).ClearActions();

			UIManager.Objects.MiddleGear[0].GetChild(0).ClearActions(UIAction.MouseUp);
			UIManager.Objects.MiddleGear[0].GetChild(0).AddAction(UIAction.MouseUp,
			() => {
				ResumeGameActivate();
				});*/
			break;

			case 2:  // Endless
			unlocked = GameData.instance.ModeUnlocked_Endless;
			top_division_last = 2;
			ShowSettingsUI(false);
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

			for(int n = 0; n < UIManager.ClassButtons.Length; n++)
			{
				if(Player.instance._Classes[n] == null)
				{
					UIManager.ClassButtons.GetClass(n)._Sprite.enabled = false;
					UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = true;
					UIManager.ClassButtons.GetClass(n)._SpriteMask.sprite = NoHeroInSlot;
				}
				else
				{

					UIManager.ClassButtons.GetClass(n)._Sprite.sprite = Player.instance._Classes[n].Icon;
					UIManager.ClassButtons.GetClass(n)._Sprite.enabled = true;
					UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = false;
				}
				
			}
			break;

			case 3:  // QUICK CRAWL
			unlocked = GameData.instance.ModeUnlocked_Quick;
			ShowSettingsUI(false);
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
			for(int n = 0; n < UIManager.ClassButtons.Length; n++)
			{
				if(Player.instance._Classes[n] == null)
				{
					UIManager.ClassButtons.GetClass(n)._Sprite.enabled = false;
					UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = true;
					UIManager.ClassButtons.GetClass(n)._SpriteMask.sprite = NoHeroInSlot;
				}
				else
				{
					UIManager.ClassButtons.GetClass(n)._Sprite.sprite = Player.instance._Classes[n].Icon;
					UIManager.ClassButtons.GetClass(n)._Sprite.enabled = true;
					UIManager.ClassButtons.GetClass(n)._SpriteMask.enabled = false;
				}
			}
			break;
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
		//If targetslot was initally null, set back to null
		if(set_from_null) TargetSlot = null;
	}

	public void ShowSettingsUI(bool open)
	{
		Color targ;
		UIGear MidGear = UIManager.Objects.MiddleGear as UIGear;
			MidGear[0][3].SetActive(open);
			if(!open) return;

			MidGear[0][3][0].Txt[0].text = "Real\nHP";
			 targ = Player.Options.RealHP ? GameData.instance.GoodColour : GameData.instance.BadColour;
			MidGear[0][3][0].SetInitCol(targ);
			MidGear[0][3][0].Img[0].color = targ;

			MidGear[0][3][0].ClearActions();
			MidGear[0][3][0].AddAction(UIAction.MouseUp, () =>
			{
				Player.Options.RealHP = !Player.Options.RealHP;	
				ShowSettingsUI(true);
			});

			MidGear[0][3][1].Txt[0].text = "Show\nNumbers";
			 targ = Player.Options.ShowNumbers ? GameData.instance.GoodColour : GameData.instance.BadColour;
			MidGear[0][3][1].SetInitCol(targ);
			MidGear[0][3][1].Img[0].color = targ;
			
			MidGear[0][3][1].ClearActions();
			MidGear[0][3][1].AddAction(UIAction.MouseUp, () =>
			{
				Player.Options.ShowNumbers = !Player.Options.ShowNumbers;	
				ShowSettingsUI(true);
			});


			string title = Player.Options.StorySet == Ops_Story.Default ? "DEFAULT\nSTORY" : 
						Player.Options.StorySet == Ops_Story.AlwaysShow ? "ALWAYS \nSHOW \nSTORY" :
						"NEVER\nSHOW\nSTORY";
			 targ = Player.Options.StorySet == Ops_Story.Default ? GameData.Colour(GENUS.DEX) : 
						Player.Options.StorySet == Ops_Story.AlwaysShow ? GameData.Colour(GENUS.WIS) :
						GameData.Colour(GENUS.STR);

			MidGear[0][3][2].Txt[0].text = title;									
			MidGear[0][3][2].SetInitCol(targ);
			MidGear[0][3][2].Img[0].color = targ;
			MidGear[0][3][2].ClearActions();
			MidGear[0][3][2].AddAction(UIAction.MouseUp, () =>
			{
				switch(Player.Options.StorySet)
				{
					case Ops_Story.Default:
						Player.Options.StorySet = Ops_Story.AlwaysShow;
					break;
					case Ops_Story.AlwaysShow:
						Player.Options.StorySet = Ops_Story.NeverShow;
					break;
					case Ops_Story.NeverShow:
						Player.Options.StorySet = Ops_Story.Default;
					break;
				}	
				ShowSettingsUI(true);
			});


			MidGear[0][3][3].SetActive(false);
			MidGear[0][3][3].Txt[0].text = "Real HP";
			MidGear[0][3][3].Img[0].color = Player.Options.RealHP ? GameData.instance.GoodColour : GameData.instance.BadColour;
			MidGear[0][3][3].ClearActions();
			MidGear[0][3][3].AddAction(UIAction.MouseUp, () =>
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
		Player.instance._Classes[0] = GameData.instance.GetClass("Barbarian");
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