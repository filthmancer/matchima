using UnityEngine;
using System.Collections;

public class Teleport : Ability {

	public bool NoEnemies;

	private float upgrade_noenemy_chance = 0.0F;
	private float upgrade_ignore_cd = 0.0F;
	private bool upgrade_health = false;

	public override StCon [] Description_Tooltip
	{
		get
		{
			return new StCon[]
			{
				new StCon("Teleports to a new\ngrid of tiles" + (NoEnemies ? " with no enemies." : "."), Color.white, false)
			};
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Teleports to a new grid of tiles" + (NoEnemies ? " with no enemies.": ".");
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Teleports to a new grid of tiles" + (NoEnemies ? " with no enemies.": ".");;
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;
		if(Random.value > upgrade_ignore_cd) cooldown_time = cooldown;

		bool no_enemies = NoEnemies || Random.value < upgrade_noenemy_chance;
		if(upgrade_health) 
		for(int i = 0; i < 10; i++)
		{
			TileMaster.instance.QueueTile(TileMaster.Types["Health"], GENUS.NONE);
		}
		TileMaster.instance.NewGrid(null, GENUS.NONE, no_enemies);
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		NoEnemies = (new_ab as Teleport).NoEnemies;
		Start();
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
		NoEnemies = _input.args[3] == "T";
		Start();
		
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 3") + "\n";
			s+= UpgradeInfoColoured(2, "10% chance to ignore cooldown") + "\n";
			s+= UpgradeInfoColoured(3, (NoEnemies ? "Creates 10 health tiles" : "10% of no enemies")) + "\n";
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
			_defaultCooldown -= 3;
			if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
			break;
			case 3:
			upgrade_ignore_cd = 0.1F;
			break;
			case 4:
			if(NoEnemies) upgrade_health = true;
			else upgrade_noenemy_chance = 0.1F;
			break;
		}
		return true;
	}
}