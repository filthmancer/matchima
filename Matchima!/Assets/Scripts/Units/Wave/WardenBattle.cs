using UnityEngine;
using System.Collections;

public class WardenBattle : Wave {
	int Timer = 0;

	bool ward_info_shown = false;
	float ward_info_chance = 0.0F;
	float ward_info_chance_inc = 0.15F;

	int warden_actual_health; //Marker for warden's actual health while it is -1 (invisible)

	public override StCon [] IntroText
	{
		get{
			return new StCon []{new StCon("The Warden")};
		}
	} 
	public override StCon [] EnterText 
	{
		get {
			return new StCon []{new StCon("The Warden")};
		}
	}
	public override StCon [] ExitText 
	{
		get {
			return new StCon []{new StCon("Warden Defeated!")};
		}
	}

	public override IEnumerator OnStart()
	{
		warden_actual_health = 120;
		
		Slot1.Timer = 0;
		Slot1.Activate();
		Current = -1;
		Timer = 0;

		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Excellent work, making it this far.",  Slot1, true, 0.8F);
		tute.AddQuote("But I can't let you go any further.",  Slot1, true, 0.8F);
		tute.AddQuote("The more mana you absorb, the stronger you'll be...",  Slot1, true, 0.8F);
		tute.AddQuote("And I can't have you escaping back to the surface.",  Slot1, true, 0.8F);

		tute.AddQuote("So I'll just get rid of you now.",  Slot1, true, 0.8F);

		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		Slot2.Timer = 0;
		Slot2.Activate();
		Slot3.Timer = 0;
		Slot3.Activate();
		
		Current = warden_actual_health;
	}

	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		Timer ++;

		if(!ward_info_shown)
		{
			if(Random.value < ward_info_chance)
			{
				ward_info_shown = true;
				QuoteGroup tute = new QuoteGroup("Tute");
				tute.AddQuote("How is he controlling these beasts?",  Player.Classes[1], true, 1.4F);
				if(Random.value > 0.5F) tute.AddQuote("The warding magicks! A tool of power!",  Player.Classes[2], true, 1.4F);
				else tute.AddQuote("It must have to do with those wards he placed!",  Player.Classes[3], true, 1.4F);
				yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			}
			else ward_info_chance += ward_info_chance_inc;
		}

		yield return null;
	}


	protected override IEnumerator WaveActivateRoutine()
	{
		UIManager.Objects.TopGear[2].SetActive(false);
		UIManager.Objects.BotGear.SetTween(0, false);
		UIManager.Objects.TopGear.SetTween(0, true);
		UIManager.Objects.TopGear.FreeWheelDrag = true;

		GameManager.instance.paused = true;
		UIManager.instance.ScreenAlert.SetTween(0,true);

		UIManager.Objects.TopGear.FreeWheelDrag = false;
		UIManager.Objects.TopGear.MoveToDivision(0);
		StCon [] namecon = new StCon[] {new StCon(Name)};
		yield return StartCoroutine(UIManager.instance.Alert(1.25F, namecon));

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
		UIManager.instance.ScreenAlert.SetTween(0,false);
	}

	protected override IEnumerator WaveEndRoutine()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Ack! My nefarious plan has failed!",  Slot1, true, 1.4F);
		tute.AddQuote("This isn't the last you'll see of me, criminals!",  Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		Slot1.Active = false;
		Slot1.Ended = true;
		Slot2.Active = false;
		Slot2.Ended = true;
		Slot3.Active = false;
		Slot3.Ended = true;

		yield return StartCoroutine(UIManager.instance.Alert(1.1F, ExitText));

		int tx = ((TileMaster.Grid.Size[0]-1) / 2) - 1;
		int ty = ((TileMaster.Grid.Size[1]-1) / 2) - 1;
		TileMaster.instance.ReplaceTile(tx, ty, TileMaster.Types["chest"], GENUS.ALL, 3, 10 + (int)GameManager.Difficulty);
	}


}
