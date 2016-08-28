using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bard : Class {

	TileChance harp;

	bool warcry_a, warcry_b, warcry_c;
	private int _currentmanapower;

	public override StCon [] _Desc
	{
		get
		{
			List<StCon> final = new List<StCon>();
			final.Add(new StCon(Meter + "/" + MeterTop + " Mana", GameData.Colour(Genus)));
			final.Add(new StCon("Lvl " + Level + " ("+ Exp_Current + "/" + Exp_Max + " xp)", GameData.Colour(GENUS.WIS)));

			for(int i = 0; i < Stats.Length; i++)
			{
				bool last = i==Stats.Length-1;
				final.Add(new StCon(Stats[i].StatCurrent+"", GameData.Colour((GENUS)i), last));
				if(!last) final.Add(new StCon(" /", Color.white, false));
			}
			
			if(warcry_a) final.Add(new StCon("Applies Sleep to 1 Enemy Per Turn", Color.white));
			if(warcry_b) final.Add(new StCon("Applies Charm to 1 Enemy Per Turn", Color.white));
			if(warcry_c) final.Add(new StCon("Applies Sleep to 1 Enemy Per Turn", Color.white));
			foreach(Slot child in AllMods)
			{
				if(child != null) final.AddRange(child.Description_Tooltip);
			}
			
			foreach(ClassEffect child in _Status)
			{
				final.AddRange(child.Description);
			}
				
			return final.ToArray();
		}
	}
	public override void StartClass () {

		harp = new TileChance();
		harp.Genus = GameData.ResourceLong(Genus);
		harp.Type = "harp";
		harp.Chance = 0.13F;
		InitStats.TileChances.Add(harp);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.03F;
		InitStats.TileChances.Add(health);

		PowerupSpell = GameData.instance.GetPowerup("Lullaby", this);

		base.StartClass();
	}
}
