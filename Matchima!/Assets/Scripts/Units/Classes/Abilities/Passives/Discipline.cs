using UnityEngine;
using System.Collections;

public class Discipline : Ability {

	public int KillThreshold = 4;
	public int RewardNumber = 1;
	public int PunishmentNumber = 1;
	public bool Reward = true, Punishment = true;

	public string [] RewardType, PunishmentType;
	public string [] RewardGenus, PunishmentGenus;

	public override void Start()
	{
		base.Start();
		Description_Basic = ">" + KillThreshold + " kills spawns " + RewardNumber + " ";
		for(int i = 0; i < RewardType.Length; i++)
		{
			Description_Basic += RewardType[i];
			if(i == RewardType.Length-2) Description_Basic += " or ";
			else if(i == RewardType.Length-1) Description_Basic += " ";
			else Description_Basic += ", ";
		}
		Description_Basic += "tile.\nLess spawns " + PunishmentNumber + " ";
		for(int i = 0; i < PunishmentType.Length; i++)
		{
			Description_Basic += PunishmentType[i];
			if(i == PunishmentType.Length-2) Description_Basic += " or ";
			else if(i == PunishmentType.Length-1) Description_Basic += " ";
			else Description_Basic += ", ";
		}
		Description_Basic += "tile.";
	}

	public override IEnumerator AfterMatch()
	{
		int kills = Player.Stats.PrevTurnKills;
		if(kills >= KillThreshold && Reward) 
		{
			while(kills >= KillThreshold)
			{
				for(int i = 0; i < RewardNumber;i++)
				{
					int num = Random.Range(0, RewardType.Length);
					string type = RewardType[num];
					string genus = RewardGenus[num];
					TileMaster.instance.QueueTile(TileMaster.Types[type], TileMaster.Genus[genus]);
				}
				kills -= KillThreshold;
			}
		}
		else if(Punishment) 
		{
			for(int i = 0; i < PunishmentNumber;i++)
			{
				int num = Random.Range(0, PunishmentType.Length);
				string type = PunishmentType[num];
				string genus = PunishmentGenus[num];
				TileMaster.instance.QueueTile(TileMaster.Types[type], TileMaster.Genus[genus]);
			}
		}
		yield break;
	}
}
