using DG.Tweening;
using UnityEngine;

public class ffxHotTile : ffxPlusBase
{
	public int emberCount = 9;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Color fadedAlp = new Color(1f, 1f, 1f, 0.6f);

	public override bool runOnHit => true;

	public override void Awake()
	{
		base.Awake();
	}

	private float RandF(float mi = 0f, float ma = 1f)
	{
		return Random.Range(mi, ma);
	}

	public override void StartEffect(scrPlanet planet)
	{
		float num = 1f / scrConductor.instance.song.pitch;
		for (int i = 0; i < emberCount; i++)
		{
			float num2 = RandF(0.5f, 1.2f);
			Quaternion quaternion = Quaternion.Euler(0f, 0f, 360 / emberCount * i);
			Mawaru_Sprite newEmber = TaroBGScript.instance.GetTileEmber();
			newEmber.transform.position = base.transform.position;
			newEmber.transform.localScale = Vector3.one * RandF(1.1f, 1.5f);
			newEmber.transform.parent.gameObject.transform.position = Vector3.zero;
			newEmber.render.enabled = true;
			Vector3 endValue = quaternion * new Vector3(RandF(0.5f, 2f), 0f, 0f);
			newEmber.transform.DOMove(endValue, num2 * num).SetEase(Ease.OutExpo).SetRelative(isRelative: true);
			DOTween.Sequence().Append(newEmber.render.DOColor(fadedAlp, 0f).SetEase(Ease.Linear)).Append(newEmber.render.DOColor(whiteClear, num2 * num).SetEase(Ease.Linear))
				.AppendCallback(delegate
				{
					newEmber.render.enabled = false;
				});
		}
	}
}
