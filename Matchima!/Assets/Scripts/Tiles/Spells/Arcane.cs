using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Arcane : Tile {

	public string InputGenus, InputType;
	public int EndValueAdded = 0;
	public int TilesCollected
	{
		get
		{
			CheckStats();
			return 2 + Stats.Value/4;
		}
	}

	public int final_damage 
	{
		get
		{
			CheckStats();
			return 1 + (Stats.Value/2) + (PlayerControl.instance.Controller ? (int) PlayerControl.instance.Controller.Stats.Spell : 0);
		}
	}

	public GameObject ArcaneParticle;
	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Attacks " + TilesCollected + " enemies", Color.white,true, 40),
				new StCon(final_damage + "damage", GameData.Colour(GENUS.WIS), false, 40)
			};
		}
	}
	public override void Setup(GridInfo g, int x, int y, int scale, TileInfo inf, int value_inc)
	{
		base.Setup(g, x, y, scale, inf, value_inc);	
	}

	public override void SetArgs(params string [] args)
	{
		
	}


	public override IEnumerator BeforeMatch(Tile Controller)
	{
		float part_time = 0.25F;
		if(isMatching) yield break;

		isMatching = true;
		List<Tile> to_collect = new List<Tile>();
		int x = TileMaster.Tiles.GetLength(0);
		int y = TileMaster.Tiles.GetLength(1);

		int part_num = 1;

		for(int xx = 0; xx < x; xx ++)
		{
			for(int yy = 0; yy < y; yy++)
			{
				if(TileMaster.Tiles[xx,yy] == null) continue;
				if(TileMaster.Tiles[xx,yy].IsGenus(GENUS.OMG, false) &&
					!TileMaster.Tiles[xx,yy].isMatching) to_collect.Add(TileMaster.Tiles[xx,yy]);
			}
		}	
		while(to_collect.Count > TilesCollected)
		{
			to_collect.RemoveAt(UnityEngine.Random.Range(0, to_collect.Count));
		}

		foreach(Tile child in to_collect)
		{
			PlayerControl.instance.RemoveTileToMatch(child);
			PlayAudio("cast");

			AttackParticle((float)part_num/TilesCollected, child, (Tile c) =>
			{
				c.SetState(TileState.Selected, true);
				c.ChangeGenus(Genus);
				EffectManager.instance.PlayEffect(c.transform, Effect.Replace, GameData.instance.GetGENUSColour(c.Genus));	
			});

			part_num ++;

			yield return new WaitForSeconds(part_time);	
		}

		if(TileMaster.instance.EnemiesOnScreen != 0 || TileMaster.Enemies.Length != 0)
		{
			List<Tile> enemies = new List<Tile>();
			enemies.AddRange(TileMaster.Enemies);
			
			while(to_collect.Count < TilesCollected)
			{
				if(enemies.Count == 0) enemies.AddRange(TileMaster.Enemies);

				Tile c = enemies[UnityEngine.Random.Range(0, enemies.Count)];
				enemies.Remove(c);
				to_collect.Add(c);	
				PlayAudio("cast");


				AttackParticle((float) part_num/TilesCollected, c, (Tile child) =>
				{
					child.SetState(TileState.Selected, true);
					child.InitStats.TurnDamage += final_damage;
					child.PlayAudio("hit");

					PlayerControl.instance.AddTilesToSelected(child);

					Vector3 pos = TileMaster.Grid.GetPoint(child.Point.Point(0)) + Vector3.down * 0.3F;
					MiniAlertUI hit = UIManager.instance.DamageAlert(pos, final_damage);

					CameraUtility.instance.ScreenShake(0.26F + 0.02F * final_damage,  GameData.GameSpeed(0.06F));
					EffectManager.instance.PlayEffect(child.transform,Effect.Attack);
				});
				part_num++;
					
				yield return new WaitForSeconds(part_time);
			}
			yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		}

		
		yield return StartCoroutine(base.BeforeMatch(Controller));
	}

	public void AttackParticle(float ratio, Tile t, Action<Tile> a)
	{
		GameObject part = EffectManager.instance.PlayEffect(this.transform, Effect.Spell);

		MoveToPoint mp = part.GetComponent<MoveToPoint>();
		
		mp.SetPath(28.0F, 0.2F);

		Vector3 offset = Vector3.up + Vector3.Lerp(Vector3.left*1.2F, Vector3.right * 1.2F, ratio);


		mp.AddStep(this.transform.position + offset, 0.3F);
		mp.AddStep(t.transform.position, 0.0F);
		part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);

		mp.SetTileMethod(t, a);
	}

}
