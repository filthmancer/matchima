using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Backstab : Ability {

	public float StealAmount;
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
				new StCon("Steals ", Color.white),
				new StCon((finalsteal*100).ToString("0") + "% of Mana Gains", GameData.Colour(GENUS.WIS))
			};
		}
	}

	public override void Update()
	{
		base.Update();
		finalsteal = StealAmount * WisdomFactor;
		finalDamage = extraDamage * DexterityFactor;
		//Description_Basic = "Spell cooldowns lowered by " + (finalcd*100).ToString("0") + "%, but " + Parent.Name + " steals " + finalsteal + "% of other heros mana gains";
	}

	


	protected override void Setup()
	{
		base.Setup();

		QuoteGroup steal = new QuoteGroup("Stealing Mana");
		steal.AddQuote("You don't mind if I borrow this?", Parent, false, 2.5F);
		steal.AddQuote("I need it more than you...", Parent, false, 2.5F);
		steal.AddQuote("Thanks for doing the hard work!", Parent, false, 2.5F);
		Parent.Quotes.Special.Add(steal);
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


	public override void AfterTurnB()
	{
		int total_stolen = 0;
		foreach(Class child in Player.Classes)
		{
			if(child == Parent) continue;
			if(child.ManaThisTurn > 0)
			{
				int stolen = (int) ((float) child.ManaThisTurn * finalsteal);
				if(child.Meter < stolen) stolen = child.Meter;
				//print("STOLE " + stolen + " FROM " + child.Name + " : " + child.ManaThisTurn);				
				child.AddToMeter(-stolen);
				total_stolen += stolen;	
				for(int i = 0; i < stolen; i++)
				{
					TileMaster.instance.CreateMiniTile(UIManager.ClassButtons[child.Index].transform.position,
													UIManager.ClassButtons[Parent.Index].transform, 
													TileMaster.Genus.Frame[Parent.Index]);
				}
			}
		}

		if(total_stolen > 0)
		{
			Parent.AddToMeter(total_stolen);
			UIManager.instance.MiniAlert(UIManager.ClassButtons[Parent.Index].transform.position + Vector3.up*0.6F, "Stole " + total_stolen + " Mana!", 34, GameData.Colour(Parent.Genus), 0.8F, 0.1F, true);
			if(Random.value < 0.15F) StartCoroutine(UIManager.instance.Quote(Parent.Quotes.GetSpecial("Stealing Mana").RandomQuote));
		}
		
		
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