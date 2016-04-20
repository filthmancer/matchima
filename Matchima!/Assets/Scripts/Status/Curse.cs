using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Curse : TileEffect {
	public int AttackReduction;
	public int HealthReduction;

	public override StCon [] Description
	{
		get{
			List<StCon> st = new List<StCon>();
			if(AttackReduction > 0) st.Add(new StCon("-" + AttackReduction + " Attack " + DurationString, GameData.Colour(GENUS.STR),false));
			if(HealthReduction > 0) st.Add(new StCon("-" + HealthReduction + " Health " + DurationString, GameData.Colour(GENUS.STR),false));
			st.Add(new StCon(DurationString));

			return st.ToArray();
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.Stats.Attack -= AttackReduction;
		_Tile.Stats.Hits  -= HealthReduction;
	}

	public override void GetArgs(int _dur, params string [] args)
	{
		base.GetArgs(_dur,args);
		AttackReduction = GameData.StringToInt(args[0]);
		HealthReduction = GameData.StringToInt(args[1]);
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			_Tile.Stats.Attack -= AttackReduction;
			_Tile.Stats.Hits  -= HealthReduction;
		}
		return Duration == 0;
	}
}
