using System;
using System.Collections.Generic;
using DG.Tweening;
using MobileMenu;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class scrPortal : ADOBase
{
	private readonly float[] lanternLightAlphas = new float[3] { 0.8f, 0.588f, 0.71f };

	[Header("World Info")]
	public string world;

	public Transform jumpPositionMarker;

	public Transform trialPositionMarker;

	[Header("Components")]
	public CanvasGroup stats;

	public SpriteRenderer sprPortal;

	public GameObject padlockContainer;

	public SpriteRenderer padlock1;

	public SpriteRenderer padlock2;

	public Material defaultMaterial;

	public Material grayscale;

	public scrMenuWorldStatsText statsText;

	public RectTransform statsTextContainer;

	public Text backText;

	public Button button;

	public SpriteRenderer xtraDecoration;

	public ParticleSystem[] borderParticles;

	public bool useMetadataCreditsFromPC;

	public PortalCredit levelCredits;

	public PortalCredit songCredits;

	public PortalCredit secondaryLevelCredits;

	public PortalCredit secondarySongCredits;

	public PortalCredit tertiaryLevelCredits;

	public PortalCredit tertiarySongCredits;

	public RectTransform statsCanvasMobilePlaceholder;

	public MedalLayout medalLayout;

	public PortalSign sign;

	public SpriteRenderer helperIcon;

	[FormerlySerializedAs("canvasGroup")]
	public CanvasGroup creditsCanvasGroup;

	[NonSerialized]
	public float originalPortalScale;

	private Vector3 originalDecScale;

	private bool xtraWorld;

	private bool crownWorld;

	private bool museDashWorld;

	private bool isXtraDesktopPortal;

	public bool locked;

	public bool medalLocked;

	public bool usesCrownSign;

	public static Dictionary<string, scrPortal> portals = new Dictionary<string, scrPortal>();

	[Header("Taro DLC")]
	public GameObject taroStats;

	public Text medalCount;

	private float timeDisplayed;

	private bool showingMedals;

	private Sequence statsTransition;

	private Sequence medalsFade;

	private Sequence medalsAppear;

	[Header("Taro DLC EX World")]
	public Text medalRequirementWorld;

	public Text medalRequirementCount;

	public GameObject taroLock;

	public scrFloor portalEntrance;

	private const float timeForTransition = 5f;

	private bool fadetoggle;

	[NonSerialized]
	public bool hidden;

	private Vector2Int prevPlanetPosition = new Vector2Int(-100, -100);

	public Vector2Int jumpPosition => new Vector2Int((int)jumpPositionMarker.position.x, (int)jumpPositionMarker.position.y);

	public Vector2Int trialPosition => new Vector2Int((int)trialPositionMarker.position.x, (int)trialPositionMarker.position.y);

	private bool neverPlayed
	{
		get
		{
			if (Persistence.GetPercentCompletion(ADOBase.worldData[world].index, scrController.coopMode) == 0f)
			{
				return Persistence.GetWorldAttempts(ADOBase.worldData[world].index, scrController.coopMode) == 0;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (sprPortal != null)
		{
			originalPortalScale = sprPortal.transform.localScale.x;
		}
		if (!portals.ContainsKey(world))
		{
			portals.Add(world, this);
		}
		else
		{
			portals[world] = this;
		}
		FadeCredits(0f, instant: true);
		if (GCS.FOOL_JOKER && !ADOBase.isMobileMenu)
		{
			world += "J";
			if (!ADOBase.worldData.ContainsKey(world))
			{
				base.gameObject.SetActive(value: false);
			}
			sprPortal.sprite = Resources.Load<Sprite>("InternalLevels/" + world + "/portal");
		}
	}

	private void Start()
	{
		Setup();
		if (!ADOBase.isMobileMenu && !locked)
		{
			ShowStats(show: false, instant: true);
		}
		ShowMedals(show: false);
		if (!ADOBase.isMobileMenu && useMetadataCreditsFromPC)
		{
			SetupCredits();
		}
	}

	public void SetupCredits()
	{
		GCNS.WorldData worldData = GCNS.worldData[world];
		levelCredits.Load(worldData.levelCredits);
		songCredits.Load(worldData.songCredits);
		secondaryLevelCredits.Load(worldData.secondaryLevelCredits);
		secondarySongCredits.Load(worldData.secondarySongCredits);
		tertiaryLevelCredits.Load(worldData.tertiaryLevelCredits);
		tertiarySongCredits.Load(worldData.tertiarySongCredits);
	}

	public void Setup(bool speedTrial = false)
	{
		if (world.IsNullOrEmpty())
		{
			return;
		}
		if (world.EndsWith("EX") && GCNS.worldData.ContainsKey(world))
		{
			string text = world.Remove(world.Length - 2);
			int requiredMedals = GCNS.worldData[world].requiredMedals;
			int num = 0;
			int[] medalsForDLCLevel = Persistence.GetMedalsForDLCLevel(text);
			for (int i = 0; i < medalsForDLCLevel.Length; i++)
			{
				if (medalsForDLCLevel[i] >= 3)
				{
					num++;
				}
			}
			if (num < requiredMedals && neverPlayed && !RDC.forceUnlockAllLevels)
			{
				medalRequirementWorld.text = text;
				medalRequirementCount.text = "x" + requiredMedals;
				taroLock.gameObject.SetActive(value: true);
				taroStats.gameObject.SetActive(value: false);
				statsText.gameObject.SetActive(value: false);
				ShowStats(show: true);
				if (!ADOBase.isMobileMenu)
				{
					portalEntrance.gameObject.SetActive(value: false);
				}
				LockWorld(locked: true);
				medalLocked = true;
			}
		}
		xtraWorld = world.IsXtra() || world.IsMuseDashWorld();
		museDashWorld = world.IsMuseDashWorld();
		bool active = world.IsTaro();
		if (taroStats != null && !locked && !neverPlayed)
		{
			taroStats.SetActive(active);
		}
		isXtraDesktopPortal = xtraWorld;
		if (isXtraDesktopPortal)
		{
			sign.worldName.color = sign.worldName.color.WithAlpha(0f);
			stats.alpha = 0f;
			if (xtraDecoration != null)
			{
				xtraDecoration.gameObject.SetActive(value: false);
			}
		}
		sign.UpdateLanterns(world);
		sign.UpdateWorldName(world, speedTrial);
		UpdateMedals();
	}

	public void LockWorld(bool locked, bool speedTrial = false)
	{
		this.locked = locked;
		padlockContainer.SetActive(locked);
		sprPortal.material = (locked ? grayscale : defaultMaterial);
		statsText.UpdateText(locked, speedTrial);
	}

	private void Update()
	{
		_ = ADOBase.sceneName;
		if (MobileMenuController.instance != null || ADOBase.isMobileMenu || isXtraDesktopPortal || locked)
		{
			return;
		}
		Vector3 position = scrController.instance.chosenPlanet.transform.position;
		Vector2Int vector2Int = (prevPlanetPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)));
		string text = null;
		Vector2Int vector2Int2 = jumpPosition - vector2Int;
		if (Math.Max(Math.Abs(vector2Int2.x), Math.Abs(vector2Int2.y)) == 0)
		{
			text = world;
			if (scnLevelSelect.instance != null)
			{
				scnLevelSelect.instance.lastVisitedWorld = world;
			}
		}
		if (text == world && !fadetoggle && !hidden)
		{
			ExpandPortal(expand: true);
			FadeCredits(1f);
		}
		else if (text != world && fadetoggle)
		{
			ExpandPortal(expand: false);
			FadeCredits(0f);
		}
		if (fadetoggle && world.IsTaro() && Persistence.IsWorldComplete(world))
		{
			timeDisplayed += Time.deltaTime;
			timeDisplayed %= 10f;
			if ((!showingMedals && timeDisplayed < 5f) || (showingMedals && timeDisplayed >= 5f))
			{
				SwitchBetwenTaroStats(!showingMedals);
			}
		}
		if (ADOBase.gc.debug && portalEntrance != null)
		{
			portalEntrance.gameObject.SetActive(value: true);
		}
	}

	public void FadeCredits(float alpha, bool instant = false)
	{
		float duration = (instant ? 0f : 0.4f);
		alpha = (locked ? 0f : alpha);
		if (creditsCanvasGroup != null)
		{
			creditsCanvasGroup.DOKill();
			creditsCanvasGroup.DOFade(alpha, duration);
			creditsCanvasGroup.interactable = alpha >= 1f;
		}
		if (helperIcon != null)
		{
			helperIcon.DOFade(alpha, duration);
		}
	}

	public void ExpandPortal(bool expand, bool instant = false)
	{
		float num = (instant ? 0f : 0.4f);
		float alpha = (expand ? 0.4f : 1f);
		float alpha2 = (expand ? 1f : 0f);
		sprPortal.transform.DOScale(originalPortalScale * (expand ? 0.8f : 1f), num * 0.75f);
		if (xtraDecoration != null)
		{
			xtraDecoration.gameObject.SetActive(expand);
		}
		FadeCredits(alpha2, instant);
		FadePortalImage(alpha, instant);
		ShowStats(expand);
	}

	public void ExpandPortalMobile(bool expand)
	{
		float num = 0.4f;
		float alpha = (expand ? 0.2f : 1f);
		sprPortal.transform.DOScale(originalPortalScale * (expand ? 0.8f : 1f), num * 0.75f);
		FadeCredits(expand ? 0f : 1f);
		if (world.IsTaro())
		{
			ShowMedals(expand, fade: true);
			if (!neverPlayed)
			{
				FadePortalImage(alpha);
			}
		}
	}

	public void FadePortalImage(float alpha, bool instant = false)
	{
		float duration = (instant ? 0f : 0.4f);
		ParticleSystem[] array = borderParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetComponent<Renderer>().material.DOFade(alpha, duration);
		}
		sprPortal.DOFade(alpha, duration);
	}

	public void FadePortal(float alpha, bool instant = false)
	{
		float duration = (instant ? 0f : 0.4f);
		sign.Fade(alpha, duration);
		FadePortalImage(alpha, instant);
	}

	public void ShowStats(bool show, bool instant = false)
	{
		fadetoggle = show;
		float endValue = (show ? 1f : 0f);
		float duration = (instant ? 0f : 0.4f);
		stats.DOKill();
		stats.DOFade(endValue, duration);
		if (isXtraDesktopPortal)
		{
			sign.worldName.DOFade(endValue, duration);
		}
		if (!world.IsTaro() || locked || neverPlayed)
		{
			return;
		}
		medalsFade.Kill();
		statsTransition.Kill();
		List<Mawaru_Medal> medals = medalLayout.medals;
		if (show)
		{
			foreach (Mawaru_Medal item in medals)
			{
				item.front.render.SetAlpha(1f);
				item.back.render.SetAlpha(1f);
				item.transform.localScale = Vector3.zero;
			}
		}
		else
		{
			medalsFade = DOTween.Sequence();
			foreach (Mawaru_Medal item2 in medals)
			{
				medalsFade.Insert(0f, item2.front.render.DOFade(0f, duration));
				medalsFade.Insert(0f, item2.back.render.DOFade(0f, duration));
			}
		}
		if (show && !ADOBase.isMobileMenu)
		{
			ShowMedals(show: true);
			timeDisplayed = 3f;
		}
	}

	public void ShowMedals(bool show, bool fade = false)
	{
		if (medalLayout.medals != null)
		{
			showingMedals = show;
			statsText.gameObject.SetActive(!show);
			timeDisplayed = 5f;
			if (show)
			{
				medalCount.gameObject.SetActive(value: true);
			}
			float num = (show ? 1f : 0f);
			if (fade)
			{
				medalCount.DOFade(num, 0.25f);
			}
			else
			{
				medalCount.color = medalCount.color.WithAlpha(num);
			}
			DoMedalsAnimation(show);
		}
	}

	public void ShakePortal()
	{
		GetComponentInChildren<scrGfxFloat>().Shake();
	}

	private void UpdateMedals()
	{
		if (!world.IsTaro() || locked || neverPlayed)
		{
			return;
		}
		if (medalLayout.medals == null || medalLayout.medals.Count == 0)
		{
			medalLayout.Generate(GCNS.worldData[world].medalCount);
		}
		List<Mawaru_Medal> medals = medalLayout.medals;
		int[] medalsForDLCLevel = Persistence.GetMedalsForDLCLevel(world);
		int num = 0;
		for (int i = 0; i < medals.Count && i < medalsForDLCLevel.Length; i++)
		{
			Mawaru_Medal mawaru_Medal = medals[i];
			int num2 = medalsForDLCLevel[i];
			if (num2 > 0)
			{
				mawaru_Medal.front.SetState(num2 - 1);
				if (num2 == 3)
				{
					num++;
				}
			}
			else
			{
				mawaru_Medal.front.render.enabled = false;
			}
			medals[i].transform.localScale = Vector3.zero;
		}
		string text = RDString.Get("levelSelect.taroWorldStats");
		text = text.Replace("[current]", num.ToString());
		text = text.Replace("[total]", medals.Count.ToString());
		medalCount.text = text;
		medalCount.SetLocalizedFont();
	}

	private void SwitchBetwenTaroStats(bool showTaroStats)
	{
		showingMedals = showTaroStats;
		statsTransition.Kill();
		Sequence t = DoMedalsAnimation(showTaroStats);
		if (showTaroStats)
		{
			statsTransition = DOTween.Sequence().Append(stats.DOFade(0f, 0.5f)).SetEase(Ease.Linear)
				.Append(stats.DOFade(1f, 0.5f))
				.SetEase(Ease.Linear)
				.Join(t);
		}
		else
		{
			statsTransition = DOTween.Sequence().Append(t).Join(stats.DOFade(0f, 0.5f))
				.SetEase(Ease.Linear)
				.Append(stats.DOFade(1f, 0.5f))
				.SetEase(Ease.Linear);
		}
		statsTransition.Join(DOVirtual.DelayedCall(0f, delegate
		{
			statsText.gameObject.SetActive(!showTaroStats);
			medalCount.gameObject.SetActive(showTaroStats);
		}));
	}

	private Sequence DoMedalsAnimation(bool show)
	{
		List<Mawaru_Medal> medals = medalLayout.medals;
		if (medalsAppear != null)
		{
			medalsAppear.Kill();
		}
		medalsAppear = DOTween.Sequence();
		for (int i = 0; i < medals.Count; i++)
		{
			Mawaru_Medal mawaru_Medal = medals[i];
			medalsAppear.Insert((float)i * 0.025f, mawaru_Medal.transform.DOScale(show ? Vector3.one : Vector3.zero, 0.1f).SetEase(show ? Ease.OutBack : Ease.InBack));
			if (show)
			{
				mawaru_Medal.transform.localScale = Vector3.zero;
			}
		}
		return medalsAppear;
	}
}
