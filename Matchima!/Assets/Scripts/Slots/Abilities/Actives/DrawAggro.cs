using UnityEngine;
using System.Collections;

public class DrawAggro : Ability {
	public float AggroChange = 1.1F;


	public override void Start()
	{
		base.Start();
		Description_Basic = AggroChange > 0.0F ? "Draws enemy aggro" : "Removes enemy aggro";

	}

	public override void Activate()
	{
		activated = true;
		StartCoroutine(ParticleEffect());

		//Parent.AggroValue += AggroChange;
		Player.instance.ResetStats();
		cooldown_time = cooldown;
		activated = false;
	}

	public IEnumerator ParticleEffect()
	{
		float part_time = 1.2F;

		GameObject part = (GameObject) Instantiate(Particle);
		part.transform.position = UIManager.CrewButtons[Parent.Index].transform.position;
		yield return new WaitForSeconds(part_time);
		Destroy(part);
		yield return null;
	}
}
