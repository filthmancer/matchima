using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : Wave {
	
	public override IEnumerator OnStart()
	{
		IntroAlert = false;
		Slot1.Timer = 0;
		Slot1.Activate();
		Player.instance.InitStats.MapSize = new Vector2(1,3);
		Player.Stats.MapSize = new Vector2(1,3);
		TileMaster.Types["resource"]["Red"].ChanceInitial = 1.0F;
		TileMaster.Types["resource"]["Green"].ChanceInitial = 0.0F;
		TileMaster.Types["resource"]["Blue"].ChanceInitial = 0.0F;
		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 0.0F;

		TileMaster.Types["health"]["Red"].ChanceInitial = 0.0F;
		TileMaster.Types["health"]["Green"].ChanceInitial = 0.0F;
		TileMaster.Types["health"]["Blue"].ChanceInitial = 0.0F;
		TileMaster.Types["health"]["Yellow"].ChanceInitial = 0.0F;
		Player.Classes[0].CanCollectMana = false;
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Ten Hut!",  Slot1, true, 1.4F);
		tute.AddQuote("Alright scoundrel, welcome to the Undercity.",  Slot1, true, 1.4F);
		tute.AddQuote("The Council has judged your crime and sentenced you here.",  Slot1, true, 1.4F);
		tute.AddQuote("You must clear the Undercity of this filthy plague of Mana!",  Slot1, true, 1.4F);
		tute.AddQuote("...",  Player.Classes[0], true, 1.4F, 0.35F);
		tute.AddQuote("Well? Don't just stand there! Get cleaning!",  Slot1, true, 1.4F);
		tute.AddQuote("Drag through Mana to collect it!",  Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		yield break;
	}

	public override IEnumerator BeginTurn()
	{
		yield return StartCoroutine(base.BeginTurn());
		switch(Player.instance.Turns)
		{
			case 1:
			QuoteGroup tute = new QuoteGroup("Tute");
			tute.AddQuote("Good!",  Slot1, true, 1.4F);
			tute.AddQuote("Mana can be collected in groups of 3 or more!", Slot1, true, 1.4F);
			tute.AddQuote("You can collect tiles in any direction.", Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			case 2:
			yield return StartCoroutine(AddRogue());
			break;
			case 3:
			//Player.instance.InitStats.MapSize = new Vector2(6,6);
			//Player.instance.ResetStats();
			break;
			case 4:
			TileMaster.Tiles[1,1].InitStats.Hits = 2;
			TileMaster.Tiles[1,1].CheckStats();
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Some tiles take multiple hits to collect.",  Slot1, true, 1.4F);
			tute.AddQuote("The number of hits needed will show underneath the tile.",  Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			case 5:
			yield return StartCoroutine(AddBard());
			break;
			case 7:
			yield return StartCoroutine(AddWizard());
			break;
			case 8:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Match enemies to attack them!",  Slot1, true, 1.4F);
			tute.AddQuote("Keep their numbers down!",  Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			for(int i = 0; i < Player.Classes.Length; i++)
			{
				Player.Classes[i].CanCollectMana = true;
			}
			break;
			case 9:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Enemy tiles attack your health at the end of every turn.",  Slot1, true, 1.4F);
			tute.AddQuote("If you lose all your health, you'll all die!",  Slot1, true, 1.4F);
			tute.AddQuote("Luckily, you can regain health by grabbing Health Tiles.",  Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			
			case 12:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Collecting enough mana makes you Level Up!",  Slot1, true, 1.4F);
			tute.AddQuote("Leveling up increases your stats and creates a magic tile.",  Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
		}

		yield return null;
	}

	IEnumerator AddRogue()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Doing fine, prisoner #3.",  Slot1, true, 1.4F);
		tute.AddQuote("But your friends aren't pulling their weight!",  Slot1, true, 1.4F);
		tute.AddQuote("Stop lurking around back there, come do your civic duty!",  Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		Player.instance.AddClassToSlot(1, GameData.instance.GetClass("Rogue"));
		Player.Classes[1].CanCollectMana = false;

		TileMaster.Types["resource"]["Blue"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return new WaitForSeconds(1.1F);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("This kind of work is really beneath me.",  Player.Classes[1], true, 1.4F);
		tute.AddQuote("Keep it down vermin!",  Slot1, true, 1.4F);
		tute.AddQuote("Tiles can only be collected with the same colour.",  Slot1, true, 1.4F);
		tute.AddQuote("I want to see you all working hard!",  Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		yield return null;
	}

	IEnumerator AddBard()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Oi Yellow! You're gonna help out, right?", Player.Classes[1], true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		Player.instance.AddClassToSlot(3, GameData.instance.GetClass("Bard"));
		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return new WaitForSeconds(1.1F);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("This oppressive rule won't keep the people down!", Player.Classes[3], true, 1.4F);
		tute.AddQuote("Save it for your freeloading pals!", Slot1, true, 1.4F);
		tute.AddQuote("You're here to clean up mana and you'd better do it fast!", Slot1, true, 1.4F);
		tute.AddQuote("There's worse things down here than hard labour...", Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
	}

	IEnumerator AddWizard()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Someone wake up the vagrant.", Slot1, true, 1.4F);
		tute.AddQuote("I've tried, but he's not stirring!", Player.Classes[3], true, 1.4F);
		tute.AddQuote("...", Player.Classes[0], true, 1.4F);
		tute.AddQuote("Hmm, I know what will work...", Player.Classes[1], true, 1.4F);
		tute.AddQuote("Wow! Look at all that MANA over THERE!", Player.Classes[1], true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		Player.instance.AddClassToSlot(2, GameData.instance.GetClass("Wizard"));
		TileMaster.Types["resource"]["Green"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);
		yield return new WaitForSeconds(1.1F);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("Wuh... MANA! Where!?", Player.Classes[2], true, 1.4F);
		tute.AddQuote("Easy man, there's plenty to go around!", Player.Classes[3], true, 1.4F);
		tute.AddQuote("All of you keep it down!", Slot1, true, 1.4F, 0.08F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		CameraUtility.instance.ScreenShake(0.6F, 1.4F);
		yield return new WaitForSeconds(Time.deltaTime * 30);
		tute = new QuoteGroup("Tute");
		tute.AddQuote("What in the gears was that?", Player.Classes[1], true, 1.4F);
		tute.AddQuote("Oh, nice going prisoners! You've disturbed the wildlife!", Slot1, true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		GameManager.instance.GetWave(GameData.instance.GetWave("Red Grunts"));

		tute = new QuoteGroup("Tute");
		tute.AddQuote("Ugh, what's that foul thing?",  Player.Classes[3], true, 1.4F);
		tute.AddQuote("A Wave of Enemies! Creatures spawned from mana!", Slot1, true, 1.4F);
		tute.AddQuote("They're attacking us!",  Slot1, true, 1.4F);
		StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
	}

	IEnumerator Turn3()
	{
		QuoteGroup tute = new QuoteGroup("Tute2");
		tute.AddQuote("Oh, by the way, mana is very unstable", Slot1, true, 1.4F);
		tute.AddQuote("... and has some side effects.", Slot1, true, 1.4F);
		tute.AddQuote("Ohhh, I don't feel very well...", Player.Classes[0], true, 1.4F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		int x = Utility.RandomInt(TileMaster.Tiles.GetLength(0));
		int y = Utility.RandomInt(TileMaster.Tiles.GetLength(1));
		ParticleSystem part = (ParticleSystem) Instantiate(EffectManager.instance.Particles.TouchParticle);
		part.startColor = GameData.Colour(GENUS.STR);
		part.transform.position = UIManager.ClassButtons[Index].transform.position;

		MoveToPoint move = part.GetComponent<MoveToPoint>();
		move.enabled = true;
		move.SetTarget(TileMaster.Tiles[x,y].transform.position);
		move.SetPath(0.9F, 0.25F);
		move.SetMethod( () => {
			Player.Classes[0].GetSpellTile(x,y,GENUS.STR,10);
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Oh, disgusting! Not again!",  Slot1, true, 1.4F);
			tute.AddQuote("And all over my new shoes.",  Slot1, true, 1.4F);
			tute.AddQuote("Collecting enough mana makes you Level Up!",  Slot1, true, 1.4F);
			StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		});

		
	}
}
