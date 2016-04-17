using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Change : Ability {
	public string TypeOverride;
	public string ChangedType;

	public int ChangedTileValue = 0;

	private int upgrade_changedtilevalue = 0;


	public override void Start()
	{
		base.Start();
		Description_Basic = (TypeOverride != "" ?  TypeOverride  + " tiles " : "Tiles ") + "in next match are changed to " + " " + ChangedType;
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = (TypeOverride != "" ?  TypeOverride  + " tiles " : "Tiles ") + "in next match are changed to " + " " + ChangedType;
	}

	public override IEnumerator BeforeMatch(List<Tile> tiles)
	{
		if(activated)
		{
			for(int i = 0; i < tiles.Count; i++)
			{
				if(TypeOverride != "")
				{
					if(tiles[i].IsType(TypeOverride))
					{
						PlayerControl.instance.RemoveTileToMatch(tiles[i]);
						TileMaster.instance.ReplaceTile(tiles[i].Point.Base[0], tiles[i].Point.Base[1], TileMaster.Types[ChangedType]);
						TileMaster.Tiles[tiles[i].Point.Base[0], tiles[i].Point.Base[1]].Stats.Value = ChangedTileValue;
						TileMaster.Tiles[tiles[i].Point.Base[0], tiles[i].Point.Base[1]].AddValue((int) StatBonus() + upgrade_changedtilevalue);
					}
				}
				else {
					PlayerControl.instance.RemoveTileToMatch(tiles[i]);
					TileMaster.instance.ReplaceTile(tiles[i].Point.Base[0], tiles[i].Point.Base[1], TileMaster.Types[ChangedType]);
					TileMaster.Tiles[tiles[i].Point.Base[0], tiles[i].Point.Base[1]].Stats.Value = ChangedTileValue;
					TileMaster.Tiles[tiles[i].Point.Base[0], tiles[i].Point.Base[1]].AddValue((int) StatBonus() + upgrade_changedtilevalue);
				}
			}

			yield return new WaitForSeconds(0.2F);
			
			activated = false;
			cooldown_time = cooldown;
			yield break;
		}
		yield break;
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
		_output = null;
		if(_out.HasValue)
		{
			_output = con.Output[(int)_out];
		} 
		else
		{
			_output = GetOutputData(con);
		}

		TypeOverride = _input.args[0];
		ChangedType = _output.args[0];
		ChangedTileValue = GameData.StringToInt(_output.args[4]);

	}

	public string OLDUPGRADE
	{
		get
		{

			string s = UpgradeInfoColoured(1, "Reduces cooldown by 3") + "\n";
				s+= UpgradeInfoColoured(2, "Increases tile value by 2") + "\n";
				s+= UpgradeInfoColoured(3, "Reduces cooldown by 4") + "\n";
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
	//		if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
	//		break;
	//		case 3:
	//		upgrade_changedtilevalue = 2;
	//		break;
	//		case 4:
	//		_defaultCooldown -= 4;
	//		if(_defaultCooldown < 1 && !passive) _defaultCooldown = 1;
	//		break;
	//	}
	//	return true;
	//}

}
