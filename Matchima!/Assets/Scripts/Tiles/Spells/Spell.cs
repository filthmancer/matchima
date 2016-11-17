using UnityEngine;
using System.Collections;
using TMPro;


public class Spell : Tile {

	public override StCon _Name {
		get{return new StCon(SpellEffect.Name_Basic, GameData.Colour(Genus));}
	}

	public override StCon [] Description
	{
		get
		{
			return SpellEffect.Description_Tooltip;
		}
	}

	private Ability SpellEffect;
	public string SpellName;

	public bool SpendUses, SpendCooldown;
	public int Uses = 0;
	public int CooldownTime = 0, CooldownCurrent = 0;
	public TextMeshPro UsesText;
	public SpriteRenderer CooldownSprite, SpellSprite;

	private bool can_be_destroyed = true;

	public override void Setup(GridInfo g, int x, int y, int scale, TileInfo inf, int value_inc = 0)
	{
		base.Setup(g, x,y,scale,inf, value_inc);
		if(SpellName == string.Empty)
		{
			SpellEffect = (Ability) Instantiate(GameData.instance.GetTeamAbility());
			SpellEffect.transform.parent = this.transform;
			if((int)Genus < 4) SpellEffect.Parent = Player.Classes[(int)Genus];
			SpellEffect.Init(0);
		}
		else
		{
			SpellEffect = (Ability) Instantiate(GameData.instance.GetAbilityByName(SpellName));
			SpellEffect.transform.parent = this.transform;
		}
		

		if(SpellEffect.Icon != null) 
		{
			SpellSprite.sprite = SpellEffect.Icon;//SetRender(SpellEffect.Icon);
			SpellSprite.transform.localScale *= 0.8F;
			SpellSprite.color = GameData.Colour(Genus);
		}
	}

	public override void Update()
	{
		base.Update();

		CooldownSprite.enabled = (SpendCooldown && CooldownCurrent > 0);
		if(SpendCooldown && CooldownCurrent > 0)
		{
			CooldownSprite.enabled = true;
			Params.counter.text = "" + CooldownCurrent;
		}
		else CooldownSprite.enabled = false;

		UsesText.text = (SpendUses ? "" + Uses : "");
	}

	public float CooldownRatio()
	{
		float final = (float) CooldownCurrent / (float)CooldownTime;
		return final;
	}

	public override bool Match(int resource)
	{
		if(isMatching) return true;
		isMatching = true;

		CheckStats();
		Stats.Value *=  resource;

		bool activate = true;
		if(SpendUses)
		{
			Uses -= 1;
			if(Uses < 0) 
			{
				activate = false;
			}
		}
		if(SpendCooldown)
		{
			if(CooldownCurrent > 0)
			{
				activate = false;
			}
			else CooldownCurrent = CooldownTime;
		}

		if(activate)
		{
			SpellEffect.Activate();
			return true;
		}
		else if(can_be_destroyed)
		{
			DestroyThyself();
			TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
			return false;
		}

		return false;
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);

		int diff = (int) InitStats.value_soft - InitStats.Value;
		if(diff != 0)
		{
			InitStats.Value = (int) InitStats.value_soft;
			CheckStats();
			SetSprite();
		}
	}
}
