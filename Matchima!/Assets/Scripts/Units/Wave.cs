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
	public Vector2 PointfieldAddedPerDifficulty = new Vector2(0, 5);

	public int Current;
	public bool Active;
	public WaveTile [] Tiles;
	public WaveTile [] TilesOnStart;
	public int Timer;

	public Vector2 PointfieldEndTime = new Vector2(2,5);

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

	public void IncreaseChances()
	{
		foreach(WaveTile child in Tiles)
		{
			SPECIES s = TileMaster.Types[child.Species];
			GenusInfo g = s[child.Genus];
			//s.ChanceAdded += child.Chance;
			g.ChanceAdded += (child.Chance * TileMaster.ChanceRatio);
			g.Value.Add(child.Value);
			//s.LockValue = child.LockValue;
		}
	}

	public void ResetChances()
	{
		foreach(WaveTile child in Tiles)
		{
			SPECIES s = TileMaster.Types[child.Species];
			GenusInfo g = s[child.Genus];
			if(g.ChanceAdded == 0.0F) continue;
			//s.ChanceAdded -= child.Chance;
			g.ChanceAdded -= (child.Chance * TileMaster.ChanceRatio);
			g.Value.Sub(child.Value);
		}
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

	public void AddPoints(int p)
	{
		PointsThisTurn += p;
	}

	public void WaveCheck()
	{
		if(Effects.TileRegen)
		{
			Tile [,] tiles = TileMaster.Tiles;
			for(int xx = 0; xx < tiles.GetLength(0); xx++)
			{
				for(int yy = 0; yy < tiles.GetLength(1); yy++)
				{
					if(tiles[xx,yy].Type.isEnemy) (tiles[xx,yy] as Enemy).Stats.Hits += Effects.TileRegen_amount;
				}
			}
		}
		if(Effects.WaveRegen)
		{
			AddPoints(Effects.WaveRegen_amount);
		}
		if(Effects.WaveDamage)
		{
			Player.Stats.Hit(Effects.WaveDamage_amount);
		}
	}

	public virtual void OnStart(int _index)
	{
		Index = _index;
		Active = true;
		bool [,] replacedtile = new bool [(int)TileMaster.instance.MapSize.x, (int)TileMaster.instance.MapSize.y];
		for(int i = 0; i < TilesOnStart.Length; i++)
		{
			for(int x = 0; x < TilesOnStart[i].NumToSpawn; x++)
			{
				int randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
				int randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				while(replacedtile[randx, randy])
				{
					randx = (int)Random.Range(0, TileMaster.instance.MapSize.x);
					randy = (int)Random.Range(0, TileMaster.instance.MapSize.y);
				}
				replacedtile[randx,randy] = true;
				TileMaster.instance.ReplaceTile(randx, randy, TileMaster.Types[TilesOnStart[i].Species], TileMaster.Genus[TilesOnStart[i].Genus], TilesOnStart[i].Scale, TilesOnStart[i].FinalValue);
			}
		}
	}

	public virtual void OnTurn()
	{

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
		Required += (int)Random.Range(PointfieldAddedPerDifficulty.x * GameManager.Difficulty, PointfieldAddedPerDifficulty.y * GameManager.Difficulty);
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

[System.Serializable]
public class WaveTile
{
	public string Genus;
	public string Species;
	public float Chance;
	public IntVector Value;
	public int FinalValue
	{
		get
		{
			return Random.Range(Value.x, Value.y);
		}
	}
	public bool LockValue = false;
	public int NumToSpawn;
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

