using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line : MonoBehaviour {

	public Vector2 A;
	public Vector2 B;
	public float Thickness;

	public GameObject LineChild;
	public LineRenderer LineObj;

	void Start()
	{
		Segments = new List<Segment>();
		LineRend = new List<LineRenderer>();
	}

	public Line(Vector2 a, Vector2 b, float thick)
	{
		A = a;
		B = b;
		Thickness = thick;
	}

	public void SetColor(Color col)
	{
		LineChild.GetComponent<SpriteRenderer>().color = col;
	}

	public void SetPoints(Vector3 a, Vector3 b)
	{
		A = new Vector2(a.x, a.y);
		B = new Vector2(b.x, b.y);
		Thickness = 1;
	}

	public void Update()
	{
	}
	
	public void Draw()
	{
		Vector2 difference = B - A;
		float rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		 LineChild.transform.localScale = new Vector3(100 * (difference.magnitude / LineChild.GetComponent<SpriteRenderer>().sprite.rect.width), 
		                                                 Thickness, 
		                                                 LineChild.transform.localScale.z);
		     
		    //Rotate the line so that it is facing the right direction
		    LineChild.transform.rotation = Quaternion.Euler(new Vector3(0,0, rotation));
		     
		    //Move the line to be centered on the starting point
		    LineChild.transform.position = new Vector3 (A.x, A.y, LineChild.transform.position.z);
		     
		    //Need to convert rotation to radians at this point for Cos/Sin
		    rotation *= Mathf.Deg2Rad;
		     
		    //Store these so we only have to access once
		    float lineChildWorldAdjust = LineChild.transform.localScale.x * LineChild.GetComponent<SpriteRenderer>().sprite.rect.width / 2f;
		     
		    //Adjust the middle segment to the appropriate position
		    LineChild.transform.position += new Vector3 (.01f * Mathf.Cos(rotation) * lineChildWorldAdjust, 
		                                                 .01f * Mathf.Sin(rotation) * lineChildWorldAdjust,
		                                                 0);
	}

	List<Segment> Segments = new List<Segment>();
	List<LineRenderer> LineRend = new List<LineRenderer>();
	float offset = 0.1F;
	Vector2 MidPoint;
	int generations = 1;
	public void DrawLightning()
	{
		generations = 1;
		foreach(LineRenderer child in LineRend)
		{
		 	Destroy(child.gameObject);
		}
		LineRend.Clear();
		Segments.Clear();
		Segments.Add(new Segment(A, B));
		offset = 0.1F;
		print(generations);
		for(int i = 0; i < generations; i++)
		{
			for(int s = 0; s < Segments.Count; s++)
			{
				Vector2 _a = Segments[s].a;
				Vector2 _b = Segments[s].b;
				//Segments.RemoveAt(s);
				MidPoint = Vector2.Lerp(_a, _b, 0.5F);
				MidPoint += (_b - _a).normalized * Random.Range(-offset, offset);

				Segments.Add(new Segment(_a, MidPoint));
				Segments.Add(new Segment(MidPoint, _b));

			}
			offset /= 2;
		}


		foreach(Segment child in Segments)
		{
		 	LineRenderer l = (LineRenderer) Instantiate(LineObj);
		 	l.SetPosition(0, child.a);
		 	l.SetPosition(1, child.b);
		 	LineRend.Add(l);
		}
	}
}

public class Segment
{
	public Vector2 a, b;
	LineRenderer _Line; 
	public Segment(Vector2 _a, Vector2 _b)
	{
		a = _a;
		b = _b;
	}
}
