using DG.Tweening;
using UnityEngine;

public class scrACGear : ADOBase
{
	public DOTweenAnimation rotAnim;

	public int beatInBar = 3;

	private Tween currTween;

	private AnimationCurve animCurve;

	private float dur;

	private Vector3 targetRot;

	private void Start()
	{
		ADOBase.conductor.onBeats.Add(this);
		animCurve = rotAnim.easeCurve;
		dur = rotAnim.duration;
		targetRot = rotAnim.endValueV3;
		rotAnim.DOKill();
		dur *= ADOBase.conductor.song.pitch;
	}

	public override void OnBeat()
	{
		if (ADOBase.conductor.hasSongStarted && ADOBase.conductor.beatNumber >= 0 && (ADOBase.conductor.beatNumber - beatInBar + 1) % 4 == 0)
		{
			if (currTween != null)
			{
				currTween.Kill();
			}
			currTween = base.transform.DOLocalRotate(targetRot, dur).SetRelative(isRelative: true).SetEase(animCurve);
		}
	}
}
