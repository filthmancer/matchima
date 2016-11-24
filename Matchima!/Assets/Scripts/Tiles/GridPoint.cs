using UnityEngine;
using System.Collections;


public class GridPoint : MonoBehaviour
{
	public IntVector num; //int [] num = new int[2];
	public Vector3 Pos
	{
		get{
			return transform.position;
		}
	}
	public TileInfo Info;
	public Tile _Tile;
	public bool Empty;
	public bool RoomInfluencedGenus;
	public GENUS GenusOverride;

	public bool ToFill()
	{
		if(Empty) 
		{
			return false;
		}
		return _Tile == null;
	}

	public void SetInfo(TileInfo inf)
	{
		Info = inf;
	}

	public void SetInfo(GridPoint g)
	{
		Info = g.Info;
		Empty = g.Empty;
		GenusOverride = g.GenusOverride;
		StartSpawns = g.StartSpawns;
		ConstantSpawns = g.ConstantSpawns;
	}

	public bool HasStartSpawns(){return StartSpawns != null && StartSpawns.Length > 0;}
	public TileShortInfo [] StartSpawns;

	public bool HasConstantSpawns() {return ConstantSpawns != null && ConstantSpawns.Length > 0;}
	public TileShortInfo [] ConstantSpawns;

	public void Setup(GridPoint old)
	{
		num = old.num;
		transform.position = old.Pos;
		Empty = old.Empty;
		SetInfo(old);
		GenusOverride = GENUS.NONE;
		StartSpawns = new TileShortInfo[0];
		ConstantSpawns = new TileShortInfo[0];
	}

	public void Setup(int x, int y)
	{
		num = new IntVector(x,y);
		Empty = false;
		Info = null;
		GenusOverride = GENUS.NONE;
		StartSpawns = new TileShortInfo[0];
		ConstantSpawns = new TileShortInfo[0];
	}

	public void Destroy()
	{
		if(_Tile != null) _Tile.DestroyThyself();
		DestroyImmediate(this.gameObject);
	}
}
