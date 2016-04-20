﻿using UnityEngine;
using System.Collections;

public class Ward : Tile {
	public string Buff;
	public int Duration_init;
	
	public int Timer = 1;
	private string [] args;

	private int timer_current;
	private int duration_added = 0;

	private string target_name
	{
		get{
			int g = (int) Genus;
			if(g > 3) return "All Units";
			else 
			{
				Class c = Player.Classes[g];
				if(c != null) return c.Name;
				else return GameData.ResourceLong(Genus) + " Unit";
			}
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Casts ", Color.white, false),
				new StCon(Buff, GameData.Colour(Genus), false),
				new StCon(" on " + target_name)
			};
		}
	}

	public int Duration
	{
		get
		{
			return Duration_init + duration_added;
		}
	}

	protected override TileUpgrade [] AddedUpgrades
	{
		get{
			return new TileUpgrade []
			{
				new TileUpgrade(0.1F, 2, () => {duration_added++;})
			};
		}
	}
 
	public override IEnumerator AfterTurnRoutine()
	{
		if(Destroyed) yield break;
		timer_current -= 1;
		if(timer_current > 0) yield break;
		else timer_current = Timer;
		SetState(TileState.Selected, true);
		yield return new WaitForSeconds(0.3F);
		SetState(TileState.Idle, true);
		int num = (int)Genus;
		if(num > 3)
		{

		}
		else 
		{
			GameObject part = EffectManager.instance.PlayEffect(this.transform, Effect.Force, "", GameData.instance.GetGENUSColour(Genus));
			MoveToPoint mp = part.GetComponent<MoveToPoint>();
			mp.enabled = true;
			mp.SetTarget(UIManager.ClassButtons[(int)Genus].transform.position);
			mp.SetPath(0.3F, 0.02F);
			mp.SetMethod(() => {
					Player.instance.AddStatus((int)Genus, Buff, Duration, args);
				});
		}
			
		yield return null;
	}

	public override void SetArgs(params string [] _args)
	{
		Buff = _args[0];
		Duration_init = GameData.StringToInt(_args[1]);
		Timer = GameData.StringToInt(_args[2]);
		args = new string[_args.Length-3];
		for(int i = 0; i < _args.Length-3; i++)
		{
			args[i] = _args[i+3];
		}
	}
}
