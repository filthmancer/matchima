using UnityEngine;
using System.Collections;

public class Stacker : Tile {

	public string Type;
	public string _Genus
	{
		get
		{
			return GameData.ResourceLong(Genus);
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Queues " + _Genus + " " + Type + " tiles", GameData.Colour(Genus))
			};
		}
	}


	public int added_value = 0;

	public override void Update()
	{
		base.Update();
		if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, GameData.Colour(Genus), 0.6F);
	}


	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(x,y,scale,inf,value_inc);
	}

	public override bool Match(int resource)
	{
		if(this == null) return false;
		InitStats.Hits -= 1;
		CheckStats();

		foreach(Tile child in PlayerControl.instance.finalTiles)
		{
			TileMaster.instance.QueueTile(TileMaster.Types[Type], TileMaster.Genus[_Genus], added_value);
		}

		if(Stats.Hits <= 0)
		{
			isMatching = true;
			Stats.Value *=  resource;
			
			CollectThyself(true);
			TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
			return true;			
		}
		else 
		{
			isMatching = false;
			EffectManager.instance.PlayEffect(this.transform,Effect.Attack);
		}
		return false;
	}

}
