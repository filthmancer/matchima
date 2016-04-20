using UnityEngine;
using System.Collections;

public class AntiGravity : TileEffect {
	public ShiftType Shift;

	public override StCon [] Description
	{
		get{
			return new StCon[] {
				new StCon((Shift == ShiftType.None ? "Tile is not affected by gravity " : "Tile gravity is " + Shift) + " " + DurationString, GameData.Colour(GENUS.PRP))};
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
		t.Stats.Shift = Shift;
	}

	public override void GetArgs(int _dur, params string [] args)
	{
		base.GetArgs(_dur, args);
		Shift = (ShiftType) GameData.StringToInt(args[0]);
	}


	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration != 0)
		{
			_Tile.Stats.Shift = Shift;
		}
		return Duration == 0;
	}

}
