using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIObjects : MonoBehaviour {

	public GameObject CheatsUI, MenuUI, MainUI;
	public GameObject ClassUpgradeUI, ItemUI;
	public RectTransform AlertBubble;

	public UIObj ClassQuote, WaveQuote;
	public UIObj Alert_Button, Alert_ButtonParent;
	public TextMeshProUGUI LevelInfo, Alert;

	public GameObject AffordText;
	public UIObj ArmourParent;

	public GameObject Options;
	public UIScoreWindowNew ScoreWindowB;
	public MiniAlertUI MiniAlert;

	public UIObj BigUI;
	public UILevelUp LevelUpMenu;

	public UIObj [] WaveSlots;

	public UISlotButton SlotObj;

	public Animator PlayerAnim, WaveAnim;

	private bool IsWarning = false;
	private float warningtime = 0.0F;
	private Vector3 initpos;
	// Use this for initialization
	void Start () {
		initpos = AffordText.transform.position;
		warningtime = 0.0F;
	}
	
	// Update is called once per frame
	void Update () {
		IsWarning = (warningtime > 0.0F);
		if(IsWarning)
		{
			warningtime -= Time.deltaTime;
			AffordText.SetActive(true);
			AffordText.transform.position = initpos + new Vector3(Random.Range(-0.02F, 0.02F), Random.Range(-0.02F, 0.02F), 0);
		}
		else
		{
			AffordText.SetActive(false);
			AffordText.transform.position = initpos;
		}
	}

	public void ShowObj(GameObject obj, bool? Active = null)
	{
		if(!Active.HasValue) obj.SetActive(!obj.activeSelf);
		else obj.SetActive((bool) Active);
	}

	public void ShowObj(UIObj obj, bool? Active = null)
	{
		if(!Active.HasValue) obj.SetActive(!obj.gameObject.activeSelf);
		else obj.SetActive((bool) Active);
	}

	public UIScoreWindowNew GetScoreWindow()
	{
		return ScoreWindowB;
	}

	public void StartWarning(string s = null)
	{
		warningtime = 1.0F;
		if(s != null) AffordText.GetComponent<TextMeshProUGUI>().text = s;
		else AffordText.GetComponent<TextMeshProUGUI>().text  = "CAN'T AFFORD!";
	}
}
