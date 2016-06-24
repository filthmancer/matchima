using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour {
	public static UIManager instance;
	public static Canvas Canvas;
	public static bool loaded = false;
	public static UIObjects Objects;
	public static UIMenu Menu;

	public static int? LevelChoice = null;

	public Color BackingTint, WallTint;

	public TextMeshProUGUI Health, Armour;
	public TextMeshProUGUI WaveHealthText;
	public TextMeshProUGUI WaveTimer;

	public UITooltip Tooltip;

	public UIKillScreen KillUI;
	public UIObjTweener ScreenAlert;

	public ClassUpgradeUI ClassUI;
	public ClassAbilityUI ClassAbUI;

	public Image [] PlayerHealth;
	public Image [] WaveHealth;

	public ClassSlotsUI _ClassButtons;
	public static ClassSlotsUI ClassButtons{get{return UIManager.instance._ClassButtons;}}
	public static UIObj [] WaveButtons{get{return Objects.TopGear[1][0].Child;}}
	public ItemUI ItemUI;

	[HideInInspector]
	public Class current_class, targetui_class;
	[HideInInspector]
	public Quote current_quote;
	[HideInInspector]
	public Item current_item;

	public static bool InMenu;
	private int TopGear_lastdivision = 0;

	[HideInInspector]
	public bool ResUIOpen = false;

	private long current_tens = 0, current_hunds = 0, current_thous = 0;
	private int resource_store_index = 1;

	public static bool ItemUI_active, BoonUI_active;
	public static bool ShowingHealth, ShowingHit;

	private Vector3 ClassArmourBanner_pos;
	private List<TextMeshProUGUI> levelupinfo = new List<TextMeshProUGUI>();
	public bool isQuoting;

	public UIObj HealPowerUp;
	void Awake()
	{
		instance = this;
		Objects = this.GetComponent<UIObjects>();
		Canvas = this.transform.GetChild(0).GetComponent<Canvas>();
		Menu = Objects.MenuUI.GetComponent<UIMenu>();
	}

	void Start () {
		//Menu.gameObject.SetActive(true);
		
		//Objects.ShowObj(Objects.MenuUI, true);
		//Objects.ShowObj(Objects.StatsbarUI, false);
		Objects.MainUI.SetActive(false);	
		//UpdatePlayerUI();
		for(int i = 0; i < ClassButtons.Length; i++)
		{
			ClassButtons.GetClass(i)._Frame.sprite = TileMaster.Genus.Frame[i];
			ClassButtons.GetClass(i)._FrameMask.sprite = TileMaster.Genus.Frame[i];
		}
	}

	float update_interval_current = 0.0F;
	float update_interval_amount = 3;

	void Update () {

		if(!GameManager.instance.gameStart) return;


		if(Time.time > update_interval_current)
		{
			update_interval_current = Time.time + (Time.deltaTime * update_interval_amount);
		}
		else return;

		if(!Application.isEditor)
		{
			if(Input.touches.Length == 0) ShowGearTooltip(false);
		}

		Objects.TopRightButton.Txt[0].text = "" + GameManager.Floor;
		Objects.TopRightButton.Txt[1].text = "" + GameManager.ZoneNum;
		Objects.TopRightButton.Txt[2].enabled = Player.NewItems;
		Health.text = Player.Stats.Health + "/" + Player.Stats.HealthMax;

		for(int i = 0; i < PlayerHealth.Length; i++)
		{
			PlayerHealth[i].fillAmount = Mathf.Lerp(PlayerHealth[i].fillAmount, Player.Stats.GetHealthRatio()*0.88F, Time.deltaTime * 15);
			PlayerHealth[i].color = Color.Lerp(GameData.instance.ShieldEmpty, GameData.instance.ShieldFull, Player.Stats.GetHealthRatio());
		}
		if(GameManager.Wave != null)
		{
			WaveHealthText.text = ""+ GameManager.Wave.Current;
			for(int i = 0; i < WaveHealth.Length; i++)
			{
				WaveHealth[i].fillAmount = Mathf.Lerp(WaveHealth[i].fillAmount, GameManager.Wave.GetRatio()*0.88F, Time.deltaTime * 15);
				WaveHealth[i].color = Color.Lerp(GameData.instance.ShieldEmpty, GameData.instance.ShieldFull, GameManager.Wave.GetRatio());
			}
		}
		
		
		//CameraUtility.instance.MainLight.color = Color.Lerp(
			//CameraUtility.instance.MainLight.color, BackingTint, Time.deltaTime * 5);
		//Objects.Walls.Img[0].color = Color.Lerp(
			//Objects.Walls.Img[0].color, WallTint, Time.deltaTime * 5);

		Objects.ArmourParent.Txt[0].text = Player.Stats._Armour > 0 ? Player.Stats.Armour : "";
		Objects.ArmourParent.SetActive(Player.Stats._Armour > 0);

		if(GameManager.Wave != null)
		{
			for(int i = 0; i < GameManager.Wave.Length; i++)
			{
				if(GameManager.Wave[i] != null && GameManager.Wave[i].Active && Objects.TopGear[1].Length > i)
				{
					UIObj w = WaveButtons[i];
					w.SetActive(true);
					w.Txt[0].text = "";	
					w.Img[1].transform.gameObject.SetActive(true);
					w.Img[1].enabled = true;	
					w.Img[0].enabled = true;
					w.Img[0].sprite = GameManager.Wave[i].Inner;
					w.Img[0].color = Color.white;
					w.Img[2].enabled = true;
					w.Img[2].sprite = GameManager.Wave[i].Outer;
					w.Img[2].color = Color.white;					
				}
				else 
				{
					WaveButtons[i].SetActive(false);
				}
			}
						
		}
				
		if(!ShowingHealth && Player.Stats.HealThisTurn > 0)
		{
			StartCoroutine(HealLoop());
		}
		if(!ShowingHit && Player.Stats.DmgThisTurn > 0)
		{
			StartCoroutine(HitLoop());
		}
	}

	public void UpdatePlayerUI()
	{
		//ClassArmourBanner.gameObject.SetActive(Player.Stats.Armour > 0);
		int final = 0;
		int steps = Player.Stats._Armour;
		int scale = 1;
		while(steps > 0)
		{
			steps -= scale;
			scale *= 10;
			final++;
		}
		float dist = 0.23F * (final);
		dist += (Player.Stats._Armour > 0 ? 0.18F : 0.0F);
		ClassArmourBanner_pos = (Vector3.right * dist);
		
	}

	IEnumerator HealLoop()
	{
		if(ItemUI_active) yield break;
		ShowingHealth = true;

		int current_heal = Player.Stats.HealThisTurn;
		Vector3 tpos = Vector3.up * 0.35F + Vector3.left * 0.6F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.instance.Health.transform.position + tpos, 
			" +" + current_heal, 65,  GameData.instance.GoodColour, 1.7F,	0.01F);
		heal.transform.SetParent(UIManager.instance.Health.transform);
		heal.Txt[0].outlineColor = GameData.instance.GoodColourFill;
		while(heal.lifetime > 0.0F)
		{
			if(Player.Stats.HealThisTurn == 0)
			{
				yield return new WaitForSeconds(Time.deltaTime * 30);
				heal.lifetime = 0.0F;
				heal.text = "";
				break;
			}
			else if(Player.Stats.HealThisTurn != current_heal)
			{
				heal.lifetime += 0.5F;
				heal.size = 65 + current_heal * 0.75F;
				current_heal = Player.Stats.HealThisTurn;
				heal.text = " +" + current_heal;
			}
			yield return null;
		}

		ShowingHealth = false;
		//Player.Stats.CompleteHealth();
		yield return null;
	}

	IEnumerator HitLoop()
	{
		if(ItemUI_active) yield break;
		ShowingHit = true;

		int current_hit = Player.Stats.DmgThisTurn;
		Vector3 tpos = Vector3.up * 0.35F + Vector3.right * 0.6F;
		MiniAlertUI hit = UIManager.instance.MiniAlert(
			UIManager.instance.Health.transform.position + tpos, 
			" -" + current_hit, 65, GameData.instance.BadColour, 1.7F, 0.01F);
		hit.Txt[0].outlineColor = GameData.instance.BadColourFill;
		hit.transform.SetParent(UIManager.instance.Health.transform);

		while(hit.lifetime > 0.0F)
		{
			if(Player.Stats.DmgThisTurn == 0)
			{
				yield return new WaitForSeconds(Time.deltaTime * 30);
				hit.lifetime = 0.0F;
				hit.size = 65 + current_hit*0.75F;
				break;
			}
			if(Player.Stats.DmgThisTurn != current_hit)
			{
				hit.lifetime += 0.6F;
				current_hit = Player.Stats.DmgThisTurn;
			}
			hit.text = " -" + current_hit;

			yield return null;
		}

		ShowingHit = false;
		yield return null;
	}
	
	public void SwapSlotButtons(UISlotButton a, Class c, int slot)
	{
		UIClassButton _class = ClassButtons.GetClass(c.Index);
		UISlotButton b = _class.SlotUI[0] as UISlotButton;

		if(a != null && b != null)
		{
			Slot slota = a.slot;
			Slot slotb = b.slot;
			if(a.Parent != null)
			{
				//StartCoroutine(
					a.Parent.GetSlot(slotb, a.Index);
					ClassButtons.GetClass(a.Parent.Index).TweenClass(false);
			}
			if(b.Parent != null) 
			{
				//StartCoroutine(

					b.Parent.GetSlot(slota, b.Index);
					ClassButtons.GetClass(b.Parent.Index).TweenClass(false);
			}
			
			b.Setup(slota);
			a.Setup(slotb);
			b.Drag = DragType.None;
			a.Drag = DragType.None;

			_class.Setup(c);
		}
	}
	public void ShowKillUI(long alltokens, int tens, int hunds, int thous)
	{
		KillUI.Activate(alltokens, tens, hunds, thous);
	}


	public void ShowTooltip(bool Active, UIButton ab = null)
	{
		Tooltip.SetTooltip(Active, ab);
		//(Objects.TopGear as UIGear).MoveToDivision(Active ? 3 : TopGear_lastdivision);
		//Objects.TopGear[1][1][3].SetActive(Active);
	}


	public void MoveTopGear(int i, bool actions = false)
	{
		(Objects.TopGear as UIGear).DragLerpSpeed = 15;
		(Objects.TopGear as UIGear).DoDivisionLerpActions = actions;
		(Objects.TopGear as UIGear).MoveToDivision(i);
	}

	public void ResetTopGear()
	{
		(Objects.TopGear as UIGear).DragLerpSpeed = 15;
		(Objects.TopGear as UIGear).DoDivisionLerpActions = true;
		(Objects.TopGear as UIGear).MoveToDivision(TopGear_lastdivision);
	}

	Tile uitarget = null;
	public void ShowGearTooltip(bool active, Tile t = null)
	{
		if(!active) uitarget = null;

		(Objects.TopGear as UIGear).DragLerpSpeed = active ? 15 : 5;
		(Objects.TopGear as UIGear).DoDivisionLerpActions = !active;
		(Objects.TopGear as UIGear).MoveToDivision(active ? 3 : TopGear_lastdivision);
		//Objects.TopGear[1][1][3].SetActive(active);

		if(uitarget == t) return;
		uitarget = t;
		
		if(!active || t == null) return;

		Objects.TopGear[1][1][3].Txt[0].text = t._Name.Value;
		Objects.TopGear[1][1][3].Txt[0].color = t._Name.Colour;

		if(t.BaseDescription.Length > 0)
		{
			Objects.TopGear[1][1][3][0].SetActive(true);
			for(int i = 0; i < Objects.TopGear[1][1][3][0].Length; i++)
			{
				if(Objects.TopGear[1][1][3][0].GetChild(i) != null)
					Destroy(Objects.TopGear[1][1][3][0].GetChild(i).gameObject);
			}
			List<UIObj> newchildren = new List<UIObj>();
			
			Transform TopParent = Objects.TopGear[1][1][3][0].transform;
			UIObj ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
			Transform ParentTrans = ParentObj.transform;
			ParentTrans.SetParent(TopParent);
			ParentTrans.position = Vector3.zero;
			ParentTrans.localScale = Vector3.one;
			ParentTrans.localRotation = Quaternion.Euler(0,0,0);
			newchildren.Add(ParentObj);
			for(int i = 0; i < t.BaseDescription.Length; i++)
			{
				UIObj new_desc = (UIObj) Instantiate(Objects.TextObj);
				new_desc.transform.SetParent(ParentTrans);
				new_desc.transform.position = Vector3.zero;
				new_desc.transform.localScale = Vector3.one;
				new_desc.transform.localRotation = Quaternion.Euler(0,0,0);
				new_desc.Txt[0].text = t.BaseDescription[i].Value;
				new_desc.Txt[0].color = t.BaseDescription[i].Colour;

				if(t.BaseDescription[i].NewLine && i < t.BaseDescription.Length-1)
				{
					ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
					ParentTrans = ParentObj.transform;
					ParentTrans.SetParent(TopParent);
					ParentTrans.position = Vector3.zero;
					ParentTrans.localScale = Vector3.one;
					ParentTrans.localRotation = Quaternion.Euler(0,0,0);
					newchildren.Add(ParentObj);
				}
				Objects.TopGear[1][1][3][0].Child = newchildren.ToArray();
			}
		}
		else Objects.TopGear[1][1][3][0].SetActive(false);
		
		if(t.EffectDescription.Length > 0)
		{
			Objects.TopGear[1][1][3][1].SetActive(true);
			for(int i = 0; i < Objects.TopGear[1][1][3][1].Length; i++)
			{
				if(Objects.TopGear[1][1][3][1].GetChild(i) != null)
					Destroy(Objects.TopGear[1][1][3][1].GetChild(i).gameObject);
			}
			List<UIObj> newchildren = new List<UIObj>();
			
			Transform TopParent = Objects.TopGear[1][1][3][1].transform;
			UIObj ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
			Transform ParentTrans = ParentObj.transform;
			ParentTrans.SetParent(TopParent);
			ParentTrans.position = Vector3.zero;
			ParentTrans.localScale = Vector3.one;
			ParentTrans.localRotation = Quaternion.Euler(0,0,0);
			newchildren.Add(ParentObj);
			for(int i = 0; i < t.EffectDescription.Length; i++)
			{
				UIObj new_desc = (UIObj) Instantiate(Objects.TextObj);
				new_desc.transform.SetParent(ParentTrans);
				new_desc.transform.position = Vector3.zero;
				new_desc.transform.localScale = Vector3.one;
				new_desc.transform.localRotation = Quaternion.Euler(0,0,0);
				new_desc.Txt[0].text = t.EffectDescription[i].Value;
				new_desc.Txt[0].color = t.EffectDescription[i].Colour;

				if(t.EffectDescription[i].NewLine && i < t.EffectDescription.Length-1)
				{
					ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
					ParentTrans = ParentObj.transform;
					ParentTrans.SetParent(TopParent);
					ParentTrans.position = Vector3.zero;
					ParentTrans.localScale = Vector3.one;
					ParentTrans.localRotation = Quaternion.Euler(0,0,0);
					newchildren.Add(ParentObj);
				}
				Objects.TopGear[1][1][3][1].Child = newchildren.ToArray();
			}
		}
		else Objects.TopGear[1][1][3][1].SetActive(false);

		if(t.Description != null && t.Description.Length > 0)
		{
			Objects.TopGear[1][1][3][2].SetActive(true);
			for(int i = 0; i < Objects.TopGear[1][1][3][2].Length; i++)
			{
				if(Objects.TopGear[1][1][3][2].GetChild(i) != null)
					Destroy(Objects.TopGear[1][1][3][2].GetChild(i).gameObject);
			}
			List<UIObj> newchildren = new List<UIObj>();
			
			Transform TopParent = Objects.TopGear[1][1][3][2].transform;
			UIObj ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
			Transform ParentTrans = ParentObj.transform;
			ParentTrans.SetParent(TopParent);
			ParentTrans.position = Vector3.zero;
			ParentTrans.localScale = Vector3.one;
			ParentTrans.localRotation = Quaternion.Euler(0,0,0);
			newchildren.Add(ParentObj);
			for(int i = 0; i < t.Description.Length; i++)
			{
				UIObj new_desc = (UIObj) Instantiate(Objects.TextObj);
				new_desc.transform.SetParent(ParentTrans);
				new_desc.transform.position = Vector3.zero;
				new_desc.transform.localScale = Vector3.one;
				new_desc.transform.localRotation = Quaternion.Euler(0,0,0);
				new_desc.Txt[0].text = t.Description[i].Value;
				new_desc.Txt[0].color = t.Description[i].Colour;

				if(t.Description[i].NewLine && i < t.Description.Length-1)
				{
					ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
					ParentTrans = ParentObj.transform;
					ParentTrans.SetParent(TopParent);
					ParentTrans.position = Vector3.zero;
					ParentTrans.localScale = Vector3.one;
					ParentTrans.localRotation = Quaternion.Euler(0,0,0);
					newchildren.Add(ParentObj);
				}
				Objects.TopGear[1][1][3][2].Child = newchildren.ToArray();
			}
		}
		else Objects.TopGear[1][1][3][2].SetActive(false);		
	}

	public void ShowStats()
	{
		//Objects.ShowObj(Objects.StatsbarUI);
		//if(!ResUIOpen)
		//{
		//	Objects.ShowObj(Objects.BigUI);
		//}
	}

	public void ShowClassUI(bool active)
	{
		//Menu.ClassMenu.SetActive(active);
		//Objects.ShowObj(Objects.MainUI, !active);
	}

	public static void ShowClassButtons(bool? active = null)
	{
		foreach(UIObj child in ClassButtons.Child)
		{
			child.SetActive(active);
		}
	}

	public void SetClassButtons(bool open)
	{
		foreach(UIClassButton child in ClassButtons.Class)
		{
			if(child.PartialOpen.IsObjectOpened() != open)
			{
			 child.PartialOpen.OpenCloseObjectAnimation();
			}
		}
	}

	
	public static void ShowWaveButtons(bool? active = null)
	{
		foreach(UIObj child in WaveButtons)
		{
			child.SetActive(active);
		}
	}

	public void WaveButton(int i)
	{
		//if(GameManager.inStartMenu)
		//{
		//}
		//else
		//{
		//	//ShowTooltip()
		//}
	}

	public void ClassButton(int i )
	{

	}

	public void ShowClassAbilities(Class c, bool? over = null)
	{
		bool active = over ?? (current_class != c);
		current_class = (active ? c : null);
		targetui_class = (active ? c : null);
		for(int i = 0; i < ClassButtons.Length; i++)
		{
			ClassButtons.GetClass(i).Setup(ClassButtons.GetClass(i)._class);
		}
	}

	public void ShowItemUI(params Item [] i)
	{
		current_class = null;
		//Objects.ShowObj(Objects.BigUI,true);
		//Objects.ShowObj(Objects.ClassUpgradeUI,false);
		ItemUI.gameObject.SetActive(true);
		ItemUI_active = true;
		Objects.ShowObj(Objects.ItemUI, true);
		//current_item = i;

		ItemUI.Setup((Slot[] )i);
	}

	public void HideItemUI()
	{
		ItemUI.gameObject.SetActive(false);
		ItemUI_active = false;
		ItemUI.DestroySlots();
		ShowClassAbilities(null, false);
		current_class = null;
		targetui_class = null;
		UIManager.instance.SetClassButtons(false);
	}

	public void BotGearTween(bool? open = null)
	{
		Objects.ClassUI.SetActive(open);
	}

	public void TopGearTween(bool? open = null)
	{
		Objects.WaveUI.SetActive(open);
	}

	//public void ItemConfirm(Class c)
	//{
	//	c.GetSlot(current_item);
	//	if(!InMenu)Objects.ShowObj(Objects.BigUI, false);
	//	ItemUI.gameObject.SetActive(false);
	//	ClassAbUI.SetActive(false);
	//	current_item = null;
	//	current_class = null;
	//}

	public void ItemDestroy()
	{
		Destroy(current_item.gameObject);
		ItemUI.gameObject.SetActive(false);
		current_item = null;
	}


	public void ShowMenu(bool active)
	{
		//Menu.PauseMenu.SetActive(active);
		//Objects.ShowObj(Objects.MainUI, !active);
	}

	public void ShowOptions()
	{
		bool open = (UIManager.Objects.MiddleGear[3] as UIObjTweener).Tween.IsObjectOpened();
		if(!open && !GameManager.instance.paused)
		{
			ScreenAlert.SetTween(0,true);
			GameManager.instance.paused = true;
			(UIManager.Objects.MiddleGear[3] as UIObjTweener).SetTween(0, true);
		}
		else
		{
				ScreenAlert.SetTween(0,false);
				GameManager.instance.paused = false;
				(UIManager.Objects.MiddleGear[3] as UIObjTweener).SetTween(0, false);			
		}
	}

	public void ShowZoneMenu()
	{
		bool open = (UIManager.Objects.MiddleGear[1] as UIObjTweener).Tween.IsObjectOpened();
		if(!open && !GameManager.instance.paused)
		{
			ScreenAlert.SetTween(0,true);
			GameManager.instance.paused = true;
			(UIManager.Objects.MiddleGear[1] as UIObjTweener).SetTween(0, true);
		}
		else
		{
			if((UIManager.Objects.MiddleGear[1] as UIObjTweener).Tweens[0].IsObjectOpened())
			{
				ScreenAlert.SetTween(0,false);
				GameManager.instance.paused = false;
				(UIManager.Objects.MiddleGear[1] as UIObjTweener).SetTween(0, false);
			}
		}
	}

	public void ShowResourceUI()
	{
		InMenu = true;
		ResUIOpen = true;
		Objects.ShowObj(Objects.ItemUI, false);
		Objects.GetScoreWindow().gameObject.SetActive(false);

		ClassUI.GetInfo(Player.Classes[0]);
		current_class = Player.Classes[0];

		foreach(Tile child in PlayerControl.instance.selectedTiles)
		{
			child.Reset(true);
		}
		PlayerControl.instance.selectedTiles.Clear();

	}

	public IEnumerator LevelChoiceRoutine(Class c, params ClassUpgrade [] ups)
	{
		LevelChoice = null;
		InMenu = true;
		while(LevelChoice == null)
		{
			yield return null;
		}

		//yield return new WaitForSeconds(Time.deltaTime * 4);
		//
		yield return null;
	}

	public void SetChoice(int i)
	{
		LevelChoice = i;
	}

	public void OpenBoonUI(UpgradeGroup group)
	{
		current_class = group.Target;
		ShowClassAbilities(current_class, true);
		InMenu = true;
		BoonUI_active = true;
		LevelChoice = null;
	}

	public void HideBoonUI()
	{
		BoonUI_active = false;
		//LevelChoice = 1;
		PlayerControl.HeldButton = null;
		PlayerControl.HoldingSlot = false;
		InMenu = false;
		current_class = null;
	}

	public void HideResourceUI()
	{
		//Objects.ShowObj(Objects.BigUI, false);
		//Objects.ShowObj(Objects.BigUIShopAlert, false);
		//Objects.ShowObj(Objects.BigUIShopButtons, false);
		Objects.GetScoreWindow().gameObject.SetActive(true);
		foreach(Tile child in PlayerControl.instance.selectedTiles)
		{
			child.Reset(true);
		}
		current_class = null;
		targetui_class = null;
		Player.instance.ResetStats();
		Player.instance.ResetChances();
		PlayerControl.instance.selectedTiles.Clear();

		//Objects.ShowObj(Objects.StatsbarUI, false);

		InMenu = false;
		ResUIOpen = false;
	}

	public IEnumerator LoadUI()
	{
		yield return new WaitForSeconds(0.2F);
		//Menu.ClassMenu.SetActive(false);
		//Objects.ShowObj(Objects.Options, false);
		Objects.ShowObj(Objects.MainUI, true);
		(UIManager.Objects.TopLeftButton as UIObjTweener).SetTween(0, true);
		(UIManager.Objects.TopRightButton as UIObjTweener).SetTween(0,true);
		loaded  = true;
		int i =0;

		i = 0;
		foreach(UIClassButton child in ClassButtons.Class)
		{
			child.Setup(Player.Classes[i]);
			i++;
		}

		UIObj zone = Objects.MiddleGear[1] as UIObj;
		zone[0].AddAction(UIAction.MouseUp,() => {ShowStashUI();});
		zone[1].AddAction(UIAction.MouseUp,() => {GameManager.instance.EnterZone(GameManager.ZoneChoiceA);});
		zone[2].AddAction(UIAction.MouseUp,() => {GameManager.instance.EnterZone(GameManager.ZoneChoiceB);});

		Objects.MiddleGear[2][0].AddAction(UIAction.MouseUp, ()=>
		{
			(Objects.MiddleGear[2] as UIObjTweener).SetTween(0, false);
		});

		Objects.MiddleGear[3][0].AddAction(UIAction.MouseUp, () =>
		{
			ShowOptions();
		});
		Objects.MiddleGear[3][2].AddAction(UIAction.MouseUp, ()=>
		{
			GameManager.instance.Retire();
		});

		Objects.MiddleGear[3][1].AddAction(UIAction.MouseUp, ()=>
		{
			GameManager.instance.SaveAndQuit();
		});

		Objects.MiddleGear[1].SetActive(false);
		Objects.MiddleGear[2].SetActive(false);
		Objects.MiddleGear[3].SetActive(false);

		Objects.TopLeftButton.AddAction(UIAction.MouseUp, () =>
		{
			ShowOptions();
			
		});
		Objects.TopRightButton.AddAction(UIAction.MouseUp, () =>
		{
			//GameManager.instance.EscapeZone();
			Objects.TopGear.SetActive(false);
			Objects.MiddleGear.SetActive(false);
			Objects.BotGear.SetActive(false);

			//ShowZoneUI(false);
			//(Objects.MiddleGear[2] as UIObjTweener).SetTween(0, false);
			//UIManager.Objects.MiddleGear[1].Txt[0].text = GameManager.Zone.Name;
		});

		(Objects.TopGear as UIGear).DivisionActions.Add((int num) =>
		{
			TopGear_lastdivision = num;
		});

		
		yield return null;
	}

	public bool AlertShowing = false;
	public IEnumerator Alert(float time, bool show_floor, string title = null, string desc = null, string floor_override = null)
	{
		while(AlertShowing) yield return null;
		AlertShowing = true;
		GameManager.instance.paused = true;
		ScreenAlert.SetActive(true);
		ScreenAlert.SetTween(0,true);

		if(show_floor)
		{

			(ScreenAlert.Child[2] as UIObjTweener).Txt[0].text = "Floor ";
			(ScreenAlert.Child[2] as UIObjTweener).Txt[1].text = "" + GameManager.Floor;
			(ScreenAlert.Child[2] as UIObjTweener).SetTween(0, true);
			yield return new WaitForSeconds(GameData.GameSpeed(0.35F));
		}
		else
		{
			if(floor_override != null)
			{
				ScreenAlert.Child[2].Txt[0].text = floor_override;
				ScreenAlert.Child[2].Txt[1].text = "";
				(ScreenAlert.Child[2] as UIObjTweener).SetTween(0, true);
				yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
			}
		}

		if(title != null)
		{
			(ScreenAlert.Child[0] as UIObjTweener).Txt[0].text = title;
			(ScreenAlert.Child[0] as UIObjTweener).SetTween(0, true);
			yield return new WaitForSeconds(GameData.GameSpeed(0.35F));
		}

		if(desc != null)
		{
			(ScreenAlert.Child[1] as UIObjTweener).Txt[0].text = desc;
			(ScreenAlert.Child[1] as UIObjTweener).SetTween(0, true);
		}


		yield return new WaitForSeconds(GameData.GameSpeed(time));
		
		ScreenAlert.SetTween(0,false);
		(ScreenAlert.Child[0] as UIObjTweener).SetTween(0, false);
		(ScreenAlert.Child[1] as UIObjTweener).SetTween(0, false);
		(ScreenAlert.Child[2] as UIObjTweener).SetTween(0, false);

		PlayerControl.instance.ResetSelected();
		GameManager.instance.paused = false;
		AlertShowing = false;
		yield break;
	}

	public IEnumerator Quote(params Quote [] q)
	{
		while(isQuoting || InMenu) yield return null;
		isQuoting = true;
		Unit curr_parent = null;
		for(int i = 0; i < q.Length; i++)
		{
			current_quote = q[i];
			current_quote.Reset();
			TextMeshProUGUI targettext = null;
			if(current_quote.Parent is Class)
			{
				Objects.WaveQuote.SetActive(false);
				Objects.ClassQuote.SetActive(true);
				if(i == 0 || curr_parent != current_quote.Parent) 
				{
					Objects.ClassQuote.GetComponent<Animator>().SetTrigger("PopIn");
					curr_parent = current_quote.Parent;
				}
				targettext = Objects.ClassQuote.Txt[0];
				//Objects.ClassQuote.Txt[0].text = current_quote.Text;

				if(current_quote.Parent != null)
				{
					Objects.ClassQuote.Img[0].color = GameData.Colour(current_quote.Parent.Genus);
					for(int t = 1; t < Objects.ClassQuote.Img.Length; t++)
					{
						Objects.ClassQuote.Img[t].gameObject.SetActive(t-1 == current_quote.Parent.Index && current_quote.ShowTail);
					}
				}
			}
			else
			{
				Objects.ClassQuote.SetActive(false);
				Objects.WaveQuote.SetActive(true);
				if(i == 0 || curr_parent != current_quote.Parent) 
				{
					Objects.WaveQuote.GetComponent<Animator>().SetTrigger("PopIn");
					curr_parent = current_quote.Parent;
				}
				//Objects.WaveQuote.Txt[0].text = current_quote.Text;
				targettext = Objects.WaveQuote.Txt[0];
				if(current_quote.Parent != null)
				{
					Objects.WaveQuote.Img[0].color = GameData.Colour(current_quote.Parent.Genus);
					Objects.WaveQuote.Img[1].gameObject.SetActive(current_quote.ShowTail);
					Objects.WaveQuote.Img[1].color = GameData.Colour(current_quote.Parent.Genus);
				}
			}
			
			
			if(current_quote.OverrideTouch)
			{
				PlayerControl.instance.canMatch = false;
				GameManager.instance.paused = true;
			}

			while(!current_quote.CheckForAccept(targettext))
			{
				yield return null;
			}			
		}

		Objects.WaveQuote.SetActive(false);
		Objects.ClassQuote.SetActive(false);
		PlayerControl.instance.canMatch = true;
		GameManager.instance.paused = false;
		//current_quote = null;
		isQuoting = false;
		yield return null;
	}

	public IEnumerator Alert(params Quote [] q)
	{
		//while(isQuoting || InMenu) yield return null;
		//yield return null;
		//UIManager.Objects.LevelUpMenu.SetActive(false);
		isQuoting = true;
		for(int i = 0; i < q.Length; i++)
		{
			current_quote = q[i];
			//Objects.ShowObj(Objects.BigUI, true);
			Objects.ShowObj(Objects.AlertBubble.gameObject, true);
			if(i == 0) Objects.AlertBubble.GetComponent<Animator>().SetTrigger("PopIn");
			for(int old = 0; old < Objects.Alert_ButtonParent.transform.childCount; old++)
			{
				Destroy(Objects.Alert_ButtonParent.transform.GetChild(old).gameObject);
			}

			if(current_quote.SlotButtons)
			{
				Objects.Alert_ButtonParent.SetActive(true);
				Class _class = current_quote.Parent as Class;
				for(int s = 0; s < _class._Slots.Length; s++)
				{
					UIObj new_button = (UIObj) Instantiate(Objects.Alert_Button);
					new_button.transform.parent = Objects.Alert_ButtonParent.transform;
					new_button.transform.position = Vector3.zero;
					new_button.transform.localScale = Vector3.one;
					AddConfirmListener(new_button, s);
				}
				
				UIObj no = (UIObj) Instantiate(Objects.Alert_Button);
				no.transform.parent = Objects.Alert_ButtonParent.transform;
				no.transform.position = Vector3.zero;
				no.transform.localScale = Vector3.one;
				AddConfirmListener(no, _class._Slots.Length);
			}
			else if(current_quote.YesNoButtons)
			{
				Objects.Alert_ButtonParent.SetActive(true);
				UIObj yes = (UIObj) Instantiate(Objects.Alert_Button);
				yes.transform.parent = Objects.Alert_ButtonParent.transform;
				yes.transform.position = Vector3.zero;
				yes.transform.localScale = Vector3.one;
				AddConfirmListener(yes, 0);

				UIObj no = (UIObj) Instantiate(Objects.Alert_Button);
				no.transform.parent = Objects.Alert_ButtonParent.transform;
				no.transform.position = Vector3.zero;
				no.transform.localScale = Vector3.one;
				AddConfirmListener(no, 1);
			}
			else Objects.Alert_ButtonParent.SetActive(false);

			//Objects.ShowObj(Objects.QuoteBubble_YesNoButtons, current_quote.YesNoButtons);
			//Objects.ShowObj(Objects.QuoteBubble_SlotButtons, current_quote.SlotButtons);
			Objects.Alert.text = current_quote.Text;

			if(current_quote.Parent != null)
			{
				Objects.AlertBubble.GetComponent<Image>().color = GameData.Colour(current_quote.Parent.Genus);
			}
			
			if(current_quote.OverrideTouch)
			{
				PlayerControl.instance.canMatch = false;
				GameManager.instance.paused = true;
			}

			while(!current_quote.CheckForAccept())
			{
				yield return null;
			}			
		}

		//Objects.ShowObj(Objects.BigUI, false);
		Objects.ShowObj(Objects.AlertBubble.gameObject, false);
		PlayerControl.instance.canMatch = true;
		GameManager.instance.paused = false;
		//current_quote = null;
		isQuoting = false;
		yield return null;
	}

	public void AddConfirmListener(UIObj obj, int i)
	{
		obj.GetComponent<Button>().onClick.AddListener(() => QuoteConfirm(i));
	}

	public void QuoteConfirm(int num)
	{
		current_quote.answer = num;
	}

	public RectTransform GetResourceByTileType(Tile t)
	{
		if(t.Type.isHealth) return ClassButtons[0].transform as RectTransform;
		if(t.Genus == GENUS.CHA) return ClassButtons[3].transform as RectTransform;

		if(t.Genus == GENUS.DEX) return ClassButtons[1].transform as RectTransform;

		if(t.Genus == GENUS.WIS) return ClassButtons[2].transform as RectTransform;

		if(t.Genus == GENUS.STR) return ClassButtons[0].transform as RectTransform;

		return null;
	}

	public void ShowWaveInfo(bool active, int num)
	{
		if(GameManager.Wave == null) return;
		//ShowSimpleTooltip(active, Objects.TopGear[1][num].Img[1].transform, GameManager.Wave[num].Name, GameManager.Wave[num].Description);
	}

	public void AddClass(Class c, int slot)
	{
		ClassButtons.GetClass(slot).Setup(c);
		MiniAlert(Health.transform.position + Vector3.up * 1.3F,
	  	c.Name + " joined the party!", 50, GameData.Colour((GENUS)slot), 1.5F);
	}

	public void ShowZoneUI(bool ended)
	{
		(Objects.MiddleGear[1] as UIObjTweener).SetTween(0);
		bool open = (Objects.MiddleGear[1] as UIObjTweener).Tween.IsObjectOpened();
		GameManager.instance.paused = open;
		if(!open) UIManager.instance.SetClassButtons(false);
		if(ended)
		{
			Objects.MiddleGear[1].Txt[0].text = "Escaped " + GameManager.Zone.Name;
			Objects.MiddleGear[1].Txt[1].text = "";
			Objects.MiddleGear[1].Txt[2].enabled = Player.NewItems;
			Objects.MiddleGear[1].Txt[3].enabled = true;
			Objects.MiddleGear[1].Child[1].SetActive(true);
			Objects.MiddleGear[1].Child[2].SetActive(true);
			Objects.MiddleGear[1].Child[1].Txt[0].text = GameManager.ZoneChoiceA.Name;
			Objects.MiddleGear[1].Child[2].Txt[0].text = GameManager.ZoneChoiceB.Name;
		}
		else
		{
			Objects.MiddleGear[1].Txt[0].text = GameManager.Zone.Name;
			Objects.MiddleGear[1].Txt[1].text = "";
			Objects.MiddleGear[1].Txt[2].enabled = Player.NewItems;
			Objects.MiddleGear[1].Txt[3].enabled = false;
			Objects.MiddleGear[1].Child[1].SetActive(false);
			Objects.MiddleGear[1].Child[2].SetActive(false);
		}
	}

	public void ShowStashUI()
	{
		(Objects.MiddleGear[2] as UIObjTweener).SetTween(0);
		for(int i = 0; i < Objects.MiddleGear[2][1].Length; i++)
		{
			Destroy(Objects.MiddleGear[2][1][i].gameObject);
		}
		List<UIObj> newitems = new List<UIObj>();
		for(int i = 0; i < Player.StashItems.Count; i++)
		{
			Item item = Player.StashItems[i];
			item.Seen = true;

			UISlotButton newslot = (UISlotButton) Instantiate(Objects.SlotObj);
			Transform ns = newslot.transform;
			ns.SetParent(Objects.MiddleGear[2][1].transform);
			ns.localScale = Vector3.one;
			newslot.Setup(item);
			newslot.Drag = DragType.Hold;
			newitems.Add(newslot);
			/*Objects.MiddleGear[2][1][i].SetActive(true);
			Objects.MiddleGear[2][1][i].Img[1].sprite = item.Icon;
			Objects.MiddleGear[2][1][i].Img[0].color = item.Colour;*/
		}
		Objects.MiddleGear[2][1].Child = newitems.ToArray();
	}



	public void Reset()
	{
		loaded = false;
		Player.instance.Reset();

		GameManager.instance.gameStart = false;
		GameData.instance.Save();
		Application.LoadLevel(0);
	}

	public void ResetScore()
	{
		PlayerPrefs.SetInt("Points", 0);
		Reset();
	}

	public MiniAlertUI MiniAlert(Vector3 position, string alert, float size = 30, Color? col = null, float life = 0.6F, float speed = 0.1F, bool background = false)
	{
		MiniAlertUI alertobj = (MiniAlertUI) Instantiate(Objects.MiniAlert);
		alertobj.transform.SetParent(Canvas.transform);
		alertobj.transform.localScale = Vector3.one;
		alertobj.Setup(position, alert, life, size, col??Color.white, speed, background);
		return alertobj;
	}

}
