using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassSlotsUI : UIObj {

	public RectTransform [] BasePoints;
	public UIClassButton [] Class;

	public int Length
	{
		get{return Class.Length;}
	}
	public UIClassButton this[int a]
	{
		get{
			for(int i = 0; i < Class.Length; i++)
			{
				if(Class[i]._class == null) continue;
				if(Class[i]._class.Index == a) return Class[i];
			}
			return null;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
