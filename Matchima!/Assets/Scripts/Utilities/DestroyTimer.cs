using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {
	public float Timer;
	public float current = 0.0F;
	private ObjectPoolerReference poolref;
	void Update()
	{
		current += Time.deltaTime;
		if(current > Timer) 
		{
			poolref = GetComponent<ObjectPoolerReference>();
			if(poolref) 
			{
				current = 0.0F;
				poolref.Unspawn();
			}
			else Destroy(this.gameObject);
		}
	}

	public void Reset()
	{
		current = 0.0F;
	}
}
