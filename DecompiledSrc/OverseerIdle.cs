using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OverseerIdle : MonoBehaviour
{
	private SpriteRenderer[] allSRs;

	private List<SpriteRenderer> stuff = new List<SpriteRenderer>();

	public Mawaru_Sprite head;

	public Mawaru_Sprite eye;

	public SpriteRenderer glare;

	public Transform torso;

	public float bobHeight = 0.13f;

	public float bobDelay = 0.8f;

	private Color hitColor = new Color(1f, 0.6f, 0.6f, 1f);

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Color bgColor = new Color(0.8f, 0.5f, 0.5f, 1f);

	private Color bgClear = new Color(0.8f, 0.5f, 0.5f, 0f);

	private Tween glareTween;

	private void Awake()
	{
		if (glare != null)
		{
			glare.DOColor(whiteClear, 4f);
		}
		allSRs = GetComponentsInChildren<SpriteRenderer>();
		SpriteRenderer[] array = allSRs;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (spriteRenderer != glare)
			{
				stuff.Add(spriteRenderer);
			}
		}
	}

	public void ChangeLayer(string nl)
	{
		int layer = LayerMask.NameToLayer(nl);
		SpriteRenderer[] array = allSRs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = layer;
		}
	}

	public void Hit()
	{
		DOTween.Shake(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, 0.3f, 0.5f, 50);
		foreach (SpriteRenderer item in stuff)
		{
			DOTween.Sequence().Append(item.DOColor(hitColor, 0f)).Append(item.DOColor(Color.white, 0.4f).SetEase(Ease.Linear));
		}
		head.SetState(1);
		eye.SetState(0);
	}

	public void FadeIn(float dur)
	{
		foreach (SpriteRenderer item in stuff)
		{
			DOTween.Sequence().Append(item.DOColor(whiteClear, 0f).SetEase(Ease.Linear)).Append(item.DOColor(Color.white, dur).SetEase(Ease.Linear));
		}
	}

	public void FadeOut(float dur)
	{
		foreach (SpriteRenderer item in stuff)
		{
			DOTween.Sequence().Append(item.DOColor(whiteClear, dur).SetEase(Ease.Linear));
		}
	}

	public void FadeInBG(float dur)
	{
		foreach (SpriteRenderer item in stuff)
		{
			DOTween.Sequence().Append(item.DOColor(bgClear, 0f).SetEase(Ease.Linear)).Append(item.DOColor(bgColor, dur).SetEase(Ease.Linear));
		}
	}

	public void FadeOutBG(float dur)
	{
		foreach (SpriteRenderer item in stuff)
		{
			DOTween.Sequence().Append(item.DOColor(bgClear, dur).SetEase(Ease.Linear));
		}
	}

	public void Glare()
	{
		if (glare != null && TaroBGScript.instance.songBeat > 10.0)
		{
			glareTween.Kill(complete: true);
			glareTween = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(glare.DOColor(Color.white, 0.05f).SetEase(Ease.InQuad))
				.Append(glare.DOColor(whiteClear, 2f).SetEase(Ease.OutQuad));
		}
	}

	private void Update()
	{
		torso.localPosition = new Vector3(-1.44f, 1.36f + bobHeight * Mathf.Sin(Time.time * bobDelay * (float)Math.PI), 0f);
	}
}
