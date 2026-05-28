using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuTreePuzzle : ADOBase
{
	public MobileMenuController menuController;

	public RectTransform XHContainer;

	public Button XHButton;

	public Transform XHTree;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		MobileMenuController mobileMenuController = menuController;
		mobileMenuController.onFinishLoading = (Action)Delegate.Combine(mobileMenuController.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		if (!GCS.FOOL_JOKER && !ADOBase.isExpo)
		{
			bool unlockedXH = Persistence.unlockedXH;
			if (Persistence.IsWorldComplete(ADOBase.worldData["XC"].index) && !unlockedXH)
			{
				base.gameObject.SetActive(value: true);
				Transform parent = ADOBase.controller.pauseMenu.mainMenuContainer.transform;
				XHContainer.SetParent(parent);
				XHContainer.anchoredPosition = Vector2.zero;
				XHButton.onClick.AddListener(DoUnlock);
			}
		}
	}

	private void DoUnlock()
	{
		XHButton.enabled = false;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Insert(0f, XHButton.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
			.Insert(0f, XHButton.transform.DOLocalRotate(Vector3.forward * 360f * -1.5f, 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InExpo))
			.Insert(0.2f, XHTree.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
			.Insert(0.2f, XHTree.transform.DOLocalRotate(Vector3.forward * 360f * 1.5f, 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InExpo));
		MobileMenuController.PlayPuzzleSfx(SfxSound.MobileMenuXH);
		UnlockXH();
	}

	private void UnlockXH()
	{
		scrController.instance.TogglePauseGame();
		scrFlash.Flash();
		Persistence.unlockedXH = true;
		menuController.map.portalLUT["XH"].visible = true;
		menuController.map.Build(instantiate: true);
	}
}
