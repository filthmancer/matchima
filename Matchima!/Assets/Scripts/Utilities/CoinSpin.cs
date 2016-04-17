using UnityEngine;
using System.Collections;

public class CoinSpin : MonoBehaviour {
	public float Speed = 4.0F;
	public float Decay = 0.03F;

	public bool Output = false;

	public bool IsSpinning = false;
	private float Speed_current = 0.0F;
	private float TrueChance = 0.55F;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S))
		{
			StartCoroutine(Spin());
		}

		
	}

	public void StartSpin()
	{
		IsSpinning = true;
		Speed_current = Speed * Random.Range(0.6F, 1.8F);
	}

	public void CheckFinal()
	{
		float rot_y = transform.eulerAngles.y;
		Output = (rot_y < 90 || rot_y > 270);
	}

	public IEnumerator Spin()
	{
		IsSpinning = true;
		Speed_current = Speed;

		while(IsSpinning)
		{
			if(Speed_current > Decay)
			{
				transform.Rotate(0, Speed_current,0);
				Speed_current -= Decay;
			}
			else 
			{
				IsSpinning = false;
				CheckFinal();
			}
			yield return null;
		}

		float value = Random.value;
		Output = value < TrueChance;
	
		bool IsFinalising = true;
		float final_y = (Output ? 0: 180);
		while(IsFinalising)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, final_y, 0), Time.deltaTime * 5);
			if(Mathf.Abs(transform.eulerAngles.y-final_y) < 5.0F) IsFinalising = false;
			yield return null;
		}

		yield return null;
	}
}
