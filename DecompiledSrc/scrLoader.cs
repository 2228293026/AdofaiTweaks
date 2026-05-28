using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scrLoader : ADOBase
{
	public GameObject planets;

	public Transform ring;

	public GameObject loadingText;

	public Image transitionPanel;

	private float timeSpentLoading;

	private const float TransitionDuration = 0.3f;

	private const bool CompleteKilledTweens = false;

	private WipeDirection wipeDirection;

	private Tweener wipeToBlack;

	private float startFrame;

	private bool wipeCued;

	private bool disableLoadingVisuals;

	public bool startingGame = true;

	public static scrLoader instance { get; private set; }

	private bool isWipingToBlack
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

	private void Awake()
	{
		planets.gameObject.SetActive(value: false);
		loadingText.gameObject.SetActive(value: false);
		transitionPanel.gameObject.SetActive(value: false);
		timeSpentLoading = -1f;
		instance = this;
	}

	private void Update()
	{
		if (timeSpentLoading != -1f)
		{
			timeSpentLoading += Time.unscaledDeltaTime;
		}
		if (timeSpentLoading > 1f && !disableLoadingVisuals)
		{
			planets.gameObject.SetActive(value: true);
			loadingText.gameObject.SetActive(value: true);
		}
		ring.Rotate(Vector3.back, -30f * Time.unscaledDeltaTime);
		int num = 4;
		if (wipeCued && (float)Time.frameCount - startFrame >= (float)num && ((ADOBase.isScnGame && !scnGame.instance.isLoading) || !ADOBase.isScnGame))
		{
			wipeCued = false;
			disableLoadingVisuals = true;
			planets.gameObject.SetActive(value: false);
			loadingText.gameObject.SetActive(value: false);
			WipeFromBlack();
		}
	}

	public void LoadScene(string scene)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (ADOBase.isExpo)
		{
			scrController.lastClickTime = Time.unscaledTime;
		}
		if (DLCManager.DLCManagers.Any((DLCManager x) => x.IsDLCSceneOrLevel(scene)))
		{
			Addressables.LoadSceneAsync((object)scene, LoadSceneMode.Single, true, 100, (SceneReleaseMode)0);
		}
		else
		{
			SceneManager.LoadScene(scene);
		}
	}

	public void LoadSceneWithTransition(WipeDirection direction, string scene = null)
	{
		if (scene != null)
		{
			GCS.sceneToLoad = scene;
		}
		GCS.directionToWipe = direction;
		if (ADOBase.isExpo)
		{
			scrController.lastClickTime = Time.unscaledTime;
		}
		WipeToBlack(GCS.directionToWipe);
	}

	private void WipeFromBlack()
	{
		if (!isWipingToBlack && transitionPanel.IsActive())
		{
			scrSfx.instance.PlaySfx(SfxSound.ScreenWipeIn, MixerGroup.InterfaceParent, 0.5f);
			transitionPanel.gameObject.SetActive(value: true);
			transitionPanel.color = Color.black;
			RectTransform rectTransform = transitionPanel.rectTransform;
			float x = ((wipeDirection == WipeDirection.StartsFromLeft) ? 1f : 0f);
			rectTransform.pivot = new Vector2(x, 0.5f);
			rectTransform.localScale = Vector3.one;
			rectTransform.DOKill();
			float duration = (GCS.speedTrialMode ? (0.3f / GCS.currentSpeedTrial) : 0.3f);
			wipeToBlack = rectTransform.DOScaleX(0f, duration).SetEase(Ease.InOutQuint).SetUpdate(isIndependentUpdate: true);
		}
	}

	private void WipeToBlack(WipeDirection direction, Action onCancel = null)
	{
		scrSfx.instance.PlaySfx(SfxSound.ScreenWipeOut, MixerGroup.InterfaceParent, 0.5f);
		if (isWipingToBlack)
		{
			onCancel?.Invoke();
			MonoBehaviour.print("wipe2black canceled");
			return;
		}
		wipeDirection = direction;
		transitionPanel.gameObject.SetActive(value: true);
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
				if (false && scrUIController.instance != null)
				{
					scrUIController.instance.ShowAchievements(delegate
					{
						LoadTargetScene();
					});
				}
				else
				{
					LoadTargetScene();
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

	private void LoadTargetScene()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		timeSpentLoading = 0f;
		DOTween.KillAll();
		if (GCS.sceneToLoad != null)
		{
			string sceneName = GCS.sceneToLoad;
			SceneManager.sceneLoaded += OnSceneLoaded;
			if (base.dlcManagers.Any((DLCManager x) => x.IsDLCSceneOrLevel(sceneName)))
			{
				Addressables.LoadSceneAsync((object)sceneName, LoadSceneMode.Single, true, 100, (SceneReleaseMode)0);
			}
			else
			{
				SceneManager.LoadScene(GCS.sceneToLoad);
			}
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		wipeCued = true;
		timeSpentLoading = -1f;
		startFrame = Time.frameCount;
	}
}
