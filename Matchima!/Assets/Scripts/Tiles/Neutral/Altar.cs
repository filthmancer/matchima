using UnityEngine;
using System.Collections;

public class Altar : Tile {


	protected override TileUpgrade [] BaseUpgrades
	{
		get
		{
			return new TileUpgrade []
			{
				new TileUpgrade(1.0F, 1, () => {InitStats.Value += 1;})
			};
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Disappears in " + (Stats.Deathtime - Stats.Lifetime) + " turns")};
		}
	}

	public override  void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale,inf,value_inc);
		
		InitStats.Deathtime = Stats.Value;
		CheckStats();
	}


	public override IEnumerator AfterTurnRoutine()
	{
		yield return StartCoroutine(base.AfterTurnRoutine());
		if(Stats.Lifetime >= Stats.Deathtime) 
		{
			DestroyThyself(true);
			//StartCoroutine(GoAway());
		}
		yield break;
	}

	IEnumerator GoAway()
	{
		Animate("Attack");
		yield return new WaitForSeconds(0.2F);
		DestroyThyself(true);
		yield return null;
	}
}
