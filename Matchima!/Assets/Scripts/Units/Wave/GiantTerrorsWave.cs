using UnityEngine;
using System.Collections;

public class GiantTerrorsWave : Wave {

	public override StCon [] ExitText 
	{
		get {
			return new StCon []{new StCon("Terrors Defeated!")};
		}
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

		if(!HasEntered)
		{
			int xpos = 0, ypos = TileMaster.Grid.Size[1]-2;
			for(int i = 0; i < AllSlots.Length; i++)
			{
				if(AllSlots[i] == null || !AllSlots[i] is WaveTile) continue;
				WaveTile slot = AllSlots[i] as WaveTile;

				GameObject initpart = EffectManager.instance.PlayEffect(UIManager.WaveButtons[Index].transform, Effect.Spell);
				MoveToPoint mp = initpart.GetComponent<MoveToPoint>();
				mp.SetTarget(TileMaster.Tiles[xpos,ypos].transform.position);
				mp.SetPath(30, 0.2F);
				mp.SetTileMethod(TileMaster.Tiles[xpos,ypos], (Tile t) => 
					{
						Tile newtile = TileMaster.instance.ReplaceTile(t, TileMaster.Types[slot.SpeciesFinal], slot.Genus, slot.Scale, slot.FinalValue);
					});

				xpos += 2;
				if(xpos > TileMaster.Grid.Size[0]-2)
				{
					ypos -= 2;
					xpos = 0;
					}
				
				yield return new WaitForSeconds(Time.deltaTime * 20);
			}

		}
		
		HasEntered = true;

		for(int i = 1; i < UIManager.Objects.TopGear[1].Length; i++)
		{
			UIManager.Objects.TopGear[1][i][0].SetActive(false);
		}

		GameManager.instance.paused = false;
		UIManager.Objects.BotGear.SetTween(0, true);
		UIManager.Objects.TopGear.SetTween(0, false);
		UIManager.instance.ScreenAlert.SetTween(0,false);

	}
}
