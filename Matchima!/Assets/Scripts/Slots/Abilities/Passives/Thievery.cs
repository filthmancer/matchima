using UnityEngine;
using System.Collections;

public class Thievery : Ability {
	public float StealInit;
	private float finalsteal
	{
		get{
			return StealInit * WisdomFactor;
		}
	}

	public override StCon [] Description_Tooltip
	{
		get{
			return new StCon [] {
				new StCon("Steals ", Color.white, false),
				new StCon((finalsteal*100).ToString("0") + "% of Mana Gains", GameData.Colour(GENUS.WIS))
			};
		}
	}


	protected override void Setup()
	{
		base.Setup();

		QuoteGroup steal = new QuoteGroup("Stealing Mana");
		steal.AddQuote("You dont mind if I borrow this?", Parent, false, 2.5F);
		steal.AddQuote("I need it more than you...", Parent, false, 2.5F);
		steal.AddQuote("Thanks for doing the hard work!", Parent, false, 2.5F);
		Parent.Quotes.Special.Add(steal);
	}

	public override void AfterTurnB()
	{
		int total_stolen = 0;
		foreach(Class child in Player.Classes)
		{
			if(child == Parent || child == null) continue;
			if(child.ManaThisTurn > 0)
			{
				int stolen = (int) ((float) child.ManaThisTurn * finalsteal);
				if(child.Meter < stolen) stolen = child.Meter;
				//print("STOLE " + stolen + " FROM " + child.Name + " : " + child.ManaThisTurn);				
				child.AddToMeter(-stolen);
				//total_stolen += stolen;	
				for(int i = 0; i < stolen; i++)
				{
					//TileMaster.instance.CreateMiniTile(UIManager.ClassButtons[child.Index].transform.position,
					//								UIManager.ClassButtons[Parent.Index].transform, 
					//								TileMaster.Genus.Frame[Parent.Index]);

					MoveToPoint mini = TileMaster.instance.CreateMiniTile(	
						UIManager.ClassButtons[child.Index].transform.position,
						UIManager.ClassButtons[Parent.Index].transform, 
						TileMaster.Genus.Frame[Parent.Index]);
					
					mini.Target = Parent;
					mini.SetPath(0.35F, 0.0F, 0.0F, 0.08F);
					mini.SetMethod(() =>{
							if(mini.Target != null) (mini.Target as Class).AddToMeter(stolen);
						}
					);
				}
			}
		}

		if(total_stolen > 0)
		{
			Parent.AddToMeter(total_stolen);
			UIManager.instance.MiniAlert(UIManager.ClassButtons[Parent.Index].transform.position + Vector3.up*0.6F, "Stole " + total_stolen + " Mana!", 40, GameData.Colour(Parent.Genus), 0.8F, 0.1F, true);
			if(Random.value < 0.15F) StartCoroutine(UIManager.instance.Quote(Parent.Quotes.GetSpecial("Stealing Mana").RandomQuote));
		}		
	}
}
