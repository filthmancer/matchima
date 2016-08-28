using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slash : Powerup {
	public GameObject Particle;
	int [] slash_num = new int [3]{1,1,1};
	protected override IEnumerator Minigame(int level)
	{
		yield return StartCoroutine(PowerupStartup());

		int slashes = slash_num[level];
		int leafs_hit = 0;
		int ypos = 0;

		UIObj leaf = CreateMinigameObj(0);
		leaf.AddAction(UIAction.MouseDown, () =>
		{
			leafs_hit ++;
			MiniAlertUI alert  = UIManager.instance.MiniAlert(leaf.transform.position,
			"Good!", 100, GameData.Colour(Parent.Genus), 0.3F, 0.2F);
			alert.AddJuice(Juice.instance.BounceB, 0.1F);
			ypos = ClosestPoint(leaf.transform.position);
			Destroy(leaf.gameObject);

		});
		MoveToPoint mp = leaf.GetComponent<MoveToPoint>(); 
		mp.enabled = true;
		mp.SetTarget(UIManager.Objects.BotGear.transform.position);
		mp.SetPath(0.2F * CameraUtility.OrthoFactor, 0.0F, 0.0F);


		while(leaf!=null)
		{
			yield return null;
		}

		GameObject [] part_x = new GameObject[leafs_hit];
		List<Tile> to_collect = new List<Tile>();
		for(int i = 0; i < leafs_hit; i++)
		{
			part_x[i] = Instantiate(Particle);
			Vector3 pos = TileMaster.Tiles[0,ypos].transform.position + Vector3.back * 2;
			pos.x = -5;
			part_x[i].transform.position = pos;
			for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
			{
				TileMaster.Tiles[x,ypos].SetState(TileState.Selected, true);
				if(!TileMaster.Tiles[x, ypos].IsGenus(GENUS.OMG)) 
					to_collect.Add(TileMaster.Tiles[x,ypos]);
			}
		}

	}

	int ClosestPoint(Vector3 pos)
	{
		float closest_dist = 100.0F;
		int closest = 100;
		for(int i = 0; i < TileMaster.Grid.Size[0]; i++)
		{
			float d = Vector3.Distance(TileMaster.Tiles[i,0].transform.position, pos);
			if(d < closest_dist)
			{
				closest = i;
				closest_dist = d;
			}
		}
		return closest;
	}

}
