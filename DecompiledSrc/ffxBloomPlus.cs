using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxBloomPlus : ffxPlusBase
{
	public bool enableBloom;

	public float threshold;

	public float intensity;

	public Color color;

	private static VideoBloom videoBloom;

	private static Tween thresholdTween;

	private static Tween intensityTween;

	private static Tween colorTween;

	protected override IEnumerable<Tween> eventTweens => new Tween[3] { thresholdTween, intensityTween, colorTween };

	public override void Awake()
	{
		base.Awake();
		if (videoBloom == null)
		{
			videoBloom = cam.GetComponent<VideoBloom>();
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		thresholdTween?.Kill(complete: true);
		intensityTween?.Kill(complete: true);
		colorTween?.Kill(complete: true);
		videoBloom.enabled = enableBloom;
		thresholdTween = DOTween.To(() => videoBloom.Threshold, delegate(float t)
		{
			videoBloom.Threshold = t;
		}, threshold, duration).SetEase(ease).Done();
		intensityTween = DOTween.To(() => videoBloom.MasterAmount, delegate(float i)
		{
			videoBloom.MasterAmount = i;
		}, intensity, duration).SetEase(ease).Done();
		colorTween = DOTween.To(() => videoBloom.Tint, delegate(Color c)
		{
			videoBloom.Tint = c;
		}, color, duration).SetEase(ease).Done();
	}

	public override void Decode(LevelEvent evnt)
	{
		enableBloom = evnt.GetBool("enabled");
		threshold = evnt.GetFloat("threshold") / 100f;
		intensity = evnt.GetFloat("intensity") / 100f;
		color = evnt.GetColor("color");
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
	}
}
