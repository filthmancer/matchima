using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridInfo : MonoBehaviour{
	public bool setup = false;
	
	public GameObject [] Column;
	public GameObject [] TilePoints;
	//public GridPoint [,] Points;

	public Vector3 Position
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
	public GridColumn [] Grid;
	public IntVector Size;
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

	public void SetActive(bool? active = null)
	{
		bool actual = active ?? !GridParent.activeSelf;
		GridParent.SetActive(actual);
	}

/*	public int [] Size
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
		GridParent.transform.SetParent(TileMaster.instance.transform);

		Size = new IntVector((int)_size.x, (int)_size.y);
		Column = new GameObject[Size.x];
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.instance.tileBufferX;
		float bufferY = TileMaster.instance.tileBufferY;

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
			Column[xx] = column;

			TilePoints[xx] = new GameObject("Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  pointParent.transform;

			Grid[xx] = new GridColumn(Size.y);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				GameObject _point = new GameObject("Point " + xx + ":" + yy);

				float posx = (-Size.x/2 * (1+bufferX)) + (xx*(1+bufferX));
				float posy = (-Size.y/2 * (1+bufferY)) + (yy*(1+bufferY));
				_point.transform.position = Offset + new Vector3(posx + bufferX,  posy + bufferY, 0);
				_point.transform.parent = TilePoints[xx].transform;
				//CreatePoint(xx, yy, _point.transform.position);
				Grid[xx][yy] = CreatePoint(xx, yy, _point.transform.position);
				//Grid[xx][yy].SetInfo(info.Grid[xx][yy]);
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
		GridParent.transform.SetParent(TileMaster.instance.transform);

		Size = new IntVector(_size);
		Column = new GameObject[Size.x];
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.instance.tileBufferX;
		float bufferY = TileMaster.instance.tileBufferY;

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
			Column[xx] = column;

			TilePoints[xx] = new GameObject("Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  pointParent.transform;

			Grid[xx] = new GridColumn(Size.y);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				GameObject _point = new GameObject("Point " + xx + ":" + yy);
				float posx = (-Size.x/2 * (1+bufferX)) + (xx*(1+bufferX));
				float posy = (-Size.y/2 * (1+bufferY)) + (yy*(1+bufferY));
				_point.transform.position = Offset + new Vector3(posx+ bufferX,  posy + bufferY, 0);
				_point.transform.parent = TilePoints[xx].transform;
				//CreatePoint(xx, yy, _point.transform.position);
				Grid[xx][yy] = CreatePoint(xx, yy, _point.transform.position);
				//Grid[xx][yy].SetInfo(info.Grid[xx][yy]);
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
		GridParent.transform.SetParent(TileMaster.instance.transform);

		Size = new IntVector(info.Size); //new int [2] {(int)_size.x, (int)_size.y};
		Column = new GameObject[Size.x];
		//Points = new GridPoint[Size.x,  Size.y];
		TilePoints = new GameObject[Size.x];

		float bufferX = TileMaster.instance.tileBufferX;
		float bufferY = TileMaster.instance.tileBufferY;

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
			Column[xx] = column;

			TilePoints[xx] = new GameObject("Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  pointParent.transform;

			Grid[xx] = new GridColumn(Size.y);
			for(int yy = 0; yy < Size.y;yy++)
			{	
				GameObject _point = new GameObject("Point " + xx + ":" + yy);
				float posx =  (-Size.x/2 * (1+bufferX)) + (xx*(1+bufferX));
				float posy =  (-Size.y/2 * (1+bufferY)) + (yy*(1+bufferY));
				_point.transform.position = Offset + new Vector3(posx+ bufferX,  posy + bufferY, 0);
				_point.transform.parent = TilePoints[xx].transform;
				Grid[xx][yy] = CreatePoint(xx, yy, _point.transform.position);
				Grid[xx][yy].SetInfo(info.Grid[xx][yy]);
			}
		}
	}

	public void ChangeBy(Vector2 _size)
	{
		GridPoint [,] newpoints = new GridPoint[Grid.Length + (int)_size.x,
												Grid[0].Length + (int)_size.y];
		GameObject [] newcolumns = new GameObject[Column.Length + (int)_size.x];
		GameObject [] newtilepoints = new GameObject[TilePoints.Length + (int)_size.x];

		int curr_x = Grid.Length;
		int curr_y = Grid[0].Length;

		float bufferX = TileMaster.instance.tileBufferX;
		float bufferY = TileMaster.instance.tileBufferY;

		for(int c = 0; c < newcolumns.Length; c++)
		{
			if(Column.Length > c) newcolumns[c] = Column[c];
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
				GameObject _point = new GameObject("Point " + x + ":" + yy);
				_point.transform.position = new Vector3(x*(1+bufferX), yy*(1+bufferY), 0);
				_point.transform.parent = newtilepoints[x].transform;
				newpoints[x,yy] = new GridPoint(new int[] {x,yy},_point.transform.position);
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
					GameObject _point = new GameObject("Point " + xx + ":" + yy);
					_point.transform.position = new Vector3(xx*(1+bufferX), yy*(1+bufferY), 0);
					_point.transform.parent = newtilepoints[xx].transform;
					newpoints[xx,yy] = new GridPoint(new int[] {xx,yy},_point.transform.position);
				}
			}
		//}
		
		//Points = newpoints;
		int nx = newpoints.GetLength(0);
		Grid = new GridColumn[nx];
		for(int x = 0; x < newpoints.GetLength(0); x++)
		{
			Grid[x] = new GridColumn(newpoints.GetLength(1));
			for(int y = 0; y < newpoints.GetLength(1); y++)
			{
				Grid[x][y] = new GridPoint(newpoints[x,y]);
			}
		}
		Column = newcolumns;
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
		Grid[x][y].position = p;
	}

	public GridPoint CreatePoint(int x, int y, Vector3 position)
	{
		GridPoint p =  new GridPoint(new int []{x,y}, position);
		return p;
	}

	public Vector3 GetPoint(int x, int y)
	{
		if(x > Grid.Length-1 || y > Grid[0].Length - 1) return Vector3.zero;
		return Grid[x][y].position;
	}


	public Vector3 GetPoint(int[] x)
	{
		if(x[0] > Size[0]-1) x[0] = Size[0]-1;
		if(x[1] > Size[1]-1) x[1] = Size[1]-1;
		return Grid[x[0]][x[1]].position;
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

}

[System.Serializable]
public class GridColumn
{
	public GridPoint [] Points;
	public GridPoint this[int v]{
		get{return Points[v];}
		set{Points[v] = value;}
	}

	public int Length {get{return Points.Length;}}
	public GridColumn (int s)
	{
		Points = new GridPoint[s];
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
public class GridPoint
{
	public IntVector num; //int [] num = new int[2];
	public Vector3 position;
	public Vector3 Pos
	{
		get{
			if(_Tile != null) return _Tile.transform.position;
			else return position;
		}
	}
	public TileInfo Info;
	public Tile _Tile;
	public bool Empty;

	public bool ToFill()
	{
		if(Empty) 
		{
			return false;
		}
		return _Tile == null;
	}

	public GridPoint(int [] _num, Vector3 pos)
	{
		num = new IntVector(_num[0], _num[1]);
		position = pos;
		Empty = false;
		Info = null;
	}

	public GridPoint(GridPoint old)
	{
		num = old.num;
		position = old.position;
		Empty = old.Empty;
		SetInfo(old);
	}

	public void SetInfo(TileInfo inf)
	{
		Info = inf;
	}

	public void SetInfo(GridPoint g)
	{
		Info = g.Info;
	}
}


/*public void SetUp(Vector2 _size)
{
	if(pointParent != null)
	{
		GameObject.Destroy(pointParent);
	}
	Size = new int [2] {(int)_size.x, (int)_size.y};
	Column = new GameObject[size[0]];
	Points = new GridPoint[size[0],  size[1]];

	int x = 0, y = 0;
	bool evenX = size[0]%2 == 0;
	bool evenY = size[1]%2 == 0;

	float bufferX = TileMaster.instance.tileBufferX;
	float bufferY = TileMaster.instance.tileBufferY;

	float tileBufferX_offset = (float)(evenX ? (1.0F+bufferX)/2 : 0);
	float tileBufferY_offset = (float)(evenY ? (1.0F+bufferY)/2 : 0);

	if(pointParent == null) pointParent = new GameObject("Points");
	pointParent.transform.parent = TileMaster.instance.transform;
	if(tileParent == null) tileParent = new GameObject("Tiles");
	tileParent.transform.parent = TileMaster.instance.transform;

	for(int xx = -size[0]/2; xx < (evenX ? size[0]/2 : size[0]/2+1);xx++)
	{
		y = 0;
		GameObject column = new GameObject("Column " + x);
		column.transform.position += new Vector3(xx * (1+bufferX) + tileBufferX_offset, 0,0);
		column.transform.parent = tileParent.transform;
		Column[x] = column;

		GameObject points = new GameObject("Column " + x);
		points.transform.position += new Vector3(xx * (1+bufferX) + tileBufferX_offset, 0,0);
		points.transform.parent =  pointParent.transform;

		for(int yy = -size[1]/2; yy < (evenY ? size[1]/2 : size[1]/2+1);yy++)
		{	
			GameObject _point = new GameObject("Point " + x + ":" + y);
			_point.transform.position = new Vector3(xx*(1+bufferX) + tileBufferX_offset, yy*(1+bufferY) + tileBufferY_offset - TileMaster.instance.YOffset, 0);
			_point.transform.parent = points.transform;
			CreatePoint(x, y, _point.transform.position);
			y++;
		}
		x ++;
	}
}
*/

