using UnityEngine;

public class ffxRatPhase : ffxPlusBase
{
	public SpriteRenderer target;

	public Sprite newSprite;

	public Color newEyeColor;

	public override void StartEffect(scrPlanet planet)
	{
		target.sprite = newSprite;
		SpriteRenderer[] componentsInChildren = target.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			if (spriteRenderer != target)
			{
				spriteRenderer.color = newEyeColor;
			}
		}
	}
}
