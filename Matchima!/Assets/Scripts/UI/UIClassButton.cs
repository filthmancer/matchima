using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UIClassButton : UIObj {

	public TextMeshProUGUI LevelUp;
	public Image Back, _Sprite, _SpriteMask;
	public tk2dSprite _Frame;
	public tk2dClippedSprite _FrameMask;
	public Image Target, Death;
	public UIObj Banner;
	public UIObj Health;

	public UIButton [] SlotUI;

	public Class _class;
	public Animator Anim;
	public EasyTween PartialOpen, FullOpen;
	public EasyTween ClassInit;
	bool class_set;

	public string [] input;

	Color color_default;

	bool over;
	bool isPulsing = false;

	bool activated = false;
	public bool shop_activated = false;
	float tooltip_timer = 0.3F;

	// Update is called once per frame
	public void Update () {
		if(!class_set && _class != null) 
		{
			AddListener(_class);
			AddAction(UIAction.MouseOver, () => {HoverOver();});
			AddAction(UIAction.MouseDown, () => {
				if(_class.MeterLvl > 0 && !GameManager.instance.isPaused) StartCoroutine(_class.UseManaPower());
				//PartialOpen.OpenCloseObjectAnimation();
				});
			if(ClassInit.IsObjectOpened()) ClassInit.OpenCloseObjectAnimation();
		}
		else if(_class == null && ClassInit.IsObjectOpened()) ClassInit.OpenCloseObjectAnimation();
		if(!Application.isEditor && Input.touches.Length == 0) over = false;
		
		if(over && _class != null)
		{
			if(tooltip_timer > 0.0F)	tooltip_timer -= Time.deltaTime;
			else 
			{
				//UIManager.instance.ShowSimpleTooltip(true, this.transform, _class._Name, _class._Desc);
				over = false;
			}
		}
		else tooltip_timer = 0.3F;

		if(_class)
		{
			Death.enabled = _class.isKilled;
			Txt[0].text = _class.isKilled ? _class.killtimer + "": "";
			Banner.SetActive(false);
			//Banner._Text.text = _class.MeterString;//_class.LevelPoints + "\n" + _class.TurnLevelRate + ":" + _class.BonusLevelRate + ":" + _class.WaveLevelRate;
			//Banner._Text.color =  GameData.Colour(_class.Genus);
			/*if(_class.Meter > 0)
			{
				Banner.SetActive(true);
				Banner._Text.text = _class.Meter + "";
				Banner._Text.color =  GameData.Colour(_class.Genus);
			}
			else */ //Banner.SetActive(false);

			GetCooldown();
		}

		activated = UIManager.instance.current_class == this._class;
		if(UIManager.instance.current_class != null)
		{
			_Sprite.color = (activated ? color_default : color_default * Color.grey);
			_Frame.color = (activated ? Color.white : Color.grey);
		}
		else 
		{
			_Sprite.color = color_default;
			_Frame.color = Color.white;
		}

	}

	public void Setup(Class ab)
	{
		if(ab == null) 
		{
			transform.parent.gameObject.SetActive(false);
			return;
		}
		else transform.parent.gameObject.SetActive(true);

		_class = ab;
	
		color_default = Color.white;//GameData.instance.GetGENUSColour(_class.Genus);

		LevelUp.color = color_default;
		
		_Sprite.sprite = ab.Icon;
		_Sprite.color = color_default;
		_Sprite.enabled = true;
		_SpriteMask.sprite = ab.Icon;
		_SpriteMask.enabled = false;
		_FrameMask.clipTopRight = new Vector2(1,1);

		for(int i = 0; i < _class._Slots.Length; i++)
		{
			if(SlotUI.Length < i - 1) continue;
			if(_class._Slots[i] == null)
			{
				SlotUI[i].SetActive(false);
			}
			else 
			{
				SlotUI[i].SetActive(true);
				(SlotUI[i] as UISlotButton).Parent = _class;
				SlotUI[i].Setup(_class._Slots[i]);
				SlotUI[i].Txt[0].text = (_class._Slots[i] as Item).ScaleString;
			}
		}
	}

	public void Remove()
	{
		_class = null;
		_Sprite.enabled = false;
		//this.GetComponent<Button>().enabled = false;
	}

	void GetCooldown()
	{
		if(float.IsNaN(_class.GetMeterRatio())) return;
		float curr = _FrameMask.clipBottomLeft.y;
		_FrameMask.clipBottomLeft = new Vector2(0,Mathf.Lerp(curr, _class.GetMeterRatio(), 0.3F));
	}

	public void Activate(Class ab)
	{
		if(_class == null || UIManager.InMenu) return;
		PlayerControl.HeldButton = null;
		PlayerControl.HoldingSlot = false;
		//ButtonHit();
	}

	public void ButtonHit()
	{
		if(GameManager.inStartMenu)  UIManager.Menu.HeroMenu(ParentObj.Index);
		else ShowClass();
	}

	public void ShowClass(bool? active = null)
	{
		//UIManager.WaveButtons[0].Txt[1].text = "Back";
		bool actual = active ?? !_class.activated;
		_class.activated = actual;

		for(int i = 0; i < _class._Slots.Length; i++)
		{
			if(SlotUI.Length < i - 1) continue;
			if(_class._Slots[i] == null)
			{
				SlotUI[i].SetActive(false);
			}
			else 
			{
				SlotUI[i].SetActive(true);
				SlotUI[i].Setup(_class._Slots[i]);
				SlotUI[i].Txt[0].text = (_class._Slots[i] as Item).ScaleString;
			}
		}
		TweenClass(active);		
	}

	public void ActivatePower()
	{

	}

	public void TweenClass(bool? active = null)
	{
		bool actual = active ?? !_class.activated;
		if(actual != PartialOpen.IsObjectOpened())
		{
		 PartialOpen.OpenCloseObjectAnimation();
		}
	}

	public void HoverOver()
	{
		over = true;
		if(PlayerControl.HoldingSlot)
		{
			UIManager.instance.ShowClassAbilities(_class);
			if(!PartialOpen.IsObjectOpened())
			{
			 	PartialOpen.OpenCloseObjectAnimation();
			}
		}
	}

	public void MouseOut()
	{
		over = false;
		//UIManager.instance.ShowTooltip(false);
		//if(PartialOpen.IsObjectOpened())
		//{
		// PartialOpen.OpenCloseObjectAnimation();
		//}
	}

	void AddListener(Class ab)
	{
		//GetComponent<Button>().onClick.AddListener(() => Activate(ab));
		class_set = true;
	}

	IEnumerator LevelUpAlert()
	{
		LevelUp.text = "LVL UP!";
		bool isMoving = true;
		float velocity = 0.5F;
		while(isMoving)
		{
			LevelUp.transform.position += Vector3.up * velocity;
			velocity -= 0.08F;
			if(velocity < - 0.1F) isMoving = false;
			yield return null;
		}
		LevelUp.text = "";
		yield return null;
	}
}
