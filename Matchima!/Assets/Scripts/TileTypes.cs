﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System;

public enum GENUS 
{
	STR,
	DEX,
	WIS,
	CHA,
	PRP,
	ALL,
	OMG,
	RAND,
	NONE
}


public enum TYPE
{
	Base,
	Spell,
	Enemy,
	Other,
	None
}


[System.Serializable]
public class GenusTypes
{
	public GENUS this[string s]
	{
		get{return GenusOf(s);}
	}

	public int Length {get{ return 7;}}

	public GENUS GenusOf(string name)
	{
		string fin = name.ToLower();
		if(fin == "red" || fin == "str") return GENUS.STR;
		if(fin == "green"  || fin == "wis") return GENUS.WIS;
		if(fin == "blue"  || fin == "dex") return GENUS.DEX;
		if(fin == "yellow"  || fin == "cha") return GENUS.CHA;
		if(fin == "purple") return GENUS.PRP;
		if(fin == "grey" || fin == "omega") return GENUS.OMG;
		if(fin == "allcol" || fin == "all" || fin == "prism" || fin == "alpha") return GENUS.ALL;

		return GENUS.NONE;
	}

	public Sprite [] Frame {
		get{
			return new Sprite [] {RedFrame, BlueFrame,GreenFrame, YellowFrame, PurpleFrame, AllFrame, GreyFrame};
			}
		}

	public Sprite RedFrame, BlueFrame,GreenFrame, YellowFrame, PurpleFrame, AllFrame, GreyFrame;

	public float [] ChancesAdded = new float [] {0.0F,
												 0.0F,
												 0.0F,
												 0.0F,
												 0.0F,
												 0.0F,
												 0.0F};

}

public class TileTypes : MonoBehaviour {

	public static TileTypes none;


	public SPECIES this[string s]
	{
		get
		{
			if(s == string.Empty) return SPECIES.None;
			if(s.ToLower() == "mana") return this["resource"];
			foreach(SPECIES child in Species)
			{
				if(child.Name == s.ToLower()) return child;
			}

			return null;
		}
	}

	public SPECIES this[int i]
	{
		get{return Species[i];}
	}

	public int Length {get{return Species.Count;}}

	public List<SPECIES> Species
	{
		get
		{
			List <SPECIES> final = new List<SPECIES>();
			final.AddRange(Base);
			final.AddRange(Spells);
			final.AddRange(Enemies);
			final.AddRange(Other);
			return final;
		}
	}

	public List<SPECIES> SpeciesNoEnemies
	{
		get
		{
			List <SPECIES> final = new List<SPECIES>();
			final.AddRange(Base);
			final.AddRange(Spells);
			final.AddRange(Other);
			return final;
		}
	}

	public List<SPECIES> Base;
	public List<SPECIES> Spells;
	public List<SPECIES> Enemies;
	public List<SPECIES> Other;


										 
	// Use this for initialization
	void Start () {
	}

	public void Setup()
	{
		int i = 0;
		foreach(SPECIES child in Species)
		{
			child.Index = i;
			child.ResetChances();
			i++;
		}
	}

	string [] SPS = new string [] {"red", "blue", "green", "yellow", "purple", "alpha", "grey"};
	public IEnumerator LoadSprites()
	{
		foreach(SPECIES child in Species)
		{
			for(int i = 0; i < SPS.Length; i++)
			{
				string path = "Inners/" + child.Name + "/" + SPS[i];
				Sprite[] textures =  Resources.LoadAll<Sprite>(path);
				child[i].Sprites = textures;
			}
		}
		yield return null;
	}


	int prefabs_per_spec = 0;
	public IEnumerator LoadPrefabs()
	{
		GameObject poolpar = new GameObject("Pooled Tiles");
		poolpar.transform.SetParent(this.transform);
		foreach(SPECIES child in Species)
		{
			GameObject par = new GameObject(child.Name);
			par.transform.SetParent(poolpar.transform);
			child.TilePool = new ObjectPool(child.Prefab, prefabs_per_spec, par.transform);
			yield return null;
		}

		yield return null;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void GetTypes(TileTypes t)
	{
		if(t == null || t == TileTypes.none) return;

		foreach(SPECIES child in t.Species)
		{
			foreach(SPECIES spec in Species)
			{
				if(child.Name == spec.Name)
				{
					for(int i = 0; i < spec.AllGenus.Length; i++)
					{
						spec[i].ChanceInitial = child[i].ChanceInitial;
					}
				}
			}
		}
	}



	public TileInfo RandomType(GENUS type = GENUS.NONE)
	{
		int index = 0;
		List<float> chances = new List<float>();
		List<int> genus_index = new List<int>();
		List<int> species_index = new List<int>();
		if(type != GENUS.NONE) 
		{
			for(int i = 0; i < Species.Count; i++)
			{
				int a = Species[i].RARITY((int)type);
				if(a == 0) continue;
				chances.Add(1.0F);
				genus_index.Add((int) type);
				species_index.Add(i);
			}
		}
		else
		{
			for(int i = 0; i < Species.Count; i++)
			{
				for(int g = 0; g < Species[i].AllGenus.Length; g++)
				{
					int a = Species[i].RARITY(g);
					if(a == 0) continue;
					chances.Add(1.0F);
					genus_index.Add(g);
					species_index.Add(i);
				}
			}
			
		}

		index = ChanceEngine.Index(chances.ToArray());
		TileInfo final_spec = new TileInfo(Species[species_index[index]], (GENUS) genus_index[index]);
		return final_spec;
	}

	public TileInfo RandomTypeByRarity(GENUS type = GENUS.NONE, bool limit_by_chance = false)
	{
		int index = 0;
		List<float> chances = new List<float>();
		List<int> genus_index = new List<int>();
		List<int> species_index = new List<int>();
		if(type != GENUS.NONE) 
		{
			for(int i = 0; i < Species.Count; i++)
			{
				if(limit_by_chance && Species[i].Chance == 1.0F) continue;
				else
				{
					int a = Species[i].RARITY((int)type);
					if(a == 0) continue;
					genus_index.Add((int) type);
					species_index.Add(i);
					chances.Add(1.0F/a);
				}
			}
		}
		else
		{
			for(int i = 0; i < Species.Count; i++)
			{
				if(limit_by_chance && Species[i].Chance == 1.0F) continue;
				else
				{
					for(int g = 0; g < Species[i].AllGenus.Length; g++)
					{
						int a = Species[i].RARITY(g);
						if(a == 0) continue;
						genus_index.Add(g);
						species_index.Add(i);
						chances.Add(1.0F/a);
					}
				}
			}
		}


		index = ChanceEngine.Index(chances.ToArray());
		TileInfo final_spec = new TileInfo(Species[species_index[index]], (GENUS) genus_index[index]);
		return final_spec;

	}

	public SPECIES GetRandomEnemyType()
	{
		List<SPECIES> choice = new List<SPECIES>();

		foreach(SPECIES child in Species)
		{
			if(child.isEnemy)
			{
				choice.Add(child);
			}
		}

		if(choice.Count == 0) return null;
		int rand = UnityEngine.Random.Range(0, choice.Count);
		return choice[rand];
	}

	public Sprite [] SpriteOf(string name, int value = 0)
	{
		if(name == null || name == string.Empty || name == " ") return new Sprite[0];
		List<Sprite> sprites = new List<Sprite>();
		string [] array = name.Split(' ');

		if(array[0] == "Green") sprites.Add(TileMaster.Genus.GreenFrame);
		if(array[0] == "Red") sprites.Add(TileMaster.Genus.RedFrame);
		if(array[0] == "Blue") sprites.Add(TileMaster.Genus.BlueFrame);
		if(array[0] == "Yellow") sprites.Add(TileMaster.Genus.YellowFrame);
		if(array[0] == "Purple") sprites.Add(TileMaster.Genus.PurpleFrame);
		if(array[0] == "Grey") sprites.Add(TileMaster.Genus.GreyFrame);
		if(array[0] == "All" || array[0] == "Prism") sprites.Add(TileMaster.Genus.AllFrame);

		if(array.Length > 1)
		{
			SPECIES s = this[array[1]];
			if(s != null) 
			{
				if(array[0] != " " && s[array[0]] != null)
				{
					if(s[array[0]].Sprites != null && s[array[0]].Sprites.Length != 0)
					{
						sprites.Add(s[array[0]].Sprites[0]);
					}
				}
				
			}
		}
		else if(sprites.Count == 0)
		{
			SPECIES s = this[array[0]];
			if(s != null)
			{
				sprites.Add(s["All"].Sprites[0]);
			}
		}
		
		return sprites.ToArray();
	}


	public GENUS GENUSOf(string name)
	{
		return TileMaster.Genus.GenusOf(name);
	}

	public int GetIndex(string name)
	{
		if(name == "Green") return 2;
		else if(name == "Red") return 0;
		else if(name == "Blue") return 1;
		else if(name == "Yellow") return 3;
		else if(name == "Purple") return 4;
		else if(name == "All") return 5;
		else if(name == "Grey")	return 6;

		foreach(SPECIES child in Species)
		{
			if(name.ToLower() == child.Name) return child.Index;
		}
	
		return 100;
	}
}

[System.Serializable]
public class GenusInfo
{
	public Rarity _Rarity;

	[Range (0, 1.0F)]
	public float ChanceInitial;
	public float ChanceAdded;

	public float Chance{
		get{return ChanceInitial + ChanceAdded;}
	}

	public IntVector ValueInitial = new IntVector(1,1);
	public IntVector ValueAdded;
	public IntVector Value
	{
		get{
			IntVector final = new IntVector(ValueInitial);
			final.Add(ValueAdded);
			return final;
		}
	}
	[HideInInspector]
	public GENUS Genus;
	
	public Sprite [] Sprites;
	public List<TileEffectInfo> Effects;

	public GenusInfo(GENUS g)
	{
		Genus = g;
	}
}

[System.Serializable]
public class SPECIES
{
	public static SPECIES None = new SPECIES();
	
	public string Name;
	[HideInInspector]
	public int Index;
	public TYPE Type;
	public GameObject Prefab;
	public ShiftType Shift = ShiftType.Down;

	public GenusInfo Red = new GenusInfo(GENUS.STR),
					 Blue = new GenusInfo(GENUS.DEX),
					 Green = new GenusInfo(GENUS.WIS),
					 Yellow = new GenusInfo(GENUS.CHA),
					 Purple = new GenusInfo(GENUS.PRP),
					 All = new GenusInfo(GENUS.ALL),
					 Grey = new GenusInfo(GENUS.OMG);

	public ObjectPool TilePool;

	public GenusInfo [] AllGenus
	{
		get{return new GenusInfo[] {Red, Blue, Green, Yellow, Purple, All, Grey};}
	}

	public GenusInfo this [int i]{
		get{return AllGenus[i];}
	}

	public GenusInfo this [GENUS i] {
		get{return AllGenus[(int)i];}
	}

	public GenusInfo this[string s]
	{
		get{
			return AllGenus[(int)TileMaster.Genus[s]];
		}
	}

	public int Length {get{return AllGenus.Length;}}

	public Rarity _Rarity;
	public int RARITY(int i)
	{
		if(_Rarity == Rarity.Impossible|| AllGenus[i]._Rarity == Rarity.Impossible) 
		{
			return 0;
		}
		return ((int) _Rarity * (int) AllGenus[i]._Rarity);
	}

	public Sprite [] GetSprites(int g)
	{
		GenusInfo _genus = this[g];
		if(_genus.Sprites.Length == 0) return All.Sprites;
		return _genus.Sprites;
	}

	//[Range (0, 1.0F)]
	private float ChanceInit = 1;
	public float ChanceAdded;

	public float Chance{
		get{return ChanceInit + ChanceAdded;}
	}

	public void ResetChances()
	{
		ChanceAdded = 0.0F;
		_ValueAdded = new IntVector(0,0);
		Effects = new List<TileEffectInfo>();
		foreach(GenusInfo child in AllGenus)
		{
			child.ChanceAdded = 0.0F;
			child.ValueAdded = new IntVector(0,0);
			child.Effects = new List<TileEffectInfo>();
		}
	}


	public IntVector _ValueAdded;

	public bool LockValue = false;

	public int Value
	{
		get
		{
			int v = UnityEngine.Random.Range(_ValueAdded.x, _ValueAdded.y);
			if(v <= 0) v = 1;
			return v;
		}
	}

	public int [] GetValues()
	{
		return _ValueAdded.ToInt;
	}

	public List<TileEffectInfo> Effects;

	public bool IsType(string species)
	{		
		string s = species.ToLower();
		if(species == string.Empty) return true;
		if(s == "enemy") return isEnemy;
		if(s == "ally") return isAlly;
		if(s == "resource") return isResource;
		if(s == "armour") return isArmour;
		if(s == "health") return isHealth;
		return Name == s;
	}

	public bool isEnemy;
	public bool isAlly;
	public bool isHealth;
	public bool isArmour;
	public bool isResource;
	public bool isSpell;
}

[System.Serializable]
public class TileInfo
{

	public string Name
	{
		get{return _GenusName + " " + _TypeName;}
	}
	public GENUS _GenusEnum;
	public string _GenusName
	{
		get{
			switch(_GenusEnum)
			{
				case GENUS.STR: return "Red";
				case GENUS.WIS: return "Green";
				case GENUS.DEX: return "Blue";
				case GENUS.CHA: return "Yellow";
				case GENUS.PRP: return "Purple";
				case GENUS.ALL: return "Alpha";
				case GENUS.OMG: return "Omega";
			}
			return "Null";
		}

	}

	public SPECIES _Type;
	public TYPE _TypeEnum;
	public string _TypeName;

	public Sprite [] Inner;
	public Sprite Outer;
	public int Rarity;
	
	public ShiftType Shift;
	public bool ShiftOverride;

	public float FinalChance;
	public IntVector FinalValue;
	public int Scale = 1;

	public List<TileEffectInfo> FinalEffects;

	public TileInfo(SPECIES s, GENUS g)
	{
		if(g == GENUS.NONE)
		{
			g = (GENUS) UnityEngine.Random.Range(0, s.AllGenus.Length-1);
		}
		if(g == GENUS.RAND)
		{
			g = (GENUS) UnityEngine.Random.Range(0,4);
		}
		int i = (int) g;

		_Type = s;
		_TypeName = s.Name;
		_TypeEnum = s.Type;

		_GenusEnum = g;
		
		Inner = s.GetSprites(i);
		Outer = TileMaster.Genus.Frame[i];
		Rarity = s.RARITY(i);
		
		Shift = s.Shift;
		FinalChance = s[i].Chance * s.Chance + TileMaster.Genus.ChancesAdded[i];	
		FinalValue = new IntVector(1,1);
		FinalValue.Add(s._ValueAdded);
		FinalValue.Add(s[g].Value);

		FinalEffects = new List<TileEffectInfo>();
		FinalEffects.AddRange(s.Effects);
		FinalEffects.AddRange(s[g].Effects);
		
		if(!s.isEnemy) 
		{
			FinalValue.Mult(1 + GameManager.Difficulty/10);
		}
	}

	public TileInfo(TileInfo t)
	{
		_GenusEnum = t._GenusEnum;
		_Type = t._Type;
		_TypeEnum = t._TypeEnum;
		_TypeName = t._TypeName;
		Inner = t.Inner;
		Outer = t.Outer;
		Rarity = t.Rarity;
		Shift = t.Shift;
		FinalChance = t.FinalChance;
		FinalValue = t.FinalValue;
		FinalEffects = t.FinalEffects;
	}

	public int Value
	{
		get
		{
			int v = UnityEngine.Random.Range(FinalValue.x, FinalValue.y);
			if(v <= 0) v = 1;
			return v;
		}
	}


	public bool IsType(string g, string t)
	{
		bool genus = false;
		bool type = false;

		GENUS GG = TileMaster.Genus[g];
		genus = (GG == _GenusEnum);
		type = (t == string.Empty || t == _TypeName);

		return genus && type;
	}

	public void ChangeGenus(GENUS g)
	{
		int i = (int)g;
		_GenusEnum = g;
		Outer = TileMaster.Genus.Frame[i];
		Inner = _Type.GetSprites(i);
	}
}

[System.Serializable]
public class IntVector
{
	//public static IntVector zero = new IntVector(0,0);
	public int x, y;
	public IntVector(int a, int b)
	{
		x = a;
		y = b;
	}

	public IntVector(float a, float b)
	{
		x = (int) a;
		y = (int) b;
	}
	public IntVector(IntVector a)
	{
		x = a.x;
		y = a.y;
	}

	public IntVector(int a)
	{
		x = a;
		y = a;
	}

	public Vector2 ToVector2
	{
		get{
			return new Vector2(x, y);
		}
	}

	public int [] ToInt
	{
		get{
			return new int[] {x,y};
		}
	}

	public void Add(IntVector a)
	{
		x += a.x;
		y += a.y;
	}

	public void Sub(IntVector a)
	{
		x -= a.x;
		y -= a.y;
	}

	public void Mult(float m)
	{
		x = (int)((float)x*m);
		y = (int)((float)y*m);
	}
}

public class ObjectPool
{
	Tile Object;
	Transform Parent;
	public int Count;

	public Stack<Tile> Available;
	public ArrayList All;

	public Vector3 PoolPos = new Vector3(0, -100, 0);

	public ObjectPool(Tile t, int num, Transform _parent)
	{
		Object = t;
		Parent = _parent;
		Available = new Stack<Tile>();
		All = new ArrayList(num);
		for(int i = 0; i < num; i++)
		{
			Tile Obj = InstantiateTileObj();
			Available.Push(Obj);
			All.Add(Obj);

			Obj.gameObject.SetActive(false);
		}
	}

	public ObjectPool(GameObject t, int num, Transform _parent)
	{
		Object = t.GetComponent<Tile>();
		Parent = _parent;
		Available = new Stack<Tile>();
		All = new ArrayList(num);
		for(int i = 0; i < num; i++)
		{
			Tile Obj =  InstantiateTileObj();
			Available.Push(Obj);
			All.Add(Obj);
			Obj.gameObject.SetActive(false);
		}
	}

	public Tile Spawn()
	{
		Tile result;
		if(Available.Count == 0)
		{
			result =  InstantiateTileObj();
			All.Add(result);
		}
		else
		{
			result = Available.Pop();
		}
		result.gameObject.SetActive(true);
		return result;
	}

	public bool Unspawn(Tile t)
	{
		if(!Available.Contains(t))
		{
			Available.Push(t);
			t.transform.position = PoolPos;
			t.transform.parent = Parent;
			t.gameObject.SetActive(false);
			return true;
		}
		return false;
	}

	public Tile InstantiateTileObj()
	{
		Tile Obj = (Tile) GameObject.Instantiate(Object);

		GameObject newmodel = GameObject.Instantiate(GameData.TileModel);
		newmodel.transform.SetParent(Obj.transform);
		newmodel.transform.position = Obj.transform.position;
		Obj.Params = newmodel.GetComponent<TileParamContainer>();

		Obj.transform.position = PoolPos;
		Obj.transform.SetParent(Parent);
		return Obj;
	}

}

//[CustomPropertyDrawer (typeof (IntVector))]
//public class IntVectorDrawer : PropertyDrawer {
//	
//	// Draw the property inside the given rect
//	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
//		// Using BeginProperty / EndProperty on the parent property means that
//		// prefab override logic works on the entire property.
//		EditorGUI.BeginProperty (position, label, property);
//		
//		// Draw label
//		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
//		
//		// Don't make child fields be indented
//		var indent = EditorGUI.indentLevel;
//		EditorGUI.indentLevel = 0;
//		
//		// Calculate rects
//		var amountRect = new Rect (position.x, position.y, 50, position.height);
//		var unitRect = new Rect (position.x+55, position.y, 50, position.height);
//		
//		// Draw fields - passs GUIContent.none to each so they are drawn without labels
//		EditorGUI.PropertyField (amountRect, property.FindPropertyRelative ("x"), GUIContent.none);
//		EditorGUI.PropertyField (unitRect, property.FindPropertyRelative ("y"), GUIContent.none);
//	
//		// Set indent back to what it was
//		EditorGUI.indentLevel = indent;
//		
//		EditorGUI.EndProperty ();
//	}
//}