using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UITooltip : MonoBehaviour {

	public TextMeshProUGUI Title;
	public TextMeshProUGUI SimpleDescription;
	public UIObj Damage;
	public UIObj CooldownObj, CostObj;
	public GameObject Costs;
	
	public GameObject TileObj, TextObj;
	public Image Base, Base2, TitleParent, ContentParent;
	public Transform HorizontalParent;
	private RectTransform rect;
	private Canvas canvas;

	private bool obj_set, show;
	private List<GameObject> allObj = new List<GameObject>();

	private Transform target;
	public void Start()
	{
		rect = GetComponent<RectTransform>();
		canvas = UIManager.Canvas;
	}

	public void SetSimpleTooltip(bool Active, Transform obj, string name, string desc)
	{
		Base.enabled = Active;
		Base2.enabled = Active;
		ContentParent.enabled = false;
		Title.text = Active ? name : "";
		SimpleDescription.text = Active ? desc : ""; 
		if(Active) GetPosition(obj);
	}

	public void SetSimpleTooltip(bool Active, Transform obj, StCon name, StCon [] desc)
	{
		Base.enabled = Active;
		Base2.enabled = Active;
		TitleParent.gameObject.SetActive(Active);
		ContentParent.gameObject.SetActive(Active);
		ContentParent.enabled = false;
		Title.text = Active ? name.Value : "";
		Title.color = name.Colour;
		
		if(!Active || (allObj.Count > 0 && obj != target))
		{
			for(int i = 0; i < allObj.Count; i++)
			{
				Destroy(allObj[i]);
			}
			allObj.Clear();
		}

		if(target == obj.transform) return;
		if(!Active) target = null;
		else target = obj.transform;

		Transform TopParent = ContentParent.transform;
		Transform ParentObj = (Transform) Instantiate(HorizontalParent);
		ParentObj.transform.SetParent(TopParent);
		ParentObj.transform.position = Vector3.zero;
		ParentObj.transform.localScale = Vector3.one;
		allObj.Add(ParentObj.gameObject);

		for(int i = 0; i < desc.Length; i++)
		{
			GameObject new_desc = (GameObject) Instantiate(TextObj);
			new_desc.transform.SetParent(ParentObj);
			new_desc.transform.position = Vector3.zero;
			new_desc.transform.localScale = Vector3.one;
			TextMeshProUGUI d = new_desc.GetComponent<TextMeshProUGUI>();
			d.text = desc[i].Value;
			d.color = desc[i].Colour;

			

			if(desc[i].NewLine && i < desc.Length-1)
			{
				ParentObj =(Transform) Instantiate(HorizontalParent);
				ParentObj.transform.SetParent(TopParent);
				ParentObj.transform.position = Vector3.zero;
				ParentObj.transform.localScale = Vector3.one;
				allObj.Add(ParentObj.gameObject);
			}
			
		}
		if(Active) GetPosition(obj);
	}

	public void SetTooltip(bool Active, UIButton obj)
	{
		if(PlayerControl.HoldingSlot)
		{
			TitleParent.gameObject.SetActive(false);
			ContentParent.gameObject.SetActive(false);
			Base.enabled = false;
			Base2.enabled = false;
			return;
		}
		else
		{
			TitleParent.gameObject.SetActive(Active);
			ContentParent.gameObject.SetActive(Active);
		}
		Base.enabled = Active;
		Base2.enabled = Active;
		Costs.SetActive(Active);
		if(!Active) target = null;

		if(obj != null && Active)
		{
			if(obj.transform == target) return;
			target = obj.transform;

			StCon [] Description = obj.GetTooltip();
			string name = obj.GetName();
			Color col = obj.GetColour();

			if(Active) GetPosition(obj.transform);
			if(!Active || (allObj.Count > 0 && obj != target))
			{
				for(int i = 0; i < allObj.Count; i++)
				{
					Destroy(allObj[i]);
				}
				allObj.Clear();
			}

			if(Active && (Description == null || Description.Length == 0))
			{
				Title.text = name;
				Title.color = col;
				if(name == string.Empty)
				{
					Base.enabled = false;
					Base2.enabled = false;
				}
			}
			else
			{
				Title.text = name;
				Title.color = col;
				//Tiles.SetActive(true);

				Transform TopParent = ContentParent.transform;
				Transform ParentObj = (Transform) Instantiate(HorizontalParent);
				ParentObj.transform.SetParent(TopParent);
				ParentObj.transform.position = Vector3.zero;
				ParentObj.transform.localScale = Vector3.one;
				allObj.Add(ParentObj.gameObject);
				int i = 0;
				foreach(StCon child in Description)
				{
					Sprite [] s = TileMaster.Types.SpriteOf(child.Value);
					if(s.Length != 0) 
					{
						GameObject newtile = (GameObject) Instantiate(TileObj);
						newtile.transform.SetParent(ParentObj.transform);
						newtile.transform.position = Vector3.zero;
						newtile.transform.localScale = Vector3.one;
						if(s.Length > 1)
						{
							newtile.GetComponent<Image>().sprite = s[1];
							newtile.transform.GetChild(0).gameObject.SetActive(true);
							newtile.transform.GetChild(0).GetComponent<Image>().sprite = s[0];
						}
						else 
						{
							newtile.GetComponent<Image>().sprite = s[0];
							newtile.transform.GetChild(0).gameObject.SetActive(false);
						}
						

						if(child.NewLine && i < Description.Length-1)
						{
							ParentObj = (Transform) Instantiate(HorizontalParent);
							ParentObj.transform.SetParent(TopParent);
							ParentObj.transform.position = Vector3.zero;
							ParentObj.transform.localScale = Vector3.one;
							allObj.Add(ParentObj.gameObject);
						}
					}
					else if(child != null)
					{
						GameObject newstring = (GameObject) Instantiate(TextObj);
						newstring.transform.SetParent(ParentObj.transform);
						newstring.transform.position = Vector3.zero;
						newstring.transform.localScale = Vector3.one;
						newstring.GetComponent<TextMeshProUGUI>().text = child.Value;
						newstring.GetComponent<TextMeshProUGUI>().color = child.Colour;
						allObj.Add(newstring);

						if(child.NewLine && i < Description.Length-1)
						{
							ParentObj = (Transform) Instantiate(HorizontalParent);
							ParentObj.transform.SetParent(TopParent);
							ParentObj.transform.position = Vector3.zero;
							ParentObj.transform.localScale = Vector3.one;
							allObj.Add(ParentObj.gameObject);
						}
					}
					i++;
				}	
			}
		
			Slot sloot = null;
			Ability a = null;
			if(obj is UISlotButton)
			{
				sloot = (obj as UISlotButton).slot;
				a = sloot as Ability;
			}
			//if(obj is UISlotButton && (obj as UISlotButton).slot != null && (obj as UISlotButton).slot is Ability)
			if(a != null)
			{
				if(a.cooldown > 0)
				{
					UIObj cd = (UIObj) Instantiate(CooldownObj);
					cd.transform.SetParent(Costs.transform);
					cd.transform.position = Vector3.zero;
					cd.transform.localScale = Vector3.one;
					cd.Txt[0].text = (a.cooldown - a.cooldown_time) + "/" + a.cooldown;
					allObj.Add(cd.gameObject);
				}

				for(int i = 0; i < a.cost.Length; i++)
				{
					if(a.cost[i] > 0)
					{
						UIObj cost = (UIObj) Instantiate(CostObj);
						cost.transform.SetParent(Costs.transform);
						cost.transform.position = Vector3.zero;
						cost.transform.localScale = Vector3.one;
						//cost.GetComponent<Image>().color = GameData.instance.GetGNUSColour(ab.CostType)*0.75F;
						cost.Txt[0].color = GameData.Colour((GENUS)i);
						cost.Txt[0].text = a.cost[i] + "";
						allObj.Add(cost.gameObject);
					}
				}
			}
		}
		else
		{
			TitleParent.gameObject.SetActive(false);
			ContentParent.gameObject.SetActive(false);
			Base.enabled = false;
			Base2.enabled = false;
			Costs.SetActive(false);
		}
		obj_set = Active;
		show = false;
	}


	public void SetTooltip2(bool Active, UIAbilityButton obj, string [] input)
	{
		Base.enabled = Active;
		Base2.enabled = Active;
		ContentParent.enabled = false;

		Slot ab = obj ?  obj.slot : null;
		string name = ab ? ab.name : "";
		string description = ab ? "" : "";

		if(Active) GetPosition(obj.transform);

		SimpleDescription.gameObject.SetActive(false);
		//Tiles.SetActive(false);
		Title.text = "";
		if(!Active)target = null;

		if(!Active || (allObj.Count > 0 && obj != target))
		{
			for(int i = 0; i < allObj.Count; i++)
			{
				Destroy(allObj[i]);
			}
			allObj.Clear();
		}

		Costs.SetActive(Active);
		if(ab == null) return;
		if(obj.transform == target) return;
		target = obj.transform;

		if(Active && (input == null || input.Length == 0))
		{
			Title.text = name;
			Title.color = ab.Colour;
			SimpleDescription.gameObject.SetActive(true);
			SimpleDescription.text = description;
		}
		else
		{
			Title.text = name;
			Title.color = ab.Colour;
			//Tiles.SetActive(true);
			foreach(string child in input)
			{
				Sprite [] s = TileMaster.Types.SpriteOf(child);
				if(s.Length != 0) 
				{
					GameObject newtile = (GameObject) Instantiate(TileObj);
					//newtile.transform.SetParent(Tiles.transform);
					newtile.transform.position = Vector3.zero;
					newtile.transform.localScale = Vector3.one;
					if(s.Length > 1)
					{
						newtile.GetComponent<Image>().sprite = s[1];
						newtile.transform.GetChild(0).gameObject.SetActive(true);
						newtile.transform.GetChild(0).GetComponent<Image>().sprite = s[0];
					}
					else 
					{
						newtile.GetComponent<Image>().sprite = s[0];
						newtile.transform.GetChild(0).gameObject.SetActive(false);
					}
					allObj.Add(newtile);
				}
				else if(child != null)
				{
					GameObject newstring = (GameObject) Instantiate(TextObj);
					//newstring.transform.SetParent(Tiles.transform);
					newstring.transform.position = Vector3.zero;
					newstring.transform.localScale = Vector3.one;
					newstring.GetComponent<TextMeshProUGUI>().text = child;
					allObj.Add(newstring);
				}
			}	
		}
	
		
		if(ab is Ability)
		{
			
		}
		
		
		obj_set = Active;
		show = false;
	}

	public void Update()
	{
		if(obj_set && !show)
		{
			show = true;
			return;
		}
		//Tiles.SetActive(show);
		Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
       	Vector3 dpos = canvas.transform.TransformPoint(pos) + 
       	new Vector3((Input.mousePosition.x > Screen.width*0.8F) ? -1f : (Input.mousePosition.x < Screen.width*0.2F) ? 1F : 0.0F, 1.5F, 0.0F);

       // Damage.transform.position = Vector3.Lerp(Damage.transform.position, dpos, Time.deltaTime * 15);
       	// Determine which corner of the screen is closest to the mouse position
		
       //if(GameManager.instance.isPaused || Player.Stats.isKilled || GameManager.instance.BotTeamTurn || GameManager.instance.WaveAlert || UIManager.InMenu) 
       //{
       //	Damage.Txt[0].text = "";
       //	Damage.SetActive(false);
       //}
       //else if(PlayerControl.instance.AttackMatch)
       //{
       //	Damage.Txt[0].text = PlayerControl.instance.AttackValue + "";
       //	Damage.SetActive(true);
       //}
       //else
       //{
       //	Damage.Txt[0].text = "";
       //	Damage.SetActive(false);

       //}
	}

	public void GetPosition(Transform obj)
	{
		Vector3 guipos = Camera.main.WorldToScreenPoint(obj.position);
   
       	//Vector2 corner = new Vector2(
		//    ((guipos.x > (Screen.width / 2f)) ? 1f : 0f),
		//    ((guipos.y > (Screen.height / 2f)) ? 1f : 0f)
		//);
       	//(this.transform as RectTransform).pivot = corner;

		Vector2 offset = Vector2.zero;
       //offset.x += ((guipos.x > (Screen.width / 2f)) ? -2.0F: 2.0F);
       offset.y += ((guipos.y > (Screen.height / 2f)) ? -Screen.height * 0.15F : Screen.height * 0.15F);

       	Vector3 final = new Vector3(
       		Mathf.Clamp(guipos.x+offset.x, Screen.width*0.45F, Screen.width*0.55F),
       		Mathf.Clamp(guipos.y+offset.y, Screen.height*0.3F, Screen.height * 0.85F),
       		-20
       		);

       	final =  Camera.main.ScreenToWorldPoint(final);
       	final.z = 0;
        transform.position = final;
	}
}
