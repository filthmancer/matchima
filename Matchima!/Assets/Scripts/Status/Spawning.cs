using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawning : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Spawning", GameData.Colour(_Tile.Genus), false),
				new StCon(DurationString)
			};
		}
	}
	public string Genus, Species;
	public float AddedValueRatio = 0.2F;
	public override void GetArgs(int _duration, params string [] args)
	{
		Duration = _duration;
		Genus = args[0];
		Species = args[1];
		AddedValueRatio = GameData.StringToFloat(args[2]);
	}

	public virtual void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.AfterTurnEffect = true;
	}

	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0)
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
					!child.IsType(final_species)
				) 
				final.Add(child);
			}

			int added_value = (int) ((float)_Tile.Stats.Value * AddedValueRatio);
			for(int i = 0; i < final.Count; i++)
			{
				if(Genus == "Random") final_genus =  GameData.ResourceLong((GENUS)Random.Range(0,4));
				else if(Genus == "RandomAll") final_genus =  GameData.ResourceLong((GENUS)Random.Range(0,7));
				
				Tile t = TileMaster.instance.ReplaceTile(final[i], TileMaster.Types[final_species], TileMaster.Genus[final_genus], 1, 
												added_value);

				foreach(TileEffect child in _Tile.Effects)
				{
					if(child == this) continue;
					TileEffect neweff = (TileEffect) Instantiate(child);
					t.AddEffect(neweff);
				}
				yield return new WaitForSeconds(Time.deltaTime*2);
			}
		}
	}
}
