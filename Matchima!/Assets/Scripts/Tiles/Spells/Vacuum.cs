using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vacuum : Tile {

	bool magnify;
	bool undead_attack;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Sucks value of\nmatched tiles\ninto last tile\nof match.", GameData.Colour(Genus))
			};
		}
	}

	public override void Update()
	{
		base.Update();
		if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, GameData.Colour(Genus), 0.6F);
	}

	public override IEnumerator BeforeMatch(Tile Controller)
	{
		List<Tile> to_collect = new List<Tile>();
		Tile target = null;

		magnify = false;
		undead_attack = false;

		List<Tile> init = PlayerControl.instance.selectedTiles;
		for(int i = 0; i < init.Count; i++)
		{
			if(init[i] == null || init[i].isMatching) continue;
			if(init[i] == this) 
			{
				if(i < init.Count-1) 
				{
					target = init[i+1];
					if(target.IsType("lens"))
					{
						if(i < init.Count - 2)
						{
							magnify = true;
							target = init[i+2];
						}
					}
					else if(target.IsType("undead"))
					{
						undead_attack = true;
					}
				}
				else if(i == init.Count-1) target = this;
				break;
			}
			to_collect.Add(init[i]);
		}

		foreach(Tile child in to_collect)
		{
			int val = child.Stats.Value;
			child.isMatching = true;

			/*MoveToPoint mini = TileMaster.instance.CreateMiniTile(child.transform.position, target.transform,child.Inner);
			mini.SetMethod(() => 
			{
				if(undead_attack) target.InitStats.TurnDamage += val;
				else target.AddValue(val);
			});*/
			yield return new WaitForSeconds(Time.deltaTime * 5);
			PlayerControl.instance.RemoveTileToMatch(child);

			child.DestroyThyself(false);
		}
		PlayerControl.instance.RemoveTileToMatch(target);
		if(magnify) TileMaster.instance.ReplaceTile(target, target.Type, target.Genus, target.Point.Scale+1, target.Stats.Value);

		yield return new WaitForSeconds(Time.deltaTime * 10);
		yield return null;
	}
}
