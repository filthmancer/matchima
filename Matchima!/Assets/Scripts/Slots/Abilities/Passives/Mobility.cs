using UnityEngine;
using System.Collections;

public class Mobility : Ability {

	public TileTypes Types;
	public int AttackRateIncrease = 1;

	public override void Start()
	{
		base.Start();
		Description_Basic = "Adds " + AttackRateIncrease + " additional turn per enemy turn.";
	}
	
	public override void Activate()
	{
		//TileMaster.Types = Types;
		//TileMaster.Types.GetTypes(Types);
		//SpawnableTileContainer.GetSpawnables(Types);
		Player.Stats.AttackRate += AttackRateIncrease;
	}
}
