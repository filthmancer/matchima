using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIObj : MonoBehaviour {
	public TextMeshProUGUI _Text;
	public Image _Image;
	public Image [] Img;
	public TextMeshProUGUI [] Txt;
	public int Index = 100;

	public virtual void SetActive(bool? active)
	{
		bool actual = active ?? !this.gameObject.activeSelf;
		this.gameObject.SetActive(actual);
	}

	public UIObj this[int i]
	{
		get
		{
			foreach(Transform child in transform)
			{
				UIObj obj = child.GetComponent<UIObj>();
				if(obj && obj.Index == i) return obj;
			}
			return null;
		}
	}
}
