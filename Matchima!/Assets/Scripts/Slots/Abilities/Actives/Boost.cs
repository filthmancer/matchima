using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boost : Ability {

	public string BoostGenus, BoostSpecies;
	public int valueMultiplier = 2;
	private float upgrade_valueMulti = 0;
	Ability_UpgradeInfo ValueInc;

	float final_mult;
	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(ValueInc.Info, GameData.Colour(GENUS.WIS))};
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			
			return new StCon [] {
				new StCon(final_mult.ToString("0.0") + "x", GameData.Colour(Parent.Genus)),
				new StCon("the value of ", Color.white),
				new StCon(BoostGenus + " " + BoostSpecies, Color.white)
			};
		}
	}

	public override void Start()
	{
		base.Start();
		final_mult = (valueMultiplier*1.0F) + upgrade_valueMulti + StatBonus();

		Description_Basic = final_mult.ToString("0.0") + "x the value of " + BoostGenus + BoostSpecies + " tiles.";
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;

		activated = !activated;
	}

	public override void Update()
	{
		base.Update();
		final_mult = (valueMultiplier*1.0F) + upgrade_valueMulti + StatBonus();
		Description_Basic = final_mult.ToString("0.0") + "x the value of " + BoostGenus + BoostSpecies + " tiles.";
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 2") + "\n";
				s+= UpgradeInfoColoured(2, "Inc. value multiplier by 0.5") + "\n";
				s+= UpgradeInfoColoured(3,  "Inc. value multiplier by 1") + "\n";
				return s;
		}
	}

	public override Bonus CheckBonus(GENUS g)
	{
		if(activated)
		{
			//PlayerControl.matchingTile.IsType(BoostSpecies) && 
			if(TileMaster.Types.GENUSOf(BoostGenus) == g)
			{
				Bonus bonus = new Bonus(final_mult, 
										"ABL",
										"Bonus from " + this.name + " ability",
										GameData.instance.GetGENUSColour(GENUS.PRP));
				activated = false;
				cooldown_time = cooldown;
				return bonus;
			}
			
			activated = false;
			cooldown_time = cooldown;
			return null;
		}
		return null;
	}

	public override void DamageIndicator(ref int[] damage, Tile [] selected)
	{
		damage = damage;
		if(activated) 
		{
			for(int i = 0; i < damage.Length; i++)
			{
				damage[i] *= (valueMultiplier + (int) StatBonus());
			}
		}
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Boost new_boost = (Boost) new_ab;
		BoostGenus = new_boost.BoostGenus;
		BoostSpecies = new_boost.BoostSpecies;
		valueMultiplier = new_boost.valueMultiplier;
		Start();
	}
	protected override void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
			BoostGenus = GameData.ResourceLong(Parent.Genus);
		}
		
	}

	//Input
	//Args 0: Title
	//Args 1: Chance
	//Args 2: GENUS
	//Args 3: Genus Type
	//Args 4: Species Type
	//Args 5: Multiplier
	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		string prefix = "Boost", suffix = "S";
		
		_input = null;
		if(_in.HasValue) _input = con.Input[(int)_in]; 
		else
		{
			_input = GetContainerData(con);
		}

		suffix = _input.args[0];
		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		BoostGenus = _input.args[3];
		BoostSpecies = _input.args[4];
		valueMultiplier = GameData.StringToInt(_input.args[5]);

		name = prefix + " " + suffix;

		ValueInc = new Ability_UpgradeInfo(0, 10, "+", "% Value", Color.green, () => {upgrade_valueMulti += 0.1F;});
		Upgrades.Add(ValueInc);
	}
}