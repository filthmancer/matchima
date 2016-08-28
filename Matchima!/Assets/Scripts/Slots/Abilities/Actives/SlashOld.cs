using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashOld : Ability{

	int init_slashes = 0;
	int total_slashes
	{
		get{
			return (int) ((float)init_slashes * StrengthFactor);
		}
	}

	public override StCon [] Description_Tooltip
	{
		get
		{
			return new StCon[]
			{
				new StCon("Collects all tiles"),
				new StCon("on a random"),
				new StCon((SlashX ? "X " : "") + (SlashX && SlashY ? "and " : "") + (SlashY ? "Y " : "") + " axis")
			};
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Collects all tiles on a random " + (SlashX ? "X " : "") + (SlashX && SlashY ? "and " : "") + (SlashY ? "Y " : "") + "axis";
	}


	public bool SlashX, SlashY;

	public override IEnumerator BeforeTurn()
	{	
		yield return StartCoroutine(SlashRoutine());
	}

	public override void SetArgs(params string [] args)
	{
		init_slashes = GameData.StringToInt(args[0]);
		SlashX = args[1].Contains("X");
		SlashY = args[1].Contains("Y");
	}

	public IEnumerator SlashRoutine()
	{
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);
		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons.GetClass(Parent.Index).transform.position + Vector3.up, 
													"Slash", 55, GameData.Colour(Parent.Genus), 1.2F, 0.25F);
		yield return new WaitForSeconds(Time.deltaTime * 15);
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		GameObject [] part_x = null, part_y = null;

		int [] resource = new int[6];
		int [] heal = new int[6];
		int [] armour = new int[6];

		if(SlashX)
		{
			part_x = new GameObject[total_slashes];
			List<int> poss_y = new List<int>();
			for(int i = 0; i < _tiles.GetLength(1); i++)
			{
				poss_y.Add(i);
			}

			for(int i = 0; i < total_slashes; i++)
			{
				int rand_y = Random.Range(0, poss_y.Count);
				int y = poss_y[rand_y];
				poss_y.RemoveAt(rand_y);

				part_x[i] = Instantiate(Particle);
				Vector3 pos = _tiles[0,y].transform.position + Vector3.back * 2;
				pos.x = -5;
				part_x[i].transform.position = pos;
				for(int x = 0; x < _tiles.GetLength(0); x++)
				{
					_tiles[x,rand_y].SetState(TileState.Selected, true);
					if(!_tiles[x, rand_y].IsGenus(GENUS.OMG)) 
						to_collect.Add(_tiles[x,rand_y]);
				}
			}
		}

		if(SlashY)
		{
			part_y = new GameObject[total_slashes];
			List<int> poss_x = new List<int>();
			for(int i = 0; i < _tiles.GetLength(0); i++)
			{
				poss_x.Add(i);
			}
			for(int i = 0; i < total_slashes; i++)
			{
				int rand_x = Random.Range(0, poss_x.Count);
				int x = poss_x[rand_x];
				poss_x.RemoveAt(rand_x);

				part_y[i] = Instantiate(Particle);
				Vector3 pos =  _tiles[x, 0].transform.position + Vector3.back * 2;

				pos.y = 7;
				part_y[i].transform.position = pos;
	
				for(int y = 0; y < _tiles.GetLength(1); y++)
				{
					_tiles[rand_x,y].SetState(TileState.Selected, true);
					if(!_tiles[rand_x, y].IsGenus(GENUS.OMG, false, false))
						to_collect.Add(_tiles[rand_x,y]);
				}
			}
		}

		for(int i = 0; i < total_slashes; i++)
		{
			bool movingParticles = true;
			bool xfin = false, yfin = false;
			float xrate = 0.1F, yrate = 0.1F;
			while(movingParticles)
			{
				if(SlashX) 
				{
					part_x[i].transform.position += Vector3.right * xrate;
					if(part_x[i].transform.position.x > 5) xfin = true;
					xrate *= 1.4F;
				}
				else xfin = true;
				if(SlashY) 
				{
					part_y[i].transform.position -= Vector3.up * yrate;
					if(part_y[i].transform.position.y < -7) yfin = true;
					yrate *= 1.4F;
				}
				else yfin = true;
				if(xfin && yfin) movingParticles = false;
				yield return null;
			}
		}
		
		if(part_x != null && part_x.Length != 0){
			for(int i = 0; i < part_x.Length; i++)
			{
				Destroy(part_x[i]);
			}
		}

		if(part_y != null && part_y.Length != 0){
			for(int i = 0; i < part_y.Length; i++)
			{
				Destroy(part_y[i]);
			}
		}
		yield return new WaitForSeconds(0.2F);
		
		
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		
		//if(Player.instance.CompleteMatch)
		//{
			//foreach(Tile child in PlayerControl.instance.finalTiles)
			//{
			//	int v = 1;
			//	if(child.Type.isEnemy) 
			//	{
			//		child.InitStats.TurnDamage += Damage;
			//	}
			//	if(child.IsGenus(GENUS.OMG))
			//	{
			//		continue;
			//	}
			//	else if(child.Match(v))
			//	{
			//		int [] values = child.Stats.GetValues();
			//		resource[(int)child.Genus] += values[0];
			//		heal[(int)child.Genus]   += values[1];
			//		armour[(int)child.Genus]   += values[2];
			//	}
			//}

		
		//}
		

		//cooldown_time = cooldown;
		//activated = false;
		//GameManager.instance.CollectResources(resource, heal, armour);
		//yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());
		TileMaster.instance.ResetTiles();
		//TileMaster.instance.SetFillGrid(true);

		yield break;
	}
}