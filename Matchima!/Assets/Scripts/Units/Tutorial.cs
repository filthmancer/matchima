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

		QuoteGroup tute = new QuoteGroup("Tute");
		tute.AddQuote("Ten Hut!",  this, true, 1F);
		tute.AddQuote("Alright scoundrel, welcome to the Undercity.",  this, true, 1F);
		tute.AddQuote("The Council has judged your crime and sentenced you here.",  this, true, 1F);
		tute.AddQuote("You must clear the Undercity of this filthy plague of Mana!",  this, true, 1F);
		tute.AddQuote("...",  this, true, 1F, 0.35F);
		tute.AddQuote("Well? Don't just stand there! Get cleaning!",  this, true, 1F);
		tute.AddQuote("Collect that mana in groups of 3!",  this, true, 1F);
		StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

	}

	public override void OnTurn()
	{
		switch(Player.instance.Turns)
		{
			case 1:
			QuoteGroup tute = new QuoteGroup("Tute");
			tute.AddQuote("Good! More!",  this, true, 1F);
			tute.AddQuote("You can also match tiles diagonally.", this, true, 1f);
			StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			case 3:
			StartCoroutine(Turn3());
			break;
			case 4:

			break;
		}
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
