using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ActiveTimer : MonoBehaviour {

	public bool DestroyOnEnd;
	[SerializeField]
	protected float Lifetime;

	protected float Current_time;
	protected List<TimedAction> TimedActions = new List<TimedAction>();
	protected List<Action> EndActions = new List<Action>();
	// Update is called once per frame
	public virtual void Update () {
		if(Current_time < Lifetime)
		{
			foreach(TimedAction child in TimedActions)
			{
				if(child.Type == TimerType.OnEnd) continue;
				switch(child.Type)
				{
					case TimerType.NoTimer:
					child.method();
					break;
					case TimerType.TickTimer:
					if(Current_time % child.Timer == 0.0F) child.method();
					break;
					case TimerType.PostTimer:
					if(Current_time > child.Timer) child.method();
					break;
					case TimerType.OnTime:
					if(Mathf.Abs(Current_time - child.Timer) < Time.deltaTime)
					{
						child.method();
					}
					break;

				}				
			}
			Current_time += Time.deltaTime;
		}
		else
		{
			foreach(TimedAction child in TimedActions)
			{
				if(child.Type == TimerType.OnEnd) child.method();
			}
			if(DestroyOnEnd) Destroy(this.gameObject);
		}
	}

	public void SetLifetime(float life)
	{
		Lifetime = life;
	}

	public void AddTimedAction(Action m, TimerType t, float time = 0.0F)
	{
		TimedActions.Add(new TimedAction(m, t, time));
	}


}

public enum TimerType
{
	NoTimer, //Action will check every frame
	TickTimer, // Action will check every tick
	PostTimer, //Action will check after this time
	OnTime, //Action will check on this time
	OnEnd //Action will check at the end of lifetime
}

public class TimedAction
{
	public float Timer;
	public Action method;
	public TimerType Type;
	public TimedAction(Action m, TimerType t, float time)
	{
		Type = t;
		Timer = time;
		method = m;
	}
}

