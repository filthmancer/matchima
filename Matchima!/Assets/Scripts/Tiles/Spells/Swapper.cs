using UnityEngine;
using System.Collections;

public class Swapper : Tile {


	public string Input_EndGenus, Input_EndType;
	public string Input_StartGenus, Input_StartType;

	public string StartTotal
	{get{return StartGenus + " " + StartType;}} 
	public string EndTotal
	{get{return EndGenus + " " + EndType;}} 

	public string StartGenus
	{
		get{
			if(Input_StartGenus == "Genus") return GameData.ResourceLong(Genus);
			else return Input_StartGenus;
		}
	}
	public string StartType
	{
		get{return Input_StartType;}
	} 


	public string EndGenus
	{
		get
		{
			if(Input_EndGenus == "Genus") return GameData.ResourceLong(Genus);
			else return Input_EndGenus;
		}
	}
	public string EndType
	{
		get{return Input_EndType;}
	} 

	private int added_value = 0;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Swaps " + StartTotal + " to " + EndTotal, GameData.Colour(Genus))
			};
		}
	}

	protected virtual TileUpgrade [] AddedUpgrades
	{
		get
		{
			return new TileUpgrade []
			{
				new TileUpgrade(0.4F, 2, ()=>{added_value ++;})
			};
		}
	}

	public override void SetArgs(params string [] args)
	{
		Input_StartGenus = args[0];
		Input_StartType = args[1];
		Input_EndGenus = args[2];
		Input_EndType = args[3];

		if(Input_StartGenus == "Random") Input_StartGenus = GameData.ResourceLong((GENUS)Random.Range(0,4));
		else if(Input_StartGenus == "RandomAll") Input_StartGenus = GameData.ResourceLong((GENUS)Random.Range(0,6));

		if(Input_EndGenus == "Random") Input_EndGenus = GameData.ResourceLong((GENUS)Random.Range(0,4));
		else if(Input_EndGenus == "RandomAll") Input_EndGenus = GameData.ResourceLong((GENUS)Random.Range(0,6));
	}



	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale, inf, value_inc);

	}

	public override void Update()
	{
		base.Update();
		if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, GameData.Colour(Genus), 0.6F);
	}

	public override IEnumerator BeforeMatch(bool original)
	{		
		Tile [,] _tiles = TileMaster.Tiles;
		string init = StartGenus;

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			bool has_tile = false;
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				if(_tiles[x,y] == null || _tiles[x,y] == this)
				{
					continue;
				} 
				SPECIES sp = null;
				GENUS g = TileMaster.Types.GENUSOf(EndGenus);
				if(_tiles[x,y].IsType(init, StartType)) 
				{
					has_tile = true;
					int old_value = _tiles[x,y].Stats.Value;
					int final_value = old_value + added_value;

					sp = _tiles[x,y].Type;
					if(EndType != string.Empty) sp = TileMaster.Types[EndType];

					if(sp == null)
					{
						_tiles[x,y].ChangeGenus(g);
						_tiles[x,y].AddValue(added_value);
						EffectManager.instance.PlayEffect(_tiles[x,y].transform, Effect.Replace, "", GameData.instance.GetGENUSColour(_tiles[x,y].Genus));
					} 
					else
					{
						bool add = false;
						if(PlayerControl.instance.IsInMatch(TileMaster.Tiles[x,y]))
						{
							add = true;
							PlayerControl.instance.RemoveTileToMatch(TileMaster.Tiles[x,y]);
							} 
						TileMaster.instance.ReplaceTile(x,y, sp, g, 1, final_value);
						if(add) PlayerControl.instance.AddTilesToSelected(TileMaster.Tiles[x,y]);
					}
				}
			}
			if(has_tile) yield return new WaitForSeconds(Time.deltaTime*5);
		}
		yield return null;
	}
}
