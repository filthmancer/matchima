using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassSlotsUI : UIObj {

	public RectTransform [] BasePoints;
	public UIClassButton [] Class;


	public UIClassButton GetClass(int a)
	{
			if(a < Class.Length) return Class[a];
			return null;
	}
}
