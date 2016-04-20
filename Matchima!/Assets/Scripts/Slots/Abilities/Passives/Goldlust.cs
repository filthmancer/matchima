using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goldlust : Ability {

	public string LustGenus, LustSpecies;
	public int TilesToCooldownPoint = 3;
	public string LustType
	{
		get{
			return LustGenus + (LustSpecies == string.Empty ? "" : " " + LustSpecies);
		}
	}


	public override StCon [] Description_Tooltip
	{
		get{
			return new StCon[] {new StCon("Matching ", Color.white, false),
								new StCon(LustType, Color.white, false),
								new StCon(" lowers cooldown", Color.white)
							};
		}
	}



	public override void Start()
	{
		Description_Basic = "Matching " + LustType + " tiles decreases skill cooldown.";
		AfterMatchEffect = true;
	}

	public override IEnumerator AfterMatch()
	{
		int lusttiles = 0;
		foreach(Tile child in PlayerControl.instance.finalTiles)
		{
			if(child.IsType(LustGenus, LustSpecies)) lusttiles ++;
		}

		if(lusttiles == 0) yield break;

		int coolpoint = Mathf.Clamp(TilesToCooldownPoint - (int) StatBonus(), 1, 100);
		int cool_num = lusttiles/coolpoint;

		if(Player.Abilities.Length == 0) yield break;

		List<Ability> possible_abilities = new List<Ability>();
		foreach(Ability child in Player.Abilities)
		{
			if(child == this) continue;
			if(child is Ability && child.cooldown_time != 0) possible_abilities.Add((Ability)child);
		}

		if(possible_abilities.Count == 0) yield break;

		int act = Random.Range(0, possible_abilities.Count);
		possible_abilities[act].cooldown_time -= cool_num;

		yield break;
	}

	protected override void Setup()
	{
		base.Setup();
		if(RelativeStats)
		{
			LustGenus = GameData.ResourceLong(Parent.Genus);
		}
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

		LustGenus = _input.args[0];
		LustSpecies = _input.args[4];

		GENUS = (GENUS)GameData.StringToInt(_input.args[2]);

		name = _input.args[3];
		TilesToCooldownPoint = GameData.StringToInt(_input.args[5]);
	}
}