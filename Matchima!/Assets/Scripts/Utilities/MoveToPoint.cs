using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveToPoint : MonoBehaviour {
	public Vector3 Point = Vector3.zero;
	public float Speed = 0.5F;

	public bool ArcingMovement = false;
	public bool LerpingMovement = true;
	public bool DontDestroy = false;

	[HideInInspector]
	public Unit Target;
	[HideInInspector]
	public Tile Target_Tile;
	[HideInInspector]
	public int [] Target_Int;

	private float LerpingSpeed = 1.0F;

	private float arc_power = 0.5F;
	private float arc_decay = 0.02F;
	private Vector3 arc_velocity;
	private Vector3 velocity;

	private float delay = 0.0F;
	private float threshold = 0.17F;
	private float final_scale = 1.0F;
	private float final_scale_time = 7.0F;
	Action method;
	Action<Tile> tilemethod;
	Action<int[]> intmethod;

	private ObjectPoolerReference poolref;

	void Start()
	{
		poolref = GetComponent<ObjectPoolerReference>();
		
	}

	bool wait_for_delay = false;
	// Update is called once per frame
	void Update () {
		if(Steps_curr < Steps.Count)//Point != Vector3.zero)
		{
			Point = Steps[Steps_curr].point;
			if(transform.localScale != Vector3.one * final_scale) 
				transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*final_scale, Time.deltaTime * final_scale_time);
			if(LerpingMovement) transform.position = Vector3.Lerp(transform.position, Point, Speed * LerpingSpeed);
			else 
			{
				velocity = Point - transform.position;
				velocity.Normalize();
				transform.position += velocity * Speed * Time.deltaTime;
			}

			if(ArcingMovement)
			{
				transform.position += arc_velocity * arc_power * Time.deltaTime;
				arc_power -= arc_decay;
			} 

			if(Vector3.Distance(transform.position,Point) < threshold + (Speed * Time.deltaTime)) 
			{
				if(!wait_for_delay)
				{
					delay = Steps[Steps_curr].time;
					wait_for_delay = true;
				}
				else
				{	
					if(delay <= 0.0F)
					{
						Steps_curr ++;
						wait_for_delay = false;
					}
					else delay -= Time.deltaTime;
				}
				
			}
		}
		else 
		{
			if(method != null) method();
			if(tilemethod != null) tilemethod(Target_Tile);
			if(intmethod != null) intmethod(Target_Int);
			if(!DontDestroy)
			{
				ClearAndDestroy();
			}
			else Destroy(this);//GetComponent<ObjectPoolerReference>().Unspawn();
			//ClearAndDestroy();
		}
	}

	public void SetTarget(Vector3 pos)
	{
		Point = pos;
		velocity = Point - transform.position;
		velocity.Normalize();

		MoveStep s;
		s.point = pos;
		s.time = 0.0F;
		Steps.Add(s);
	}

	List<MoveStep> Steps = new List<MoveStep>();
	int Steps_curr = 0;
	struct MoveStep{
		public Vector3 point;
		public float time;
	}
	public void AddStep(Vector3 pos, float time)
	{
		MoveStep s;
		s.point = pos;
		s.time = time;
		Steps.Add(s);
	}

	public void ClearAndDestroy()
	{
		method = null;
		tilemethod = null;
		intmethod = null;
		Target_Tile = null;
		Target_Int = new int [0];
		Target = null;
		Steps = new List<MoveStep>();
		Steps_curr = 0;

		if(poolref) poolref.Unspawn();
		else Destroy(this.gameObject);
	}

	public void SetThreshold(float thresh)
	{
		threshold = thresh;
	}

	public void SetPath(float speed = 25.0F, float _arc = 0.0F, float _lerp = 0.0F, float scale = 1.0F)
	{
		Speed = speed;
		if(_arc != 0.0F)
		{
			ArcingMovement = true;
			arc_velocity = Vector3.Cross(Vector3.forward, velocity);
			if(UnityEngine.Random.value > 0.5F) arc_velocity = -arc_velocity;
			arc_power = Speed * _arc;
			arc_decay = arc_power * 0.04F;
		}
		else ArcingMovement = false;
		if(_lerp != 0.0F) 
		{
			LerpingMovement = true;
			LerpingSpeed = _lerp;
		}
		else LerpingMovement = false;

		final_scale = scale;
	}

	public void SetDelay(float f)
	{
		delay = f;
	}

	public void SetMethod(Action a)
	{
		method = a;
	}

	public void SetTileMethod(Tile t, Action <Tile> a)
	{
		Target_Tile = t;
		tilemethod = a;
	}

	public void SetIntMethod(Action <int[]> a, params int [] i)
	{
		Target_Int = i;
		intmethod = a;
	}

	public void SetScale(float scale, float time = 7)
	{
		final_scale = scale;
		final_scale_time = time;
	}


}
