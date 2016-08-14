using UnityEngine;
using System.Collections;

public class Heal : Powerup {

	public int [] HealPower = new int []
	{
		20,
		45,
		75
	};
	protected override IEnumerator Minigame(int Level)
	{
		int HealTotal = HealPower[Level-1];
		GameManager.instance.paused = true;
		
		yield return StartCoroutine(PowerupStartup());

		
		float final_ratio = 0.0F;
		UIObj MGame = (UIObj)Instantiate(MinigameObj[0]);
		MGame.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		MGame.transform.localScale = Vector3.one;
		MGame.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		MGame.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		MGame.AddAction(UIAction.MouseDown, () => 
		{
			final_ratio += 0.07F;
			MiniAlertUI alert  = UIManager.instance.MiniAlert(PlayerControl.InputPos,
			(int)(final_ratio * HealTotal) + "%", 140, GameData.Colour(Parent.Genus), 0.3F, 0.4F);
			alert.AddJuice(Juice.instance.BounceB, 0.1F);
		});


		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Tap the heart to begin", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!Input.GetMouseButton(0)) yield return null;
		m.PoolDestroy();
		
		float taptimer = 3.0F;
		bool istapping = true;
		while(istapping)
		{
			MGame.transform.localScale *= 1.0F - (Time.deltaTime/5);
			MGame.Img[1].fillAmount = Mathf.Lerp(MGame.Img[1].fillAmount, final_ratio, Time.deltaTime * 5);
			MGame.Txt[0].text ="";// "Healing\n" + (int)(final_ratio * HealTotal) + "%";

			taptimer -= Time.deltaTime;

			if(taptimer <= 0.0F || final_ratio >= 1.0F) istapping = false;
			yield return null;
		}
		MGame.ClearActions(UIAction.MouseDown);

		
		GameObject initpart = EffectManager.instance.PlayEffect(MGame.transform, "spell");
		initpart.GetComponent<MoveToPoint>().enabled = true;
		initpart.GetComponent<MoveToPoint>().SetTarget(UIManager.instance.Health.transform.position);
		initpart.GetComponent<MoveToPoint>().SetPath(0.2F, 0.2F);
		yield return new WaitForSeconds(0.7F);

		//GameObject part = Instantiate(Particle);
		//part.transform.position = UIManager.instance.Health.transform.position;
		//yield return new WaitForSeconds(part_time);

		Destroy(MGame.gameObject);

		Player.Stats.Heal((int)(HealTotal * final_ratio));
		Player.Stats.CompleteHealth();

		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		GameManager.instance.paused = false;
		yield return null;
	}
}
