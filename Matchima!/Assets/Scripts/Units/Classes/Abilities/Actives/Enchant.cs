using UnityEngine;
using System.Collections;

public class Enchant : Ability {

	public int levelNum;
	public string statType;

	private int _defaultLevelNum;


	public override StCon [] Description_Tooltip
	{
		get
		{
			return new StCon[]
			{
				new StCon("Grants a free Boon")
			};
		}
	}
	public override void Start()
	{
		base.Start();
		_defaultLevelNum = levelNum;
		Description_Basic = "Grants " + levelNum + " levels of " + statType;
	}

	public override void Update()
	{
		base.Update();
		levelNum = _defaultLevelNum + (int)StatBonus();
		Description_Basic = "Grants " + levelNum + " levels of " + statType;
	}
	
	public override void Activate()
	{
		if(cooldown_time > 0) return;
		if(!CanAfford()) return;
		activated = true;
		cooldown_time = cooldown;
		
		Player.instance.LevelUp();
	}

	public override void SetStatsRandom()
	{
		levelNum = 1;
		int random = Random.Range(0,3);
		string [] types = new string[3] {"Gold", "Exp", "Shield"};
		statType = types[random];

		_defaultLevelNum = levelNum;
	}
}