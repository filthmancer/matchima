using UnityEngine;
using System.Collections;

public class Ward : Tile {
	public string Buff;
	public int Duration = 1;
	private int PowerInc = 5;

	public override IEnumerator AfterTurnRoutine()
	{
		if(Destroyed) yield break;
		SetState(TileState.Selected, true);
		yield return new WaitForSeconds(0.3F);
		SetState(TileState.Idle, true);
		int num = (int)Genus;
		if(num > 3)
		{

		}
		else Player.instance.AddStatus((int)Genus, Buff, 2, (PowerInc) + "");
		yield return null;
	}
}
