using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class UIBonus : MonoBehaviour {
	public Image BonusParent;
	public TextMeshProUGUI Amount;
	public Bonus [] _Bonus;
	public Image BonusObj;

	Image [] bonus_obj = new Image[0];
	Bonus [] bonus_scr = new Bonus[0];
	public int pts = 0;

	Class _Class;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Reset()
	{
		Amount.text = "";
		if(bonus_obj == null || bonus_obj.Length == 0) return;
		for(int i = 0; i < bonus_obj.Length; i++)
		{
			Destroy(bonus_obj[i].gameObject);
		}
		bonus_obj = null;
		bonus_scr = null;

	}

	public IEnumerator Setup(int points, Color c, Class _class)
	{
		Amount.color = c;
		_Class = _class;
		pts = 0;
		bool isShowing = true;
		int rate = Mathf.Clamp(points / 15, 1, 100000);
		
		while(isShowing)
		{
			if(Amount.transform.localScale.x > 1.0F) Amount.transform.localScale -= Vector3.one * 0.1F;
			pts += rate;
			if(points - pts < rate)
			{
				pts = points;
				isShowing = false;
			}
			Amount.text = "" + pts;
			yield return null;
		}
	}

	public IEnumerator SetupBonus(Bonus [] bonus)
	{
		BonusParent.gameObject.SetActive(true);
		bonus_obj = new Image[bonus.Length];
		bonus_scr = bonus;
		for(int i = 0; i < bonus.Length; i++)
		{
			if(bonus[i] == null) continue;
			bonus_obj[i] = (Image) Instantiate(BonusObj);
			bonus_obj[i].transform.SetParent(BonusParent.transform);

			bonus_obj[i].transform.position = Vector3.zero;
			bonus_obj[i].transform.localScale = Vector3.one * 1.6F;
			bonus_obj[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
						"x " + bonus[i].Multiplier.ToString("0.0") + "\n" + bonus[i].Name;
			bonus_obj[i].color = bonus[i].col;
			
			EventTrigger bon = bonus_obj[i].GetComponent<EventTrigger>();
			AddBonusListeners(bon, i);

			int pts_prev = pts;
			pts = (int) (pts * bonus[i].Multiplier);
			Amount.text = "" + pts;
			_Class.AddToMeter(pts-pts_prev);
			bool isMulting = true;
			while(isMulting)
			{
				bonus_obj[i].transform.localScale -= Vector3.one * 0.045F;

				if(bonus_obj[i].transform.localScale.x <= 1.0F)
				{
					isMulting = false;
				}
				yield return null;
			}
			yield return new WaitForSeconds(Time.deltaTime * 10);
			
			bonus_obj[i].gameObject.SetActive(false);
		}

		yield return new WaitForSeconds(Time.deltaTime * 10);

		BonusParent.gameObject.SetActive(false);
		for(int i = 0; i < bonus_obj.Length; i++) {bonus_obj[i].gameObject.SetActive(true);}
		EventTrigger temp = this.GetComponent<EventTrigger>();
		//AddParentListener(temp);
		//yield break;
	}

	void AddBonusListeners(EventTrigger b, int num)
	{
		EventTrigger.Entry act1 = new EventTrigger.Entry();
		act1.eventID = EventTriggerType.PointerDown;
		act1.callback.AddListener((eventData) => {this.Activate(num);});
		b.triggers.Add(act1);

		EventTrigger.Entry act2 = new EventTrigger.Entry();
		act2.eventID = EventTriggerType.PointerUp;
		act2.callback.AddListener((eventData) => {this.Deactivate();});
		b.triggers.Add(act2);

		EventTrigger.Entry act3 = new EventTrigger.Entry();
		act3.eventID = EventTriggerType.PointerExit;
		act3.callback.AddListener((eventData) => {this.Deactivate();});
		b.triggers.Add(act3);
	}

	void AddParentListener(EventTrigger e)
	{
		EventTrigger.Entry act1 = new EventTrigger.Entry();
		act1.eventID = EventTriggerType.PointerDown;
		act1.callback.AddListener((eventData) => {this.ShowBonuses(true);});
		e.triggers.Add(act1);

		EventTrigger.Entry act2 = new EventTrigger.Entry();
		act2.eventID = EventTriggerType.PointerUp;
		act2.callback.AddListener((eventData) => {this.ShowBonuses(false);});
		e.triggers.Add(act2);

		/*EventTrigger.Entry act3 = new EventTrigger.Entry();
		act3.eventID = EventTriggerType.PointerExit;
		act3.callback.AddListener((eventData) => {this.ShowBonuses();});
		e.triggers.Add(act3);*/
	}

	public void Activate(int n)
	{
		if(bonus_scr.Length == 0) return;

		//UIManager.instance.ShowSimpleTooltip(true, bonus_obj[n].transform, bonus_scr[n].Name, bonus_scr[n].Description);
	}

	public void Deactivate()
	{
		//UIManager.instance.ShowTooltip(false);
	}

	public void ShowBonuses(bool enable)
	{
		BonusParent.gameObject.SetActive(enable);
	}
}
