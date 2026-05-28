using System;
using DG.Tweening;
using UnityEngine;

public class scrAnimatedAngledLine : MonoBehaviour
{
	public float delay;

	public float fullTime;

	public float angle;

	public float minRadius;

	public float maxRadius;

	public LineRenderer lineRenderer;

	private float progress;

	private void Update()
	{
		progress += Time.deltaTime;
		progress = Mathf.Min(progress, fullTime + delay);
		float lifetimePercentage = Mathf.Max((progress - delay) / fullTime, 0f);
		float num = DOVirtual.EasedValue(minRadius, maxRadius, lifetimePercentage, Ease.OutQuint);
		lineRenderer.SetPosition(0, new Vector2(Mathf.Cos(angle / 180f * (float)Math.PI) * minRadius, Mathf.Sin(angle / 180f * (float)Math.PI) * minRadius));
		lineRenderer.SetPosition(1, new Vector2(Mathf.Cos(angle / 180f * (float)Math.PI) * num, Mathf.Sin(angle / 180f * (float)Math.PI) * num));
	}
}
