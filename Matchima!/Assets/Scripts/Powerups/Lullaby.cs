using UnityEngine;
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

	protected override IEnumerator Minigame(int level)
	{
		int notes_played = NotesPlayed[level-1];
		int sleep_ratio = SleepRatio[level-1];
		notes_hit = 0;
		yield return StartCoroutine(PowerupStartup());

		Harp = (UIObj) Instantiate(HarpObj);
		RectTransform rect = Harp.GetComponent<RectTransform>();
		Harp.transform.SetParent(UIManager.Objects.MainUI.transform);
		Harp.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;

		UIObj n = CreateNote();
		n.transform.position = UIManager.Objects.MiddleGear.transform.position + Vector3.down;
		n.transform.localScale *= 2.0F;
		n.GetComponent<MoveToPoint>().enabled = false;

		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up/2, "Tap the notes\nto play!", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!Input.GetMouseButton(0)) yield return null;

		m.PoolDestroy();
		Destroy(n.gameObject);

		int sleep_duration = 0; //Duration of mass sleep

		List<UIObj> notes = new List<UIObj>();
		for(int i = 0; i < notes_played; i++)
		{
			float time = Random.Range(0.1F, 0.6F);
			notes.Add(CreateNote());
			//alert.transform.position = UIManager.Objects.TopGear.transform.position + Vector3.down * 3;
			//alert.text = notes_hit/sleep_ratio + " TURN SLEEP";
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

		sleep_duration = notes_hit/ sleep_ratio;

		MiniAlertUI alert  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up * 2.0F,
			sleep_duration + " Turn Sleep!", 120, GameData.Colour(Parent.Genus), 1.0F, 0.2F);
		alert.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.7F));
		alert.PoolDestroy();

		UIManager.instance.ScreenAlert.SetTween(0,false);
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);

		
		Tile [] targets  = TileMaster.Enemies;
		for(int i = 0; i < targets.Length; i++)
		{
			if(targets[i].HasEffect("Charm") || targets[i].Stats.isAlly) CharmAndValue(targets[i], sleep_duration);
			else Sleep(targets[i], sleep_duration);
			yield return new WaitForSeconds(GameData.GameSpeed(0.11F));
		}

		yield return new WaitForSeconds(0.3F);
		GameManager.instance.paused = false;
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		TileMaster.instance.ResetTiles(true);
	}

	UIObj CreateNote()
	{
		int line = Random.Range(0, 4);
		UIObj note = CreateMinigameObj(0);
		note.transform.localScale = Vector3.one * 0.1F;
		note.transform.position = Harp[line].transform.position;
		note.Img[0].color = GameData.Colour((GENUS)line);
		note.AddAction(UIAction.MouseDown, () =>
		{
			notes_hit ++;
			MiniAlertUI alert  = UIManager.instance.MiniAlert(note.transform.position,
			"Good!", 100, GameData.Colour(Parent.Genus), 0.3F, 0.2F);
			alert.AddJuice(Juice.instance.BounceB, 0.1F);

			Destroy(note.gameObject);

		});
		MoveToPoint mp = note.GetComponent<MoveToPoint>();
		mp.enabled = true;
		mp.SetTarget(Harp.Img[line].transform.position);
		mp.SetPath(0.2F * CameraUtility.OrthoFactor, 0.0F, 0.0F);
		mp.SetScale(0.5F, 0.4F);

		return note;
	}

	public void Sleep(Tile target, int duration)
	{
		Transform par = UIManager.instance.Health.transform;
		if(Parent != null) par = UIManager.ClassButtons[Parent.Index].transform;

		target.SetState(TileState.Selected, true);
		GameObject initpart = EffectManager.instance.PlayEffect(par, "spell");
		MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
		charm.enabled = true;
		charm.SetTarget(target.transform.position);
		charm.SetPath(0.45F, 0.3F);
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
		if(Parent != null) par = UIManager.ClassButtons[Parent.Index].transform;

		target.SetState(TileState.Selected, true);
		GameObject initpart = EffectManager.instance.PlayEffect(par, "spell");
		MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
		charm.enabled = true;
		charm.SetTarget(target.transform.position);
		charm.SetPath(0.45F, 0.3F);
		charm.Target_Tile = target;
		charm.SetThreshold(0.15F);
		charm.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(charm.Target_Tile.Point.targetPos, "Charm", 85, GameData.Colour(charm.Target_Tile.Genus), 1.2F, 0.1F);
			charm.Target_Tile.AddEffect("Charm", duration);
			charm.Target_Tile.InitStats.Hits += hpinc;
			charm.Target_Tile.InitStats.Attack += atkinc;
			charm.Target_Tile.CheckStats();
		});
	}

}
