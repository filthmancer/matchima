using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CrewMenuUIObj : UIObjtk {

	public MiniAlertUI ManaAlert;

	public Tile Target;
	public Image Death;

	public tk2dClippedSprite _FrameMask;
	
	/*Imgtk 
	0 - Icon
	1 - Frame
	2 - Mask
	*/

	/* Txt
	0 - 
	*/

	private bool class_set = false;

	public void Setup(Tile ab)
	{
		if(ab == null) 
		{
			_FrameMask.gameObject.SetActive(true);
			_FrameMask.clipBottomLeft = new Vector2(0,0);
			//Img[0].sprite = UIManager.Menu.NoHeroInSlot;
			return;
		}
		//else transform.parent.gameObject.SetActive(true);

		Target = ab;
			
		string render = Target.GenusName;
		tk2dSpriteDefinition id = Target.Inner.GetSpriteDefinition(render);
		if(id == null) render = "Alpha";
		Imgtk[0].SetSprite(Target.Inner, render);
		Imgtk[0].scale = (Target is Hero) ? new Vector3(-2F, 2F, 1.0F) : Vector3.one * 11.2F;
		Imgtk[1].SetSprite(TileMaster.Genus.Frames, Target.Info.Outer);
		Imgtk[2].color = GameData.Colour(Target.Genus);

		_FrameMask.gameObject.SetActive(true);
		_FrameMask.clipTopRight = new Vector2(1,1);

		class_set = true;

		ClearActions();

		AddAction(UIAction.MouseDown, () => {
			if(!GameManager.instance.isPaused) 
				UIManager.instance.TargetTile(Target);
		});
	}
}
