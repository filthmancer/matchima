using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Banish : Ability {

	public int BanishPower = 0;
	public bool LevelUpOnBanish = true;

	private int upgrade_power = 0;
	private float upgrade_ignorecooldown = 0.0F;

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			if(BanishPower == 0) All.Add(new StCon("Ends the current wave", Color.white));
			else All.Add(new StCon("Deals " + (BanishPower) + " damage to current wave", Color.white));
			return All.ToArray();
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = BanishPower == 0 ? "Ends the current wave." : "Deals " + (BanishPower + (int) StatBonus()) + " damage to the current wave." + (LevelUpOnBanish ? "" : " Will not level up.");
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		if(GameManager.instance._Wave == null) return;

		activated = true;

		if(BanishPower == 0) GameManager.instance._Wave[0].InstaKill();
		else 
		{
			int damage = BanishPower + (int)StatBonus() + upgrade_power;
			GameManager.instance._Wave[0].AddPoints(-damage);
		}

		activated = false;
		if(Random.value > upgrade_ignorecooldown)	cooldown_time = cooldown;
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = BanishPower == 0 ? "Ends the current wave." : "Deals " + (BanishPower + (int) StatBonus()) + " damage to the current wave." + (LevelUpOnBanish ? "" : " Will not level up.");
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
		BanishPower = GameData.StringToInt(_input.args[3]);
		LevelUpOnBanish = _input.args[4] == "T";
		//CostType = GameData.StringToGENUS(_input.args[0]);
	}

}//
