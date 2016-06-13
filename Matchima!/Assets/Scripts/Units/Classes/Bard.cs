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
		harp.Chance = 0.1F;
		InitStats.TileChances.Add(harp);

		base.StartClass();
	}


	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["harp"], g, 1, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["collector"], g, 1, points);	
				(TileMaster.Tiles[x,y] as Collector).Type = "";
			break;
			case 2:

				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["ward"], g, 1, points);	
				TileMaster.Tiles[x,y].SetArgs("Healing", "1", "2", "5");
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["arcane"], g, 1, points/2);	
				TileMaster.Tiles[x,y].SetArgs("Genus", "resource", "", "Harp");
			break;
			case 4:

			break;
		}
	}


	public override IEnumerator EndTurn()
	{

		if(warcry_a) yield return StartCoroutine(WarcryA());
		if(warcry_b) yield return StartCoroutine(WarcryB());
		if(warcry_c) yield return StartCoroutine(WarcryC());
		yield return StartCoroutine(base.EndTurn());
	}


	public override void ManaPower(int lvl)
	{
		switch(lvl)
		{
			case 0:
				warcry_a = false;
				warcry_b = false;
				warcry_c = false;
			break;
			case 1:
				warcry_a = true;
				warcry_b = false;
				warcry_c = false;
			break;
			case 2:
				warcry_a = false;
				warcry_b = true;
				warcry_c = false;
			break;
			case 3:
				warcry_a = false;
				warcry_b = false;
				warcry_c = true;
			break;
		}

	}

	IEnumerator WarcryA()
	{
		List<Tile> targets = new List<Tile>();
		for(int x = 0; x < TileMaster.Grid.Size[0];x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				Tile t = TileMaster.Tiles[x,y];
				if(t == null) continue;
				if(t.Type.isEnemy && !t.Stats.isAlly)
				{
					targets.Add(t);
				}
			}
		}
		if(targets.Count == 0) yield break;

		int c = Random.Range(0,targets.Count);
		Tile charmtarget = targets[c];
		targets.RemoveAt(c);
		if(charmtarget == null) yield break;

		UIManager.ClassButtons[Index].ShowClass(true);
		UIManager.instance.MiniAlert(UIManager.ClassButtons[Index].transform.position + Vector3.up, "Sleep", 55, GameData.Colour(Genus), 1.2F, 0.25F);

		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Genus].transform, Effect.Force);
		MoveToPoint charm = initpart.GetComponent<MoveToPoint>();
		charm.SetTarget(charmtarget.transform.position);
		charm.SetPath(0.35F, 0.3F);
		charm.Target_Tile = charmtarget;
		charm.SetThreshold(0.15F);
		charm.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(charm.Target_Tile.Point.targetPos, "Sleep", 55, GameData.Colour(charm.Target_Tile.Genus), 1.2F, 0.1F);
			charm.Target_Tile.AddEffect("Sleep", 2);
		});

		yield return new WaitForSeconds(0.3F);

	}

	IEnumerator WarcryB()
	{
		List<Tile> targets = new List<Tile>();
		for(int x = 0; x < TileMaster.Grid.Size[0];x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				Tile t = TileMaster.Tiles[x,y];
				if(t == null) continue;
				if(t.Type.isEnemy && !t.Stats.isAlly)
				{
					targets.Add(t);
				}
			}
		}

		if(targets.Count == 0) yield break;
		UIManager.ClassButtons[Index].ShowClass(true);
		UIManager.instance.MiniAlert(UIManager.ClassButtons[Index].transform.position + Vector3.up, 
													"Charm", 55, GameData.Colour(Genus), 1.2F, 0.25F);

		Tile sleeptarget = targets[Random.Range(0, targets.Count)];
		if(sleeptarget == null) yield break;
		GameObject initsleep = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Genus].transform, Effect.Force);
		MoveToPoint sleep = initsleep.GetComponent<MoveToPoint>();
		sleep.SetTarget(sleeptarget.transform.position);
		sleep.SetPath(0.35F, 0.3F);
		sleep.Target_Tile = sleeptarget;
		sleep.SetThreshold(0.15F);
		sleep.SetMethod(() =>
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(sleep.Target_Tile.Point.targetPos, "Charm", 55, GameData.Colour(sleep.Target_Tile.Genus), 1.2F, 0.1F);
			sleep.Target_Tile.AddEffect("Charm", 3);
			
		});
		yield return new WaitForSeconds(0.3F);
	}


	IEnumerator WarcryC()
	{
		List<Tile> targets = new List<Tile>();
		for(int x = 0; x < TileMaster.Grid.Size[0];x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				Tile t = TileMaster.Tiles[x,y];
				if(t == null) continue;
				if(t.Type.isEnemy && !t.Stats.isAlly)
				{
					targets.Add(t);
				}
			}
		}
		
		if(targets.Count == 0) yield break;
		UIManager.ClassButtons[Index].ShowClass(true);
			Tile sleeptarget = targets[Random.Range(0, targets.Count)];
			if(sleeptarget == null) yield break;
			GameObject initsleep = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Genus].transform, Effect.Force);
			MoveToPoint sleep = initsleep.GetComponent<MoveToPoint>();
			sleep.SetTarget(sleeptarget.transform.position);
			sleep.SetPath(0.35F, 0.3F);
			sleep.Target_Tile = sleeptarget;
			sleep.SetThreshold(0.15F);
			sleep.SetMethod(() =>
			{
				MiniAlertUI m = UIManager.instance.MiniAlert(sleep.Target_Tile.Point.targetPos, "Sleep", 55, GameData.Colour(sleep.Target_Tile.Genus), 1.2F, 0.1F);
				sleep.Target_Tile.AddEffect("Sleep", 3);
			});
		yield return new WaitForSeconds(0.3F);
	}
}
