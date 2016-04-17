using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cross : Tile {
	
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

	public override IEnumerator BeforeMatch()
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
						if(dist > Stats.Value) continue;
						if(_tiles[x,y].Type.isEnemy) 
						{
							_tiles[x,y].InitStats.TurnDamage += CrossDamage;
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
		yield return new WaitForSeconds(0.5F);

		for(int i = 0; i < Particles.Length; i++)
		{
			//Particles[i].Stop();
			Particles[i].enableEmission = false;
		}

		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.RemoveTileToMatch(this);
		to_collect.Add(this);
		PlayerControl.instance.AddTilesToMatch(to_collect.ToArray());
		
		yield break;
	}
}