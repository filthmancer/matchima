using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Firestorm : Powerup {

	public int [] _Lines = new int []
	{
		1,2,5
	};

	int ClosestPoint(Vector3 pos)
	{
		float closest_dist = 100.0F;
		int closest = 100;
		for(int i = 0; i < TileMaster.Grid.Size[0]; i++)
		{
			float d = Vector3.Distance(TileMaster.Tiles[i,0].transform.position, pos);
			if(d < closest_dist)
			{
				closest = i;
				closest_dist = d;
			}
		}
		return closest;
	}

	int? target_column = null;
	protected override IEnumerator Minigame(int level)
	{
		int lines = _Lines[level-1];
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
			"Wizard Casts", 70, GameData.Colour(Parent.Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Firestorm", 170, GameData.Colour(Parent.Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 2,
			"Press to cast!", 140, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		Destroy(powerup);

		target_column = null;
		int currtile = 0;
		int nexttile = 1;
		float taptimer = 3.0F;
		int nexttile_acc = 1;

		UIObj [] MGame = new UIObj[lines];
		int [] MGame_target = new int[lines];
		float [] MGame_vel = new float[lines];
		for(int i = 0; i < MGame.Length; i++)
		{
			MGame[i] = CreateTarget(TileMaster.Grid.Size[0]/2);
			MGame_vel[i] = Random.Range(0.03F, 0.09F * lines);
			if(Random.value > 0.5F) MGame_vel[i] = -MGame_vel[i];
			yield return null;
		}

		while(!Input.GetMouseButtonDown(0))
		{
			for(int i = 0; i < MGame.Length; i++)
			{
				MGame[i].transform.position += Vector3.right * MGame_vel[i];
				MGame_target[i] = ClosestPoint(MGame[i].transform.position);
				if(Mathf.Abs(MGame_vel[i]) < 0.45F) MGame_vel[i] *= 1.008F;
				if(MGame[i].transform.position.x > TileMaster.Tiles[TileMaster.Grid.Size[0]-1,0].transform.position.x) MGame_vel[i] = -MGame_vel[i];
				else if(MGame[i].transform.position.x < TileMaster.Tiles[0,0].transform.position.x) MGame_vel[i] = -MGame_vel[i];
			}
			yield return null;
		}

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		for(int i = 0; i < lines;i++)
		{
			Destroy(MGame[i].gameObject);
			yield return Cast(TileMaster.Tiles[MGame_target[i],0]);
		}
		
		TileMaster.instance.SetFillGrid(false);
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		yield return new WaitForSeconds(Time.deltaTime * 10);
		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());
		yield return new WaitForSeconds(GameData.GameSpeed(0.4F));

		GameManager.instance.paused = false;
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
	}

	UIObj CreateTarget(int i)
	{
		UIObj obj = (UIObj)Instantiate(MinigameObj[0]);
		RectTransform rect = obj.GetComponent<RectTransform>();
		obj.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		obj.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;
		rect.transform.position = TileMaster.Tiles[i,0].transform.position;
		return obj;
	}

	int Damage = 20;
	IEnumerator Cast(Tile target)
	{
		int targX = target.Point.Base[0];
		int targY = target.Point.Base[1];

		float particle_time = 0.7F;

		if(target == null) yield break;

		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<GameObject> particles = new List<GameObject>();

		for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
		{
			Tile tile = _tiles[targX,y];
			tile.SetState(TileState.Selected, true);
			to_collect.Add(tile);

			GameObject new_part = EffectManager.instance.PlayEffect(tile.transform, Effect.Fire);
			particles.Add(new_part);
			yield return new WaitForSeconds(GameData.GameSpeed(0.025F));

			tile = _tiles[targX,y];
			tile.SetState(TileState.Selected, true);
			to_collect.Add(tile);

			new_part = EffectManager.instance.PlayEffect(tile.transform, Effect.Fire);
			particles.Add(new_part);			
		}
		
		yield return new WaitForSeconds(Time.deltaTime * 10);

		for(int i = 0; i < to_collect.Count; i++)
		{
			if(to_collect[i].Type.isEnemy)
			{
				to_collect[i].InitStats.Hits -= Damage;
			}
			if(to_collect[i].IsType("", "Chicken"))
			{
				TileMaster.instance.ReplaceTile(to_collect[i].Point.Base[0], to_collect[i].Point.Base[1], TileMaster.Types["Health"]);
				to_collect[i].AddValue(to_collect[i].Stats.Value * 10);
				to_collect.RemoveAt(i);
			}

		}
		
		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
		particles.Clear();
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());

	}
}
