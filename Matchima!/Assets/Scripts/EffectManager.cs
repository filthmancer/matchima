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

	public ParticleSystem _ManaPowerUp, _ManaPowerDown;
	public ParticleSystem _ManaPowerLvl1,_ManaPowerLvl2,_ManaPowerLvl3;
	public ParticleSystem TouchParticle;
	public ParticleSystem SpellParticle;
}


public class EffectManager : MonoBehaviour {
	public static EffectManager instance;
	public ParticleContainer Particles;
	void Awake()
	{
		if(instance == null)
		{
			DontDestroyOnLoad(transform.gameObject);
			instance = this;
		}
		else if(instance != this) 
		{
			instance.Start();
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameObject _PlayEffect(Transform t, Effect e, string s = "", Color? col = null)
	{
		return EffectManager.instance.PlayEffect(t,e,s,col);
	}


	public GameObject PlayEffect(Transform t, Effect e, string s = "", Color? col = null)
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
				Vector3 offset = new Vector3(Random.Range(-1, 1), Random.Range(-1,1), 0);
				obj.transform.position = t.position - offset + Vector3.back*3;
				obj.transform.parent = t;
				obj.GetComponent<MoveToPoint>().SetTarget(t.position + offset + Vector3.back*3);
				obj.GetComponent<MoveToPoint>().SetDelay(0.3F);
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
			case Effect.Force:
				part = (ParticleSystem) Instantiate(Particles._Force);
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

		}

		if(part == null) return null;
		if(col.HasValue) part.startColor = col.Value;

		return part.gameObject;
	}

	private Effect EffectFromString(string s)
	{
		switch(s)
		{
			case "Charm":
			return Effect.Charm;
			case "Curse":
			return Effect.Curse;
			case "Stun":
			return Effect.Sleep;
			case "Sleep":
			return Effect.Sleep;
			case "Replace":
			return Effect.Replace;
			case "Collect":
			return Effect.Shiny;
			case "Gravity":
			return Effect.AntiGrav;
		}
		return Effect.STRING;
	}
}


