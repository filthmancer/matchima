using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minion : Enemy {

	private int MinionHPAdded = 1;
	private int MinionATKAdded = 4;

	protected sealed override void SetupEnemy()
	{

		float factor = GameManager.Difficulty;//(Mathf.Exp(GameManager.GrowthRate * Player.instance.Difficulty));
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);
		Name        = "Minion";

		factor *= Random.Range(0.8F, 1.1F);
		factor = factor * (InitStats.Value);

		hpfactor    *= MinionHPAdded + factor / 6;
		atkfactor   *= MinionATKAdded + factor / 15;

		
		InitStats._Hits.Set((int)(hpfactor) * InitStats.Value);
		//InitStats.Hits = InitStats._Hits.Max;
		InitStats._Attack.Set((int)(atkfactor) * InitStats.Value);
		CheckStats();
		SetSprite();

		if(Stats.isNew)
		{

			AddEffect("Sleep", 1);
			//sleep_part = EffectManager.instance.PlayEffect(this.transform, Effect.Sleep);
		}
	}


	public override IEnumerator AttackRoutine()
	{
		Tile [] column = TileMaster.GetColumn(x);
		Tile [] row = TileMaster.GetRow(y);

		List<Tile> targ = new List<Tile>();
		if(column!= null) 
			for(int i = 0; i < column.Length; i++) {
				if(column[i] == null) continue;
				if(column[i].IsType("hero")) targ.Add(column[i]);}
		if(row != null)
			for(int i = 0; i < row.Length; i++) {
				if(row[i] == null) continue;
				if(row[i].IsType("hero")) targ.Add(row[i]);}

		if(targ.Count > 0)
		{
			Tile targ_final = targ[Random.Range(0, targ.Count)];
			SetState(TileState.Selected);
			OnAttack();
			yield return StartCoroutine(Animate("Attack", 0.05F));
			AttackTile(targ_final);
		}
		yield return null;
	}
}

