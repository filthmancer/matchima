using UnityEngine;
using System.Collections;

public class Wave : Unit {

	public string Description
	{
		get
		{
			string d = "";
			if(DescriptionOverride != "") 
			{
				d += DescriptionOverride;
			}
			else
			{
				d = "Spawning ";
				for(int i = 0; i < Tiles.Length; i++)
				{
					d += Tiles[i].Genus + " " + Tiles[i].Species;
					if(i < Tiles.Length-1) d += ", ";
				}
				d += " tiles.";
			}
			d += "\n";
			if(!Effects.LevelUpOnEnd) d += "\nWill not level up on wave end.";
			if(Effects.TileRegen) d += "\nEnemies gain health each turn.";
			if(Effects.WaveRegen) d += "\nWave gains health each turn.";
			if(Effects.WaveDamage) d += "\nWave damages player each turn.";
				
			return d;
		}
	}

	public string DescriptionOverride;
	public Sprite Inner, Border;

	public float Chance = 1.0F;
	public int RequiredDifficulty = 1;

	public int Required = 60;
	public int PointsPerTurn = 3, PointsPerEnemy = 1;
	public Vector2 DiffScale = new Vector2(0, 5);

	public int Current;
	public bool Active;
	public WaveTile [] Tiles;
	public int Timer;

	public Vector2 PrepTime = new Vector2(2,5);

	public string [] _Quotes;
	public bool Ignore = false;
	private int PointsThisTurn = 0;

	private bool ShowingHealth;


	public float HealthRatio{
		get{ return (float)Current/(float)Required;}
	}


	public WaveEffects Effects;

	public string Quote
	{
		get{
			if(_Quotes.Length == 0) return null;
			int r = UnityEngine.Random.Range(0, _Quotes.Length);
			return _Quotes[r];
		}
	}

	public void GetChances()
	{
		foreach(WaveTile w in Tiles)
		{
			if(w.SpawnType != WaveTileSpawn.XChance) continue;
			TileMaster.instance.IncreaseChance(w.Genus, w.Species, w.SpawnFactor);
			if(w.Value.y > 0)
			{
				SPECIES s = TileMaster.Types[w.Species];
				GenusInfo g = s[w.Genus];
				g.ValueAdded.Add(w.Value);
			}
		}
	}


	public void AddPoints(int p)
	{
		PointsThisTurn += p;
	}

	public void EnemyKilled(Enemy e)
	{
		PointsThisTurn -= PointsPerEnemy * e.Stats.Value;	
		Current -= PointsPerEnemy * e.Stats.Value;
		if(!ShowingHealth)
		{
			StartCoroutine(ShowHealthRoutine());
		}
	}

	IEnumerator ShowHealthRoutine()
	{
		ShowingHealth = true;

		int current_heal = PointsThisTurn;
		Vector3 tpos = Vector3.right * 0.4F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.Objects.WaveSlots[Index].Txt[0].transform.position + tpos, 
			"-" + current_heal, 42, GameData.instance.BadColour, 1.7F,	0.01F);

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
				heal.text = "+" + current_heal;
			}
			

			yield return null;
		}

		ShowingHealth = false;

		yield return null;
	}


	public virtual IEnumerator BeginTurn()
	{
		for(int i = 0; i < _Status.Count; i++)
		{
			if(_Status[i].CheckDuration()) 
			{
				Destroy(_Status[i].gameObject);
				_Status.RemoveAt(i);
			}
		}

		bool [,] replacedtile = new bool [(int)TileMaster.instance.MapSize.x, (int)TileMaster.instance.MapSize.y];
		for(int i = 0; i < Tiles.Length; i++)
		{
			if(Tiles[i].SpawnType != WaveTileSpawn.XPerTurn) continue;

			for(int x = 0; x < (int)Tiles[i].SpawnFactor; x++)
			{
				int randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
				int randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				while(replacedtile[randx, randy])
				{
					randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
					randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				}
				replacedtile[randx,randy] = true;
				TileMaster.instance.ReplaceTile(randx, randy, TileMaster.Types[Tiles[i].Species], TileMaster.Genus[Tiles[i].Genus], Tiles[i].Scale, Tiles[i].FinalValue);
			}
		}
		yield return null;
	}

	public void Reset()
	{
		for(int i = 0; i < _Status.Count; i++)
		{
			_Status[i].StatusEffect();
		}
	}

	public virtual void OnStart(int _index)
	{
		Index = _index;
		Active = true;
		bool [,] replacedtile = new bool [(int)TileMaster.instance.MapSize.x, (int)TileMaster.instance.MapSize.y];
		for(int i = 0; i < Tiles.Length; i++)
		{
			if(Tiles[i].SpawnType != WaveTileSpawn.XAtStart) continue;
			for(int x = 0; x < (int)Tiles[i].SpawnFactor; x++)
			{
				int randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
				int randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				while(replacedtile[randx, randy])
				{
					randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
					randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				}
				replacedtile[randx,randy] = true;
				TileMaster.instance.ReplaceTile(randx, randy, TileMaster.Types[Tiles[i].Species], TileMaster.Genus[Tiles[i].Genus], Tiles[i].Scale, Tiles[i].FinalValue);
			}
		}
	}

	public bool Complete()
	{
		if(PointsThisTurn == 0) return false;
		//Vector3 wpos = Vector3.right * 0.5F;
		//UIManager.instance.MiniAlert(UIManager.instance.WavePoints.transform.position + wpos, "" + PointsThisTurn, 55, PointsThisTurn > 0 ? Color.green : Color.red, 1.0F, PointsThisTurn > 0 ? 0.02F : -0.02F);
		
		Current += PointsThisTurn;
		if(Current < 0) Current = 0;
		if(Current > Required) Current = Required;
		PointsThisTurn = 0;
		return true;
	}

	public void InstaKill()
	{
		PointsThisTurn = -Current;
		Complete();
		GameManager.instance.WaveEnd(Index);
	}

	public void GetEffects(bool [] fx, int [] amt)
	{
		Effects.TileRegen = fx[0];
		Effects.TileRegen_amount = amt[0];
		Effects.WaveRegen = fx[1];
		Effects.WaveRegen_amount = amt[1];
		Effects.WaveDamage = fx[2];
		Effects.WaveDamage_amount = amt[2];
	}

	public void Randomise()
	{
		Required += (int)Random.Range(DiffScale.x * GameManager.Difficulty, DiffScale.y * GameManager.Difficulty);
		if(Random.value < Effects.TileRegenChance) 
		{
			Effects.TileRegen = true;
			Effects.TileRegen_amount += (int)GameManager.Difficulty/2;
		}
		if(Random.value < Effects.WaveRegenChance) 
		{
			Effects.WaveRegen = true;
			Effects.WaveRegen_amount += (int)GameManager.Difficulty/5;
		}
		if(Random.value < Effects.WaveDamageChance)
		{
		 Effects.WaveDamage = true;
		  Effects.WaveDamage_amount += (int)GameManager.Difficulty/2;
		}
	}
}

public enum WaveTileSpawn
{
	XAtStart,
	XPerTurn,
	XChance,
	XOnScreen
}

[System.Serializable]
public class WaveTile
{
	public string Genus;
	public string Species;
	public WaveTileSpawn SpawnType;
	public int TotalToSpawn = -1;

	public float SpawnFactor;
	public IntVector Value;
	public int FinalValue
	{
		get
		{
			return Random.Range(Value.x, Value.y);
		}
	}
	public int Scale = 1;
}


[System.Serializable]
public class WaveEffects
{
	public bool LevelUpOnEnd = true;
	public bool HitByPresence = false;

	public float TileRegenChance = 0.0F;
	[HideInInspector]
	public bool TileRegen = false;
	[HideInInspector]
	public int TileRegen_amount = 1;

	public float WaveRegenChance = 0.0F;
	[HideInInspector]
	public bool WaveRegen = false;
	[HideInInspector]
	public int WaveRegen_amount = 1;

	public float WaveDamageChance = 0.0F;
	[HideInInspector]
	public bool WaveDamage = false;
	[HideInInspector]
	public int WaveDamage_amount = 2;

	public bool [] FXActive 
	{
		get{
			return new bool [] {TileRegen,
								WaveRegen,
								WaveDamage};
		}
	}

	public int [] FXAmount
	{
		get
		{
			return new int [] { TileRegen_amount,
								WaveRegen_amount,
								WaveDamage_amount};
		}
	}
}

