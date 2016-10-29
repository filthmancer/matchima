using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGear : UIObjTweener {
	public Transform gearTrans;
	public Image [] Arrows;
	public bool Rotate;
	public bool Drag;

	public bool FreeWheelDrag;
	public bool FlipDrag;
	
	public float DragSpeed = 2;
	public float DragLerpThreshold = 0.1F; //Threshold of speed to start lerping
	public float DragLerpSpeed = 5;
	public int DragLerpDivisions; // Divisions of rotation it should lerp to if
								  // it stops dragging.
	public bool DontLandOnSameDivision; //Move to next division if it will land
										//on the same one
	[HideInInspector]
	public bool DoDivisionLerpActions;

	public int ThisDivision
	{
		get{return drag_closest_division;}
	}
	public int LastDivision
	{
		get{return drag_last_division;}
	}


	private bool isDragging;
	private Vector3 dragAcc, dragAcc_avg;
	
	private float drag_timeout = 0.2F;
	private float drag_timeout_curr = 0.0F;

	private int divisionrot
	{
		get{return 360/DragLerpDivisions;}
	}
	private int drag_closest_division;	
	private int drag_last_division;
	private bool drag_divisionlerp = true;

	private Vector3 rotationVelocity;
	private float rotate_audiotime = 0.0F;
	private float rotate_audiocap = 0.1F;

	public void FixedUpdate()
	{
		if(Rotate)
		{
			/*if(rotate_audiotime > rotate_audiocap)
			{
				rotate_audiotime = 0.0F;
				AudioManager.instance.PlayClipOn(this.transform, "UI", "GearTick");
			}
			else rotate_audiotime += Time.deltaTime;*/
			gearTrans.Rotate(rotationVelocity);
		}
	}
	public void Update()
	{
		if(!Rotate)
		{
			if(isPressed && Drag)
			{
				if(Input.GetMouseButtonDown(0)) 
				{
					dragAcc = Vector3.zero;
					drag_divisionlerp = false;
				}
				else dragAcc = new Vector3(-Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

				drag_timeout_curr = 0.0F;
				isDragging = true;
				dragAcc_avg = Vector3.Lerp(dragAcc_avg, dragAcc, Time.deltaTime * 5);
			}
			else
			{
				if(isDragging)
				{
					dragAcc = dragAcc_avg;
					isDragging = false;
				}
				if(drag_timeout_curr > drag_timeout) dragAcc_avg = Vector3.zero;
				else drag_timeout_curr += Time.deltaTime;
				dragAcc = Vector3.Lerp(dragAcc, Vector3.zero, Time.deltaTime * DragSpeed);
			}
			dragAcc.x = Mathf.Clamp(dragAcc.x, -8.5F, 8.5F);

			if(!FreeWheelDrag)
			{
				if(Mathf.Abs(dragAcc.x) > DragLerpThreshold) 
				{
					/*if(rotate_audiotime > rotate_audiocap)
					{
						rotate_audiotime = 0.0F;
						AudioManager.instance.PlayClipOn(this.transform, "UI", "GearTick");
					}
					else rotate_audiotime += Time.deltaTime;*/
					gearTrans.Rotate(new Vector3(0,0,FlipDrag ? -dragAcc.x : dragAcc.x));
				}
					
				else if(DragLerpDivisions > 0)
				{
					if(!drag_divisionlerp && !isDragging)
					{
						drag_divisionlerp = true;
						drag_closest_division = (int)Mathf.Round(gearTrans.eulerAngles.z/divisionrot);
						if(drag_closest_division >= DragLerpDivisions) drag_closest_division -= DragLerpDivisions;
						else if(drag_closest_division <= 0)
							drag_closest_division += DragLerpDivisions;

						if(drag_closest_division == drag_last_division && DontLandOnSameDivision)
						{
							int spindir = dragAcc.x > 0.0F ? 1 : -1;
							spindir = FlipDrag ? -spindir : spindir;
							drag_closest_division = (int)(Mathf.Round(gearTrans.eulerAngles.z/divisionrot)+spindir);
						}

						
						if(DoDivisionLerpActions) OnDivisionLerp(drag_closest_division);
						drag_last_division = drag_closest_division;
						
					}

					gearTrans.rotation = Quaternion.Slerp(
						gearTrans.rotation,
						Quaternion.Euler(0,0,drag_closest_division * divisionrot),
						Time.deltaTime*DragLerpSpeed);
				}
			}
			else 
			{
				if(Mathf.Abs(dragAcc.x) > DragLerpThreshold)
				{

					/*if(rotate_audiotime > rotate_audiocap)
					{
						rotate_audiotime = 0.0F;
						AudioManager.instance.PlayClipOn(this.transform, "UI", "GearTick");
					}
					else rotate_audiotime += Time.deltaTime;*/
				}
				
				gearTrans.Rotate(new Vector3(0,0,FlipDrag ? -dragAcc.x : dragAcc.x));	
			}
		}
	}

	public void MoveToDivision(int i)
	{
		drag_divisionlerp = true;
		drag_closest_division = i;
		drag_last_division = i;
	}

	public void MoveLeft()
	{
		drag_divisionlerp = true;
		drag_closest_division+=1;
		drag_last_division = drag_closest_division;	
	}
	public void MoveRight()
	{
		drag_divisionlerp = true;
		drag_closest_division-=1;
		drag_last_division = drag_closest_division;
	}


	public void AddSpin(float spin)
	{
		dragAcc.x += spin;
	}
	public void SetRotate(bool? active = null, Vector3? speed = null)
	{
		Rotate = active ?? !Rotate;
		rotationVelocity = speed ?? Vector3.zero;
	}

	float roundUp(float numToRound, int multiple)  
	{  
	 if(multiple == 0)  
	 {  
	  return numToRound;  
	 }  

	 float remainder = numToRound % multiple; 
	 if (remainder == 0)
	  {
	    return numToRound; 
	  }

	 return numToRound + multiple - remainder; 
	}  

	public List<Action<int>> DivisionActions = new List<Action<int>>();
	public void OnDivisionLerp(int i)
	{
		AudioManager.instance.PlayClipOn(this.transform, "UI", "GearOpen");
		foreach(Action<int> child in DivisionActions)
		{
			child(i);
		}
	}

	int pstate = 0;
	public void SetToState(int i)
	{
	 	switch(i)
	 	{
	 		case 0:
	 		if(GetTween(pstate)) AudioManager.instance.PlayClipOn(this.transform, "UI", "GearOpen");
	 		SetTween(pstate,false);
	 		
	 		break;
	 		case 1:
	 		if(!GetTween(1)) AudioManager.instance.PlayClipOn(this.transform, "UI", "GearClose");
	 		SetTween(1,true);
	 		pstate = 1;
	 		break;
	 		case 2:
	 		if(!GetTween(2)) AudioManager.instance.PlayClipOn(this.transform, "UI", "GearClose");
	 		SetTween(2,true);
	 		pstate = 2;
	 		break;
	 		case 3:
	 		SetTween(0,true);
	 		pstate = 0;
	 		break;
	 	}
	}

	[HideInInspector]
	public bool isFlashing;
	public void FlashArrows()
	{
		StartCoroutine(_FlashArrows());
	}

	IEnumerator _FlashArrows()
	{
		while(Input.GetMouseButton(0))
		{
			yield return null;
		}
		isFlashing = true;
		AddAction(UIAction.MouseUp, () => {isFlashing = false;});
		float flashtime = 2.8F;
		float alphavel = 0.18F;
		
		while(isFlashing)
		{
			for(int i = 0; i < Arrows.Length; i++)
			{
				Color target = Arrows[i].color;
				target.a += alphavel;
				Arrows[i].color = Color.Lerp(Arrows[i].color, target, Time.deltaTime * 6);
			}

			if(Arrows[0].color.a > 0.9F)
			{
				alphavel = -alphavel;
			}
			else if(Arrows[0].color.a <= 0.0F) 
				alphavel = -alphavel;

			yield return null;
		}
		bool allhidden = false;
		while(!allhidden)
		{
			allhidden = true;
			for(int i = 0; i < Arrows.Length; i++)
			{
				if(Arrows[i].color.a > 0.001F) allhidden = false;
				Color target = Arrows[i].color;
				target.a = 0.0F;
				Arrows[i].color = Color.Lerp(Arrows[i].color, target, Time.deltaTime * 6);
			}

			yield return null;
		}
		
	}
}
