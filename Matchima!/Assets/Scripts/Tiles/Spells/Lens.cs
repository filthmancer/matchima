using UnityEngine;
using System.Collections;

public class Lens : Tile {

	float multiplier = 2.0F;

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Increases value\nof matched tiles.", GameData.Colour(Genus), true, 40)
			};
		}
	}


	public override void Update()
	{
		base.Update();
		if(Params._render != null) Params._render.color = Color.Lerp(Params._render.color, GameData.Colour(Genus), 0.6F);
	}

	public override IEnumerator BeforeMatch(bool original, int Damage = 0)
	{
		foreach(Tile child in PlayerControl.instance.selectedTiles)
		{
			if(child == this) continue;
			if(child.IsType("Vacuum")) break;
			int val = (int) ((float)child.Stats.Value * multiplier) - child.Stats.Value;
			child.AddValue(val);
			GameObject part = EffectManager.instance.PlayEffect(child.transform, Effect.Shiny, GameData.instance.GetGENUSColour(child.Genus));
			part.GetComponent<DestroyTimer>().Timer = 0.5F;
			yield return new WaitForSeconds(Time.deltaTime * 5);
		}
		yield return null;
	}
}
