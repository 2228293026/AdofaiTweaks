using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxRecolorFloorPlus : ffxPlusBase
{
	private scrLevelMaker levelMaker;

	public int start;

	public int end;

	public Color color1 = Color.clear;

	public Color color2 = Color.clear;

	public TrackColorType colorType;

	public float colorAnimDuration;

	public TrackColorPulse pulseType;

	public int pulseLength = 10;

	public TrackStyle style;

	public float glowMult = 1f;

	public int gapLength;

	private readonly List<TweenType> tweenTypes = new List<TweenType>
	{
		TweenType.Color,
		TweenType.Glow
	};

	protected override IEnumerable<Tween> eventTweens => levelMaker.listFloors.Where((scrFloor f) => f.seqID >= start && f.seqID <= end).SelectMany((scrFloor f) => from t in f.moveTweens
		where tweenTypes.Contains(t.Key)
		select t.Value);

	public override void Awake()
	{
		base.Awake();
		levelMaker = ADOBase.lm;
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		if (end < start)
		{
			int num = end;
			end = start;
			start = num;
		}
		for (int i = start; i <= end; i += 1 + gapLength)
		{
			scrFloor target = levelMaker.listFloors[i];
			base.enabled = false;
			target.styleNum = (int)style;
			target.UpdateAngle(rotate: false);
			target.SetTrackStyle(style);
			tweenTypes.ForEach(delegate(TweenType t)
			{
				if (target.moveTweens.TryGetValue(t, out var value))
				{
					value.Kill(complete: true);
				}
			});
			target.ColorFloor(colorType, color1, color2, colorAnimDuration / cond.song.pitch, pulseType, pulseLength, start, duration, ease);
			target.moveTweens[TweenType.Glow] = DOTween.To(() => target.glowMultiplier, delegate(float x)
			{
				target.glowMultiplier = x;
			}, glowMult, duration).SetEase(ease).Done();
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
		start = scnGame.IDFromTile(evnt.GetTile("startTile"), floorID, floors);
		end = scnGame.IDFromTile(evnt.GetTile("endTile"), floorID, floors);
		evnt.TryGetAndSet("gapLength", ref gapLength);
		color1 = evnt.GetColor("trackColor");
		color2 = evnt.GetColor("secondaryTrackColor");
		colorAnimDuration = evnt.GetFloat("trackColorAnimDuration");
		colorType = (TrackColorType)evnt["trackColorType"];
		pulseType = (TrackColorPulse)evnt["trackColorPulse"];
		pulseLength = (int)evnt["trackPulseLength"];
		style = (TrackStyle)evnt["trackStyle"];
		if (evnt.TryGet<float>("trackGlowIntensity", out var output))
		{
			glowMult = output / 100f;
		}
	}
}
