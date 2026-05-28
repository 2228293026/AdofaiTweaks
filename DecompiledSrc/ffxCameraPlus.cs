using System;
using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxCameraPlus : ffxPlusBase
{
	public Vector2 targetPos;

	public bool positionUsed = true;

	public float targetRot;

	public bool rotationUsed = true;

	public float targetZoom;

	public bool zoomUsed = true;

	private Transform camParent;

	public CamMovementType movementType;

	public bool movementTypeUsed = true;

	public Vector3 floorPos;

	public bool dontDisable;

	[NonSerialized]
	public Vector2 finalPos;

	private static Tween moveXTween;

	private static Tween moveYTween;

	private static Tween rotationTween;

	private static Tween zoomTween;

	public static bool legacyRelativeTo;

	public bool movementTypeIsLastPosition
	{
		get
		{
			if (movementType != CamMovementType.LastPosition)
			{
				return movementType == CamMovementType.LastPositionNoRotation;
			}
			return true;
		}
	}

	protected override IEnumerable<Tween> eventTweens => new Tween[4] { moveXTween, moveYTween, rotationTween, zoomTween };

	public override void Awake()
	{
		base.Awake();
		SetupVFXSettings();
		if (ADOBase.customLevel != null)
		{
			camParent = ADOBase.customLevel.camParent;
		}
		else
		{
			camParent = scrCamera.instance.transform.parent;
		}
		floorPos = floor.transform.position;
	}

	private void SetupVFXSettings()
	{
		disableIfMinFx = !dontDisable;
		if (disableIfMaxFx)
		{
			disableIfMinFx = false;
		}
		if (disableIfMaxFx && ADOBase.controller.visualEffects == VisualEffects.Minimum)
		{
			dontDisable = true;
		}
	}

	public void ForceUpdateCamParent()
	{
		cam = scrCamera.instance;
		camParent = scrCamera.instance.transform.parent;
		ctrl = scrController.instance;
		vfx = scrVfxPlus.instance;
	}

	public void StartEffectManual()
	{
		StartEffect(null);
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (movementTypeUsed && (movementType != CamMovementType.Tile || ((bool)floor && floor.seqID == cam.lastTileCamFloor)) && positionUsed && (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y)) && movementType == cam.lastUsedMovementType && movementType != CamMovementType.Global && movementType != CamMovementType.LastPosition && movementType != CamMovementType.LastPositionNoRotation)
		{
			movementTypeUsed = false;
		}
		if (positionUsed || movementTypeUsed)
		{
			if (!float.IsNaN(targetPos.x) || movementTypeUsed)
			{
				moveXTween?.Kill(complete: true);
			}
			if (!float.IsNaN(targetPos.y) || movementTypeUsed)
			{
				moveYTween?.Kill(complete: true);
			}
		}
		if (rotationUsed || (movementTypeUsed && movementTypeIsLastPosition))
		{
			rotationTween?.Kill(complete: true);
		}
		if (zoomUsed)
		{
			zoomTween?.Kill(complete: true);
		}
		AdjustDurationForHardbake();
		float num = 0f;
		Vector2 vector = targetPos;
		if (movementTypeUsed)
		{
			if (float.IsNaN(vector.x))
			{
				vector.x = 0f;
			}
			if (float.IsNaN(vector.y))
			{
				vector.y = 0f;
			}
		}
		CamMovementType camMovementType = (movementTypeUsed ? movementType : cam.lastUsedMovementType);
		Vector2 vector2 = (positionUsed ? vector : (cam.lastEventRelativePosition - (Vector2)camParent.position));
		if (legacyRelativeTo && !movementTypeUsed)
		{
			Vector2 vector3 = camParent.position;
			if (positionUsed)
			{
				finalPos = vector + vector3;
			}
			else
			{
				finalPos = vector3;
			}
		}
		else
		{
			switch (camMovementType)
			{
			case CamMovementType.Player:
				if (!cam.followMode)
				{
					Vector3 position = camParent.position;
					camParent.position = position - cam.furthestPlanet.transform.position;
					cam.transform.position = position + Vector3.back * 10f;
					cam.followMode = true;
					cam.UpdateFollowCam(force: true);
				}
				finalPos = vector2;
				break;
			case CamMovementType.Tile:
				if (cam.followMode)
				{
					camParent.position = cam.transform.position - Vector3.back * 10f;
					cam.SetToFreeMode();
				}
				cam.lastEventRelativePosition = floorPos;
				if ((bool)floor)
				{
					cam.lastTileCamFloor = floor.seqID;
				}
				finalPos = vector2 + (Vector2)floorPos;
				break;
			case CamMovementType.Global:
				if (cam.followMode)
				{
					camParent.position = cam.transform.position - Vector3.back * 10f;
					cam.SetToFreeMode();
				}
				cam.lastEventRelativePosition = Vector2.zero;
				finalPos = vector2;
				break;
			case CamMovementType.LastPosition:
			case CamMovementType.LastPositionNoRotation:
			{
				Vector2 vector4 = camParent.position;
				if (camMovementType == CamMovementType.LastPosition)
				{
					num = vfx.camAngle;
				}
				if (positionUsed)
				{
					finalPos = vector + vector4;
				}
				else
				{
					finalPos = vector4;
				}
				break;
			}
			}
		}
		if (movementTypeUsed)
		{
			cam.lastUsedMovementType = movementType;
		}
		if (positionUsed || movementTypeUsed)
		{
			if (!float.IsNaN(finalPos.x))
			{
				moveXTween = DOTween.To(() => camParent.position.x, delegate(float x)
				{
					camParent.MoveX(x);
				}, finalPos.x, duration).SetEase(ease).Done();
			}
			if (!float.IsNaN(finalPos.y))
			{
				moveYTween = DOTween.To(() => camParent.position.y, delegate(float y)
				{
					camParent.MoveY(y);
				}, finalPos.y, duration).SetEase(ease).Done();
			}
		}
		if (rotationUsed || (movementTypeUsed && movementTypeIsLastPosition))
		{
			rotationTween = DOTween.To(() => vfx.camAngle, delegate(float x)
			{
				vfx.camAngle = x;
			}, targetRot + num, duration).SetEase(ease).Done();
		}
		if (zoomUsed)
		{
			zoomTween = DOTween.To(() => cam.zoomSize, delegate(float x)
			{
				cam.zoomSize = x;
			}, targetZoom, duration).SetEase(ease).Done();
		}
	}

	public override void ScrubToTime(float t)
	{
		if (!dontDisable && ADOBase.controller.visualEffects == VisualEffects.Minimum)
		{
			triggered = true;
			return;
		}
		if (disableIfMaxFx && ADOBase.controller.visualEffects == VisualEffects.Full)
		{
			triggered = true;
			return;
		}
		bool flag = (movementTypeUsed ? (movementType == CamMovementType.Player) : (cam.lastUsedMovementType == CamMovementType.Player));
		bool flag2 = (double)t > startTime + (double)duration;
		if ((double)t >= startTime)
		{
			_ = !flag2;
		}
		else
			_ = 0;
		base.ScrubToTime(t);
		if (flag2 && flag)
		{
			cam.ViewObjectInstant(cam.furthestPlanet.transform);
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = RDUtils.GetRandomFloat(evnt, "duration") * crotchet;
		Vector2 randomVector = RDUtils.GetRandomVector2(evnt, "position");
		targetPos = randomVector * ADOBase.controller.tileSize;
		positionUsed = !evnt.disabled["position"];
		targetRot = RDUtils.GetRandomFloat(evnt, "rotation");
		rotationUsed = !evnt.disabled["rotation"];
		targetZoom = RDUtils.GetRandomFloat(evnt, "zoom") / 100f;
		zoomUsed = !evnt.disabled["zoom"];
		ease = (Ease)evnt["ease"];
		movementType = (CamMovementType)evnt["relativeTo"];
		movementTypeUsed = !evnt.disabled["relativeTo"];
		evnt.TryGetAndSet("dontDisable", ref dontDisable);
		evnt.TryGetAndSet("minVfxOnly", ref disableIfMaxFx);
		SetupVFXSettings();
	}
}
