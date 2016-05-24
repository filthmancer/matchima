﻿using UnityEngine;
using System.Collections;

public class Target : Ability {

	public override void Start()
	{
		base.Start();
		Description_Basic = "If alive, draws enemy aggro";
	}

	public override IEnumerator AfterTurn()
	{
		base.AfterTurn();
		foreach(Class child in Player.Classes)
		{
			if(child == Parent) continue;
			//if(child.AggroValue > Parent.AggroValue) Parent.AggroValue = child.AggroValue + 0.1F;
		}
		//Player.instance.GetAggroOrder();

		yield return null;
	}

}