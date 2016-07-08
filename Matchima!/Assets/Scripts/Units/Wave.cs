using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wave : Unit {

	public string Description
	{
		get
		{
			string d = "";
			return d;
		}
	}

	public WaveUnit this [int i]
	{
		get{
			return AllSlots[i];
		}
	}

	public int Length
	{
		get{
			return AllSlots.Length;
		}
	}

	public StCon _Name
	{
		get{
			if(!GenerateName) return new StCon(Name, GameData.Colour(Genus));
			else return new StCon(ModContainer.GenerateWaveName(true, true), GameData.Colour(Genus));
		}
	}

	public bool GenerateName;


	public bool Active;
	public float Chance = 1.0F;
	public int RequiredDifficulty = 1;

	public int Current;
	public int Required;
	private int PointsThisTurn;
	public int PointsPerTurn;


	public WaveUnit Slot1, Slot2, Slot3;
	public bool HasDialogue;
	public bool ExplodeOnEnd;
	public bool PointsFromTiles = true;
	public Quote [] Quotes;
	public WaveUnit [] AllSlots
	{
		get{return new WaveUnit [] {Slot1, Slot2, Slot3};}
	}

	public bool IntroAlert = false, EnterAlert = false;

	public virtual StCon [] IntroText
	{
		get{
			return null;
		}
	} 
	public virtual StCon [] EnterText 
	{
		get {
			return null;
		}
	}
	public virtual StCon [] ExitText 
	{
		get {
			return new StCon[]{new StCon("Floor Completed!")};
		}
	}

	public float GetRatio()
	{
		return (float) Current / Required;
	}

	public bool Ended;
	public int Experience = 5;
	private bool ShowingHealth;
	
	public IEnumerator Setup()
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].Setup(this,i);
		}
		Active = true;
		yield return StartCoroutine(OnStart());

		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].Activate();
		}	

		yield return StartCoroutine(WaveActivateRoutine());
	}

	public void AddPoints(int p)
	{
		if(!Active || Ended || Current == -1 || !PointsFromTiles) return;
		PointsThisTurn += p;
		Current = Mathf.Clamp(Current + p, 0, Required);
		if(!ShowingHealth)
		{
			StartCoroutine(ShowHealthRoutine());
		}
	}

	IEnumerator ShowHealthRoutine()
	{
		ShowingHealth = true;
		int current_heal = PointsThisTurn;

		string prefix = current_heal > 0 ? " +" : " ";
		Vector3 tpos = Vector3.down + Vector3.right * 0.4F;
		MiniAlertUI heal = UIManager.instance.MiniAlert(
			UIManager.instance.WaveHealthText.transform.position, 
			prefix + current_heal, 65, GameData.instance.GoodColour, 1.7F,	-0.08F);
		heal.transform.parent = UIManager.instance.WaveHealthText.transform;

		while(heal.lifetime > 0.0F)
		{
			if(PointsThisTurn == 0)
			{
				heal.lifetime = 0.0F;
				heal.text = "";
				break;
			}
			else if(PointsThisTurn != current_heal)
			{
				heal.lifetime += 0.2F;
				heal.size = 42 + current_heal * 0.75F;
				current_heal = PointsThisTurn;
				heal.text = prefix + current_heal;
			}
			

			yield return null;
		}

		ShowingHealth = false;

		yield return null;
	}


	public virtual IEnumerator OnStart()
	{
		yield break;
	}

	public virtual IEnumerator BeginTurn()
	{
		AddPoints(PointsPerTurn);
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				AllSlots[i].Timer ++;

				AddPoints(-AllSlots[i].PointsPerTurn);
				yield return StartCoroutine(AllSlots[i].BeginTurn());
			}
		}

		yield return null;
	}


	public virtual IEnumerator AfterTurn()
	{
		PointsThisTurn = 0;
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				yield return StartCoroutine(AllSlots[i].AfterTurn());
				AllSlots[i].Complete();
			}
		}
		if(Current == Required && Active)
		{
			for(int i = 0; i < AllSlots.Length; i++)
			{
				if(AllSlots[i] == null) continue;
				AllSlots[i].OnEnd();
			}
			
			GameManager.instance.paused = true;
			yield return StartCoroutine(WaveEndRoutine());
			GameManager.instance.paused = false;
			Ended = true;
		}
		yield return null;
	}

	public void AddWaveUnit(WaveUnit wavetile)
	{
		WaveUnit w = (WaveUnit) Instantiate(wavetile);
		w.transform.parent = this.transform;
		bool added = false;
		int slot = 0;
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] != null) continue;
			else 
			{
				LinkWaveUnit(w, i);
				added = true;
				slot = i;
				break;
			}
		}
		if(!added) Debug.LogError("COULD NOT ADD MOD WAVE");
		w.Setup(this, slot);
	}

	public void LinkWaveUnit(WaveUnit w, int i)
	{
		switch(i)
		{
			case 0:
			Slot1 = w;
			break;
			case 1:
			Slot2 = w;
			break;
			case 2:
			Slot3 = w;
			break;
		}
	}

	public virtual void EnemyKilled(Enemy e)
	{
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;

			AddPoints(AllSlots[i].EnemyKilled(e));
		}
	}

	public List<TileEffectInfo> GetEffects()
	{
		List<TileEffectInfo> Effects = new List<TileEffectInfo>();
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].GetEffects() != null) Effects.AddRange(AllSlots[i].GetEffects());
		}
		return Effects;
	}

	public void GetChances()
	{
		//List<TileEffectInfo> Effects = GetEffects();
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			AllSlots[i].GetChances();
		}
	}

	protected virtual IEnumerator WaveStartRoutine()
	{

		PlayerControl.instance.ResetSelected();
		if(IntroAlert)
		{
			yield return StartCoroutine(UIManager.instance.Alert(1.25F, IntroText));
		}
		yield return null;
		Player.instance.ResetStats();
	}

	protected virtual IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.TopGear[2].SetActive(false);
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);
		UIManager.Objects.TopGear.FreeWheelDrag = true;
		UIManager.instance.ShowGearTooltip(false);
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);

		for(int i = 1; i < UIManager.Objects.TopGear[1].Length; i++)
		{
			int genus = Random.Range(0,4);
			int num = TileMaster.Types.Length;
			SPECIES t = TileMaster.Types[Random.Range(0,num)];
			if(t.GetSprites(genus) == null || t.GetSprites(genus).Length == 0) continue;
			UIManager.Objects.TopGear[1][i][0].Img[0].sprite = t.GetSprites(genus)[0];
			UIManager.Objects.TopGear[1][i][0].Img[2].sprite = TileMaster.Genus.Frame[genus];
			UIManager.Objects.TopGear[1][i][0].SetActive(true);
		}

		float spintime = Random.Range(0.6F, 0.95F);
		while((spintime-=Time.deltaTime) > 0.1F)
		{
			UIManager.Objects.TopGear.AddSpin(spintime*3);
			yield return null;
		}
		//yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		UIManager.Objects.TopGear.FreeWheelDrag = false;
		UIManager.Objects.TopGear.MoveToDivision(0);
		//yield return new WaitForSeconds(GameData.GameSpeed(0.1F));
		StCon [] floor = new StCon[] {new StCon("Floor"), new StCon(GameManager.Floor + "")};

		Current = 0;
		StCon [] namecon = new StCon[] {_Name};
		yield return StartCoroutine(UIManager.instance.Alert(1.25F, floor, namecon));

		UIManager.Objects.TopGear[2].SetActive(true);
		for(int i = 0; i < AllSlots.Length; i++)
		{
			if(AllSlots[i] == null) continue;
			if(AllSlots[i].Active)
			{
				 yield return StartCoroutine(AllSlots[i].OnStart());
			}
		}

		for(int i = 1; i < UIManager.Objects.TopGear[1].Length; i++)
		{
			UIManager.Objects.TopGear[1][i][0].SetActive(false);
		}

		GameManager.instance.paused = false;
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		UIManager.instance.ScreenAlert.SetTween(0,false);
	}

	protected virtual IEnumerator WaveEndRoutine()
	{
		yield return StartCoroutine(UIManager.instance.Alert(1.05F, ExitText));
		if(ExplodeOnEnd)
		{
			for(int x = 0; x < TileMaster.Grid.Size[0]; x++)
			{
				for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
				{
					if(TileMaster.Tiles[x,y].Type.isEnemy)
					{
						yield return StartCoroutine(Cast(TileMaster.Tiles[x,y], 2));
					}
				}
			}
			TileMaster.instance.ClearQueuedTiles();
			yield return new WaitForSeconds(GameData.GameSpeed(0.4F));
		}
	}

	IEnumerator Cast(Tile target, int radius)
	{
		int targX = target.Point.Base[0];
		int targY = target.Point.Base[1];

		float particle_time = 0.4F;

		int final_radius =  (radius);

		if(target == null) yield break;

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<GameObject> particles = new List<GameObject>();

		for(int x = 0; x < _tiles.GetLength(0); x++)
		{
			for(int y = 0; y < _tiles.GetLength(1); y++)
			{
				int distX = Mathf.Abs(x - targX);
				int distY = Mathf.Abs(y - targY);
				
				
					if(distX + distY < final_radius)
					{
						Tile tile = _tiles[x,y];
						_tiles[x,y].SetState(TileState.Selected, true);
						to_collect.Add(tile);

						GameObject new_part = EffectManager.instance.PlayEffect(tile.transform, Effect.Fire);
						
						//new_part.transform.localScale *= 0.2F;
						particles.Add(new_part);
					}
				
			}
		}

		yield return new WaitForSeconds(Time.deltaTime * 10);

		for(int i = 0; i < to_collect.Count; i++)
		{
			if(to_collect[i].Type.isEnemy)
			{
				to_collect[i].InitStats.TurnDamage += 10;
			}
			if(to_collect[i].IsType("", "Chicken"))
			{
				TileMaster.instance.ReplaceTile(to_collect[i].Point.Base[0], to_collect[i].Point.Base[1], TileMaster.Types["Health"]);
				to_collect[i].AddValue(to_collect[i].Stats.Value * 10);
				to_collect.RemoveAt(i);
			}
		}

		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());

		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
	}

	public WaveReward GenerateWaveReward()
	{	
		if(Random.value > 1.0F) return null;

		WaveReward reward = new WaveReward();
		reward.value = GameManager.Difficulty;
		float val = Random.value;
		if(val < 0.25F)
		{
			reward.value = (reward.value * 2);
			reward.Title = "+" + (int)reward.value + " mana";
			foreach(Class child in Player.Classes)
			{
				child.AddToMeter((int)reward.value);
			}
		}
		else if(val < 0.5F)
		{
			reward.value = 1.0F + reward.value / 100;
			reward.Title = "+" + reward.value.ToString("0.00") + "x mana values";
		}
		else if(val < 0.75F)
		{
			reward.value = Mathf.Clamp(reward.value / 3, 1, Mathf.Infinity);
			reward.Title = "+" + (int)reward.value + " Upgrade Value!";
			foreach(Class child in Player.Classes)
			{
				child.WaveLevelRate += (int) reward.value;
			}
		}
		else
		{
			reward.value = Mathf.Clamp(reward.value * 5, 1, Player.Stats._HealthMax);
			reward.Title = (int)reward.value + " HP HEAL";
			Player.Stats.Heal((int) reward.value);
		}
		return reward;
	}

	public class WaveReward
	{
		public string Title;
		public float value;
	}


}

public enum WaveTileSpawn
{
	XAtStart,
	XPerTurn,
	XChance,
	XPsuedoChance,
	XOnScreen

}
