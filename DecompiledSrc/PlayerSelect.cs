using System;
using System.Linq;
using DG.Tweening;
using MobileMenu;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : ADOBase
{
	public const float TransitionDuration = 0.5f;

	public PlayerSelectButton[] buttons;

	public PauseMenu pauseMenu;

	public ZodiacBackground zodiacBackground;

	public CanvasGroup canvasGroup;

	public CanvasGroup joyconsCanvasGroup;

	public Text title;

	private bool setup;

	[NonSerialized]
	public int? playersSelected;

	private ControllerType[] playerControllers;

	private Joystick[] playerJoysticks;

	[NonSerialized]
	public bool waitingForInput;

	private bool showingAtStartup;

	private double lastTimeSelectedInput;

	private bool selectingControllers => playersSelected.HasValue;

	private int playerCount => playersSelected.Value;

	private PlayerSelectButton currentButton => (PlayerSelectButton)pauseMenu.currentPauseButton;

	private int currentIndex
	{
		get
		{
			GeneralPauseButton[] array = buttons;
			return Array.IndexOf(array, pauseMenu.currentPauseButton);
		}
	}

	public void Setup()
	{
		PlayerSelectButton[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Setup(this);
		}
		setup = true;
	}

	public void Show(bool instant)
	{
		if (!setup)
		{
			Setup();
		}
		if (!instant)
		{
			MoveButtons(show: true);
		}
		showingAtStartup = instant;
		base.gameObject.SetActive(value: true);
		float num = (instant ? 0f : 0.5f);
		canvasGroup.DOFade(1f, num).From(0f).SetUpdate(isIndependentUpdate: true);
		joyconsCanvasGroup.DOFade(1f, num).From(0f).SetDelay(num / 2f)
			.SetUpdate(isIndependentUpdate: true);
		DOVirtual.DelayedCall(num * 2f, delegate
		{
			pauseMenu.pausePlanetsImage.enabled = true;
			pauseMenu.SelectPauseButton(buttons[0], 1f, 4f, instant: false, null, playSound: false);
			pauseMenu.transitioning = false;
		});
		zodiacBackground.Show();
	}

	public void MoveButtons(bool show)
	{
		if (!setup)
		{
			Setup();
		}
		PlayerSelectButton[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Move(show);
		}
		SfxSound sfxSound = (show ? SfxSound.MenuChainSlideIn : SfxSound.MenuChainSlideOut);
		scrSfx.instance.PlaySfx(sfxSound, MixerGroup.InterfaceParent);
	}

	public void Hide(bool instant)
	{
		if (!instant)
		{
			pauseMenu.transitioning = true;
			pauseMenu.pausePlanetsImage.enabled = false;
			MoveButtons(show: false);
			DOVirtual.DelayedCall(0.5f, delegate
			{
				canvasGroup.DOFade(0f, 0.5f).SetUpdate(isIndependentUpdate: true);
				pauseMenu.ShowFromPlayerSelect(show: true);
				zodiacBackground.Hide();
			});
			joyconsCanvasGroup.DOFade(0f, 0.25f).SetUpdate(isIndependentUpdate: true);
		}
		else
		{
			pauseMenu.ShowFromPlayerSelect(show: true, instant: true);
			zodiacBackground.Hide();
		}
	}

	public void InputUpdate()
	{
		if (RDInput.backPress && !waitingForInput)
		{
			BackButtonAction();
		}
		else if (!selectingControllers)
		{
			if (RDInput.rightPress)
			{
				SelectHorizontal(1);
			}
			else if (RDInput.leftPress)
			{
				SelectHorizontal(-1);
			}
		}
		else if (waitingForInput && ReInput.isReady)
		{
			if (IsAnyJoystickAvailable())
			{
				SetWaitingForInput(waiting: false);
				currentButton.label.text = RDString.Get("playerSelect.noGamepadsAvailable");
			}
			else
			{
				CheckForJoystickInputs();
			}
		}
	}

	public int SelectHorizontal(int direction)
	{
		int num = (int)Mathf.Repeat(currentIndex + direction, buttons.Length);
		zodiacBackground.SwitchPlayerCount(num, direction);
		Select(num);
		return num;
	}

	public void Select(int index, bool playSound = true)
	{
		pauseMenu.SelectPauseButton(buttons[index], 1f, 4f, instant: false, null, playSound: false);
		if (playSound)
		{
			pauseMenu.PlayMenuSfx(SfxSound.MenuNavigate);
		}
	}

	private void Finish()
	{
		scrPlayerManager.ResetPlayersAppearance();
		scrPlayerManager.SetPlayerCount(playerCount);
		RDInput.ReassignControllers(playerCount, playerControllers, playerJoysticks);
		ADOBase.controller.RestartProgress();
		if (pauseMenu.shouldUseGamePauseButtons)
		{
			if (ADOBase.isMobileMenu && playerCount != 1)
			{
				scnMobileMenu.introPhase = IntroPhase.PlayerSelected;
				scnMobileMenu.returnToLevelAfterIntroFinished = true;
				GCS.sceneToLoad = GCNS.sceneLevelSelect;
				ADOBase.controller.StartLoadingScene();
			}
			else
			{
				ADOBase.controller.Restart();
			}
		}
		else
		{
			scnMobileMenu.introPhase = IntroPhase.PlayerSelected;
			GCS.sceneToLoad = GCNS.sceneLevelSelect;
			ADOBase.controller.StartLoadingScene();
		}
	}

	public void Choose(int index)
	{
		if (Time.realtimeSinceStartupAsDouble - lastTimeSelectedInput < 0.1)
		{
			return;
		}
		int num = index + 1;
		if (!selectingControllers)
		{
			if (currentIndex != index)
			{
				Select(index);
			}
			pauseMenu.PlayMenuSfx(SfxSound.MobileButtonEnter);
			if (ADOBase.isSwitch)
			{
				base.enabled = false;
				playersSelected = num;
				Finish();
			}
			else if (num == 1)
			{
				playersSelected = 1;
				Finish();
			}
			else
			{
				SetSelectingControllers(enable: true, num);
			}
		}
		else if (!waitingForInput && currentIndex == index)
		{
			playerControllers[index] = currentButton.controllerType;
			currentButton.FlashIcon(currentButton.icon);
			if (currentButton.controllerType == ControllerType.Gamepad)
			{
				SetWaitingForInput(waiting: true);
				currentButton.label.text = RDString.Get("playerSelect.waitingForInput");
			}
			else
			{
				SelectNextControllerType();
			}
		}
	}

	private void SelectNextControllerType()
	{
		lastTimeSelectedInput = Time.realtimeSinceStartupAsDouble;
		SetWaitingForInput(waiting: false);
		if (currentIndex + 1 < playerCount)
		{
			SelectHorizontal(1);
		}
		else
		{
			Finish();
		}
	}

	private void SetSelectingControllers(bool enable, int playerCount = 4)
	{
		playersSelected = (enable ? new int?(playerCount) : ((int?)null));
		if (enable)
		{
			title.text = RDString.Get("playerSelect.controllers");
			for (int i = 0; i < buttons.Length; i++)
			{
				PlayerSelectButton playerSelectButton = buttons[i];
				if (i < playerCount)
				{
					playerSelectButton.Relocate(playerCount, animate: true);
					playerSelectButton.controllerType = ControllerType.None;
					playerSelectButton.SetSelectingControllers(enable: true);
				}
				else
				{
					playerSelectButton.Move(show: false);
				}
			}
			playerControllers = new ControllerType[playerCount];
			playerJoysticks = (Joystick[])(object)new Joystick[playerCount];
			zodiacBackground.SwitchPlayerCount(0, 1);
			Select(0, playSound: false);
		}
		else
		{
			title.text = RDString.Get("playerSelect.title");
			PlayerSelectButton[] array = buttons;
			foreach (PlayerSelectButton obj in array)
			{
				obj.Relocate(4, animate: true);
				obj.SetSelectingControllers(enable: false);
			}
		}
	}

	private void OnGUI()
	{
		string text = "";
		foreach (Joystick joystick in ReInput.controllers.Joysticks)
		{
			text += $"joystick {((Controller)joystick).name}, id {((Controller)joystick).deviceInstanceGuid}\n";
		}
		GUI.Label(new Rect(0f, 0f, 600f, 200f), text);
	}

	private void CheckForJoystickInputs()
	{
		foreach (Joystick joystick in ReInput.controllers.Joysticks)
		{
			if (((Controller)joystick).GetAnyButtonDown())
			{
				PlayerSelectButton playerSelectButton = (PlayerSelectButton)pauseMenu.currentPauseButton;
				if (!playerJoysticks.Contains(joystick))
				{
					playerSelectButton.label.text = ((Controller)joystick).name;
					playerJoysticks[currentIndex] = joystick;
					SelectNextControllerType();
					break;
				}
				playerSelectButton.label.text = RDString.Get("playerSelect.gamepadAlreadyAssigned");
				SetWaitingForInput(waiting: false);
			}
		}
	}

	private void SetWaitingForInput(bool waiting)
	{
		lastTimeSelectedInput = Time.realtimeSinceStartupAsDouble;
		waitingForInput = waiting;
		currentButton.arrows.SetActive(!waiting);
	}

	private bool IsAnyJoystickAvailable()
	{
		return ReInput.controllers.Joysticks.All((Joystick joystick) => playerJoysticks.Contains(joystick));
	}

	public void BackButtonAction(bool isUIButton = false)
	{
		if (!showingAtStartup)
		{
			if (selectingControllers)
			{
				SetSelectingControllers(enable: false);
			}
			else
			{
				Hide(instant: false);
			}
			pauseMenu.PlayMenuSfx(SfxSound.MenuBack);
		}
	}
}
