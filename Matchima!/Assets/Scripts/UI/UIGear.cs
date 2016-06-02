using UnityEngine;
using System.Collections;

public class UIGear : UIObjTweener {
	public bool isRotating;
	public Vector3 rotationSpeed;
	public Transform gearTrans;
	public bool Drag;
	private bool isDragging;
	private Vector3 dragAcc, dragAcc_avg;
	private float dragSpeed = 2;
	private float drag_timeout = 0.2F;
	private float drag_timeout_curr = 0.0F;
	public void Update()
	{
		if(isRotating)
		{
			gearTrans.Rotate(rotationSpeed);
		}
		else
		{
			if(Input.GetMouseButton(0) && Drag)
			{
				drag_timeout_curr = 0.0F;
				if(Input.GetMouseButtonDown(0)) dragAcc = Vector3.zero;
				else dragAcc = new Vector3(-Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
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
				dragAcc = Vector3.Lerp(dragAcc, Vector3.zero, Time.deltaTime * dragSpeed);
			}
			dragAcc.x = Mathf.Clamp(dragAcc.x, -8.5F, 8.5F);
			gearTrans.Rotate(new Vector3(0,0,dragAcc.x));
		}
		
	}
	public void SetRotate(bool? active = null, Vector3? speed = null)
	{
		isRotating = active ?? !isRotating;
		rotationSpeed = speed ?? Vector3.zero;
	}

}
