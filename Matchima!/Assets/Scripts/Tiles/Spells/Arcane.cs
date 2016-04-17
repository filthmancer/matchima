using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arcane : Tile {

	public string InputGenus, InputType;

	public string EndGenus
	{
		get
		{
			if(InputGenus == "Random") return GameData.ResourceLong((GENUS)Random.Range(0,4));
			else if(InputGenus == "RandomAll") return GameData.ResourceLong((GENUS)Random.Range(0,7));
			else if(InputGenus == "Genus") return GameData.ResourceLong(Genus);
			else return InputGenus;
		}
	}

	public string EndType
	{
		get
		{
			if(InputType == "Random") return "Resource";
			else return InputType;
		}
	}
	public int EndValueAdded = 0;
	public int TilesCollected
	{
		get
		{
			CheckStats();
			return 1 + Stats.Value/2;
		}
	}

	public GameObject ArcaneParticle;
	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Changes " + TilesCollected + (TilesCollected > 1 ? " tiles" : " tile")
										 + " to " + EndGenus + " " + EndType)};
		}
	}
	public override void Setup(int x, int y, int scale, TileInfo inf, int value_inc)
	{
		base.Setup(x, y, scale, inf, value_inc);
		
	}

	public override IEnumerator BeforeMatch()
	{
		float part_time = 0.6F;
		if(isMatching) yield break;

		List<Tile> to_collect = new List<Tile>();
		int x = TileMaster.Tiles.GetLength(0);
		int y = TileMaster.Tiles.GetLength(1);

		for(int xx = 0; xx < x; xx ++)
		{
			for(int yy = 0; yy < y; yy++)
			{
				if(TileMaster.Tiles[xx,yy].IsGenus(GENUS.OMG)) to_collect.Add(TileMaster.Tiles[xx,yy]);
			}
		}

		
		while(to_collect.Count > TilesCollected)
		{
			to_collect.RemoveAt(Random.Range(0, to_collect.Count));
		}

		foreach(Tile child in to_collect)
		{
			child.SetState(TileState.Selected, true);

			GameObject part = Instantiate(ArcaneParticle);
			part.transform.position = this.transform.position;
			part.GetComponent<MoveToPoint>().SetTarget(child.transform.position);
			part.GetComponent<MoveToPoint>().SetPath(0.5F, true);
			part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);

			float dist = Vector3.Distance(child.transform.position, this.transform.position);
			part.GetComponent<MoveToPoint>().Speed = 0.05F * dist;
			part_time = 0.1F + (0.03F * dist);

			
			yield return new WaitForSeconds(part_time);

			if(EndType == string.Empty)
			{
				child.ChangeGenus(Genus);	
				child.AddValue(EndValueAdded);
			}
			else if(EndGenus == string.Empty)
			{
				TileMaster.instance.ReplaceTile(child, TileMaster.Types[EndType], child.Genus, 1, EndValueAdded);
			}
			else TileMaster.instance.ReplaceTile(child, TileMaster.Types[EndType], TileMaster.Genus[EndGenus], 1, EndValueAdded);
			
			EffectManager.instance.PlayEffect(child.transform, Effect.Replace, "", GameData.instance.GetGENUSColour(child.Genus));	
		}

		int check = 0;
		while(to_collect.Count < TilesCollected)
		{
			Tile c = TileMaster.Tiles[Utility.RandomInt(x), Utility.RandomInt(y)];
			if(c.Genus != Genus)
			{
				c.SetState(TileState.Selected, true);
				to_collect.Add(c);	

				GameObject part = Instantiate(ArcaneParticle);
				part.transform.position = this.transform.position;
				part.GetComponent<MoveToPoint>().SetTarget(c.transform.position);
				part.GetComponent<MoveToPoint>().SetPath(0.5F, true);
				part.GetComponent<ParticleSystem>().startColor = GameData.Colour(Genus);

				float dist = Vector3.Distance(c.transform.position, this.transform.position);
				part.GetComponent<MoveToPoint>().Speed = 0.05F * dist;
				part_time = 0.1F + (0.03F * dist);

				
				yield return new WaitForSeconds(part_time);
				if(EndType == string.Empty)
				{
					c.ChangeGenus(Genus);	
					c.AddValue(EndValueAdded);
				}
				else if(EndGenus == string.Empty)
				{
					TileMaster.instance.ReplaceTile(c, TileMaster.Types[EndType], c.Genus, 1, EndValueAdded);
				}
				else TileMaster.instance.ReplaceTile(c, TileMaster.Types[EndType], TileMaster.Genus[EndGenus], 1, EndValueAdded);
				
				EffectManager.instance.PlayEffect(c.transform, Effect.Replace, "", GameData.instance.GetGENUSColour(c.Genus));	
				}
			else 
			{
				if(check >= (x*y)) break;
				if(check % 10 == 0) yield return null;
			}
			check++;
		}
		
		yield return new WaitForSeconds(Time.deltaTime * 5);
		foreach(Tile child in to_collect)
		{
			child.Reset(true);
		}
		yield return null;
	}

}
