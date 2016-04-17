using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heal : Ability {

	public int HealAmount = 20;
	private int upgrade_healamount = 0;

	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(HealUpgrade.Info, GameData.Colour(GENUS.WIS))};
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Restores " + HealAmount + " HP", Color.white));
			return All.ToArray();
		}
	}



	Ability_UpgradeInfo HealUpgrade;

	public override void Start()
	{
		base.Start();
		Description_Basic = "Restores " + (HealAmount + upgrade_healamount) + " HP";
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Restores " + (HealAmount + upgrade_healamount) + " HP";
		
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 3") + "\n";
			s+= UpgradeInfoColoured(2, "Inc. heal by 15") + "\n";
			s+= UpgradeInfoColoured(3, "Dec. cooldown by 4") + "\n";
			return s;
		}
	}


	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		
		StartCoroutine(ActiveRoutine());

		
	}

	protected IEnumerator ActiveRoutine()
	{
		activated = true;
		float part_time = 0.6F;

		GameObject part = Instantiate(Particle);
		part.transform.position = UIManager.instance.HealthImg.transform.position;

		yield return new WaitForSeconds(part_time);

		cooldown_time = cooldown;
		int heal = HealAmount + upgrade_healamount;
		Player.Stats.Heal(heal);
		Player.Stats.CompleteHealth();
		activated = false;
		yield return null;
	}

	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Heal heal = (Heal) new_ab;
		HealAmount = heal.HealAmount;
		Start();
	}

	protected override void Setup()
	{
		base.Setup();
	}

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		string suffix = "S";
	
		_input = null;
		_output = null;
		if(_in.HasValue)
		{
			_input = con.Input[(int)_in];
		} 
		else
		{
			_input = GetContainerData(con);
		}
		if(_out.HasValue) _output = con.Output[(int)_out];
		else _output = GetOutputData(con);

		int min = GameData.StringToInt(_input.args[4]);
		int max = GameData.StringToInt(_input.args[5]);
		HealAmount = Random.Range(min, max);
		
		suffix = _input.args[3];
		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);

		
		name = suffix;

		HealUpgrade = new Ability_UpgradeInfo(0, 10, "+", " healing", Color.green, () => {upgrade_healamount += 10;});
		Upgrades.Add(HealUpgrade);
	}
}
