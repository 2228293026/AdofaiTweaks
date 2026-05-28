using DG.Tweening;
using UnityEngine;

public class ffxPADiamond : ffxPlusBase
{
	public bool hideDiamond;

	public bool doubleDiamond;

	public bool circle;

	public bool heart;

	private GameObject diamondPrefab;

	private GameObject circlePrefab;

	private GameObject heartPrefab;

	public override bool runOnHit => true;

	public override void Awake()
	{
		base.Awake();
		circlePrefab = ADOBase.gc.prefab_PA_circle;
		diamondPrefab = ADOBase.gc.prefab_PA_diamond;
		heartPrefab = ADOBase.gc.prefab_PA_heart;
	}

	public override void StartEffect(scrPlanet planet)
	{
		float num = 1f / scrConductor.instance.song.pitch;
		if (!hideDiamond)
		{
			Transform smallDiamond = Object.Instantiate(diamondPrefab, base.transform.position, base.transform.rotation).transform;
			smallDiamond.ScaleXY(0f, 0f);
			smallDiamond.DOScale(new Vector2(1f, 1f), 3f * num).SetEase(Ease.OutExpo);
			smallDiamond.GetComponent<SpriteRenderer>().DOFade(0f, num).SetDelay(num)
				.OnComplete(delegate
				{
					Object.Destroy(smallDiamond.gameObject);
				});
		}
		if (doubleDiamond)
		{
			Transform bigDiamond = Object.Instantiate(diamondPrefab, base.transform.position, base.transform.rotation).transform;
			bigDiamond.ScaleXY(0f, 0f);
			bigDiamond.DOScale(new Vector2(1.5f, 1.5f), 2.5f * num).SetEase(Ease.OutExpo);
			bigDiamond.GetComponent<SpriteRenderer>().DOFade(0f, num).SetDelay(0.75f * num)
				.OnComplete(delegate
				{
					Object.Destroy(bigDiamond.gameObject);
				});
		}
		if (circle)
		{
			Transform _circle = Object.Instantiate(circlePrefab, base.transform.position, Quaternion.identity).transform;
			_circle.ScaleXY(0f, 0f);
			_circle.DOScale(new Vector2(1f, 1f), 10f * num).SetEase(Ease.OutExpo);
			_circle.GetComponent<SpriteRenderer>().DOFade(0f, 2f * num).SetDelay(num)
				.OnComplete(delegate
				{
					Object.Destroy(_circle.gameObject);
				});
		}
		if (heart)
		{
			_ = Object.Instantiate(heartPrefab, base.transform.position.WithY(base.transform.position.y + 0.5f), Quaternion.identity).transform;
		}
	}
}
