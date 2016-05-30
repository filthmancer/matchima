using UnityEngine;
using System.Collections;

public class WardenBattle : Wave {
	int Timer = 0;

	bool ward_info_shown = false;
	float ward_info_chance = 0.0F;
	float ward_info_chance_inc = 0.15F;

	int warden_actual_health; //Marker for warden's actual health while it is -1 (invisible)

	public override string IntroText
	{
		get{
			return "The Warden";
		}
	} 
	public override string EnterText 
	{
		get {
			return "The Warden";
		}
	}
	public override string ExitText 
	{
		get {
			return "Warden Defeated!";
		}
	}

	public override IEnumerator OnStart()
	{
		warden_actual_health = Slot1.Required;
		
		Slot1.Timer = 0;
		Slot1.Activate();
		Slot1.Current = -1;
		Timer = 0;

		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Excellent work, making it this far.",  Slot1, true, 1.4F);
		tute.AddQuote("But I can't let you go any further.",  Slot1, true, 1.4F);
		tute.AddQuote("The more mana you absorb, the stronger you'll be...",  Slot1, true, 1.4F);
		tute.AddQuote("And I can't have you escaping back to the surface.",  Slot1, true, 1.4F);

		tute.AddQuote("So I'll just get rid of you now.",  Slot1, true, 1.4F);

		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		Slot2.Timer = 0;
		Slot2.Activate();
		Slot3.Timer = 0;
		Slot3.Activate();
		
		Slot1.Current = warden_actual_health;
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

		yield return StartCoroutine(UIManager.instance.Alert(1.1F, false, ExitText));

		int tx = ((TileMaster.Grid.Size[0]-1) / 2) - 1;
		int ty = ((TileMaster.Grid.Size[1]-1) / 2) - 1;
		TileMaster.instance.ReplaceTile(tx, ty, TileMaster.Types["chest"], GENUS.ALL, 3, 10 + (int)GameManager.Difficulty);
	}


}
