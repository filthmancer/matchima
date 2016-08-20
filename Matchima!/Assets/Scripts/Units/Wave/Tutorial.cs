﻿using UnityEngine;
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
		Alert("Drag through\ntiles to match");
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
			Alert("Matches must have\n3 or more tiles");
			current++;
			break;
			case 2:
			TileMaster.instance.MapSize_Default = new Vector2(3,3);
			TileMaster.Tiles[1,1].InitStats.Hits = 3;
			TileMaster.Tiles[1,1].CheckStats();
			Alert("Some tiles take\nmultiple hits to destroy");
			//TileMaster.Tiles[0,1].InitStats.Hits = 2;
			//TileMaster.Tiles[0,1].CheckStats();
			current++;
			break;
			case 3:
			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 30);

			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);
			yield return new WaitForSeconds(Time.deltaTime * 10);

			Alert("Enemy tiles attack your\nhealth after every turn");
			current++;
			break;

			case 4:
			Alert("Match enemy tiles\nto destroy");
			current++;
			break;

			case 5:
			Alert("A red X means your\nattack will kill\nthe enemy");
			TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["grunt"], GENUS.STR);
			TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["grunt"], GENUS.STR);	
			current++;
			break;
			case 6:
			Alert("Enemies have different\nattacks and health");
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
			if(dex_alert)
			{
				Alert("Dexterity increases\nattack damage", 2.4F);
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

					TileMaster.instance.ReplaceTile(0,2, TileMaster.Types["health"], GENUS.STR,1,2);
					Alert("Collect health tiles\nto regain health",2.4F);
					current++;
				}
			break;
			case 10:
			
			TileMaster.instance.ReplaceTile(1,0, TileMaster.Types["health"], GENUS.STR, 1,2);
			TileMaster.instance.ReplaceTile(2,0, TileMaster.Types["health"], GENUS.DEX, 1,2);
			TileMaster.instance.ReplaceTile(1,1, TileMaster.Types["health"], GENUS.STR, 1,2);
			TileMaster.instance.ReplaceTile(2,1, TileMaster.Types["health"], GENUS.DEX, 1,2);
			
			CameraUtility.instance.ScreenShake(0.6F, 1.4F);
			yield return new WaitForSeconds(Time.deltaTime * 30);
			Tile t = TileMaster.instance.ReplaceTile(1,2, TileMaster.Types["blob"], GENUS.STR, 2,3);
			t.AddAction(() =>
			{
				current++;
				//StartCoroutine(SplitBlobBoss());
			});

			TileMaster.Types["grunt"]["Red"].ChanceInitial = 0.25F;
			TileMaster.Types["blob"]["Blue"].ChanceInitial = 0.25F;
			TileMaster.Types.IgnoreAddedChances = false;
			Spawner2.GetSpawnables(TileMaster.Types);

			current++;
			break;
			case 12:
				Player.Classes[1].CanCollectMana = true;
				Player.Classes[0].CanCollectMana = true;
				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Collect mana to\ncast hero spells", "", true, 60));
				TileMaster.instance.MapSize_Default = new Vector2(5,5);
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
			if(wis_alert)
			{
				Alert("Wisdom increases\nspell damage", 2.4F);
				current++;
			}
			break;
			case 15:
				yield return StartCoroutine(AddBard());
				current++;
			break;
			case 16:
			if(cha_alert)
			{
				Alert("Charisma increases\ntile value", 2.4F);
				current++;
			}
			break;
			case 17:
			current++;
			TileMaster.instance.MapSize_Default = new Vector2(5,6);
			Current = Required;
			break;
		}

		if(!shownpowerupalert)
		{
			for(int i = 0; i < Player.Classes.Length; i++)
			{
				if(Player.Classes[i] == null) continue;
				if(Player.Classes[i].MeterLvl >= 1)
				{
					yield return StartCoroutine(UIManager.instance.Alert(0.3F, "A spell\nis ready!", "Tap on a powered\nhero's icon to\ncast a spell", "", true, 60));
					shownpowerupalert = true;
				}
			}
		}

		yield return null;
	}

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
		StartCoroutine(UIManager.instance.ImageQuote(0.7F, Player.Classes[0], 
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
	 	MiniAlertUI al = Alert("The Rogue has \nhigh Dexterity", 2.4F);
	 	al.AddAction( ()=>
	 	{
	 		dex_alert = true;
	 		//Alert("Dexterity increases\nattack damage", 2.4F);
	 	});
	 	
	}

	IEnumerator AddWizard()
	{
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

		yield return StartCoroutine(Player.instance.AddClassToSlot(2, GameData.instance.GetClass("Wizard")));

		TileMaster.Types["resource"]["Green"].ChanceInitial = 1.0F;
		TileMaster.Types["minion"]["Green"].ChanceInitial = 0.25F;

		Player.Classes[2].CanCollectMana = true;
		Player.Classes[2].CanMutate = false;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[2], 
													TileMaster.Types["resource"].Atlas, "Green",
													TileMaster.Genus.Frames, "Green"));
		MiniAlertUI al = Alert("The Wizard has \nhigh Wisdom", 2.4F);
	 	al.AddAction( ()=>
	 	{
	 		wis_alert = true;
	 		//Alert("Wisdom increases\nspell damage", 2.4F);
	 	});
	}

	IEnumerator AddBard()
	{
		Lullaby p = GameData.instance.GetPowerup("Lullaby", null) as Lullaby;
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

		yield return new WaitForSeconds(Time.deltaTime * 15);
		yield return StartCoroutine(Player.instance.AddClassToSlot(3, GameData.instance.GetClass("Bard")));

		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		Player.Classes[3].CanCollectMana = true;
		Player.Classes[3].CanMutate = false;
		yield return StartCoroutine(UIManager.instance.ImageQuote(1.1F, Player.Classes[3], 
													TileMaster.Types["resource"].Atlas, "Yellow",
													TileMaster.Genus.Frames, "Yellow"));
		
		
		MiniAlertUI al = Alert("The Bard has \nhigh Charisma", 2.4F);
	 	al.AddAction( ()=>
	 	{
	 		cha_alert = true;
	 		//Alert("Charisma increases\n tile value", 2.4F);
	 	});
	}


	protected override IEnumerator WaveEndRoutine()
	{
		yield return null;
	}
}