using UnityEngine;
using System.Collections;

public class LoadScreen : UIObj {
	bool isSpinning = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isSpinning)
		{
			Img[0].transform.Rotate(0,0,Time.deltaTime * 45);
		}
	}

	public void SetSpin(bool active)
	{
		isSpinning = active;
	}
}
