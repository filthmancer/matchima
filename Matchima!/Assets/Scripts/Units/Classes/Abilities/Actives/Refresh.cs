using UnityEngine;
using System.Collections;

public class Refresh : Ability {

	private float upgrade_ignore_cost = 0.0F, upgrade_ignore_cd = 0.0F;

	public override StCon [] Description_Tooltip
	{
		get
		{
			return new StCon[]
			{
				new StCon("Refreshes all other abilities")
			};
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Refreshes all other abilities.";
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Refreshes all other abilities.";
	}

	public override void Activate()
	{
		if(!CanAfford()) return;
		activated = true;
		foreach(Ability child in Player.Abilities)
		{
			if(child == null) continue;
			if(child != this) child.cooldown_time = 0;
		}
		activated = false;
		//if(Random.value > upgrade_ignore_cost)	Player.Stats.GetResourceFromGENUS(CostType).ResCurrent -= cost[(int)CostType];
		if(Random.value > upgrade_ignore_cd) 	cooldown_time = cooldown;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Start();
	}

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		GENUS = (GENUS)2;
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 5") + "\n";
				s+= UpgradeInfoColoured(2, "5% chance to ignore cost") + "\n";
				s+= UpgradeInfoColoured(3,  "3% chance to ignore cooldown") + "\n";
				return s;
		}
	}

	bool UPGRADEOLD2()
	{
		if(UpgradeLevel >= 4) return false;
		UpgradeLevel ++;
		switch(UpgradeLevel)
		{
			case 2:
			_defaultCooldown -= 5;
			if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
			break;
			case 3:
			upgrade_ignore_cost = 0.05F;
			break;
			case 4:
			upgrade_ignore_cd = 0.03F;
			break;
		}
		return true;
	}
}
