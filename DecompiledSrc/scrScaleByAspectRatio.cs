using UnityEngine;

public class scrScaleByAspectRatio : ADOBase
{
	public float referenceAspectRatio = 1.7777778f;

	private Vector2 baseScale;

	private void Awake()
	{
		baseScale = base.transform.localScale;
	}

	private void Update()
	{
		float num = (float)Screen.width * 1f / (float)Screen.height / referenceAspectRatio;
		if (num < 1f)
		{
			base.transform.localScale = baseScale * num;
		}
	}
}
