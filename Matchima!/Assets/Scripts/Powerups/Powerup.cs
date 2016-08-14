﻿using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	public string Name;
	public string Instruction;
	public Class Parent;
	public bool PlayMinigame = true;
	public bool DestroyOnEnd = false;
	public UIObj [] MinigameObj;
	public int LastLevel = 0;
	public virtual IEnumerator Activate(int Level)
	{
		LastLevel = Level;
		if(PlayMinigame) yield return StartCoroutine(Minigame(Level));
		else yield return StartCoroutine(Cast(Level));

		if(DestroyOnEnd) Destroy(this.gameObject);
	}

	protected  virtual IEnumerator Minigame(int Level)
	{
		yield return StartCoroutine(Cast(Level));
	}

	IEnumerator Cast(int Level)
	{
		yield return null;
	}

	protected UIObj CreateMinigameObj(int i)
	{
		UIObj obj = (UIObj)Instantiate(MinigameObj[i]);
		RectTransform rect = obj.GetComponent<RectTransform>();
		obj.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		obj.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;
		return obj;
	}

	public virtual void Setup(Class c)
	{
		Parent = c;
	}

	public IEnumerator PowerupStartup()
	{
		
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(true);
		
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, "powerupstart", GameData.Colour(Parent.Genus));
		
		powerup.transform.SetParent(UIManager.ClassButtons.GetClass(Parent.Index).transform);
		powerup.transform.position = UIManager.ClassButtons.GetClass(Parent.Index).transform.position;
		powerup.transform.localScale = Vector3.one;

		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			Parent.Name + " Casts", 70, GameData.Colour(Parent.Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, Name, 170, GameData.Colour(Parent.Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 3,
			Instruction, 160, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Parent.Index).ShowClass(false);
		Destroy(powerup);
	}
}
