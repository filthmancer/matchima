using UnityEngine;
using System.Collections;

public class ShapeShifter : Enemy {

	private float ShiftChance = 1.0F;
	public override void Start()
	{
		base.Start();
		SpecialEnemy = true;
	}

	public override IEnumerator AfterTurnRoutine()
	{
		base.AfterTurn();
		if(Random.value < ShiftChance)
		{
			string EndType = "";
			
			//_Species end = TileMaster.Types.GenusOf(EndType).TypeOf(Species);
		}
		yield break;
	}
	
}
