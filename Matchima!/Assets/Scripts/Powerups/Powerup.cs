using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	public string Name;
	public Class Parent;
	public bool PlayMinigame = true;
	public bool DestroyOnEnd = false;
	public UIObj [] MinigameObj;
	public int LastLevel = 0;
	public virtual IEnumerator Activate(int Level)
	{
		LastLevel = Level;
		if(PlayMinigame) yield return StartCoroutine(Minigame(Level));
		else yield return StartCoroutine(Cast(Level));

		if(DestroyOnEnd) Destroy(this.gameObject);
	}

	protected  virtual IEnumerator Minigame(int Level)
	{
		yield return StartCoroutine(Cast(Level));
	}

	IEnumerator Cast(int Level)
	{
		yield return null;
	}

	protected UIObj CreateMinigameObj(int i)
	{
		UIObj obj = (UIObj)Instantiate(MinigameObj[i]);
		RectTransform rect = obj.GetComponent<RectTransform>();
		obj.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		obj.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;
		return obj;
	}

	public virtual void Setup(Class c)
	{
		Parent = c;
	}
}
