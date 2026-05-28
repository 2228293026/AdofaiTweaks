using UnityEngine;

public class ffxLowQualityBlizzard : ffxPlusBase
{
	private SpriteRenderer sprFade;

	public float crotchets;

	public Color color;

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low && ADOBase.isOfficialLevel)
		{
			scrFlash.FlashReverse(color, crotchets);
		}
	}
}
