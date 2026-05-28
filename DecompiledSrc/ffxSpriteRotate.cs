using System;
using DG.Tweening;
using UnityEngine;

public class ffxSpriteRotate : ffxPlusBase
{
	public GameObject spriteObject;

	public Vector3 angleDegrees;

	public float time;

	[NonSerialized]
	public RotateMode rotateMode;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		if (!(spriteObject == null) && ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			if (time < 0f)
			{
				time *= -60f / (cond.bpm * GetComponent<scrFloor>().speed) / cond.song.pitch;
			}
			spriteObject.SetActive(value: true);
			spriteObject.transform.DOLocalRotate(angleDegrees, time, rotateMode);
		}
	}
}
