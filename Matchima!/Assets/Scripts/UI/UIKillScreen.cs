using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIKillScreen : MonoBehaviour {

	public GameObject KillBox;
	public TextMeshProUGUI Turns;
	public Image PointsImg;
	public TextMeshProUGUI Tens, Hunds, Thous, AllTokens;
	public TextMeshProUGUI Defeat;
	public TextMeshProUGUI KillerInfo, BestCombo, NextSpecial;

	public TextMeshProUGUI Class;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Activate(long alltokens, int tens, int hunds, int thous)
	{
		StartCoroutine(CheckPoints(alltokens, tens, hunds, thous));
		
	}

	IEnumerator CheckPoints(long currenttokens, int tens, int hunds, int thous)
	{
		yield return new WaitForSeconds(1.0F);

		KillBox.SetActive(true);
		Defeat.text = Player.instance.retired ? "RETIRED" : "DEFEATED";
		KillerInfo.text = "YOU WERE KILLED BY A GRUNT";
		Turns.text = "YOU SURVIVED " + Player.instance.Turns + " TURNS";
		BestCombo.text = "BEST COMBO : " + Player.instance.BestCombo + " PTS";
		//Class.text = Player.instance.Class.Name + " : " + Player.instance.Class.Level;
		StartCoroutine(CycleInfo());

		long finalPoints = currenttokens + tens + (hunds * 50) + (thous * 500);
		int speed = 1;
		bool isIncreasing = true;
		
		//float ratio = (float)(Player.instance.Class.PointsCurrent % 1000) / Player.instance.Class.PointsToLevel;
		//PointsImg.transform.localScale = new Vector3(ratio, 1,1);
		AllTokens.text = currenttokens + " Tokens";
		Tens.text = tens + "";
		Hunds.text = hunds + "";
		Thous.text = thous + "";

		
		yield return new WaitForSeconds(0.6F);

		while(isIncreasing)
		{
			if(tens > 0)
			{
				tens -= speed;
				currenttokens += speed;
				speed = (int) (speed * 2.0F);
				if(tens <= speed)
				{
					currenttokens += tens;
					tens = 0;
					speed = 1;
				}
				Vector3 tokenpos = Tens.transform.position + Vector3.down + Vector3.left * (Random.value - Random.value);
				TileMaster.instance.CreateMiniTile(tokenpos, AllTokens.transform);
			}
			else if(hunds > 0)
			{
				hunds -= speed;
				currenttokens += speed*10;
				speed = (int) (speed * 2.0F);
				if(hunds <= speed)
				{
					currenttokens += hunds*50;
					hunds = 0;
					speed = 1;
				}
				for(int i = 0; i < 10; i++)
				{
				Vector3 tokenpos = Hunds.transform.position + Vector3.down + Vector3.left * (Random.value - Random.value);
				TileMaster.instance.CreateMiniTile(tokenpos, AllTokens.transform);
				}
			}
			else if(thous > 0)
			{
				thous -= speed;
				currenttokens += speed*100;
				speed = (int) (speed * 2.0F);
				if(thous <= speed)
				{
					currenttokens += thous*500;
					thous = 0;
					speed = 1;
				}
				for(int i = 0; i < 50; i++)
				{
					Vector3 tokenpos = Thous.transform.position + Vector3.down + Vector3.left * (Random.value - Random.value);
					TileMaster.instance.CreateMiniTile(tokenpos, AllTokens.transform);
				}
			}
			
			if(currenttokens > finalPoints - 10)
			{
				currenttokens = finalPoints;
				tens = 0;
				hunds = 0;
				thous = 0;
				isIncreasing = false;
			}

			AllTokens.text = currenttokens + " Tokens";
			Tens.text = tens + "";
			Hunds.text = hunds + "";
			Thous.text = thous + "";

			yield return new WaitForSeconds(0.1F);
		}	

		
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