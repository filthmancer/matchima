using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIKillScreen : UIObj {

	public GameObject KillBox;
	public TextMeshProUGUI Turns;
	public TextMeshProUGUI EndType;
	public TextMeshProUGUI Blurb;

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
		int totalxp = 0;
		Color endcol = Color.white;
		switch(e)
		{
			case End_Type.Victory:
			endtext = "Victory";
			endcol = Color.green;
			break;
			case End_Type.Defeat:
			endtext = "Defeated";
			endcol = Color.red;
			break;
			case End_Type.Retire:
			endtext = "Retired";
			endcol = Color.yellow;
			break;
		}

		Txt[0].text = GameManager.Floor + " Depth";
		Txt[1].text = "In " + Player.instance.Turns + " Turns";
		Txt[2].text = "0";

		Txt[0].enabled = false;
		Txt[1].enabled = false;
		Txt[2].enabled = false;
		Txt[3].enabled = false;

		EndType.enabled = false;
		Child[0].SetActive(false);
		UIManager.instance.ScreenAlert.SetActive(true);
		UIManager.instance.ScreenAlert.SetTween(0,true);
		KillBox.SetActive(true);

		yield return new WaitForSeconds(Time.deltaTime * 5);
		//EndType.enabled = true;
		MiniAlertUI xp = UIManager.instance.MiniAlert(EndType.transform.position, endtext, 170);
		xp.AddJuice(Juice.instance.BounceB, 0.1F);
		xp.DestroyOnEnd = false;
		xp.Txt[0].color = endcol;

		yield return new WaitForSeconds(Time.deltaTime * 10);
		totalxp = xp_steps[0];
		MiniAlertUI totalxp_alert = UIManager.instance.MiniAlert(Txt[2].transform.position, "" + totalxp + " xp", 170);
		totalxp_alert.AddJuice(Juice.instance.BounceB, 0.1F);
		totalxp_alert.DestroyOnEnd = false;
		totalxp_alert.Txt[0].color = Color.white;

		
		//Txt[2].text = "" + totalxp + " xp";

		yield return new WaitForSeconds(Time.deltaTime * 25);

		MiniAlertUI diff = UIManager.instance.MiniAlert(Txt[3].transform.position, GameManager.instance.DifficultyMode + " Mode", 100);
		diff.AddJuice(Juice.instance.BounceB, 0.1F);
		diff.DestroyOnEnd = false;
		diff.Txt[0].color = endcol;

		yield return new WaitForSeconds(Time.deltaTime * 10);
		totalxp = xp_steps[1];
		totalxp_alert.text = "" + totalxp + " xp";
		totalxp_alert.AddJuice(Juice.instance.BounceB, 0.1F);

		yield return new WaitForSeconds(Time.deltaTime * 45);
		//Txt[0].enabled = true;
		MiniAlertUI depth = UIManager.instance.MiniAlert(Txt[0].transform.position, GameManager.Floor + " Depth", 130);
		depth.AddJuice(Juice.instance.BounceB, 0.1F);
		depth.DestroyOnEnd = false;

		yield return new WaitForSeconds(Time.deltaTime * 10);
		totalxp += xp_steps[2];
		totalxp_alert.text = "" + totalxp + " xp";
		totalxp_alert.AddJuice(Juice.instance.BounceB, 0.1F);

		yield return new WaitForSeconds(Time.deltaTime * 45);
		//Txt[1].enabled = true;
		MiniAlertUI turns = UIManager.instance.MiniAlert(Txt[1].transform.position, "In " + Player.instance.Turns + " Turns", 130);
		turns.AddJuice(Juice.instance.BounceB, 0.1F);
		turns.DestroyOnEnd = false;

		yield return new WaitForSeconds(Time.deltaTime * 10);
		totalxp += xp_steps[3];
		totalxp_alert.text = "" + totalxp + " xp";
		totalxp_alert.AddJuice(Juice.instance.BounceB, 0.1F);

		//UIManager.instance.ShowPlayerLvl(1);
		yield return new WaitForSeconds(Time.deltaTime * 15);

		Child[0].ClearActions();
		Child[0].AddAction(UIAction.MouseDown, () => {
			StartCoroutine(GameManager.instance.Reset());
			diff.PoolDestroy();
			totalxp_alert.PoolDestroy();
			depth.PoolDestroy();
			xp.PoolDestroy();
			turns.PoolDestroy();
		});
		Child[0].SetActive(true);
		//KillerInfo.text = "YOU WERE KILLED BY A GRUNT";
		//Turns.text = "YOU SURVIVED " + Player.instance.Turns + " TURNS";
		//BestCombo.text = "BEST COMBO : " + Player.instance.BestCombo + " PTS";
		//Class.text = Player.instance.Class.Name + " : " + Player.instance.Class.Level;
		//StartCoroutine(CycleInfo());

		yield return null;
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