using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shield : Ability {

	public float InitialShield = 1;

	public float TotalShield
	{
		get{
			return (InitialShield * CharismaFactor);
		}
	}

	public override int OnHit(int Hit, params Tile[] attackers)
	{
		if(!activated) return Hit;
		
		int shield_hit = (int)((float) Hit*TotalShield);
		Parent.AddToMeter(-shield_hit);

		int final_hit = Hit - shield_hit;
		return final_hit;
	}


	public override void SetArgs(params string [] args)
	{
		InitialShield = GameData.StringToInt(args[0]);
	}

}
