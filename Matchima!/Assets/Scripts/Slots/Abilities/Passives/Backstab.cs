using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Backstab : Ability {

	public Vector2 [] stabAngle;
	public float extraDamage = 2;

	float finalsteal;
	float finalDamage;

	public override StCon [] Description_Tooltip
	{
		get{
			return new StCon [] {
				new StCon("Attacks from above deals", Color.white, true),
				new StCon((finalDamage*100).ToString("0") + "% extra damage", GameData.Colour(GENUS.DEX)),
			};
		}
	}

	public override void Update()
	{
		base.Update();
		finalDamage = extraDamage * DexterityFactor;
		//Description_Basic = "Spell cooldowns lowered by " + (finalcd*100).ToString("0") + "%, but " + Parent.Name + " steals " + finalsteal + "% of other heros mana gains";
	}

	
	public override void DamageIndicator(ref List<int> damage, List<Tile> selected)
	{
		for(int i = 0; i < selected.Count; i++)
		{
			if(i == 0) continue;
			if(selected[i].Type.isEnemy)
			{
				Vector2 num_a = new Vector2(selected[i].Point.Base[0],selected[i].Point.Base[1]);
				Vector2 num_b = new Vector2(selected[i-1].Point.Base[0],selected[i-1].Point.Base[1]);

				Vector2 diff = num_a - num_b;
				
				for(int s = 0; s < stabAngle.Length; s++)
				{
					if(diff == stabAngle[s]) 
					{
						selected[i].SetOtherWarning("STAB");
						damage[i] = damage[i] + (int) ((float)damage[i] * finalDamage);
						break;
					}	
				}
				
			}
		}

		//return damage;
	}


	public override IEnumerator BeforeMatch(List<Tile> _tiles)
	{
		bool has_stabbed = false;
		
		for(int i = 0; i < _tiles.Count; i++)
		{
			if(i == 0 || _tiles[i] == null) continue;
			if(_tiles[i].Type.isEnemy)
			{
				Vector2 num_a = new Vector2(_tiles[i].Point.Base[0],_tiles[i].Point.Base[1]);
				Vector2 num_b = new Vector2(_tiles[i-1].Point.Base[0],_tiles[i-1].Point.Base[1]);

				Vector2 diff = num_a - num_b;
				
				for(int s = 0; s < stabAngle.Length; s++)
				{
					if(diff == stabAngle[s]) 
					{
						_tiles[i].InitStats.TurnDamage += (int) ((float)PlayerControl.instance.AttackValue * finalDamage);
						_tiles[i].SetOtherWarning("STAB");
						has_stabbed = true;
						break;
					}	
				}
				
			}
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
		extraDamage = GameData.StringToInt(_input.args[3]);
		List<Vector2> angles = new List<Vector2>();
		for(int i = 4; i < _input.args.Length; i+=2)
		{
			int x = GameData.StringToInt(_input.args[i]);
			int y = GameData.StringToInt(_input.args[i+1]);
			angles.Add(new Vector2(x,y)); 
		}
		stabAngle = angles.ToArray();
	}
}