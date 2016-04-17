using UnityEngine;
using System.Collections;
using TMPro;

public class TileParamContainer : MonoBehaviour {

	public GameObject HitCounter;
	public TextMeshPro counter, otherWarning, HitCounterText;
	
	public LineRenderer lineIn, lineOut;
	public SpriteRenderer _render, _border, _shiny;

	public SpriteRenderer _effect;

	public Vector3 _render_defaultpos;
	public Vector3 _render_defaultscale;

	public void Start()
	{
		_render_defaultpos = _render.transform.localPosition;
		_render_defaultscale = _render.transform.localScale;
	}
	

}
