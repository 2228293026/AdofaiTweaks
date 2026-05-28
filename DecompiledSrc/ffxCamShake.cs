using DG.Tweening;
using UnityEngine;

public class ffxCamShake : ffxPlusBase
{
	private Tween tween;

	public float strength;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		tween?.Kill();
		tween = DOTween.Shake(() => cam.shake, delegate(Vector3 x)
		{
			cam.shake = x;
		}, duration, strength);
	}
}
