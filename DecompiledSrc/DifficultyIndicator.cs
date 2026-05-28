using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyIndicator : ADOBase
{
	private const float spinDur = 1.2f;

	private const float spinOffset = 0.120000005f;

	private const float loopInterval = 0f;

	public Image[] stars;

	private int starCount;

	private float loopDelay;

	private float timeTillSpin;

	private Sequence spinSeq;

	private void Update()
	{
		timeTillSpin -= Time.unscaledDeltaTime;
		if (loopDelay > 0f && timeTillSpin <= 0f)
		{
			Spin();
			timeTillSpin = loopDelay;
		}
	}

	public void SetStars(int val)
	{
		val = Mathf.Clamp(val, 0, stars.Length);
		Color color = ((val <= 3) ? ADOBase.gc.difficultyEasy : ((val <= 6) ? ADOBase.gc.difficultyMedium : ((val <= 9) ? ADOBase.gc.difficultyHard : ADOBase.gc.difficultyHardest)));
		Image[] array = stars;
		foreach (Image image in array)
		{
			int num = Array.IndexOf(stars, image) + 1;
			Color color2 = color;
			image.color = color2;
			image.gameObject.SetActive(num <= val);
		}
		starCount = val;
		Spin();
	}

	private void Spin()
	{
		int num = starCount;
		loopDelay = 0.120000005f * (float)num + 1.2f + 0f;
		for (int i = 0; i < num; i++)
		{
			int num2 = i;
			stars[num2].rectTransform.DORotate(new Vector3(0f, 0f, 360f), 1.2f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine).SetDelay((float)num2 * 0.120000005f);
		}
	}
}
