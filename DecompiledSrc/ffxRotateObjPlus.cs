using DG.Tweening;
using UnityEngine;

public class ffxRotateObjPlus : ffxPlusBase
{
	public GameObject objRot;

	public float targetAngle;

	public override void StartEffect(scrPlanet planet)
	{
		float startAngle = objRot.transform.eulerAngles.z;
		float currAngle = 0f;
		DOTween.To(() => currAngle, delegate(float z)
		{
			currAngle = z;
		}, targetAngle, duration).SetEase(ease).OnUpdate(delegate
		{
			objRot.transform.localRotation = Quaternion.Euler(0f, 0f, startAngle + currAngle);
		});
	}
}
