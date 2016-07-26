using UnityEngine;
using System.Collections;

public class Guard : Enemy {

	public int PatrolStage;
	public int PatrolLength;
	public int [] PatrolVelocity;

	private int Patrol_NextStage;

	protected sealed override void SetupEnemy()
	{

		float factor = GameManager.Difficulty;
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		Rank = 1;

		Name        = "Guard";

		factor *= Random.Range(0.8F, 1.1F);
		factor = factor * (InitStats.Value);
		
		InitStats.Hits        = (int)(hpfactor) * InitStats.Value;
		InitStats.Attack      = (int)(atkfactor) * InitStats.Value;
		CheckStats();
		SetSprite();
		PatrolVelocity = new int[]{Utility.RandomIntInclusive(1), Utility.RandomIntInclusive(1)};

		if(Stats.isNew)
		{
			int stonetime = PatrolLength-1;
			int delaytime = 2;
			AddEffect("Stoneform", -1, stonetime.ToString(), delaytime.ToString());
		}
	}

	public override IEnumerator AfterTurnRoutine()
	{
		//Patrol Routine
		if(Destroyed) yield break;
		if(HasEffect("Sleep")) yield break;
		while(!TileMaster.AllLanded) 
		{
			if(this == null) yield break;
			if(isMatching || Destroyed) yield break;
			yield return null;
		}
		int dist = 0;
		Tile nextpoint = Point.GetNeighbour(PatrolVelocity[0], PatrolVelocity[1]);
		TileMaster.instance.SwapTiles(nextpoint, this);

		yield return StartCoroutine(base.AfterTurnRoutine());
	}
}
