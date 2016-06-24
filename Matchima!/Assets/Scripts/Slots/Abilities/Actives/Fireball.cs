using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fireball : Ability {

	public int radius = 2;
	public bool collectSquare = false;

	private int upgrade_radius = 0;
	Ability_UpgradeInfo Radius;
	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(Radius.Info, GameData.Colour(GENUS.WIS))};
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Collects tiles in a ", Color.white, true));
			All.Add(new StCon(" " + radius + " tile radius", GameData.Colour(GENUS.WIS), false));
			All.Add(new StCon(" around target.", Color.white, true));
			All.Add(new StCon("Deals", Color.white, false));
			All.Add(new StCon(final_damage + " damage", GameData.Colour(Parent.Genus), false));
			return All.ToArray();
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Collects all tiles in a " + radius + " tile radius around the target.";
	}

	public override void SetArgs(params string [] args)
	{
		radius = GameData.StringToInt(args[0]);
	}

	public override IEnumerator ActiveRoutine()
	{
		//if(activated) return;
		//activated = true;
		int xx = Random.Range(0, TileMaster.Grid.Size[0]);
		int yy = Random.Range(0, TileMaster.Grid.Size[1]);
		Tile target = TileMaster.Tiles[xx,yy];

		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);
		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons.GetClass(Parent.Index).transform.position + Vector3.up, 
													"Fireball", 55, GameData.Colour(Parent.Genus), 1.2F, 0.25F);
		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Parent.Genus].transform, Effect.Force);

		MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
		mp.SetTarget(target.transform.position);
		mp.SetPath(0.35F, 0.2F);
		mp.SetMethod(() => 
			{
				if(this != null) StartCoroutine(Cast(target));
			});
		yield return new WaitForSeconds(GameData.GameSpeed(0.6F));

		
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		//yield return StartCoroutine(CollectTiles(target));
		//activated = false;
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Collects all tiles in a " + radius + " tile radius around the target.";
	}


	IEnumerator Cast(Tile target)
	{
		int targX = target.Point.Base[0];
		int targY = target.Point.Base[1];

		float particle_time = 0.7F;

		int final_radius =  (radius + upgrade_radius + (int) StatBonus());

		if(target == null) yield break;

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<GameObject> particles = new List<GameObject>();

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				int distX = Mathf.Abs(x - targX);
				int distY = Mathf.Abs(y - targY);
				if(collectSquare)
				{
					if(distX < final_radius && distY < final_radius)
					{
						Tile tile = _tiles[x,y];
						_tiles[x,y].SetState(TileState.Selected, true);
						to_collect.Add(tile);
						GameObject new_part = Instantiate(Particle);

						new_part.transform.position = tile.transform.position;
						//new_part.transform.localScale *= 0.2F;
						particles.Add(new_part);
					}
				}
				else
				{
					if(distX + distY < final_radius)
					{
						Tile tile = _tiles[x,y];
						_tiles[x,y].SetState(TileState.Selected, true);
						to_collect.Add(tile);

						GameObject new_part = Instantiate(Particle);
						new_part.transform.position = tile.transform.position;
						//new_part.transform.localScale *= 0.2F;
						particles.Add(new_part);
					}
				}
			}
		}

		yield return new WaitForSeconds(Time.deltaTime * 10);

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
}