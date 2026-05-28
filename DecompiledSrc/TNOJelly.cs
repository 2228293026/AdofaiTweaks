using UnityEngine;

public class TNOJelly : ADOBase
{
	public int jellySprite;

	public float startAngle;

	public float floatAmp;

	public float floatPeriod = 1f;

	private bool hasInited;

	private void Update()
	{
		if (!hasInited)
		{
			Init();
		}
	}

	private void Init()
	{
		hasInited = true;
		Transform transform = null;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		foreach (Transform transform2 in componentsInChildren)
		{
			if (transform2 != base.transform)
			{
				transform = transform2;
				break;
			}
		}
		if (!(transform == null))
		{
			transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
			scrGfxFloat obj = transform.gameObject.AddComponent<scrGfxFloat>();
			obj.amplitude = floatAmp;
			obj.period = floatPeriod;
			obj.useLocalPos = true;
			transform.GetComponent<SpriteRenderer>().sprite = ADOBase.gc.TNOJellySprites[jellySprite];
		}
	}
}
