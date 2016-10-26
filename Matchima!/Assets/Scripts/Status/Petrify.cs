using UnityEngine;
using System.Collections;

public class Petrify : TileEffect {
	
	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Petrified", GameData.Colour(GENUS.OMG), false),
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
		PetrifyTime = GameData.StringToInt(args[0]);
		Duration = PetrifyTime+1;
		//StoneformDelay = GameData.StringToInt(args[1]);
	}

	public int PetrifyTime;
	//public int StoneformDelay = 1;

	private int PetrifyTime_current=0;// StoneformDelay_current=0;
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

			PetrifyTime_current ++;
			if(PetrifyTime_current >= PetrifyTime)
			{
				_Tile.ChangeGenus(initial_genus);
				yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
			}
			else if(_Tile.Genus == GENUS.OMG)
			{
				UIManager.instance.MiniAlert(_Tile.Point.targetPos, (PetrifyTime - PetrifyTime_current) + "", 190, GameData.Colour(_Tile.Genus), 0.4F, 0.01F);
				yield return new WaitForSeconds(GameData.GameSpeed(0.3F));	
			}
		}
		yield break;
	}

	public override bool CheckDuration()
	{
		if(Duration > 0) Duration -= 1;
		if(PetrifyTime_current >= PetrifyTime) return true;
		if(_Tile.Genus != GENUS.OMG) return true;
		
		return Duration == 0;
	}

	public override bool CanAttack()
	{
		return _Tile.Genus != GENUS.OMG;
	}
}
