using DG.Tweening;
using UnityEngine;

public class ffxFlashSprite : ffxPlusBase
{
	public GameObject objFade;

	public float time;

	public override void StartEffect(scrPlanet planet)
	{
		StartEffectStatic(objFade, time);
	}

	public static void StartEffectStatic(GameObject objFade, float time)
	{
		if (!(objFade == null))
		{
			objFade.SetActive(value: true);
			SpriteRenderer component = objFade.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				component.color = component.color.WithAlpha(1f);
				component.DOFade(0f, time);
			}
			SpriteRenderer[] componentsInChildren = objFade.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer obj in componentsInChildren)
			{
				obj.color = obj.color.WithAlpha(1f);
				obj.DOFade(0f, time);
				obj.gameObject.SetActive(value: true);
			}
			for (int j = 0; j < objFade.transform.childCount; j++)
			{
				objFade.transform.GetChild(j).gameObject.SetActive(value: true);
			}
		}
	}
}
