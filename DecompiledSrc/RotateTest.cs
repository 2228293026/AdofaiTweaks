using System;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
	private const float radius = 1f;

	private const float bpm = 150f;

	private void Update()
	{
		Vector3 position = base.transform.position;
		float num = 2.5f;
		float f = Time.time * (float)Math.PI * num * 0.8f;
		position.x = Mathf.Sin(f);
		position.y = Mathf.Cos(f);
		base.transform.position = position;
	}
}
