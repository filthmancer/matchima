using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMaster : MonoBehaviour {
	public static TileMaster instance;
	public static TileTypes Types;
	public static GenusTypes Genus;
	[SerializeField] private GenusTypes _GenusTypes;

	public static GridInfo Grid;
	public static bool FillGridActive
	{
		get {return instance.FillGrid;}
	}
	public static bool GridSetup
	{
		get {
			if (Grid == null) return false;
			else return Grid.setup;
		}
	}
	public static Tile[,] Tiles
	{
		get {return TileMaster.Grid.Tiles;}
	}

	public static Tile RandomTile
	{
		get
		{
			int x = Random.Range(0, Grid.Size[0]);
			int y = Random.Range(0, Grid.Size[1]);
			return Tiles[x,y];
		}
	}

	private static float YScale
	{
		get {
			return 1.0F + (float)(TileMaster.Grid.Size[1] - 6) * 0.14F;
		}
	}

	private static float XScale
	{
		get {
			return 1.0F + (float)(TileMaster.Grid.Size[0] - 5) * 0.14F;
		}
	}

	public Tile this[int x, int y]
	{
		get {
			return TileMaster.Grid.Tiles[x, y];
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
	public static Tile [] Enemies
	{
		get
		{
			List<Tile> final = new List<Tile>();
			for (int x = 0; x < Tiles.GetLength(0); x++)
			{
				for (int y = 0; y < Tiles.GetLength(1); y++)
				{
					if (Tiles[x, y] == null) continue;
					if (Tiles[x, y].Stats.isEnemy && !Tiles[x,y].Stats.isAlly) 
					{
						if(!final.Contains(Tiles[x,y])) final.Add(Tiles[x,y]);
					}
				}
			}
			return final.ToArray();
		}
	}

	public static Tile [] Allies
	{
		get
		{
			List<Tile> final = new List<Tile>();
			for (int x = 0; x < Tiles.GetLength(0); x++)
			{
				for (int y = 0; y < Tiles.GetLength(1); y++)
				{
					if (Tiles[x, y] == null) continue;
					if (Tiles[x, y].Stats.isAlly && !Tiles[x,y].Stats.isEnemy) 
					{
						if(!final.Contains(Tiles[x,y])) final.Add(Tiles[x,y]);
					}
				}
			}
			return final.ToArray();
		}
	}


	public static bool EnemiesAttacking()
	{
		for (int x = 0; x < Tiles.GetLength(0); x++)
		{
			for (int y = 0; y < Tiles.GetLength(1); y++)
			{
				if (Tiles[x, y] == null) continue;
				if (Tiles[x, y].Stats.isEnemy || Tiles[x,y].Stats.isAlly) 
				{
					if((Tiles[x,y] as Enemy).isAttacking) 
					{
						Debug.LogWarning("ENEMIES ATTACKING");
						return true;
					}
				}
			}
		}
		return false;
	}

	public Vector2 MapSize;
	[HideInInspector]
	public Vector2 MapSize_Default;
	public float tileBufferX = 0.2F, tileBufferY = 0.2F;
	public float YOffset = 1.0F;

	public GameObject ResMiniTile;

	[HideInInspector]
	public GameObject TileBasic;
	public GameObject [] GroundBlocks;

	public bool scaleup;

	public Sprite KillEffect;
	public MiniTile2 MiniTileObj;

	public bool generated = false;
	public List<SPECIES> QueuedTiles = new List<SPECIES>();
	public List<GENUS> QueuedGenus = new List<GENUS>();
	public List<int> QueuedValue = new List<int>();

	private float [] spawn_stack;
	private bool first_enemy = false;
	[SerializeField]
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
			if (Tiles == null) return false;
			for (int x = 0; x < Tiles.GetLength(0); x++)
			{
				for (int y = 0; y < Tiles.GetLength(1); y++)
				{
					if (Tiles[x, y] == null) { //Debug.LogWarning("NULL " + x + ":" + y);
						continue;
					}
					if (Tiles[x, y].isFalling || Tiles[x, y].UnlockedFromGrid)
					{
						if(Tiles[x,y].Destroyed) continue;
						//Debug.LogWarning("WAITING FOR " + x + ":" + y);
						return false;
					}
				}
			}
			return true;
		}
	}

	public List<MatchContainer> Matches
	{
		get {
			List<MatchContainer> m = new List<MatchContainer>();
			m.AddRange(DiagMatches);
			m.AddRange(StrtMatches);
			return m;
		}
	}
	public List<MatchContainer> DiagMatches;
	public List<MatchContainer> StrtMatches;

	void Awake()
	{
		if (instance == null)
		{
			DontDestroyOnLoad(transform.gameObject);
			instance = this;
		}
		else if (instance != this)
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
		if (GameManager.instance.gameStart)
		{
			if (!generated)
			{
				print("GENERATING LEVEL " + (level_to_load != null));

				ClearQueuedTiles();
				spawn_stack = new float [(int)MapSize.x];
				GenerateGrid(level_to_load);

				Player.instance.ResetChances();
				AllChanceInit = Spawner2.AllChanceFactors;
				AllChanceCurrent = AllChanceInit;
				ChanceRatio = AllChanceCurrent / AllChanceInit;

				generated = true;
			}

			if (FillGrid) ShiftTiles2(Player.Stats.Shift);
		}
	}

	public void IncreaseChance(string g, string t, float amt)
	{

		if (g != string.Empty && t == string.Empty)
		{
			int num = (int) Genus[g];
			Genus.ChancesAdded[num] += amt;
		}
		else if (g == string.Empty && t != string.Empty)
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

	public void Reset()
	{
		ResetChances();
		generated = false;
		FillGrid = false;
		Types.Setup();
		Genus = _GenusTypes;
		EnemyTute = false;
	}

	public void ResetChances()
	{
		Genus.ChancesAdded = new float [] {0.0F,
		                                   0.0F,
		                                   0.0F,
		                                   0.0F,
		                                   0.0F,
		                                   0.0F,
		                                   0.0F
		                                  };

		Types.Setup();
		//Spawner2.GetSpawnables(Types, GameManager.Wave);
	}

	public Tile GetTile(int x, int y)
	{
		if (Tiles.GetLength(0) > x && x >= 0)
		{
			if (Tiles.GetLength(1) > y && y >= 0)
			{
				return Tiles[x, y];
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
		if (clear)
		{
			ClearGrid(true);
			StartCoroutine(_GenerateGrid(level, wait));
		}
	}

	public void SetOrtho()
	{
		float ortho = Mathf.Max(Grid.Size[0] * 1.55F, Grid.Size[1] * 1.55F);
		CameraUtility.TargetOrtho = Mathf.Clamp(ortho, 7, Mathf.Infinity);
	}

//Generates a grid
	private IEnumerator _GenerateGrid(GridInfo level, float wait)
	{
		if (level != null) //If loading a GridInfo file
		{

			MapSize = new Vector2(level.Points.GetLength(0),  level.Points.GetLength(1));
			bool evenX = MapSize[0] % 2 == 0;
			bool evenY = MapSize[1] % 2 == 0;
			float tileBufferX_offset = (float)(evenX ? (1.0F + tileBufferX) / 2 : 0);
			float tileBufferY_offset = (float)(evenY ? (1.0F + tileBufferY) / 2 : 0);

			print(level.Points[0, 0].Info._TypeName);
			Grid = new GridInfo();
			Grid.SetUp(MapSize);
			Grid.SetInfo(level);
			//print(level.Points[0,0].Info + " : " + Grid.Points[0,0].Info);

			int [] tile_size = new int [2] {(int)Grid.Size[0], (int)Grid.Size[1]};


			SetOrtho();

			CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0, 0].position,
			                           Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].position,
			                           0.5F));
			spawn_stack = new float [Grid.Size[0]];
			for (int sx = 0; sx < Grid.Size[0]; sx++)
			{
				if (Player.Stats.Shift == ShiftType.Down) spawn_stack[sx] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
				else if (Player.Stats.Shift == ShiftType.Up) spawn_stack[sx] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
			}

			int x = 0, y = 0;
			for (int xx = -tile_size[0] / 2; xx < (evenX ? tile_size[0] / 2 : tile_size[0] / 2 + 1); xx++)
			{
				y = 0;
				for (int yy = -tile_size[1] / 2; yy < (evenY ? tile_size[1] / 2 : tile_size[1] / 2 + 1); yy++)
				{
					if (Grid.Points[x, y].Info != null) CreateTile(x, y, new Vector2(0, 1), Grid.Points[x, y].Info);
					//else print(x + ":" + y + " -- " + Grid.Points[x,y]);
					y++;
				}
				x++;
				yield return null;
			}
		}
		else //Create random grid
		{
			if (wait != 0.0F) yield return new WaitForSeconds(wait);
			MapSize = MapSize_Default;

			bool evenX = MapSize[0] % 2 == 0;
			bool evenY = MapSize[1] % 2 == 0;
			float tileBufferX_offset = (float)(evenX ? (1.0F + tileBufferX) / 2 : 0);
			float tileBufferY_offset = (float)(evenY ? (1.0F + tileBufferY) / 2 : 0);

			Grid = new GridInfo();
			Grid.SetUp(MapSize);

			SetOrtho();

			CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0, 0].position,
			                           Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].position,
			                           0.5F));

			spawn_stack = new float [Grid.Size[0]];
			for (int x = 0; x < Grid.Size[0]; x++)
			{
				if (Player.Stats.Shift == ShiftType.Down) spawn_stack[x] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
				else if (Player.Stats.Shift == ShiftType.Up) spawn_stack[x] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
			}
			Vector2 velocity = Vector2.zero;
			if (Player.Stats.Shift == ShiftType.Down) velocity = new Vector2(0, 1);
			else if (Player.Stats.Shift == ShiftType.Up) velocity = new Vector2(0, -1);
			for (int xx = 0; xx < Grid.Size[0]; xx++)
			{
				for (int yy = 0; yy < Grid.Size[1]; yy++)
				{
					CreateTile(xx, yy, velocity);
				}
				yield return null;
			}
		}
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Grid[x, y]._Tile == null) continue;
				if (Tiles[x, y] != null || Tiles[x, y].Type == null)
				{
					Tiles[x, y].InitStats.isNew = false;
				}
			}
		}
	}

	public void CheckGrid()
	{
		Vector2 finalMap = TileMaster.instance.MapSize_Default
		                   + Player.Stats.MapSize;

		if(MapSize != finalMap)
		{
			IncreaseGridTo(finalMap);
		}
	}

	public void IncreaseGridTo(Vector2 final)
	{
		GameData.Log("Changing Grid by " + final.x + ":" + final.y);
		Vector2 diff = final - MapSize;
		if (diff == Vector2.zero || Grid == null) return;
		Grid.ChangeBy(diff);

		MapSize = final;
		SetOrtho();
		//print(Grid.Size[0] + ":" + Grid.Points.GetLength(0) + " -- " + Grid.Size[1] + ":" + Grid.Points.GetLength(1));
		CameraUtility.SetTargetPos(Vector3.Lerp(Grid[0, 0].position,
		                                        Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].position,
		                                        0.5F));

		spawn_stack = new float [Grid.Size[0]];
		for (int sx = 0; sx < Grid.Size[0]; sx++)
		{
			if (Player.Stats.Shift == ShiftType.Down) spawn_stack[sx] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
			else if (Player.Stats.Shift == ShiftType.Up) spawn_stack[sx] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
		}
		FillGrid = true;
	}

	public void IncreaseGrid(Vector2 diff)
	{
		IncreaseGrid((int)diff.x, (int)diff.y);
	}

	public void IncreaseGrid(int x, int y)
	{
		GameData.Log("Changing Grid by " + x + ":" + y);
		MapSize += new Vector2(x, y);

		Grid.ChangeBy(new Vector2(x, y));

		SetOrtho();

		CameraUtility.SetTargetPos(Vector3.Lerp(Grid[0, 0].position,
		                                        Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].position,
		                                        0.5F));

		spawn_stack = new float [Grid.Size[0]];
		for (int sx = 0; sx < Grid.Size[0]; sx++)
		{
			if (Player.Stats.Shift == ShiftType.Down) spawn_stack[sx] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
			else if (Player.Stats.Shift == ShiftType.Up) spawn_stack[sx] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
		}
		FillGrid = true;
	}

	private IEnumerator _IncreaseGrid(int x, int y)
	{
		MapSize += new Vector2(x, y);

		Grid.ChangeBy(new Vector2(x, y));

		yield return null;

		SetOrtho();

		CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0, 0].position,
		                           Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].position,
		                           0.5F));

		spawn_stack = new float [Grid.Size[0]];
		for (int sx = 0; sx < Grid.Size[0]; sx++)
		{
			if (Player.Stats.Shift == ShiftType.Down) spawn_stack[sx] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
			else if (Player.Stats.Shift == ShiftType.Up) spawn_stack[sx] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
		}
		FillGrid = true;
		yield return null;
	}


	private Tile CreateTile(int x, int y, Vector2 velocity, TileInfo t, int scale = 1, int value = 0)
	{
		//???? WAHT IS DIS
		//if(Tiles.GetLength(0)>x && Tiles.GetLength(1) > y)
		//{
		//	if(Tiles[x,y] != null) return null;
		//}
		//????

		if (t == null) return null;

		if (t.Inner == null) t.Inner = t._Type.Atlas;
		if (t.Outer == null) t.Outer = TileMaster.Genus.Frames.GetSpriteIdByName(t._GenusName);
		Tile new_tile = t._Type.TilePool.Spawn();
		float ny = velocity.y == 0.0F ? Grid.GetPoint(x, y).y : spawn_stack[x];
		float nx = velocity.x == 0.0F ? Grid.GetPoint(x, y).x : spawn_stack[y];

		new_tile.transform.position = new Vector3(nx, ny, 0);
		new_tile.name = "Tile | " + x + ":" + y;

		new_tile.transform.parent = Grid.Column[x].transform;
		for (int xx = 0; xx < scale; xx++)
		{
			for (int yy = 0; yy < scale; yy++)
			{
				Grid[x + xx, y + yy]._Tile = new_tile;
			}
		}

		Grid[x, y]._Tile.Setup(x, y, scale, t, value);

		if (GameManager.instance.EnemyTurn) Grid[x, y]._Tile.SetState(TileState.Locked);
		else Grid[x, y]._Tile.SetState(TileState.Idle);


		float half_x = (tileBufferX) / 2;
		float half_y = (tileBufferY) / 2;
		SphereCollider tile_col = new_tile.GetComponent<SphereCollider>();
		tile_col.radius = 0.5F + half_x;

		if (velocity.y > 0.0F) spawn_stack[x] += tile_col.radius * 2;
		else if (velocity.y < 0.0F) spawn_stack[x] -= tile_col.radius * 2;
		else if (velocity.x > 0.0F) spawn_stack[y] += tile_col.radius * 2;
		else if (velocity.x < 0.0F) spawn_stack[y] -= tile_col.radius * 2;
		return Grid[x, y]._Tile;
	}

	private Tile CreateTile(int x, int y, Vector2 velocity, SPECIES s = null, GENUS g = GENUS.NONE, bool no_enemies = false, int scale = 1, int value = 0)
	{
		TileInfo t = null;
		if (s == null || s == SPECIES.None)
		{
			if (no_enemies) t = Spawner2.RandomTileNoEnemies();
			else t = Spawner2.RandomTile();

			if (g != GENUS.NONE) t.ChangeGenus(g);
			if (t._GenusEnum == GENUS.RAND)
			{
				t._GenusEnum = (GENUS) Random.Range(0, 3);
			}
		}
		else
		{
			t = new TileInfo(s, g);
		}

		return CreateTile(x, y, velocity, t, scale, value);
	}

	public Tile ReplaceTile(int x, int y, SPECIES sp = null, GENUS g = GENUS.NONE, int newscale = 1, int addvalue = 0, bool override_old = false)
	{
		while (x + newscale > Grid.Size[0] || y + newscale > Grid.Size[1])
		{
			if (x + newscale > Grid.Size[0]) x--;
			if (y + newscale > Grid.Size[1]) y--;
		}
		int tempval = 0;
		Tile newtile = null;

		for (int xx = 0; xx < newscale; xx++)
		{
			for (int yy = 0; yy < newscale; yy++)
			{
				if (Grid[x + xx, y + yy]._Tile != null)
				{
					tempval +=  override_old ? 0 : Grid[x + xx, y + yy]._Tile.Stats.Value - 1;
					if (!override_old && !Grid[x + xx, y + yy]._Tile.Destroyed) Grid[x + xx, y + yy]._Tile.DestroyThyself();
				}

			}
		}
		newtile = CreateTile(x, y, Vector2.zero, sp, g, false, newscale, addvalue);
		EffectManager.instance.PlayEffect(newtile.transform, Effect.Replace, GameData.instance.GetGENUSColour(newtile.Genus));
		newtile.InitStats.Value += tempval;
		newtile.PlayAudio("replace",0.6F);
		return newtile;
	}

	public Tile ReplaceTile(Tile t, SPECIES sp = null, GENUS g = GENUS.NONE, int newscale = 1, int addvalue = 0)
	{
		if (t == null) return null;
		return ReplaceTile(t.Point.Base[0], t.Point.Base[1], sp, g, newscale, addvalue);
	}

	public void SwapTiles(Tile a, Tile b)
	{
		//Grid[a.Point.Base[0], a.Point.Base[1]]._Tile = b;
		//Grid[b.Point.Base[0], b.Point.Base[1]]._Tile = a;
		int [] a_point = a.Point.Base;
		int [] b_point = b.Point.Base;
		
		a.MoveToGridPoint(b_point[0], b_point[1], 0.25F);
		b.MoveToGridPoint(a_point[0], a_point[1], -0.25F);
		//a.Setup(b_point[0], b_point[1]);
		//b.Setup(a_point[0], a_point[1]);
	}

	public void AddVelocity(Tile t)
	{

	}


	public void AddVelocityToColumn(int x, int y, float amount)
	{
		return;
		if (Tiles == null) return;
		for (int yy = 0; yy < Grid.Size[1]; yy++)
		{
			if (yy <= y || Tiles.GetLength(0) <= x || Tiles.GetLength(1) <= yy) continue;
			if (Tiles[x, yy] == null || Tiles[x, yy].Stats.Shift == ShiftType.None) continue;
			Tiles[x, yy].speed += amount;
		}
	}


	public void ClearGrid(bool destroy = false)
	{
		if (Grid == null) return;
		if (Tiles == null || Tiles.GetLength(0) == 0) return;
		FillGrid = false;
		for (int x = 0; x < Grid.Points.GetLength(0); x++)
		{
			for (int y = 0; y < Grid.Points.GetLength(1); y++)
			{
				if (Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x, y] != null)
				{
					Tiles[x, y].DestroyThyself(destroy);
					Grid[x, y]._Tile = null;
				}
			}
		}

		if (!destroy) return;
		if (Grid != null) Grid.DestroyThyself();
	}

	public void NewGrid(SPECIES sp = null, GENUS g = GENUS.NONE, bool no_enemies = false)
	{
		if (Grid == null) return;
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x, y] != null)
				{
					Tiles[x, y].DestroyThyself(); // true);
					//Destroy(Tiles[x,y].gameObject);
					Grid[x, y]._Tile = null;
					CreateTile(x, y, Vector2.up, sp, g, no_enemies);
				}
			}
		}
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Grid[x, y]._Tile == null) continue;
				if (Tiles[x, y] != null || Tiles[x, y].Type == null)
				{
					Tiles[x, y].InitStats.isNew = false;
				}
			}
		}
	}

	public IEnumerator NewGridRoutine(SPECIES sp = null, GENUS g = GENUS.NONE, bool no_enemies = false)
	{
		if (Grid == null) yield break;
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x, y] != null)
				{
					Tiles[x, y].DestroyThyself(); // true);
					//Destroy(Tiles[x,y].gameObject);
					Grid[x, y]._Tile = null;
					CreateTile(x, y, Vector2.up, sp, g, no_enemies);
				}
			}
		}
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Grid[x, y]._Tile == null) continue;
				if (Tiles[x, y] != null || Tiles[x, y].Type == null)
				{
					Tiles[x, y].InitStats.isNew = false;
				}
			}
		}
		yield return StartCoroutine(BeforeTurn());
		yield return StartCoroutine(AfterTurn());
	}

	private bool EnemyTute = false;
	public IEnumerator BeforeTurn()
	{
		while (!AllLanded)	yield return null;

		EnemiesOnScreen = 0;
		bool show_enemy_tute = false;
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Grid[x, y]._Tile == null) continue;
				if (Tiles[x, y] != null || Tiles[x, y].Type == null)
				{
					Tiles[x, y].AfterTurnCheck = false;
					if (Tiles[x, y].BeforeTurnEffect)
						yield return StartCoroutine(Tiles[x, y].BeforeTurnRoutine());
					else Tiles[x, y].BeforeTurn();
					if (Tiles[x, y] == null || Tiles[x, y].Type == null) continue;
					if (Tiles[x, y].Type.isEnemy) 
					{
						EnemiesOnScreen ++;
						if(!EnemyTute)
						{
							show_enemy_tute = true;
							EnemyTute = true;
						} 
					}
				}
			}
		}
		if(show_enemy_tute)
		//yield return StartCoroutine(UIManager.instance.Alert(0.3F, "Monster tiles attack you", "Match them to attack back!", "", true));

		yield break;
	}

	public IEnumerator AfterTurn()
	{
		if (Player.Stats.isKilled) yield break;
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] != null)
				{
					if (Tiles[xx, yy].Destroyed)
					{
						DestroyTile(Tiles[xx, yy]);
					}
				}
			}
		}
		while (!AllLanded)
		{
			if (Player.Stats.isKilled) yield break;
			yield return null;
		}

		//BASICALLY THE JEWELLER PASSIVE, WAS JUST TESTING
		/*
		CheckForMatches();
		while(StrtMatches.Count > 0)
		{
			CheckForMatches();
			yield return StartCoroutine(CollectMatchesRoutine(false));

			spawn_stack = new float [Grid.Size[0]];
			for(int x = 0; x < Grid.Size[0]; x++)
			{
				if(Player.Stats.Shift == ShiftType.Down) spawn_stack[x] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
				else if(Player.Stats.Shift == ShiftType.Up) spawn_stack[x] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
			}

			while(!AllLanded)
			{
				if(Player.Stats.isKilled) yield break;
				yield return null;
			}
		}
		*/
		//END

		EnemiesOnScreen = 0;
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] != null)
				{
					if (Tiles[xx, yy].isMatching)
					{
						Tiles[xx, yy].DestroyThyself();
						continue;
					}
					if (Tiles[xx, yy].AfterTurnCheck) continue;
					Tiles[xx, yy].AfterTurnCheck = true;
					if (Tiles[xx, yy].Type.isEnemy) EnemiesOnScreen ++;

					Tiles[xx, yy].AfterTurn();
					if (Tiles[xx, yy].HasAfterTurnEffect())
						yield return StartCoroutine(Tiles[xx, yy].AfterTurnRoutine());
				}
				//else if(fill_from_none[xx, yy]) ReplaceTile(xx,yy);
			}
		}

		spawn_stack = new float [Grid.Size[0]];
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			if (Player.Stats.Shift == ShiftType.Down) spawn_stack[x] = CameraUtility.instance.Cam.CameraSettings.orthographicSize + 2.0F;
			else if (Player.Stats.Shift == ShiftType.Up) spawn_stack[x] = -(CameraUtility.instance.Cam.CameraSettings.orthographicSize);
		}

		ClearQueuedTiles();
		ResetTiles();
		CheckForEmptys();
		if (!FillGrid) yield break;

		if (!TileMaster.instance.CheckForMatches())
		{
			yield return new WaitForSeconds(0.2F);
			NewGrid();
		}

		yield break;
	}

	public void CheckForEmptys()
	{
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] == null && fill_from_none[xx, yy]) ReplaceTile(xx, yy);
			}
		}
	}

	public void SetAllTileStates(TileState s, bool _override = false)
	{

		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] != null) Tiles[xx, yy].SetState(s, _override);
			}
		}
	}

	public void ResetTiles(bool idle = false)
	{
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] != null) Tiles[xx, yy].Reset(idle);
			}
		}
	}

	public void DestroyTile(int x, int y)
	{
		if (Tiles[x, y] != null && Tiles[x, y].gameObject != null)
		{
			Player.instance.OnTileDestroy(Tiles[x, y]);
			int scale = Grid[x, y]._Tile.Point.Scale;
			for (int xx = 0; xx < scale; xx++)
			{
				for (int yy = 0; yy < scale; yy++)
				{
					if (Grid[Grid[x, y]._Tile.Point.Base[0] + xx, Grid[x, y]._Tile.Point.Base[1] + yy]._Tile == Grid[x, y]._Tile)
					{
						Grid[Grid[x, y]._Tile.Point.Base[0] + xx, Grid[x, y]._Tile.Point.Base[1] + yy]._Tile = null;
					}
				}
			}

			Tiles[x,y].ClearActions();
			Tiles[x,y].ClearEffects();

			Tiles[x, y].Info._Type.TilePool.Unspawn(Tiles[x, y]);
		}
	}

	public void DestroyTile(Tile t)
	{
		Player.instance.OnTileDestroy(t);
		for (int xx = 0; xx < t.Point.Scale; xx++)
		{
			for (int yy = 0; yy < t.Point.Scale; yy++)
			{
				if (Grid[t.Point.Base[0] + xx, t.Point.Base[1] + yy]._Tile == t)
				{
					Grid[t.Point.Base[0] + xx, t.Point.Base[1] + yy]._Tile = null;
				}
			}
		}
		
		t.ClearActions();
		t.ClearEffects();
		t.Info._Type.TilePool.Unspawn(t);
		if (Player.Stats.Shift == ShiftType.None) ReplaceTile(t.Point.Base[0], t.Point.Base[1]);
	}

	public void CollectTile(Tile t, bool destroy)
	{
		t.CheckStats();
		CollectTileResource(t, destroy);
		Player.instance.OnTileCollect(t);
	}

	int [] points = new int [4];
	private void CollectTileResource(Tile t, bool destroy)
	{
		ParticleSystem col = EffectManager.instance.PlayEffect(t.transform, Effect.Destroy, GameData.instance.GetGENUSColour(t.Genus)).GetComponent<ParticleSystem>();
		int combo = GameManager.ComboSize;
		if (combo < 10)
			col.startColor = Color.Lerp(Color.black, GameData.Colour(t.Genus), 0.4F + (float)combo / 8.0F);
		else
			col.startColor = Color.Lerp(GameData.Colour(t.Genus), Color.white, (float)(combo - 10) / 20.0F);
		col.startSize = Mathf.Clamp(0.1F + (float)combo / 25, 0.2F, 0.5F);
		col.transform.SetParent(this.transform);

		Vector3 startpos = t.transform.position;
		startpos.z = -8;
		float init_size = Random.Range(80, 120);
		float init_rotation = Random.Range(-7, 7);

		float info_time = 0.4F;
		float info_start_size = init_size + (t.Stats.Value * 5);
		float info_movespeed = 0.4F;
		float info_finalscale = 0.35F;

		MoveToPoint mini;

		int [] values = t.Stats.GetValues();
		int g = (int) t.Genus;
		if (values[0] > 0)
		{
			Vector3 pos = Grid.GetPoint(t.Point.Point(0)) + Vector3.down * 0.3F;
			MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + values[0], info_start_size, GameData.Colour(t.Genus), info_time, 0.04F, false);
			if(m)
			{
				m.transform.rotation = Quaternion.Euler(0, 0, init_rotation);
				m.AddJuice(Juice.instance.BounceB, info_time);
				m.DestroyOnEnd = true;
			}
			
			UIManager.instance.GetMeterPoints(g, values[0]);
			

		}
		if (values[1] > 0)
		{
			info_time *= 1.4F;
			Vector3 pos = Grid.GetPoint(t.Point.Point(0)) + Vector3.down * 0.3F;
			MiniAlertUI m = UIManager.instance.MiniAlert(pos, values[1] + "%\nHP" , info_start_size * 0.55F, GameData.instance.GoodColour, info_time, 0.04F, false);
			if(m)
			{
				m.Txt[0].outlineColor = GameData.instance.GoodColourFill;
				m.transform.rotation = Quaternion.Euler(0, 0, init_rotation);
				mini = m.GetComponent<MoveToPoint>();
				m.AddJuice(Juice.instance.BounceB, info_time);
				m.AddAction(() => {mini.enabled = true;});
				m.DestroyOnEnd = true;
			}
			
			Player.Stats.Heal(values[1]);
		}
		if (values[2] > 0)
		{
			info_time *= 1.4F;
			Vector3 pos = Grid.GetPoint(t.Point.Point(0)) + Vector3.down * 0.3F;
			MiniAlertUI m = UIManager.instance.MiniAlert(pos, values[2] + "%\nHP" , info_start_size * 0.55F, GameData.instance.GoodColour, info_time, 0.04F, false);
			if(m)
			{
				m.Txt[0].outlineColor = GameData.instance.GoodColourFill;
				m.transform.rotation = Quaternion.Euler(0, 0, init_rotation);
				mini = m.GetComponent<MoveToPoint>();
				m.AddJuice(Juice.instance.BounceB, info_time);
				m.AddAction(() => {mini.enabled = true;});
				m.DestroyOnEnd = true;
			}
		}
		if (destroy) DestroyTile(t);
	}

	public MoveToPoint CreateMiniTile(Vector3 pos, Transform target, Sprite sprite = null)
	{
		if (sprite == null)
		{
			return null;
			int num = Random.Range(0, 4);
			//sprite = Genus.Frame[num];
		}

		GameObject new_mini = Instantiate(ResMiniTile);
		new_mini.transform.position = pos;
		new_mini.GetComponent<SpriteRenderer>().sprite = sprite;

		MoveToPoint mover = new_mini.GetComponent<MoveToPoint>();
		mover.SetTarget(target.position);
		//mover.SetPath(speed, arc, lerp);
		return mover;
	}

	public void QueueTilesSpawnOnStart(SPECIES spec, GENUS g, int num)
	{
		
	}

	public void QueueTile(SPECIES spec, GENUS g = GENUS.NONE, int value = 0)
	{
		QueuedTiles.Add(spec);
		QueuedGenus.Add(g);
		QueuedValue.Add(value);
	}

	public void ClearQueuedTiles()
	{
		QueuedTiles.Clear();
		QueuedGenus.Clear();
		QueuedValue.Clear();
	}

	private void CreateQueuedTiles(ref int tile_num, ref int? [] tile_start, Vector2 velocity)
	{
		if (QueuedTiles == null || QueuedTiles.Count == 0)
		{
			tile_num = tile_num;
			tile_start = tile_start;
			return;
		}

		GameData.Log("Generating " + QueuedTiles.Count + " queued tiles");
		int remove = 0;
		for (int i = 0; i < QueuedTiles.Count; i++)
		{
			if (tile_num <= 0) continue;

			List<int> poss_x = new List<int>();
			for (int _x = 0; _x < Grid.Size[0]; _x++)
			{
				if (tile_start[_x] != null)	poss_x.Add(_x);
			}

			int rand = Random.Range(0, poss_x.Count - 1);
			int x = poss_x[rand];
			int y = (int) tile_start[x];

			GameData.Log("Queued tile " + i + " generated at " + x + ":" + y);

			CreateTile(x, y, velocity, QueuedTiles[i], QueuedGenus[i]);
			Tiles[x, y].InitStats.Value += QueuedValue[i];

			if (tile_start[x] < Grid.Size[1] - 1)
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
		QueuedTiles.RemoveRange(0, remove);
		QueuedGenus.RemoveRange(0, remove);
		QueuedValue.RemoveRange(0, remove);
	}

	private void ShiftTiles2(ShiftType type)
	{
		fill_from_none = new bool[(int)Grid.Size[0], (int)Grid.Size[1]];

		bool [,] ignore = new bool[(int)Grid.Size[0], (int)Grid.Size[1]];

		if (type == ShiftType.None)
		{
			for (int x = 0; x < Grid.Size[0]; x++)
			{
				for (int y = 0; y < Grid.Size[1]; y++)
				{
					if (Tiles[x, y] == null) fill_from_none[x, y] = true;
				}
			}
		}
		else if (type == ShiftType.Down)
		{
			int? empty = null;
			for (int x = 0; x < Grid.Size[0]; x++)
			{
				empty = null;
				for (int y = 0; y < Grid.Size[1]; y++)
				{

					//IF THERE IS A PROBLEM WITH GIANTS, ITS PROBABLY CAUSE I TURNED THIS OFF
					//if(ignore[x,y])continue;

					//INITAL SETTING OF THE BOTTOM EMPTY TILE OF A ROW
					if (Tiles[x, y] == null && !empty.HasValue)
					{
						empty = y;
						if (y < Grid.Size[1] - 1) fill_from_none[x, y] = true;
					}

					//IF THE TILE ABOVE AN EMPTY SPOT ISN'T NULL
					else if (empty.HasValue && Tiles[x, y] != null)
					{
						if (Tiles[x, y].Stats.Shift != ShiftType.None)
						{

							//IF TILE ABOVE IS GIANT WITH BASE ELSWHERE, CONTINUE
							if (Tiles[x, y].Point.BaseX != x)
							{
								fill_from_none[x, empty.Value] = true;
								empty++;
								continue;
							}

							//IF NO SPACE FOR TILE TO FALL, CONTINUE
							bool nospace = false;
							for (int xx = 0; xx < Tiles[x, y].Point.Scale; xx++)
							{
								for (int yy = 0; yy < Tiles[x, y].Point.Scale; yy++)
								{
									if (x + xx > Grid.Size[0] - 1 || y + yy > Grid.Size[1] - 1)
									{
										fill_from_none[x, empty.Value] = true;
										nospace = true;
										break;
									}

									nospace = Tiles[x + xx, y] != null && Tiles[x + xx, y] != Tiles[x, y];
									nospace = Tiles[x + xx, y + yy] != null && Tiles[x + xx, y + yy] != Tiles[x, y];

									if (nospace) break;
								}
								if (nospace)
								{
									fill_from_none[x + xx, empty.Value] = true;
									break;
								}
								else
								{
									for (int i = empty.Value; i < y; i++)
									{
										if (Tiles[x + xx, i] != null)
										{
											fill_from_none[x + xx, empty.Value] = true;
											nospace = true;
											break;
										}
									}
								}
							}
							if (nospace)
							{
								//fill_from_none[x, empty.Value] = true;
								continue;
							}

							//SET TILE POINTS
							Grid[x, empty.Value]._Tile = Tiles[x, y];
							Grid[x, empty.Value]._Tile.Point.BaseX = x;
							Grid[x, empty.Value]._Tile.Point.BaseY = empty.Value;
							Grid[x, empty.Value]._Tile.Point.SetPoints();
							Grid[x, y]._Tile = null;

							int scale = Grid[x, empty.Value]._Tile.Point.Scale;
							for (int xx = 0; xx < scale; xx++)
							{
								for (int yy = 0; yy < scale; yy++)
								{
									Grid[x + xx, y + yy]._Tile = null;
									if (x + xx > Grid.Size[0] - 1 || empty.Value + yy > Grid.Size[1] - 1) continue;
									Grid[x + xx, empty.Value + yy]._Tile = Grid[x, empty.Value]._Tile;
									ignore[x + xx, empty.Value + yy] = true;

								}
								//if(y + scale != y) Grid[x+xx,y+scale]._Tile = null;
							}

							//ANTIGRAV CHECK

							fill_from_none[x, empty.Value] = false;
							if (y < Grid.Size[1] - 1) fill_from_none[x, y] = true;
							empty += scale;

						}
						else
						{
							//fill_from_none[x,empty.Value] = true;
							empty = null;
						}
					}
					//IF THE TILE ABOVE AN EMPTY SPOT IS NULL
					else if (Tiles[x, y] == null)
					{
						if (y < Grid.Size[1] - 1) fill_from_none[x, y] = true;
						empty = y;
					}
				}
			}

			int new_tiles_num = 0;
			int? [] new_tiles_start = new int? [Grid.Size[0]];
			for (int x = 0; x < Grid.Size[0]; x++)
			{
				new_tiles_start[x] = null;
				for (int y = 0; y < Grid.Size[1]; y++)
				{

					if (Tiles[x, y] == null && !fill_from_none[x, y])
					{
						new_tiles_num++;
						if (new_tiles_start[x] == null) new_tiles_start[x] = y;
					}
				}
			}

			if (new_tiles_num != 0) {
				CreateQueuedTiles(ref new_tiles_num, ref new_tiles_start, Vector2.up);
				for (int x = 0; x < Grid.Size[0]; x++)
				{
					if (new_tiles_start[x] == null) continue;

					for (int _y = (int) new_tiles_start[x]; _y < Grid.Size[1]; _y++)
					{
						if (Tiles[x, new_tiles_start[x].Value] != null) continue;

						//INSERT STUFF FOR FALLING BIG TILES HERE
						int scale = 1;

						if (!fill_from_none[x, _y]) //??? new_tiles_start[x].Value????
						{
							CreateTile(x, _y, Vector2.up, null, GENUS.NONE, false, scale);
						}
					}
				}
			}
		}
		else if (type == ShiftType.Up)
		{
			int? empty = null;
			for (int x = Grid.Size[0] - 1; x >= 0; x--)
			{
				empty = null;
				for (int y = Grid.Size[1] - 1; y >= 0; y--)
				{

					//IF THERE IS A PROBLEM WITH GIANTS, ITS PROBABLY CAUSE I TURNED THIS OFF
					//if(ignore[x,y])continue;

					//INITAL SETTING OF THE BOTTOM EMPTY TILE OF A ROW
					if (Tiles[x, y] == null && !empty.HasValue)
					{
						empty = y;
						if (y > 0) fill_from_none[x, y] = true;
					}

					//IF THE TILE ABOVE AN EMPTY SPOT ISN'T NULL
					else if (empty.HasValue && Tiles[x, y] != null)
					{

						if (Tiles[x, y].Stats.Shift != ShiftType.None)
						{

							//IF TILE ABOVE IS GIANT WITH BASE ELSWHERE, CONTINUE
							if (Tiles[x, y].Point.BaseX != x)
							{
								fill_from_none[x, empty.Value] = true;
								empty--;
								continue;
							}

							//IF NO SPACE FOR TILE TO FALL, CONTINUE
							bool nospace = false;
							for (int xx = 0; xx < Tiles[x, y].Point.Scale; xx++)
							{
								for (int yy = 0; yy < Tiles[x, y].Point.Scale; yy++)
								{
									if (x - xx < 0 || y - yy < 0)
									{
										fill_from_none[x, empty.Value] = true;
										nospace = true;
										break;
									}

									nospace = Tiles[x - xx, y] != null && Tiles[x - xx, y] != Tiles[x, y];
									nospace = Tiles[x - xx, y - yy] != null && Tiles[x - xx, y - yy] != Tiles[x, y];

									if (nospace) break;
								}
								if (nospace)
								{
									fill_from_none[x - xx, empty.Value] = true;
									break;
								}
								else
								{
									for (int i = empty.Value; i < y; i--)
									{
										if (Tiles[x - xx, i] != null)
										{
											fill_from_none[x - xx, empty.Value] = true;
											nospace = true;
											break;
										}
									}
								}
							}
							if (nospace)
							{
								//fill_from_none[x, empty.Value] = true;
								continue;
							}

							//SET TILE POINTS
							if (x == 0) print(empty.Value);
							Grid[x, empty.Value]._Tile = Tiles[x, y];
							Grid[x, empty.Value]._Tile.Point.BaseX = x;
							Grid[x, empty.Value]._Tile.Point.BaseY = empty.Value;
							Grid[x, empty.Value]._Tile.Point.SetPoints();
							Grid[x, y]._Tile = null;


							int scale = Grid[x, empty.Value]._Tile.Point.Scale;
							for (int xx = 0; xx < scale; xx++)
							{
								for (int yy = 0; yy < scale; yy++)
								{
									if (x - xx < 0 || y - yy < 0) continue;
									Grid[x - xx, y - yy]._Tile = null;
									if (empty.Value - yy < 0) continue;

									Grid[x - xx, empty.Value - yy]._Tile = Grid[x, empty.Value]._Tile;
									//ignore[x-xx,empty.Value-yy] = true;

								}
								//if(y + scale != y) Grid[x+xx,y+scale]._Tile = null;
							}

							//ANTIGRAV CHECK
							//fill_from_none[x,empty.Value] = false;
							if (y > 0) fill_from_none[x, y] = true;
							empty -= scale;

							//continue;



						}
						else
						{
							//fill_from_none[x,empty.Value] = true;
							empty = null;
						}
					}
					//IF THE TILE ABOVE AN EMPTY SPOT IS NULL
					else if (Tiles[x, y] == null)
					{
						if (y > 0) fill_from_none[x, y] = true;
						empty = y;
					}
				}
			}


			int new_tiles_num = 0;
			int? [] new_tiles_start = new int? [Grid.Size[0]];
			for (int x = Grid.Size[0] - 1; x >= 0 ; x--)
			{
				new_tiles_start[x] = null;
				for (int y = Grid.Size[1] - 1; y >= 0; y--)
				{

					if (Tiles[x, y] == null && !fill_from_none[x, y])
					{
						new_tiles_num++;
						if (new_tiles_start[x] == null) new_tiles_start[x] = y;
					}
				}
			}

			if (new_tiles_num != 0) {
				CreateQueuedTiles(ref new_tiles_num, ref new_tiles_start, Vector2.down);
				for (int x = Grid.Size[0] - 1; x >= 0; x--)
				{
					if (new_tiles_start[x] == null) continue;

					for (int _y = (int) new_tiles_start[x]; _y >= 0; _y--)
					{
						if (Tiles[x, new_tiles_start[x].Value] != null) continue;

						//INSERT STUFF FOR FALLING BIG TILES HERE
						int scale = 1;

						if (!fill_from_none[x, new_tiles_start[x].Value])
						{
							CreateTile(x, _y, Vector2.down, null, GENUS.NONE, false, scale);
						}
					}
				}
			}
		}
	}

	public bool CheckForMatches()
	{
		DiagMatches = new List<MatchContainer>();
		StrtMatches = new List<MatchContainer>();
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] == null) continue;
				Tiles[xx, yy].marked = false;
			}
		}

		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] == null || Tiles[xx, yy].Genus == GENUS.OMG || Tiles[xx, yy].Genus == GENUS.NONE) continue;
				MatchContainer diag = FloodCheck(Tiles[xx, yy]);

				if (diag.Size >= 3) DiagMatches.Add(diag);

			}
		}

		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] == null) continue;
				Tiles[xx, yy].marked = false;
			}
		}

		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] == null || Tiles[xx, yy].Genus == GENUS.OMG || Tiles[xx, yy].Genus == GENUS.NONE) continue;
				MatchContainer strt = FloodCheck(Tiles[xx, yy], false);
				if (strt.Size >= 3) StrtMatches.Add(strt);

			}
		}
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{
				if (Tiles[xx, yy] == null) continue;
				Tiles[xx, yy].marked = false;
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
		Tile [] nbours = t.Point.GetNeighbours(diagonals);


		for (int i = 0; i < nbours.Length; i++)
		{
			if (nbours[i].IsGenus(s, false, false) && !nbours[i].marked) checks.Add(nbours[i]);
		}

		foreach (Tile child in checks)
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
		List<MatchContainer> final;
		if (diagonal)final = Matches;
		else final = StrtMatches;
		foreach (MatchContainer child in final)
		{
			child.SetStates(TileState.Selected, true);
			yield return new WaitForSeconds(GameData.GameSpeed(0.05F));
		}

		yield return new WaitForSeconds(GameData.GameSpeed(0.25F));
		foreach (MatchContainer child in final)
		{
			child.Collect(diagonal);
		}
	}

	public void SetGroundBlocks(int num)
	{
		for (int i = 0; i < GroundBlocks.Length; i++)
		{
			if (i == num) GroundBlocks[i].SetActive(true);
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
		if (a == null || b == null) return false;
		for (int ai = 0; ai < a.Point.Length; ai++)
		{
			for (int bi = 0; bi < b.Point.Length; bi++)
			{
				diff[0] = a.Point.Point(ai)[0] - b.Point.Point(bi)[0];
				diff[1] = a.Point.Point(ai)[1] - b.Point.Point(bi)[1];
				if (Mathf.Abs(diff[0]) <= 1 && Mathf.Abs(diff[1]) <= 1) return true;
			}
		}
		return false;
		//diff[0] = a.num[0] - b.num[0];
		//diff[1] = a.num[1] - b.num[1];
		//return (Mathf.Abs(diff[0]) <=1 && Mathf.Abs(diff[1]) <= 1);
	}

	public void Ripple(Tile focus, float intensity = 1.0F, float time = 1.4F, float dropoff = 0.15F)
	{
		StartCoroutine(RippleRoutine(focus, 0.0F, intensity, time));
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Grid.Tiles[x, y] == focus) continue;

				int [] diff;
				AreNeighbours(Grid.Tiles[x, y], focus, out diff);
				float wait = 0.1F * Mathf.Abs(diff[0]) + 0.1F * Mathf.Abs(diff[1]);
				float inten = (1.0F - (Mathf.Abs(diff[0]) * dropoff + Mathf.Abs(diff[1]) * dropoff)) * intensity;
				if (inten < 0.05F) continue;
				StartCoroutine(RippleRoutine(Grid.Tiles[x, y], wait, inten, time));
			}
		}
	}

	public void Ripple(Tile focus, List<Tile> targets, float intensity = 1.0F, float time = 1.4F, float dropoff = 0.15F)
	{
		StartCoroutine(RippleRoutine(focus, 0.0F, intensity, time));
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == focus) continue;

			int [] diff;
			AreNeighbours(targets[i], focus, out diff);
			float wait = 0.1F * Mathf.Abs(diff[0]) + 0.1F * Mathf.Abs(diff[1]);
			float inten = (1.0F - (Mathf.Abs(diff[0]) * dropoff + Mathf.Abs(diff[1]) * dropoff)) * intensity;
			if (inten < 0.05F) continue;
			StartCoroutine(RippleRoutine(targets[i], wait, inten, time));
		}
	}

	IEnumerator RippleRoutine(Tile t, float wait, float inten, float time)
	{
		if (wait != 0.0F)yield return new WaitForSeconds(wait);
		Juice.instance.JuiceIt(Juice.instance.Ripple, t.transform, time, inten);
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
		for (int i = 0; i < chances.Length; i++)
		{
			allchance += chances[i];
		}
		chancevalue = Random.value * allchance;
		for (int i = 0; i < chances.Length; i++)
		{
			if (chancevalue >= currchance && chancevalue < currchance + chances[i])
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
		for (int i = 0; i < Tiles.Count; i++)
		{
			if (value > value_current && value <= value_current + Tiles[i].FinalChance)
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

		for (int i = 0; i < TilesNoEnemies.Count; i++)
		{
			if (value > value_current && value <= value_current + TilesNoEnemies[i].FinalChance)
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


		for (int g = 0; g < t.Species.Count; g++)
		{
			SPECIES s = t.Species[g];
			if (s.Chance > 0.0F)
			{
				for (int i = 0; i < s.AllGenus.Length; i++)
				{
					//Skip if genus tile chance is 0
					if (s.AllGenus[i].Chance == 0.0F) continue;
					TileInfo Info = new TileInfo(s, (GENUS)i);

					Tiles.Add(Info);
					AllChanceFactors += Info.FinalChance;

					if (!s.isEnemy)
					{
						NoEnemiesChanceFactors += Info.FinalChance;
						TilesNoEnemies.Add(Info);
					}
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
		foreach (Tile child in Tiles)
		{
			if (t == child) add = false;
		}
		if (add) Tiles.Add(t);
	}
	public void AddRange(MatchContainer m)
	{
		foreach (Tile child in m.Tiles)
		{
			bool add = true;
			foreach (Tile old in Tiles)
			{
				if (old == child)
				{
					add = false;
					break;
				}
			}
			if (add) Tiles.Add(child);
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
		foreach (Tile child in Tiles)
		{
			if (child == t) return true;
		}
		return false;
	}

	public bool IsType(SPECIES s)
	{
		foreach (Tile child in Tiles)
		{
			if (child.IsType(s)) return true;
		}
		return false;
	}

	public bool IsType(string s)
	{
		foreach (Tile child in Tiles)
		{
			if (child.IsType(s)) return true;
		}
		return false;
	}

	public void Collect(bool d)
	{
		if (!d && DiagonalMatch) return;
		foreach (Tile child in Tiles)
		{
			child.Match(1);
		}
	}

	public void SetStates(TileState s, bool over = false)
	{
		foreach (Tile child in Tiles)
		{
			child.SetState(s, over);
		}
	}
}