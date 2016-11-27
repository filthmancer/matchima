using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMaster : MonoBehaviour {
	public static TileMaster instance;
	public static TileTypes Types;
	public static GenusTypes Genus;
	[SerializeField] private GenusTypes _GenusTypes;

	public static float TileBuffer_X = 0.53F,
						TileBuffer_Y = 0.53F;

	public static GridInfo Grid;
	public static GridInfo Room
	{
		get{
			return Grid;
		}
	}

	public GridInfo GridTest;

	public static bool FillGridActive
	{
		get {return instance.FillGrid;}
	}
	public static bool HasRoom
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

	public static Tile[] Controllers
	{
		get
		{
			return Grid.Controllers;
		}
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

	public static Tile [] GetColumn(int x)
	{
		Tile [] final = new Tile[Grid.Size[0]];
		for(int i = 0; i < Grid.Size[0]; i++)
		{
			final[i] = Grid[x,i]._Tile;
		}
		return final;
	}

	public static Tile [] GetRow(int y)
	{
		Tile [] final = new Tile[Grid.Size[1]];
		for(int i = 0; i < Grid.Size[1]; i++)
		{
			final[i] = Grid[i,y]._Tile;
		}
		return final;
	}

	public static Tile RandomResTile
	{
		get{
			int x = Random.Range(0, Grid.Size[0]);
			int y = Random.Range(0, Grid.Size[1]);

			int check = 0;
			int max_check = (int)((float)(Grid.Size[0] * Grid.Size[1]) * 0.8F);

			while(Grid[x,y].Empty || Tiles[x,y] == null || !Tiles[x,y].IsType("resource"))
			{
				x = Random.Range(0, Grid.Size[0]);
				y = Random.Range(0, Grid.Size[1]);
				check++;
				if(check > max_check) break;
			}
			return Tiles[x,y];
		}
	}

	public static Tile [] TilesOfType(params string [] types)
	{
		List<Tile> final = new List<Tile>();
		for(int i = 0; i < types.Length; i++)
		{
			for(int x =0 ;x < Grid.Size[0]; x++)
			{
				for(int y =0; y < Grid.Size[1];y++)
				{
					if(Tiles[x,y] != null)
					{
						if(Tiles[x,y].IsType(types[i])) final.Add(Tiles[x,y]);
					}
				}
			}
		}

		return final.ToArray();
	}

	public static Tile RandomTileOfType(params string [] types)
	{
		int max_check = (int)((float)(Grid.Size[0] * Grid.Size[1]) * 0.8F);
		for(int i = 0; i < types.Length; i++)
		{
			int x = Random.Range(0, Grid.Size[0]);
			int y = Random.Range(0, Grid.Size[1]);

			int check = 0;
			bool found = true;
			while(Tiles[x,y] == null || !Tiles[x,y].IsType(types[i]))
			{
				x = Random.Range(0, Grid.Size[0]);
				y = Random.Range(0, Grid.Size[1]);
				check++;
				if(check > max_check) 
				{
					found = false;
					break;
				}
			}
			if(found) return Tiles[x,y];
		}
		return null;//Tiles[0,0];
	}

	public static Tile RandomTileOfGenus(params GENUS [] genus)
	{
		int max_check = (int)((float)(Grid.Size[0] * Grid.Size[1]) * 0.8F);
		for(int i = 0; i < genus.Length; i++)
		{
			int x = Random.Range(0, Grid.Size[0]);
			int y = Random.Range(0, Grid.Size[1]);

			int check = 0;
			bool found = true;
			while(Tiles[x,y] == null || Tiles[x,y].Genus != genus[i])
			{
				x = Random.Range(0, Grid.Size[0]);
				y = Random.Range(0, Grid.Size[1]);
				check++;
				if(check > max_check) 
				{
					found = false;
					break;
				}
			}
			if(found) return Tiles[x,y];
		}
		return null;//Tiles[0,0];
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
					if (Tiles[x, y] == null || Tiles[x,y].Destroyed) continue;
					if(Tiles[x,y].Stats.isEnemy)
					{
						if(!final.Contains(Tiles[x,y])) final.Add(Tiles[x,y]);
					}
				}
			}
			if(Tiles[0,0] != null && Tiles[0,0].Stats.isEnemy)
			{
				final.Add(Tiles[0,0]);
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
					if (Tiles[x, y] == null || Tiles[x,y].Destroyed) continue;
					if (Tiles[x, y].Stats.isAlly && !Tiles[x,y].Stats.isEnemy) 
					{
						if(!final.Contains(Tiles[x,y])) final.Add(Tiles[x,y]);
					}
				}
			}
			if(Tiles[0,0] != null && Tiles[0,0].Stats.isAlly)
			{
				final.Add(Tiles[0,0]);
			}
			return final.ToArray();
		}
	}


	public static bool TilesAttacking()
	{
		for (int x = 0; x < Tiles.GetLength(0); x++)
		{
			for (int y = 0; y < Tiles.GetLength(1); y++)
			{
				if (Tiles[x, y] == null || Tiles[x,y].Destroyed) continue;
					if(Tiles[x,y].isAttacking) 
					{
						Debug.LogWarning("ATTACKING: " + x + "-" + y);
						return true;
					}
			}
		}
		return false;
	}

	public Vector2 MapSize;
	//[HideInInspector]
	public Vector2 MapSize_Default;
	public float tileBufferX = 0.2F, tileBufferY = 0.2F;
	public float YOffset = 1.0F;

	public GridPoint GridPointObj;
	public GameObject ResMiniTile;

	[HideInInspector]
	public GameObject TileBasic;
	public GameObject [] GroundBlocks;

	public bool scaleup;

	public Sprite KillEffect;
	public MiniTile2 MiniTileObj;

	public bool generated = false;

	private float [] spawn_stack;
	private bool first_enemy = false;
	[SerializeField]
	private bool FillGrid = false;
	private GridInfo level_to_load = null;
	bool [,] fill_from_none;

	public float AllChanceInit;
	public float AllChanceCurrent;
	public static float ChanceRatio = 1.0F;

	public bool JustAddEverything = false;

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
	public List<MatchContainer> HeroMatches;

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

				//GenerateGrid(level_to_load);

				Player.instance.ResetChances();
				AllChanceInit = Spawner2.AllChanceFactors;
				AllChanceCurrent = AllChanceInit;
				ChanceRatio = AllChanceCurrent / AllChanceInit;

				generated = true;
			}

			if (FillGrid) ShiftTiles2(ShiftType.None);//Player.Stats.Shift);
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
		if (Grid.Size[0] > x && x >= 0)
		{
			if (Grid.Size[1] > y && y >= 0)
			{
				if(!Grid[x,y].Empty)
					return Tiles[x, y];
			}
		}
		return null;
	}

	public Tile [] GetTiles(string species, GENUS g = GENUS.NONE)
	{
		List<Tile> final = new List<Tile>();
		for(int x = 0; x < Grid.Size[0]; x++)
		{
			for(int y = 0; y < Grid.Size[1]; y++)
			{
				Tile t = Tiles[x,y];
				if(Tiles[x,y]== null) continue;
				bool is_spec = (species == string.Empty) || t.IsType(species);
				bool is_gen = (g == GENUS.NONE) || t.Genus == g;
				if(is_spec && is_gen && !final.Contains(t)) final.Add(t);
			}
		}
		return final.ToArray();
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
		generated = true;
		if (level != null) //If loading a GridInfo file
		{
			MapSize = new Vector2(level.Size[0],  level.Size[1]);
			bool evenX = MapSize[0] % 2 == 0;
			bool evenY = MapSize[1] % 2 == 0;
			float tileBufferX_offset = (float)(evenX ? (1.0F + tileBufferX) / 2 : 0);
			float tileBufferY_offset = (float)(evenY ? (1.0F + tileBufferY) / 2 : 0);
			Grid = (GridInfo) Instantiate(RoomObj);//new GridInfo(offset);
			Grid.Setup(Vector2.zero, level);

			int [] tile_size = new int [2] {(int)Grid.Size[0], (int)Grid.Size[1]};


			SetOrtho();

			CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0, 0].Pos,
			                           Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].Pos,
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
					if (Grid[x, y].Info != null) CreateTile(x, y, new Vector2(0, 1), Grid[x, y].Info);
					//else print(x + ":" + y + " -- " + Grid[x,y]);
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

			Grid = (GridInfo) Instantiate(RoomObj);//new GridInfo(offset);
			Grid.Setup(Vector2.zero, MapSize);

			Grid[1,1].Empty = true;
			Grid[3,1].Empty = true;
			Grid[3,3].Empty = true;

			GridTest = Grid;
			SetOrtho();
			SetFillGrid(true);
			CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0, 0].Pos,
			                           Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].Pos,
			                           0.5F));

			for (int xx = 0; xx < Grid.Size[0]; xx++)
			{
				for (int yy = 0; yy < Grid.Size[1]; yy++)
				{
					CreateTile(xx, yy, Vector2.zero);
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
	public GridInfo RoomObj;
	public GridInfo GenerateGrid2(Vector2 offset, GridInfo level, bool populate = false)
	{
		generated = true;
		GridInfo final;
		if (level != null) //If loading a GridInfo file
		{
			MapSize = new Vector2(level.Size[0],  level.Size[1]);
			final = (GridInfo) Instantiate(RoomObj);//new GridInfo(offset);
			final.Setup(offset, level);
		}
		else //Create random grid
		{
			MapSize = MapSize_Default;
			if(populate)
			{
				MapSize += Utility.RandomVector2(2,3) + Utility.RandomVector2(-1, 0);
			}

			final = (GridInfo) Instantiate(RoomObj);//new GridInfo(offset);
			final.Setup(offset, MapSize);
		}


		if(populate)
		{
			for(int i = 0; i < Random.Range(0,5);i++)
			{
				int x = Random.Range(0, final.Grid.Length-1);
				int y = Random.Range(1, final.Grid[0].Length-1);
				final[x,y].Empty = true;
			}

			string [] randtypes = new string []
			{
				"cross",
				"health",
				"bomb",
				"arcane",
				"chest",
				"health"
			};

			for(int i = 0; i < Random.Range(3, 8); i++)
			{
				int x = Random.Range(0, final.Grid.Length);
				int y = Random.Range(1, final.Grid[0].Length);
				int r = Random.Range(0, randtypes.Length);
				final[x,y].Info = new TileInfo(Types[randtypes[r]], GENUS.RAND);
			}

			string [] randenemies = new string []
			{
				"grunt",
				"minion",
				"blob"
			};

			for(int i = 0; i < Random.Range(2, 6); i++)
			{
				int x = Random.Range(0, final.Grid.Length);
				int y = Random.Range(1, final.Grid[0].Length);
				int r = Random.Range(0, randenemies.Length);
				final[x,y].Info = new TileInfo(Types[randenemies[r]], GENUS.RAND);
			}

		}

		for (int xx = 0; xx < final.Size[0]; xx++)
		{
			for (int yy = 0; yy < final.Size[1]; yy++)
			{
				if(final[xx,yy].HasStartSpawns()) 
				{
					string type = final[xx,yy].StartSpawns[0]._Type;
					GENUS genus = final[xx,yy].StartSpawns[0]._Genus;
					int val = final[xx,yy].StartSpawns[0].Value;
					CreateTile(final, xx,yy, Vector2.zero, TileMaster.Types[type], genus, false, 1, val);
				}
				else CreateTile(final, xx,yy, Vector2.zero);
			}
		}
		for (int x = 0; x < final.Size[0]; x++)
		{
			for (int y = 0; y < final.Size[1]; y++)
			{
				if (final[x, y]._Tile == null) continue;
				if (final[x, y]._Tile != null || final[x, y]._Tile.Type == null)
				{
					final[x, y]._Tile.InitStats.isNew = false;
				}
			}
		}

		return final;
	}

	public void CheckGrid()
	{
		Vector2 finalMap = TileMaster.instance.MapSize_Default
		                   + Player.Stats.MapSize;

		if(MapSize != finalMap)
		{
			//IncreaseGridTo(finalMap);
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
		CameraUtility.SetTargetPos(Vector3.Lerp(Grid[0, 0].Pos,
		                                        Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].Pos,
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

		CameraUtility.SetTargetPos(Vector3.Lerp(Grid[0, 0].Pos,
		                                        Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].Pos,
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

		CameraUtility.SetTargetPos(Vector3.Lerp( Grid[0, 0].Pos,
		                           Grid[Grid.Size[0] - 1, Grid.Size[1] - 1].Pos,
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

	public Stairs [] stairs;
	public List<GridInfo> oldrooms = new List<GridInfo>();

	public IEnumerator MoveToRoom(IntVector direction, GridInfo r = null, GENUS inc = GENUS.NONE)
	{
		GameManager.instance.paused = true;
		GameManager.Difficulty += Mathf.Exp(GameManager.instance.Difficulty_Growth);
		Vector3 pos = Vector3.zero;
		GridInfo old = Grid;

		if(Grid != null)
		{
			pos = Grid.Position;
			pos += new Vector3(direction.x * Grid.RoomRadius, direction.y * Grid.RoomRadius, 0);
		}
		
		if(r == null) r = GameData.instance.GetRandomRoom();

		r.SetInfluence(inc);

		GridInfo newroom = GenerateGrid2(pos, r);

		pos += new Vector3(direction.x * newroom.RoomRadius, direction.y * newroom.RoomRadius, 0);

		newroom.SetPosition(pos);
	
		Grid = newroom;
		GridTest = Grid;
		
		yield return StartCoroutine(CameraUtility.instance.MoveToRoom(newroom));

		yield return new WaitForSeconds(0.4F);

		int stairsnum = Random.value > 0.8F ? 3 : Random.value > 0.6F ? 2 : 1;
		List<Vector2> positions = new List<Vector2>();
		positions.Add(new Vector2(0.5F, 1));
		positions.Add(new Vector2(1, 0.5F));
		positions.Add(new Vector2(0.5F, 0));
		positions.Add(new Vector2(0, 0.5F));
		
		stairs = new Stairs[stairsnum];
		for(int i = 0; i < stairsnum; i++)
		{
			int snum = Random.Range(0, positions.Count);
			Vector2 spos = positions[snum];
			stairs[i] = ReplaceTile((int)(spos.x * Grid.Size[0]), (int)(spos.y * Grid.Size[1]), Types["stairs"], GENUS.OMG) as Stairs;
			if(stairs[i] == null) continue;
			IntVector realdir = new IntVector(	(spos.x > 0 ? (spos.x > 0.5F ? 1 : 0) : -1),
												(spos.y > 0 ? (spos.y > 0.5F ? 1 : 0) : -1));
			stairs[i].SetDirection(realdir);
			stairs[i].ChangeGenus(GENUS.OMG);
			positions.RemoveAt(snum);
		}

		newroom.Exits = stairs;

		//Center point of the entry side
		IntVector enter_side = new IntVector(Grid.Size[0]/2-(Grid.Size[0]/2*direction.x), Grid.Size[1]/2 - (Grid.Size[1]/2*direction.y));
		//Vector to add to entry position for each character
		IntVector entry_velocity = new IntVector((direction.x == 0 ? 1:0), (direction.y == 0 ? 1 : 0));
		if(old != null) 
		{
			for(int i = 0; i < old.Controllers.Length; i++)
			{
				old.Controllers[i].gameObject.SetActive(true);
				int side = (i % 2 == 0) ? -1 : 1;
				side *= 1 + (i/2);
				yield return StartCoroutine(old.Controllers[i].MoveToGrid(newroom, enter_side.x + (entry_velocity.x*side),
																		enter_side.y + (entry_velocity.y*side), true, 100.0F));
				yield return new WaitForSeconds(GameData.GameSpeed(0.05F));
			}
			yield return new WaitForSeconds(0.3F);
			old.SetActive(false);
			oldrooms.Add(old);
		}

		GameManager.instance.paused = false;
		PlayerControl.instance.SetController(null);

	}

	public IEnumerator MoveToLevel()
	{
		for(int i = 0; i < oldrooms.Count; i++)
		{
			Destroy(oldrooms[i].gameObject);

		}

		oldrooms.Clear();

		for(int i = 0; i < Player.ClassTiles.Length; i++)
		{
			Player.ClassTiles[i].transform.SetParent(this.transform);
		}
		Destroy(Grid.gameObject);
		
		yield return StartCoroutine(MoveToRoom(new IntVector(0,0), null));
	}

	private Tile CreateTile(int x, int y, Vector2 velocity, TileInfo t, int scale = 1, int value = 0)
	{
		if(Grid == null) return null;
		return CreateTile(Grid, x, y, velocity, t, scale, value);
	}
	private Tile CreateTile(GridInfo g, int x, int y, Vector2 velocity, TileInfo t, int scale = 1, int value = 0)
	{
		//???? WAHT IS DIS
		//if(Tiles.GetLength(0)>x && Tiles.GetLength(1) > y)
		//{
		//	if(Tiles[x,y] != null) return null;
		//}
		//????
		if(t == null) return null;
		if(g != null && !g[x,y].ToFill()) return null;
		if(g[x,y].GenusOverride != GENUS.NONE) t.ChangeGenus(g[x,y].GenusOverride);
		if(g.RoomInfluence != GENUS.NONE && g[x,y].RoomInfluencedGenus) t.ChangeGenus(g.RoomInfluence);

		if (t.Inner == null) t.Inner = t._Type.Atlas;
		if (t.Outer == null) t.Outer = TileMaster.Genus.Frames.GetSpriteIdByName(t._GenusName);
		Tile new_tile = t._Type.TilePool.Spawn();
		float ny = velocity.y == 0.0F ? g.GetPoint(x, y).y : spawn_stack[x];
		float nx = velocity.x == 0.0F ? g.GetPoint(x, y).x : spawn_stack[y];

		new_tile.transform.position = new Vector3(nx, ny, 0);
		new_tile.name = "Tile | " + x + ":" + y;

		for (int xx = 0; xx < scale; xx++)
		{
			for (int yy = 0; yy < scale; yy++)
			{
				g[x + xx, y + yy]._Tile = new_tile;
			}
		}

		g[x, y]._Tile.Setup(g, x, y, scale, t, value);

		if (GameManager.instance.BotTeamTurn) g[x, y]._Tile.SetState(TileState.Locked);
		else g[x, y]._Tile.SetState(TileState.Idle);


		float half_x = (tileBufferX) / 2;
		float half_y = (tileBufferY) / 2;
		SphereCollider tile_col = new_tile.GetComponent<SphereCollider>();
		tile_col.radius = 0.5F + half_x;

		if (velocity.y > 0.0F) spawn_stack[x] += tile_col.radius * 2;
		else if (velocity.y < 0.0F) spawn_stack[x] -= tile_col.radius * 2;
		else if (velocity.x > 0.0F) spawn_stack[y] += tile_col.radius * 2;
		else if (velocity.x < 0.0F) spawn_stack[y] -= tile_col.radius * 2;
		return g[x, y]._Tile;
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

	private Tile CreateTile(GridInfo grid, int x, int y, Vector2 velocity, SPECIES s = null, GENUS g = GENUS.NONE, bool no_enemies = false, int scale = 1, int value = 0)
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

		return CreateTile(grid, x, y, velocity, t, scale, value);
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
			if(Grid[x+xx, y].Empty) x--;
			for (int yy = 0; yy < newscale; yy++)
			{
				if(Grid[x+xx,y+yy].Empty)y--;
				if (Grid[x + xx, y + yy]._Tile != null)
				{
					tempval +=  override_old ? 0 : Grid[x + xx, y + yy]._Tile.Stats.Value - 1;
					if (!override_old && !Grid[x + xx, y + yy]._Tile.Destroyed) Grid[x + xx, y + yy]._Tile.DestroyThyself();
				}

			}
		}

		newtile = CreateTile(x, y, Vector2.zero, sp, g, false, newscale, addvalue);
		if(newtile == null) return null;
		//Juice.instance.JuiceIt(Juice.instance.LandFromAbove, newtile.transform, 0.6F, 0.4F);
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
		int [] a_point = a.Point.Base;
		int [] b_point = b.Point.Base;
		
		a.MoveToGridPoint(b_point[0], b_point[1], 0.25F);
		b.MoveToGridPoint(a_point[0], a_point[1], -0.25F);
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
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x, y] != null)
				{
					Tiles[x, y].DestroyThyself(true);
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

		List<Tile> old_con = new List<Tile>();
		old_con.AddRange(Controllers);
		old_con.AddRange(Grid.Exits);
		for (int x = 0; x < Grid.Size[0]; x++)
		{
			for (int y = 0; y < Grid.Size[1]; y++)
			{
				if (Tiles.GetLength(0) > x && Tiles.GetLength(1) > y && Tiles[x, y] != null)
				{
					if(!old_con.Contains(Tiles[x,y]))
					{
						Tiles[x, y].DestroyThyself();
						Grid[x, y]._Tile = null;
						CreateTile(x, y, Vector2.zero, sp, g, no_enemies);
					}
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
						
					}
				}
			}
		}
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
					if (Tiles[xx, yy].Type.isEnemy) 
					{
						EnemiesOnScreen ++;
						Tiles[xx,yy].isAttacking = false;
					}

					Tiles[xx, yy].AfterTurn();
					if (Tiles[xx, yy].HasAfterTurnEffect())
					{
						UIManager.instance.TargetTile(Tiles[xx,yy]);
						yield return StartCoroutine(Tiles[xx, yy].AfterTurnRoutine());	
					}
						
				}
				//else if(fill_from_none[xx, yy]) ReplaceTile(xx,yy);
			}
		}

		//yield return new WaitForSeconds(Time.deltaTime * 20);

		for(int i =0; i < stairs.Length; i++)
			if(EnemiesOnScreen == 0 && stairs[i].Genus == GENUS.OMG) stairs[i].ChangeGenus(GENUS.RAND);

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

		//CheckForMatches();
		/*if(HeroMatches.Count < Player.Classes.Length)
		{
			for(int i = 0; i < Player.Classes.Length; i++)
			{
				bool add = true;
				for(int h =0;h<HeroMatches.Count;h++)
				{
					if(HeroMatches[h].Controller == Player.Classes[i]._Tile)
					{
						add = false;
						break;
					}
				}
				if(add)
				{
					Tile [] n = Player.Classes[i]._Tile.Point.GetNeighbours(true, "resource");
					if(n.Length > 0) 
					{
						ReplaceTile(n[Random.Range(0,n.Length)], Types["resource"], Player.Classes[i].Genus);
						yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
					}
				}

			}
		}*/
		if (!CheckForMatches())
		{
			MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.ObjectsT._Main.transform.position, "NO MATCHES!", 140, GameData.Colour(GENUS.OMG),
														GameData.GameSpeed(0.85F), 0.1F, true);
			yield return new WaitForSeconds(GameData.GameSpeed(0.9F));

			Tile [] omegas = GetTiles("", GENUS.OMG);
			for(int i = 0; i < omegas.Length; i++)
			{
				if(omegas[i].IsType("stairs")) continue;
				omegas[i].InitStats._Attack.Add(omegas[i].Stats.Value / 2);
				omegas[i].CheckStats();
				omegas[i].Animate("Attack", 0.05F);
				omegas[i].AttackTile(Player.ClassTiles[0]);
			}
			NewGrid();
		}

		yield break;
	}

	public void CheckForEmptys()
	{
		//if(fill_from_none == null || Tiles == null) return;
		for (int xx = 0; xx < Grid.Size[0]; xx++)
		{
			for (int yy = 0; yy < Grid.Size[1]; yy++)
			{

				if(xx >= Tiles.GetLength(0) || yy >= Tiles.GetLength(1)) continue;
				if(xx >= fill_from_none.GetLength(0) || yy >= fill_from_none.GetLength(1)) continue;
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
			//print(Tiles[x,y]);
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
		if(GameManager.Zone != null) GameManager.Zone.OnTileDestroy(t);
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
	}

	public void CollectTile(Tile t, bool destroy)
	{
		t.CheckStats();
		CollectTileResource(t, destroy);
		if(GameManager.Zone != null) GameManager.Zone.OnTileCollect(t);
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
		int hp = (t.Stats.isEnemy ? (int)((float)t.Stats.Hits/(float)Player.Stats._Attack) : t.Stats.Hits);
		if(hp > 1) 
		{
			values[0] /= hp;
			
			//t.AddValue(-values[0]);
		}
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
			

			if(PlayerControl.instance.Controller) PlayerControl.instance.Controller.AddMana(t.Genus, values[0]);
			//UIManager.instance.GetMeterPoints(g, values[0]);
			

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
			if(PlayerControl.instance.Controller) PlayerControl.instance.Controller.AddHealth(t.Genus, values[1]);
			//Player.Stats.Heal(values[1]);
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

	public MoveToPoint CreateMiniTile(Vector3 pos, Transform target, Tile t)
	{
		MiniTile2 new_mini = Instantiate(MiniTileObj);
		new_mini.transform.position = pos;
		new_mini.transform.localScale = Vector3.one * 0.08F;
		new_mini.Setup(t);

		MoveToPoint mover = new_mini.GetComponent<MoveToPoint>();
		mover.SetTarget(target.position);
		mover.SetScale(0.04F, 5);
		//mover.SetPath(speed, arc, lerp);
		return mover;
	}

	public void QueueTilesSpawnOnStart(SPECIES spec, GENUS g, int num)
	{
		
	}

	private List<TileQuickInfo> QueuedTiles = new List<TileQuickInfo>();
	public void QueueTile(SPECIES spec, GENUS g = GENUS.NONE, int value = 0)
	{
		QueuedTiles.Add(new TileQuickInfo(g, spec, 1, value));
	}

	public void ClearQueuedTiles()
	{
		QueuedTiles.Clear();
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

			CreateTile(x, y, velocity, QueuedTiles[i].Species, QueuedTiles[i].Genus, false, QueuedTiles[i].Scale, QueuedTiles[i].Value);

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
	}

	//private List<TileQuickInfo> TravelTiles = new List<TileQuickInfo>();
	private List<Tile> TravelTiles = new List<Tile>();
	public void AddTravelTiles(params Tile [] t)
	{
		TravelTiles.AddRange(t);
		/*for(int i = 0; i < t.Length; i++)
		{
			TravelTiles.Add(t);//new TileQuickInfo(t[i].Genus, t[i].Type, t[i].Info.Scale, t[i].Stats.Value-1));
		}*/
		
	}
	public IEnumerator CreateTravelTiles()
	{
		if(TravelTiles.Count == 0) yield break;

		GameObject powerup = EffectManager.instance.PlayEffect(Tiles[0,0].transform, "stairstravel", Color.white);
		yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		for(int i = 0; i < TravelTiles.Count; i++)
		{
			if(i > Grid.Size[0]-1) continue;
			CastQuickTile(powerup.transform, i, 1, TravelTiles[i]);
			
			yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		}
		yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		TravelTiles.Clear();
		Destroy(powerup);
	}

	private void CastQuickTile(Transform trans, int x, int y, Tile trav)
	{
		/*GameData.instance.ActionCaster(trans, target, () => {
					TileMaster.instance.ReplaceTile(target, t.Species, t.Genus,t.Scale, t.Value);
				});*/
		StartCoroutine(trav.MoveToPoint(x,y, true, 100.0F));
	}

	private class TileQuickInfo
	{
		public GENUS Genus;
		public SPECIES Species;
		public int Scale;
		public int Value;
		public TileQuickInfo(GENUS g, SPECIES s, int sc, int v)
		{
			Genus = g;
			Species = s;
			Scale = sc;
			Value = v;
		}
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
					fill_from_none[x, y] = Grid[x,y].ToFill();
				}
			}
			//CheckForEmptys();
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
					if (Grid[x,y].ToFill() && !empty.HasValue)
					{
						empty = y;
						if (y < Grid.Size[1] - 1) fill_from_none[x, y] = true;
					}

					//IF THE TILE ABOVE AN EMPTY SPOT ISN'T NULL
					else if (empty.HasValue && !Grid[x,y].ToFill())
					{
						if (Grid[x,y]._Tile.Stats.Shift != ShiftType.None)
						{

							//IF TILE ABOVE IS GIANT WITH BASE ELSWHERE, CONTINUE
							if (Grid[x,y]._Tile.Point.BaseX != x)
							{
								fill_from_none[x, empty.Value] = true;
								empty++;
								continue;
							}

							//IF NO SPACE FOR TILE TO FALL, CONTINUE
							bool nospace = false;
							for (int xx = 0; xx < Grid[x,y]._Tile.Point.Scale; xx++)
							{
								for (int yy = 0; yy < Grid[x,y]._Tile.Point.Scale; yy++)
								{
									if (x + xx > Grid.Size[0] - 1 || y + yy > Grid.Size[1] - 1)
									{
										fill_from_none[x, empty.Value] = true;
										nospace = true;
										break;
									}

									nospace = Tiles[x + xx, y] != null && Tiles[x + xx, y] != Grid[x,y]._Tile;
									nospace = Tiles[x + xx, y + yy] != null && Tiles[x + xx, y + yy] != Grid[x,y]._Tile;

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
							Grid[x, empty.Value]._Tile = Grid[x,y]._Tile;
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
					else if (Grid[x,y]._Tile == null)
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

					if (Grid[x,y]._Tile == null && !fill_from_none[x, y])
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
		HeroMatches = new List<MatchContainer>();

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

				if (diag.Size >= 3) 
				{
					DiagMatches.Add(diag);
					if (diag.ContainsController()) 
					{
						bool add = true;
						for(int i = 0; i < HeroMatches.Count; i++)
						{
							if(HeroMatches[i].Controller == diag.Controller) add = false;
						}
						if(add) HeroMatches.Add(diag);
					}
				}
				
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
				if (strt.Size >= 3) 
				{
					StrtMatches.Add(strt);
					if (strt.ContainsController()) 
					{
						bool add = true;
						for(int i = 0; i < HeroMatches.Count; i++)
						{
							if(HeroMatches[i].Controller == strt.Controller) add = false;
						}
						if(add) HeroMatches.Add(strt);
					}
				}
				

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
			if (s.Chance > 0.0F ||  TileMaster.instance.JustAddEverything)
			{
				for (int i = 0; i < s.AllGenus.Length; i++)
				{
					//Skip if genus tile chance is 0
					if (s.AllGenus[i].Chance == 0.0F && !TileMaster.instance.JustAddEverything) continue;
					TileInfo Info = new TileInfo(s, (GENUS)i);

					if(TileMaster.instance.JustAddEverything) Info.FinalChance = 1.0F;
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
	public Tile Controller;
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

	public bool ContainsController()
	{
		foreach(Tile child in Tiles)
		{
			if(child.Controllable) 
			{	
				Controller = child;
				return true;
			}
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