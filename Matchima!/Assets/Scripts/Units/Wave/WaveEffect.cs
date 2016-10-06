using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveEffect : WaveUnit {
	public int EffectApplyTimer = 0;
	public WaveEffectType Type;

	public TileEffectInfo Effect;

	public override TileEffectInfo [] GetEffects()
	{
		if(!Active || Ended) return null;

		return new TileEffectInfo[]{Effect};
	}
}

[System.Serializable]
public class TileEffectInfo
{
	public string Name;
	public int Duration;
	public string [] Args;
	public bool ApplyToTileType, ApplyToSpecies;
	public TileEffectInfo(string n, int d, params string [] arg)
	{
		Name = n;
		Duration = d;
		Args = arg;
	}
}

public enum WaveEffectType
{
	PerTurn,
	ToAll,
}