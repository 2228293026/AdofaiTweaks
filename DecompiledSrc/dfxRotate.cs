using System;
using UnityEngine;

public class dfxRotate : MonoBehaviour
{
	public float speed = 0.5f;

	public float angle = 5f;

	private void Update()
	{
		base.transform.eulerAngles = Vector3.forward * Mathf.Sin(Time.time * (float)Math.PI * speed) * angle;
	}
}
