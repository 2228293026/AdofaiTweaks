using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuReviewPrompt : ADOBase
{
	public MobileMenuController menuController;

	public Button buttonSubmit;

	public Button buttonCancel;

	public Transform background;

	public Camera camera;

	public Canvas canvas;

	private void Awake()
	{
		buttonSubmit.onClick.AddListener(delegate
		{
			GoToReviewPage();
		});
		buttonCancel.onClick.AddListener(delegate
		{
			ShowRatingPrompt(show: false);
		});
	}

	public bool TryRunReviewPrompt()
	{
		if (ADOBase.isExpo || !ADOBase.isMobile || ADOBase.isSwitch)
		{
			return false;
		}
		if (!Persistence.ratedGame && Persistence.IsWorldComplete(0))
		{
			int nextRatingPromptDay = Persistence.GetNextRatingPromptDay();
			DateTime now = DateTime.Now;
			DateTime dateTime = new DateTime(2020, 1, 1, 0, 0, 0);
			int days = (now - dateTime).Days;
			if (nextRatingPromptDay < 0 || nextRatingPromptDay <= days)
			{
				Persistence.SetNextRatingPromptDay(days + 7);
				Persistence.Save();
				if (ShowRatingPrompt())
				{
					return true;
				}
			}
		}
		return false;
	}

	private void LateUpdate()
	{
		int num = Mathf.CeilToInt((float)Screen.width * 1f / (float)Screen.height);
		float orthographicSize = camera.orthographicSize;
		float x = orthographicSize * 2f * (float)num;
		float y = orthographicSize * 2f;
		background.ScaleXY(x, y);
		canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
	}

	public bool ShowRatingPrompt(bool show = true)
	{
		if (!show)
		{
			ADOBase.conductor.song2.DOFade(1f, 0.5f);
			ADOBase.conductor.song3.DOFade(1f, 0.5f);
		}
		base.gameObject.SetActive(show);
		menuController.Enable(!show);
		if (!show)
		{
			menuController.JumpToMenuEntrance();
		}
		return true;
	}

	private void GoToReviewPage()
	{
		ShowRatingPrompt(show: false);
		Persistence.ratedGame = true;
		ADOBase.platformHelper.OpenURL("https://play.google.com/store/apps/details?id=com.fizzd.connectedworlds");
	}
}
