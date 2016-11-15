using UnityEngine;
using System.Collections;

public class Lens : Tile {

	public float Multiplier
	{
		get{
			CheckStats();
			return 2.0F + (Stats.Value * 0.2F);
		}
	}

	public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon("Doubles row's values", GameData.Colour(GENUS.WIS), true, 40)
			};
		}
	}


	public override IEnumerator BeforeMatch(Tile Controller)
	{
		for(int xx = 0; xx < TileMaster.Grid.Size[0]; xx++)
		{
			Tile targ = TileMaster.Tiles[xx, this.y];
			if(targ == this) continue;
			int val = (int) ((float)targ.Stats.Value * Multiplier) - targ.Stats.Value;
				targ.AddValue(val);
				GameObject part = EffectManager.instance.PlayEffect(targ.transform, "collect", GameData.instance.GetGENUSColour(targ.Genus));
				part.GetComponent<DestroyTimer>().Timer = 0.5F;
			yield return new WaitForSeconds(Time.deltaTime * 5);
		}
		yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		yield return null;
	}

	public void AddValueToTarg(Tile targ)
	{
		GameObject cast = GameData.instance.ActionCaster(this.transform, targ, () =>
			{
				int val = (int) ((float)targ.Stats.Value * Multiplier) - targ.Stats.Value;
				targ.AddValue(val);
				GameObject part = EffectManager.instance.PlayEffect(targ.transform, "collect", GameData.instance.GetGENUSColour(targ.Genus));
				part.GetComponent<DestroyTimer>().Timer = 0.5F;
			});

	}
}
