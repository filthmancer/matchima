using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThrowKnives : Powerup {

	public UIObj CatcherObj;
	public float CatcherDist = 0.9F;
	public int CatchNum = 0;

	private UIObj CatcherObjActual;
	List<Tile> to_collect = new List<Tile>();

	public int [] KnivesThrown = new int []
	{
		5, 8, 9
	};

	public int [] KnifeDamage = new int[]
	{
		10, 15, 30
	};
	protected override IEnumerator Minigame(int level)
	{
		int knives = KnivesThrown[level-1];
		int power = KnifeDamage[level-1];

		CatchNum = 0;
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Parent.Genus));
		powerup.transform.SetParent(UIManager.ClassButtons.GetClass(Parent.Index).transform);
		powerup.transform.position = UIManager.ClassButtons.GetClass(Parent.Index).transform.position;
		powerup.transform.localScale = Vector3.one;

		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			"Rogue Casts", 70, GameData.Colour(Parent.Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Knife Throw", 170, GameData.Colour(Parent.Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 2,
			"Catch the\nknives!", 140, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
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
		CatchNum + " Knives!", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.4F));

		
		UIManager.instance.ScreenAlert.SetTween(0,false);
		to_collect.AddRange(TileMaster.Enemies);
		if(to_collect.Count == 0 || CatchNum == 0)
		{
			GameManager.instance.paused = false;
			yield break;
		}

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		for(int i = 0; i < CatchNum; i++)
		{
			int num = Random.Range(0, to_collect.Count);
			Tile target = to_collect[num];

			if(to_collect.Count > 1) to_collect.RemoveAt(num);

			yield return StartCoroutine(ThrowKnife(target, power));
			yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		}

		TileMaster.instance.SetFillGrid(false);
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		TileMaster.instance.SetFillGrid(true);
		TileMaster.instance.ResetTiles(true);
		yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());

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
		UIObj knife = CreateMinigameObj(0);
		knife.transform.position = UIManager.Objects.BotGear.transform.position;
		float velx = Random.Range(0.09F, 0.17F);
		if(Random.value < 0.5F) velx = -velx;
		Vector3 vel = new Vector3(velx, 2.0F * TileMaster.YScale, 0.0F);
		knife.GetComponent<Velocitizer>().SetVelocity(vel, 30);
		knife.GetComponent<Velocitizer>().SetRotation(new Vector3(0,0,Random.Range(-1.2F, 1.2F)));
		knife.GetComponent<Velocitizer>().AddTimedAction(() =>
		{
			float d = Vector3.Distance(knife.transform.position, CatcherObjActual.transform.position);
			if(d <= CatcherDist)
			{
				CatchNum ++;
				UIManager.instance.MiniAlert(knife.transform.position, 
				"Caught!", 100, GameData.Colour(Parent.Genus), 0.4F, 0.1F);
				Destroy(knife.gameObject);
			} 
		}, TimerType.PostTimer, 0.2F);

		return knife;
	}

	IEnumerator ThrowKnife(Tile target, int power)
	{
		target.SetState(TileState.Selected, true);
		UIObj part = CreateMinigameObj(0);
		part.transform.position = UIManager.ClassButtons.GetClass(Parent.Index).transform.position;
		part.transform.localScale *= 0.7F;
		part.GetComponent<Velocitizer>().enabled = false;
		MoveToPoint mp = part.GetComponent<MoveToPoint>();
		mp.enabled = true;
		mp.SetTarget(target.transform.position);
		mp.SetPath(GameData.GameSpeed(0.55F), 0.0F);

		float dist = Vector3.Distance(target.transform.position, UIManager.ClassButtons.GetClass(Parent.Index).transform.position);
		//mp.Speed = 0.1F + 0.05F * dist;
		float part_time = 0.2F;// + (0.03F * dist);
		int final_damage = power;
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
			MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + final_damage, info_start_size, GameData.Colour(Parent.Genus), info_time, 0.6F, false);
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
