using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIStatsbar : MonoBehaviour {
	public TextMeshProUGUI Health, Armour;
	public TextMeshProUGUI Strength, Dexterity, Wisdom, Charisma;
	public TextMeshProUGUI Attack, AttackRate;

	public TextMeshProUGUI Combo, ComboBonus;
	public TextMeshProUGUI HealthMult, ArmourMult, GoldMult, ExpMult;
	public TextMeshProUGUI RedNum, GreenNum, BlueNum, YellowNum;

	public TextMeshProUGUI Leech, Regen, CoolDec, CostDec, Spikes, Mercantile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		SetStats();
	}

	public void SetStats()
	{
		Health.text = Player.Stats._Health + "/" + Player.Stats._HealthMax;
		//RedNum.text = "" + Player.Stats.Red.ResCurrent + "/" + Player.Stats.Red.ResMax;
		//BlueNum.text = "" + Player.Stats.Blue.ResCurrent + "/" + Player.Stats.Blue.ResMax;
		//GreenNum.text = "" + Player.Stats.Green.ResCurrent + "/" + Player.Stats.Green.ResMax;
		//YellowNum.text = "" + Player.Stats.Yellow.ResCurrent + "/" + Player.Stats.Yellow.ResMax;

		Strength.text = "" + Player.Stats.Strength;
		Dexterity.text = "" + Player.Stats.Dexterity;
		Wisdom.text = "" + Player.Stats.Wisdom;
		Charisma.text = "" + Player.Stats.Charisma;

		Attack.text = "" + Player.Stats.Attack;
		AttackRate.text = "" + Player.Stats.AttackRate;

		Combo.text = "" + Player.Stats.ComboCounter;
		ComboBonus.text = "" + Player.Stats.ComboBonus;

		HealthMult.text = Player.Stats.Red.ResMultiplier.ToString("0.0");
		ArmourMult.text = Player.Stats.Blue.ResMultiplier.ToString("0.0");
		GoldMult.text = Player.Stats.Yellow.ResMultiplier.ToString("0.0");
		ExpMult.text = Player.Stats.Green.ResMultiplier.ToString("0.0");

		//Leech.text = Player.Stats.Leech.ToString();
		//Regen.text = Player.Stats.Regen.ToString();
		CoolDec.text = Player.Stats.CooldownDecrease.ToString();
		CostDec.text = Player.Stats.CostDecrease.ToString();
		Spikes.text = Player.Stats.Spikes.ToString();
		Mercantile.text = Player.Stats.Presence.ToString();
	}

	public void ShowTooltip(string name)
	{
	//	UIManager.instance.ShowSimpleTooltip(true, this.transform, "", name);
	}

	public void HideTooltip()
	{
		//UIManager.instance.ShowTooltip(false);
	}
}