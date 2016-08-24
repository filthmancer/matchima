using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System;
using System.Collections.Generic;

public class UIObj : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,IPointerEnterHandler, IPointerExitHandler{
	public string _Name;
	[HideInInspector]
	public int Index = 100;
	[HideInInspector]
	public UIObj ParentObj;
	public Image [] Img;
	public TextMeshProUGUI [] Txt;
	public UIObj [] Child;

	public bool SetInactiveAfterLoading;
	public TextMeshProUGUI _Text
	{get{
			if(Txt.Length > 0) return Txt[0];
			else return null;
	}}
	public Image _Image
	{get{
			if(Img.Length > 0) return Img[0];
			else return null;
	}}

	protected ObjectPoolerReference poolref;
	public ObjectPoolerReference GetPoolRef(){return poolref;}

	public bool isActive{get{return this.gameObject.activeSelf;}}

	public virtual void Start()
	{
		if(_Name == string.Empty) _Name = gameObject.name;
		else gameObject.name = _Name;
		
		if(Img.Length == 0 && GetComponent<Image>()) Img = new Image[]{GetComponent<Image>()};
		if(Txt.Length == 0 && GetComponent<TextMeshProUGUI>()) Txt = new TextMeshProUGUI[]{GetComponent<TextMeshProUGUI>()};
		for(int i = 0; i < Child.Length; i++)
		{
			Child[i].Index = i;
			Child[i].ParentObj = this;
		}

		if(Img.Length > 0) init = Img[0].color;

		if(SetInactiveAfterLoading) SetActive(false);
	}


	public virtual void SetActive(bool? active = null)
	{
		bool actual = active ?? !this.gameObject.activeSelf;
		this.gameObject.SetActive(actual);
	}

	public UIObj this[int i]
	{
		get
		{
			foreach(UIObj obj in Child)
			{
				if(obj && obj.Index == i) return obj;
			}
			if(Child.Length > i) return Child[i];
			return null;
		}
	}

	public UIObj this[string s]
	{
		get
		{
			foreach(UIObj child in Child)
			{
				if(child == null || child._Name == string.Empty) continue;
				if(child._Name == s) return child;
				if(child[s] != null) return child[s];
			}
			return null;
		}
	}

	public int Length
	{
		get{return Child.Length;}
	}

	public UIObj GetIndex(int i)
	{
		foreach(UIObj child in Child)
		{
			if(child.Index == i) return child;
		}
		return null;
	}

	public UIObj GetChild(int i)
	{
		if(Child.Length-1 < i) return null;
		return Child[i];
	}

	public void AddChild(params UIObj [] c)
	{
		UIObj [] newchild = new UIObj[Child.Length+c.Length];
		for(int i = 0; i < Child.Length; i++)
		{
			newchild[i] = Child[i];
		}
		int x = 0;
		for(int i = Child.Length; i < Child.Length + c.Length; i++)
		{
			newchild[i] = c[x];
			newchild[i].Index = i;
			newchild[i].ParentObj = this;
			x++;
		}
		Child = newchild;
	}

	public void DestroyChildren()
	{
		for(int i = 0; i < Child.Length; i++)
		{
			if(Child[i].GetPoolRef()) Child[i].GetPoolRef().Unspawn();
			else Destroy(Child[i].gameObject);
		}
		Child = new UIObj[0];
	}

	public void BooleanObjColor(bool good)
	{
		if(Img.Length == 0) return;
		Color col = good ? GameData.instance.GoodColour : GameData.instance.BadColour;
		Img[0].color = col;
		init = col;
	}

	public virtual void LateUpdate()
	{
		if(isPressed)
		{
			time_over += Time.deltaTime;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		foreach(Action child in Actions_MouseOver)
		{
			child();
		}
		foreach(UIAction_Method child in TypeActions_MouseOver)
		{
			child.Act();
		}
		if(Img.Length > 0) init = Img[0].color;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		foreach(Action child in Actions_MouseOut)
		{
			child();
		}
		foreach(UIAction_Method child in TypeActions_MouseOut)
		{
			child.Act();
		}
		isPressed = false;
		time_over = 0.0F;
		if(Img.Length > 0) Img[0].color = init;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		//if(Application.isMobilePlatform) return;
		if(UIManager.instance.LogUIObjs) print(Actions_MouseDown.Count + ":" +  this);
		foreach(Action child in Actions_MouseDown)
		{
			child();
		}
		foreach(UIAction_Method child in TypeActions_MouseDown)
		{
			child.Act();
		}
		isPressed = true;
		time_over += Time.deltaTime;
		if(Img.Length > 0) 
		{
			init = Img[0].color;
			Img[0].color = Color.Lerp(init, Color.black, 0.2F);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		foreach(UIAction_Method child in TypeActions_MouseUp)
		{
			child.Act();
		}
		//if(Application.isMobilePlatform) return;
		foreach(Action child in Actions_MouseUp)
		{
			child();
		}
		isPressed = false;
		time_over = 0.0F;
		if(Img.Length > 0) Img[0].color = init;	
	}
	List<Action>	Actions_MouseOut = new List<Action>(), 
					Actions_MouseOver = new List<Action>(),
					Actions_MouseUp = new List<Action>(),
					Actions_MouseDown = new List<Action>(),
					Actions_MouseClick = new List<Action>();
	List<UIAction_Method>	TypeActions_MouseOut = new List<UIAction_Method>(), 
						TypeActions_MouseOver = new List<UIAction_Method>(),
						TypeActions_MouseUp = new List<UIAction_Method>(),
						TypeActions_MouseDown = new List<UIAction_Method>(),
						TypeActions_MouseClick = new List<UIAction_Method>();

	protected Color init;
	public void SetInitCol(Color c) {init = c;}
	protected float time_over = 0.0F;
	public bool isPressed;

	public void AddAction(UIAction a, Action func)
	{
		switch(a)
		{
			case UIAction.MouseOut:
			Actions_MouseOut.Add(func);
			break;
			case UIAction.MouseOver:
			Actions_MouseOver.Add(func);
			break;
			case UIAction.MouseUp:
			Actions_MouseUp.Add(func);

			break;
			case UIAction.MouseDown:
			Actions_MouseDown.Add(func);
			break;
		}
	}

	public void AddAction(UIAction a, Action<string[]> func, params string [] t)
	{

		switch(a)
		{
			case UIAction.MouseOut:
			TypeActions_MouseOut.Add(new UIAction_Method(func, t));
			break;
			case UIAction.MouseOver:
			TypeActions_MouseOver.Add(new UIAction_Method(func, t));
			break;
			case UIAction.MouseUp:
			TypeActions_MouseUp.Add(new UIAction_Method(func, t));
			break;
			case UIAction.MouseDown:
			TypeActions_MouseDown.Add(new UIAction_Method(func, t));
			break;
		}
	}

	public virtual void ClearActions(UIAction a = UIAction.None)
	{
		if(a == UIAction.None)
		{
			Actions_MouseOut.Clear();
			Actions_MouseOver.Clear();
			Actions_MouseUp.Clear();
			Actions_MouseDown.Clear();
			TypeActions_MouseOut.Clear();
			TypeActions_MouseOver.Clear();
			TypeActions_MouseUp.Clear();
			TypeActions_MouseDown.Clear();
		}
		switch(a)
		{
			case UIAction.MouseOut:
			Actions_MouseOut.Clear();
			TypeActions_MouseOut.Clear();
			break;
			case UIAction.MouseOver:
			Actions_MouseOver.Clear();
			TypeActions_MouseOver.Clear();
			break;
			case UIAction.MouseUp:
			Actions_MouseUp.Clear();
			TypeActions_MouseUp.Clear();
			break;
			case UIAction.MouseDown:
			Actions_MouseDown.Clear();
			TypeActions_MouseDown.Clear();
			break;
			case UIAction.MouseClick:
			Actions_MouseClick.Clear();
			TypeActions_MouseClick.Clear();
			break;
		}
	}
}

public enum UIAction
{
	MouseOut,
	MouseOver,
	MouseUp,
	MouseDown,
	MouseClick,
	None
}

public class UIAction_Method
{
	public string [] Values;
	public Action<string[]> Method;
	public void Act()
	{
		Method(Values);
	}

	public UIAction_Method(Action<string[]> m, params string [] v)
	{
		Values = v;
		Method = m;
	}
}