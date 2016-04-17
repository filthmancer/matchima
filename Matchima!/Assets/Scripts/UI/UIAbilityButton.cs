using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIAbilityButton : MonoBehaviour {

	public Text Name;
	public Image Back, Cooldown, _Sprite;
	public int index;

	public Ability ability;
	public Slot slot;
	bool slot_set;

	public string [] input;

	Color color_default;

	bool over;
	bool isPulsing = false;

	bool activated = false;
	public bool shop_activated = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!slot_set && slot != null) AddListener();
		if(!Application.isEditor && Input.touches.Length == 0) over = false;
		if(over && slot != null)
		{
			//UIManager.instance.ShowTooltip(true, this);
			over = false;
		}

		if(slot)
		{
			GetCooldown();
			if(slot.activated) 
			{
				activated = true;
			}
			else if(shop_activated) activated = true;
			else activated = false;
		}
		else if(shop_activated) activated = true;
		else activated = false;
		transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * (activated ? 1.3F : 1.0F), Time.deltaTime * 20);
	}


	public void Setup(Slot s)
	{
		slot = s;

		if(slot == null)
		{
			_Sprite.sprite = null;
			_Sprite.enabled = false;
			this.GetComponent<Button>().enabled = false;
			Cooldown.fillAmount = 0.0F;
		}
		else
		{
			_Sprite.sprite = s.Icon;
			_Sprite.color = s.Colour;
			_Sprite.enabled = true;
			this.GetComponent<Button>().enabled = true;
			Cooldown.fillAmount = 0.0F;
		}
	}

	public void Remove()
	{
		slot = null;
		Name.text = "";
		_Sprite.enabled = false;
		this.GetComponent<Button>().enabled = false;
	}

	void GetCooldown()
	{
		if(float.IsNaN(slot.GetCooldownRatio())) return;
		Cooldown.fillAmount = Mathf.Lerp(Cooldown.fillAmount, slot.GetCooldownRatio(), 0.3F);
	}

	public void Activate()
	{
		if(slot == null || UIManager.InMenu) return;
		slot.Activate();		
	}

	public void ButtonHit()
	{
		if(UIManager.InMenu)
		{
			
		}
	}

	public void MouseOver()
	{
		over = true;
	}

	public void MouseOut()
	{
		over = false;
		UIManager.instance.ShowTooltip(false);
	}

	void AddListener()
	{
		GetComponent<Button>().onClick.AddListener(() => Activate());
		slot_set = true;
	}

	public void StartPulse()
	{
		if(isPulsing) return;
		StartCoroutine(Pulse());
	}

	public void StopPulse()
	{
		StopAllCoroutines();
		isPulsing = false;
		transform.localScale = Vector3.one;
	}

	IEnumerator Pulse()
	{
		isPulsing = true;
		float speed = 0.01F;
		while(true)
		{
			transform.localScale += Vector3.one * speed;
			if(transform.localScale.x > 1.2F || transform.localScale.x < 0.8F) speed = -speed;
			yield return null;
		}
	}

}
