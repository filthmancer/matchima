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
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);
		
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Parent.Genus));
		
		powerup.transform.SetParent(UIManager.ClassButtons.GetClass(Parent.Index).transform);
		powerup.transform.position = UIManager.ClassButtons.GetClass(Parent.Index).transform.position;
		powerup.transform.localScale = Vector3.one;

		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			"Barbarian Casts", 70, GameData.Colour(Parent.Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Heal", 170, GameData.Colour(Parent.Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 3,
			"Tap the heart!", 160, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		Destroy(powerup);
		
		
		float final_ratio = 0.0F;
		UIObj MGame = (UIObj)Instantiate(MinigameObj[0]);
		MGame.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		MGame.transform.localScale = Vector3.one;
		MGame.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		MGame.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		MGame.AddAction(UIAction.MouseDown, () => 
		{
			final_ratio += 0.07F;
		});

		float taptimer = 3.0F;
		bool istapping = true;
		while(istapping)
		{
			if(final_ratio > 0.0F) final_ratio -= 0.003F;
			MGame.Img[1].fillAmount = Mathf.Lerp(MGame.Img[1].fillAmount, final_ratio, Time.deltaTime * 5);
			MGame.Txt[0].text = "Healing\n" + (int)(final_ratio * HealTotal) + "%";

			taptimer -= Time.deltaTime;

			if(taptimer <= 0.0F || final_ratio >= 1.0F) istapping = false;
			yield return null;
		}
		MGame.ClearActions(UIAction.MouseDown);

		
		GameObject initpart = EffectManager.instance.PlayEffect(MGame.transform, Effect.Force);
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
