using UnityEngine;
using System.Collections;

public class Gyromancy : Ability {

	DeviceOrientation prev_orient;

	float rotation;

	public override void Start()
	{
		Description_Basic = "Rotate the screen to change gravity";
		prev_orient = DeviceOrientation.Portrait;
	}

	public override void Activate()
	{
		
	}
	

	public override void Update()
	{

		if(GameManager.instance.EnemyTurn || TileMaster.AllLanded) return;
		if(Application.isMobilePlatform) 
		{
			Vector3 acc = Input.acceleration;
			rotation = -Mathf.Rad2Deg * Mathf.Atan2(acc.y, -acc.x);
			
	
			if(rotation > 55 && rotation < 125) Player.Stats.Shift = ShiftType.Down;//DOWN
			else if(rotation < 35 && rotation > -35) Player.Stats.Shift = ShiftType.Left;//LEFT
			else if(rotation < -55 && rotation > -125)Player.Stats.Shift = ShiftType.Up; //UP
			else if(rotation < -145 || rotation > 145) Player.Stats.Shift = ShiftType.Right;//RIGHT
		}
		else 
		{
			if(Input.GetKeyDown(Player.Options.GravityUp)) 
			{
				//TileMaster.instance.SetGroundBlocks(2);
				Player.Stats.Shift = ShiftType.Up;
			}
			else if(Input.GetKeyDown(Player.Options.GravityDown)) 
			{
				//TileMaster.instance.SetGroundBlocks(0);
				Player.Stats.Shift = ShiftType.Down;
			}
			else if(Input.GetKeyDown(Player.Options.GravityRight)) 
			{
				//TileMaster.instance.SetGroundBlocks(3);
				Player.Stats.Shift = ShiftType.Right;
			}
			else if(Input.GetKeyDown(Player.Options.GravityLeft)) 
			{
				//TileMaster.instance.SetGroundBlocks(1);
				Player.Stats.Shift = ShiftType.Left;
			}
		}
	}

	public override void Destroy()
	{
		Destroy(this.gameObject);
	}
}
