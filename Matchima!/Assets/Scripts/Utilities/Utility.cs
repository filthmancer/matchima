using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	public static int RandomInt(int a)
	{
		return (int) Random.Range(0, a);
	}

	public static int RandomInt(float a){
		return (int) Random.Range(0, a);
	}

	public static Vector3 RandomVector(float x, float y, float z)
	{
		return new Vector3(Random.Range(0, x), Random.Range(0, y), Random.Range(0,z));
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
}
