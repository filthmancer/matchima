using UnityEngine;
using System.Collections;
using System;

public class MoveToPoint : MonoBehaviour {
	public Vector3 Point = Vector3.zero;
	public float Speed = 0.5F;

	public bool ArcingMovement = false;
	public bool LerpingMovement = true;
	public bool DontDestroy = false;

	private float LerpingSpeed = 1.0F;

	private float arc_power = 0.5F;
	private float arc_decay = 0.02F;
	private Vector3 arc_velocity;
	private Vector3 velocity;

	private float delay = 0.0F;
	private float threshold = 0.2F;
	Action method;

	// Update is called once per frame
	void Update () {
		if(Point != Vector3.zero)
		{
			if(LerpingMovement) transform.position = Vector3.Lerp(transform.position, Point, Speed * LerpingSpeed);
			else 
			{
				velocity = Point - transform.position;
				velocity.Normalize();
				transform.position += velocity * Speed;
			}

			if(ArcingMovement)
			{
				transform.position += arc_velocity * arc_power;
				arc_power -= arc_decay;
			} 

			if(Vector3.Distance(transform.position,Point) < threshold) 
			{
				
				if(delay <= 0.0F)
				{
					if(method != null) method();
					if(!DontDestroy) Destroy(this.gameObject);
					else Destroy(this);
				}
				else delay -= Time.deltaTime;
			}
		}
		else Destroy(this.gameObject);
	}

	public void SetTarget(Vector3 pos)
	{
		Point = pos;
		velocity = Point - transform.position;
		velocity.Normalize();

	}

	public void SetThreshold(float thresh)
	{
		threshold = thresh;
	}

	public void SetPath(float speed = 0.5F, bool? Arc = null, bool? Lerp = null, float _arc = 0.3F, float _lerp = 1.0F)
	{
		if(Arc.HasValue)
		{
			ArcingMovement = (bool) Arc;
			if(ArcingMovement)
			{
				arc_velocity = Vector3.Cross(Vector3.forward, velocity);
				if(UnityEngine.Random.value > 0.5F) arc_velocity = -arc_velocity;
				arc_power = Speed * _arc;
				arc_decay = arc_power * 0.04F;
			}
		}
		if(Lerp.HasValue) 
		{
			LerpingMovement = (bool)Lerp;
			LerpingSpeed = _lerp;
		}
	}

	public void SetDelay(float f)
	{
		delay = f;
	}

	public void SetMethod(Action a)
	{
		method = a;
	}
}
