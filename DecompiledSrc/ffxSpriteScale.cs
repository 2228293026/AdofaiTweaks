using DG.Tweening;
using UnityEngine;

public class ffxSpriteScale : ffxPlusBase
{
	public GameObject spriteObject;

	public Vector3 scale;

	public float time;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		StartEffectStatic(spriteObject, scale, time, ease);
	}

	public static void StartEffectStatic(GameObject spriteObject, Vector3 scale, float time, Ease ease)
	{
		if (!(spriteObject == null))
		{
			spriteObject.SetActive(value: true);
			spriteObject.transform.DOScale(scale, time).SetEase(ease);
		}
	}
}
