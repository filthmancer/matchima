using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniTile2 : MonoBehaviour {
	public tk2dSprite _Border, _Render;
	
	public void Setup(Tile t)
	{
		transform.position = t.transform.position;
		transform.position -= Vector3.forward * 2;
		_Border.SetSprite(TileMaster.Genus.Frames, t.Info.Outer);

		string render = t.InnerRender();
		tk2dSpriteDefinition id = t.InnerAtlas().GetSpriteDefinition(render);
		if(id == null) render = "Alpha";
		_Render.SetSprite(t.InnerAtlas(), render);
	}

	public void Explode()
	{
		Vector3 vel =  Utility.RandomVectorInclusive(1,1,0);
		Vector3 startpoint = transform.position + vel;
		Vector3 endpoint = transform.position - vel;

		/*_Border.GetComponent<Rigidbody2D>().isKinematic = false;
		SpriteSlicer2D.s_overrideColor = new Color(0.6F,0.6F,0.6F,0.7F);
		SpriteSlicer2D.ExplodeSprite(_Border.gameObject, 5, 150);
		
		//SpriteSlicer2D.SliceSprite(startpoint, endpoint, t.Params._border.gameObject, false, ref info);
		//SpriteSlicer2D.ShatterSprite(t.Params._border.gameObject, 300);

		_Render.GetComponent<Rigidbody2D>().isKinematic = false;
		SpriteSlicer2D.ExplodeSprite(_Render.gameObject, 5, 150);
		//SpriteSlicer2D.SliceSprite(startpoint, endpoint, t.Params._render.gameObject, false, ref info);
		//SpriteSlicer2D.ShatterSprite(t.Params._render.gameObject, 50);*/
	}
}
