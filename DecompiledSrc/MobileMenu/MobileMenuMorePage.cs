using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuMorePage : ADOBase
{
	public MobileMenuController menuController;

	public TMP_Text featuredLevelsDescription;

	public SpriteRenderer promptIcon;

	public TMP_Text promptLeft;

	public TMP_Text promptRight;

	public MobileMenuMoreScreen descriptionScreen;

	private Sequence promptFadeSequence;

	private bool featuredLevelsDLCInstalled => ADOBase.isSwitch;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		MobileMenuController mobileMenuController = menuController;
		mobileMenuController.onFinishLoading = (Action)Delegate.Combine(mobileMenuController.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		if (!GCS.FOOL_JOKER && !ADOBase.isExpo && !ADOBase.isMobile)
		{
			base.gameObject.SetActive(value: true);
			string[] array = RDString.Get("levelSelect.more.storePrompt").Split("[prompt]", StringSplitOptions.None);
			promptLeft.text = array[0].TrimEnd();
			promptRight.text = array[1].TrimStart();
			float num = promptIcon.bounds.size.x * promptIcon.transform.localScale.x;
			num += num * 2f;
			float num2 = promptLeft.preferredWidth * promptLeft.transform.localScale.x;
			float num3 = promptRight.preferredWidth * promptRight.transform.localScale.x;
			float num4 = (num2 + num + num3) / 2f;
			promptLeft.transform.localPosition = promptLeft.transform.localPosition.WithX(0f - num4);
			promptIcon.transform.localPosition = promptIcon.transform.localPosition.WithX(num2 + num / 2f - num4);
			promptRight.transform.localPosition = promptRight.transform.localPosition.WithX(num2 + num - num4);
			if (!featuredLevelsDLCInstalled)
			{
				TMP_Text tMP_Text = featuredLevelsDescription;
				tMP_Text.text = tMP_Text.text + "\n\n<color=#85afff>" + RDString.Get("levelSelect.more.featuredLevelsDownload") + "</color>";
			}
			ShowPrompt(show: false, instant: true);
		}
	}

	public void OnSelectDescription(bool select, bool instant)
	{
		if (!featuredLevelsDLCInstalled)
		{
			ShowPrompt(select, instant);
		}
	}

	private void ShowPrompt(bool show, bool instant)
	{
		float duration = (instant ? 0f : 0.25f);
		float endValue = (show ? 1f : 0f);
		promptFadeSequence?.Kill();
		promptFadeSequence = DOTween.Sequence().Append(promptIcon.DOFade(endValue, duration)).Join(promptLeft.DOFade(endValue, duration))
			.Join(promptRight.DOFade(endValue, duration))
			.Done();
	}

	public bool TryGoToStorePage()
	{
		if (featuredLevelsDLCInstalled)
		{
			return false;
		}
		return true;
	}
}
