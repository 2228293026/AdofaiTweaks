using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ADOFAI;

public class TransformGizmo : EditorGizmo, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public enum GizmoPlacement
	{
		Right,
		TopRight,
		Top,
		TopLeft,
		Left,
		BottomLeft,
		Bottom,
		BottomRight
	}

	public TransformGizmoHolder holder;

	public GizmoPlacement gizmoPlacement;

	public SpriteRenderer sprite;

	public Tween enlargeTween;

	public Tween shortenTween;

	private static Vector2[] PlacementMultipliers = new Vector2[8]
	{
		new Vector2(1f, 0f),
		new Vector2(1f, 1f),
		new Vector2(0f, 1f),
		new Vector2(-1f, 1f),
		new Vector2(-1f, 0f),
		new Vector2(-1f, -1f),
		new Vector2(0f, -1f),
		new Vector2(1f, -1f)
	};

	public Vector2 GizmoPixelsSize { get; private set; }

	public void Start()
	{
		startScale = 1.272727f;
	}

	public Vector2 GetDirectionVector()
	{
		int num = (int)gizmoPlacement;
		return PlacementMultipliers[num];
	}

	public bool IsSideGizmo()
	{
		if (gizmoPlacement != GizmoPlacement.Right && gizmoPlacement != GizmoPlacement.Top && gizmoPlacement != GizmoPlacement.Left)
		{
			return gizmoPlacement == GizmoPlacement.Bottom;
		}
		return true;
	}

	public void OnMouseEnter()
	{
		if (!RDEditorUtils.MouseOnScreenEdge)
		{
			ADOBase.editor.lastHoveredGizmo = this;
		}
	}

	public void OnMouseExit()
	{
		ADOBase.editor.lastHoveredGizmo = null;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnMouseEnter();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnMouseExit();
	}
}
