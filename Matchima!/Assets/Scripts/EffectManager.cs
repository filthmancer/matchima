using UnityEngine;
using System.Collections;

public enum Effect
{
	Attack,
	AntiGrav,
	Charm,
	Collect,
	Curse,
	Destroy,
	Force,
	Fire,
	Lightning,
	Replace,
	Shiny,
	Sleep,
	Spell,


	ManaPowerUp,
	ManaPowerDown,
	ManaPowerLvl1,
	ManaPowerLvl2,
	ManaPowerLvl3,
	STRING
}


[System.Serializable]
public class ParticleContainer
{
	public GameObject _Attack;
	public ParticleSystem _Destroy;
	public ParticleSystem Charm;
	public ParticleSystem _Sleep;
	public ParticleSystem _Curse;
	public ParticleSystem _Replace;
	public ParticleSystem _Shiny;
	public ParticleSystem _AntiGravity;
	public ParticleSystem _Force;
	public ParticleSystem _Lightning;
	public ParticleSystem _Fireball;

	public ParticleSystem _ManaPowerUp, _ManaPowerDown;
	public ParticleSystem _ManaPowerLvl1,_ManaPowerLvl2,_ManaPowerLvl3;
	public ParticleSystem TouchParticle;
	public ParticleSystem SpellParticle;
}

[System.Serializable]
public class ParticleContainer2
{
	public string Name;
	public Effect EffectEnum;
	public GameObject Particle;
	public ObjectPooler Pool;


	public GameObject Spawn()
	{
		if(Pool == null)
		{
			Pool = new ObjectPooler(Particle, 1, EffectManager.instance.transform);
		}
		return Pool.Spawn();
	}
}


public class EffectManager : MonoBehaviour {
	public static EffectManager instance;
	public ParticleContainer Particles;
	public ParticleContainer2 [] Particles2;


	void Awake()
	{
		if(instance == null)
		{
			DontDestroyOnLoad(transform.gameObject);
			instance = this;
		}
		else if(instance != this) 
		{
			Destroy(this.gameObject);
		}
	}

	private static GameObject _PlayEffect(Transform t, Effect e, string s = "", Color? col = null)
	{
		return EffectManager.instance.PlayEffect(t,e,s,col);
	}



	public GameObject PlayEffect(Transform t, string name, Color? col = null)
	{
		if(t == null) return null;
		GameObject final = null;
		name = name.ToLower();
		
		for(int i = 0; i < Particles2.Length; i++)
		{
			if(string.Equals(name, Particles2[i].Name.ToLower()))
			{
				final = Particles2[i].Spawn();
				break;
			}
		}

		final.transform.SetParent(t);
		final.transform.position = t.position;
		if(name == "attack")
		{
			Vector3 pos = t.position + Vector3.back * 3;
			Vector3 offset = Utility.RandomVectorInclusive(1,1).normalized * 1.4F;
			final.transform.position = pos - offset;
			final.transform.parent = t;
			MoveToPoint p = final.GetComponent<MoveToPoint>();
			p.SetTarget(t.position + offset);
			p.SetDelay(0.35F);
			p.SetThreshold(0.25F);
		}

		ParticleSystem final_part = final.GetComponent<ParticleSystem>();
		if(final_part)
		{
			if(col.HasValue) final_part.startColor = col.Value;
			final_part.Clear();
			final_part.Play();
		}


		return final;
	}


	public GameObject PlayEffect(Transform t, Effect e, Color? col = null)
	{
		if(t == null) return null;
		GameObject final = null;
		ParticleSystem final_part = null;
		
		for(int i = 0; i < Particles2.Length; i++)
		{
			if(e == Particles2[i].EffectEnum)
			{
				final = Particles2[i].Spawn();
				final_part = final.GetComponent<ParticleSystem>();
				break;
			}
		}

		final.transform.SetParent(t);
		final.transform.position = t.position;
		if(e == Effect.Attack)
		{
			Vector3 pos = t.position + Vector3.back * 3;
			Vector3 offset = Utility.RandomVectorInclusive(1,1).normalized * 1.4F;
			final.transform.position = pos - offset;
			final.transform.parent = t;
			MoveToPoint p = final.GetComponent<MoveToPoint>();
			p.SetTarget(t.position + offset);
			p.SetDelay(0.35F);
			p.SetThreshold(0.25F);
		}
		if(final_part)
		{
			if(col.HasValue) final_part.startColor = col.Value;
			final_part.Clear();
			final_part.Play();
		}
		return final;
	}


	private GameObject PlayEffect(Transform t, Effect e, string s = "", Color? col = null)
	{
		if(t == null) return null;
		if(e == Effect.STRING)
		{
			e = EffectFromString(s);
		}
		ParticleSystem part = null;
		GameObject obj = null;
		switch(e)
		{
			case Effect.Attack:
				obj = (GameObject)Instantiate(Particles._Attack);
				Vector3 pos = t.position + Vector3.back * 3;
				Vector3 offset = Utility.RandomVectorInclusive(1,1).normalized * 1.4F;
				obj.transform.position = pos - offset;
				obj.transform.parent = t;
				MoveToPoint p = obj.GetComponent<MoveToPoint>();
				p.SetTarget(t.position + offset);
				p.SetDelay(0.35F);
				p.SetThreshold(0.25F);
			return obj;
			case Effect.Destroy:
				part = (ParticleSystem) Instantiate(Particles._Destroy);
				part.transform.position = t.position;
				
			break;
			case Effect.Charm:
				part = (ParticleSystem) Instantiate(Particles.Charm);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Curse:
				part = (ParticleSystem)Instantiate(Particles._Curse);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Sleep:
				part = (ParticleSystem)Instantiate(Particles._Sleep);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Replace:
				part = (ParticleSystem) Instantiate(Particles._Replace);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Shiny:
				part = (ParticleSystem) Instantiate(Particles._Shiny);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.AntiGrav:
				part = (ParticleSystem) Instantiate(Particles._AntiGravity);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Spell:
				part = (ParticleSystem) Instantiate(Particles.SpellParticle);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Lightning:
				part = (ParticleSystem) Instantiate(Particles._Lightning);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.ManaPowerUp:
				part = (ParticleSystem) Instantiate(Particles._ManaPowerUp);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.ManaPowerDown:
				part = (ParticleSystem) Instantiate(Particles._ManaPowerDown);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.ManaPowerLvl1:
				part = (ParticleSystem) Instantiate(Particles._ManaPowerLvl1);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.ManaPowerLvl2:
				part = (ParticleSystem) Instantiate(Particles._ManaPowerLvl2);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.ManaPowerLvl3:
				part = (ParticleSystem) Instantiate(Particles._ManaPowerLvl3);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;
			case Effect.Fire:
				part = (ParticleSystem) Instantiate(Particles._Fireball);
				part.transform.position = t.position;
				part.transform.parent = t;
			break;

		}

		if(part == null) return null;
		if(col.HasValue) part.startColor = col.Value;

		return part.gameObject;
	}

	private Effect EffectFromString(string s)
	{
		s = s.ToLower();
		switch(s)
		{
			case "charm":
			return Effect.Charm;
			case "curse":
			return Effect.Curse;
			case "stun":
			return Effect.Sleep;
			case "sleep":
			return Effect.Sleep;
			case "replace":
			return Effect.Replace;
			case "collect":
			return Effect.Shiny;
			case "gravity":
			return Effect.AntiGrav;
		}
		return Effect.STRING;
	}
}


