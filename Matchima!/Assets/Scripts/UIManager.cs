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

	public UIObj Health;
	public TextMeshProUGUI Armour;
	public TextMeshProUGUI WaveHealthText;
	public TextMeshProUGUI WaveTimer;

	public UITooltip Tooltip;

	public UIKillScreen KillUI;
	public UIObjTweener ScreenAlert;

	public tk2dClippedSprite [] PlayerHealth;
	public tk2dClippedSprite [] WaveHealth;

	public TextMeshProUGUI [] TDebug;


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

	public UIObjTweener TuteAlert;

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
		Objects.MainUI.SetActive(false);	
		for(int i = 0; i < ClassButtons.Length; i++)
		{
			ClassButtons.GetClass(i)._Frame.SetSprite(TileMaster.Genus.Frames, i);
			ClassButtons.GetClass(i)._FrameMask.SetSprite(TileMaster.Genus.Frames, i);
		}
		
	}

	float update_interval_current = 0.0F;
	float update_interval_amount = 4;

	void Update () {

		if(!GameManager.instance.gameStart) return;

		if(MeterTimer <= 0.0F) CashMeterPoints();
		else if(IsShowingMeters && StartedMeter) MeterTimer -= Time.deltaTime;

		if(Time.time > update_interval_current)
		{
			update_interval_current = Time.time + (Time.deltaTime * update_interval_amount);
		}
		else return;

		if(!Application.isEditor)
		{
			if(Input.touches.Length == 0) ShowGearTooltip(false);
		}
		if(!Objects.MiddleGear[5].isActive)
			Objects.MiddleGear[5].SetActive(true);
		Objects.MiddleGear[5].Txt[0].text = ""+Player.AttackValue;
		Objects.MiddleGear[5].Txt[1].text = ""+Player.SpellValue;
		Objects.TopRightButton.Txt[0].text = "" + GameManager.Floor;
		Objects.TopRightButton.Txt[1].text = "" + GameManager.ZoneNum;
		Objects.TopRightButton.Txt[2].enabled = false;//Player.NewItems;
		if(Player.Stats._HealthMax <= 0) Health.Txt[0].text = "";
		else Health.Txt[0].text = Player.Stats.Health + "/" + Player.Stats.HealthMax;
		Health.Txt[1].text = "HEALTH";

		for(int i = 0; i < PlayerHealth.Length; i++)
		{
			float curr = PlayerHealth[i].clipBottomLeft.x;
			PlayerHealth[i].clipBottomLeft = new Vector2(Mathf.Lerp(curr, 1.0F - (Player.Stats.GetHealthRatio()*0.65F), Time.deltaTime * 15), 0);
			PlayerHealth[i].color = Color.Lerp(GameData.instance.BadColour, GameData.instance.GoodColour, Player.Stats.GetHealthRatio());
		}
		if(GameManager.Wave != null)
		{
			WaveHealthText.text = GameManager.Wave.WaveNumbers;
			for(int i = 0; i < WaveHealth.Length; i++)
			{
				float curr = WaveHealth[i].clipTopRight.x;
				WaveHealth[i].clipTopRight = new Vector2(Mathf.Lerp(curr, GameManager.Wave.GetRatio()*0.88F, Time.deltaTime * 15),1);
				WaveHealth[i].color = Color.Lerp(GameData.instance.ShieldEmpty, GameData.instance.ShieldFull, GameManager.Wave.GetRatio());
			}
		}
		
		CameraUtility.instance.MainLight.color = Color.Lerp(
			CameraUtility.instance.MainLight.color, BackingTint, Time.deltaTime * 5);
		Objects.Walls.color = Color.Lerp(
			Objects.Walls.color, WallTint, Time.deltaTime * 5);

		Objects.ArmourParent.Txt[0].text = Player.Stats._Armour > 0 ? Player.Stats.Armour : "";
		Objects.ArmourParent.SetActive(Player.Stats._Armour > 0);

		if(GameManager.Wave != null)
		{
			for(int i = 0; i < GameManager.Wave.Length; i++)
			{
				if(WaveButtons.Length <= i) continue;
				if(GameManager.Wave[i] != null && GameManager.Wave[i].Active)
				{
					UIObjtk obj = WaveButtons[i] as UIObjtk;
					GetWaveButton(ref obj, GameManager.Wave[i] as WaveUnit);								
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

	public void GetWaveButton(ref UIObjtk obj, 
									 WaveUnit wunit)
	{
		if(obj == null) return;

		obj.SetActive(true);
		obj.Txt[0].text = "";	
		obj.Imgtk[0].enabled = true;
		obj.Imgtk[1].enabled = true;
		if(wunit.InnerOverrideData != null)
		{
			obj.Imgtk[1].SetSprite(wunit.InnerOverrideData, wunit.InnerOverride);
			obj.Imgtk[0].SetSprite(TileMaster.Genus.Frames, "Omega");
		}
		else if(wunit is WaveTile)
		{

			WaveTile wtile = wunit as WaveTile;
			string render = wtile.GenusString;
			tk2dSpriteDefinition id = TileMaster.Types[wtile.SpeciesFinal].Atlas.GetSpriteDefinition(render);
			if(id == null) render = "Alpha";
			obj.Imgtk[1].SetSprite(TileMaster.Types[wtile.SpeciesFinal].Atlas, render);
			obj.Imgtk[0].SetSprite(TileMaster.Genus.Frames, wtile.GenusString);
		}
	}

	public void GetWaveButton(ref UIObjtk obj, 
									 SPECIES sp, GENUS g)
	{
		if(obj == null) return;

		obj.SetActive(true);
		obj.Txt[0].text = "";	
		obj.Imgtk[0].enabled = true;
		obj.Imgtk[1].enabled = true;

		string sp_render = GameData.ResourceLong(g);
		string g_render = GameData.ResourceLong(g);

		tk2dSpriteDefinition sp_id = TileMaster.Types[sp.Name].Atlas.GetSpriteDefinition(sp_render);
		if(sp_id == null) sp_render = "Alpha";
		tk2dSpriteDefinition g_id = TileMaster.Genus.Frames.GetSpriteDefinition(g_render);
		if(g_id == null) g_render = "Alpha";

		obj.Imgtk[1].SetSprite(TileMaster.Types[sp.Name].Atlas, sp_render);

		obj.Imgtk[0].SetSprite(TileMaster.Genus.Frames, g_render);
	}

	public void GetWaveButton(ref UIObjtk obj, 
									 tk2dSpriteCollectionData inner,
									 string inner_def,
									 string outer_def)
	{
		if(obj == null) return;

		obj.SetActive(true);
		obj.Txt[0].text = "";	
		obj.Imgtk[0].enabled = true;
		obj.Imgtk[1].enabled = true;


		tk2dSpriteDefinition sp_id = inner.GetSpriteDefinition(inner_def);
		if(sp_id == null) inner_def = "Alpha";
		tk2dSpriteDefinition g_id = TileMaster.Genus.Frames.GetSpriteDefinition(outer_def);
		if(g_id == null) outer_def = "Alpha";

		obj.Imgtk[1].SetSprite(inner, inner_def);

		obj.Imgtk[0].SetSprite(TileMaster.Genus.Frames, outer_def);
	}
				
		

	public IEnumerator Reset()
	{
		loaded = false;
		
		yield return StartCoroutine(UnloadUI());
		yield return StartCoroutine(Menu.LoadMenu());
	}

	public void ResetScore()
	{
		PlayerPrefs.SetInt("Points", 0);
		Reset();
	}

	public IEnumerator LoadUI()
	{
		while(!Player.loaded) yield return null;
		
		Objects.TopGear.DivisionActions.Clear();
		Objects.ShowObj(Objects.MainUI, true);
		int i =0;

		i = 0;
		
		foreach(UIClassButton child in ClassButtons.Class)
		{
			child.Setup(Player.Classes[i]);
			i++;
		}
		yield return null;

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

		Objects.MiddleGear[3][3].AddAction(UIAction.MouseUp, ()=>
		{
			AudioManager.PlaySFX = !AudioManager.PlaySFX;
		});

		Objects.MiddleGear[3][4].AddAction(UIAction.MouseUp, ()=>
		{
			AudioManager.PlayMusic = !AudioManager.PlayMusic;
		});

		Objects.MiddleGear[3][6].AddAction(UIAction.MouseUp, ()=>
		{	
			Objects.TopGear.SetActive(false);
			});

		Objects.MiddleGear[3][7].AddAction(UIAction.MouseUp, ()=>
		{	
			Objects.BotGear.SetActive(false);
			});

		Objects.MiddleGear[1].SetActive(false);
		Objects.MiddleGear[2].SetActive(false);
		Objects.MiddleGear[3].SetActive(false);

		Objects.TopRightButton.ClearActions();
		Objects.TopRightButton.AddAction(UIAction.MouseUp, () =>
		{
			ShowZoneUI(false);
			(Objects.MiddleGear[2] as UIObjTweener).SetTween(0, false);
			UIManager.Objects.MiddleGear[1].Txt[0].text = GameManager.Zone.Name;
		});

		(Objects.TopGear as UIGear).Drag = true;
		(Objects.TopGear as UIGear).DivisionActions.Add((int num) =>
		{
			TopGear_lastdivision = num;
		});

		loaded  = true;

		yield return null;
	}

	public IEnumerator UnloadUI()
	{
		Objects.TopGear.DivisionActions.Clear();
		Objects.ShowObj(Objects.MainUI, false);

		(Objects.MiddleGear[1] as UIObjTweener).SetTween(0, false);
		(Objects.MiddleGear[1] as UIObjTweener).SetTween(1, false);
		ScreenAlert.SetTween(0, false);
		KillUI.Deactivate();
		Objects.MiddleGear[2][0].ClearActions(); 
		Objects.MiddleGear[3][0].ClearActions(); 
		Objects.MiddleGear[3][2].ClearActions(); 
		Objects.MiddleGear[3][1].ClearActions(); 
		Objects.MiddleGear[3][3].ClearActions();
		Objects.MiddleGear[3][4].ClearActions(); 
		Objects.MiddleGear[3][6].ClearActions(); 
		Objects.MiddleGear[3][7].ClearActions(); 
		Objects.MiddleGear[5].SetActive(false);

		Objects.TopRightButton.ClearActions();

		
		UIManager.ShowClassButtons(false);
		UIManager.ShowWaveButtons(false);
		
		UIManager.Objects.BotGear[0].SetActive(false);
		UIManager.Objects.TopGear[2].SetActive(false);
		UIManager.Objects.MiddleGear[0].SetActive(true);
		(UIManager.Objects.MiddleGear[0][0] as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.MiddleGear[0][1] as UIObjTweener).SetTween(0, false);
		UIManager.Objects.MiddleGear[0].Txt[0].text = "";

		UIManager.Objects.MiddleGear.Img[0].enabled = false;

		UIManager.Objects.BotGear[3].AddAction(UIAction.MouseDown,()=>
		{
			UIManager.Menu.HeroMenu(0);
		});

		UIManager.Objects.TopGear.Txt[0].text = "";
		UIManager.Objects.MiddleGear.Txt[0].text = "";
		UIManager.Objects.BotGear.Txt[0].text = "";	

		UIManager.ShowWaveButtons(false);
		yield return null;
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

	public bool IsShowingMeters
	{
		get
		{
			for(int i = 0; i < ShowingMeter.Length; i++)
			{
				if(ShowingMeter[i]) return true;
			}
			return false;
		}
	}
	public void SetMeter(int g, bool active)
	{
		ShowingMeter[g] = active;
	}

	bool [] ShowingMeter = new bool[7];
	int [] Meters = new int[7];
	MiniAlertUI [] MeterObj = new MiniAlertUI[7];
	public float MeterTimer = 0.4F;
	float MeterTimer_init = 0.15F;
	public void CashMeterPoints()
	{
		StartedMeter = false;
		MeterTimer = MeterTimer_init;
		StartCoroutine(ShowBonuses());
	}

	bool StartedMeter;
	public void StartTimer()
	{
		StartedMeter = true;
		MeterTimer = MeterTimer_init;
	}

	public void GetMeterPoints(int g, int points)
	{
		int init_font_size = 280;
		GENUS genus = (GENUS) g;
		if(g >= 4) return;
		if(ShowingMeter[g] && Meters[g] > 0)
		{
			Meters[g] += points;
			if(MeterObj[g] != null)
			{
				MeterObj[g].AddJuice(Juice.instance.BounceB, 0.32F);
				MeterObj[g].size = init_font_size + Meters[g]*5;
				MeterObj[g].text = "" + Meters[g];	
			}
			
			//MeterObj[g].lifetime += 0.12F;
			MeterTimer = Mathf.Clamp(MeterTimer + 0.2F, 0.0F, MeterTimer_init);
			
		}
		else
		{
			ShowingMeter[g] = true;
			Meters[g] = points;
			//MeterTimer = MeterTimer_init;
			float init_rotation = Random.Range(-7,7);
			

			MeterObj[g] = UIManager.instance.MiniAlert(
				UIManager.Objects.MiddleGear[4][g].transform.position, 
				"" + Meters[g], init_font_size,  GameData.Colour((GENUS)g), 2.0F, 0.01F);
			MeterObj[g].transform.SetParent(UIManager.Objects.MiddleGear[4][g].transform);
			MeterObj[g].transform.rotation = Quaternion.Euler(0,0,init_rotation);
			MeterObj[g].AddJuice(Juice.instance.BounceB, 0.45F);
			MeterObj[g].DestroyOnEnd = false;
			MeterObj[g].AddAction(() =>
			{
				
			});
		}
	}

	public class BonusGroup
	{
		public List<Bonus> bonuses = new List<Bonus>();
		public int Length{get{return bonuses.Count;}}
		public void Add(Bonus b){bonuses.Add(b);}
		public Bonus this[int i]{get{return bonuses[i];}}
	}

	public BonusGroup [] BonusGroups;

	public void SetBonuses(Bonus [] b)
	{
		BonusGroups = new BonusGroup[Player.Classes.Length];
		for(int bb = 0; bb < BonusGroups.Length; bb++)
		{
			BonusGroups[bb] = new BonusGroup();
		}
		for(int i = 0; i < b.Length; i++)
		{
			int index = b[i].index;
			BonusGroups[0].Add(b[i]);
		}
	}

	public IEnumerator ShowBonuses()
	{
		float bonus_time = 0.32F;

		for(int i = 0; i < BonusGroups[0].Length; i++)
		{
			MiniAlertUI BonusObj = UIManager.instance.MiniAlert(
				UIManager.Objects.MiddleGear[4][4].transform.position, 
				BonusGroups[0][i].Name, 230, BonusGroups[0][i].col, bonus_time, 0.1F);
			//BonusObj.transform.SetParent(UIManager.Objects.MiddleGear[4][g].transform);
			BonusObj.transform.rotation = Quaternion.Euler(0,0,0);
			BonusObj.AddJuice(Juice.instance.BounceB, 0.45F);

			for(int g = 0; g < Meters.Length; g++)
			{
				if(Meters[g] == 0 || MeterObj[g] == null) continue;
				Meters[g] = (int)((float)Meters[g] * BonusGroups[0][i].Multiplier);
				MeterObj[g].AddJuice(Juice.instance.BounceB, 0.45F);
				MeterObj[g].text = Meters[g] + "";	
			}
			
			yield return new WaitForSeconds(bonus_time);
		}

		yield return new WaitForSeconds(GameData.GameSpeed(0.08F));

		float info_movespeed = 0.66F;
		float info_finalscale = 0.3F;
		
		for(int g = 0; g < Meters.Length; g++)
		{
			if(Meters[g] == 0 || MeterObj[g] == null) continue;
			
			MiniAlertUI wavetarget = MiniAlert(MeterObj[g]);
			
			//wavetarget.transform.SetParent(UIManager.Objects.MiddleGear[4][g].transform);
			wavetarget.transform.position = UIManager.Objects.MiddleGear[4][g].transform.position;
			wavetarget.transform.localScale = Vector3.one;

			MoveToPoint wavetarget_mover = AttachMoverToAlert(ref wavetarget);
			wavetarget_mover.SetTarget(Objects.TopGear[1][0][0].transform.position);
			wavetarget_mover.SetPath(info_movespeed * 1.5F, 0.4F, 0.0F, info_finalscale);
			wavetarget_mover.SetIntMethod( 
				(int [] amt) =>
				{
					if(GameManager.Wave != null)
						GameManager.Wave.AddPoints(amt[1]);
				},
				new int []{g, Meters[g]}
			);
			wavetarget.lifetime = 0.0F;

			MiniAlertUI classtarget = MiniAlert(MeterObj[g]);
			//classtarget.transform.SetParent(UIManager.Objects.MiddleGear[4][g].transform);
			classtarget.transform.position = UIManager.Objects.MiddleGear[4][g].transform.position;
			classtarget.transform.localScale = Vector3.one;

			MoveToPoint classtarget_mover = AttachMoverToAlert(ref classtarget);
			classtarget_mover.SetTarget(ClassButtons.GetClass(g).transform.position);
			classtarget_mover.SetPath(info_movespeed, 0.4F, 0.0F, info_finalscale);
			classtarget_mover.SetIntMethod( 
				(int [] amt) =>
				{
					if(Player.Classes[amt[0]] == null) return;
					if((GENUS)amt[0] != GENUS.ALL) Player.Classes[amt[0]].AddToMeter(Meters[amt[0]]);
					else
					{
						for(int cl = 0; cl < 4; cl++)
						{
							Player.Classes[cl].AddToMeter(amt[1]);
						}
					}
					ShowingMeter[amt[0]] = false;
					Meters[amt[0]] = 0;
				},
				new int []{g, Meters[g]}
			);
			classtarget.lifetime = 0.0F;
			Destroy(MeterObj[g].gameObject);
			yield return null;
		}

		
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
	public void ShowKillUI(End_Type e, int [] xp_steps)
	{
		KillUI.Activate(e, xp_steps);
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
		if(!active) 
		{
			uitarget = null;
			for(int i = 0; i < Objects.TopGear[1][1][3][0].Length; i++)
			{
				if(Objects.TopGear[1][1][3][0].GetChild(i) != null)
					Destroy(Objects.TopGear[1][1][3][0].GetChild(i).gameObject);
			}
			for(int i = 0; i < Objects.TopGear[1][1][3][1].Length; i++)
			{
				if(Objects.TopGear[1][1][3][1].GetChild(i) != null)
					Destroy(Objects.TopGear[1][1][3][1].GetChild(i).gameObject);
			}
			for(int i = 0; i < Objects.TopGear[1][1][3][2].Length; i++)
			{
				if(Objects.TopGear[1][1][3][2].GetChild(i) != null)
					Destroy(Objects.TopGear[1][1][3][2].GetChild(i).gameObject);
			}
		}

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

			Transform TopParent = Objects.TopGear[1][1][3][0].transform;
			Objects.TopGear[1][1][3][0].Child = GenerateUIObjFromStCon(TopParent, t.BaseDescription);
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

			Transform TopParent = Objects.TopGear[1][1][3][1].transform;
			Objects.TopGear[1][1][3][1].Child = GenerateUIObjFromStCon(TopParent, t.EffectDescription);

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

			Transform TopParent = Objects.TopGear[1][1][3][2].transform;
			Objects.TopGear[1][1][3][2].Child = GenerateUIObjFromStCon(TopParent, t.Description);
		}
		else Objects.TopGear[1][1][3][2].SetActive(false);		
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


	public void ItemDestroy()
	{
		Destroy(current_item.gameObject);
		ItemUI.gameObject.SetActive(false);
		current_item = null;
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


	public void ShowResourceUI()
	{
		InMenu = true;
		ResUIOpen = true;
		Objects.ShowObj(Objects.ItemUI, false);
		Objects.GetScoreWindow().gameObject.SetActive(false);
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

	

	public bool AlertShowing = false;

	public IEnumerator Alert(float time, string floor = "", string title = "", string desc = "",
							 bool wait_for_touch = false, float size = 70)
	{
		StCon [] fl = null;
		if(floor != string.Empty) fl = new StCon[] {new StCon(floor, Color.white, true, size)};
		StCon [] ti = null;
		if(title != string.Empty) ti = new StCon[] {new StCon(title, Color.white, true, size)};
		StCon [] de = null;
		if(desc != string.Empty) de = new StCon[] {new StCon(desc, Color.white, true, size)};
		yield return StartCoroutine(Alert(time, fl, ti, de, wait_for_touch));
	}
	
	public IEnumerator Alert(float time, StCon [] floor = null, StCon [] title = null, StCon [] desc = null, bool wait_for_touch = false)
	{
		while(AlertShowing) yield return null;
		AlertShowing = true;
		GameManager.instance.paused = true;
		ScreenAlert.SetActive(true);
		ScreenAlert.SetTween(0,true);
		UIManager.Objects.BotGear.SetTween(3, true);

		
		/*for(int i = 0; i < ScreenAlert.Child[2].Length; i++)
		{
			if(ScreenAlert.Child[2][0].GetChild(i) != null)
				Destroy(ScreenAlert.Child[2].GetChild(i).gameObject);
		}*/


		if(floor != null)
		{
			(ScreenAlert[2] as UIObjTweener).Txt[0].text = floor[0].Value;
			if(floor.Length > 1) (ScreenAlert[2] as UIObjTweener).Txt[1].text = floor[1].Value;
			else (ScreenAlert[2] as UIObjTweener).Txt[1].text = "";

			(ScreenAlert[2] as UIObjTweener).SetTween(0, true);
			yield return new WaitForSeconds(GameData.GameSpeed(0.55F));
		}

		if(title != null)
		{
			Transform TopParent = ScreenAlert[0][0].transform;
			ScreenAlert[0][0].Child = GenerateUIObjFromStCon(TopParent, title);
			for(int i = 0; i < ScreenAlert[0][0].Length; i++)
			{
				ScreenAlert[0][0].GetChild(i).SetActive(false);
			}
			(ScreenAlert[0] as UIObjTweener).SetTween(0, true);
			for(int i = 0; i < ScreenAlert[0][0].Length; i++)
			{
				ScreenAlert[0][0].GetChild(i).SetActive(true);
				yield return new WaitForSeconds(GameData.GameSpeed(0.35F));
			}
		}

		if(desc != null)
		{
			Transform TopParent = ScreenAlert.Child[1][0].transform;
			ScreenAlert[1][0].Child = GenerateUIObjFromStCon(TopParent, desc);
			(ScreenAlert[1] as UIObjTweener).SetTween(0, true);
		}


		yield return new WaitForSeconds(GameData.GameSpeed(time));
		
		if(wait_for_touch)
		{
			bool has_touched = false;
			(ScreenAlert[4] as UIObjTweener).SetTween(0, true);
			ScreenAlert[4].AddAction(UIAction.MouseUp, () =>{has_touched = true;});
			while(!has_touched)
			{
			
				yield return null;
			}
			(ScreenAlert[4] as UIObjTweener).SetTween(0,false);
			ScreenAlert[4].ClearActions();
		}

		ScreenAlert.SetTween(0,false);
		(ScreenAlert[0] as UIObjTweener).SetTween(0, false);
		(ScreenAlert[1] as UIObjTweener).SetTween(0, false);
		(ScreenAlert[2] as UIObjTweener).SetTween(0, false);
		UIManager.Objects.BotGear.SetTween(3, false);
		PlayerControl.instance.ResetSelected();
		GameManager.instance.paused = false;
		AlertShowing = false;

		yield return new WaitForSeconds(0.1F);
		for(int i = 0; i < ScreenAlert.Child[0][0].Length; i++)
		{
			if(ScreenAlert.Child[0][0].GetChild(i) != null)
				Destroy(ScreenAlert.Child[0][0].GetChild(i).gameObject);
		}

		for(int i = 0; i < ScreenAlert.Child[1][0].Length; i++)
		{
			if(ScreenAlert.Child[1][0].GetChild(i) != null)
				Destroy(ScreenAlert.Child[1][0].GetChild(i).gameObject);
		}

		yield break;
	}

	public IEnumerator LvlAlert(float time, StCon [] title = null, StCon [] desc = null, bool wait_for_touch = false)
	{
		while(AlertShowing) yield return null;
		AlertShowing = true;
		GameManager.instance.paused = true;
		ScreenAlert.SetActive(true);
		ScreenAlert.SetTween(0,true);

		UIObjTweener Lvl_Alert = (ScreenAlert[3] as UIObjTweener);
		Lvl_Alert.SetTween(0, true);
		if(title != null)
		{
			Lvl_Alert.Txt[0].text = title[0].Value + title[1].Value;
			Lvl_Alert.Txt[0].color = title[0].Colour;
		}

		if(desc != null)
		{
			Transform TopParent = Lvl_Alert[0].transform;
			Lvl_Alert[0].Child = GenerateUIObjFromStCon(TopParent, desc);
			for(int i = 0; i < Lvl_Alert[0].Length; i++)
			{
				Lvl_Alert[0].GetChild(i).SetActive(false);
			}
			for(int i = 0; i < Lvl_Alert[0].Length; i++)
			{
				Lvl_Alert[0].GetChild(i).SetActive(true);
				yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
			}
		}


		yield return new WaitForSeconds(GameData.GameSpeed(time));
		
		if(wait_for_touch)
		{
			bool has_touched = false;
			while(!has_touched)
			{
				has_touched = Input.GetMouseButtonDown(0);
				yield return null;
			}
		}

		Lvl_Alert.SetTween(0, false);

		PlayerControl.instance.ResetSelected();
		GameManager.instance.paused = false;
		AlertShowing = false;
		

		yield return new WaitForSeconds(0.1F);
		for(int i = 0; i < Lvl_Alert[0].Length; i++)
		{
			if(Lvl_Alert[0].GetChild(i) != null)
				Destroy(Lvl_Alert[0].GetChild(i).gameObject);
		}
		ScreenAlert.SetActive(false);
		yield break;
	}

	public UIObj [] GenerateUIObjFromStCon(Transform TopParent, params StCon [] title)
	{
		List<UIObj> final = new List<UIObj>();
		UIObj ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
		Transform ParentTrans = ParentObj.transform;
		ParentTrans.SetParent(TopParent);
		ParentTrans.position = Vector3.zero;
		ParentTrans.localScale = Vector3.one;
		ParentTrans.localRotation = Quaternion.Euler(0,0,0);
		final.Add(ParentObj);
		for(int i = 0; i < title.Length; i++)
		{
			UIObj new_desc = (UIObj) Instantiate(Objects.TextObj);
			new_desc.transform.SetParent(ParentTrans);
			new_desc.transform.position = Vector3.zero;
			new_desc.transform.localScale = Vector3.one;
			new_desc.transform.localRotation = Quaternion.Euler(0,0,0);
			new_desc.Txt[0].text = title[i].Value;
			new_desc.Txt[0].color = title[i].Colour;
			new_desc.Txt[0].fontSize = title[i].Size;
			if(title[i].NewLine && i < title.Length-1)
			{
				ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
				ParentTrans = ParentObj.transform;
				ParentTrans.SetParent(TopParent);
				ParentTrans.position = Vector3.zero;
				ParentTrans.localScale = Vector3.one;
				ParentTrans.localRotation = Quaternion.Euler(0,0,0);
				final.Add(ParentObj);
			}
		}

		return final.ToArray();
	}

	public IEnumerator ImageQuote(float time, Unit parent, tk2dSpriteCollectionData inner, string inner_str,
															tk2dSpriteCollectionData outer = null, string outer_str = "")
	{
		while(isQuoting || InMenu) yield return null;
		isQuoting = true;

		UIObjtk quoter = null;
		if(parent is Class)
		{
			quoter = Objects.ClassImageQuote[parent.Index];
		}
		else 
		{
			quoter = Objects.WaveImageQuote;
		}

		EasyTween eas = quoter.GetComponent<EasyTween>();
		if(!eas.IsObjectOpened()) eas.OpenCloseObjectAnimation();

		quoter.Imgtk[0].SetSprite(inner, inner_str);
		if(outer != null) 
		{
			quoter.Imgtk[1].gameObject.SetActive(true);
			quoter.Imgtk[1].SetSprite(outer, outer_str);	
		}
		else quoter.Imgtk[1].gameObject.SetActive(false);


		yield return new WaitForSeconds(time);
		eas = Objects.WaveImageQuote.GetComponent<EasyTween>();
		if(eas.IsObjectOpened()) eas.OpenCloseObjectAnimation();

		for(int i = 0; i < Objects.ClassImageQuote.Length; i++)
		{
			eas = Objects.ClassImageQuote[i].GetComponent<EasyTween>();
			if(eas.IsObjectOpened()) eas.OpenCloseObjectAnimation();
		}
		
		PlayerControl.instance.canMatch = true;
		GameManager.instance.paused = false;
		isQuoting = false;
	}

	public IEnumerator Quote(params Quote [] q)
	{
		yield break;
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

	public IEnumerator AddClass(Class c, int slot)
	{
		ClassButtons.GetClass(slot).Setup(c);
	
	  	UIManager.ClassButtons.GetClass(slot).ShowClass(true);
	  	GameObject powerup = EffectManager.instance.PlayEffect(UIManager.ClassButtons.GetClass(slot).transform, Effect.ManaPowerUp, GameData.Colour(c.Genus));
	  	powerup.transform.localScale = Vector3.one;
	  	
	  	yield return new WaitForSeconds(Time.deltaTime * 55);
	  	MiniAlert(Health.transform.position + Vector3.up * 2.5F,
		c.Name + " joined\nthe party!", 80, GameData.Colour((GENUS)slot), 1.1F, 0.1F, true);

	  	UIManager.ClassButtons.GetClass(slot).ShowClass(false);
	  	Destroy(powerup);
	  	yield return new WaitForSeconds(Time.deltaTime * 55);
	}


	public void ShowPlayerLvl()
	{

		(Objects.MiddleGear[1] as UIObjTweener).SetTween(0);
		bool open = (Objects.MiddleGear[1] as UIObjTweener).Tween.IsObjectOpened();
		GameManager.instance.paused = open;
		ScreenAlert.SetTween(0, open);
		if(!open) UIManager.instance.SetClassButtons(false);
			UpdatePlayerLvl();

	}
	public void UpdatePlayerLvl()
	{
		Objects.MiddleGear[1][0].SetActive(true);
		Objects.MiddleGear[1][1].SetActive(false);
		Objects.MiddleGear[1][2].SetActive(true);
		Objects.MiddleGear[1].Txt[0].text = "Unlocks";
		Objects.MiddleGear[1].Txt[3].enabled = false;
		Objects.MiddleGear[1][2].Txt[0].text = Player.Level.XP_Current + "/" + Player.Level.XP_Required;
		Objects.MiddleGear[1][2].Txt[1].text = "" + Player.Level.Level;
		Objects.MiddleGear[1][2].Txt[2].text = "Player Level";
		Objects.MiddleGear[1][2].Img[1].fillAmount = Player.Level.XP_Ratio;
		Objects.MiddleGear[1][2].Img[2].color = Player.Level.LevelColor;

		Objects.MiddleGear[1][0].DestroyChildren();
		List<UIObj> unlocks = new List<UIObj>();
		for(int i = 0; i < Player.instance.Unlocks.Length; i++)
		{
			UIObj b = GenerateUnlock(Objects.MiddleGear[1][0].transform, Player.instance.Unlocks[i]);
			unlocks.Add(b);
		}
		Objects.MiddleGear[1][0].Child = unlocks.ToArray();
	}

	public UIObj GenerateUnlock(Transform TopParent, Unlock u)
	{
		UIObj ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
		Transform ParentTrans = ParentObj.transform;
		ParentTrans.SetParent(TopParent);
		ParentTrans.position = Vector3.zero;
		ParentTrans.localScale = Vector3.one;
		ParentTrans.localRotation = Quaternion.Euler(0,0,0);

		HorizontalLayoutGroup hori = ParentObj.GetComponent<HorizontalLayoutGroup>();
		hori.spacing = 130;
		hori.childAlignment =  TextAnchor.MiddleLeft;

		Color unlockcol = u.Value ? Player.Level.GetColor(u.Level_Required) : Color.grey;
		UIObj[] child = new UIObj[2];
		child[0] = (UIObj) Instantiate(Objects.TextObj);
		child[0].transform.SetParent(ParentTrans);
		child[0].transform.position = Vector3.zero;
		child[0].transform.localScale = Vector3.one * 1.6F;
		child[0].transform.localRotation = Quaternion.Euler(0,0,0);
		child[0].Txt[0].text = "" + u.Level_Required;
		child[0].Txt[0].fontSize = 100;
		child[0].Txt[0].color = Player.Level.GetColor(u.Level_Required);
		child[0].Txt[0].GetComponent<RectTransform>().pivot = new Vector2(0, 0.5F);
		//child[0].Txt[0].alignment = AlignmentTypes.Left;

		child[1] = (UIObj) Instantiate(Objects.TextObj);
		child[1].transform.SetParent(ParentTrans);
		child[1].transform.position = Vector3.zero;
		child[1].transform.localScale = Vector3.one * 1.6F;
		child[1].transform.localRotation = Quaternion.Euler(0,0,0);
		child[1].Txt[0].text = u.GetTitle();
		child[1].Txt[0].fontSize = 60;
		child[1].Txt[0].color = unlockcol;
		child[1].Txt[0].GetComponent<RectTransform>().pivot = new Vector2(0, 0.5F);
		//child[1].Txt[0].alignment = AlignmentTypes.Left;
		
		ParentObj.Child = child;

		return ParentObj;
	}

	public void GenerateZoneMap()
	{
		Objects.MiddleGear[1][0].DestroyChildren();
		List<UIObj> Brackets = new List<UIObj>();
		for(int i = 0; i < GameManager.ZoneMap.Length; i++)
		{
			UIObj bracket = GenerateZoneChoice(Objects.MiddleGear[1][0].transform, GameManager.ZoneMap[i].Choices);
			Brackets.Add(bracket);
			if(i > GameManager.ZoneMap.Current)
			{
				for(int c = 0; c < bracket.Length; c++)
				{
					bracket.Child[c].Txt[0].text = "?";
					bracket.Child[c].Img[0].color = Color.grey;
				}
			}
		}
		Objects.MiddleGear[1][0].Child = Brackets.ToArray();
	}

	public void ShowZoneUI(bool ended)
	{
		(Objects.MiddleGear[1] as UIObjTweener).SetTween(0);
		bool open = (Objects.MiddleGear[1] as UIObjTweener).Tween.IsObjectOpened();
		GameManager.instance.paused = open;
		ScreenAlert.SetTween(0,open);

		Objects.MiddleGear[1].Txt[3].enabled = false;
		Objects.MiddleGear[1].Img[2].enabled = false;
		Objects.MiddleGear[1].Img[4].enabled = false;
		Objects.MiddleGear[1][0].SetActive(true);
		Objects.MiddleGear[1][1].SetActive(false);
		Objects.MiddleGear[1][2].SetActive(true);

		if(!open) UIManager.instance.SetClassButtons(false);
		if(ended)
		{
			//TITLE
			Objects.MiddleGear[1].Txt[0].text = "Escaped " + GameManager.Zone.Name;
			//ZONE INFO
			Objects.MiddleGear[1].Txt[1].text = "";
			Objects.MiddleGear[1].Txt[2].enabled = false;

			//CHOOSE ZONE
			Objects.MiddleGear[1].Txt[3].enabled = true;
			Objects.MiddleGear[1][2].Txt[2].text = "Waves";

			for(int i = 0; i < Objects.MiddleGear[1][0].Length; i++)
			{
				UIObj bracket = Objects.MiddleGear[1][0].Child[i];
				for(int c = 0; c < bracket.Length; c++)
				{
					if(i == GameManager.ZoneMap.Current)
					{
						bracket.Child[c].Txt[0].text = GameManager.ZoneMap[i][c]._Name;
						bracket.Child[c].Img[0].color = GameManager.ZoneMap[i][c].Tint;
						bracket.Child[c].ClearActions();

						bracket.Child[c].AddAction(UIAction.MouseUp, (string []	 val)=>
						{
							int v = GameData.StringToInt(val[0]);
							GameManager.instance.AdvanceZoneMap(v);
						}, c +"");
					}
					else if(i > GameManager.ZoneMap.Current)
					{
						bracket.Child[c].ClearActions();
						bracket.Child[c].Txt[0].text = "?";
						bracket.Child[c].Img[0].color = Color.grey * 0.6F;
					}
					else 
					{
						bracket.Child[c].Txt[0].text = GameManager.ZoneMap[i][c]._Name;
						bracket.Child[c].Txt[0].color = Color.grey;
						bracket.Child[c].Img[0].color = GameManager.ZoneMap[i][c].Tint * 0.6F;
						bracket.Child[c].ClearActions();
					}
				}
			}
		}
		else
		{
			Objects.MiddleGear[1].Txt[0].text = GameManager.Zone.Name;
			Objects.MiddleGear[1][2].Txt[1].text = GameManager.Wave.Name;
			Objects.MiddleGear[1][2].Txt[2].text = "Waves";
			Objects.MiddleGear[1][2].Txt[0].text = GameManager.Zone.CurrentDepthInZone + "/" + GameManager.Zone.GetZoneDepth();

			Objects.MiddleGear[1].Txt[3].enabled = false;
			Objects.MiddleGear[1].Txt[2].enabled = false;

			Objects.MiddleGear[1][2].Img[1].fillAmount = GameManager.Zone.GetZoneDepth_Ratio();
			Objects.MiddleGear[1].Img[3].enabled = false;

			for(int i = 0; i < Objects.MiddleGear[1][0].Length; i++)
			{
				UIObj bracket = Objects.MiddleGear[1][0].Child[i];
				for(int c = 0; c < bracket.Length; c++)
				{
					bracket.Child[c].ClearActions();
					if(i > GameManager.ZoneMap.Current)
					{
						bracket.Child[c].ClearActions();
						bracket.Child[c].Txt[0].text = "?";
						bracket.Child[c].Img[0].color = Color.grey * 0.6F;
					}
					else 
					{
						bracket.Child[c].Txt[0].text = GameManager.ZoneMap[i][c]._Name;
						bracket.Child[c].Txt[0].color = Color.white;
						bracket.Child[c].Img[0].color = GameManager.ZoneMap[i][c].Tint;
					}
				}
			}
		}
	}

	public UIObj GenerateZoneChoice(Transform TopParent, params Zone [] zones)
	{
		UIObj ParentObj = (UIObj) Instantiate(Objects.HorizontalGrouper);
		Transform ParentTrans = ParentObj.transform;
		ParentTrans.SetParent(TopParent);
		ParentTrans.position = Vector3.zero;
		ParentTrans.localScale = Vector3.one;
		ParentTrans.localRotation = Quaternion.Euler(0,0,0);

		HorizontalLayoutGroup hori = ParentObj.GetComponent<HorizontalLayoutGroup>();
		hori.spacing = 310;
		hori.childAlignment =  TextAnchor.MiddleCenter;

		List<UIObj> child = new List<UIObj>();
		for(int i = 0; i < zones.Length; i++)
		{
			UIObj new_desc = (UIObj) Instantiate(Objects.ZoneObj);
			new_desc.transform.SetParent(ParentTrans);
			new_desc.transform.position = Vector3.zero;
			new_desc.transform.localScale = Vector3.one * 1.6F;
			new_desc.transform.localRotation = Quaternion.Euler(0,0,0);
			new_desc.Txt[0].text = zones[i]._Name;
			new_desc.Img[0].color = zones[i].Tint;
			child.Add(new_desc);
		}
		ParentObj.Child = child.ToArray();

		return ParentObj;
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

	public IEnumerator ShowDeathIcon(Class c, bool outcome_death)
	{
		GameManager.instance.paused = true;
		ScreenAlert.SetActive(true);
		ScreenAlert.SetTween(0,true);

		Objects.DeathIcon.gameObject.SetActive(true);
		Objects.DeathIcon.SetFrame(0);
		Objects.DeathIcon.Play("Title Animation");

		//Objects.DeathIcon.Img[0].enabled = true;
		Transform death = Objects.DeathIcon.transform;
		Transform classbutton = ClassButtons.GetClass(c.Index).Img[0].transform;
		death.position = classbutton.position;

		float rising_time = 0.8F;
		float rising_acc = 0.35F, rising_decay = 0.02F;
		while((rising_time -= Time.deltaTime) > 0.0F)
		{
			death.position += Vector3.up * rising_acc;
			if(rising_acc > 0.0F) rising_acc -= rising_decay;
			yield return null;
		}

		yield return new WaitForSeconds(1.2F);

		float spin_time = 1.3F;
		float spin_acc = 35F, spin_decay = 0.7F;
		while((spin_time -= Time.deltaTime) > 0.0F)
		{
			classbutton.Rotate(0,spin_acc, 0);
			spin_acc -= spin_decay;
			yield return null;
		}
		if(outcome_death) c.OnDeath();
		MiniAlertUI m = UIManager.instance.MiniAlert(classbutton.position, (outcome_death ? "DEATH!" : "SAFE!"), 75, GameData.Colour(c.Genus), 1.2F, 0.2F);
		(ClassButtons.GetClass(c.Index) as UIClassButton).Death.enabled = outcome_death;
		float falling_time = 0.8F;
		float falling_acc = 0.5F;
		while((falling_time -= Time.deltaTime) > 0.0F)
		{
			classbutton.rotation = Quaternion.Slerp(classbutton.rotation, Quaternion.Euler(0,0,0), 0.2F);
			death.position += Vector3.down *falling_acc;
			yield return null;
		}
		Objects.DeathIcon.gameObject.SetActive(false);

		GameManager.instance.paused = false;
		ScreenAlert.SetActive(false);
		ScreenAlert.SetTween(0,false);

		yield return null;
	}


	public void ShowTuteAlert(string s)
	{
		TuteAlert.SetTween(0, true);
		GameManager.instance.paused = true;
		ScreenAlert.SetTween(0, true);
		TuteAlert.Txt[0].text = s;

		TuteAlert.Child[0].AddAction(UIAction.MouseUp, ()=>
		{
			GameManager.instance.paused = false;
			ScreenAlert.SetTween(0, false);
			TuteAlert.SetTween(0, false);
		});
	}

	public ObjectPooler MiniAlertPool;

	public MiniAlertUI MiniAlert(Vector3 position, string alert, float size = 30, Color? col = null, float life = 0.6F, float speed = 0.1F, bool background = false)
	{
		if(MiniAlertPool == null)
		{
			MiniAlertPool = new ObjectPooler(Objects.MiniAlert.gameObject, 1, Objects.MiniAlertPool);
		}
		GameObject gobj = MiniAlertPool.Spawn();
		if(gobj == null) return null;
		MiniAlertUI alertobj = gobj.GetComponent<MiniAlertUI>();
		alertobj.transform.SetParent(Canvas.transform);
		alertobj.transform.localScale = Vector3.one;
		alertobj.Setup(position, alert, life, size, col??Color.white, speed, background);
		alertobj.GetComponent<MoveToPoint>().enabled = false;
		alertobj.ClearActions();
		alertobj.transform.rotation = Quaternion.identity;
		return alertobj;
	}

	public MiniAlertUI MiniAlert(MiniAlertUI prev)
	{
		if(MiniAlertPool == null)
		{
			MiniAlertPool = new ObjectPooler(Objects.MiniAlert.gameObject, 1, Objects.MiniAlertPool);
		}
		MiniAlertUI alertobj = MiniAlertPool.Spawn().GetComponent<MiniAlertUI>();
		alertobj.transform.SetParent(Canvas.transform);
		alertobj.transform.localScale = Vector3.one;
		alertobj.Setup(prev);
		alertobj.ClearActions();
		alertobj.GetComponent<MoveToPoint>().enabled = false;
		alertobj.transform.rotation = Quaternion.identity;
		return alertobj;
	}

	public MoveToPoint AttachMoverToAlert(ref MiniAlertUI alert)
	{
		MoveToPoint mini = alert.GetComponent<MoveToPoint>();
		mini.enabled = true;
		alert.AddAction(() => {
				mini.transform.SetParent(Canvas.transform);
				mini.enabled = true;
			});
		alert.DestroyOnEnd = false;
		alert = alert;
		return mini;
	}

}
