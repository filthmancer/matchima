using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIKillScreen : UIObj {

	public GameObject KillBox;
	public TextMeshProUGUI Turns;
	public Image PointsImg;
	public TextMeshProUGUI AllTokens;
	public TextMeshProUGUI Defeat;
	public TextMeshProUGUI KillerInfo, BestCombo, NextSpecial;

	public TextMeshProUGUI Class;
	public void Activate(long alltokens, int tens, int hunds, int thous)
	{
		Child[0].ClearActions();
		Child[0].AddAction(UIAction.MouseUp, () => {
			UIManager.instance.Reset();
			GameManager.instance.Reset();
			TileMaster.instance.Reset();
		});
		StartCoroutine(CheckPoints(alltokens));
		
	}

	IEnumerator CheckPoints(long currenttokens)
	{
		yield return new WaitForSeconds(1.0F);

		KillBox.SetActive(true);
		Defeat.text = Player.instance.retired ? "RETIRED" : "DEFEATED";
		KillerInfo.text = "YOU WERE KILLED BY A GRUNT";
		Turns.text = "YOU SURVIVED " + Player.instance.Turns + " TURNS";
		BestCombo.text = "BEST COMBO : " + Player.instance.BestCombo + " PTS";
		//Class.text = Player.instance.Class.Name + " : " + Player.instance.Class.Level;
		StartCoroutine(CycleInfo());

		int speed = 1;
		bool isIncreasing = true;
		
		//float ratio = (float)(Player.instance.Class.PointsCurrent % 1000) / Player.instance.Class.PointsToLevel;
		//PointsImg.transform.localScale = new Vector3(ratio, 1,1);
		AllTokens.text = currenttokens + " Tokens";
		
		yield return null;
	}

	IEnumerator CycleInfo()
	{
		bool isCycling = true;
		float cycletime = 4.0F;

		float currtime = cycletime;
		int currentcycle = 0;
		while (isCycling)
		{
			if(currtime > 0.0F) currtime -= Time.deltaTime;
			else 
			{
				currentcycle ++;
				if(currentcycle >= 3) currentcycle = 0;
				currtime = cycletime;
			}
			switch(currentcycle)
			{
				case 0:
				KillerInfo.color = Color.Lerp(KillerInfo.color, Color.red, Time.deltaTime*15);
				BestCombo.color = Color.Lerp(BestCombo.color, new Color(1,1,1,0), Time.deltaTime*15);
				Turns.color = Color.Lerp(Turns.color, new Color(1,1,1,0), Time.deltaTime*15);
				break;
				case 1:
				KillerInfo.color = Color.Lerp(KillerInfo.color, new Color(1,1,1,0), Time.deltaTime*15);
				BestCombo.color = Color.Lerp(BestCombo.color, new Color(1,1,1,0), Time.deltaTime*15);
				Turns.color = Color.Lerp(Turns.color, Color.yellow, Time.deltaTime*15);
				break;
				case 2:
				KillerInfo.color = Color.Lerp(KillerInfo.color, new Color(1,1,1,0), Time.deltaTime*15);
				BestCombo.color = Color.Lerp(BestCombo.color, Color.green*Color.white, Time.deltaTime*15);
				Turns.color = Color.Lerp(Turns.color, new Color(1,1,1,0), Time.deltaTime*15);
				break;
			}
			yield return null;
		}
	}

	//int class_exp = Player.instance.Class.PointsCurrent;
		//int class_exp_final = class_exp + (int)(roundtokens / 4);
		//isIncreasing = true;
		//while(isIncreasing)
		//{
		//	if(class_exp >= class_exp_final - 10)
		//	{
		//		class_exp = class_exp_final;
		//		Player.instance.Class.PointsCurrent = class_exp_final;
		//		isIncreasing = false;
		//	}
		//	else if(class_exp >= Player.instance.Class.PointsToLevel)
		//	{
		//		Player.instance.Class.LevelUp();
		//	}
		//	else 
		//	{
		//		class_exp += 4;
		//	}
		//	ratio = (float)(class_exp) / Player.instance.Class.PointsToLevel;
		//	PointsImg.transform.localScale = new Vector3(ratio, 1,1);
		//	yield return null;
		//}
}