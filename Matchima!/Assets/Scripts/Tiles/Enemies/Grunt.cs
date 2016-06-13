using UnityEngine;
using System.Collections;

public class Grunt : Enemy {


	private int GruntHPFactor = 9, GruntHPAdded = 1;
	private int GruntATKFactor = 18, GruntATKAdded = 1;

	private int CaptainHPFactor = 8, CaptainHPAdded = 1;
	private int CaptainATKFactor = 18, CaptainATKAdded = 1;

	private int ChiefHPFactor = 11, ChiefHPAdded = 2;
	private int ChiefATKFactor = 18, ChiefATKAdded = 1;

	private int TerrorHPFactor = 13, TerrorHPAdded = 5;
	private int TerrorATKFactor = 16, TerrorATKAdded = 1;

	public override StCon [] Description
	{
		get{
			string d = "";
			switch(Rank)
			{
				case 1:
				d = "A medium enemy with\n a weak attack. ";
				break;
				case 2:
				d = "A strong enemy with\n a weak attack";
				break;
				case 3:
				d = "A strong enemy that\n rallies nearby enemies. ";
				break;
				case 4:
				d = "A highly dangerous\n enemy. ";
				break;
			}
			return new StCon[]{new StCon((_EnemyType + " Enemy")),
							   new StCon(d)
							   };
		}
	}

	public override StCon _Name {
		get{
			string d = "";
			switch(Rank)
			{
				case 1:
				d = "Grunt";
				break;
				case 2:
				d = "Captain";
				break;
				case 3:
				d = "Chief";
				break;
				case 4:
				d = "Terror";
				break;
			}
			return new StCon(d, GameData.Colour(Genus));}
	}



	protected sealed override void SetupEnemy()
	{
		float factor = GameManager.Difficulty;
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		factor *= Random.Range(0.8F, 1.4F);
		factor = factor * (InitStats.Value);
		Rank = 1;
		if(InitStats.Value > 10) Rank = 4;
		else 
		{
			float captain_chance = 0.25F + Mathf.Clamp((factor/400), 0.0F, 0.25F);
			float chief_chance = 0.1F + Mathf.Clamp((factor/600), 0.0F, 0.15F);
			float terror_chance = 0.02F + Mathf.Clamp((factor/800), 0.0F, 0.08F);
			//float chance_to_promote = 0.01F + Mathf.Clamp((factor/200), 0.0F, 0.05F);

			if(Random.value < captain_chance)
			{
				if(Random.value < chief_chance)
				{
					//if(Random.value < terror_chance) Rank = 4;
					//else 
					Rank = 3;
				}
				//else if(Random.value < terror_chance)
				//{
				//	Rank = 4;
				//}
				else Rank = 2;
			}
			else if(Random.value < terror_chance) Rank = 4;
		}

		
		
		switch(Rank)
		{
			case 1:
			Name        = "Grunt";
			//InitStats.Value += (int)(factor/5);
			hpfactor    *= GruntHPAdded + factor / GruntHPFactor;
			atkfactor   *= GruntATKAdded + factor / GruntATKFactor;
			break;
			case 2:
			Name        = "Captain";
			InitStats.Value += 1 + (int)(factor/5);
			hpfactor    *= CaptainHPAdded + factor / CaptainHPFactor;
			atkfactor   *= CaptainATKAdded + factor / CaptainATKFactor;
			break;
			case 3:
			Name        = "Chief";
			InitStats.Value += 2 + (int)(factor/5);
			hpfactor    *= ChiefHPAdded + factor / ChiefHPFactor;
			atkfactor   *= ChiefATKAdded + factor / ChiefATKFactor;
			break;
			case 4:
			Name        = "Terror";
			InitStats.Value += 4 + (int)(factor/2);
			hpfactor    *= TerrorHPAdded + factor / TerrorHPFactor;
			atkfactor   *= TerrorATKAdded + factor / TerrorATKFactor;
			break;
		}
		
		InitStats.Hits        = (int)(hpfactor);
		InitStats.Attack      = (int)(atkfactor);

		Stats = new TileStat(InitStats);
		SetSprite();

		if(Stats.isNew)
		{
			AddEffect("Sleep", 1);
			//sleep_part = EffectManager.instance.PlayEffect(this.transform, Effect.Sleep);
		}
	}


}
