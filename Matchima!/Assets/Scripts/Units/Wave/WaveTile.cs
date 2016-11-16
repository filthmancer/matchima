using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveTile : WaveUnit
{
	public string GenusOverride;
	[HideInInspector]
	public string GenusString;
	[SerializeField]
	protected string Species;
	public string SpeciesFinal
	{
		get{
			if(!SetStringFinal.HasValue)
			{
				SetStringFinal = Random.Range(0, SpeciesChoice.Length);
			}
			return SpeciesChoice[SetStringFinal.Value];
		}
	}
	private int? SetStringFinal = null;
	public string [] SpeciesChoice;

	protected WaveTileSpawnStyle Style;
	public WaveTileSpawnStyle [] _SpawnStyles = new WaveTileSpawnStyle[]{
		new WaveTileSpawnStyle(WaveTileSpawn.XPsuedoChance, new Vector2(1,3))
	};


	public int PointsPerTurn = 3, PointsPerEnemy = 1;
	private int PointsThisTurn = 0;

	//public WaveTileSpawn SpawnType;

	
	//public int TotalToSpawn = -1;

	/*public float Factor
	{
		get{
			return Random.Range(SpawnFactorField.x, SpawnFactorField.y+0.001F);
		}
	}*/
	//public Vector2 SpawnFactorField = new Vector2(1.0F, 1.0F);
	public IntVector Value;
	public int FinalValue
	{
		get
		{
			return Random.Range(Value.x, Value.y);
		}
	}
	public int Scale = 1;
	public bool HitByPresence = false;

	public override void Setup(Wave p, int i)
	{
		Parent = p;
		Index = i;
		Active = false;
		//Timer = Random.Range(PrepTime.x, PrepTime.y);
		if(GenusOverride.ToLower() == "random") GenusString = GameData.ResourceLong((GENUS)Random.Range(0,4));
		else if(GenusOverride.ToLower() == "randomall") GenusString = GameData.ResourceLong((GENUS)Random.Range(0,6));
		else GenusString = GenusOverride;

		Genus = TileMaster.Genus[GenusString];

		if(SpeciesChoice == null || SpeciesChoice.Length == 0) SpeciesChoice = new string []{Species};

		if(_SpawnStyles.Length == 0) Style = new WaveTileSpawnStyle(WaveTileSpawn.XPsuedoChance, new Vector2(1,3));
		else
		{
			int num = (int) Random.Range(0, _SpawnStyles.Length);
			Style = _SpawnStyles[num];
		}
	}

	public override void GetChances()
	{
		if(!Active || Ended) return;
		if(Style.Type == WaveTileSpawn.XChance)
		{
			TileMaster.instance.IncreaseChance(GenusString, SpeciesFinal, Style.Value);
		}
		List<TileEffectInfo> effects = Parent.GetEffects();
		SPECIES s = TileMaster.Types[SpeciesFinal];
		GenusInfo g = s[GenusString];
		if(Value.y > 0)
		{
			g.ValueAdded.Add(Value);
		}

		for(int i = 0; i < effects.Count; i++)
		{
			if(!effects[i].ApplyToTileType) continue;
			if(effects[i].ApplyToSpecies)
			{
				s.Effects.Add(effects[i]);
			}
			else
			{
				g.Effects.Add(effects[i]);
			}
		}
	}


	public override IEnumerator OnStart()
	{
		if(!Active || Ended) yield break;
		
	//Spawn at start
		if(Style.Type != WaveTileSpawn.XAtStart) yield break;
		GameManager.instance.paused = true;

		Tile [] replaces = GetTilesToReplace((int)Style.Value, "resource", "enemy", "health");
		List<TileEffectInfo> Effects = Parent.GetEffects();

		for(int x = 0; x < (int)Style.Value; x++)
		{
			GameObject initpart = EffectManager.instance.PlayEffect(UIManager.WaveButtons[Index].transform, Effect.Spell);
			MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
			mp.SetTarget(replaces[x].transform.position);
			mp.SetPath(30, 0.2F);
			mp.SetTileMethod(replaces[x], (Tile t) => 
				{
					print(t);
					Tile newtile = TileMaster.instance.ReplaceTile(t, TileMaster.Types[SpeciesFinal], Genus, Scale, FinalValue);

					for(int i = 0; i < Effects.Count; i++)
					{
						TileEffect effect = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName(Effects[i].Name));
						effect.GetArgs(Effects[i].Duration, Effects[i].Args);
						newtile.AddEffect(Effects[i]);
					}
				});
			
			if(Style.Value > 1) yield return new WaitForSeconds(Time.deltaTime * 10);
		}
		yield return new WaitForSeconds(Time.deltaTime * 15);
	}


	public override void OnEnd()
	{
		Active = false;
		Ended = true;
	}


	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		if(!Active || Ended) yield break;
		//if(HitByPresence) AddPoints(-Player.Stats.Presence);
	//Spawn per round

		if(Style.Type != WaveTileSpawn.XPerTurn) yield break;
		GameManager.instance.paused = true;
		Tile [] replaces = GetTilesToReplace((int)Style.Value, "resource", "enemy", "health");

		for(int x = 0; x < (int)Style.Value; x++)
		{
			List<TileEffectInfo> Effects = Parent.GetEffects();
			GameObject initpart = EffectManager.instance.PlayEffect(UIManager.WaveButtons[Index].transform, Effect.Spell);
			MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
			mp.SetTarget(replaces[x].transform.position);
			mp.SetPath(30, 0.2F);
			mp.SetTileMethod(replaces[x], (Tile t) => 
				{
					Tile newtile = TileMaster.instance.ReplaceTile(t, TileMaster.Types[SpeciesFinal], Genus, Scale, FinalValue);
					for(int i = 0; i < Effects.Count; i++)
					{
						TileEffect effect = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName(Effects[i].Name));
						effect.GetArgs(Effects[i].Duration, Effects[i].Args);
						newtile.AddEffect(Effects[i]);
					}
				});
			yield return new WaitForSeconds(Time.deltaTime * 5);
		}
		//yield return new WaitForSeconds(Time.deltaTime * 5);
		GameManager.instance.paused = false;
	}

	public override IEnumerator AfterTurn()
	{
		if(!Active) yield break;
		Complete();
		if(Style.Type == WaveTileSpawn.XOnScreen)
		{
			int onscreen = 0;
			for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
			{
				for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
				{
					if(TileMaster.Tiles[x,y].IsType(GenusString, SpeciesFinal)) onscreen++;
				}
			}
			while(onscreen < (int) Style.Value)
			{
				TileMaster.instance.QueueTile(TileMaster.Types[SpeciesFinal], TileMaster.Genus[GenusString]);
				onscreen++;
			}
		}
		
		if(Style.Type == WaveTileSpawn.XPsuedoChance)
		{
			for(int i = 0; i < (int) Style.Value; i++)
			{
				TileMaster.instance.QueueTile(TileMaster.Types[SpeciesFinal], TileMaster.Genus[GenusString]);
			}
		}
		
		
		yield return null;
	}

	
}

public class WaveTileSpawnStyle
{
	public WaveTileSpawn Type;
	public int Value;
	public WaveTileSpawnStyle(WaveTileSpawn s, Vector2 v)
	{

	}
}
