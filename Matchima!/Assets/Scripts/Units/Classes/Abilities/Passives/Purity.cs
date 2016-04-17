using UnityEngine;
using System.Collections;

public class Purity : Ability {

	public TileTypes Types;
	public int Turns;

	public override void Start()
	{
		base.Start();
		Description_Basic = "Removes some types of tiles from spawning.";
	}


	public override void Activate()
	{
		TileMaster.Types = Types;
		
		//Spawner2.GetSpawnables(Types);
	}
}