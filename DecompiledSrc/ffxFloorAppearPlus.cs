using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ffxFloorAppearPlus : ffxPlusBase
{
	public scrFloor prevFloor;

	public float tilesAhead = 3f;

	public TrackAnimationType animType;

	protected override IEnumerable<Tween> eventTweens => floor.moveTweens.Values;

	public override void Awake()
	{
		base.Awake();
	}

	public override void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		bool num = animType == TrackAnimationType.Drop || animType == TrackAnimationType.Rise;
		float num2 = 60f / (bpm * floor.speed);
		float num3 = tilesAhead;
		if (num)
		{
			num3 *= 2f;
		}
		startTime = Mathf.Max((float)floor.entryTime - num3 * num2, 0f);
		if (num)
		{
			duration = num2 / cond.song.pitch * tilesAhead;
		}
		else
		{
			duration = Mathf.Min(num2 / cond.song.pitch * 0.5f, 0.5f);
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		Tween tween = null;
		Tween tween2 = null;
		Tween tween3 = null;
		Tween tween4 = null;
		if (animType == TrackAnimationType.Extend)
		{
			if (floor.prevfloor != null)
			{
				floor.transform.position = floor.prevfloor.transform.position;
			}
			floor.transform.eulerAngles = floor.startRot;
			floor.tweenRot = floor.startRot;
			floor.transform.ScaleXY(0f, 0f);
			tween2 = floor.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutSine);
			DOTween.To(() => floor.extendAnim, delegate(float e)
			{
				floor.extendAnim = e;
			}, 1f, duration).SetEase(Ease.OutSine).Done();
		}
		else if (animType == TrackAnimationType.Assemble || animType == TrackAnimationType.Assemble_Far)
		{
			tween3 = floor.TweenRotation(0f, duration, Ease.OutSine);
		}
		else if (animType == TrackAnimationType.Grow)
		{
			tween2 = floor.transform.DOScale(Vector3.one, duration);
		}
		else if (animType == TrackAnimationType.Grow_Spin)
		{
			tween2 = floor.transform.DOScale(Vector3.one, duration);
			tween3 = floor.TweenRotation(0f, duration, Ease.OutSine);
		}
		else if (animType == TrackAnimationType.Fade)
		{
			tween4 = floor.TweenOpacity(1f, duration);
		}
		else if (animType == TrackAnimationType.Drop || animType == TrackAnimationType.Rise)
		{
			tween2 = floor.transform.DOScale(Vector3.one, duration / 8f);
		}
		Ease ease = ((animType == TrackAnimationType.Drop || animType == TrackAnimationType.Rise) ? Ease.Linear : Ease.OutSine);
		tween = floor.transform.DOMove(floor.startPos, duration).SetEase(ease);
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

	public override void ScrubToTime(float t)
	{
		FloorSetup();
		base.ScrubToTime(t);
	}

	public void FloorSetup()
	{
		floor.extendAnim = -1f;
		switch (animType)
		{
		case TrackAnimationType.Extend:
			floor.extendAnim = 0f;
			floor.transform.ScaleXY(0f, 0f);
			break;
		case TrackAnimationType.Grow:
			floor.transform.ScaleXY(0f, 0f);
			break;
		case TrackAnimationType.Grow_Spin:
			floor.transform.ScaleXY(0f, 0f);
			floor.transform.eulerAngles = floor.startRot + Vector3.forward * -180f;
			floor.tweenRot = floor.startRot + Vector3.forward * -180f;
			break;
		case TrackAnimationType.Assemble:
		{
			float x2 = Random.Range(-4f, 4f);
			float y2 = Random.Range(-4f, 4f);
			float num2 = Random.Range(-75f, 75f);
			floor.transform.position = floor.startPos + new Vector3(x2, y2);
			floor.transform.eulerAngles = floor.startRot + Vector3.forward * num2;
			floor.tweenRot = floor.startRot + Vector3.forward * num2;
			break;
		}
		case TrackAnimationType.Assemble_Far:
		{
			float x = Random.Range(-8f, 8f);
			float y = Random.Range(-8f, 8f);
			float num = Random.Range(-75f, 75f);
			floor.transform.position = floor.startPos + new Vector3(x, y);
			floor.transform.eulerAngles = floor.startRot + Vector3.forward * num;
			floor.tweenRot = floor.startRot + Vector3.forward * num;
			break;
		}
		case TrackAnimationType.Fade:
			floor.TweenOpacity(0f, 0f);
			break;
		case TrackAnimationType.Drop:
			floor.transform.position = floor.startPos + Vector3.up * 8f;
			floor.transform.ScaleXY(0f, 0f);
			break;
		case TrackAnimationType.Rise:
			floor.transform.position = floor.startPos - Vector3.up * 8f;
			floor.transform.ScaleXY(0f, 0f);
			break;
		}
	}
}
