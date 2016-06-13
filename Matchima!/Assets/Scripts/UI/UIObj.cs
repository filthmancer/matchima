using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System;
using System.Collections.Generic;

public class UIObj : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,IPointerEnterHandler, IPointerExitHandler{
	public string Name;
	[HideInInspector]
	public int Index = 100;
	[HideInInspector]
	public UIObj ParentObj;
	public Image [] Img;
	public TextMeshProUGUI [] Txt;
	public UIObj [] Child;

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

	public void Start()
	{
		if(Name == string.Empty) Name = gameObject.name;
		else gameObject.name = Name;
		
		if(Img.Length == 0 && GetComponent<Image>()) Img = new Image[]{GetComponent<Image>()};
		if(Txt.Length == 0 && GetComponent<TextMeshProUGUI>()) Txt = new TextMeshProUGUI[]{GetComponent<TextMeshProUGUI>()};
		for(int i = 0; i < Child.Length; i++)
		{
			Child[i].Index = i;
			Child[i].ParentObj = this;
		}

		if(Img.Length > 0) init = Img[0].color;
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
			return null;
		}
	}

	public UIObj this[string s]
	{
		get
		{
			foreach(UIObj child in Child)
			{
				if(child.Name == s) return child;
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

	public void BooleanObjColor(bool good)
	{
		Img[0].color = good ? GameData.instance.GoodColour : GameData.instance.BadColour;
	}

	public virtual void LateUpdate()
	{
		if(isPressed)
		{
			time_over += Time.deltaTime;
		}

	}

	void _GenerateEvents()
	{
		//EventTrigger e = this.GetComponent<EventTrigger>();
		//if(!e)	e = this.AddComponent<EventTrigger>();
		//
		//EventTrigger.Entry entry = new EventTrigger.Entry();
		//entry.eventID = EvenTriggerType.PointerClick;
		//entry.callback.AddListener(() => {_MouseClick();});
		//e.triggers.Add(entry);
		//entry = new EventTrigger.Entry();
		//entry.eventID = EvenTriggerType.PointerEnter;
		//entry.callback.AddListener(() => {_MouseOver();});
		//e.triggers.Add(entry);
		//entry = new EventTrigger.Entry();
		//entry.eventID = EvenTriggerType.PointerExit;
		//entry.callback.AddListener(() => {_MouseOut();});
		//e.triggers.Add(entry);
		//entry = new EventTrigger.Entry();
		//entry.eventID = EvenTriggerType.PointerDown;
		//entry.callback.AddListener(() => {_MouseDown();});
		//e.triggers.Add(entry);
		//entry = new EventTrigger.Entry();
		//entry.eventID = EvenTriggerType.PointerUp;
		//entry.callback.AddListener(() => {_MouseUp();});
		//e.triggers.Add(entry);
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
		if(Img.Length > 0) init = Img[0].color;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		foreach(Action child in Actions_MouseOut)
		{
			child();
		}
		isPressed = false;
		time_over = 0.0F;
		if(Img.Length > 0) Img[0].color = init;
	}

	public void OnPointerDown(PointerEventData eventData)
	{

		//if(Application.isMobilePlatform) return;
		foreach(Action child in Actions_MouseDown)
		{
			child();
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
		Img[0].color = init;
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


	protected Color init;
	protected float time_over = 0.0F;
	public bool isPressed;
	public void _MouseOut()
	{
		//if(Application.isMobilePlatform) return;
		foreach(Action child in Actions_MouseOut)
		{
			child();
		}
		if(isPressed) 
		{
			time_over = 0.0F;
			isPressed = false;
		}
		Img[0].color = init;
	}

	public void _MouseOver()
	{
		//if(Application.isMobilePlatform) return;
		//if(!Application.isEditor) isPressed = true;
		foreach(Action child in Actions_MouseOver)
		{
			child();
		}
		//if(!Application.isEditor || isPressed) time_over += Time.deltaTime;
	}

	public void _MouseUp()
	{
		Img[0].color = init;
		//if(Application.isMobilePlatform) return;
		foreach(Action child in Actions_MouseUp)
		{
			child();
		}
		
		isPressed = false;
		time_over = 0.0F;
		init = Img[0].color;
	}

	public void _MouseDown()
	{
		//if(Application.isMobilePlatform) return;
		foreach(Action child in Actions_MouseDown)
		{
			child();
		}
		isPressed = true;
		time_over += Time.deltaTime;
		init = Img[0].color;
		Img[0].color = Color.Lerp(init, Color.black, 0.2F);
	}

	public void _MouseClick()
	{

	}

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

	public void ClearActions(UIAction a = UIAction.None)
	{
		if(a == UIAction.None)
		{
			Actions_MouseOut.Clear();
			Actions_MouseOver.Clear();
			Actions_MouseUp.Clear();
			Actions_MouseDown.Clear();
		}
		switch(a)
		{
			case UIAction.MouseOut:
			Actions_MouseOut.Clear();
			break;
			case UIAction.MouseOver:
			Actions_MouseOver.Clear();
			break;
			case UIAction.MouseUp:
			Actions_MouseUp.Clear();
			break;
			case UIAction.MouseDown:
			Actions_MouseDown.Clear();
			break;
			case UIAction.MouseClick:
			Actions_MouseClick.Clear();
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