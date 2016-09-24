using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPooler
{
	GameObject _Obj;
	Transform Parent;
	public int Count;

	public Stack<GameObject> Available;
	public ArrayList All;
	public Vector3 PoolPos = new Vector3(0, -50, 0);

	public bool IsAvailable
	{
		get{return Available.Count > 0;}
	}

	public ObjectPooler(GameObject u, int num, Transform _parent)
	{
		_Obj = u;
		Parent = _parent;
		Available = new Stack<GameObject>();
		All = new ArrayList(num);
		for(int i = 0; i < num; i++)
		{
			GameObject objtemp = InstantiateObj();
			Available.Push(objtemp);
			All.Add(objtemp);
			objtemp.SetActive(false);
		}
	}

	public GameObject Spawn()
	{
		GameObject result;
		if(Available.Count == 0)
		{
			result =  InstantiateObj();
			All.Add(result);
		}
		else
		{
			result = Available.Pop();
		}
		if(result != null) result.gameObject.SetActive(true);
		return result;
	}

	public bool Unspawn(GameObject t)
	{
		if(!Available.Contains(t))
		{
			Available.Push(t);
			t.transform.position = PoolPos;
			t.transform.SetParent(Parent);
			t.gameObject.SetActive(false);
			return true;
		}
		return false;
	}

	public GameObject InstantiateObj()
	{
		GameObject obj = (GameObject) GameObject.Instantiate(_Obj);
		obj.transform.position = PoolPos;
		obj.transform.SetParent(Parent);
		ObjectPoolerReference refer = obj.AddComponent<ObjectPoolerReference>();
		refer.SetPool(this);
		return obj;
	}
}
