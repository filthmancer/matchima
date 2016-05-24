using UnityEngine;
using System.Collections;

public class UIGear : UIObjTweener {
	public bool isRotating;
	public Vector3 rotationSpeed;

	public bool isDragging;
	private Vector3 dragAcc, dragAcc_avg;
	private float dragSpeed = 2;
	public void Update()
	{
		if(isRotating)
		{
			Img[0].transform.Rotate(rotationSpeed);
		}
		else
		{

			if(Input.GetMouseButton(0) && isDragging)
			{
				dragAcc = new Vector3(-Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
				dragAcc_avg = Vector3.Lerp(dragAcc_avg, dragAcc, Time.deltaTime * 5);
			}
			else
			{
				if(isDragging)
				{
					dragAcc = dragAcc_avg;
					isDragging = false;
				}
				dragAcc = Vector3.Lerp(dragAcc, Vector3.zero, Time.deltaTime * dragSpeed);
			}
			transform.Rotate(new Vector3(0,0,dragAcc.x));
		}
		
	}
	public void SetRotate(bool? active = null, Vector3? speed = null)
	{
		isRotating = active ?? !isRotating;
		rotationSpeed = speed ?? Vector3.zero;
	}

}
