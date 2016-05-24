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
			return "Wave Approaching!";
		}
	} 
	public virtual string EnterText 
	{
		get {
			return "Wave Arrived!";
		}
	}
	public virtual string ExitText 
	{
		get {
			return "Wave Defeated!";
		}
	}

	public bool AllEnded
	{
		get
		{
			bool ended = true;
			for(int i = 0; i < AllSlots.Length; i++)
			{
				if(AllSlots[i] == null) continue;
				if(AllSlots[i] is WaveEffect) continue;
				if(!AllSlots[i].Ended) ended = false;
			}
			return ended;
		}
	}
	
	public IEnumerator Setup()
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].Setup(this,i);
		}

		yield return StartCoroutine(OnStart());

		yield return StartCoroutine(WaveStartRoutine());


		//OLD FORMAT FOR GETTING MODS (RANDOM)
		/*
			//Get a list of possible mods
				List<WaveUnit> allmodwaves = new List<WaveUnit>();
				allmodwaves.AddRange(ModWaveUnits);

			//While a random roll is greater than the chance of X number of mods
				int curr = 0;
				while(Random.value < ModChance[curr])
				{
			//Roll chances of each individual mod
					List<float> mod_chances = new List<float>();
					List<WaveUnit> mod_waves = new List<WaveUnit>();
					for(int i = 0; i < ModWaveUnits.Length; i++)
					{
						float c = ModWaveUnits[i].Chance;
						if(c > 0.0F)
						{
							mod_chances.Add(c);
							mod_waves.Add(ModWaveUnits[i]);
						}
					}

			//Roll the index of selected mod and add mod
					int index = ChanceEngine.Index(mod_chances.ToArray());
					WaveUnit mod = mod_waves[index];
					AddWaveUnit(mod);
					curr++;

			// Check if all slots are full
					bool allfull = true;
					for(int i = 0; i < AllSlots.Length; i++)
					{
						if(AllSlots[i] == null) allfull = false;
					}
					if(allfull) break;
				}
		*/

	}

	public virtual IEnumerator OnStart()
	{
		yield break;
	}

	public virtual IEnumerator BeginTurn()
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
		
			AllSlots[i].Timer --;
			if(AllSlots[i].Timer == 0) 
			{
				AllSlots[i].Activate();
				yield return new WaitForSeconds(Time.deltaTime * 10);
				if(i == 0) yield return StartCoroutine(WaveActivateRoutine());
			}
		}		
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				if(AllSlots[i].Timer < 0) yield return StartCoroutine(AllSlots[i].BeginTurn());
				else yield return StartCoroutine(AllSlots[i].OnStart());
			}
		}


		yield return null;
	}


	public virtual IEnumerator AfterTurn()
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				yield return StartCoroutine(AllSlots[i].AfterTurn());
				AllSlots[i].Complete();
				if(AllSlots[i].Current == 0 && AllSlots[i].Active)
				{
					AllSlots[i].OnEnd();
					GameManager.instance.paused = true;
					yield return StartCoroutine(WaveEndRoutine());
					GameManager.instance.paused = false;
				}
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
			AllSlots[i].EnemyKilled(e);
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
			
			GameManager.instance.paused = true;

			UIManager.instance.WaveAlert.SetTween(0,true);
			UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(true);
			UIManager.instance.WaveAlert.Txt[0].text = IntroText;

			yield return new WaitForSeconds(1.25F);
			
			UIManager.instance.WaveAlert.SetTween(0,false);
			UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(false);
			UIManager.instance.WaveAlert.Txt[0].text = "";

			PlayerControl.instance.ResetSelected();
			GameManager.instance.paused = false;
			
		}
		yield return null;
		Player.instance.ResetStats();
	}

	protected virtual IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);
		//CameraUtility.SetTurnOffset(true);

		PlayerControl.instance.ResetSelected();
		GameManager.instance.paused = true;
		UIManager.instance.WaveAlert.SetTween(0,true);
		UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(true);
		UIManager.instance.WaveAlert.Txt[0].text = EnterText;
		UIManager.instance.WaveAlert.Img[2].gameObject.SetActive(true);
		UIManager.instance.WaveAlert.Txt[2].text = Name;

		yield return new WaitForSeconds(1.25F);
		
		UIManager.instance.WaveAlert.SetTween(0,false);
		UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(false);
		UIManager.instance.WaveAlert.Txt[0].text = "";
		UIManager.instance.WaveAlert.Img[2].gameObject.SetActive(false);
		UIManager.instance.WaveAlert.Txt[2].text = "";
		PlayerControl.instance.ResetSelected();
		GameManager.instance.paused = false;
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		//CameraUtility.SetTurnOffset(false);
		yield return null;
	}

	protected virtual IEnumerator WaveEndRoutine()
	{
		//WaveReward reward = GenerateWaveReward();
		UIManager.instance.WaveAlert.SetTween(0,true);
		UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(true);
		UIManager.instance.WaveAlert.Txt[0].text = ExitText;
		//if(reward != null)
		//{
		//	UIManager.instance.WaveAlert.Img[2].gameObject.SetActive(true);
		//	UIManager.instance.WaveAlert.Txt[0].text = reward.Title;
		//}
		yield return new WaitForSeconds(1.3F);

		UIManager.instance.WaveAlert.SetTween(0,false);
		UIManager.instance.WaveAlert.Img[0].gameObject.SetActive(false);
		UIManager.instance.WaveAlert.Img[2].gameObject.SetActive(false);

		//yield return StartCoroutine(GameManager.instance.CompleteTurnRoutine());
		yield return null;
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
