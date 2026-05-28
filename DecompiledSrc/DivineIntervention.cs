using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class DivineIntervention : TaroBGScript
{
	public bool isLongVersion = true;

	public AudioClip shortVersion;

	[Header("Resource management", order = 0)]
	public List<Mawaru_Sprite> stuffToDisable;

	public Image allCover;

	public List<GameObject> highQualBG;

	[Header("Assets", order = 0)]
	public List<Mawaru_Sprite> embers;

	private List<Ember> elist;

	private float edelay = 0.15f;

	private float etimer;

	private int curEmber;

	public OverseerIdle bossIdle;

	public GameObject bossBody;

	public GameObject bossHand1;

	public GameObject bossHand2;

	public Mawaru_Sprite glare;

	public List<Mawaru_Sprite> bossAll;

	public GameObject fallTileWarn;

	public List<AreaCover> areaCovers = new List<AreaCover>();

	public List<LaserSpawner> laserSpawners = new List<LaserSpawner>();

	private Vector3 squish = new Vector3(0.6f, 1f, 1f);

	public List<GameObject> BGsToStretch = new List<GameObject>();

	public GameObject menuBGcontainer;

	public Mawaru_Sprite menuBGgradient;

	public Mawaru_Sprite menuBGcover;

	public Mawaru_Sprite lasertile;

	public Mawaru_Sprite hotBG;

	public Transform kickFloorTarg;

	public Mawaru_Sprite driftSky;

	public Mawaru_Sprite plasmaBGcover;

	public Mawaru_Sprite plasmaCold;

	public Mawaru_Sprite plasmaHot;

	public List<Mawaru_Sprite> clearsky_bgstuff;

	public List<Mawaru_Sprite> cloudy_bgstuff;

	public List<Mawaru_Sprite> hope_bgstuff;

	public List<Mawaru_Sprite> darkClouds;

	public List<Mawaru_Sprite> lightClouds;

	public List<Mawaru_Sprite> clouds;

	public List<Transform> sunbeamParents;

	public List<Mawaru_Sprite> sunbeams;

	public List<Mawaru_Sprite> closingEyes;

	public Mawaru_Sprite darken;

	private List<Transform> cloudAddX = new List<Transform>();

	private List<Vector3> cloudDefaultPos = new List<Vector3>();

	private Dictionary<scrFloor, Mawaru_Sprite> floorWarns = new Dictionary<scrFloor, Mawaru_Sprite>();

	private List<int> tilesAppearingLate = new List<int> { 162, 163, 164, 174, 175, 176 };

	public TaroCutsceneScript scene;

	private GameObject camRotAux;

	private GameObject camPulseAux;

	public GameObject acParent1;

	public Transform bossBGParent;

	public List<GameObject> bossEndHandContainers;

	public List<Mawaru_Sprite> bossEndHands;

	public TextMeshProUGUI timerText;

	public TextMeshProUGUI myBestText;

	public GameObject goalTime;

	public TextMeshPro goalTimeText;

	public TextMeshPro endingTTText;

	public Mawaru_Sprite ttGoalGraphic;

	public Mawaru_Sprite ttStartGraphic;

	public GameObject endingTTBar;

	private scrFloor timeTrialEndFloor;

	public Transform bootlegFloor;

	public Transform bootlegInner;

	public Transform bootlegSpawn;

	public Transform bootlegTarg;

	private List<scrSpike> listSpikes = new List<scrSpike>();

	public scrSpike lateSpike;

	public Mawaru_Sprite charlie_mario;

	public Mawaru_Sprite sef_mario;

	public GameObject charlie_planet;

	public GameObject sef_planet;

	public Transform charlie_planet_parent;

	public Transform sef_planet_parent;

	public Sef sef;

	public Mawaru_Charlie charlie;

	public EndingSparkle endingSparkle;

	private float longVer;

	private int judgingSection = -1;

	private bool cursection_hasMadeMistakes;

	private bool cursection_hasNonPerfects;

	private bool ns_hasMadeMistakes;

	private bool ns_hasNonPerfects;

	private bool judgedFinalSection;

	public List<GameObject> endingStuff;

	public List<Mawaru_Medal> endingMedals;

	public List<float> endingStuff_originalSizes = new List<float>();

	private List<int> sectionFloors = new List<int>
	{
		17, 92, 104, 149, 150, 178, 179, 256, 257, 337,
		338, 404, 405, 405, 412, 456, 491, 641, 680
	};

	private List<Color> RankingTextColors = new List<Color>
	{
		new Color(1f, 1f, 1f, 1f),
		new Color(0.6f, 0.4f, 0f, 1f),
		new Color(0.7f, 0.7f, 0.7f, 1f),
		new Color(1f, 0.8f, 0f, 1f)
	};

	private float fontScale = 1f;

	private bool lowVfx;

	private bool lowQual;

	private int frameStart;

	private bool beatT5;

	private Dictionary<int, Sequence> laserSpawnerTweens = new Dictionary<int, Sequence>();

	private List<List<int>> killFloors = new List<List<int>>();

	private List<float> killFloorCheckbeat = new List<float> { 101f, 113f, 125f, 137f };

	private List<int> killFloor2a = new List<int>
	{
		0, 1, 4, 5, 8, 9, 12, 13, 16, 17,
		20, 21, 24, 25, 28, 29
	};

	private List<int> killFloor2b = new List<int>
	{
		2, 3, 6, 7, 10, 11, 14, 15, 18, 19,
		22, 23, 26, 27, 30, 31
	};

	private Sequence unfurl;

	private List<float> angleList = new List<float> { 0f, 0f, -90f, -90f };

	private List<FloorMeshRenderer> animatedFloors = new List<FloorMeshRenderer>();

	private float deadTimer;

	private bool spawnedMarios;

	private int curPoiZoom;

	private List<float> poiZoomBeats = new List<float>();

	private GameObject campos;

	private GameObject camposReal;

	private bool timeTrialFreeCam;

	private bool stoppedFollowmode;

	private float followModeTriggerPosition = 8f;

	private bool toSkyTween;

	private bool darkenSkyTween;

	private bool lightenTween;

	private bool fhs2t;

	private float beat;

	private float drunkVal = 0.3f;

	private bool tweeningMenuBGIntro;

	private bool divineOnTween;

	private bool divineOffTween;

	private bool NCGradientTweening;

	private Vector3 scatter;

	private Vector3 scatter2;

	private float midpointJump;

	private float edir;

	private Color blackClear = new Color(0f, 0f, 0f, 0f);

	private scrPlanet planetThatHit455;

	private Dictionary<LaserSpawner, bool> laserState = new Dictionary<LaserSpawner, bool>();

	private bool timeTrial;

	private bool timeTrialWon;

	private bool timeTrialFailed;

	private float timeTrialTime;

	private float timeTrialTimeUnscaled;

	private float parTime = 13.5f;

	private bool finalEnded;

	private bool finalResultsed;

	private void DisableStuff()
	{
		foreach (Mawaru_Sprite item in stuffToDisable)
		{
			item.render.enabled = false;
		}
	}

	private void JudgeSection(int section, int grade, bool showText = true, int floor = 0)
	{
		JudgeSection(section, grade, showText, scrLevelMaker.instance.listFloors[floor].transform);
	}

	private void JudgeSection(int section, int grade, bool showText, Transform pos)
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && GCS.currentSpeedTrial < 1f))
		{
			return;
		}
		Divine_Stats.sectionStats[section] = grade;
		if (showText && (!RDC.noHud || RDC.partialNoHud))
		{
			Vector3 position = pos.position;
			switch (grade)
			{
			case 3:
				sectionJudgment.text = RDString.Get("mawaru.sectionPurePerfect");
				break;
			case 2:
				sectionJudgment.text = RDString.Get("mawaru.sectionPerfect");
				break;
			case 1:
				sectionJudgment.text = RDString.Get("mawaru.sectionOK");
				break;
			default:
				sectionJudgment.text = "";
				break;
			}
			sectionJudgment.transform.position = position + Vector3.up * 2f;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sectionJudgment.transform.DOScale(Vector3.one * 1.3f, 0f))
				.Append(sectionJudgment.transform.DOScale(Vector3.one, 0.2f * speed).SetEase(Ease.Linear));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sectionJudgment.DOColor(Color.white, 0f))
				.Append(sectionJudgment.DOColor(RankingTextColors[grade], 0.2f * speed).SetEase(Ease.Linear))
				.AppendInterval(0.6f * speed)
				.Append(sectionJudgment.DOColor(RankingTextColors[grade] - Color.black, 1f * speed).SetEase(Ease.Linear));
		}
	}

	private void ShowSectionText(int floor, string text)
	{
		if (!RDC.noHud || RDC.partialNoHud)
		{
			Vector3 position = scrLevelMaker.instance.listFloors[floor].transform.position;
			sectionJudgment.text = text;
			sectionJudgment.transform.position = position + Vector3.up * 2f + Vector3.left * 2f;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sectionJudgment.transform.DOScale(Vector3.one * 1.3f, 0f))
				.Append(sectionJudgment.transform.DOScale(Vector3.one, 0.2f * speed).SetEase(Ease.Linear));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sectionJudgment.DOColor(Color.white, 0f))
				.AppendInterval(0.8f * speed)
				.Append(sectionJudgment.DOColor(whiteClear, 1f * speed).SetEase(Ease.Linear));
		}
	}

	private void Grading()
	{
		for (int i = 0; i < 9; i++)
		{
			if (i == 6)
			{
				continue;
			}
			if (judgingSection == -1)
			{
				if (ADOBase.controller.currFloor.seqID == sectionFloors[i * 2])
				{
					cursection_hasMadeMistakes = false;
					cursection_hasNonPerfects = false;
					judgingSection = i;
				}
			}
			else if (ADOBase.controller.currFloor.seqID == sectionFloors[i * 2 + 1])
			{
				int grade = 3;
				if (cursection_hasNonPerfects)
				{
					grade = 2;
				}
				if (cursection_hasMadeMistakes)
				{
					grade = 1;
				}
				if (i != 6)
				{
					JudgeSection(i, grade, showText: true, sectionFloors[i * 2 + 1]);
				}
				judgingSection = -1;
			}
		}
		if (!judgedFinalSection && ADOBase.controller.currFloor.seqID == sectionFloors[18])
		{
			int grade2 = 3;
			if (ns_hasNonPerfects)
			{
				grade2 = 2;
			}
			if (ns_hasMadeMistakes)
			{
				grade2 = 1;
			}
			JudgeSection(9, grade2, showText: false);
			judgedFinalSection = true;
		}
		if (judgingSection == -1)
		{
			if (ADOBase.controller.currFloor.seqID < 646)
			{
				return;
			}
			if (!ns_hasMadeMistakes && !RDC.auto)
			{
				if (ADOBase.controller.playerOne.missesOnCurrFloor.Count > 0)
				{
					ns_hasMadeMistakes = true;
				}
				if (ADOBase.controller.currFloor.grade == HitMargin.VeryEarly || ADOBase.controller.currFloor.grade == HitMargin.VeryLate || ADOBase.controller.currFloor.grade == HitMargin.TooLate || ADOBase.controller.currFloor.grade == HitMargin.FailMiss || ADOBase.controller.currFloor.grade == HitMargin.FailOverload)
				{
					ns_hasMadeMistakes = true;
				}
			}
			if (!ns_hasNonPerfects && !RDC.auto && (ADOBase.controller.currFloor.grade == HitMargin.EarlyPerfect || ADOBase.controller.currFloor.grade == HitMargin.LatePerfect))
			{
				ns_hasNonPerfects = true;
			}
			return;
		}
		if (!cursection_hasMadeMistakes && !RDC.auto)
		{
			if (ADOBase.controller.playerOne.missesOnCurrFloor.Count > 0)
			{
				cursection_hasMadeMistakes = true;
			}
			if (ADOBase.controller.currFloor.grade == HitMargin.VeryEarly || ADOBase.controller.currFloor.grade == HitMargin.VeryLate || ADOBase.controller.currFloor.grade == HitMargin.TooLate || ADOBase.controller.currFloor.grade == HitMargin.FailMiss || ADOBase.controller.currFloor.grade == HitMargin.FailOverload)
			{
				cursection_hasMadeMistakes = true;
			}
		}
		if (!cursection_hasNonPerfects && !RDC.auto && (ADOBase.controller.currFloor.grade == HitMargin.EarlyPerfect || ADOBase.controller.currFloor.grade == HitMargin.LatePerfect))
		{
			cursection_hasNonPerfects = true;
		}
	}

	private void SetupFonts()
	{
		sectionJudgment.SetLocalizedFont();
		sectionJudgment.fontSize = Mathf.RoundToInt(sectionJudgment.fontSize * fontScale);
	}

	private new void Awake()
	{
		base.Awake();
		frameStart = Time.frameCount;
		GCS.pauseMedalStatsCurrent = Divine_Stats.sectionStats;
		GCS.pauseMedalFloors = new List<int>();
		GCS.pauseMedalFloors.Clear();
		for (int i = 0; i < sectionFloors.Count - 1; i += 2)
		{
			GCS.pauseMedalFloors.Add(sectionFloors[i]);
		}
		GCS.pauseMedalFloors.Add(sectionFloors[sectionFloors.Count - 2]);
		allCover.gameObject.SetActive(value: true);
		lowVfx = ADOBase.controller.visualEffects == VisualEffects.Minimum;
		lowQual = ADOBase.controller.visualQuality == VisualQuality.Low;
		beatT5 = Persistence.IsWorldComplete("T5");
		isLongVersion = !Persistence.IsWorldComplete("T5");
		if (GCS.enableCutsceneT5)
		{
			isLongVersion = true;
		}
		if (GCS.speedTrialMode || GCS.practiceMode)
		{
			isLongVersion = false;
		}
		if (!isLongVersion)
		{
			ADOBase.conductor.song.clip = shortVersion;
			ADOBase.lm.listFloors[456].extraBeats = 17f;
			for (int j = 456; j < 494; j++)
			{
				ADOBase.lm.listFloors[j].isSafe = false;
			}
		}
		longVer = (isLongVersion ? 8f : 0f);
		if (!Divine_Stats.init)
		{
			Divine_Stats.Reset();
		}
		for (int k = 0; k < endingStuff.Count; k++)
		{
			GameObject gameObject = endingStuff[k];
			endingStuff_originalSizes.Add(gameObject.transform.localScale.x);
			gameObject.transform.localScale = Vector3.zero;
		}
		timeTrialEndFloor = GameObject.Find("405-48/Floor freeroam x23 y1").GetComponent<scrFloor>();
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.78f)
		{
			foreach (GameObject item2 in BGsToStretch)
			{
				item2.transform.localScale = new Vector3(item2.transform.localScale.x * (num / 1.7777778f), item2.transform.localScale.y, item2.transform.localScale.z);
			}
		}
		for (int l = 0; l < 5; l++)
		{
			foreach (scrFloor item3 in ADOBase.lm.listFreeroam[l])
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(fallTileWarn, item3.transform);
				gameObject2.transform.localPosition = Vector3.zero;
				floorWarns[item3] = gameObject2.GetComponent<Mawaru_Sprite>();
				floorWarns[item3].render.enabled = false;
				floorWarns[item3].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
			}
		}
		for (int m = 0; m < 6; m++)
		{
			poiZoomBeats.Add(39 + m * 7);
			poiZoomBeats.Add((float)(39 + m * 7) + 1.5f);
			poiZoomBeats.Add((float)(39 + m * 7) + 3.5f);
			poiZoomBeats.Add(39 + m * 7 + 5);
		}
		poiZoomBeats.Add(88f);
		poiZoomBeats.Add(89.5f);
		foreach (LaserSpawner laserSpawner in laserSpawners)
		{
			laserSpawner.floorWarns = floorWarns;
		}
		GameObject gameObject3 = new GameObject("Object Controllers");
		for (int n = 0; n < 8; n++)
		{
			cloudAddX.Add(new GameObject("Cloud Controller").transform);
			cloudAddX[n].transform.SetParent(gameObject3.transform);
			cloudAddX[n].position = Vector3.right * 1f * (n % 2 * -2 + 1);
			cloudDefaultPos.Add(clouds[n].transform.localPosition);
		}
		camRotAux = new GameObject("Cam Rotation Aux");
		camRotAux.transform.SetParent(gameObject3.transform);
		camPulseAux = new GameObject("Cam Pulse Aux");
		camPulseAux.transform.SetParent(gameObject3.transform);
		glare.render.DOColor(whiteClear, 4f);
		foreach (Mawaru_Sprite item4 in bossAll)
		{
			item4.render.enabled = false;
		}
		menuBGcover.render.DOColor(whiteClear, beats(0f));
		lasertile.render.DOColor(whiteClear, beats(0f));
		hotBG.render.DOColor(whiteClear, beats(0f));
		lasertile.render.enabled = false;
		hotBG.render.enabled = false;
		bootlegFloor.localScale = Vector3.zero;
		scrSpike[] array = UnityEngine.Object.FindObjectsByType(typeof(scrSpike), FindObjectsSortMode.None) as scrSpike[];
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			listSpikes.Add(array[num2]);
		}
		elist = new List<Ember>();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 200.0));
		bpms.Add(new Tuple<double, double>(329.0, 180.0));
		bpms.Add(new Tuple<double, double>(341.0, 220.0));
		bpms.Add(new Tuple<double, double>(353.0, 180.0));
		bpms.Add(new Tuple<double, double>(365.0, 220.0));
		bpms.Add(new Tuple<double, double>(378.0, 200.0));
		bpms.Add(new Tuple<double, double>(558.0, 37.5));
		bpms.Add(new Tuple<double, double>(566.0 + (double)longVer, 75.0));
		bpms.Add(new Tuple<double, double>(578.0 + (double)longVer, 150.0));
		bpms.Add(new Tuple<double, double>(650.0 + (double)longVer, 18.75));
		bpms.Add(new Tuple<double, double>(658.5 + (double)longVer, 150.0));
		mb(-199f, DisableStuff, 9999f);
		if (lowQual || GCS.practiceMode)
		{
			mb(-198f, LowBG, 9999f);
		}
		mb(-1f, InitKillFloors, 9999f);
		mb(-1f, SetupTimeTrial, 9999f);
		mb(-200f, SetupFonts, 99999f);
		mb(253f, MoveShurikenTiles);
		mb(254f, MoveShurikenTilesQ);
		mb(16f, FadeMenuBG, 29f);
		mb(30f, FadeMenuBGQ, 999f);
		mb(32f, DivineOn, 39f);
		mb(33f, DivineOnQ, 999f);
		mb(32f, base.BGPersp, 999f);
		mb(158f, DivineOff, 170f);
		mb(171f, DivineOffQ, 999f);
		mb(181f, FadeNCGradient);
		mb(183f, FadeNCGradientQ, 999f);
		mb(249f, FadeMenuBG2);
		mb(252f, SkySetup);
		mb(252.5f, ToSky, 266f);
		mb(266f, ToSkyQ, 999f);
		mb(415f, DarkSkySetup);
		mb(416f, DarkenSky);
		mb(419f, DarkenSkyQ, 999f);
		mb(493f, LightSkySetup, 520f);
		mb(494f, LightenSky, 520f);
		mb(504f, FadeDarkClouds, 520f);
		if (!lowVfx)
		{
			mb(75f, RotateCam);
		}
		mb(-1f, HideFR0, 91f);
		mb(91f, ShowFR0);
		mb(0f, base.SetResultTextPos, 9999f);
		AreaBlocker(13, 16f, 0.5f);
		AreaBlocker(14, 20f, 0.5f);
		AreaBlocker(15, 22f, 0.5f);
		AreaBlocker(17, 24f, 0.25f);
		AreaBlocker(18, 25f, 0.25f);
		AreaBlocker(19, 26f, 0.25f);
		AreaBlocker(16, 27f, 0.25f);
		for (int num3 = 13; num3 < 20; num3++)
		{
			HideBlocker(num3, 31f);
		}
		AreaBlocker(1, 67f);
		AreaBlocker(2, 67f);
		HideBlockerGentle(1, 84f);
		HideBlocker(2, 91f);
		mb(67f, BossSGlare);
		AreaBlocker(21, 252.5f);
		HideBlocker(21, 264.5f);
		AreaBlocker(20, 405f);
		HideBlocker(20, 416f);
		AreaBlocker(7, 422f);
		HideBlocker(7, 427f);
		for (int num4 = 0; num4 < 4; num4++)
		{
			mba(95 + 12 * num4, KillSound, new List<float>
			{
				95 + 12 * num4,
				101 + 12 * num4
			});
			mba(95 + 12 * num4, WarnKillFloor, new List<float> { num4 });
			mba(101 + 12 * num4, AnimateKillFloor, new List<float> { num4 });
			mba(102.5f + (float)(12 * num4), CheckKillFloor, new List<float> { num4 });
		}
		Laser(0, 137f, 143f, 158f);
		MoveLaser(0, 143f, 8f, 0f, 3f, 0f, 5);
		MoveLaser(0, 151f, 7f, 0f, 6f, 0f, 1);
		mba(143f, KillSound, new List<float> { 143f, 148f });
		mba(150f, KillSound, new List<float> { 150f, 155f });
		mb(143f, WarnKillFloor2a);
		mb(148f, AnimateKillFloor2a);
		mb(150f, CheckKillFloor2a);
		mb(150f, WarnKillFloor2b);
		mb(155f, AnimateKillFloor2b);
		mb(157f, CheckKillFloor2b);
		for (int num5 = 0; num5 < 3; num5++)
		{
			Laser(1 + num5, 201 + 16 * num5, 205 + 16 * num5, 217 + 16 * num5);
		}
		MoveLaser(1, 205f, 6f, 3f, 0f, 0f, 5);
		MoveLaser(1, 211f, 6f, 5f, 0f, 0f, 1);
		MoveLaser(2, 221f, 6f, 3f, 0f, 0f, 5);
		MoveLaser(2, 227f, 6f, 6.5f, 0f, 0f, 1);
		MoveLaser(3, 237f, 7f, 3f, 0f, 0f, 5);
		MoveLaser(3, 244f, 5f, 6.5f, 0f, 0f, 1);
		for (int num6 = 0; num6 < 7; num6++)
		{
			mba(274.5f + 7.5f * (float)num6, KickFloors, new List<float> { 189 + 10 * num6 });
			mba(277f + 7.5f * (float)num6, KickFloorsQ, new List<float> { 189 + 10 * num6 }, 999f);
		}
		mb(-99f, FadeOutBossQ, 999f);
		mb(1f, BossInPoi, 260f);
		mb(266.9f, BossInSquare);
		mba(39f, FadeInBossBG, new List<float> { 2f });
		mba(91f, FadeOutBossBG, new List<float> { 3f });
		mba(267f, FadeInBoss, new List<float> { 2f });
		mba(323.5f, FadeOutBoss, new List<float> { 3.5f });
		for (int num7 = 0; num7 < 4; num7++)
		{
			mba(506 + 4 * num7, DestroyFloors, new List<float>
			{
				412 + 6 * num7,
				6f,
				45f
			});
		}
		mba(522f, DestroyFloors, new List<float> { 437f, 7f, -45f });
		mba(526f, DestroyFloors, new List<float> { 444f, 7f, 45f });
		for (int num8 = 0; num8 < 6; num8++)
		{
			Laser(6 + num8, 502 + 4 * num8, 506 + 4 * num8, 508 + 4 * num8);
		}
		if (isLongVersion)
		{
			for (int num9 = 0; num9 < 4; num9++)
			{
				if (num9 == 0)
				{
					Laser(12 + num9, 530f, 534f, 540f, 6f, 0.4f);
				}
				else
				{
					Laser(12 + num9, 530f, 534f, 540f, 0f, 0f);
				}
			}
		}
		AreaBlocker(4, 297f);
		AreaBlocker(5, 297f);
		AreaBlocker(6, 310f);
		mb(297f, BossSGlare);
		for (int num10 = 4; num10 < 7; num10++)
		{
			HideBlocker(num10, 323.5f);
		}
		Laser(4, 434f, 438f, 500f);
		MoveLaser(4, 438f, 12f, 2f, 0f, 0f, 5);
		MoveLaser(4, 450f, 50f, 34f, 0f, 0f, 1);
		mb(323.5f, PlasmaCoverOn, 373f);
		mb(328f, ColdPlasBG);
		mb(340f, HotPlasBG);
		mb(352f, UnHotPlasBG);
		mb(364f, HotPlasBG);
		mb(374f, BackToSkyFromPlasma);
		mb(374f, DriftSkyFrame2, 999f);
		mb(0f, HideLateSpike, 999f);
		mb(371f, SpikeAppear, 388f);
		plasmaBGcover.render.DOColor(whiteClear, 0f);
		plasmaHot.render.DOColor(whiteClear, 0f);
		plasmaCold.render.DOColor(whiteClear, 0f);
		if (!GCS.practiceMode)
		{
			mb(438f, StartCountingTimeTrial);
			mb(498f, StopCountingTimeTrial);
		}
		campos = new GameObject("cam pos");
		camposReal = new GameObject("cam pos (real)");
		mb(502f, RemoveTTText, 999f);
		mb(530f, FadeHopeStuff);
		if (lowVfx)
		{
			mb(534f, DivineOn, 999f);
			mb(534f, FadeDark, 999f);
		}
		else
		{
			mb(534f, DivineOnQ, 999f);
			mb(534f, FadeDarkQ, 999f);
		}
		if (lowVfx)
		{
			mb(534f, FlattenFloors);
		}
		else
		{
			mb(534f, FlattenFloorsQ, 9999f);
		}
		if (isLongVersion)
		{
			mb(536f, BossGlare);
			mb(558f, FadeVillain);
		}
		if (!isLongVersion)
		{
			mb(534f, ShowSnail, 9999f);
		}
		mb(562f + longVer, DisableDrunk, 9999f);
		mb(573f + longVer, LightSkySetup);
		mb(574f + longVer, LightenSky2);
		mb(575f + longVer, LightenSky2Q, 9999f);
		mba(498f, FadeSunbeams, new List<float> { 1f, 8f, 200f });
		mba(524f, FadeSunbeams, new List<float> { 0f, 8f, 200f });
		mba(574f + longVer, FadeSunbeams, new List<float> { 1f, 8f, 200f }, 641f + longVer);
		mba(642f + longVer, FadeSunbeams, new List<float> { 0f, 4f, 150f });
		mb(642f + longVer, DarkenSky2Q, 9999f);
		mb(642f + longVer, FadeHopeStuff2);
		mb(643f + longVer, FadeHopeStuff2Q, 9999f);
		mb(642f + longVer, SpawnBootlegFloor);
		for (int num11 = 451; num11 < 455; num11++)
		{
			animatedFloors.Add(scrLevelMaker.instance.listFloors[num11].gameObject.GetComponent<FloorMeshRenderer>());
		}
		AreaBlocker(8, 653f + longVer);
		AreaBlocker(9, 653f + longVer);
		AreaBlocker(10, 657f + longVer);
		AreaBlocker(11, 657.75f + longVer);
		for (int num12 = 8; num12 < 13; num12++)
		{
			HideBlocker(num12, 658.5f + longVer);
		}
		mpf(658.5f + longVer, FinalEnd, 999f);
		mpf(661f + longVer, ShowFinalResults, 999f);
		mb(649f + longVer, SetupDarken, 999f);
		for (int num13 = 0; num13 < 7; num13++)
		{
			float num14 = (float)num13 / 6f * 0.3f + 0.7f;
			float numBeats = 1f - (float)num13 / 6f * 0.5f;
			mba(650f + longVer + (float)num13, CloseEyes, new List<float>
			{
				beats(1f, 18.75f),
				num14,
				beats(numBeats, 18.75f),
				Mathf.Min(num14 + 0.3f, 1f)
			});
		}
		mba(657f + longVer, OpenEyes, new List<float> { beats(0.75f, 18.75f) });
		if (isLongVersion)
		{
			mb(538f, AddScene1);
			mb(546f, AdvanceText);
			mb(558f, AdvanceText);
			mb(-1000f, PrepareKillPlanet, 9999f);
			mb(534f, KillPlanet, 9999f);
			mb(564f, CharlieTransform);
			mb(564.25f, CharliePlanetGo);
			mb(565f, DestroyDummyPlanets, 9999f);
			mb(565f, RevivePlanet, 9999f);
			mb(558.5f, SpawnCharlie);
			mb(560f, AddScene2);
			mb(561f, AdvanceText);
			mb(563f, AdvanceText);
			mb(565.5f, AdvanceText);
			mb(570f, AddScene3);
			mb(572f, AdvanceText);
			mb(574f, AdvanceText);
			mb(570f, SpawnSef);
			mb(573f, SefTransform);
			mb(573.25f, SefPlanetGo);
			mb(574f, AwakenDummyPlanets, 9999f);
			mb(578f, AddScene4);
			mb(582f, AdvanceText);
			mb(586f, AdvanceText);
			mb(592f, AdvanceText);
			mb(594f, AdvanceText);
			mb(651f + longVer, HandsOn);
			mba(651f + longVer, SetHandsPos, new List<float> { -6.94f, 3.02f, 0f, -6.97f, -2.6399999f, -10f });
			mba(652f + longVer, SetHandsPos, new List<float> { -6.24f, 2.59f, 0f, -6.32f, -2.36f, -10f });
			mba(653f + longVer, SetHandsPos, new List<float> { -5.54f, 2.29f, 0f, -5.54f, -2.13f, -10f });
			mba(654f + longVer, SetHandsPos, new List<float> { -5.03f, 2.06f, -3f, -4.89f, -2.01f, -6f });
			mba(655f + longVer, SetHandsPos, new List<float> { -4.37f, 2.09f, -6f, -4.23f, -1.99f, -3f });
			mba(656f + longVer, SetHandsPos, new List<float> { -3.61f, 1.81f, -10f, -3.61f, -1.76f, 0f });
			mb(657f + longVer, HandsOff);
			mb(-1000f, HandsOff, 999f);
		}
		for (int num15 = 478; num15 < ADOBase.lm.listFloors.Count; num15++)
		{
			ADOBase.lm.listFloors[num15].coll = UnityEngine.Object.Instantiate(ADOBase.gc.collider180, ADOBase.lm.listFloors[num15].transform).GetComponent<Collider2D>();
			((Behaviour)(object)ADOBase.lm.listFloors[num15].coll).enabled = true;
		}
		Laser(5, 584f + longVer, 586f + longVer, 642f + longVer, 6f, 0.3f);
		MoveLaser(5, 586f + longVer, 32f, 31f, 32f, 0f, 1);
		MoveLaser(5, 618f + longVer, 24f, 25f, 27f, 0f, 1);
		SortTables();
		foreach (Mawaru_Sprite ember in embers)
		{
			Ember item = new Ember(ember);
			elist.Add(item);
		}
		GCS.staticPlanetColors = false;
	}

	private void ShowFR0()
	{
		foreach (scrFloor item in ADOBase.lm.listFreeroam[0])
		{
			item.TweenOpacity(1f, beats(2f));
		}
	}

	private void HideFR0()
	{
		foreach (scrFloor item in ADOBase.lm.listFreeroam[0])
		{
			item.TweenOpacity(0f, 0f);
		}
	}

	private void AreaBlocker(int w, float beat, float d = 1f)
	{
		mba(beat, SpawnAreaBlocker, new List<float> { w, d });
	}

	private void HideBlocker(int w, float beat)
	{
		mba(beat, HideAreaBlocker, new List<float> { w });
	}

	private void HideBlockerGentle(int w, float beat)
	{
		mba(beat, HideAreaBlockerGentle, new List<float> { w });
	}

	private void MoveBlocker(int w, float beat, float dur, float x, float y, float z, int tween)
	{
		mba(beat, MoveBlockerInternal, new List<float> { w, dur, x, y, z, tween });
	}

	private void MoveBlockerInternal(List<float> t)
	{
		areaCovers[(int)t[0]].transform.DOLocalMove(new Vector3(t[2], t[3], t[4]), beats(t[1])).SetRelative(isRelative: true).SetEase((Ease)t[5]);
	}

	private void SpawnAreaBlocker(List<float> t)
	{
		areaCovers[(int)t[0]].Spawn(t[1]);
	}

	private void HideAreaBlocker(List<float> t)
	{
		areaCovers[(int)t[0]].Hide();
	}

	private void HideAreaBlockerGentle(List<float> t)
	{
		areaCovers[(int)t[0]].HideGentle();
	}

	private void KillSound(List<float> t)
	{
		float num = t[0] * 60f / 200f;
		float num2 = t[1] * 60f / 200f;
		float num3 = (t[1] + 1.5f) * 60f / 200f;
		CueSound((double)num / (double)ADOBase.conductor.song.pitch, "DangerTileWarning");
		CueSound((double)num2 / (double)ADOBase.conductor.song.pitch, "DangerTilePreSpin");
		CueSound((double)num3 / (double)ADOBase.conductor.song.pitch, "DangerTileSpin");
	}

	private void CueSound(double time, string s)
	{
		double num = ADOBase.conductor.dspTimeSongPosZero + time + ADOBase.conductor.crotchetAtStart / (double)ADOBase.controller.currFloor.speed * (double)ADOBase.conductor.adjustedCountdownTicks;
		AudioMixerGroup mixerGroup = RDUtils.GetMixerGroup("ConductorPlaySound");
		int num2 = 0;
		AudioManager.Play("snd" + s, num - (double)num2, mixerGroup, 0.6f);
	}

	private void Laser(int w, float p, float f, float e, float shakeDur = 2f, float shakeAmt = 0.2f)
	{
		mba(p, PrepareLaser, new List<float>
		{
			w,
			f - p
		}, f);
		mba(f, FireLaser, new List<float>
		{
			w,
			beats(shakeDur),
			shakeAmt
		});
		mba(e, EndLaser, new List<float> { w });
	}

	private void MoveLaser(int w, float beat, float dur, float x, float y, float z, int tween)
	{
		mba(beat, MoveLaserInternal, new List<float> { w, dur, x, y, z, tween });
	}

	private void MoveLaserInternal(List<float> t)
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			Sequence sequence = DOTween.Sequence().Append(laserSpawners[(int)t[0]].transform.DOLocalMove(new Vector3(t[2], t[3], t[4]), beats(t[1])).SetRelative(isRelative: true).SetEase((Ease)t[5]));
			laserSpawnerTweens[(int)t[0]] = sequence;
			laserSpawners[(int)t[0]].moveAnim = sequence;
		}
	}

	private void MoveBoss(float beat, float dur, float x, float y, float z, int tween)
	{
		mba(beat, MoveBossInternal, new List<float> { dur, x, y, z, tween });
	}

	private void MoveBossInternal(List<float> t)
	{
		bossIdle.transform.DOLocalMove(new Vector3(t[1], t[2], t[3]), beats(t[0])).SetRelative(isRelative: true).SetEase((Ease)t[4]);
	}

	private void PrepareLaser(List<float> w)
	{
		if (w.Count > 1)
		{
			laserSpawners[(int)w[0]].Prepare(beats(w[1]));
		}
		else
		{
			laserSpawners[(int)w[0]].Prepare(beats(4f));
		}
	}

	private void FireLaser(List<float> w)
	{
		if (w[1] > 0f && w[2] > 0f && !lowVfx)
		{
			ADOBase.controller.ScreenShake(beats(w[1]), w[2]);
		}
		laserSpawners[(int)w[0]].Fire();
	}

	private void EndLaser(List<float> w)
	{
		laserSpawners[(int)w[0]].End(beats(8f));
	}

	private void InitKillFloors()
	{
		killFloors.Add(new List<int> { 0, 1, 4, 5 });
		killFloors.Add(new List<int> { 8, 9, 10, 11, 12, 13, 14, 15 });
		killFloors.Add(new List<int> { 0, 1, 2, 3, 4, 5, 8, 9, 12, 13 });
		killFloors.Add(new List<int>
		{
			0, 3, 4, 7, 8, 9, 10, 11, 12, 13,
			14, 15
		});
	}

	private bool isKillFloor(scrFloor fl, int fra)
	{
		foreach (int item in killFloors[fra])
		{
			if (fl == ADOBase.lm.listFreeroam[fra][item])
			{
				return true;
			}
		}
		return false;
	}

	private bool isKillFloor2a(scrFloor fl)
	{
		foreach (int item in killFloor2a)
		{
			if (fl == ADOBase.lm.listFreeroam[4][item])
			{
				return true;
			}
		}
		return false;
	}

	private bool isKillFloor2b(scrFloor fl)
	{
		foreach (int item in killFloor2b)
		{
			if (fl == ADOBase.lm.listFreeroam[4][item])
			{
				return true;
			}
		}
		return false;
	}

	private void WarnKillFloor(List<float> l)
	{
		int index = (int)l[0];
		List<int> list = killFloors[index];
		if (!(Math.Abs(songBeat - (double)(killFloorCheckbeat[index] - 5f)) < 1.0))
		{
			return;
		}
		foreach (int item in list)
		{
			scrFloor key = ADOBase.lm.listFreeroam[index][item];
			Mawaru_Sprite w = floorWarns[key];
			w.render.enabled = true;
			DOTween.Sequence().Append(w.render.material.DOColor(Color.white, beats(1f)).SetEase(Ease.Linear)).Append(w.render.material.DOColor(whiteClear, beats(5f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				w.render.enabled = false;
			}));
			DOTween.Sequence().Append(w.transform.DOScale(Vector3.one * 0.8f, beats(0f)).SetEase(Ease.Linear)).Append(w.transform.DOScale(Vector3.one * 1f, beats(1f)).SetEase(Ease.Linear))
				.Append(w.transform.DOScale(Vector3.one * 0.7f, beats(5f)).SetEase(Ease.Linear));
		}
	}

	private void CheckKillFloor(List<float> l)
	{
		int num = (int)l[0];
		if (Math.Abs(songBeat - (double)(killFloorCheckbeat[num] + 1.5f)) < 1.0 && ADOBase.controller.curFreeRoamSection == num && ADOBase.controller.currFloor.freeroamGenerated && !RDC.auto && isKillFloor(ADOBase.controller.currFloor, num))
		{
			ADOBase.controller.playerOne.DieByHitbox();
		}
	}

	private void AnimateKillFloor(List<float> l)
	{
		int index = (int)l[0];
		List<int> list = killFloors[index];
		if (!(Math.Abs(songBeat - (double)killFloorCheckbeat[index]) < 1.0))
		{
			return;
		}
		foreach (int item in list)
		{
			scrFloor scrFloor2 = ADOBase.lm.listFreeroam[index][item];
			scrFloor2.TweenColor(new Color(1f, 0f, 0f, 1f), beats(1f), Ease.Linear, beats(0.5f));
			scrFloor2.TweenColor(new Color(1f, 1f, 1f, 1f), beats(1f), Ease.Linear, beats(1.5f));
			DOTween.Sequence().Append(scrFloor2.transform.DORotate(Vector3.forward * -20f, beats(0.5f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).Append(scrFloor2.transform.DORotate(Vector3.forward * 380f, beats(2f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.InOutExpo));
		}
	}

	private void WarnKillFloor2a()
	{
		foreach (int item in killFloor2a)
		{
			scrFloor key = ADOBase.lm.listFreeroam[4][item];
			Mawaru_Sprite w = floorWarns[key];
			w.render.enabled = true;
			DOTween.Sequence().Append(w.render.material.DOColor(Color.white, beats(1f)).SetEase(Ease.Linear)).Append(w.render.material.DOColor(whiteClear, beats(5f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				w.render.enabled = false;
			}));
			DOTween.Sequence().Append(w.transform.DOScale(Vector3.one * 0.8f, beats(0f)).SetEase(Ease.Linear)).Append(w.transform.DOScale(Vector3.one * 1f, beats(1f)).SetEase(Ease.Linear))
				.Append(w.transform.DOScale(Vector3.one * 0.7f, beats(5f)).SetEase(Ease.Linear));
		}
	}

	private void CheckKillFloor2a()
	{
		if (ADOBase.controller.currFloor.freeroamGenerated && isKillFloor2a(ADOBase.controller.currFloor))
		{
			ADOBase.controller.playerOne.Die();
		}
	}

	private void AnimateKillFloor2a()
	{
		foreach (int item in killFloor2a)
		{
			scrFloor scrFloor2 = ADOBase.lm.listFreeroam[4][item];
			scrFloor2.TweenColor(new Color(1f, 0f, 0f, 1f), beats(1f), Ease.Linear, beats(0.5f));
			scrFloor2.TweenColor(new Color(1f, 1f, 1f, 1f), beats(1f), Ease.Linear, beats(1.5f));
			DOTween.Sequence().Append(scrFloor2.transform.DORotate(Vector3.forward * -20f, beats(0.5f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).Append(scrFloor2.transform.DORotate(Vector3.forward * 380f, beats(2f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.InOutExpo));
		}
	}

	private void WarnKillFloor2b()
	{
		foreach (int item in killFloor2b)
		{
			scrFloor key = ADOBase.lm.listFreeroam[4][item];
			Mawaru_Sprite w = floorWarns[key];
			w.render.enabled = true;
			DOTween.Sequence().Append(w.render.material.DOColor(Color.white, beats(1f)).SetEase(Ease.Linear)).Append(w.render.material.DOColor(whiteClear, beats(5f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				w.render.enabled = false;
			}));
			DOTween.Sequence().Append(w.transform.DOScale(Vector3.one * 0.8f, beats(0f)).SetEase(Ease.Linear)).Append(w.transform.DOScale(Vector3.one * 1f, beats(1f)).SetEase(Ease.Linear))
				.Append(w.transform.DOScale(Vector3.one * 0.7f, beats(5f)).SetEase(Ease.Linear));
		}
	}

	private void CheckKillFloor2b()
	{
		if (ADOBase.controller.currFloor.freeroamGenerated && isKillFloor2b(ADOBase.controller.currFloor))
		{
			ADOBase.controller.playerOne.Die();
		}
	}

	private void AnimateKillFloor2b()
	{
		foreach (int item in killFloor2b)
		{
			scrFloor scrFloor2 = ADOBase.lm.listFreeroam[4][item];
			scrFloor2.TweenColor(new Color(1f, 0f, 0f, 1f), beats(1f), Ease.Linear, beats(0.5f));
			scrFloor2.TweenColor(new Color(1f, 1f, 1f, 1f), beats(1f), Ease.Linear, beats(1.5f));
			DOTween.Sequence().Append(scrFloor2.transform.DORotate(Vector3.forward * -20f, beats(0.5f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).Append(scrFloor2.transform.DORotate(Vector3.forward * 380f, beats(2f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.InOutExpo));
		}
	}

	private void FlattenFloors()
	{
		float[] array = new float[4];
		for (int i = 0; i < angleList.Count; i++)
		{
			int j = i;
			float duration = beats(4f - (float)i * 0.5f);
			unfurl = DOTween.Sequence().AppendInterval(beats(0.5f * (float)j)).Append(DOTween.To(() => angleList[j], delegate(float x)
			{
				angleList[j] = x;
			}, array[j], duration).SetEase(Ease.InOutCubic));
		}
		if (!isLongVersion)
		{
			return;
		}
		foreach (Mawaru_Sprite item in bossAll)
		{
			item.render.enabled = true;
		}
		FadeStuff(bossAll, 1f, beats(4f, 200f));
	}

	private void FlattenFloorsQ()
	{
		unfurl.Kill();
		for (int i = 0; i < angleList.Count; i++)
		{
			angleList[i] = 0f;
		}
		UpdateFloorAnglesFromBack(animatedFloors, angleList);
		CacheFloorPosition(451, 482);
		if (!isLongVersion)
		{
			return;
		}
		foreach (Mawaru_Sprite item in bossAll)
		{
			item.render.enabled = true;
		}
	}

	private void RotateCam()
	{
		scrVfxPlus.instance.camAngle = 90f;
		DOTween.Sequence().Append(camRotAux.transform.DOLocalMoveZ(18f, beats(1f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).AppendInterval(beats(0.5f))
			.Append(camRotAux.transform.DOLocalMoveZ(18f, beats(1f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(camRotAux.transform.DOLocalMoveZ(18f, beats(1f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(camRotAux.transform.DOLocalMoveZ(18f, beats(1f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(camRotAux.transform.DOLocalMoveZ(18f, beats(1f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		DOTween.Sequence().Append(acParent1.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).AppendInterval(beats(0.5f))
			.Append(acParent1.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(acParent1.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(acParent1.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(acParent1.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		DOTween.Sequence().Append(lasertile.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).AppendInterval(beats(0.5f))
			.Append(lasertile.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(lasertile.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(lasertile.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
			.AppendInterval(beats(0.5f))
			.Append(lasertile.transform.DORotate(Vector3.forward * 18f, beats(1f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
	}

	private void FadeAllCover()
	{
		DOTween.Sequence().AppendInterval(0f).Append(allCover.DOColor(new Color(0f, 0f, 0f, 0f), 0.1f).SetEase(Ease.Linear).OnComplete(delegate
		{
			allCover.enabled = false;
		}));
	}

	private new void Update()
	{
		base.Update();
		int num = Time.frameCount - frameStart;
		if (num == 5)
		{
			FadeAllCover();
		}
		if (num < 5)
		{
			followModeTriggerPosition = 8f * (Camera.main.aspect / 1.77777f);
		}
		if (songBeat > 38.0 && songBeat < 92.0)
		{
			ADOBase.controller.strictHolds = false;
		}
		else
		{
			ADOBase.controller.strictHolds = ADOBase.controller.strictHoldsSaved;
		}
		Grading();
		if (RDC.debug && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
		{
			GCS.enableCutsceneT5 = true;
			RDC.customCheckpointPos = 455;
			RDC.customCheckpoint = true;
			scrController.instance.Restart();
		}
		if (ADOBase.controller.currentState == States.Start)
		{
			scrCamera.instance.timer = 0f;
		}
		if (timeTrial && !timeTrialWon && !timeTrialFailed && ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			timeTrialTime += Time.deltaTime * ADOBase.conductor.song.pitch;
			timeTrialTimeUnscaled += Time.deltaTime;
			foreach (LaserSpawner laserSpawner in laserSpawners)
			{
				if (laserSpawner.killedPlanet != laserState[laserSpawner])
				{
					timeTrialFailed = true;
				}
			}
		}
		else if (timeTrialWon && !timeTrialFailed)
		{
			if (timeTrialTime < parTime)
			{
				timerText.DOColor(new Color(1f, 0.8f, 0f, 1f), 0f);
			}
			else
			{
				timerText.DOColor(new Color(0.3f, 0.6f, 1f, 1f), 0f);
			}
		}
		else if (timeTrialFailed || ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			timerText.DOColor(new Color(0.6f, 0f, 0f, 1f), 0f);
		}
		if (!base.camy.followMode && !stoppedFollowmode && (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2))
		{
			stoppedFollowmode = true;
			ADOBase.controller.MoveCameraToObject(camposReal, 0f, Ease.InOutCubic);
		}
		camposReal.transform.position = new Vector3(base.camy.transform.position.x, base.camy.transform.position.y, 0f);
		if (timeTrial && !timeTrialFreeCam && ADOBase.controller.chosenPlanet.transform.position.x >= base.camy.transform.position.x + followModeTriggerPosition)
		{
			timeTrialFreeCam = true;
			ADOBase.controller.MoveCameraToObject(camposReal, 0f, Ease.InOutCubic);
			ADOBase.controller.MoveCameraToPlayer(beats(8f), Ease.InOutCubic);
		}
		if (ADOBase.controller.currFloor == timeTrialEndFloor && timeTrial && !timeTrialWon)
		{
			int grade = 2;
			if (timeTrialTime < parTime)
			{
				grade = 3;
			}
			if (timeTrialFailed)
			{
				grade = 1;
			}
			campos.transform.position = new Vector3(base.camy.transform.position.x + 3f, base.camy.transform.position.y + 2f, 0f);
			if (timeTrialFailed)
			{
				goalTimeText.text = $"= {parTime:#,0.000}";
				goalTime.transform.localScale = Vector3.one;
			}
			JudgeSection(6, grade, showText: true, campos.transform);
			timeTrialWon = true;
			timeTrial = false;
			if (!timeTrialFailed)
			{
				Divine_Stats.timeTrialTime = timeTrialTime;
			}
			if (!timeTrialFailed && timeTrialTime < Divine_Stats.bestTimeTrialTime)
			{
				if (timeTrialTime < parTime)
				{
					myBestText.DOColor(new Color(1f, 0.8f, 0f, 1f), 0f);
				}
				else
				{
					myBestText.DOColor(new Color(0.3f, 0.6f, 1f, 1f), 0f);
				}
				Divine_Stats.bestTimeTrialTime = timeTrialTime;
			}
		}
		if (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			deadTimer += Time.deltaTime;
			if ((double)deadTimer > 1.5)
			{
				for (int i = 0; i < laserSpawners.Count; i++)
				{
					if (laserSpawners[i].active)
					{
						if (laserSpawnerTweens.ContainsKey(i))
						{
							laserSpawnerTweens[i].Kill();
						}
						laserSpawners[i].End(beats(8f));
					}
				}
			}
		}
		if (isLongVersion && ADOBase.controller.currentState == States.Start && songBeat > 565.0)
		{
			RevivePlanet();
		}
		if (isLongVersion && songBeat > 594.0 && !spawnedMarios && ADOBase.controller.currentState == States.Fail2)
		{
			spawnedMarios = true;
			charlie_mario.transform.localScale = Vector3.one * 0.7f;
			charlie_mario.transform.position = ADOBase.controller.chosenPlanet.transform.position;
			charlie_mario.transform.DOLocalMoveX(-1f, 1f).SetRelative(isRelative: true).SetEase(Ease.OutCubic);
			DOTween.Sequence().Append(charlie_mario.transform.DOLocalMoveY(1.5f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.OutCubic)
				.OnComplete(delegate
				{
					charlie_mario.SetState(1);
				})).Append(charlie_mario.transform.DOLocalMoveY(-14f, 0.75f).SetRelative(isRelative: true).SetEase(Ease.InCubic));
			DOTween.Sequence().Append(charlie_mario.render.material.DOColor(new Color(1f, 0.4f, 0.4f, 1f), "_Glow", 0f)).Append(charlie_mario.render.material.DOColor(new Color(1f, 0.4f, 0.4f, 0f), "_Glow", 0.5f));
			if (ADOBase.controller.currFloor.numPlanets == 3)
			{
				sef_mario.transform.localScale = Vector3.one * 0.7f;
				sef_mario.transform.position = ADOBase.controller.chosenPlanet.transform.position;
				sef_mario.transform.DOLocalMove(new Vector3(3.5f, 1.5f, 0f), 2f).SetRelative(isRelative: true).SetEase(Ease.Linear);
				DOTween.Sequence().AppendInterval(1.6f).Append(sef_mario.transform.DOScale(new Vector3(0.7f, 0f, 1f), 0.4f).SetEase(Ease.InBack));
				DOTween.Sequence().Append(sef_mario.render.material.DOColor(new Color(1f, 0.8f, 0f, 1f), "_Glow", 0f)).Append(sef_mario.render.material.DOColor(new Color(1f, 0.8f, 0f, 0f), "_Glow", 0.5f));
			}
		}
		if (ADOBase.controller.currFloor == ADOBase.lm.listFloors[455])
		{
			planetThatHit455 = ADOBase.controller.chosenPlanet;
		}
		if ((beatT5 && timeTrial) || timeTrialWon)
		{
			if (Mathf.Abs(timeTrialTimeUnscaled - timeTrialTime) > 0.01f)
			{
				timerText.text = $"{timeTrialTime:#,0.000} ({timeTrialTimeUnscaled:#,0.000})";
			}
			else
			{
				timerText.text = $"{timeTrialTime:#,0.000}";
			}
		}
		else
		{
			timerText.text = "";
		}
		if ((timeTrial || timeTrialWon) && Divine_Stats.bestTimeTrialTime < 60f)
		{
			myBestText.text = string.Format("{0} {1:#,0.000}", RDString.Get("t5.bestTime"), Divine_Stats.bestTimeTrialTime);
		}
		else
		{
			myBestText.text = "";
		}
		float num2 = (float)songBeat;
		SpikeCollisions();
		if (num2 > 534f)
		{
			AnimateBoss();
		}
		sunbeamParents[0].localEulerAngles = Vector3.forward * (-65f + 6f * Mathf.Sin(Time.time * 0.11f * (float)Math.PI));
		sunbeamParents[1].localEulerAngles = Vector3.forward * (-60f + 5f * Mathf.Sin(Time.time * 0.06f * (float)Math.PI));
		sunbeamParents[2].localEulerAngles = Vector3.forward * (-70f + 4f * Mathf.Sin(Time.time * 0.07f * (float)Math.PI));
		if ((num2 > 32f && num2 < 172f) || (num2 > 534f && num2 < 600f))
		{
			AnimateEmbers(spawningMore: true);
		}
		else
		{
			AnimateEmbers(spawningMore: false);
		}
		if (num2 > 530f && num2 < 534f)
		{
			UpdateFloorAnglesFromBack(animatedFloors, angleList);
		}
		if (num2 > 534f && (double)num2 < 583.9)
		{
			Drunk();
		}
		if (num2 > 39f && num2 < 91f && !lowVfx)
		{
			base.camy.transform.eulerAngles = camRotAux.transform.position;
			if (curPoiZoom < poiZoomBeats.Count && num2 > poiZoomBeats[curPoiZoom])
			{
				if (num2 < poiZoomBeats[curPoiZoom] + 1f)
				{
					DOTween.Sequence().Append(camPulseAux.transform.DOMoveX(-0.08f, 0f)).Append(camPulseAux.transform.DOMoveX(0f, beats(0.5f)).SetEase(Ease.OutSine));
					DOTween.Sequence().Append(lasertile.render.DOColor(new Color(1f, 1f, 1f, 0.7058824f), 0.1f).SetEase(Ease.Linear)).Append(lasertile.render.DOColor(new Color(1f, 1f, 1f, 0.22745098f), beats(1.5f)).SetEase(Ease.OutSine));
				}
				curPoiZoom++;
			}
			base.camy.transform.eulerAngles = camRotAux.transform.position;
			base.camy.zoomSize = 1.2f + camPulseAux.transform.position.x;
		}
		for (int num3 = 0; num3 < 2; num3++)
		{
			bossEndHands[num3].transform.localPosition = Vector3.right * 0.15f * Mathf.Sin((float)num3 / 4f * 2f * (float)Math.PI + Time.time * 0.3f * (float)Math.PI) + Vector3.up * 0.1f * Mathf.Cos((float)num3 / 4f * 2f * (float)Math.PI + Time.time * 0.3f * (float)Math.PI);
			bossEndHands[num3].transform.localEulerAngles = Vector3.forward * 4f * Mathf.Sin((float)num3 / 4f * 2f * (float)Math.PI + Time.time * 0.3f * (float)Math.PI);
		}
		for (int num4 = 0; num4 < 8; num4++)
		{
			clouds[num4].transform.localPosition = cloudDefaultPos[num4] + cloudAddX[num4].position + Vector3.right * 0.015f * Mathf.Sin((float)num4 / 4f * 2f * (float)Math.PI + Time.time * 0.4f * (float)Math.PI) + Vector3.up * 0.01f * Mathf.Cos((float)num4 / 4f * 2f * (float)Math.PI + Time.time * 0.4f * (float)Math.PI);
			clouds[num4].transform.eulerAngles = Vector3.forward * 1f * Mathf.Sin((float)num4 / 4f * 2f * (float)Math.PI + Time.time * 0.4f * (float)Math.PI);
		}
	}

	private void SkySetup()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			EnableStuff(clearsky_bgstuff);
		}
	}

	private void DarkSkySetup()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			EnableStuff(cloudy_bgstuff);
			EnableStuff(darkClouds);
		}
	}

	private void LightSkySetup()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			EnableStuff(hope_bgstuff);
			EnableStuff(lightClouds);
		}
	}

	private void ToSky()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			toSkyTween = true;
			FadeStuff(clearsky_bgstuff, 1f, beats(14.5f, 200f));
		}
	}

	private void ToSkyQ()
	{
		if (!GCS.practiceMode && !lowQual && !toSkyTween)
		{
			SkySetup();
			FadeStuff(clearsky_bgstuff, 1f, 0f);
		}
	}

	private void DarkenSky()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			darkenSkyTween = true;
			for (int i = 0; i < 8; i++)
			{
				Transform target = cloudAddX[i];
				DOTween.Sequence().AppendInterval(beats(8f, 200f)).Append(target.DOLocalMoveX(0f, beats(12f, 200f)).SetEase(Ease.InOutCubic));
			}
			FadeStuff(cloudy_bgstuff, 1f, beats(16f, 200f));
			FadeStuff(darkClouds, 1f, beats(8f, 200f));
		}
	}

	private void DarkenSkyQ()
	{
		if (!GCS.practiceMode && !lowQual && !darkenSkyTween)
		{
			DarkSkySetup();
			for (int i = 0; i < 8; i++)
			{
				cloudAddX[i].DOLocalMoveX(0f, 0f);
			}
			FadeStuff(cloudy_bgstuff, 1f, 0f);
			FadeStuff(darkClouds, 1f, 0f);
		}
	}

	private void DarkenSky2Q()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			DarkSkySetup();
			for (int i = 0; i < 8; i++)
			{
				cloudAddX[i].DOLocalMoveX(0f, 0f);
			}
			FadeStuff(cloudy_bgstuff, 1f, 0f);
		}
	}

	private void LightenSky()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			FadeStuff(hope_bgstuff, 1f, beats(16f, 200f));
			FadeStuff(lightClouds, 1f, beats(16f, 200f));
		}
	}

	private void LightenSky2()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			lightenTween = true;
			FadeStuff(hope_bgstuff, 1f, beats(16f, 150f));
			FadeStuff(lightClouds, 1f, beats(16f, 150f));
		}
	}

	private void LightenSky2Q()
	{
		if (!GCS.practiceMode && !lowQual && !lightenTween)
		{
			LightSkySetup();
			FadeStuff(hope_bgstuff, 1f, 0f);
			FadeStuff(lightClouds, 1f, 0f);
		}
	}

	private void FadeDarkClouds()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			FadeStuff(darkClouds, 0f, beats(8f, 200f));
		}
	}

	private void FadeDark()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			FadeStuff(darkClouds, 0f, beats(4f, 200f));
			FadeStuff(cloudy_bgstuff, 0f, beats(4f, 200f));
			FadeStuff(clearsky_bgstuff, 0f, beats(4f, 200f));
		}
	}

	private void FadeDarkQ()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			FadeStuff(darkClouds, 0f, 0f);
			FadeStuff(cloudy_bgstuff, 0f, 0f);
			FadeStuff(clearsky_bgstuff, 0f, 0f);
		}
	}

	private void FadeHopeStuff()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			FadeStuff(hope_bgstuff, 0f, beats(4f, 200f));
			FadeStuff(lightClouds, 0f, beats(4f, 200f));
		}
	}

	private void FadeHopeStuff2()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			fhs2t = true;
			FadeStuff(hope_bgstuff, 0f, beats(8f, 150f));
			FadeStuff(lightClouds, 0f, beats(8f, 150f));
			FadeStuff(darkClouds, 1f, beats(8f, 150f));
		}
	}

	private void FadeHopeStuff2Q()
	{
		if (!GCS.practiceMode && !lowQual && !fhs2t)
		{
			FadeStuff(hope_bgstuff, 0f, 0f);
			FadeStuff(lightClouds, 0f, 0f);
			FadeStuff(darkClouds, 1f, 0f);
		}
	}

	private void FadeSunbeams(List<float> t)
	{
		if (!GCS.practiceMode && !lowQual)
		{
			if (t[0] > 0f)
			{
				EnableStuff(sunbeams);
			}
			FadeStuff(sunbeams, t[0], beats(t[1], t[2]));
		}
	}

	private void FadeVillain()
	{
		FadeStuff(bossAll, 0f, beats(4f, 200f));
	}

	private void HitBoss()
	{
		bossIdle.Hit();
	}

	private void FunnyText()
	{
		Debug.Log("Yee haw!");
	}

	private void AnimateBoss()
	{
		bossBody.transform.localPosition = Vector3.right * 1.6f + Vector3.up * (1.746f + 0.2f * Mathf.Sin(Time.time * 0.9f)) + Vector3.forward * 35f;
		bossHand1.transform.localPosition = Vector3.right * -3.5f + Vector3.up * (-0.25f + 0.25f * Mathf.Sin(Time.time * 1f)) + Vector3.forward * 35f;
		bossHand2.transform.localPosition = Vector3.right * 17f + Vector3.up * (-0.4f + -0.25f * Mathf.Sin(Time.time * 1f)) + Vector3.forward * 35f;
		glare.transform.localPosition = Vector3.right * 0.256f + Vector3.up * (6.41f + 0.2f * Mathf.Sin(Time.time * 0.9f)) + Vector3.forward * 35f;
	}

	private void AnimateEmbers(bool spawningMore)
	{
		if (GCS.practiceMode || lowQual)
		{
			return;
		}
		if (spawningMore)
		{
			etimer -= Time.deltaTime;
		}
		if (etimer < 0f)
		{
			etimer += edelay;
			Ember ember = elist[curEmber];
			ember.spawnTime = Time.time;
			ember.maxLife = 3.5f;
			ember.lifeTime = ember.maxLife;
			ember.randScale = UnityEngine.Random.Range(1f, 1.5f);
			ember.emb.transform.localScale = squish * ember.randScale;
			ember.xspeed = UnityEngine.Random.Range(0.3f, 0.6f);
			ember.yspeed = UnityEngine.Random.Range(1.8f, 2.5f);
			ember.xwaveSize = UnityEngine.Random.Range(0.6f, 0.8f);
			ember.xwavePeriod = UnityEngine.Random.Range(0.4f, 0.8f);
			ember.xwaveOffset = UnityEngine.Random.Range(0f, 1f);
			ember.alive = true;
			ember.emb.render.enabled = true;
			ember.emb.transform.localPosition = Vector3.up * -8f + Vector3.forward * 10f + Vector3.right * UnityEngine.Random.Range(-10f, 10f);
			curEmber = (curEmber + 1) % elist.Count;
		}
		foreach (Ember item in elist)
		{
			if (item.alive)
			{
				item.lifeTime -= Time.deltaTime;
				item.emb.render.color = new Color(1f, 1f, 1f, item.lifeTime / item.maxLife);
				float num = item.yspeed * Time.deltaTime;
				float num2 = item.xspeed * Time.deltaTime + item.xwaveSize * Mathf.Sin(Time.time * (float)Math.PI * 2f * item.xwavePeriod + item.xwaveOffset * (float)Math.PI * 2f) * Time.deltaTime;
				item.emb.transform.localPosition += Vector3.right * num2;
				item.emb.transform.localPosition += Vector3.up * num;
				item.emb.transform.localEulerAngles = Vector3.forward * (57.29578f * Mathf.Atan2(num, num2) - 90f);
				item.emb.transform.localScale = squish * item.randScale * (0.5f + 0.5f * item.lifeTime / item.maxLife);
				if (item.lifeTime < 0f)
				{
					item.alive = false;
					item.emb.render.enabled = false;
				}
			}
		}
	}

	private void DisableDrunk()
	{
		DOTween.To(() => drunkVal, delegate(float x)
		{
			drunkVal = x;
		}, 0.1f, beats(4f, 37.5f)).SetEase(Ease.InOutCubic);
	}

	private void Drunk()
	{
		beat = (float)songBeat;
		for (int i = 451; i < 483; i++)
		{
			scrFloor floor = scrLevelMaker.instance.listFloors[i];
			FloorResetPosition(floor);
			FloorDrunkVibe(floor, drunkVal, 0f, 6f);
		}
	}

	private void BossGlare()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.8f)
			.Append(glare.render.DOColor(Color.white, 0.06f).SetEase(Ease.InQuad))
			.Append(glare.render.DOColor(whiteClear, 4f).SetEase(Ease.OutQuad));
	}

	private void BossSGlare()
	{
		bossIdle.Glare();
	}

	private void UpdateFloorAngles(List<FloorMeshRenderer> fm, List<float> anglesToSet, float initAngle = 0f)
	{
		float[] array = new float[anglesToSet.Count];
		float num = initAngle;
		for (int i = 0; i < anglesToSet.Count; i++)
		{
			float num2 = anglesToSet[i] + num;
			num += anglesToSet[i];
			if (num2 < 0f)
			{
				num2 += 360f;
			}
			if (num2 > 360f)
			{
				num2 -= 360f;
			}
			array[i] = num2;
		}
		int num3 = array.Length;
		float num4 = 1.5f;
		float num5 = initAngle;
		Vector2 vector = default(Vector2);
		for (int j = 0; j < num3; j++)
		{
			FloorMesh floorMesh = fm[j].floorMesh;
			float f = (float)Math.PI / 180f * num5;
			if (j > 0)
			{
				floorMesh.transform.localPosition = new Vector3(vector.x + Mathf.Cos(f) * num4, vector.y + Mathf.Sin(f) * num4, (float)j * 0.01f);
			}
			floorMesh._angle0 = num5 - 180f;
			floorMesh._angle1 = array[j];
			vector = floorMesh.transform.localPosition;
			if (j != num3)
			{
				num5 = floorMesh._angle1;
			}
		}
	}

	private void UpdateFloorAnglesFromBack(List<FloorMeshRenderer> fm, List<float> anglesToSet, float initAngle = 0f)
	{
		float[] array = new float[anglesToSet.Count];
		float num = initAngle;
		for (int num2 = anglesToSet.Count - 1; num2 >= 0; num2--)
		{
			float num3 = anglesToSet[num2] + num;
			num += anglesToSet[num2];
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			if (num3 > 360f)
			{
				num3 -= 360f;
			}
			array[num2] = num3;
		}
		int num4 = array.Length;
		float num5 = 1.5f;
		float num6 = initAngle;
		Vector2 vector = default(Vector2);
		FloorMesh floorMesh = fm[0].floorMesh;
		for (int num7 = num4 - 1; num7 >= 0; num7--)
		{
			FloorMesh floorMesh2 = fm[num7].floorMesh;
			float f = (float)Math.PI / 180f * num6;
			if (num7 < num4 - 1)
			{
				floorMesh2.transform.localPosition = floorMesh.transform.localPosition + new Vector3(vector.x - Mathf.Cos(f) * num5, vector.y + Mathf.Sin(f) * num5, (float)num7 * 0.01f);
			}
			floorMesh2._angle1 = 0f - num6;
			floorMesh2._angle0 = 0f - (array[num7] - 180f);
			vector = floorMesh2.transform.localPosition - floorMesh.transform.localPosition;
			if (num7 != num4)
			{
				num6 = array[num7];
			}
		}
	}

	private void SpikeCollisions()
	{
		foreach (scrSpike listSpike in listSpikes)
		{
			if (ADOBase.controller.currentState != States.PlayerControl)
			{
				continue;
			}
			for (int i = 0; i < ADOBase.controller.planetarySystem.planetList.Count; i++)
			{
				scrPlanet scrPlanet2 = ADOBase.controller.planetarySystem.planetList[i];
				if (!listSpike.hit && listSpike.hittable && Vector3.Distance(scrPlanet2.transform.position, listSpike.pos) < 0.6f && !RDC.auto && scrPlanet2.iFrames <= 0f)
				{
					if (listSpike.hitTime[i] < 0.03f)
					{
						listSpike.hitTime[i] += Time.deltaTime;
						continue;
					}
					listSpike.hit = true;
					if (!ADOBase.controller.freeroamInvulnerability)
					{
						scrPlanet2.Die();
						scrPlanet2.player.DieByHitbox();
						DOTween.Sequence().Append(listSpike.transform.DOScale(Vector3.one * 0.2f, 0f).SetRelative(isRelative: true)).Append(listSpike.transform.DOScale(Vector3.one * -0.2f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
						continue;
					}
					listSpike.Die();
					if (timeTrial)
					{
						timeTrialFailed = true;
					}
					scrPlanet2.player.marginTracker.AddHit(HitMargin.FailMiss);
					ADOBase.controller.chosenPlanet.MarkFail()?.BlinkForSeconds(3f);
					ADOBase.controller.playerManager.hitTextManager.ShowHitText(HitMargin.FailMiss, ADOBase.controller.playerOne.planetarySystem.chosenPlanet);
					float f = (float)ADOBase.controller.chosenPlanet.angle - 1.570796f * (float)(ADOBase.controller.playerOne.planetarySystem.isCW ? 1 : (-1));
					scatter = Vector3.left * 2f * Mathf.Sin(f) + Vector3.down * 2f * Mathf.Cos(f) + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f);
					listSpike.ballSprite.render.DOColor(whiteClear, (float)ADOBase.conductor.crotchetAtStart / ADOBase.conductor.song.pitch).SetEase(Ease.Linear);
					listSpike.transform.DOLocalMove(scatter, (float)ADOBase.conductor.crotchetAtStart / ADOBase.conductor.song.pitch).SetEase(Ease.OutCubic).SetRelative(isRelative: true);
					listSpike.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), (float)ADOBase.conductor.crotchetAtStart / ADOBase.conductor.song.pitch, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
					scrSfx.instance.PlaySfx(SfxSound.ModifierActivate, MixerGroup.InterfaceParent, 0.8f);
				}
				else
				{
					listSpike.hitTime[i] = 0f;
				}
			}
		}
	}

	private void AddScene1()
	{
		scene.dialog.Clear();
		scene.canSkip = false;
		scene.canAdvance = false;
		if (RDString.isChinese)
		{
			scene.scene_text.font = RDC.data.chineseFontTMProDuplicate;
		}
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.a.1"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.a.2"));
		scene.StartScene();
	}

	private void AddScene2()
	{
		scene.dialog.Clear();
		scene.canSkip = false;
		scene.canAdvance = false;
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.b.1"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.b.2"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.b.3"));
		scene.StartScene();
	}

	private void AddScene3()
	{
		scene.dialog.Clear();
		scene.canSkip = false;
		scene.canAdvance = false;
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.c.1"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.c.2"));
		scene.StartScene();
	}

	private void AddScene4()
	{
		scene.dialog.Clear();
		scene.canSkip = false;
		scene.canAdvance = false;
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.d.1"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.d.2"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.d.3"));
		scene.dialog.Add(RDString.Get("neoCosmosStory.T5.X.d.4"));
		scene.StartScene();
	}

	private void AdvanceText()
	{
		scene.AdvanceText();
	}

	private void PlasmaCoverOn()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			plasmaCold.render.enabled = true;
			plasmaBGcover.render.enabled = true;
			plasmaHot.render.enabled = true;
			plasmaBGcover.render.DOColor(Color.white, beats(5.5f, 200f)).SetEase(Ease.Linear);
		}
	}

	private void ColdPlasBG()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			plasmaCold.render.DOColor(Color.white, beats(2f, 200f)).SetEase(Ease.InOutCubic);
		}
	}

	private void HotPlasBG()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			plasmaHot.render.DOColor(Color.white, beats(2f, 180f)).SetEase(Ease.InOutCubic);
		}
	}

	private void UnHotPlasBG()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			plasmaHot.render.DOColor(whiteClear, beats(2f, 220f)).SetEase(Ease.InOutCubic);
		}
	}

	private void BackToSkyFromPlasma()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			plasmaCold.render.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
			plasmaBGcover.render.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
			plasmaHot.render.DOColor(whiteClear, beats(4f, 220f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				plasmaCold.render.enabled = false;
				plasmaBGcover.render.enabled = false;
				plasmaHot.render.enabled = false;
			});
		}
	}

	private void DriftSkyFrame2()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			driftSky.SetState(1);
		}
	}

	private void FadeMenuBG()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			tweeningMenuBGIntro = true;
			menuBGcover.render.enabled = true;
			menuBGcover.render.DOColor(Color.white, beats(16f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				menuBGcontainer.SetActive(value: false);
				menuBGgradient.render.enabled = false;
			});
		}
	}

	private void FadeMenuBGQ()
	{
		if (!GCS.practiceMode && !lowQual && !tweeningMenuBGIntro)
		{
			menuBGcover.render.enabled = true;
			menuBGcover.render.DOColor(Color.white, 0f).SetEase(Ease.Linear);
			menuBGcontainer.SetActive(value: false);
			menuBGgradient.render.enabled = false;
		}
	}

	private void FadeMenuBG2()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			menuBGcover.render.enabled = true;
			menuBGcover.render.DOColor(Color.white, beats(4f, 200f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				menuBGcontainer.SetActive(value: false);
				menuBGgradient.render.enabled = false;
			});
		}
	}

	private void LowBG()
	{
		menuBGcover.render.DOColor(Color.white, 0f).SetEase(Ease.Linear);
		hotBG.render.enabled = true;
		lasertile.render.enabled = true;
		lasertile.render.DOColor(new Color(1f, 1f, 1f, 0.22745098f), 0f).SetEase(Ease.Linear);
		hotBG.render.DOColor(new Color(1f, 1f, 1f, 44f / 85f), 0f).SetEase(Ease.Linear);
		foreach (GameObject item in highQualBG)
		{
			item.SetActive(value: false);
		}
	}

	private void DivineOn()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			divineOnTween = true;
			hotBG.render.enabled = true;
			lasertile.render.enabled = true;
			lasertile.render.DOColor(new Color(1f, 1f, 1f, 0.22745098f), beats(7f)).SetEase(Ease.Linear);
			hotBG.render.DOColor(new Color(1f, 1f, 1f, 44f / 85f), beats(7f)).SetEase(Ease.Linear);
		}
	}

	private void DivineOnQ()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			menuBGcover.render.DOColor(Color.white, 0f).SetEase(Ease.Linear);
			if (!divineOnTween)
			{
				hotBG.render.enabled = true;
				lasertile.render.enabled = true;
				lasertile.render.DOColor(new Color(1f, 1f, 1f, 0.22745098f), 0f).SetEase(Ease.Linear);
				hotBG.render.DOColor(new Color(1f, 1f, 1f, 44f / 85f), 0f).SetEase(Ease.Linear);
			}
		}
	}

	private void DivineOff()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			divineOffTween = true;
			lasertile.render.DOColor(whiteClear, beats(9f)).SetEase(Ease.Linear);
			hotBG.render.DOColor(whiteClear, beats(9f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				lasertile.render.enabled = false;
				hotBG.render.enabled = false;
			});
			menuBGcontainer.SetActive(value: true);
			menuBGgradient.render.enabled = true;
			menuBGcover.render.enabled = true;
			DOTween.Sequence().Append(menuBGcover.render.DOColor(Color.white, 0f).SetEase(Ease.Linear)).AppendInterval(beats(9f))
				.Append(menuBGcover.render.DOColor(whiteClear, beats(9f)).SetEase(Ease.Linear));
		}
	}

	private void DivineOffQ()
	{
		if (!GCS.practiceMode && !lowQual && !divineOffTween)
		{
			lasertile.render.DOColor(whiteClear, beats(0f)).SetEase(Ease.Linear);
			hotBG.render.DOColor(whiteClear, beats(0f)).SetEase(Ease.Linear);
			lasertile.render.enabled = false;
			hotBG.render.enabled = false;
			menuBGgradient.render.enabled = true;
			menuBGcover.render.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
			menuBGcontainer.SetActive(value: true);
		}
	}

	private void FadeNCGradient()
	{
		if (!GCS.practiceMode && !lowQual)
		{
			NCGradientTweening = true;
			menuBGgradient.render.DOColor(whiteClear, beats(20f)).SetEase(Ease.Linear);
		}
	}

	private void FadeNCGradientQ()
	{
		if (!GCS.practiceMode && !lowQual && !NCGradientTweening)
		{
			menuBGgradient.render.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		}
	}

	private void KickFloors(List<float> l)
	{
		int num = 4;
		int num2 = (int)l[0];
		List<scrFloor> list = new List<scrFloor>();
		for (int i = num2 - num; i < num2; i++)
		{
			list.Add(ADOBase.lm.listFloors[i]);
		}
		bool hitboss = false;
		foreach (scrFloor f in list)
		{
			f.isLandable = false;
			if (isLongVersion)
			{
				DOTween.Sequence().AppendInterval(beats(2f)).Append(f.TweenOpacity(0f, beats(0.5f)));
			}
			else
			{
				DOTween.Sequence().AppendInterval(beats(1f)).Append(f.TweenOpacity(0f, beats(0.5f)));
			}
			scatter = Vector3.right * UnityEngine.Random.Range(-1f, 1f) + Vector3.up * UnityEngine.Random.Range(-1f, 1f) + kickFloorTarg.position;
			midpointJump = 1f + (scatter.y + f.transform.position.y) / 2f;
			scatter2 = f.transform.position + (kickFloorTarg.position - f.transform.position) * -0.5f + Vector3.right * UnityEngine.Random.Range(-2f, 2f) + Vector3.up * UnityEngine.Random.Range(-2f, 2f);
			if (isLongVersion)
			{
				DOTween.Sequence().Append(f.transform.DOMove(scatter2, beats(1f)).SetEase(Ease.OutExpo)).Append(f.transform.DOMove(scatter, beats(1f)).SetEase(Ease.InExpo).OnComplete(delegate
				{
					if (!hitboss)
					{
						hitboss = true;
						HitBoss();
					}
				}))
					.Append(DOTween.Shake(() => f.transform.position, delegate(Vector3 x)
					{
						f.transform.position = x;
					}, 1.5f, beats(0.5f), 60));
			}
			else
			{
				DOTween.Sequence().Append(f.transform.DOMove(scatter2, beats(1f)).SetEase(Ease.OutExpo));
			}
			f.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), beats(1f), RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
		}
	}

	private void KickFloorsQ(List<float> l)
	{
		int num = 4;
		int num2 = (int)l[0];
		List<scrFloor> list = new List<scrFloor>();
		for (int i = num2 - num; i < num2; i++)
		{
			list.Add(ADOBase.lm.listFloors[i]);
		}
		foreach (scrFloor item in list)
		{
			item.transform.position = Vector3.one * 9999f;
		}
	}

	private float ic(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t /= d;
		return c * Mathf.Pow(t, 3f) + b;
	}

	private void SetupDarken()
	{
		darken.render.enabled = true;
	}

	private void CloseEyes(List<float> l)
	{
		for (int i = 0; i < 2; i++)
		{
			Mawaru_Sprite mawaru_Sprite = closingEyes[i];
			mawaru_Sprite.render.enabled = true;
			edir = (float)i * 2f - 1f;
			DOTween.Sequence().Append(mawaru_Sprite.transform.DOLocalMoveY(-10f * edir, l[0] * 0.1f).SetEase(Ease.OutCubic)).Append(mawaru_Sprite.transform.DOLocalMoveY(-10f * edir + l[1] * 10f * edir, l[0] * 0.9f).SetEase(Ease.Linear));
		}
		DOTween.Sequence().Append(darken.render.DOColor(blackClear, l[2] * 0.1f).SetEase(Ease.OutCubic)).Append(darken.render.DOColor(new Color(0f, 0f, 0f, l[3]), l[2] * 0.9f).SetEase(Ease.Linear));
	}

	private void OpenEyes(List<float> l)
	{
		for (int i = 0; i < 2; i++)
		{
			Mawaru_Sprite mawaru_Sprite = closingEyes[i];
			edir = (float)i * 2f - 1f;
			DOTween.Sequence().Append(mawaru_Sprite.transform.DOLocalMoveY(-10f * edir, l[0]).SetEase(Ease.OutCubic));
		}
		DOTween.Sequence().Append(darken.render.DOColor(blackClear, l[0]).SetEase(Ease.OutCubic));
	}

	private void DestroyFloors(List<float> l)
	{
		float f = l[2];
		int num = (int)l[1];
		int num2 = (int)l[0];
		List<scrFloor> list = new List<scrFloor>();
		for (int i = num2 - num; i < num2; i++)
		{
			list.Add(ADOBase.lm.listFloors[i]);
		}
		foreach (scrFloor item in list)
		{
			item.isLandable = false;
			item.TweenOpacity(0f, beats(1f));
			scatter = Vector3.right * 2f * Mathf.Sin(f) + Vector3.down * 2f * Mathf.Cos(f) + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f);
			item.transform.DOLocalMove(scatter, beats(1f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true);
			item.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), beats(1f), RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
		}
	}

	private void FadeInBossBG(List<float> l)
	{
		bossIdle.FadeInBG(beats(l[0]));
	}

	private void FadeOutBossBG(List<float> l)
	{
		bossIdle.FadeOutBG(beats(l[0]));
	}

	private void FadeInBoss(List<float> l)
	{
		bossIdle.FadeIn(beats(l[0]));
	}

	private void FadeOutBoss(List<float> l)
	{
		bossIdle.FadeOut(beats(l[0]));
	}

	private void FadeOutBossQ()
	{
		bossIdle.FadeOut(0f);
	}

	private void BossInPoi()
	{
		bossIdle.ChangeLayer("BG Static");
		bossIdle.transform.SetParent(bossBGParent, worldPositionStays: true);
		bossIdle.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
		bossIdle.transform.localPosition = new Vector3(-5f, -4f, 10f);
	}

	private void BossInSquare()
	{
		bossIdle.ChangeLayer("BG");
		bossIdle.transform.SetParent(null, worldPositionStays: true);
		bossIdle.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
		bossIdle.transform.localPosition = new Vector3(159.88f, 41.91f, 0f);
	}

	private void PrepareKillPlanet()
	{
		planetThatHit455 = scrController.instance.planetBlue;
	}

	private void KillPlanet()
	{
		planetThatHit455.planetRenderer.DisableAllSpecialPlanets();
		planetThatHit455.Destroy();
		foreach (PlanetRenderer dummyPlanet in scrController.instance.dummyPlanets)
		{
			dummyPlanet.gameObject.SetActive(value: false);
		}
		foreach (LineRenderer multiPlanetLine in scrController.instance.multiPlanetLines)
		{
			multiPlanetLine.gameObject.SetActive(value: false);
		}
	}

	private void DestroyDummyPlanets()
	{
		foreach (PlanetRenderer dummyPlanet in scrController.instance.dummyPlanets)
		{
			dummyPlanet.EnableCustomColor();
			dummyPlanet.SetPlanetColor(new Color(1f, 0.8f, 0f, 1f));
			dummyPlanet.SetTailColor(new Color(1f, 0.9f, 0.6f, 1f));
			dummyPlanet.SetEmojiMode(enabled: false);
			dummyPlanet.DisableParticles();
			if (dummyPlanet.onlyRing)
			{
				dummyPlanet.Destroy(keepRing: true);
			}
		}
	}

	private void RevivePlanet()
	{
		PlanetRenderer planetRenderer = planetThatHit455.planetRenderer;
		planetRenderer.ToggleSamurai(enabled: false, isRed: false);
		planetRenderer.sprite.visible = true;
		planetRenderer.EnableCustomColor();
		planetRenderer.SetPlanetColor(new Color(1f, 0.4f, 0.4f, 1f));
		planetRenderer.SetTailColor(new Color(1f, 0.8f, 0.8f, 1f));
		planetRenderer.SetEmojiMode(enabled: false);
		planetRenderer.Revive(liteRevive: true);
		PlanetRenderer planetRenderer2 = scrController.instance.planetarySystem.planetGreen.planetRenderer;
		planetRenderer2.ToggleSamurai(enabled: false, isRed: false);
		planetRenderer2.EnableCustomColor();
		planetRenderer2.SetPlanetColor(new Color(1f, 0.8f, 0f, 1f));
		planetRenderer2.SetTailColor(new Color(1f, 0.9f, 0.6f, 1f));
		planetRenderer2.SetEmojiMode(enabled: false);
		ShowSnail();
		GCS.staticPlanetColors = true;
	}

	private void ShowSnail()
	{
		ADOBase.lm.listFloors[457].hideIcon = false;
		ADOBase.lm.listFloors[457].UpdateIconSprite();
	}

	private void SpawnCharlie()
	{
		charlie.AddEntry(new CharlieAction(charlie.waypoints[3].localPosition, 0.1f, "warp", -1f));
		charlie.AddEntry(new CharlieAction(charlie.waypoints[3].localPosition, 0.1f, "warp", -1f));
		charlie.AddEntry(new CharlieAction(charlie.waypoints[0].localPosition, 0.6f, "jump", -1f, 2f));
		charlie.AddEntry(new CharlieAction(charlie.waypoints[1].localPosition, 0.6f, "jump", -1f, 2f));
		charlie.AddEntry(new CharlieAction(charlie.waypoints[2].localPosition, 0.6f, "jump", -1f, 2f));
		charlie.AddEntry(new CharlieAction(charlie.waypoints[2].localPosition, 0.1f, "wait", -1f));
		charlie.AddEntry(new CharlieAction(charlie.waypoints[2].localPosition, 0f, "talk", -1f));
	}

	private void SpawnSef()
	{
		sef.actor.transform.localScale = new Vector3(2f * sef.defaultScale, 0f, 0f);
		sef.actor.transform.DOScale(Vector3.one * sef.defaultScale, 0.3f).SetEase(Ease.OutBack);
		sef.transform.position = ADOBase.lm.listFloors[463].transform.position + Vector3.up * 2f;
	}

	private void CharlieTransform()
	{
		charlie.actor.render.material.DOColor(new Color(1f, 0.4f, 0.4f, 1f), "_Glow", beats(1f, 150f)).OnComplete(delegate
		{
			charlie.gameObject.SetActive(value: false);
		});
		charlie_planet_parent.position = charlie.transform.position + Vector3.up;
		scrSfx.instance.PlaySfx(SfxSound.PlanetTransform, MixerGroup.SfxParent);
	}

	private void SefTransform()
	{
		sef.actor.render.material.DOColor(new Color(1f, 0.8f, 0f, 1f), "_Glow", beats(1f, 150f)).OnComplete(delegate
		{
			sef.gameObject.SetActive(value: false);
		});
		scrSfx.instance.PlaySfx(SfxSound.PlanetTransform, MixerGroup.SfxParent);
	}

	private void CharliePlanetGo()
	{
		charlie_planet_parent.gameObject.SetActive(value: true);
		charlie_planet_parent.position = charlie.transform.position + Vector3.up;
		charlie_planet_parent.DOLocalMove(ADOBase.controller.chosenPlanet.transform.position + Vector3.right * 1.5f, beats(3f, 150f)).SetEase(Ease.InQuad).OnComplete(delegate
		{
			charlie_planet_parent.gameObject.SetActive(value: false);
		});
		charlie_planet_parent.localScale = Vector3.one * 0.75f;
		DOTween.Sequence().Append(charlie_planet_parent.DOScale(Vector3.one * 2f, 0f).SetEase(Ease.OutCubic)).Append(charlie_planet_parent.DOScale(Vector3.one * 0.75f, beats(0.25f, 150f)).SetEase(Ease.InQuad))
			.Append(charlie_planet_parent.DOScale(Vector3.one, beats(2.75f, 150f)).SetEase(Ease.InQuad));
		DOTween.Sequence().Append(charlie_planet.transform.DOLocalMoveY(2f, beats(1.5f, 150f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).Append(charlie_planet.transform.DOLocalMoveY(-2f, beats(1.5f, 150f)).SetRelative(isRelative: true).SetEase(Ease.InCubic));
	}

	private void SefPlanetGo()
	{
		sef_planet_parent.gameObject.SetActive(value: true);
		DOTween.Sequence().Append(sef_planet_parent.DOScale(Vector3.one * 2f, 0f).SetEase(Ease.OutCubic)).Append(sef_planet_parent.DOScale(Vector3.one, beats(0.25f, 150f)).SetEase(Ease.InQuad));
		sef_planet_parent.position = sef.transform.position;
		sef_planet_parent.DOLocalMove(ADOBase.lm.listFloors[469].transform.position + Vector3.up * 1.3f + Vector3.left * 0.75f, beats(3f, 150f)).SetEase(Ease.InQuad).OnComplete(delegate
		{
			sef_planet_parent.gameObject.SetActive(value: false);
			AwakenDummyPlanets();
		});
		DOTween.Sequence().Append(sef_planet.transform.DOLocalMoveY(2f, beats(1.5f, 150f)).SetRelative(isRelative: true).SetEase(Ease.OutCubic)).Append(sef_planet.transform.DOLocalMoveY(-2f, beats(1.5f, 150f)).SetRelative(isRelative: true).SetEase(Ease.InCubic));
	}

	private void AwakenDummyPlanets()
	{
		foreach (PlanetRenderer dummyPlanet in scrController.instance.dummyPlanets)
		{
			dummyPlanet.gameObject.SetActive(value: true);
			if (dummyPlanet.onlyRing)
			{
				dummyPlanet.DisableParticles();
				dummyPlanet.Destroy(keepRing: true);
			}
		}
		foreach (LineRenderer multiPlanetLine in scrController.instance.multiPlanetLines)
		{
			multiPlanetLine.gameObject.SetActive(value: true);
		}
	}

	private void MoveShurikenTiles()
	{
	}

	private void MoveShurikenTilesQ()
	{
	}

	private void HandsOn()
	{
		bossEndHands[0].render.enabled = true;
		bossEndHands[1].render.enabled = true;
	}

	private void HandsOff()
	{
		bossEndHands[0].render.enabled = false;
		bossEndHands[1].render.enabled = false;
	}

	private void SetHandsPos(List<float> l)
	{
		bossEndHandContainers[0].transform.localPosition = Vector3.right * l[0] + Vector3.up * l[1] + Vector3.forward * 10f;
		bossEndHandContainers[0].transform.localEulerAngles = Vector3.forward * l[2];
		bossEndHandContainers[1].transform.localPosition = Vector3.right * l[3] + Vector3.up * l[4] + Vector3.forward * 10f;
		bossEndHandContainers[1].transform.localEulerAngles = Vector3.forward * l[5];
	}

	private void HideLateSpike()
	{
		lateSpike.ballSprite.render.DOColor(whiteClear, beats(0f));
	}

	private void SpikeAppear()
	{
		lateSpike.ballSprite.render.DOColor(Color.white, beats(1f)).SetEase(Ease.Linear);
	}

	private void SpawnBootlegFloor()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(bootlegFloor.DOScale(Vector3.one * 1.5f, 0f))
				.Append(bootlegFloor.DOScale(Vector3.one * 0.95f, beats(2f, 150f)).SetEase(Ease.Linear))
				.AppendInterval(beats(0.05f, 150f))
				.Append(bootlegFloor.DOScale(Vector3.zero, 0f));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(bootlegFloor.DOMove(bootlegSpawn.position, 0f))
				.Append(bootlegFloor.DOMove(bootlegTarg.position, beats(2f, 150f)).SetEase(Ease.Linear).OnComplete(delegate
				{
				}));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(bootlegInner.transform.DOLocalMoveY(5f, beats(1f, 150f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
				.Append(bootlegInner.transform.DOLocalMoveY(-5f, beats(1f, 150f)).SetEase(Ease.InQuad).SetRelative(isRelative: true));
			bootlegInner.transform.DORotate(Vector3.forward * 720f, beats(2f, 150f), RotateMode.FastBeyond360).SetEase(Ease.Linear);
		}
	}

	private void SetupTimeTrial()
	{
		if (!beatT5)
		{
			ttStartGraphic.transform.position = Vector3.up * 9999f;
		}
		RemoveTTText();
	}

	private void RemoveTTText()
	{
		timerText.transform.localScale = Vector3.zero;
		myBestText.transform.localScale = Vector3.zero;
		goalTime.transform.localScale = Vector3.zero;
	}

	private void StartCountingTimeTrial()
	{
		foreach (LaserSpawner laserSpawner in laserSpawners)
		{
			laserState[laserSpawner] = laserSpawner.killedPlanet;
		}
		timeTrialTime = 0f;
		timeTrial = true;
		timerText.transform.localScale = Vector3.one;
		myBestText.transform.localScale = Vector3.one;
	}

	private void StopCountingTimeTrial()
	{
		timeTrial = false;
		if (!timeTrialWon && ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			JudgeSection(6, 1, showText: true, ADOBase.controller.currFloor.transform);
		}
		ttGoalGraphic.render.DOColor(whiteClear, beats(2f)).SetEase(Ease.Linear);
		ttStartGraphic.render.DOColor(whiteClear, beats(2f)).SetEase(Ease.Linear);
	}

	private void FinalEnd()
	{
		if (!finalEnded && ADOBase.controller.currentState == States.Won)
		{
			finalEnded = true;
			LightSkySetup();
			LightenSky();
			if (!lowVfx)
			{
				ADOBase.controller.ScreenShake(1f, 0.2f);
				ProcEnd();
			}
			for (int i = 0; i < 8; i++)
			{
				Transform target = cloudAddX[i];
				DOTween.Sequence().Append(target.DOLocalMoveX(i % 2 * -2 + 1, beats(12f, 200f)).SetEase(Ease.OutCubic));
			}
		}
	}

	private void ProcEnd()
	{
		endingSparkle.Proc();
	}

	private void ShowFinalResults()
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && (!GCS.speedTrialMode || !(GCS.currentSpeedTrial >= 1f))) || finalResultsed || ADOBase.controller.currentState != States.Won || ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		ADOBase.controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: false);
		int[] array = SaveMedals("T5", Divine_Stats.sectionStats);
		finalResultsed = true;
		if (Divine_Stats.timeTrialTime < 60f)
		{
			endingTTText.text = string.Format("{0} {1:#,0.000}", RDString.Get("t5.bestTime"), Divine_Stats.timeTrialTime);
			if (Divine_Stats.timeTrialTime < parTime)
			{
				endingTTText.DOColor(new Color(1f, 0.8f, 0f, 1f), 0f);
			}
			else
			{
				endingTTText.DOColor(new Color(0.3f, 0.6f, 1f, 1f), 0f);
			}
		}
		else
		{
			endingTTText.transform.localPosition = Vector3.up * 999f;
			endingTTBar.transform.localPosition = Vector3.up * 999f;
		}
		for (int i = 0; i < endingMedals.Count; i++)
		{
			if (array[i] > 0)
			{
				endingMedals[i].front.SetState(Divine_Stats.sectionStats[i] - 1);
			}
			else
			{
				endingMedals[i].front.render.enabled = false;
			}
		}
		for (int j = 0; j < endingStuff.Count; j++)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.1f * (float)j)
				.Append(endingStuff[j].transform.DOScale(Vector3.one * endingStuff_originalSizes[j], 0.3f).SetEase(Ease.OutBack));
		}
		SaveT5Time(Divine_Stats.timeTrialTime);
		ADOBase.controller.canExitLevel = true;
	}
}
