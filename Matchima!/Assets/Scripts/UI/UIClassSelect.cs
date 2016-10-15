using UnityEngine;
using System.Collections;
using UnityEngine.UI.Extensions;

public class UIClassSelect : UIObj {

	public Class _class;
	float drag_threshold = 0.2F;

	public bool CenterWheel;
	public RadarPolygon Chart;
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
			Img[1].sprite = _class.GetIcon();
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
			Child[1].SetActive(GameData.FullVersion);
			Child[1].ClearActions();
			Child[1].AddAction(UIAction.MouseUp, ()=>{ToggleClassIcon();});

			float str = Mathf.Clamp(_class.GetStatScale(0), 0.1F, 0.9F);
			float dex = Mathf.Clamp(_class.GetStatScale(1), 0.1F, 0.9F);
			float wis = Mathf.Clamp(_class.GetStatScale(2), 0.1F, 0.9F);
			float cha = Mathf.Clamp(_class.GetStatScale(3), 0.1F, 0.9F);

			Chart.Elements[0].value = str;
			Chart.Elements[1].value = Mathf.Min(str, cha);
			Chart.Elements[2].value = cha;
			Chart.Elements[3].value = Mathf.Min(cha, wis);
			Chart.Elements[4].value = wis;
			Chart.Elements[5].value = Mathf.Min(wis,dex);
			Chart.Elements[6].value = dex;
			Chart.Elements[7].value = Mathf.Min(dex,str);
		}
		else
		{
			Img[0].color = Color.Lerp(GameData.Colour(_class.Genus), Color.black, 0.6F);
			Txt[0].text = "???";
			Txt[1].text = "???";
			Img[1].color = Color.black;
			Child[0].SetActive(false);
			Txt[2].text = "";
			Txt[3].text = "";
			Child[1].SetActive(false);
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

	public void ToggleClassIcon()
	{
		if(!_class) return;
		_class.CycleIcon();
		Img[1].sprite = _class.GetIcon();
	}
}