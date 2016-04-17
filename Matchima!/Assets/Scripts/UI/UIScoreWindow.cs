using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIScoreWindow : MonoBehaviour {
	public TextMeshProUGUI Resource;
	//public Text Score;
	//public Text Bonus;
	public Image BonusParent, Bonus;
	public GameObject BlurbBonus;
	public Image Back;
	Vector3 finalpos;
	// Use this for initialization
	public virtual void Start () {
		//Back = this.GetComponent<Image>();
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//Vector2 pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Canvas.transform as RectTransform, Input.mousePosition, UIManager.Canvas.worldCamera, out pos);
		//Vector2 corner = new Vector2(
		//    0.5F,//((Input.mousePosition.x > (Screen.width / 2f)) ? 1f : 0f),
		//    1f//((Input.mousePosition.y > (Screen.height / 2f)) ? 1f : 0f)
		//);     
		//(this.transform as RectTransform).pivot = corner;
		//transform.position = UIManager.Canvas.transform.TransformPoint(pos) + new Vector3(0,0,-5);
		//Vector2 offset = Vector2.zero;
       	//offset.x += ((Input.mousePosition.x > (Screen.width / 2f)) ? -80f: 80F);
       	//offset.y += ((Input.mousePosition.y > (Screen.height / 2f)) ? -80f : 80f);
		//pos += offset;

		//this.GetComponent<RectTransform>().anchoredPosition3D = mpos;
        transform.position = Vector3.Lerp(transform.position, finalpos, Time.deltaTime);
	}


	public virtual IEnumerator ShowScore(int points, Bonus [] bonus = null)
	{
		if(points == 0) yield break;
		int pts = 0;
		int rate = Mathf.Clamp(points / 15, 1, 100000);
		bool isShowing = true;
		Back.enabled = true;
		Resource.text = "" + pts;
		Resource.transform.localScale = Vector3.one * 1.6F;
		Resource.color = (bonus.Length == 0) ? Color.white : bonus[0].col;

		Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Canvas.transform as RectTransform, Input.mousePosition, UIManager.Canvas.worldCamera, out pos);

       	// Determine which corner of the screen is closest to the mouse position
		Vector2 corner = new Vector2(
		    ((Input.mousePosition.x > (Screen.width / 2f)) ? 1f : 0f),
		    1f//((Input.mousePosition.y > (Screen.height / 2f)) ? 1f : 0f)
		);
		         
		(this.transform as RectTransform).pivot = corner;
		transform.position = UIManager.Canvas.transform.TransformPoint(pos) + new Vector3(0,0,-5);

		finalpos = transform.position + Vector3.up * 2F;
		while(isShowing)
		{
			if(Resource.transform.localScale.x > 1.0F) Resource.transform.localScale -= Vector3.one * 0.1F;

			pts += rate;
			if(points - pts < rate)
			{
				pts = points;
				isShowing = false;
			}
			Resource.text = "" + pts;

			yield return null;
		}

		yield return new WaitForSeconds(0.2F);

		Image [] bonus_obj = new Image[bonus.Length];
		for(int i = 0; i < bonus.Length; i++)
		{
			if(bonus[i] == null) continue;
			bonus_obj[i] = (Image) Instantiate(Bonus);
			bonus_obj[i].transform.parent = BonusParent.transform;
			bonus_obj[i].transform.position = Vector3.zero;
			bonus_obj[i].transform.localScale = Vector3.one * 1.6F;
			bonus_obj[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
						"x " + bonus[i].Multiplier.ToString("0.0") + "\n" + bonus[i].Name;
			bonus_obj[i].color = bonus[i].col;

			bool isMulting = true;
			while(isMulting)
			{
				bonus_obj[i].transform.localScale -= Vector3.one * 0.1F;
				if(bonus_obj[i].transform.localScale.x <= 1.0F)
				{
					isMulting = false;
					pts = (int) (pts * bonus[i].Multiplier);
					Resource.text = "" + pts;
				}
				yield return null;
			}
			yield return new WaitForSeconds(0.1F);
		}

		yield return new WaitForSeconds(0.35F);
		Resource.text = "";
		Back.enabled = false;

		for(int i = 0; i < bonus_obj.Length; i++)
		{
			Destroy(bonus_obj[i].gameObject);
		}
		yield return null;
	}
}


