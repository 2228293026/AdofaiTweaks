using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrShowOnlyInPhase : ADOBase
{
	public static readonly float FadeDuration = 0.3f;

	public int activephase;

	public bool alsoActivateObjWhenFadeIn;

	public List<SpriteRenderer> spriteList;

	public List<Text> textList;

	public List<TMP_Text> TMPList;

	public List<GameObject> goList;

	public List<Image> imageList;

	private List<float> imageAlphas = new List<float>();

	private bool isVisible = true;

	private void Awake()
	{
		foreach (Image image in imageList)
		{
			imageAlphas.Add(image.color.a);
		}
	}

	private void Update()
	{
		bool flag = ADOBase.levelSelectBase.menuPhase == activephase;
		if (flag != isVisible)
		{
			Fade(flag);
		}
	}

	private void Fade(bool visible)
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		Text component2 = GetComponent<Text>();
		isVisible = visible;
		float endValue = (visible ? 1f : 0f);
		if ((bool)component)
		{
			component.DOFade(endValue, FadeDuration);
		}
		if ((bool)component2)
		{
			component2.DOFade(endValue, FadeDuration);
		}
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.DOFade(endValue, FadeDuration);
			if (visible && alsoActivateObjWhenFadeIn)
			{
				spriteRenderer.gameObject.SetActive(value: true);
			}
		}
		foreach (SpriteRenderer sprite in spriteList)
		{
			sprite?.DOFade(visible ? 1f : 0f, FadeDuration);
		}
		foreach (Text text in textList)
		{
			text?.DOFade(visible ? 1f : 0f, FadeDuration);
		}
		foreach (TMP_Text tMP in TMPList)
		{
			tMP?.DOFade(visible ? 1f : 0f, FadeDuration);
		}
		int num = 0;
		foreach (Image image in imageList)
		{
			image?.DOFade(visible ? imageAlphas[num] : 0f, FadeDuration);
			num++;
		}
		foreach (GameObject go in goList)
		{
			if (visible && alsoActivateObjWhenFadeIn)
			{
				go.SetActive(value: true);
			}
			componentsInChildren = go.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer obj in componentsInChildren)
			{
				float num2 = 1f;
				if (obj.transform.parent.TryGetComponent<scrFloor>(out var component3) && component3.dontChangeMySprite)
				{
					num2 = 0.3f;
				}
				obj.DOFade(visible ? num2 : 0f, FadeDuration);
			}
			Text[] componentsInChildren2 = go.GetComponentsInChildren<Text>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].DOFade(visible ? 1f : 0f, FadeDuration);
			}
		}
	}
}
