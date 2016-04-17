using UnityEngine;
using System.Collections;

public class Scarecrow : Tile {

	private int LastHit = 0;

	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();
		//Params.counter.gameObject.SetActive(true);
		Params.counter.text = "";// + Stats.Hits;
		if(Stats.Hits <= 0) DestroyThyself();
	}

	public override IEnumerator AfterTurnRoutine()
	{
		LastHit = 0;
		int all_dmg = Player.Stats.DmgThisTurn;
		if(all_dmg > 0)
		{
			int dmg_taken = 0;
			if(all_dmg < Stats.Hits)
			{
				dmg_taken = all_dmg;
			}	
			else dmg_taken = Stats.Hits;
			
			Player.Stats.DmgThisTurn -= dmg_taken;
			InitStats.Hits -= dmg_taken;
			InitStats.Value = InitStats.Hits;
			InitStats.value_soft = InitStats.Value;
			Animate("Alert");
			yield return new WaitForSeconds(0.1F);
		}

		yield break;
		//yield return null;
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);
		if((int) InitStats.value_soft != InitStats.Value)
		{
			InitStats.Resource *= (int) (InitStats.value_soft / InitStats.Value);
			InitStats.Heal *= (int) (InitStats.value_soft / InitStats.Value);
			InitStats.Armour *= (int) (InitStats.value_soft / InitStats.Value);

			InitStats.Hits += (int) (InitStats.value_soft) - InitStats.Value;
			//InitStats.Attack += (int) (InitStats.value_soft) - InitStats.Value;
			InitStats.Value = (int)InitStats.value_soft;
			CheckStats();
			UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + (Stats.Value), 75, Color.white,0.8F,0.0F);
			Animate("Alert");
		}
		
	}

}
