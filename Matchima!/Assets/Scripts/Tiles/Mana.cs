using UnityEngine;
using System.Collections;

public class Mana : Tile {

		public override StCon _Name {
		get{
			string valpref = Stats.Value > 1 ? "+" + Stats.Value : "";
			string effectpref = "";
			for(int i = 0; i < Effects.Count; i++)
			{
				if(Effects[i].Duration == -1) effectpref += " " + Effects[i].Description[0].Value;
			}
			return new StCon(valpref + effectpref + " Mana", GameData.Colour(Genus));}
	}

}
