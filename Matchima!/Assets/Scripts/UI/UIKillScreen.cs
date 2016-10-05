using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIKillScreen : UIObj {

	public GameObject KillBox;
	public TextMeshProUGUI Blurb, EndType;

	public MiniAlertUI Mode, Depth, Turns;
	public MiniAlertUI xp;

	public MiniAlertUI endtype_alert, endtype_mult;

	public IEnumerator Activate(End_Type e, int [] xp_steps)
	{
		yield return StartCoroutine(CheckPoints(e, xp_steps));
	}

	public void Deactivate()
	{
		Child[0].ClearActions();
		KillBox.SetActive(false);
	}

	IEnumerator CheckPoints(End_Type e, int [] xp_steps)
	{

		string endtext = "";
		int currtotal = 0;
		float multi = 1.0F;

		Color endcol = Color.white;
		switch(e)
		{
			case End_Type.Victory:
			endtext = "Victory";
			endcol = Color.green;
			multi = 2.0F;
			break;
			case End_Type.Defeat:
			endtext = "Defeated";
			endcol = Color.red;
			multi = 1.7F;
			break;
			case End_Type.Retire:
			endtext = "Retired";
			endcol = Color.yellow;
			multi = 1.0F;
			break;
		}

		
		xp.SetVelocity(Vector3.zero);
		Turns.SetVelocity(Vector3.zero);
		Depth.SetVelocity(Vector3.zero);
		Mode.SetVelocity(Vector3.zero);

		Child[0].SetActive(false);
		Child[1].SetActive(false);
		UIManager.instance.ScreenAlert.SetActive(true);
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.Objects.BotGear.SetTween(3, true);
		UIManager.Objects.BotGear[0].Txt[0].text = "";
		UIManager.Objects.BotGear[0].Txt[1].text = "";
		KillBox.SetActive(true);

		yield return new WaitForSeconds(Time.deltaTime * 15);

		endtype_alert = UIManager.instance.MiniAlert(EndType.transform.position, endtext, 145, endcol, 1.4F, 0.0F);
		endtype_alert.DestroyOnEnd = false;
		endtype_alert.AddJuice(Juice.instance.BounceB, 0.7F);

		endtype_mult = UIManager.instance.MiniAlert(EndType.transform.position +Vector3.down, "("+multi.ToString("0.0") + "x xp)", 70, endcol, 1.4F, 0.0F);
		endtype_mult.DestroyOnEnd = false;
		endtype_mult.AddJuice(Juice.instance.BounceB, 0.7F);
		yield return new WaitForSeconds(Time.deltaTime * 25);

		xp.text = "0 xp";
		xp.AddJuice(Juice.instance.BounceB, 0.7F);
		xp.DestroyOnEnd = false;
		xp.Txt[0].color = Color.white;

		float ratio = 0.0F;
		float xptotal = xp_steps[0] + xp_steps[1] + xp_steps[2];
		float [] xpratios = new float [] {xp_steps[0]/xptotal, (xp_steps[0] + xp_steps[1])/xptotal, (xp_steps[0] + xp_steps[1] + xp_steps[2])/xptotal};

		
		yield return new WaitForSeconds(Time.deltaTime * 25);

		//MiniAlertUI diff = UIManager.instance.MiniAlert(Txt[3].transform.position, GameManager.instance.DifficultyMode + " Mode", 100);
		Mode.text = GameManager.instance.DifficultyMode + " Mode";
		Mode.AddJuice(Juice.instance.BounceB, 0.7F);
		Mode.DestroyOnEnd = false;
		Mode.Txt[0].color = GameData.Colour(GENUS.STR);

		currtotal = xp_steps[0];
		xp.text = "" + currtotal + " xp";
		xp.AddJuice(Juice.instance.BounceB, 0.7F);


		while(ratio < xpratios[0])
		{
			ratio+= Time.deltaTime;
			Img[0].fillAmount = ratio;
			yield return null;
		}
		
		

		yield return new WaitForSeconds(Time.deltaTime * 25);
		//Txt[0].enabled = true;
		//MiniAlertUI depth = UIManager.instance.MiniAlert(Txt[0].transform.position, GameManager.Floor + " Depth", 130);
		Depth.text = "On Depth " + GameManager.Floor;
		Depth.AddJuice(Juice.instance.BounceB, 0.7F);
		Depth.DestroyOnEnd = false;
		Depth.Txt[0].color = GameData.Colour(GENUS.DEX);

		currtotal = xp_steps[1];
		xp.text = "" + currtotal + " xp";
		xp.AddJuice(Juice.instance.BounceB, 0.7F);

		while(ratio < xpratios[1])
		{
			ratio+= Time.deltaTime;
			Img[1].fillAmount = ratio;
			yield return null;
		}
		
		yield return new WaitForSeconds(Time.deltaTime * 25);
		//Txt[1].enabled = true;
		//MiniAlertUI turns = UIManager.instance.MiniAlert(Txt[1].transform.position, "In " + Player.instance.Turns + " Turns", 130);
		Turns.text = "In " + Player.instance.Turns + " Turns";
		Turns.AddJuice(Juice.instance.BounceB, 0.7F);
		Turns.DestroyOnEnd = false;
		Turns.Txt[0].color = GameData.Colour(GENUS.WIS);

		currtotal += xp_steps[2];
		xp.text = "" + currtotal + " xp";
		xp.AddJuice(Juice.instance.BounceB, 0.7F);

		while(ratio < xpratios[2])
		{
			ratio+= Time.deltaTime;
			Img[2].fillAmount = ratio;
			yield return null;
		}

	
		//UIManager.instance.ShowPlayerLvl(1);
		yield return new WaitForSeconds(Time.deltaTime * 15);

		Child[0].ClearActions();
		Child[0].AddAction(UIAction.MouseDown, () => {
			Hide();
			StartCoroutine(GameManager.instance.ExitGame());
			//diff.PoolDestroy();
			//xp.PoolDestroy();
			//depth.PoolDestroy();
			//xp.PoolDestroy();
			//turns.PoolDestroy();
		});
		Child[0].SetActive(true);

		//Child[1].SetActive(true);

		yield return null;
	}

	public void Hide()
	{
		Depth.text = "";
		Turns.text = "";
		xp.text = "";
		Mode.text = "";
		for(int i = 0; i < Img.Length; i++) Img[i].fillAmount = 0.0F;
		KillBox.SetActive(false);
		endtype_alert.PoolDestroy();
		endtype_mult.PoolDestroy();
		UIManager.Objects.BotGear.SetTween(3, false);
		(UIManager.instance.AdAlertMini as UIObjTweener).SetTween(0, false);
		(UIManager.instance.AdAlertMini[0] as UIObjTweener).SetTween(0, false);
	}

	IEnumerator CycleInfo()
	{
		bool isCycling = true;
		float cycletime = 4.0F;

		float currtime = cycletime;
		int currentcycle = 0;
		yield break;
		/*while (isCycling)
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
		}*/
	}


}