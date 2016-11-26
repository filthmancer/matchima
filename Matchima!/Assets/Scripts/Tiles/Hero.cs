using UnityEngine;
using System.Collections;

public class Hero : Tile {

	public Class _Class;

	public override string GenusName
	{
		get{return "Default";}
	}

	public override StCon _Name {
		get{
			string valpref = Stats.Value > 1 ? "+" + Stats.Value : "";
			return new StCon(valpref + " " + _Class.Name, GameData.Colour(Genus), true, 60);}
	}

	protected override TileUpgrade [] BaseUpgrades
	{
		get
		{
			return new TileUpgrade []
			{
				new TileUpgrade(1.0F, 5, () => {InitStats._Hits.Max += 1;}),
				new TileUpgrade(1.0F, 5, () => {InitStats.Attack += 1;}),
				new TileUpgrade(0.4F, 1, () => {InitStats.Value += 1;}),
				new TileUpgrade(0.1F, 2, () => {InitStats.Resource +=1;})
			};
		}
	}

	public void SetClass(Class c)
	{
		_Class = c;
		_Class.GetTile(this);

		_Class.Reset();

		InitStats._Hits.Set(_Class.Stats._Health);
		//InitStats.Hits = _Class.Stats._Health;
		InitStats._Attack.Set(_Class.Stats._Attack);
		InitStats._Spell.Set(_Class.Stats._Spell);
		InitStats._Movement.Set(_Class.Stats._Move);

		InitStats._Strength.Add(_Class.Stats._Strength);
		InitStats._Dexterity.Add(_Class.Stats._Dexterity);
		InitStats._Charisma.Add(_Class.Stats._Charisma);
		InitStats._Wisdom.Add(_Class.Stats._Wisdom);


		CheckStats();

		Stats._Team = Team.Ally;

		Stats.Hits = Stats._Hits.Max;
		SetSprite();
	}
	public override bool CanBeAttacked() {return !isKilled && Genus != GENUS.OMG;}
	public bool CastSpell()
	{
		//if(_Class.MeterLvl > 0)
		//{
			StartCoroutine(_Class.UseManaPower());
			return true;
		//}
		//return false;
	}

	public override void SetSprite()
	{
		if(_Class == null) return;

		if(Params._border != null) 
		{
			Params._border.gameObject.SetActive(true);
			Params._border.SetSprite(TileMaster.Genus.Frames, Info.Outer);
		}

		string render = "Default";
		if(Params._render != null && _Class.Atlas != null) 
		{
			tk2dSpriteDefinition id = _Class.Atlas.GetSpriteDefinition(render);
			if(id == null) render = "Default";
			Params._render.SetSprite(_Class.Atlas, render);
			Params._render.scale = new Vector3(0.2F, 0.2F, 1.0F);
		}
	}

	public override string InnerRender(){return "Default";}
	public override tk2dSpriteCollectionData InnerAtlas(){return _Class.Atlas;}

	public override void GetParams(params string [] args)
	{
		SetClass(GameData.instance.GetClass(args[0]));
	}


	public override void AddMana(GENUS g, int m)
	{
		if(_Class != null)
		{
			_Class.AddToMeter(m);
			AddToStat(g, m);
		}
	}

	public void AddToStat(GENUS g, int res)
	{
		int lvl = InitStats[(int)g].Level(res);
		CheckStats();
		if(lvl > 0) 
		{
			MiniAlertUI a = UIManager.instance.MiniAlert(transform.position, GameData.Stat(g) + "+" + lvl, 40, GameData.Colour(g), 0.85F,0.04F);
		}
	}

	public override IEnumerator TakeTurnDamage()
	{
		CheckStats();
		int fullhit = 0;

		fullhit += Stats.TurnDamage;

		InitStats.TurnDamage = 0;
		InitStats._Hits.Current -= fullhit;
		CheckStats();

		yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		Player.instance.OnTileMatch(this);
		if(Stats._Hits.Current <= 0)
		{
			StartDeathTimer();
			PlayAudio("death");
		}
	}

	public override IEnumerator AfterTurnRoutine()
	{
		CheckDeathTimer();
		if(isKilled) yield break;
		yield return StartCoroutine(base.AfterTurnRoutine());
	}


	GENUS Death_PreGenus;
	int Death_Time = 0;
	bool isKilled = false;

	public void StartDeathTimer()
	{
		Death_Time = 3;
		isKilled = true;
		Death_PreGenus = Genus;
		ChangeGenus(GENUS.OMG);
	}

	public void CheckDeathTimer()
	{
		if(Death_Time > 0) Death_Time--;
		if(Death_Time <= 0 && isKilled)
		{
			isKilled = false;
			ChangeGenus(Death_PreGenus);
		}
	}
}
