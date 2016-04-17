using UnityEngine;
using System.Collections;

public class Health : Tile {	

	//public override StCon [] Description
	//{
	//	get{
	//		return new StCon[]{new StCon("Heals " + Stats.Heal + " HP")};
	//	}
	//}

	public override void AfterTurn()
	{
		base.AfterTurn();
		//Stats.Heal += (int) (Stats.Value * GameManager.Difficulty)/2;
	}

	public override bool Match(int resource)
	{
		if(this == null) return false;
		InitStats.Hits -= 1;
		CheckStats();
		if(Stats.Hits <= 0)
		{
			isMatching = true;

			Stats.Value *=  resource;
			
			CollectThyself(true);
			TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
			return true;			
		}
		else 
		{
			isMatching = false;
			EffectManager.instance.PlayEffect(this.transform,Effect.Attack);
			CollectThyself(false);
			return true;
		}
		return false;
	}

	// Use this for initialization
	public override void Start () {
		base.Start();
		//Stats.Heal += (int) (Stats.Value * GameManager.Difficulty)/2;
	}
}
