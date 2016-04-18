using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : Wave {
	
	public override void OnStart(int _index)
	{
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
		tute.AddQuote("Ten Hut!",  this, true, 1F);
		tute.AddQuote("Alright scoundrel, welcome to the Undercity.",  this, true, 1F);
		tute.AddQuote("The Council has judged your crime and sentenced you here.",  this, true, 1F);
		tute.AddQuote("You must clear the Undercity of this filthy plague of Mana!",  this, true, 1F);
		tute.AddQuote("...",  Player.Classes[0], true, 1F, 0.35F);
		tute.AddQuote("Well? Don't just stand there! Get cleaning!",  this, true, 1F);
		tute.AddQuote("Drag through Mana to collect it!",  this, true, 1F);
		StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

	}

	public override void OnTurn()
	{
		switch(Player.instance.Turns)
		{
			case 1:
			QuoteGroup tute = new QuoteGroup("Tute");
			tute.AddQuote("Good!",  this, true, 1F);
			tute.AddQuote("Mana can be collected in groups of 3 or more!", this, true, 1f);
			tute.AddQuote("You can collect tiles in any direction.", this, true, 1f);
			StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			case 2:
			StartCoroutine(AddRogue());
			break;
			case 3:
			//Player.instance.InitStats.MapSize = new Vector2(6,6);
			//Player.instance.ResetStats();
			break;
			case 4:
			TileMaster.Tiles[1,1].InitStats.Hits = 2;
			TileMaster.Tiles[1,1].CheckStats();
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Some tiles take multiple hits to collect.",  this, true, 1F);
			tute.AddQuote("The number of hits needed will show underneath the tile.",  this, true, 1F);
			StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			case 5:
			StartCoroutine(AddBard());
			break;
			case 7:
			StartCoroutine(AddWizard());
			break;
			case 12:
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Collecting enough mana makes you Level Up!",  this, true, 1F);
			tute.AddQuote("Leveling up increases your stats and creates a magic tile.",  this, true, 1F);
			StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
		}
	}

	IEnumerator AddRogue()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Doing fine, prisoner #3.",  this, true, 1F);
		tute.AddQuote("But your friends aren't pulling their weight!",  this, true, 1F);
		tute.AddQuote("Stop lurking around back there, come do your civic duty!",  this, true, 1F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		Player.instance.AddClassToSlot(1, GameData.instance.GetClass("Rogue"));
		Player.Classes[1].CanCollectMana = false;

		TileMaster.Types["resource"]["Blue"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("This kind of work is really beneath me.",  Player.Classes[1], true, 1F);
		tute.AddQuote("Keep it down vermin!",  this, true, 1F);
		tute.AddQuote("Tiles can only be collected with the same colour.",  this, true, 1F);
		tute.AddQuote("I want to see you all working hard!",  this, true, 1F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		yield return null;
	}

	IEnumerator AddBard()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Oi Yellow! You're gonna help out, right?", Player.Classes[1], true, 1f);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		Player.instance.AddClassToSlot(3, GameData.instance.GetClass("Bard"));
		TileMaster.Types["resource"]["Yellow"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("This oppressive rule won't keep the people down!", Player.Classes[1], true, 1f);
		tute.AddQuote("Save it for your freeloading pals!", this, true, 1f);
		tute.AddQuote("You're here to clean up mana and you'd better do it fast!", this, true, 1f);
		tute.AddQuote("There's worse things down here than hard labour...", this, true, 1f);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
	}

	IEnumerator AddWizard()
	{
		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Someone wake up the vagrant.", this, true, 1f);
		tute.AddQuote("I've tried, but he's not stirring!", Player.Classes[3], true, 1f);
		tute.AddQuote("...", Player.Classes[0], true, 1f);
		tute.AddQuote("Hmm, I know what will work...", Player.Classes[1], true, 1f);
		tute.AddQuote("Wow! Look at all that MANA over THERE!", Player.Classes[1], true, 1f);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		Player.instance.AddClassToSlot(2, GameData.instance.GetClass("Wizard"));
		TileMaster.Types["resource"]["Green"].ChanceInitial = 1.0F;
		Spawner2.GetSpawnables(TileMaster.Types);

		tute = new QuoteGroup("Tute");
		tute.AddQuote("Wuh... MANA! Where!?", Player.Classes[2], true, 1f);
		tute.AddQuote("Easy man, there's plenty to go around.", Player.Classes[3], true, 1f);
		tute.AddQuote("All of you keep it down!", this, true, 1f, 0.08F);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
	}

	IEnumerator Turn3()
	{
		QuoteGroup tute = new QuoteGroup("Tute2");
		tute.AddQuote("Oh, by the way, mana is very unstable", this, true, 1F);
		tute.AddQuote("... and has some side effects.", this, true, 1f);
		tute.AddQuote("Ohhh, I don't feel very well...", Player.Classes[0], true, 1f);
		yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		int x = Utility.RandomInt(TileMaster.Tiles.GetLength(0));
		int y = Utility.RandomInt(TileMaster.Tiles.GetLength(1));
		while((TileMaster.Tiles[x,y].Info._TypeName != "resource" && TileMaster.Tiles[x,y].Point.Scale > 1)  && Player.QueuedSpell(x,y))
		{
			x = Utility.RandomInt(TileMaster.Tiles.GetLength(0));
			y = Utility.RandomInt(TileMaster.Tiles.GetLength(1));				
		}
		Player.QueueSpell(x,y);
		ParticleSystem part = (ParticleSystem) Instantiate(EffectManager.instance.Particles.TouchParticle);
		part.startColor = GameData.Colour(GENUS.STR);
		part.transform.position = UIManager.ClassButtons[Index].transform.position;

		MoveToPoint move = part.GetComponent<MoveToPoint>();
		move.enabled = true;
		move.SetTarget(TileMaster.Tiles[x,y].transform.position);
		move.SetPath(0.9F, true, false, 0.25F);
		move.SetMethod( () => {
			Player.Classes[0].GetSpellTile(x,y,GENUS.STR,10);
			tute = new QuoteGroup("Tute");
			tute.AddQuote("Oh, disgusting! Not again!",  this, true, 1F);
			tute.AddQuote("And all over my new shoes.",  this, true, 1F);
			tute.AddQuote("Collecting enough mana makes you Level Up!",  this, true, 1F);
			StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
		});

		
	}
}
