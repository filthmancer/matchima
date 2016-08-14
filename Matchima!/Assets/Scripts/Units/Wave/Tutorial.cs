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
		Player.Classes[0].CanMutate = false;
		Player.Stats._Health = Player.Stats._HealthMax;

		(UIManager.Objects.TopGear as UIGear).Drag = false;

		Alert("Drag through\ntiles to match");
		current++;
		yield return new WaitForSeconds(Time.deltaTime * 15);
		StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[0], 
																	TileMaster.Types["resource"].Atlas, "Red",
																	TileMaster.Genus.Frames, "Red"));
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

			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);
			yield return new WaitForSeconds(Time.deltaTime * 10);

			Alert("Enemy tiles attack\nyour health");
		
			current++;
			break;

			case 4:

			Alert("Match enemy tiles\nto destroy");
			
			current++;
			break;

			case 5:
			TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["grunt"], GENUS.STR);
			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);	
			current++;
			break;
			case 6:
		
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
				yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[0], 
														UIManager.Objects.QuoteAtlas, "heart"));
				yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[1], 
														TileMaster.Types["health"].Atlas, "Red",
														TileMaster.Genus.Frames, "Red"));

				TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["health"], GENUS.STR,1,2);
				Alert("Collect health tiles\nto regain health");
				current++;
			}
			
			break;
			case 9:

			yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Collect mana to\nfill a hero's meter", "", true, 60));
			
			TileMaster.instance.ReplaceTile(1,0, TileMaster.Types["health"], GENUS.STR, 1,2);
			TileMaster.instance.ReplaceTile(2,0, TileMaster.Types["health"], GENUS.DEX, 1,2);
			TileMaster.instance.ReplaceTile(1,1, TileMaster.Types["health"], GENUS.STR, 1,2);
			TileMaster.instance.ReplaceTile(2,1, TileMaster.Types["health"], GENUS.DEX, 1,2);
			
			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 30);
			Tile t = TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["blob"], GENUS.STR, 2,3);
			t.AddAction(() =>
			{
				StartCoroutine(SplitBlobBoss());
			});

			TileMaster.Types["grunt"]["Red"].ChanceInitial = 0.25F;
			TileMaster.Types["blob"]["Blue"].ChanceInitial = 0.25F;
			TileMaster.Types.IgnoreAddedChances = false;
			Spawner2.GetSpawnables(TileMaster.Types);

			current++;
			break;
			case 10:
				TileMaster.instance.MapSize_Default = new Vector2(5,5);
				current++;
			break;
			case 11:
			if(shownpowerupalert || Player.instance.Turns > 15)
			{
				yield return StartCoroutine(AddWizard());
				current++;
			}
			break;
			case 12:
				Alert("Use tiles to help\ndefeat enemies.");
				current++;
			break;
			case 13:
			TileMaster.instance.MapSize_Default = new Vector2(5,6);
				Current = Required;
				
			break;
		}


		if(Player.Classes[1] != null && Player.Classes[1].MeterLvl == 1 && !shownpowerupalert)
		{
			shownpowerupalert = true;
			yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Rogue has\npowered up!", "Tap on a powered\nhero's icon to\ncast a spell", "", true, 60));
		}
		else if(Player.Classes[0] != null && Player.Classes[0].MeterLvl == 1 && !shownpowerupalert)
		{
			shownpowerupalert = true;
			yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Barbarian has\npowered up!", "Tap on a powered\nhero's icon to\ncast a spell", "", true, 60));
		}
		yield return null;
	}

	public WaveUnit Warden;

	private bool shownpowerupalert = false;

	public IEnumerator SplitBlobBoss()
	{
		Player.Classes[1].CanCollectMana = true;
		Player.Classes[0].CanCollectMana = true;
		
		
		Player.Classes[1].AddToMeter(Player.Classes[1].MeterTop-5);
		Player.Classes[0].AddToMeter(Player.Classes[0].MeterTop-5);
		
		yield return null;
	}



	IEnumerator AddRogue()
	{
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[0], 
														UIManager.Objects.QuoteAtlas, "death"));
		ThrowKnives p = GameData.instance.GetPowerup("Throw Knives", null) as ThrowKnives;
		StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[0], 
														UIManager.Objects.QuoteAtlas, "confused"));
		for(int i = 0; i < TileMaster.Enemies.Length; i++)
		{
			yield return StartCoroutine(p.ThrowKnife(TileMaster.Enemies[i], 10));
			yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		}
		yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		
		TileMaster.instance.SetFillGrid(false);
		PlayerControl.instance.AddTilesToSelected(TileMaster.Enemies);
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		TileMaster.instance.SetFillGrid(true);
		TileMaster.instance.ResetTiles(true);

		for(int i = 0; i < p.knifelist.Count; i++)
		{if(p.knifelist[i] != null) Destroy(p.knifelist[i].gameObject);}

		Destroy(p.gameObject);

		yield return StartCoroutine(Player.instance.AddClassToSlot(1, GameData.instance.GetClass("Rogue")));
		Player.Classes[1].CanCollectMana = false;
		Player.Classes[1].CanMutate = false;

		TileMaster.Types["resource"]["Blue"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[1], 
													TileMaster.Types["resource"].Atlas, "Blue",
													TileMaster.Genus.Frames, "Blue"));
	 	Alert("The Rogue has \nhigh Dexterity");
	}

	


	IEnumerator AddWizard()
	{
		Firestorm p = GameData.instance.GetPowerup("Firestorm", null) as Firestorm;
		yield return StartCoroutine(p.Cast(TileMaster.Tiles[2,0]));
		yield return StartCoroutine(p.Cast(TileMaster.Tiles[0,0]));
		yield return StartCoroutine(p.Cast(TileMaster.Tiles[4,0]));

		TileMaster.instance.SetFillGrid(false);
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		
		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		Destroy(p.gameObject);

		yield return StartCoroutine(Player.instance.AddClassToSlot(2, GameData.instance.GetClass("Wizard")));

		TileMaster.Types["resource"]["Green"].ChanceInitial = 1.0F;
		TileMaster.Types["minion"]["Green"].ChanceInitial = 0.25F;

		Player.Classes[2].CanCollectMana = true;
		Player.Classes[2].CanMutate = false;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[2], 
													TileMaster.Types["resource"].Atlas, "Green",
													TileMaster.Genus.Frames, "Green"));
		Alert("The Wizard has \nhigh Wisdom");
	}

	protected override IEnumerator WaveEndRoutine()
	{
		yield return null;
	}
}