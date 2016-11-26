using UnityEngine;
using System.Collections;

public class Heal : Powerup {

	public int [] HealPower = new int []
	{
		40,
		60,
		85
	};

	public float [] HealResist = new float []
	{
		0.00F,
		0.001F,
		0.003F
	};

	protected override IEnumerator Minigame(int Level)
	{
		int HealTotal = HealPower[Level-1];
		GameManager.instance.paused = true;
		
		yield return StartCoroutine(PowerupStartup());

		float final_ratio = 0.0F;
		UIObj MGame = (UIObj)Instantiate(MinigameObj[0]);
		MGame.transform.SetParent(UIManager.Objects.MainUI.transform);
		MGame.transform.localScale = Vector3.one;
		MGame.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		MGame.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		MGame.AddAction(UIAction.MouseDown, () => 
		{
			AudioManager.instance.PlayClipOn(this.transform, "Powerup", "HealHeartbeat");
			final_ratio += 0.07F;
			MGame.transform.localScale += Vector3.one * 0.03F;
			MiniAlertUI alert  = UIManager.instance.MiniAlert(PlayerControl.InputPos+Vector3.up,
			(int)(final_ratio * HealTotal) + "%", 140, GameData.Colour(Parent.Genus), 0.3F, 0.4F);
			alert.AddJuice(Juice.instance.BounceB, 0.1F);
		});


		MiniAlertUI m = UIManager.instance.MiniAlert(UIManager.Objects.MainUI.transform.position+ Vector3.up*1.8F, "Keep tapping the\nheart to fill it!", 100, GameData.Colour(Parent.Genus), 0.8F, 0.25F);
		m.DestroyOnEnd = false;
		while(!Input.GetMouseButton(0)) yield return null;
		m.PoolDestroy();
		
		float taptimer = 3.0F;
		bool istapping = true;
		while(istapping)
		{
			MGame.transform.localScale *= 1.0F - (Time.deltaTime/3);
			MGame.Img[1].fillAmount = Mathf.Lerp(MGame.Img[1].fillAmount, final_ratio, Time.deltaTime * 5);
			MGame.Txt[0].text ="";// "Healing\n" + (int)(final_ratio * HealTotal) + "%";

			taptimer -= Time.deltaTime;
			if(final_ratio > 0.0F) final_ratio -= HealResist[Level-1];
			if(taptimer <= 0.0F || final_ratio >= 1.0F) istapping = false;
			yield return null;
		}
		Destroy(MGame.gameObject);

		yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		int final = (int) ((float)HealTotal * final_ratio);
		MiniAlertUI finalert  = UIManager.instance.MiniAlert(UIManager.Objects.MainUI.transform.position + Vector3.up * 3.0F,
			final + "% Heal!", 130, GameData.Colour(Parent.Genus), 0.65F, 0.2F);
		finalert.AddJuice(Juice.instance.BounceB, 0.1F);
		yield return new WaitForSeconds(GameData.GameSpeed(0.3F));
		UIManager.instance.ScreenAlert.SetTween(0,false);
		yield return new WaitForSeconds(GameData.GameSpeed(0.2F));
		for(int i = 0; i < Player.ClassTiles.Length; i++)
		{
			CastHeal(Player.ClassTiles[i], final);
			yield return null;
		}
		
		GameManager.instance.paused = false;
		yield return null;
	}
	
	void CastHeal(Tile t, int final)
	{
		GameObject cast = GameData.instance.ActionCaster(Parent._Tile.transform, t, ()=>
		{
			t.AddHealth(GENUS.ALL, final);
		});
	}

}
