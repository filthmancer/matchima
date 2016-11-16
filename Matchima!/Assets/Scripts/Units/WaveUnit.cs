using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WaveUnit : Unit {

	public string Description;
	protected Wave Parent;
	public tk2dSpriteCollectionData InnerOverrideData, InnerAtlas;
	public string InnerOverride, InnerString, OuterString;

	public bool Active = false, Ended = false;


	public IntVector DiffScale = new IntVector(0, 2);

	public virtual void Setup(Wave p, int i)
	{
		Parent = p;
		Index = i;
		Active = false;
		//Timer = UnityEngine.Random.Range(PrepTime.x, PrepTime.y);
		//Inner.SetSprite(InnerOverride);
		//Outer.SetSprite(OuterOverride);
	}

	public virtual void Activate()
	{
		Active = true;
		Randomise();
		//OnStart();
	}

	public virtual void Randomise()
	{

	}

	public virtual void GetChances()
	{
		if(!Active || Ended) return;
	}

	public Tile [] GetTilesToReplace(int num, params string [] types)
	{
		Tile [] final = new Tile[num];
		bool [,] replacedtile = new bool [(int)TileMaster.Grid.Size[0], (int)TileMaster.Grid.Size[1]];

		int checks_max = (TileMaster.Grid.Size[0] * TileMaster.Grid.Size[1])-1;
		for(int i = 0; i < num; i++)
		{
			int checks = 0;
			Tile t = TileMaster.RandomTileOfType(types);

			int x = t.Point.Base[0];
			int y = t.Point.Base[1];
			while(replacedtile[x, y]||
					TileMaster.Tiles[x,y].Point.Scale > 1 ||
					y < 2)
			{
				t = TileMaster.RandomTileOfType(types);
				x = t.Point.Base[0];
				y = t.Point.Base[1];

				if(checks >= checks_max) break;
				checks ++;
			}
			replacedtile[x,y] = true;

			final[i] = TileMaster.Tiles[x,y];
		}
		return final;
	}



	public virtual IEnumerator OnStart()
	{
		if(!Active || Ended) yield break;
		yield break;
	}


	public virtual TileEffectInfo [] GetEffects()
	{
		if(!Active || Ended) return null;
		return null;
	}


	public virtual void OnEnd()
	{
		Active = false;
		Ended = true;
	}


	public virtual IEnumerator BeginTurn()
	{
		if(!Active || Ended) yield break;
		//TimeActive ++;
		//AddPoints(-PointsPerTurn);
	}

	public virtual IEnumerator AfterTurn()
	{
		if(!Active) yield break;
		Complete();
		yield return null;
	}

	private bool ShowingHealth;
	public virtual void Complete()
	{
		
	}


	public virtual int EnemyKilled(Enemy e)
	{
		if(!Active || Ended) return 0;
		//if(PointsPerEnemy <= 0) return 0;
		return 0;//PointsPerEnemy * e.Stats.Value;
		
	}

	public void Reset()
	{

	}

	public void SetImgtk(tk2dSprite inner, tk2dSprite outer)
	{
		if(InnerAtlas != null)
		{
			inner.SetSprite(InnerAtlas, InnerString);
		}
		else
		{
			string render = OuterString;
			tk2dSpriteDefinition id = TileMaster.Types[InnerString].Atlas.GetSpriteDefinition(render);
			if(id == null) render = "Alpha";
			inner.SetSprite(TileMaster.Types[InnerString].Atlas, render);
		}
		
		outer.SetSprite(TileMaster.Genus.Frames, OuterString);
	}

	public void CastAction(Tile targ, Action<Tile> a)
	{
		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.WaveButtons[Index].transform, Effect.Spell);
		MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
		mp.SetTarget(targ.transform.position);
		mp.SetPath(20.0F, 0.2F);
		mp.SetTileMethod(targ, a);
	}

	public virtual bool IsWaveTarget(Tile t)
	{
		return false;
	}
}
