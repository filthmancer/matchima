using UnityEngine;
using System.Collections;

public enum StatusType
{
	Buff,
	Debuff
}
public class Status : MonoBehaviour {
	public string Name;
	public int Duration;
	public StatusType Type;

	public virtual StCon [] Description
	{ 
		get{
			return new StCon [] {
				new StCon(DurationString)
			};
		}
	}
	public string DurationString
	{
		get{return (Duration <= -1 ? "" : "(" + Duration + "T)");}//(Duration > 1 ? " T)" : " TURN)"));}
	}
	public string FX;
	// Use this for initialization
	public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

	public virtual bool CheckDuration()
	{
		if(Duration > 0) Duration -= 1;
		return Duration == 0;
	}

	public virtual void OnAdd()
	{
		
	}

	public virtual bool CanAttack()
	{
		return true;
	}

	[HideInInspector]
	public string [] Args;
	public virtual void GetArgs(int _duration, params string [] args)
	{
		Args = args;
		Duration = _duration;
	}

	public virtual void StatusEffect()
	{

	}
}
