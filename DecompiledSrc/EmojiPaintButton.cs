using DG.Tweening;
using UnityEngine;

public class EmojiPaintButton : ADOBase
{
	public SpriteRenderer faceSprite;

	public SpriteRenderer faceDetails;

	private int faceIndex;

	private Vector3 baseScale;

	private void Awake()
	{
		baseScale = base.transform.localScale;
	}

	private void Update()
	{
		if (ADOBase.conductor.onBeatHappened)
		{
			Beat();
		}
	}

	public void Beat()
	{
		int num = ADOBase.gc.faceBaseSprites.Length - 1;
		int num2 = 0;
		for (int i = 0; i < 100; i++)
		{
			num2 = ((Random.Range(0, 1000) != 0) ? Random.Range(0, num) : num);
			if (num2 != faceIndex)
			{
				break;
			}
		}
		faceIndex = num2;
		faceSprite.sprite = ADOBase.gc.faceBaseSprites[faceIndex];
		faceDetails.sprite = ADOBase.gc.faceBaseDetailSprites[faceIndex];
		base.transform.localScale = baseScale * 1.25f;
		base.transform.DOScale(baseScale, 0.33f).SetEase(Ease.OutCubic);
	}
}
