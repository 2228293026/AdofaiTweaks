using System;
using DG.Tweening;
using UnityEngine;

public class scrCalibrationLine : MonoBehaviour
{
	public scnCalibration calibrationScene;

	public SpriteRenderer spriteRenderer;

	public float rotationSpeed;

	public static scrCalibrationLine instance;

	private void Awake()
	{
		instance = this;
	}

	private float StandardizeAngle(float angle)
	{
		if (angle > 180f)
		{
			angle -= 360f;
		}
		else if (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	private void Update()
	{
		float num = (0f - (float)calibrationScene.averageAngleOffset) * 180f / (float)Math.PI;
		if (Mathf.Abs(num - StandardizeAngle(base.transform.localEulerAngles.z)) > 90f)
		{
			if (num < StandardizeAngle(base.transform.localEulerAngles.z))
			{
				base.transform.Rotate(0f, 0f, -180f);
			}
			else
			{
				base.transform.Rotate(0f, 0f, 180f);
			}
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, num), Time.deltaTime * rotationSpeed);
	}

	public void FadeIn()
	{
		spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
		spriteRenderer.DOFade(0.8f, 0.5f).SetEase(Ease.InOutQuad);
	}

	public void FadeOut()
	{
		spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad);
	}
}
