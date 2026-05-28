using System;
using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public abstract class TransformGizmoHolder : ADOBase
{
	[Serializable]
	public class Handle
	{
		public TransformGizmo transformGizmo;

		public RectTransform gizmoRect;

		public SpriteRenderer spriteRenderer;

		public RectTransform imageRect;

		public BoxCollider2D collider;
	}

	protected enum GizmoSprite
	{
		Scale,
		Rotate
	}

	private const float GizmoSpriteSize = 0.15f;

	private const float GizmoOpenedHeightMultiplier = 2.25f;

	private const float EnlargeAnimTime = 0.075f;

	private const float HandleSizeMult = 0.883f;

	public bool forUI;

	public List<Handle> handles = new List<Handle>();

	[SerializeField]
	protected Sprite rotationSprite;

	[SerializeField]
	protected Sprite scaleSprite;

	protected GizmoSprite gizmoSprite;

	protected bool showRotateGizmos;

	private TransformGizmo cacheGizmo;

	protected void Awake()
	{
		foreach (Handle handle in handles)
		{
			handle.transformGizmo.holder = this;
		}
	}

	protected virtual void LateUpdate()
	{
		UpdateGizmosVisibility();
	}

	public abstract void DragStart(TransformGizmo handle);

	public abstract void DragEnd();

	public abstract void Drag(Vector2 mouseTranslation, Vector2 mouseDelta);

	public virtual void UpdateGizmosVisibility()
	{
		foreach (Handle handle in handles)
		{
			handle.transformGizmo.gameObject.SetActive(value: true);
		}
	}

	public void UpdateGizmosTransform(Vector2 centerPos, Vector2 worldSize, Vector2? pLocalScale = null, float rotation = 0f, Vector2? nPivotOffset = null, Vector2? nParallaxPos = null)
	{
		Vector2 vector = nPivotOffset ?? Vector2.zero;
		Vector2 vector2 = pLocalScale ?? Vector2.one;
		Vector2 vector3 = nParallaxPos ?? centerPos;
		base.transform.position = centerPos;
		Vector2 vector4 = Input.mousePosition;
		Vector2 vector5 = ADOBase.editor.camera.ScreenToWorldPoint(vector4);
		foreach (Handle handle in handles)
		{
			TransformGizmo transformGizmo = handle.transformGizmo;
			Vector2 directionVector = transformGizmo.GetDirectionVector();
			Vector2 vector6 = vector * vector2;
			Vector2 vector7 = worldSize / 2f * directionVector;
			Vector2 originalPos = vector7 + vector6;
			bool num = handle.transformGizmo.IsSideGizmo();
			Vector2 rotatedVector = originalPos.GetRotatedVector(rotation * ((float)Math.PI / 180f));
			Vector2 vector8 = vector3 + rotatedVector;
			transformGizmo.transform.position = vector8;
			Transform child = transformGizmo.transform.GetChild(0);
			if (num)
			{
				Vector2 originalPos2 = vector5 - vector8;
				float current2 = Mathf.Atan2(originalPos2.y, originalPos2.x) * 57.29578f;
				float target = rotation + 90f + Mathf.Atan2(directionVector.y, directionVector.x) * 57.29578f;
				float num2 = Mathf.DeltaAngle(current2, target);
				Vector2 rotatedVector2 = originalPos2.GetRotatedVector(num2 * ((float)Math.PI / 180f));
				if (num2 > 90f || num2 < -90f)
				{
					rotatedVector2 *= -1f;
				}
				child.position = vector8 + rotatedVector2;
			}
			float num3 = Mathf.Round((vector7.FindAngleAtan2() - 45f) / 90f) * 90f + rotation - 45f;
			if (num)
			{
				float num4 = ((directionVector.x == 0f) ? 45f : (-45f));
				transformGizmo.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, num3 + num4));
			}
			else
			{
				child.rotation = Quaternion.Euler(new Vector3(0f, 0f, num3));
			}
			HandleAnimation(handle);
			if (!forUI)
			{
				SpriteRenderer spriteRenderer = handle.spriteRenderer;
				Vector2 size = spriteRenderer.sprite.bounds.size * spriteRenderer.transform.localScale.xy() / 2f;
				if (transformGizmo.IsSideGizmo())
				{
					float num5 = ((directionVector.x == 0f) ? (worldSize.x / size.x) : (worldSize.y / size.y));
					new Vector2(size.x * num5 - size.x, size.y).Abs();
				}
				else
				{
					handle.collider.size = size;
				}
			}
		}
	}

	protected void HandleAnimation(Handle handle)
	{
		TransformGizmo transformGizmo = handle.transformGizmo;
		bool flag = handle.transformGizmo.IsSideGizmo();
		Tween tween = null;
		Tween tween2 = null;
		Vector2 vector = new Vector2(0.0001f, 0.0001f);
		SpriteRenderer gizmoSpriteRend = handle.spriteRenderer;
		RectTransform gizmoRect = handle.imageRect;
		if (transformGizmo == ADOBase.editor.lastHoveredGizmo && !showRotateGizmos)
		{
			cacheGizmo = transformGizmo;
			Vector2 vector2 = new Vector2(0.15f, 0.29801252f);
			if (forUI)
			{
				tween = DOTween.To(() => gizmoRect.sizeDelta, delegate(Vector2 x)
				{
					gizmoRect.sizeDelta = x;
				}, vector2 * 100f, 0.075f).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				tween = DOTween.To(() => gizmoSpriteRend.size, delegate(Vector2 x)
				{
					gizmoSpriteRend.size = x;
				}, vector2, 0.075f).SetUpdate(isIndependentUpdate: true);
			}
		}
		else if (ADOBase.editor.lastHoveredGizmo == null && transformGizmo == cacheGizmo && !showRotateGizmos)
		{
			cacheGizmo = transformGizmo;
			Vector2 vector3 = (flag ? vector : new Vector2(0.15f, 0.13245001f));
			tween2 = ((!forUI) ? DOTween.To(() => gizmoSpriteRend.size, delegate(Vector2 x)
			{
				gizmoSpriteRend.size = x;
			}, vector3, 0.075f).SetUpdate(isIndependentUpdate: true) : DOTween.To(() => gizmoRect.sizeDelta, delegate(Vector2 x)
			{
				gizmoRect.sizeDelta = x;
			}, vector3 * 100f, 0.075f).SetUpdate(isIndependentUpdate: true));
			tween2.OnComplete(delegate
			{
				cacheGizmo = null;
			});
		}
		else
		{
			tween?.Kill();
			tween2?.Kill();
			if (forUI)
			{
				gizmoRect.sizeDelta = (flag ? Vector2.zero : new Vector2(15.000001f, 13.245002f));
			}
			else
			{
				gizmoSpriteRend.size = (flag ? vector : new Vector2(0.15f, 0.13245001f));
			}
		}
	}

	protected void ChangeGizmosSprite(GizmoSprite gizmoSprite)
	{
		bool flag = gizmoSprite == GizmoSprite.Rotate;
		Sprite sprite = (flag ? rotationSprite : scaleSprite);
		foreach (Handle handle in handles)
		{
			handle.spriteRenderer.drawMode = ((!flag) ? SpriteDrawMode.Sliced : SpriteDrawMode.Simple);
			handle.spriteRenderer.sprite = sprite;
		}
	}
}
