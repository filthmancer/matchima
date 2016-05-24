using UnityEngine;
using System.Collections;

public class UIObjTweener : UIObj {

	public EasyTween [] Tweens;

	public EasyTween Tween{
		get{return Tweens[0];}
	}

	public void SetTween(int i, bool? active = null)
	{
		bool actual = active ?? !Tweens[i].IsObjectOpened();
		if(Tweens[i].IsObjectOpened() != actual) Tweens[i].OpenCloseObjectAnimation();
	}
}
