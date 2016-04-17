using UnityEngine;
using System.Collections;
using System;

public class MiniTile : MonoBehaviour {

	public SpriteRenderer _render;
	public Transform target;

	private Vector3 targetpos;
	
	Vector3 velocity;
	float speedX = 0.3F;
	float speedY = 0.3F;
	public float acc = 0.06F;

	Action method;
	public Class _Class;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null)
		{
			float d = Vector2.Distance(targetpos, transform.position);
			if(d > 0.5F)
			{
				velocity = targetpos - transform.position;
				velocity.Normalize();

				Vector3 nextpos = transform.position + (new Vector3(velocity.x * speedX, velocity.y * speedY, velocity.z));
				nextpos.z = -8;

				transform.position = nextpos;
				speedX += acc;
				speedY += acc;
				transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.07F, Time.deltaTime * 7);
			}
			else 
			{
				if(method != null) method();
				Destroy(this.gameObject);
			}

		}
		else Destroy(this.gameObject);
	}

	public void SetTarget(Transform _target, float _scale = 0.23F, float _speedY = -0.2F)
	{
		target = _target;
		speedY = _speedY;
		transform.localScale = Vector3.one * _scale;
		Vector3 offset = Vector3.left * (UnityEngine.Random.value - UnityEngine.Random.value) * 0.2F;
		targetpos = target.transform.position + offset * 2;
		transform.position += offset;
		speedX = 0.2F;//Random.value > 0.5F ? 0.2F : -0.2F;
		_render.sortingOrder = 3;

	}

	public void SetMethod(Action a)
	{
		method = a;
	}
}
