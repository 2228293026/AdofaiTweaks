using System;
using DG.Tweening;
using UnityEngine;

public class scrGfxFloat : ADOBase
{
	[NonSerialized]
	public Vector2 start;

	public float amplitude = 1f;

	public float period = 1f;

	public bool useLocalPos;

	public float shakeDuration = 0.2f;

	public float shakeStrength = 3f;

	public int shakeVibrato = 50;

	private bool isPortalShaking;

	public float swingoffsetpct;

	private float c;

	private void Start()
	{
		swingoffsetpct = UnityEngine.Random.value;
		Vector3 v = (useLocalPos ? base.transform.localPosition : base.transform.position);
		start = v.xy();
		if (period == 0f)
		{
			period = 1f;
		}
	}

	private void Update()
	{
		c += (float)scrConductor.instance.deltaSongPos * ADOBase.controller.d_speed;
		if (!isPortalShaking)
		{
			UpdatePosition();
		}
	}

	private void UpdatePosition()
	{
		float x = start.x;
		float y = start.y + amplitude * Mathf.Sin(c / period + swingoffsetpct * (float)Math.PI);
		if (useLocalPos)
		{
			base.transform.LocalMoveXY(x, y);
		}
		else
		{
			base.transform.MoveXY(x, y);
		}
	}

	public void Shake()
	{
		base.transform.DOKill();
		UpdatePosition();
		isPortalShaking = true;
		base.transform.DOShakePosition(shakeDuration, Vector2.right * shakeStrength, shakeVibrato).SetEase(Ease.OutQuint).SetUpdate(isIndependentUpdate: true)
			.OnKill(delegate
			{
				isPortalShaking = false;
			});
	}
}
