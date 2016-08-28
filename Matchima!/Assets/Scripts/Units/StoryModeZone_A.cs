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
		
		Randomise();
		
		(UIManager.Objects.MiddleGear[2] as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.MiddleGear[1] as UIObjTweener).SetTween(0, false);
		UIManager.instance.BackingTint = Tint;
		UIManager.instance.WallTint = WallTint;
		
		TileMaster.instance.MapSize_Default = GetMapSize();
		
		
		Player.instance.ResetStats();
		
		if(GameManager.ZoneNum > 1) 
		{
			yield return null;
			yield return StartCoroutine(TileMaster.instance.NewGridRoutine());
		}
		yield return null;
	}
}