﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cross : Tile {
	public int CrossSize{
		get{
			return Stats.Value/2 + 2;
		}
	}
	public GameObject CrossParts;
	public ParticleSystem [] Particles;
	int[] UpLeft = new int[] {-1, 1};
	int[] UpRight = new int[] {1, 1};
	int[] DownLeft = new int[] {-1, -1};
	int[] DownRight = new int[] {1, -1};

	public int CrossDamage = 10;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Collects in " + Stats.Value + "L cross", GameData.Colour(Genus)),
				new StCon("Deals ", Color.white, false),
				new StCon(CrossDamage + " damage", GameData.Colour(Genus))
			};
		}
	}


	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale,inf,value_inc);
		CrossParts = (GameObject) Instantiate(CrossParts);
		CrossParts.transform.position = this.transform.position;
		CrossParts.transform.parent = this.transform;
		Particles = new ParticleSystem[CrossParts.transform.childCount];
		for(int i = 0; i < CrossParts.transform.childCount; i++)
		{
			Particles[i] = CrossParts.transform.GetChild(i).GetComponent<ParticleSystem>();
		}
		for(int i = 0; i < Particles.Length; i++)
		{
			//Particles[i].Stop();
			Particles[i].enableEmission = false;
		}
	}

	public override IEnumerator BeforeMatch(bool original)
	{
		if(isMatching) yield break;
		isMatching = true;
		for(int i = 0; i < Particles.Length; i++)
		{
			Particles[i].startSize = 2 * _Scale;
			Particles[i].startSpeed = Stats.Value * 4;
			Particles[i].emissionRate = Stats.Value * 25;
			//Particles[i].Play();
			Particles[i].enableEmission = true;
		}
		

		List<Tile> to_collect = new List<Tile>();
		Tile [,] _tiles = TileMaster.Tiles;
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				if(_tiles[x,y] == null) continue;
				if(Point.Contains(x,y, false))
				{
					if(_tiles[x,y] != this) 
					{
						int dist = 1;
						Point.Closest(x,y, out dist);
						if(dist > CrossSize) continue;
						if(_tiles[x,y].Type.isEnemy) 
						{
							_tiles[x,y].InitStats.TurnDamage += CrossDamage;
							AudioManager.instance.PlayClipOn(_tiles[x,y].transform, "Enemy", "Hit");
							EffectManager.instance.PlayEffect(_tiles[x,y].transform,Effect.Attack);
						}
						if(!_tiles[x,y].isMatching)
						{
							_tiles[x,y].SetState(TileState.Selected, true);
							to_collect.Add(_tiles[x,y]);	
						}			
					}
				} 
			}
		}
		//CameraUtility.instance.ScreenShake((float)Stats.Value/25, Time.deltaTime*15);
		TileMaster.instance.Ripple(this, to_collect, 1.2F*Stats.Value, GameData.GameSpeed(0.35F), 0.5F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.4F));

		for(int i = 0; i < Particles.Length; i++)
		{
			//Particles[i].Stop();
			Particles[i].enableEmission = false;
		}

		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		
		yield break;
	}
}