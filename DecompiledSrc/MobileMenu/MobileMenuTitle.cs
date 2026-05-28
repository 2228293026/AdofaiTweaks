using DG.Tweening;
using TMPro;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuTitle : ADOBase
{
	public GameObject logoNeoCosmos;

	public GameObject logoAdofai;

	public SpriteRenderer buttonNeoCosmos;

	public SpriteRenderer buttonAdofai;

	public TMP_Text subtitle;

	public CanvasGroup canvasGroup;

	public Transform dlcTransitionButton;

	public RectTransform loadingTransform;

	public GameObject background;

	public GameObject foreground;

	public bool loading;

	public void ShowButtons(bool show, bool instant)
	{
		dlcTransitionButton.DOLocalMoveY((!show) ? 3 : 0, instant ? 0f : 0.25f).SetEase(Ease.OutSine);
	}

	public void SetNeoCosmos(bool neoCosmos)
	{
		logoNeoCosmos.SetActive(neoCosmos);
		logoAdofai.SetActive(!neoCosmos);
		buttonNeoCosmos.gameObject.SetActive(!neoCosmos && !ADOBase.isSwitch);
		buttonAdofai.gameObject.SetActive(neoCosmos);
	}

	public void SetLoading(bool loading)
	{
		bool flag = MobileMenuCoopIntro.GetIntroType() == IntroType.NoIntro;
		if (loading)
		{
			loadingTransform.DORotate(Vector3.forward * 360f, 1f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
		}
		loadingTransform.gameObject.SetActive(loading);
		background.SetActive(loading && !flag);
		scrLogoText.instance.Enable(flag);
		loadingTransform.anchoredPosition = new Vector2(0f, flag ? (-229.5f) : (-100f));
		this.loading = loading;
		FadeSubtitle(loading ? 0f : 1f, loading);
	}

	public void UpdateSubtitle()
	{
		if (RDC.forceUnlockAllLevels)
		{
			RDC.forceUnlockAllLevels = false;
		}
		int overallProgressStage = Persistence.GetOverallProgressStage();
		RDC.forceUnlockAllLevels = Persistence.unlockAllLevels;
		scrTextChanger[] components = subtitle.GetComponents<scrTextChanger>();
		bool flag = false;
		scrTextChanger[] array = components;
		foreach (scrTextChanger scrTextChanger2 in array)
		{
			if (overallProgressStage >= scrTextChanger2.minStage)
			{
				flag = true;
				break;
			}
		}
		if (!flag || GCS.FOOL_JOKER || ADOBase.isExpo)
		{
			logoAdofai.transform.DOLocalMoveY((GCS.FOOL_JOKER || ADOBase.isExpo) ? 0.4f : 0f, 0.25f).SetEase(Ease.OutSine);
			logoNeoCosmos.transform.DOLocalMoveY((GCS.FOOL_JOKER || ADOBase.isExpo) ? 0.4f : 0f, 0.25f).SetEase(Ease.OutSine);
		}
		dlcTransitionButton.gameObject.SetActive(overallProgressStage >= 1);
		subtitle.gameObject.SetActive(!GCS.FOOL_JOKER && !ADOBase.isExpo);
	}

	public void FadeSubtitle(float alpha, bool instant)
	{
		float duration = (instant ? 0f : 0.25f);
		canvasGroup.DOKill();
		canvasGroup.DOFade(alpha, duration);
		if (instant)
		{
			canvasGroup.DOComplete();
		}
	}
}
