using UnityEngine;
using System.Collections;

public class StoryModeZone_Init : Zone {
	public override IEnumerator Enter()
	{
		switch(CurrentDepthInZone)
		{
			case 0:
			(UIManager.Objects.TopGear as UIGear).Drag = false;

			QuoteGroup tute = new QuoteGroup("Tute");
			tute.AddQuote("",  Player.Classes[0], true, 1.0F);
			

			yield return StartCoroutine(UIManager.instance.Quote(tute.ToArray()));
			break;
			case 2:

			break;
		}
		(UIManager.Objects.TopGear as UIGear).Drag = true;
		yield return StartCoroutine(base.Enter());
	}
}
