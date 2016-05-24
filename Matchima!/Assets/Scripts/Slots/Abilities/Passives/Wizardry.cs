using UnityEngine;
using System.Collections;

public class Wizardry : Ability {

	public GENUS CostGenus;

	public float CooldownDecrease = 0.25F;

	Ability_UpgradeInfo CDRedUpgrade, CostRedUpgrade;

	float finalcd;

	float upgrade_cdred;
	int upgrade_costred;
	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(CDRedUpgrade.Info, GameData.Colour(GENUS.PRP)),
								new StCon(CostRedUpgrade.Info, GameData.Colour(GENUS.WIS))};
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			
			return new StCon [] {
				new StCon("Reduces all cooldowns by ", Color.white),
				new StCon((finalcd*100).ToString("0") +"%", GameData.Colour(GENUS.WIS))
			};
		}
	}


	public override void Start()
	{
		base.Start();
		finalcd = CooldownDecrease * WisdomFactor;
		Description_Basic = "Spell cooldowns lowered by " + (finalcd*100).ToString("0") + "%";

		CDRedUpgrade = new Ability_UpgradeInfo(0, 1, "+", "% CD Reduction", Color.green, () =>{upgrade_cdred += 0.01F;});

		Upgrades.Add(CDRedUpgrade);
	}

	public override void Update()
	{
		base.Update();
		finalcd = CooldownDecrease * WisdomFactor;
		Description_Basic = "Spell cooldowns lowered by " + (finalcd*100).ToString("0") + "%";
	}

	public override IEnumerator AfterTurn()
	{
		base.AfterTurn();
		foreach(Ability child in Player.Abilities)
		{
			if(child == null) continue;
			if(child == this) continue;
			child.addedCooldownPercent += -finalcd;
		}		
		yield return null;
	}

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);

		_input = null;
		if(_in.HasValue)
		{
			_input = con.Input[(int)_in];
		} 
		else
		{
			_input = GetContainerData(con);
		}

		CostGenus = (GENUS)GameData.StringToInt(_input.args[0]);
		CooldownDecrease = GameData.StringToInt(_input.args[5]);

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);

		CDRedUpgrade = new Ability_UpgradeInfo(0, 1, "+", "% CD Reduction", Color.green, () =>{upgrade_cdred += 0.01F;});
		Upgrades.Add(CDRedUpgrade);

		name = _input.args[3];
	}
}
