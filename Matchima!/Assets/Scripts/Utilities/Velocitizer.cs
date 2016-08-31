using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Velocitizer : ActiveTimer {
	
	public bool Gravity;
	[SerializeField]
	private Vector3 Velocity;
	[SerializeField]
	private Vector3 Rotation;

	private float Speed = 0.2F;

	private Transform trans;
	private Vector3 gravity_velocity
	{
		get{
			return Gravity ? new Vector3(0.0F,-2.44F, 0.0F) : Vector3.zero;
		}
	}
	// Update is called once per frame
	public override void Update () {
		base.Update();
		if(trans == null) trans = this.transform;
		trans.position += Velocity * Speed * Time.deltaTime;
		trans.Rotate(Rotation);
		if(Gravity) Velocity += gravity_velocity * Time.deltaTime;
	}

	public void SetVelocity(Vector3 vel, float spd = 1.0F)
	{
		Velocity = vel.normalized;
		Speed = spd;
	}

	public void SetRotation(Vector3 vel)
	{
		Rotation = vel;
	}
}