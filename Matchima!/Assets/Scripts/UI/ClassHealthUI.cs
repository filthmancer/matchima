using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassHealthUI : UIObj {

	public UIObj [] HealthBars;
	public LayoutElement [] HealthBars_Elements;
	public RectTransform HealthParent;
	public UIObj [] AggroAlert;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		int totalHealth = Player.Stats._Health;
		//for(int i = Player.AggroOrder.Length - 1; i >= 0 ; i--)
		//{
			////totalHealth += Player.Classes[i].Stats._Health;
			//HealthBars[i]._Text.text = Player.AggroOrder[i].Stats.Health;
			//HealthBars[i]._Image.color = Player.AggroOrder[i].isKilled ? Color.black : GameData.Colour(Player.AggroOrder[i].Genus) * Color.grey;
		//}

		//float totalratio = 0.0F;
//		for(int i = Player.AggroOrder.Length-1; i >= 0; i--)
//		{
//			float ratio = (float)Player.AggroOrder[i].Stats._Health / (float)totalHealth;
//			totalratio += ratio;
//			HealthBars_Elements[i].minWidth = Mathf.Lerp(HealthBars_Elements[i].minWidth, HealthParent.rect.width * ratio, Time.deltaTime * 15);
//		}


	}
}
