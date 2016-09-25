using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calldown : Powerup {

	public GameObject FireballParticle;
	public GameObject ExplodeParticle;
List<Tile> to_collect = new List<Tile>();
List<GameObject> particles = new List<GameObject>();
bool [] landed;
bool alllanded
{
	get{
		for(int i = 0; i < landed.Length; i++)
		{
			if(!landed[i]) return false;
		}
		return true;
	}
}

float max_target_speed = 0.25F;
int[] fireballs = new int[]
{
	1,
	2,
	3
};
	protected override IEnumerator Minigame(int Level)
	{
		yield return StartCoroutine(PowerupStartup());
		int fireballnum = fireballs[Level-1];

		int currtile = 0;
		int nexttile = 1;
		float taptimer = 3.0F;
		int nexttile_acc = 1;

		UIObj [] MGame = new UIObj[fireballnum];
		IntVector [] MGame_target = new IntVector[fireballnum];
		Vector3 [] MGame_vel = new Vector3[fireballnum];
		for(int i = 0; i < MGame.Length; i++)
		{
			MGame[i] = CreateMinigameObj(0);
			MGame[i].transform.position = TileMaster.RandomTile.transform.position;
			MGame_vel[i] = new Vector3(Random.Range(0.03F, 0.04F * fireballnum), Random.Range(0.03F, 0.04F * fireballnum),0);
			if(Random.value > 0.5F) MGame_vel[i] = -MGame_vel[i];
			yield return null;
		}

		Vector3 max = TileMaster.Tiles[TileMaster.Grid.Size[0]-1, TileMaster.Grid.Size[1]-1].transform.position;
		Vector3 min = TileMaster.Tiles[0,0].transform.position;
		while(!Input.GetMouseButtonDown(0))
		{
			for(int i = 0; i < MGame.Length; i++)
			{
				MGame[i].transform.position += MGame_vel[i];

				if(Mathf.Abs(MGame_vel[i].x) + Mathf.Abs(MGame_vel[i].y) < max_target_speed) MGame_vel[i] *= 1.008F;

				Vector3 mpos = MGame[i].transform.position;

				if(mpos.x > max.x || mpos.x < min.x)
				{
					MGame_vel[i].x = -MGame_vel[i].x;
				}
				else if (mpos.y > max.y || mpos.y < min.y)
				{
					 MGame_vel[i].y = -MGame_vel[i].y;
				}
			}
			yield return null;
		}
		for(int i = 0; i < MGame.Length; i++)
		{
			MGame_target[i] = ClosestPoint(MGame[i].transform.position);
		}

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		for(int i = 0; i < MGame.Length;i++)
		{
			Destroy(MGame[i].gameObject);
		}

		yield return StartCoroutine(Cast(MGame_target));

		

	}

	IEnumerator Cast(IntVector [] targets)
	{
		float particle_time = 0.7F;
		int radius = 2;

		int Damage = 50;

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);

		particles.Clear();
		to_collect.Clear();
		landed = new bool[targets.Length];

		UIManager.instance.ScreenAlert.SetTween(0, false);

		for(int i = 0; i < targets.Length; i++)
		{
			Tile targ = TileMaster.Tiles[targets[i].x, targets[i].y];
			yield return StartCoroutine(CreateFireball(targ, radius, i));
		}
		
		while(!alllanded) 
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.1F);
		

		for(int i = 0; i < to_collect.Count; i++)
		{
			if(to_collect[i].Type.isEnemy)
			{
				to_collect[i].InitStats.TurnDamage += Damage;
			}
			if(to_collect[i].IsType("", "Chicken"))
			{
				TileMaster.instance.ReplaceTile(to_collect[i].Point.Base[0], to_collect[i].Point.Base[1], TileMaster.Types["Health"]);
				to_collect[i].AddValue(to_collect[i].Stats.Value * 10);
				to_collect.RemoveAt(i);
			}
		}

		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());

		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
		particles.Clear();
	}

	IEnumerator CreateFireball(Tile targ, int final_radius, int num)
	{
		GameObject missile = Instantiate(FireballParticle);
		missile.transform.position = UIManager.Objects.TopGear.transform.position + Vector3.left;
		missile.GetComponent<ParticleSystem>().startSize = 2.1F;
		missile.GetComponent<ParticleSystem>().startColor = Color.red;
		MoveToPoint mp = missile.GetComponent<MoveToPoint>();
		mp.enabled = true;
		mp.SetTarget(targ.transform.position);
		mp.SetPath(30.0F, 0.0F);

		//COLLECT TILES AROUND HIT TILE
		mp.SetMethod(() =>
		{
			AudioManager.instance.PlayClipOn(this.transform, "Powerup", "Explosion");
			CameraUtility.instance.ScreenShake(0.5F, 0.5F);
			for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
			{
				for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
				{
					int distX = Mathf.Abs(x - targ.Point.BaseX);
					int distY = Mathf.Abs(y - targ.Point.BaseY);
					if(distX + distY < final_radius)
					{
						Tile tile = TileMaster.Tiles[x,y];
						TileMaster.Tiles[x,y].SetState(TileState.Selected, true);
						to_collect.Add(tile);

						GameObject new_part = Instantiate(ExplodeParticle);
						new_part.transform.position = tile.transform.position;
						//new_part.transform.localScale *= 0.2F;
						particles.Add(new_part);
					}
				}
			}
			landed[num] = true;
		});

		yield return new WaitForSeconds(Time.deltaTime * 10);
	}


	IntVector ClosestPoint(Vector3 pos)
	{
		float closest_dist = 100.0F;
		IntVector closest = new IntVector(0,0);
		for(int i = 0; i < TileMaster.Grid.Size[0]; i++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1];y++)
			{
				float d = Vector3.Distance(TileMaster.Tiles[i,y].transform.position, pos);
				if(d < closest_dist)
				{
					closest = new IntVector(i,y);
					closest_dist = d;
				}
			}
			
		}
		if(closest.x == 0) closest.x = 1;
		else if(closest.y == 0) closest.y = 1;
		return closest;
	}
}
