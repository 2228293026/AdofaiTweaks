using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace ADOFAI.DOTweenExt;

public class MinMaxCurveTween : ABSTweenPlugin<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions>
{
	public static readonly MinMaxCurveTween Plugin = new MinMaxCurveTween();

	private static ParticleSystem.MinMaxCurve Plus(ParticleSystem.MinMaxCurve left, ParticleSystem.MinMaxCurve right)
	{
		return new ParticleSystem.MinMaxCurve(left.constantMin + right.constantMin, left.constantMax + right.constantMax);
	}

	private static ParticleSystem.MinMaxCurve Minus(ParticleSystem.MinMaxCurve left, ParticleSystem.MinMaxCurve right)
	{
		return new ParticleSystem.MinMaxCurve(left.constantMin - right.constantMin, left.constantMax - right.constantMax);
	}

	public override void Reset(TweenerCore<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions> t)
	{
	}

	public override void SetFrom(TweenerCore<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions> t, bool isRelative)
	{
		ParticleSystem.MinMaxCurve endValue = t.endValue;
		t.endValue = t.getter();
		t.startValue = (isRelative ? Plus(t.endValue, endValue) : endValue);
		t.setter(t.startValue);
	}

	public override void SetFrom(TweenerCore<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions> t, ParticleSystem.MinMaxCurve fromValue, bool setImmediately, bool isRelative)
	{
		t.startValue = fromValue;
		if (setImmediately)
		{
			t.setter(fromValue);
		}
	}

	public override ParticleSystem.MinMaxCurve ConvertToStartValue(TweenerCore<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions> t, ParticleSystem.MinMaxCurve value)
	{
		return value;
	}

	public override void SetRelativeEndValue(TweenerCore<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions> t)
	{
		t.endValue = Plus(t.startValue, t.changeValue);
	}

	public override void SetChangeValue(TweenerCore<ParticleSystem.MinMaxCurve, ParticleSystem.MinMaxCurve, NoOptions> t)
	{
		t.changeValue = Plus(t.endValue, t.startValue);
	}

	public override float GetSpeedBasedDuration(NoOptions options, float unitsXSecond, ParticleSystem.MinMaxCurve changeValue)
	{
		return unitsXSecond;
	}

	public override void EvaluateAndApply(NoOptions options, Tween t, bool isRelative, DOGetter<ParticleSystem.MinMaxCurve> getter, DOSetter<ParticleSystem.MinMaxCurve> setter, float elapsed, ParticleSystem.MinMaxCurve startValue, ParticleSystem.MinMaxCurve changeValue, float duration, bool usingInversePosition, int newCompletedSteps, UpdateNotice updateNotice)
	{
		float num = EaseManager.Evaluate(t, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
		startValue.constantMin += changeValue.constantMin * num;
		startValue.constantMax += changeValue.constantMax * num;
		setter(startValue);
	}
}
