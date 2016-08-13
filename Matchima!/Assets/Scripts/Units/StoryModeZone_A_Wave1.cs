using UnityEngine;
using System.Collections;

public class StoryModeZone_A_Wave1 : Wave {
	public override IEnumerator OnStart()
	{
		switch(Player.instance.Turns)
		{
			case 0:
				(UIManager.Objects.TopGear as UIGear).Drag = false;
				QuoteGroup tute = new QuoteGroup("Tute");
				tute.AddQuote("Phew! That was quite a drop!",  Player.Classes[1], true, 1.0F);
				tute.AddQuote("...",  Player.Classes[0], true, 1.0F);
				tute.AddQuote("The entrance to the undercity should be this way!",  Player.Classes[1], true, 1.0F);
				yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));

				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Collect 3 or\nmore tiles to\nmake a match", "", true));

			break;
			case 1:
				yield return StartCoroutine(UIManager.instance.Alert(0.3F, "", "Matches can be\ncollected\ndiagonally", "", true));
			break;
		}
		yield return null;
	}
}
