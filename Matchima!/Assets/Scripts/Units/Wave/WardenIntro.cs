using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WardenIntro : Wave {

	int current = 0;
	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		current++;
		switch(current)
		{
			case 3:
			yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Strong heroes\nlive longer", "Cast spells to\nstrengthen heroes", "", true, 60));
			break;
			case 2:
			//Alert("Using a spell can\ncause hero mutations");
			break;
		}
		yield return null;
	}

	protected override IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.BotGear.SetTween(3, true);
		UIManager.Objects.TopGear[2].SetActive(false);

		UIManager.Objects.BotGear.SetToState(3);
		UIManager.Objects.TopGear.SetToState(0);
		UIManager.Objects.TopGear.FreeWheelDrag = true;
		UIManager.instance.ShowGearTooltip(false);
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);

		for(int i = 1; i < UIManager.Objects.TopGear[1].Length; i++)
		{
			int genus = Random.Range(0,4);
			int num = TileMaster.Types.Length;
			SPECIES t = TileMaster.Types[Random.Range(0,num)];
			if(t.Atlas == null) continue;
			//UIManager.Objects.TopGear[1][i][0].Img[0].sprite = t.GetSprites(genus)[0];
			//UIManager.Objects.TopGear[1][i][0].Img[2].sprite = TileMaster.Genus.Frame[genus];
			UIManager.Objects.TopGear[1][i][0].SetActive(true);
		}

		StCon [] floor = new StCon[] {new StCon("Floor"), new StCon(GameManager.Floor + "")};

		Current = 0;
		StCon [] namecon = new StCon[] {_Name};
		yield return StartCoroutine(UIManager.instance.Alert(1.25F, floor, namecon));

		UIManager.Objects.TopGear[2].SetActive(true);

		UIManager.instance.ScreenAlert.SetTween(0,false);
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.3F, Slot1, UIManager.Objects.QuoteAtlas, "barbarian", 
																			   UIManager.Objects.QuoteAtlas, "cross", Color.white, Color.red));
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.3F, Slot1, TileMaster.Types["grunt"].Atlas, "Red", 
																				TileMaster.Genus.Frames, "Red"));
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				 yield return StartCoroutine(AllSlots[i].OnStart());
			}
		}

		for(int i = 1; i < UIManager.Objects.TopGear[1].Length; i++)
		{
			UIManager.Objects.TopGear[1][i][0].SetActive(false);
		}


		TileMaster.Types["minion"]["Green"].ChanceInitial = 0.0F;
		TileMaster.Types["grunt"]["Red"].ChanceInitial = 0.0F;
		TileMaster.Types["blob"]["Blue"].ChanceInitial = 0.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		Player.Classes[0].CanMutate = true;
		Player.Classes[1].CanMutate = true;
		Player.Classes[2].CanMutate = true;
		Player.Classes[3].CanMutate = true;
		
		GameData.ChestsFromEnemies = true;

		GameManager.instance.paused = false;
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		UIManager.Objects.TopGear.FreeWheelDrag = false;
		UIManager.Objects.BotGear.SetTween(3, false);
		yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Defeat the\nwarden", "Keep enemy numbers\ndown to survive", "", true, 60));
	}
}
