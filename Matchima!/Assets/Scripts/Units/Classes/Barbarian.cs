using UnityEngine;
using System.Collections;

public class Barbarian : Class {

	TileChance health;

	private Slot manapower;
	private int _currentmanapower;


	// Use this for initialization
	public override void StartClass () {
		
		/*ClassUpgrade a = new ClassUpgrade((int val) => {InitStats._HealthMax += 10 * val;});
		a.BaseAmount = 10;
		a.Rarity = Rarity.Common;
		a.Name = "Health Max";
		a.ShortName = "HP MAX";
		a.Description = " Maximum Health";
		a.Prefix = "+";*/

		health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.3F;
		InitStats.TileChances.Add(health);

		TileChance sword = new TileChance();
		sword.Genus = GameData.ResourceLong(Genus);
		sword.Type = "bomb";
		sword.Chance = 0.15F;
		InitStats.TileChances.Add(sword);

		PowerupSpell = GameData.instance.GetPowerup("Heal", this);

		base.StartClass();
	}


	public IEnumerator HealRoutine(int HealTotal)
	{
		activated = true;
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);
		
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Genus));
		
		powerup.transform.SetParent(UIManager.ClassButtons.GetClass(Index).transform);
		powerup.transform.position = UIManager.ClassButtons.GetClass(Index).transform.position;
		powerup.transform.localScale = Vector3.one;

		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			"Barbarian Casts", 70, GameData.Colour(Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Heal", 170, GameData.Colour(Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 3,
			"Tap the heart!", 160, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
		Destroy(powerup);
		
		
		float final_ratio = 0.0F;
		UIObj MGame = (UIObj)Instantiate(MinigameObj);
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

		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
		activated = false;
		UIManager.instance.ScreenAlert.SetTween(0,false);
		GameManager.instance.paused = false;
		yield return null;
	}

}
