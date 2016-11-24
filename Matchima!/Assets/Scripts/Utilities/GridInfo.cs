using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable, ExecuteInEditMode]
public class GridInfo : MonoBehaviour{
	public bool setup = false;
	
	public GridColumn [] Grid;

	public GameObject [] TilePoints;
	public RoomInfo Info;

	public GENUS RoomInfluence = GENUS.NONE;

	public bool [] Entries = new bool[4];

	public GridPoint this [int x, int y]
	{
		get{
			if(Grid == null || Grid.Length == 0) return null;
			if(x > Grid.Length-1) x = Grid.Length - 1;
			if(y > Grid[x].Length-1) y = Grid[x].Length - 1;
			return Grid[x][y];
		}
		set
		{
			if(Grid == null || Grid.Length == 0) return;
			if(x > Grid.Length-1) x = Grid.Length - 1;
			if(y > Grid[x].Length-1) y = Grid[x].Length - 1;
		 	Grid[x][y] = value;
		}
	}

	public GridColumn this[int x]{
		get{
			if(Grid == null || Grid.Length == 0) return null;
			if(x > Grid.Length-1) x = Grid.Length - 1;
			return Grid[x];
		}
		set{
			if(Grid == null || Grid.Length == 0) return;
			if(x > Grid.Length-1) x = Grid.Length - 1;
			Grid[x] = value;
		}
	}
	
	

	public void SetActive(bool? active = null)
	{
		bool actual = active ?? !GridParent.activeSelf;
		GridParent.SetActive(actual);
	}

    /*public int [] Size
	{
		get{
			return new int [] {Grid.Length, Grid[0].Length};
		}
	}*/

	public GameObject GridParent;
	private GameObject pointParent, tileParent;

	public void DestroyThyself()
	{
		GameObject.Destroy(pointParent);
		GameObject.Destroy(tileParent);
		setup = false;
		//Destroy(this.gameObject);
	}

	public void SetInfluence(GENUS g)
	{

	}

	public void Setup(RoomInfo inf)
	{
		setup = true;
		Info = inf;
		Offset = inf.Position;
		if(GridParent != null)
		{
			GameObject.Destroy(GridParent);
		}
		GridParent = this.gameObject;
		GridParent.transform.name = inf.Name;

		GridParent.transform.SetParent(inf.Parent);
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.TileBuffer_X;
		float bufferY = TileMaster.TileBuffer_Y;

		if(pointParent == null) pointParent = new GameObject("Points");
		pointParent.transform.parent = GridParent.transform;
		pointParent.transform.position = Info.Position;
		if(tileParent == null) tileParent = new GameObject("Tiles");
		tileParent.transform.parent = GridParent.transform;
		tileParent.transform.position = Info.Position;

		Grid = new GridColumn[Size.x];
		for(int xx = 0; xx < Size.x;xx++)
		{
			GameObject column = new GameObject("Point Column " + xx);
			column.transform.position += new Vector3(xx * (1+bufferX), 0,0);
			column.transform.parent = pointParent.transform;

			TilePoints[xx] = new GameObject("Tile Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  tileParent.transform;

			Grid[xx] = new GridColumn(Size.y, column);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				Grid[xx][yy] = CreatePoint(xx, yy, Size);
			}
		}
	}

	public void Setup(Vector2 offset, Vector2 _size)
	{
		Offset = new Vector3(offset.x, offset.y, 0);
		setup = true;
		if(GridParent != null)
		{
			GameObject.Destroy(GridParent);
		}
		GridParent = this.gameObject;
		GridParent.transform.name = "Room";
		Transform master = TileMaster.instance.transform;
		if(master == null) master = GameObject.Find("TileMaster").transform;
		if(master == null) master = this.transform;

		GridParent.transform.SetParent(master.transform);

		Info.Size = new IntVector((int)_size.x, (int)_size.y);
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.TileBuffer_X;
		float bufferY = TileMaster.TileBuffer_Y;

		if(pointParent == null) pointParent = new GameObject("Points");
		pointParent.transform.parent = GridParent.transform;
		if(tileParent == null) tileParent = new GameObject("Tiles");
		tileParent.transform.parent = GridParent.transform;
		Grid = new GridColumn[Size.x];
		for(int xx = 0; xx < Size.x;xx++)
		{
			GameObject column = new GameObject("Point Column " + xx);
			column.transform.position += new Vector3(xx * (1+bufferX), 0,0);
			column.transform.parent = pointParent.transform;

			TilePoints[xx] = new GameObject("Tile Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  tileParent.transform;

			Grid[xx] = new GridColumn(Size.y, column);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				Grid[xx][yy] = CreatePoint(xx, yy,Size);
			}
		}
	}

	public void Setup(Vector2 offset, IntVector _size)
	{
		Offset = new Vector3(offset.x, offset.y, 0);
		setup = true;
		if(GridParent != null)
		{
			GameObject.Destroy(GridParent);
		}
		GridParent = this.gameObject;
		GridParent.transform.name = "Room";
		Transform master = TileMaster.instance.transform;
		if(master == null) master = GameObject.Find("TileMaster").transform;
		if(master == null) master = this.transform;

		GridParent.transform.SetParent(master.transform);

		Info.Size = new IntVector(_size);
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.TileBuffer_X;
		float bufferY = TileMaster.TileBuffer_Y;

		if(pointParent == null) pointParent = new GameObject("Points");
		pointParent.transform.parent = GridParent.transform;
		if(tileParent == null) tileParent = new GameObject("Tiles");
		tileParent.transform.parent = GridParent.transform;
		Grid = new GridColumn[Size.x];
		for(int xx = 0; xx < Size.x;xx++)
		{
			GameObject column = new GameObject("Column " + xx);
			column.transform.position += new Vector3(xx * (1+bufferX), 0,0);
			column.transform.parent = tileParent.transform;

			TilePoints[xx] = new GameObject("Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  pointParent.transform;

			Grid[xx] = new GridColumn(Size.y, column);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				Grid[xx][yy] = CreatePoint(xx, yy, Size);
			}
		}
	}

	public void Setup(Vector2 offset, GridInfo info)
	{
		Offset = new Vector3(offset.x, offset.y, 0);
		setup = true;
		if(GridParent != null)
		{
			GameObject.Destroy(GridParent);
		}

		GridParent = this.gameObject;
		GridParent.transform.name = "Room";
		if(TileMaster.instance != null) GridPointObj = TileMaster.instance.GridPointObj;
		Transform master = TileMaster.instance.transform;
		if(master == null) master = GameObject.Find("TileMaster").transform;
		if(master == null) master = this.transform;


		GridParent.transform.SetParent(master.transform);

		Info.Size = new IntVector(info.Size); //new int [2] {(int)_size.x, (int)_size.y};
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.TileBuffer_X;
				float bufferY = TileMaster.TileBuffer_Y;

		if(pointParent == null) pointParent = new GameObject("Points");
		pointParent.transform.parent = GridParent.transform;
		if(tileParent == null) tileParent = new GameObject("Tiles");
		tileParent.transform.parent = GridParent.transform;
		Grid = new GridColumn[Size.x];
		for(int xx = 0; xx < Size.x;xx++)
		{
			GameObject column = new GameObject("Column " + xx);
			column.transform.position += new Vector3(xx * (1+bufferX), 0,0);
			column.transform.parent = tileParent.transform;

			TilePoints[xx] = new GameObject("Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  pointParent.transform;

			Grid[xx] = new GridColumn(Size.y, column);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				Grid[xx][yy] = CreatePoint(xx, yy, Size);
				Grid[xx][yy].SetInfo(info.Grid[xx][yy]);
			}
		}
	}

	public void ChangeGridSizeTo(IntVector _size)
	{
		if(_size.x == 0 || _size.y == 0) return;
		float bufferX = TileMaster.TileBuffer_X;
		float bufferY = TileMaster.TileBuffer_Y;

		int curr_x = Grid.Length;
		int curr_y = Grid[0].Length;
		
		GridColumn [] newcolumns = new GridColumn[_size.x];
		GameObject [] newtiles = new GameObject[_size.x];
		for(int c = 0; c < newcolumns.Length; c++)
		{
			if(Grid.Length > c) 
			{
				newcolumns[c] = new GridColumn(_size.y, Grid[c].Obj);
				newcolumns[c].Obj.transform.SetParent(pointParent.transform);
				newtiles[c] = TilePoints[c];
				newtiles[c].transform.SetParent(tileParent.transform);
			}
			else
			{
				GameObject col = new GameObject("Point Column " + c);
				col.transform.position += new Vector3(c * (1+bufferX), 0,0);
				col.transform.SetParent(pointParent.transform);
				newcolumns[c] = new GridColumn(_size.y, col);

				GameObject t = new GameObject("Tile Column " + c);
				t.transform.position += new Vector3(c * (1+bufferX), 0,0);
				t.transform.SetParent(tileParent.transform);
				newtiles[c] = t;
			} 
		}

		for(int x = 0; x < newcolumns.Length; x++)
		{
			for(int y = 0; y < newcolumns[0].Length; y++){

				if(Grid.Length > x && Grid[0].Length > y)
				{
					newcolumns[x][y] = this[x,y];
				}
				else 
				{

					newcolumns[x][y] = CreatePoint(x,y, _size);
				}

				newcolumns[x][y].transform.SetParent(newcolumns[x].Obj.transform);
			}
		}


		if(curr_x > newcolumns.Length || curr_y > newcolumns[0].Length)
		{
			for(int xx = newcolumns.Length; xx < curr_x;xx++)
			{
				for(int yy = 0; yy < curr_y; yy++)
				{
					if(Grid.Length <= xx || Grid[0].Length <= yy) continue;
					if(Grid[xx][yy] != null) 
					{
						Grid[xx][yy].Destroy();
					}
				}
				DestroyImmediate(Grid[xx].Obj);
				if(TilePoints[xx] != null) DestroyImmediate(TilePoints[xx]);
				
			}

			for(int yy = newcolumns[0].Length; yy < curr_y; yy++)
			{
				for(int xx = 0; xx < curr_x; xx++)
				{
					if(Grid.Length <= xx || Grid[0].Length <= yy) continue;
					if(Grid[xx][yy] != null) 
					{
						Grid[xx][yy].Destroy();
					}
				}
			}
		}

		Grid = newcolumns;
		TilePoints = newtiles;
		Info.Size = _size;
	}

	public void ChangeBy(Vector2 _size)
	{
		GridPoint [,] newpoints = new GridPoint[Grid.Length + (int)_size.x,
												Grid[0].Length + (int)_size.y];
		GameObject [] newcolumns = new GameObject[Grid.Length + (int)_size.x];
		GameObject [] newtilepoints = new GameObject[TilePoints.Length + (int)_size.x];

		int curr_x = Grid.Length;
		int curr_y = Grid[0].Length;

		float bufferX = TileMaster.TileBuffer_X;
		float bufferY = TileMaster.TileBuffer_Y;

		for(int c = 0; c < newcolumns.Length; c++)
		{
			if(Grid.Length > c) newcolumns[c] = Grid[c].Obj;
			else newcolumns[c] = new GameObject("Column " + c);
		}

		for(int c = 0; c < newtilepoints.Length; c++)
		{
			if(TilePoints.Length > c) newtilepoints[c] = TilePoints[c];
			else newtilepoints[c] = new GameObject("Column " + c);
		}

		for(int x = 0; x < newpoints.Length; x++)
		{
			for(int y = 0; y < newpoints.GetLength(1); y++)
			{
				if(Grid.Length <= x || Grid[0].Length <= y) continue;
				newpoints[x,y] = Grid[x][y];
			}
			for(int yy = curr_y; yy < newpoints.GetLength(1); yy++)
			{
				newpoints[x,yy] = CreatePoint(x,yy, Size);
			}
		}
		
		if(curr_x > newpoints.Length || curr_y > newpoints.GetLength(1))
		{
			for(int xx = newpoints.Length; xx < curr_x;xx++)
			{
				for(int yy = 0; yy < curr_y; yy++)
				{
					if(Grid.Length <= xx || Grid[0].Length <= yy) continue;
					if(Grid[xx][yy]._Tile != null) 
					{
						Grid[xx][yy]._Tile.DestroyThyself();
					}
				}
				
			}

			for(int yy = newpoints.GetLength(1); yy < curr_y; yy++)
			{
				for(int xx = 0; xx < curr_x; xx++)
				{
					if(Grid.Length <= xx || Grid[0].Length <= yy) continue;
					if(Grid[xx][yy]._Tile != null) 
					{
						Grid[xx][yy]._Tile.DestroyThyself();
					}
				}
			}
		}
		//else
		//{
			for(int xx = curr_x; xx < newpoints.Length;xx++)
			{
				GameObject column = new GameObject("Column " + xx);
				column.transform.position += new Vector3(xx * (1+bufferX), 0,0);
				column.transform.parent = tileParent.transform;
				newcolumns[xx] = column;

				newtilepoints[xx] = new GameObject("Column " + xx);
				newtilepoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
				newtilepoints[xx].transform.parent =  pointParent.transform;

				for(int yy = 0; yy < newpoints.GetLength(1); yy++)
				{
					newpoints[xx,yy] = CreatePoint(xx,yy, Size);
				}
			}
		//}
		
		//Points = newpoints;
		int nx = newpoints.GetLength(0);
		Grid = new GridColumn[nx];
		for(int x = 0; x < newpoints.GetLength(0); x++)
		{
			Grid[x] = new GridColumn(newpoints.GetLength(1),  newcolumns[x]);
			for(int y = 0; y < newpoints.GetLength(1); y++)
			{
				Grid[x][y] = CreatePoint(x,y, Size);
			}
		}
		TilePoints = newtilepoints;
	}

	public void SetInfo(GridInfo g)
	{
		for(int x = 0; x < Grid.Length; x++)
		{
			for(int y = 0; y < Grid[0].Length; y++)
			{
				Grid[x][y].SetInfo(g[x,y].Info);
			}
		}
	}

	public void SetPointPosition(Vector3 p, int x, int y)
	{
		Grid[x][y].transform.position = p;
	}

	public GridPoint GridPointObj;
	public GridPoint CreatePoint(int x, int y, IntVector s)
	{
		float bufferX = TileMaster.TileBuffer_X;
		float bufferY = TileMaster.TileBuffer_Y;

		float posx =  (1+bufferX) + (x*(1+bufferX));//((float)-s.x/2 *
		float posy =  (1+bufferY) + (y*(1+bufferY));//((float)-s.y/2 *
		
		Vector3 position = Offset + new Vector3(posx + bufferX,  posy + bufferY, 0);

		GridPoint p = (GridPoint) Instantiate(GridPointObj);
		p.transform.position = position;
		if(Grid.Length > x) p.transform.parent = Grid[x].Obj.transform;
		p.Setup(x,y);
		return p;
	}

	public Vector3 GetPoint(int x, int y)
	{
		if(x > Grid.Length-1 || y > Grid[0].Length - 1) return Vector3.zero;
		return Grid[x][y].Pos;
	}


	public Vector3 GetPoint(int[] x)
	{
		if(x[0] > Size[0]-1) x[0] = Size[0]-1;
		if(x[1] > Size[1]-1) x[1] = Size[1]-1;
		return Grid[x[0]][x[1]].Pos;
	}

	public void SetPointInfo(int [] num, TileInfo t)
	{
		Grid[num[0]][num[1]].Info = t;
	}

	public void SetPointInfo(int x, int y, TileInfo t)
	{
		
		Grid[x][y].Info = t;
	}

	public void SetPointInfo(int x, int y, GridPoint t)
	{
		Grid[x][y].Info = t.Info;
	}

	public void SetTile(int x, int y, Tile t)
	{
		Grid[x][y]._Tile = t;
	}


	public void CompareInfo(RoomInfo r)
	{
		if(!Info.Compare(r)) return;
		bool update_y = false;
		if(Size.x > Grid.Length-1 || Size.x < Grid.Length-1)
		{
			GridColumn [] n = new GridColumn[Size.x];
			for(int i =0; i < Grid.Length; i++) 
			{
				if(i > n.Length-1) continue;
				n[i] = Grid[i];
			}
			for(int i = 0; i < n.Length; i++)
			{
				if(n[i] == null)
				{
					float bufferX = TileMaster.TileBuffer_X;
					GameObject c = new GameObject("Column " + i);
					c.transform.position += new Vector3(i * (1+bufferX), 0,0);
					//c.transform.parent = tileParent.transform;

					n[i] = new GridColumn(Size.y, c);
					update_y = true;
				} 
			}

			Grid = n;
		}

		if(Size.y > Grid[0].Length-1 || Size.y < Grid.Length-1 || update_y)
		{
			for(int x =0; x < Grid.Length; x++) 
			{
				GridPoint [] n = new GridPoint[Size.y];
				for(int y = 0; y < Grid[x].Length; y++)
				{
					if(x > n.Length-1) continue;
					n[x] = Grid[x][y];
				}
				for(int y = 0; y < n.Length; y++)
				{
					if(n[y] == null)
					{
						float bufferX = TileMaster.TileBuffer_X;
						float bufferY = TileMaster.TileBuffer_Y;

						float posx = (-Size.x/2 * (1+bufferX)) + (x*(1+bufferX));
						float posy = (-Size.y/2 * (1+bufferY)) + (y*(1+bufferY));
						
						Vector3 position = Position + new Vector3(posx + bufferX,  posy + bufferY, 0);

						n[y] = CreatePoint(x,y, Size);
					} 
				}
				Grid[x] = new GridColumn(n);
			}
			
		}
	}


	/*void OnDrawGizmos()
	{
		if(Info == null) return;
		Gizmos.color = Color.yellow;
		for(int x = 0; x < Grid.Length; x++)
		{
			for(int y = 0; y < Grid[x].Length; y++)
			{
				Gizmos.color = Grid[x][y].Empty ? Color.black : Color.green;
				Gizmos.DrawIcon(Grid[x][y].Pos, "Tile.png", true);
			}
		}
	}*/

	public IntVector Size {get{return Info.Size;}}
	public Vector3 Position{get{return Info.Position;}}

	public Vector3 CamPosition
	{
		get{
			//return Offset;
			return Vector3.Lerp(Grid[0][0].Pos, Grid[Grid.Length-1][Grid[Grid.Length-1].Length-1].Pos, 0.5F);
		}
	}

	public Tile [] Controllers
	{
		get
		{
			List<Tile> final = new List<Tile>();
			for(int x = 0; x < Size[0]; x++)
			{
				for(int y = 0; y < Size[1]; y++)
				{
					if(Tiles[x,y] == null) continue;
					if(Tiles[x,y].Controllable) final.Add(Tiles[x,y]);
				}
			}
			return final.ToArray();
		}
	}

	public Vector3 Offset = new Vector3(0,0,0);

	public Tile[,] Tiles
	{
		get
		{
			Tile[,] final = new Tile[Grid.Length, Grid[0].Length];
			for(int x = 0; x < Grid.Length; x++)
			{
				for(int y = 0; y < Grid[0].Length; y++)
				{
					final[x,y] = Grid[x][y]._Tile;
				}
			}
			return final;
		}
	}

}

[System.Serializable]
public class GridColumn
{
	public GameObject Obj;
	public GridPoint [] Points;
	public GridPoint this[int v]{
		get{return Points[v];}
		set{Points[v] = value;}
	}

	public int Length {get{return Points.Length;}}
	public GridColumn (int s, GameObject o)
	{
		Points = new GridPoint[s];
		Obj = o;
	}

	public GridColumn(GridPoint [] p)
	{
		Points = p;
	}

	public Tile [] GetTiles()
	{
		Tile [] final = new Tile[Points.Length];
		for(int i = 0; i < Length; i++)
		{
			final[i] = Points[i]._Tile;
		}
		return final;
	}


}

[System.Serializable]
public class RoomInfo
{
	public string Name;
	public int Index;
	public IntVector Size;

	public Transform Parent;

	public ToggleList GenusToIgnore;
	public SPECIES [] SpeciesToIgnore;

	public Vector3 Position;

	public RoomInfo()
	{
		Name = "Room";
		Index  = 0;
		Size = new IntVector(0,0);
		Position = Vector3.zero;

		GenusToIgnore = new ToggleList("STR", "DEX", "CHA", "WIS", "PRP", "OMG", "ALL");

	}

	public RoomInfo(RoomInfo r)
	{
		Name = r.Name;
		Index  = r.Index;
		Size = r.Size;
		
		Position = r.Position;
	}

	public bool Compare(RoomInfo r)
	{
		if(Name != r.Name)
		{
			Name = r.Name;
		}
		if(Index != r.Index) Index = r.Index;
		if(Size != r.Size)
		{
			Size = r.Size;
			return true;
		}

		if(Position != r.Position)
		{
			Position = r.Position;
			return true;
		}
		return false;
	}
}