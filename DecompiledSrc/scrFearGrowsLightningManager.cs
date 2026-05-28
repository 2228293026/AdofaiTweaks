using UnityEngine;

public class scrFearGrowsLightningManager : ADOBase
{
	public SpriteRenderer[] lightningBolts;

	public GameObject characters;

	private void Awake()
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low)
		{
			return;
		}
		foreach (scrFloor listFloor in ADOBase.lm.listFloors)
		{
			if (listFloor.tapsNeeded == 2)
			{
				listFloor.gameObject.AddComponent<ffxFearGrowsLightning>().lightningBolts = lightningBolts;
			}
		}
		SpriteRenderer[] array = lightningBolts;
		foreach (SpriteRenderer obj in array)
		{
			obj.color = obj.color.WithAlpha(0f);
		}
		characters.SetActive(value: true);
	}
}
