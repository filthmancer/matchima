using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UISlotButton : UIButton {

	public Slot slot;
	public Class Parent;
	bool slot_set;
	
	// Update is called once per frame
	public override void Update () {

		base.Update();

		if(slot)
		{
			GetCooldown();
		}
	}

	public override StCon [] GetTooltip()
	{
		return slot ? slot.Description_Tooltip : null;
	}

	public override string GetName()
	{
		return slot ? slot.Name.Value + "\n" + slot.Level: "";
	}
	public override Color GetColour()
	{
		return slot ? slot.Colour : Color.white;
	}

	public override void Setup(Slot s)
	{
		slot = s;
		drag_distance = GetComponent<LayoutElement>().preferredWidth/200;
		dragging = false;
		if(slot == null)
		{
			Img[0].color = color_default;
			Img[1].sprite = null;
			Img[1].enabled = false;
			this.GetComponent<Button>().enabled = false;
			Img[2].fillAmount = 0.0F;
			Drag = DragType.None;
		}
		else
		{
			if(slot is Item) 
			{
				Img[0].color = slot.Colour;
				Img[1].color = Color.black;
			}
			else 
			{
				Img[0].color = color_default;
				Img[1].color = slot.Colour;
			}
			Img[1].sprite = slot.Icon;
			Img[1].enabled = true;
			this.GetComponent<Button>().enabled = true;
			Img[2].fillAmount = 0.0F;

			Drag = DragType.Hold;
			AddAction(UIAction.MouseDown, () => {MouseOver();});
			AddAction(UIAction.MouseUp, () => {LetGo();});
		}
		init_pos = transform.position;
	}

	public void Remove()
	{
		slot = null;
		Txt[0].text = "";
		Img[1].enabled = false;
		Img[2].enabled = false;
		this.GetComponent<Button>().enabled = false;
	}

	void GetCooldown()
	{
		if(float.IsNaN(slot.GetCooldownRatio())) return;
		Img[2].fillAmount = Mathf.Lerp(Img[2].fillAmount, slot.GetCooldownRatio(), 0.3F);
	}

	public void Activate()
	{
		activated = true;	
	}



	public void LetGo()
	{
		dragging = false;
		UIManager.instance.ShowTooltip(false);
		if(PlayerControl.HeldButton != null)
		{
			if(PlayerControl.HeldButton != this)
			{
				//print("HIT");
				UIManager.instance.SwapSlotButtons(this, (PlayerControl.HeldButton as UISlotButton).Parent, 0);
				//PlayerControl.instance.SwapSlots();
				//UIManager.instance.HideBoonUI();
			}
			else if(PlayerControl.HeldButton == this && Drag == DragType.Hold)
			{
				Class c = UIManager.instance.current_class;

				if(c != null)
				{
					if(c._Slots.Length > 1)
					{
						for(int i = 0; i < c._Slots.Length; i++)
						{
							if(c._Slots[i] == null)
							{
								c.GetSlot(slot, i);
								Destroy(this.gameObject);//Setup(null);
								UIManager.instance.ShowClassAbilities(c);
								break;
							}
						}
					}
					else 
					{
						if(c._Slots[0] == null)
						{
							c.GetSlot(slot, 0);
							Destroy(this.gameObject);//Setup(null);
							UIManager.instance.ShowClassAbilities(c);
							
						}
						else 
						{
							UIManager.instance.SwapSlotButtons(this, c, 0);
						}
					}
					
				}
			}
		}
	
		GetComponent<LayoutElement>().ignoreLayout = false;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		dragging = false;
		activated = false;
		if(PlayerControl.TouchParticle != null) Destroy(PlayerControl.TouchParticle.gameObject);
		//if(slot != null) slot.activated = false;
		PlayerControl.HoldingSlot = false;
		PlayerControl.HeldButton = null;
		PlayerControl.SwapButton = null;
	}

	/*public override void MouseDown()
	{
		if(Drag != DragType.None)
		{
			activated = true;
			init_pos = transform.position;
			//if(UIManager.instance.current_class != null) UIManager.instance.ShowClassAbilities(UIManager.instance.current_class);
		}
	}*/

	public override void MouseOver()
	{
		//UIManager.instance.ShowTooltip(true, this);
		over = true;
		activated = true;
		init_pos = transform.position;
		if(PlayerControl.HoldingSlot && PlayerControl.HeldButton != this && PlayerControl.HeldButton.Drag == DragType.Hold)
		{
			PlayerControl.SwapButton = this;
			activated = true;
		}
	}

	public override void MouseOut()
	{
		over = false;
		//UIManager.instance.ShowTooltip(false);
		prepare_to_swap = false;
		PlayerControl.SwapButton = null;
	}

	void AddListener()
	{
		//GetComponent<Button>().onClick.AddListener(() => Activate());
		slot_set = true;
	}

}
