﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lullaby : Powerup {

	public UIObj HarpObj;
	UIObj Harp;

	int notes_hit = 0;

	public int [] NotesPlayed = new int[]{
		6,12,14
	};
	public int [] SleepRatio = new int[]{
		3,3,2
	};
	public float [] Speed = new float []
	{
		7, 9, 12
	};

	protected override IEnumerator Minigame(int level)
	{
		int notes_played = NotesPlayed[level-1];
		int sleep_ratio = SleepRatio[level-1];
		notes_hit = 0;
		yield return StartCoroutine(PowerupStartup());

		Harp = (UIObj) Instantiate(HarpObj);
		RectTransform rect = Harp.GetComponent<RectTransform>();
		Harp.transform.SetParent(UIManager.Objects.MainUI.transform);
		Harp.transform.localScale = Vector3.one * 2.1F;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;

		UIObj n = CreateNote(0);
		n.transform.position = UIManager.Objects.MiddleGear.transform.position + Vector3.down;
		n.transform.localScale *= 2.0F;
		n.GetComponent<MoveToPoint>().enabled = false;

		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up/2, "Tap the notes\nto play!", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!Input.GetMouseButton(0)) yield return null;

		m.PoolDestroy();
		if(n) Destroy(n.gameObject);

		int sleep_duration = 0; //Duration of mass sleep

		List<UIObj> notes = new List<UIObj>();
		for(int i = 0; i < notes_played; i++)
		{
			float time = Random.Range(0.1F, 0.6F);
			notes.Add(CreateNote(level-1));
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
		

		Destroy(Harp.gameObject);
		yield return new WaitForSeconds(GameData.GameSpeed(0.15F));

		sleep_duration = Mathf.Clamp(notes_hit/ sleep_ratio, 1, 100);

		MiniAlertUI alert  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up * 2.0F,
			sleep_duration + " Turn Sleep!", 120, GameData.Colour(Parent.Genus), 1.2F, 0.2F);
		alert.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.7F));
		alert.PoolDestroy();

		UIManager.instance.ScreenAlert.SetTween(0,false);
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		//UIManager.CrewButtons[Parent.Index].ShowClass(true);

		Tile [] targets  = TileMaster.Enemies;
		for(int i = 0; i < targets.Length; i++)
		{
			Sleep(targets[i], sleep_duration);
			yield return new WaitForSeconds(GameData.GameSpeed(0.11F));
		}

		targets  = TileMaster.Allies;
		for(int i = 0; i < targets.Length; i++)
		{
			CharmAndValue(targets[i], sleep_duration);
			yield return new WaitForSeconds(GameData.GameSpeed(0.11F));
		}

		yield return new WaitForSeconds(0.3F);
		GameManager.instance.paused = false;
		//UIManager.CrewButtons[Parent.Index].ShowClass(false);
		TileMaster.instance.ResetTiles(true);
	}

	UIObj CreateNote(int Lvl)
	{
		int line = Random.Range(0, 4);
		UIObj note = CreateMinigameObj(0);
		note.transform.localScale = Vector3.one * 0.1F;
		note.transform.position = Harp[line].transform.position;
		note.Img[0].color = GameData.Colour((GENUS)line);
		note.AddAction(UIAction.MouseDown, () =>
		{
			AudioSource c = AudioManager.instance.PlayClipOn(this.transform, "Powerup", "HarpNote");
			if(c) c.pitch += (float)line*0.5F;
			notes_hit ++;
			MiniAlertUI alert  = UIManager.instance.MiniAlert(note.transform.position,
			"Good!", 100, GameData.Colour(Parent.Genus), 0.3F, 0.2F);
			alert.AddJuice(Juice.instance.BounceB, 0.1F);

			Destroy(note.gameObject);

		});
		MoveToPoint mp = note.GetComponent<MoveToPoint>();
		mp.enabled = true;
		mp.SetTarget(Harp.Img[line].transform.position);
		mp.SetPath(Speed[Lvl] * CameraUtility.OrthoFactor, 0.0F, 0.0F);
		mp.SetScale(0.3F, 0.2F);

		return note;
	}

	public void Sleep(Tile target, int duration)
	{
		Transform par;
		if(Parent) par = UIManager.CrewButtons[Parent.Index].transform;
		else if(ParentOverride) par = ParentOverride;
		else par = UIManager.instance.Health.transform;

		target.SetState(TileState.Selected, true);
		GameObject initpart = EffectManager.instance.PlayEffect(par, "spell");
		MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
		charm.enabled = true;
		charm.SetTarget(target.transform.position);
		charm.SetPath(25.0F, 0.3F);
		charm.Target_Tile = target;
		charm.SetThreshold(0.15F);
		charm.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(charm.Target_Tile.Point.targetPos, "Sleep", 85, GameData.Colour(charm.Target_Tile.Genus), 1.2F, 0.1F);
			charm.Target_Tile.AddEffect("Sleep", duration);
		});
	}

	void CharmAndValue(Tile target, int duration, int hpinc = 1, int atkinc = 3)
	{
		Transform par = UIManager.instance.Health.transform;
		if(Parent != null) par = UIManager.CrewButtons[Parent.Index].transform;

		target.SetState(TileState.Selected, true);
		GameObject initpart = EffectManager.instance.PlayEffect(par, "spell");
		MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
		charm.enabled = true;
		charm.SetTarget(target.transform.position);
		charm.SetPath(25.0F, 0.3F);
		charm.Target_Tile = target;
		charm.SetThreshold(0.15F);
		charm.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(charm.Target_Tile.Point.targetPos, "Charm", 85, GameData.Colour(charm.Target_Tile.Genus), 1.2F, 0.1F);
			charm.Target_Tile.AddEffect("Charm", duration);
			charm.Target_Tile.InitStats._Hits.Max += hpinc;
			charm.Target_Tile.InitStats._Attack.Max += atkinc;
			charm.Target_Tile.CheckStats();
		});
	}

}
