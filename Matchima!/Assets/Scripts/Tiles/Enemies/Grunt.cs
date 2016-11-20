﻿using UnityEngine;
using System.Collections;

public class Grunt : Enemy {


	private int GruntHPScale = 7, GruntHPMult = 1;
	private int GruntATKScale = 19, GruntATKMult = 1;

	private int CaptainHPScale = 5, CaptainHPMult = 1;
	private int CaptainATKScale = 19, CaptainATKMult = 1;

	private int ChiefHPScale = 3, ChiefHPMult = 1;
	private int ChiefATKScale = 19, ChiefATKMult = 1;

	private int TerrorHPScale = 14, TerrorHPMult = 3;
	private int TerrorATKScale = 19, TerrorATKMult = 1;

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
		RankCounter = 0;
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
					Rank = 3;
				}
				else Rank = 2;
			}
			else if(Random.value < terror_chance) Rank = 4;
		}

		switch(Rank)
		{
			case 1:
			Name        = "Grunt";
			InitStats.Value = 2;//(int)(factor/5);
			hpfactor    *= GruntHPMult + factor / GruntHPScale;
			atkfactor   *= GruntATKMult + factor / GruntATKScale;
			break;
			case 2:
			Name        = "Captain";
			InitStats.Value = 4;//1 + (int)(factor/5);
			hpfactor    *= CaptainHPMult + factor / CaptainHPScale;
			atkfactor   *= CaptainATKMult + factor / CaptainATKScale;
			break;
			case 3:
			Name        = "Chief";
			InitStats.Value = 6;//2 + (int)(factor/5);
			hpfactor    *= ChiefHPMult + factor / ChiefHPScale;
			atkfactor   *= ChiefATKMult + factor / ChiefATKScale;
			break;
			case 4:
			Name        = "Terror";
			//InitStats.Value += 4 + (int)(factor/2);
			hpfactor    *= TerrorHPMult + factor / TerrorHPScale;
			atkfactor   *= TerrorATKMult + factor / TerrorATKScale;
			break;
		}
		
		
		InitStats._Hits.Set((int)(hpfactor));
		//InitStats.Hits = InitStats._Hits.Max;
		InitStats._Attack.Set((int)(atkfactor));

		Stats = new TileStat(InitStats);
		SetSprite();

		if(Stats.isNew)
		{
			AddEffect("Sleep", 1);
			//sleep_part = EffectManager.instance.PlayEffect(this.transform, Effect.Sleep);
		}
	}

	int [] RankUpHealth = new int [] {5,10,20,5};
	int [] RankUpAttack = new int [] {5,5,5,5};

	public void UpgradeToRank()
	{
		switch(Rank)
		{
			case 1:
			Name        = "Grunt";
			InitStats.Value *= 1;
			InitStats.Hits    += RankUpHealth[0];
			InitStats.Attack   += RankUpAttack[0];
			break;
			case 2:
			Name        = "Captain";
			InitStats.Value *=2;
			InitStats.Hits    += RankUpHealth[1];
			InitStats.Attack   += RankUpAttack[1];
			break;
			case 3:
			Name        = "Chief";
			InitStats.Value *=3;
			InitStats.Hits    += RankUpHealth[2];
			InitStats.Attack   += RankUpAttack[2];
			break;
			case 4:
			Name        = "Terror";
			//InitStats.Value += RankUpHealth[0];
			InitStats.Hits   += RankUpHealth[3];
			InitStats.Attack  += RankUpAttack[3];
			break;
		}

		CheckStats();
		SetSprite();
	}

	public override void SetSprite()
	{

		SetBorder(Info.Outer);

		string rankrender = Info._GenusName;
		if(Rank > 1) rankrender += "_"+Rank;
		SetRender(rankrender);
		
		//if(Params._shiny != null && Params._render != null) Params._shiny.sprite = Inner;
		//transform.position = Point.targetPos;
		Params.transform.position = transform.position;
		Params._render.transform.localPosition = Vector3.zero;
	}

	public int RankCounter = 0;
	public int [] RankMarkers = new int []
	{
		2,
		4,
		8,
		15
	};

	public override IEnumerator AfterTurnRoutine()
	{
		if(Rank-1 < RankMarkers.Length) 
		{
			RankCounter ++;

			if(RankCounter > RankMarkers[Rank-1])
			{
				Rank++;
				UpgradeToRank();
				MiniAlertUI m = UIManager.instance.MiniAlert(Point.targetPos, "MUTATE!", 85, GameData.Colour(Genus), 1.2F, 0.1F);
			}
				

			
		}
		
		yield return StartCoroutine(base.AfterTurnRoutine());
	}
}