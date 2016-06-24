using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rally : Ability {

	public float HealthThresholdRatio = 0.25F;
	public int Heal = 50;
	public string DamageBuff;
	public int DamageBuff_Duration = 4;
	public int DamageBuff_AttackInc = 10;

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Activates if Health is\n below " + (HealthThresholdRatio*100).ToString("0") + "%", Color.white));
			All.Add(new StCon("Heals " + (int) (Heal * StrengthFactor) + " HP", GameData.Colour(GENUS.STR)));
			All.Add(new StCon("Gives " + DamageBuff + " status", GameData.Colour(GENUS.DEX)));
			return All.ToArray();
		}
	}


	public override void CheckHealth()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;

		if(Player.Stats.GetHealthRatio() < HealthThresholdRatio)
		{
			StartCoroutine(RallyRoutine());
		}
	}

	IEnumerator RallyRoutine()
	{
		GameObject part = Instantiate(Particle);
		part.transform.position = UIManager.ClassButtons.GetClass(Parent.Index).transform.position;
		yield return new WaitForSeconds(0.4F);

		Player.Stats.Heal((int) (Heal * StrengthFactor));
		Player.Stats.CompleteHealth();
		UIManager.instance.MiniAlert(UIManager.ClassButtons.GetClass(Parent.Index).transform.position + Vector3.up*0.6F, "Rally!", 34, GameData.Colour(Parent.Genus), 0.8F, 0.1F, true);
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			Player.instance.AddStatus(i, DamageBuff, DamageBuff_Duration, (DamageBuff_AttackInc*DexterityFactor) + "");
		}
	
		cooldown_time = cooldown;
		yield return null;
	}
}
