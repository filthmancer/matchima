using UnityEngine;
using System.Collections;

public class Armour : Tile {
	//private int _defaultArmour = 5;
	//public override StCon [] Description
	//{
	//	get{
	//		return new StCon[]{new StCon("Adds " + Stats.Armour + " points of armour")};
	//	}
	//}
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

}
