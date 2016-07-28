﻿using UnityEngine;
using System.Collections;

public class Minion : Enemy {

	private int MinionHPAdded = 1;
	private int MinionATKAdded = 1;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
			new StCon((_EnemyType + " Enemy"), Color.white,true, 40),
			new StCon("A weak enemy with\n a strong attack", Color.white,true, 40)
			};
		}
	}

	protected sealed override void SetupEnemy()
	{

		float factor = GameManager.Difficulty;//(Mathf.Exp(GameManager.GrowthRate * Player.instance.Difficulty));
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		Rank = 1;

		Name        = "Minion";

		factor *= Random.Range(0.8F, 1.1F);
		factor = factor * (InitStats.Value);

		hpfactor    *= MinionHPAdded + factor / 6;
		atkfactor   *= MinionATKAdded + factor / 5;

		
		InitStats.Hits        = (int)(hpfactor) * InitStats.Value;
		InitStats.Attack      = (int)(atkfactor) * InitStats.Value;
		CheckStats();
		SetSprite();

		if(Stats.isNew)
		{

			AddEffect("Sleep", 1);
			//sleep_part = EffectManager.instance.PlayEffect(this.transform, Effect.Sleep);
		}
	}

	
	private float MinionSpawnChance = 0.35F;
		public override IEnumerator AfterTurnRoutine()
		{
			yield return StartCoroutine(base.AfterTurnRoutine());
			if(Random.value > MinionSpawnChance) yield break;

			while(!TileMaster.AllLanded) yield return null;
			if(isMatching || Genus == GENUS.OMG) yield break;
		//MINIONS summon other minions on isolated resource tiles
			for(int xx = 0; xx < TileMaster.Grid.Size[0]; xx++)
			{
				for(int yy = 0; yy < TileMaster.Grid.Size[1]; yy++)
				{
					if(TileMaster.Tiles[xx,yy].IsType("resource") && TileMaster.Tiles[xx,yy].Isolated)
					{
						TileMaster.instance.ReplaceTile(xx,yy, TileMaster.Types["minion"], GENUS.RAND);
						yield break;
					}
				}
			}
			yield break;
			
		}
}

