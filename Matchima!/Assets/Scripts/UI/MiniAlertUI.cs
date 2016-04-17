using UnityEngine;
using System.Collections;
using TMPro;

public class MiniAlertUI : UIObj {

	public float lifetime;
	public Vector3 velocity = Vector3.up;
	public float _speed = 0.4F;
	public string text;
	public float size;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(lifetime < -1.0F) return;
		if(lifetime > 0.0F){
			transform.position += velocity * _speed;
			lifetime -= Time.deltaTime;
			_speed = Mathf.Lerp(_speed, 0.0F, Time.deltaTime * 10);
			Txt[0].text = text;
			Txt[0].fontSize = Mathf.Lerp(Txt[0].fontSize, size, Time.deltaTime*10);
		}
		else Destroy(this.gameObject);
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
		_speed = speed;
		Img[0].enabled = back;
	}
}
