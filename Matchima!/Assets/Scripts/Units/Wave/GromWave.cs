using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GromWaveTile : WaveTileEndOnTileDestroy {

	public override IEnumerator OnStart()
	{
		if(!Active || Ended) yield break;
		targets = new List<Tile>();
	//Spawn at start

		bool [,] replacedtile = new bool [(int)TileMaster.instance.MapSize.x, (int)TileMaster.instance.MapSize.y];

		List<TileEffectInfo> Effects = Parent.GetEffects();
		for(int x = 0; x < (int)Style.Value; x++)
		{
			int randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
			int randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
			int checks = 0;
			while(replacedtile[randx, randy] || 
				!TileMaster.Tiles[randx,randy].IsType("resource") || 
				TileMaster.Tiles[randx,randy].Point.Scale > 1)
			{
				randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
				randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				if(checks == 10) yield break;
				checks++;
			}
			replacedtile[randx,randy] = true;

			targets.Add(TileMaster.instance.ReplaceTile(randx, randy, TileMaster.Types[Species], Genus, Scale, FinalValue));
			for(int i = 0; i < Effects.Count; i++)
			{
				TileMaster.Tiles[randx, randy].AddEffect(Effects[i]);
			}
			yield return new WaitForSeconds(Time.deltaTime * 5);
		}
		yield return new WaitForSeconds(Time.deltaTime * 5);
	}
	
	/*public override void EnemyKilled(Enemy e)
	{
		if(!Active || Ended || Current == -1) return;

		if(targets[0]!= null && e as Tile == targets[0])
		{

			return;
		}

		PointsThisTurn += PointsPerEnemy * e.Stats.Value;
		Current = Mathf.Clamp(Current - PointsPerEnemy * e.Stats.Value, 0, Required);
		if(!ShowingHealth)
		{
			StartCoroutine(ShowHealthRoutine());
		}

	}*/

}
