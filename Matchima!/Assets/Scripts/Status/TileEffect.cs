using UnityEngine;
using System.Collections;

public class TileEffect : Status {
	public Tile _Tile;

	private GameObject particle;
	public virtual void Setup(Tile t)
	{
		_Tile = t;
		this.transform.SetParent(_Tile.transform);
		if(FX != string.Empty)
		{
			particle = EffectManager.instance.PlayEffect(this.transform, FX);
			particle.transform.parent = this.transform;
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
		ObjectPoolerReference poolref = particle.GetComponent<ObjectPoolerReference>();
		if(poolref)
		{
			poolref.Unspawn();
		}
		else Destroy(particle);
	}

	public override bool CanAttack()
	{
		return true;
	}


}
