using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxShakeScreenPlus : ffxPlusBase
{
	public float strength;

	public float intensity;

	public bool fadeOut;

	private static Tween shakeTween;

	protected override IEnumerable<Tween> eventTweens => new Tween[1] { shakeTween };

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		if (ADOBase.controller.visualEffects == VisualEffects.Minimum || duration == 0f)
		{
			return;
		}
		shakeTween?.Kill(complete: true);
		float multiplier = 0f;
		if (ease != Ease.Linear)
		{
			string text = ease.ToString();
			if (text.StartsWith("InOut"))
			{
				float value = 0f;
				DOTween.To(() => value, delegate(float x)
				{
					value = x;
					multiplier = ((x > 1f) ? (2f - x) : x);
				}, 2f, duration).SetEase(ease);
			}
			else if (text.StartsWith("Out"))
			{
				multiplier = 1f;
				DOTween.To(() => multiplier, delegate(float x)
				{
					multiplier = x;
				}, 0f, duration).SetEase(ease);
			}
			else
			{
				DOTween.To(() => multiplier, delegate(float x)
				{
					multiplier = x;
				}, 1f, duration).SetEase(ease);
			}
		}
		else
		{
			multiplier = 1f;
		}
		shakeTween = DOTween.Shake(() => cam.shake, delegate(Vector3 x)
		{
			cam.shake = x * multiplier;
		}, duration, strength, Mathf.RoundToInt(20f * intensity), 90f, ignoreZAxis: false, fadeOut);
		shakeTween.OnComplete(delegate
		{
			cam.shake = Vector3.zero;
		});
	}

	public override void Decode(LevelEvent evnt)
	{
		intensity = evnt.GetFloat("intensity") / 100f;
		strength = evnt.GetFloat("strength") / 100f;
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
		fadeOut = evnt.GetBool("fadeOut");
	}
}
