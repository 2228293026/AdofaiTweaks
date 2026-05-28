using DG.Tweening;
using UnityEngine;

public class scrCalibrax : ADOBase
{
	public SpriteRenderer spriteRenderer;

	public Color basicColor;

	public Color outlierColor;

	private Material material;

	private void Awake()
	{
		material = spriteRenderer.material;
		Time.timeScale = 1f;
		AudioListener.pause = false;
	}

	private void Start()
	{
		base.transform.localScale = Vector3.one * 1.5f;
		Color color = material.color;
		material.color = new Color(1f, 1f, 1f, 0.5f);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(base.transform.DOScale(0.3f, 0.5f));
		sequence.Insert(0f, material.DOColor(color, 0.5f).SetEase(Ease.InOutQuad));
		sequence.Play();
	}

	public void FadeAndDestroy()
	{
		spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(base.transform.DOScale(0.6f, 0.5f));
		sequence.Play();
		Object.Destroy(base.gameObject, 1f);
	}

	public void SetOutlier(bool isOutlier)
	{
		if (isOutlier)
		{
			spriteRenderer.color = outlierColor;
		}
		else
		{
			spriteRenderer.color = basicColor;
		}
	}
}
