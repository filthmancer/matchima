using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIShopTooltip : MonoBehaviour {

	public TextMeshProUGUI Old, New, Simple;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CompareItems(Item a, Item b)
	{
		Simple.text = "";
		Old.text = a.name + "\n\n";
		New.text = b.name + "\n\n";
		for(int i = 0; i < a.desc.Count; i++)
		{
			Old.text += a.desc[i] + "\n";
		}

		for(int i = 0; i < b.desc.Count; i++)
		{
			New.text += b.desc[i] + "\n";
		}
	}

	public void ShowSimple(string s)
	{
		Simple.text = s;
		Old.text = "";
		New.text = "";
	}
}
