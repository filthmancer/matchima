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

	public void SetClass(Class c)
	{
		_Class = c;
		_Class.GetTile(this);

		_Class.Reset();

		InitStats.HitsMax = _Class.Stats._Health;
		InitStats.Hits = _Class.Stats._Health;
		InitStats.Attack = _Class.Stats._Attack;
		InitStats.Spell = _Class.Stats._Spell;
		InitStats.Movement = _Class.Stats._Move;

		CheckStats();

		Stats._Team = Team.Ally;

		Stats.Hits = Stats.HitsMax;
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
