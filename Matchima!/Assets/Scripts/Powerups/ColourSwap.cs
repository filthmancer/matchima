using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColourSwap : Powerup {

	public GENUS [] PreferredColours;
	

	protected override IEnumerator Minigame(int Level)
	{
		GameManager.instance.paused = true;
		yield return StartCoroutine(PowerupStartup());

		UIManager.instance.ScreenAlert.SetTween(0, false);
		yield return StartCoroutine(Cast(Level));

		yield return new WaitForSeconds(Time.deltaTime * 30);
		GameManager.instance.paused = false;
	}

	IEnumerator Cast(int Level)
	{
		List<GENUS> prev = new List<GENUS>();
		GENUS final = GetFinal();

		for(int col = 0; col < Level; col++)
		{
			GENUS [] targets = GetTargets(final, prev);
			GENUS StartGenus = targets[Random.Range(0, targets.Length)];

			if(col == 0)
			{
				StartGenus = Parent.Genus;
			}
			
			prev.Add(StartGenus);

			print(StartGenus);
			Tile [,] _tiles = TileMaster.Tiles;
			for(int x = 0; x < _tiles.GetLength(0); x++)
			{
				for(int y = 0; y < _tiles.GetLength(1); y++)
				{
					if(_tiles[x,y] == null) continue;
					if(_tiles[x,y].IsGenus(StartGenus, false, false)) 
					{
						int old_value = _tiles[x,y].Stats.Value;
						_tiles[x,y].ChangeGenus(final);
						EffectManager.instance.PlayEffect(_tiles[x,y].transform, "replace", GameData.instance.GetGENUSColour(_tiles[x,y].Genus));
						
					}
				}
			}
		}
		yield return null;
	}

	public GENUS [] GetTargets(GENUS fin, List<GENUS> previous)
	{
		List<GENUS> final = new List<GENUS>();
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			if(!previous.Contains(Player.Classes[i].Genus) && Player.Classes[i].Genus != fin)  final.Add(Player.Classes[i].Genus);
		}
		return final.ToArray();
	}

	public GENUS GetFinal()
	{
		List<GENUS> final = new List<GENUS>();
		for(int i = 0; i < Player.Classes.Length; i++)
		{
			if(Player.Classes[i] != Parent)  final.Add(Player.Classes[i].Genus);
		}
		return final[Random.Range(0, final.Count)];
	}
}
