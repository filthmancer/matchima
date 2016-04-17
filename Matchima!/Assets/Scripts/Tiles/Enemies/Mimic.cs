﻿using UnityEngine;
using System.Collections;

public class Mimic : Enemy {

	public bool revealed = false;
	public override StCon [] Description
	{
		get{
			if(revealed) return new StCon[]{
				new StCon((Stats.Hits > 0 ? Stats.Hits : 0) + " Health", GameData.Colour(GENUS.STR), false),
				new StCon((Stats.Attack > 0 ? Stats.Attack : 0) + " Attack", GameData.Colour(GENUS.DEX)),
				new StCon("High damage trickster enemy.", GameData.Colour(Genus))
			};
			else return new StCon[]{new StCon("Contains an item.", GameData.Colour(Genus))};
		}
	}

	public override StCon _Name {
		get{
			if(revealed) return new StCon(Info._TypeName, GameData.Colour(Genus)); 
			else return new StCon("Chest", GameData.Colour(Genus));
		}
	}

	public override void Update()
	{
		base.Update();

		float hp = Mathf.Ceil(Player._Options.HPBasedOnHits ? ((float)Stats.Hits/(float)Player.Stats._Attack) : (Stats.Hits));
		if(Params.HitCounter != null) Params.HitCounter.SetActive(hp > 1 && revealed);
		if(Params.HitCounterText != null)
		{
			if(hp < 1) Params.HitCounterText.text = "" + 1;
			else if(hp > 99) Params.HitCounterText.text = "???";
			else Params.HitCounterText.text = "" + (int)(hp);	
		}
	}

	protected sealed override void SetupEnemy()
	{
		float factor = GameManager.Difficulty;
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		factor *= Random.Range(0.8F, 1.1F);
		factor += factor * InitStats.Value / 50;
		hpfactor *= factor;
		atkfactor *= factor;
		Rank = 1;

		InitStats.Hits        = (int)(hpfactor);
		InitStats.Attack      = (int)(atkfactor);

		Stats = new TileStat(InitStats);
		SetSprite();

		if(!revealed)
		{
			Params.HitCounter.SetActive(false);	
		}
		else
		{
			TileEffect sleep = (TileEffect) Instantiate(GameData.instance.GetTileEffectByName("Sleep"));
			sleep.Duration = 1;
			AddEffect(sleep);
		}
		
	}

	public override bool Match(int resource)
	{
		if(!revealed)
		{
			Reveal();

			if(originalMatch)
			{
				Player.Stats.Hit(GetAttack()*2);
				MiniTile enemy = (MiniTile) Instantiate(TileMaster.instance.ResMiniTile);
				enemy._render.sprite = Info.Outer;
				enemy.transform.position = transform.position;
				enemy.SetTarget(UIManager.instance.HealthImg.transform, 0.2F, 0.0F);
				StartCoroutine(Animate("Attack", 0.05F));
				HasAttackedThisTurn = true;
				Player.Stats.CompleteHealth();
			}
			
			return true;
		}
		else
		{
			CheckStats();
			int fullhit = 0;
			if(originalMatch) 
			{
				fullhit += resource;
			}
			fullhit += Stats.TurnDamage;
			InitStats.TurnDamage = 0;

			InitStats.Hits -= fullhit;
			CheckStats();

			Player.instance.OnMatch(this);
			if(Stats.Hits <= 0)
			{
				isMatching = true;
				Player.Stats.PrevTurnKills ++;			
				CollectThyself(true);

				float item_chance = (float)Stats.Value/32.0F;
				if(Stats.Value > 10) item_chance += 0.4F;
				if(Random.value < 1.0F)//item_chance) 
				{
					for(int reward = 0; reward < Point.Scale; reward++)
					{
						int x = Random.Range(Point.BaseX, Point.BaseX + Point.Scale);
						int y = Random.Range(Point.BaseY, Point.BaseY + Point.Scale);

						TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["chest"], Genus);
						//TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["mimic"], Genus);
						TileMaster.Tiles[x,y].AddValue(Stats.Value);
					}
					
				}
				else TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
				return true;
			}
			else 
			{
				isMatching = false;
				EffectManager.instance.PlayEffect(this.transform,Effect.Attack);
			}
			return false;
		}
	}

	public override bool CanAttack()
	{
		return revealed && !Stats.isFrozen && !Stats.isAlly && !HasAttackedThisTurn;
	}


	public override bool Tap()
	{
		if(!revealed)
		{
			Reveal();
			return true;	
		}
		else return false;
		
	}

	public void Reveal()
	{
		revealed = true;
		InitStats.isAlerted = true;
		Rank = 2;
		SetSprite();
		Params.HitCounter.SetActive(true);
	}

	public override bool AddEffect(TileEffect e)
	{
		foreach(TileEffect child in Effects)
		{
			if(child.Name == e.Name)
			{
				child.Duration += e.Duration;
				if(e != null) Destroy(e.gameObject);
				return false;
			}
		}
		Reveal();
		
		e.Setup(this);
		e.transform.position = this.transform.position;
		e.transform.parent = this.transform;
		Effects.Add(e);


		return true;
	}

	public override void AfterTurn(){

		Reset();
		InitStats.TurnDamage = 0;
		HasAttackedThisTurn = false;
		InitStats.Lifetime ++;
		if(InitStats.Lifetime >= 1) 
		{
			InitStats.isNew = false;
		}

		Stats = new TileStat(InitStats);
		for(int i = 0; i < Effects.Count; i++)
		{
			if(Effects[i].CheckDuration()) 
			{
				Destroy(Effects[i].gameObject);
				Effects.RemoveAt(i);
			}
		}

		if(Stats.isAlerted)
		{
			SetState(TileState.Idle, true);
			_anim.SetBool("Sleep",false);
			//if(sleep_part)Destroy(sleep_part);
		}


		if(Stats.isAlly)
		{
			Tile target = null;
			for(int x = 0; x < TileMaster.Tiles.GetLength(0); x++)
			{
				for(int y = 0; y < TileMaster.Tiles.GetLength(1); y++)
				{
					if(TileMaster.Tiles[x,y].Type.isEnemy)
					{
						if(TileMaster.Tiles[x,y] == null) continue;
						if(TileMaster.Tiles[x,y] == this) continue;
						if(!TileMaster.Tiles[x,y].Type.isAlly) 
						{
							target = TileMaster.Tiles[x,y];
							break;
						}
					}
				}
				if(target!= null) break;
			}
			
			if(target != null)
			{
				StartCoroutine(AllyAttack(target));
			}
		}
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);
		if((int) InitStats.value_soft != InitStats.Value)
		{
			InitStats.Resource *= (int) (InitStats.value_soft / InitStats.Value);
			InitStats.Heal *= (int) (InitStats.value_soft / InitStats.Value);
			InitStats.Armour *= (int) (InitStats.value_soft / InitStats.Value);

			InitStats.Hits += (int) (InitStats.value_soft) - InitStats.Value;
			InitStats.Attack += (int) (InitStats.value_soft) - InitStats.Value;
			InitStats.Value = (int)InitStats.value_soft;
			CheckStats();
			
			if(revealed)
			{
				UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + (Stats.Value), 75, Color.white,0.8F,0.0F);
				Animate("Alert");
			}
		}
		
	}
}
