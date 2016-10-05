using UnityEngine;
using System.Collections;

public class UIClassSelect : UIObj {

	public Class _class;
	float drag_threshold = 0.2F;

	public bool CenterWheel;
	public void Setup(Class c)
	{
		_class = c;
		transform.localScale = Vector3.one * 0.98F;
		(transform as RectTransform).anchorMin = Vector3.zero;
		(transform as RectTransform).anchorMax = Vector3.one;
		(transform as RectTransform).anchoredPosition = Vector3.one*0.5F;
		(transform as RectTransform).offsetMin = Vector3.zero;
		(transform as RectTransform).offsetMax = Vector3.zero;
		

		if(c.Unlocked)
		{
			Img[0].color = GameData.Colour(_class.Genus);
			Img[1].sprite = _class.Icon;
			Img[1].color = Color.white;
			Txt[0].text = _class._Name.Value;
			Txt[1].text = _class.Description;
			Txt[2].text = "";//_class.PowerupSpell.Name;
			Txt[3].text = "";//_class.PowerupSpell.Instruction;
			AddAction(UIAction.MouseUp, (string [] f) => {
				UIManager.Menu.SetTargetClass(this);
				(ParentObj as UIGear).Drag = true;
			});

			Child[0].SetActive(true);
			for(int i = 0; i < Child[0].Img.Length; i++)
			{
				Child[0].Img[i].transform.localScale = Vector3.one * _class.GetStatScale(i);
			}
		}
		else
		{
			Img[0].color = Color.Lerp(GameData.Colour(_class.Genus), Color.black, 0.6F);
			Txt[0].text = "???";
			Img[1].color = Color.black;
			Child[0].SetActive(false);
			Txt[2].text = "";
			Txt[3].text = "";
		}
		
	

		AddAction(UIAction.MouseDown, () =>
		{
			(ParentObj as UIGear).Drag = false;			
		});
		 init = Img[0].color;
	}

	public override void LateUpdate()
	{
		if(isPressed)
		{
			time_over += Time.deltaTime;
			//if(time_over > drag_threshold)
			//{
				
			//}
		}
	}



}
