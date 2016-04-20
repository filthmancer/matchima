using UnityEngine;
using System.Collections;

public class Maturation : Ability {

	public int matureValue = 1;
	public int matureTime = 3;

	public int fieldY = 4;

	public override void Start()
	{
		base.Start();

		switch(matureValue)
		{
			case 2:
			Description_Basic += "Doubles ";
			break;
			case 3:
			Description_Basic += "Triples ";
			break;
			case 4:
			Description_Basic += "Quadruples " ;
			break;
		}

		Description_Basic += "value of tiles in bottom " + fieldY + " rows";
	}

	public override void BeforeTurn()
	{
		base.BeforeTurn();
		if(cooldown_time > 0) return;
		Tile [,] _tiles = TileMaster.Tiles;
		if(_tiles == null || _tiles.GetLength(0) == 0) return;
		
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < fieldY; y++)
			{
				if(_tiles[x,y].matureTime < 3)
				{
					int addedVal = _tiles[x,y].Stats.Value;
					addedVal = (addedVal * matureValue) - addedVal;
					_tiles[x,y].AddValue(addedVal);					

					Vector3 pos = TileMaster.Grid.GetPoint(x,y);
					UIManager.instance.MiniAlert(pos + Vector3.up*0.5F, "" + addedVal, 45,Color.green, 1.0F,0.00F);
					
				}
				else
				{
					int tooripe = _tiles[x,y].Stats.Value /2;
					_tiles[x,y].AddValue(-tooripe);

					Vector3 pos = TileMaster.Grid.GetPoint(x,y);
					UIManager.instance.MiniAlert(pos + Vector3.up*0.5F, "" + tooripe, 45, Color.red, 1.0F,0.00F);
				}

				_tiles[x,y].matureTime ++;

				//if(_tiles[x,y].matureTime == matureTime)
				//	{
				//		GameObject part = Instantiate(Particle);
				//		part.transform.position = _tiles[x,y].transform.position;
				//		part.transform.parent = _tiles[x,y].transform.parent;
				//	}
			}
		}
		cooldown_time = cooldown;
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

		matureValue = GameData.StringToInt(_input.args[3]);
		matureTime = GameData.StringToInt(_input.args[4]);
		fieldY = GameData.StringToInt(_input.args[5]);
		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
	}
}
