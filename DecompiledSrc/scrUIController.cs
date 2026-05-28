using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrUIController : ADOBase
{
	private const float TransitionDuration = 0.3f;

	private const bool CompleteKilledTweens = false;

	private const bool PrintCalls = false;

	public static int deathCounterToFixDotweenBug = 0;

	public static int deathCeiling = 50;

	public static float speedUpAtDeathMax = 8f;

	private static scrUIController _instance;

	public Canvas canvas;

	public CanvasScaler canvasScaler;

	public Text txtLevelName;

	public Text txtOffset;

	public Text txtDebug;

	public Text txtCountdown;

	public Text txtCongrats;

	public Text txtAprilCongrats;

	public Text txtAllStrictClear;

	public Text txtPercent;

	public Text txtPressToStart;

	public Text txtTryCalibrating;

	public Image transitionPanel;

	public GameObject achievementPanel;

	public GameObject achievementBlackPanel;

	public Button achievementSkip;

	public Button autoplayButton;

	public Image mutedImage;

	public Button pauseButton;

	public Sprite[] autoSprites;

	public EndscreenLanterns[] endscreenLanternsSets;

	public DetailedResults txtResults;

	[Header("Difficulty")]
	public RectTransform difficultyContainer;

	public CanvasGroup difficultyFadeContainer;

	public float difficultyAnimDuration;

	public RectTransform leftArrow;

	public RectTransform rightArrow;

	public Button difficultyButtonLeft;

	public Button difficultyButtonRight;

	public Image difficultyImage;

	public Text difficultyText;

	[NonSerialized]
	public DifficultyUIMode difficultyUIMode;

	[Header("Modifiers")]
	public RectTransform modifiersContainer;

	public Image noFailImage;

	public Image unlockKeyLimiterImage;

	private Vector2 leftArrowDefaultPos;

	private Vector2 rightArrowDefaultPos;

	private Tweener wipeToBlack;

	public static scrUIController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindAnyObjectByType<scrUIController>();
			}
			return _instance;
		}
	}

	public bool achievementIsActive
	{
		get
		{
			if (achievementPanel != null)
			{
				return achievementPanel.activeInHierarchy;
			}
			return false;
		}
	}

	public bool isWipingToBlack
	{
		get
		{
			if (wipeToBlack != null && wipeToBlack.active)
			{
				return wipeToBlack.IsPlaying();
			}
			return false;
		}
	}

	public void Awake()
	{
		leftArrowDefaultPos = leftArrow.anchoredPosition;
		rightArrowDefaultPos = rightArrow.anchoredPosition;
		GCS.difficulty = Persistence.GetDefaultDifficulty();
		autoplayButton.gameObject.SetActive(value: false);
		difficultyButtonLeft.onClick.AddListener(delegate
		{
			DifficultyArrowPressed(rightPressed: false);
		});
		difficultyButtonRight.onClick.AddListener(delegate
		{
			DifficultyArrowPressed(rightPressed: true);
		});
	}

	public void Start()
	{
		modifiersContainer.gameObject.SetActive(ADOBase.isScnGame || ADOBase.isCLS);
	}

	public void LevelFinishedLoading()
	{
		EndscreenLanterns[] array = endscreenLanternsSets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Setup();
		}
	}

	public void PrepareWipeFromBlack()
	{
		DebugPrint("scrUIController.PrepareWipeFromBlack");
		EnableTransitionPanel(enable: true);
		transitionPanel.color = Color.black;
		RectTransform rectTransform = transitionPanel.rectTransform;
		rectTransform.pivot = new Vector2(0f, 0.5f);
		rectTransform.localScale = Vector3.one;
	}

	public void WipeFromBlack(bool withSound = true)
	{
		if (!isWipingToBlack)
		{
			DebugPrint("scrUIController.WipeFromBlack");
			if (withSound)
			{
				scrSfx.instance.PlaySfx(SfxSound.ScreenWipeIn, MixerGroup.InterfaceParent, 0.5f);
			}
			EnableTransitionPanel(enable: true);
			transitionPanel.color = Color.black;
			RectTransform rectTransform = transitionPanel.rectTransform;
			float x = ((GCS.directionToWipe == WipeDirection.StartsFromLeft) ? 1f : 0f);
			rectTransform.pivot = new Vector2(x, 0.5f);
			rectTransform.localScale = Vector3.one;
			rectTransform.DOKill();
			float duration = (GCS.speedTrialMode ? (0.3f / GCS.currentSpeedTrial) : 0.3f);
			rectTransform.DOScaleX(0f, duration).SetEase(Ease.InOutQuint).SetUpdate(isIndependentUpdate: true)
				.OnComplete(delegate
				{
					EnableTransitionPanel(enable: false);
				});
		}
	}

	public void WipeToBlack(WipeDirection direction, Action onComplete, Action onCancel = null)
	{
		scrSfx.instance.PlaySfx(SfxSound.ScreenWipeOut, MixerGroup.InterfaceParent, 0.5f);
		if (isWipingToBlack)
		{
			onCancel?.Invoke();
			MonoBehaviour.print("wipe2black canceled");
			return;
		}
		GCS.directionToWipe = direction;
		DebugPrint("scrUIController.WipeToBlack");
		EnableTransitionPanel(enable: true);
		transitionPanel.color = Color.black;
		RectTransform rectTransform = transitionPanel.rectTransform;
		wipeToBlack.Kill();
		float x = ((direction == WipeDirection.StartsFromLeft) ? 0f : 1f);
		rectTransform.pivot = new Vector2(x, 0.5f);
		rectTransform.localScale = new Vector3(0f, 1f, 1f);
		rectTransform.DOKill();
		float duration = (GCS.speedTrialMode ? (0.3f / GCS.currentSpeedTrial) : 0.3f);
		wipeToBlack = rectTransform.DOScaleX(1f, duration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutQuint)
			.OnComplete(delegate
			{
				if (false)
				{
					ShowAchievements(delegate
					{
						WipeToBlackOnComplete(onComplete);
					});
				}
				else
				{
					WipeToBlackOnComplete(onComplete);
				}
			})
			.OnKill(delegate
			{
				if (onCancel != null)
				{
					onCancel();
				}
			});
	}

	private void WipeToBlackOnComplete(Action onComplete)
	{
		achievementBlackPanel?.SetActive(value: false);
		achievementPanel?.SetActive(value: false);
		DebugPrint("Finished wiping to black");
		onComplete();
	}

	public void FadeFromBlack(float duration = 1f)
	{
		DebugPrint("scrUIController.FadeFromBlack");
		ResetScale();
		EnableTransitionPanel(enable: true);
		transitionPanel.DOKill();
		transitionPanel.DOFade(0f, duration).SetUpdate(isIndependentUpdate: true).OnComplete(delegate
		{
			EnableTransitionPanel(enable: false);
		});
	}

	public void FadeToBlack(float duration = 1f)
	{
		ResetScale();
		DebugPrint("scrUIController.FadeToBlack");
		EnableTransitionPanel(enable: true);
		transitionPanel.DOKill();
		transitionPanel.DOFade(1f, duration).SetUpdate(isIndependentUpdate: true).OnComplete(delegate
		{
			EnableTransitionPanel(enable: false);
		});
	}

	private void ResetScale()
	{
		transitionPanel.rectTransform.localScale = Vector3.one;
	}

	public void SetToBlack()
	{
		ResetScale();
		EnableTransitionPanel(enable: true);
		transitionPanel.DOKill();
		transitionPanel.color = Color.black;
	}

	public void SetToTransparent()
	{
		ResetScale();
		EnableTransitionPanel(enable: false);
		transitionPanel.DOKill();
		transitionPanel.color = Color.black;
	}

	private void EnableTransitionPanel(bool enable)
	{
		DebugPrint(enable);
		transitionPanel.gameObject.SetActive(enable);
	}

	private void Update()
	{
		if (GCS.useNoFail != noFailImage.gameObject.activeSelf)
		{
			noFailImage.gameObject.SetActive(GCS.useNoFail);
		}
		if (GCS.useUnlockKeyLimiter != unlockKeyLimiterImage.gameObject.activeSelf)
		{
			unlockKeyLimiterImage.gameObject.SetActive(GCS.useUnlockKeyLimiter);
		}
	}

	public void ToggleAutoplay()
	{
		RDC.auto = !RDC.auto;
	}

	public void ShowDifficultyContainer(DifficultyUIMode mode)
	{
		difficultyUIMode = mode;
		UpdateDifficultyUI(GCS.difficulty);
		if (mode == DifficultyUIMode.DontShow)
		{
			difficultyContainer.gameObject.SetActive(value: false);
			return;
		}
		difficultyContainer.gameObject.SetActive(value: true);
		difficultyContainer.DOKill();
		difficultyContainer.DOAnchorPosX(-20f, difficultyAnimDuration).SetEase(Ease.OutBack);
		difficultyFadeContainer.DOKill();
		difficultyFadeContainer.alpha = 1f;
		difficultyFadeContainer.gameObject.SetActive(value: true);
		difficultyFadeContainer.blocksRaycasts = true;
		difficultyButtonLeft.enabled = true;
		difficultyButtonRight.enabled = true;
		HorizontalLayoutGroup layout = modifiersContainer.GetComponent<HorizontalLayoutGroup>();
		layout.DOKill();
		DOTween.To(() => layout.spacing, delegate(float s)
		{
			layout.spacing = s;
		}, 0f, 0.5f).SetEase(Ease.OutQuad).SetId(layout);
	}

	public void MinimizeDifficultyContainer()
	{
		if (difficultyUIMode != DifficultyUIMode.DontShow)
		{
			difficultyContainer.DOKill();
			difficultyContainer.DOAnchorPosX(335f, difficultyAnimDuration).SetEase(Ease.OutExpo);
			difficultyFadeContainer.DOKill();
			difficultyFadeContainer.DOFade(0f, difficultyAnimDuration).SetEase(Ease.OutExpo).OnComplete(delegate
			{
				difficultyFadeContainer.gameObject.SetActive(value: false);
			});
			difficultyFadeContainer.blocksRaycasts = false;
			difficultyButtonLeft.enabled = false;
			difficultyButtonRight.enabled = false;
			HorizontalLayoutGroup layout = modifiersContainer.GetComponent<HorizontalLayoutGroup>();
			layout.DOKill();
			DOTween.To(() => layout.spacing, delegate(float s)
			{
				layout.spacing = s;
			}, -40f, 0.5f).SetEase(Ease.OutQuad).SetId(layout);
		}
	}

	public void DifficultyArrowPressed(bool rightPressed)
	{
		DebugPrint("difficultyArrowPressed: " + rightPressed);
		bool flag = difficultyUIMode == DifficultyUIMode.ShowNormalAndStrict;
		int num = ((difficultyUIMode == DifficultyUIMode.ShowLenientAndNormal) ? 2 : 3);
		RectTransform obj = (rightPressed ? rightArrow : leftArrow);
		Vector2 anchoredPosition = (rightPressed ? rightArrowDefaultPos : leftArrowDefaultPos);
		Vector2 punch = (rightPressed ? new Vector2(5f, 0f) : new Vector2(-5f, 0f));
		obj.DOKill();
		obj.anchoredPosition = anchoredPosition;
		obj.DOPunchAnchorPos(punch, 0.2f, 0);
		int difficulty = (int)GCS.difficulty;
		difficulty += (rightPressed ? 1 : (-1));
		if (difficulty >= num)
		{
			difficulty = (flag ? 1 : 0);
		}
		else if (difficulty < 0)
		{
			difficulty = num - 1;
		}
		if (flag && difficulty == 0)
		{
			difficulty = 2;
		}
		scrSfx.instance.PlaySfx(SfxSound.MenuSquelch, MixerGroup.InterfaceParent, 1.5f);
		UpdateDifficultyUI((Difficulty)difficulty);
		Persistence.SetDefaultDifficulty((Difficulty)difficulty);
	}

	private void UpdateDifficultyUI(Difficulty difficulty)
	{
		if (difficulty == Difficulty.Strict && difficultyUIMode == DifficultyUIMode.ShowLenientAndNormal)
		{
			difficulty = Difficulty.Normal;
		}
		else if (difficulty == Difficulty.Lenient && difficultyUIMode == DifficultyUIMode.ShowNormalAndStrict)
		{
			difficulty = Difficulty.Strict;
		}
		else if (difficultyUIMode == DifficultyUIMode.DontShow)
		{
			difficulty = Difficulty.Normal;
		}
		GCS.difficulty = difficulty;
		difficultyText.text = RDString.Get("enum.Difficulty." + difficulty);
		difficultyImage.sprite = RDConstants.data.bullseyeSprites[(int)difficulty];
	}

	public void ShowAchievements(Action onComplete)
	{
		Debug.Log("Show");
		achievementBlackPanel.SetActive(value: false);
		achievementPanel.SetActive(value: true);
		UnityEngine.Object.Instantiate(RDConstants.data.prefab_achievement).GetComponent<AchievementDisplay>().ShowAchievements(GameServices.Instance.achievementsQueue.ToArray(), onComplete);
		GameServices.Instance.achievementsQueue.Clear();
	}

	public void ShowRetroactiveAchievements()
	{
		achievementBlackPanel.SetActive(value: true);
		achievementPanel.SetActive(value: true);
		UnityEngine.Object.Instantiate(RDConstants.data.prefab_achievement).GetComponent<AchievementDisplay>().ShowRetroactiveAchievements(GameServices.Instance.achievementsQueue.ToArray(), delegate
		{
			achievementBlackPanel.SetActive(value: false);
			achievementPanel.SetActive(value: false);
			GCS.directionToWipe = WipeDirection.StartsFromRight;
			WipeFromBlack();
		});
		GameServices.Instance.achievementsQueue.Clear();
	}

	private void DebugPrint(object o)
	{
	}

	public void ShowEndscreenLanterns()
	{
		if (scrController.coopMode)
		{
			ADOBase.controller.camy.flashEndscreen.material.color = Color.black.WithAlpha(0f);
			ADOBase.controller.camy.flashEndscreen.material.DOFade(0.333f, 0.5f);
		}
		float num = 0f;
		foreach (scrPlayer item in ADOBase.controller.playerManager)
		{
			num = Mathf.Max(num, item.marginTracker.percentXAcc);
		}
		int num2 = 0;
		for (int i = 0; i < ADOBase.controller.playerManager.players.Length && i < endscreenLanternsSets.Length; i++)
		{
			scrPlayer scrPlayer2 = ADOBase.controller.playerManager.players[i];
			EndscreenLanterns endscreenLanterns = endscreenLanternsSets[i];
			int num3 = i - num2;
			bool crown = false;
			if (scrController.coopMode)
			{
				foreach (scrPlanet allPlanet in scrPlayer2.planetarySystem.allPlanets)
				{
					allPlanet.planetRenderer.SetLayer("ForegroundUI");
				}
				crown = scrPlayer2.marginTracker.percentXAcc >= num;
			}
			float num4 = 1.25f;
			int soundSet = (scrController.coopMode ? i : (-1));
			if (scrPlayerManager.playerCount == 2 && i == 1)
			{
				soundSet = 2;
			}
			if (endscreenLanterns.Show(scrPlayer2, (float)num3 * num4, soundSet, crown) == 0)
			{
				num2++;
			}
		}
	}

	public void HideEndscreenLanterns()
	{
		EndscreenLanterns[] array = endscreenLanternsSets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Hide();
		}
	}
}
