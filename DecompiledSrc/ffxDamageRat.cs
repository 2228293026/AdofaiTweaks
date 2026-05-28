using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ffxDamageRat : ffxPlusBase
{
	public Transform target;

	private static SpriteRenderer ratObj;

	public int floorCount = 3;

	public float durationBeats = 1f;

	private List<scrFloor> targetFloors = new List<scrFloor>();

	public override bool runOnHit => true;

	private void Start()
	{
		if (ratObj == null)
		{
			GameObject gameObject = GameObject.Find("ratking1");
			if (gameObject != null)
			{
				ratObj = gameObject.GetComponent<SpriteRenderer>();
			}
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ratObj == null)
		{
			return;
		}
		_ = cond == null;
		int num = floor.seqID - 1;
		float interval = durationBeats * (float)cond.crotchetAtStart / (floor.speed * cond.song.pitch);
		for (int num2 = num; num2 >= num - floorCount; num2--)
		{
			scrFloor scrFloor2 = ADOBase.lm.listFloors[num2];
			targetFloors.Add(scrFloor2);
			scrFloor2.transform.SetParent(target, worldPositionStays: true);
			scrFloor2.transform.DOLocalMove(Vector3.zero, interval).SetEase(Ease.InCubic);
			scrFloor2.transform.DORotate(new Vector3(0f, 0f, Random.Range(-180, 180)), interval).SetEase(Ease.InCubic);
			scrFloor2.MoveToBack();
		}
		Sequence s = DOTween.Sequence();
		s.AppendInterval(interval);
		s.AppendCallback(delegate
		{
			foreach (scrFloor targetFloor in targetFloors)
			{
				targetFloor.transform.localScale = Vector3.zero;
			}
			float num3 = 0.25f * (float)cond.crotchetAtStart;
			ratObj.material.SetFloat("_Flash", 1f);
			ratObj.material.DOFloat(0f, "_Flash", num3).SetUpdate(isIndependentUpdate: true);
			DOTween.Shake(() => target.localPosition, delegate(Vector3 x)
			{
				target.localPosition = x;
			}, num3, 1f, 100, 90f, ignoreZAxis: false);
		});
	}
}
