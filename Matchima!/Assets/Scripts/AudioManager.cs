using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	private static AudioManager _instance;
	public static AudioManager instance;
	//{
	//	get
	//	{
	//		if(_instance == null)
	//		{
	//			_instance = this;
	//		}
	//		else if(_instance != this)
	//		{
	//			Destroy(this.gameObject);
	//		}
	//		return _instance;
	//	}
	//}

	public void Awake()
	{
		instance = this;
	}
	public AudioSource AudioObj;

	public AudioGroup Enemy;
	public AudioGroup Player;

	void Start()
	{

	}

	void Update()
	{

	}

	public void PlayClipOn(Transform t, string group, string clip)
	{
		return;
		AudioClipProperties prop = GetGroup(group).GetClip(clip);
		if(prop == null) return;
		AudioSource aud = (AudioSource) Instantiate(AudioObj);
		aud.clip = prop.Clip;
		aud.volume = prop.Volume;
		aud.Play();
		aud.transform.position = t.position;
		aud.transform.parent = this.transform;
	}

	public AudioGroup GetGroup(string name)
	{
		if(name == Enemy.Name) return Enemy;
		else if(name == Player.Name) return Player;
		else return null;
	}
}

[System.Serializable]
public class AudioGroup
{
	public string Name;
	public AudioClipProperties [] Clips;

	public AudioClipProperties GetClip(string name)
	{
		for(int i = 0; i < Clips.Length; i++)
		{
			if(Clips[i].Name == name) return Clips[i];
		}
		return null;
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
