using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ffxFloorDisappearPlus : ffxPlusBase
{
	public float tilesBehind = 4f;

	public TrackAnimationType2 animType;

	public static int maxID_debug = 40;

	protected override IEnumerable<Tween> eventTweens => floor.moveTweens.Values;

	public override void Awake()
	{
		base.Awake();
	}

	public override void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		float num = 60f / (bpm * floor.speed);
		if (floor.seqID != scrLevelMaker.instance.listFloors.Count - 1)
		{
			startTime = (float)floor.nextfloor.entryTime + tilesBehind * num;
		}
		else
		{
			startTime = 0.0;
			triggered = true;
		}
		duration = Mathf.Min(num / cond.song.pitch * 0.5f, 0.5f);
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		if (floor.nextfloor == null)
		{
			return;
		}
		Tween tween = null;
		Tween tween2 = null;
		Tween tween3 = null;
		Tween tween4 = null;
		float angle = Random.Range(-75f, 75f);
		switch (animType)
		{
		case TrackAnimationType2.Retract:
			floor.MoveBackBy(3);
			tween2 = floor.transform.DOScale(Vector3.zero, duration).SetEase(Ease.OutSine).Done();
			tween = floor.transform.DOMove(floor.nextfloor.transform.position, duration).SetEase(Ease.OutSine).Done();
			break;
		case TrackAnimationType2.Scatter:
		{
			floor.MoveToBack();
			float x2 = Random.Range(-4f, 4f);
			float y2 = Random.Range(-4f, 4f);
			tween = floor.transform.DOMove(floor.transform.position + new Vector3(x2, y2), duration).SetEase(Ease.OutSine).Done();
			tween3 = floor.TweenRotation(angle, duration, Ease.OutSine).Done();
			break;
		}
		case TrackAnimationType2.Scatter_Far:
		{
			floor.MoveToBack();
			float x = Random.Range(-8f, 8f);
			float y = Random.Range(-8f, 8f);
			tween = floor.transform.DOMove(floor.transform.position + new Vector3(x, y), duration).SetEase(Ease.OutSine).Done();
			tween3 = floor.TweenRotation(angle, duration, Ease.OutSine).Done();
			break;
		}
		case TrackAnimationType2.Shrink:
			tween2 = floor.transform.DOScale(Vector3.zero, duration).Done();
			break;
		case TrackAnimationType2.Shrink_Spin:
			tween2 = floor.transform.DOScale(Vector3.zero, duration).Done();
			tween3 = floor.TweenRotation(-180f, duration, Ease.OutSine).Done();
			break;
		case TrackAnimationType2.Fade:
			tween4 = floor.TweenOpacity(0f, duration).Done();
			break;
		}
		Dictionary<TweenType, Tween> moveTweens = floor.moveTweens;
		if (tween != null)
		{
			if (moveTweens.ContainsKey(TweenType.Position))
			{
				moveTweens[TweenType.Position].Kill(complete: true);
			}
			moveTweens[TweenType.Position] = tween;
		}
		if (tween2 != null)
		{
			if (moveTweens.ContainsKey(TweenType.Scale))
			{
				moveTweens[TweenType.Scale].Kill(complete: true);
			}
			moveTweens[TweenType.Scale] = tween2;
		}
		if (tween3 != null)
		{
			if (moveTweens.ContainsKey(TweenType.Rotation))
			{
				moveTweens[TweenType.Rotation].Kill(complete: true);
			}
			moveTweens[TweenType.Rotation] = tween3;
		}
		if (tween4 != null)
		{
			if (moveTweens.ContainsKey(TweenType.Opacity))
			{
				moveTweens[TweenType.Opacity].Kill(complete: true);
			}
			moveTweens[TweenType.Opacity] = tween4;
		}
	}
}
