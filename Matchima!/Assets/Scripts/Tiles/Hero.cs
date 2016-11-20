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
			string effectpref = "";
			for(int i = 0; i < Effects.Count; i++)
			{
				if(Effects[i].Duration == -1) effectpref += " " + Effects[i].Description[0].Value;
			}
			return new StCon(valpref + effectpref + " " + _Class.Name, GameData.Colour(Genus), true, 60);}
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
			_Class.AddToStat(g, m);
		}
	}
}
