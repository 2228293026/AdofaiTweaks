using DG.Tweening;
using UnityEngine;

public class ffxHueSpriteTween : ffxPlusBase
{
	public GameObject spriteObject;

	public Color color;

	public float time;

	public bool tweenChildren = true;

	private SpriteRenderer sprite;

	public override void StartEffect(scrPlanet planet)
	{
		if (spriteObject == null)
		{
			return;
		}
		spriteObject.SetActive(value: true);
		sprite = spriteObject.GetComponent<SpriteRenderer>();
		if (sprite != null)
		{
			sprite.DOColor(color, time);
		}
		SpriteRenderer[] componentsInChildren = spriteObject.GetComponentsInChildren<SpriteRenderer>();
		if (tweenChildren)
		{
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer obj in array)
			{
				obj.DOColor(color, time);
				obj.gameObject.SetActive(value: true);
			}
		}
		for (int j = 0; j < spriteObject.transform.childCount; j++)
		{
			spriteObject.transform.GetChild(j).gameObject.SetActive(value: true);
		}
	}
}
