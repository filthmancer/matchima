using UnityEngine;
using System.Collections;

public class GridInfo {
	public GameObject [] Column;
	public GameObject [] TilePoints;
	public GridPoint [,] Points;
	public Tile[,] Tiles
	{
		get
		{
			Tile[,] final = new Tile[Points.GetLength(0), Points.GetLength(1)];
			for(int x = 0; x < Points.GetLength(0); x++)
			{
				for(int y = 0; y < Points.GetLength(1); y++)
				{
					final[x,y] = Points[x,y]._Tile;
				}
			}
			return final;
		}
	}

	public GridPoint this[int x, int y]
	{
		get{
			if(x > Size[0]-1) x = Size[0]-1;
			if(y > Size[1]-1) y = Size[1]-1;
			return Points[x,y];
		}
	}

	public int [] Size
	{
		get{
			return new int [] {Points.GetLength(0), Points.GetLength(1)};
		}
	}

	private GameObject pointParent, tileParent;
	public GridInfo()
	{

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DestroyThyself()
	{
		GameObject.Destroy(pointParent);
		GameObject.Destroy(tileParent);
		//Destroy(this.gameObject);
	}

	public void SetUp(Vector2 _size)
	{
		if(pointParent != null)
		{
			GameObject.Destroy(pointParent);
		}
		int [] size = new int [2] {(int)_size.x, (int)_size.y};
		Column = new GameObject[size[0]];
		Points = new GridPoint[size[0],  size[1]];
		TilePoints = new GameObject[size[0]];

		float bufferX = TileMaster.instance.tileBufferX;
		float bufferY = TileMaster.instance.tileBufferY;

		//float tileBufferX_offset = (float)(evenX ? (1.0F+bufferX)/2 : 0);
		//float tileBufferY_offset = (float)(evenY ? (1.0F+bufferY)/2 : 0);

		if(pointParent == null) pointParent = new GameObject("Points");
		pointParent.transform.parent = TileMaster.instance.transform;
		if(tileParent == null) tileParent = new GameObject("Tiles");
		tileParent.transform.parent = TileMaster.instance.transform;

		for(int xx = 0; xx < size[0];xx++)
		{
			GameObject column = new GameObject("Column " + xx);
			column.transform.position += new Vector3(xx * (1+bufferX), 0,0);
			column.transform.parent = tileParent.transform;
			Column[xx] = column;

			TilePoints[xx] = new GameObject("Column " + xx);
			TilePoints[xx].transform.position += new Vector3(xx * (1+bufferX), 0,0);
			TilePoints[xx].transform.parent =  pointParent.transform;

			for(int yy = 0; yy < size[1];yy++)
			{	
				GameObject _point = new GameObject("Point " + xx + ":" + yy);
				_point.transform.position = new Vector3(xx*(1+bufferX), yy*(1+bufferY), 0);
				_point.transform.parent = TilePoints[xx].transform;
				CreatePoint(xx, yy, _point.transform.position);
			}
		}
	}

	public void ChangeBy(Vector2 _size)
	{
		GridPoint [,] newpoints = new GridPoint[Points.GetLength(0) + (int)_size.x,
												Points.GetLength(1) + (int)_size.y];
		GameObject [] newcolumns = new GameObject[Column.Length + (int)_size.x];
		GameObject [] newtilepoints = new GameObject[TilePoints.Length + (int)_size.x];

		int curr_x = Points.GetLength(0);
		int curr_y = Points.GetLength(1);

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


		for(int x = 0; x < newpoints.GetLength(0); x++)
		{
			for(int y = 0; y < newpoints.GetLength(1); y++)
			{
				if(Points.GetLength(0) <= x || Points.GetLength(1) <= y) continue;
				newpoints[x,y] = Points[x,y];
			}
			for(int yy = curr_y; yy < newpoints.GetLength(1); yy++)
			{
				GameObject _point = new GameObject("Point " + x + ":" + yy);
				_point.transform.position = new Vector3(x*(1+bufferX), yy*(1+bufferY), 0);
				_point.transform.parent = newtilepoints[x].transform;
				newpoints[x,yy] = new GridPoint(new int[] {x,yy},_point.transform.position);
			}
		}
		
		if(curr_x > newpoints.GetLength(0) || curr_y > newpoints.GetLength(1))
		{
			for(int xx = newpoints.GetLength(0); xx < curr_x;xx++)
			{
				for(int yy = 0; yy < curr_y; yy++)
				{
					if(Points[xx,yy] != null) 
					{
						Points[xx,yy]._Tile.DestroyThyself();
					}
				}
				
			}

			for(int yy = newpoints.GetLength(1); yy < curr_y; yy++)
			{
				for(int xx = 0; xx < curr_x; xx++)
				{
					if(Points[xx,yy] != null) 
					{
						Points[xx,yy]._Tile.DestroyThyself();
					}
				}
			}
		}
		//else
		//{
			for(int xx = curr_x; xx < newpoints.GetLength(0);xx++)
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
		
		Points = newpoints;
		Column = newcolumns;
		TilePoints = newtilepoints;
	}

	public void SetInfo(GridInfo g)
	{
		for(int x = 0; x < Points.GetLength(0); x++)
		{
			for(int y = 0; y < Points.GetLength(1); y++)
			{
				SetPointInfo(x,y, g.Points[x,y].Info);
			}
		}
	}

	public void SetPointPosition(Vector3 p, int x, int y)
	{
		Points[x,y].position = p;
	}

	public void CreatePoint(int x, int y, Vector3 position)
	{
		Points[x,y] = new GridPoint(new int []{x,y}, position);
	}

	public Vector3 GetPoint(int x, int y)
	{
		if(x > Points.GetLength(0)-1 || y > Points.GetLength(1) - 1) return Vector3.zero;
		return Points[x,y].position;
	}

	public Vector3 GetPoint(int [] num)
	{
		return Points[num[0], num[1]].position;
	}

	public void SetPointInfo(int [] num, TileInfo t)
	{
		Points[num[0], num[1]].Info = t;
	}

	public void SetPointInfo(int x, int y, TileInfo t)
	{
		
		Points[x, y].Info = t;
	}

	public void SetTile(int x, int y, Tile t)
	{
		Points[x,y]._Tile = t;
	}

}

[System.Serializable]
public class GridPoint
{
	public int [] num = new int[2];
	public Vector3 position;
	public TileInfo Info;
	public Tile _Tile;

	public GridPoint(int [] _num, Vector3 pos)
	{
		num = _num;
		position = pos;
	}
}


/*public void SetUp(Vector2 _size)
{
	if(pointParent != null)
	{
		GameObject.Destroy(pointParent);
	}
	int [] size = new int [2] {(int)_size.x, (int)_size.y};
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

