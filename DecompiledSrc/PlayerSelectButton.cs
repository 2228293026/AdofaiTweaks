using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButton : PauseButton
{
	private const float ButtonXOffset = 60f;

	public int playerIndex;

	public Image upArrow;

	public Image downArrow;

	public GameObject arrows;

	public GameObject[] playerIcons;

	public RectTransform[] planets;

	public RectTransform sign;

	[NonSerialized]
	public ControllerType controllerType = ControllerType.None;

	[NonSerialized]
	private PlayerSelect playerSelect;

	[NonSerialized]
	private List<ControllerType> availableControllers;

	private bool focus;

	private float ySign;

	private float startPosition;

	private bool selectingControllers;

	public new void Awake()
	{
		base.Awake();
		playerIcons[playerIndex].SetActive(value: true);
		ySign = sign.anchoredPosition.y;
		label.gameObject.SetActive(value: false);
	}

	public void Setup(PlayerSelect playerSelect)
	{
		this.playerSelect = playerSelect;
		Relocate(4, animate: false);
		SetFocus(false);
	}

	public override void SetFocus(bool focus)
	{
		base.SetFocus(focus);
		this.focus = focus;
		arrows.SetActive(selectingControllers && focus);
		if (focus)
		{
			sign.DOKill(complete: true);
			sign.AnchorPosY(ySign);
			sign.DOAnchorPosY(ySign + 4f, 0.1f).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true)
				.SetLoops(2, LoopType.Yoyo);
			if (!selectingControllers)
			{
				return;
			}
			availableControllers = new List<ControllerType>(RDInput.desktopControllerTypes);
			PlayerSelectButton[] buttons = playerSelect.buttons;
			foreach (PlayerSelectButton playerSelectButton in buttons)
			{
				if (playerSelectButton.index <= playerSelect.playersSelected.Value)
				{
					ControllerType controllerType = playerSelectButton.controllerType;
					switch (controllerType)
					{
					case ControllerType.KeyboardLeft:
						availableControllers.Remove(ControllerType.KeyboardLeft);
						availableControllers.Remove(ControllerType.KeyboardFull);
						break;
					case ControllerType.KeyboardRight:
						availableControllers.Remove(ControllerType.KeyboardRight);
						availableControllers.Remove(ControllerType.KeyboardFull);
						break;
					case ControllerType.KeyboardFull:
						availableControllers.Remove(ControllerType.KeyboardLeft);
						availableControllers.Remove(ControllerType.KeyboardRight);
						availableControllers.Remove(ControllerType.KeyboardFull);
						break;
					default:
						availableControllers.Remove(controllerType);
						break;
					case ControllerType.Gamepad:
						break;
					}
				}
			}
			this.controllerType = availableControllers[0];
			ControllerTypeChanged();
		}
		else if (!selectingControllers)
		{
			int num = 0;
			RectTransform[] array = planets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
				num++;
			}
		}
	}

	private new void LateUpdate()
	{
		base.LateUpdate();
		if (!focus)
		{
			return;
		}
		RectTransform[] array = planets;
		foreach (RectTransform obj in array)
		{
			float num = (obj.localEulerAngles.z + 70f * Time.unscaledDeltaTime) % 360f;
			obj.localEulerAngles = Vector3.forward * num;
		}
		if (selectingControllers && !playerSelect.waitingForInput)
		{
			if (RDInput.upPress)
			{
				GoToNextControllerType(-1);
			}
			else if (RDInput.downPress)
			{
				GoToNextControllerType(1);
			}
		}
		void GoToNextControllerType(int direction)
		{
			FlashIcon((direction == 1) ? downArrow : upArrow);
			base.pauseMenu.PlayMenuSfx(SfxSound.MenuNavigate);
			_ = base.pauseMenu.currentPauseButton;
			int num2 = (int)Mathf.Repeat(availableControllers.IndexOf(controllerType) + direction, availableControllers.Count);
			controllerType = availableControllers[num2];
			ControllerTypeChanged();
		}
	}

	public void ControllerTypeChanged()
	{
		bool flag = controllerType != ControllerType.None;
		label.text = (flag ? RDString.Get("enum.ControllerType." + controllerType) : "");
		icon.gameObject.SetActive(flag);
		icon.sprite = (flag ? Resources.Load<Sprite>($"ControllerType/{controllerType}") : null);
	}

	public void Move(bool show)
	{
		CanvasScaler canvasScaler = base.pauseMenu.canvasScaler;
		float num = (float)Screen.height / canvasScaler.referenceResolution.y * canvasScaler.referenceResolution.x;
		Ease ease = (show ? Ease.OutQuad : Ease.InQuad);
		float x = (show ? (startPosition + num) : startPosition);
		float endValue = (show ? startPosition : (startPosition + num));
		SetFocus(false);
		base.rectTransform.DOKill();
		base.rectTransform.AnchorPosX(x);
		base.rectTransform.DOAnchorPosX(endValue, 1f).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.Done();
	}

	public void Relocate(int playerCount, bool animate)
	{
		bool flag = playerCount == 4;
		startPosition = 60f * ((float)playerIndex - (float)(playerCount - 1) / 2f);
		float duration = ((!animate) ? 0f : (flag ? 0.25f : 0.5f));
		base.rectTransform.DOKill();
		base.rectTransform.DOAnchorPosX(startPosition, duration).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true)
			.Done();
	}

	public void SetSelectingControllers(bool enable)
	{
		RectTransform[] array = planets;
		foreach (RectTransform target in array)
		{
			Vector3 endValue = (enable ? Vector3.zero : Vector3.one);
			Ease ease = (enable ? Ease.InBack : Ease.OutBack);
			target.DOScale(endValue, 0.25f).SetEase(ease).SetUpdate(isIndependentUpdate: true);
		}
		selectingControllers = enable;
		if (enable)
		{
			ControllerTypeChanged();
		}
		else
		{
			icon.gameObject.SetActive(value: false);
			arrows.SetActive(value: false);
		}
		label.gameObject.SetActive(enable);
	}
}
