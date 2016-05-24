using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ClassUpgradeUI : MonoBehaviour {

	Class _class;
	public TextMeshProUGUI 	Name, Level, LevelPoints, Meter, HP, Attack, Armour,
							Str, Dex, Wis, Cha,
							StrName, DexName, WisName, ChaName,
							BonusANum, BonusACost, BonusBNum, BonusBCost,
							SlotA, SlotB, SlotC;

	public UIObj STR, DEX, WIS, CHA, BonusA, BonusB;

	public Image Icon;

	public Image SlotAImg, SlotBImg, SlotCImg;
	public Image ArmourBanner;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetInfo(Class c)
	{
		_class = c;
		Name.text = c.Name;
		Name.color = c._Name.Colour;
		Icon.sprite = c.Icon;
		Icon.color = GameData.Colour(c.Genus);

		HP.text = c.Stats.Health + "/" + c.Stats.HealthMax;
		ArmourBanner.enabled = c.Stats._Armour > 0;
		Armour.text = (c.Stats._Armour > 0 ? c.Stats.Armour: "");
		Attack.text = c.Stats.Attack;

		Level.text = "LVL: " + c.Level;
		LevelPoints.text = c.LevelPoints+"";
		Meter.text = c.Meter + "/" + c.MeterTop;

		STR.Txt[1].text = "" + c.Stats.Strength;
		DEX.Txt[1].text = "" + c.Stats.Dexterity;
		WIS.Txt[1].text = "" + c.Stats.Wisdom;
		CHA.Txt[1].text = "" + c.Stats.Charisma;

		STR.Txt[0].fontStyle = (c.Genus == GENUS.STR ? FontStyles.Underline : FontStyles.Normal);
		DEX.Txt[0].fontStyle = (c.Genus == GENUS.DEX ? FontStyles.Underline : FontStyles.Normal);
		WIS.Txt[0].fontStyle = (c.Genus == GENUS.WIS ? FontStyles.Underline : FontStyles.Normal);
		CHA.Txt[0].fontStyle = (c.Genus == GENUS.CHA ? FontStyles.Underline : FontStyles.Normal);

		//BonusA.Txt[0].text = c.BonusA.Name;
		//BonusA.Txt[1].text = c.BonusA.ValueString;

		//BonusB.Txt[0].text = c.BonusB.Name;
		//BonusB.Txt[1].text = c.BonusB.ValueString;
	}

	public void StatButton(int i)
	{
		//_class.UpgradeStat(i);
		
		GetInfo(_class);
	}

}
