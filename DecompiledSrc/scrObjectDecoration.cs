using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class scrObjectDecoration : scrDecoration
{
	[Header("GameObject")]
	public GameObject planetObject;

	public GameObject floorObject;

	public GameObject bubbleObject;

	public PlanetRenderer planetRenderer;

	public scrFloor floor;

	public PlayerBubble bubble;

	[NonSerialized]
	private GameObject activeObject;

	[Header("Additional Hitbox Properties")]
	public PolygonCollider2D polygonCollider;

	private Collider2D activeEditorCollider;

	[Header("Object Decoration")]
	public ObjectDecorationType objectType;

	public Dictionary<ObjectDecorationTweenType, Tween> objectTweens = new Dictionary<ObjectDecorationTweenType, Tween>();

	[NonSerialized]
	public SpriteRenderer bordersRenderer;

	[NonSerialized]
	public Vector2 bordersScaleMultiplier = Vector2.one;

	public PlanetDecorationColorType planetColorType;

	public Color planetTailColor = Color.white;

	public Color floorGlowColor = Color.white;

	public CustomFloorIcon floorCustomIcon;

	public float floorIconAngle;

	public int appearStartOffset = 8;

	public int appearEndOffset = 2;

	public int disappearOffset = 4;

	public int spawnOffset = 3;

	public override string gameObjectName => sourceLevelEvent["objectType"]?.ToString().RemoveRichTags() ?? "";

	public override string decorationName => RDString.Get("enum.ObjectDecorationType." + gameObjectName);

	private new void Awake()
	{
		if (ADOBase.isLevelEditor && selectionBordersObject != null)
		{
			bordersRenderer = selectionBordersObject.GetComponent<SpriteRenderer>();
		}
		planetRenderer.objectDecoration = this;
	}

	public void SetType(ObjectDecorationType type)
	{
		objectType = type;
		planetObject.SetActive(value: false);
		floorObject.SetActive(value: false);
		bubbleObject.SetActive(value: false);
		switch (type)
		{
		case ObjectDecorationType.Planet:
			activeObject = planetObject;
			activeEditorCollider = (Collider2D)(object)editorCollider;
			break;
		case ObjectDecorationType.Floor:
			activeObject = floorObject;
			activeEditorCollider = (Collider2D)(object)polygonCollider;
			break;
		case ObjectDecorationType.PlayerBubble:
			activeObject = bubbleObject;
			activeEditorCollider = (Collider2D)(object)editorCollider;
			break;
		}
		activeObject.SetActive(value: true);
		childTransform = activeObject.transform;
		if (ADOBase.isEditingLevel)
		{
			SetCollider(enable: true);
		}
	}

	public override bool GetVisible()
	{
		if (rendererEnabled)
		{
			if (GetAlpha() == 0f)
			{
				return objectType == ObjectDecorationType.Planet;
			}
			return true;
		}
		return false;
	}

	public override void SetVisible(bool visible)
	{
		activeObject.SetActive(visible);
		rendererEnabled = visible;
	}

	public Color GetColor()
	{
		return objectType switch
		{
			ObjectDecorationType.Planet => planetRenderer.sprite.color, 
			ObjectDecorationType.Floor => floor.floorRenderer.color, 
			_ => Color.white, 
		};
	}

	public override float GetAlpha()
	{
		return objectType switch
		{
			ObjectDecorationType.Planet => planetRenderer.sprite.color.a, 
			ObjectDecorationType.Floor => floor.opacity, 
			_ => 1f, 
		};
	}

	public override void SetDepth(int depth)
	{
		string sortingLayerName = ((depth >= 0) ? "Bg" : "Default");
		int layer = ((depth >= 0) ? 9 : 7);
		int sortingOrder = -depth;
		if (syncFloorDepth)
		{
			sortingLayerName = parentFloor.floorRenderer.renderer.sortingLayerName;
			layer = parentFloor.gameObject.layer;
			sortingOrder = parentFloor.floorRenderer.sortingOrder;
		}
		switch (objectType)
		{
		case ObjectDecorationType.Floor:
			((MeshRenderer)floor.floorRenderer.renderer).sortingLayerName = sortingLayerName;
			floor.gameObject.layer = layer;
			floor.SetSortingOrder(sortingOrder);
			break;
		case ObjectDecorationType.Planet:
			if (!ADOBase.controller.legacyTween)
			{
				Renderer[] appearanceRenderers = planetRenderer.appearanceRenderers;
				foreach (Renderer obj in appearanceRenderers)
				{
					obj.sortingLayerName = sortingLayerName;
					obj.gameObject.layer = layer;
					obj.sortingOrder = sortingOrder;
				}
			}
			break;
		}
	}

	protected override void ApplyColor()
	{
		if (objectType == ObjectDecorationType.Floor)
		{
			SetFloorOpacity(opacity);
			SetFloorGlowColor(floorGlowColor);
		}
	}

	public override void SetScale(Vector2 scale)
	{
		base.SetScale(scale);
		bordersScaleMultiplier = Vector2.one;
		switch (objectType)
		{
		case ObjectDecorationType.Planet:
			editorCollider.size = planetRenderer.transform.localScale;
			break;
		case ObjectDecorationType.Floor:
		{
			FloorMesh floorMesh = ((FloorMeshRenderer)floor.floorRenderer).floorMesh;
			if (floorMesh.cacheKey != null && FloorMesh.cache.TryGetValue(floorMesh.cacheKey, out var value))
			{
				polygonCollider.points = value.polygon;
			}
			bordersScaleMultiplier.x *= ADOBase.controller.tileSize;
			break;
		}
		}
	}

	public override void SetCollider(bool enable)
	{
		((Behaviour)(object)editorCollider).enabled = false;
		((Behaviour)(object)polygonCollider).enabled = false;
		((Component)(object)editorCollider).gameObject.SetActive(enable);
		((Behaviour)(object)activeEditorCollider).enabled = enable;
	}

	private void UpdateTailVisibility()
	{
		planetRenderer.tailParticles.gameObject.SetActive(!ADOBase.isEditingLevel);
		planetRenderer.sparks.gameObject.SetActive(!ADOBase.isEditingLevel && GetAlpha() != 0f);
	}

	public void SetPlanetColorType(PlanetDecorationColorType colorType)
	{
		planetColorType = colorType;
		if (colorType != PlanetDecorationColorType.Custom)
		{
			planetRenderer.SetColor(new PlanetColor(Enum.Parse<PlanetColorPreset>(colorType.ToString())));
			UpdateTailVisibility();
		}
	}

	public void SetPlanetColor(Color color, bool force = false)
	{
		if (force || planetColorType == PlanetDecorationColorType.Custom)
		{
			planetRenderer.EnableCustomColor();
			planetRenderer.SetPlanetColor(color);
			UpdateTailVisibility();
		}
	}

	public void SetPlanetTailColor(Color color, bool force = false)
	{
		if (force || planetColorType == PlanetDecorationColorType.Custom)
		{
			planetRenderer.SetTailColor(color);
			planetTailColor = color;
		}
	}

	public void SetFloorMidspin(bool midspin)
	{
		(floor.floorRenderer as FloorMeshRenderer).floorMesh._curvaturePoints = (midspin ? 3 : 40);
		SetFloorAngle(360f);
	}

	public float GetFloorAngle()
	{
		return 180f - ((FloorMeshRenderer)floor.floorRenderer).floorMesh._angle1;
	}

	public void SetFloorAngle(float angle)
	{
		FloorMesh floorMesh = (floor.floorRenderer as FloorMeshRenderer).floorMesh;
		floorMesh._angle0 = -180f;
		floorMesh._angle1 = 180f - angle;
	}

	public void SetFloorColor(Color color, Color? color2, FloorDecorationColorType? floorDecoColorType, float? colorAnimDuration, float duration = 0f, Ease ease = Ease.Linear)
	{
		TrackColorType colorType = (TrackColorType)Enum.Parse(typeof(TrackColorType), floorDecoColorType.ToString());
		floor.ColorFloor(colorType, color, color2 ?? floor.specialColor2, colorAnimDuration.HasValue ? (colorAnimDuration / ADOBase.conductor.song.pitch).Value : floor.specialAnimDuration, TrackColorPulse.None, 0f, 0, duration, ease);
	}

	public void SetFloorOpacity(float opacity)
	{
		floor.SetOpacity(opacity);
	}

	public void SetFloorStyle(TrackStyle style)
	{
		floor.SetTrackStyle(style);
	}

	public void SetFloorGlowEnabled(bool enable)
	{
		floor.topGlow.gameObject.SetActive(enable);
	}

	public void SetFloorGlowColor(Color color)
	{
		floor.topGlow.color = color.WithAlpha(color.a * 0.8f * floor.opacity);
		floorGlowColor = color;
	}

	public void SetFloorRedSwirl(bool redSwirl)
	{
		floor.redSwirl = redSwirl;
	}

	public void SetFloorIcon(CustomFloorIcon icon, bool graySetSpeedIcon = false, float setSpeedIconBpm = 100f)
	{
		floor.usedCustomFloorIcon = true;
		FloorIcon floorIcon = (FloorIcon)Enum.Parse(typeof(FloorIcon), icon.ToString());
		floor.floorIcon = floorIcon;
		floorCustomIcon = icon;
		bool flag = floorIcon == FloorIcon.AnimatedRabbit || floorIcon == FloorIcon.AnimatedDoubleRabbit || floorIcon == FloorIcon.Rabbit || floorIcon == FloorIcon.DoubleRabbit;
		bool flag2 = floorIcon == FloorIcon.AnimatedSnail || floorIcon == FloorIcon.AnimatedDoubleSnail || floorIcon == FloorIcon.Snail || floorIcon == FloorIcon.DoubleSnail;
		bool flag3 = floorIcon == FloorIcon.AnimatedDoubleRabbit || floorIcon == FloorIcon.AnimatedDoubleSnail || floorIcon == FloorIcon.DoubleRabbit || floorIcon == FloorIcon.DoubleSnail;
		Sprite[] array = (flag ? ADOBase.gc.rabbitSpritesArr : ADOBase.gc.snailSpritesArr);
		if (graySetSpeedIcon && (flag || flag2))
		{
			floor.SetIconSprite(array[5 * (flag3 ? 1 : 0)]);
			floor.SetIconOutlineSprite(null);
		}
		else
		{
			floor.UpdateIconSprite(resetToDefault: false);
		}
		floor.graySetSpeedIcon = graySetSpeedIcon;
		floor.speed = setSpeedIconBpm / ADOBase.conductor.bpm;
	}

	private void ReloadFloorIcon()
	{
		SetFloorIcon(floorCustomIcon, floor.graySetSpeedIcon, floor.speed * ADOBase.conductor.bpm);
	}

	public void SetFloorIconAngle(float angle)
	{
		floor.SetIconAngle((0f - angle) * ((float)Math.PI / 180f));
		floorIconAngle = angle;
	}

	public void SetFloorIconFlipped(bool flipped)
	{
		floor.SetIconFlipped(flipped);
		ReloadFloorIcon();
	}

	public void SetFloorIconOutlines(bool enable)
	{
		floor.outline = enable;
		ReloadFloorIcon();
	}
}
