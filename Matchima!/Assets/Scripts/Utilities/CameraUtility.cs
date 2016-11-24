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
	public Transform Background;

	public static Vector3 TargetPos;
	public static float TargetOrtho = 0.0F;

	public static float OrthoFactor
	{
		get{
			if(!instance.IgnoreTargetOrtho)	return 1.0F + (TargetOrtho-6.6F)*0.03F;
			else return 1.0F + (instance.Cam.CameraSettings.orthographicSize - 6.6F) * 0.03F;
		}
	}

	public bool IgnoreTargetOrtho = false;

	public tk2dCamera Cam, TrackCam;
	public tk2dUICamera UICam;
	private static float yOffset = 0.00F;

	private static Vector3 TurnOffsetA, TurnOffsetB;
	private static bool TurnOffset_enabled;

	public float background_factor_a = 2.0F, background_factor_b = 5.0F;

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
			//print(TargetOrtho);
			Cam.CameraSettings.orthographicSize = Mathf.Lerp(Cam.CameraSettings.orthographicSize, TargetOrtho, Time.deltaTime * 8);
			TrackCam.CameraSettings.orthographicSize = Mathf.Lerp(TrackCam.CameraSettings.orthographicSize, TargetOrtho, Time.deltaTime * 8);
			UICam.HostCamera.orthographicSize = Mathf.Lerp(UICam.HostCamera.orthographicSize, TargetOrtho, Time.deltaTime * 8);
		}

		Background.localScale = (Vector3.one * background_factor_a) + Vector3.one * (OrthoFactor * background_factor_b);
		/*if(!isShaking && TargetRoom != null) 
		{
			Vector3 vel = TargetPos -Cam.transform.position;
			if(Vector3.Distance(vel, Vector3.zero) > 0.2F)
			{
				Cam.transform.position += vel.normalized * Time.deltaTime * 15;
			}
			
		}*/
		//Vector3.Lerp(Cam.transform.position, TargetPos, Time.deltaTime*2);
	}

	float max_intensity = 0.7F;
	public void ScreenShake(float intensity, float time)
	{
		if(isShaking)
		{
			currentIntensity = Mathf.Clamp(currentIntensity + intensity, 0.0F, max_intensity);
			currentTime += time*0.7F;
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
		currentIntensity = Mathf.Clamp(intensity, 0, max_intensity);
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


	public static GridInfo TargetRoom;
	public void SetTargetRoom(GridInfo r)
	{
		//StartCoroutine(MoveToRoom(r));
		/*TargetRoom = r;
		TargetPos = r.Position;

		TargetPos.z = -18.8F;
		TargetPos.y += yOffset * r.Size[1];*/
	}

	public IEnumerator MoveToRoom(GridInfo r)
	{
		float pullout_timer = 0.3F;
		while((pullout_timer -= Time.deltaTime) > 0.0F)
		{
			TargetOrtho += Time.deltaTime * 7;
			yield return null;
		}

		yield return new WaitForSeconds(Time.deltaTime * 5);

		TargetRoom = r;
		TargetPos = r.CamPosition;
		TargetPos.z = -18.8F;
		TargetPos.y += yOffset * r.Size[1];

		while(Vector3.Distance(Cam.transform.position, TargetPos) > 0.2F)
		{
			Vector3 vel = TargetPos - Cam.transform.position;
			Cam.transform.position += vel * Time.deltaTime * 6;
			yield return null;
		}

		Cam.transform.position = TargetPos;
		
		float ortho = Mathf.Max(r.Size[0] * 1.4F, r.Size[1] * 1.15F);
		TargetOrtho = Mathf.Clamp(ortho, 7, Mathf.Infinity);
		IgnoreTargetOrtho = false;
		yield return null;

	}

	public static void SetTurnOffset(bool active)
	{	
		TurnOffset_enabled = active;
	}
}
