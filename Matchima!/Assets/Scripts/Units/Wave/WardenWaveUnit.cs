using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WardenWaveUnit : WaveUnit {

	List<Tile> controllers;
	int controller_count = 3;
	public bool controllers_dead = false;
	public bool controllers_attack = false;

	public override IEnumerator OnStart()
	{
		if(!Active || Ended) yield break;
		GameManager.instance.paused = true;
		controllers = new List<Tile>();
		bool [,] replacedtile = new bool [(int)TileMaster.Grid.Size[0], (int)TileMaster.Grid.Size[1]];
		for(int i = 0; i < controller_count; i++)
		{
			int randx = (int)Random.Range(0, TileMaster.Grid.Size[0]-1);
			int randy = (int)Random.Range(0, TileMaster.Grid.Size[1]-1);
			while(replacedtile[randx, randy] || 
				TileMaster.Tiles[randx,randy].Point.Scale > 1)
			{
				randx = (int)Random.Range(0, TileMaster.Grid.Size[0]-1);
				randy = (int)Random.Range(0, TileMaster.Grid.Size[1]-1);
			}
			replacedtile[randx,randy] = true;

			CreateWard(randx, randy);
		
			yield return new WaitForSeconds(Time.deltaTime * 10);
		}
		yield return new WaitForSeconds(Time.deltaTime * 5);
	}

	void CreateWard(int x, int y)
	{
		GameObject initpart = EffectManager.instance.PlayEffect(UIManager.WaveButtons[Index].transform, Effect.Spell);
		MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
		mp.SetTarget(TileMaster.Tiles[x,y].transform.position);
		mp.SetPath(20.0F, 0.2F);
		//mp.Target_Tile = TileMaster.Tiles[x,y];
		mp.SetTileMethod(TileMaster.Tiles[x,y], (Tile t) => 
			{
				controllers.Add(TileMaster.instance.ReplaceTile(x, y, TileMaster.Types["ward"], GENUS.RAND, 1, 0));
				TileMaster.Tiles[x, y].InitStats.Hits = 3;
				TileMaster.Tiles[x, y].CheckStats();
				TileMaster.Tiles[x, y].DescriptionOverride = "Enemies ignore the Warden";
			});
	}

	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		if(!Active || Ended) yield break;
		yield return null;
		TimeActive ++;
		if(TimeActive % 3 == 0)
		{
			QuoteGroup tute = new QuoteGroup("Tute");
			int r = Random.Range(0,3);
			switch(r)
			{
				case 0: tute.AddQuote("Come on troops!",  this, false, 1.2F); break;
				case 1: tute.AddQuote("Fight for your precious mana.",  this, false, 1.2F); break;
				case 2: tute.AddQuote("I have total command!",  this, false, 1.2F); break;
			}
			
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

			if(TileMaster.Enemies.Length > 0)
			{
				Tile targ = TileMaster.Enemies[Random.Range(0, TileMaster.Enemies.Length)];
				CastAction(targ, (Tile t) =>
					{
						MiniAlertUI m = UIManager.instance.MiniAlert(t.Point.targetPos, "Frenzy!", 120, GameData.Colour(t.Genus), 0.3F, 0.1F);
						t.AddEffect("Frenzy", 1, "2");	
					});
			}
			
		}
	}

	public override IEnumerator AfterTurn()
	{
		if(!Active || Ended) yield break;
		bool end = true;
		for(int i = 0; i < controllers.Count; i++)
		{
			if(!controllers[i].Destroyed)
			{
				end = false;
			}	
		}
		if(end) 
		{
			if(!controllers_dead)
			{
				controllers_dead = true;
				QuoteGroup tute = new QuoteGroup("Tute");
				tute.AddQuote("We destroyed the wards!",  Player.Classes[3], true, 1.4F);
				tute.AddQuote("... What happens now?",  Player.Classes[3], true, 1.4F);
				yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			}
			

			yield return StartCoroutine(TakeAttack());
		}
		Complete();
		yield return null;
	}

	public IEnumerator TakeAttack()
	{
		float per_column = 0.15F;
		List<Tile> all_attackers = new List<Tile>();
		List<Tile> column_attackers;
		int damage = 0;
		bool took_damage = false;

		for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
		{
			column_attackers = new List<Tile>();
			for(int y = 0; y < TileMaster.Grid.Size[1];y++)
			{
				Tile tile = TileMaster.Tiles[x,y];
				if(tile == null) continue;
				if(tile.CanAttack())
				{
					tile.OnAttack();
					damage += tile.GetAttack();
					column_attackers.Add(tile as Tile);
					tile.SetState(TileState.Selected);
					took_damage = true;
				}
			}
			foreach(Tile child in column_attackers)
			{
				if(child == null) continue;

				//AudioManager.instance.PlayClipOn(child.transform, "Enemy", "Attack");

				child.AttackWaveUnit(Parent);
				yield return StartCoroutine(child.Animate("Attack", 0.05F));
			}

			all_attackers.AddRange(column_attackers);
			if(column_attackers.Count > 0) yield return new WaitForSeconds(GameData.GameSpeed(per_column));
		}
		//UIManager.instance.targetui_class = nexttarget;
		//Player.Stats.Hit(damage, all_attackers);
		yield return new WaitForSeconds(0.2F);

		if(took_damage)
		{
			if(!controllers_attack)
			{
				controllers_attack =  true;
				QuoteGroup tute = new QuoteGroup("Tute");
				tute.AddQuote("No! Stop it, you monsters!",  this, true, 1.4F);
				tute.AddQuote("I command you!",  this, true, 1.4F);
				yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			}
		}
	}


	public override void OnEnd()
	{
	}
}