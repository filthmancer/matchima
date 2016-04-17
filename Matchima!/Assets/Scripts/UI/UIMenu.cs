using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIMenu : UIObj {

	public UIObj ClassMenu;
	public UIObj PauseMenu;
	public UIObj OptionsMenu;
	public UIObj HelpMenu;
	public UIObj DefaultMenu;

	public UIObj ClassButton;
	public UIObj ResumeGame;
	public UIObj NewGame;
	public UIObj ClassLeft, ClassRight;

	public UIObj Difficulty;
	public UIObj StartGame;
	public UIObj LevelUp;
	public UIObj AlertObj;
	public UIObj PipObj;
	public UIObj HealthPipParent, AttackPipParent, MagicPipParent;

	public RectTransform ClassParent;

	public TextMeshProUGUI ClassName, ClassDescription;
	public TextMeshProUGUI Tokens, LevelUpCost;
	public TextMeshProUGUI ResumeGameInfo;

	public string [] DiffText;

	public UIObj [] SlotButtons;
	public int? TargetSlot;

	private List<UIObj> class_buttons = new List<UIObj>();
	private List<string> class_names = new List<string>();
	private List<string> class_short_names = new List<string>();
	private UIObj target_obj;

	private int selected_class = 0;
	private Vector3 ClassParent_pos = Vector3.zero;

	private List<GameObject> tiles = new List<GameObject>();
	private float time_to_tile = 0.6F;
	private float time = 0.0F;

	void Start () {
		_Text.gameObject.SetActive(true);
		Difficulty._Text.text = DiffText[(int)GameManager.instance.DifficultyMode];
		DefaultMenu.SetActive(true);
		ClassParent_pos = ClassParent.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Tokens.text = PlayerPrefs.GetInt("AllTokens") + " Tokens";

		ClassParent.transform.position = Vector3.Lerp(ClassParent.transform.position, ClassParent_pos, Time.deltaTime * 15);

		for(int i = 0; i < SlotButtons.Length; i++)
		{
			if(TargetSlot.HasValue)
			{
				SlotButtons[i].transform.localScale = Vector3.one * (i==TargetSlot.Value ? 1.3F : 1.15F);
				SlotButtons[i].Img[1].color = (i == TargetSlot.Value ? Color.white : Color.grey);
			}
			else
			{
				SlotButtons[i].transform.localScale = Vector3.one * 1.15F;
				SlotButtons[i].Img[1].color =  Color.grey;
			}
			
		}

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


	public void SetupClass()
	{
		if(PlayerPrefs.GetInt("Resume") == 1) 
		{
			ResumeGame.transform.gameObject.SetActive(true);
			ResumeGameInfo.text = PlayerPrefs.GetString("Name") + "\nTurn: " + PlayerPrefs.GetInt("Turns");
		}
		else ResumeGame.transform.gameObject.SetActive(false);

		for(int i = 0; i < GameData.instance.ClassesTest.Length; i++)
		{
			Class child = GameData.instance.ClassesTest[i];
			//if(child.NoAccess) continue;
			UIObj newclass = (UIObj)Instantiate(ClassButton);

			newclass._Image.color = 
				GameData.instance.GetGENUSColour(child != null ? child.Stats.Class_Type : GENUS.NONE);
			newclass._Image.sprite = child.Icon;


			//if(!child.Unlocked)
			//{
			//	newclass._Text.text = "???";
			//	newclass.GetComponent<Image>().color *= 0.5F;
			//}

			newclass.GetComponent<Button>().onClick.AddListener(() => SetClass(newclass));
			
			newclass.transform.SetParent(ClassParent.transform);
			newclass.transform.localScale = Vector3.one;
			newclass.Index = i;
			class_buttons.Add(newclass);
			class_names.Add(child.Info.Name);
			class_short_names.Add(child.Info.ShortName);
		}
		return;
		
		//Button endless = (Button)Instantiate(ClassButton);
		//endless.GetComponent<Image>().color = GameData.instance.GetGENUSColour(GENUS.PRP);
		//UIObj endlessObj = endless.GetComponent<UIObj>();
		//endlessObj._Text.text = "Endless";
		//endless.GetComponent<Button>().onClick.AddListener(() => SetClass(endlessObj));
		//endless.transform.SetParent(ClassParent.transform);
		//endless.transform.localScale = Vector3.one;
		//endlessObj.Index = GameData.instance.Classes.Length;
		//class_buttons.Add(endlessObj);
		//class_names.Add("The Endless");
		//class_short_names.Add("Endless");

	}

	Class targetClass;

	public void SetClass(UIObj button)
	{

		if(target_obj != button)
		{
			if(target_obj != null) 
			{
				target_obj._Text.text = (targetClass.Unlocked ?  class_short_names[target_obj.Index] : "???");
			}
			
			target_obj = button;
			button._Text.text = "OK?";

			targetClass =  GameData.instance.ClassesTest[button.Index];//GameManager.instance.CheckForClass(class_names[button.Index]);

			GetPips(targetClass.Info);
			ClassName.text = (targetClass.Unlocked ?  targetClass.Info.Name : "???");
			ClassDescription.text = (targetClass.Unlocked ?  targetClass.Info.Description : "???");
			//LevelUp.SetActive(targetClass.Unlocked && targetClass.Level < targetClass.MaxLevel);
			//LevelUp._Text.text = targetClass.LevelUpCost + " Tokens";
			StartGame._Text.text = "START";//(targetClass.Unlocked ? "START" : "UNLOCK");
		}
		else
		{
			ConfirmClass();
		}
	}

	public void SetClass(int i)
	{
		target_obj = class_buttons[i];
		targetClass = GameData.instance.ClassesTest[i];

		ClassName.text = (targetClass.Unlocked ?  targetClass.Info.Name : "???");
		ClassDescription.text = (targetClass.Unlocked ?  targetClass.Info.Description : "???");
		//LevelUp.SetActive(targetClass.Unlocked && targetClass.Level < targetClass.MaxLevel);
		//LevelUp._Text.text = targetClass.LevelUpCost + " Tokens";
		StartGame._Text.text = "START";//(targetClass.Unlocked ? "START" : "UNLOCK");
	}


	public void ConfirmClass()
	{
		if(targetClass == null) return;
		if(!targetClass.Unlocked) 
		{
			//if(PlayerPrefs.GetInt("AllTokens") >= targetClass.UnlockPoints)
			//{
			//	int newpoints = PlayerPrefs.GetInt("AllTokens") - targetClass.UnlockPoints;
			//	PlayerPrefs.SetInt("AllTokens", newpoints);
			//	targetClass.Unlocked = true;
			//	targetClass.Level = 1;
			//	class_buttons[selected_class]._Image.color *= 2.0F;
			//	class_buttons[selected_class]._Text.text = class_short_names[selected_class];
			//	SetClass(selected_class);
			//	//class_buttons[selected_class] = null;
			//}
		}
		else
		{
			//GameManager.instance.DifficultyMode = targetClass.Difficulty;
			//GameManager.instance.LoadClass(targetClass);
			Player.instance._Classes[TargetSlot.Value] = targetClass;
			SlotButtons[TargetSlot.Value]._Image.enabled = true;
			SlotButtons[TargetSlot.Value]._Image.sprite = targetClass.Icon;
			TargetSlot = null;
		}
	}

	public void LevelUpClass()
	{
		//if(targetClass == null) return;
		//if(!targetClass.Unlocked) return;
		//if(PlayerPrefs.GetInt("AllTokens") < targetClass.LevelUpCost) return;
		//int cost = targetClass.LevelUpCost;
		//if(!targetClass.LevelUp()) return;
		//int newpoints = PlayerPrefs.GetInt("AllTokens") - cost;
		//PlayerPrefs.SetInt("AllTokens", newpoints);
		//GameData.instance.LoadAbilities();
		//ClassName.text = (targetClass.Unlocked ?  targetClass.Name + (targetClass.MaxLevel != 0 ? (" : Lvl " + //targetClass.Level) : " ") : "???");
		//LevelUp.SetActive(targetClass.Unlocked && targetClass.Level < 5);
		//LevelUpCost.text = targetClass.LevelUpCost + " Tokens";
	}

	public void CheckClassButtons()
	{
		for(int i = 0; i < GameData.instance.Classes.Length; i++)
		{
			if(class_buttons.Count <= i) continue;
			ClassContainer c = GameData.instance.Classes[i];
			class_buttons[i]._Image.color =	GameData.instance.GetGENUSColour(c.Prefab != null ? c.Prefab.Stats.Class_Type : GENUS.NONE);
			if(c.Unlocked)
			{
				class_buttons[i]._Text.text = c.ShortName;
			}
			else
			{
				class_buttons[i]._Image.color *= 0.5F;
				class_buttons[i]._Text.text = "???";
			}
		}
	}

	public void SetTargetSlot(int i)
	{
		TargetSlot = i;
	}

	public void ChangeDifficulty()
	{
		GameManager.instance.DifficultyMode = GameManager.instance.DifficultyMode + 1;
		if(GameManager.instance.DifficultyMode > (DiffMode)2) GameManager.instance.DifficultyMode = 0;
		Difficulty._Text.text = DiffText[(int)GameManager.instance.DifficultyMode];
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
		GameManager.instance.LoadGame();
	}

	public void QuickStartActivate()
	{
		DefaultMenu.SetActive(false);
		Player.instance._Classes[0] = GameData.instance.GetClass("Barbarian");
		Player.instance._Classes[1] = GameData.instance.GetClass("Rogue");
		Player.instance._Classes[2] = GameData.instance.GetClass("Wizard");
		Player.instance._Classes[3] = GameData.instance.GetClass("Bard");
		
		NewGameActivate();
	}

	public void CustomStartActivate()
	{
		SetupClass();
		ClassMenu.SetActive(true);
		DefaultMenu.SetActive(false);
	}

	public void ResumeGameActivate()
	{
		DefaultMenu.SetActive(false);
	}

	public void OptionsActivate()
	{
		OptionsMenu.SetActive(true);
		DefaultMenu.SetActive(false);
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
		GameManager.instance.LoadGame();
	}

	public void RotateClass(int i)
	{
		selected_class = Mathf.Clamp(selected_class + i, 0, class_buttons.Count-1);
		ClassLeft.SetActive(selected_class != 0);
		ClassRight.SetActive(selected_class != class_buttons.Count - 1);

		ClassParent_pos.x = -2.45F * selected_class;
		SetClass(selected_class);
		
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


}
