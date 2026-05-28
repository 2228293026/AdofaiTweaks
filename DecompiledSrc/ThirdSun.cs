using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ThirdSun : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	public List<scrPlanetCopyCam> cloneCams;

	public List<GameObject> BGsToStretch;

	private int lastJudgedSection = -1;

	private int judgingSection = -1;

	private bool cursection_hasMadeMistakes;

	private bool cursection_hasNonPerfects;

	public List<GameObject> endingStuff;

	public List<Mawaru_Medal> endingMedals;

	public List<float> endingStuff_originalSizes = new List<float>();

	private List<int> sectionFloors = new List<int>();

	public List<SpriteRenderer> sef_frameDump = new List<SpriteRenderer>();

	private List<Color> RankingTextColors = new List<Color>
	{
		new Color(1f, 1f, 1f, 1f),
		new Color(0.6f, 0.4f, 0f, 1f),
		new Color(0.7f, 0.7f, 0.7f, 1f),
		new Color(1f, 0.8f, 0f, 1f)
	};

	public float localTimer;

	public GameObject sky;

	public GameObject bgstuff;

	public GameObject vortex;

	public Sef sef;

	public GameObject floatSefContainer;

	public Mawaru_Sprite floatSef;

	public GameObject fishBox;

	public GameObject beatBox;

	private Transform fishAux;

	private Transform beatAux;

	public MeshRenderer miasma1;

	public MeshRenderer miasma2;

	public List<Transform> mPulse = new List<Transform>();

	public List<Transform> tPulse = new List<Transform>();

	public List<float> mPulseB = new List<float>();

	public List<float> tPulseB = new List<float>();

	public List<Mawaru_Sprite> totems = new List<Mawaru_Sprite>();

	private List<Tuple<int, int>> beatFloors = new List<Tuple<int, int>>();

	private List<Tuple<int, int>> beatFloors2 = new List<Tuple<int, int>>();

	public SpriteRenderer portalBeam;

	public List<SpriteRenderer> ligmae;

	private Color alpha = new Color(0f, 0f, 0f, 1f);

	private List<FloorMeshRenderer> animatedFloors = new List<FloorMeshRenderer>();

	private List<FloorMeshRenderer> animatedFloors2 = new List<FloorMeshRenderer>();

	private List<scrFloor> animatedFloors2parents = new List<scrFloor>();

	private List<float> angleList = new List<float> { 120f, 0f, 0f, 0f };

	private List<float> angleList2 = new List<float> { 60f, 60f, 60f, 60f, 60f, 60f };

	private List<double> bgPulsing = new List<double>
	{
		32.0, 0.0, 33.5, 0.0, 34.0, 1.0, 35.0, 0.0, 37.0, 0.0,
		38.0, 1.0, 40.0, 0.0, 41.5, 0.0, 42.0, 1.0, 43.0, 0.0,
		45.0, 0.0, 46.0, 1.0, 48.0, 0.0, 49.5, 0.0, 50.0, 1.0,
		51.0, 0.0, 53.0, 0.0, 54.0, 1.0, 56.0, 0.0, 57.5, 0.0,
		58.0, 1.0, 59.0, 0.0, 61.0, 0.0, 62.0, 1.0, 68.0, 0.0,
		69.0, 0.0, 70.0, 1.0, 72.0, 0.0, 73.5, 0.0, 74.0, 1.0,
		75.0, 0.0, 77.0, 0.0, 78.0, 1.0, 80.0, 0.0, 81.5, 0.0,
		82.0, 1.0, 83.0, 0.0, 85.0, 0.0, 86.0, 1.0, 88.0, 0.0,
		89.5, 0.0, 90.0, 1.0, 91.0, 0.0, 381.0, 0.0, 382.0, 1.0,
		383.0, 0.0, 383.5, 0.0, 384.0, 1.0, 386.0, 1.0, 387.0, 0.0,
		388.0, 1.0, 389.0, 0.0, 390.0, 1.0, 391.0, 0.0, 392.0, 1.0,
		393.0, 0.0, 394.0, 1.0, 394.5, 0.0, 395.5, 0.0, 396.0, 1.0,
		397.0, 0.0, 397.0, 1.0
	};

	private int curBgPulse;

	private Transform camParent;

	private bool lowVfx;

	private bool lowQual;

	public List<GameObject> highQualBG;

	private float fIntensity;

	private float pAmt;

	private float fishDefault = -0.032f;

	private float fishLow = -0.01f;

	private float fishHigh = -0.1f;

	private float fishIntro = -0.08f;

	private float beat;

	private float amp;

	private int driftFloorStart = 208;

	private int driftFloorEnd = 231;

	private float osc;

	private float rot;

	private float xpos;

	private float ypos;

	private float amp2;

	private float slummult = 0.5f;

	private float slummult2 = 0.5f;

	private Color floorColorAnim = new Color(0.702f, 0.329f, 0.329f, 1f);

	private Color floorColorAnimEnd = new Color(0.443f, 0.443f, 0.443f, 1f);

	private bool sf2anim;

	private Vector3 sefStartPos = new Vector3(12f, 88f, 10f);

	private Vector3 sefEndPos = new Vector3(74f, 91f, 10f);

	private Vector3 sefStartPos2 = new Vector3(74f, 80.5f, 10f);

	private Vector3 sefEndPos2 = new Vector3(64f, 80f, 10f);

	private bool shouldFloat;

	private List<float> ligRad = new List<float>();

	private List<float> ligOff = new List<float>();

	private List<float> ligSpeed = new List<float>();

	private List<float> ligY = new List<float>();

	private float rypos;

	private float alp;

	private Dictionary<Filter, MonoBehaviour> filterToComp => scrVfxPlus.instance.filterToComp;

	private void JudgeSection(int section, int grade, bool showText = true, int floor = 0, float xpos = 0f, float ypos = 2f, float ang = 0f)
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && GCS.currentSpeedTrial < 1f))
		{
			return;
		}
		if (slumpo)
		{
			ThirdSun2_Stats.sectionStats[section] = grade;
		}
		else
		{
			ThirdSun_Stats.sectionStats[section] = grade;
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
			sectionJudgment.transform.position = position + Vector3.up * ypos + Vector3.right * xpos;
			sectionJudgment.transform.eulerAngles = Vector3.forward * ang;
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
				float num = 0f;
				float num2 = 2f;
				float ang = 0f;
				switch (i)
				{
				case 0:
					num2 = 3f;
					break;
				case 3:
					ang = 180f;
					break;
				case 4:
				case 5:
					ang = 180f;
					num2 = -2f;
					break;
				case 6:
					num = -2f;
					break;
				}
				int grade = 3;
				if (cursection_hasNonPerfects)
				{
					grade = 2;
				}
				if (cursection_hasMadeMistakes)
				{
					grade = 1;
				}
				JudgeSection(i, grade, showText: true, sectionFloors[i], num, num2, ang);
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

	private void DisableStuff()
	{
		if (!lowQual)
		{
			return;
		}
		foreach (GameObject item in highQualBG)
		{
			item.SetActive(value: false);
		}
	}

	private new void Awake()
	{
		base.Awake();
		lowVfx = ADOBase.controller.visualEffects == VisualEffects.Minimum;
		lowQual = ADOBase.controller.visualQuality == VisualQuality.Low;
		if (!lowVfx)
		{
			base.camy.GetComponent<CameraFilterPackLegacy_Distortion_FishEye>().enabled = true;
		}
		if (!slumpo)
		{
			if (!ThirdSun_Stats.init)
			{
				ThirdSun_Stats.Reset();
			}
		}
		else if (!ThirdSun2_Stats.init)
		{
			ThirdSun2_Stats.Reset();
		}
		if (!slumpo)
		{
			GCS.pauseMedalStatsCurrent = ThirdSun_Stats.sectionStats;
		}
		else
		{
			GCS.pauseMedalStatsCurrent = ThirdSun2_Stats.sectionStats;
		}
		mb(0f, base.SetResultTextPos, 9999f);
		mb(-200f, SetupFonts, 99999f);
		mb(-199f, DisableStuff, 9999f);
		for (int i = 0; i < endingStuff.Count; i++)
		{
			GameObject gameObject = endingStuff[i];
			endingStuff_originalSizes.Add(gameObject.transform.localScale.x);
			gameObject.transform.localScale = Vector3.zero;
		}
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.78f)
		{
			foreach (GameObject item in BGsToStretch)
			{
				item.transform.localScale = new Vector3(item.transform.localScale.x * (num / 1.7777778f), item.transform.localScale.y * (num / 1.7777778f), item.transform.localScale.z);
			}
		}
		for (int j = 0; j < mPulse.Count; j++)
		{
			mPulseB.Add(mPulse[j].localScale.y);
		}
		for (int k = 0; k < tPulse.Count; k++)
		{
			tPulseB.Add(tPulse[k].localScale.y);
		}
		if (slumpo && !lowVfx && !lowQual)
		{
			foreach (int item2 in new List<int>
			{
				79, 111, 149, 185, 276, 286, 298, 356, 366, 376,
				395, 405
			})
			{
				scrLevelMaker.instance.listFloors[item2].gameObject.AddComponent<ffxHotTile>();
			}
			foreach (int item3 in new List<int> { 81, 113, 151, 187, 350, 370, 389 })
			{
				scrLevelMaker.instance.listFloors[item3].gameObject.AddComponent<ffxColdTile>();
			}
		}
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 150.0));
		bpms.Add(new Tuple<double, double>(96.0, 75.0));
		bpms.Add(new Tuple<double, double>(104.0, 150.0));
		bpms.Add(new Tuple<double, double>(120.0, 75.0));
		bpms.Add(new Tuple<double, double>(122.0, 150.0));
		bpms.Add(new Tuple<double, double>(238.0, 112.5));
		bpms.Add(new Tuple<double, double>(250.0, 56.25));
		bpms.Add(new Tuple<double, double>(252.0, 112.5));
		bpms.Add(new Tuple<double, double>(255.0, 225.0));
		bpms.Add(new Tuple<double, double>(313.0, 112.5));
		bpms.Add(new Tuple<double, double>(317.0, 225.0));
		bpms.Add(new Tuple<double, double>(381.0, 112.5));
		fishAux = fishBox.transform;
		beatAux = beatBox.transform;
		if (!slumpo)
		{
			sectionFloors = new List<int> { 47, 172, 239, 359, 423, 501, 572, 600 };
			beatFloors.Add(new Tuple<int, int>(442, 451));
			beatFloors.Add(new Tuple<int, int>(458, 467));
			beatFloors.Add(new Tuple<int, int>(474, 481));
			beatFloors.Add(new Tuple<int, int>(493, 500));
			beatFloors.Add(new Tuple<int, int>(505, 511));
			beatFloors.Add(new Tuple<int, int>(521, 527));
			beatFloors.Add(new Tuple<int, int>(537, 543));
			beatFloors.Add(new Tuple<int, int>(555, 562));
			beatFloors2.Add(new Tuple<int, int>(572, 588));
			for (int l = 40; l < 44; l++)
			{
				animatedFloors.Add(scrLevelMaker.instance.listFloors[l].gameObject.GetComponent<FloorMeshRenderer>());
			}
			if (!slumpo)
			{
				for (int m = 423; m < 429; m++)
				{
					animatedFloors2.Add(scrLevelMaker.instance.listFloors[m].gameObject.GetComponent<FloorMeshRenderer>());
					animatedFloors2parents.Add(scrLevelMaker.instance.listFloors[m]);
				}
			}
			else
			{
				for (int n = 479; n < 485; n++)
				{
					animatedFloors2.Add(scrLevelMaker.instance.listFloors[n].gameObject.GetComponent<FloorMeshRenderer>());
					animatedFloors2parents.Add(scrLevelMaker.instance.listFloors[n]);
				}
			}
		}
		else
		{
			sectionFloors = new List<int> { 53, 192, 259, 414, 479, 563, 645, 673 };
			beatFloors.Add(new Tuple<int, int>(498, 509));
			beatFloors.Add(new Tuple<int, int>(517, 528));
			beatFloors.Add(new Tuple<int, int>(536, 543));
			beatFloors.Add(new Tuple<int, int>(555, 562));
			beatFloors.Add(new Tuple<int, int>(569, 576));
			beatFloors.Add(new Tuple<int, int>(588, 595));
			beatFloors.Add(new Tuple<int, int>(607, 623));
			beatFloors.Add(new Tuple<int, int>(626, 633));
			beatFloors2.Add(new Tuple<int, int>(645, 661));
			for (int num2 = 40; num2 < 44; num2++)
			{
				animatedFloors.Add(scrLevelMaker.instance.listFloors[num2].gameObject.GetComponent<FloorMeshRenderer>());
			}
			for (int num3 = 479; num3 < 485; num3++)
			{
				animatedFloors2.Add(scrLevelMaker.instance.listFloors[num3].gameObject.GetComponent<FloorMeshRenderer>());
				animatedFloors2parents.Add(scrLevelMaker.instance.listFloors[num3]);
			}
		}
		GCS.pauseMedalFloors = new List<int>();
		GCS.pauseMedalFloors.Clear();
		GCS.pauseMedalFloors.Add(0);
		for (int num4 = 0; num4 < sectionFloors.Count - 1; num4++)
		{
			GCS.pauseMedalFloors.Add(sectionFloors[num4]);
		}
		mb(26f, Bounce47, 27f);
		mb(170f, Bounce335, 172f);
		if (!lowVfx)
		{
			mb(98.97f, CameraPipes1, 104f);
			mb(102.97f, CameraPipes1, 104f);
			mb(109.94f, CameraPipes1b, 111f);
			mb(221f, CameraOyOyOy, 223f);
		}
		mb(116f, FishStart, 122f);
		for (float num5 = 126f; num5 < 170f; num5 += 2f)
		{
			mb(num5, DoFish, num5 + 1f);
		}
		for (float num6 = 174f; num6 <= 184f; num6 += 2f)
		{
			mb(num6, DoFish, num6 + 1f);
		}
		mb(193f, DoFishYEOW, 195f);
		mb(209f, DoFishYEOW, 211f);
		for (float num7 = 198f; num7 <= 204f; num7 += 2f)
		{
			mb(num7, DoFishSmall, num7 + 1f);
		}
		for (float num8 = 214f; num8 <= 218f; num8 += 2f)
		{
			mb(num8, DoFishSmall, num8 + 1f);
		}
		for (float num9 = 0f; num9 <= 11f; num9 += 1f)
		{
			mb(222f + num9 * 1.3333334f, DoFish112, 223f + num9 * 1.3333334f);
		}
		mb(238f, DoFishEnd, 240f);
		mpf(104f, DriftyMidspins, 113f);
		if (!lowVfx)
		{
			mb(270.5f, CameraPipes2, 272f);
			mb(286.5f, CameraPipes2, 288f);
			mb(384.75f, CameraEndStab, 386f);
			mb(254.75f, EnableBeat1, 256f);
			mb(272.5f, EnableBeat2, 274f);
			mb(288.5f, EnableBeat3, 290f);
			mb(304.5f, EnableBeat3, 306f);
			mb(316.75f, EnableBeat3, 318f);
			mb(332.5f, EnableBeat3, 334f);
			mb(348.5f, EnableBeat3, 350f);
			mb(364.5f, EnableBeat3, 366f);
		}
		if (!GCS.practiceMode)
		{
			mb(170f, RotateSky1, 172f);
			mb(313f, RotateSky2, 315f);
		}
		else
		{
			mpf(170f, RotateSky1f, 174f);
			mpf(313f, RotateSky2f, 317f);
		}
		mb(175f, RotateSky1Q, 312f);
		mb(318f, RotateSky2Q, 999f);
		mb(238f, TotemsVisible, 999f);
		mb(399f, ShowFinalResults, 999f);
		mb(397f, TweenEndingPlatform, 999f);
		mb(-101f, SetUpSef, 999f);
		mb(-101f, ClearMiasma1, 999f);
		mb(-101f, ClearMiasma2, 999f);
		mb(-101f, FlattenFloors, 14f);
		mb(16f, SetupFloors, 18f);
		mb(214f, CurlUpFloors2, 233f);
		mb(233f, SetupFloors2, 235f);
		mb(235f, SetupFloors2Q, 999f);
		mb(15f, SefStart, 18f);
		mpf(120f, SefFloat, 185f);
		mb(80f, DoSefFloat, 159f);
		mb(230f, SefStart2, 235f);
		mb(393f, SefStart3, 395f);
		mb(300f, PrepareLigma, 999f);
		mpf(301f, AnimateLigma, 999f);
		if (slumpo)
		{
			driftFloorStart = 228;
			driftFloorEnd = 251;
		}
		SortTables();
	}

	private void Start()
	{
		camParent = base.camy.transform.parent;
		foreach (SpriteRenderer item in sef_frameDump)
		{
			item.enabled = false;
		}
		if (slumpo)
		{
			ADOBase.controller.caption = ADOBase.GetLocalizedLevelName("T4-X").Replace("-X", "-EX");
		}
	}

	private new void Update()
	{
		base.Update();
		Grading();
		localTimer += Time.deltaTime;
		vortex.transform.eulerAngles = Vector3.forward * localTimer * 10f;
		if (!lowVfx)
		{
			fIntensity = fishAux.position.x + 0.5f;
			if (fishAux.position.x > 0f)
			{
				fIntensity = fishAux.position.x * 2f + 0.5f;
			}
			(filterToComp[Filter.Fisheye] as CameraFilterPackLegacy_Distortion_FishEye).Distortion = fIntensity;
			CameraFilterPackLegacy_Distortion_FishEye.ChangeDistortion = fIntensity;
		}
		if (songBeat > 122.0 && songBeat < 186.0 && !lowVfx)
		{
			FloatyCam();
		}
		if (songBeat > 254.75 && songBeat < 392.5 && !lowVfx)
		{
			BeatSpam();
		}
		beat = (float)songBeat;
		if (beat > 16f && beat < 24f && ADOBase.controller.chosenPlanet.currfloor.seqID < 40)
		{
			UpdateFloorAngles(animatedFloors, angleList, 240f);
		}
		if (beat > 233f && beat < 240f)
		{
			UpdateFloorAngles(animatedFloors2, angleList2, 180f);
			foreach (scrFloor animatedFloors2parent in animatedFloors2parents)
			{
				animatedFloors2parent.SetColor(floorColorAnim);
			}
		}
		while (curBgPulse < bgPulsing.Count - 1 && (double)beat > bgPulsing[curBgPulse] && !lowVfx && !lowQual)
		{
			if ((double)beat < bgPulsing[curBgPulse] + 1.0)
			{
				if (bgPulsing[curBgPulse + 1] == 0.0)
				{
					for (int i = 0; i < mPulse.Count; i++)
					{
						pAmt = (0.4f - 0.1f * (float)i) * 1f;
						int index = i;
						DOTween.Sequence().Append(mPulse[index].DOScaleY(mPulseB[index] + pAmt, 0f)).SetEase(Ease.Linear)
							.Append(mPulse[index].DOScaleY(mPulseB[index], beats(1f)).SetEase(Ease.OutCubic));
					}
				}
				else if (bgPulsing[curBgPulse + 1] == 1.0)
				{
					for (int j = 0; j < tPulse.Count; j++)
					{
						pAmt = (0.3f - 0.1f * (float)j) * 0.6f;
						int index2 = j;
						DOTween.Sequence().Append(tPulse[index2].DOScaleY(tPulseB[index2] + pAmt, 0f)).SetEase(Ease.Linear)
							.Append(tPulse[index2].DOScaleY(tPulseB[index2], beats(1f)).SetEase(Ease.OutCubic));
					}
				}
			}
			curBgPulse += 2;
		}
	}

	private void FishStart()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fishAux.DOMoveX(fishIntro, beats(4f)).SetEase(Ease.Linear))
			.Append(fishAux.DOMoveX(fishLow, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault, beats(2f)).SetEase(Ease.OutCubic));
	}

	private void FishStartQ()
	{
		fishAux.DOMoveX(fishDefault, 0f).SetEase(Ease.OutCubic);
	}

	private void DoFish()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fishAux.DOMoveX(fishLow, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault, beats(1.5f)).SetEase(Ease.OutCubic));
	}

	private void DoFishSmall()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fishAux.DOMoveX(fishLow, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault, beats(1.5f)).SetEase(Ease.InCubic));
	}

	private void DoFishYEOW()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fishAux.DOMoveX(fishHigh, beats(1.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault, beats(1.5f)).SetEase(Ease.InCubic));
	}

	private void DoFish112()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fishAux.DOMoveX(fishLow, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault, beats(0.75f, 112.5f)).SetEase(Ease.OutCubic));
	}

	private void DoFishEnd()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault * 1f, beats(1f, 112.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault * (5f / 6f), beats(1f, 112.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault * (2f / 3f), beats(1f, 112.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault * 0.5f, beats(1f, 112.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault * (1f / 3f), beats(1f, 112.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(fishDefault * (1f / 6f), beats(1f, 112.5f)).SetEase(Ease.OutCubic))
			.Append(fishAux.DOMoveX(0f, 0f).SetEase(Ease.OutCubic));
	}

	private void Bounce47()
	{
		if (!slumpo)
		{
			scrFloor scrFloor2 = scrLevelMaker.instance.listFloors[47];
			scrFloor scrFloor3 = scrLevelMaker.instance.listFloors[48];
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(2f))
				.Append(scrFloor2.transform.DOLocalMoveY(1f, beats(0.5f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
				.Append(scrFloor2.transform.DOLocalMoveY(-1f, beats(0.5f)).SetEase(Ease.InCubic).SetRelative(isRelative: true))
				.Append(scrFloor3.transform.DOLocalMoveY(1f, beats(0.5f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
				.Append(scrFloor3.transform.DOLocalMoveY(-1f, beats(0.5f)).SetEase(Ease.InCubic).SetRelative(isRelative: true));
		}
		if (slumpo)
		{
			List<int> list = new List<int> { 47, 49 };
			List<int> list2 = new List<int> { 50, 52 };
			for (int i = 0; i < 2; i++)
			{
				scrFloor scrFloor4 = scrLevelMaker.instance.listFloors[list[i]];
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(2f))
					.Append(scrFloor4.transform.DOLocalMoveY(1f, beats(0.5f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
					.Append(scrFloor4.transform.DOLocalMoveY(-1f, beats(0.5f)).SetEase(Ease.InCubic).SetRelative(isRelative: true));
				scrFloor scrFloor5 = scrLevelMaker.instance.listFloors[list2[i]];
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(3f))
					.Append(scrFloor5.transform.DOLocalMoveY(1f, beats(0.5f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
					.Append(scrFloor5.transform.DOLocalMoveY(-1f, beats(0.5f)).SetEase(Ease.InCubic).SetRelative(isRelative: true));
			}
		}
	}

	private void Bounce335()
	{
		if (!slumpo)
		{
			scrFloor scrFloor2 = scrLevelMaker.instance.listFloors[335];
			scrFloor scrFloor3 = scrLevelMaker.instance.listFloors[336];
			scrFloor scrFloor4 = scrLevelMaker.instance.listFloors[337];
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(scrFloor2.transform.DOLocalMove(new Vector3(0.25f, 0.433f, 0f), beats(0.666f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
				.Append(scrFloor2.transform.DOLocalMove(new Vector3(-0.25f, -0.433f, 0f), beats(0.666f)).SetEase(Ease.InCubic).SetRelative(isRelative: true))
				.Append(scrFloor3.transform.DOLocalMoveX(0.5f, beats(0.666f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
				.Append(scrFloor3.transform.DOLocalMoveX(-0.5f, beats(0.666f)).SetEase(Ease.InCubic).SetRelative(isRelative: true))
				.Append(scrFloor4.transform.DOLocalMove(new Vector3(0.25f, -0.433f, 0f), beats(0.666f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
				.Append(scrFloor4.transform.DOLocalMove(new Vector3(-0.25f, 0.433f, 0f), beats(0.666f)).SetEase(Ease.InCubic).SetRelative(isRelative: true));
		}
		if (slumpo)
		{
			List<int> list = new List<int> { 379, 381 };
			List<int> list2 = new List<int> { 382, 384 };
			List<int> list3 = new List<int> { 385, 387 };
			for (int i = 0; i < 2; i++)
			{
				scrFloor scrFloor5 = scrLevelMaker.instance.listFloors[list[i]];
				scrFloor scrFloor6 = scrLevelMaker.instance.listFloors[list2[i]];
				scrFloor scrFloor7 = scrLevelMaker.instance.listFloors[list3[i]];
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(scrFloor5.transform.DOLocalMove(new Vector3(0.25f, 0.433f, 0f), beats(0.666f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
					.Append(scrFloor5.transform.DOLocalMove(new Vector3(-0.25f, -0.433f, 0f), beats(0.666f)).SetEase(Ease.InCubic).SetRelative(isRelative: true))
					.Append(scrFloor6.transform.DOLocalMoveX(0.5f, beats(0.666f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
					.Append(scrFloor6.transform.DOLocalMoveX(-0.5f, beats(0.666f)).SetEase(Ease.InCubic).SetRelative(isRelative: true))
					.Append(scrFloor7.transform.DOLocalMove(new Vector3(0.25f, -0.433f, 0f), beats(0.666f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
					.Append(scrFloor7.transform.DOLocalMove(new Vector3(-0.25f, 0.433f, 0f), beats(0.666f)).SetEase(Ease.InCubic).SetRelative(isRelative: true));
			}
		}
	}

	private void CameraPipes1()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.03f)).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(0.47f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(0.03f)).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.47f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
	}

	private void CameraPipes1b()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.06f)).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(0.94f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(0.06f)).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.94f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
	}

	private void CameraOyOyOy()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.333f)).SetEase(Ease.OutExpo).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -20f, beats(0.333f)).SetEase(Ease.OutExpo).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * 20f, beats(0.333f)).SetEase(Ease.OutExpo).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(1f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
	}

	private void CameraPipes2()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(0f))
			.Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.05f)).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(1.2f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
			.AppendInterval(beats(0.25f))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(0.05f)).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(1.2f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
	}

	private void CameraEndStab()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.camy.transform.DORotate(Vector3.forward * 10f, 0f).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, beats(0.5f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * -10f, 0f).SetEase(Ease.Linear).SetRelative(isRelative: true))
			.Append(base.camy.transform.DORotate(Vector3.forward * 10f, beats(0.5f)).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
	}

	private void RotateSky1()
	{
		if (!lowQual)
		{
			sky.transform.DOLocalRotate(Vector3.forward * 180f, beats(4f, 150f)).SetEase(Ease.InOutCubic);
			bgstuff.transform.DOLocalRotate(Vector3.forward * 180f, beats(4f, 150f)).SetEase(Ease.InOutCubic);
		}
	}

	private void RotateSky1f()
	{
		if (!lowQual)
		{
			sky.transform.localEulerAngles = -base.camy.transform.eulerAngles;
			bgstuff.transform.localEulerAngles = -base.camy.transform.eulerAngles;
		}
	}

	private void RotateSky1Q()
	{
		if (!lowQual)
		{
			sky.transform.DOLocalRotate(Vector3.forward * 180f, beats(0f)).SetEase(Ease.InOutCubic);
			bgstuff.transform.DOLocalRotate(Vector3.forward * 180f, beats(0f)).SetEase(Ease.InOutCubic);
		}
	}

	private void RotateSky2()
	{
		if (!lowQual)
		{
			sky.transform.DOLocalRotate(Vector3.forward * 0f, beats(4f, 112.5f)).SetEase(Ease.InOutCubic);
			bgstuff.transform.DOLocalRotate(Vector3.forward * 0f, beats(4f, 112.5f)).SetEase(Ease.InOutCubic);
		}
	}

	private void RotateSky2f()
	{
		if (!lowQual)
		{
			sky.transform.localEulerAngles = -base.camy.transform.eulerAngles;
			bgstuff.transform.localEulerAngles = -base.camy.transform.eulerAngles;
		}
	}

	private void RotateSky2Q()
	{
		if (!lowQual)
		{
			sky.transform.DOLocalRotate(Vector3.forward * 0f, beats(4f, 0f)).SetEase(Ease.InOutCubic);
			bgstuff.transform.DOLocalRotate(Vector3.forward * 0f, beats(4f, 0f)).SetEase(Ease.InOutCubic);
		}
	}

	private void DriftyMidspins()
	{
		beat = (float)songBeat;
		if (beat >= 104f && beat < 110f)
		{
			amp = 0.5f * (beat - 104f) / 6f;
		}
		else if (beat >= 110f && beat < 112f)
		{
			amp = 0.5f - 0.5f * (beat - 110f) / 2f;
		}
		else
		{
			amp = 0f;
		}
		for (int i = driftFloorStart; i <= driftFloorEnd; i++)
		{
			scrFloor floor = scrLevelMaker.instance.listFloors[i];
			FloorResetPosition(floor);
			FloorDrunkVibeXPos(floor, amp, 0f, 4f, 0f, 2f);
		}
	}

	private void EnableBeat1()
	{
		EnableBeat(0.5f, 11f, 4f);
	}

	private void EnableBeat2()
	{
		EnableBeat(0.5f, 9f, 4f);
	}

	private void EnableBeat3()
	{
		EnableBeat(0.5f, 7f, 4f);
	}

	private void EnableBeat(float amt, float dur, float fade)
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(beatAux.DOMoveX(amt, beats(0.25f)).SetEase(Ease.Linear))
				.AppendInterval(beats(dur - fade - 0.25f, 225f))
				.Append(beatAux.DOMoveX(0f, beats(fade, 225f)).SetEase(Ease.Linear));
		}
	}

	private void BeatSpam()
	{
		beat = (float)songBeat;
		if (((double)beat > 380.5 && (double)beat < 384.5) || ((double)beat > 385.5 && (double)beat < 392.5))
		{
			for (int i = beatFloors2[0].Item1; i <= beatFloors2[0].Item2; i++)
			{
				scrFloor floor = scrLevelMaker.instance.listFloors[i];
				FloorResetPosition(floor);
				FloorBeatVibe(floor, 0.3f, -1f, 0f, 1f, 6f);
			}
			return;
		}
		for (int j = 0; j < beatFloors.Count; j++)
		{
			for (int k = beatFloors[j].Item1; k <= beatFloors[j].Item2; k++)
			{
				scrFloor floor2 = scrLevelMaker.instance.listFloors[k];
				FloorResetPosition(floor2);
				FloorBeatVibe(floor2, beatAux.position.x, -1f, 0f, 1f, 50f * beatAux.position.x);
			}
		}
	}

	private float ic(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t /= d;
		return c * Mathf.Pow(t, 3f) + b;
	}

	private float oc(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t = t / d - 1f;
		return c * (Mathf.Pow(t, 3f) + 1f) + b;
	}

	private float ioc(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t = t / d * 2f;
		if (t < 1f)
		{
			return c / 2f * t * t * t + b;
		}
		t -= 2f;
		return c / 2f * (t * t * t + 2f) + b;
	}

	private void FloatyCam()
	{
		beat = (float)songBeat;
		slummult = (slumpo ? 0.6f : 0.3f);
		slummult2 = (slumpo ? 0.2f : 0f);
		osc = Mathf.Sin(beat * 0.5f * (float)Math.PI);
		if (osc > 0f)
		{
			osc = Mathf.Sqrt(osc);
		}
		else
		{
			osc = 0f - Mathf.Sqrt(0f - osc);
		}
		amp = 0f;
		amp2 = 0f;
		if (beat >= 126f && beat < 138f)
		{
			amp = oc(beat - 126f, 0f, 1f, 12f) * slummult;
			amp2 = oc(beat - 126f, 0f, 1f, 12f) * slummult2;
		}
		else if (beat >= 138f && beat < 166f)
		{
			amp = 1f * slummult;
			amp2 = 1f * slummult2;
		}
		else if (beat >= 166f && beat < 170f)
		{
			amp = (1f - (beat - 166f) / 4f) * slummult;
			amp2 = (1f - (beat - 166f) / 4f) * slummult2;
		}
		else if (beat >= 170f && beat < 174f)
		{
			amp = 0f;
			amp2 = 0f;
		}
		else if (beat >= 174f && beat < 178f)
		{
			amp = oc(beat - 174f, 0f, 1f, 4f) * slummult;
			amp2 = oc(beat - 174f, 0f, 1f, 4f) * slummult2;
		}
		else if (beat >= 178f && beat < 182f)
		{
			amp = 1f * slummult;
			amp2 = 1f * slummult2;
		}
		else if (beat >= 182f && beat < 186f)
		{
			amp = (1f - (beat - 182f) / 4f) * slummult;
			amp2 = (1f - (beat - 182f) / 4f) * slummult2;
		}
		rot = 5f * (amp2 * Mathf.Sin(localTimer * 2.5f * (float)Math.PI * 0.25f));
		xpos = amp * (0.5f * Mathf.Sin(localTimer * 2.5f * (float)Math.PI * 0.25f * 0.7f) + 0.5f * Mathf.Sin(localTimer * 2.5f * (float)Math.PI * 0.25f * 0.3f));
		ypos = amp * (0.5f * Mathf.Sin(localTimer * 2.5f * (float)Math.PI * 0.25f * 0.4f) + 0.5f * Mathf.Sin(localTimer * 2.5f * (float)Math.PI * 0.25f * 0.6f));
		ypos += 0.5f * amp * osc;
		if (beat < 170f)
		{
			ypos += 1.5f * amp;
		}
		if (beat >= 174f)
		{
			rot -= 180f;
			xpos -= 1.5f;
		}
		else
		{
			xpos += 2.25f;
		}
		if (beat < 170f || beat > 174f)
		{
			camParent.position = Vector3.right * xpos + Vector3.up * ypos;
			base.camy.transform.eulerAngles = Vector3.forward * rot;
		}
	}

	private void TotemsVisible()
	{
		if (lowQual)
		{
			return;
		}
		foreach (Mawaru_Sprite totem in totems)
		{
			totem.render.enabled = true;
		}
	}

	private void TweenEndingPlatform()
	{
		scrLevelMaker.instance.listFloors.Last().TweenOpacity(0f, beats(2f), Ease.OutCubic);
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(2f))
			.Append(scrLevelMaker.instance.listFloors.Last().transform.DOLocalMoveY(15f, beats(8f)).SetRelative(isRelative: true).SetEase(Ease.InSine));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(2f))
			.Append(scrLevelMaker.instance.listFloors.Last().transform.DOLocalRotate(Vector3.forward * 75f, beats(8f), RotateMode.FastBeyond360).SetEase(Ease.InSine));
	}

	private void SetupFloors()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(miasma1.material.DOColor(whiteClear, "_Color", 0f).SetEase(Ease.Linear))
			.AppendInterval(beats(1f))
			.Append(miasma1.material.DOColor(Color.white, "_Color", beats(2f)))
			.Append(miasma1.material.DOColor(whiteClear, "_Color", beats(4f)).SetEase(Ease.InQuad));
		float[] array = new float[4] { 120f, 60f, 60f, 60f };
		for (int i = 0; i < angleList.Count; i++)
		{
			int j = i;
			float duration = beats(3f - (float)i * 0.2f);
			DOTween.Sequence().AppendInterval(beats(0.5f * (float)j)).Append(DOTween.To(() => angleList[j], delegate(float x)
			{
				angleList[j] = x;
			}, array[j], duration).SetEase(Ease.InOutBack));
		}
	}

	private void SetupFloors2()
	{
		sf2anim = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(miasma2.material.DOColor(whiteClear, "_Color", 0f).SetEase(Ease.Linear))
			.AppendInterval(beats(1f))
			.Append(miasma2.material.DOColor(Color.white, "_Color", beats(2f)))
			.Append(miasma2.material.DOColor(whiteClear, "_Color", beats(4f)).SetEase(Ease.InQuad));
		float[] array = new float[6];
		for (int i = 0; i < angleList2.Count; i++)
		{
			int j = i;
			float duration = beats(4f - (float)i * 0.25f);
			DOTween.Sequence().AppendInterval(beats(0.5f * (float)j)).Append(DOTween.To(() => angleList2[j], delegate(float x)
			{
				angleList2[j] = x;
			}, array[j], duration).SetEase(Ease.InOutBack));
		}
		DOTween.To(() => floorColorAnim, delegate(Color x)
		{
			floorColorAnim = x;
		}, floorColorAnimEnd, beats(5f)).SetEase(Ease.InOutCubic);
	}

	private void SetupFloors2Q()
	{
		if (sf2anim)
		{
			return;
		}
		sf2anim = true;
		float[] array = new float[6];
		for (int i = 0; i < angleList2.Count; i++)
		{
			int j = i;
			DOTween.To(() => angleList2[j], delegate(float x)
			{
				angleList2[j] = x;
			}, array[j], 0f).SetEase(Ease.InOutBack);
		}
		DOTween.To(() => floorColorAnim, delegate(Color x)
		{
			floorColorAnim = x;
		}, floorColorAnimEnd, 0f).SetEase(Ease.InOutCubic);
	}

	private void ClearMiasma1()
	{
		miasma1.material.DOColor(whiteClear, "_Color", 0f).SetEase(Ease.Linear);
	}

	private void ClearMiasma2()
	{
		miasma2.material.DOColor(whiteClear, "_Color", 0f).SetEase(Ease.Linear);
	}

	private void FlattenFloors()
	{
		if (ADOBase.controller.chosenPlanet.currfloor.seqID < 40)
		{
			UpdateFloorAngles(animatedFloors, angleList, 240f);
		}
	}

	private void CurlUpFloors2()
	{
		UpdateFloorAngles(animatedFloors2, angleList2, 180f);
		foreach (scrFloor animatedFloors2parent in animatedFloors2parents)
		{
			animatedFloors2parent.SetColor(floorColorAnim);
		}
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

	private void SefStart()
	{
		sef.AddEntry(new CharlieAction(sef.waypoints[0].localPosition, 0f, "spawn"));
		sef.AddEntry(new CharlieAction(sef.waypoints[0].localPosition, 0.8f, "wait"));
		sef.AddEntry(new CharlieAction(sef.waypoints[0].localPosition, 1.6f, "curl"));
		sef.AddEntry(new CharlieAction(sef.waypoints[0].localPosition, 0f, "hide"));
	}

	private void SefStart2()
	{
		sef.transform.eulerAngles = Vector3.forward * 180f;
		sef.AddEntry(new CharlieAction(sef.waypoints[1].localPosition, 0f, "spawn"));
		sef.AddEntry(new CharlieAction(sef.waypoints[2].localPosition, 1.4f, "wait"));
		sef.AddEntry(new CharlieAction(sef.waypoints[2].localPosition, 1.8f, "uncurl"));
		sef.AddEntry(new CharlieAction(sef.waypoints[2].localPosition, 0f, "hide"));
	}

	private void SefStart3()
	{
		sef.transform.eulerAngles = Vector3.forward * 0f;
		sef.actor.render.flipX = true;
		sef.AddEntry(new CharlieAction(sef.waypoints[3].localPosition, 0f, "spawn"));
	}

	private void SetUpSef()
	{
		if (slumpo)
		{
			sefStartPos = new Vector3(12f, 89f, 10f);
			sefEndPos = new Vector3(71f, 81f, 10f);
			sefStartPos2 = new Vector3(69f, 69f, 10f);
			sefEndPos2 = new Vector3(61f, 69f, 10f);
		}
	}

	private void DoSefFloat()
	{
		if (!GCS.practiceMode)
		{
			shouldFloat = true;
		}
	}

	private void SefFloat()
	{
		float num = (float)songBeat;
		float num2 = (num - 120f) / 50f;
		float num3 = (num - 173f) / 10f;
		float num4 = 0.45f;
		float num5 = ((num > 173f) ? 180f : 0f);
		float num6 = ((num > 173f) ? (-1f) : 1f);
		float num7 = 20f;
		float num8 = 0f;
		Vector3 vector = Vector3.up * 0.02f * Mathf.Sin(localTimer * (float)Math.PI * 2f * 0.8f) + Vector3.right * 0.02f * Mathf.Cos(localTimer * (float)Math.PI * 2f * 0.8f);
		if (num >= 125f && num < 127f)
		{
			num7 = -1f + 11f * (1f - oc(num - 125f, 0f, 1f, 2f));
		}
		else if (num >= 127f && num < 136f)
		{
			num7 = -1f * (1f - ioc(num - 127f, 0f, 1f, 9f));
		}
		else if (num >= 136f && num < 152f)
		{
			num7 = 0f;
		}
		else if (num >= 152f && num < 168f)
		{
			num7 = -2f * ioc(num - 152f, 0f, 1f, 16f);
		}
		else if (num >= 168f && num < 170f)
		{
			num7 = -2f + 10f * ic(num - 168f, 0f, 1f, 2f);
		}
		else if (num >= 170f && num < 173f)
		{
			num7 = 800f;
		}
		else if (num >= 173f && num < 175f)
		{
			num7 = -10f * (1f - oc(num - 173f, 0f, 1f, 2f));
		}
		else if (num >= 175f && num < 181f)
		{
			num7 = 0f;
		}
		else if (num >= 181f && num < 183f)
		{
			num7 = -10f * ic(num - 181f, 0f, 1f, 2f);
		}
		else if (num >= 183f)
		{
			num7 = 800f;
		}
		if (num > 123f && num < 125f)
		{
			num8 = ic(num - 123f, 0f, 1f, 2f);
		}
		else if (num >= 125f && num < 183f)
		{
			num8 = 1f;
		}
		else if (num >= 183f && num < 185f)
		{
			num8 = 1f - oc(num - 183f, 0f, 1f, 2f);
		}
		else if (num >= 185f)
		{
			num8 = 0f;
		}
		if (shouldFloat)
		{
			floatSef.transform.localPosition = Vector3.right * 0.3f * Mathf.Sin(localTimer * 2f * (float)Math.PI * 0.5f) + Vector3.up * num7 + Vector3.up * 0.3f * Mathf.Cos(localTimer * 2f * (float)Math.PI * 0.5f);
			floatSef.transform.localScale = Vector3.right * num4 * num6 + Vector3.up * num4 * num8 + Vector3.forward + vector;
			floatSef.transform.eulerAngles = Vector3.forward * 4f * Mathf.Sin(localTimer * 2f * (float)Math.PI * 0.5f) + Vector3.forward * num5;
			if (num < 173f)
			{
				floatSefContainer.transform.position = sefStartPos + (sefEndPos - sefStartPos) * num2;
			}
			else
			{
				floatSefContainer.transform.position = sefStartPos2 + (sefEndPos2 - sefStartPos2) * num3;
			}
		}
		else
		{
			floatSefContainer.gameObject.SetActive(value: false);
		}
	}

	private void PrepareLigma()
	{
		if (lowQual)
		{
			return;
		}
		foreach (SpriteRenderer item in ligmae)
		{
			ligRad.Add(UnityEngine.Random.Range(0.75f, 2f));
			ligOff.Add(RandF(0f, (float)Math.PI));
			ligSpeed.Add(1.2f + RandF(-0.3f, 0.3f));
			rypos = RandF(-2f, 4f);
			ligY.Add(rypos);
			item.transform.position = portalBeam.transform.position + Vector3.down * rypos;
			item.DOColor(new Color(1f, 1f, 1f, RandF(0.5f, 0.8f)), 0f);
		}
	}

	private void AnimateLigma()
	{
		if (!lowQual)
		{
			alp = 0.4f + 0.05f * Mathf.Sin((float)Math.PI * Time.time * 3f);
			portalBeam.DOColor(whiteClear + alpha * alp, 0f);
			portalBeam.transform.localScale = Vector3.up * 2f + Vector3.right * 2f + Vector3.forward + Vector3.right * 0.07f * Mathf.Sin((float)Math.PI * Time.time * 3f);
			for (int i = 0; i < ligmae.Count; i++)
			{
				SpriteRenderer spriteRenderer = ligmae[i];
				spriteRenderer.transform.position = portalBeam.transform.position + Vector3.right * ligRad[i] * Mathf.Sin(ligOff[i] + Time.time * 2f * ligSpeed[i]) + Vector3.down * ligY[i];
				spriteRenderer.transform.localScale = Vector3.one * 0.5f + Vector3.one * 0.2f * Mathf.Cos(ligOff[i] + Time.time * 2f * ligSpeed[i]);
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
		int[] array = SaveMedals(slumpo ? "T4EX" : "T4", slumpo ? ThirdSun2_Stats.sectionStats : ThirdSun_Stats.sectionStats);
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
}
