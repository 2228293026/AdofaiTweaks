using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrLifePlanet : ADOBase
{
	public Image image;

	private Tween tween;

	public bool dead { get; private set; } = true;

	public void Kill()
	{
		if (!dead)
		{
			tween?.Kill();
			tween = DOTween.Sequence().Insert(0f, image.transform.DOScale(2f, 0.125f).SetEase(Ease.OutSine)).Insert(0f, image.DOFade(0f, 0.25f).SetEase(Ease.OutCubic).From(0.75f));
			scrSfx.instance.PlaySfx("sndVehicleNegative", MixerGroup.SfxParent);
			dead = true;
		}
	}

	public void Revive()
	{
		if (dead)
		{
			tween?.Kill();
			image.transform.localScale = Vector3.one;
			image.rectTransform.anchoredPosition = Vector3.zero;
			image.color = image.color.WithAlpha(1f);
			dead = false;
		}
	}
}
