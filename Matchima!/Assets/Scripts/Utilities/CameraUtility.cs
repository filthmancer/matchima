using UnityEngine;
using System.Collections;

public class CameraUtility : MonoBehaviour {
	public static CameraUtility instance;
	
	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this) 
		{
			instance.Invoke("Start",0.05F);
			Destroy(this.gameObject);
		}
	}

	public Light MainLight;

	public static Vector3 TargetPos;
	public static float TargetOrtho = 0.0F;

	public static float OrthoFactor
	{
		get{
			return 1.0F + (TargetOrtho-6.6F)*0.03F;
		}
	}

	public bool IgnoreTargetOrtho = false;

	public tk2dCamera Cam;
	public tk2dUICamera UICam;
	private static float yOffset = 0.00F;

	private static Vector3 TurnOffsetA, TurnOffsetB;
	private static bool TurnOffset_enabled;

	void Start() {
		TargetPos = Cam.transform.position;
		TargetOrtho = 6.6F;
	}

	bool isShaking;
	float currentIntensity;
	float currentTime;

	void Update()
	{
		if(!IgnoreTargetOrtho)
		{
			Cam.CameraSettings.orthographicSize = Mathf.Lerp(Cam.CameraSettings.orthographicSize, TargetOrtho, Time.deltaTime * 8);
			UICam.HostCamera.orthographicSize = Mathf.Lerp(UICam.HostCamera.orthographicSize, TargetOrtho, Time.deltaTime * 8);
		}
		Vector3 final_pos = TargetPos + (TurnOffset_enabled ? TurnOffsetA:TurnOffsetB);
		if(!isShaking && TileMaster.Grid != null) Cam.transform.position = Vector3.Lerp(Cam.transform.position, final_pos, Time.deltaTime * 8);
	}

	public void ScreenShake(float intensity, float time)
	{
		if(isShaking)
		{
			currentIntensity += intensity;
			currentTime += time;
			//if(currentIntensity < intensity)
			//{
				//StopAllCoroutines();
				//StartCoroutine(ScreenShakeRoutine(time, intensity));
			//}
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
		currentTime = time;

		Vector3 init_pos = transform.position;

		float time_c = 0.0F;
		Vector3 next_pos = Utility.RandomVector(0.45F,0.45F,0) + Utility.RandomVector(-0.45F,-0.45F,0);
		next_pos *= intensity;

		Vector3 final_pos;

		while(time_c < currentTime)
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
		currentTime = 0.0F;
		currentIntensity = 0.0F;
		transform.position = init_pos;
	}

	public static void SetTargetPos(Vector3 pos)
	{
		TargetPos = pos;
		TargetPos.z = -18.8F;
		TargetPos.y += yOffset * TileMaster.Grid.Size[1];
		TurnOffsetA = Vector3.up * 0.35F;
		TurnOffsetB = Vector3.down * 0.7F;
	}

	public static void SetTurnOffset(bool active)
	{	
		TurnOffset_enabled = active;
	}
}
