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

	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		if(activated) return;

		activated = true;

		Tile target = PlayerControl.instance.LastSelected();
		if(Application.isMobilePlatform) target = PlayerControl.instance.focusTile;
		StartCoroutine(CollectTiles(target));
		cooldown_time = cooldown;
		activated = false;
	}

	public override void Update()
	{
		base.Update();
		Description_Basic = "Collects all tiles in a " + radius + " tile radius around the target.";
	}

	public string OLDUPGRADE
	{
		get
		{
			string s = UpgradeInfoColoured(1, "Dec. cooldown by 2") + "\n";
				s+= UpgradeInfoColoured(2, "Inc. radius by 1") + "\n";
				s+= UpgradeInfoColoured(3,  "Inc. radius by 1") + "\n";
				return s;
		}
	}

	public IEnumerator CollectTiles(Tile target)
	{
		int targX = target.Point.Base[0];
		int targY = target.Point.Base[1];
		int [] resource = new int [6];
		int [] heal = new int [6];
		int [] armour = new int [6];

		float particle_time = 0.3F;

		int final_radius =  (radius + upgrade_radius + (int) StatBonus());

		if(target == null) yield break;

		yield return new WaitForSeconds(0.1F);

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		GameObject initpart = Instantiate(Particle);
		initpart.transform.position = UIManager.ClassButtons[Parent.Index].transform.position;
		initpart.GetComponent<MoveToPoint>().SetTarget(target.transform.position);

		List<GameObject> particles = new List<GameObject>();

		//yield return new WaitForSeconds(0.5F);

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

		yield return new WaitForSeconds(particle_time);

		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));

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
		PlayerControl.instance.AddTilesToMatch(to_collect.ToArray());
		if(Player.instance.CompleteMatch)
		{
				foreach(Tile child in PlayerControl.instance.finalTiles)
				{
					//if(child.isMatching) continue;

					int [] values = child.Stats.GetValues();
					int added_res = 0, added_health = 0, added_armour = 0;
					
					if(child.Match(1))
					{
						resource[(int)child.Genus] += values[0] + added_res;
						heal[(int)child.Genus]   += values[1] + added_health;
						armour[(int)child.Genus]   += values[2] + added_armour;
					}
				}
			}
			
		yield return StartCoroutine(Player.instance.AfterMatch());

		List<Bonus> bonuses = new List<Bonus>();
		bonuses.Add(new Bonus(1 + StatBonus(), "ABL", "Bonus from " + name + " ability", GameData.instance.GetGENUSColour(GENUS.PRP)));


		GameManager.instance.CollectResources(resource, heal, armour, bonuses.ToArray());
		yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());

		TileMaster.instance.ResetTiles();
		TileMaster.instance.SetFillGrid(true);

		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
		particles.Clear();


		yield break;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);

		Fireball new_fireball = (Fireball) new_ab;
		radius = new_fireball.radius;
		Start();
	}

	public override void Setup(AbilityContainer con, int? _in = null, int? _out = null)
	{
		base.Setup(con, _in, _out);
		
		_input = null;
		if(_in.HasValue)
		{
			_input = con.Input[(int)_in];
		} 
		else
		{
			_input = GetContainerData(con);
		}

		radius = GameData.StringToInt(_input.args[1]);
		name = "Fireball";
		Radius = new Ability_UpgradeInfo(0, 1, "+", " Radius", Color.green, () => {upgrade_radius += 1;});
		Upgrades.Add(Radius);
	}
}