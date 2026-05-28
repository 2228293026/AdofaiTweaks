using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ffxFadeIn : ffxPlusBase
{
	public GameObject objFade;

	public float time;

	public float value;

	public bool includeChildren = true;

	public override void StartEffect(scrPlanet planet)
	{
		StartEffectStatic(objFade, time, value, includeChildren);
	}

	public static void StartEffectStatic(GameObject objFade, float time, float value, bool includeChildren = true)
	{
		if (objFade == null)
		{
			return;
		}
		objFade.SetActive(value: true);
		SpriteRenderer component = objFade.GetComponent<SpriteRenderer>();
		CanvasGroup component2 = objFade.GetComponent<CanvasGroup>();
		Text component3 = objFade.GetComponent<Text>();
		Image component4 = objFade.GetComponent<Image>();
		if ((bool)component2)
		{
			component2.DOFade(value, time);
		}
		else if (component != null)
		{
			component.DOFade(value, time);
		}
		else if (component3 != null)
		{
			component3.DOFade(value, time);
		}
		else if (component4 != null)
		{
			component4.DOFade(value, time);
		}
		Transform[] array = ((!includeChildren) ? new Transform[1] { objFade.GetComponent<Transform>() } : objFade.GetComponentsInChildren<Transform>());
		Transform[] array2 = array;
		foreach (Transform obj in array2)
		{
			SpriteRenderer component5 = obj.GetComponent<SpriteRenderer>();
			Text component6 = obj.GetComponent<Text>();
			Image component7 = obj.GetComponent<Image>();
			CanvasGroup component8 = obj.GetComponent<CanvasGroup>();
			if ((bool)component8)
			{
				component8.DOFade(value, time);
			}
			else if (component5 != null)
			{
				component5.DOFade(value, time);
			}
			else if (component6 != null)
			{
				component6.DOFade(value, time);
			}
			else if (component7 != null)
			{
				component7.DOFade(value, time);
			}
			obj.gameObject.SetActive(value: true);
		}
		for (int j = 0; j < objFade.transform.childCount; j++)
		{
			objFade.transform.GetChild(j).gameObject.SetActive(value: true);
		}
	}
}
