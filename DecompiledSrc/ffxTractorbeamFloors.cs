using System;
using DG.Tweening;
using UnityEngine;

public class ffxTractorbeamFloors : ffxPlusBase
{
	public Transform tractorbeamSource;

	public int tileRange;

	[NonSerialized]
	public bool origin = true;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		if (origin)
		{
			int seqID = GetComponent<scrFloor>().seqID;
			for (int i = 1; i <= tileRange && seqID + i <= ADOBase.lm.listFloors.Count; i++)
			{
				ffxTractorbeamFloors obj = ADOBase.lm.listFloors[seqID + i].gameObject.AddComponent<ffxTractorbeamFloors>();
				obj.origin = false;
				obj.tractorbeamSource = tractorbeamSource;
			}
		}
		float num = (float)cond.crotchetAtStart / (floor.speed * cond.song.pitch);
		float delay = UnityEngine.Random.Range(2f * num, 6f * num);
		float shakePeriod = UnityEngine.Random.Range(num, 2f * num);
		float num2 = UnityEngine.Random.Range(0.5f * num, 2f * num);
		AbductFloor(delay, num2, shakePeriod);
	}

	private void AbductFloor(float delay, float duration, float shakePeriod)
	{
		Sequence s = DOTween.Sequence();
		s.AppendInterval(delay);
		s.Append(base.transform.DOShakeRotation(shakePeriod, new Vector3(0f, 0f, 45f), 50, 90f, fadeOut: false));
		s.AppendCallback(delegate
		{
			if (ADOBase.controller.visualQuality == VisualQuality.High)
			{
				base.transform.SetParent(tractorbeamSource, worldPositionStays: true);
				base.transform.DOLocalMove(Vector3.zero, duration);
				if (scrController.instance.currFloor == floor && ADOBase.controller.state == States.Fail2)
				{
					tractorbeamSource.GetComponent<DOTweenAnimation>().DOKill();
					DOTween.Sequence().AppendInterval(duration).Append(tractorbeamSource.DOLocalMoveY(20f, 1f).SetEase(Ease.InBack));
				}
			}
			else
			{
				base.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InExpo);
			}
			base.transform.DORotate(new Vector3(0f, 0f, UnityEngine.Random.Range(-75f, 75f)), duration);
		});
	}
}
