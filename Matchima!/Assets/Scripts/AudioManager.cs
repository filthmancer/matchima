using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AudioManager : MonoBehaviour {
	private static AudioManager _instance;
	public static AudioManager instance;

	public void Awake()
	{
		instance = this;
	}
	public AudioSource AudioObj;

	public AudioGroup Enemy;
	public AudioGroup Player;

	public AudioGroup Tiles_Default;

	public AudioGroup [] Tiles;

	public AudioSource Music;

	public static bool PlaySFX = true, PlayMusic = true;

	void Start()
	{

	}

	void Update()
	{
		if(!PlayMusic && Music.enabled) Music.enabled = false;
		else if(PlayMusic && !Music.enabled) Music.enabled = true;
	}

	public void PlayClipOn(Transform t, string group, string clip)
	{
		if(!PlaySFX) return;
		AudioClipProperties prop = GetGroup(group).GetClip(clip);
		if(prop == null) return;
		AudioSource aud = (AudioSource) Instantiate(AudioObj);
		aud.clip = prop.Clip;
		aud.volume = prop.Volume;
		aud.Play();
		aud.transform.position = t.position;
		aud.transform.parent = this.transform;
	}


	public AudioSource PlayClip(Transform t, AudioGroup group, string clip)
	{
		if(!PlaySFX) return null;
		AudioClipProperties prop = group.GetClip(clip);
		if(prop == null) return null;
		AudioSource aud = (AudioSource) Instantiate(AudioObj);
		aud.clip = prop.Clip;
		aud.volume = prop.Volume;
		aud.Play();
		aud.transform.position = t.position;
		aud.transform.parent = this.transform;
		return aud;
	}

	public AudioSource PlayTileAudio(Tile t, string clip)
	{
		if(!PlaySFX) return null;
		AudioGroup grop = GetTile(t.Info._TypeName);
		if(grop == null) grop = Tiles_Default;
		AudioClipProperties prop = grop.GetClip(clip);
		if(prop == null) return null;
		
		AudioSource aud = (AudioSource) Instantiate(AudioObj);
		aud.clip = prop.Clip;
		aud.volume = prop.Volume;
		aud.Play();
		aud.transform.position = t.transform.position;
		aud.transform.parent = this.transform;
		return aud;
	}

	public AudioGroup GetGroup(string name)
	{
		if(name == Enemy.Name) return Enemy;
		else if(name == Player.Name) return Player;
		else return null;
	}

	public AudioGroup GetTile(string name)
	{
		for(int i = 0; i < Tiles.Length; i++)
		{
			if(Tiles[i].Name == name) return Tiles[i];
		}
		return null;
	}

	public IEnumerator LoadAudio(string path)
	{
		Tiles_Default = GenerateGroup(path, "default");

		Tiles = new AudioGroup[TileMaster.Types.Species.Count];
		for(int i = 0; i < TileMaster.Types.Species.Count; i++)
		{
			Tiles[i] = GenerateGroup(path, TileMaster.Types.Species[i].Name);
		}
		yield return null;
	}


	AudioGroup GenerateGroup(string path, string name)
	{
		string pathfinal = path + "/" + name + "/audio";
		AudioClip [] obj = Resources.LoadAll<AudioClip>(pathfinal);
		print("loaded " + obj.Length + " clips at " + pathfinal);

		AudioGroup groupfinal = new AudioGroup(obj.Length);
		groupfinal.Name = name;
		for(int i = 0; i < obj.Length; i++)
		{
			groupfinal.AddClip(i, obj[i].name, obj[i]);
		}
		return groupfinal;
	}
}

[System.Serializable]
public class AudioGroup
{
	public string Name;
	public AudioClipProperties [] Clips;

	public AudioGroup(int num)
	{
		Clips = new AudioClipProperties[num];
	}

	public AudioClipProperties GetClip(string name)
	{
		for(int i = 0; i < Clips.Length; i++)
		{
			if(Clips[i].Name == name) return Clips[i];
		}
		return null;
	}

	public void AddClip(int num, string name, AudioClip clip, float vol = 1.0f)
	{
		AudioClipProperties final = new AudioClipProperties();
		final.Name = name;
		final.Clip = clip;
		final.Volume = vol;
		Clips[num] = final;
	}
}

[System.Serializable]
public class AudioClipProperties
{
	public string Name;
	public AudioClip Clip;
	public float Volume = 1.0F;
	public float StopAtPercent = 1.0F;
}
