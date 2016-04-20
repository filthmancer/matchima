using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	public int Index = 0;
	public GENUS Genus;
	public string Name;
	public List<ClassEffect> _Status = new List<ClassEffect>();
	// Use this for initialization
	public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}
}
