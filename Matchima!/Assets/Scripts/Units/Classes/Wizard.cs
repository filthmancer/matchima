using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wizard : Class {

TileChance lightning;
private Slot manapower;
private int _currentmanapower = 100;

	// Use this for initialization
	public override void StartClass () {
		
		/*ClassUpgrade a = new ClassUpgrade((int val) => {InitStats.CooldownDecrease += 0.01F * val;});
		a.Name = "Cooldowns";
		a.ShortName = "CD%";
		a.Description = " spell cooldowns";
		a.BaseAmount = 1;
		a.Prefix = "-";
		a.Suffix = "%";
		a.Rarity = Rarity.Common;

		ClassUpgrade b = new ClassUpgrade((int val) => {InitStats.MagicPower += 1 * val;});
		b.BaseAmount = 1;
		b.Name = "Spell Power";
		b.ShortName = "SP";
		b.Description = " Spell Power.\nIncreases strength of spells";
		b.Prefix = "+";
		b.Rarity = Rarity.Uncommon;

		wiz_cross = new TileChance();
		wiz_cross.Genus = "Alpha";
		wiz_cross.Type = "cross";
		InitStats.TileChances.Add(wiz_cross);

		ClassUpgrade c = new ClassUpgrade((int val) => {wiz_cross.Chance += 0.01F * val;});
		c.Name = "Alpha Cross Tiles";
		c.ShortName = "CRS";
		c.BaseAmount = 1;
		c.Description = " chance of Alpha Cross Tiles";
		c.Prefix = "+";
		c.Suffix = "%";
		c.Rarity = Rarity.Magic;

		wiz_arcane = new TileChance();
		wiz_arcane.Genus = GameData.ResourceLong(Genus);
		wiz_arcane.Type = "arcane";
		InitStats.TileChances.Add(wiz_arcane);

		ClassUpgrade wiz_arcane_up = new ClassUpgrade((int val) => {wiz_arcane.Chance += 0.02F * val;});
		wiz_arcane_up.Name = GameData.ResourceLong(Genus) + " Arcane Tiles";
		wiz_arcane_up.ShortName = GameData.Resource(Genus) + " ARC";
		wiz_arcane_up.Description = " chance of\n" + GameData.ResourceLong(Genus) + " Arcane Tiles";
		wiz_arcane_up.BaseAmount = 2;
		wiz_arcane_up.Prefix = "+";
		wiz_arcane_up.Suffix = "%";
		wiz_arcane_up.Rarity = Rarity.Uncommon;

		ClassUpgrade manamax = new ClassUpgrade((int val) => {InitStats.MeterMax += 5 * val;});
		manamax.Name = "Mana Max";
		manamax.ShortName = "MP MAX";
		manamax.Description = " Maximum Mana";
		manamax.BaseAmount = 5;
		manamax.Prefix = "+";
		manamax.Rarity = Rarity.Common;
		AddUpgrades(new ClassUpgrade[] {a,b,c, wiz_arcane_up, manamax});*/

		lightning = new TileChance();
		lightning.Genus = GameData.ResourceLong(Genus);
		lightning.Type = "lightning";
		lightning.Chance = 0.2F;
		InitStats.TileChances.Add(lightning);

		TileChance vanilla = new TileChance();
		vanilla.Genus = string.Empty;
		vanilla.Type = "lightning";
		vanilla.Chance = 0.1F;
		//InitStats.TileChances.Add(vanilla);

		TileChance arcane = new TileChance();
		arcane.Genus = GameData.ResourceLong(Genus);
		arcane.Type = "cross";
		arcane.Chance = 0.1F;
		InitStats.TileChances.Add(arcane);

		TileChance health = new TileChance();
		health.Genus = GameData.ResourceLong(Genus);
		health.Type = "health";
		health.Chance = 0.05F;
		InitStats.TileChances.Add(health);

		base.StartClass();	
	}


	public override void GetSpellTile(int x, int y, GENUS g, int points)
	{
		int rand = Random.Range(0,4);
		switch(rand)
		{
			case 0:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["bomb"], g, 1, points);
			break;
			case 1:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["lightning"], g, 1, points);	
			break;
			case 2:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["flame"], g, 1, points);	
			break;
			case 3:
				TileMaster.instance.ReplaceTile(x,y, TileMaster.Types["cross"], g, 1, points);	
			break;
			case 4:

			break;
		}
	}

	public override IEnumerator UseManaPower()
	{
		//UIManager.instance.ScreenAlert.SetTween(0,true);
		if(manapower != null)
		{
			Destroy(manapower.gameObject);
			AllMods.Remove(manapower);
		}

		switch(MeterLvl)
		{
			case 1:
				yield return StartCoroutine(ActiveRoutine(1));
				yield return StartCoroutine(PowerDown());
				yield return StartCoroutine(LevelUp(1));
				
			break;
			case 2:
				yield return StartCoroutine(ActiveRoutine(2));
				yield return StartCoroutine(PowerDown());
				yield return StartCoroutine(LevelUp(2));
				
			break;
			case 3:
				yield return StartCoroutine(ActiveRoutine(3));
				yield return StartCoroutine(PowerDown());
				yield return StartCoroutine(LevelUp(3));
				
			break;
		}

		//UIManager.instance.ScreenAlert.SetTween(0,false);
		yield return null;
	}

	int ClosestPoint(Vector3 pos)
	{
		float closest_dist = 100.0F;
		int closest = 100;
		for(int i = 0; i < TileMaster.Grid.Size[0]; i++)
		{
			float d = Vector3.Distance(TileMaster.Tiles[i,0].transform.position, pos);
			if(d < closest_dist)
			{
				closest = i;
				closest_dist = d;
			}
		}
		return closest;
	}

	int? target_column = null;
	IEnumerator ActiveRoutine(int lines)
	{
		activated = true;
		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);
		UIManager.ClassButtons.GetClass(Index).ShowClass(true);
		GameObject powerup = EffectManager.instance.PlayEffect(this.transform, Effect.ManaPowerUp, "", GameData.Colour(Genus));
		
		powerup.transform.SetParent(UIManager.ClassButtons.GetClass(Index).transform);
		powerup.transform.position = UIManager.ClassButtons.GetClass(Index).transform.position;
		powerup.transform.localScale = Vector3.one;

		
		float step_time = 0.75F;
		float total_time = step_time * 3;
		MiniAlertUI a = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.up*2, 
			"Wizard Casts", 70, GameData.Colour(Genus), total_time, 0.2F);
		a.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI b = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position, "Fireball", 170, GameData.Colour(Genus), step_time * 2, 0.2F);
		b.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		MiniAlertUI c  = UIManager.instance.MiniAlert(UIManager.Objects.MiddleGear.transform.position + Vector3.down * 2,
			"Press to cast!", 140, GameData.Colour(GENUS.STR), step_time, 0.2F);
		c.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(step_time));
		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
		Destroy(powerup);

		target_column = null;
		int currtile = 0;
		int nexttile = 1;
		float taptimer = 3.0F;
		int nexttile_acc = 1;

		UIObj [] MGame = new UIObj[lines];
		int [] MGame_target = new int[lines];
		float [] MGame_vel = new float[lines];
		for(int i = 0; i < MGame.Length; i++)
		{
			MGame[i] = CreateTarget(TileMaster.Grid.Size[0]/2);
			MGame_vel[i] = Random.Range(0.03F, 0.09F * lines);
			if(Random.value > 0.5F) MGame_vel[i] = -MGame_vel[i];
			yield return null;
		}

		while(!Input.GetMouseButtonDown(0))
		{
			for(int i = 0; i < MGame.Length; i++)
			{
				MGame[i].transform.position += Vector3.right * MGame_vel[i];
				MGame_target[i] = ClosestPoint(MGame[i].transform.position);
				if(Mathf.Abs(MGame_vel[i]) < 0.45F) MGame_vel[i] *= 1.003F;
				if(MGame[i].transform.position.x > TileMaster.Tiles[TileMaster.Grid.Size[0]-1,0].transform.position.x) MGame_vel[i] = -MGame_vel[i];
				else if(MGame[i].transform.position.x < TileMaster.Tiles[0,0].transform.position.x) MGame_vel[i] = -MGame_vel[i];
			}
			yield return null;
		}

		TileMaster.instance.SetAllTileStates(TileState.Locked, true);
		UIManager.instance.ScreenAlert.SetTween(0,false);
		for(int i = 0; i < lines;i++)
		{
			Destroy(MGame[i].gameObject);
			yield return Cast(TileMaster.Tiles[MGame_target[i],0]);
		}
		
		TileMaster.instance.SetFillGrid(false);
		yield return StartCoroutine(GameManager.instance.BeforeMatchRoutine());
		yield return null;
		yield return StartCoroutine(GameManager.instance.MatchRoutine(PlayerControl.instance.finalTiles.ToArray()));
		yield return StartCoroutine(Player.instance.AfterMatch());
		yield return new WaitForSeconds(Time.deltaTime * 10);
		TileMaster.instance.ResetTiles(true);
		TileMaster.instance.SetFillGrid(true);

		yield return new WaitForSeconds(GameData.GameSpeed(0.6F));

		GameManager.instance.paused = false;
		UIManager.ClassButtons.GetClass(Index).ShowClass(false);
	}

	UIObj CreateTarget(int i)
	{
		UIObj obj = (UIObj)Instantiate(MinigameObj);
		RectTransform rect = obj.GetComponent<RectTransform>();
		obj.transform.SetParent(UIManager.Objects.MiddleGear.transform);
		obj.transform.localScale = Vector3.one;
		rect.sizeDelta = Vector2.one;
		rect.anchoredPosition = Vector2.zero;
		rect.transform.position = TileMaster.Tiles[i,0].transform.position;
		return obj;
	}

	int Damage = 20;
	IEnumerator Cast(Tile target)
	{
		int targX = target.Point.Base[0];
		int targY = target.Point.Base[1];

		float particle_time = 0.7F;

		if(target == null) yield break;

		Tile [,] _tiles = TileMaster.Tiles;
		List<Tile> to_collect = new List<Tile>();

		List<GameObject> particles = new List<GameObject>();

		for(int y = 0; y < TileMaster.Grid.Size[1]; y++)
		{
			Tile tile = _tiles[targX,y];
			tile.SetState(TileState.Selected, true);
			to_collect.Add(tile);

			GameObject new_part = EffectManager.instance.PlayEffect(tile.transform, Effect.Fire);
			particles.Add(new_part);
			yield return new WaitForSeconds(GameData.GameSpeed(0.025F));

			tile = _tiles[targX,y];
			tile.SetState(TileState.Selected, true);
			to_collect.Add(tile);

			new_part = EffectManager.instance.PlayEffect(tile.transform, Effect.Fire);
			particles.Add(new_part);			
		}
		
		yield return new WaitForSeconds(Time.deltaTime * 10);

		for(int i = 0; i < to_collect.Count; i++)
		{
			if(to_collect[i].Type.isEnemy)
			{
				to_collect[i].InitStats.Hits -= Damage;
			}
			if(to_collect[i].IsType("", "Chicken"))
			{
				TileMaster.instance.ReplaceTile(to_collect[i].Point.Base[0], to_collect[i].Point.Base[1], TileMaster.Types["Health"]);
				to_collect[i].AddValue(to_collect[i].Stats.Value * 10);
				to_collect.RemoveAt(i);
			}

		}
		
		for(int i = 0; i < particles.Count; i++)
		{
			Destroy(particles[i]);
		}
		particles.Clear();
		PlayerControl.instance.AddTilesToSelected(to_collect.ToArray());

	}

}
