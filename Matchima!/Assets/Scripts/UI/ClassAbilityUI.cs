using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassAbilityUI : UIObj {

	public UISlotButton [] Slots;
	public EasyTween tweener;

	bool open = false;
	public Transform OpenTrans, ClosedTrans;

	// Update is called once per frame
	public void Update () {
		
		if(open)
		{
			//transform.position = Vector3.Lerp(transform.position, OpenTrans.position, Time.deltaTime * 15);
		}
		else 
		{
			//transform.position = Vector3.Lerp(transform.position, ClosedTrans.position, Time.deltaTime * 15);
			//if(Vector3.Distance(transform.position, ClosedTrans.position) < 0.2F) this.gameObject.SetActive(false);
		}
		
	}

	public void Setup(Class c)
	{
		float newx = UIManager.CrewButtons[c.Index].transform.position.x;
		//newx = Mathf.Clamp(newx, -1.3F, 1.3F);

		//ClosedTrans.position = new Vector3(newx, ClosedTrans.position.y, ClosedTrans.position.z);
		//OpenTrans.position = new Vector3(newx, OpenTrans.position.y, OpenTrans.position.z);
		transform.position = new Vector3(newx, transform.position.y, transform.position.z);

		tweener.OpenCloseObjectAnimation();

		for(int i = 0; i < Slots.Length; i++)
		{
			Slots[i].Setup(c._Slots[i]);
			Slots[i].Parent = c;
			Slots[i].Index = i;
		}
	}

	public override void SetActive(bool? active)
	{
		bool actual = active ?? !this.gameObject.activeSelf;
		if(actual != tweener.IsObjectOpened())
		{
		 tweener.OpenCloseObjectAnimation();
		}
		open = actual;
		if(open) this.gameObject.SetActive(true);
	}
}
