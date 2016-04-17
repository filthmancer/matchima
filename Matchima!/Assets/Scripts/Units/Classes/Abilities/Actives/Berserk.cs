using UnityEngine;
using System.Collections;

public class Berserk : Ability {

	public string changedTile;
	public int healthPerTile = 3;

	public string Genus, Species;

	private int upgrade_healthPerTile = 0;

	private string c_Genus;
	private string c_Type;

	Ability_UpgradeInfo HealthPerTile;

	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(HealthPerTile.Info, GameData.Colour(GENUS.WIS))};
		}
	}


	public override StCon [] Description_Tooltip
	{
		get{
			
			return new StCon [] {
				new StCon("Heal " + healthPerTile + " HP",GameData.Colour(GENUS.STR), false),
				new StCon("for matching", Color.white),
				new StCon(changedTile + " tiles", GameData.Colour(GENUS.PRP))
			};
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Gain " + healthPerTile + " health per tile for matching " + changedTile + " tiles";
		
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

			string s = UpgradeInfoColoured(1, "Dec. cooldown by 4") + "\n";
				s+= UpgradeInfoColoured(2, "Inc. health per tile by 1") + "\n";
				s+= UpgradeInfoColoured(3, "Inc. health per tile by 2") + "\n";
				return s;
		}
	}

	public override void Update()
	{
		base.Update();
		int heal = healthPerTile + (int)StatBonus() + upgrade_healthPerTile;
		Description_Basic = "Gain " + healthPerTile + " health per tile for matching " + changedTile;
	}

	public override IEnumerator AfterMatch()
	{
		if(activated)
		{
			if(PlayerControl.matchingTile.IsType(Genus, Species))
			{
				int heal = 0;
				foreach(Tile child in PlayerControl.instance.selectedTiles)
				{
					if(child.IsType(Genus, Species)) heal += healthPerTile + (int) StatBonus() + upgrade_healthPerTile;
				}
				Parent.Stats.Heal(heal);
				//Player.Stats.Heal(heal);
			}
			cooldown_time = cooldown;
			activated = false;
		}
		yield break;
	}

	protected override void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
			Genus = GameData.ResourceLong(Parent.Genus);
		}

		changedTile = Genus + " " + Species;
	}

	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Berserk berserk = (Berserk) new_ab;
		changedTile = berserk.changedTile;
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

		//changedTile = _input.args[0];
		Genus = _input.args[3];
		Species = _input.args[4];

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
		HealthPerTile = new Ability_UpgradeInfo(0, 2, "+", " HP Per Tile", Color.green, () =>{upgrade_healthPerTile += 2;});
		Upgrades.Add(HealthPerTile);
	}
}