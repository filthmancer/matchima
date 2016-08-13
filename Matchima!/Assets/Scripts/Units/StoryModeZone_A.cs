using UnityEngine;
using System.Collections;

public class StoryModeZone_A : Zone {
	public WaveUnit WardenUnit;

	public override Wave CheckZone()
	{
		
		return GetWaveProgressive();
	}

	public override IEnumerator Enter()
	{
		TileMaster.Types.IgnoreAddedChances = true;
		GameData.ChestsFromEnemies = false;
		
		TileMaster.Types["resource"]["Red"].ChanceInitial = 1.0F;
		TileMaster.Types["resource"]["Green"].ChanceInitial = 0.0F;
		TileMaster.Types["resource"]["Blue"].ChanceInitial = 0.0F;
		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 0.0F;

		TileMaster.Types["health"]["Red"].ChanceInitial = 0.0F;
		TileMaster.Types["health"]["Green"].ChanceInitial = 0.0F;
		TileMaster.Types["health"]["Blue"].ChanceInitial = 0.0F;
		TileMaster.Types["health"]["Yellow"].ChanceInitial = 0.0F;
		
		
		yield return StartCoroutine(base.Enter());
	}
}