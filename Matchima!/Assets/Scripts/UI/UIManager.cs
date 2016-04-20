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

	public LoadScreen LoadScreen;

	public TextMeshProUGUI Health, Armour;
	public TextMeshProUGUI [] WaveTimer;

	//public UIClassButton [] ClassButtons;

	public UITooltip Tooltip;

	public UIKillScreen KillUI;
	public UIObj WaveAlert;

	public ClassUpgradeUI ClassUI;
	public ClassAbilityUI ClassAbUI;
	//public ClassHealthUI ClassHPUI;
	public Image HealthImg;
	public ClassSlotsUI _ClassButtons;
	public static ClassSlotsUI ClassButtons{get{return UIManager.instance._ClassButtons;}}
	public ItemUI ItemUI;

	[HideInInspector]
	public Class current_class, targetui_class;
	[HideInInspector]
	public Quote current_quote;
	//[HideInInspector]
	public Item current_item;

	public static bool InMenu;

	[HideInInspector]
	public bool ResUIOpen = false;

	private long current_tens = 0, current_hunds = 0, current_thous = 0;
	private int resource_store_index = 1;

	public static bool ItemUI_active, BoonUI_active;
	public static bool ShowingHealth, ShowingHit;

	private Vector3 ClassArmourBanner_pos;
	private List<TextMeshProUGUI> levelupinfo = new List<TextMeshProUGUI>();
	public bool isQuoting;
	void Awake()
	{
		instance = this;
		Objects = this.GetComponent<UIObjects>();
		Canvas = this.transform.GetChild(0).GetComponent<Canvas>();
		Menu = Objects.MenuUI.GetComponent<UIMenu>();
	}

	void Start () {
		Menu.gameObject.SetActive(true);
		
		//Objects.ShowObj(Objects.MenuUI, true);
		//Objects.ShowObj(Objects.StatsbarUI, false);
		Objects.MainUI.SetActive(false);	
		//UpdatePlayerUI();
	}

	void Update () {
		if(!GameManager.instance.gameStart) return;
		//print(PlayerControl.HoldingSlot + ":" + PlayerControl.HeldButton);
		//InMenu = (Objects.BigUI.activeSelf);

		if(Application.isEditor)
		{

		}
		else 
		{
			if(Input.touches.Length == 0) ShowTooltip(false);
		}

		HealthImg.fillAmount = Mathf.Lerp(HealthImg.fillAmount, Player.Stats.GetHealthRatio(), Time.deltaTime * 15);
		Health.text = Player.Stats.Health + "/" + Player.Stats.HealthMax;

		Objects.ArmourParent.Txt[0].text = Player.Stats.Armour;
		Objects.ArmourParent.SetActive(Player.Stats._Armour > 0);

		for(int i = 0; i < GameManager.instance._Wave.Length; i++)
		{
			if(GameManager.instance._Wave[i] != null && GameManager.instance._Wave[i].Active)
			{
				Objects.WaveSlots[i].Txt[1].text = "";
				Objects.WaveSlots[i].Img[0].transform.gameObject.SetActive(true);
	
				Objects.WaveSlots[i].Img[1].enabled = true;
				Objects.WaveSlots[i].Img[1].sprite = GameManager.instance._Wave[i].Inner;
				Objects.WaveSlots[i].Img[1].color = Color.white;
				Objects.WaveSlots[i].Img[2].enabled = true;
				Objects.WaveSlots[i].Img[2].sprite = GameManager.instance._Wave[i].Border;
				Objects.WaveSlots[i].Img[2].color = Color.white;
				Objects.WaveSlots[i].Txt[0].text = GameManager.instance._Wave[i].Current+"";
			}
			else 
			{
				Objects.WaveSlots[i].Img[0].transform.gameObject.SetActive(false);
				for(int s = 1; s < Objects.WaveSlots[i].Img.Length; s++)
				{
					Objects.WaveSlots[i].Img[s].enabled = false;
				}
				Objects.WaveSlots[i].Txt[0].text = "";
				if(GameManager.instance._Wave[i] != null) Objects.WaveSlots[i].Txt[1].text = "" + GameManager.instance._Wave[i].Timer;
				else Objects.WaveSlots[i].Txt[1].text = "";
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
		Vector3 tpos = Vector3.up * 0.2F + Vector3.left * 0.8F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.instance.Health.transform.position + tpos, 
			"+" + current_heal, 42, GameData.instance.GoodColour, 1.7F,	0.01F);

		while(heal.lifetime > 0.0F)
		{
			if(Player.Stats.HealThisTurn == 0)
			{
				heal.lifetime = 0.0F;
				heal.text = "";
				break;
			}
			else if(Player.Stats.HealThisTurn != current_heal)
			{
				heal.lifetime += 0.4F;
				heal.size = 42 + current_heal * 0.75F;
				current_heal = Player.Stats.HealThisTurn;
				heal.text = "+" + current_heal;
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
		Vector3 tpos = Vector3.up * 0.2F + Vector3.left * 0.3F;
		MiniAlertUI hit = UIManager.instance.MiniAlert(
			UIManager.instance.Health.transform.position + tpos, 
			"-" + current_hit, 42, GameData.instance.BadColour, 1.7F, 0.01F);

		while(hit.lifetime > 0.0F)
		{
			if(Player.Stats.DmgThisTurn == 0)
			{
				hit.lifetime = 0.0F;
				hit.size = 42 + current_hit*0.75F;
				break;
			}
			if(Player.Stats.DmgThisTurn != current_hit)
			{
				hit.lifetime += 0.2F;
				current_hit = Player.Stats.DmgThisTurn;
			}
			hit.text = "-" + current_hit;

			yield return null;
		}

		ShowingHit = false;
		yield return null;
	}


	
	public void SwapSlotButtons(UISlotButton a, Class c, int slot)
	{
		UISlotButton b = ClassButtons[(int)c.Genus].SlotUI[0] as UISlotButton;

		if(a != null && b != null)
		{
			Slot slota = a.slot;
			Slot slotb = b.slot;
			if(a.Parent != null)
			{
				//StartCoroutine(
					a.Parent.GetSlot(slotb, a.Index);
			}
			if(b.Parent != null) 
			{
				//StartCoroutine(

					b.Parent.GetSlot(slota, b.Index);
			}

			b.Setup(slota);
			a.Setup(slotb);
		}
	}
	public void ShowKillUI(long alltokens, int tens, int hunds, int thous)
	{
		KillUI.Activate(alltokens, tens, hunds, thous);
	}


	public void ShowTooltip(bool Active, UIButton ab = null)
	{
		Tooltip.SetTooltip(Active, ab);
		//AdvTooltip.SetTooltip(false, ab, input, output);
	}

	public void ShowAdvTooltip(bool Active, UIButton ab = null)
	{
		Tooltip.SetTooltip(false, ab);
		//AdvTooltip.SetTooltip(Active, ab, input, output);
	}

	public void ShowSimpleTooltip(bool active, Transform obj, string name, string desc)
	{
		Tooltip.SetSimpleTooltip(active, obj, name, desc);
	}


	public void ShowSimpleTooltip(bool active, Transform obj, StCon name, StCon [] desc)
	{
		Tooltip.SetSimpleTooltip(active, obj, name, desc);
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
		Menu._Text.enabled = active;
		Menu.Tokens.enabled = active;
		Menu.ClassMenu.SetActive(active);
		Objects.ShowObj(Objects.MainUI, !active);
	}

	public void ShowClassAbilities(Class c, bool? over = null)
	{
		bool active = over ?? (current_class != c);
		current_class = (active ? c : null);
		targetui_class = (active ? c : null);
		for(int i = 0; i < ClassButtons.Length; i++)
		{
			ClassButtons[i].Setup(ClassButtons[i]._class);
		}
	}

	public void ShowItemUI(params Item [] i)
	{
		current_class = null;
		//Objects.ShowObj(Objects.BigUI,true);
		Objects.ShowObj(Objects.ClassUpgradeUI,false);
		ItemUI.gameObject.SetActive(true);
		ItemUI_active = true;
		Objects.ShowObj(Objects.ItemUI, true);
		//current_item = i;

		ItemUI.Setup((Slot[] )i);
	}

	public void HideItemUI()
	{
		Objects.ShowObj(Objects.BigUI,false);
		ItemUI.gameObject.SetActive(false);
		ItemUI_active = false;
		ItemUI.DestroySlots();
		ShowClassAbilities(null, false);
		current_class = null;
		targetui_class = null;
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
		if(!InMenu) Objects.ShowObj(Objects.BigUI, false);
		ItemUI.gameObject.SetActive(false);
		current_item = null;
	}


	public void ShowMenu(bool active)
	{
		Menu._Text.enabled = active;
		Menu.Tokens.enabled = active;
		Menu.PauseMenu.SetActive(active);
		Objects.ShowObj(Objects.MainUI, !active);
	}

	public void ShowResourceUI()
	{
		InMenu = true;
		ResUIOpen = true;
		Objects.ShowObj(Objects.BigUI, true);
		Objects.ShowObj(Objects.ClassUpgradeUI,true);
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
		Objects.BigUI.SetActive(true);
		Objects.LevelUpMenu.SetActive(true);
		//Objects.LevelUpMenu.Setup(c, ups);

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
		Objects.BigUI.SetActive(true);
		Objects.LevelUpMenu.SetActive(true);
		Objects.LevelUpMenu.Setup(group);
		LevelChoice = null;
	}

	public void HideBoonUI()
	{
		BoonUI_active = false;
		Objects.LevelUpMenu.Destroy();
		//LevelChoice = 1;
		PlayerControl.HeldButton = null;
		PlayerControl.HoldingSlot = false;
		InMenu = false;
		current_class = null;
	}

	public void HideResourceUI()
	{
		Objects.ShowObj(Objects.BigUI, false);
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
		Menu.ClassMenu.SetActive(false);
		Menu._Text.enabled = false;
		Menu.Tokens.enabled = false;

		Objects.ShowObj(Objects.Options, false);
		Objects.ShowObj(Objects.MainUI, true);


		//Objects.ShowObj(Objects.ClassTopUI, false);

		loaded  = true;
		int i =0;

		i = 0;
		foreach(UIClassButton child in ClassButtons.Class)
		{
			child.Setup(Player.Classes[i]);
			i++;
		}
		yield return null;
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
		UIManager.Objects.LevelUpMenu.SetActive(false);
		isQuoting = true;
		for(int i = 0; i < q.Length; i++)
		{
			current_quote = q[i];
			Objects.ShowObj(Objects.BigUI, true);
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
					new_button._Text.text = _class._Slots[s].name;
					new_button._Image.color = _class._Slots[s].Colour * Color.grey;
					new_button.transform.parent = Objects.Alert_ButtonParent.transform;
					new_button.transform.position = Vector3.zero;
					new_button.transform.localScale = Vector3.one;
					AddConfirmListener(new_button, s);
				}
				UIObj no = (UIObj) Instantiate(Objects.Alert_Button);
				no._Text.text = "NO";
				no._Image.color =  GameData.Colour(GENUS.STR) *  Color.grey;
				no.transform.parent = Objects.Alert_ButtonParent.transform;
				no.transform.position = Vector3.zero;
				no.transform.localScale = Vector3.one;
				AddConfirmListener(no, _class._Slots.Length);
			}
			else if(current_quote.YesNoButtons)
			{
				Objects.Alert_ButtonParent.SetActive(true);
				UIObj yes = (UIObj) Instantiate(Objects.Alert_Button);
				yes._Text.text = "YES";
				yes._Image.color =  GameData.Colour(GENUS.WIS) *  Color.grey;
				yes.transform.parent = Objects.Alert_ButtonParent.transform;
				yes.transform.position = Vector3.zero;
				yes.transform.localScale = Vector3.one;
				AddConfirmListener(yes, 0);

				UIObj no = (UIObj) Instantiate(Objects.Alert_Button);
				no._Text.text = "NO";
				no._Image.color =  GameData.Colour(GENUS.STR) * Color.grey;
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
		if(GameManager.instance._Wave == null) return;
		ShowSimpleTooltip(active, Objects.WaveSlots[num].Img[1].transform, GameManager.instance._Wave[num].Name, GameManager.instance._Wave[num].Description);
	}

	public void AddClass(Class c, int slot)
	{
		ClassButtons[slot].Setup(c);
		MiniAlert(Health.transform.position + Vector3.up * 1.3F,
				  c.Name + " joined the party!", 50, GameData.Colour((GENUS)slot), 1.5F);
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