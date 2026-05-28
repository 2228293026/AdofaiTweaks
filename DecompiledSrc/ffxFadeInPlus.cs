using UnityEngine;

public class ffxFadeInPlus : ffxPlusBase
{
	public GameObject objFade;

	public float time;

	public float value;

	public override void Awake()
	{
		base.Awake();
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		float num = ((duration != 0f) ? duration : time);
		ffxFadeIn.StartEffectStatic(objFade, num, value);
	}

	public override void ScrubToTime(float t)
	{
	}
}
