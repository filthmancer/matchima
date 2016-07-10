using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveUnit : Unit {

	public string Description;
	protected Wave Parent;
	[HideInInspector]
	public tk2dSprite Inner, Outer;
	public tk2dSprite InnerOverride, OuterOverride;
	public IntVector PrepTime = new IntVector(0,0);
	public int Timer = 0;
	public int TimeActive = 0;

	public bool Active = false, Ended = false;

	public int PointsPerTurn = 3, PointsPerEnemy = 1;
	private int PointsThisTurn = 0;

	public IntVector DiffScale = new IntVector(0, 2);

	public virtual void Setup(Wave p, int i)
	{
		Parent = p;
		Index = i;
		Active = false;
		Timer = Random.Range(PrepTime.x, PrepTime.y);
		Inner = InnerOverride;
		Outer = OuterOverride;
	}

	public void Activate()
	{
		Active = true;
		Randomise();
		//OnStart();
	}

	public void Randomise()
	{

	}

	public virtual void GetChances()
	{
		if(!Active || Ended) return;
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
		TimeActive ++;
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
		if(PointsPerEnemy <= 0) return 0;
		return PointsPerEnemy * e.Stats.Value;
		
	}

	IEnumerator ShowHealthRoutine()
	{
		ShowingHealth = true;
		int current_heal = PointsThisTurn;

		string prefix = current_heal < 0 ? "  +" : "  -";
		Vector3 tpos = Vector3.right * 0.4F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.Objects.TopGear[1][Index].Txt[0].transform.position + tpos, 
			prefix + current_heal, 42, GameData.instance.BadColour, 1.7F,	0.01F);

		while(heal.lifetime > 0.0F)
		{
			if(PointsThisTurn == 0)
			{
				heal.lifetime = 0.0F;
				heal.text = "";
				break;
			}
			else if(PointsThisTurn != current_heal)
			{
				heal.lifetime += 0.2F;
				heal.size = 42 + current_heal * 0.75F;
				current_heal = PointsThisTurn;
				heal.text = prefix + current_heal;
			}
			

			yield return null;
		}

		ShowingHealth = false;

		yield return null;
	}
	public void Reset()
	{

	}
}
