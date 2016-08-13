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
	public string _Prefix = "The";
	public string Name
	{
		get{
			return _Prefix + " " + _Name;
		}
	}
	public Wave IntroWave;
	public Wave [] Waves;
	public Wave BossWave;
	public ZoneStyle Style;
	public Wave this [int i]{get{return Waves[i];}}

	public bool SkipAllStory;
	public bool ShowIntroWave;
	public bool UseInGeneration = true;
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

	[SerializeField]
	private int Current = 0;
	public void SetCurrent(int i) {Current = i;}
	private bool shown_intro_wave;
	private bool ShownBoss;

	public virtual void Start()
	{
		Current = StartAt;
		for(int i = 0; i < Waves.Length; i++)
		{
			if(Waves[i] == null) continue;
			Waves[i].Index = i;
		}
		isNew = true;
	}

	public virtual Wave GetWaveProgressive()
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

		if(Current >= Waves.Length) 
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
			else Current = 0;
		}
		Wave w = Waves[Current];
		Current++;
		while(w == null)
		{
			w = Waves[Current];
			Current++;
		}
		return w;
	}

	public virtual Wave GetWaveRandom()
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
		Current = index;
		Wave w = choices[Current];
		
		return w;
	}

	public virtual Wave CheckZone()
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

	public int CurrentDepthInZone
	{
		get{
			return Current;
		}
	}

	public float GetZoneDepth_Ratio()
	{
		return (float) Current / (float) Depth;
	}

	public int GetZoneDepth()
	{
		return Depth;
	}

	public virtual IEnumerator Enter()
	{
		Randomise();
		
		(UIManager.Objects.MiddleGear[2] as UIObjTweener).SetTween(0, false);
		(UIManager.Objects.MiddleGear[1] as UIObjTweener).SetTween(0, false);
		UIManager.instance.BackingTint = Tint;
		UIManager.instance.WallTint = WallTint;
		
		TileMaster.instance.MapSize_Default = GetMapSize();
		
		
		Player.instance.ResetStats();
		
		if(GameManager.ZoneNum > 1) 
		{
			yield return null;
			yield return StartCoroutine(TileMaster.instance.NewGridRoutine());
		}
		
		StCon [] floor = new StCon[]{new StCon("Entered")};
		StCon [] title = new StCon[]{new StCon(Name, WallTint * 1.5F, false, 110)};
		
		yield return StartCoroutine(UIManager.instance.Alert(0.9F, floor, title));
	}

}
