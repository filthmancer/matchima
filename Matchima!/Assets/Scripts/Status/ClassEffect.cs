using UnityEngine;
using System.Collections;

public class ClassEffect : Status {
	public Unit _Unit;

	public virtual void Setup(Unit c)
	{
		_Unit = c;
	}


	public virtual Stat CheckStats()
	{
		return null;
	}
}
