using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveGroup : MonoBehaviour {
	public string Name;
	public Wave IntroWave;
	public Wave [] Waves;
	public Wave this [int i]{get{return Waves[i];}}

	public bool ShowIntroWave;
	public bool Repeat;
	public int StartAt = 0;

	private int curr = 0;
	private bool shown_intro_wave;

	public void Start()
	{
		curr = StartAt;
		for(int i = 0; i < Waves.Length; i++)
		{
			if(Waves[i] == null) continue;
			Waves[i].Index = i;
		}
	}

	public Wave GetWaveProgressive()
	{
		if(!shown_intro_wave && Player.Options.StorySet == Ops_Story.AlwaysShow)
		{
			shown_intro_wave = true;
			PlayerPrefs.SetInt(Name + " Intro", 1);
			return IntroWave;
		}
		else if(Player.Options.StorySet == Ops_Story.Default)
		{
			bool intro_in_past = PlayerPrefs.GetInt(Name + " Intro")==1;
			if(!intro_in_past)
			{
				shown_intro_wave = true;
				PlayerPrefs.SetInt(Name + " Intro", 1);
				return IntroWave;
			}	
		}


		if(curr >= Waves.Length) 
		{
			if(!Repeat) return null;
			else curr = 0;
		}
		Wave w = Waves[curr];
		curr++;
		while(w == null)
		{
			w = Waves[curr];
			curr++;
		}
		
		return w;
	}

	public Wave GetWaveRandom()
	{
		if(!shown_intro_wave && Player.Options.StorySet == Ops_Story.AlwaysShow)
		{
			shown_intro_wave = true;
			PlayerPrefs.SetInt(Name + " Intro", 1);
			return IntroWave;
		}
		else if(Player.Options.StorySet == Ops_Story.Default)
		{
			bool intro_in_past = PlayerPrefs.GetInt(Name + " Intro")==1;
			if(!intro_in_past)
			{
				shown_intro_wave = true;
				PlayerPrefs.SetInt(Name + " Intro", 1);
				return IntroWave;
			}	
		}

		List<Wave> choices = new List<Wave>();
		List<float> chance = new List<float>();
		foreach(Wave child in Waves)
		{
			if(child.Chance > 0.0F && GameManager.Difficulty > child.RequiredDifficulty)
			{
				choices.Add(child);
				chance.Add(child.Chance);
			}
		}

		int index = ChanceEngine.Index(chance.ToArray());
		curr = index;
		Wave w = choices[curr];
		
		return w;
	}
}
