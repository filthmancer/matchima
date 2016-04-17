using UnityEngine;
using System.Collections;

public class Cocoon : Tile {

	private float boon_chance_init = 0.6F;
	private float boon_chance_current;
	private int target_lifetime = 1;
	private float boon_chance_inc_rate = 0.08F;
	private float boon_chance_decay_rate = -0.01F;
	private float pop_chance = 0.25F;

	public override StCon [] Description
	{
		get{return new StCon[] {new StCon("Contains a Boon or Curse tile.", GameData.Colour(Genus)),
								new StCon("Value increases while alive."),
								new StCon((boon_chance_current*100).ToString("0") + "% boon tile chance.", GameData.Colour(GENUS.STR))};}
	}

	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale, inf, value_inc);
		InitStats.Hits = 3 + (target_lifetime - InitStats.Lifetime)/10;
		boon_chance_current = boon_chance_init;

		CheckStats();
		pop_chance = 0.25F / (float)Stats.Value;
		//int turns_to_100 = target_lifetime/3;
		//boon_chance_decay_rate = -0.08F;
		//boon_chance_inc_rate = 0.4F/(float)turns_to_100;// + boon_chance_decay_rate * turns_to_100;
		//print(boon_chance_inc_rate);
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);

		int diff = (int) InitStats.value_soft - InitStats.Value;
		if(diff != 0)
		{
			InitStats.Value = (int) InitStats.value_soft;
			target_lifetime += diff;
			InitStats.Hits += diff;

			CheckStats();
			int turns_to_100 = Mathf.Clamp(target_lifetime/5, 2, 100);
			boon_chance_decay_rate = -0.14F;
			boon_chance_inc_rate = 0.4F/(float)turns_to_100;// + boon_chance_decay_rate * turns_to_100;
			//print(boon_chance_inc_rate + ":" + turns_to_100);
			//UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + Stats.Value, 75, Color.white, 0.8F,0.00F);
			//Animate("Alert");
			SetSprite();
			
			//AddBoonChance(0.05F * diff);
		}
	}

	public override bool Match(int resource)
	{
		if(isMatching) return true;
		//if(Stats.Hits == 1) AddBoonChance(0.15F);

		InitStats.Hits -= 1;
		//InitStats.Lifetime ++;
		CheckStats();
		if(Stats.Hits <= 0)
		{
			isMatching = true;
			InitStats.Value *= resource;
			CreateReward();
			return true;			
		}
		else 
		{
			isMatching = false;
			EffectManager.instance.PlayEffect(this.transform,Effect.Attack);
			
		}
		return false;
	}

	public override void AfterTurn()
	{
		//InitStats.Hits --;
		InitStats.Value ++;
		InitStats.value_soft ++;
		AddBoonChance(boon_chance_inc_rate);
		boon_chance_inc_rate += boon_chance_decay_rate;
		//print(boon_chance_inc_rate);

		//print(boon_chance_current);
		base.AfterTurn();
		
		CheckStats();
		pop_chance += 0.08F;
		if(Random.value < pop_chance)
		{
			isMatching = true;
			CreateReward();
		}
		//if(Stats.Lifetime >= target_lifetime)
		//{
		//	isMatching = true;
		//	CreateReward();
		//}
	}

	public void CreateReward()
	{
		CheckStats();
		bool boon = Random.value < boon_chance_current;
		string type = boon ? "boon" : "curse";

		TileMaster.instance.ReplaceTile(this, TileMaster.Types[type], Genus, 1, Stats.Value);
	}


	public void AddBoonChance(float f)
	{
		boon_chance_current = Mathf.Clamp(boon_chance_current + f, 0.0F, 1.0F);
	}

}
