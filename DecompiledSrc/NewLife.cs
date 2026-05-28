using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NewLife : TaroBGScript
{
	private static NewLife _instance;

	private int lastJudgedSection = -1;

	private int judgingSection = -1;

	private bool cursection_hasMadeMistakes;

	private bool cursection_hasNonPerfects;

	public List<GameObject> endingStuff;

	public List<Mawaru_Medal> endingMedals;

	public List<float> endingStuff_originalSizes = new List<float>();

	public List<Mawaru_Sprite> threads = new List<Mawaru_Sprite>();

	private List<Vector3> threadpos = new List<Vector3>();

	public Mawaru_Sprite bgcoverSome;

	public Mawaru_Sprite bgcoverAll;

	public List<Mawaru_Sprite> lasers = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> tris = new List<Mawaru_Sprite>();

	private List<int> sectionFloors = new List<int>();

	private List<Action> showThread = new List<Action>();

	public GameObject barAlpha;

	public Transform wobbleStuff;

	public Transform bgzoom;

	private List<Color> RankingTextColors = new List<Color>
	{
		new Color(1f, 1f, 1f, 1f),
		new Color(0.6f, 0.4f, 0f, 1f),
		new Color(0.7f, 0.7f, 0.7f, 1f),
		new Color(1f, 0.8f, 0f, 1f)
	};

	[Header("Assets", order = 0)]
	private GameObject something;

	public List<GameObject> BGsToStretch;

	private Transform camParent;

	public List<GameObject> highQualBG;

	private bool lowVfx;

	private bool lowQual;

	private Color redPlanetCol;

	private Color bluePlanetCol;

	private Color alpha = new Color(0f, 0f, 0f, 1f);

	private float bobang;

	public new static NewLife instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindAnyObjectByType<NewLife>();
			}
			return _instance;
		}
	}

	private void JudgeSection(int section, int grade, bool showText = true, int floor = 0, float xpos = 0f, float ypos = 2f, float ang = 0f)
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && GCS.currentSpeedTrial < 1f))
		{
			return;
		}
		if (slumpo)
		{
			NewLife2_Stats.sectionStats[section] = grade;
		}
		else
		{
			NewLife_Stats.sectionStats[section] = grade;
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
				float xpos = 0f;
				float ypos = 2f;
				float ang = 0f;
				int grade = 3;
				if (cursection_hasNonPerfects)
				{
					grade = 2;
				}
				if (cursection_hasMadeMistakes)
				{
					grade = 1;
				}
				JudgeSection(i, grade, showText: true, sectionFloors[i], xpos, ypos, ang);
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

	private void ShowFinalResults()
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && (!GCS.speedTrialMode || !(GCS.currentSpeedTrial >= 1f))) || ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		ADOBase.controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: false);
		int[] array = SaveMedals(slumpo ? "T1EX" : "T1", slumpo ? NewLife2_Stats.sectionStats : NewLife_Stats.sectionStats);
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
		if (!slumpo)
		{
			if (!NewLife_Stats.init)
			{
				NewLife_Stats.Reset();
			}
		}
		else if (!NewLife2_Stats.init)
		{
			NewLife2_Stats.Reset();
		}
		if (!slumpo)
		{
			GCS.pauseMedalStatsCurrent = NewLife_Stats.sectionStats;
		}
		else
		{
			GCS.pauseMedalStatsCurrent = NewLife2_Stats.sectionStats;
		}
		redPlanetCol = Persistence.GetPlayerColor(red: true).ToRealColor();
		bluePlanetCol = Persistence.GetPlayerColor(red: false).ToRealColor();
		mb(0f, base.SetResultTextPos, 9999f);
		mb(-200f, SetupFonts, 99999f);
		mb(-199f, DisableStuff, 9999f);
		showThread.Add(Thread1);
		showThread.Add(Thread2);
		showThread.Add(Thread3);
		showThread.Add(Thread4);
		for (int i = 0; i < threads.Count; i++)
		{
			threadpos.Add(threads[i].transform.localPosition);
		}
		barAlpha = new GameObject();
		bgzoom = new GameObject().transform;
		bgzoom.position = new Vector3(1.1f, 1.1f, 1f);
		for (int j = 0; j < endingStuff.Count; j++)
		{
			GameObject gameObject = endingStuff[j];
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
		if (!slumpo)
		{
			sectionFloors = new List<int> { 61, 100, 137, 177, 211, 223 };
		}
		else
		{
			sectionFloors = new List<int> { 61, 106, 146, 187, 219, 264, 340, 408 };
		}
		GCS.pauseMedalFloors = new List<int>();
		GCS.pauseMedalFloors.Clear();
		GCS.pauseMedalFloors.Add(0);
		for (int k = 0; k < sectionFloors.Count - 1; k++)
		{
			GCS.pauseMedalFloors.Add(sectionFloors[k]);
		}
		if (slumpo && !lowVfx && !lowQual)
		{
			foreach (int item2 in new List<int> { 109, 121, 133 })
			{
				scrLevelMaker.instance.listFloors[item2].gameObject.AddComponent<ffxColdTile>();
			}
		}
		bpms = new List<Tuple<double, double>>();
		if (!slumpo)
		{
			bpms.Add(new Tuple<double, double>(0.0, 166.25));
		}
		else
		{
			bpms.Add(new Tuple<double, double>(0.0, 174.0));
		}
		mb(-4f, BarsOn, 999f);
		mb(3.5f, BG1Intro, 5f);
		mb(11.5f, BG2Intro, 13f);
		mb(19.5f, MainBG, 999f);
		mb(19.5f, Thread1Long, 21f);
		mb(76f, BarsOff, 78f);
		if (lowVfx)
		{
			mb(80f, PlasmaOn, 203f);
		}
		else
		{
			mb(80f, PlasmaOnQ, 203f);
		}
		for (int l = 0; l < 12; l++)
		{
			mb(30 + 4 * l, showThread[(l + 2) % 4], 31 + 4 * l);
		}
		int num2 = 0;
		for (int m = 80; m < 172; m += 4)
		{
			mb(m, showThread[num2++ % 4], m + 1);
		}
		if (!lowVfx)
		{
			mb(180f, MainBGOffQ, 181f);
			mb(183.5f, MainBGOnQ, 184.5f);
			mb(188f, MainBGOffQ, 189f);
			mb(190.5f, MainBGOnQ, 191.5f);
			mb(195.5f, MainBGOffQ, 196.5f);
			mb(199.5f, MainBGOnQ, 200.5f);
			mb(180f, BarsOnQ, 181f);
			mb(183.5f, BarsOffQ, 184.5f);
			mb(188f, BarsOnQ, 189f);
			mb(190.5f, BarsOffQ, 191.5f);
			mb(195.5f, BarsOnQ, 196.5f);
			mb(199.5f, BarsOffQ, 200.5f);
			mb(180f, BG1OnQ, 181f);
			mb(183.5f, BG1OffQ, 184.5f);
			mb(188f, BG2OnQ, 189f);
			mb(190.5f, BG2OffQ, 191.5f);
			mb(195.5f, BG1OnQ, 196.5f);
			mb(199.5f, BG1OffQ, 200.5f);
		}
		if (!slumpo)
		{
			mb(204f, MainBGOff, 210f);
		}
		mb(204f, BarsOn, 205f);
		if (!slumpo)
		{
			if (!lowVfx)
			{
				mb(191.5f, ZoomN, 192f);
			}
			mb(224f, BarsOff, 225f);
			mb(224f, PlasmaOffQ, 225f);
			mb(232f, PlasmaOn, 233f);
			mb(224f, MainBG, 225f);
			mb(224f, Thread1Final, 225f);
		}
		if (slumpo)
		{
			if (!lowVfx)
			{
				mb(191.5f, ZoomS, 192f);
			}
			mb(204f, MainBGOff, 223f);
			mb(224f, MainBG, 225f);
			mb(224f, PlasmaOn, 233f);
			mb(224f, BarsOff, 251f);
			mb(224f, Thread1Final, 225f);
			if (!lowVfx)
			{
				mb(252f, Thread1, 360f);
				mb(252f, Thread2, 360f);
			}
			if (lowVfx)
			{
				mb(252f, MainBGOff, 365f);
				mb(252f, PlasmaOff, 365f);
			}
			else
			{
				mb(252f, MainBGOffQ, 365f);
				mb(252f, PlasmaOffQ, 365f);
			}
			mb(252f, BarsOn, 253f);
			mb(256f, BG1OnS, 257f);
			mb(312f, BG1OffS, 313f);
			mb(320f, BG2OnS, 328f);
			mb(360f, BG2OffS, 363f);
			mb(368f, ZoomEnd, 999f);
			mb(368f, MainBG, 999f);
			mb(380f, PlasmaOn, 999f);
			mb(368f, BarsOff, 999f);
			mb(368f, Thread1Final, 999f);
		}
		if (!slumpo)
		{
			mb(234f, ShowFinalResults, 999f);
		}
		else
		{
			mb(386f, ShowFinalResults, 999f);
		}
		SortTables();
	}

	private void Start()
	{
		base.redPlanet.planetRenderer.tailParticles.gameObject.transform.localPosition = Vector3.forward * 0.1f;
		base.bluePlanet.planetRenderer.tailParticles.gameObject.transform.localPosition = Vector3.forward * 0.1f;
		base.redPlanet.planetRenderer.coreParticles.gameObject.transform.localPosition = Vector3.forward * 0.2f;
		base.bluePlanet.planetRenderer.coreParticles.gameObject.transform.localPosition = Vector3.forward * 0.2f;
		camParent = base.camy.transform.parent;
		if (slumpo)
		{
			ADOBase.controller.caption = ADOBase.GetLocalizedLevelName("T1-X").Replace("-X", "-EX");
		}
	}

	private new void Update()
	{
		base.Update();
		float num = (float)songBeat;
		wobbleStuff.eulerAngles = Vector3.forward * 1f * Mathf.Sin(num * (float)Math.PI * 0.0625f * 0.125f);
		wobbleStuff.localScale = Vector3.up * bgzoom.position.y + Vector3.right * bgzoom.position.x + Vector3.forward;
		wobbleStuff.localPosition = Vector3.up * 0.1f * Mathf.Sin(num * (float)Math.PI * 0.0625f * 0.225f) + Vector3.right * 0.1f * Mathf.Sin(num * (float)Math.PI * 0.0625f * 0.225f) + Vector3.right * 0.05f * Mathf.Sin(num * (float)Math.PI * 0.0625f) + Vector3.up * 0.1f;
		for (int i = 0; i < threads.Count; i++)
		{
			bobang = 1.57075f * (float)i;
			threads[i].transform.localPosition = threadpos[i] + Vector3.up * 0.1f * Mathf.Sin(bobang + num * (float)Math.PI * 0.125f);
		}
		Grading();
	}

	private void FunnyText()
	{
		Debug.Log("Yee haw!");
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

	public void BarsOn()
	{
		if (!lowQual)
		{
			if (!lowVfx)
			{
				barAlpha.transform.DOMoveX(1f, beats(4f));
			}
			else
			{
				barAlpha.transform.DOMoveX(0f, 0f);
			}
		}
	}

	public void BarsOff()
	{
		if (!lowQual)
		{
			barAlpha.transform.DOMoveX(0f, beats(4f));
		}
	}

	public void BarsOffQ()
	{
		if (!lowQual)
		{
			barAlpha.transform.DOMoveX(0f, beats(0f));
		}
	}

	public void BarsOnQ()
	{
		if (!lowQual)
		{
			if (!lowVfx)
			{
				barAlpha.transform.DOMoveX(1f, 0f);
			}
			else
			{
				barAlpha.transform.DOMoveX(0f, 0f);
			}
		}
	}

	public void ZoomN()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgzoom.DOMove(Vector3.up * 1.6f + Vector3.right * 1.6f, beats(1f)).SetEase(Ease.OutCubic)).Append(bgzoom.DOMove(Vector3.up * 1.1f + Vector3.right * 1.1f, beats(2f)).SetEase(Ease.OutCubic));
		}
	}

	public void ZoomS()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgzoom.DOMove(Vector3.up * 1.6f + Vector3.right * 1.6f, beats(0f)).SetEase(Ease.OutCubic)).AppendInterval(beats(1.5f))
				.Append(bgzoom.DOMove(Vector3.up * 1.1f + Vector3.right * 1.1f, beats(2f)).SetEase(Ease.OutCubic));
		}
	}

	public void ZoomEnd()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgzoom.DOMove(Vector3.up * 1.4f + Vector3.right * 1.4f, beats(0f)).SetEase(Ease.OutCubic)).Append(bgzoom.DOMove(Vector3.up * 1.6f + Vector3.right * 1.6f, beats(12f)).SetEase(Ease.Linear))
				.Append(bgzoom.DOMove(Vector3.up * 1.1f + Vector3.right * 1.1f, beats(4f)).SetEase(Ease.OutCubic));
		}
	}

	public void BG1Intro()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(lasers[0].render.DOColor(redPlanetCol - alpha * 0.6f, beats(3.4f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.1f))
				.Append(lasers[0].render.DOColor(redPlanetCol - alpha, beats(1f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[0].render.DOColor(redPlanetCol - alpha * 0.8f, beats(3.4f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.1f))
				.Append(tris[0].render.DOColor(redPlanetCol - alpha, beats(1f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[1].render.DOColor(redPlanetCol - alpha * 0.8f, beats(3.4f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.1f))
				.Append(tris[1].render.DOColor(redPlanetCol - alpha, beats(1f)).SetEase(Ease.Linear));
		}
	}

	public void BG2Intro()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(lasers[1].render.DOColor(bluePlanetCol - alpha * 0.6f, beats(3.4f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.1f))
				.Append(lasers[1].render.DOColor(bluePlanetCol - alpha, beats(1f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[0].render.DOColor(bluePlanetCol - alpha * 0.8f, beats(3.4f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.1f))
				.Append(tris[0].render.DOColor(bluePlanetCol - alpha, beats(1f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[1].render.DOColor(bluePlanetCol - alpha * 0.8f, beats(3.4f)).SetEase(Ease.OutQuad))
				.AppendInterval(beats(0.1f))
				.Append(tris[1].render.DOColor(bluePlanetCol - alpha, beats(1f)).SetEase(Ease.Linear));
		}
	}

	public void BG1On()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(lasers[0].render.DOColor(redPlanetCol - alpha * 0.6f, beats(4f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[0].render.DOColor(redPlanetCol - alpha * 0.8f, beats(4f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[1].render.DOColor(redPlanetCol - alpha * 0.8f, beats(4f)).SetEase(Ease.OutQuad));
		}
	}

	public void BG2On()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(lasers[1].render.DOColor(bluePlanetCol - alpha * 0.6f, beats(4f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[0].render.DOColor(bluePlanetCol - alpha * 0.8f, beats(4f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[1].render.DOColor(bluePlanetCol - alpha * 0.8f, beats(4f)).SetEase(Ease.OutQuad));
		}
	}

	public void BG1Off()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha, beats(4f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha, beats(4f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha, beats(4f)).SetEase(Ease.Linear));
		}
	}

	public void BG2Off()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha, beats(4f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha, beats(4f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha, beats(4f)).SetEase(Ease.Linear));
		}
	}

	public void BG1OnS()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(lasers[0].render.DOColor(redPlanetCol - alpha * 0.6f, beats(8f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[0].render.DOColor(redPlanetCol - alpha * 0.8f, beats(8f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[1].render.DOColor(redPlanetCol - alpha * 0.8f, beats(8f)).SetEase(Ease.OutQuad));
		}
	}

	public void BG2OnS()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(lasers[1].render.DOColor(bluePlanetCol - alpha * 0.6f, beats(8f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[0].render.DOColor(bluePlanetCol - alpha * 0.8f, beats(8f)).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.Linear)).Append(tris[1].render.DOColor(bluePlanetCol - alpha * 0.8f, beats(8f)).SetEase(Ease.OutQuad));
		}
	}

	public void BG1OffS()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha, beats(8f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha, beats(8f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha, beats(8f)).SetEase(Ease.Linear));
		}
	}

	public void BG2OffS()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha, beats(8f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha, beats(8f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha, beats(8f)).SetEase(Ease.Linear));
		}
	}

	public void BG1OnQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha * 0.4f, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha * 0.8f, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha * 0.8f, 0f).SetEase(Ease.OutQuad));
		}
	}

	public void BG2OnQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha * 0.8f, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha * 0.8f, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha * 0.8f, 0f).SetEase(Ease.OutQuad));
		}
	}

	public void BG1OffQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(redPlanetCol - alpha, 0f).SetEase(Ease.OutQuad));
		}
	}

	public void BG2OffQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(lasers[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[0].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.OutQuad));
			DOTween.Sequence().Append(tris[1].render.DOColor(bluePlanetCol - alpha, 0f).SetEase(Ease.OutQuad));
		}
	}

	public void MainBG()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverAll.render.DOColor(new Color(0f, 0f, 0f, 0.3f), beats(8f)).SetEase(Ease.OutQuad));
		}
	}

	public void MainBGOffQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverAll.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0f)).SetEase(Ease.OutQuad));
		}
	}

	public void MainBGOnQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverAll.render.DOColor(new Color(0f, 0f, 0f, 0.3f), beats(0f)).SetEase(Ease.OutQuad));
		}
	}

	public void MainBGOff()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverAll.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(4f)).SetEase(Ease.OutQuad));
		}
	}

	public void PlasmaOn()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverSome.render.DOColor(new Color(0f, 0f, 0f, 0.3f), beats(4f)).SetEase(Ease.OutQuad));
		}
	}

	public void PlasmaOff()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverSome.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(4f)).SetEase(Ease.Linear));
		}
	}

	public void PlasmaOnQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverSome.render.DOColor(new Color(0f, 0f, 0f, 0.3f), beats(0f)).SetEase(Ease.Linear));
		}
	}

	public void PlasmaOffQ()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(bgcoverSome.render.DOColor(new Color(0f, 0f, 0f, 1f), beats(0f)).SetEase(Ease.Linear));
		}
	}

	public void Thread1Long()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(threads[0].render.material.DOFloat(3f, "_FadeApex", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(threads[0].render.material.DOFloat(1.5f, "_FadeScale", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().AppendInterval(beats(2f)).Append(threads[1].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad))
				.Append(threads[1].render.material.DOFloat(3f, "_FadeApex", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().AppendInterval(beats(2f)).Append(threads[1].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad))
				.Append(threads[1].render.material.DOFloat(1.5f, "_FadeScale", beats(12f)).SetEase(Ease.Linear));
		}
	}

	public void Thread1Final()
	{
		if (!lowQual)
		{
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(threads[0].render.material.DOFloat(3f, "_FadeApex", beats(24f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(threads[0].render.material.DOFloat(1.5f, "_FadeScale", beats(24f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear)).Append(threads[0].render.material.DOFloat(0f, "_RingEffect", beats(24f)).SetEase(Ease.Linear));
			DOTween.Sequence().AppendInterval(beats(2f)).Append(threads[1].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad))
				.Append(threads[1].render.material.DOFloat(3f, "_FadeApex", beats(24f)).SetEase(Ease.Linear));
			DOTween.Sequence().AppendInterval(beats(2f)).Append(threads[1].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad))
				.Append(threads[1].render.material.DOFloat(1.5f, "_FadeScale", beats(24f)).SetEase(Ease.Linear));
			DOTween.Sequence().AppendInterval(beats(2f)).Append(threads[1].render.material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear))
				.Append(threads[1].render.material.DOFloat(0f, "_RingEffect", beats(24f)).SetEase(Ease.Linear));
		}
	}

	public void Thread1()
	{
		if (!lowQual)
		{
			threads[0].render.material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear);
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(threads[0].render.material.DOFloat(3.5f, "_FadeApex", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[0].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(threads[0].render.material.DOFloat(1f, "_FadeScale", beats(12f)).SetEase(Ease.Linear));
		}
	}

	public void Thread2()
	{
		if (!lowQual)
		{
			threads[1].render.material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear);
			DOTween.Sequence().Append(threads[1].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(threads[1].render.material.DOFloat(3.5f, "_FadeApex", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[1].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(threads[1].render.material.DOFloat(1f, "_FadeScale", beats(12f)).SetEase(Ease.Linear));
		}
	}

	public void Thread3()
	{
		if (!lowQual)
		{
			threads[2].render.material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear);
			DOTween.Sequence().Append(threads[2].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(threads[2].render.material.DOFloat(3.5f, "_FadeApex", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[2].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(threads[2].render.material.DOFloat(1f, "_FadeScale", beats(12f)).SetEase(Ease.Linear));
		}
	}

	public void Thread4()
	{
		if (!lowQual)
		{
			threads[3].render.material.DOFloat(1f, "_RingEffect", 0f).SetEase(Ease.Linear);
			DOTween.Sequence().Append(threads[3].render.material.DOFloat(-0.4f, "_FadeApex", 0f).SetEase(Ease.OutQuad)).Append(threads[3].render.material.DOFloat(3.5f, "_FadeApex", beats(12f)).SetEase(Ease.Linear));
			DOTween.Sequence().Append(threads[3].render.material.DOFloat(0.5f, "_FadeScale", 0f).SetEase(Ease.OutQuad)).Append(threads[3].render.material.DOFloat(1f, "_FadeScale", beats(12f)).SetEase(Ease.Linear));
		}
	}
}
