using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndscreenLanterns : ADOBase
{
	public RectTransform chainsContainer;

	public Image blueLantern;

	public Image yellowLantern;

	public Image redLantern;

	public Text blueLanternText;

	public Text yellowLanternText;

	public Text redLanternText;

	public Transform planetsScaler;

	public Transform planetsBubble;

	public SpinningPlanets spinningPlanets;

	public RectTransform planetsPositionTransform;

	public SpriteRenderer bubbleSprite;

	public AudioClip lanternSound1;

	public AudioClip lanternSound1B;

	public AudioClip lanternSound2;

	public AudioClip lanternSound2B;

	public AudioClip lanternSound3;

	public AudioClip lanternSound3B;

	public AudioClip lanternSound4;

	public AudioClip lanternSound4B;

	public List<RectTransform> autoLayoutChains;

	public SpriteRenderer[] crowns;

	private (bool, bool, bool) prevLampStates;

	private Sequence chainSequence;

	private Sequence[] lampSequences = new Sequence[3];

	public const float TimeBetweenLanternSets = 1.25f;

	public void Setup()
	{
		blueLanternText.gameObject.SetActive(value: false);
		redLanternText.gameObject.SetActive(value: false);
		yellowLanternText.gameObject.SetActive(value: false);
		planetsScaler.gameObject.SetActive(scrController.coopMode);
		if (ADOBase.isCLSLevel)
		{
			string hash = scnGame.instance.levelData.Hash;
			prevLampStates = (Persistence.GetCustomWorldCompletion(hash) >= 1f, Persistence.GetCustomWorldIsHighestPossibleAcc(hash), Persistence.GetCustomWorldSpeedTrial(hash) > 1f);
		}
		else if (ADOBase.isOfficialLevel && !scrController.currentWorldString.IsNullOrEmpty())
		{
			int currentWorld = scrController.currentWorld;
			if (RDC.forceUnlockAllLevels)
			{
				RDC.forceUnlockAllLevels = false;
			}
			prevLampStates = (Persistence.IsWorldComplete(currentWorld), Persistence.IsWorldPerfect(currentWorld), Persistence.IsSpeedTrialComplete(currentWorld));
			RDC.forceUnlockAllLevels = Persistence.unlockAllLevels;
		}
	}

	private void Update()
	{
		if (scrController.coopMode)
		{
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;
			Vector2 vector = new Vector2(Mathf.Sin(timeSinceLevelLoad * 1f + 1.5f), Mathf.Sin(timeSinceLevelLoad * 1f * 0.6f)) * 0.1f;
			planetsBubble.localPosition = vector;
		}
	}

	private void LateUpdate()
	{
		if (scrController.coopMode)
		{
			CanvasScaler canvasScaler = ADOBase.uiController.canvasScaler;
			Camera camobj = ADOBase.controller.camy.camobj;
			float num = canvasScaler.referenceResolution.y / (float)camobj.scaledPixelHeight;
			float num2 = camobj.orthographicSize / 5f;
			Vector3 v = camobj.ScreenToWorldPoint(planetsPositionTransform.position);
			planetsScaler.position = v.WithZ(planetsScaler.position.z);
			planetsScaler.localScale = Vector3.one * num2 * num;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public int Show(scrPlayer player, float lanternsDelay = 0f, int soundSet = -1, bool crown = false)
	{
		base.gameObject.SetActive(value: true);
		int lanternsShown = 0;
		scrMarginTracker marginTracker = player.marginTracker;
		bool coopMode = scrController.coopMode;
		bool flag = marginTracker.percentComplete >= 1f && player.alive;
		bool flag2 = marginTracker.percentAcc >= 1f;
		bool flag3 = marginTracker.IsAllPurePerfect();
		float num = 0f;
		if (ADOBase.isCLSLevel)
		{
			num = (float)scnGame.instance.levelData.levelSettings["speedTrialAim"];
		}
		else if (!scrController.currentWorldString.IsNullOrEmpty())
		{
			num = Persistence.GetSpeedTrialAimForWorld(scrController.currentWorldString);
		}
		bool flag4 = GCS.speedTrialMode && GCS.currentSpeedTrial >= num && marginTracker.deadTiles == 0;
		bool flag5 = !coopMode && prevLampStates.Item1;
		bool flag6 = !coopMode && prevLampStates.Item2;
		bool flag7 = !coopMode && prevLampStates.Item3;
		bool flag8 = flag && !flag5;
		bool flag9 = flag2 && !flag6;
		bool flag10 = flag4 && !flag7;
		bool flag11 = (coopMode ? GCS.speedTrialMode : (GCS.speedTrialMode || flag7));
		blueLanternText.text = RDString.Get((ADOBase.isCLSLevel && !ADOBase.isCLSBossLevel) ? "status.results.levelComplete" : "status.results.worldComplete");
		yellowLanternText.text = RDString.Get(flag3 ? "status.allPurePerfect" : "status.results.greatAccuracy");
		redLanternText.text = RDString.Get("status.results.speedTrial");
		redLanternText.SetLocalizedFont();
		yellowLanternText.SetLocalizedFont();
		blueLanternText.SetLocalizedFont();
		SpriteRenderer[] array = crowns;
		foreach (SpriteRenderer obj in array)
		{
			obj.gameObject.SetActive(crown);
			obj.color = Color.Lerp(Color.white, player.planetarySystem.chosenPlanet.planetRenderer.planetColor.ToRealColor(), 0.25f);
			scrMisc.Rotate2D(obj.transform.parent, scrController.instance.camy.transform.rotation.eulerAngles.z);
		}
		Sequence[] array2 = lampSequences;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i]?.Kill(complete: true);
		}
		chainSequence?.Kill(complete: true);
		Color color = Color.gray.WithAlpha(0.5f);
		if (!flag5)
		{
			blueLantern.color = color;
			blueLantern.transform.GetChild(1).gameObject.SetActive(value: false);
		}
		if (!flag6)
		{
			yellowLantern.color = color;
			yellowLantern.transform.GetChild(1).gameObject.SetActive(value: false);
		}
		if (!flag7)
		{
			redLantern.color = color;
			redLantern.transform.GetChild(1).gameObject.SetActive(value: false);
		}
		RectTransform component = GetComponent<RectTransform>();
		component.gameObject.SetActive(value: true);
		component.anchoredPosition = new Vector2(0f, 0f);
		redLantern.gameObject.SetActive(flag11);
		redLanternText.transform.parent.gameObject.SetActive(flag11);
		if (!flag11)
		{
			yellowLantern.transform.SetParent(autoLayoutChains[1].transform, worldPositionStays: false);
			yellowLantern.transform.localPosition = Vector3.zero;
			blueLantern.transform.SetParent(autoLayoutChains[0].transform, worldPositionStays: false);
			blueLantern.transform.localPosition = Vector3.zero;
		}
		Transform parent = blueLanternText.transform.parent;
		Transform parent2 = yellowLanternText.transform.parent;
		Transform parent3 = redLanternText.transform.parent;
		parent.position = blueLantern.transform.GetChild(0).position;
		parent2.position = yellowLantern.transform.GetChild(0).position;
		parent3.position = redLantern.transform.GetChild(0).position;
		if (flag11)
		{
			parent2.PositionY((parent.position.y + parent3.position.y) / 2f);
		}
		List<RectTransform> list = new List<RectTransform>();
		for (int j = 0; j < chainsContainer.childCount; j++)
		{
			list.Add(chainsContainer.GetChild(j).GetComponent<RectTransform>());
		}
		chainSequence = DOTween.Sequence();
		for (int k = 0; k < list.Count; k++)
		{
			RectTransform rectTransform = list[k];
			float duration = 2.5f;
			float num2 = 0.01f;
			chainSequence.Insert((float)k * num2, rectTransform.DOAnchorPos(rectTransform.anchoredPosition, duration).From(rectTransform.anchoredPosition + new Vector2(0f - component.sizeDelta.x, component.sizeDelta.y) * 0.666f).SetEase(Ease.OutElastic, 0.0005f, 1f));
		}
		AudioClip sound = lanternSound1B;
		AudioClip sound2 = lanternSound3B;
		AudioClip sound3 = lanternSound4B;
		switch (soundSet)
		{
		case 0:
			sound = lanternSound1B;
			sound2 = (sound3 = lanternSound1);
			break;
		case 1:
			sound = lanternSound2B;
			sound2 = (sound3 = lanternSound2);
			break;
		case 2:
			sound = lanternSound3B;
			sound2 = (sound3 = lanternSound3);
			break;
		case 3:
			sound = lanternSound4B;
			sound2 = (sound3 = lanternSound4);
			break;
		}
		float num3 = (flag11 ? 4f : 3f);
		float num4 = 1.25f / num3;
		int num5 = 0;
		if (flag8)
		{
			lampSequences[0] = CreateGhostEffect(blueLantern, blueLanternText, sound, lanternsDelay + 0.5f + num4 * (float)(++num5));
		}
		if (flag9)
		{
			lampSequences[1] = CreateGhostEffect(yellowLantern, yellowLanternText, sound2, lanternsDelay + 0.5f + num4 * (float)(++num5));
		}
		if (flag10)
		{
			lampSequences[2] = CreateGhostEffect(redLantern, redLanternText, sound3, lanternsDelay + 0.5f + num4 * (float)(++num5));
		}
		if (scrController.coopMode)
		{
			planetsScaler.gameObject.SetActive(value: true);
			spinningPlanets.SetAppearance(player);
			planetsBubble.DOKill();
			planetsBubble.transform.DOScale(player.alive ? 1.5f : 0.8f, 0.5f).From(Vector3.one * 0.01f).SetEase(Ease.OutBack)
				.SetDelay(0.5f);
			bubbleSprite.enabled = !player.alive;
		}
		return lanternsShown;
		Sequence CreateGhostEffect(Image lanternImage, Text lanternText, AudioClip clip, float delay)
		{
			Image component2 = lanternImage.transform.GetChild(0).GetComponent<Image>();
			component2.material = Object.Instantiate(component2.material);
			GameObject lanternLight = lanternImage.transform.GetChild(1).gameObject;
			RectMask2D textMask = lanternText.GetComponentInParent<RectMask2D>();
			lanternText.gameObject.SetActive(value: true);
			lanternText.GetComponent<scrHUDText>().enabled = false;
			lanternText.color = lanternText.color.WithAlpha(1f);
			lanternsShown++;
			return DOTween.Sequence().AppendInterval(delay).Append(component2.material.DOFloat(1f, "_FlashAlpha", 0.3f).From(0f))
				.Append(component2.transform.DOScale(5f, 0.4f).From(1.05f))
				.JoinCallback(delegate
				{
					lanternLight.SetActive(value: true);
				})
				.JoinCallback(delegate
				{
					scrSfx.instance.PlaySfx(clip, MixerGroup.SfxParent);
				})
				.Join(component2.material.DOFloat(0f, "_FlashAlpha", 0.39f))
				.Join(lanternImage.DOColor(Color.white, 0.3f))
				.Join(DOTween.To(() => textMask.padding.z, delegate(float value)
				{
					textMask.padding = textMask.padding.WithZ(value);
				}, 0f, 1f).From(lanternText.preferredWidth - 120f - 5f))
				.AppendInterval(0.25f)
				.Append(lanternText.DOFade(0f, 1f));
		}
	}
}
