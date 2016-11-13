using UnityEngine;
using System.Collections;

public class CrewMenuUIObj : UIObjtk {

	public MiniAlertUI ManaAlert;

	public Class _class;

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
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Setup(Class ab)
	{
		if(ab == null) 
		{
			_FrameMask.gameObject.SetActive(true);
			_FrameMask.clipBottomLeft = new Vector2(0,0);
			//Img[0].sprite = UIManager.Menu.NoHeroInSlot;
			return;
		}
		//else transform.parent.gameObject.SetActive(true);

		_class = ab;
			
		//Imgtk[0].sprite = ab.GetIcon();
		//Imgtk[0].color = Color.white;
		//Imgtk[0].enabled = true;
		//_SpriteMask.color = new Color(1,1,1,0.2F);
		//_SpriteMask.enabled = false;

		_FrameMask.gameObject.SetActive(true);
		_FrameMask.clipTopRight = new Vector2(1,1);

		class_set = true;

		ClearActions();

		AddAction(UIAction.MouseDown, () => {
			if(_class.MeterLvl > 0 && !GameManager.instance.isPaused) 
				UIManager.instance.TargetTile(_class._Tile);
		});
	}
}
