using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : WaveUnit {

	public string Mission
	{
		get{
			return _Mission.Command;
		}
	}

	public float MissionRatio
	{
		get{
			return (float)_Mission.Current / (float)_Mission.TargetNum;
		}
	}
	[SerializeField]
	private MissionContainer _Mission;

	public void Setup()
	{
		Active = true;
		mission_completed = false;
		_entered = false;
	}

	public void CheckMission(Tile t)
	{
		mission_completed = _Mission.Check(t);
	}

	[SerializeField]
	public SpawnTileInfo [] DormantTiles;
	[SerializeField]
	public SpawnTileInfo [] ArrivalTiles;

	public SpawnTileInfo [] EndTiles;

	
	public bool Arrived
	{
		get{return mission_completed;}
	}
	public bool Entered
	{
		get{return _entered;}
	}
	private bool _entered;
	private bool mission_completed;
	public void OnArrive()
	{
		_entered = true;
		SpawnTileInfo [] list = ArrivalTiles;
		for(int i = 0; i < list.Length; i++)
		{
			if(list[i].SpawnStyle != WaveTileSpawn.OnArrive) continue;//yield break;
			//GameManager.instance.paused = true;

			int final_num = (int) list[i].SpawnValue;
			Tile [] replaces = GetTilesToReplace(final_num, "resource", "enemy", "health");

			for(int x = 0; x < final_num; x++)
			{
				CreateTileFromSpawnInfo(list[i], replaces[x]);
				//if(final_num > 1) yield return new WaitForSeconds(Time.deltaTime * 5);
			}
		}
	}

	public override IEnumerator OnStart()
	{
		if(!Active || Ended) yield break;
		
		SpawnTileInfo [] list = Arrived ? ArrivalTiles : DormantTiles;
		//GameManager.instance.paused = true;

	//Spawn at start
		for(int i = 0; i < list.Length; i++)
		{
			if(list[i].SpawnStyle != WaveTileSpawn.XAtStart) continue;
			
			int final_num = (int) list[i].SpawnValue;
			Tile [] replaces = GetTilesToReplace(final_num, "resource", "enemy", "health");
			List<TileEffectInfo> Effects = new List<TileEffectInfo>();//Parent.GetEffects();

			for(int x = 0; x < final_num; x++)
			{
				CreateTileFromSpawnInfo(list[i], replaces[x]);
				
				if(final_num > 1) yield return new WaitForSeconds(Time.deltaTime * 10);
			}
			yield return new WaitForSeconds(Time.deltaTime * 15);
		}
		
	}



	public override void OnEnd()
	{
		Active = false;
		Ended = true;

		SpawnTileInfo [] list = EndTiles;
		for(int i = 0; i < list.Length; i++)
		{
			if(list[i].SpawnStyle != WaveTileSpawn.OnEnd) continue;//yield break;
			//GameManager.instance.paused = true;

			int final_num = (int) list[i].SpawnValue;
			Tile [] replaces = GetTilesToReplace(final_num, "resource", "enemy", "health");

			for(int x = 0; x < final_num; x++)
			{
				CreateTileFromSpawnInfo(list[i], replaces[x]);
				//if(final_num > 1) yield return new WaitForSeconds(Time.deltaTime * 5);
			}
		}
	}

	public override void GetChances()
	{
		if(!Active || Ended) return;
		SpawnTileInfo [] list = Arrived ? ArrivalTiles : DormantTiles;

		for(int i = 0; i < list.Length; i++)
		{
			if(list[i].SpawnStyle == WaveTileSpawn.XChance)
			{
				TileMaster.instance.IncreaseChance(list[i].GenusString, list[i]._Type, list[i].SpawnValue);
				List<TileEffectInfo> effects = new List<TileEffectInfo>();//Parent.GetEffects();
				SPECIES s = TileMaster.Types[list[i]._Type];
				GenusInfo g = s[list[i].GenusString];
				if(list[i]._Value.y > 0)
				{
					g.ValueAdded.Add(list[i]._Value);
				}

				for(int e = 0; e < effects.Count; e++)
				{
					if(!effects[e].ApplyToTileType) continue;
					if(effects[e].ApplyToSpecies)
					{
						s.Effects.Add(effects[e]);
					}
					else
					{
						g.Effects.Add(effects[e]);
					}
				}
			}
			
		}
	}


	public IEnumerator BeforeTurn()
	{
		//yield return StartCoroutine(base.BeforeTurn());
		if(!Active || Ended) yield break;
		//if(HitByPresence) AddPoints(-Player.Stats.Presence);
	//Spawn per round

		SpawnTileInfo [] list = Arrived ? ArrivalTiles : DormantTiles;
		for(int i = 0; i < list.Length; i++)
		{
			//if(list[i].SpawnStyle != WaveTileSpawn.XPerTurn) 
			continue;//yield break;
			//GameManager.instance.paused = true;

			int final_num = (int) list[i].SpawnValue;
			Tile [] replaces = GetTilesToReplace(final_num, "resource", "enemy", "health");

			for(int x = 0; x < final_num; x++)
			{
				CreateTileFromSpawnInfo(list[i], replaces[x]);
				if(final_num > 1) yield return new WaitForSeconds(Time.deltaTime * 5);
			}
		}
		//yield return new WaitForSeconds(Time.deltaTime * 5);
		//GameManager.instance.paused = false;
	}

	public override IEnumerator AfterTurn()
	{
		if(!Active) yield break;
		Complete();

		SpawnTileInfo [] list = Arrived ? ArrivalTiles : DormantTiles;
		//print(Arrived + ":" + list.Length);
		for(int i = 0; i < list.Length; i++)
		{
			if(list[i].SpawnStyle == WaveTileSpawn.XPerTurn)
			{
				int final_num = (int) list[i].SpawnValue;
				Tile [] replaces = GetTilesToReplace(final_num, "resource", "enemy", "health");
				//print(final_num);
				for(int x = 0; x < final_num; x++)
				{
					List<TileEffectInfo> Effects = new List<TileEffectInfo>();//Parent.GetEffects();
					CreateTileFromSpawnInfo(list[i], replaces[x]);
					if(final_num > 1) yield return new WaitForSeconds(Time.deltaTime * 5);
				}
			}
			else if(list[i].SpawnStyle == WaveTileSpawn.XOnScreen)
			{
				int final_num = (int) list[i].SpawnValue;
				int onscreen = 0;
				for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
				{
					for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
					{
						if(TileMaster.Tiles[x,y].IsType(list[i].GenusString, list[i]._Type)) onscreen++;
					}
				}
				while(onscreen < final_num)
				{
					TileMaster.instance.QueueTile(TileMaster.Types[list[i]._Type], TileMaster.Genus[list[i].GenusString]);
					onscreen++;
				}
			}
			else if(list[i].SpawnStyle == WaveTileSpawn.XPsuedoChance)
			{
				int finalnum = (int) list[i].SpawnValue;
				for(int e = 0; e < finalnum; e++)
				{
					TileMaster.instance.QueueTile(TileMaster.Types[list[i]._Type], TileMaster.Genus[list[i].GenusString]);
				}
			}
		}
		yield return null;
	}

	void CreateTileFromSpawnInfo(SpawnTileInfo s, Tile targ)
	{
		List<TileEffectInfo> Effects = new List<TileEffectInfo>();//Parent.GetEffects();
		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.instance.ZoneObj.transform, Effect.Spell);
		MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
		mp.SetTarget(targ.transform.position);
		mp.SetPath(30, 0.2F);
		mp.SetTileMethod(targ, (Tile t) => 
			{
				Tile newtile = TileMaster.instance.ReplaceTile(t, TileMaster.Types[s._Type], s._Genus, s.Scale, s.Value);
				newtile.GetParams(s.Params);
				for(int e = 0; e < Effects.Count; e++)
				{
					TileEffect effect = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName(Effects[e].Name));
					effect.GetArgs(Effects[e].Duration, Effects[e].Args);
					newtile.AddEffect(Effects[e]);
				}
			});
	}
}

[System.Serializable]
public class SpawnTileInfo : TileShortInfo
{
	public WaveTileSpawn SpawnStyle;
	public Vector2 _SpawnValue;

	public float SpawnValue
	{
		get{
			return Random.Range(_SpawnValue.x, _SpawnValue.y+0.001F);
		}
	}
}




public enum WaveTileSpawn
{
	XAtStart,
	XPerTurn,
	XChance,
	XPsuedoChance,
	XOnScreen,
	OnEnd,
	OnArrive
}



[System.Serializable]
public class MissionContainer
{
	public string Prefix, Suffix;
	public string Command
	{
		get
		{
			return Prefix + " " + ToTarget + " " + Suffix;
		}
	}

	public string TargetType;
	public int TargetNum;
	public int Current;
	public int ToTarget
	{
		get{
			return Mathf.Clamp(TargetNum - Current, 0, TargetNum);
		}
	}

	public MissionContainer(string type, int num, string pre = "", string suff = "")
	{
		TargetType = type;
		TargetNum = num;
		Prefix = pre;
		Suffix = suff;
		Current = 0;
	}

	public bool Check(Tile t)
	{
		if(t.IsType(TargetType))
		{
			Current ++;
		}
		return (Current == TargetNum);
	}
}