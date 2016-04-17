using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blob : Enemy {

	private bool stacker = false;

	private int BlobHPAdded = 1;
	private int BlobATKAdded = 1;
	public Tile merger;
	private Tile [] nb = new Tile[4];

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon((_EnemyType + " Enemy")),
				new StCon((Stats.Hits > 0 ? Stats.Hits : 0) + " Health", GameData.Colour(GENUS.STR), false),
				new StCon((Stats.Attack > 0 ? Stats.Attack : 0) + " Attack", GameData.Colour(GENUS.DEX)),
				new StCon("Merges with other\nenemies to increase\nattack and health", GameData.Colour(GENUS.WIS))
			};
		}
	}

	protected sealed override void SetupEnemy()
	{
		float factor = GameManager.Difficulty;//(Mathf.Exp(GameManager.GrowthRate * Player.instance.Difficulty));
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		Rank = 1;

		Name        = "Blob";
		InitStats.Value       += (int)(factor/10);
		
		hpfactor    *= BlobHPAdded + factor / 6;

		atkfactor   *= BlobATKAdded + factor / 10;

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

	public override IEnumerator AfterTurnRoutine()
	{
		while(!TileMaster.AllLanded) yield return null;
		if(isMatching) yield break;
		base.AfterTurn();

		//Blobs will merge into a single tile if they have 2 neigbouring blobs
		merger = null;
		
		nb[0] = TileMaster.instance.GetTile(Point.Base[0], Point.Base[1]-Point.Scale);
		nb[1] = TileMaster.instance.GetTile(Point.Base[0], Point.Base[1]+Point.Scale);
		nb[2] = TileMaster.instance.GetTile(Point.Base[0]-Point.Scale, Point.Base[1]);
		nb[3] = TileMaster.instance.GetTile(Point.Base[0]+Point.Scale, Point.Base[1]);
		for(int i = 0; i < nb.Length; i++)
		{
			if(nb[i] == null) continue;
			
			if(nb[i].isMatching) continue;
			if(nb[i].IsType("Enemy") && nb[i].Stats.Value >= Stats.Value) 
			{
				merger = nb[i];
				break;
			}
		}
		if(merger != null)
		{
			isMatching = true;
			yield return StartCoroutine(MergeBlobs(merger));
		}
		else yield break;
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);
		if((int) InitStats.value_soft != InitStats.Value)
		{
			//InitStats.Resource += (int) (InitStats.value_soft) - InitStats.Value;
			InitStats.Value = (int)InitStats.value_soft;


			UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + (Stats.Value), 75, Color.white,0.8F,0.0F);
			Animate("Alert");
		}
	}

	public IEnumerator MergeBlobs(Tile m)
	{

		SetState(TileState.Idle, true);

		TileMaster.instance.CreateMiniTile(this.transform.position, m.transform, Inner, 0.5F);


		yield return new WaitForSeconds(0.35F);
		m.AddValue(Stats.Value);
		m.InitStats.Hits += Stats.Hits;
		m.InitStats.Attack += Stats.Attack;
		m.CheckStats();
		
		yield return new WaitForSeconds(0.15F);
		Reset();

		if(m.Stats.Value >= m.Point.Scale * 10)
		{
			TileMaster.instance.ReplaceTile(m.Point.Base[0], m.Point.Base[1], m.Type, m.Genus, m.Point.Scale+1, m.Stats.Value);
		}
		//TileMaster.instance.ResetTiles();
		TileMaster.instance.SetFillGrid(true);
		DestroyThyself();

		yield break;
	}
}
