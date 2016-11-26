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
		4, 6, 7
	};

	public int [] KnifeDamage = new int[]
	{
		10, 15, 30
	};
	protected override IEnumerator Minigame(int level)
	{
		int knives = KnivesThrown[level-1];
		int power = (int) ((float)KnifeDamage[level-1] * Player.SpellPower);

		CatchNum = 0;
		yield return StartCoroutine(PowerupStartup());

		yield return StartCoroutine(Catcher());

		List<UIObj> knifeobj = new List<UIObj>();
		for(int i = 0; i < knives; i++)
		{
			knifeobj.Add(CreateKnife());
			yield return StartCoroutine(GameData.DeltaWait(0.5F));
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

		UIManager.instance.MiniAlert(UIManager.Objects.MainUI.transform.position, 
		"+" + CatchNum + " Knives!", 120, GameData.Colour(Parent.Genus), 0.6F, 0.25F);
		yield return StartCoroutine(GameData.DeltaWait(0.6F));

		UIManager.instance.ScreenAlert.SetTween(0,false);
		
		if(TileMaster.Enemies.Length == 0 || CatchNum == 0)
		{
			GameManager.instance.paused = false;
			yield break;
		}

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		int catchfinal = 0;
		for(int i = 0; i < TileMaster.Enemies.Length; i++)
		{
			Tile target = TileMaster.Enemies[i];
			if(target == null) continue;
			
			yield return StartCoroutine(ThrowKnife(target, power));
			yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
			catchfinal++;
			if(catchfinal > CatchNum) break;
			else if(i == TileMaster.Enemies.Length-1) i = -1;
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

		for(int i = 0; i < knifelist.Count; i++)
		{if(knifelist[i] != null) Destroy(knifelist[i].gameObject);}

		GameManager.instance.paused = false;
	}

	IEnumerator Catcher()
	{
		CatcherObjActual = (UIObj) Instantiate(CatcherObj);
		RectTransform rect = CatcherObjActual.GetComponent<RectTransform>();
		CatcherObjActual.transform.SetParent(UIManager.Objects.MainUI.transform);
		CatcherObjActual.transform.localScale = Vector3.one * 2.1F;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;
		

		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MainUI.transform.position, "Drag the hand\n to catch knives!", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!Input.GetMouseButton(0)) yield return null;
		m.PoolDestroy();
		StartCoroutine(CatcherLoop(CatcherObjActual));
		yield return null;
	}

	IEnumerator CatcherLoop(UIObj c_actual)
	{
		while(c_actual != null)
		{
			Vector3 point = PlayerControl.InputPos;
			point.y = c_actual.transform.position.y;
			c_actual.transform.position = Vector3.Lerp(
			c_actual.transform.position, point, Time.deltaTime * 10);
			yield return null;
		}
		
	}

	UIObj CreateKnife()
	{
		UIObj knife = CreateMinigameObj(0);
		knife.transform.position = UIManager.Objects.MainUI.transform.position + Vector3.down * 4;
		knife.transform.position += Vector3.right * (Random.value - Random.value);
		knife.transform.localScale = Vector3.one * 0.5F;
		AudioManager.instance.PlayClipOn(this.transform, "Powerup", "KnifeThrow");
		float velx = Random.Range(0.0F, 0.09F);
		if(Random.value < 0.5F) velx = -velx;
		Vector3 vel = new Vector3(velx, 1.0F, 0.0F);
		bool mobile = Application.isMobilePlatform;
		knife.GetComponent<Velocitizer>().SetVelocity(vel, 50 * CameraUtility.OrthoFactor);
		knife.GetComponent<Velocitizer>().SetRotation(new Vector3(0,0,Random.Range(-1.2F, 1.2F)));
		knife.GetComponent<Velocitizer>().AddTimedAction(() =>
		{
			float d = Vector3.Distance(knife.transform.position, CatcherObjActual.transform.position);
			if(d <= CatcherDist)
			{
				CatchNum ++;
				UIManager.instance.MiniAlert(knife.transform.position, 
				"Caught!", 100, GameData.Colour(Parent.Genus), 0.4F, 0.1F);
				AudioManager.instance.PlayClipOn(this.transform, "Powerup", "KnifeCatch");
				Destroy(knife.gameObject);
			} 
		}, TimerType.PostTimer, 0.2F);

		return knife;
	}

	public List<UIObj> knifelist = new List<UIObj>();
	public IEnumerator ThrowKnife(Tile target, int power)
	{
		Transform par;
		Color classcol = Color.white;
		if(Parent) 
		{
			par = Parent._Tile.transform;
			classcol = GameData.Colour(Parent.Genus);
		}
		else if(ParentOverride) par = ParentOverride;
		else par = UIManager.instance.Health.transform;

		target.SetState(TileState.Selected, true);
		UIObj part = CreateMinigameObj(0);
		part.transform.position = par.position;
		part.transform.localScale = Vector3.one * 0.3F;
		AudioManager.instance.PlayClipOn(this.transform, "Powerup", "KnifeThrow");
		
		part.GetComponent<Velocitizer>().enabled = false;
		MoveToPoint mp = part.GetComponent<MoveToPoint>();

		knifelist.Add(part);

		mp.enabled = true;
		mp.SetTarget(target.transform.position);
		mp.SetPath(30.0F, 0.0F);
		mp.SetThreshold(0.1F);

		float part_time = 0.2F;
		int final_damage = power;
		bool add = true;
		mp.SetTileMethod(target, (Tile child) =>
		{
			child.PlayAudio("attack");
			child.SetState(TileState.Selected, true);
			child.InitStats._Hits.Add(-final_damage);
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
			MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + final_damage, info_start_size, classcol, info_time, 0.6F, false);
			m.SetToDamageIndicator();
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
