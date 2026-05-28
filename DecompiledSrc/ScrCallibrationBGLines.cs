using System;
using UnityEngine;

public class ScrCallibrationBGLines : MonoBehaviour
{
	private const float StartDelay = 0.5f;

	public Gradient lineColor;

	[Header("Circle Settings")]
	public float numCircles;

	public float smallCircleRadius;

	public float circleRadiusSpacing;

	public float circleTimingSpacing;

	public float circleDuration;

	public float circleInitialWidth;

	public float circleExtraWidth;

	[Header("Angled Line Settings")]
	public float numLines;

	public float angleOffset;

	[Header("Axis Lines Settings")]
	public float numAxisLines;

	public float axisLinesWidth;

	public Gradient axisLinesColor;

	[Header("Prefabs")]
	public scrAnimatedLineCircle circlePrefab;

	public scrAnimatedAngledLine linePrefab;

	private void Start()
	{
		for (int i = 0; (float)i < numCircles; i++)
		{
			scrAnimatedLineCircle obj = UnityEngine.Object.Instantiate(circlePrefab, base.transform);
			obj.radius = smallCircleRadius + (float)i * circleRadiusSpacing;
			obj.delay = 0.5f + (float)i * circleTimingSpacing;
			obj.fullTime = circleDuration;
			obj.lineRenderer.colorGradient = lineColor;
			float widthMultiplier = circleInitialWidth + (float)i * circleExtraWidth;
			obj.lineRenderer.widthMultiplier = widthMultiplier;
		}
		for (int j = 0; (float)j < numLines; j++)
		{
			scrAnimatedAngledLine obj2 = UnityEngine.Object.Instantiate(linePrefab, base.transform);
			float num = 90f - 360f / numLines * (float)j - angleOffset;
			float num2 = 0f - (num - 90f) / 360f;
			obj2.delay = 0.5f + circleDuration * num2;
			obj2.fullTime = circleTimingSpacing * (numCircles - 1f) * 2f;
			obj2.angle = num;
			obj2.minRadius = smallCircleRadius;
			obj2.maxRadius = smallCircleRadius + circleRadiusSpacing * (numCircles - 1f) * 2f;
			obj2.lineRenderer.colorGradient = lineColor;
		}
	}

	public void MakeAxisLines()
	{
		float num = Camera.main.orthographicSize * 2f;
		float maxRadius = Math.Max(Camera.main.aspect * num, num);
		for (int i = 0; (float)i < numAxisLines; i++)
		{
			scrAnimatedAngledLine obj = UnityEngine.Object.Instantiate(linePrefab, base.transform);
			float angle = 90 * i;
			obj.fullTime = circleDuration * 4f;
			obj.angle = angle;
			obj.minRadius = 0f;
			obj.maxRadius = maxRadius;
			obj.lineRenderer.colorGradient = axisLinesColor;
			obj.lineRenderer.widthMultiplier = axisLinesWidth;
		}
	}
}
