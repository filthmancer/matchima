using UnityEngine;
using System.Collections;

public class CreateBombs : Powerup {

	int bombs_per_lvl = 3;
	protected override IEnumerator Minigame(int Level)
	{
		yield return StartCoroutine(PowerupStartup());
		UIManager.instance.ScreenAlert.SetTween(0, false);

		yield return StartCoroutine(Cast(Level));

		yield return new WaitForSeconds(Time.deltaTime * 30);
		GameManager.instance.paused = false;
		
	}

	IEnumerator Cast(int Level)
	{
		int bombnum = Level * bombs_per_lvl;

		for(int i = 0 ; i < bombnum; i++)
		{
			Tile target = TileMaster.RandomTile;
			while(!target.IsType("resource")) target = TileMaster.RandomTile;
			yield return StartCoroutine(CreateBomb(target));	
		}
	}

	IEnumerator CreateBomb(Tile target)
	{
		yield return new WaitForSeconds(Time.deltaTime * 10);
	}
}
