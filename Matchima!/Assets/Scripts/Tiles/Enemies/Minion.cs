using UnityEngine;
using System.Collections;

public class Minion : Enemy {

	private int MinionHPAdded = 1;
	private int MinionATKAdded = 1;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
			new StCon((Stats.Hits > 0 ? Stats.Hits : 0) + " Health", GameData.Colour(GENUS.STR), false),
			new StCon((Stats.Attack > 0 ? Stats.Attack : 0) + " Attack", GameData.Colour(GENUS.DEX)),
			new StCon("A weak enemy with\n a strong attack")
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
		InitStats.Value       = 1 + (int)(factor/10);
		hpfactor    *= MinionHPAdded + factor / 3;
		atkfactor   *= MinionATKAdded + factor / 5;

		
		InitStats.Hits        = (int)(hpfactor) * InitStats.Value;
		InitStats.Attack      = (int)(atkfactor) * InitStats.Value;
		CheckStats();
		SetSprite();

		if(Stats.isNew)
		{
			TileEffect sleep = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName("Sleep"));
			sleep.Duration = 1;
			AddEffect(sleep);
			//sleep_part = EffectManager.instance.PlayEffect(this.transform, Effect.Sleep);
		}
	}
}

