using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Firestorm : Powerup {

	public Sprite ClosedTome, OpenTome;
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
		yield return StartCoroutine(PowerupStartup());

		target_column = null;
		int currtile = 0;
		int nexttile = 1;
		float taptimer = 3.0F;
		int nexttile_acc = 1;

		yield return new WaitForSeconds(Time.deltaTime * 10);
		UIObj [] MGame = new UIObj[lines];
		int [] MGame_target = new int[lines];
		float [] MGame_vel = new float[lines];
		for(int i = 0; i < MGame.Length; i++)
		{
			MGame[i] = CreateTarget(TileMaster.Grid.Size[0]/2);
			MGame[i].transform.localScale *= 1.2F;
			MGame_vel[i] = Random.Range(0.03F, 0.09F * lines);
			if(Random.value > 0.5F) MGame_vel[i] = -MGame_vel[i];
			yield return null;
		}


		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*0.5F, "Tap to cast\nfirestorm!", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!Input.GetMouseButtonDown(0))
		{
			for(int i = 0; i < MGame.Length; i++)
			{
				MGame[i].transform.position += Vector3.right * MGame_vel[i];
				MGame_target[i] = ClosestPoint(MGame[i].transform.position);
				if(Mathf.Abs(MGame_vel[i]) < 0.4F) MGame_vel[i] *= 1.008F;
				if(MGame[i].transform.position.x > TileMaster.Tiles[TileMaster.Grid.Size[0]-1,0].transform.position.x) MGame_vel[i] = -MGame_vel[i];
				else if(MGame[i].transform.position.x < TileMaster.Tiles[0,0].transform.position.x) MGame_vel[i] = -MGame_vel[i];
			}
			yield return null;
		}
		m.PoolDestroy();

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		for(int i = 0; i < lines;i++)
		{
			MGame[i].Img[0].enabled= false;
			MGame[i].Img[1].enabled= true;
			yield return Cast(TileMaster.Tiles[MGame_target[i],0]);
		}
		yield return new WaitForSeconds(Time.deltaTime * 10);
		for(int i = 0; i < lines;i++)
		{
			Destroy(MGame[i].gameObject);
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
		////UIManager.CrewButtons[Parent.Index].ShowClass(false);
	}

	UIObj CreateTarget(int i)
	{
		UIObj obj = (UIObj)Instantiate(MinigameObj[0]);
		obj.Img[0].enabled= true;
		obj.Img[1].enabled= false;
		RectTransform rect = obj.GetComponent<RectTransform>();
		obj.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		obj.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;
		rect.transform.position = TileMaster.Tiles[i,0].transform.position;
		return obj;
	}

	int Damage = 20;
	public IEnumerator Cast(Tile target)
	{
		int targX = target.Point.Base[0];
		int targY = target.Point.Base[1];

		float particle_time = 0.7F;

		if(target == null) yield break;

		AudioManager.instance.PlayClipOn(this.transform, "Powerup", "Firestorm");
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<GameObject> particles = new List<GameObject>();

		for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
		{
			Tile tile = _tiles[targX,y];
			tile.SetState(TileState.Selected, true);
			to_collect.Add(tile);

			GameObject new_part = EffectManager.instance.PlayEffect(tile.transform, "fire");
			particles.Add(new_part);
			yield return new WaitForSeconds(GameData.GameSpeed(0.025F));

			tile = _tiles[targX,y];
			tile.SetState(TileState.Selected, true);
			to_collect.Add(tile);

			new_part = EffectManager.instance.PlayEffect(tile.transform, "fire");
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
