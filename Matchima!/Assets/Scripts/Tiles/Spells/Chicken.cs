using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chicken : Tile {

	[SerializeField]
	private bool scared;
	private bool isleaving = false;
	private int scared_count = 0;
	private float idle_audio_time = 4.5F;
	public override StCon [] Description
	{
		get{
			return new StCon[]{new StCon("Disappears when hitting the bottom row", Color.white,true, 40)};
		}
	}

	public override void Update()
	{
		base.Update();
		if(GameManager.inStartMenu) return;
		if((idle_audio_time -= Time.deltaTime) <= 0.0F)
		{
			PlayAudio("idle");
			idle_audio_time = Random.Range(6.5F, 20.0F);
		}
		if(Point.Base[1] == 0 && !isleaving)
		{
			StartCoroutine(GoAway());
			isleaving = true;
		}
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
			PlayAudio("touch");
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
		//while(IsState(TileState.Falling) && !Destroyed) yield return null;
		//while(Destroyed) yield return null;
		//yield return new WaitForSeconds(0.05F);
		Animate("Attack");
		PlayAudio("death");
		yield return new WaitForSeconds(0.05F);
		DestroyThyself();
		TileMaster.instance.SetFillGrid(true);
		yield return null;
	}

	public override bool Match(int resource)
	{
		if(this == null) return false;
		scared = true;
		return false;
	}

}
