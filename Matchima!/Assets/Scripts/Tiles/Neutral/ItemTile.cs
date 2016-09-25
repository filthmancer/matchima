using UnityEngine;
using System.Collections;

public class ItemTile : Tile {
	public Item _Item;
	public bool isChoosing;
	public GameObject ItemObj;


	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Contains an secret tile", GameData.Colour(Genus),true, 40)};
		}
	}


	protected override TileUpgrade [] BaseUpgrades
	{
		get
		{
			return new TileUpgrade []
			{
				new TileUpgrade(1.0F, 1, () => {InitStats.Value += 1;})
			};
		}
	}

	private class RollCon
	{
		public string species;
		public GENUS genus;
		public RollCon(string s, GENUS g)
		{
			genus = g;
			species = s;
		}
	}

	private RollCon [] Rolls = new RollCon[]
	{
		new RollCon("cross", GENUS.RAND),
		new RollCon("lightning", GENUS.RAND),
		new RollCon("bomb", GENUS.RAND),
		new RollCon("arcane", GENUS.RAND),
	//	new RollCon("altar", GENUS.OMG),
		//new RollCon("minion", GENUS.RAND),
		//new RollCon("chicken", GENUS.OMG),
		new RollCon("health", GENUS.ALL),
		new RollCon("flame", GENUS.RAND),
		new RollCon("lens", GENUS.RAND)
	};

	public override bool Match(int resource)
	{
		if(isMatching) return true;
		isMatching = true;

		InitStats.Value *=  resource;
		CheckStats();
		
		RollCon type = Rolls[Random.Range(0, Rolls.Length)];
		TileMaster.instance.ReplaceTile(this, TileMaster.Types[type.species], type.genus, Point.Scale, Stats.Value);
		PlayAudio("cast");
		return false;
	//CHANGE ITEM STATS BASED ON VALUE

	/*	GameObject item_obj = Instantiate(ItemObj);
		_Item = item_obj.GetComponent<Item>();
		_Item.SetStats(null, Stats.Value, Genus);
		Player.instance.PickupItem(_Item);
		CollectThyself(true);
		TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;*/
		
	}

	//public override void Setup(int x, int y, int scale, TileInfo inf)
	//{
	//	base.Setup(x,y,scale, inf);
	//	_ren
	//}
}
