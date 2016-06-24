using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class UIScoreWindowNew : MonoBehaviour {
	
	public Sprite SpriteGreat, SpriteAmazing, SpriteImpossible;
	//public Text Score;
	//public Text Bonus;
	public Image BonusParent, Bonus;
	public GameObject BlurbBonus;
	public Image Back;
	Vector3 finalpos;

	public UIBonus [] SetBonusObj;
	public List<UIBonus> UIBonuses;
	public UIBonus UIBonusObj;

	public bool [] BonusComplete = new bool[6]{true, true, true, true, true, true};

	public void Update () {

	}

	public void Reset()
	{
		for(int i = 0; i < SetBonusObj.Length; i++)
		{
			SetBonusObj[i].Reset();
			//SetBonusObj[i].gameObject.SetActive(false);
		}
		//SetBonusObj[4].gameObject.SetActive(false);
		//SetBonusObj[5].gameObject.SetActive(false);
		BlurbBonus.SetActive(false);
		BonusComplete = new bool[6]{true, true, true, true, true, true};
		return;

		
		for(int i = 0; i < UIBonuses.Count; i++){
			Destroy(UIBonuses[i].gameObject);
		}
		UIBonuses.Clear();
		
		
	}

	public IEnumerator AddScore(GENUS Genus, Class c, int points, int health, int armour, Bonus [] bonus = null)
	{
		if(points == 0) yield break;
		if(Genus == GENUS.NONE || Genus == GENUS.OMG || Genus == GENUS.ALL || Genus == GENUS.PRP) yield break;
		Color col = GameData.instance.GetGENUSColour(Genus);
		UIBonus Score = SetBonusObj[(int) Genus];
		BonusComplete[(int) Genus] = false;
		Score.gameObject.SetActive(true);
		UIManager.ClassButtons.GetClass((int)Genus)._SpriteMask.enabled = true;
		yield return StartCoroutine(Score.Setup(points, col, c));

		yield return StartCoroutine(Score.SetupBonus(bonus));
		
		Sprite tile = TileMaster.Genus.Frame[(int)Genus];
		RectTransform trans = UIManager.ClassButtons[(int)Genus].transform as RectTransform;//UIManager.instance.Wheel.GetResourceRect(Genus);

		if(trans != null)
		{
			//for(int i = 0; i < Score.pts; i++)
			//{
				//if(i % 2 == 0) continue;
				//TileMaster.instance.CreateMiniTile(transform.position, trans, tile, 0.2F, 0.0F);
//				
			//}
			//yield return new WaitForSeconds(Time.deltaTime * 5);
			
			int overflow = Player.instance.CompleteClasses();//Player.Stats.Complete();
			if(Score.pts > 50)
			{
				int type = Score.pts >= 1000 ? 3 : (Score.pts >= 250 ? 2 : 1);
				BlurbBonus.SetActive(true);
				Image blurb_img = BlurbBonus.transform.GetChild(0).GetComponent<Image>();
				if(type == 3)	blurb_img.sprite = SpriteImpossible;
				else if(type == 2) blurb_img.sprite = SpriteAmazing;
				else blurb_img.sprite = SpriteGreat;
			}
			//yield return new WaitForSeconds(Time.deltaTime * 5);
	
			//if(overflow != 0) UIManager.instance.MiniAlert(UIManager.instance.Hunds.transform.position + Vector3.up * 0.7F, "	Overflow: " + overflow, 30, bonus[0].col, 0.6F, 0.02F);
			Score.pts += overflow;
		}
		else trans = Score.GetComponent<RectTransform>();


		BonusComplete[(int) Genus] = true;
		UIManager.ClassButtons.GetClass((int)Genus)._SpriteMask.enabled = false;
		Score.Reset();
		yield break;
		
		/*int all = Score.pts;
		int d = 10;

		int ones = all % d - all % (d/10);
		
		all -= ones;
		d *= 10;
		for(int i = 0; i < ones/5; i ++)
		{
			if(i % 2 == 0) continue;
			TileMaster.instance.CreateMiniTile(trans.position, UIManager.instance.Tens.transform, tile, 0.2F, 0.0F);
			
		}

		int tens = all % d - all % (d/10);
		
		all -= tens;
		d *= 10;
		for(int i = 0; i < tens/5; i ++)
		{
			if(i % 2 == 0) continue;
			TileMaster.instance.CreateMiniTile(trans.position, UIManager.instance.Hunds.transform, tile, 0.2F, 0.0F);
			
		}

		int hunds = all % d - all % (d/10);
		
		all -= hunds;
		d *= 10;
		for(int i = 0; i < hunds/5; i +=10)
		{
			if(i % 2 == 0) continue;
			TileMaster.instance.CreateMiniTile(trans.position, UIManager.instance.Thous.transform, tile, 0.2F, 0.0F);
			
		}
		
		//GameManager.instance.AddTokens(Score.pts);
		
		yield return null;*/
	}

	/*void AddListener(EventTrigger b, int num)
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

	public void Activate(int n)
	{
		if(bonus_scr.Length == 0) return;

		UIManager.instance.ShowSimpleTooltip(true, bonus_obj[n].transform, bonus_scr[n].Name, bonus_scr[n].Description);
	}

	public void Deactivate()
	{
		UIManager.instance.ShowTooltip(false);
	}*/
}