using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : Wave {
	
	public override IEnumerator OnStart()
	{
		IntroAlert = false;
		yield return StartCoroutine(Player.instance.AddClassToSlot(0, GameData.instance.GetClass("Barbarian")));

		TileMaster.Types["resource"]["Red"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);
		Player.Classes[0].CanCollectMana = false;
		Player.Stats._Health = Player.Stats._HealthMax;

		(UIManager.Objects.TopGear as UIGear).Drag = false;

		/*QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Phew! That was quite a drop!",  Player.Classes[0], true, 1.0F);
		tute.AddQuote("The entrance to the undercity should be this way!",  Player.Classes[0], true, 1.0F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));*/

		Alert("Drag through\ntiles to match");
		current++;
		//yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Drag through\ntiles to match", "", true, 50));
	}

	protected override IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.TopGear[2].SetActive(true);
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				 yield return StartCoroutine(AllSlots[i].OnStart());
			}
		}

		yield return null;
	}

	int current = 0;
	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		UIManager.Objects.BotGear.SetTween(0, true);
		QuoteGroup tute;
		MiniAlertUI mini;
		switch(current)
		{
			case 1:
			

			TileMaster.instance.MapSize_Default = new Vector2(2,3);
			tute = new QuoteGroup("Tute");
			tute.AddQuote("I've got to grab as much mana as I can!",  Player.Classes[0], true, 1.0F);
			//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			Alert("Matches must have\n3 or more tiles");
			//yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Matches must have\n3 or more tiles", "", true, 60));
			current++;
			break;
			case 2:
			TileMaster.instance.MapSize_Default = new Vector2(3,3);
			TileMaster.Tiles[1,1].InitStats.Hits = 2;
			TileMaster.Tiles[1,1].CheckStats();

			TileMaster.Tiles[0,1].InitStats.Hits = 2;
			TileMaster.Tiles[0,1].CheckStats();
			current++;
			break;
			case 3:
			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 30);

			tute = new QuoteGroup("Tute");
			tute.AddQuote("What in the gears was that?",  Player.Classes[0], true, 1.0F);
			//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);
			yield return new WaitForSeconds(Time.deltaTime * 10);
			tute = new QuoteGroup("Tute");
			tute.AddQuote("A beast of mana!",  Player.Classes[0], true, 1.0F);
			//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			Alert("Enemy tiles attack\nyour health");
			//
			current++;
			break;

			case 4:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Argh! It's attacking me!",  Player.Classes[0], true, 1.0F);
			tute.AddQuote("I'll have to destroy it or it will keep attacking.",  Player.Classes[0], true, 1.0F);
			//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			Alert("Match enemy tiles\nto destroy");
			current++;
			break;

			case 5:
			TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["grunt"], GENUS.STR);
			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);	
			current++;
			break;
			case 6:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("There's too many!",  Player.Classes[0], true, 1.0F);
			//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["grunt"], GENUS.STR);
			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);
			current++;
			break;
			case 7:
			yield return StartCoroutine(AddRogue());
			TileMaster.instance.MapSize_Default = new Vector2(4,4);
			current++;
			break;
			case 8:
			if(TileMaster.Enemies.Length == 0)
			{
				tute = new QuoteGroup("Tute");
				tute.AddQuote("That's the last of them.",  Player.Classes[1], true, 1.0F);
				tute.AddQuote("But now we're low on health!",  Player.Classes[1], true, 1.0F);
				tute.AddQuote("...",  Player.Classes[0], true, 1.0F, 0.35F);
				//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

				TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["health"], GENUS.STR);

				tute = new QuoteGroup("Tute");
				tute.AddQuote("A health tile!",  Player.Classes[1], true, 1.0F);
				//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
				Alert("Collect health tiles\nto regain health");
				current++;
			}
			
			break;
			case 9:
			
			TileMaster.instance.ReplaceTile(1,0, TileMaster.Types["health"], GENUS.STR);
			TileMaster.instance.ReplaceTile(2,0, TileMaster.Types["health"], GENUS.DEX);
			TileMaster.instance.ReplaceTile(1,1, TileMaster.Types["health"], GENUS.STR);
			TileMaster.instance.ReplaceTile(2,1, TileMaster.Types["health"], GENUS.DEX);

			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 30);
			Tile t = TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["blob"], GENUS.STR, 2,3);
			t.AddAction(() =>
			{
				StartCoroutine(SplitBlobBoss());
			});

			tute = new QuoteGroup("Tute");
			tute.AddQuote("That one's huge!",  Player.Classes[0], true, 1.0F);
			//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			TileMaster.Types["grunt"]["Red"].ChanceInitial = 0.25F;
			TileMaster.Types["grunt"]["Blue"].ChanceInitial = 0.25F;
			TileMaster.Types.IgnoreAddedChances = false;
			Spawner2.GetSpawnables(TileMaster.Types);

			current++;
			break;
			case 10:
				yield return StartCoroutine(AddWizard());
				current++;
			break;
		}


		if(Player.Classes[1] != null && Player.Classes[1].MeterLvl == 1 && !shownpowerupalert)
		{
			shownpowerupalert = true;
			yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Rogue has\npowered up!", "Tap on a powered\nhero's icon to\ncast a spell", "", true, 60));
		}
		yield return null;
	}

	private bool shownpowerupalert = false;

	public IEnumerator SplitBlobBoss()
	{
		QuoteGroup split = new QuoteGroup("Blob split");
		split.AddQuote("Now there's two of them!", Player.Classes[0], true, 1.0F);
		split.AddQuote("I've got a plan! I just need to collect enough mana...", Player.Classes[1], true, 1.0F);
		//yield return StartCoroutine(UIManager.instance.Quote(split.ToArray()));
		Player.Classes[1].CanCollectMana = true;
		Player.Classes[1].AddToMeter(15);
		Alert("Collecting mana\nfills a hero's\nmana meters");
		yield return null;
	}

	MiniAlertUI alerter;
	public MiniAlertUI Alert(string s)
	{
		if(alerter != null) alerter.PoolDestroy();
		alerter = UIManager.instance.MiniAlert(UIManager.Objects.TopGear.transform.position + Vector3.down* 1.4F,
						s, 70, Color.white, 3.4F, -0.2F, true);
		alerter.transform.localScale *= 0.85F;
		return alerter;
	}


	IEnumerator AddRogue()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("I can't kill them quick enough!",  Player.Classes[0], true, 1.0F);
		
		//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		yield return StartCoroutine(Player.instance.AddClassToSlot(1, GameData.instance.GetClass("Rogue")));
		Player.Classes[1].CanCollectMana = false;

		TileMaster.Types["resource"]["Blue"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("Need some help, traveller?",  Player.Classes[1], true, 1.0F);
		tute.AddQuote("I'll get rid of these monsters.",  Player.Classes[1], true, 1.0F);
		//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

	 	Alert("The Barbarian has \nhigh Dexterity, which\ngives extra attack damage");
	}

	
	IEnumerator AddBard()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Oi Yellow! You're gonna help out, right?", Player.Classes[1], true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		Player.instance.AddClassToSlot(3, GameData.instance.GetClass("Bard"));
		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return new WaitForSeconds(1.1F);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("This oppressive rule won't keep the people down!", Player.Classes[3], true, 1.4F);
		tute.AddQuote("Save it for your freeloading pals!", Slot1, true, 1.4F);
		tute.AddQuote("You're here to clean up mana and you'd better do it fast!", Slot1, true, 1.4F);
		tute.AddQuote("There's worse things down here than hard labour...", Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
	}

	IEnumerator AddWizard()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Wait! Someone's coming!", Player.Classes[0], true, 1.4F);
		//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		yield return StartCoroutine(Player.instance.AddClassToSlot(2, GameData.instance.GetClass("Wizard")));

		TileMaster.Types["resource"]["Green"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("... Hello.", Player.Classes[2], true, 1.0F, 0.4F);
		tute.AddQuote("Do you have any mana you could spare?", Player.Classes[2], true, 1.0F);
		//yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		Alert("The Barbarian has \nhigh Wisdom, which\ngives extra spell damage");
	}
}