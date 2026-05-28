using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuNeoCosmosBuyPage : ADOBase
{
	public MobileMenuController menuController;

	public Transform descriptionContainer;

	public AudioSource audioSource;

	public Transform galleryContainer;

	[Header("Background")]
	public SpriteRenderer[] threads;

	public SpriteRenderer[] backgroundElems;

	[Header("Trailer")]
	public MobileTrailerPlayer trailerPlayer;

	public Image bgFader;

	public Image trailerGlow;

	public RectTransform trailerRectTransform;

	public RectTransform trailerRectTo;

	public RectTransform trailerRectFrom;

	[Header("Buy Screen")]
	public GameObject purchaseScreenContainer;

	public GameObject purchaseButtonContainer;

	public GameObject purchaseButtonLoading;

	public Button purchaseButton;

	public Button restorePurchasesButton;

	public TMP_Text descriptionText;

	public TMP_Text infoText;

	public TMP_Text restorePurchasesText;

	public TMP_Text purchaseButtonText;

	public Image purchaseButtonGlow;

	[Header("Notification Screen")]
	public Transform notificationScreen;

	public GameObject loadingContainer;

	public Transform loadingImage;

	public CanvasGroup loadingCanvasGroup;

	public GameObject popUpContainer;

	public Transform popUpTextContainer;

	public CanvasGroup popUpCanvasGroup;

	public GameObject popUpTitle;

	public TMP_Text popUpTitleText;

	public TMP_Text popUpText;

	public TMP_Text popUpButtonText;

	public Button popUpButton;

	private const float bpm = 120f;

	private const float crotchet = 0.5f;

	private float timeOfLastVisit;

	private MobileMenuScreen titleScreen;

	private MobileMenuDescriptionScreen descriptionScreen;

	private MobileMenuGalleryScreen galleryScreen;

	private float time;

	private int threadProgress;

	private float? origSongVolume = 1f;

	private bool showing;

	private readonly Color[] purchaseGlowColors = new Color[3]
	{
		"FF585A".HexToColor(),
		"589DFF".HexToColor(),
		"58FF5B".HexToColor()
	};

	public void DoThreadAnimation(SpriteRenderer thread)
	{
		Material material = thread.material;
		material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear);
		DOTween.Sequence().Append(material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(material.DOFloat(3.5f, "_FadeApex", 6f).SetEase(Ease.Linear));
		DOTween.Sequence().Append(material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(material.DOFloat(1f, "_FadeScale", 6f).SetEase(Ease.Linear));
	}

	public void OnDestroy()
	{
	}

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		purchaseButtonContainer.SetActive(value: false);
		MobileMenuController mobileMenuController = menuController;
		mobileMenuController.onFinishLoading = (Action)Delegate.Combine(mobileMenuController.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		if (GCS.FOOL_JOKER || ADOBase.isExpo || ADOBase.isSwitch)
		{
			return;
		}
		base.gameObject.SetActive(value: true);
		RectTransform componentInChildren = descriptionContainer.GetComponentInChildren<RectTransform>();
		componentInChildren.sizeDelta = new Vector2(Camera.main.aspect * componentInChildren.sizeDelta.y, componentInChildren.sizeDelta.y);
		if (!MobileMenuMap.EvaluateConditions(new string[1] { "condition:NeoCosmosAd" }))
		{
			base.gameObject.SetActive(value: false);
			SpriteRenderer[] array = threads;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			array = backgroundElems;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			return;
		}
		string text = RDUtils.GenerateHash(GCNS.neoCosmosBundleScenesPath);
		string text2 = RDUtils.GenerateHash(GCNS.bundleShadersPath);
		if (text.IsNullOrEmpty() && text2.IsNullOrEmpty())
		{
			descriptionText.text = RDString.Get("levelSelect.neoCosmosDescription");
		}
		else if (GCNS.neoCosmosScenesHashes.Contains(text) && GCNS.neoCosmosShadersHashes.Contains(text2))
		{
			descriptionText.text = RDString.Get("error.newVersion.NeoCosmos");
		}
		else
		{
			descriptionText.text = RDString.Get("error.corrupted.NeoCosmos");
		}
		descriptionText.SetLocalizedFont();
		restorePurchasesText.text = RDString.Get("levelSelect.restorePurchases");
		restorePurchasesText.SetLocalizedFont();
		infoText.SetLocalizedFont();
		purchaseButtonText.SetLocalizedFont();
		popUpButtonText.text = RDString.Get("editor.ok");
		popUpButtonText.SetLocalizedFont();
		popUpText.SetLocalizedFont();
		popUpTitleText.SetLocalizedFont();
		ShowBackground(show: false, instant: true);
		MobileMenuMap map = menuController.map;
		titleScreen = map.rootGroup[0];
		descriptionScreen = (MobileMenuDescriptionScreen)map.groupLUT["neoCosmosAdGroup"][0];
		descriptionScreen.purchaseAction = delegate
		{
			PurchaseFromKeyboard();
		};
		descriptionScreen.restoreAction = delegate
		{
			RestorePurchasesFromKeyboard();
		};
		galleryScreen = (MobileMenuGalleryScreen)map.groupLUT["neoCosmosGalleryGroup"][0];
		galleryScreen.trailerPlayer = trailerPlayer;
		descriptionContainer.SetParent(descriptionScreen.transform, worldPositionStays: false);
		galleryContainer.SetParent(galleryScreen.transform, worldPositionStays: false);
		notificationScreen.SetParent(descriptionScreen.transform, worldPositionStays: false);
		popUpContainer.SetActive(value: false);
		galleryContainer.gameObject.SetActive(value: true);
		MobileMenuDescriptionScreen mobileMenuDescriptionScreen = descriptionScreen;
		mobileMenuDescriptionScreen.onSelect = (Action<bool, bool>)Delegate.Combine(mobileMenuDescriptionScreen.onSelect, new Action<bool, bool>(OnSelectDescription));
		MobileMenuGalleryScreen mobileMenuGalleryScreen = galleryScreen;
		mobileMenuGalleryScreen.onSelect = (Action<bool, bool>)Delegate.Combine(mobileMenuGalleryScreen.onSelect, new Action<bool, bool>(OnSelectGallery));
		MobileMenuScreen mobileMenuScreen = titleScreen;
		mobileMenuScreen.onSelect = (Action<bool, bool>)Delegate.Combine(mobileMenuScreen.onSelect, new Action<bool, bool>(OnSelectTitleScreen));
		trailerPlayer.onPause = delegate
		{
			OnToggleTrailer(play: false);
		};
		trailerPlayer.onPlay = delegate
		{
			OnToggleTrailer(play: true);
		};
		purchaseButton.onClick.AddListener(Purchase);
		restorePurchasesButton.onClick.AddListener(RestorePurchases);
		popUpButton.onClick.AddListener(ClosePopup);
	}

	private void OnSelectGallery(bool show, bool instant)
	{
		trailerPlayer.isSelected = show;
		StartCoroutine(trailerPlayer.Prepare("https://7thbe.at/neo-cosmos-trailer", show));
		if (!show)
		{
			trailerPlayer.Stop();
		}
	}

	private void OnToggleTrailer(bool play)
	{
		ToggleNeoMusic(!play, play);
		float duration = (play ? 2f : 0.5f);
		bgFader.DOKill();
		bgFader.DOFade(play ? 1f : 0f, duration).SetEase(Ease.OutQuad);
		trailerGlow.DOKill();
		trailerGlow.DOFade(play ? 0f : 1f, duration).SetEase(Ease.OutQuad);
		float duration2 = (play ? 1f : 0.25f);
		RectTransform rectTransform = (play ? trailerRectTo : trailerRectFrom);
		trailerRectTransform.DOKill();
		trailerRectTransform.DOSizeDelta(rectTransform.sizeDelta + Vector2.one * 36f, duration2).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			galleryScreen.OnToggleTrailerComplete(play);
		});
		trailerRectTransform.DOLocalMove(rectTransform.localPosition, duration2).SetEase(Ease.OutQuad);
		menuController.Enable(!play);
	}

	private void OnSelectDescription(bool show, bool instant)
	{
		if (show)
		{
			ShowBackground(show: true);
			ToggleNeoMusic(on: true);
		}
		purchaseButtonContainer.SetActive(show);
	}

	private void OnSelectTitleScreen(bool show, bool instant)
	{
		if (show)
		{
			ToggleNeoMusic(on: false);
			ShowBackground(show: false);
		}
	}

	public void OpenPopUp(string message, bool showTitle = true, string titleKey = "error.ocurred")
	{
		popUpTitle.SetActive(showTitle);
		popUpTitleText.text = RDString.Get(titleKey);
		popUpText.text = message;
		popUpTextContainer.ScaleXY(0.5f);
		popUpButton.gameObject.transform.ScaleXY(0.5f);
		popUpContainer.SetActive(value: true);
		popUpCanvasGroup.DOFade(1f, 0.25f).SetEase(Ease.OutSine);
		popUpTextContainer.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		popUpButton.gameObject.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	private void ClosePopup()
	{
		popUpCanvasGroup.DOFade(0f, 0.25f).SetEase(Ease.OutSine).OnComplete(delegate
		{
			popUpContainer.SetActive(value: false);
		});
		popUpTextContainer.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack);
		popUpButton.gameObject.transform.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack);
		scrSfx.instance.PlaySfx(SfxSound.MobileButton, MixerGroup.InterfaceParent);
	}

	private void ShowLoading(bool show)
	{
		float endValue = (show ? 1f : 0f);
		loadingImage.ScaleXY(show ? 0.1f : 0.5f);
		loadingContainer.SetActive(value: true);
		loadingCanvasGroup.DOFade(endValue, 0.25f).SetEase(Ease.OutSine).OnComplete(delegate
		{
			loadingContainer.SetActive(show);
		});
		loadingImage.DOScale(show ? 0.5f : 0.1f, show ? 0.5f : 0.1f).SetEase(Ease.OutBack);
	}

	private void ShowBackground(bool show, bool instant = false)
	{
		if (showing == show && !instant)
		{
			return;
		}
		showing = show;
		float num = (instant ? 0f : 2f);
		float endValue = (show ? 1f : 0f);
		SpriteRenderer[] array = backgroundElems;
		foreach (SpriteRenderer target in array)
		{
			target.DOKill();
			target.DOFade(endValue, num).SetEase(Ease.OutSine);
		}
		array = threads;
		foreach (SpriteRenderer target2 in array)
		{
			target2.DOKill();
			target2.DOFade(endValue, num).SetEase(Ease.OutSine);
		}
		if (show)
		{
			float valueOrDefault = origSongVolume.GetValueOrDefault();
			if (!origSongVolume.HasValue)
			{
				valueOrDefault = ADOBase.conductor.song.volume;
				origSongVolume = valueOrDefault;
			}
			audioSource.volume = 0f;
			ADOBase.conductor.DuckSongStart(0f, num);
		}
		else
		{
			ADOBase.conductor.DuckSongStop(num);
		}
	}

	private void ToggleNeoMusic(bool on, bool instant = false)
	{
		float num = (on ? origSongVolume.Value : 0f);
		if (audioSource.volume == num)
		{
			return;
		}
		float duration = (instant ? 0f : 2f);
		if (on)
		{
			float valueOrDefault = origSongVolume.GetValueOrDefault();
			if (!origSongVolume.HasValue)
			{
				valueOrDefault = ADOBase.conductor.song.volume;
				origSongVolume = valueOrDefault;
			}
			if (Time.timeSinceLevelLoad - timeOfLastVisit > 1f)
			{
				audioSource.Play();
			}
		}
		timeOfLastVisit = Time.timeSinceLevelLoad;
		audioSource.DOKill();
		audioSource.DOFade(num, duration);
	}

	private void UpdateThreads()
	{
		time += Time.deltaTime;
		float num = 2f * (float)threadProgress;
		if (time >= num)
		{
			SpriteRenderer thread = threads[threadProgress % threads.Length];
			DoThreadAnimation(thread);
			threadProgress++;
		}
	}

	private void Update()
	{
		UpdateThreads();
		UpdatePurchaseGlow();
	}

	private void UpdatePurchaseGlow()
	{
		float num = audioSource.time / 2f;
		int num2 = Mathf.FloorToInt(num % 3f);
		float num3 = Mathf.PingPong(num * 2f % 2f, 1f);
		DOVirtual.EasedValue(0f, 1f, num3, Ease.InOutSine);
		purchaseButtonGlow.color = purchaseGlowColors[num2].WithAlpha(num3 * 0.5f);
		trailerGlow.color = trailerGlow.color.WithAlpha(0.2f + num3 * 0.4f);
	}

	public void PurchaseFromKeyboard()
	{
		if (!loadingContainer.activeInHierarchy)
		{
			if (popUpContainer.activeInHierarchy)
			{
				ClosePopup();
			}
			else if (purchaseButton.interactable)
			{
				Purchase();
			}
		}
	}

	private void Purchase()
	{
		if (base.neoCosmosManager.installed)
		{
			MobileMenuScreen mobileMenuScreen = menuController.map.groupLUT["neoCosmosEntranceGroup"][0];
			if (menuController.currentScreen == descriptionScreen)
			{
				menuController.JumpToScreen(mobileMenuScreen);
			}
			else
			{
				descriptionScreen.visible = false;
				galleryScreen.visible = false;
				ShowLoading(show: false);
				mobileMenuScreen.parentGroup.inaccessible = false;
				menuController.map.Build();
			}
		}
		else
		{
			ShowLoading(show: true);
			EnableRestorePurchasesButton(enable: false);
		}
		scrSfx.instance.PlaySfx(SfxSound.MobileButton, MixerGroup.InterfaceParent);
	}

	private void OnPurchaseCompleted()
	{
		if (!base.neoCosmosManager.own)
		{
			ShowLoading(show: false);
			EnableRestorePurchasesButton(enable: true);
		}
	}

	private void Download()
	{
		ShowLoading(show: false);
		purchaseButtonText.text = RDString.Get("editor.dialog.downloading");
		purchaseButton.interactable = false;
		purchaseButtonGlow.gameObject.SetActive(value: false);
	}

	private void OnFinishDownload()
	{
		purchaseButtonGlow.gameObject.SetActive(value: true);
		EnableRestorePurchasesButton(enable: true);
		if (base.neoCosmosManager.installed)
		{
			purchaseButtonText.text = RDString.Get("levelSelect.play");
			purchaseButton.interactable = true;
		}
	}

	private void EnableRestorePurchasesButton(bool enable)
	{
		restorePurchasesButton.gameObject.SetActive(showing && enable);
	}

	public void RestorePurchasesFromKeyboard()
	{
		if (!loadingContainer.activeInHierarchy && !popUpContainer.activeInHierarchy && restorePurchasesButton.interactable)
		{
			RestorePurchases();
		}
	}

	private void RestorePurchases()
	{
		ShowLoading(show: true);
		scrSfx.instance.PlaySfx(SfxSound.MobileButton, MixerGroup.InterfaceParent);
	}

	private void OnRestorePurchases()
	{
		ShowLoading(show: false);
	}
}
