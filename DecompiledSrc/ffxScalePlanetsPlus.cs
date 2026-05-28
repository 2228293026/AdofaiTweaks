using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using DG.Tweening;

public class ffxScalePlanetsPlus : ffxPlusBase
{
	public float targetScale = 1f;

	public TargetPlanet targetPlanet;

	private static readonly List<TargetPlanet> targetPlanetValues;

	protected override IEnumerable<Tween> eventTweens => new Tween[3]
	{
		ADOBase.controller.planetRed.scaleTween,
		ADOBase.controller.planetBlue.scaleTween,
		ADOBase.controller.planetGreen.scaleTween
	};

	static ffxScalePlanetsPlus()
	{
		targetPlanetValues = Enum.GetValues(typeof(TargetPlanet)).Cast<TargetPlanet>().ToList();
		targetPlanetValues.Remove(TargetPlanet.All);
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		TweenPlanet(targetPlanet);
	}

	private void TweenPlanet(TargetPlanet targetEnum)
	{
		foreach (scrPlayer item in ADOBase.controller.playerManager)
		{
			PlanetarySystem planetarySystem = item.planetarySystem;
			List<scrPlanet> list = new List<scrPlanet>();
			switch (targetEnum)
			{
			case TargetPlanet.All:
				list = planetarySystem.planetList;
				break;
			case TargetPlanet.FirePlanet:
				list.Add(planetarySystem.planetRed);
				break;
			case TargetPlanet.IcePlanet:
				list.Add(planetarySystem.planetBlue);
				break;
			case TargetPlanet.GreenPlanet:
				list.Add(planetarySystem.planetGreen);
				break;
			}
			foreach (scrPlanet planet in list)
			{
				if (!(planet == null))
				{
					planet.scaleTween.Kill();
					planet.scaleTween = DOTween.To(() => planet.planetScale, delegate(float s)
					{
						planet.planetScale = s;
					}, targetScale, duration).SetEase(ease).OnComplete(delegate
					{
						planet.planetScale = targetScale;
					})
						.Done();
				}
			}
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		targetPlanet = (TargetPlanet)evnt["targetPlanet"];
		targetScale = evnt.GetFloat("scale") / 100f;
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
	}
}
