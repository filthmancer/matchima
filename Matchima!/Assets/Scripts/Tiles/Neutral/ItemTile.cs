using UnityEngine;
using System.Collections;

public class ItemTile : Tile {
	public Item _Item;
	public bool isChoosing;
	public GameObject ItemObj;


	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Contains an item.", GameData.Colour(Genus))};
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

	public override bool Match(int resource)
	{
		if(isMatching) return true;
		isMatching = true;

		CheckStats();
		Stats.Value *=  resource;

	//CHANGE ITEM STATS BASED ON VALUE

		GameObject item_obj = Instantiate(ItemObj);
		_Item = item_obj.GetComponent<Item>();
		_Item.SetStats(null, Stats.Value, Genus);
		Player.instance.PickupItem(_Item);
		CollectThyself(true);
		TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
		return true;
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
