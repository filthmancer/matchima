using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wave : Unit {

	public string Description
	{
		get
		{
			string d = "";
			return d;
		}
	}

	public WaveUnit this [int i]
	{
		get{
			return AllSlots[i];
		}
	}

	public int Length
	{
		get{
			return AllSlots.Length;
		}
	}


	public bool Active;
	public float Chance = 1.0F;
	public int RequiredDifficulty = 1;

	public int Current;
	public int Required;
	private int PointsThisTurn;
	public int PointsPerTurn;

	public WaveUnit Slot1, Slot2, Slot3;
	public bool HasDialogue;
	public Quote [] Quotes;
	public WaveUnit [] AllSlots
	{
		get{return new WaveUnit [] {Slot1, Slot2, Slot3};}
	}

	public bool IntroAlert = false, EnterAlert = false;

	public virtual string IntroText
	{
		get{
			return "";
		}
	} 
	public virtual string EnterText 
	{
		get {
			return string.Empty;
		}
	}
	public virtual string ExitText 
	{
		get {
			return "Floor Completed!";
		}
	}

	public float GetRatio()
	{
		return (float) Current / Required;
	}

	public bool Ended;
	public int Experience = 5;
	private bool ShowingHealth;
	
	public IEnumerator Setup()
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].Setup(this,i);
		}
		Active = true;
		yield return StartCoroutine(OnStart());

		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].Activate();
		}	

		yield return StartCoroutine(WaveActivateRoutine());
	}

	public void AddPoints(int p)
	{
		if(!Active || Ended || Current == -1) return;
		PointsThisTurn += p;
		Current = Mathf.Clamp(Current + p, 0, Required);
		if(!ShowingHealth)
		{
			StartCoroutine(ShowHealthRoutine());
		}
	}

	IEnumerator ShowHealthRoutine()
	{
		ShowingHealth = true;
		int current_heal = PointsThisTurn;

		string prefix = current_heal > 0 ? " +" : " ";
		Vector3 tpos = Vector3.down + Vector3.right * 0.4F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.instance.WaveHealthText.transform.position, 
			prefix + current_heal, 42, GameData.instance.BadColour, 1.7F,	-0.08F);
		heal.transform.parent = UIManager.instance.WaveHealthText.transform;

		while(heal.lifetime > 0.0F)
		{
			if(PointsThisTurn == 0)
			{
				heal.lifetime = 0.0F;
				heal.text = "";
				break;
			}
			else if(PointsThisTurn != current_heal)
			{
				heal.lifetime += 0.2F;
				heal.size = 42 + current_heal * 0.75F;
				current_heal = PointsThisTurn;
				heal.text = prefix + current_heal;
			}
			

			yield return null;
		}

		ShowingHealth = false;

		yield return null;
	}


	public virtual IEnumerator OnStart()
	{
		yield break;
	}

	public virtual IEnumerator BeginTurn()
	{
		AddPoints(PointsPerTurn);
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				AllSlots[i].Timer ++;

				AddPoints(-AllSlots[i].PointsPerTurn);
				yield return StartCoroutine(AllSlots[i].BeginTurn());
			}
		}

		yield return null;
	}


	public virtual IEnumerator AfterTurn()
	{
		PointsThisTurn = 0;
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				yield return StartCoroutine(AllSlots[i].AfterTurn());
				AllSlots[i].Complete();
			}
		}
		if(Current <= 0 && Active)
		{
			for(int i = 0; i < AllSlots.Length; i++)
			{
				if(AllSlots[i] == null) continue;
				AllSlots[i].OnEnd();
			}
			
			GameManager.instance.paused = true;
			yield return StartCoroutine(WaveEndRoutine());
			GameManager.instance.paused = false;
			Ended = true;
			foreach(Class child in Player.Classes)
			{
				child.AddExp((int) (Experience * GameManager.Difficulty));
			}
		}
		yield return null;
	}

	public void AddWaveUnit(WaveUnit wavetile)
	{
		WaveUnit w = (WaveUnit) Instantiate(wavetile);
		w.transform.parent = this.transform;
		bool added = false;
		int slot = 0;
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] != null) continue;
			else 
			{
				LinkWaveUnit(w, i);
				added = true;
				slot = i;
				break;
			}
		}
		if(!added) Debug.LogError("COULD NOT ADD MOD WAVE");
		w.Setup(this, slot);
	}

	public void LinkWaveUnit(WaveUnit w, int i)
	{
		switch(i)
		{
			case 0:
			Slot1 = w;
			break;
			case 1:
			Slot2 = w;
			break;
			case 2:
			Slot3 = w;
			break;
		}
	}

	public virtual void EnemyKilled(Enemy e)
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;

			AddPoints(-AllSlots[i].EnemyKilled(e));
		}
	}

	public List<TileEffectInfo> GetEffects()
	{
		List<TileEffectInfo> Effects = new List<TileEffectInfo>();
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].GetEffects() != null) Effects.AddRange(AllSlots[i].GetEffects());
		}
		return Effects;
	}

	public void GetChances()
	{
		//List<TileEffectInfo> Effects = GetEffects();
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].GetChances();
		}
	}

	protected virtual IEnumerator WaveStartRoutine()
	{
		PlayerControl.instance.ResetSelected();
		if(IntroAlert)
		{
			yield return StartCoroutine(UIManager.instance.Alert(1.25F, false, IntroText));
		}
		yield return null;
		Player.instance.ResetStats();
	}

	protected virtual IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);
		//CameraUtility.SetTurnOffset(true);

		yield return StartCoroutine(UIManager.instance.Alert(1.25F, true, Name));
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				 yield return StartCoroutine(AllSlots[i].OnStart());
			}
		}
	}

	protected virtual IEnumerator WaveEndRoutine()
	{
		yield return StartCoroutine(UIManager.instance.Alert(1.25F, false, ExitText));
	}

	public WaveReward GenerateWaveReward()
	{	
		if(Random.value > 1.0F) return null;

		WaveReward reward = new WaveReward();
		reward.value = GameManager.Difficulty;
		float val = Random.value;
		if(val < 0.25F)
		{
			reward.value = (reward.value * 2);
			reward.Title = "+" + (int)reward.value + " mana";
			foreach(Class child in Player.Classes)
			{
				child.AddToMeter((int)reward.value);
			}
		}
		else if(val < 0.5F)
		{
			reward.value = 1.0F + reward.value / 100;
			reward.Title = "+" + reward.value.ToString("0.00") + "x mana values";
		}
		else if(val < 0.75F)
		{
			reward.value = Mathf.Clamp(reward.value / 3, 1, Mathf.Infinity);
			reward.Title = "+" + (int)reward.value + " Upgrade Value!";
			foreach(Class child in Player.Classes)
			{
				child.WaveLevelRate += (int) reward.value;
			}
		}
		else
		{
			reward.value = Mathf.Clamp(reward.value * 5, 1, Player.Stats._HealthMax);
			reward.Title = (int)reward.value + " HP HEAL";
			Player.Stats.Heal((int) reward.value);
		}
		return reward;
	}

	public class WaveReward
	{
		public string Title;
		public float value;
	}


}

public enum WaveTileSpawn
{
	XAtStart,
	XPerTurn,
	XChance,
	XOnScreen
}
