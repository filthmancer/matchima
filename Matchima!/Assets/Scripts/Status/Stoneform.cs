using UnityEngine;
using System.Collections;

public class Stoneform : TileEffect {

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Stoneform", GameData.Colour(GENUS.OMG), false),
				new StCon(DurationString)
			};
		}
	}


	public override void Setup(Tile t)
	{
		base.Setup(t);
		_Tile.AfterTurnEffect = true;
		initial_genus = _Tile.Genus;
		_Tile.ChangeGenus(GENUS.OMG);
	}

	public override void GetArgs(int _duration, params string [] args)
	{
		base.GetArgs(_duration, args);
		StoneformTime = GameData.StringToInt(args[0]);
		StoneformDelay = GameData.StringToInt(args[1]);
	}

	public int StoneformTime;
	public int StoneformDelay = 1;

	private int StoneformTime_current=0, StoneformDelay_current=0;
	private GENUS initial_genus;

	public override IEnumerator StatusEffectRoutine()
	{
		if(Duration != 0)
		{
			if(_Tile.Destroyed) yield break;
			if(_Tile.HasEffect("Sleep")) yield break;
			while(!TileMaster.AllLanded) 
			{
				if(_Tile == null) yield break;
				if(_Tile.isMatching || _Tile.Destroyed) yield break;
				yield return null;
			}
			
			StoneformTime_current ++;
			if(StoneformTime_current >= StoneformTime)
			{
				StoneformDelay_current ++;
				if(StoneformDelay_current >= StoneformDelay)
				{
					StoneformTime_current = 0;
					StoneformDelay_current = 0;
					MiniAlertUI m = UIManager.instance.MiniAlert(_Tile.Point.targetPos, "Stone!", 120, GameData.Colour(_Tile.Genus), 0.4F, 0.1F);
					_Tile.ChangeGenus(GENUS.OMG);
					yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
				}
				else if(_Tile.Genus == GENUS.OMG)
				{
					_Tile.ChangeGenus(initial_genus);
					yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
				}
			}
			else 
			{
				if(_Tile.Genus != GENUS.OMG)
				{
					MiniAlertUI m = UIManager.instance.MiniAlert(_Tile.Point.targetPos, "Stone!", 120, GameData.Colour(_Tile.Genus), 0.4F, 0.1F);
					_Tile.ChangeGenus(GENUS.OMG);
					yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
				}
				else 
				{
					UIManager.instance.MiniAlert(_Tile.Point.targetPos, (StoneformTime - StoneformTime_current) + "", 120, GameData.Colour(_Tile.Genus), 0.4F, 0.1F);
					yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
				}	
			}
		}
		yield break;
	}

	public override bool CanAttack()
	{
		return _Tile.Genus != GENUS.OMG;
	}
}
