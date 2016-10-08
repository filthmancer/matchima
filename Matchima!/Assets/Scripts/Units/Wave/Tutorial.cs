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

		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[0], 
													TileMaster.Types["resource"].Atlas, "Red",
													TileMaster.Genus.Frames, "Red"));

		yield return new WaitForSeconds(Time.deltaTime * 10);
		Alert("Drag through\n3 or more tiles");
		current++;
		
		
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
	MiniAlertUI TuteAlert;
	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		UIManager.Objects.BotGear.SetToState(0);
		QuoteGroup tute;
		switch(current)
		{
			case 1:
			GameManager.Zone.MapSize = new IntVector(2,3);
			TileMaster.instance.MapSize_Default = GameManager.Zone.GetMapSize();
			TileMaster.instance.CheckGrid();
			yield return new WaitForSeconds(Time.deltaTime * 3);
			while(!TileMaster.AllLanded) yield return null;

			TileMaster.Tiles[0,2].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[1,2].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[0,1].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[1,1].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[0,0].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[1,0].ChangeGenus(GENUS.DEX);

			//TuteAlert = Alert("Matches must have\n3 or more tiles");
			
			current++;
			break;
			case 2:

			GameManager.Zone.MapSize = new IntVector(3,3);
			TileMaster.instance.MapSize_Default = GameManager.Zone.GetMapSize();
			TileMaster.instance.CheckGrid();
			yield return new WaitForSeconds(Time.deltaTime * 3);
			while(!TileMaster.AllLanded) yield return null;

			TileMaster.Tiles[0,2].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[1,2].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[2,2].ChangeGenus(GENUS.WIS);

			TileMaster.Tiles[0,1].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[1,1].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[2,1].ChangeGenus(GENUS.WIS);

			TileMaster.Tiles[0,0].ChangeGenus(GENUS.WIS);
			TileMaster.Tiles[1,0].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[2,0].ChangeGenus(GENUS.STR);

			current++;
			break;
			case 3:

			GameManager.Zone.MapSize = new IntVector(4,4);
			TileMaster.instance.MapSize_Default = GameManager.Zone.GetMapSize();
			TileMaster.instance.CheckGrid();
			yield return new WaitForSeconds(Time.deltaTime * 3);
			while(!TileMaster.AllLanded) yield return null;

			TileMaster.Tiles[0,3].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[1,3].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[2,3].ChangeGenus(GENUS.WIS);
			TileMaster.Tiles[3,3].ChangeGenus(GENUS.CHA);

			TileMaster.Tiles[0,2].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[1,2].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[2,2].ChangeGenus(GENUS.CHA);
			TileMaster.Tiles[3,2].ChangeGenus(GENUS.WIS);

			TileMaster.Tiles[0,1].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[1,1].ChangeGenus(GENUS.CHA);
			TileMaster.Tiles[2,1].ChangeGenus(GENUS.STR);
			TileMaster.Tiles[3,1].ChangeGenus(GENUS.WIS);

			TileMaster.Tiles[0,0].ChangeGenus(GENUS.CHA);
			TileMaster.Tiles[1,0].ChangeGenus(GENUS.DEX);
			TileMaster.Tiles[2,0].ChangeGenus(GENUS.WIS);
			TileMaster.Tiles[3,0].ChangeGenus(GENUS.STR);

			TileMaster.Tiles[1,1].InitStats.Hits = 2;
			TileMaster.Tiles[1,1].CheckStats();

			TileMaster.Tiles[2,1].InitStats.Hits = 2;
			TileMaster.Tiles[2,1].CheckStats();

			TileMaster.Tiles[1,2].InitStats.Hits = 2;
			TileMaster.Tiles[1,2].CheckStats();

			TileMaster.Tiles[2,2].InitStats.Hits = 2;
			TileMaster.Tiles[2,2].CheckStats();
			TuteAlert = Alert("Some tiles take\nmultiple hits to destroy");
			current++;
			break;

			case 4:
			TuteAlert.PoolDestroy();
			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 25);
			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);
			yield return new WaitForSeconds(Time.deltaTime * 25);
			
			UIManager.instance.SetTileAlert(TileMaster.Types["grunt"], "Red");
			yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Enemy tiles attack your\nhealth after every turn", "", true, 60));
			
			
			current++;
			break;
			
			case 5:
			if(alerter != null) Destroy(alerter.gameObject);
			TuteAlert = Alert("Match enemy tiles\nto destroy");
			current++;
			break;
			case 6:
			TuteAlert = Alert("A red X means your\nattack will kill\nthe enemy");
			TileMaster.instance.ReplaceTile(0,3, TileMaster.Types["grunt"], GENUS.STR);
			TileMaster.instance.ReplaceTile(3,3, TileMaster.Types["grunt"], GENUS.DEX);	
			current++;
			
			break;
			case 7:
			TuteAlert = Alert("Enemies have different\nattacks and health");
			TileMaster.instance.ReplaceTile(1,3, TileMaster.Types["grunt"], GENUS.CHA);
			TileMaster.instance.ReplaceTile(2,3, TileMaster.Types["grunt"], GENUS.WIS);

			yield return StartCoroutine(AddRogue());
			
			current+=2;
			break;
			case 8:
			if(dex_alert)
			{
				//TuteAlert = Alert("10 Dexterity increases\n+1 attack damage", 2.4F);
				current++;
			}
			break;
			case 9:
				if(TileMaster.Enemies.Length == 0)
				{
					yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[0], 
															UIManager.Objects.QuoteAtlas, "heart"));
					yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[1], 
															TileMaster.Types["health"].Atlas, "Red",
															TileMaster.Genus.Frames, "Red"));

					TileMaster.instance.ReplaceTile(1,0, TileMaster.Types["health"], GENUS.STR,1,2);
					//TuteAlert = Alert("Collect health tiles\nto regain health",2.4F);
					yield return new WaitForSeconds(Time.deltaTime * 25);
					UIManager.instance.SetTileAlert(TileMaster.Types["health"], "Red");
					yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Collect Health Tiles\nto regain health", "", true, 60));
					current++;
				}
			break;
			case 10:
			TileMaster.instance.ReplaceTile(1,1, TileMaster.Types["health"], GENUS.STR, 1,2);
			TileMaster.instance.ReplaceTile(2,1, TileMaster.Types["health"], GENUS.DEX, 1,2);
			
			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 30);
			Tile t = TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["blob"], GENUS.STR, 2,1);
			t.AddAction(() =>
			{
				//current++;
				//StartCoroutine(SplitBlobBoss());
			});

			TileMaster.Types["grunt"]["Red"].ChanceInitial = 0.16F;
			TileMaster.Types["blob"]["Blue"].ChanceInitial = 0.14F;
			TileMaster.Types.IgnoreAddedChances = false;
			Spawner2.GetSpawnables(TileMaster.Types);


			current++;
			break;
			case 11:
			TileMaster.instance.MapSize_Default = new Vector2(5,5);
			current++;
			break;
			case 12:
				Player.Classes[1].CanCollectMana = true;
				Player.Classes[0].CanCollectMana = true;
				Player.Classes[0].Meter = Player.Classes[0].MeterTop - 3;
				Player.Classes[1].Meter = Player.Classes[1].MeterTop - 3;
				UIManager.instance.SetTileAlert(TileMaster.Types["resource"], "Blue");
				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Collect mana to\ncast hero spells", "", true, 60));
				
				current++;
			break;
			case 13:
			if(shownpowerupalert || Player.instance.Turns > 19)
			{
				yield return StartCoroutine(AddWizard());
				current++;
			}
			break;
			case 14:
			current++;
			break;
			case 15:
				yield return StartCoroutine(AddBard());
				current++;
			break;
			case 16:
			current++;
			TileMaster.instance.MapSize_Default = new Vector2(5,6);
			Current = RequiredFinal;

			break;
		}

		if(!shownpowerupalert)
		{
			if(Player.Classes[0].MeterLvl >= 1)
			{
				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "A spell\nis ready!", "Tap Barbarian's\nicon to\ncast Heal", "",
																	 true, 60, UIManager.ClassButtons.GetClass(0)));
				yield return StartCoroutine(Player.Classes[0].UseManaPower());
				shownpowerupalert = true;		
			}
			if(Player.Classes[1] != null && Player.Classes[1].MeterLvl >= 1)
			{
				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "A spell\nis ready!", "Tap Rogue's\nicon to\ncast Throw Knives", "",
																	 true, 60, UIManager.ClassButtons.GetClass(1)));
				yield return StartCoroutine(Player.Classes[1].UseManaPower());
				shownpowerupalert = true;
			}
		}

		yield return null;
	}

	//TuteAlert = Alert("30 Charisma increases\n+1 tile value", 4.4F);
	//TuteAlert = Alert("1 Strength gives\n+5 health points",4.4F);
	//TuteAlert = Alert("5 Wisdom gives\n+1 spell damage", 4.4F);

	public WaveUnit Warden;

	private bool shownpowerupalert = false;
	private bool wis_alert, dex_alert, cha_alert;

	public IEnumerator SplitBlobBoss()
	{
		
		
		yield return null;
	}

	IEnumerator AddRogue()
	{
		yield return StartCoroutine(UIManager.instance.ImageQuote(0.9F, Player.Classes[0], 
														UIManager.Objects.QuoteAtlas, "death"));
		ThrowKnives p = GameData.instance.GetPowerup("Throw Knives", null) as ThrowKnives;
		p.SetParentOverride(UIManager.ClassButtons.GetClass(1).transform);
		UIManager.ClassButtons.GetClass(1).ShowClass(true);
		
		GameObject powerup = EffectManager.instance.PlayEffect(UIManager.ClassButtons.GetClass(1).transform, Effect.ManaPowerUp, GameData.Colour((GENUS) 1));
		powerup.transform.localScale = Vector3.one;

		//StartCoroutine(UIManager.instance.ImageQuote(0.7F, Player.Classes[0], 
		//												UIManager.Objects.QuoteAtlas, "confused"));

		for(int i = 0; i < TileMaster.Enemies.Length; i++)
		{
			yield return StartCoroutine(p.ThrowKnife(TileMaster.Enemies[i], 50));
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
		UIManager.ClassButtons.GetClass(1).ShowClass(false);
		Destroy(powerup);

		yield return new WaitForSeconds(Time.deltaTime * 25);
		yield return StartCoroutine(Player.instance.AddClassToSlot(1, GameData.instance.GetClass("Rogue")));
		Player.Classes[1].CanCollectMana = false;
		Player.Classes[1].CanMutate = false;

		TileMaster.Types["resource"]["Blue"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[1], 
													TileMaster.Types["resource"].Atlas, "Blue",
													TileMaster.Genus.Frames, "Blue"));
	 	TuteAlert =  Alert("The Rogue has \nhigh Dexterity", 2.4F);
	 	TuteAlert.AddAction( ()=>
	 	{
	 		dex_alert = true;
	 		//Alert("Dexterity increases\nattack damage", 2.4F);
	 	});
	 	
	}

	IEnumerator AddWizard()
	{
		UIManager.ClassButtons.GetClass(2).ShowClass(true);
		GameObject powerup = EffectManager.instance.PlayEffect(UIManager.ClassButtons.GetClass(2).transform, Effect.ManaPowerUp, GameData.Colour((GENUS) 2));
		powerup.transform.localScale = Vector3.one;

		Firestorm p = GameData.instance.GetPowerup("Firestorm", null) as Firestorm;
		yield return StartCoroutine(p.Cast(TileMaster.Tiles[2,0]));
		yield return StartCoroutine(p.Cast(TileMaster.Tiles[0,0]));
		yield return StartCoroutine(p.Cast(TileMaster.Tiles[4,0]));
		yield return new WaitForSeconds(0.1F);

		TileMaster.instance.SetFillGrid(false);
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		
		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		Destroy(p.gameObject);
		UIManager.ClassButtons.GetClass(2).ShowClass(false);
		Destroy(powerup);
	
		yield return new WaitForSeconds(Time.deltaTime * 25);
		yield return StartCoroutine(Player.instance.AddClassToSlot(2, GameData.instance.GetClass("Wizard")));

		TileMaster.Types["resource"]["Green"].ChanceInitial = 1.0F;
		TileMaster.Types["minion"]["Green"].ChanceInitial = 0.16F;

		Player.Classes[2].CanCollectMana = true;
		Player.Classes[2].CanMutate = false;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[2], 
													TileMaster.Types["resource"].Atlas, "Green",
													TileMaster.Genus.Frames, "Green"));
		TuteAlert = Alert("The Wizard has \nhigh Wisdom", 2.4F);
	 	TuteAlert.AddAction( ()=>
	 	{
	 		wis_alert = true;
	 		//Alert("Wisdom increases\nspell damage", 2.4F);
	 	});
	}

	IEnumerator AddBard()
	{
		UIManager.ClassButtons.GetClass(3).ShowClass(true);
		GameObject powerup = EffectManager.instance.PlayEffect(UIManager.ClassButtons.GetClass(3).transform, Effect.ManaPowerUp, GameData.Colour((GENUS) 3));
		powerup.transform.localScale = Vector3.one;

		Lullaby p = GameData.instance.GetPowerup("Lullaby", null) as Lullaby;
		p.SetParentOverride(UIManager.ClassButtons.GetClass(3).transform);
		for(int i = 0; i < TileMaster.Enemies.Length; i++)
		{
			p.Sleep(TileMaster.Enemies[i], 2);
			yield return new WaitForSeconds(0.1F);
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
		UIManager.ClassButtons.GetClass(3).ShowClass(false);
		Destroy(powerup);

		yield return new WaitForSeconds(Time.deltaTime * 25);
		yield return StartCoroutine(Player.instance.AddClassToSlot(3, GameData.instance.GetClass("Bard")));

		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		Player.Classes[3].CanCollectMana = true;
		Player.Classes[3].CanMutate = false;
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[3], 
													TileMaster.Types["resource"].Atlas, "Yellow",
													TileMaster.Genus.Frames, "Yellow"));
		
		
		TuteAlert = Alert("The Bard has \nhigh Charisma", 2.4F);
	 	TuteAlert.AddAction( ()=>
	 	{
	 		cha_alert = true;
	 		//Alert("Charisma increases\n tile value", 2.4F);
	 	});
	}


	protected override IEnumerator WaveEndRoutine()
	{
		OnWaveDestroy();
		if(Player.Level.Level <= 1) yield return StartCoroutine(Player.instance.AddXP(100));
		yield return null;
	}

	public override void OnWaveDestroy()
	{
		if(TuteAlert != null) Destroy(TuteAlert.gameObject);
	}
}