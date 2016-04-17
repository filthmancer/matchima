using UnityEngine;
using System.Collections;

public class CameraUtility : MonoBehaviour {
	public static CameraUtility instance;
	void Awake(){instance = this;}

	public static Vector3 TargetPos;
	public static float TargetOrtho = 0.0F;

	public Camera Cam;
	private static float yOffset = -0.02F;

	void Start() {
		//Cam = GetComponent<Camera>();
		TargetPos = Cam.transform.position;
		TargetOrtho = 6.6F;
	}

	bool isShaking;
	float currentIntensity;

	void Update()
	{
		Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, TargetOrtho, Time.deltaTime * 15);
		if(!isShaking && TileMaster.Grid != null) Cam.transform.position = Vector3.Lerp(Cam.transform.position, TargetPos, Time.deltaTime * 15);
	}

	public void ScreenShake(float time, float intensity)
	{
		if(isShaking)
		{
			if(currentIntensity < intensity)
			{
				StopAllCoroutines();
				StartCoroutine(ScreenShakeRoutine(time, intensity));
			}
		}
		else
		{
			StartCoroutine(ScreenShakeRoutine(time, intensity));
		}
	}

	IEnumerator ScreenShakeRoutine(float time, float intensity)
	{
		isShaking = true;
		currentIntensity = intensity;

		Vector3 init_pos = transform.position;

		float time_c = 0.0F;
		Vector3 next_pos = Utility.RandomVector(0.45F,0.45F,0) + Utility.RandomVector(-0.45F,-0.45F,0);
		next_pos *= intensity;

		Vector3 final_pos;

		while(time_c < time)
		{
			final_pos = init_pos + next_pos;
			transform.position = Vector3.Lerp(transform.position, final_pos, Time.deltaTime * 50);

			if(Vector3.Distance(transform.position, final_pos) < 0.2F)
			{
				next_pos = Utility.RandomVector(0.45F,0.45F,0) + Utility.RandomVector(-0.45F,-0.45F,0);
				next_pos *= intensity;
			}
			time_c += Time.deltaTime;
			yield return null;
		}

		isShaking = false;
		transform.position = init_pos;
	}

	public static void SetTargetPos(Vector3 pos)
	{
		TargetPos = pos;
		TargetPos.z = -18.8F;
		TargetPos.y += yOffset * TileMaster.Grid.Size[1];

	}
}
