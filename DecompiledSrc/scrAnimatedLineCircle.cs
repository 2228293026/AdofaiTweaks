using System;
using System.Collections.Generic;
using UnityEngine;

public class scrAnimatedLineCircle : MonoBehaviour
{
	public float radius;

	public float delay;

	public float fullTime;

	public LineRenderer lineRenderer;

	private float progress;

	private int lastRevealed;

	private List<Vector2> coords = new List<Vector2>();

	private void Start()
	{
		for (int i = 0; i < 100; i++)
		{
			float f = (float)Math.PI * 2f * (float)(-i) / 100f + (float)Math.PI / 2f;
			coords.Add(new Vector2(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius));
		}
	}

	private void Update()
	{
		progress += Time.deltaTime;
		progress = Mathf.Min(progress, fullTime + delay);
		int num = (int)((progress - delay) / fullTime * 100f);
		if (num > lastRevealed)
		{
			for (int i = lastRevealed + 1; i <= num; i++)
			{
				lineRenderer.positionCount++;
				lineRenderer.SetPosition(i - 1, coords[i - 1]);
			}
			lastRevealed = num;
			if (num == 100)
			{
				lineRenderer.positionCount++;
				lineRenderer.SetPosition(100, coords[0]);
			}
		}
	}
}
