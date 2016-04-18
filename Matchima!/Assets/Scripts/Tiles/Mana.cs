using UnityEngine;
using System.Collections;

public class Mana : Tile {

	public override bool Match(int resource)
	{
		if(this == null) return false;
		InitStats.Hits -= 1;
		CheckStats();
		AudioManager.instance.PlayClipOn(this.transform, "Player", "Match");
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
			return true;
		}
		return false;
	}
}
