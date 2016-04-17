using UnityEngine;
using System.Collections;

public class ClassEffect : Status {
	public Class _Class;

	public virtual void Setup(Class c)
	{
		_Class = c;
	}


	public virtual Stat CheckStats()
	{
		return null;
	}
}
