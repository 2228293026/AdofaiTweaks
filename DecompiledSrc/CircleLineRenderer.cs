using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class CircleLineRenderer : MonoBehaviour
{
	public int steps;

	public float radius = 1f;

	private LineRenderer line => GetComponent<LineRenderer>();

	private void OnValidate()
	{
		line.positionCount = steps;
		for (int i = 0; i < steps; i++)
		{
			float f = (float)i / (float)steps * (float)Math.PI * 2f;
			Vector3 position = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
			position *= radius;
			line.SetPosition(i, position);
		}
	}
}
