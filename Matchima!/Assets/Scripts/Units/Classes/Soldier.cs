using UnityEngine;
using System.Collections;

public class Soldier : Class {

	public Quote [] QueuedQuotes;
	public Quote RedShop, BlueShop, GreenShop,YellowShop;
	int curr = 0;

	bool first_levelup = false, first_wave = false;
	bool redshopshown, blushopshown, grnshopshown,ylwshopshown;
	public override void StartClass()
	{
		UIManager.instance.Health.gameObject.SetActive(false);
		//UIManager.instance.TimeToWave.enabled = false;

		for(int g = 0; g < TileMaster.Types.Length; g++)
		{
			for(int s = 0; s < TileMaster.Types[g].Length; s++)
			{
				TileMaster.Types[g][s].ChanceAdded = -1;
			}
		}
		TileMaster.Types["Resource"]["Red"].ChanceAdded = 0;
	}

	public override void Update()
	{
		if(GameManager.Wave != null && !first_wave)
		{
			first_wave = true;
			if(GameManager.Wave[0].Required > 8)
			{
				GameManager.Wave[0].Required = 8;
				GameManager.Wave[0].Current = 8;
			}
		}
		/*if(GameManager.Wave != null && !GameManager.instance.WaveActive && !first_levelup)
		{
			StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
													QueuedQuotes[curr++]));
			first_levelup = true;
		}*/

	}

	public override IEnumerator BeginTurn()
	{
		int turn = Player.instance.Turns;
		switch(turn)
		{
			case 0:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++],
														QueuedQuotes[curr++],
														QueuedQuotes[curr++]));
				curr = 4;
				TileMaster.FillGrid_Override = false;
			break;
			case 1:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++],
														QueuedQuotes[curr++],
														QueuedQuotes[curr++]));
				//UIManager.instance.Wheel.gameObject.SetActive(true);
				TileMaster.FillGrid_Override = true;
				Player.Stats.MapSize = new Vector2(4,4);
				TileMaster.instance.MapSize = Player.Stats.MapSize;

				for(int g = 0; g < TileMaster.Types.Length; g++)
				{
					for(int s = 0; s < TileMaster.Types[g].Length; s++)
					{
						TileMaster.Types[g][s].ChanceAdded = 0;
					}
				}
				TileMaster.Types["Health"]["All"].ChanceAdded = -1;

				//Spawner2.GetSpawnables(TileMaster.Types);
				TileMaster.instance.GenerateGrid(null, 0.1F);
			break;
			case 3:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++],
														QueuedQuotes[curr++]));
				UIManager.Objects.GetScoreWindow().gameObject.SetActive(true);
				for(int i = 0; i < 3; i++)
				{
					TileMaster.instance.QueueTile(TileMaster.Types["Resource"], TileMaster.Genus["Yellow"]);
				}
			break;
			case 4:
				TileMaster.instance.QueueTile(TileMaster.Types["Enemy"], TileMaster.Genus["Green"]);
			break;
			case 5:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++]));
				UIManager.instance.Health.gameObject.SetActive(true);
				
			break;
			case 6:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++],
														QueuedQuotes[curr++]));
				TileMaster.instance.QueueTile(TileMaster.Types["Enemy"], TileMaster.Genus["Blue"]);
				TileMaster.instance.QueueTile(TileMaster.Types["Enemy"], TileMaster.Genus["Blue"]);

				for(int i = 0; i < 2; i++)
				{
					TileMaster.instance.QueueTile(TileMaster.Types["Enemy"], TileMaster.Genus["Yellow"]);
				}
			break;
			case 7:
				for(int i = 0; i < 3; i++)
				{
					TileMaster.instance.QueueTile(TileMaster.Types["Enemy"], TileMaster.Genus["Red"]);
				}
			break;
			case 9:

				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++]));
				TileMaster.instance.QueueTile(TileMaster.Types["Health"], TileMaster.Genus["Red"]);
			break;
			case 11:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++]));

				TileMaster.Types["Health"]["Yellow"].ChanceAdded = 0.1F;
				TileMaster.Types["Health"]["Red"].ChanceAdded = 0.1F;
				TileMaster.Types["Health"]["Prism"].ChanceAdded = 0.2F;
				//Spawner2.GetSpawnables(TileMaster.Types);

				for(int i = 0; i < 2; i++)
				{
					TileMaster.instance.QueueTile(TileMaster.Types["Enemy"], TileMaster.Genus["Red"]);
				}

			break;
			case 15:
			break;
			case 16:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++],
														QueuedQuotes[curr++]));	
			break;
			case 34:
				TileMaster.FillGrid_Override = false;
			break;
			case 35:
				StartCoroutine(UIManager.instance.Quote(QueuedQuotes[curr++]));	
				TileMaster.FillGrid_Override = true;
				Player.Stats.MapSize = new Vector2(6,6);
				TileMaster.instance.MapSize = Player.Stats.MapSize;
				//Spawner2.GetSpawnables(TileMaster.Types);
				TileMaster.instance.GenerateGrid(null, 0.1F);
			break;
			case 36:
			
				//TileMaster.instance.QueueTile(TileMaster.Types.TypeOf("All Cross"));
				//TileMaster.instance.QueueTile(TileMaster.Types.TypeOf("Altar"));
				TileMaster.Types["Health"]["All"].ChanceAdded = 0.3F;
				//Spawner2.GetSpawnables(TileMaster.Types);
			break;
		}
		

		yield return null;
	}
}
