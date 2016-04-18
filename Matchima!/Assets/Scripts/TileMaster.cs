using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMaster : MonoBehaviour {
	public static TileMaster instance;
	public static TileTypes Types;
	public static GenusTypes Genus;
	[SerializeField] private GenusTypes _GenusTypes;

	public static GridInfo Grid;
	public static Tile[,] Tiles
	{
		get{return TileMaster.Grid.Tiles;}
	}

	public Tile this[int x, int y]
	{
		get{
			return TileMaster.Grid.Tiles[x,y];
		}
	}

	public Tile this[int [] num]
	{
		get
		{
			return TileMaster.Grid.Tiles[num[0], num[1]];
		}
	}

	public static bool FillGrid_Override = true;
	
	public int EnemiesOnScreen;

	public Vector2 MapSize;
	public float tileBufferX = 0.2F, tileBufferY = 0.2F;
	public float YOffset = 1.0F;
	
	public MiniTile ResMiniTile;

	[HideInInspector]
	public GameObject TileBasic;
	public GameObject [] GroundBlocks;


	public bool scaleup;

	
	public Sprite KillEffect;

	public bool generated = false;
	public List<SPECIES> QueuedTiles = new List<SPECIES>();
	public List<GENUS> QueuedGenus = new List<GENUS>();
	public List<int> QueuedValue = new List<int>();

	private float [] spawn_stack;
	private bool first_enemy = false;

	private bool FillGrid = false;
	private GridInfo level_to_load = null;
	bool [,] fill_from_none;

	public float AllChanceInit;
	public float AllChanceCurrent;
	public static float ChanceRatio = 1.0F;


	public static bool AllLanded
	{
		get
		{
			if(Tiles == null) return false;
			for(int x = 0; x < Tiles.GetLength(0); x++)
			{
				for(int y = 0; y < Tiles.GetLength(1); y++)
				{
					if(Tiles[x,y] == null) continue;
					if(Tiles[x,y].isFalling) {Debug.LogWarning("WAITING FOR " + x + ":" + y); return false;}
				}
			}
			return true;
		}
	}

	public List<MatchContainer> Matches;

	void Awake()
	{
		if(instance == null)
		{
			DontDestroyOnLoad(transform.gameObject);
			instance = this;
		}
		else if(instance != this) 
		{
			instance.Start();
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {

		ClearGrid(true);
		generated = false;
		FillGrid = false;
		Types = this.GetComponent<TileTypes>();
		Types.Setup();
		Genus = _GenusTypes;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.instance.gameStart) 
		{
			if(!generated)
			{
				print("GENERATING LEVEL " + (level_to_load != null));
				
				Player.instance.ResetChances();
				AllChanceInit = Spawner2.AllChanceFactors;
				AllChanceCurrent = AllChanceInit;
				ChanceRatio = AllChanceCurrent / AllChanceInit;
				QueuedTiles.Clear();
				spawn_stack = new float [(int)MapSize.x];
				GenerateGrid(level_to_load);
				generated = true;	
			}
			
			if(FillGrid) ShiftTiles2(Player.Stats.Shift);
		}
	}

	public void IncreaseChance(string g, string t, float amt)
	{
		
		if(g != string.Empty && t == string.Empty)
		{	
			int num = (int) Genus[g];
			Genus.ChancesAdded[num] += amt;
		}
		else if(g == string.Empty && t != string.Empty)
		{
			Types[t].ChanceAdded += amt;
		}
		else
		{
			Types[t][g].ChanceAdded += amt;
		}
		//Spawner2.GetSpawnables(Types);
		//AllChanceCurrent = Spawner2.AllChanceFactors;
		//ChanceRatio = AllChanceCurrent / AllChanceInit;
	}

	public void ResetChances()
	{
		Genus.ChancesAdded = new float [] {0.0F,
											0.0F,
											0.0F,
											0.0F,
											0.0F,
											0.0F,
											0.0F};

		Types.Setup();
		//Spawner2.GetSpawnables(Types, GameManager.instance._Wave);
	}

	public Tile GetTile(int x, int y)
	{
		if(Tiles.GetLength(0) > x && x >= 0)
		{
			if(Tiles.GetLength(1) > y && y >= 0)
			{
				return Tiles[x,y];
			}
		}
		return null;
	}

	public void LevelToLoad(GridInfo level)
	{
		level_to_load = level;
	}

	public void GenerateGrid(GridInfo level = null, float wait = 0.0F, bool clear = true)
	{
		if(clear) 
		{
			ClearGrid(true);
			StartCoroutine(_GenerateGrid(level, wait));
		} 
	}

//Generates a grid
	private IEnumerator _GenerateGrid(GridInfo level, float wait)
	{
		if(level != null) //If loading a GridInfo file
		{
			MapSize = new Vector2(level.Points.GetLength(0),  level.Points.GetLength(1));
			bool evenX = MapSize[0]%2 == 0;
			bool evenY = MapSize[1]%2 == 0;
			float tileBufferX_offset = (float)(evenX ? (1.0F+tileBufferX)/2 : 0);
			float tileBufferY_offset = (float)(evenY ? (1.0F+tileBufferY)/2 : 0);

			int [] tile_size = new int [2] {(int)Grid.Size[0], (int)Grid.Size[1]};
			int x = 0, y = 0;

			Grid = level;
			Grid.SetUp(MapSize);
			Grid.SetInfo(level);

			float ortho = Mathf.Max(Grid.Size[0] * 1.4F, Grid.Size[1] *1.35F);
			CameraUtility.TargetOrtho = Mathf.Clamp(ortho, 7, Mathf.Infinity);

			CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0,0].position, 
													Grid[Grid.Size[0]-1, Grid.Size[1]-1].position,
													0.5F));
			spawn_stack = new float [tile_size[0]];
			for(int xx = 0; xx < Grid.Size[0];xx++)
			{
				spawn_stack[xx] = Camera.main.orthographicSize + 2.0F;
			}
						
			
			for(int xx = -tile_size[0]/2; xx < (evenX ? tile_size[0]/2 : tile_size[0]/2+1);xx++)
			{
				y = 0;
				for(int yy = -tile_size[1]/2; yy < (evenY ? tile_size[1]/2 : tile_size[1]/2+1);yy++)
				{		
					CreateTile(x,y, new Vector2(0, 1)); //level.Points[x,y].Species);
					y++;			
				}
				x++;
				yield return null;
			}
		}
		else //Create random grid
		{
			if(wait != 0.0F) yield return new WaitForSeconds(wait);
			MapSize = Player.Stats.MapSize;

			bool evenX = MapSize[0]%2 == 0;
			bool evenY = MapSize[1]%2 == 0;
			float tileBufferX_offset = (float)(evenX ? (1.0F+tileBufferX)/2 : 0);
			float tileBufferY_offset = (float)(evenY ? (1.0F+tileBufferY)/2 : 0);

			Grid = new GridInfo();
			Grid.SetUp(MapSize);

			float ortho = Mathf.Max(Grid.Size[0] * 1.4F, Grid.Size[1] *1.35F);
			CameraUtility.TargetOrtho = Mathf.Clamp(ortho, 7, Mathf.Infinity);

			CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0,0].position, 
													Grid[Grid.Size[0]-1, Grid.Size[1]-1].position,
													0.5F));
			
			spawn_stack = new float [Grid.Size[0]];
			for(int x = 0; x < Grid.Size[0]; x++)
			{
				spawn_stack[x] = Camera.main.orthographicSize + 2.0F;
			}
			for(int xx = 0; xx < Grid.Size[0];xx++)
			{
				for(int yy = 0; yy < Grid.Size[1];yy++)
				{		
					CreateTile(xx,yy, new Vector2(0, 1));	
				}
				yield return null;
			}

			
		}
	}

	public void IncreaseGridTo(Vector2 final)
	{
		IncreaseGrid((int)(final.x - MapSize.x), (int)(final.y - MapSize.y));
	}

	public void IncreaseGrid(Vector2 diff)
	{
		IncreaseGrid((int)diff.x, (int)diff.y);
	}

	public void IncreaseGrid(int x, int y)
	{
		//StartCoroutine(_IncreaseGrid(x,y));
		MapSize += new Vector2(x,y);

		Grid.Increase(new Vector2(x,y));

		float ortho = Mathf.Max(Grid.Size[0] * 1.4F, Grid.Size[1] *1.35F);
		CameraUtility.TargetOrtho = Mathf.Clamp(ortho, 7, Mathf.Infinity);

		CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0,0].position, 
												Grid[Grid.Size[0]-1, Grid.Size[1]-1].position,
												0.5F));

		spawn_stack = new float [Grid.Size[0]];
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			spawn_stack[xx] = Camera.main.orthographicSize + 2.0F;
		}
		FillGrid = true;
	}

	private IEnumerator _IncreaseGrid(int x, int y)
	{
		MapSize += new Vector2(x,y);

		Grid.Increase(new Vector2(x,y));
		
		yield return null;

		float ortho = Mathf.Max(Grid.Size[0] * 1.4F, Grid.Size[1] *1.35F);
		CameraUtility.TargetOrtho = Mathf.Clamp(ortho, 7, Mathf.Infinity);

		CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0,0].position, 
												Grid[Grid.Size[0]-1, Grid.Size[1]-1].position,
												0.5F));

		spawn_stack = new float [Grid.Size[0]];
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			spawn_stack[xx] = Camera.main.orthographicSize + 2.0F;
		}
		FillGrid = true;
		yield return null;
	}

	private void CreateTile(int x, int y, Vector2 velocity, SPECIES s = null, GENUS g = GENUS.NONE, bool no_enemies = false, int scale = 1, int value = 0)
	{
		if(Tiles.GetLength(0)>x && Tiles.GetLength(1) > y)
		{
			if(Tiles[x,y] != null) return;
		}
		
		TileInfo t = null;
		if(s == null || s == SPECIES.None)
		{
			if(no_enemies) t = Spawner2.RandomTileNoEnemies();
			else t = Spawner2.RandomTile();

			if(g != GENUS.NONE) t.ChangeGenus(g);
			if(t._GenusEnum == GENUS.RAND)
			{
				t._GenusEnum = (GENUS) Random.Range(0,4);
			}
		}
		else
		{
			t = new TileInfo(s, g);
		}

		if(t == null) return;

		GameObject tile_temp = (GameObject) Instantiate(t._Type.Prefab);

		float ny = velocity.y == 0.0F ? Grid.GetPoint(x,y).y : spawn_stack[x];
		float nx = velocity.x == 0.0F ? Grid.GetPoint(x,y).x : spawn_stack[y];

		tile_temp.transform.position = new Vector3(nx, ny,0);
		tile_temp.name = "Tile | " + x + ":" + y;
		tile_temp.transform.parent = Grid.Column[x].transform;
	
		Grid[x,y]._Tile = tile_temp.GetComponent<Tile>();
		Grid[x,y]._Tile.Setup(x,y,scale,t, value);

		for(int xx = 0; xx < scale; xx++)
		{
			for(int yy = 0; yy < scale; yy++)
			{
				Grid[x+xx,y+yy]._Tile = Grid[x,y]._Tile;
			}
		}
	
		float half_x = (tileBufferX)/2;
		float half_y = (tileBufferY)/2;
		SphereCollider tile_col = tile_temp.GetComponent<SphereCollider>();
		tile_col.radius += half_x;
		//Grid[x,y]._Tile.collide_radius = tile_col.radius;


		if(velocity.y > 0.0F) spawn_stack[x] += tile_col.radius * 2;
		else if(velocity.y < 0.0F) spawn_stack[x] -= tile_col.radius * 2;
		else if(velocity.x > 0.0F) spawn_stack[y] += tile_col.radius * 2;
		else if(velocity.x < 0.0F) spawn_stack[y] -= tile_col.radius * 2;
	}

	public void ReplaceTile(int x, int y, SPECIES sp = null, GENUS g = GENUS.NONE, int newscale = 1, int addvalue = 0)
	{
		while(x+newscale > Grid.Size[0] || y+newscale > Grid.Size[1])
		{
			if(x+newscale > Grid.Size[0]) x--;
			if(y+newscale > Grid.Size[1]) y--;
		}
		Tile temp = Tiles[x,y];
		int scale = 1;
		if(temp != null) scale = temp.Point.Scale;
		if(scale <= newscale) scale = 1;
		
		Grid[x,y]._Tile = null;
		for(int xx = 0; xx < scale; xx++)
		{
			for(int yy = 0; yy < scale; yy++)
			{
				if(Grid[x+xx,y+yy]._Tile != null) temp =  Grid[x+xx,y+yy]._Tile;
				Grid[x+xx,y+yy]._Tile = null;

				CreateTile(x+xx,y+yy, Vector2.zero, sp, g, false, newscale, addvalue);
				//Grid[x+xx,y+yy]._Tile.AddValue(addvalue);
				EffectManager.instance.PlayEffect(Grid[x+xx,y+yy]._Tile.transform, Effect.Replace, "", GameData.instance.GetGENUSColour(Grid[x+xx,y+yy]._Tile.Genus));
			
				if(temp != null)
				{
					if(temp.Stats.Value > 0) Grid[x+xx,y+yy]._Tile.InitStats.Value += temp.Stats.Value-1;
					Destroy(temp.gameObject);
				}	
			}
		}	
	}

	public void ReplaceTile(Tile t, SPECIES sp = null, GENUS g = GENUS.NONE, int newscale = 1, int addvalue = 0)
	{
		if(t == null) return;
		ReplaceTile(t.Point.Base[0], t.Point.Base[1], sp, g, newscale, addvalue);
	}

	public void SwapTiles(Tile a, Tile b)
	{
		//Grid[a.Point.Base[0], a.Point.Base[1]]._Tile = b;
		//Grid[b.Point.Base[0], b.Point.Base[1]]._Tile = a;
		int [] a_point = a.Point.Base;
		int [] b_point = b.Point.Base;
		a.MoveToGridPoint(b_point[0],b_point[1], 0.7F);
		b.MoveToGridPoint(a_point[0],a_point[1], -0.7F);
		//a.Setup(b_point[0], b_point[1]);
		//b.Setup(a_point[0], a_point[1]);
	}

	public void AddVelocity(Tile t)
	{

	}


	public void AddVelocityToColumn(int x, int y, float amount)
	{
		return;
		if(Tiles == null) return;
		for(int yy = 0; yy < Grid.Size[1]; yy++)
		{
			if(yy <= y || Tiles.GetLength(0) <= x || Tiles.GetLength(1) <= yy) continue;
			if(Tiles[x,yy] == null || Tiles[x,yy].Stats.Shift == ShiftType.None) continue;
			Tiles[x,yy].speed += amount;
		}
	}


	public void ClearGrid(bool destroy = false)
	{
		if(Grid == null) return;
		if(Tiles == null || Tiles.GetLength(0) == 0) return;
		FillGrid = false;
		for(int x = 0; x < Grid.Points.GetLength(0);x++)
		{
			for(int y = 0; y < Grid.Points.GetLength(1);y++)
			{
				if(Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x,y] != null)
				{
					Tiles[x,y].DestroyThyself();//true);
					Grid[x,y]._Tile = null;
				}
			}
		}

		if(!destroy) return;
		if(Grid != null) Grid.DestroyThyself();
	}

	public void NewGrid(SPECIES sp = null, GENUS g = GENUS.NONE, bool no_enemies = false)
	{
		for(int x = 0; x < Grid.Size[0];x++)
		{
			for(int y = 0; y < Grid.Size[1];y++)
			{
				if(Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x,y] != null)
				{
					Tiles[x,y].DestroyThyself();// true);
					//Destroy(Tiles[x,y].gameObject);
					Grid[x,y]._Tile = null;
					CreateTile(x,y, Vector2.up, sp, g, no_enemies);
				}
			}
		}
	}

	public IEnumerator BeforeTurn()
	{
		while(!AllLanded)	yield return null;

		EnemiesOnScreen = 0;
		for(int x = 0; x < Grid.Size[0]; x++)
		{
			for(int y = 0; y < Grid.Size[1];y++)
			{
				if(Grid[x,y]._Tile == null) continue;
				if(Tiles[x,y] != null || Tiles[x,y].Type == null)
				{
					Tiles[x,y].AfterTurnCheck = false;
					if(Tiles[x,y].BeforeTurnEffect)
						yield return StartCoroutine(Tiles[x,y].BeforeTurnRoutine());
					else Tiles[x,y].BeforeTurn();
					if(Tiles[x,y] == null || Tiles[x,y].Type == null) continue;
					if(Tiles[x,y].Type.isEnemy) EnemiesOnScreen ++;
				}
			}
		}
		
		yield break;
	}

	public IEnumerator AfterTurn()
	{
		if(Player.Stats.isKilled) yield break;
		while(!AllLanded) 
		{
			if(Player.Stats.isKilled) yield break;
			yield return null;
		}
		EnemiesOnScreen = 0;
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for(int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if(Tiles[xx,yy] != null) 
				{
					if(Tiles[xx,yy].isMatching) 
					{
						Tiles[xx,yy].DestroyThyself();
						continue;
					}
					if(Tiles[xx,yy].AfterTurnCheck) continue;
					Tiles[xx,yy].AfterTurnCheck = true;
					if(Tiles[xx,yy].Type.isEnemy) EnemiesOnScreen ++;
					Tiles[xx,yy].AfterTurn();
					if(Tiles[xx,yy].AfterTurnEffect)
						yield return StartCoroutine(Tiles[xx,yy].AfterTurnRoutine());
				}
				//else if(fill_from_none[xx, yy]) ReplaceTile(xx,yy);
			}
		}

		spawn_stack = new float [(int)Grid.Size[0]];
		for(int x = 0; x < Grid.Size[0]; x++)
		{
			spawn_stack[x] = Camera.main.orthographicSize + 2.0F;
		}
		ResetTiles();
		CheckForEmptys();
		if(!FillGrid) yield break;
		if(!TileMaster.instance.CheckForMatches()) 
		{
			yield return new WaitForSeconds(0.2F);
			NewGrid();
		}
		yield break;
	}

	public void CheckForEmptys()
	{
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for(int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if(Tiles[xx,yy] == null && fill_from_none[xx, yy]) ReplaceTile(xx,yy);
			}
		}
	}

	public void SetAllTileStates(TileState s, bool _override = false)
	{
	
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for(int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if(Tiles[xx,yy] != null) Tiles[xx,yy].SetState(s, _override);
			}
		}
	}

	public void ResetTiles(bool idle = false)
	{
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for(int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if(Tiles[xx,yy] != null) Tiles[xx,yy].Reset(idle);
			}
		}
	}

	public void DestroyTile(int x, int y)
	{
		if(Tiles[x,y] != null && Tiles[x,y].gameObject != null)
		{
			Destroy(Tiles[x,y].gameObject);
			Player.instance.OnTileDestroy(Tiles[x,y]);
			Grid[x,y]._Tile = null;
			//if(FillGrid) ShiftTiles2(Player.Stats.Shift);
		}
	}

	public void DestroyTile(Tile t)
	{
		Destroy(t.gameObject);
		Player.instance.OnTileDestroy(t);
		if(Player.Stats.Shift == ShiftType.None) ReplaceTile(t.Point.Base[0], t.Point.Base[1]);
	}

	public void CollectTile(Tile t, bool destroy)
	{
		//t.GetComponent<SphereCollider>().enabled = false;
		CollectTileResource(t, destroy);
		Player.instance.OnTileCollect(t);
		//if(Player.Stats.Shift == ShiftType.None) ReplaceTile(t.num[0], t.num[1]);
		//ShiftTiles2(Player.Stats.Shift);
	}

	private void CollectTileResource(Tile t, bool destroy)
	{

		bool isMoving = true;
		bool isRes = false, isEnemy = false, isHealth = false;

		Vector3 startpos = t.transform.position;
		startpos.z = -8;

		EffectManager.instance.PlayEffect(t.transform, Effect.Destroy, "", GameData.instance.GetGENUSColour(t.Genus));
		Class [] c = null;
		RectTransform [] res = null;

		t.CheckStats();

		if(t.Genus != GENUS.OMG && t.Genus != GENUS.ALL && t.Genus != GENUS.NONE && t.Genus != GENUS.PRP)
		{
			c = new Class[1] {Player.Classes[(int)t.Genus]};
			res = new RectTransform[1] {UIManager.ClassButtons[(int)t.Genus].transform as RectTransform};
		} 
		else if(t.Genus == GENUS.ALL)
		{
			c = new Class[UIManager.ClassButtons.Length];
			res = new RectTransform[UIManager.ClassButtons.Length];
			for(int i = 0; i < UIManager.ClassButtons.Length; i++)
			{
				if(UIManager.ClassButtons[i]._class == null) continue;
				c[i] = UIManager.ClassButtons[i]._class;
				res[i] = UIManager.ClassButtons[i].transform as RectTransform;
			}
		}

		if(res == null) res = new RectTransform[1]{UIManager.ClassButtons[0].transform as RectTransform};

		List<MiniTile> restiles = new List<MiniTile>();

		if(t.Type.isResource)
		{
			for(int rect = 0; rect < res.Length; rect++)
			{
				int val = Mathf.Clamp(t.Stats.Value/4, 1, 10);
				for(int i = 0; i < val; i++)
				{
					MiniTile note = (MiniTile) Instantiate(ResMiniTile);
					note._render.sprite = t.Params._border.sprite;
					note.transform.position = t.transform.position + (i > 0 ? GameData.RandomVector*1.4F : Vector3.zero);
					note.SetTarget(res[rect] as Transform, 0.45F);
					note._Class = rect < c.Length ? c[rect] : null;
					note.SetMethod(() =>{
							if(note._Class != null) note._Class.AddToMeter(t.Stats.GetValues()[0]);
						}
						);
					note.acc = 0.04F;
					restiles.Add(note);
				}
			}
			
		}
		if(t.Type.isHealth)
		{
			for(int rect = 0; rect < res.Length; rect++)
			{
				MiniTile health = (MiniTile) Instantiate(ResMiniTile);
				health._render.sprite = t.Params._border.sprite;
				health.transform.position = t.transform.position;
				health.SetTarget(UIManager.instance.HealthImg.transform, 0.2F, 0.0F);
				health._Class = rect < c.Length ? c[rect] : null;
				health.SetMethod(() =>{
						Player.Stats.Heal(t.Stats.GetValues()[1]);
						//health._Class.Stats.Heal(t.Stats.Heal);
					}
					);
				restiles.Add(health);
			}
		}
		if(t.Type.isArmour)
		{
			for(int rect = 0; rect < res.Length; rect++)
			{
				MiniTile health = (MiniTile) Instantiate(ResMiniTile);
				health._render.sprite = t.Params._border.sprite;
				health.transform.position = t.transform.position;
				health.SetTarget(UIManager.instance.HealthImg.transform, 0.2F, 0.0F);
				health._Class = rect < c.Length ? c[rect] : null;
				health.SetMethod(() =>{
						Player.Stats.AddArmour(t.Stats.GetValues()[2]);
						//health._Class.Stats.AddArmour(t.Stats.Armour);
					}
					);
				restiles.Add(health);
			}
		}
		if(t.Type.isEnemy)
		{
			for(int i = 0; i < GameManager.instance._Wave.Length; i++)
			{
				if(GameManager.instance._Wave[i] == null || !GameManager.instance._Wave[i].Active) continue;

				if(GameManager.instance._Wave[i].PointsPerEnemy > 0)
				{
				 	Wave w = GameManager.instance._Wave[i];
					MiniTile enemy = (MiniTile) Instantiate(ResMiniTile);
					enemy._render.sprite = t.Params._border.sprite;
					enemy.transform.position = t.transform.position;
					enemy.SetTarget(UIManager.Objects.WaveSlots[i].transform, 0.2F, 0.0F);
					enemy.SetMethod(() =>{
								if(w != null) w.EnemyKilled(t as Enemy);
							}
							);
					restiles.Add(enemy);
				}
			}
			
			
			for(int rect = 0; rect < res.Length; rect++)
			{
				int val = Mathf.Clamp(t.Stats.Value/4, 1, 10);
				for(int i = 0; i < val; i++)
				{
					MiniTile note = (MiniTile) Instantiate(ResMiniTile);
					note._render.sprite = t.Params._border.sprite;
					note.transform.position = t.transform.position + (i > 0 ? GameData.RandomVector*1.4F : Vector3.zero);
					note.SetTarget(res[rect] as Transform, 0.45F);
					note._Class = rect < c.Length ? c[rect] : null;
					note.SetMethod(() =>{
							if(note._Class != null) note._Class.AddToMeter(t.Stats.GetValues()[0]);
						}
						);
					note.acc = 0.04F;
					restiles.Add(note);
				}
			}
		}

		if(destroy) DestroyTile(t);
	}

	public MiniTile CreateMiniTile(Vector3 pos, Transform target, Sprite sprite = null, float scale = 0.23F, float speedY = -0.2F)
	{
		MiniTile new_mini = (MiniTile)Instantiate(ResMiniTile);
		if(sprite == null)
		{
			int num = Random.Range(0,4);
			sprite = Genus.Frame[num];
		}
		
		new_mini._render.sprite = sprite;
		new_mini.transform.position = pos;
		new_mini.SetTarget(target, scale, speedY);
		return new_mini;
	}

	public void QueueTile(SPECIES spec, GENUS g = GENUS.NONE, int value = 0)
	{
		QueuedTiles.Add(spec);
		QueuedGenus.Add(g);
		QueuedValue.Add(value);
	}

	private void CreateQueuedTiles(ref int tile_num, ref int? [] tile_start, Vector2 velocity)
	{
		if(QueuedTiles == null || QueuedTiles.Count == 0)
		{
			tile_num = tile_num;
			tile_start = tile_start;
			return;
		}

		GameData.Log("Generating " + QueuedTiles.Count + " queued tiles");
		int remove = 0;
		for(int i = 0; i < QueuedTiles.Count; i++)
		{
			if(tile_num <= 0) continue;

			List<int> poss_x = new List<int>();
			for(int _x = 0; _x < Grid.Size[0]; _x++)
			{
				if(tile_start[_x] != null)	poss_x.Add(_x);
			}

			int rand = Random.Range(0, poss_x.Count-1);
			int x = poss_x[rand];
			int y = (int) tile_start[x];

			GameData.Log("Queued tile " + i + " generated at " + x + ":" + y);

			CreateTile(x,y, velocity, QueuedTiles[i], QueuedGenus[i]);
			Tiles[x,y].InitStats.Value += QueuedValue[i];
			
			if(tile_start[x] < Grid.Size[1]-1) 
			{
				tile_start[x] ++;
			}
			else 
			{
				tile_start[x] = null;
				poss_x.RemoveAt(rand);
			}

			tile_num --;
			remove++;
		}
		QueuedTiles.RemoveRange(0,remove);
		QueuedGenus.RemoveRange(0,remove);
		QueuedValue.RemoveRange(0,remove);
	}

	private void ShiftTiles2(ShiftType type)
	{
		fill_from_none = new bool[(int)Grid.Size[0], (int)Grid.Size[1]];

		bool [,] ignore = new bool[(int)Grid.Size[0], (int)Grid.Size[1]];

		if(type == ShiftType.None)
		{
			for(int x = 0; x < Grid.Size[0]; x++)
			{
				for(int y = 0; y < Grid.Size[1]; y++)
				{
					if(Tiles[x,y] == null) fill_from_none[x,y] = true;
				}
			}
		}
		else if(type == ShiftType.Down)
		{
			int? empty = null;
			for(int x = 0; x < Grid.Size[0]; x++)
			{
				empty = null;
				for(int y = 0; y < Grid.Size[1]; y++)
				{

				//IF THERE IS A PROBLEM WITH GIANTS, ITS PROBABLY CAUSE I TURNED THIS OFF
					//if(ignore[x,y])continue;

				//INITAL SETTING OF THE BOTTOM EMPTY TILE OF A ROW
					if(Tiles[x,y] == null && !empty.HasValue)
					{
						empty = y;
						if(y < Grid.Size[1] - 1) fill_from_none[x,y] = true;
					}

				//IF THE TILE ABOVE AN EMPTY SPOT ISN'T NULL
					else if(empty.HasValue && Tiles[x,y] != null)
					{
						if(Tiles[x,y].Stats.Shift != ShiftType.None)
						{

				//IF TILE ABOVE IS GIANT WITH BASE ELSWHERE, CONTINUE
							if(Tiles[x,y].Point.BaseX != x) 
							{
								fill_from_none[x,empty.Value] = true;
								empty++;
								continue;
							}

				//IF NO SPACE FOR TILE TO FALL, CONTINUE
							bool nospace = false;
							for(int xx = 0; xx < Tiles[x,y].Point.Scale; xx++)
							{
								for(int yy = 0; yy < Tiles[x,y].Point.Scale; yy++)
								{
									if(x + xx > Grid.Size[0] - 1 || y + yy > Grid.Size[1] - 1) 
									{
										fill_from_none[x,empty.Value] = true;
										nospace = true;
										break;
									}

									nospace = Tiles[x + xx,y] != null && Tiles[x + xx,y] != Tiles[x,y];
									nospace = Tiles[x + xx,y + yy] != null && Tiles[x + xx,y + yy] != Tiles[x,y];

									if(nospace) break;
								}
								if(nospace)
								{
									fill_from_none[x + xx,empty.Value] = true;
									break;
								}
								else
								{
									for(int i = empty.Value; i < y; i++)
									{
										if(Tiles[x + xx,i] != null)
										{
											fill_from_none[x + xx,empty.Value] = true;
											nospace = true;
											break;
										}
									}
								}
							}
							if(nospace) 
							{
								//fill_from_none[x, empty.Value] = true;
								continue;
							}

				//SET TILE POINTS
							Grid[x,empty.Value]._Tile = Tiles[x,y];
							Grid[x,empty.Value]._Tile.Point.BaseX = x;
							Grid[x,empty.Value]._Tile.Point.BaseY = empty.Value;
							Grid[x,empty.Value]._Tile.Point.SetPoints();
							Grid[x,y]._Tile = null;

							int scale = Grid[x,empty.Value]._Tile.Point.Scale;
							for(int xx = 0; xx < scale; xx++)
							{
								for(int yy = 0; yy < scale; yy++)
								{
									Grid[x+xx,y+yy]._Tile = null;
									if(x+xx > Grid.Size[0] - 1 || empty.Value+yy > Grid.Size[1] - 1) continue;
									Grid[x+xx,empty.Value+yy]._Tile = Grid[x,empty.Value]._Tile;
									ignore[x+xx,empty.Value+yy] = true;

								}
								//if(y + scale != y) Grid[x+xx,y+scale]._Tile = null;
							}
						
				//ANTIGRAV CHECK
							
							fill_from_none[x,empty.Value] = false;
							if(y < Grid.Size[1] - 1) fill_from_none[x,y] = true;
							empty += scale;
							
						}	
						else 
						{
							//fill_from_none[x,empty.Value] = true;
							empty = null;	
						}				
					}
				//IF THE TILE ABOVE AN EMPTY SPOT IS NULL
					else if(Tiles[x,y] == null)
					{
						if(y < Grid.Size[1] - 1) fill_from_none[x,y] = true;
					}
				}
			}
		
			int new_tiles_num = 0;
			int? [] new_tiles_start = new int? [Grid.Size[0]];
			for(int x = 0; x < Grid.Size[0]; x++)
			{
				new_tiles_start[x] = null;
				for(int y = 0; y < Grid.Size[1]; y++)
				{
					
					if(Tiles[x,y] == null && !fill_from_none[x, y]) 
					{
						new_tiles_num++;
						if(new_tiles_start[x] == null) new_tiles_start[x] = y;
					}
				}
			}

			if(new_tiles_num != 0) {
				CreateQueuedTiles(ref new_tiles_num, ref new_tiles_start, Vector2.up);
				for(int x = 0; x < Grid.Size[0]; x++)
				{
					if(new_tiles_start[x] == null) continue;
					
					for(int _y = (int) new_tiles_start[x]; _y < Grid.Size[1]; _y++)
					{
						if(Tiles[x, new_tiles_start[x].Value] != null) continue;
						
			//INSERT STUFF FOR FALLING BIG TILES HERE
						int scale = 1;

						if(!fill_from_none[x, new_tiles_start[x].Value]) 
						{ 
							CreateTile(x, _y, Vector2.up, null, GENUS.NONE, false, scale);
						}
					}
				}
			}
		}
	}

	public bool CheckForMatches()
	{
		Matches = new List<MatchContainer>();
		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for(int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if(Tiles[xx,yy] == null) continue;
				Tiles[xx,yy].marked = false;
			}
		}

		for(int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for(int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if(Tiles[xx,yy] == null) continue;
				MatchContainer m = FloodCheck(Tiles[xx,yy]);
				if(m.Size >= 3) Matches.Add(m);
			}
		}

		return Matches.Count != 0;
	}

	public MatchContainer FloodCheck(Tile t, bool diagonals = true)
	{
		MatchContainer Match = new MatchContainer(diagonals);
		Match.Add(t);
		t.marked = true;
		GENUS s = t.Genus;

		int x = t.Point.Base[0], y = t.Point.Base[1];
		int ax = x - t.Point.Scale, bx = x + t.Point.Scale;
		int ay = y - t.Point.Scale, by = y + t.Point.Scale;

		List<Tile> checks = new List<Tile>();

		if(ax >= 0 && ax < Grid.Size[0])
		{
			if(Tiles[ax,y] != null)
			{
				if(Tiles[ax,y].IsGenus(s) && !Tiles[ax,y].marked) checks.Add(Tiles[ax,y]);
			}
		}

		if(bx >= 0 && bx < Grid.Size[0])
		{
			if(Tiles[bx,y] != null)
			{
				if(Tiles[bx,y].IsGenus(s) && !Tiles[bx,y].marked) checks.Add(Tiles[bx,y]);
			}
		}

		if(ay >= 0 && ay < Grid.Size[1])
		{
			if(Tiles[x,ay] != null)
			{
				if(Tiles[x,ay].IsGenus(s) && !Tiles[x,ay].marked) checks.Add(Tiles[x,ay]);
			}
		}

		if(by >= 0 && by < Grid.Size[1])
		{
			if(Tiles[x,by] != null)
			{
				if(Tiles[x,by].IsGenus(s) && !Tiles[x,by].marked) checks.Add(Tiles[x,by]);
			}
		}

		if(diagonals)
		{
			if(ax >= 0 && ax < Grid.Size[0])
			{
				if(ay >= 0 && ay < Grid.Size[1])
				{
					if(Tiles[ax,ay] != null)
					{
						if(Tiles[ax,ay].IsGenus(s) && !Tiles[ax,ay].marked) checks.Add(Tiles[ax,ay]);
					}
				} 
				if(by >= 0 && by < Grid.Size[1])
				{
					if(Tiles[ax,by] != null)
					{
						if(Tiles[ax,by].IsGenus(s) && !Tiles[ax,by].marked) checks.Add(Tiles[ax,by]);
					}
				} 
			}
			if(bx >= 0 && bx < Grid.Size[0])
			{
				if(ay >= 0 && ay < Grid.Size[1])
				{
					if(Tiles[bx,ay] != null)
					{
						if(Tiles[bx,ay].IsGenus(s) && !Tiles[bx,ay].marked) checks.Add(Tiles[bx,ay]);
					}
				} 
				if(by >= 0 && by < Grid.Size[1])
				{
					if(Tiles[bx,by] != null)
					{
						if(Tiles[bx,by].IsGenus(s) && !Tiles[bx,by].marked) checks.Add(Tiles[bx,by]);
					}
				} 
			}
		}
		foreach(Tile child in checks)
		{
			Match.Add(child);
			Match.AddRange(FloodCheck(child, diagonals));
		}

		return Match;
	}

	public void CollectMatches(bool diagonal = false)
	{
		StartCoroutine(CollectMatchesRoutine(diagonal));
	}

	IEnumerator CollectMatchesRoutine(bool diagonal)
	{
		FillGrid = true;
		foreach(MatchContainer child in Matches)
		{
			child.Collect(diagonal);
			yield return new WaitForSeconds(GameData.GameSpeed(0.6F));
		}
	}

	public void SetGroundBlocks(int num)
	{
		for(int i = 0; i < GroundBlocks.Length; i++)
		{
			if(i == num) GroundBlocks[i].SetActive(true);
			else GroundBlocks[i].SetActive(false);
		}
	}

	public void SetFillGrid(bool enabled)
	{
		FillGrid = enabled;
	}

	public bool AreNeighbours(Tile a, Tile b, out int [] diff)
	{
		diff = new int [2];
		if(a == null || b == null) return false;
		for(int ai = 0; ai < a.Point.Length; ai++)
		{
			for(int bi = 0; bi < b.Point.Length; bi++)
			{
				diff[0] = a.Point.Point(ai)[0] - b.Point.Point(bi)[0];
				diff[1] = a.Point.Point(ai)[1] - b.Point.Point(bi)[1];
				if(Mathf.Abs(diff[0]) <= 1 && Mathf.Abs(diff[1]) <= 1) return true;
			}
		}
		return false;
		//diff[0] = a.num[0] - b.num[0];
		//diff[1] = a.num[1] - b.num[1];
		//return (Mathf.Abs(diff[0]) <=1 && Mathf.Abs(diff[1]) <= 1);
	}

	public void Ripple(Tile focus, float intensity = 1.0F)
	{
		StartCoroutine(RippleRoutine(focus, 0.0F, intensity));
		for(int x = 0; x < Grid.Size[0]; x++)
		{
			for(int y = 0; y < Grid.Size[1]; y++)
			{
				if(Grid.Tiles[x,y] == focus) continue;

				int [] diff;
				AreNeighbours(Grid.Tiles[x,y], focus, out diff);
				float wait = 0.2F * Mathf.Abs(diff[0]) + 0.2F * Mathf.Abs(diff[1]);
				float inten = 1.0F - (Mathf.Abs(diff[0])*0.15F + Mathf.Abs(diff[1]) * 0.15F) * intensity;
				StartCoroutine(RippleRoutine(Grid.Tiles[x,y], wait, inten));
			}
		}
	}

	IEnumerator RippleRoutine(Tile t, float wait, float inten)
	{
		if(wait!=0.0F)yield return new WaitForSeconds(wait);
		Juice.instance.JuiceIt(Juice.instance.Ripple, t.transform, 1.4F, inten);
		yield return null;
	}
}


public enum ShiftType
{
	Down = 0,
	Left = 1,
	Right = 2,
	Up = 3,
	None = 4
}


public static class ChanceEngine
{
	public static int Index(float [] chances)
	{
		float allchance = 0.0F;
		float currchance = 0.0F;
		float chancevalue = 0.0F;
		for(int i = 0; i < chances.Length; i++)
		{
			allchance += chances[i];
		}
		chancevalue = Random.value * allchance;
		for(int i = 0; i < chances.Length; i++)
		{
			if(chancevalue >= currchance && chancevalue < currchance + chances[i])
			{
				return i;
			}
			currchance += chances[i];
		}
		return 0;
	}
}

public static class Spawner2
{
	private static List<TileInfo> Tiles;
	public static float AllChanceFactors = 0.0F;

	private static List<TileInfo> TilesNoEnemies;
	private static float NoEnemiesChanceFactors = 0.0F;

	public static TileInfo RandomTile()
	{
		float value = Random.value * AllChanceFactors;
		float value_current = 0.0F;
		for(int i = 0; i < Tiles.Count; i++)
		{
			if(value > value_current && value <= value_current + Tiles[i].FinalChance) 
			{
				GameData.Log("Spawning Random Tile " + Tiles[i]._Type + " (" + value + "/" + AllChanceFactors + ")");
				return Tiles[i];
			}
			value_current += Tiles[i].FinalChance;
		}
		return null;
	}

	public static TileInfo RandomTileNoEnemies()
	{
		float value = Random.value * NoEnemiesChanceFactors;
		float value_current = 0.0F;

		for(int i = 0; i < TilesNoEnemies.Count; i++)
		{
			if(value > value_current && value <= value_current + TilesNoEnemies[i].FinalChance) 
			{
				GameData.Log("Spawning Random Tile " + TilesNoEnemies[i]._Type + " (" + value + "/" + AllChanceFactors + ")");
				return TilesNoEnemies[i];
			}
			value_current += TilesNoEnemies[i].FinalChance;
		}
		return null;
		
	}

	public static void GetSpawnables(TileTypes t, Wave w = null)
	{
		Tiles = new List<TileInfo>();
		AllChanceFactors = 0.0F;
		TilesNoEnemies = new List<TileInfo>();
		NoEnemiesChanceFactors = 0.0F;

		//if(w != null) w.IncreaseChances();

		for(int g = 0; g < t.Species.Count; g++)
		{
			SPECIES s = t.Species[g];
			if(s.Chance > 0.0F) 
			{
				for(int i = 0; i < s.AllGenus.Length; i++)
				{
					if(s.AllGenus[i].Chance > 0.0F)
					{
						TileInfo Info = new TileInfo(s, (GENUS)i);
						Tiles.Add(Info);
						AllChanceFactors += Info.FinalChance;
						if(!s.isEnemy)
						{
							NoEnemiesChanceFactors += Info.FinalChance;
							TilesNoEnemies.Add(Info);
						}
					}
				}
			}
			else if(s.ChanceAdded > 0.0F)
			{
				TileInfo Info = new TileInfo(s, GENUS.RAND);
				Info.FinalChance = s.ChanceAdded;
				Tiles.Add(Info);
				AllChanceFactors += Info.FinalChance;
				if(!s.isEnemy)
				{
					NoEnemiesChanceFactors += Info.FinalChance;
					TilesNoEnemies.Add(Info);
				}
			}
		}
	}
}

[System.Serializable]
public class MatchContainer
{
	public bool DiagonalMatch;
	public MatchContainer(bool d)
	{
		Tiles = new List<Tile>();
		DiagonalMatch = d;
	}

	public void Add(Tile t)
	{
		bool add = true;
		foreach(Tile child in Tiles)
		{
			if(t == child) add = false;
		}
		if(add) Tiles.Add(t);
	}
	public void AddRange(MatchContainer m)
	{
		foreach(Tile child in m.Tiles)
		{
			bool add = true;
			foreach(Tile old in Tiles) 
			{
				if(old == child) 
				{
					add = false;
					break;
				}
			}
			if(add) Tiles.Add(child);
		}
	}

	public List<Tile> Tiles;
	public int Size
	{
		get
		{
			return Tiles.Count;
		}
	}
	public bool IsInMatch(Tile t)
	{
		foreach(Tile child in Tiles)
		{
			if(child == t) return true;
		}
		return false;
	}

	public bool IsType(SPECIES s)
	{
		foreach(Tile child in Tiles)
		{
			if(child.IsType(s)) return true;
		}
		return false;
	}

	public bool IsType(string s)
	{
		foreach(Tile child in Tiles)
		{
			if(child.IsType(s)) return true;
		}
		return false;
	}

	public void Collect(bool d)
	{
		if(!d && DiagonalMatch) return;
		foreach(Tile child in Tiles)
		{
			child.Match(1);
		}
	}
}