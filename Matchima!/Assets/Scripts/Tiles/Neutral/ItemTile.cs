using UnityEngine;
using System.Collections;

public class ItemTile : Tile {
	public Item _Item;
	public bool isChoosing;
	public GameObject ItemObj;


	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Contains an item.", GameData.Colour(Genus),true, 40)};
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

	public string [] RollTypes = new string[]
	{
		"cross",
		"lightning",
		"bomb",
		"altar",
		"minion",
		"blackhole"
	};

	public override bool Match(int resource)
	{
		if(isMatching) return true;
		isMatching = true;

		InitStats.Value *=  resource;
		CheckStats();
		if(Random.value > 0.3F)
		{
			InitStats.Value *= 3;
			CollectThyself(true);
			TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
			CheckStats();
			return true;
		}
		else
		{
			string type = RollTypes[Random.Range(0, RollTypes.Length)];
			TileMaster.instance.ReplaceTile(this, TileMaster.Types[type], GENUS.RAND, 1, Stats.Value);
			return false;
		}
	//CHANGE ITEM STATS BASED ON VALUE

	/*	GameObject item_obj = Instantiate(ItemObj);
		_Item = item_obj.GetComponent<Item>();
		_Item.SetStats(null, Stats.Value, Genus);
		Player.instance.PickupItem(_Item);
		CollectThyself(true);
		TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;*/
		
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);

		int diff = (int) InitStats.value_soft - InitStats.Value;
		if(diff != 0)
		{
			InitStats.Value = (int) InitStats.value_soft;
			CheckStats();
			SetSprite();
		}
		
	}


	//public override void Setup(int x, int y, int scale, TileInfo inf)
	//{
	//	base.Setup(x,y,scale, inf);
	//	_ren
	//}
}
