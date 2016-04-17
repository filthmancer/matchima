using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UILevelUp : UIObj {

	public GameObject ButtonParent;
	public GameObject DragHereText;
	public GameObject SkipButton;

	public GameObject ORObj;
	public UIButton ButtonObj;
	public UISlotButton SlotObj;
	
	private List<GameObject> current_buttons = new List<GameObject>();
	

	void Update()
	{
		DragHereText.SetActive(PlayerControl.HoldingSlot);
	}

	public void Setup(UpgradeGroup group)
	{
		Class c = group.Target;
		ClassUpgrade [] ups = group.Upgrades;

		//if(group.CanSkip)
			SkipButton.SetActive(true);
		Img[0].color = GameData.Colour(c.Genus);
		Txt[0].text = c.Name + " found a Boon!";
		if(current_buttons.Count > 0)
		{
			for(int i = 0; i < current_buttons.Count; i++)
			{
				Destroy(current_buttons[i].gameObject);
			}
		}
		current_buttons = new List<GameObject>();
		for(int i = 0; i < ups.Length; i++)
		{
			UIButton button;
			if(ups[i].slotobj != null)
			{
				button = (UIButton) Instantiate(SlotObj);
				//button.Img[1].sprite = GameData.instance.GetIconByName(ups[i].slotobj.IconString);
				//button.Img[1].enabled = true;
				Slot slotobj = (Slot) Instantiate(ups[i].slotobj);
				button.GetComponent<LayoutElement>().preferredHeight = 300;
				button.GetComponent<LayoutElement>().preferredWidth = 300;
				slotobj.Parent = UIManager.instance.current_class;
				slotobj.Init(0, ups[i].Value);

				button.Setup(slotobj);
				(button as UISlotButton).slot = slotobj;
				button.Img[1].color = slotobj.Colour;
			}
			else
			{
				button = (UIButton) Instantiate(ButtonObj);
				button.GetComponent<LayoutElement>().preferredHeight = 300;
				button.GetComponent<LayoutElement>().preferredWidth = 300;
				button.Setup(null);
				button.Txt[0].text = ups[i].ShortName; //ups[i].ValueString + " " + ups[i].Name;
				button.Img[0].color = GameData.Colour(c.Genus);
			}

			button.Upgrade = ups[i];
			button.transform.parent = ButtonParent.transform;
			button.transform.localScale = Vector3.one;
			button.Drag = DragType.Hold;
			
			current_buttons.Add(button.gameObject);
		
			if(i < ups.Length - 1)
			{
				GameObject new_or = (GameObject) Instantiate(ORObj);
				new_or.transform.parent = ButtonParent.transform;
				new_or.transform.localScale = Vector3.one;
				current_buttons.Add(new_or);
			}
		}
	}

	public void Destroy()
	{
		foreach(GameObject child in current_buttons)
		{
			Destroy(child.gameObject);
		}
		current_buttons.Clear();
	}

	/*public void Setup(Class c, params Slot [] ups)
	{
		Img[0].color = GameData.Colour(c.Genus);
		Txt[0].text = c.Name + " Lvl Up!";
		if(current_buttons.Count > 0)
		{
			for(int i = 0; i < current_buttons.Count; i++)
			{
				Destroy(current_buttons[i].gameObject);
			}
		}
		current_buttons = new List<GameObject>();
		for(int i = 0; i < ups.Length; i++)
		{
			UIObj button = (UIObj) Instantiate(ButtonObj);
			button.transform.parent = ButtonParent.transform;
			button.transform.localScale = Vector3.one;
			button.Txt[0].text = ups[i].Name_Basic;
			//button.Img[0].color = ups[i].Colour;//GameData.Colour(ups[i].Genus);//ups[i].Genus;
			AddListener(button, i);
			current_buttons.Add(button.gameObject);

			if(i < ups.Length - 1)
			{
				GameObject new_or = (GameObject) Instantiate(ORObj);
				new_or.transform.parent = ButtonParent.transform;
				new_or.transform.localScale = Vector3.one;
				current_buttons.Add(new_or);
			}
		}
	}*/

	public void AddListener(UIObj button, int i)
	{
		button.GetComponent<Button>().onClick.AddListener(() => UIManager.instance.SetChoice(i));
	}
}
