using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WardenIntro : Wave {

		public override IEnumerator OnStart()
		{
			Slot1.Timer = 0;
			Slot1.Activate();

			QuoteGroup tute = new QuoteGroup("Tute");
			tute.AddQuote("Ten Hut!",  Slot1, true, 1.4F);
			tute.AddQuote("Alright scoundrel, welcome to the Undercity.",  Slot1, true, 1.4F);
			tute.AddQuote("The Council has judged your crime and sentenced you here.",  Slot1, true, 1.4F);
			tute.AddQuote("You must clear the Undercity of this filthy plague of Mana!",  Slot1, true, 1.4F);

			int rand = Random.Range(0,4);
			switch(rand)
			{
				case 0:
					tute.AddQuote("...",  Player.Classes[0], true, 1.4F, 0.35F);
					tute.AddQuote("Well? Don't just stand there!",  Slot1, true, 1.4F);
				break;
				case 1:
					tute.AddQuote("All this work is beneath me.",  Player.Classes[1], true, 1.4F);
					tute.AddQuote("Keep it down vermin!",  Slot1, true, 1.4F);
				break;
				case 2:
					tute.AddQuote("Ohh man, I can smell... mana!",  Player.Classes[2], true, 1.4F);
					tute.AddQuote("Keep quiet, you fool!",  Slot1, true, 1.4F);
				break;
				case 3:
					tute.AddQuote("This oppressive rule won't keep the people down!", Player.Classes[3], true, 1.4F);
					tute.AddQuote("Save it for your freeloading pals!", Slot1, true, 1.4F);
				break;
			}
			
			tute.AddQuote("Get matching!",  Slot1, true, 1.4F);
			//tute.AddQuote("Drag through Mana to collect it!",  Slot1, true, 1.4F);
			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

		}

		public override IEnumerator BeginTurn()
		{
			//yield return StartCoroutine(base.BeginTurn());
			switch(Player.instance.Turns)
			{
				case 2:
					CameraUtility.instance.ScreenShake(0.6F, 1.4F);
					yield return new WaitForSeconds(Time.deltaTime * 30);
					QuoteGroup tute = new QuoteGroup("Tute");

					int rand = Random.Range(0,4);
					switch(rand)
					{
						case 0:
							tute.AddQuote("...  ?", Player.Classes[0], true, 1.4F, 0.35F);
						break;
						case 1:
							tute.AddQuote("What in the gears was that?", Player.Classes[1], true, 1.4F);
						break;
						case 2:
							tute.AddQuote("A vorpus calamity! Amazing!", Player.Classes[2], true, 1.4F);
						break;
						case 3:
							tute.AddQuote("This dungeon's unstable!", Player.Classes[3], true, 1.4F);
							tute.AddQuote("... like this regime!", Player.Classes[3], true, 1.4F);
						break;
					}
					
					
					tute.AddQuote("Oh, nice going prisoners! You've disturbed the wildlife!", Slot1, true, 1.4F);
					tute.AddQuote("Well, you'd better take care of it. I'll, uhh... scout ahead.", Slot1, true, 1.4F);
					yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

					tute = new QuoteGroup("Tute");
					tute.AddQuote("But that's the way we came in!", Player.Classes[1], true, 1.4F);
					yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

					AddPoints(-1);
				break;
			}
			yield return null;
		}
	}
