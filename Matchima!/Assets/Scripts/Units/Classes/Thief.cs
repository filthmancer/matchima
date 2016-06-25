﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Thief : Class {

	TileChance attack;
	private Slot manapower;
	private int _currentmanapower = 100;
	public override void StartClass () {
		
		attack = new TileChance();
		attack.Genus = "Alpha";
		attack.Type = "sword";
		attack.Chance = 0.2F;
		InitStats.TileChances.Add(attack);

		TileChance bomb = new TileChance();
		bomb.Genus = GameData.ResourceLong(Genus);
		bomb.Type = "arcane";
		bomb.Chance = 0.15F;
		InitStats.TileChances.Add(bomb);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.05F;
		InitStats.TileChances.Add(health);

	/*	ClassUpgrade b = new ClassUpgrade((int val) => {attack.Chance += 0.02F * val;});
		b.BaseAmount = 2;
		b.Name = "Sword Tiles";
		b.ShortName = "SWRD";
		b.Description = " chance of Sword Tiles";
		b.Prefix = "+";
		b.Suffix = "%";	
		b.Rarity = Rarity.Uncommon;
		AddUpgrades(new ClassUpgrade[] {b});*/

		Barbarian barb = null;
		foreach(Class child in Player.Classes) {if(child is Barbarian) barb = child as Barbarian;}
		if(barb != null)
		{
			QuoteGroup barb_disgust = new QuoteGroup("Thief-Barb: Disgust");
			barb_disgust.AddQuote("Keep that brute away from me!", this, false, 2.5F);
			barb_disgust.AddQuote("You watch your tongue, shadow girl.", barb, false, 2.5F);
			//barb_disgust.Unlocked = () => {foreach(Class child in Player.Classes){if(child.Name == "Barbarian") return true;} return false;};
			Quotes.StartQuotes.Add(barb_disgust);
		}
		base.StartClass();
	}


	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = 1;//Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["force"], g, 1, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["bomb"], g, 1, points);	
			break;
			case 2:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["sword"], GENUS.ALL, 1, points);	
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["arcane"], g, 1, points);
				(TileMaster.Tiles[x,y] as Arcane).SetArgs("Genus", "", "", "Sword");
			break;
			case 4:

			break;
		}
		
	}

	public override IEnumerator UseManaPower()
	{
		if(manapower != null)
		{
			Destroy(manapower.gameObject);
			AllMods.Remove(manapower);
			//_currentmanapower = lvl;
		}
		switch(MeterLvl)
		{
			case 1:
				yield return StartCoroutine(ActiveRoutine(2));
				LevelUp();
				yield return StartCoroutine(PowerDown());
			break;
			case 2:
				yield return StartCoroutine(ActiveRoutine(4));
				LevelUp();
				yield return StartCoroutine(PowerDown());
			break;
			case 3:
				yield return StartCoroutine(ActiveRoutine(7));
				LevelUp();
				yield return StartCoroutine(PowerDown());
			break;
		}
		yield return null;
	}

	public UIObj CatcherObj;
	public float CatcherDist = 0.9F;
	public int CatchNum = 0;

	private UIObj CatcherObjActual;
	List<Tile> to_collect = new List<Tile>();
	IEnumerator ActiveRoutine(int knives)
	{
		activated = true;
		CatchNum = 0;
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Genus));
		powerup.transform.SetParent(UIManager.ClassButtons.GetClass(Index).transform);
		powerup.transform.position = UIManager.ClassButtons.GetClass(Index).transform.position;
		powerup.transform.localScale = Vector3.one;

		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			"Rogue Casts", 70, GameData.Colour(Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Knife Throw", 170, GameData.Colour(Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 2,
			"Catch the\nknives!", 140, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
		Destroy(powerup);
		

		StartCoroutine(Catcher());

		List<UIObj> knifeobj = new List<UIObj>();
		for(int i = 0; i < knives; i++)
		{
			knifeobj.Add(CreateKnife());
			yield return new WaitForSeconds(GameData.GameSpeed(0.7F));
		}

		bool knives_ended = false;
		while(!knives_ended)
		{
			Vector3 point = PlayerControl.InputPos;
			point.y = CatcherObjActual.transform.position.y;
			CatcherObjActual.transform.position = Vector3.Lerp(
			CatcherObjActual.transform.position, point, Time.deltaTime * 10);
			knives_ended = true;
			for(int i = 0; i < knifeobj.Count; i++)
			{
				if(knifeobj[i] != null) knives_ended = false;
			}
			yield return null;
		}

		Destroy(CatcherObjActual.gameObject);

		UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, 
		CatchNum + " Knives!", 100, GameData.Colour(Genus), 0.8F, 0.25F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.4F));

		
		UIManager.instance.ScreenAlert.SetTween(0,false);
		Tile[] targets = TileMaster.Enemies;
		if(targets.Length == 0 || CatchNum == 0)
		{
			GameManager.instance.paused = false;
			yield break;
		}

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		to_collect = new List<Tile>();
		for(int i = 0; i < CatchNum; i++)
		{
			Tile target = targets[Random.Range(0, targets.Length)];
			yield return StartCoroutine(ThrowKnife(target));
			yield return new WaitForSeconds(GameData.GameSpeed(0.35F));
		}

		TileMaster.instance.SetFillGrid(false);
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		TileMaster.instance.SetFillGrid(true);
		TileMaster.instance.ResetTiles(true);
		GameManager.instance.paused = false;
	}

	IEnumerator Catcher()
	{
		CatcherObjActual = (UIObj) Instantiate(CatcherObj);
		RectTransform rect = CatcherObjActual.GetComponent<RectTransform>();
		CatcherObjActual.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		CatcherObjActual.transform.localScale = Vector3.one * 1.5F;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;

		while(CatcherObjActual != null)
		{
			Vector3 point = PlayerControl.InputPos;
			point.y = CatcherObjActual.transform.position.y;
			CatcherObjActual.transform.position = Vector3.Lerp(
			CatcherObjActual.transform.position, point, Time.deltaTime * 10);
			yield return null;
		}
	}

	UIObj CreateKnife()
	{
		UIObj knife = CreateMinigameObj();
		knife.transform.position = UIManager.Objects.BotGear.transform.position;
		float velx = Random.Range(0.09F, 0.17F);
		if(Random.value < 0.5F) velx = -velx;
		Vector3 vel = new Vector3(velx, 2F, 0.0F);
		knife.GetComponent<Velocitizer>().SetVelocity(vel, 0.3F);
		knife.GetComponent<Velocitizer>().SetRotation(new Vector3(0,0,Random.Range(-1.5F, 1.5F)));
		knife.GetComponent<Velocitizer>().AddTimedAction(() =>
		{
			float d = Vector3.Distance(knife.transform.position, CatcherObjActual.transform.position);
			if(d <= CatcherDist)
			{
				CatchNum ++;
				UIManager.instance.MiniAlert(knife.transform.position, 
				"Caught!", 100, GameData.Colour(Genus), 0.4F, 0.1F);
				Destroy(knife.gameObject);
			} 
			
			
		}, TimerType.PostTimer, 0.2F);

		return knife;
	}

	IEnumerator ThrowKnife(Tile target)
	{
		target.SetState(TileState.Selected, true);
		UIObj part = CreateMinigameObj();
		part.transform.position = this.transform.position;
		part.transform.localScale *= 0.7F;
		part.GetComponent<Velocitizer>().enabled = false;
		MoveToPoint mp = part.GetComponent<MoveToPoint>();
		mp.enabled = true;
		mp.SetTarget(target.transform.position);
		mp.SetPath(0.55F, 0.0F);

		float dist = Vector3.Distance(target.transform.position, this.transform.position);
		mp.Speed = 0.1F + 0.05F * dist;
		float part_time = 0.2F;// + (0.03F * dist);
		int final_damage = 50;
		bool add = true;
		mp.SetTileMethod(target, (Tile child) =>
		{
			child.SetState(TileState.Selected, true);
			child.InitStats.Hits -= final_damage;
			foreach(Tile alreadycollected in to_collect)
			{
				if(alreadycollected == child) add = false;
			}
			if(add) to_collect.Add(child);

			float init_rotation = Random.Range(-3,3);
			float info_time = 0.4F;
			float info_start_size = 100 + (final_damage);
			float info_movespeed = 0.25F;
			float info_finalscale = 0.65F;

			Vector3 pos = TileMaster.Grid.GetPoint(child.Point.Point(0)) + Vector3.down * 0.3F;
			MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + final_damage, info_start_size, GameData.Colour(Genus), info_time, 0.6F, false);
			m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
			m.SetVelocity(Utility.RandomVectorInclusive(0.2F) + (Vector3.up*0.4F));
			m.Gravity = true;
			m.AddJuice(Juice.instance.BounceB, info_time/0.8F);

			CameraUtility.instance.ScreenShake(0.26F + 0.02F * final_damage,  GameData.GameSpeed(0.06F));
			EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
			
		});
		yield return null;
	}

}
