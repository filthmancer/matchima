using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ItemUI : UIObj {

	//public TextMeshProUGUI [] Item_Details = new TextMeshProUGUI[0];
	public GameObject ItemInfoParent;

	public UISlotButton [] Slots;
	public TextMeshProUGUI UpgradeObj;

	// Update is called once per frame
	void Update () {
		if(Slots.Length > 0)
		{
			for(int x = 0; x < Slots.Length; x++)
			{
				if(Slots[x] == null) continue;
				if(Slots[x].slot == null){
					Destroy(Slots[x].gameObject);
					Slots[x] = null;
					continue;
				}
				Slots[x].slot.Drag = DragType.Hold;
			}
		}
	}

	public void DestroySlots()
	{
		if(Slots.Length > 0)
		{
			for(int x = 0; x < Slots.Length; x++)
			{
				if(Slots[x] == null || Slots[x].slot == null) continue;
				Destroy(Slots[x].slot.gameObject);
				Destroy(Slots[x].gameObject);
			}
		}
		Slots = new UISlotButton [0];
	}

	public void Setup(params Slot [] i)
	{
		if(Slots.Length > 0)
		{
			for(int x = 0; x < Slots.Length; x++)
			{
				Destroy(Slots[x].gameObject);
			}
		}
		//Title.text = "Found a " + i[0].Name.Value;
		Txt[0].color = i[0].Name.Colour;
		Img[0].color = i[0].Name.Colour;
		Slots = new UISlotButton[i.Length];
		for(int ii = 0; ii < i.Length; ii++)
		{
			Slots[ii] = (UISlotButton) Instantiate(UIManager.Objects.SlotObj);
			Slots[ii].transform.parent = ItemInfoParent.transform;
			Slots[ii].transform.localScale = Vector3.one;
			Slots[ii].transform.position = Vector3.zero;
			Slots[ii].GetComponent<LayoutElement>().preferredHeight = 250;
			Slots[ii].GetComponent<LayoutElement>().preferredWidth = 250;
			Slots[ii].Setup(i[ii]);
			i[ii].Drag = DragType.Hold;
			Slots[ii].Drag = DragType.Hold;

		}
		//if(Item_Details.Length > 0)
		//{
		//	for(int old = 0; old < Item_Details.Length; old++)
		//	{
		//		Destroy(Item_Details[old].gameObject);
		//	}
		//}
		//StCon [] In = i.Description_Tooltip;
		//Item_Details = new TextMeshProUGUI[In.Length];
		//for(int u = 0; u < In.Length; u++)
		//{
		//	TextMeshProUGUI up = (TextMeshProUGUI) Instantiate(UpgradeObj);
		//	up.text = In[u].Value;
		//	up.color = In[u].Colour;
		//	up.transform.parent = ItemInfoParent.transform;
		//	up.transform.position = Vector3.zero;
		//	up.transform.localScale = Vector3.one;
		//	Item_Details[u] = up;
		//}
	}
}
