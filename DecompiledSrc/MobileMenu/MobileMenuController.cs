using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuController : ADOBase
{
	public MobileMenuMap map;

	public int selectedScreenIndex;

	public MobileMenuGroup currentGroup;

	[Header("References")]
	public MobileMenuSublevelBrowserGenerator sublevelBrowser;

	public MobileMenuGrabController grabController;

	public MobileMenuArrow buttonUp;

	public MobileMenuArrow buttonDown;

	public MobileMenuArrow buttonLeft;

	public MobileMenuArrow buttonRight;

	public RectTransform topButtonsContainer;

	public RectTransform bottomButtonsContainer;

	public RectTransform backButtonContainer;

	public CanvasGroup buttonsCanvasGroup;

	public Button buttonSpeedTrial;

	public Button buttonAprilFools;

	public Button buttonRestorePurchase;

	public Button buttonEX;

	public Button buttonBack;

	public TMP_Text textSpeedTrials;

	public TMP_Text textAprilFools;

	public TMP_Text textRestorePurchase;

	public TMP_Text caption;

	public Transform mapContainer;

	public RectTransform difficultyContainer;

	public DifficultyIndicator difficultyIndicator;

	public Button buttonClassicFeatured;

	public Button buttonTechFeatured;

	[Header("Background Elements")]
	public SpriteRenderer[] backgroundClouds;

	public SpriteRenderer background;

	public SpriteRenderer backgroundGradient;

	public SpriteRenderer[] backgrounds;

	public Action onFinishLoading;

	private Transform currentSubmenu;

	private int submenuLevel = -1;

	private float submenuScale = 1f;

	private bool expandedLevel;

	private bool speedTrial;

	private bool movingThisFrame;

	private bool wasPaused;

	private float lastAspectRatio;

	private Sequence screenTransition;

	[NonSerialized]
	public float targetTouchRefreshRate = 60f;

	public static MobileMenuController instance;

	private bool responsive = true;

	private Vector2 touchStartPos;

	private Vector2 cameraOrigPos;

	private bool dragIsVertical;

	[NonSerialized]
	public bool dragging;

	private bool dragCanBegin;

	private const float dragMinDistance = 5f;

	public MobileMenuScreen currentScreen => currentGroup.visibleScreens[selectedScreenIndex];

	public bool pauseIsPossible
	{
		get
		{
			if (currentGroup != null)
			{
				if (!isInPortal && !isInTrailer)
				{
					return !isInColor;
				}
				return false;
			}
			return true;
		}
	}

	public bool isInPortal
	{
		get
		{
			if (currentScreen is MobileMenuPortalScreen && sublevelBrowser != null)
			{
				return sublevelBrowser.container.gameObject.activeSelf;
			}
			return false;
		}
	}

	public bool isInTrailer
	{
		get
		{
			if (currentScreen is MobileMenuGalleryScreen mobileMenuGalleryScreen)
			{
				return mobileMenuGalleryScreen.isInTrailer;
			}
			return false;
		}
	}

	public bool isInColor => currentScreen is MobileMenuColorScreen;

	private bool speedTrialsAvailable
	{
		get
		{
			if (!ADOBase.isExpo)
			{
				return Persistence.GetSpeedTrialsAvailable();
			}
			return false;
		}
	}

	private bool aprilFoolsAvailable
	{
		get
		{
			if (ADOBase.IsAprilFools())
			{
				if (!GCS.FOOL_JOKER)
				{
					return Persistence.GetOverallProgressStage() >= 6;
				}
				return true;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (!ADOBase.isMobileMenu)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		instance = this;
		buttonUp.button.onClick.AddListener(delegate
		{
			MoveInDirection(MoveDirection.Up);
		});
		buttonDown.button.onClick.AddListener(delegate
		{
			MoveInDirection(MoveDirection.Down);
		});
		buttonLeft.button.onClick.AddListener(delegate
		{
			MoveInDirection(MoveDirection.Left);
		});
		buttonRight.button.onClick.AddListener(delegate
		{
			MoveInDirection(MoveDirection.Right);
		});
		lastAspectRatio = Camera.main.aspect;
		buttonSpeedTrial.onClick.AddListener(ToggleSpeedTrial);
		buttonSpeedTrial.gameObject.SetActive(speedTrialsAvailable);
		textSpeedTrials.text = RDString.Get("levelSelect.SpeedTrial");
		textSpeedTrials.SetLocalizedFont();
		buttonAprilFools.GetComponent<RectTransform>();
		buttonAprilFools.onClick.AddListener(ToggleAprilFools);
		buttonAprilFools.gameObject.SetActive(aprilFoolsAvailable);
		buttonAprilFools.image.sprite = (GCS.FOOL_JOKER ? RDC.data.sprAprilFoolslButtonOn : RDC.data.sprAprilFoolsButtonOff);
		textAprilFools.text = "???";
		textAprilFools.SetLocalizedFont();
		buttonBack.onClick.AddListener(BackButtonAction);
		caption.SetLocalizedFont();
		difficultyContainer.gameObject.SetActive(ADOBase.isExpo);
		grabController = new MobileMenuGrabController();
	}

	public void LoadMap(string mapName)
	{
		map = new MobileMenuMap(mapName);
		map.transform = mapContainer;
		map.Build(instantiate: true);
		map.SetMapCenter(map.rootGroup[0]);
		foreach (string key in map.portalLUT.Keys)
		{
			sublevelBrowser.GenerateSubmenu(key);
		}
		if (mapName == "main")
		{
			GCS.FOOL_JOKER = false;
		}
		SetSpeedTrial(GCS.speedTrialMode);
		StartCoroutine(OnFinishLoading());
	}

	public void LoadMapAsync(string mapName)
	{
		StartCoroutine(LoadMapCo(mapName));
	}

	private IEnumerator LoadMapCo(string mapName)
	{
		map = new MobileMenuMap(mapName);
		map.transform = mapContainer;
		map.Build();
		MobileMenuScreen mobileMenuScreen = map.rootGroup[0];
		Vector2 vector = map.rootGroup.GetHeight() / 2f * Vector2.up;
		mobileMenuScreen.Instantiate();
		mobileMenuScreen.RepositionTransform(vector, mapContainer);
		map.SetMapCenter(mobileMenuScreen);
		yield return GameServicesTimeOut();
		map.EvaluateAllConditions();
		map.Build();
		yield return map.InstantiateScreenTransformsCo();
		RefreshButtons();
		foreach (string key in map.portalLUT.Keys)
		{
			sublevelBrowser.GenerateSubmenu(key);
		}
		SetSpeedTrial(GCS.speedTrialMode);
		StartCoroutine(OnFinishLoading());
	}

	private IEnumerator GameServicesTimeOut()
	{
		while (GameServices.Instance.IsFirstLoading)
		{
			if (GameServices.Instance.TimeOut)
			{
				Notification.instance.ShowGameServicesTimeOut();
				break;
			}
			yield return null;
		}
	}

	private IEnumerator OnFinishLoading()
	{
		yield return new WaitForEndOfFrame();
		onFinishLoading?.Invoke();
	}

	public void JumpToMenuEntrance(float tweenDuration = 0.25f)
	{
		string worldEntrance = GCS.worldEntrance;
		MobileMenuScreen screen = map.rootGroup.visibleScreens[0];
		if (worldEntrance != null && map.portalLUT.ContainsKey(worldEntrance))
		{
			screen = map.portalLUT[worldEntrance];
		}
		JumpToScreen(screen, instant: false, tweenDuration);
	}

	public void JumpToScreen(MobileMenuScreen screen = null, bool instant = false, float tweenDuration = 0.25f)
	{
		if (screen == null)
		{
			if (currentGroup == null)
			{
				return;
			}
			screen = currentScreen;
		}
		screenTransition?.Kill();
		screenTransition = DOTween.Sequence();
		tweenDuration = (instant ? 0f : tweenDuration);
		Vector3 endValue = screen.transform.position.WithZ(ADOBase.controller.camy.transform.position.z);
		screenTransition.Join(ADOBase.controller.camy.transform.DOMove(endValue, tweenDuration).SetEase(Ease.OutSine));
		screenTransition.Join(ADOBase.controller.camy.camobj.DOOrthoSize(screen.parentGroup.zoom, tweenDuration)).SetEase(Ease.OutSine);
		MobileMenuGroup.BackgroundTheme theme = screen.parentGroup.theme.WithDefaults(map.rootGroup.theme);
		MobileMenuGroup.BackgroundTheme speedTheme = screen.parentGroup.speedTheme.WithDefaults(map.rootGroup.speedTheme);
		screenTransition.Join(DoTheme(theme, speedTheme, instant));
		screenTransition.Play();
		screenTransition.SetUpdate(UpdateType.Manual);
		if (instant)
		{
			screenTransition.Complete();
		}
		string description = screen.GetDescription();
		caption.text = description;
		caption.raycastTarget = description.Contains("<a href");
		if (currentGroup != null)
		{
			if (screen == currentScreen)
			{
				return;
			}
			currentScreen.onSelect(arg1: false, instant);
		}
		currentGroup = screen.parentGroup;
		selectedScreenIndex = currentGroup.visibleScreens.IndexOf(screen);
		screen.onSelect(arg1: true, instant);
		movingThisFrame = true;
		RefreshButtons();
		if (ADOBase.isExpo)
		{
			int difficulty = screen.GetDifficulty();
			difficultyIndicator.SetStars(difficulty);
			difficultyContainer.gameObject.SetActive(screen is MobileMenuPortalScreen && difficulty != 0);
		}
		float num = ((screen is MobileMenuPortalScreen) ? 1f : 0f);
		buttonSpeedTrial.gameObject.SetActive(speedTrialsAvailable && screen is MobileMenuPortalScreen);
		buttonSpeedTrial.image.DOFade(num, tweenDuration).SetEase(Ease.OutSine);
		textSpeedTrials.DOFade(buttonUp.gameObject.activeSelf ? 0f : num, tweenDuration).SetEase(Ease.OutSine);
		float endValue2 = ((screen is MobileMenuTitleScreen) ? 1f : 0f);
		buttonAprilFools.gameObject.SetActive(aprilFoolsAvailable && screen is MobileMenuTitleScreen);
		buttonAprilFools.image.DOFade(endValue2, tweenDuration).SetEase(Ease.OutSine);
		textAprilFools.DOFade(endValue2, tweenDuration).SetEase(Ease.OutSine);
		buttonRestorePurchase.interactable = screen is MobileMenuDescriptionScreen;
		buttonRestorePurchase.gameObject.SetActive(screen is MobileMenuDescriptionScreen);
		float endValue3 = ((screen is MobileMenuDescriptionScreen) ? 1f : 0f);
		buttonRestorePurchase.image.DOFade(endValue3, tweenDuration).SetEase(Ease.OutSine);
		textRestorePurchase.DOFade(endValue3, tweenDuration).SetEase(Ease.OutSine);
	}

	public void Enable(bool enable, bool instant = false)
	{
		responsive = enable;
		ShowButtons(enable, instant);
		if (!enable)
		{
			ShowBackButton(show: false, instant);
		}
	}

	private Tween DoTheme(MobileMenuGroup.BackgroundTheme theme, MobileMenuGroup.BackgroundTheme speedTheme = default(MobileMenuGroup.BackgroundTheme), bool instant = false)
	{
		Sequence sequence = DOTween.Sequence();
		float duration = (instant ? 0f : 0.25f);
		if (speedTrial)
		{
			MobileMenuGroup.BackgroundTheme defaultTheme = new MobileMenuGroup.BackgroundTheme
			{
				backgroundColor = "67000045".HexToColor(),
				gradientColor = "822D5480".HexToColor(),
				cloudColor = "3F283817".HexToColor()
			};
			theme = speedTheme.WithDefaults(defaultTheme);
		}
		SpriteRenderer[] array = backgroundClouds;
		foreach (SpriteRenderer target in array)
		{
			sequence.Join(target.DOColor(theme.cloudColor.Value, duration).SetEase(Ease.OutSine));
		}
		sequence.Join(background.DOColor(theme.backgroundColor.Value, duration).SetEase(Ease.OutSine));
		sequence.Join(backgroundGradient.DOColor(theme.gradientColor.Value, duration).SetEase(Ease.OutSine));
		int valueOrDefault = theme.backgroundId.GetValueOrDefault();
		for (int j = 1; j < backgrounds.Length; j++)
		{
			float alpha = ((j == valueOrDefault) ? 1f : 0f);
			SpriteRenderer target2 = backgrounds[j];
			sequence.Join(target2.DOColor(theme.backgroundColor.Value.WithAlpha(alpha), duration).SetEase(Ease.OutSine));
		}
		return sequence;
	}

	public void MoveInDirection(MoveDirection direction, bool fromKeyboard = false, bool playSound = true)
	{
		if (grabController.grabbedObject is MobileMenuGrabbablePlanet || ADOBase.controller.paused || expandedLevel)
		{
			return;
		}
		Vector2Int vector = direction.GetVector();
		MobileMenuGroup mobileMenuGroup = currentGroup;
		int num = selectedScreenIndex;
		bool flag = false;
		if (vector.x != 0)
		{
			num += vector.x;
			if (num < 0 || num > currentGroup.visibleScreens.Count - 1)
			{
				if (mobileMenuGroup.linkedGroup.ContainsKey(direction))
				{
					flag = true;
					mobileMenuGroup = mobileMenuGroup.linkedGroup[direction];
					num = ((direction != MoveDirection.Right) ? (mobileMenuGroup.visibleScreens.Count - 1) : 0);
				}
			}
			else
			{
				flag = true;
			}
		}
		else if (selectedScreenIndex == 0 && currentGroup.linkedGroup.ContainsKey(direction))
		{
			mobileMenuGroup = mobileMenuGroup.linkedGroup[direction];
			flag = true;
		}
		textSpeedTrials.text = textSpeedTrials.text.Replace(" (S)", "") + (fromKeyboard ? " (S)" : "");
		textAprilFools.text = textAprilFools.text.Replace(" (A)", "") + (fromKeyboard ? " (A)" : "");
		textRestorePurchase.text = textRestorePurchase.text.Replace(" (Ctrl-R)", "") + (fromKeyboard ? " (Ctrl-R)" : "");
		if (flag && !mobileMenuGroup.inaccessible)
		{
			JumpToScreen(mobileMenuGroup.visibleScreens[num]);
		}
		else
		{
			JumpToScreen();
		}
		if (playSound)
		{
			bool flag2 = direction == MoveDirection.Right || direction == MoveDirection.Up;
			scrSfx.instance.PlaySfx(flag2 ? SfxSound.MobileButtonRight : SfxSound.MobileButtonLeft, MixerGroup.InterfaceParent);
		}
	}

	public void RefreshButtons()
	{
		if (currentGroup != null)
		{
			bool flag = selectedScreenIndex == 0;
			Dictionary<MoveDirection, MobileMenuGroup> linkedGroup = currentGroup.linkedGroup;
			MobileMenuGroup value;
			bool showButton = linkedGroup.TryGetValue(MoveDirection.Up, out value) && flag;
			MobileMenuGroup value2;
			bool showButton2 = linkedGroup.TryGetValue(MoveDirection.Down, out value2) && flag;
			MobileMenuGroup value3;
			bool num = linkedGroup.TryGetValue(MoveDirection.Left, out value3);
			bool flag2 = num && flag;
			bool flag3 = num || selectedScreenIndex > 0;
			MobileMenuGroup value4;
			bool num2 = linkedGroup.TryGetValue(MoveDirection.Right, out value4);
			bool flag4 = num2 && flag;
			bool showButton3 = num2 || selectedScreenIndex < currentGroup.visibleScreens.Count - 1;
			bool flag5 = !flag3;
			RectTransform obj = buttonDown.transform as RectTransform;
			obj.anchoredPosition = (flag5 ? Vector2.zero.WithX(135f) : Vector2.zero);
			Vector2 vector = (flag5 ? Vector2.zero : new Vector2(0.5f, 0f));
			Vector2 anchorMin = (obj.anchorMax = vector);
			obj.anchorMin = anchorMin;
			buttonUp.Show(showButton, value, currentGroup);
			buttonDown.Show(showButton2, value2, currentGroup);
			buttonLeft.Show(flag3, flag2 ? value3 : null, currentGroup);
			buttonRight.Show(showButton3, flag4 ? value4 : null, currentGroup);
			buttonUp.glow.gameObject.SetActive(value: false);
			buttonDown.glow.gameObject.SetActive(value: false);
			buttonLeft.glow.gameObject.SetActive(value: false);
			buttonRight.glow.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (map == null || currentGroup == null)
		{
			return;
		}
		bool flag = wasPaused && !ADOBase.controller.paused;
		wasPaused = ADOBase.controller.paused;
		if (ADOBase.controller.paused)
		{
			return;
		}
		DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
		if (!responsive)
		{
			return;
		}
		Vector2 vector = Vector2.zero;
		bool flag2 = false;
		TouchPhase touchPhase = TouchPhase.Stationary;
		bool flag3 = false;
		if (Input.touchCount >= 1)
		{
			Touch touch = Input.GetTouch(0);
			vector = touch.position;
			touchPhase = touch.phase;
			flag3 = true;
		}
		else if (Input.mousePresent)
		{
			vector = Input.mousePosition;
			touchPhase = (Input.GetMouseButtonUp(0) ? TouchPhase.Ended : ((!Input.GetMouseButtonDown(0)) ? (Input.GetMouseButton(0) ? TouchPhase.Moved : TouchPhase.Stationary) : TouchPhase.Began));
			flag3 = touchPhase == TouchPhase.Moved;
		}
		if (flag && !flag3 && dragging)
		{
			touchPhase = TouchPhase.Canceled;
		}
		bool flag4 = IsPointOverUIObject(vector);
		flag2 = movingThisFrame || flag4;
		bool achievementIsActive = scrUIController.instance.achievementIsActive;
		switch (touchPhase)
		{
		case TouchPhase.Began:
			OnTouchStart(vector, flag2);
			break;
		case TouchPhase.Ended:
		case TouchPhase.Canceled:
			if (!flag2 && !grabController.grabbedObject)
			{
				OnTouchedScreen();
			}
			OnTouchEnd(vector);
			break;
		default:
			if (flag3)
			{
				OnTouchMove(vector);
			}
			else if (!achievementIsActive && expandedLevel)
			{
				if (RDInput.leftPress)
				{
					MoveInSublevel(MoveDirection.Left);
				}
				else if (RDInput.rightPress)
				{
					MoveInSublevel(MoveDirection.Right);
				}
				else if (RDInput.backPress)
				{
					OnTouchedScreen(fromKeyboard: true);
				}
				else if (RDInput.confirmPress)
				{
					EnterSubmenuLevel();
				}
			}
			else if (!achievementIsActive && !expandedLevel)
			{
				if (RDInput.leftPress)
				{
					MoveInDirection(MoveDirection.Left, !ADOBase.isSwitch);
				}
				else if (RDInput.rightPress)
				{
					MoveInDirection(MoveDirection.Right, !ADOBase.isSwitch);
				}
				else if (RDInput.upPress)
				{
					MoveInDirection(MoveDirection.Up, !ADOBase.isSwitch);
				}
				else if (RDInput.downPress)
				{
					MoveInDirection(MoveDirection.Down, !ADOBase.isSwitch);
				}
				else if (RDInput.holdingControl && currentScreen is MobileMenuDescriptionScreen descriptionScreen)
				{
					RestorePurchasesFromKeyboard(descriptionScreen);
				}
				else if (RDInput.faceUp && currentScreen is MobileMenuPortalScreen)
				{
					ToggleSpeedTrial();
				}
				else if (RDInput.faceLeft && currentScreen is MobileMenuTitleScreen)
				{
					ToggleAprilFools();
				}
				else if ((!RDInput.faceUp || !(currentScreen is MobileMenuMoreScreen mobileMenuMoreScreen) || !mobileMenuMoreScreen.morePage.TryGoToStorePage()) && RDInput.confirmPress)
				{
					OnTouchedScreen(fromKeyboard: true);
				}
			}
			break;
		}
		float aspect = Camera.main.aspect;
		if (Mathf.Abs(aspect - lastAspectRatio) > 0.01f)
		{
			if (lastAspectRatio != 0f)
			{
				map.Build();
			}
			lastAspectRatio = aspect;
			JumpToScreen(null, instant: true);
		}
		movingThisFrame = false;
	}

	private void OnTouchedScreen(bool fromKeyboard = false)
	{
		if (dragging)
		{
			return;
		}
		if (currentScreen is MobileMenuPortalScreen mobileMenuPortalScreen)
		{
			ExpandPortal(!expandedLevel, mobileMenuPortalScreen.portal);
			if (expandedLevel)
			{
				for (int i = 0; i < currentSubmenu.childCount; i++)
				{
					Transform child = currentSubmenu.GetChild(i);
					child.DOScale(((i == submenuLevel) ? 1.25f : 1f) * submenuScale, 0f);
					child.GetChild(0).GetComponent<Image>().DOFade((i == submenuLevel && fromKeyboard) ? 0.6f : 0f, 0f);
				}
			}
		}
		else
		{
			currentScreen.Interact(fromKeyboard);
		}
	}

	private void RestorePurchasesFromKeyboard(MobileMenuDescriptionScreen descriptionScreen)
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			descriptionScreen.restoreAction?.Invoke();
		}
	}

	private void MoveInSublevel(MoveDirection moveDirection)
	{
		int num = moveDirection switch
		{
			MoveDirection.Left => -1, 
			MoveDirection.Right => 1, 
			_ => 0, 
		};
		if (!(currentSubmenu == null) && currentSubmenu.childCount != 1 && num != 0 && submenuLevel + num != currentSubmenu.childCount && submenuLevel + num != -1)
		{
			currentSubmenu.GetChild(submenuLevel).DOScale(1f * submenuScale, 0.15f).SetEase(Ease.OutBack);
			currentSubmenu.GetChild(submenuLevel).GetChild(0).GetComponent<Image>()
				.DOFade(0f, 0.15f);
			submenuLevel = Math.Clamp(submenuLevel + num, 0, currentSubmenu.childCount - 1);
			currentSubmenu.GetChild(submenuLevel).DOScale(1.25f * submenuScale, 0.15f).SetEase(Ease.OutBack);
			currentSubmenu.GetChild(submenuLevel).GetChild(0).GetComponent<Image>()
				.DOFade(0.6f, 0.15f);
			scrSfx.instance.PlaySfx(SfxSound.MenuNavigate, MixerGroup.InterfaceParent);
		}
	}

	private void EnterSubmenuLevel()
	{
		currentSubmenu.GetChild(submenuLevel).GetComponent<Button>().onClick.Invoke();
		scrSfx.instance.PlaySfx(SfxSound.MobileButtonEnter, MixerGroup.InterfaceParent);
	}

	private bool IsPointOverUIObject(Vector2 screenPosition)
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = screenPosition;
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		string[] array = new string[1] { "trailerButton" };
		foreach (RaycastResult item in list)
		{
			if (Array.IndexOf(array, item.gameObject.name) != -1)
			{
				return false;
			}
		}
		return list.Count > 0;
	}

	public void ShowDifficulty(bool show, bool instant = false)
	{
		float duration = (instant ? 0f : 0.25f);
		difficultyContainer.DOPivotY((!show) ? 1 : 0, duration).SetEase(Ease.OutSine);
	}

	private void ExpandPortal(bool expand, scrPortal portal)
	{
		if (portal.locked)
		{
			scrSfx.instance.PlaySfx(SfxSound.PortalLocked, MixerGroup.InterfaceParent);
			portal.ShakePortal();
			return;
		}
		if ((ADOBase.worldData[portal.world].levelCount == 1 && !Persistence.IsWorldComplete(portal.world)) || speedTrial)
		{
			EnterLevel(portal.world + "-X", speedTrial);
			return;
		}
		JumpToScreen();
		expandedLevel = expand;
		portal.ExpandPortalMobile(expand);
		if (ADOBase.isExpo)
		{
			ShowDifficulty(!expand);
		}
		foreach (MobileMenuPortalScreen value2 in map.portalLUT.Values)
		{
			scrPortal portal2 = value2.portal;
			if (portal2 != null && portal2 != portal)
			{
				if (expand)
				{
					portal2.FadePortal(0f);
				}
				else
				{
					value2.onSelect(arg1: false, arg2: false);
				}
			}
		}
		if (expand)
		{
			currentSubmenu = null;
			submenuLevel = -1;
			submenuScale = 1f;
			foreach (KeyValuePair<string, GameObject> item in sublevelBrowser.submenu)
			{
				string key = item.Key;
				GameObject value = item.Value;
				bool flag = key == portal.world;
				value.SetActive(flag);
				if (flag)
				{
					currentSubmenu = value.transform;
					submenuLevel = ((!ADOBase.isExpo) ? (currentSubmenu.childCount - 1) : 0);
					submenuScale = ((currentSubmenu.childCount >= 11) ? 0.8f : ((currentSubmenu.childCount >= 10) ? 0.88f : 1f));
				}
			}
		}
		sublevelBrowser.container.DOKill();
		TweenerCore<Vector2, Vector2, VectorOptions> t = sublevelBrowser.container.DOPivotY((!expand) ? 1 : 0, 0.25f).SetEase((!expand) ? Ease.Linear : Ease.OutBack);
		if (expand)
		{
			sublevelBrowser.container.gameObject.SetActive(value: true);
		}
		else
		{
			t.OnComplete(delegate
			{
				sublevelBrowser.container.gameObject.SetActive(value: false);
			});
		}
		portal.statsTextContainer.DOPivotY(expand ? 1 : 0, 0.25f).SetEase(Ease.OutSine);
		ShowButtons(!expand);
		ShowBackButton(expand);
		scrSfx.instance.PlaySfx(expand ? SfxSound.PortalSelect : SfxSound.PortalDeselect, MixerGroup.InterfaceParent);
		scrSfx.instance.PlaySfx(expand ? SfxSound.MobileButtonEnter : SfxSound.MenuBack, MixerGroup.InterfaceParent);
	}

	public void ShowButtons(bool show, bool instant = false)
	{
		float duration = (instant ? 0f : 0.25f);
		topButtonsContainer.DOPivotY(show ? 1 : 0, duration).SetEase(Ease.OutSine);
		bottomButtonsContainer.DOPivotY((!show) ? 1 : 0, duration).SetEase(Ease.OutSine);
		caption.rectTransform.DOPivotY((!show) ? 1 : 0, duration).SetEase(Ease.OutSine);
		buttonsCanvasGroup.interactable = show;
		buttonDown.museDashIcon.DOFade(show ? 1 : 0, duration);
		buttonDown.cr2024Icon.DOFade(show ? 1 : 0, duration);
	}

	public void ShowBackButton(bool show, bool instant = false)
	{
		float duration = (instant ? 0f : 0.25f);
		backButtonContainer.DOPivotY(show ? 1 : 0, duration).SetEase(Ease.OutSine);
	}

	public void BackButtonAction()
	{
		if (expandedLevel)
		{
			OnTouchedScreen(fromKeyboard: true);
		}
	}

	public static void EnterLevel(string worldAndLevel, bool speedTrial)
	{
		ADOBase.controller.EnterLevel(worldAndLevel, speedTrial);
	}

	public static void PlayPuzzleSfx(SfxSound sound)
	{
		ADOBase.conductor.DuckSongStart();
		DOVirtual.DelayedCall(scrSfx.instance.PlaySfx(sound, MixerGroup.SfxParent).length + 0.05f, delegate
		{
			ADOBase.conductor.DuckSongStop();
		});
	}

	private void ToggleSpeedTrial()
	{
		if (Persistence.GetSpeedTrialsAvailable() && currentScreen is MobileMenuPortalScreen)
		{
			bool flag = !speedTrial;
			scrSfx.instance.PlaySfx(flag ? SfxSound.SpeedTrialOn : SfxSound.SpeedTrialOff, MixerGroup.InterfaceParent);
			scrFlash.Flash();
			SetSpeedTrial(flag);
		}
	}

	public void ToggleAprilFools()
	{
		if (aprilFoolsAvailable && currentScreen is MobileMenuTitleScreen)
		{
			GCS.FOOL_JOKER = !GCS.FOOL_JOKER;
			GCS.sceneToLoad = GCNS.sceneLevelSelect;
			scrSfx.instance.PlaySfx(GCS.FOOL_JOKER ? SfxSound.SpeedTrialOn : SfxSound.SpeedTrialOff, MixerGroup.InterfaceParent);
			scrFlash.Flash();
			ADOBase.controller.StartLoadingScene();
		}
	}

	private void SetSpeedTrial(bool on)
	{
		speedTrial = on;
		foreach (MobileMenuPortalScreen value in map.portalLUT.Values)
		{
			if (value.visible)
			{
				value.CheckLocked(speedTrial);
			}
		}
		buttonSpeedTrial.image.sprite = (on ? RDC.data.sprSpeedTrialButtonOn : RDC.data.sprSpeedTrialButtonOff);
		JumpToScreen();
	}

	private void OnTouchStart(Vector2 pos, bool touchingUI)
	{
		Vector3 vector = ADOBase.controller.camy.camobj.ScreenToWorldPoint(pos);
		dragCanBegin = !expandedLevel && !touchingUI && !grabController.TryGrabObjectAt(vector);
		if (dragCanBegin)
		{
			touchStartPos = pos;
			cameraOrigPos = ADOBase.controller.camy.transform.position;
			screenTransition.Kill();
		}
	}

	private void OnTouchMove(Vector2 pos)
	{
		if (expandedLevel)
		{
			return;
		}
		Vector2 vector = new Vector2(pos.x - touchStartPos.x, pos.y - touchStartPos.y);
		Vector2 vector2 = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
		bool flag = selectedScreenIndex == 0 && (currentGroup.linkedGroup.ContainsKey(MoveDirection.Up) || currentGroup.linkedGroup.ContainsKey(MoveDirection.Down));
		if ((vector2.x > 5f || vector2.y > 5f) && !dragging && dragCanBegin)
		{
			dragging = true;
			dragIsVertical = vector2.y > vector2.x;
			if (!flag)
			{
				dragIsVertical = false;
			}
		}
		if (dragging)
		{
			Vector2 vector3 = new Vector2((!dragIsVertical) ? vector.x : 0f, dragIsVertical ? vector.y : 0f) / Screen.height * ADOBase.controller.camy.camobj.orthographicSize * 2f;
			Vector3 position = ADOBase.controller.camy.transform.position;
			Vector2 vector4 = cameraOrigPos - vector3;
			ADOBase.controller.camy.transform.position = Vector3.Lerp(position, vector4, Time.deltaTime * targetTouchRefreshRate).WithZ(position.z);
		}
		else
		{
			Vector3 vector5 = ADOBase.controller.camy.camobj.ScreenToWorldPoint(pos);
			grabController.UpdateGrabbedObject(vector5);
		}
	}

	private void OnTouchEnd(Vector2 pos)
	{
		Vector2 vector = new Vector2(pos.x - touchStartPos.x, pos.y - touchStartPos.y) / Screen.height * ADOBase.controller.camy.camobj.orthographicSize * 2f;
		Vector2 vector2 = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
		if (dragging)
		{
			if (dragIsVertical)
			{
				float num = 0.5f;
				if (vector2.y > num)
				{
					MoveInDirection((vector.y > 0f) ? MoveDirection.Down : MoveDirection.Up, fromKeyboard: false, playSound: false);
				}
				else
				{
					JumpToScreen();
				}
			}
			else
			{
				float num2 = 0.5f;
				if (vector2.x > num2)
				{
					MoveInDirection((vector.x > 0f) ? MoveDirection.Left : MoveDirection.Right, fromKeyboard: false, playSound: false);
				}
				else
				{
					JumpToScreen();
				}
			}
		}
		else if (grabController.grabbedObject != null)
		{
			grabController.UngrabObject();
		}
		else
		{
			JumpToScreen();
		}
		dragging = false;
	}

	public void OpenFeaturedLevels(scnCLS.Category category)
	{
		ADOBase.controller.PortalTravelAction(Portal.CustomLevelsScene);
		scnCLS.entryCategory = category;
	}
}
