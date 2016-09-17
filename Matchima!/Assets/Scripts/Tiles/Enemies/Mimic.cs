using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mimic : Enemy {

	public bool revealed = false;

	public override StCon [] BaseDescription
	{
		get{
			List<StCon> basic = new List<StCon>();
			if(Stats.Resource != 0)
			//basic.Add(new StCon("+" + Stats.GetValues()[0] + " Mana", GameData.Colour(Genus), false, 40));
			if(Stats.Heal != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[1] + "% Health", GameData.Colour(GENUS.STR), false, 40));
			if(Stats.Armour != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[2] + " Armour", GameData.Colour(GENUS.DEX), false, 40));
			if(revealed)
			{
				basic.Add( new StCon((_EnemyType + " Enemy"), Color.white, false,40));
				basic.Add(new StCon((Stats.Hits > 0 ? Stats.Hits : 0) + " HP", GameData.Colour(GENUS.STR), false,40));
				basic.Add(new StCon((Stats.Attack > 0 ? Stats.Attack : 0) + " AP", GameData.Colour(GENUS.DEX), false,40));
			}
			return basic.ToArray();
		}
	}

	public override StCon [] Description
	{
		get{
			if(revealed) return new StCon[]{
				//new StCon((_EnemyType + " Enemy"),Color.white, true, 40),
				new StCon("High damage trickster enemy.", GameData.Colour(Genus),true, 40)
			};
			else return new StCon[]{new StCon("Contains an secret tile", GameData.Colour(Genus),true, 40)};
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

		float hp = Mathf.Ceil(Player.Options.RealHP ? ((float)Stats.Hits/(float)Player.Stats._Attack) : (Stats.Hits));
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

		InitStats.Hits        = (int)(hpfactor);
		InitStats.Attack      = (int)(atkfactor);
		InitStats._Team = Team.None;

		Stats = new TileStat(InitStats);
		SetSprite();
		revealed = false;

		if(!revealed)
		{
			Params.HitCounter.SetActive(false);	
		}
		else AddEffect("Sleep", 1);
		
	}

	public override bool Match(int resource)
	{
		if(!revealed)
		{
			Reveal();

			if(originalMatch)
			{
				UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "MIMIC!", 95, GameData.Colour(Genus), 0.8F,0.15F);
				Vector3 pos = transform.position + (GameData.RandomVector*1.4F);
				/*MoveToPoint mini = TileMaster.instance.CreateMiniTile(transform.position, UIManager.instance.Health.transform, Info.Outer);
				mini.SetPath(0.3F, 0.5F, 0.0F, 0.08F);
				mini.SetMethod(() =>{
						Player.Stats.Hit(GetAttack()*2);
					}
				);*/
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

			Player.instance.OnTileMatch(this);
			if(Stats.Hits <= 0)
			{
				isMatching = true;
				Player.Stats.PrevTurnKills ++;			
				CollectThyself(true);

				if(GameData.ChestsFromEnemies)
				{
					float item_chance = (float)Stats.Value/32.0F;
					if(Stats.Value > 10) item_chance += 0.4F;
					if(Random.value < 0.98F)//item_chance) 
					{
						for(int reward = 0; reward < Point.Scale; reward++)
						{
							int x = Random.Range(Point.BaseX, Point.BaseX + Point.Scale);
							int y = Random.Range(Point.BaseY, Point.BaseY + Point.Scale);
	
							TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["chest"], Genus);
							TileMaster.Tiles[x,y].AddValue(Stats.Value);
						}
					}
					else
					{
						TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["mimic"], Genus);
						TileMaster.Tiles[x,y].AddValue(Stats.Value);
					}
				}
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
		InitStats._Team = Team.Enemy;
		Rank = 2;
		SetSprite();
		Params.HitCounter.SetActive(true);
	}

	public override void SetSprite()
	{
		SetBorder(Info.Outer);

		if(revealed) SetRender(Info._GenusName + "_2");
		else SetRender(Info._GenusName);
		
		//if(Params._shiny != null && Params._render != null) Params._shiny.sprite = Params._render.sprite;
	}

	public override TileEffect AddEffect(TileEffect e)
	{
		foreach(TileEffect child in Effects)
		{
			if(child.Name == e.Name)
			{
				child.Duration += e.Duration;
				if(e != null) Destroy(e.gameObject);
				return child;
			}
		}
		Reveal();
		e.Setup(this);
		e.transform.position = this.transform.position;
		e.transform.parent = this.transform;
		Effects.Add(e);
		CheckStats();
		return e;
	}

	public override IEnumerator AfterTurnRoutine(){


		yield return StartCoroutine(base.AfterTurnRoutine());


		if(Stats.isAlerted)
		{
			SetState(TileState.Idle, true);
			//_anim.SetBool("Sleep",false);
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
				//StartCoroutine(AllyAttack(target));
			}
		}
	}

	protected override IEnumerator ValueAlert(int diff)
	{
		SetState(TileState.Selected, true);
		Animate("Alert");
		
		revealed = true;

		MiniAlertUI m = UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "+" + Stats.Value, 150, GameData.Colour(Genus), 0.4F,0.00F);
		m.AddJuice(Juice.instance.Ripple.Scale, 0.4F);
		yield return new WaitForSeconds(0.4F);

		InitStats.Hits += (int) (InitStats.value_soft) - InitStats.Value;
		InitStats.Attack += (int) (InitStats.value_soft) - InitStats.Value;
		InitStats.Value = (int)InitStats.value_soft;

		CheckStats();
		SetSprite();
		Reset(true);
		yield return null;
	}

}
