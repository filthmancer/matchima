using UnityEngine;
using System.Collections;

public class TileEffect : Status {
	public Tile _Tile;
	public bool TileCanAttack = true;

	public virtual void Setup(Tile t)
	{
		_Tile = t;
		if(FX != string.Empty)
		{
			GameObject part = EffectManager.instance.PlayEffect(this.transform, Effect.STRING, FX);
			part.transform.parent = this.transform;
		}
	}

	public virtual TileStat CheckStats()
	{
		return null;
	}

	public virtual IEnumerator StatusEffectRoutine()
	{
		yield break;
	}

	public virtual void _OnDestroy()
	{
		
	}

	public virtual bool CanAttack()
	{
		return TileCanAttack;
	}


}
