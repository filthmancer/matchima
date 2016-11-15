using UnityEngine;
using System.Collections;

public class Healing : ClassEffect {
	public int Heal = 0;

	public override StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon("Healing", GameData.Colour(GENUS.WIS), false),
				new StCon(DurationString)
			};
		}
	}

	public override bool CheckDuration()
	{
		Duration -= 1;
		if(Duration == 0) return true;

		if(_Unit != null)
		{
			GameObject part = EffectManager.instance.PlayEffect(UIManager.CrewButtons[(int)_Unit.Genus].transform, Effect.Shiny, GameData.instance.GetGENUSColour(_Unit.Genus));
			part.GetComponent<DestroyTimer>().Timer = 0.8F;
			part = EffectManager.instance.PlayEffect(UIManager.instance.Health.transform, Effect.Shiny, GameData.instance.GetGENUSColour(_Unit.Genus));
			part.GetComponent<DestroyTimer>().Timer = 0.8F;
		}

		Vector3 tpos = Vector3.up * 0.2F + Vector3.left * 0.3F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.instance.Health.transform.position + tpos, 
			"+" + Heal, 42, GameData.instance.GoodColour, 1.7F,	0.01F);

		Player.Stats.Heal(Heal);
		Player.Stats.CompleteHealth();
		
		return Duration == 0;
	}

	public override void Setup(Unit c)
	{
		_Unit = c;
		if(_Unit != null)
		{
			GameObject part = EffectManager.instance.PlayEffect(UIManager.CrewButtons[(int)_Unit.Genus].transform, Effect.Shiny, GameData.instance.GetGENUSColour(_Unit.Genus));
			part.GetComponent<DestroyTimer>().Timer = 0.8F;
			part = EffectManager.instance.PlayEffect(UIManager.instance.Health.transform, Effect.Shiny, GameData.instance.GetGENUSColour(_Unit.Genus));
			part.GetComponent<DestroyTimer>().Timer = 0.8F;
		}

		Vector3 tpos = Vector3.up * 0.2F + Vector3.left * 0.3F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.instance.Health.transform.position + tpos, 
			"+" + Heal, 42, GameData.instance.GoodColour, 1.7F,	0.01F);

		Player.Stats.Heal(Heal);
		Player.Stats.CompleteHealth();
	}

	public override void GetArgs(int _duration, params string [] args)
	{
		base.GetArgs(_duration, args);
		Heal = (int)GameData.StringToFloat(args[0]);
	}
}
