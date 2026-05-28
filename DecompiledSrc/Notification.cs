using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Notification : ADOBase
{
	private static Notification _instance;

	public RectTransform bar;

	public Button button;

	public Image icon;

	public Text text;

	private bool showCalibrationOnTap;

	[Header("Icons")]
	public Sprite calibrationIcon;

	public Sprite warningIcon;

	public Sprite completeIcon;

	public static Notification instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.Instantiate(RDConstants.data.prefab_notification).GetComponent<Notification>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		text.SetLocalizedFont();
	}

	private void SetupNotification(float delay = 3f, float scale = 1f, bool reset = true)
	{
		bar.SizeDeltaY(170f * scale);
		button.onClick.RemoveAllListeners();
		bar.DOKill();
		if (reset)
		{
			bar.AnchorPosX(0f - bar.sizeDelta.x);
			bar.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
				.SetDelay(0.5f)
				.OnComplete(delegate
				{
					bar.DOAnchorPosX(0f - bar.sizeDelta.x, 0.3f).SetEase(Ease.InExpo).SetUpdate(isIndependentUpdate: true)
						.SetDelay(delay);
				});
		}
		else
		{
			bar.AnchorPosX(0f);
			bar.DOAnchorPosX(0f - bar.sizeDelta.x, 0.3f).SetEase(Ease.InExpo).SetUpdate(isIndependentUpdate: true)
				.SetDelay(delay);
		}
	}

	public void ShowCalibration()
	{
		CalibrationPreset currentPreset = scrConductor.currentPreset;
		SetupNotification(currentPreset.confident ? 3f : 5f);
		button.onClick.AddListener(MoreInfoCalibration);
		icon.sprite = calibrationIcon;
		icon.color = Color.white;
		Dictionary<string, object> parameters = new Dictionary<string, object> { 
		{
			"device",
			"<b>" + currentPreset.ReadableOutputName() + "</b>"
		} };
		if (currentPreset.confident)
		{
			text.text = RDString.Get("calibration.notification.confidentPreset", parameters).Replace("[[", "<color=red>").Replace("]]", "</color>");
			showCalibrationOnTap = false;
		}
		else
		{
			text.text = RDString.Get("calibration.notification.notConfidentPreset", parameters).Replace("[[", "<color=red>").Replace("]]", "</color>");
			showCalibrationOnTap = true;
		}
	}

	private void MoreInfoCalibration()
	{
		if (showCalibrationOnTap)
		{
			ADOBase.GoToCalibration();
		}
		else
		{
			ADOBase.controller.pauseMenu.Show(PauseMenu.Submenu.Settings, playSound: true);
		}
	}

	public void ShowAudioBufferChange(int value)
	{
		SetupNotification(5f);
		icon.sprite = warningIcon;
		icon.color = new Color(1f, 0.2f, 0.2f);
		text.text = RDString.Get("error.audioBufferSizeChanged", new Dictionary<string, object> { { "value", value } });
	}

	public void ShowNoSpace()
	{
		SetupNotification(5f);
		icon.sprite = warningIcon;
		icon.color = new Color(1f, 0.2f, 0.2f);
		text.text = RDString.Get("error.noSpace.Notification");
	}

	public void ShowIncompatibleCloud()
	{
		SetupNotification(7f, 1.75f);
		icon.sprite = warningIcon;
		icon.color = new Color(1f, 0.2f, 0.2f);
		text.text = RDString.Get("error.cloudSaves.Notification", new Dictionary<string, object>
		{
			{
				"version",
				Application.version
			},
			{ "release", 141 }
		});
	}

	public void ShowGameServicesTimeOut()
	{
		SetupNotification(5f);
		button.onClick.AddListener(MoreInforServicesTimeOut);
		icon.sprite = warningIcon;
		icon.color = new Color(1f, 0.7568628f, 0.02745098f);
		text.text = RDString.Get("error.gameServices.timeOut");
	}

	private void MoreInforServicesTimeOut()
	{
		SetupNotification(5f, 1.5f, reset: false);
		text.text = RDString.Get("error.gameServices.timeOut.moreInfo");
	}

	public void ShowGameServicesComplete()
	{
		SetupNotification(5f);
		button.onClick.AddListener(ADOBase.GoToLevelSelect);
		icon.sprite = completeIcon;
		icon.color = new Color(0.2f, 0.7215686f, 0.3921569f);
		text.text = RDString.Get("error.gameServices.complete");
	}

	public void ShowEntitlementMessage(bool successfull, string token)
	{
		SetupNotification(5f);
		button.onClick.AddListener(successfull ? new UnityAction(MoreInforEntitlementMessage) : null);
		icon.sprite = (successfull ? completeIcon : warningIcon);
		icon.color = (successfull ? new Color(0.2f, 0.7215686f, 0.3921569f) : new Color(1f, 0.7568628f, 0.02745098f));
		text.text = RDString.Get(token);
	}

	private void MoreInforEntitlementMessage()
	{
		SetupNotification(5f, 2f, reset: false);
		text.text = RDString.Get("levelSelect.entitlement.complete.information");
	}
}
