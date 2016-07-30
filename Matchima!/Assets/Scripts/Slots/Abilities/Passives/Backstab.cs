using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Backstab : Ability {

	public Vector2 [] stabAngle;
	public float extraDamage = 2;

	public string RandAlert
	{
		get
		{
			int rand = Random.Range(0,4);
			string [] alerts = new string [] {"STAB!", "POKE!", "GUT!", "SLASH!"};
			return alerts[rand];
		}
	
	}

	float finalsteal;
	float finalDamage
	{
		get
		{
			return extraDamage * DexterityFactor;
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			return new StCon [] {
				new StCon("Attacks deal", Color.white, true),
				new StCon((finalDamage*100).ToString("0") + "% extra damage", GameData.Colour(GENUS.DEX)),
			};
		}
	}
	
	public override void DamageIndicator(ref int [] damage, Tile [] selected)
	{
		for(int i = 0; i < selected.Length; i++)
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
						selected[i].SetOtherWarning("2X");
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
		
		List<Tile> to_collect = new List<Tile>();

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
						to_collect.Add(_tiles[i]);
						break;
					}	
				}
			}		
		}

		float stabtime = 0.25F;
		foreach(Tile child in to_collect)
		{
			child.SetOtherWarning("");
			child.InitStats.TurnDamage += (int) ((float)Player.AttackValue * finalDamage);
			UIManager.instance.MiniAlert(child.transform.position, RandAlert, 75, GameData.Colour(Parent.Genus));
			yield return new WaitForSeconds(stabtime);
		}
		yield break;
	}

	public override void SetArgs(params string [] args)
	{
		initialized = true;
		extraDamage = GameData.StringToFloat(args[0]);
	}
}