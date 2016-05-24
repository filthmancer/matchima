using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heal : Ability {

	public int HealInit = 5;
	public int HealTotal
	{
		get{
			return (int) ((float)HealInit * (StrengthFactor/2));
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon("Restoring " + HealTotal + "% HP", Color.white));
			return All.ToArray();
		}
	}



	public override IEnumerator BeforeTurn()
	{	
		yield return StartCoroutine(ActiveRoutine());
	}

	protected IEnumerator ActiveRoutine()
	{
		UIManager.ClassButtons[Parent.Index].ShowClass(true);
		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ClassButtons[Parent.Index].transform.position + Vector3.up, 
													"Heal", 55, GameData.Colour(Parent.Genus), 1.2F, 0.25F);
		activated = true;
		float part_time = 0.6F;

		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.ClassButtons[(int)Parent.Genus].transform, Effect.Force);
		initpart.GetComponent<MoveToPoint>().SetTarget(UIManager.instance.Health.transform.position);
		initpart.GetComponent<MoveToPoint>().SetPath(0.1F);
		yield return new WaitForSeconds(0.3F);

		GameObject part = Instantiate(Particle);
		part.transform.position = UIManager.instance.Health.transform.position;

		yield return new WaitForSeconds(part_time);

		Player.Stats.Heal(HealTotal);
		Player.Stats.CompleteHealth();
		activated = false;
		yield return null;
	}

	public override void SetArgs(params string [] args)
	{
		HealInit = GameData.StringToInt(args[0]);
	}
}
