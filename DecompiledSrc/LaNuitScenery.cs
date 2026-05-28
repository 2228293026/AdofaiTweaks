using DG.Tweening;
using UnityEngine;

public class LaNuitScenery : ADOBase
{
	public Animator animator;

	public SpriteRenderer[] sprites;

	public void FadeOut()
	{
		SpriteRenderer[] array = sprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DOFade(0f, 0.5f);
		}
		base.gameObject.SetActive(value: false);
	}
}
