using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bard : Class {

	TileChance harp;

	bool warcry_a, warcry_b, warcry_c;
	private int _currentmanapower;

	public override StCon [] _Desc
	{
		get
		{
			List<StCon> final = new List<StCon>();
			final.Add(new StCon(Meter + "/" + MeterTop + " Mana", GameData.Colour(Genus)));
			final.Add(new StCon("Lvl " + Level + " ("+ Exp_Current + "/" + Exp_Max + " xp)", GameData.Colour(GENUS.WIS)));

			for(int i = 0; i < Stats.Length; i++)
			{
				bool last = i==Stats.Length-1;
				final.Add(new StCon(Stats[i].StatCurrent+"", GameData.Colour((GENUS)i), last));
				if(!last) final.Add(new StCon(" /", Color.white, false));
			}
			
			if(warcry_a) final.Add(new StCon("Applies Sleep to 1 Enemy Per Turn", Color.white));
			if(warcry_b) final.Add(new StCon("Applies Charm to 1 Enemy Per Turn", Color.white));
			if(warcry_c) final.Add(new StCon("Applies Sleep to 1 Enemy Per Turn", Color.white));
			foreach(Slot child in AllMods)
			{
				if(child != null) final.AddRange(child.Description_Tooltip);
			}
			
			foreach(ClassEffect child in _Status)
			{
				final.AddRange(child.Description);
			}
				
			return final.ToArray();
		}
	}
	public override void StartClass () {

		harp = new TileChance();
		harp.Genus = GameData.ResourceLong(Genus);
		harp.Type = "harp";
		harp.Chance = 0.15F;
		InitStats.TileChances.Add(harp);



		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.15F;
		InitStats.TileChances.Add(health);

		base.StartClass();
}
	//public override IEnumerator EndTurn()
	//{
	//	//if(warcry_a) yield return StartCoroutine(WarcryA());
	//	//if(warcry_b) yield return StartCoroutine(WarcryB());
	//	//if(warcry_c) yield return StartCoroutine(WarcryC());
	//	yield return StartCoroutine(base.EndTurn());
	//}


	public override IEnumerator UseManaPower()
	{
		switch(MeterLvl)
		{
			case 1:
			yield return StartCoroutine(ActiveRoutine(6, 3));
			LevelUp();
			yield return StartCoroutine(PowerDown());
				//warcry_a = true;
				//warcry_b = false;
				//warcry_c = false;
			break;
			case 2:
			yield return StartCoroutine(ActiveRoutine(12,3));
			LevelUp();
			yield return StartCoroutine(PowerDown());
				//warcry_a = false;
				//warcry_b = true;
				//warcry_c = false;
			break;
			case 3:
			yield return StartCoroutine(ActiveRoutine(14, 2));
			LevelUp();
			yield return StartCoroutine(PowerDown());
				//warcry_a = false;
				//warcry_b = false;
				//warcry_c = true;
			break;
		}
	}

	public UIObj HarpObj;
	UIObj Harp;

	int notes_hit = 0;

	IEnumerator ActiveRoutine(int notes_played, int sleep_ratio)
	{
		notes_hit = 0;
		activated = true;
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
			"Bard Casts", 70, GameData.Colour(Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Lullaby", 170, GameData.Colour(Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 2,
			"Play the\nnotes!", 140, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
		Destroy(powerup);


		Harp = (UIObj) Instantiate(HarpObj);
		RectTransform rect = Harp.GetComponent<RectTransform>();
		Harp.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		Harp.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;

		yield return new WaitForSeconds(Time.deltaTime * 15);

		MiniAlertUI alert  = UIManager.instance.MiniAlert(UIManager.Objects.BotGear.transform.position + Vector3.down * 1,
			"Play the\nnotes!", 100, GameData.Colour(Genus), 7.0F, 0.2F);
		alert.AddJuice(Juice.instance.BounceB, 0.1F);

		int sleep_duration = 0; //Duration of mass sleep

		List<UIObj> notes = new List<UIObj>();
		for(int i = 0; i < notes_played; i++)
		{
			float time = Random.Range(0.1F, 0.6F);
			notes.Add(CreateNote());
			alert.text = notes_hit/sleep_ratio + " TURN SLEEP";
			yield return new WaitForSeconds(GameData.GameSpeed(time));
		}

		bool isplaying = true;
		while(isplaying)
		{
			isplaying = false;
			for(int i = 0; i < notes.Count; i++)
			{
				if(notes[i] != null) isplaying = true;
			}
			yield return null;
		}
		yield return new WaitForSeconds(GameData.GameSpeed(0.15F));
		if(alert != null) Destroy(alert.gameObject);
		Destroy(Harp.gameObject);
		yield return new WaitForSeconds(GameData.GameSpeed(0.15F));
		UIManager.instance.ScreenAlert.SetTween(0,false);
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);

		sleep_duration = notes_hit/ sleep_ratio;
		Tile [] targets  = TileMaster.Enemies;
		for(int i = 0; i < targets.Length; i++)
		{
			Sleep(targets[i], sleep_duration);
			yield return new WaitForSeconds(GameData.GameSpeed(0.35F));
		}

		yield return new WaitForSeconds(0.4F);
		GameManager.instance.paused = false;
		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
		TileMaster.instance.ResetTiles(true);
	}

	UIObj CreateNote()
	{
		int line = Random.Range(0, 4);
		UIObj note = CreateMinigameObj();
		note.transform.localScale = Vector3.one * 0.1F;
		note.transform.position = Harp[line].transform.position;
		note.AddAction(UIAction.MouseDown, () =>
		{
			notes_hit ++;
			Destroy(note.gameObject);
		});
		MoveToPoint mp = note.GetComponent<MoveToPoint>();
		mp.enabled = true;
		mp.SetTarget(Harp.Img[line].transform.position);
		mp.SetPath(0.2F, 0.0F, 0.0F);
		mp.SetScale(0.5F, 0.4F);

		return note;
	}

	void Sleep(Tile target, int duration)
	{
		
		target.SetState(TileState.Selected, true);
		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Genus].transform, Effect.Force);
		MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
		charm.SetTarget(target.transform.position);
		charm.SetPath(0.25F, 0.3F);
		charm.Target_Tile = target;
		charm.SetThreshold(0.15F);
		charm.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(charm.Target_Tile.Point.targetPos, "Sleep", 85, GameData.Colour(charm.Target_Tile.Genus), 1.2F, 0.1F);
			charm.Target_Tile.AddEffect("Sleep", duration);
		});
	}

	IEnumerator WarcryB()
	{
		List<Tile> targets = new List<Tile>();
		for(int x = 0; x < TileMaster.Grid.Size[0];x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				Tile t = TileMaster.Tiles[x,y];
				if(t == null) continue;
				if(t.Type.isEnemy && !t.Stats.isAlly)
				{
					targets.Add(t);
				}
			}
		}

		if(targets.Count == 0) yield break;
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);
		UIManager.instance.MiniAlert(UIManager.ClassButtons.GetClass(Index).transform.position + Vector3.up, 
													"Charm", 55, GameData.Colour(Genus), 1.2F, 0.25F);

		Tile sleeptarget = targets[Random.Range(0, targets.Count)];
		if(sleeptarget == null) yield break;
		GameObject initsleep = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Genus].transform, Effect.Force);
		MoveToPoint sleep = initsleep.GetComponent<MoveToPoint>();
		sleep.SetTarget(sleeptarget.transform.position);
		sleep.SetPath(0.35F, 0.3F);
		sleep.Target_Tile = sleeptarget;
		sleep.SetThreshold(0.15F);
		sleep.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(sleep.Target_Tile.Point.targetPos, "Charm", 55, GameData.Colour(sleep.Target_Tile.Genus), 1.2F, 0.1F);
			sleep.Target_Tile.AddEffect("Charm", 3);
			
		});
		yield return new WaitForSeconds(0.3F);
	}


	IEnumerator WarcryC()
	{
		List<Tile> targets = new List<Tile>();
		for(int x = 0; x < TileMaster.Grid.Size[0];x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				Tile t = TileMaster.Tiles[x,y];
				if(t == null) continue;
				if(t.Type.isEnemy && !t.Stats.isAlly)
				{
					targets.Add(t);
				}
			}
		}
		
		if(targets.Count == 0) yield break;
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);
			Tile sleeptarget = targets[Random.Range(0, targets.Count)];
			if(sleeptarget == null) yield break;
			GameObject initsleep = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Genus].transform, Effect.Force);
			MoveToPoint sleep = initsleep.GetComponent<MoveToPoint>();
			sleep.SetTarget(sleeptarget.transform.position);
			sleep.SetPath(0.35F, 0.3F);
			sleep.Target_Tile = sleeptarget;
			sleep.SetThreshold(0.15F);
			sleep.SetMethod(() =>
			{
				MiniAlertUI m = UIManager.instance.MiniAlert(sleep.Target_Tile.Point.targetPos, "Sleep", 55, GameData.Colour(sleep.Target_Tile.Genus), 1.2F, 0.1F);
				sleep.Target_Tile.AddEffect("Sleep", 3);
			});
		yield return new WaitForSeconds(0.3F);
	}
}
