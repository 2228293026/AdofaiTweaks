using System.Collections;
using DG.Tweening;
using UnityEngine;

public class scrMissIndicator : ADOBase
{
	private Tween blinkingTween;

	private Color finalColor = new Color(1f, 1f, 1f, 0f);

	public SpriteRenderer sprite;

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		base.transform.rotation = ADOBase.controller.camy.transform.rotation;
	}

	public void StartBlinking()
	{
		blinkingTween = sprite.DOColor(finalColor, 0.6f).SetEase(Ease.InFlash).SetLoops(-1, LoopType.Restart)
			.SetUpdate(isIndependentUpdate: true);
	}

	public void FadeOut()
	{
		if (blinkingTween != null)
		{
			blinkingTween.Kill();
		}
		blinkingTween = sprite.DOColor(finalColor, 0.6f).SetEase(Ease.InFlash).SetUpdate(isIndependentUpdate: true)
			.OnKill(Die);
	}

	public void BlinkForSeconds(float seconds)
	{
		StartCoroutine(BlinkForSecondsCoroutine(seconds));
	}

	private IEnumerator BlinkForSecondsCoroutine(float seconds)
	{
		StartBlinking();
		yield return new WaitForSeconds(seconds);
		FadeOut();
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		blinkingTween?.Kill();
	}
}
