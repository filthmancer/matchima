using UnityEngine;
using System.Collections;

public class Maturation : Powerup {

	protected override IEnumerator Minigame(int level)
	{
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		//UIManager.CrewButtons[Parent.Index].ShowClass(true);
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform,  "powerupstart", GameData.Colour(Parent.Genus));
		powerup.transform.SetParent(UIManager.CrewButtons[Parent.Index].transform);
		powerup.transform.position = UIManager.CrewButtons[Parent.Index].transform.position;
		powerup.transform.localScale = Vector3.one;

		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			Parent._Name + " Casts", 70, GameData.Colour(Parent.Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Maturation", 170, GameData.Colour(Parent.Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 2,
			"Play the\nnotes!", 140, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		//UIManager.CrewButtons[Parent.Index].ShowClass(false);
		Destroy(powerup);


		/*Tile [,] _tiles = TileMaster.Tiles;
		if(_tiles == null || _tiles.GetLength(0) == 0) yield break;
		
		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < fieldY; y++)
			{
				if(_tiles[x,y].matureTime < 3)
				{
					int addedVal = _tiles[x,y].Stats.Value;
					addedVal = (addedVal * matureValue) - addedVal;
					_tiles[x,y].AddValue(addedVal);					

					Vector3 pos = TileMaster.Grid.GetPoint(x,y);
					UIManager.instance.MiniAlert(pos + Vector3.up*0.5F, "" + addedVal, 45,Color.green, 1.0F,0.00F);
					
				}
				else
				{
					int tooripe = _tiles[x,y].Stats.Value /2;
					_tiles[x,y].AddValue(-tooripe);

					Vector3 pos = TileMaster.Grid.GetPoint(x,y);
					UIManager.instance.MiniAlert(pos + Vector3.up*0.5F, "" + tooripe, 45, Color.red, 1.0F,0.00F);
				}

				_tiles[x,y].matureTime ++;

				//if(_tiles[x,y].matureTime == matureTime)
				//	{
				//		GameObject part = Instantiate(Particle);
				//		part.transform.position = _tiles[x,y].transform.position;
				//		part.transform.parent = _tiles[x,y].transform.parent;
				//	}
			}
		}*/


		yield return null;
	}
}
