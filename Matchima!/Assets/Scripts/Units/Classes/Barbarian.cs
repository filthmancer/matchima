using UnityEngine;
using System.Collections;

public class Barbarian : Class {

	TileChance health;

	private Slot manapower;
	private int _currentmanapower;


	// Use this for initialization
	public override void StartClass () {
	
		health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.25F;
		InitStats.TileChances.Add(health);

		PowerupSpell = GameData.instance.GetPowerup("Heal", this);

		base.StartClass();
	}

	public override Upgrade [] Boons
	{
		get{
			return new Upgrade []
			{
				new Upgrade("Hearty", " Max HP", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax += 20 + (int)val*5;}, 5, 20),
				new Upgrade("Healing", " HP Regen", 1.0F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s.HealthRegen += 1 + (int) val;}, 1, 1),
				new Upgrade("Sharp", " Attack", 0.5F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._Attack += 1 + (int)val;}, 1, 1),


				new Upgrade("Soldier's", "% chance\n of Health", 0.7F, ScaleType.GRADIENT, 1.0F,
					(Stat s, float value) => {
						s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "health", 0.1F + 0.03F * value));}, 3, 10
					),

				new Upgrade("Strengthening", " Tile Per Match", 0.2F, ScaleType.RANK, 0.0F,
					(Stat s, float value) =>
					{
						s.MatchNumberModifier -= 1;
						}, -1, -1),

				new Upgrade("Cook's", " Map X", 0.3F, ScaleType.RANK, 0.4F,
							(Stat s, float value) => {
								s.MapSize.x += 1 + (int) (1 * value);},1,1
							),
				new Upgrade("Magellan's", " Map Y", 0.3F, ScaleType.RANK, 0.4F,
					(Stat s, float value) => {
						s.MapSize.y += 1 + (int) (1 * value);},1,1
						)
			};
		}
	}

	public override Upgrade [] Curses
	{
		get
		{
 			return new Upgrade [] 
 			{
 				new Upgrade("Hearty", " Max HP", 0.2F, ScaleType.GRADIENT, 1.0F, (Stat s, float val) => {s._HealthMax -= 10 + (int)val*5;}, -5, -10),
 				new Upgrade("Sharp", " Attack", 1.0F, ScaleType.GRADIENT, 0.12F, (Stat s, float val) => {s._Attack -= 1 + (int)val;}, -1, -1),

 				new Upgrade("Demon's", "% chance\n of Grunts", 0.4F, ScaleType.GRADIENT, 0.06F,
				(Stat s, float value) => {
					s.TileChances.Add(new TileChance(GameData.ResourceLong(Genus), "grunt", 0.1F + 0.03F * value));}, 3, 10
				),

 			};
		}
	}



	public IEnumerator HealRoutine(int HealTotal)
	{
		activated = true;
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);
		
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, GameData.Colour(Genus));
		
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

		
		GameObject initpart = EffectManager.instance.PlayEffect(MGame.transform, Effect.Spell);
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
