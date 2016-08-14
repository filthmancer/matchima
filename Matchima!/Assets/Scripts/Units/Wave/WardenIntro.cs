using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WardenIntro : Wave {

		/*public override IEnumerator OnStart()
		{
			Slot1.Timer = 0;
			Slot1.Activate();

			QuoteGroup tute = new QuoteGroup("Tute");
			tute.AddQuote("Ten Hut!",  Slot1, true, 1.4F);
			tute.AddQuote("Alright scoundrel, welcome to the Undercity.",  Slot1, true, 1.4F);
			tute.AddQuote("The Council has judged your crime and sentenced you here.",  Slot1, true, 1.4F);
			tute.AddQuote("You must clear the Undercity of this filthy plague of Mana!",  Slot1, true, 1.4F);

			int rand = Random.Range(0,4);
			switch(rand)
			{
				case 0:
					tute.AddQuote("...",  Player.Classes[0], true, 1.4F, 0.35F);
					tute.AddQuote("Well? Don't just stand there!",  Slot1, true, 1.4F);
				break;
				case 1:
					tute.AddQuote("All this work is beneath me.",  Player.Classes[1], true, 1.4F);
					tute.AddQuote("Keep it down vermin!",  Slot1, true, 1.4F);
				break;
				case 2:
					tute.AddQuote("Ohh man, I can smell... mana!",  Player.Classes[2], true, 1.4F);
					tute.AddQuote("Keep quiet, you fool!",  Slot1, true, 1.4F);
				break;
				case 3:
					tute.AddQuote("This oppressive rule won't keep the people down!", Player.Classes[3], true, 1.4F);
					tute.AddQuote("Save it for your freeloading pals!", Slot1, true, 1.4F);
				break;
			}
			
			tute.AddQuote("Get matching!",  Slot1, true, 1.4F);
			//tute.AddQuote("Drag through Mana to collect it!",  Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		}*/

		int current = 0;
		public override IEnumerator BeginTurn()
		{
			yield return StartCoroutine(base.BeginTurn());
			current++;
			switch(current)
			{
				case 2:
				yield return StartCoroutine(AddBard());
				break;
				case 4:
				Alert("Using a spell can\ncause hero mutations");
				break;
				case 6:
				Alert("Each color corresponds\nto a Stat type");
				break;
			}
			yield return null;
		}

	IEnumerator AddBard()
	{
		Lullaby p = GameData.instance.GetPowerup("Lullaby", null) as Lullaby;
		for(int i = 0; i < TileMaster.Enemies.Length; i++)
		{
			p.Sleep(TileMaster.Enemies[i], 2);
		}
		

		TileMaster.instance.SetFillGrid(false);
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		yield return new WaitForSeconds(Time.deltaTime * 10);
		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		Destroy(p.gameObject);
		Player.instance.AddClassToSlot(3, GameData.instance.GetClass("Bard"));
		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[3], 
													TileMaster.Types["resource"].Atlas, "Yellow",
													TileMaster.Genus.Frames, "Yellow"));
		
		Alert("The Bard has \nhigh Charisma");
	}

	protected override IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.BotGear.SetTween(3, true);
		UIManager.Objects.TopGear[2].SetActive(false);
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);
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

		UIManager.instance.ScreenAlert.SetTween(1,true);
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.3F, Slot1, UIManager.Objects.QuoteAtlas, "death"));
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.3F, Slot1, TileMaster.Types["guard"].Atlas, "Alpha", 
																				TileMaster.Genus.Frames, "Omega"));

		UIManager.instance.ScreenAlert.SetTween(1,false);
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

		GameManager.instance.paused = false;
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		UIManager.Objects.BotGear.SetTween(3, false);
		Alert("Collecting mana\nfills the enemy bar");
	}
}
