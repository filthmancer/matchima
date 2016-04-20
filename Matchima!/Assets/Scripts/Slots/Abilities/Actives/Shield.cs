using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shield : Ability {

	public string ResShieldType;
	public float ResToDamageRatio = 1;

	float upgrade_ratio_decrease = 0;


	public override void Start()
	{
		base.Start();
		Description_Basic = "Damage this turn is removed from MP instead of HP.";
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Damage this turn is removed from MP instead of HP.";
	}

	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = !activated;
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 2") + "\n";
			s+= UpgradeInfoColoured(2, "Dec. resource per damage by 0.5") + "\n";
			s+= UpgradeInfoColoured(3, "Shield acts passively.") + "\n";
			return s;
		}
	}

	//bool UPGRADEOLD2()
	//{
	//	if(UpgradeLevel >= 4) return false;
	//	UpgradeLevel ++;
	//	switch(UpgradeLevel)
	//	{
	//		case 2:
	//		_defaultCooldown -= 3;
	//		if(_defaultCooldown < 2 && !passive) _defaultCooldown = 2;
	//		break;
	//		case 3:
	//		upgrade_ratio_decrease += 0.5F;
	//		break;
	//		case 4:
	//		_defaultCooldown = 1;
	//		passive = true;
	//		break;
	//	}
	//	return true;
	//}

	public override int OnHit(int Hit, params Tile[] attackers)
	{
		if(!activated) return Hit;
		int final_ratio = (int) (ResToDamageRatio - upgrade_ratio_decrease);
		if(final_ratio == 0) final_ratio = 1;
		
		int final = Hit;
		StatContainer stat = Parent.mainStat;//Player.Stats.GetResourceFromName(ResShieldType);
		int reshit = final * final_ratio;
		if(Parent.Meter < reshit) reshit = Parent.Meter;
		
		Parent.AddToMeter(-reshit);
		activated = false;
		cooldown_time = cooldown;
		return final - (reshit/final_ratio);
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		ResShieldType = (new_ab as Shield).ResShieldType;
		ResToDamageRatio = (new_ab as Shield).ResToDamageRatio;
		Start();
	}

	protected override void Setup()
	{
		base.Setup();
	}

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		string prefix = "P", suffix = "Shield";

		_input = null;
		if(_in.HasValue)
		{
			_input = con.Input[(int)_in];
		} 
		else
		{
			_input = GetContainerData(con);
		}

		ResShieldType = _input.args[0];
		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		prefix =  _input.args[3];
		ResToDamageRatio = GameData.StringToInt(_input.args[4]);

		name = prefix + " " + suffix;
	}
}
