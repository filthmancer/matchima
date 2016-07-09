using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heal : Ability {

	public int HealInit = 5;
	public float HealTotal
	{
		get{
			return((float)HealInit * (1 + (StrengthFactor/2)));
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
	



	//public override IEnumerator AfterTurn()
	//{	
	//	yield return StartCoroutine(ActiveRoutine());
	//}

	public override IEnumerator ActiveRoutine()
	{
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);
		

		activated = true;
		float final_ratio = 0.0F;
		MinigameObj = (UIObj)Instantiate(MinigameObj);
		MinigameObj.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		MinigameObj.transform.localScale = Vector3.one;
		MinigameObj.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		MinigameObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

		MiniAlertUI m = UIManager.instance.MiniAlert(MinigameObj.transform.position + Vector3.up,
		"Healing\n" + (int)(final_ratio * HealTotal) + "%",
		 140, GameData.Colour(Parent.Genus), 3.5F, 0.25F);
		m.AddJuice(Juice.instance.BounceB, 0.05F);

		MinigameObj.AddAction(UIAction.MouseDown, () => 
		{
			final_ratio += 0.07F;
			//m.lifetime += 0.2F;
			m.AddJuice(Juice.instance.BounceB, 0.3F);
		});
		float taptimer = 3.0F;
		while((taptimer -= Time.deltaTime) > 0.0F)
		{
			if(final_ratio > 0.0F) final_ratio -= 0.003F;
			MinigameObj.Img[1].fillAmount = Mathf.Lerp(MinigameObj.Img[1].fillAmount, final_ratio, Time.deltaTime * 5);
			MinigameObj.Txt[0].text = "TAP!";
			m.text = "Healing\n" + (int)(final_ratio * HealTotal) + "%";
			yield return null;
		}
		MinigameObj.ClearActions(UIAction.MouseDown);

		float part_time = 0.6F;
		GameObject initpart = EffectManager.instance.PlayEffect(MinigameObj.transform, Effect.Force);
		initpart.GetComponent<MoveToPoint>().SetTarget(UIManager.instance.Health.transform.position);
		initpart.GetComponent<MoveToPoint>().SetPath(0.2F, 0.2F);
		yield return new WaitForSeconds(0.7F);

		GameObject part = Instantiate(Particle);
		part.transform.position = UIManager.instance.Health.transform.position;

		yield return new WaitForSeconds(part_time);

		Destroy(MinigameObj.gameObject);
		if(m != null) Destroy(m.gameObject);

		Player.Stats.Heal((int)(HealTotal * final_ratio));
		Player.Stats.CompleteHealth();

		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		activated = false;
		UIManager.instance.ScreenAlert.SetTween(0,false);
		GameManager.instance.paused = false;
		yield return null;
	}

	public override void SetArgs(params string [] args)
	{
		HealInit = GameData.StringToInt(args[0]);
	}
}
