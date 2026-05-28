using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxSetObjectPlus : ffxPlusBase
{
	public scrDecorationManager decManager;

	public List<string> targetTags = new List<string>();

	public Color planetColor;

	public bool planetColorUsed = true;

	public Color planetTailColor;

	public bool planetTailColorUsed = true;

	public float trackAngle;

	public bool trackAngleUsed = true;

	public FloorDecorationColorType trackColorType;

	public bool trackColorTypeUsed = true;

	public Color trackColor;

	public bool trackColorUsed = true;

	public Color secondaryTrackColor;

	public bool secondaryTrackColorUsed = true;

	public float trackColorAnimDuration;

	public bool trackColorAnimDurationUsed = true;

	public float trackOpacity;

	public bool trackOpacityUsed = true;

	public TrackStyle trackStyle;

	public bool trackStyleUsed = true;

	public CustomFloorIcon trackIcon;

	public bool trackIconUsed = true;

	public float trackIconAngle;

	public bool trackIconAngleUsed = true;

	public bool trackIconFlipped;

	public bool trackIconFlippedUsed = true;

	public bool trackRedSwirl;

	public bool trackRedSwirlUsed = true;

	public bool trackGraySetSpeedIcon;

	public bool trackGraySetSpeedIconUsed = true;

	public bool trackGlowEnabled;

	public bool trackGlowEnabledUsed = true;

	public Color trackGlowColor;

	public bool trackGlowColorUsed = true;

	public bool trackIconOutlines;

	public bool trackIconOutlinesUsed = true;

	protected override IEnumerable<Tween> eventTweens => decManager.GetTaggedDecorations(targetTags).SelectMany(delegate(scrDecoration dec)
	{
		Dictionary<TweenType, Tween>.ValueCollection values = dec.eventTweens.Values;
		IEnumerable<Tween> second;
		if (!(dec is scrObjectDecoration scrObjectDecoration2))
		{
			second = Enumerable.Empty<Tween>();
		}
		else
		{
			IEnumerable<Tween> values2 = scrObjectDecoration2.floor.moveTweens.Values;
			second = values2;
		}
		return values.Concat(second);
	});

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low && (ADOBase.isOfficialLevel || Persistence.forceVisualSettings))
		{
			return;
		}
		AdjustDurationForHardbake();
		foreach (scrObjectDecoration dec in decManager.GetTaggedDecorations<scrObjectDecoration>(targetTags))
		{
			Dictionary<ObjectDecorationTweenType, Tween> objectTweens = dec.objectTweens;
			switch (dec.objectType)
			{
			case ObjectDecorationType.Planet:
				if (planetColorUsed)
				{
					if (objectTweens.TryGetValue(ObjectDecorationTweenType.PlanetColor, out var value6))
					{
						value6.Kill(complete: true);
					}
					objectTweens[ObjectDecorationTweenType.PlanetColor] = DOTween.To(() => dec.GetColor(), delegate(Color x)
					{
						dec.SetPlanetColor(x);
					}, planetColor, duration).SetEase(ease).Done();
				}
				if (planetTailColorUsed)
				{
					if (objectTweens.TryGetValue(ObjectDecorationTweenType.PlanetTailColor, out var value7))
					{
						value7.Kill(complete: true);
					}
					objectTweens[ObjectDecorationTweenType.PlanetTailColor] = DOTween.To(() => dec.planetTailColor, delegate(Color x)
					{
						dec.SetPlanetTailColor(x);
					}, planetTailColor, duration).SetEase(ease).Done();
				}
				break;
			case ObjectDecorationType.Floor:
				if (trackAngleUsed)
				{
					if (objectTweens.TryGetValue(ObjectDecorationTweenType.TrackAngle, out var value))
					{
						value.Kill(complete: true);
					}
					objectTweens[ObjectDecorationTweenType.TrackAngle] = DOTween.To(() => dec.GetFloorAngle(), delegate(float x)
					{
						dec.SetFloorAngle(x);
					}, trackAngle, duration).SetEase(ease).Done();
				}
				if (trackColorTypeUsed || trackColorUsed || secondaryTrackColorUsed || trackColorAnimDurationUsed)
				{
					if (dec.floor.moveTweens.TryGetValue(TweenType.Color, out var value2))
					{
						value2.Kill(complete: true);
					}
					if (!trackColorTypeUsed)
					{
						trackColorType = (FloorDecorationColorType)Enum.Parse(typeof(TrackColorType), dec.floor.specialColorType.ToString());
					}
					if (!trackColorUsed)
					{
						trackColor = dec.floor.specialColor1;
					}
					if (!secondaryTrackColorUsed)
					{
						secondaryTrackColor = dec.floor.specialColor2;
					}
					dec.SetFloorColor(trackColor, secondaryTrackColor, trackColorType, trackColorAnimDurationUsed ? new float?(trackColorAnimDuration) : ((float?)null), duration, ease);
				}
				if (trackOpacityUsed)
				{
					if (objectTweens.TryGetValue(ObjectDecorationTweenType.TrackOpacity, out var value3))
					{
						value3.Kill(complete: true);
					}
					objectTweens[ObjectDecorationTweenType.TrackOpacity] = DOTween.To(() => dec.GetAlpha(), delegate(float x)
					{
						dec.SetOpacity(x);
					}, trackOpacity, duration).SetEase(ease).Done();
				}
				if (trackStyleUsed)
				{
					dec.SetFloorStyle(trackStyle);
				}
				if (trackRedSwirlUsed)
				{
					dec.SetFloorRedSwirl(trackRedSwirl);
				}
				if (trackIconUsed || trackGraySetSpeedIconUsed || trackRedSwirlUsed)
				{
					dec.SetFloorIcon(trackIconUsed ? trackIcon : dec.floorCustomIcon, trackGraySetSpeedIconUsed ? trackGraySetSpeedIcon : floor.graySetSpeedIcon);
					if (!trackIconAngleUsed)
					{
						dec.SetFloorIconAngle(dec.floorIconAngle);
					}
				}
				if (trackIconAngleUsed)
				{
					if (objectTweens.TryGetValue(ObjectDecorationTweenType.TrackIconAngle, out var value4))
					{
						value4.Kill(complete: true);
					}
					objectTweens[ObjectDecorationTweenType.TrackIconAngle] = DOTween.To(() => dec.floorIconAngle, delegate(float x)
					{
						dec.SetFloorIconAngle(x);
					}, trackIconAngle, duration).SetEase(ease).Done();
				}
				if (trackIconFlippedUsed)
				{
					dec.SetFloorIconFlipped(trackIconFlipped);
				}
				if (trackGlowEnabledUsed)
				{
					dec.SetFloorGlowEnabled(trackGlowEnabled);
				}
				if (trackGlowColorUsed)
				{
					if (objectTweens.TryGetValue(ObjectDecorationTweenType.TrackGlowColor, out var value5))
					{
						value5.Kill(complete: true);
					}
					objectTweens[ObjectDecorationTweenType.TrackGlowColor] = DOTween.To(() => dec.floorGlowColor, delegate(Color x)
					{
						dec.SetFloorGlowColor(x);
					}, trackGlowColor, duration).SetEase(ease).Done();
				}
				if (trackIconOutlinesUsed)
				{
					dec.SetFloorIconOutlines(trackIconOutlines);
				}
				break;
			}
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		decManager = scnGame.suitableDecManager;
		duration = evnt.GetFloat("duration") * crotchet;
		string[] array = ((string)evnt["tag"]).Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			array = new string[1] { "NO TAG" };
		}
		targetTags.AddRange(array);
		ease = (Ease)evnt["ease"];
		planetColor = evnt.GetColor("planetColor");
		planetColorUsed = !evnt.disabled["planetColor"];
		planetTailColor = evnt.GetColor("planetTailColor");
		planetTailColorUsed = !evnt.disabled["planetTailColor"];
		trackAngle = evnt.GetFloat("trackAngle");
		trackAngleUsed = !evnt.disabled["trackAngle"];
		trackColorType = (FloorDecorationColorType)evnt["trackColorType"];
		trackColorTypeUsed = !evnt.disabled["trackColorType"];
		trackColor = evnt.GetColor("trackColor");
		trackColorUsed = !evnt.disabled["trackColor"];
		secondaryTrackColor = evnt.GetColor("secondaryTrackColor");
		secondaryTrackColorUsed = !evnt.disabled["secondaryTrackColor"];
		trackColorAnimDuration = evnt.GetFloat("trackColorAnimDuration");
		trackColorAnimDurationUsed = !evnt.disabled["trackColorAnimDuration"];
		trackOpacity = evnt.GetFloat("trackOpacity") / 100f;
		trackOpacityUsed = !evnt.disabled["trackOpacity"];
		trackStyle = (TrackStyle)evnt["trackStyle"];
		trackStyleUsed = !evnt.disabled["trackStyle"];
		trackIcon = (CustomFloorIcon)evnt["trackIcon"];
		trackIconUsed = !evnt.disabled["trackIcon"];
		trackIconAngle = evnt.GetFloat("trackIconAngle");
		trackIconAngleUsed = !evnt.disabled["trackIconAngle"];
		trackIconFlipped = evnt.GetBool("trackIconFlipped");
		trackIconFlippedUsed = !evnt.disabled["trackIconFlipped"];
		trackRedSwirl = evnt.GetBool("trackRedSwirl");
		trackRedSwirlUsed = !evnt.disabled["trackRedSwirl"];
		trackGraySetSpeedIcon = evnt.GetBool("trackGraySetSpeedIcon");
		trackGraySetSpeedIconUsed = !evnt.disabled["trackGraySetSpeedIcon"];
		trackGlowEnabled = evnt.GetBool("trackGlowEnabled");
		trackGlowEnabledUsed = !evnt.disabled["trackGlowEnabled"];
		trackGlowColor = evnt.GetColor("trackGlowColor");
		trackGlowColorUsed = !evnt.disabled["trackGlowColor"];
		trackIconOutlines = evnt.GetBool("trackIconOutlines");
		trackIconOutlinesUsed = !evnt.disabled["trackIconOutlines"];
	}
}
