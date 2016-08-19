using UnityEngine;
using System.Collections;

public class Mana : Tile {

	public override StCon _Name {
		get{return new StCon("Mana", GameData.Colour(Genus));}
	}

}
