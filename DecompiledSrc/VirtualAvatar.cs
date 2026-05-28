using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VirtualAvatar : ADOBase
{
	public int playerIndex;

	public RectTransform rectTransform;

	public Image circle;

	public Image avatar;

	public Sprite idle;

	public Sprite tap;

	public Sprite sad;

	public Sprite happy;

	private Sequence timer;

	private bool finished;

	public void Hit()
	{
		if (!finished)
		{
			avatar.sprite = tap;
			if (timer != null && timer.active)
			{
				timer.Kill();
			}
			timer = DOTween.Sequence().AppendInterval(ADOBase.controller.GetAutoBlinkDuration() / 2f).OnComplete(delegate
			{
				avatar.sprite = idle;
			})
				.SetUpdate(isIndependentUpdate: true)
				.OnKill(delegate
				{
					avatar.sprite = idle;
				});
		}
	}

	public void Win()
	{
		finished = true;
		if (happy != null)
		{
			avatar.sprite = happy;
		}
	}

	public void Lose()
	{
		finished = true;
		avatar.sprite = sad;
	}

	public void Revive()
	{
		finished = false;
		avatar.sprite = idle;
	}

	public void Show(bool show, bool instant = false)
	{
		float endValue = (show ? 170f : (-250f));
		Ease ease = (show ? Ease.OutBack : Ease.InBack);
		rectTransform.DOAnchorPosY(endValue, instant ? 0f : 0.5f).SetEase(ease, 1.3f).SetUpdate(isIndependentUpdate: true)
			.Done();
	}
}
