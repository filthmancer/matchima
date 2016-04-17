using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {
	public float Timer;
	private float current = 0.0F;
	void Update()
	{
		current += Time.deltaTime;
		if(current > Timer) Destroy(this.gameObject);
	}
}
