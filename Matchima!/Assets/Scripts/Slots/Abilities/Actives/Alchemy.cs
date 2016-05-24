using UnityEngine;
using System.Collections;

public class Alchemy : Ability {

	public TileInfo changeTile;
	public string Genus, Species;

	private int upgrade_valueInc = 0;
	private float upgrade_otherTypeChance = 0.0F;


	public override StCon [] Description_Tooltip
	{
		get{
			
			return new StCon [] {
				new StCon("Tile of next match type changed to ", Color.white, false),
				new StCon(Genus + " " + Species, Color.white)
			};
		}
	}

	public override void Start()
	{
		base.Start();
		Description_Basic = "Next match type changed to " + Genus + " " + Species;
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;

		activated = true;
		//changeTile.Clear();
	}


	public override void Update()
	{
		base.Update();
		if(activated && PlayerControl.matchingTile != null)
		{
			changeTile = PlayerControl.matchingTile.Info;
		}
		Description_Basic = "Next match type changed to " + Genus + " " + Species;
	}

	public override IEnumerator BeforeTurn()
	{
		if(activated)
		{
			Tile [,] _tiles = TileMaster.Tiles;
			bool otherType = Random.value < upgrade_otherTypeChance;
			TileInfo otherTypeTile = otherType ? TileMaster.Types.RandomType(GENUS.NONE) : null;
			for(int x = 0; x < _tiles.GetLength(0); x++)
			{
				for(int y = 0; y < _tiles.GetLength(1); y++)
				{
					if(_tiles[x,y].IsType(changeTile) || (otherType && _tiles[x,y].IsType(otherTypeTile))) 
					{
						TileMaster.instance.ReplaceTile(x,y, TileMaster.Types[Species], TileMaster.Types.GENUSOf(Genus));
						//TileMaster.Types.TypeOf(Genus, Species));
						_tiles[x,y].Stats.Value += (int) StatBonus();
					}
				}
			}
			cooldown_time = cooldown;
			activated = false;
		}
		yield return null;
	}


	public override void Setup(Ability new_ab)
	{
		base.Setup(new_ab);
		Alchemy stack = (Alchemy) new_ab;
		Genus = stack.Genus;
		Species = stack.Species;
		Start();
	}

	protected override void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
			Genus = GameData.ResourceLong(Parent.Genus);
		}
	}

	//Input
	//Args 0: Title
	//Args 1: Chance
	//Args 2: GENUS
	//Args 3: Genus
	//Args 4: Species
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

		Genus = _input.args[3];
		Species = _input.args[4];

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);
	}
}