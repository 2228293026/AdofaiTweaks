using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SingSing : TaroBGScript
{
	[Header("Resource management", order = 0)]
	public List<Mawaru_Sprite> stuffToDisable;

	public Image allCover;

	public List<GameObject> highQualBG;

	[Header("Assets", order = 0)]
	private GameObject something;

	public List<GameObject> BGsToStretch;

	private int lastJudgedSection = -1;

	private int judgingSection = -1;

	private bool cursection_hasMadeMistakes;

	private bool cursection_hasNonPerfects;

	public List<GameObject> endingStuff;

	public List<Mawaru_Medal> endingMedals;

	public List<float> endingStuff_originalSizes = new List<float>();

	private List<int> sectionFloors = new List<int>();

	private List<Color> RankingTextColors = new List<Color>
	{
		new Color(1f, 1f, 1f, 1f),
		new Color(0.6f, 0.4f, 0f, 1f),
		new Color(0.7f, 0.7f, 0.7f, 1f),
		new Color(1f, 0.8f, 0f, 1f)
	};

	public Mawaru_Sprite oceansparkle;

	public Mawaru_Sprite sky1;

	public Mawaru_Sprite sky2;

	public Mawaru_Sprite sky2Starfield;

	public Mawaru_Sprite nightSky;

	public Mawaru_Sprite nightFake;

	public Mawaru_Sprite nightStarfield1;

	public Mawaru_Sprite nightStarfield2;

	public Mawaru_Sprite morning;

	public Mawaru_Sprite psytrance;

	public List<SingSing_Glider> gliders;

	public List<Mawaru_Sprite> cubicleBGSquares;

	public Mawaru_Sprite cubicle;

	public GameObject cubicleSquareContainer;

	private GameObject camZoomAux;

	private List<float> cubicleZoomBeats = new List<float>();

	private int curCubicleZoom;

	public List<Mawaru_Sprite> transitionQuads;

	public List<Mawaru_Sprite> nightSparkleStar;

	public List<Mawaru_Sprite> nightSparkleGlow;

	public List<GameObject> nightSparkleWaypoint;

	public GameObject clockContainer;

	public List<Mawaru_Sprite> clockHands;

	public List<Mawaru_Sprite> clouds;

	public List<Mawaru_Sprite> closingEyes;

	public Mawaru_Sprite lightsOut;

	public List<scrPlanetCopyCam> cloneCams;

	private int singsing_next_notefield;

	private int singsing_next_midspin;

	private float singsing_notefield_spawntime = 34f;

	public List<SingSing_Notefield> notefields;

	private List<int> hotFloors;

	private List<int> coldFloors;

	private List<int> cubicleHitFloors;

	private List<float> cubicleHitBeats;

	public Mawaru_Sprite wflash;

	public Mawaru_Sprite wflashbg;

	private int frameStart;

	public Camera myCam;

	private int defaultMask;

	private int funkyMask;

	private bool lowVfx;

	private bool lowQual;

	private Vector4 scroller = new Vector4(0f, 0f, 0f, 0f);

	private bool cubTran;

	private bool roomIsHot = true;

	private Color hotColor = new Color(1f, 0.4f, 0f, 0.3f);

	private Color coldColor = new Color(0f, 0.4f, 1f, 0.3f);

	private Color hotColor2 = new Color(1f, 0.7f, 0f, 0.3f);

	private Color coldColor2 = new Color(0f, 0.7f, 1f, 0.3f);

	private float room_scroll_speed = 3.5f;

	private float camang;

	private List<float> midspinsThisSpawn = new List<float>();

	private List<int> floorsThisSpawn = new List<int>();

	private float xp;

	private float yp;

	private int gliderptr;

	private int sparkleParticle;

	private int glowParticle;

	private Color fadedSparkle = new Color(1f, 1f, 1f, 0.6f);

	private int curSparkle;

	private int numSparklesThisTime = 1;

	private Vector3 spawnPos;

	private Vector3 randomShift;

	private float spawnScale;

	private float drunkAmt;

	private float beat;

	private float amp;

	private int drunkStartFloor = 283;

	private int drunkEndFloor = 378;

	private Vector4 starfieldScroll = Vector4.zero;

	private float edir;

	private float eperc;

	private float sval;

	private float bhei;

	private bool wflashanim;

	private Color blackClear = new Color(0f, 0f, 0f, 0f);

	private int lastCheckedFloor;

	private void JudgeSection(int section, int grade, bool showText = true, int floor = 0)
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && GCS.currentSpeedTrial < 1f))
		{
			return;
		}
		if (slumpo)
		{
			Singsing2_Stats.sectionStats[section] = grade;
		}
		else
		{
			Singsing_Stats.sectionStats[section] = grade;
		}
		if (showText && (!RDC.noHud || RDC.partialNoHud))
		{
			Vector3 position = scrLevelMaker.instance.listFloors[floor].transform.position;
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
		for (int i = 0; i < sectionFloors.Count; i++)
		{
			if (ADOBase.controller.currFloor.seqID == sectionFloors[i] && scrLevelMaker.instance.listFloors[sectionFloors[i] - 1].grade != HitMargin.TooEarly && lastJudgedSection < i)
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
				JudgeSection(i, grade, showText: true, sectionFloors[i]);
				cursection_hasMadeMistakes = false;
				cursection_hasNonPerfects = false;
				judgingSection = i + 1;
				lastJudgedSection = i;
			}
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
	}

	private void Start()
	{
		frameStart = Time.frameCount;
		if (slumpo)
		{
			ADOBase.controller.caption = ADOBase.GetLocalizedLevelName("T2-X").Replace("-X", "-EX");
		}
		if (slumpo && GCS.checkpointNum == 73)
		{
			for (int i = 69; i < 73; i++)
			{
				scrLevelMaker.instance.listFloors[i].gameObject.SetActive(value: false);
			}
		}
		else if (!slumpo && GCS.checkpointNum == 57)
		{
			for (int j = 53; j < 57; j++)
			{
				scrLevelMaker.instance.listFloors[j].gameObject.SetActive(value: false);
			}
		}
	}

	private new void Awake()
	{
		base.Awake();
		defaultMask = myCam.cullingMask;
		funkyMask = (1 << LayerMask.NameToLayer("TransparentFX")) | (1 << LayerMask.NameToLayer("Water")) | (1 << LayerMask.NameToLayer("Camera")) | (1 << LayerMask.NameToLayer("Review")) | (1 << LayerMask.NameToLayer("WorkshopThumbnail"));
		allCover.gameObject.SetActive(value: true);
		lowVfx = ADOBase.controller.visualEffects == VisualEffects.Minimum;
		lowQual = ADOBase.controller.visualQuality == VisualQuality.Low;
		if (!slumpo)
		{
			if (!Singsing_Stats.init)
			{
				Singsing_Stats.Reset();
			}
		}
		else if (!Singsing2_Stats.init)
		{
			Singsing2_Stats.Reset();
		}
		if (!slumpo)
		{
			GCS.pauseMedalStatsCurrent = Singsing_Stats.sectionStats;
		}
		else
		{
			GCS.pauseMedalStatsCurrent = Singsing2_Stats.sectionStats;
		}
		mb(0f, base.SetResultTextPos2, 9999f);
		for (int i = 0; i < endingStuff.Count; i++)
		{
			GameObject gameObject = endingStuff[i];
			endingStuff_originalSizes.Add(gameObject.transform.localScale.x);
			gameObject.transform.localScale = Vector3.zero;
		}
		if (!slumpo)
		{
			sectionFloors = new List<int> { 57, 148, 189, 244, 324, 358, 382 };
		}
		else
		{
			sectionFloors = new List<int> { 73, 180, 225, 282, 378, 412, 436 };
		}
		GCS.pauseMedalFloors = new List<int>();
		GCS.pauseMedalFloors.Clear();
		GCS.pauseMedalFloors.Add(0);
		for (int j = 0; j < sectionFloors.Count - 1; j++)
		{
			GCS.pauseMedalFloors.Add(sectionFloors[j]);
		}
		if (!slumpo)
		{
			GCS.pauseMedalFloors[1] = 58;
		}
		else
		{
			GCS.pauseMedalFloors[1] = 74;
		}
		if (!slumpo)
		{
			hotFloors = new List<int>
			{
				29, 36, 44, 51, 60, 66, 72, 78, 108, 114,
				132, 138, 254, 258, 278, 281, 284, 287, 310, 313,
				316, 319
			};
			coldFloors = new List<int>
			{
				14, 21, 84, 90, 96, 102, 120, 126, 144, 202,
				210, 231, 239, 267, 271, 294, 297, 300, 303
			};
		}
		else
		{
			hotFloors = new List<int>
			{
				37, 40, 46, 49, 56, 59, 65, 68, 84, 90,
				123, 131, 138, 144, 172, 213, 284, 287, 290, 293,
				300, 303, 306, 309, 332, 335, 338, 341, 364, 367,
				370, 373
			};
			coldFloors = new List<int>
			{
				2, 9, 17, 20, 26, 29, 96, 102, 119, 127,
				136, 146, 151, 156, 161, 166, 176, 217, 238, 246,
				267, 275, 279, 316, 319, 322, 325, 348, 351, 354,
				357
			};
			cubicleHitFloors = new List<int>
			{
				84, 90, 96, 102, 108, 114, 119, 123, 127, 131,
				136, 138, 144, 146, 151, 154, 156, 161, 164, 166,
				172, 174, 176
			};
			cubicleHitBeats = new List<float>
			{
				41f, 44f, 47.5f, 50.5f, 53.25f, 56.25f, 58.5f, 60f, 61.5f, 63f,
				65f, 65.5f, 68f, 68.5f, 70.5f, 71.25f, 71.5f, 73.5f, 74.25f, 74.5f,
				77f, 77.25f, 77.5f
			};
		}
		camZoomAux = new GameObject();
		for (int k = 0; k < 15; k++)
		{
			cubicleZoomBeats.Add(34f + (float)k * 3f + 0f);
			cubicleZoomBeats.Add(34f + (float)k * 3f + 0.75f);
			cubicleZoomBeats.Add(34f + (float)k * 3f + 1.75f);
			cubicleZoomBeats.Add(34f + (float)k * 3f + 2.5f);
		}
		if (!lowVfx && !lowQual)
		{
			foreach (int hotFloor in hotFloors)
			{
				scrLevelMaker.instance.listFloors[hotFloor].gameObject.AddComponent<ffxHotTile>();
			}
			foreach (int coldFloor in coldFloors)
			{
				scrLevelMaker.instance.listFloors[coldFloor].gameObject.AddComponent<ffxColdTile>();
			}
		}
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.78f)
		{
			foreach (GameObject item in BGsToStretch)
			{
				item.transform.localScale = new Vector3(item.transform.localScale.x * (num / 1.7777778f), item.transform.localScale.y, item.transform.localScale.z);
			}
		}
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 114.0));
		bpms.Add(new Tuple<double, double>(193.0, 111.0));
		bpms.Add(new Tuple<double, double>(201.0, 105.0));
		bpms.Add(new Tuple<double, double>(209.0, 96.0));
		bpms.Add(new Tuple<double, double>(217.0, 84.0));
		bpms.Add(new Tuple<double, double>(225.0, 69.0));
		mb(-199f, DisableStuff, 9999f);
		mpf(-8f, AdjustPlanetAngle, 1f);
		mb(-4f, PlanetTween);
		mb(-200f, SetupFonts, 99999f);
		mb(0f, SeaDrop);
		mb(16f, Glider1);
		mb(20f, Glider2);
		mb(24f, Glider3);
		mb(24f, SeaDropFast, 225f);
		mb(33f, PreCubicle, 33.5f);
		mb(33.5f, CubicleSetup, 83f);
		mb(33.5f, HideOcean, 225f);
		mpf(25f, CubicleUpdate, 84f);
		mb(44f, CubicleTransition);
		mb(56f, CubicleTransition);
		mb(62f, CubicleTransition);
		mb(68f, CubicleTransition);
		mb(74f, CubicleTransition);
		if (slumpo)
		{
			if (!lowVfx)
			{
				mb(55f, LightsOffSlow);
				mb(61f, LightsOffSlow);
				mb(67f, LightsOffSlow);
				mb(73f, LightsOffSlow);
			}
			else
			{
				mb(55f, LightsOffSlowLow);
				mb(73f, LightsOnSlowLow);
			}
			if (!lowVfx)
			{
				mb(58f, ThreeCams);
				mb(64f, FourCams);
				mb(70f, SixCams);
				mb(76f, NormalCam);
			}
			mb(34f, RemoveNonCubicleFloors);
			mb(76f, ReturnNonCubicleFloors);
		}
		mb(79f, CubicleBGFade);
		mb(84f, CubicleHide, 225f);
		if (!lowVfx)
		{
			mb(84f, PsyCold);
			mb(86f, PsyOff);
			mb(88f, PsyHot);
			mb(90f, PsyOff);
			mb(92f, PsyCold);
			mb(94f, PsyOff);
			mb(96f, PsyHot);
			mb(97f, PsyOff);
			mb(100f, PsyCold);
			mb(102f, PsyOff);
			mb(104f, PsyHot);
			mb(106f, PsyOff);
			mb(108f, PsyCold);
			mb(110f, PsyOff);
			mb(112f, PsyHot);
			mb(113f, PsyOff);
		}
		mb(115.5f, NightRealSetup);
		mb(116f, NightRealFadeIn);
		foreach (float item2 in new List<float>
		{
			116.75f, 117f, 117.25f, 117.5f, 119.75f, 120.25f, 123f, 123.25f, 123.5f, 123.75f,
			124f, 132.25f, 132.5f, 132.75f, 133f, 133.25f, 133.5f, 135.75f, 136.25f, 136.75f,
			139f, 139.25f, 139.5f, 139.75f, 140f
		})
		{
			mb(item2, SpawnSparkle);
		}
		if (!lowVfx)
		{
			mb(148f, PsyRainbow, 178f);
			mb(176f, PsyFade);
		}
		mpf(148f, ShurikenUpdate, 178f);
		mb(178f, NoDrunk);
		mb(182.5f, ClockSpawn, 200f);
		mb(185f, ClockBegin);
		mb(192f, ClockEnd);
		for (int l = 0; l < 13; l++)
		{
			mb(185f + (float)l * (7f / 12f), ClockTick);
		}
		mpf(193f, EndingUpdate, 225f);
		mb(193f, EndingFades);
		mb(221f, MoveFinalText);
		if (!lowVfx)
		{
			mb(146f, WhiteFlash);
			mb(147.5f, BlackFlashQ);
		}
		if (!lowVfx)
		{
			mb(225f, Beeps);
			mb(228f, Reset);
		}
		if (lowVfx)
		{
			mb(225f, BeepsLow);
		}
		mb(229f, ShowFinalResults);
		SortTables();
	}

	private void DisableStuff()
	{
		foreach (Mawaru_Sprite item in stuffToDisable)
		{
			item.render.enabled = false;
		}
		if (!lowQual)
		{
			return;
		}
		foreach (GameObject item2 in highQualBG)
		{
			item2.SetActive(value: false);
		}
	}

	private void FadeAllCover()
	{
		DOTween.Sequence().AppendInterval(0f).Append(allCover.DOFade(0f, 0.1f).SetEase(Ease.Linear).OnComplete(delegate
		{
			allCover.enabled = false;
		}));
	}

	private new void Update()
	{
		base.Update();
		if (Time.frameCount - frameStart == 5)
		{
			FadeAllCover();
		}
		Grading();
		if (!cubTran && songBeat > 32.0 && songBeat < 45.0 && (ADOBase.controller.currentState == States.Checkpoint || ADOBase.controller.currentState == States.PlayerControl))
		{
			cubTran = true;
			BGTransition();
		}
		if (cubicle.render.enabled)
		{
			scroller.Set(scroller.x + base.transform.position.x * Time.deltaTime, scroller.y + base.transform.position.y * Time.deltaTime, 20f, 20f);
			cubicle.render.material.SetVector("_TexOffset", scroller);
			if (cubicle.curFrame == 0 || cubicle.curFrame == 15)
			{
				cubicle.animate = false;
			}
		}
	}

	private void PlanetTween()
	{
		DOTween.Sequence().Append(base.transform.DOMoveX((float)Math.PI * 4f, 0f)).Append(base.transform.DOMoveX(0f, beats(3.5f)).SetEase(Ease.OutQuart));
	}

	private void AdjustPlanetAngle()
	{
		ADOBase.controller.chosenPlanet.cosmeticAngle = base.transform.position.x;
	}

	private void PreCubicle()
	{
		if (lowQual || GCS.practiceMode)
		{
			return;
		}
		foreach (Mawaru_Sprite cubicleBGSquare in cubicleBGSquares)
		{
			cubicleBGSquare.render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		}
	}

	private void CubicleSetup()
	{
		if (lowQual || GCS.practiceMode)
		{
			return;
		}
		base.transform.position = Vector3.right * room_scroll_speed;
		if (!slumpo)
		{
			cubicle.render.enabled = true;
			cubicle.render.material.DOColor(hotColor, 0f).SetEase(Ease.Linear);
		}
		else
		{
			cubicle.render.material.DOColor(new Color(1f, 1f, 1f, 0.6f), 0f).SetEase(Ease.Linear);
		}
		foreach (Mawaru_Sprite cubicleBGSquare in cubicleBGSquares)
		{
			cubicleBGSquare.render.enabled = true;
			if (slumpo)
			{
				DOTween.Sequence().Append(cubicleBGSquare.render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear)).Append(cubicleBGSquare.render.material.DOColor(hotColor2, beats(1f)).SetEase(Ease.Linear));
			}
			else
			{
				cubicleBGSquare.render.material.DOColor(hotColor2, beats(1f)).SetEase(Ease.Linear);
			}
		}
	}

	private void HideOcean()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			FadeStuff(oceansparkle, 0f, 0f);
			FadeStuff(sky1, 0f, 0f);
			FadeStuff(sky2, 0f, 0f);
			FadeStuff(sky2Starfield, 0f, 0f);
		}
	}

	private void CubicleTransition()
	{
		if (lowQual || GCS.practiceMode)
		{
			return;
		}
		if (!slumpo)
		{
			cubicle.Play();
			cubicle.render.material.DOColor(roomIsHot ? coldColor : hotColor, beats(1f)).SetEase(Ease.Linear);
			base.transform.DOMove(roomIsHot ? (Vector3.up * room_scroll_speed) : (Vector3.right * room_scroll_speed), beats(1f)).SetEase(Ease.InOutExpo);
		}
		cubicleSquareContainer.transform.DORotate(Vector3.forward * 90f, beats(3f), RotateMode.FastBeyond360).SetEase(Ease.InOutExpo).SetRelative(isRelative: true);
		foreach (Mawaru_Sprite cubicleBGSquare in cubicleBGSquares)
		{
			cubicleBGSquare.render.material.DOColor(roomIsHot ? coldColor2 : hotColor2, beats(1f)).SetEase(Ease.Linear);
		}
		roomIsHot = !roomIsHot;
	}

	private void CubicleBGFade()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			FadeStuff(cubicleBGSquares, 0f, beats(5f));
		}
	}

	private void CubicleHide()
	{
		if (lowQual || GCS.practiceMode)
		{
			return;
		}
		cubicle.render.enabled = false;
		foreach (Mawaru_Sprite cubicleBGSquare in cubicleBGSquares)
		{
			cubicleBGSquare.render.enabled = false;
		}
	}

	private float ios(float t, float b, float c, float d)
	{
		return (0f - c) / 2f * (Mathf.Cos((float)Math.PI * t / d) - 1f) + b;
	}

	private void CubicleUpdate()
	{
		if (slumpo && !lowVfx)
		{
			CubicleMemes();
		}
		HarshFailbar();
		beat = (float)songBeat;
		if (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		if (slumpo && beat > singsing_notefield_spawntime - 6f)
		{
			midspinsThisSpawn.Clear();
			floorsThisSpawn.Clear();
			if (beat < singsing_notefield_spawntime + 1f && singsing_notefield_spawntime < 79f)
			{
				for (int i = singsing_next_midspin; i < cubicleHitBeats.Count; i++)
				{
					if (cubicleHitBeats[i] >= singsing_notefield_spawntime && cubicleHitBeats[i] < singsing_notefield_spawntime + 6f)
					{
						midspinsThisSpawn.Add(cubicleHitBeats[i]);
						floorsThisSpawn.Add(cubicleHitFloors[i]);
						singsing_next_midspin = i;
					}
					if (cubicleHitBeats[i] >= singsing_notefield_spawntime + 6f)
					{
						break;
					}
				}
				notefields[singsing_next_notefield].Spawn(singsing_notefield_spawntime, midspinsThisSpawn, floorsThisSpawn);
				singsing_next_notefield++;
				if (singsing_next_notefield > 2)
				{
					singsing_next_notefield = 0;
				}
			}
			singsing_notefield_spawntime += 6f;
		}
		if (!(beat > 34f) || !(beat < 79f) || lowVfx)
		{
			return;
		}
		if (beat >= 58f && (double)beat < 78.5)
		{
			camang = -360f + 360f * ios(beat - 58f, 0f, 1f, 20.5f);
		}
		if (!slumpo)
		{
			base.camy.transform.eulerAngles = Vector3.forward * camang;
		}
		if (curCubicleZoom < cubicleZoomBeats.Count && beat > cubicleZoomBeats[curCubicleZoom])
		{
			if (beat < cubicleZoomBeats[curCubicleZoom] + 1f)
			{
				DOTween.Sequence().Append(camZoomAux.transform.DOMoveX(-0.08f, 0f)).Append(camZoomAux.transform.DOMoveX(0f, beats(0.5f)).SetEase(Ease.OutSine));
			}
			curCubicleZoom++;
		}
		base.camy.zoomSize = 1.2f + camZoomAux.transform.position.x;
	}

	private void NightRealSetup()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			EnableStuff(nightSky);
			EnableStuff(nightStarfield1);
			EnableStuff(nightStarfield2);
		}
	}

	private void NightRealFadeIn()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			FadeStuff(nightSky, 1f, beats(2f));
			FadeStuff(nightStarfield1, 0.5f, beats(2f));
			FadeStuff(nightStarfield2, 0.5f, beats(2f));
		}
	}

	private void SeaDrop()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			FadeStuff(oceansparkle, 0f, beats(32f));
			sky2.transform.DOLocalMoveY(-6f, beats(32f)).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			sky2Starfield.transform.DOLocalMoveY(-3f, beats(32f)).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			sky1.transform.DOLocalMoveY(-6f, beats(32f)).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			oceansparkle.transform.DOLocalMoveY(-50f, beats(32f)).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			oceansparkle.transform.DOLocalMoveZ(-40f, beats(32f)).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
		}
	}

	private void SeaDropFast()
	{
		if (!lowQual && !GCS.practiceMode && ADOBase.controller.currentState != States.PlayerControl && ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			FadeStuff(oceansparkle, 0f, 0f);
			sky2.transform.DOLocalMoveY(-6f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			sky2Starfield.transform.DOLocalMoveY(-3f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			sky1.transform.DOLocalMoveY(-6f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			oceansparkle.transform.DOLocalMoveY(-50f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			oceansparkle.transform.DOLocalMoveZ(-40f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
		}
	}

	private void BGTransition()
	{
		if (lowQual || GCS.practiceMode)
		{
			return;
		}
		int i;
		for (i = 0; i < 60; i++)
		{
			xp = (float)i % 12f * 2.5f - 14.5f;
			yp = Mathf.Floor((float)i / 12f) * 2.5f - 5f;
			transitionQuads[i].transform.localPosition = Vector3.right * xp + Vector3.up * yp + Vector3.forward;
			transitionQuads[i].render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(Mathf.Floor((float)i / 12f) * 0.05f))
				.Append(transitionQuads[i].transform.DOScale(Vector3.one * 2.55f, beats(1.2f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.25f - Mathf.Floor((float)i / 12f) * 0.05f))
				.AppendInterval(beats(0.1f))
				.AppendInterval(beats(Mathf.Floor((float)i / 12f) * 0.05f))
				.Append(transitionQuads[i].transform.DOScale(Vector3.zero, beats(1f)).SetEase(Ease.InQuad).OnComplete(delegate
				{
					transitionQuads[i].render.enabled = false;
				}));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(Mathf.Floor((float)i / 12f) * 0.05f))
				.Append(transitionQuads[i].transform.DORotate(Vector3.forward * 90f, beats(1.2f), RotateMode.FastBeyond360).SetEase(Ease.Linear));
		}
	}

	private void PsyCold()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			psytrance.render.enabled = true;
			psytrance.render.DOColor(whiteClear, 0f);
			psytrance.render.DOColor(Color.white, 0.2f);
			psytrance.render.material.SetFloat("_Mood", 0f);
		}
	}

	private void PsyHot()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			psytrance.render.enabled = true;
			psytrance.render.DOColor(whiteClear, 0f);
			psytrance.render.DOColor(Color.white, 0.2f);
			psytrance.render.material.SetFloat("_Mood", 1f);
		}
	}

	private void PsyRainbow()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			psytrance.render.enabled = true;
			psytrance.render.material.SetFloat("_Mood", 5f);
			psytrance.render.material.SetVector("_Scale", new Vector4(2f, 1f, 0f, 0f));
			nightSky.render.enabled = false;
			nightFake.render.enabled = true;
		}
	}

	private void PsyOff()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			psytrance.render.enabled = false;
		}
	}

	private void PsyFade()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			FadeStuff(psytrance, 0f, beats(2f));
		}
	}

	private SingSing_Glider GetGlider()
	{
		SingSing_Glider result = gliders[gliderptr];
		gliderptr++;
		if (gliderptr >= gliders.Count)
		{
			gliderptr = 0;
		}
		return result;
	}

	private void Glider1()
	{
		FireGlider(-8f, base.camy.transform.position.y + 7f);
	}

	private void Glider2()
	{
		FireGlider(8f, base.camy.transform.position.y + 7f, 1);
	}

	private void Glider3()
	{
		FireGlider(-7f, base.camy.transform.position.y + 7f);
	}

	private void FireGlider(float x = 0f, float y = 0f, int dir = 0)
	{
		if (!lowQual && !GCS.practiceMode)
		{
			SingSing_Glider glider = GetGlider();
			glider.enabled = true;
			glider.transform.position = new Vector3(x, y, 1f);
			glider.dir = dir;
			glider.transform.eulerAngles = Vector3.forward * -90f * dir;
		}
	}

	private Mawaru_Sprite GetSparkle()
	{
		Mawaru_Sprite result = nightSparkleStar[sparkleParticle];
		sparkleParticle++;
		if (sparkleParticle >= nightSparkleStar.Count)
		{
			sparkleParticle = 0;
		}
		return result;
	}

	private Mawaru_Sprite GetGlow()
	{
		Mawaru_Sprite result = nightSparkleGlow[glowParticle];
		glowParticle++;
		if (glowParticle >= nightSparkleGlow.Count)
		{
			glowParticle = 0;
		}
		return result;
	}

	private void Sparkle1()
	{
		SpawnSparkle();
	}

	private void SpawnSparkle()
	{
		if (lowQual || GCS.practiceMode || ADOBase.controller.currentState != States.PlayerControl)
		{
			return;
		}
		GameObject gameObject = null;
		if (curSparkle < nightSparkleWaypoint.Count)
		{
			gameObject = nightSparkleWaypoint[curSparkle];
			spawnPos = gameObject.transform.position;
			numSparklesThisTime = Mathf.FloorToInt(gameObject.transform.localScale.y);
			spawnScale = gameObject.transform.localScale.x;
			float numBeats = 2f;
			float num = 3f;
			Mawaru_Sprite glow = GetGlow();
			glow.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(glow.render.DOColor(whiteClear, 0f))
				.Append(glow.render.DOColor(fadedSparkle, beats(0.3f)))
				.Append(glow.render.DOColor(whiteClear, beats(num - 0.3f)).SetEase(Ease.InQuad));
			glow.transform.DOMove(spawnPos, 0f);
			glow.transform.DOScale(Vector3.one * spawnScale * 0.7f, 0f);
			for (int i = 0; i < numSparklesThisTime; i++)
			{
				randomShift = Vector3.right * RandSpread(1f) + Vector3.up * RandSpread(1f);
				Mawaru_Sprite sparkle = GetSparkle();
				float num2 = RandF(0.2f, 0.4f);
				sparkle.render.enabled = true;
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sparkle.render.DOColor(fadedSparkle, 0f))
					.Append(sparkle.render.DOColor(whiteClear, beats(numBeats)).SetEase(Ease.InQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sparkle.transform.DOMove(spawnPos + randomShift, 0f))
					.Append(sparkle.transform.DOMove(randomShift * RandF(0.5f, 2f), beats(numBeats)).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sparkle.transform.DORotate(Vector3.forward * RandSpread(90f), 0f))
					.Append(sparkle.transform.DORotate(Vector3.forward * RandSpread(90f), beats(numBeats)).SetEase(Ease.OutCubic));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(sparkle.transform.DOScale(Vector3.one * num2, 0f))
					.Append(sparkle.transform.DOScale(Vector3.one * num2 * 1.2f, beats(numBeats)).SetEase(Ease.OutCubic));
			}
			curSparkle++;
		}
	}

	private void ShurikenUpdate()
	{
		if (!slumpo)
		{
			return;
		}
		beat = (float)songBeat;
		if (beat < 153f)
		{
			amp = 0.1f;
		}
		else if (beat < 158f)
		{
			amp = 0.3f;
		}
		else if (beat < 163f)
		{
			amp = 0.5f;
		}
		else if (beat < 168f)
		{
			amp = 0.6f;
		}
		else if (beat < 173f)
		{
			amp = 0.7f;
		}
		else if (beat < 178f)
		{
			amp = 0.8f * (1f - (beat - 173f) / 5f);
		}
		else
		{
			amp = 0f;
		}
		drunkAmt = Mathf.Sin((beat - 148f) * (float)Math.PI * 0.2f);
		for (int i = drunkStartFloor; i < drunkEndFloor; i++)
		{
			if (i < 296 || (i > 297 && i < 312) || (i > 313 && i < 328) || (i > 329 && i < 344) || (i > 345 && i < 360) || (i > 361 && i < 376))
			{
				scrFloor floor = scrLevelMaker.instance.listFloors[i];
				FloorResetPosition(floor);
				FloorDrunkVibe(floor, drunkAmt * amp, 0f, 4f, 0f, 2f);
				FloorDrunkVibe(floor, drunkAmt * amp, (float)Math.PI / 2f, 4f, (float)Math.PI / 2f, 2f);
			}
		}
	}

	private void NoDrunk()
	{
		if (slumpo)
		{
			for (int i = drunkStartFloor; i < drunkEndFloor; i++)
			{
				scrFloor floor = scrLevelMaker.instance.listFloors[i];
				FloorResetPosition(floor);
				FloorDrunkVibeXPos(floor, 0f);
			}
		}
	}

	private void ClockSpawn()
	{
		clockContainer.transform.DOScale(Vector3.one * 1.35f, beats(1.5f)).SetEase(Ease.InOutCubic);
		clockHands[0].transform.DORotate(Vector3.forward * -165f, 0f);
		clockHands[1].transform.DORotate(Vector3.forward * -13.75f, 0f);
		clockHands[2].transform.DORotate(Vector3.forward * -165f, 0f);
		clockHands[3].transform.DORotate(Vector3.forward * -13.75f, 0f);
		if (!lowQual)
		{
			EnableStuff(clouds);
		}
	}

	private void ClockBegin()
	{
		nightStarfield1.transform.DORotate(Vector3.forward * 120f, beats(7f), RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
		nightStarfield2.transform.DORotate(Vector3.forward * 120f, beats(7f), RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
		morning.render.color = whiteClear;
		morning.render.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(morning.render.DOColor(Color.white, beats(7f)).SetEase(Ease.InQuad));
		if (!lowQual)
		{
			FadeStuff(clouds, 0.6f, beats(16f));
		}
		clockHands[2].transform.DORotate(Vector3.forward * -2250f, beats(7f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
		clockHands[3].transform.DORotate(Vector3.forward * -2250f / 12f, beats(7f), RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
	}

	private void ClockEnd()
	{
		if (ADOBase.controller.currentState == States.PlayerControl)
		{
			clockContainer.transform.DOLocalMoveY(2.25f, beats(1f)).SetRelative(isRelative: true).SetEase(Ease.OutSine);
		}
		for (int i = 0; i < 2; i++)
		{
			closingEyes[i].render.enabled = true;
		}
	}

	private void ClockTick()
	{
		clockHands[0].transform.DORotate(Vector3.forward * -30f, beats(7f / 24f)).SetRelative(isRelative: true).SetEase(Ease.OutBack);
		clockHands[1].transform.DORotate(Vector3.forward * -30f / 12f, beats(7f / 24f)).SetRelative(isRelative: true).SetEase(Ease.OutBack);
	}

	private float ic(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t /= d;
		return c * Mathf.Pow(t, 3f) + b;
	}

	private void EndingUpdate()
	{
		beat = (float)songBeat;
		eperc = 0.8f - (225f - beat) / 32f * 0.5f;
		sval = ic((beat - 193f) * 0.25f - Mathf.Floor((beat - 193f) * 0.25f));
		for (int i = 0; i < 2; i++)
		{
			Mawaru_Sprite mawaru_Sprite = closingEyes[i];
			edir = ((float)i * 2f - 1f) * -1f;
			bhei = 10f * edir - sval * eperc * 10f * edir;
			mawaru_Sprite.transform.localPosition = Vector3.up * bhei + Vector3.forward;
		}
	}

	private void MoveFinalText()
	{
		scrUIController.instance.txtCongrats.transform.DOLocalMoveY(160f, 0f);
		scrUIController.instance.txtAllStrictClear.transform.DOLocalMoveY(260f, 0f);
	}

	private void EndingFades()
	{
	}

	private void Beeps()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			wflash.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), 0f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.6f, 69f)).SetEase(Ease.InQuad))
				.AppendInterval(beats(0.4f, 69f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), 0f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.6f, 69f)).SetEase(Ease.InQuad))
				.AppendInterval(beats(0.4f, 69f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), 0f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.6f, 69f)).SetEase(Ease.InQuad))
				.AppendInterval(beats(0.4f, 69f))
				.Append(wflash.render.DOColor(new Color(1f, 1f, 1f, 0.6f), 0f))
				.Append(wflash.render.DOColor(whiteClear, 1f).SetEase(Ease.Linear).OnComplete(delegate
				{
					wflash.render.enabled = false;
				}));
		}
	}

	private void BeepsLow()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			wflash.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), 0f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), beats(0.6f, 69f)).SetEase(Ease.InQuad))
				.AppendInterval(beats(0.4f, 69f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), 0f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), beats(0.6f, 69f)).SetEase(Ease.InQuad))
				.AppendInterval(beats(0.4f, 69f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), 0f))
				.Append(wflash.render.DOColor(new Color(0f, 0f, 0f, 0.5f), beats(0.6f, 69f)).SetEase(Ease.InQuad))
				.AppendInterval(beats(0.4f, 69f))
				.Append(wflash.render.DOColor(new Color(1f, 1f, 1f, 0.5f), 0f))
				.Append(wflash.render.DOColor(whiteClear, 1f).SetEase(Ease.Linear).OnComplete(delegate
				{
					wflash.render.enabled = false;
				}));
			for (int num = 0; num < 2; num++)
			{
				int index = num;
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(3f, 69f))
					.Append(closingEyes[index].render.DOColor(whiteClear, 1f).SetEase(Ease.Linear));
			}
		}
	}

	private void WhiteFlash()
	{
		if (!lowQual && !GCS.practiceMode)
		{
			wflashanim = true;
			wflashbg.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(wflashbg.render.DOColor(Color.white, beats(2f, 114f)))
				.Append(wflashbg.render.DOColor(whiteClear, beats(2f, 114f)).SetEase(Ease.Linear).OnComplete(delegate
				{
					wflashbg.render.enabled = false;
				}));
		}
	}

	private void BlackFlashQ()
	{
		if (!lowQual && !GCS.practiceMode && !wflashanim)
		{
			wflashbg.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(wflashbg.render.DOColor(Color.black, beats(0.5f, 114f)))
				.Append(wflashbg.render.DOColor(blackClear, beats(4f, 114f)).SetEase(Ease.Linear).OnComplete(delegate
				{
					wflashbg.render.enabled = false;
				}));
		}
	}

	private void HarshFailbar()
	{
		if (songBeat > 34.0 && songBeat < 80.0 && lastCheckedFloor < ADOBase.controller.currFloor.seqID && (ADOBase.controller.currFloor.grade == HitMargin.VeryEarly || ADOBase.controller.currFloor.grade == HitMargin.VeryLate))
		{
			ADOBase.controller.playerOne.failBar.Damage(0.2f);
			lastCheckedFloor = ADOBase.controller.currFloor.seqID;
		}
	}

	private void Reset()
	{
		if (!GCS.practiceMode && ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			for (int i = 0; i < 2; i++)
			{
				closingEyes[i].render.enabled = false;
			}
			ADOBase.controller.playerOne.planetarySystem.isCW = true;
			ADOBase.controller.MoveCameraToTile(scrLevelMaker.instance.listFloors[0], ADOBase.controller.currFloor, 0f, Ease.Linear, 1f);
			ADOBase.controller.currFloor.transform.position = Vector3.zero;
			ADOBase.controller.currFloor.SetOpacity(0f);
			ADOBase.controller.playerOne.planetarySystem.speed = 1.0;
			for (int j = 0; j <= 30; j++)
			{
				scrFloor obj = scrLevelMaker.instance.listFloors[j];
				obj.SetOpacity(1f);
				obj.topGlow.gameObject.SetActive(value: false);
			}
			if (ADOBase.controller.txtCongrats.text.EndsWith("!") || ADOBase.controller.txtCongrats.text.EndsWith("！") || ADOBase.controller.txtCongrats.text.EndsWith("."))
			{
				ADOBase.controller.txtCongrats.text = ADOBase.controller.txtCongrats.text.Remove(ADOBase.controller.txtCongrats.text.Length - 1);
			}
			ADOBase.controller.txtCongrats.text += "...?";
			if (!lowQual)
			{
				EnableStuff(oceansparkle);
				EnableStuff(sky1);
				EnableStuff(sky2);
				EnableStuff(sky2Starfield);
				FadeStuff(oceansparkle, 1f, 0f);
				FadeStuff(sky1, 1f, 0f);
				FadeStuff(sky2, 1f, 0f);
				FadeStuff(sky2Starfield, 1f, 0f);
				nightFake.render.enabled = false;
				nightSky.render.enabled = false;
				psytrance.render.enabled = false;
				morning.render.enabled = false;
				nightStarfield1.render.enabled = false;
				nightStarfield2.render.enabled = false;
				sky2.transform.DOLocalMoveY(6f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
				sky2Starfield.transform.DOLocalMoveY(3f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
				sky1.transform.DOLocalMoveY(6f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
				oceansparkle.transform.DOLocalMoveY(50f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
				oceansparkle.transform.DOLocalMoveZ(40f, 0f).SetRelative(isRelative: true).SetEase(Ease.InOutCubic);
			}
		}
	}

	private void ShowFinalResults()
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && (!GCS.speedTrialMode || !(GCS.currentSpeedTrial >= 1f))) || ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		ADOBase.controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: false);
		int[] array = SaveMedals(slumpo ? "T2EX" : "T2", slumpo ? Singsing2_Stats.sectionStats : Singsing_Stats.sectionStats);
		for (int i = 0; i < endingMedals.Count; i++)
		{
			if (array[i] > 0)
			{
				endingMedals[i].front.SetState(array[i] - 1);
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
		ADOBase.controller.canExitLevel = true;
	}

	private void LightsOffVerySlow()
	{
		lightsOut.render.DOColor(Color.black, beats(6f));
	}

	private void LightsOffSlow()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0f), 0f).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.4f), beats(0.9f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.3f), beats(0.1f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.7f), beats(0.9f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.6f), beats(0.1f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(1f)).SetEase(Ease.Linear))
			.AppendInterval(beats(0.1f))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0f), beats(0.5f)).SetEase(Ease.Linear));
	}

	private void LightsOffSlowLow()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0f), 0f).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.4f), beats(0.9f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.3f), beats(0.1f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.7f), beats(0.9f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0.6f), beats(0.1f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(1f)).SetEase(Ease.Linear));
	}

	private void LightsOnSlowLow()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), 0f).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.9f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.1f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.9f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0.1f)).SetEase(Ease.Linear))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(1f)).SetEase(Ease.Linear))
			.AppendInterval(beats(0.1f))
			.Append(lightsOut.render.DOColor(new Color(0f, 0f, 0f, 0f), beats(0.5f)).SetEase(Ease.Linear));
	}

	private void LightsOn()
	{
		lightsOut.render.DOColor(Color.clear, 0f);
	}

	private void EnableCams(int num)
	{
		myCam.cullingMask = funkyMask;
		foreach (scrPlanetCopyCam cloneCam in cloneCams)
		{
			cloneCam.Disable();
		}
		for (int i = 0; i < num; i++)
		{
			cloneCams[i].Enable();
		}
	}

	private void ThreeCams()
	{
		EnableCams(3);
	}

	private void FourCams()
	{
		EnableCams(4);
	}

	private void SixCams()
	{
		EnableCams(6);
	}

	private void NormalCam()
	{
		myCam.cullingMask = defaultMask;
		foreach (scrPlanetCopyCam cloneCam in cloneCams)
		{
			cloneCam.Disable();
		}
	}

	private void RemoveNonCubicleFloors()
	{
		Debug.Log("Hide the floors!!!");
		scrLevelMaker.instance.holdContainer.SetActive(value: false);
		for (int i = 0; i < scrLevelMaker.instance.listFloors.Count; i++)
		{
			if (i < 69 || i >= 180)
			{
				scrLevelMaker.instance.listFloors[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void ReturnNonCubicleFloors()
	{
		Debug.Log("Unhide the floors!!!");
		scrLevelMaker.instance.holdContainer.SetActive(value: true);
		for (int i = 0; i < scrLevelMaker.instance.listFloors.Count; i++)
		{
			if (i < 69 || i >= 180)
			{
				scrLevelMaker.instance.listFloors[i].gameObject.SetActive(value: true);
			}
		}
	}

	private void CubicleMemes()
	{
		beat = (float)songBeat;
		if (beat >= 58f && beat < 64f)
		{
			float f = 0.17453292f;
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				num = 6f * ((float)i - 1f) + 1.5f - 3f * (beat - 58f) / 6f;
				cloneCams[i].position = Vector3.right * num * Mathf.Cos(f) + Vector3.up * num * Mathf.Sin(f);
			}
		}
		else if (beat >= 64f && beat < 70f)
		{
			float num2 = -(float)Math.PI / 12f;
			float num3 = 0f;
			for (int j = 0; j < 4; j++)
			{
				cloneCams[j].rotationOffset = (0f - num2) * 0.3f * 57.29578f;
				if (j < 2)
				{
					num3 = 7f * ((float)j - 0.5f) + 1.5f - 3f * (beat - 64f) / 6f;
					cloneCams[j].position = Vector3.right * num3 * Mathf.Cos(num2) + Vector3.up * num3 * Mathf.Sin(num2) + Vector3.up * 2.75f;
				}
				else
				{
					num3 = 7f * ((float)j - 2.5f) - 1.5f + 3f * (beat - 64f) / 6f;
					cloneCams[j].position = Vector3.right * num3 * Mathf.Cos(num2) + Vector3.up * num3 * Mathf.Sin(num2) + Vector3.up * -2.75f;
				}
			}
		}
		else
		{
			if (!(beat >= 70f) || !(beat < 76f))
			{
				return;
			}
			float num4 = (float)Math.PI / 12f;
			float num5 = 0f;
			for (int k = 0; k < 6; k++)
			{
				cloneCams[k].rotationOffset = (0f - num4) * 0.6f * 57.29578f;
				if (k < 2)
				{
					num5 = 6f * ((float)k - 0.5f) + 1.5f - 3f * (beat - 70f) / 6f;
					cloneCams[k].position = Vector3.up * num5 * Mathf.Cos(num4) + Vector3.right * num5 * Mathf.Sin(num4) + Vector3.right * -6f;
				}
				else if (k < 4)
				{
					num5 = 6f * ((float)k - 2.5f) - 1.5f + 3f * (beat - 70f) / 6f;
					cloneCams[k].position = Vector3.up * num5 * Mathf.Cos(num4) + Vector3.right * num5 * Mathf.Sin(num4) + Vector3.right * 0f;
				}
				else if (k < 6)
				{
					num5 = 6f * ((float)k - 4.5f) + 1.5f - 3f * (beat - 70f) / 6f;
					cloneCams[k].position = Vector3.up * num5 * Mathf.Cos(num4) + Vector3.right * num5 * Mathf.Sin(num4) + Vector3.right * 6f;
				}
			}
		}
	}
}
