using UnityEngine;
using System.Collections;

public class Guard : Enemy {

	public int PatrolStage;
	private int PatrolLength = 3;
	public int [] PatrolVelocity;

	private int Patrol_NextStage;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				//new StCon(_EnemyType + " Enemy", Color.white, true,40),
				new StCon("Patrols left and right", GameData.Colour(GENUS.OMG),true, 40)
			};
		}
	}

	protected sealed override void SetupEnemy()
	{

		float factor = GameManager.Difficulty;
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		Name        = "Guard";

		factor *= Random.Range(0.8F, 1.1F);
		factor = factor * (InitStats.Value);
		
		InitStats.Hits        = (int)(hpfactor) * InitStats.Value;
		InitStats.Attack      = (int)(atkfactor) * InitStats.Value;
		CheckStats();
		SetSprite();
		int patrol_x = Random.value > 0.5F? -1:1;
		
		PatrolVelocity = new int[]{patrol_x,0};
		if(Stats.isNew)
		{
			int stonetime = PatrolLength-1;
			int delaytime = 2;
			AddEffect("Stoneform", -1, stonetime.ToString(), delaytime.ToString());
			OnAlert();

		}
	}

	public override bool CanAttack()
	{
		CheckStats();
		bool effects = true;
		foreach(TileEffect child in Effects)
		{
			if(!child.CanAttack()) effects = false;
		}
		return effects && !Stats.isNew && Stats.isAlerted && !AttackedThisTurn;
	}


	public override IEnumerator AfterTurnRoutine()
	{
		yield return StartCoroutine(base.AfterTurnRoutine());

		//Patrol Routine
		if(Destroyed) yield break;
		if(HasEffect("Sleep")) yield break;
		while(!TileMaster.AllLanded) 
		{
			if(this == null) yield break;
			if(isMatching || Destroyed) yield break;
			yield return null;
		}
		Tile nextpoint = Point.GetNeighbour(PatrolVelocity[0], PatrolVelocity[1]);
		if(PatrolStage >= PatrolLength || nextpoint == null || nextpoint == this || (nextpoint.Point.BaseX == 0 && nextpoint.Point.BaseY == 0))
		{
			PatrolVelocity[0] = -PatrolVelocity[0];
			PatrolVelocity[1] = -PatrolVelocity[1];
			PatrolStage = 0;
			nextpoint = Point.GetNeighbour(PatrolVelocity[0], PatrolVelocity[1]);
		}
		else PatrolStage++;

		
		TileMaster.instance.SwapTiles(nextpoint, this);		
	}
}
