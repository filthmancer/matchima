using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum EnemyType
{
	Beast,
	Demon,
	Machine,
	Human,
	Undead
}
public class Enemy : Tile {

	public SpriteRenderer _Effect;

	public Vector2 HPRange, ATKRange;
	public bool SpecialEnemy;
	public EnemyType _EnemyType = EnemyType.Demon;
	protected int Rank = 1;
	private float threat_time = 0.0F;

	protected bool HasAttackedThisTurn;

	private float threat_anim, threat_anim_init = 0.4F;

	/*public override StCon [] Description
	{
		get{
			return new StCon[]{
				new StCon((_EnemyType + " Enemy")),
				new StCon((Stats.Hits > 0 ? Stats.Hits : 0) + " Health"),
				new StCon((Stats.Attack > 0 ? Stats.Attack : 0) + " Attack")
			};
		}
	}*/

	public override StCon [] BaseDescription
	{
		get{
			List<StCon> basic = new List<StCon>();
			if(Stats.Resource != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[0] + " Mana", GameData.Colour(Genus), false));
			if(Stats.Heal != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[1] + "% Health", GameData.Colour(GENUS.STR), false));
			if(Stats.Armour != 0)
			basic.Add(new StCon("+" + Stats.GetValues()[2] + " Armour", GameData.Colour(GENUS.DEX), false));
			basic.Add(new StCon((Stats.Hits > 0 ? Stats.Hits : 0) + " Health", GameData.Colour(GENUS.STR), false));
			basic.Add(new StCon((Stats.Attack > 0 ? Stats.Attack : 0) + " Attack", GameData.Colour(GENUS.DEX), false));
			return basic.ToArray();
		}
	}

	protected override TileUpgrade [] BaseUpgrades
	{
		get
		{
			return new TileUpgrade []
			{
				new TileUpgrade(1.0F, 1, () => {InitStats.Value += 1;})
				//new TileUpgrade(0.1F, 2, () => {InitStats.Resource +=1;})
			};
		}
	}


	public sealed override void Setup(int x, int y, int scale, TileInfo sp, int value_inc = 0)
	{
		base.Setup(x,y, scale, sp, value_inc);
		_Effect = Params._effect;
		_Effect.enabled = false;
		SetupEnemy();
	}

	protected virtual void SetupEnemy()
	{
		float factor = GameManager.Difficulty;//(Mathf.Exp(GameManager.GrowthRate * GameManager.Difficulty));
		float hpfactor = Random.Range(HPRange.x, HPRange.y);
		float atkfactor = Random.Range(ATKRange.x, ATKRange.y);

		factor *= Random.Range(0.8F, 1.4F);
		Rank = 1;

		InitStats.Hits        = (int)(hpfactor);
		InitStats.Attack      = (int)(atkfactor);
		SetSprite();
		if(Stats.isNew)
		{
			AddEffect("Sleep", 1);
		}
	}

	public override void AddValue(float amt)
	{
		InitStats.value_soft = Mathf.Clamp(InitStats.value_soft += amt, 0, 999);
		if((int) InitStats.value_soft != InitStats.Value)
		{
			InitStats.Hits += (int) (InitStats.value_soft) - InitStats.Value;
			InitStats.Attack += (int) (InitStats.value_soft) - InitStats.Value;
			InitStats.Value = (int)InitStats.value_soft;
			CheckStats();
			UIManager.instance.MiniAlert(TileMaster.Grid.GetPoint(Point.Base), "" + (Stats.Value), 75, Color.white,0.8F,0.0F);
			Animate("Alert");
		}
		
	}

	public override void Update () {
		base.Update();

		float hp = Mathf.Ceil(Player.Options.RealHP ? ((float)Stats.Hits/(float)Player.Stats._Attack) : (Stats.Hits));
		
		if(Params.HitCounter != null) Params.HitCounter.SetActive(hp > 1);
		if(Params.HitCounterText != null)
		{
			if(hp < 1) Params.HitCounterText.text = "" + 1;
			else if(hp > 99) Params.HitCounterText.text = "???";
			else Params.HitCounterText.text = "" + (int)(hp);	
		}

		if(!Stats.isAlerted) return;
		if(Stats.isFrozen) return;

		if(threat_time <= 0.0F)
		{
			if(threat_anim < threat_anim_init)
			{
				Vector3 final_rot = Juice.instance.Twitch.Rotation.Evaluate(threat_anim/threat_anim_init);
				Params._render.transform.rotation *= Quaternion.Euler(final_rot);
				threat_anim += Time.deltaTime;
				if(threat_anim > threat_anim_init) Params._render.transform.rotation = Quaternion.identity;
			}
			else 
			{
				threat_time = Random.Range(1.9F, 5.4F);
				threat_anim = 0.0F;
			}
			
		}
		else 
		{
			//_anim.SetBool("Threat", false);
			threat_time -= Time.deltaTime;
			if(threat_time <= 0.0F) 
			{
				AudioManager.instance.PlayTileAudio(this, "threat");
			}
		}
	}

	public override bool CanAttack()
	{
		CheckStats();
		return !Stats.isNew && !Stats.isFrozen && Stats.isAlerted && !HasAttackedThisTurn;
	}

	public override IEnumerator BeforeMatch(bool original)
	{
		if(isMatching) yield break;
		isMatching = true;

		if(original) InitStats.TurnDamage += PlayerControl.instance.AttackValue;

		if(InitStats.TurnDamage == 0) yield break;

		AudioManager.instance.PlayTileAudio(this, "hit");
		GameObject part = EffectManager.instance.PlayEffect(trans, Effect.Attack);
		yield return new WaitForSeconds(GameData.GameSpeed(0.03F));

		float init_rotation = Random.Range(-3,3);
		float info_time = 0.4F;
		float info_start_size = 100 + (InitStats.TurnDamage*2);
		float info_movespeed = 0.25F;
		float info_finalscale = 0.65F;

		Vector3 pos = TileMaster.Grid.GetPoint(Point.Point(0)) + Vector3.down * 0.3F;
		MiniAlertUI m = UIManager.instance.MiniAlert(pos,  "" + InitStats.TurnDamage, info_start_size, GameData.Colour(Genus), info_time, 0.6F, false);
		m.transform.rotation = Quaternion.Euler(0,0,init_rotation);
		m.SetVelocity(Utility.RandomVectorInclusive(0.4F) + (Vector3.up*0.6F));
		m.Gravity = true;
		m.AddJuice(Juice.instance.BounceB, info_time/0.8F);

		CameraUtility.instance.ScreenShake(0.26F + 0.02F * InitStats.TurnDamage,  GameData.GameSpeed(0.06F));
		yield return new WaitForSeconds(GameData.GameSpeed(0.18F));
	}

	public override IEnumerator AfterTurnRoutine(){
		yield return StartCoroutine(base.AfterTurnRoutine());
		
		HasAttackedThisTurn = false;

		if(Stats.isAlerted)
		{
			SetState(TileState.Idle, true);
			if(_anim != null) _anim.SetBool("Sleep",false);
		}
	}

	protected IEnumerator AllyAttack(Tile target)
	{
		Vector3 pos = transform.position + (GameData.RandomVector*1.4F);
		MoveToPoint mini = TileMaster.instance.CreateMiniTile(pos,target.transform, Info.Outer);
		mini.SetPath(0.3F, 0.5F, 0.0F, 0.08F);
		mini.SetMethod(() =>{
		  	if(target == null) return;
		  	AudioManager.instance.PlayTileAudio(target, "hit");
			target.InitStats.TurnDamage += Stats.Attack;
			target.Match(0);
				
			});

		yield return StartCoroutine(Animate("Attack", 0.05F));
		
		yield return null;
	}

	public override bool Match(int resource) {

		CheckStats();
		int fullhit = 0;

		fullhit += Stats.TurnDamage;

		InitStats.TurnDamage = 0;
		InitStats.Hits -= fullhit;
		CheckStats();
		
		Player.instance.OnTileMatch(this);
		if(Stats.Hits <= 0)
		{
			isMatching = true;
			Player.Stats.PrevTurnKills ++;	
			Stats.Value *= resource;
					
			CollectThyself(true);

			PlayAudio("death");
			float item_chance = (float)Stats.Value/32.0F;
			if(Stats.Value > 10) item_chance += 0.4F;
			if(Random.value < item_chance) 
			{
				int x = Random.Range(Point.BaseX, Point.BaseX + Point.Scale);
				int y = Random.Range(Point.BaseY, Point.BaseY + Point.Scale);

				GENUS g = Genus;
				float randg = Random.value;
				if(Random.value < 0.4F) g = (GENUS) Random.Range(0,4);
				if(Random.value < 0.95F) TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["chest"], g,  Point.Scale);
				else TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["mimic"], g, Point.Scale);
			}
			else TileMaster.Tiles[Point.Base[0], Point.Base[1]] = null;
			return true;
		}
		else 
		{
			isMatching = false;
		}
		return false;
	}

	public override void OnAttack()
	{
		HasAttackedThisTurn = true;
	}



	public override IEnumerator Animate(string type, float time = 0.0F)
	{
		if(type == "Attack")
		{
			Juice.instance.JuiceIt(Juice._Attack, Params._render.transform, 0.7F, 1.0F);
		} 
		if(time != 0.0F) yield return new WaitForSeconds(time);
		else yield return null;
	}


	public override void Stun(int stun)
	{
	

	}

	public override void SetDamageWarning(int amt)
	{
		int dmg = amt;
		string txt = "";
		//Put in other calculations for armor and stuff later
		if(dmg != 0)
		{
			if(dmg >= Stats.Hits) 
			{
				if(_Effect == null) return;
				_Effect.sprite = TileMaster.instance.KillEffect;
				_Effect.enabled = true;
			}
		}
		else 
		{
			_Effect.enabled = false;
			txt = "Blocked";
		}

		SetCounter(txt);
	}

	public override void Reset(bool idle = true)
	{
		base.Reset(idle);
		_Effect.enabled = false;
	}

	public override void SetSprite()
	{
		int sprite = Rank - 1;

		if(Info.Inner.Length > 0)
		{
			if(sprite > Info.Inner.Length - 1) sprite = Info.Inner.Length - 1;
		}
		else return;

		SetRender(Info.Inner[sprite]);
		SetBorder(Info.Outer);
		Params._shiny.sprite = Params._render.sprite;
	}

	//public override bool IsGenus(_Species s)
	//{
	//	if(Species.Genus == GENUS.None) return false;
	//	if(s.IsType("Sword")) return true;
	//	return Species.Genus == s.Genus;
	//}	

}
