using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public static class DOTweenExtensions
{
	public static Sequence DORainbow(DOGetter<Color> getter, DOSetter<Color> setter, float duration, float saturation = 1f, float value = 1f, Ease ease = Ease.InOutSine)
	{
		int num = 10;
		float num2 = 1f / (float)num;
		Color baseColor = Color.HSVToRGB(1f, saturation, value);
		Tween[] array = new Tween[num];
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < num; i++)
		{
			Color endValue = Color.HSVToRGB(num2 + num2 * (float)i, saturation, value);
			array[i] = DOTween.To(getter, setter, endValue, num2 * duration).SetEase(ease);
			sequence.Append(array[i]);
		}
		sequence.OnPlay(delegate
		{
			setter(baseColor);
		});
		sequence.SetLoops(-1, LoopType.Restart);
		return sequence;
	}

	public static Sequence DORainbow(this SpriteRenderer target, float duration, float saturation = 1f, float value = 1f, Ease ease = Ease.InOutSine)
	{
		Sequence sequence = DORainbow(() => target.color, delegate(Color x)
		{
			target.color = x.WithAlpha(target.color.a);
		}, duration, saturation, value, ease);
		sequence.SetTarget(target);
		return sequence;
	}

	public static Sequence DORainbow(this Image target, float duration, float saturation = 1f, float value = 1f, Ease ease = Ease.InOutSine)
	{
		Sequence sequence = DORainbow(() => target.color, delegate(Color x)
		{
			target.color = x.WithAlpha(target.color.a);
		}, duration, saturation, value, ease);
		sequence.SetTarget(target);
		return sequence;
	}

	public static TweenerCore<float, float, FloatOptions> DOFadeStop(this AudioSource target, float duration)
	{
		TweenerCore<float, float, FloatOptions> tweenerCore = target.DOFade(0f, duration).OnComplete(delegate
		{
			target.Stop();
		});
		tweenerCore.SetTarget(target);
		return tweenerCore;
	}
}
