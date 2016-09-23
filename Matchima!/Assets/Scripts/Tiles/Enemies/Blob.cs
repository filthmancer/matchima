using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blob : Enemy {

	private bool stacker = false;

	private int BlobHPAdded = 2;
	private int BlobATKAdded = 1;
	public Tile merger;
	private Tile [] nb = new Tile[4];

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				//new StCon(_EnemyType + " Enemy", Color.white, true,40),
				new StCon("Merges with other blobs", GameData.Colour(GENUS.PRP),true, 40)
			};
		}
	}

	protected sealed override void SetupEnemy()
	{
		float factor = GameManager.Difficulty;//(Mathf.Exp(GameManager.GrowthRate * Player.instance.Difficulty));
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		Name        = "Blob";

		factor *= Random.Range(0.8F, 1.1F);
		factor = factor * (InitStats.Value);

		hpfactor    *= BlobHPAdded + factor / 4.0F;
		atkfactor   *= BlobATKAdded + factor / 6.0F;

		InitStats.Hits        = (int)(hpfactor);
		InitStats.Attack      = (int)(atkfactor);

		CheckStats();
		SetSprite();

		if(Stats.isNew)
		{
			AddEffect("Sleep", 1);
		}
	}

	public override bool Match(int resource) {

		//if(isMatching || this == null) return false;

		CheckStats();
		int fullhit = 0;

		fullhit += Stats.TurnDamage;

		InitStats.TurnDamage = 0;
		InitStats.Hits -= fullhit;
		CheckStats();
		
		Player.instance.OnTileMatch(this);
		if(Stats.Hits <= 0)
		{
			if(Point.Scale == 1)
			{
				isMatching = true;
				Player.Stats.PrevTurnKills ++;			
				CollectThyself(true);
				PlayAudio("dying");

				if(GameData.ChestsFromEnemies)
				{
					float item_chance = (float)Stats.Value/32.0F;
					if(Stats.Value > 10) item_chance += 0.4F;
	
					if(Random.value < item_chance) 
					{
						int x = Random.Range(Point.BaseX, Point.BaseX + Point.Scale);
						int y = Random.Range(Point.BaseY, Point.BaseY + Point.Scale);
	
						GENUS g = Genus;
						float randg = Random.value;
						if(Random.value < 0.4F) g = (GENUS) Random.Range(0,4);
						if(Random.value < 0.95F) TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["chest"], g,  Point.Scale);
						else TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["mimic"], g, Point.Scale);
					}
					else TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
				}

				else TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
				return true;
			}
			else
			{
				int scale = Point.Scale;
				int basex = Point.BaseX;
				int basey = Point.BaseY;
				bool add_to_wavetargets = GameManager.Wave.IsWaveTarget(this);

				DestroyThyself();
				List<TileEffect> old_eff = new List<TileEffect>();
				old_eff.AddRange(Effects);

				List<Tile> new_blobs = new List<Tile>();

				for(int xx = 0; xx < scale; xx++)
				{
					new_blobs.Add(TileMaster.instance.ReplaceTile(basex+xx, basey, TileMaster.Types["blob"], Genus, 1, Stats.Value/2, true));

					foreach(TileEffect child in old_eff)
					{
						if(child == this) continue;
						if(child.DontInherit) continue;
						TileEffect neweff = (TileEffect) Instantiate(child);
						TileMaster.Tiles[basex+xx, basey].AddEffect(neweff);
					}
				}
				for(int yy = 1; yy < scale; yy++)
				{
					for(int xx = 0; xx < scale; xx++)
					{
						TileMaster.Grid[basex + xx, basey + yy]._Tile = null;
					}
					
				}
			
				if(add_to_wavetargets) GameManager.Wave.AddTargets(new_blobs);

			}
		}
		else 
		{
			isMatching = false;
		}
		return false;
	}

	public override IEnumerator AfterTurnRoutine()
	{
		yield return StartCoroutine(base.AfterTurnRoutine());
		while(!TileMaster.AllLanded) yield return null;
		if(isMatching || Genus == GENUS.OMG) yield break;


//NEW STYLE - Blobs merge if there are 4 in a square formation
		merger = null;
		bool merge = true;
		nb[0] = this;
		nb[1] = TileMaster.instance.GetTile(Point.Base[0]+Point.Scale, Point.Base[1]+Point.Scale);
		nb[2] = TileMaster.instance.GetTile(Point.Base[0], Point.Base[1]+Point.Scale);
		nb[3] = TileMaster.instance.GetTile(Point.Base[0]+Point.Scale, Point.Base[1]);

		for(int i = 0; i < nb.Length; i++)
		{
			if(nb[i] == null) 
			{
				merge = false;
				break;
			}
			if(nb[i].isMatching || nb[i].Genus == GENUS.OMG || nb[i].Genus == GENUS.NONE)
			{
			 merge = false;
			 break;
			}
			if(!nb[i].IsType("Blob") || nb[i].Point.Scale != Point.Scale) 
			{
				merge = false;
				break;
			}
		}

		if(merge)
		{
			isMatching = true;
			yield return StartCoroutine(MergeBlobs(nb));
		}
		else yield break;

//OLD STYLE - Blobs merge into higher value enemies
/*
		//Blobs will merge into a single tile if they have 2 neigbouring blobs
		merger = null;
		
		nb[0] = TileMaster.instance.GetTile(Point.Base[0], Point.Base[1]-Point.Scale);
		nb[1] = TileMaster.instance.GetTile(Point.Base[0], Point.Base[1]+Point.Scale);
		nb[2] = TileMaster.instance.GetTile(Point.Base[0]-Point.Scale, Point.Base[1]);
		nb[3] = TileMaster.instance.GetTile(Point.Base[0]+Point.Scale, Point.Base[1]);
		for(int i = 0; i < nb.Length; i++)
		{
			if(nb[i] == null) continue;
			
			if(nb[i].isMatching || nb[i].Genus == GENUS.OMG || nb[i].Genus == GENUS.NONE) continue;
			if(nb[i].IsType("Enemy") && nb[i].Stats.Value >= Stats.Value && nb[i].Point.Scale ==Point.Scale) 
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
*/
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

		Vector3 pos = transform.position + (GameData.RandomVector*1.4F);
		/*MoveToPoint mini = TileMaster.instance.CreateMiniTile(this.transform.position, m.transform, Inner);
		//mini.Target =  target;
		mini.SetPath(0.1F, 0.0F, 0.0F, 0.14F);
		mini.SetMethod(() =>{
				m.AddValue(Stats.Value);
				m.InitStats.Hits += Stats.Hits;
				m.InitStats.Attack += Stats.Attack;
				m.CheckStats();
			}
			);*/


		yield return new WaitForSeconds(0.5F);
	
		Reset();

		if(m.Stats.Value > m.Point.Scale * 15)
		{
			TileMaster.instance.ReplaceTile(m.Point.Base[0], m.Point.Base[1], m.Type, m.Genus, m.Point.Scale+1, m.Stats.Value/15);
		}

		//TileMaster.instance.ResetTiles();
		TileMaster.instance.SetFillGrid(true);
		DestroyThyself();

		yield break;
	}

	public IEnumerator MergeBlobs(Tile[] group)
	{
		int value = 0;
		int hits = 0;
		int attack = 0;

		GENUS gen = group[0].Genus;

		int x = group[0].Point.Base[0];
		int y = group[0].Point.Base[1];
		for(int i = 0; i < group.Length; i++)
		{

			group[i].SetState(TileState.Idle, true);
			value += group[i].Stats.Value;
			hits += group[i].Stats.Hits;
			attack += group[i].Stats.Attack;
			EffectManager.instance.PlayEffect(group[i].transform, Effect.Shiny, GameData.Colour(Genus));
		}

		yield return new WaitForSeconds(Time.deltaTime * 20);

		TileMaster.instance.ReplaceTile(x,y,
										TileMaster.Types["blob"], gen, 
										group[0].Point.Scale+1, 0);

		TileMaster.Tiles[x,y].RemoveEffect("Sleep");

		
		TileMaster.Tiles[x,y].InitStats.Value = (int)Mathf.Clamp((float)TileMaster.Tiles[x,y].InitStats.Value, 1, ((float)value));
		TileMaster.Tiles[x,y].InitStats.Hits += hits;
		//TileMaster.Tiles[x,y].InitStats.Attack += attack;

		TileMaster.Tiles[x,y].CheckStats();

		TileMaster.instance.ResetTiles();
		TileMaster.instance.SetFillGrid(true);
		yield break;
	}
}
