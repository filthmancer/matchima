﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DragType
{
	None,
	Hold,
	Cast
}
public class Slot : MonoBehaviour {
	public string Name_Basic;
	public string Description_Basic;
	public Rarity Rarity = Rarity.Common;
	
	public Class Parent;
	public Sprite Icon;
	public Color Colour;
	public bool initialized = false;
	public bool activated;
	[HideInInspector]
	public bool ManaPowerMod;

	public DragType Drag;
	public string IconString;

	public bool BeforeMatchEffect = false, AfterMatchEffect = false;
	public bool BeforeTurnEffect = false, AfterTurnEffectA = false, AfterTurnEffectB = false;

	public virtual bool IsDraggable
	{
		get{
			return Drag != DragType.None;
		}
	}
	public int Index;

	public int cooldown, cooldown_time;

	public virtual StCon Name
	{
		get{
			return new StCon(Name_Basic, Colour);
		}
	}

	public virtual StCon [] Description_Tooltip
	{
		get{
			return null;
		}
	}
	
	public virtual StCon [] BaseDescription
	{
		get{
			List<StCon> All = new List<StCon>();
			All.Add(new StCon(Description_Basic, Color.white));
			return All.ToArray();
		}
	}

	public virtual void DamageIndicator(ref List<int> damage, List<Tile> selected)
	{
		damage = damage;
		//return damage;
	}

	public virtual string Level
	{
		get{return "";}
	}

	public virtual StCon [] Description
	{
		get{return null;}
	}

	public StCon [] FullDescription
	{
		get{
			List<StCon> final = new List<StCon>();
			if(BaseDescription != null)	
			{
				foreach(StCon child in BaseDescription)
				{
					if(child.Value != string.Empty) final.Add(child);
				}
			}
			if(Description != null)	
			{
				foreach(StCon child in Description)
				{
					if(child.Value != string.Empty) final.Add(child);
				}
			}
			return final.ToArray();
		}
	}

	public StCon [] FullTooltip
	{
		get{
			List<StCon> final = new List<StCon>();
			if(Description_Tooltip != null)	
			{
				foreach(StCon child in Description_Tooltip)
				{
					if(child.Value != string.Empty) final.Add(child);
				}
			}
			if(Description != null)	
			{
				foreach(StCon child in Description)
				{
					if(child.Value != string.Empty) final.Add(child);
				}
			}
			return final.ToArray();
		}
	}


	public string FullDescToString
	{
		get{
			string s = "";
			for(int i = 0; i < FullDescription.Length; i++)
			{
				s += FullDescription[i].Value + "\n";
			}
			return s;
		}
	}

	// Use this for initialization
	public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}


	public virtual IEnumerator BeforeMatch(List<Tile> tiles)
	{
		yield break;
	}

	public virtual IEnumerator AfterMatch()
	{
		yield break;
		//return 0.0F;
	}

	public virtual IEnumerator BeforeTurn()
	{
		yield break;
	}


	public virtual IEnumerator AfterTurn ()
	{
		yield break;
	}

	public virtual void Activate()
	{
		
	}

	public virtual void CheckHealth()
	{
		
	}

	public virtual int OnHit(int initial_hit, params Tile[] attackers)
	{
		return initial_hit;
	}

	public virtual void Init(int i, int lvl = 1)
	{
		Setup();
		Index = i;
	}

	protected virtual void Setup()
	{
		initialized = true;
	}

	public virtual void SetArgs(params string [] args)
	{
		
	}


	public virtual float GetCooldownRatio()
	{
		if(cooldown == 0) return 0.0F;
		float f = cooldown_time * 1.0F;
		return f/cooldown;
	}

	public virtual Stat GetStats()
	{
		return null;
	}
}