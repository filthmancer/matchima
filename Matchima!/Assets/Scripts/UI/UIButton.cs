using UnityEngine;
using System.Collections;

using UnityEngine.UI;


public class UIButton : UIObj {


	protected Color color_default;

	protected bool over;
	protected bool dragging;
	protected float drag_distance = 1.2F;
	protected bool activated = false;
	protected bool prepare_to_swap;
	public DragType Drag;

	public ClassUpgrade Upgrade;

	protected Vector3 init_pos;
	
	// Update is called once per frame
	public virtual  void Update () {

		if(!Application.isEditor && Input.touches.Length == 0) over = false;
		if(over && !dragging)
		{
			
			UIManager.instance.ShowTooltip(true, this);
			over = false;
		}

		if(activated)
			{
				if(Drag == DragType.None) return;
				Vector2 pos;
		        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Canvas.transform as RectTransform, Input.mousePosition, UIManager.Canvas.worldCamera, out pos);
		       	Vector3 dpos = UIManager.Canvas.transform.TransformPoint(pos);
		       	dpos.z = init_pos.z;
				if(!dragging)
				{
					if(Vector3.Distance(dpos, init_pos) > drag_distance) 
					{
						dragging = true;
						Img[0].transform.position = this.transform.position;
						if(Drag == DragType.Hold)
						{
							PlayerControl.HoldingSlot = true;
							PlayerControl.HeldButton = this;
							GetComponent<LayoutElement>().ignoreLayout = true;
							GetComponent<CanvasGroup>().blocksRaycasts = false;
							
						}
						else if(Drag == DragType.Cast)
						{
							PlayerControl.HoldingSlot = true;
							PlayerControl.HeldButton = this;
							PlayerControl.TouchParticle = (ParticleSystem) Instantiate(EffectManager.instance.Particles.SpellParticle);
						}
					}
					else Img[0].transform.position = Vector3.Lerp(init_pos, dpos, 0.3F);
				}
				else
				{
					prepare_to_swap = Vector3.Distance(dpos, init_pos) > drag_distance * 2;
					
					if(Drag == DragType.Hold) transform.position = Vector3.Lerp(transform.position,dpos, Time.deltaTime * 15);
				}
			}

		//transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * (over ? 1.15F : 1.0F), Time.deltaTime * 20);
	
	
	}

	public virtual StCon [] GetTooltip()
	{
		return (Upgrade != null ? Upgrade.Tooltip : null);
	}

	public virtual string GetName()
	{
		return (Upgrade != null ? Upgrade.Name : "");
	}
	public virtual Color GetColour()
	{
		return Color.white;
	}

	public virtual void Setup(Slot s)
	{
		if(Drag != DragType.None)
		{
			dragging = false;
			init_pos = transform.position;
			drag_distance = GetComponent<LayoutElement>().preferredWidth/200;
		}
		
	}

	public virtual void MouseDown()
	{
		if(Drag != DragType.None)
		{
			activated = true;
			init_pos = transform.position;
		}
	}

	public virtual void MouseUp()
	{	
		if(prepare_to_swap)
		{
			if(PlayerControl.HeldButton != null)
			{
				if(PlayerControl.HeldButton == this)
				{
					//StartCoroutine(Upgrade.Upgrade);
					UIManager.instance.current_class.GetUpgrade(Upgrade);
				}
			}
		}
		
		if(Drag != DragType.None)
		{
			dragging = false;
			activated = false;
			PlayerControl.HoldingSlot = false;
			if(PlayerControl.TouchParticle != null) Destroy(PlayerControl.TouchParticle.gameObject);
			GetComponent<LayoutElement>().ignoreLayout = false;
			GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
		
	}

	public virtual void MouseOver()
	{
		over = true;
	}

	public virtual void MouseOut()
	{
		over = false;
		//UIManager.instance.ShowTooltip(false);
	}

}


