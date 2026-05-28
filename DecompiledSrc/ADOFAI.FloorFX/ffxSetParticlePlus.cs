using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI.DOTweenExt;
using DG.Tweening;
using UnityEngine;

namespace ADOFAI.FloorFX;

public class ffxSetParticlePlus : ffxPlusBase
{
	public List<string> targetTags;

	public scrDecorationManager decManager;

	public ParticleSystem.MinMaxGradient color;

	public ParticleSystem.MinMaxGradient colorOverLifetime;

	public ParticlePlayMode targetMode;

	public int maxParticles;

	public ParticleSystem.MinMaxCurve sizeOverLifetime;

	public ParticleSystem.MinMaxCurve particleLifetime;

	public ParticleSystem.MinMaxCurve particleSize;

	public ParticleSystem.MinMaxCurve rotationOverTime;

	public Tuple<Vector2, Vector2> velocity;

	public ParticleShape shapeType;

	public float shapeRadius;

	public int emissionRate;

	public float simulationSpeed;

	public float arc;

	public ParticleSystemShapeMultiModeValue arcMode;

	public ParticleSystem.MinMaxCurve limitVelocityOverLifetime;

	public bool lockRotation;

	public bool lockScale;

	public bool useColor;

	public bool useColorOverLifetime;

	public bool useTargetMode;

	public bool useMaxParticles;

	public bool useParticleLifetime;

	public bool useParticleSize;

	public bool useSizeOverLifetime;

	public bool useRotationOverTime;

	public bool useVelocity;

	public bool useShapeType;

	public bool useShapeRadius;

	public bool useEmissionRate;

	public bool useSimulationSpeed;

	public bool useArc;

	public bool useArcMode;

	public bool useLimitVelocityOverLifetime;

	public bool useLockRotation;

	public bool useLockScale;

	protected override IEnumerable<Tween> eventTweens => decManager.GetTaggedDecorations(targetTags).SelectMany((scrDecoration d) => d.eventTweens.Values);

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low && (ADOBase.isOfficialLevel || Persistence.forceVisualSettings))
		{
			return;
		}
		foreach (scrParticleDecoration dec in decManager.GetTaggedDecorations<scrParticleDecoration>(targetTags))
		{
			Dictionary<TweenType, Tween> dictionary = dec.eventTweens;
			ParticleSystem particleSystem = dec.particleSystem;
			ParticleSystem.MainModule main = particleSystem.main;
			ParticleSystem.VelocityOverLifetimeModule velocityModule = particleSystem.velocityOverLifetime;
			ParticleSystem.RotationOverLifetimeModule rotationModule = particleSystem.rotationOverLifetime;
			ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
			ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
			if (useLockRotation)
			{
				dec.lockRotation = lockRotation;
			}
			if (useLockScale)
			{
				dec.lockScale = lockScale;
			}
			if (useTargetMode)
			{
				switch (targetMode)
				{
				case ParticlePlayMode.Start:
					particleSystem.Play();
					break;
				case ParticlePlayMode.Stop:
					particleSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
					break;
				case ParticlePlayMode.Clear:
					particleSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
					break;
				}
			}
			if (useMaxParticles)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.MaxParticles)?.Kill();
				dictionary[TweenType.MaxParticles] = DOTween.To(() => main.maxParticles, delegate(int i)
				{
					main.maxParticles = i;
				}, maxParticles, duration).SetEase(ease).Done();
			}
			if (useParticleLifetime)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleLifetime)?.Kill();
				dictionary[TweenType.ParticleLifetime] = DOTween.To(MinMaxCurveTween.Plugin, () => main.startLifetime, delegate(ParticleSystem.MinMaxCurve i)
				{
					main.startLifetime = i;
				}, particleLifetime, duration).SetEase(ease).Done();
			}
			if (useParticleSize)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleSize)?.Kill();
				dictionary[TweenType.ParticleSize] = DOTween.To(MinMaxCurveTween.Plugin, () => main.startSize, delegate(ParticleSystem.MinMaxCurve i)
				{
					main.startSize = i;
				}, particleSize, duration).SetEase(ease).Done();
			}
			if (useRotationOverTime)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleRotation)?.Kill();
				dictionary[TweenType.ParticleRotation] = DOTween.To(MinMaxCurveTween.Plugin, () => rotationModule.z, delegate(ParticleSystem.MinMaxCurve i)
				{
					rotationModule.z = i;
				}, rotationOverTime, duration).SetEase(ease).Done();
			}
			if (useSizeOverLifetime)
			{
				ParticleSystem.SizeOverLifetimeModule sizeOverLifetimeModule = particleSystem.sizeOverLifetime;
				sizeOverLifetimeModule.enabled = true;
				sizeOverLifetimeModule.size = sizeOverLifetime;
			}
			if (useLimitVelocityOverLifetime)
			{
				ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetimeModule = particleSystem.limitVelocityOverLifetime;
				limitVelocityOverLifetimeModule.enabled = true;
				limitVelocityOverLifetimeModule.drag = limitVelocityOverLifetime;
			}
			if (useVelocity)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleVelocityX)?.Kill();
				dictionary[TweenType.ParticleVelocityX] = DOTween.To(MinMaxCurveTween.Plugin, () => velocityModule.x, delegate(ParticleSystem.MinMaxCurve i)
				{
					velocityModule.x = i;
				}, new ParticleSystem.MinMaxCurve(velocity.Item1.x, velocity.Item2.x), duration).SetEase(ease).Done();
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleVelocityY)?.Kill();
				dictionary[TweenType.ParticleVelocityY] = DOTween.To(MinMaxCurveTween.Plugin, () => velocityModule.y, delegate(ParticleSystem.MinMaxCurve i)
				{
					velocityModule.y = i;
				}, new ParticleSystem.MinMaxCurve(velocity.Item1.y, velocity.Item2.y), duration).SetEase(ease).Done();
			}
			if (useShapeType)
			{
				ParticleSystemShapeType particleSystemShapeType = shapeType switch
				{
					ParticleShape.Rectangle => ParticleSystemShapeType.Rectangle, 
					ParticleShape.Circle => ParticleSystemShapeType.Circle, 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				shapeModule.shapeType = particleSystemShapeType;
			}
			if (useColor)
			{
				dec.colorGradient = color;
				main.startColor = color;
			}
			if (useColorOverLifetime)
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = particleSystem.colorOverLifetime;
				colorOverLifetimeModule.color = colorOverLifetime;
			}
			if (useShapeRadius)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleRadius)?.Kill();
				dictionary[TweenType.ParticleRadius] = DOTween.To(() => shapeModule.radius, delegate(float i)
				{
					shapeModule.radius = i;
				}, shapeRadius, duration).SetEase(ease).Done();
			}
			if (useEmissionRate)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleEmissionRate)?.Kill();
				dictionary[TweenType.ParticleEmissionRate] = DOTween.To(() => emissionModule.rateOverTime.constant, delegate(float i)
				{
					emissionModule.rateOverTime = i;
				}, emissionRate, duration).SetEase(ease).Done();
			}
			if (useSimulationSpeed)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleSimulationSpeed)?.Kill();
				dictionary[TweenType.ParticleSimulationSpeed] = DOTween.To(() => dec.simulationSpeed, delegate(float i)
				{
					dec.simulationSpeed = i;
				}, simulationSpeed, duration).SetEase(ease).Done();
			}
			if (useArc)
			{
				CollectionExtensions.GetValueOrDefault<TweenType, Tween>((IReadOnlyDictionary<TweenType, Tween>)dictionary, TweenType.ParticleArc)?.Kill();
				dictionary[TweenType.ParticleArc] = DOTween.To(() => shapeModule.arc, delegate(float i)
				{
					shapeModule.arc = i;
				}, arc, duration).SetEase(ease).Done();
			}
			if (useArcMode)
			{
				shapeModule.arcMode = arcMode;
			}
		}
	}

	public override void ScrubToTime(float time)
	{
		if ((double)time < startTime)
		{
			return;
		}
		if ((double)time < startTime + (double)duration)
		{
			foreach (scrParticleDecoration taggedDecoration in decManager.GetTaggedDecorations<scrParticleDecoration>(targetTags))
			{
				if (taggedDecoration.atStart)
				{
					if (taggedDecoration.autoPlay)
					{
						taggedDecoration.particleSystem.Play();
					}
					taggedDecoration.atStart = false;
				}
				ParticleSystem particleSystem = taggedDecoration.particleSystem;
				float num = particleSystem.time + (float)((double)time - startTime);
				particleSystem.Simulate(num);
				particleSystem.time = num;
			}
		}
		base.ScrubToTime(time);
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = evnt.GetFloat("duration") * crotchet;
		color = evnt.GetMinMaxGradient("color");
		colorOverLifetime = evnt.GetMinMaxGradient("colorOverLifetime");
		decManager = ADOBase.controller.decorationManager;
		targetMode = (ParticlePlayMode)evnt["targetMode"];
		maxParticles = evnt.GetInt("maxParticles");
		particleLifetime = evnt.GetFloatPair("particleLifetime").ToRandomCurve();
		sizeOverLifetime = evnt.GetFloatPair("sizeOverLifetime").ToLinearCurve(0.01f);
		particleSize = evnt.GetFloatPair("particleSize").ToRandomCurve(0.01f);
		rotationOverTime = evnt.GetFloatPair("rotationOverTime").ToRandomCurve();
		velocity = (Tuple<Vector2, Vector2>)evnt["velocity"];
		shapeType = (ParticleShape)evnt["shapeType"];
		shapeRadius = evnt.GetFloat("shapeRadius");
		emissionRate = evnt.GetInt("emissionRate");
		simulationSpeed = evnt.GetFloat("simulationSpeed");
		arc = evnt.GetFloat("arc");
		arcMode = (ParticleSystemShapeMultiModeValue)evnt["arcMode"];
		limitVelocityOverLifetime = evnt.GetFloatPair("velocityLimitOverLifetime").ToLinearCurve(0.01f);
		lockRotation = evnt.GetBool("lockRotation");
		lockScale = evnt.GetBool("lockScale");
		useColor = !evnt.disabled["color"];
		useColorOverLifetime = !evnt.disabled["colorOverLifetime"];
		useTargetMode = !evnt.disabled["targetMode"];
		useMaxParticles = !evnt.disabled["maxParticles"];
		useParticleLifetime = !evnt.disabled["particleLifetime"];
		useParticleSize = !evnt.disabled["particleSize"];
		useSizeOverLifetime = !evnt.disabled["sizeOverLifetime"];
		useRotationOverTime = !evnt.disabled["rotationOverTime"];
		useVelocity = !evnt.disabled["velocity"];
		useShapeType = !evnt.disabled["shapeType"];
		useShapeRadius = !evnt.disabled["shapeRadius"];
		useEmissionRate = !evnt.disabled["emissionRate"];
		useSimulationSpeed = !evnt.disabled["simulationSpeed"];
		useArc = !evnt.disabled["arc"];
		useArcMode = !evnt.disabled["arcMode"];
		useLimitVelocityOverLifetime = !evnt.disabled["velocityLimitOverLifetime"];
		useLockRotation = !evnt.disabled["lockRotation"];
		useLockScale = !evnt.disabled["lockScale"];
		string[] array = ((string)evnt["tag"]).Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			array = new string[1] { "NO TAG" };
		}
		targetTags = new List<string>();
		targetTags.AddRange(array);
		ease = (Ease)evnt["ease"];
	}
}
