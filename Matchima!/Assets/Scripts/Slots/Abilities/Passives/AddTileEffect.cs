using UnityEngine;
using System.Collections;

public class AddTileEffect : Ability {
	public float Chance = 1.0F;
	public string TargetGenus;
	public string TargetSpecies;
	public string _TileEffect;
	public int Duration;

	public string [] _args;

	public override IEnumerator AfterTurn()
	{
		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		GameObject part = Instantiate(Particle);
		part.transform.position = UIManager.ClassButtons.GetClass(Parent.Index).transform.position;

		yield return new WaitForSeconds(Time.deltaTime * 30);
		
		for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
		{
			for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
			{
				Tile t = TileMaster.Tiles[x,y];
				if(t.IsType(TargetGenus, TargetSpecies) && Random.value < Chance)
				{
					t.SetState(TileState.Selected, true);
					MiniAlertUI m = UIManager.instance.MiniAlert(t.Point.targetPos, _TileEffect, 55, GameData.Colour(t.Genus), 1.2F, 0.1F);
					t.AddEffect(_TileEffect, Duration, _args);
				}
			}
		}
		TileMaster.instance.ResetTiles();
		yield return new WaitForSeconds(Time.deltaTime * 20);
	}

	public override void SetArgs(params string [] args)
	{
		TargetGenus = args[0];
		TargetSpecies = args[1];
		_TileEffect = args[2];
		Duration = GameData.StringToInt(args[3]);
		Chance = GameData.StringToFloat(args[4]);

		if(args.Length > 5)
		{
			_args = new string [args.Length-5];
			for(int x = 5; x < args.Length; x++)
			{
				_args[x-5] = args[x];
			}
		}
		
	}
}
