using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveTileEndOnTileDestroy : WaveTile {
	public List<Tile> targets;
	public bool DestroyOnWaveEnd = true;
	public int PointsPerTarget = 1;
	public override IEnumerator OnStart()
	{
		if(!Active || Ended) yield break;
		targets = new List<Tile>();
		GameManager.instance.paused = true;
	//Spawn at start
		if(SpawnType != WaveTileSpawn.XAtStart) yield break;

		bool [,] replacedtile = new bool [(int)TileMaster.instance.MapSize.x, (int)TileMaster.instance.MapSize.y];

		List<TileEffectInfo> Effects = Parent.GetEffects();
		for(int x = 0; x < (int)Factor; x++)
		{
			int randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
			int randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
			int checks = 0;
			while(replacedtile[randx, randy] || 
				!TileMaster.Tiles[randx,randy].IsType("resource") || 
				TileMaster.Tiles[randx,randy].Point.Scale > 1 ||
				randy < 2)
			{
				randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
				randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				if(checks == 10) yield break;
				checks++;
			}
			replacedtile[randx,randy] = true;

			GameObject initpart = EffectManager.instance.PlayEffect(UIManager.WaveButtons[Index].transform, Effect.Spell);
			MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
			mp.SetTarget(TileMaster.Tiles[randx,randy].transform.position);
			mp.SetPath(30, 0.2F);
			//mp.Target_Tile = TileMaster.Tiles[randx,randy];
			mp.SetTileMethod(TileMaster.Tiles[randx,randy], (Tile t) => 
				{
					Tile newtile = TileMaster.instance.ReplaceTile(t, TileMaster.Types[SpeciesFinal], Genus, Scale, FinalValue);
					targets.Add(newtile);
					for(int i = 0; i < Effects.Count; i++)
					{
						TileEffect effect = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName(Effects[i].Name));
						effect.GetArgs(Effects[i].Duration, Effects[i].Args);
						newtile.AddEffect(Effects[i]);
					}
				});
			
		
			
			yield return new WaitForSeconds(Time.deltaTime * 10);
		}
		yield return new WaitForSeconds(Time.deltaTime * 2);
	}

	public override IEnumerator AfterTurn()
	{
		yield return StartCoroutine(base.AfterTurn());
		if(!Active) yield break;
		bool end = true;
		for(int i = 0; i < targets.Count; i++)
		{
			if(!targets[i].Destroyed)// && !targets[i].isMatching)
			{
				end = false;
			}	
		}
		if(end && !Ended) 
		{
			Ended = true;
			Parent.AddPoints(PointsPerTarget,true);
		}
		Complete();
		yield return null;
	}

	
	public override void OnEnd()
	{
		Active = false;
		Ended = true;
		if(DestroyOnWaveEnd)
		{
			foreach(Tile child in targets)
			{
				if(!child.Destroyed) child.DestroyThyself(false);
			}
		}
	}

	public void AddTargets(List<Tile> new_targs)
	{
		foreach(Tile child in new_targs)
		{
			bool add = true;
			foreach(Tile old in targets)
			{
				if(child == old) add = false;
			}
			if(add) targets.Add(child);
		}
	}

	public override bool IsWaveTarget(Tile t)
	{
		foreach(Tile child in targets)
		{
			if(child == t) return true;
		}
		return false;
	}
}
