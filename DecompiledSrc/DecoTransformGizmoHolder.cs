using System;
using ADOFAI;
using UnityEngine;
using UnityEngine.UI;

public class DecoTransformGizmoHolder : TransformGizmoHolder
{
	private SpriteRenderer currentDecSprite;

	private Text currentDecText;

	private LevelEvent currendDecEvent;

	private scrDecoration currentDec;

	private bool rotationStarted;

	private float angleStart;

	private Vector2 oppositeGizmoPos;

	private Vector2 startGizmoPos;

	private float decorationRatio;

	private void Update()
	{
		if (ADOBase.isEditingLevel)
		{
			showRotateGizmos = rotationStarted || RDInput.holdingControl;
			gizmoSprite = (showRotateGizmos ? GizmoSprite.Rotate : GizmoSprite.Scale);
			ChangeGizmosSprite(gizmoSprite);
			if (base.transform.localScale != Vector3.one)
			{
				Debug.LogError("localScale is different than one! " + base.transform.localScale);
				base.transform.localScale = Vector3.one;
			}
		}
	}

	protected override void LateUpdate()
	{
		if (!ADOBase.isEditingLevel)
		{
			return;
		}
		if (currentDec != null)
		{
			scrDecoration scrDecoration2 = currentDec;
			if (scrDecoration2 is scrVisualDecoration || scrDecoration2 is scrParticleDecoration)
			{
				UpdateGizmosTransform(currentDec.pivotPosVec, currentDec.GetDecorationWorldSize(), currentDec.transform.localScale, (float)currentDec.sourceLevelEvent["rotation"], currentDec.pivotOffsetVec, currentDec.parallax.GetPositionWithParallax());
			}
		}
		UpdateGizmosVisibility();
	}

	public override void DragStart(TransformGizmo handle)
	{
		scrDecoration decoration = scrDecorationManager.GetDecoration(ADOBase.editor.selectedDecorations[0]);
		bool num = decoration.sourceLevelEvent.eventType == LevelEventType.AddDecoration;
		bool flag = decoration.sourceLevelEvent.eventType == LevelEventType.AddParticle;
		decoration.scaleAtDragStart = (Vector2)decoration.sourceLevelEvent["scale"] / 100f;
		angleStart = (float)decoration.sourceLevelEvent["rotation"];
		if (num)
		{
			decoration.sizeAtDragStart = ((scrVisualDecoration)decoration).spriteUnscaledSize * decoration.scaleAtDragStart;
		}
		else
		{
			if (!flag)
			{
				return;
			}
			decoration.sizeAtDragStart = ((scrParticleDecoration)decoration).scale;
		}
		decorationRatio = decoration.sizeAtDragStart.x / decoration.sizeAtDragStart.y;
		ADOBase.editor.camera.ScreenToWorldPoint(Input.mousePosition).xy();
		Vector2 directionVector = ADOBase.editor.draggingGizmo.GetDirectionVector();
		float radians = angleStart * ((float)Math.PI / 180f);
		Vector2 rotatedVector = (decoration.sizeAtDragStart / 2f * directionVector).GetRotatedVector(radians);
		Vector2 positionWithParallax = decoration.parallax.GetPositionWithParallax();
		startGizmoPos = positionWithParallax + rotatedVector;
		oppositeGizmoPos = positionWithParallax - rotatedVector;
	}

	public override void DragEnd()
	{
		rotationStarted = false;
	}

	public override void Drag(Vector2 mouseTranslation, Vector2 mouseDelta)
	{
		if (!ADOBase.editor.SelectionDecorationIsSingle() || !(ADOBase.editor.draggingGizmo != null))
		{
			return;
		}
		LevelEvent levelEvent = ADOBase.editor.selectedDecorations[0];
		scrDecoration decoration = scrDecorationManager.GetDecoration(levelEvent);
		bool flag = decoration.sourceLevelEvent.eventType == LevelEventType.AddDecoration;
		bool flag2 = decoration.sourceLevelEvent.eventType == LevelEventType.AddParticle;
		if (!flag && !flag2)
		{
			return;
		}
		float num = (float)decoration.sourceLevelEvent["rotation"] * ((float)Math.PI / 180f);
		Vector2 vector = ADOBase.editor.camera.ScreenToWorldPoint(Input.mousePosition).xy();
		if (RDInput.holdingControl)
		{
			rotationStarted = true;
		}
		if (rotationStarted)
		{
			if (mouseTranslation != Vector2.zero)
			{
				float num2 = (vector - decoration.parallax.GetPositionWithParallax()).FindAngleAtan2();
				float num3 = (ADOBase.editor.camera.ScreenToWorldPoint(Input.mousePosition).xy() - decoration.parallax.GetPositionWithParallax()).FindAngleAtan2();
				if (num2 < num3)
				{
					num2 += 360f;
				}
				float num4 = num2 - num3;
				if (num4 > 180f)
				{
					num4 -= 360f;
				}
				float num5 = angleStart + num4;
				levelEvent["rotation"] = num5;
				decoration.SetRotation(num5 - decoration.startRot);
				ADOBase.editor.levelEventsPanel.UpdatePropertyText(levelEvent, "rotation");
			}
			return;
		}
		if (RDInput.holdingShift)
		{
			Vector2 vector2 = vector - oppositeGizmoPos;
			Vector2 vector3 = startGizmoPos - oppositeGizmoPos;
			float num6 = vector2.magnitude - vector3.magnitude;
			float num7 = Mathf.Abs(vector3.FindAngleAtan2() - vector2.FindAngleAtan2());
			bool flag3 = num7 > 90f && num7 < 270f;
			num6 += (flag3 ? (vector2.magnitude * -2f) : 0f);
			mouseTranslation = vector3.normalized * num6;
		}
		Vector2 directionVector = ADOBase.editor.draggingGizmo.GetDirectionVector();
		Vector2 rotatedVector = mouseTranslation.GetRotatedVector(0f - num);
		Vector2 vector4 = directionVector / 2f - decoration.pivotOffsetVec / decoration.GetDecorationWorldSize();
		Vector2 rotatedVector2 = (rotatedVector * directionVector * vector4).GetRotatedVector(num);
		Vector2 vector5 = (RDInput.holdingAlt ? Vector2.zero : rotatedVector2);
		ADOBase.editor.DragDecorations(vector5, ignoreModifiers: true);
		float num8 = (RDInput.holdingAlt ? 2f : 1f);
		float tileSize = ADOBase.controller.tileSize;
		Vector2 vector6 = rotatedVector * directionVector * tileSize;
		Vector2 decorationDragDelta = ADOBase.editor.GetDecorationDragDelta(vector6 * num8, decoration, parallax: false);
		Vector2 vector7 = decoration.sizeAtDragStart + decorationDragDelta / tileSize;
		Vector2 vector8 = ((!flag) ? vector7 : (vector7 / ((scrVisualDecoration)decoration).spriteUnscaledSize));
		if (RDInput.holdingShift && flag)
		{
			float ratio = decoration.scaleAtDragStart.GetRatio();
			if (directionVector.x == 0f)
			{
				vector8.x = vector8.y * ratio;
			}
			else if (directionVector.y == 0f)
			{
				vector8.y = vector8.x / ratio;
			}
		}
		decoration.sourceLevelEvent["scale"] = vector8 * 100f;
		decoration.SetScale(vector8);
		ADOBase.editor.levelEventsPanel.UpdatePropertyText(levelEvent, "scale");
	}

	public void Setup(LevelEvent levelEvent)
	{
		currentDec = scrDecorationManager.GetDecoration(levelEvent);
		if (levelEvent.eventType == LevelEventType.AddDecoration)
		{
			currentDecSprite = currentDec.GetComponentInChildren<SpriteRenderer>();
		}
		else if (levelEvent.eventType == LevelEventType.AddText)
		{
			currentDecText = currentDec.transform.GetChild(0).GetComponentInChildren<Text>();
		}
		currendDecEvent = levelEvent;
	}

	public override void UpdateGizmosVisibility()
	{
		if (currentDec == null)
		{
			return;
		}
		LevelEvent sourceLevelEvent = currentDec.sourceLevelEvent;
		bool flag = currentDec.parallax.multiplier_x == 1f;
		bool flag2 = currentDec.parallax.multiplier_y == 1f;
		foreach (Handle handle in handles)
		{
			int num;
			if (ADOBase.editor.SelectionDecorationIsSingle())
			{
				LevelEventType eventType = sourceLevelEvent.eventType;
				if ((eventType == LevelEventType.AddDecoration || eventType == LevelEventType.AddParticle) && ADOBase.editor.selectedDecorations.Contains(sourceLevelEvent))
				{
					num = ((!ADOBase.editor.lockPathEditing) ? 1 : 0);
					goto IL_00a7;
				}
			}
			num = 0;
			goto IL_00a7;
			IL_00a7:
			bool active = (byte)num != 0;
			Vector2 directionVector = handle.transformGizmo.GetDirectionVector();
			bool output;
			if (flag && (directionVector.x == -1f || directionVector.x == 1f) && !showRotateGizmos)
			{
				active = false;
			}
			else if (flag2 && (directionVector.y == -1f || directionVector.y == 1f) && !showRotateGizmos)
			{
				active = false;
			}
			else if (sourceLevelEvent.TryGet<bool>("lockScale", out output) ? output : (sourceLevelEvent.eventType != LevelEventType.AddParticle))
			{
				active = false;
			}
			else if (handle.transformGizmo.IsSideGizmo() && showRotateGizmos)
			{
				active = false;
			}
			handle.transformGizmo.gameObject.SetActive(active);
		}
	}
}
