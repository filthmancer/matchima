using UnityEngine;
using System.Collections;
using System;

public class Utility : MonoBehaviour {

	public static int RandomInt(int a)
	{
		return (int) UnityEngine.Random.Range(0, a);
	}

	public static int RandomInt(float a){
		return (int) UnityEngine.Random.Range(0, a);
	}

	public static Vector3 RandomVector(float x, float y, float z)
	{
		return new Vector3(UnityEngine.Random.Range(0, x), UnityEngine.Random.Range(0, y), UnityEngine.Random.Range(0,z));
	}

	public static Vector3 RandomVectorInclusive(float x, float y, float z)
	{
		return new Vector3(UnityEngine.Random.Range(-x, x), UnityEngine.Random.Range(-y, y), UnityEngine.Random.Range(-z,z));
	}

	public static int [] IntNormal(int [] x, int [] y)
	{
		int a = y[0] - x[0];
		int b = y[1] - x[1];
		a = a >= 1 ? 1 : (a <= -1 ? -1 : 0);
		b = b >= 1 ? 1 : (b <= -1 ? -1 : 0);
		return new int [] 
		{
			a,
			b
		};
	}

	public static void Flog(params object [] s)
	{
		string final = "";
		for(int i = 0; i < s.Length; i++)
		{
			final += s[i].ToString();
			if(i < s.Length-1) final += " : ";
		}
		Debug.Log(final);
	}
}
