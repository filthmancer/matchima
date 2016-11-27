using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawning : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Spawning", GameData.Colour(_Tile.Genus), false)
			};
		}
	}
	public string Genus, Species;
	public float AddedValueRatio = 0.2F;
	public int SpawnPerTurn = 1;
	public override void GetArgs(int _duration, params string [] args)
	{
		base.GetArgs(_duration, args);
		Genus = args[0];
		Species = args[1];
		SpawnPerTurn = GameData.StringToInt(args[2]);
		AddedValueRatio = GameData.StringToFloat(args[3]);
	}

	bool setupturn = false;
	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.AfterTurnEffect = true;
		setupturn = true;
	}

	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0 && !setupturn)
		{
			
			Tile [] nbours = _Tile.Point.GetNeighbours();
			List<Tile> final = new List<Tile>();

			string final_genus = Genus, final_species = Species;
			
			if(Genus == "Genus") final_genus =  _Tile.Info._GenusName;
			if(Species == "Species") final_species =  _Tile.Info._TypeName;


			foreach(Tile child in nbours)
			{
				if(!child.isMatching && 
					child.Point.Scale == _Tile.Point.Scale &&
					!child.IsType(final_species) && !child.Controllable && !child.IsGenus(GENUS.OMG)
				) 
				final.Add(child);
			}

			int added_value = (int) ((float)_Tile.Stats.Value * AddedValueRatio);
			for(int i = 0; i < SpawnPerTurn; i++)
			{
				if(final.Count <= 0) continue;

				if(Genus == "Random") final_genus =  GameData.ResourceLong((GENUS)Random.Range(0,4));
				else if(Genus == "RandomAll") final_genus =  GameData.ResourceLong((GENUS)Random.Range(0,7));
				
				int targ_num = Random.Range(0, final.Count);
				Tile targ = final[targ_num];
				Tile t = TileMaster.instance.ReplaceTile(targ, TileMaster.Types[final_species], TileMaster.Genus[final_genus], 1, 
												added_value);

				foreach(TileEffect child in _Tile.Effects)
				{
					if(child == this || child.DontInherit) continue;
					
					TileEffect neweff = (TileEffect) Instantiate(child);
					t.AddEffect(neweff);
				}
				final.RemoveAt(targ_num);
				yield return new WaitForSeconds(Time.deltaTime*2);
			}
		}
		else setupturn = false;
	}
}
