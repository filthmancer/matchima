using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class MiniAlertUI : UIObj {

	public float lifetime;
	private Vector3 velocity = Vector3.up;
	private float _speed = 0.4F;
	public string text;
	public float size;
	public bool Gravity;
	public bool DestroyOnEnd = true;
	private List<Action> EndActions = new List<Action>();

	private bool DoJuiceScale;
	private Curve3D JuiceScale;
	private float juice_time_curr, juice_time_total;
	private Vector3 gravity_velocity
	{
		get{
			return Gravity ? new Vector3(0.0F,-0.06F, 0.0F) : Vector3.zero;
		}
	}

	private ObjectPoolerReference poolref;

	bool ended = false;
	// Update is called once per frame
	void Update () {
		if(DoJuiceScale)
		{
			if(juice_time_curr < juice_time_total)
			{
				Txt[0].transform.localScale = Juice.instance.ScaleItNow(
					JuiceScale, Vector3.one, juice_time_curr/juice_time_total, 1.0F);
				juice_time_curr += Time.deltaTime;
			}
			
		}
		Txt[0].text = text;
		if(lifetime < -1.0F) return;
		if(lifetime > 0.0F){
			transform.position += velocity * _speed;
			transform.position += gravity_velocity;

			lifetime -= Time.deltaTime;
			_speed = Mathf.Lerp(_speed, 0.0F, Time.deltaTime * 10);
			
		}
		else 
		{
			if(ended) return;
			ended = true;
			foreach(Action child in EndActions)
			{
				child();
			}
			if(DestroyOnEnd) 
			{
				if(poolref) 
				{
					EndActions.Clear();
					poolref.Unspawn();
				}
				else Destroy(this.gameObject);
			}
		}
	}

	public void Setup(Vector3 position, string _text, float life, float _size, Color col, float speed, bool back)
	{
		transform.position = position;
		lifetime = life;
		text = _text;
		size = _size;
		Txt[0].text = text;
		Txt[0].fontSize = size;
		Txt[0].color = col;
		Txt[0].outlineColor = Color.black;
		_speed = speed;
		Img[0].enabled = back;
		DestroyOnEnd = true;

		Gravity = false;
		ended = false;
	}

	public void Setup(MiniAlertUI prev)
	{
		transform.position = prev.transform.position;
		lifetime = prev.lifetime;
		text = prev.text;
		size = prev.size;
		Txt[0].text = prev.Txt[0].text;
		Txt[0].fontSize = prev.Txt[0].fontSize;
		Txt[0].color = prev.Txt[0].color;
		Txt[0].outlineColor = prev.Txt[0].outlineColor;
		_speed = prev._speed;
		Img[0].enabled = prev.Img[0].enabled;
		DestroyOnEnd = prev.DestroyOnEnd;

		poolref = GetComponent<ObjectPoolerReference>();
		Gravity = false;
		ended = false;
	}

	public void AddAction(Action m)
	{
		EndActions.Add(m);
	}

	public void SetVelocity(Vector3 vel)
	{
		velocity = vel;
	}

	public void AddJuice(Curve3D j, float total = 1.0F)
	{
		JuiceScale = j;
		juice_time_total = total;
		if(juice_time_total > lifetime) juice_time_total = lifetime;
		Txt[0].transform.localScale = Juice.instance.ScaleItNow(
			JuiceScale, Vector3.one, 0.0F, 1.0F);

		juice_time_curr = 0.0F;
		DoJuiceScale = true;
	}

	public void ResetJuice(float f = 0.0F){juice_time_curr = f;}
}
