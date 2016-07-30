using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ZoneStyle
{
	Progressive,
	Random
}

public class Zone : MonoBehaviour {
	public string _Name;
	public string Name
	{
		get{
			return "The " + _Name;
		}
	}
	public Wave IntroWave;
	public Wave [] Waves;
	public Wave BossWave;
	public ZoneStyle Style;
	public Wave this [int i]{get{return Waves[i];}}

	public bool SkipAllStory;
	public bool ShowIntroWave;
	public bool Repeat;
	public int StartAt = 0;

	public bool isNew = true;
	public Color Tint, WallTint;
	[SerializeField]
	public IntVector MapSize;
	[SerializeField]
	private IntVector _Depth;

	[SerializeField]
	private int Depth;
	private int Initial;

	private int curr = 0;
	private bool shown_intro_wave;
	private bool ShownBoss;
	public void Start()
	{
		curr = StartAt;
		for(int i = 0; i < Waves.Length; i++)
		{
			if(Waves[i] == null) continue;
			Waves[i].Index = i;
		}
		isNew = true;
	}

	public Wave GetWaveProgressive()
	{
		ShowIntroWave = Player.Options.ShowIntroWaves;
		SkipAllStory = Player.Options.SkipAllStory;
		bool intro_in_past = PlayerPrefs.GetInt(Name + " Intro")==1;
		if(((!intro_in_past && !SkipAllStory) || ShowIntroWave) && !shown_intro_wave && IntroWave != null)
		{
			shown_intro_wave = true;
			PlayerPrefs.SetInt(Name + " Intro", 1);
			return IntroWave;
		}

		if(curr >= Waves.Length) 
		{
			if(BossWave != null && !ShownBoss) 
			{
				ShownBoss = true;
				return BossWave;
			}
			else if(!Repeat) 
			{
				//GameManager.instance.EscapeZone();
				return null;
			}
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
		ShowIntroWave = Player.Options.ShowIntroWaves;
		SkipAllStory = Player.Options.SkipAllStory;
		bool intro_in_past = PlayerPrefs.GetInt(Name + " Intro")==1;
		if(((!intro_in_past && !SkipAllStory) || ShowIntroWave) && !shown_intro_wave && IntroWave != null)
		{
			shown_intro_wave = true;
			PlayerPrefs.SetInt(Name + " Intro", 1);
			return IntroWave;
		}

		List<Wave> choices = new List<Wave>();
		List<float> chance = new List<float>();
		foreach(Wave child in Waves)
		{
			if(child.Chance > 0.0F && GameManager.Difficulty > child.RequiredDifficulty)
			{
				choices.Add(child);
				chance.Add(child.Chance * 3);
			}
		}
		foreach(Wave child in GameManager.instance.DefaultWaves.Waves)
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

	public Wave CheckZone()
	{
		if(Style == ZoneStyle.Progressive) return GetWaveProgressive();
		else if(Style == ZoneStyle.Random)
		{
			if(GameManager.Floor >= Initial + Depth)
			{
				GameManager.instance.EscapeZone();
			}
			if(GameManager.Floor == Initial+Depth-1 && BossWave != null)
			{
				return BossWave;
			}
			else return GetWaveRandom();
		}
		return null;
	}

	public void Randomise()
	{
		Depth = Random.Range(_Depth.x, _Depth.y);
		Initial = GameManager.Floor;
	}
	public Vector2 GetMapSize()
	{
		return MapSize.ToVector2;
	}
}
