using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slash : Ability{

	Ability_UpgradeInfo DamageUp, ExtraRows;
	public int upgrade_extra_rows = 0;
	public int upgrade_damage = 0;
	public override StCon [] Description
	{
		get
		{
			return new StCon[] {new StCon(DamageUp.Info, GameData.Colour(GENUS.WIS)),
								new StCon(ExtraRows.Info, GameData.Colour(GENUS.WIS))};
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
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;
		StartCoroutine(SlashRoutine());
	}

	public IEnumerator SlashRoutine()
	{
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		GameObject [] part_x = null, part_y = null;

		int [] resource = new int[6];
		int [] heal = new int[6];
		int [] armour = new int[6];

		if(SlashX)
		{
			part_x = new GameObject[1 + upgrade_extra_rows];
			List<int> poss_y = new List<int>();
			for(int i = 0; i < _tiles.GetLength(1); i++)
			{
				poss_y.Add(i);
			}

			for(int i = 0; i < 1 + upgrade_extra_rows; i++)
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
					to_collect.Add(_tiles[x,rand_y]);
				}
			}
		}

		if(SlashY)
		{
			part_y = new GameObject[1 + upgrade_extra_rows];
			List<int> poss_x = new List<int>();
			for(int i = 0; i < _tiles.GetLength(0); i++)
			{
				poss_x.Add(i);
			}
			for(int i = 0; i < 1 + upgrade_extra_rows; i++)
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
					to_collect.Add(_tiles[rand_x,y]);
				}
			}
		}

		for(int i = 0; i < 1 + upgrade_extra_rows; i++)
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
		
		yield return StartCoroutine(Player.instance.BeforeMatch(to_collect));
		PlayerControl.instance.AddTilesToMatch(to_collect.ToArray());
		if(Player.instance.CompleteMatch)
		{
			foreach(Tile child in PlayerControl.instance.finalTiles)
			{
				int v = 1;
				if(child.Type.isEnemy) 
				{
					child.InitStats.TurnDamage += Damage;
				}

				if(child.IsType("","Altar"))
				{
					break;
				}
				else if(child.Match(v))
				{
					int [] values = child.Stats.GetValues();
					resource[(int)child.Genus] += values[0];
					heal[(int)child.Genus]   += values[1];
					armour[(int)child.Genus]   += values[2];
				}
			}
		}
		yield return StartCoroutine(Player.instance.AfterMatch());

		cooldown_time = cooldown;
		activated = false;
		GameManager.instance.CollectResources(resource, heal, armour);
		yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());
		TileMaster.instance.ResetTiles();
		TileMaster.instance.SetFillGrid(true);

		yield break;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Slash stack = (Slash) new_ab;
		SlashY = stack.SlashY;
		SlashX = stack.SlashX;
		Start();
	}

	public override void Setup(AbilityContainer con, int? _in, int? _out)
	{
		base.Setup(con, _in, _out);
		_input = null;
		if(_in.HasValue) _input = con.Input[(int)_in];
		else
		{
			_input = GetContainerData(con);
		}
		GENUS = (GENUS) GameData.StringToInt(_input.args[2]);
		SlashY = _input.args[3].Contains("Y");
		SlashX = _input.args[3].Contains("X");

		if(SlashX && !SlashY) name = "Slash";
		else if(SlashY && !SlashX) name = "Swipe";
		else name = "Swing";

		ExtraRows = new Ability_UpgradeInfo(0,1, "", "extra slashes", Color.green, () => {upgrade_extra_rows += 1;});
		Upgrades.Add(ExtraRows);
		DamageUp = new Ability_UpgradeInfo(0, 2, "+", " Damage", Color.green, () => {upgrade_damage += 2;});
		Upgrades.Add(DamageUp);
	}
}