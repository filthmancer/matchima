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

	public ObjectPooler AudioPool;
	public AudioSource AudioObj;

	public AudioGroup Player;

	public AudioGroup Tiles_Default;
	public AudioGroup [] Tiles;

	public AudioGroup Class_Default;
	public AudioGroup [] Classes;

	public AudioGroup UI;
	public AudioGroup Status;
	public AudioGroup Powerup;

	public AudioSource Music;

	public AudioClip HomeScreenMusic;
	public AudioClip [] ZoneMusic;
	public AudioClip Ambient;

	public static bool PlaySFX = true, PlayMusic = true;
	public bool PrintLogs = false;

	public float AudioTimer = 0.0F;
	private float AudioTimer_cap = 240.0F;

	void Start()
	{

	}

	void Update()
	{
		if(!PlayMusic && Music.enabled) Music.enabled = false;
		else if(PlayMusic && !Music.enabled) Music.enabled = true;

		if(PlayMusic && GameManager.instance.gameStart)
		{
			AudioTimer += Time.deltaTime;
			if(AudioTimer > AudioTimer_cap)
			{
				if(Music.isPlaying)
				{
					Music.loop = false;
				}
				else
				{
					AudioTimer = 0.0F;
					GetZoneMusic();
				}
				
				
			}
		}
	}

	public AudioSource PlayClipOn(Transform t, string group, string clip, float vol = 1.0F)
	{
		if(!PlaySFX) return null;
		AudioClipProperties prop = GetGroup(group).GetClip(clip);
		if(prop == null) return null;
		AudioSource aud = CreateAudioObj(prop);
		if(!aud) return null;
		aud.volume *= vol;
		aud.transform.position = t.position;
		aud.transform.parent = this.transform;
		return aud;
	}


	public AudioSource PlayClip(Transform t, AudioGroup group, string clip)
	{
		if(!PlaySFX) return null;
		AudioClipProperties prop = group.GetClip(clip);
		if(prop == null) return null;
		AudioSource aud = CreateAudioObj(prop);
		if(!aud) return null;

		aud.transform.position = t.position;
		aud.transform.parent = this.transform;
		return aud;
	}

	public AudioSource PlayTileAudio(Tile t, string clip, float vol = 1.0F)
	{
		if(!PlaySFX) return null;
		AudioGroup grop = GetTile(t.Info._TypeName);
		if(grop == null) grop = Tiles_Default;
		AudioClipProperties prop = grop.GetClip(clip);
		if(prop == null) prop = Tiles_Default.GetClip(clip);
		if(prop == null) return null;
		AudioSource aud = CreateAudioObj(prop);
		if(!aud) return null;

		aud.volume = prop.Volume * vol;
		aud.transform.position = t.transform.position;
		aud.transform.parent = this.transform;

		if(PrintLogs) print("played " + prop.Name + " at " + t);
		return aud;
	}

	public AudioSource PlayClassAudio(Class cl, string clip)
	{
		if(!PlaySFX) return null;
		AudioGroup group = GetClass(cl.Name);
		if(group == null) group = Class_Default;
		AudioClipProperties prop = group.GetClip(clip);
		if(prop == null) return null;
		AudioSource aud = CreateAudioObj(prop);
		if(!aud) return null;

		aud.transform.position = UIManager.ClassButtons.GetClass(cl.Index).transform.position;
		aud.transform.parent = this.transform;

		if(PrintLogs) print("played " + prop.Name + " at " + cl);
		return aud;
	}

	public AudioSource CreateAudioObj(AudioClipProperties prop)
	{
		if(AudioPool == null)
		{
			AudioPool = new ObjectPooler(AudioObj.gameObject, 20, this.transform);
		}
		if(!AudioPool.IsAvailable) return null;

		AudioSource aud = AudioPool.Spawn().GetComponent<AudioSource>();
		aud.GetComponent<DestroyTimer>().Timer = 2.0F;
		aud.clip = prop.GetClip();
		aud.volume = prop.Volume;
		aud.Play();
		return aud;
	}

	public AudioGroup GetGroup(string name)
	{
		//if(name == Enemy.Name) return Enemy;
		if(name == Player.Name) return Player;
		else if(name == UI.Name) return UI;
		else if(name == Powerup.Name) return Powerup;
		else if(name == Status.Name) return Status;
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

	public AudioGroup GetClass(string name)
	{
		for(int i = 0; i < Classes.Length; i++)
		{
			if(Classes[i].Name == name) return Classes[i];
		}
		return null;
	}

	string [] zonemusicnames = new string [] 
	{
		"carla_intro",
		"greg_gothicjam",
		"greg_onwards",
		"unknown_matchima1",
		"lalks_ferambe"
		//,"greg_spookyjam"
	};

	public IEnumerator LoadAudioInit()
	{
		string path = "audio/music/zone";
		ZoneMusic = new AudioClip[zonemusicnames.Length];
		for(int i = 0; i < zonemusicnames.Length; i++)
		{
			string final = path + "/" + zonemusicnames[i];
			ResourceRequest r = Resources.LoadAsync(final);
			while(!r.isDone) yield return null;
			ZoneMusic[i] = r.asset as AudioClip;
		}

		PlayMusic = ZPlayerPrefs.GetInt("Music") == 0;
		PlaySFX = ZPlayerPrefs.GetInt("SFX") == 0;
		yield return null;
	}

	public IEnumerator LoadAudio(string path)
	{
		Tiles_Default = GenerateGroup(path, "default");

		Tiles = new AudioGroup[TileMaster.Types.Species.Count];
		for(int i = 0; i < TileMaster.Types.Species.Count; i++)
		{
			//Tiles[i] = GenerateGroup(path, TileMaster.Types.Species[i].Name);
			string name = TileMaster.Types.Species[i].Name;

			string pathfinal = path + "/" + name + "/audio";
			AudioClip [] obj = Resources.LoadAll<AudioClip>(pathfinal);
			if(AudioManager.instance.PrintLogs) print("loaded " + obj.Length + " clips at " + pathfinal);

			AudioGroup groupfinal = new AudioGroup(obj.Length);
			groupfinal.Name = name;
			for(int a = 0; a < obj.Length; a++)
			{
				string [] array = obj[a].name.Split('_');
				groupfinal.AddClip(a, array[0], obj[a]);
				yield return null;
			}
			groupfinal.Minimize();
			Tiles[i] =  groupfinal;

			yield return new WaitForSeconds(Time.deltaTime * 3);
		}
		yield return null;
	}


	public static AudioGroup GenerateGroup(string path, string name)
	{
		string pathfinal = path + "/" + name + "/audio";
		AudioClip [] obj = Resources.LoadAll<AudioClip>(pathfinal);
		if(AudioManager.instance.PrintLogs) print("loaded " + obj.Length + " clips at " + pathfinal);

		AudioGroup groupfinal = new AudioGroup(obj.Length);
		groupfinal.Name = name;
		for(int i = 0; i < obj.Length; i++)
		{
			string [] array = obj[i].name.Split('_');
			groupfinal.AddClip(i, array[0], obj[i]);
		}
		groupfinal.Minimize();
		return groupfinal;
	}

	public void SetMusicClip(AudioClip clip)
	{
		Music.clip = clip;
		Music.Play();
	}

	public void SetMusic(bool? active = null)
	{
		PlayMusic = active ?? !PlayMusic;
		ZPlayerPrefs.SetInt("Music", PlayMusic ? 0 : 1);
	}

	public void SetSFX(bool? active = null)
	{
		PlaySFX = active ?? !PlaySFX;
		ZPlayerPrefs.SetInt("SFX", PlaySFX ? 0 : 1);
	}

	public void GetZoneMusic()
	{
		int r = Random.Range(0, ZoneMusic.Length);
		Music.clip = ZoneMusic[r];
		Music.loop = true;
		Music.Play();
	}

	private List<Tile> alerts = new List<Tile>();
	public void QueueAlert(Tile t)
	{
		foreach(Tile child in alerts)
		{
			if(string.Equals(child.TypeName, t.TypeName)) return;
		}
		if(Random.value > 0.7F) t.PlayAudio("alert", 0.4F);
		alerts.Add(t);
	}

	public void ClearAlerts()
	{
		alerts.Clear();
	}




}

[System.Serializable]
public class AudioGroup
{
	public string Name;
	public AudioClipProperties [] Clips;

	public int Length
	{
		get{return Clips.Length;}
	}

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
		AudioClipProperties addto = null;
		foreach(AudioClipProperties child in Clips)
		{
			if(child == null || child.Name == string.Empty) continue;
			if(child.Name == name)
			{
				addto = child;
				break;
			}
		}
		if(addto == null)
		{
			AudioClipProperties final = new AudioClipProperties();
			final.Name = name;
			final.Clip.Add(clip);
			final.Volume = vol;
			Clips[num] = final;
		}
		else
		{
			addto.Clip.Add(clip);
		}
	}

	public void Minimize()
	{
		List<AudioClipProperties> final = new List<AudioClipProperties>();
		final.AddRange(Clips);

		for(int i = 0; i < final.Count; i++)
		{
			if(final[i] == null || 
				final[i].Name == string.Empty || 
				final[i].Clip == null || 
			    final[i].Clip.Count == 0)
			    {
			    	final.RemoveAt(i);
			    	i--;
			    } 
			
		}
		Clips = final.ToArray();
	}


}

[System.Serializable]
public class AudioClipProperties
{
	public string Name;
	public List<AudioClip> Clip;

	public AudioClipProperties()
	{
		Clip = new List<AudioClip>();
	}

	public AudioClip GetClip()
	{
		if(Clip.Count == 0) return null;
		else if(Clip.Count == 1) return Clip[0];
		else return Clip[Random.Range(0, Clip.Count)];
	}
	public float Volume = 1.0F;
	public float StopAtPercent = 1.0F;
}
