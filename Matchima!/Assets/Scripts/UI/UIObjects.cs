﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIObjects : MonoBehaviour {

	public GameObject CheatsUI, MenuUI, MainUI;
	public GameObject ItemUI;
	public RectTransform AlertBubble;

	public UIObj ClassUI, WaveUI;
	public UIGear TopGear, BotGear, MiddleGear;
	public UIObj ClassQuote, WaveQuote;
	public UIObj Alert_Button, Alert_ButtonParent;
	public TextMeshProUGUI LevelInfo, Alert;

	public UIObj ArmourParent;

	public GameObject Options;
	public UIScoreWindowNew ScoreWindowB;
	public MiniAlertUI MiniAlert;

	public UIObj [] WaveSlots;

	public UISlotButton SlotObj;

	private bool IsWarning = false;
	private float warningtime = 0.0F;
	private Vector3 initpos;
	// Use this for initialization
	void Start () {
		warningtime = 0.0F;
	}
	
	// Update is called once per frame
	void Update () {
		IsWarning = (warningtime > 0.0F);
		if(IsWarning)
		{
			warningtime -= Time.deltaTime;
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
	}
}