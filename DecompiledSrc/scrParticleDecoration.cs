using System;
using ADOFAI;
using JetBrains.Annotations;
using UnityEngine;

public class scrParticleDecoration : scrDecoration
{
	public ParticleSystem particleSystem;

	public Transform particleTransform;

	public SpriteRenderer selectionBorders;

	public GameObject gizmo;

	public Transform gizmoTransform;

	public bool isPreview;

	public Vector2 scale;

	public ParticleSystem.MinMaxGradient colorGradient;

	public override string gameObjectName => sourceLevelEvent.GetString("decorationImage")?.RemoveRichTags();

	public override string decorationName
	{
		get
		{
			string text = sourceLevelEvent.GetString("decorationImage");
			if (string.IsNullOrEmpty(text))
			{
				return RDString.Get("editor.particle");
			}
			return text;
		}
	}

	public ParticleSystemRenderer particleRenderer => particleSystem.GetComponent<ParticleSystemRenderer>();

	public bool autoPlay { get; set; }

	public bool atStart { get; set; }

	public float simulationSpeed { get; set; }

	public override void SetVisible(bool visible)
	{
		particleSystem.gameObject.SetActive(visible);
	}

	public override void SetScale(Vector2 scale)
	{
		scaleVec = scale;
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		shape.scale = scale;
		this.scale = scale;
		particleTransform.localScale = Vector3.one * base.camScaleMultiplier;
		if (ADOBase.isLevelEditor)
		{
			editorCollider.size = scale;
			if (gizmoTransform != null)
			{
				gizmoTransform.localScale = Vector3.one / base.camScaleMultiplier;
			}
		}
	}

	public override float GetAlpha()
	{
		return 1f;
	}

	public override void SetDepth(int depth)
	{
		if (!isPreview)
		{
			string sortingLayerName = ((depth >= 0) ? "Bg" : "Default");
			int layer = ((depth >= 0) ? 9 : 7);
			particleRenderer.sortingLayerName = sortingLayerName;
			particleSystem.gameObject.layer = layer;
			int sortingOrder = -depth;
			particleRenderer.sortingOrder = sortingOrder;
		}
	}

	protected override void ApplyColor()
	{
		ParticleSystem.MainModule main = particleSystem.main;
		main.startColor = colorGradient;
	}

	public void SetSprite([CanBeNull] TextureManager.CustomSprite sprite)
	{
		particleRenderer.material.mainTexture = sprite?.GetSprite(TextureManager.ImageOptions.None)?.texture;
	}

	public void SetMaskingType(MaskingType type)
	{
		switch (type)
		{
		case MaskingType.None:
			particleRenderer.maskInteraction = SpriteMaskInteraction.None;
			break;
		case MaskingType.VisibleInsideMask:
			particleRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
			break;
		case MaskingType.VisibleOutsideMask:
			particleRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
			break;
		case MaskingType.Mask:
			break;
		}
	}

	private void Update()
	{
		if (atStart)
		{
			if (autoPlay && !ADOBase.controller.paused)
			{
				particleSystem.Play();
			}
			autoPlay = false;
		}
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		shape.scale = scale;
		ParticleSystem.MainModule main = particleSystem.main;
		main.simulationSpeed = simulationSpeed * ADOBase.conductor.song.pitch;
		if (ADOBase.isLevelEditor && gizmo != null)
		{
			gizmo.SetActive(ADOBase.isEditingLevel);
		}
	}

	public override Vector2 GetDecorationWorldSize()
	{
		return scale;
	}

	public void ResetParticle(LevelEvent ev, bool restart)
	{
		ParticleSystem particleSystem = this.particleSystem;
		ParticleSystem.MainModule main = particleSystem.main;
		if (restart)
		{
			particleSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleSystem.time = 0f;
			particleSystem.randomSeed = (uint)ev.GetInt("randomSeed");
			particleSystem.useAutoRandomSeed = particleSystem.randomSeed == 0;
			main.duration = ev.GetFloat("playDuration");
		}
		main.loop = ev.GetBool("loop");
		main.useUnscaledTime = isPreview;
		colorGradient = ev.GetMinMaxGradient("color");
		ApplyColor();
		simulationSpeed = ev.GetFloat("simulationSpeed") / 100f;
		main.maxParticles = ev.GetInt("maxParticles");
		main.startLifetime = ev.GetFloatPair("particleLifetime").ToRandomCurve();
		main.startSize = ev.GetFloatPair("particleSize").ToRandomCurve(0.01f);
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		shape.shapeType = (ParticleShape)ev["shapeType"] switch
		{
			ParticleShape.Rectangle => ParticleSystemShapeType.Rectangle, 
			ParticleShape.Circle => ParticleSystemShapeType.Circle, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		shape.radius = ev.GetFloat("shapeRadius") * ADOBase.controller.tileSize;
		shape.scale = (Vector2)ev["scale"] / 100f * ADOBase.controller.tileSize;
		shape.arc = ev.GetFloat("arc");
		shape.arcMode = (ParticleSystemShapeMultiModeValue)ev["arcMode"];
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.rateOverTime = ev.GetFloatPair("emissionRate").ToRandomCurve();
		ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
		velocityOverLifetime.enabled = true;
		Tuple<Vector2, Vector2> tuple = (Tuple<Vector2, Vector2>)ev["velocity"];
		velocityOverLifetime.x = new Tuple<float, float>(tuple.Item1.x, tuple.Item2.x).ToRandomCurve(ADOBase.controller.tileSize);
		velocityOverLifetime.y = new Tuple<float, float>(tuple.Item1.y, tuple.Item2.y).ToRandomCurve(ADOBase.controller.tileSize);
		velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
		ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = particleSystem.limitVelocityOverLifetime;
		limitVelocityOverLifetime.enabled = true;
		limitVelocityOverLifetime.drag = ((Tuple<float, float>)ev["velocityLimitOverLifetime"]).ToLinearCurve(0.01f);
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particleSystem.sizeOverLifetime;
		sizeOverLifetime.enabled = !ev.disabled["sizeOverLifetime"];
		Tuple<float, float> tuple2 = (Tuple<float, float>)ev["sizeOverLifetime"];
		sizeOverLifetime.size = tuple2.ToLinearCurve(0.01f);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		colorOverLifetime.enabled = !ev.disabled["colorOverLifetime"];
		colorOverLifetime.color = ev.GetMinMaxGradient("colorOverLifetime");
		ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = particleSystem.rotationOverLifetime;
		rotationOverLifetime.enabled = true;
		rotationOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, 0f);
		rotationOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
		rotationOverLifetime.z = ev.GetFloatPair("rotationOverTime").ToRandomCurve();
		ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = particleSystem.textureSheetAnimation;
		textureSheetAnimation.enabled = true;
		textureSheetAnimation.mode = ParticleSystemAnimationMode.Grid;
		textureSheetAnimation.rowMode = ParticleSystemAnimationRowMode.Random;
		textureSheetAnimation.cycleCount = 0;
		Vector2 vector = ev.Get<Vector2>("randomTextureTiling");
		textureSheetAnimation.numTilesX = (int)vector.x;
		textureSheetAnimation.numTilesY = (int)vector.y;
		textureSheetAnimation.startFrame = new ParticleSystem.MinMaxCurve(0f, 1f);
		main.startRotation = ev.GetFloatPair("startRotation").ToRandomCurve((float)Math.PI / 180f);
		main.simulationSpace = (ParticleSimulationSpace)ev["simulationSpace"] switch
		{
			ParticleSimulationSpace.Local => ParticleSystemSimulationSpace.Local, 
			ParticleSimulationSpace.World => ParticleSystemSimulationSpace.World, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
