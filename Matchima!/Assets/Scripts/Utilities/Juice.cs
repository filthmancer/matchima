using UnityEngine;
using System.Collections;

public class Juice : MonoBehaviour {
	public static Juice instance
	{
		get {return GameManager.instance._Juice;}
	}
	public AnimationCurve ClockworkA;

	public AnimationCurve BounceA;

	public JuiceIt Squish;
	public JuiceIt Ripple;
	public JuiceIt Twitch;
	public JuiceIt Attack;

	public static JuiceIt _Squish
	{
		get{
			return Juice.instance.Squish;
		}
	}

	public static JuiceIt _Ripple
	{
		get
		{
			return Juice.instance.Ripple;
		}
	}

	public static JuiceIt _Attack
	{
		get{return Juice.instance.Attack;}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float CurvePoint(AnimationCurve anim, float t)
	{
		return anim.Evaluate(t);
	}

	public Vector3 JuiceItNow(Curve3D j, Vector3 init, float curr)
	{
		return Vector3.Scale(init, j.Evaluate(curr));
	}

	public void JuiceIt(JuiceIt j, Transform t, float time, float intensity = 1.0F)
	{
		StartCoroutine(JuiceRoutine(j, t, time, intensity));
	}

	IEnumerator JuiceRoutine(JuiceIt j, Transform t, float time, float inten)
	{
		if(t == null) yield break;
		float curr = 0.0F;
		Vector3 pos_init = t.position;
		Vector3 rot_init = t.eulerAngles;
		Vector3 scale_init = t.localScale;

		while(curr < time)
		{
			if(t == null) yield break;
			t.position += j.Position.Evaluate(curr/time) * inten;
			t.rotation *= Quaternion.Euler(j.Rotation.Evaluate(curr/time));

			Vector3 finscale = j.Scale.Evaluate(curr/time);
			finscale.x = 1.0F - ((1.0F - finscale.x) * inten);
			finscale.y = 1.0F - ((1.0F - finscale.y) * inten);
			finscale.z = 1.0F - ((1.0F - finscale.z) * inten);

			t.localScale = Vector3.Scale(scale_init, finscale);

			curr += Time.deltaTime;
			yield return null;
		}

		if(t == null) yield break;
		t.position = pos_init;
		t.rotation = Quaternion.Euler(rot_init);
		t.localScale = scale_init;
	}
}

[System.Serializable]
public class Curve3D
{
	public AnimationCurve x, y, z;

	public Vector3 Evaluate(float t)
	{
		return new Vector3(x.Evaluate(t), y.Evaluate(t),z.Evaluate(t));
	}
}

[System.Serializable]
public class JuiceIt
{
	public Curve3D Scale;
	public Curve3D Position;
	public Curve3D Rotation;
}
