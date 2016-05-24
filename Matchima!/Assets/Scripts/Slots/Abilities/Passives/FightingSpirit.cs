using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightingSpirit : Ability {

	public float AttackPerEnemy;
	public int AttackFinal;
	public float HealthPerEnemy;
	public int HealthFinal;

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> final = new List<StCon>();
			if(AttackPerEnemy > 0) 
			{
				final.Add(new StCon("+" + AttackFinal + " Attack", GameData.Colour(GENUS.DEX)));
				final.Add(new StCon("from enemy tiles"));
			}
			if(HealthPerEnemy > 0) 
			{
				final.Add(new StCon("+" + HealthFinal + " HP Max", GameData.Colour(GENUS.STR)));
				final.Add(new StCon("from enemy tiles"));
			}
			return final.ToArray();
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = (AttackPerEnemy > 0 ? "+1 Attack per " + (1/AttackPerEnemy) + " enemy tiles on screen": "" ) +
							(HealthPerEnemy > 0 ? "+1 Health per " + (1/HealthPerEnemy) + " enemy tiles on screen": "" );
	}
	public override void Update()
	{
		base.Update();
	}
	public override IEnumerator AfterTurn()
	{
		base.AfterTurn();

		AttackFinal = (int) (TileMaster.instance.EnemiesOnScreen * AttackPerEnemy * DexterityFactor);
		HealthFinal = (int) (TileMaster.instance.EnemiesOnScreen * HealthPerEnemy * StrengthFactor);
		yield return null;
	//ADD VISUAL CUE FOR ATTACK BONUS
	}


	public override Stat GetStats()
	{
		Stat s = new Stat();
		s._Attack = AttackFinal;
		s._HealthMax = HealthFinal;
		return s;
	}

	public override void SetArgs(params string [] args)
	{
		initialized = true;
		HealthPerEnemy = GameData.StringToInt(args[0]);
		AttackPerEnemy = GameData.StringToInt(args[1]);
	}

}
