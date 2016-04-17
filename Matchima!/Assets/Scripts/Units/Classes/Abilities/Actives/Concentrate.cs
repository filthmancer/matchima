using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Concentrate : Ability {
	public TileTypes changeType;

	private float upgrade_chancetostack = 0.0F;
	private float upgrade_value_mult = 1.0F;

	public override StCon [] Description_Tooltip
	{
		get
		{
			return new StCon[]
			{
				new StCon("Target tile absorbs\nthe value of other tiles\nin the match.")
			};
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Matched tiles are absorbed into the first tile of the match, combining their value.";

	}


	public override void Activate()
	{
		if(cooldown_time > 0) return;
		activated = !activated;
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Matched tiles are absorbed into the first tile of the match, combining their value.";
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 2") + "\n";
				s+= UpgradeInfoColoured(2, "1.5x concentrated tile value") + "\n";
				s+= UpgradeInfoColoured(3,  "10% chance new tiles will be concentrated type") + "\n";
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
	//		_defaultCooldown -= 2;
	//		if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
	//		break;
	//		case 3:
	//		upgrade_value_mult = 1.5F;
	//		break;
	//		case 4:
	//		upgrade_chancetostack = 0.10F;
	//		break;
	//	}
	//	return true;
	//}

	public override IEnumerator BeforeMatch(List<Tile> tiles)
	{
		if(activated)
		{
			Tile first = tiles[0];
			int concentrate_value = 0;
			bool willstack = Random.value < upgrade_chancetostack;

			for(int i = 1; i < tiles.Count; i++)
			{
				if(tiles[i].IsType(first))
				{
					concentrate_value += tiles[i].Stats.Value + (int) (Player.Stats.GetGENUSStat(GENUS) / GENUSMultiplier);
					TileMaster.instance.CreateMiniTile(tiles[i].transform.position, tiles[0].transform, tiles[i].Params._render.sprite);
					if(willstack) TileMaster.instance.QueueTile(first.Type, first.Genus);
					//tiles[i].DestroyThyself(false);
				}
				else tiles[i].Reset();
			}

			yield return new WaitForSeconds(0.2F);
			tiles[0].AddValue(concentrate_value * upgrade_value_mult);
			yield return new WaitForSeconds(0.2F);
			tiles[0].Reset();
			
			activated = false;
			cooldown_time = cooldown;
			Player.instance.CompleteMatch = false;
			TileMaster.instance.SetFillGrid(true);

			yield break;
		}
		yield break;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Start();
	}

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
	}
}