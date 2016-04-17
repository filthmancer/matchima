using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UIClassButton : UIObj {

	public TextMeshProUGUI Name;
	public TextMeshProUGUI LevelUp;
	public Image Back, _Frame, _FrameMask, _Sprite, _SpriteMask;
	public Image Target, Death;
	public int index;
	public UIObj Banner;
	public UIObj Health;

	public UIButton [] SlotUI;

	public Class _class;
	public Animator Anim;
	public EasyTween PartialOpen, FullOpen;
	bool class_set;

	public string [] input;

	Color color_default;

	bool over;
	bool isPulsing = false;

	bool activated = false;
	public bool shop_activated = false;
	float tooltip_timer = 0.3F;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {
		if(!class_set && _class != null) AddListener(_class);
		if(!Application.isEditor && Input.touches.Length == 0) over = false;
		if(over && _class != null)
		{
			//if(!UIManager.InMenu) 
			//{
				if(tooltip_timer > 0.0F)	tooltip_timer -= Time.deltaTime;
				else 
				{
					UIManager.instance.ShowSimpleTooltip(true, this.transform, _class._Name, _class._Desc);
					over = false;
				}
		//	}
		}
		else tooltip_timer = 0.3F;

		if(_class)
		{
			Death.enabled = _class.isKilled;
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

			if(_class.LevelUpAlert) 
			{
				UIManager.instance.MiniAlert(transform.position + Vector3.up * 0.23F,
											 "LVL UP!", 50, GameData.Colour(_class.Genus));
				_class.LevelUpAlert = false;
			}
			if(_class.PulseAlert)
			{
				_class.PulseAlert = false;
				//this.GetComponent<Animator>().SetTrigger("Pulse");
			}
			GetCooldown();
		}

		activated = UIManager.instance.current_class == this._class;
	//	Anim.SetBool("Activate", activated);
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
		_class = ab;

		color_default = GameData.instance.GetGENUSColour(_class.Genus);
		_Frame.sprite = TileMaster.Genus.Frame[(int)_class.Genus];
		_FrameMask.sprite = TileMaster.Genus.Frame[(int)_class.Genus];

		LevelUp.color = color_default;
		
		_Sprite.sprite = ab.Icon;
		_Sprite.color = color_default;
		_Sprite.enabled = true;
		_SpriteMask.sprite = ab.Icon;
		_SpriteMask.enabled = false;
		this.GetComponent<Button>().enabled = true;
		_FrameMask.fillAmount = 0.0F;
	}

	public void Remove()
	{
		_class = null;
		Name.text = "";
		_Sprite.enabled = false;
		this.GetComponent<Button>().enabled = false;
	}

	void GetCooldown()
	{
		if(float.IsNaN(_class.GetMeterRatio())) return;
		_FrameMask.fillAmount = Mathf.Lerp(_FrameMask.fillAmount, 1.0F - _class.GetMeterRatio(), 0.3F);
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
		_class.activated = !_class.activated;

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
		if(_class.activated != PartialOpen.IsObjectOpened())
		{
		 PartialOpen.OpenCloseObjectAnimation();
		}
	}

	public void MouseOver()
	{
		over = true;
		if(PlayerControl.HoldingSlot)
		{
			if(UIManager.ItemUI_active) UIManager.instance.ShowClassAbilities(_class);
			else if(UIManager.BoonUI_active && UIManager.instance.current_class == _class) UIManager.instance.ShowClassAbilities(_class, true);
		}
		//if(!PartialOpen.IsObjectOpened())
		//{
		// PartialOpen.OpenCloseObjectAnimation();
		//}
	}

	public void MouseOut()
	{
		over = false;
		UIManager.instance.ShowTooltip(false);
		//if(PartialOpen.IsObjectOpened())
		//{
		// PartialOpen.OpenCloseObjectAnimation();
		//}
	}

	void AddListener(Class ab)
	{
		GetComponent<Button>().onClick.AddListener(() => Activate(ab));
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
