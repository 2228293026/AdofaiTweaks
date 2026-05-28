using DG.Tweening;
using UnityEngine;

public class scrPulseOnBeat : ADOBase
{
	private Vector3 startScale;

	public float pulsewidth;

	public float time;

	private int counter;

	private void Start()
	{
		startScale = base.transform.localScale;
		ADOBase.conductor.onBeats.Add(this);
	}

	public override void OnBeat()
	{
		if (!(base.transform == null))
		{
			base.transform.DOKill();
			base.transform.ScaleXY(pulsewidth);
			base.transform.DOScale(startScale, time);
		}
	}
}
