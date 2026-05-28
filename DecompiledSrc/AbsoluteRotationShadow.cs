using System;
using UnityEngine;
using UnityEngine.UI;

public class AbsoluteRotationShadow : Shadow
{
	private Vector2 origDistance;

	private new void Awake()
	{
		origDistance = base.effectDistance;
	}

	private void LateUpdate()
	{
		if (Application.isPlaying)
		{
			float f = base.transform.eulerAngles.z * ((float)Math.PI / 180f);
			base.effectDistance = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * origDistance;
		}
	}
}
