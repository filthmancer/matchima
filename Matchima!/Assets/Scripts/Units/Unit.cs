using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	public int Index = 0;
	public GENUS Genus;
	public string Name;
	public List<ClassEffect> _Status = new List<ClassEffect>();

	void Awake()
	{
		Name = Name.Replace("!N", "\n");
	}
}
