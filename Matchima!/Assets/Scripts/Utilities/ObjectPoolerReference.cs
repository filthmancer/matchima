using UnityEngine;

using System.Collections;

public class ObjectPoolerReference : MonoBehaviour {

	private ObjectPooler Pool;
	public void SetPool(ObjectPooler p)
	{
		Pool = p;
	}

	public void Unspawn()
	{
		Pool.Unspawn(this.gameObject);
	}
}


