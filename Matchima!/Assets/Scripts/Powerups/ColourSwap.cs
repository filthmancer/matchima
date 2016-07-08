using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColourSwap : Powerup {

	public GENUS [] PreferredColours;
	

	protected override IEnumerator Minigame(int Level)
	{

		yield return StartCoroutine(Cast(Level));
	}

	IEnumerator Cast(int Level)
	{
		List<GENUS> prev = new List<GENUS>();
		for(int col = 0; col < Level; col++)
		{
			GENUS [] targets = GetTargets(prev.ToArray());
			GENUS StartGenus = targets[Random.Range(0, targets.Length)];
			prev.Add(StartGenus);

			Tile [,] _tiles = TileMaster.Tiles;
			for(int x = 0; x < _tiles.GetLength(0); x++)
			{
				for(int y = 0; y < _tiles.GetLength(1); y++)
				{
					if(_tiles[x,y] == null) continue;
					GENUS EndGenus = Parent.Genus;
					if(_tiles[x,y].IsGenus(StartGenus, false, false)) 
					{
						int old_value = _tiles[x,y].Stats.Value;
						_tiles[x,y].ChangeGenus(EndGenus);
						EffectManager.instance.PlayEffect(_tiles[x,y].transform, Effect.Replace, "", GameData.instance.GetGENUSColour(_tiles[x,y].Genus));
						
					}
				}
			}
		}
		yield return null;
	}

	public GENUS [] GetTargets(GENUS [] previous)
	{
		List<GENUS> final = new List<GENUS>();
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			if(Player.Classes[i] != Parent)
			{
				bool add = true;
				for(int x = 0; x < previous.Length; x++)
				{
					if(previous[x] == Player.Classes[i].Genus)
					{
						add = false;
						break;
					}
				}
				if(add) final.Add(Player.Classes[i].Genus);
			}
		}
		return final.ToArray();
	}
}
