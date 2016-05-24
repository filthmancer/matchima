using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chicken : Tile {

	[SerializeField]
	private bool scared;
	private bool isleaving = false;
	private int scared_count = 0;
	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Disappears when hitting the bottom row")};
		}
	}

	public override void Update()
	{
		base.Update();
		//if(GameManager.inStartMenu) return;
		//if(Point.Base[1] == 0 && !isleaving && !IsState(TileState.Falling))
		//{
		//	StartCoroutine(GoAway());
		//	isleaving = true;
		//}
	}

	public void SetScare(bool _scared)
	{
		scared = _scared;
	}

	public override IEnumerator AfterTurnRoutine()
	{
		yield return StartCoroutine(base.AfterTurnRoutine());
		Reset();
		if(scared)
		{
			scared = false;
			scared_count ++;
			if(scared_count < 3)
			{
				CheckStats();
				Tile [] nbours = Point.GetNeighbours();
				List<Tile> final = new List<Tile>();
				foreach(Tile child in nbours)
				{
					if(!child.isMatching && child.Point.Scale == Point.Scale) final.Add(child);
				}

				Tile target = final[Random.Range(0, final.Count)];
				TileMaster.instance.SwapTiles(target, this);
			}
			else 
			{
				for(int i = 0; i < 5; i++)
				{
					TileMaster.instance.QueueTile(TileMaster.Types["chicken"], GENUS.OMG, 5);
				}
				scared_count = 0;
			}
		}

		if(Point.Base[1] == 0)	
		{
			yield return StartCoroutine(GoAway());
			isleaving = true;
		}
		else yield break;
	}

	IEnumerator GoAway()
	{
		yield return new WaitForSeconds(0.2F);
		while(Destroyed) yield return null;
		//yield return new WaitForSeconds(0.05F);
		//Animate("Attack");
		
		DestroyThyself();
		yield return null;
	}

	public override bool Match(int resource)
	{
		if(this == null) return false;
		scared = true;
		return false;
	}

}
