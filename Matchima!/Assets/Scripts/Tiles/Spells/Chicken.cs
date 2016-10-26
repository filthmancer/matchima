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
			return new StCon[]{new StCon("Destroyed at bottom row", Color.white, true, 30)};
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
		/*if(Point.Base[1] == 0 && !isleaving)
		{
			StartCoroutine(GoAway());
			isleaving = true;
		}*/
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
				if(Point.Scale == 1)
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
					yield return StartCoroutine(GameData.DeltaWait(0.2F));
				}
				else
				{
					CheckStats();
					Tile [] nbours = Point.GetNeighbours();
					TileMaster.instance.ReplaceTile(nbours[Random.Range(0,nbours.Length)], TileMaster.Types["chicken"], GENUS.OMG, 1);
				}
				
			}
			else 
			{
				int skip_col = Random.Range(0, TileMaster.Grid.Size[0]-1);
				for(int i = 0; i < TileMaster.Grid.Size[0]; i++)
				{
					if(i == skip_col) continue;
					TileMaster.instance.ReplaceTile(TileMaster.Tiles[i,TileMaster.Grid.Size[1]-1], TileMaster.Types["chicken"], GENUS.OMG, 5);
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
		//while(IsState(TileState.Falling) && !Destroyed) yield return null;
		//while(Destroyed) yield return null;
		//yield return new WaitForSeconds(0.05F);
		Animate("Attack");
		PlayAudio("death");
		yield return new WaitForSeconds(0.2F);
		DestroyThyself(true);
		TileMaster.instance.SetFillGrid(true);
		yield return null;
	}

	public override bool Match(int resource)
	{
		if(this == null) return false;
		scared = true;
		return false;
	}

	public override void SetSprite()
	{
		string render = Info._GenusName;
		if(Point.Scale > 1) render += "_2";
		SetBorder(Info.Outer);
		SetRender(render);
		
		//transform.position = new Vector3(Point.targetPos.x, Point.targetPos.y, transform.position.z);
		Params.transform.position = transform.position;
		Params._render.transform.localPosition = Vector3.zero;
	}

}
