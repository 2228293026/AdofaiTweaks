using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mawaru : TaroBGScript
{
	public AudioSource audio;

	public AudioClip wrongAudio;

	public Mawaru_Sprite wrongX;

	public Camera myCam;

	public GameObject bgAll;

	public bool playerDead;

	private int judgingSection = -1;

	private bool cursection_hasMadeMistakes;

	private bool cursection_hasNonPerfects;

	private bool ns_hasMadeMistakes;

	private bool ns_hasNonPerfects;

	private bool judgedFinalSection;

	private bool judgedCoinSection;

	private int frameStart;

	private List<int> sectionFloors = new List<int>();

	private List<Color> RankingTextColors = new List<Color>
	{
		new Color(1f, 1f, 1f, 1f),
		new Color(0.6f, 0.4f, 0f, 1f),
		new Color(0.7f, 0.7f, 0.7f, 1f),
		new Color(1f, 0.8f, 0f, 1f)
	};

	[Header("Resource management", order = 0)]
	public List<Mawaru_Sprite> stuffToDisable;

	public Image allCover;

	[Header("Assets", order = 0)]
	public List<GameObject> BGsToStretch = new List<GameObject>();

	[Header("  Text", order = 1)]
	public TextMeshProUGUI gameText;

	private float countdownOrigPos;

	public List<GameObject> endingStuff;

	public List<Mawaru_Medal> endingMedals;

	public List<TextMeshPro> endingStuffText;

	public GameObject endingFootballIcon;

	public GameObject endingHandIcon;

	public List<float> endingStuff_originalSizes = new List<float>();

	public int curTextCommand;

	public List<TextCommand> textCommands = new List<TextCommand>();

	private RectTransform percRT;

	private RectTransform trycalRT;

	private Color fadedText = new Color(1f, 1f, 1f, 0.6f);

	private RectTransform lvlname;

	private Vector2 lvlnameAnchorPos;

	private bool levelNameTextPresent = true;

	private List<scrSpike> listSpikes = new List<scrSpike>();

	[Header("  Title")]
	public Mawaru_Sprite mawaru_logo_nope;

	public Mawaru_Sprite mawaru_logo;

	public TextMeshPro mawaru_logo_subtitle;

	public Mawaru_Sprite subtitle_behind;

	public Mawaru_Sprite mawaru_logo_flash;

	public List<Mawaru_Sprite> mawaru_logo_particles;

	public GameObject mawaru_gantry_frame;

	public List<Mawaru_Sprite> mawaru_gantry_lights;

	public Mawaru_Sprite fast_stars;

	public Mawaru_Charlie charlie;

	[Header("  Innervoice")]
	public GameObject innervoice_bg_container;

	public Mawaru_Sprite innervoice_skybg;

	public List<Mawaru_Sprite> innervoice_clouds;

	public List<Mawaru_Sprite> innervoice_text = new List<Mawaru_Sprite>();

	[Header("  Some Quads That Rotate")]
	public GameObject innervoice_container;

	public GameObject innervoice_quads_rot;

	public List<Mawaru_Mesh> innervoice_quads = new List<Mawaru_Mesh>();

	[Header("  Sol")]
	public GameObject sol_container;

	public Mawaru_Sprite sol_bg;

	public List<Mawaru_Sprite> sol_stuff = new List<Mawaru_Sprite>();

	[Header("  Soccer")]
	public GameObject soccer_container;

	public Mawaru_Sprite soccer_field;

	public Mawaru_Sprite soccer_goal2;

	public List<Mawaru_Soccerball> soccerBalls = new List<Mawaru_Soccerball>();

	public List<Mawaru_Sprite> soccerBoots = new List<Mawaru_Sprite>();

	public List<bool> soccerKicked = new List<bool>();

	public List<Mawaru_Sprite> mawaru_soccer_splash;

	public List<Mawaru_Sprite> mawaru_soccer_text;

	private List<int> mawaru_soccer_scored = new List<int>();

	private bool mawaru_soccer_playing;

	private List<int> soccer_floors;

	public Transform kickTranslation;

	public Transform kickTranslation2;

	[Header("  Tricky Triangles")]
	public List<Mawaru_Sprite> triangle_bgstuff = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> triangles = new List<Mawaru_Sprite>();

	[Header("  High Five")]
	public List<Mawaru_Sprite> h5_bgstuff = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> h5_bottomrow = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> h5_toprow = new List<Mawaru_Sprite>();

	public List<Mawaru_Arm> h5_arms = new List<Mawaru_Arm>();

	private List<int> h5_floors;

	[Header("  Collect")]
	public List<Mawaru_Coin> collect_coins = new List<Mawaru_Coin>();

	private int coinJudgeFloor;

	[Header("  Remember")]
	public Mawaru_Oskari remember;

	[Header("  Lofi")]
	public List<Mawaru_Sprite> lofi_bgstuff = new List<Mawaru_Sprite>();

	public List<Mawaru_WarpTile> lofi_warptiles = new List<Mawaru_WarpTile>();

	public List<Mawaru_Sprite> lofi_links = new List<Mawaru_Sprite>();

	public List<Mawaru_WarpTile> lofi2_warptiles = new List<Mawaru_WarpTile>();

	public List<Mawaru_Sprite> lofi2_links = new List<Mawaru_Sprite>();

	[Header("  Math 1 & 2")]
	public List<Mawaru_Sprite> math1_bgstuff = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math1_fakefloor = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math1_fakenumber = new List<Mawaru_Sprite>();

	public List<bool> math1_miss = new List<bool> { false, false };

	private List<int> math1_floors;

	public Color math1_floorcolor;

	public List<Mawaru_Sprite> math1_floorstotint = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math_numbers;

	public List<Mawaru_Sprite> math2_bgstuff = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math2_fakefloor = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math2_fakenumber = new List<Mawaru_Sprite>();

	public List<bool> math2_miss = new List<bool> { false, false };

	private List<int> math2_floors;

	public Color math2_floorcolor;

	public List<Mawaru_Sprite> math2_floorstotint = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math_movableFloor = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> math_randomize = new List<Mawaru_Sprite>();

	[Header("  Math Extra Questions")]
	public List<Sprite> math_extraquestions1b = new List<Sprite>();

	public List<Sprite> math_extraquestions1c = new List<Sprite>();

	public List<Sprite> math_extraquestions1d = new List<Sprite>();

	public List<Sprite> math_extraquestions2 = new List<Sprite>();

	public List<Sprite> math_extraquestions2b = new List<Sprite>();

	public List<Sprite> math_extraquestions3 = new List<Sprite>();

	public List<Sprite> math_extraquestions3b = new List<Sprite>();

	public List<Sprite> math_extraquestions3c = new List<Sprite>();

	public List<Sprite> math_extraquestions4 = new List<Sprite>();

	public List<Sprite> math_extraanswers1b = new List<Sprite>();

	public List<Sprite> math_extraanswers1c = new List<Sprite>();

	public List<Sprite> math_extraanswers1d = new List<Sprite>();

	public List<Sprite> math_extraanswers2 = new List<Sprite>();

	public List<Sprite> math_extraanswers2b = new List<Sprite>();

	public List<Sprite> math_extraanswers3 = new List<Sprite>();

	public List<Sprite> math_extraanswers3b = new List<Sprite>();

	public List<Sprite> math_extraanswers3c = new List<Sprite>();

	public List<Sprite> math_extraanswers4 = new List<Sprite>();

	[Header("  Hardstyle")]
	public List<Mawaru_Sprite> hardstyle_bgstuff = new List<Mawaru_Sprite>();

	public List<Mawaru_Sprite> hardstyle_quads;

	public Mawaru_Sprite hardstyle_sky;

	private int hardstyleStartFloor;

	private int hardstyleEndFloor;

	[Header("  Drift")]
	public List<Mawaru_Sprite> drift_bgstuff;

	private int driftFloorStart;

	private int driftFloorEnd;

	[Header("  Drunk")]
	public List<Mawaru_Sprite> drunk_bgstuff;

	public Mawaru_Sprite drunk_pig;

	private int drunkFloorStart;

	private int drunkFloorEnd;

	[Header("  Defend")]
	public List<Mawaru_Sprite> defend_bgstuff;

	public List<Mawaru_Rock> defend_projectiles;

	public Mawaru_Castle defend_castle;

	public Mawaru_Sprite defend_crosshair;

	public GameObject defend_crosshair_parent;

	public TextMeshPro defend_target_text;

	public List<Mawaru_Rock> defend2_projectiles;

	public Mawaru_Castle defend2_castle;

	public Mawaru_Sprite defend2_crosshair;

	public GameObject defend2_crosshair_parent;

	public TextMeshPro defend2_target_text;

	private int defend_first_floor;

	private int defend2_first_floor;

	[Header("  Fog")]
	public List<Mawaru_Sprite> fog_bgstuff;

	public Mawaru_Sprite fog_overlay;

	public List<Mawaru_Sprite> fogp1;

	public List<Mawaru_Sprite> fogp2;

	public GameObject fogBPM;

	[Header("  Psytrance")]
	public List<Mawaru_Sprite> psytrance_bgstuff;

	public List<Mawaru_Sprite> psytrance_lightmass;

	private int psytranceStartFloor;

	private int psytranceEndFloor;

	[Header("  Nishizabu")]
	public List<Mawaru_Sprite> nishizabu_bgstuff;

	public List<Mawaru_Bar> nishizabu_bars;

	public List<Mawaru_Sprite> nishizabu_goat_stuff;

	public List<Mawaru_Goat> nishizabu_goats;

	public List<Mawaru_Sprite> nishizabu_gear;

	public List<Mawaru_Sprite> nishizabu_numbers;

	[Header("  Triangles2")]
	public List<Mawaru_Sprite> triangle2_bgstuff;

	public List<Mawaru_Sprite> triangles2;

	public List<Mawaru_Sprite> ghosts;

	[Header("  VictoryLap")]
	public List<scrPlanetCopyCam> cloneCams;

	[Header("  Ending")]
	public TextMeshPro almostThere;

	public List<GameObject> bootleg_squares;

	public List<Tuple<int, int>> bootleg_stuff;

	private bool lowVfx;

	private int defaultMask;

	private int funkyMask;

	private int endingBeatStartFloor = 899;

	private int endingBeatEndFloor = 934;

	private List<int> endingFloorsToCheck = new List<int>();

	private List<CharlieAction> charlieStartEvents = new List<CharlieAction>();

	private List<CharlieAction> charlieEndEvents1 = new List<CharlieAction>();

	private List<CharlieAction> charlieEndEvents1b = new List<CharlieAction>();

	private List<CharlieAction> charlieEndEvents2 = new List<CharlieAction>();

	private CharlieAction finalSplat;

	private CharlieAction charlieWaitABit;

	private CharlieAction charlieJumpToGoal;

	private CharlieAction charlieVictory;

	private bool charlieWon;

	private float fontScale = 1f;

	private bool cha1;

	private Sequence s1;

	private Sequence s2;

	private Sequence s3;

	private Sequence s4;

	private Sequence s5;

	private Sequence s6;

	private Sequence s7;

	private Sequence s8;

	private Sequence s9;

	private List<Sequence> introSeqs = new List<Sequence>();

	private float next_sparkle = 1f;

	private int curSparkleSpawn;

	private Color fadedParticles = new Color(1f, 1f, 1f, 1f);

	private bool shouldBoots;

	private Vector3 kickPos;

	private Vector3 kickPos2;

	private float kickRad;

	private float kickRad2;

	private Color GoalRed = new Color(1f, 0f, 0f, 1f);

	private Color GoalYellow = new Color(1f, 0.8f, 0f, 1f);

	private int current_tri;

	private int current_tri2;

	private int current_ghost;

	private Vector3 tempPos;

	private int coins;

	private float lofi_alp;

	private float lofi_sub;

	private Color lofi_origColor;

	private Color lofi_newColor;

	private int curLofiWarp;

	private List<bool> movedCameraMath1 = new List<bool> { false, false };

	private List<int> math_floors = new List<int>();

	private List<float> math_correctAnswer = new List<float> { -1f, 1f, 1f, -1f };

	private float next_num = 672f;

	private List<bool> movedCameraMath2 = new List<bool> { false, false };

	private int curNumberSpawn;

	private Color numberColor = new Color(0f, 0f, 0f, 0.3f);

	private Vector3 hsQuadScale = new Vector3(8f, 8f, 1f) * 1.2f;

	private float hsmult = 1f;

	private float hspingpong;

	private float hspos;

	private Sequence driftSeq;

	private Sequence drunkSeq;

	private float defend_approachDist = 13f;

	private float defend_approachTime = 3f;

	private int curRock;

	private int nextRock;

	private List<double> defend_times = new List<double>();

	private List<Color> defend_colors = new List<Color>();

	private double perc;

	private Mawaru_Rock r;

	private Color rCol;

	private bool fogDied;

	private Vector3 fogAdd1;

	private Vector3 fogAdd2;

	private bool fog2Died;

	private List<int> goatFloors = new List<int>();

	private List<bool> goatFloorHidden = new List<bool> { false, false, false, false };

	private bool nishifading;

	private Vector3 goatViewAngle;

	private Dictionary<int, Sequence> goatBodyTweens = new Dictionary<int, Sequence>();

	private Dictionary<int, Sequence> goatLightTweens = new Dictionary<int, Sequence>();

	public List<SpriteRenderer> goatFloorIcons = new List<SpriteRenderer>();

	private float gearBeat;

	private Vector3 scatter;

	private bool hiding;

	private float cang;

	private float cang2;

	private float camp;

	private int curBootlegFloor;

	private float defend2_approachDist = 13f;

	private float defend2_approachTime = 2.2f;

	private int curRock2;

	private int nextRock2;

	private List<double> defend2_times = new List<double>();

	private double starttime = 328.521;

	private int curEndingWarp;

	private int endingFloorChecking;

	private Vector3 C2P;

	private bool gotC2P;

	private void JudgeSection(int section, int grade, bool showText = true, int floor = 0)
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && GCS.currentSpeedTrial < 1f))
		{
			return;
		}
		if (slumpo)
		{
			Mawaru2_Stats.sectionStats[section] = grade;
		}
		else
		{
			Mawaru_Stats.sectionStats[section] = grade;
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
		for (int i = 0; i < 19; i++)
		{
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
				JudgeSection(i, grade, showText: true, sectionFloors[i * 2 + 1]);
				judgingSection = -1;
			}
		}
		if (!judgedFinalSection && ADOBase.controller.currFloor.seqID == sectionFloors[38])
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
			JudgeSection(19, grade2, showText: false);
			judgedFinalSection = true;
		}
		if (!judgedCoinSection && ADOBase.controller.currFloor.seqID == coinJudgeFloor)
		{
			if (slumpo)
			{
				ShowSectionText(coinJudgeFloor, $"{Mawaru2_Stats.coins}/{collect_coins.Count}");
			}
			else
			{
				ShowSectionText(coinJudgeFloor, $"{Mawaru_Stats.coins}/{collect_coins.Count}");
			}
			judgedCoinSection = true;
		}
		if (judgingSection == -1)
		{
			if (ADOBase.controller.currFloor.seqID < (slumpo ? 880 : 794))
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

	private void Wrong(bool anim = true)
	{
		scrSfx.instance.PlaySfx(SfxSound.MawaruWrong, MixerGroup.SfxParent);
		if (anim)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(wrongX.render.material.DOColor(Color.white, 0f).SetEase(Ease.Linear))
				.AppendInterval(0.1f)
				.Append(wrongX.render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear))
				.AppendInterval(0.1f)
				.Append(wrongX.render.material.DOColor(Color.white, 0f).SetEase(Ease.Linear))
				.Append(wrongX.render.material.DOColor(whiteClear, 0.9f).SetEase(Ease.Linear));
		}
	}

	public void txt(float b, string text, float hang, float persist)
	{
		textCommands.Add(new TextCommand(b, text, hang, persist));
	}

	public void DoText(string text, float hangTime, float persistTime, float zoom = 1f)
	{
		if (!RDC.noHud || RDC.partialNoHud)
		{
			scrUIController.instance.txtCountdown.rectTransform.DOAnchorPosY(countdownOrigPos - 160f, 0.3f);
			gameText.text = text;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(gameText.transform.DOLocalMove(Vector3.up * 140f, 0f))
				.AppendInterval(beats(hangTime, currentBPM))
				.Append(gameText.transform.DOLocalMove(Vector3.up * 400f, beats(2f, currentBPM)).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(gameText.transform.DOScale(Vector3.one * 2f * zoom + Vector3.right * zoom, 0f))
				.Append(gameText.transform.DOScale(Vector3.one * zoom, 0.3f * speed).SetEase(Ease.InCubic))
				.AppendInterval(beats(hangTime, currentBPM) - 0.3f * speed)
				.Append(gameText.transform.DOScale(Vector3.one * 0.5f * zoom, beats(2f, currentBPM)).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(gameText.DOColor(whiteClear, 0f))
				.Append(gameText.DOColor(Color.white, 0.3f * speed).SetEase(Ease.Linear))
				.AppendInterval(beats(hangTime, currentBPM) - 0.3f * speed)
				.Append(gameText.DOColor(fadedText, beats(2f, currentBPM)).SetEase(Ease.Linear))
				.AppendInterval(beats(persistTime, currentBPM))
				.Append(gameText.DOColor(whiteClear, 1f * speed).SetEase(Ease.Linear));
		}
	}

	public void LevelNameTextAway()
	{
		lvlname = scrUIController.instance.txtLevelName.transform.GetComponent<RectTransform>();
		lvlnameAnchorPos = lvlname.anchoredPosition;
		lvlname.DOAnchorPosY(200f, 1f).SetEase(Ease.InBack);
	}

	private void FadeAllCover()
	{
		for (int i = 0; i < defend_times.Count; i++)
		{
			scrLevelMaker.instance.listFloors[defend_first_floor + 2 * i].SetOpacity(0f);
			scrLevelMaker.instance.listFloors[defend_first_floor + 1 + 2 * i].SetOpacity(0f);
		}
		for (int j = 0; j < defend2_times.Count; j++)
		{
			scrLevelMaker.instance.listFloors[defend2_first_floor + 2 * j].SetOpacity(0f);
			scrLevelMaker.instance.listFloors[defend2_first_floor + 1 + 2 * j].SetOpacity(0f);
		}
		DOTween.Sequence().AppendInterval(0f).Append(allCover.DOColor(new Color(0f, 0f, 0f, 0f), 0.1f).SetEase(Ease.Linear).OnComplete(delegate
		{
			allCover.enabled = false;
		}));
	}

	private void DisableStuff()
	{
		foreach (Mawaru_Sprite item in stuffToDisable)
		{
			item.render.enabled = false;
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
				if (!listSpike.hit && listSpike.hittable && Vector3.Distance(scrPlanet2.transform.position, listSpike.pos) < 0.8f && !RDC.auto && scrPlanet2.iFrames <= 0f)
				{
					if (listSpike.hitTime[i] < 0f)
					{
						listSpike.hitTime[i] += Time.deltaTime;
						continue;
					}
					listSpike.hit = true;
					scrPlanet2.Die();
					scrPlanet2.player.DieByHitbox();
					DOTween.Sequence().Append(listSpike.transform.DOScale(Vector3.one * 0.2f, 0f).SetRelative(isRelative: true)).Append(listSpike.transform.DOScale(Vector3.one * -0.2f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
				}
				else
				{
					listSpike.hitTime[i] = 0f;
				}
			}
		}
	}

	public new void Update()
	{
		base.Update();
		if (Time.frameCount - frameStart == 5)
		{
			FadeAllCover();
		}
		if (ADOBase.controller.currentState == States.Start)
		{
			scrCamera.instance.timer = 0f;
		}
		SpikeCollisions();
		if (curTextCommand < textCommands.Count && songBeat > (double)textCommands[curTextCommand].fBeat)
		{
			if (songBeat < (double)(textCommands[curTextCommand].fBeat + 1f))
			{
				TextCommand textCommand = textCommands[curTextCommand];
				DoText(textCommand.sText, textCommand.fHang, textCommand.fPersist);
			}
			curTextCommand++;
		}
		if (songBeat > 0.0 && ADOBase.controller.currentState == States.PlayerControl && levelNameTextPresent)
		{
			levelNameTextPresent = false;
			LevelNameTextAway();
		}
		if (!playerDead && (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2))
		{
			if (slumpo)
			{
				if (!Mawaru2_Stats.apologized && slumpo && ((songBeat > 359.0 && songBeat < 395.0) || (songBeat > 785.0 && songBeat < 832.0) || (songBeat > 848.0 && songBeat < 881.0)))
				{
					if (scrController.instance?.errorMeter?.gameObject.activeInHierarchy == true)
					{
						ADOBase.controller.txtTryCalibrating.alignment = TextAnchor.UpperCenter;
					}
					else
					{
						ADOBase.controller.txtTryCalibrating.alignment = TextAnchor.MiddleCenter;
					}
					ADOBase.controller.txtTryCalibrating.text = RDString.Get("mawaru.sorry");
					Mawaru2_Stats.apologized = true;
				}
			}
			else if (!Mawaru_Stats.apologized && !slumpo && ((songBeat > 455.0 && songBeat < 499.0) || (songBeat > 743.0 && songBeat < 787.0)))
			{
				if (scrController.instance?.errorMeter?.gameObject.activeInHierarchy == true)
				{
					ADOBase.controller.txtTryCalibrating.alignment = TextAnchor.UpperCenter;
				}
				else
				{
					ADOBase.controller.txtTryCalibrating.alignment = TextAnchor.MiddleCenter;
				}
				ADOBase.controller.txtTryCalibrating.text = RDString.Get("mawaru.sorry");
				Mawaru_Stats.apologized = true;
			}
			if (songBeat > 49.0)
			{
				if (slumpo)
				{
					Mawaru2_Stats.checkpointsUsed++;
				}
				else
				{
					Mawaru_Stats.checkpointsUsed++;
				}
			}
			playerDead = true;
		}
		endingFootballIcon.transform.eulerAngles = Vector3.forward * 180f * Time.time;
		endingHandIcon.transform.eulerAngles = Vector3.forward * 5f * Mathf.Sin(Time.time * (float)Math.PI * 2f);
		Grading();
	}

	private void Start()
	{
		frameStart = Time.frameCount;
		if (slumpo)
		{
			ADOBase.controller.caption = ADOBase.GetLocalizedLevelName("T3-X").Replace("-X", "-EX");
		}
		percRT = scrUIController.instance.txtPercent.transform.GetComponent<RectTransform>();
		trycalRT = scrUIController.instance.txtTryCalibrating.transform.GetComponent<RectTransform>();
	}

	private new void Awake()
	{
		base.Awake();
		defaultMask = myCam.cullingMask;
		funkyMask = (1 << LayerMask.NameToLayer("TransparentFX")) | (1 << LayerMask.NameToLayer("Water")) | (1 << LayerMask.NameToLayer("Camera")) | (1 << LayerMask.NameToLayer("Review")) | (1 << LayerMask.NameToLayer("UI")) | (1 << LayerMask.NameToLayer("WorkshopThumbnail"));
		allCover.gameObject.SetActive(value: true);
		lowVfx = ADOBase.controller.visualEffects == VisualEffects.Minimum;
		if (!slumpo)
		{
			soccer_floors = new List<int> { 89, 101 };
			mawaru_soccer_scored = new List<int> { 0, 0 };
			soccerKicked = new List<bool> { false, false };
			h5_floors = new List<int> { 140, 144, 146, 150, 152, 156, 160, 162 };
			hardstyleStartFloor = 275;
			hardstyleEndFloor = 309;
			driftFloorStart = 312;
			driftFloorEnd = 339;
			drunkFloorStart = 341;
			drunkFloorEnd = 367;
			psytranceStartFloor = 508;
			psytranceEndFloor = 545;
			coinJudgeFloor = 168;
			goatFloors = new List<int> { 593, 599, 605, 611 };
			defend_times = new List<double>
			{
				186.539, 186.914, 187.289, 187.664, 188.414, 189.539, 189.914, 190.476, 191.039, 192.164,
				192.539, 193.101, 193.664, 194.039, 194.601, 195.164, 195.726, 196.289, 196.664, 197.039,
				197.601, 197.789, 198.539, 199.101, 199.289, 199.664, 200.039, 200.601, 200.789, 201.539,
				202.101, 202.289, 202.476, 202.664, 203.039, 203.289, 203.539, 203.789, 204.164, 204.539,
				204.789, 205.039, 205.289, 205.664, 206.039, 206.601, 206.789, 207.164, 207.539, 208.289
			};
			defend2_times = new List<double> { 328.837, 329.074, 329.311, 329.548, 329.785, 330.021, 330.258, 330.495, 330.732 };
			math1_floors = new List<int> { 248, 260 };
			math2_floors = new List<int> { 555, 567 };
			defend_first_floor = 370;
			defend2_first_floor = 754;
			bootleg_stuff = new List<Tuple<int, int>>();
			bootleg_stuff.Add(new Tuple<int, int>(812, 4));
			bootleg_stuff.Add(new Tuple<int, int>(818, 4));
			bootleg_stuff.Add(new Tuple<int, int>(824, 4));
			bootleg_stuff.Add(new Tuple<int, int>(830, 4));
			sectionFloors = new List<int>
			{
				37, 57, 58, 81, 82, 104, 113, 138, 139, 165,
				178, 217, 218, 239, 240, 263, 275, 311, 312, 340,
				341, 368, 369, 470, 471, 507, 508, 546, 547, 570,
				588, 613, 614, 652, 653, 693, 743, 787, 868
			};
		}
		else
		{
			soccer_floors = new List<int> { 99, 105, 117, 120 };
			mawaru_soccer_scored = new List<int> { 0, 0, 0, 0 };
			soccerKicked = new List<bool> { false, false, false, false };
			h5_floors = new List<int>
			{
				159, 161, 163, 167, 169, 171, 173, 177, 179, 181,
				183, 187, 189, 191, 193, 195, 199, 201, 203, 205,
				207, 209
			};
			hardstyleStartFloor = 346;
			hardstyleEndFloor = 403;
			driftFloorStart = 405;
			driftFloorEnd = 450;
			drunkFloorStart = 452;
			drunkFloorEnd = 481;
			psytranceStartFloor = 680;
			psytranceEndFloor = 717;
			goatFloors = new List<int> { 765, 771, 777, 783 };
			coinJudgeFloor = 215;
			defend_times = new List<double>
			{
				186.539, 186.914, 187.289, 187.664, 188.414, 189.539, 189.914, 190.476, 191.039, 191.601,
				192.164, 192.539, 192.726, 193.101, 193.664, 194.039, 194.601, 195.164, 195.539, 195.726,
				196.289, 196.664, 197.039, 197.601, 197.695, 197.789, 198.164, 198.351, 198.539, 198.914,
				199.101, 199.289, 199.476, 199.664, 199.851, 200.039, 200.414, 200.601, 200.695, 200.789,
				200.976, 201.07, 201.164, 201.351, 201.539, 201.914, 202.101, 202.289, 202.476, 202.664,
				202.851, 203.039, 203.289, 203.539, 203.789, 204.039, 204.289, 204.414, 204.539, 204.789,
				205.039, 205.289, 205.476, 205.851, 206.039, 206.414, 206.601, 206.789, 206.976, 207.351,
				207.539, 207.914, 208.289, 208.664
			};
			defend2_times = new List<double>
			{
				329.116, 329.274, 329.353, 329.59, 329.748, 329.827, 330.064, 330.222, 330.3, 330.537,
				330.695, 330.774, 331.011
			};
			math1_floors = new List<int> { 319, 331 };
			math2_floors = new List<int> { 727, 739 };
			defend_first_floor = 484;
			defend2_first_floor = 951;
			bootleg_stuff = new List<Tuple<int, int>>();
			bootleg_stuff.Add(new Tuple<int, int>(1023, 4));
			bootleg_stuff.Add(new Tuple<int, int>(1029, 4));
			bootleg_stuff.Add(new Tuple<int, int>(1035, 4));
			bootleg_stuff.Add(new Tuple<int, int>(1041, 4));
			sectionFloors = new List<int>
			{
				37, 59, 60, 95, 96, 122, 131, 157, 158, 212,
				225, 282, 283, 310, 311, 335, 346, 404, 405, 451,
				452, 482, 483, 632, 633, 679, 680, 718, 719, 743,
				760, 784, 785, 832, 833, 878, 936, 998, 1093
			};
		}
		GCS.pauseMedalFloors = new List<int>();
		GCS.pauseMedalFloors.Clear();
		for (int i = 0; i < sectionFloors.Count - 1; i += 2)
		{
			GCS.pauseMedalFloors.Add(sectionFloors[i]);
		}
		GCS.pauseMedalFloors.Add(sectionFloors[sectionFloors.Count - 2]);
		for (int j = 0; j < soccerBalls.Count; j++)
		{
			soccerBalls[j].floor = scrLevelMaker.instance.listFloors[soccer_floors[j]];
		}
		for (int k = 0; k < h5_arms.Count; k++)
		{
			h5_arms[k].floor = scrLevelMaker.instance.listFloors[h5_floors[k]];
		}
		for (int l = 0; l < bootleg_stuff.Count; l++)
		{
			for (int m = 0; m < bootleg_stuff[l].Item2; m++)
			{
				endingFloorsToCheck.Add(bootleg_stuff[l].Item1 + m);
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
		for (int n = 0; n < endingStuff.Count; n++)
		{
			GameObject gameObject = endingStuff[n];
			endingStuff_originalSizes.Add(gameObject.transform.localScale.x);
			gameObject.transform.localScale = Vector3.zero;
		}
		if (!slumpo)
		{
			if (!Mawaru_Stats.init)
			{
				Mawaru_Stats.Reset();
			}
		}
		else if (!Mawaru2_Stats.init)
		{
			Mawaru2_Stats.Reset();
		}
		if (!slumpo)
		{
			GCS.pauseMedalStatsCurrent = Mawaru_Stats.sectionStats;
		}
		else
		{
			GCS.pauseMedalStatsCurrent = Mawaru2_Stats.sectionStats;
		}
		mb(0f, base.SetResultTextPos2, 9999f);
		scrSpike[] array = UnityEngine.Object.FindObjectsByType(typeof(scrSpike), FindObjectsSortMode.None) as scrSpike[];
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			listSpikes.Add(array[num2]);
		}
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 170.0));
		bpms.Add(new Tuple<double, double>(41.0, 140.0));
		bpms.Add(new Tuple<double, double>(117.0, 155.0));
		bpms.Add(new Tuple<double, double>(213.0, 170.0));
		bpms.Add(new Tuple<double, double>(341.0, 185.0));
		bpms.Add(new Tuple<double, double>(505.0, 160.0));
		bpms.Add(new Tuple<double, double>(569.0, 200.0));
		bpms.Add(new Tuple<double, double>(721.0, 215.0));
		if (slumpo)
		{
			bpms.Add(new Tuple<double, double>(787.0, 107.5));
			bpms.Add(new Tuple<double, double>(788.0, 215.0));
		}
		bpms.Add(new Tuple<double, double>(898.0, 170.0));
		bpms.Add(new Tuple<double, double>(930.0, 180.0));
		bpms.Add(new Tuple<double, double>(962.0, 190.0));
		bpms.Add(new Tuple<double, double>(994.0, 200.0));
		wrongX.render.material.SetColor("_Color", whiteClear);
		countdownOrigPos = scrUIController.instance.txtCountdown.rectTransform.anchoredPosition.y;
		txt(42f, RDString.Get("mawaru.warmUp"), 5f, 12f);
		txt(66f, RDString.Get("mawaru.holdsAndMidspins"), 4f, 13f);
		if (slumpo)
		{
			txt(92f, RDString.Get("mawaru.soccer2"), 5f, 12f);
		}
		else
		{
			txt(92f, RDString.Get("mawaru.soccer"), 5f, 12f);
		}
		txt(124f, RDString.Get("mawaru.triangles"), 5f, 12f);
		txt(148f, RDString.Get("mawaru.highFives"), 5f, 12f);
		txt(170f, RDString.Get("mawaru.collect"), 5f, 28f);
		txt(218f, RDString.Get("mawaru.backtrack"), 5f, 24f);
		txt(259f, RDString.Get("mawaru.warpTile"), 5f, 28f);
		txt(299f, RDString.Get("mawaru.math"), 5f, 28f);
		txt(353f, RDString.Get("mawaru.hardstyle"), 8f, 28f);
		txt(401f, RDString.Get("mawaru.drift"), 8f, 28f);
		txt(449f, RDString.Get("mawaru.drunk"), 8f, 28f);
		txt(497f, RDString.Get("mawaru.defend"), 8f, 60f);
		txt(577f, RDString.Get("mawaru.fog"), 8f, 28f);
		txt(625f, RDString.Get("mawaru.psytrance"), 8f, 28f);
		txt(673f, RDString.Get("mawaru.math2"), 8f, 28f);
		txt(737f, RDString.Get("mawaru.stealth"), 8f, 36f);
		txt(794f, RDString.Get("mawaru.triangles2"), 8f, 28f);
		txt(842f, RDString.Get("mawaru.victoryLap"), 8f, 28f);
		if (slumpo)
		{
			almostThere.text = RDString.Get("mawaru.almostThere2");
		}
		else
		{
			almostThere.text = RDString.Get("mawaru.almostThere");
		}
		mb(-199f, DisableStuff, 9999f);
		mb(-7f, IntroLight1);
		mb(-5f, IntroLight2);
		mb(-3f, IntroLight3);
		mb(-1f, IntroLight4);
		mb(1f, HideGantry, 99999f);
		mb(-999f, Force3CountdownTicks, 7f);
		mb(8f, Force4CountdownTicks, 99999f);
		mb(-3f, CharlieStart);
		mb(-200f, SetupFonts, 99999f);
		mb(-200f, RandomizeTiles, 99999f);
		mb(1f, MawaruIntro);
		mpf(1f, MawaruIntroUpdate, 14f);
		mb(41f, base.BGPersp, 9999f);
		mb(41f, InnervoiceSetup, 64f);
		mb(45f, InnervoiceFadeIn);
		mb(48.5f, InnervoiceText);
		mb(57f, InnervoiceTextFade);
		mb(65f, InnervoiceFadeOut);
		mb(72f, InnervoiceHide);
		mb(64f, SolSetup, 89f);
		mb(69f, SolFadeIn);
		mb(77f, SolMoon1);
		mb(80.5f, SolSun1);
		mb(85f, SolMoon2);
		mb(88.5f, SolSun2);
		mb(89f, SolFadeOut);
		mb(113f, SolHide);
		mb(90f, SoccerBootParent, 112f);
		mb(90f, SoccerSetup, 112f);
		mb(93f, SoccerFadeIn, 112f);
		if (!slumpo)
		{
			mb(105f, SoccerCheckGoal1);
			mb(113f, SoccerCheckGoal2);
		}
		else
		{
			mb(102f, SoccerCheckGoal1S);
			mb(105f, SoccerCheckGoal2S);
			mb(112f, SoccerCheckGoal3S);
			mb(113f, SoccerCheckGoal4S);
		}
		mb(113f, SoccerFadeOut);
		mpf(97f, SoccerUpdate, 113f);
		mb(123f, SoccerHide, 130f);
		mb(124f, TrianglesSetup, 145f);
		mb(125f, TrianglesFadeIn);
		mb(145f, TrianglesFadeOut);
		List<float> list = new List<float>();
		if (!slumpo)
		{
			list = new List<float>
			{
				129f, 129.75f, 130.25f, 133f, 135f, 137f, 139f, 141f, 143f, 143.75f,
				144.5f
			};
		}
		else
		{
			for (float num3 = 129f; num3 < 142f; num3 += 1f)
			{
				list.Add(num3);
			}
		}
		foreach (float item2 in list)
		{
			mb(item2, TriangleAppear);
		}
		mb(147f, HighFiveSetup);
		mpf(148f, HighFiveUpdate, 172f);
		mb(149f, HighFiveFadeIn);
		mb(167f, HighFiveFadeOut);
		mb(167f, FreeroamSetup);
		mpf(175f, FreeroamUpdate, 204.9f);
		mb(204.9f, FreeroamCheckCoins);
		mb(228f, OskariAppear);
		mb(250f, OskariDisappear);
		mb(256f, LofiSetup, 264f);
		mb(257f, LofiFadeIn, 264f);
		List<float> list2 = (slumpo ? new List<float> { 267f, 271f, 275f, 277.5f, 279f, 280.5f, 283f, 286f, 291f, 294f } : new List<float> { 268f, 271f, 275f, 277.5f, 279f, 283f, 288f, 292f, 295f });
		foreach (float item3 in list2)
		{
			mb(item3, LofiWarp, 999f);
		}
		mb(297f, LofiFadeOut);
		mpf(256f, LofiUpdate, 300f);
		mb(0f, MathRandomize, 713f);
		mb(0f, MathTint1, 713f);
		mpf(300f, MathUpdate, 337f);
		mb(300f, MathSetup);
		mb(301f, MathFadeIn);
		mb(337f, MathFadeOut);
		mb(0f, MathTint2, 713f);
		mpf(672f, Math2Update, 713f);
		mb(672f, Math2Setup);
		mb(673f, Math2FadeIn);
		mb(713f, Math2FadeOut);
		if (lowVfx)
		{
			mpf(360f, HardstyleUpdateLow, 393f);
		}
		else
		{
			mpf(360f, HardstyleUpdate, 393f);
			mb(375f, HardstyleKickCam);
		}
		mb(352f, HardstyleSetup, 393f);
		mb(353f, HardstyleFadeIn);
		mb(389f, HardstyleEndQuads);
		mb(393f, HardstyleFadeOut);
		mpf(409f, DriftUpdate, 441f);
		mb(400f, DriftSetup, 441f);
		mb(401f, DriftFadeIn);
		mb(441f, DriftFadeOut);
		mpf(457f, DrunkUpdate, 489f);
		mb(448f, DrunkSetup, 489f);
		mb(449f, DrunkFadeIn);
		mb(489f, DrunkFadeOut);
		mb(0f, DefendColors, 9999f);
		mpf(489f, DefendUpdate, 577f);
		mb(489f, DefendSetup, 568f);
		mb(497f, DefendFadeIn);
		mb(569f, DefendFadeOut);
		mpf(577f, FogUpdate, 625f);
		mb(576f, FogSetup, 600f);
		mb(577f, FogFadeIn);
		mb(585f, FogBegin);
		mb(617f, FogFadeOut);
		mb(615f, FogBPMMoveBack);
		mpf(629f, PsytranceUpdate, 666f);
		mb(624f, PsytranceSetup);
		mb(625f, PsytranceFadeIn);
		mb(665f, PsytranceFadeOut);
		goatFloorIcons.Add(GameObject.Find("tile_hold_arrowshort").GetComponent<SpriteRenderer>());
		goatFloorIcons.Add(GameObject.Find("tile_hold_arrowshort (1)").GetComponent<SpriteRenderer>());
		goatFloorIcons.Add(GameObject.Find("tile_hold_arrowshort (2)").GetComponent<SpriteRenderer>());
		goatFloorIcons.Add(GameObject.Find("tile_hold_arrowshort (3)").GetComponent<SpriteRenderer>());
		mb(732f, NishiSetup, 740f);
		mb(737f, NishiFadeIn);
		mb(786f, NishiFadeOut);
		mb(786.1f, NishiFadeOutQ, 9999f);
		if (!slumpo)
		{
			mb(749f, NishiGoat1);
			mb(749.95f, NishiBar1);
			mb(758f, NishiGoat2);
			mb(758.95f, NishiBar2);
			mb(769f, NishiGoat3);
			mb(769.95f, NishiBar3);
			mb(779f, NishiGoat4);
			mb(779.95f, NishiBar4);
		}
		else
		{
			mb(749f, NishiGoat1);
			mb(749.95f, NishiBar1);
			mb(760f, NishiGoat2);
			mb(760.95f, NishiBar2);
			mb(769f, NishiGoat3);
			mb(769.95f, NishiBar3);
			mb(776f, NishiGoat4);
			mb(776.95f, NishiBar4);
		}
		mpf(736f, NishiGoatUpdate, 794f);
		mb(793f, Triangles2Setup);
		mb(794f, Triangles2FadeIn);
		mb(834f, Triangles2FadeOut);
		foreach (float item4 in new List<float>
		{
			802f, 806f, 807.5f, 809f, 811f, 812.5f, 814f, 815.5f, 817f, 818f,
			820f, 822f, 824f, 826f, 827.5f, 829f, 830f
		})
		{
			mb(item4, Triangle2Appear);
		}
		mb(805f, Triangle2GhostAppear);
		mb(810f, Triangle2GhostAppear);
		mb(819f, Triangle2GhostAppear);
		mb(821f, Triangle2GhostAppear);
		mb(823f, Triangle2GhostAppear);
		if (lowVfx)
		{
			mb(850f, VictoryLapSetup);
			mpf(850f, VictoryLapUpdateLow, 882f);
			mb(882f, VictoryLapEnd);
			mb(850f, Cams2);
		}
		else
		{
			mb(850f, VictoryLapSetup);
			mpf(850f, VictoryLapUpdate, 882f);
			mb(882f, VictoryLapEnd);
			mb(850f, Cams4);
			mb(854f, Cams2);
			mb(858f, Cams4);
			mb(866f, Cams2);
			mb(870f, Cams4);
			mb(874f, Cams2);
			mb(878f, Cams4);
		}
		mb(891f, CharlieEnding1);
		mb(892f, CharlieEnding1b, 944f);
		mb(989f, CharlieEnding2, 1008f);
		if (slumpo)
		{
			mpf(929f, EndingBeatVibe, 955f);
		}
		mb(962f, Defend2Setup, 976.5f);
		mb(970f, Defend2RingOn);
		mb(977f, Defend2RingOff);
		mb(962f, Lofi2Setup, 999f);
		foreach (float item5 in new List<float> { 966f, 982f, 986f })
		{
			mb(item5, EndingWarp, 999f);
		}
		mb(970f, Lofi2FadeOut0);
		mb(974f, Lofi2FadeIn12);
		if (slumpo)
		{
			mb(962f, Fog2Setup);
			mb(974f, Fog2Begin);
			mb(990f, Fog2FadeOut);
		}
		if (slumpo)
		{
			mb(1008f, SpawnBootlegFloors1);
			mb(1012f, SpawnBootlegFloors2);
			mb(1016f, SpawnBootlegFloors3);
			mb(1020f, SpawnBootlegFloors4);
		}
		else
		{
			mb(1006f, SpawnBootlegFloors1);
			mb(1010f, SpawnBootlegFloors2);
			mb(1014f, SpawnBootlegFloors3);
			mb(1018f, SpawnBootlegFloors4);
		}
		mpf(898f, FinaleUpdate, 1056f);
		mb(1047f, MoveFinalText);
		mb(1051f, CharlieSplat);
		mb(1055f, ShowFinalResults);
		charlieStartEvents.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.8f, "run"));
		charlieStartEvents.Add(new CharlieAction(charlie.waypoints[1].localPosition, 0.8f, "jump"));
		charlieStartEvents.Add(new CharlieAction(charlie.waypoints[2].localPosition, 1f, "jump", 1f, 2f));
		charlieStartEvents.Add(new CharlieAction(charlie.waypoints[2].localPosition, 0f, "hide"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[24].localPosition + Vector3.up * 5f, 0.1f, "warp"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[24].localPosition, 1.1f, "jump"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[25].localPosition, 5f, "run"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[3].localPosition, 1f, "jump"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[26].localPosition, 5.5f, "run"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[27].localPosition, 1f, "jump"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[4].localPosition, 5.5f, "run"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[5].localPosition, 1f, "jump"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[6].localPosition, 1f, "jump"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[22].localPosition, 1f, "jump"));
		charlieEndEvents1.Add(new CharlieAction(charlie.waypoints[22].localPosition, 0f, "hide"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[27].localPosition + Vector3.up * 5f, 0.1f, "warp"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[27].localPosition, 1f, "jump"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[4].localPosition, 5.5f, "run"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[5].localPosition, 1f, "jump"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[6].localPosition, 1f, "jump"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[22].localPosition, 1f, "jump"));
		charlieEndEvents1b.Add(new CharlieAction(charlie.waypoints[22].localPosition, 0f, "hide"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[22].localPosition, 0.1f, "warp", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[8].localPosition, 1.2f, "jump", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[9].localPosition, 1f, "run", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[10].localPosition, 1f, "jump", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[12].localPosition, 3.6f, "run", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[13].localPosition, 1f, "jump", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.4f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[14].localPosition, 1f, "jump"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.3f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[15].localPosition, 1f, "jump", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.4f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[16].localPosition, 1f, "jump"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.3f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[17].localPosition, 1f, "jump"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.4f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[18].localPosition, 1f, "jump"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.3f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[23].localPosition, 1f, "jump"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.4f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[19].localPosition, 1f, "jump", -1f));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[0].localPosition, 0.1f, "wait"));
		charlieEndEvents2.Add(new CharlieAction(charlie.waypoints[20].localPosition, 1.05f, "dive", -1f, 0f, "endingDive"));
		charlieWaitABit = new CharlieAction(charlie.waypoints[0].localPosition, 0.4f, "wait");
		charlieJumpToGoal = new CharlieAction(charlie.waypoints[28].localPosition, 1.3f, "jump", -1f);
		charlieVictory = new CharlieAction(charlie.waypoints[28].localPosition, 0f, "victory", -1f);
		finalSplat = new CharlieAction(charlie.waypoints[21].localPosition, 0f, "flop", -1f);
		SortTables();
	}

	private void Force3CountdownTicks()
	{
	}

	private void Force4CountdownTicks()
	{
	}

	private void SetupFonts()
	{
		FontData fontData = RDString.fontData;
		fontScale = ((fontData.fontScale < 1f) ? fontData.fontScale : (fontData.fontScale / 1.25f));
		mawaru_logo_subtitle.SetLocalizedFont();
		gameText.SetLocalizedFont();
		almostThere.SetLocalizedFont();
		sectionJudgment.SetLocalizedFont();
		defend_target_text.SetLocalizedFont();
		defend2_target_text.SetLocalizedFont();
		SystemLanguage language = RDString.language;
		switch (language)
		{
		case SystemLanguage.ChineseSimplified:
			mawaru_logo.SetState(1);
			mawaru_logo_nope.SetState(1);
			break;
		case SystemLanguage.ChineseTraditional:
			mawaru_logo.SetState(2);
			mawaru_logo_nope.SetState(2);
			break;
		case SystemLanguage.Korean:
			mawaru_logo.SetState(3);
			mawaru_logo_nope.SetState(3);
			mawaru_logo_nope.transform.localPosition = new Vector3(-4.4f, 2.6f, 0f);
			break;
		default:
			mawaru_logo.SetState(0);
			mawaru_logo_nope.SetState(0);
			break;
		}
		if (RDString.isChinese)
		{
			mawaru_logo_subtitle.transform.DOLocalMoveY(0.2f, 0f).SetRelative(isRelative: true);
		}
		else if (language == SystemLanguage.Korean)
		{
			mawaru_logo_subtitle.transform.DOLocalMoveY(0.1f, 0f).SetRelative(isRelative: true);
		}
	}

	private void CharlieStart()
	{
		foreach (CharlieAction charlieStartEvent in charlieStartEvents)
		{
			charlie.AddEntry(charlieStartEvent);
		}
	}

	private void CharlieEnding1()
	{
		charlie.lastSection = true;
		cha1 = true;
		foreach (CharlieAction item in charlieEndEvents1)
		{
			charlie.AddEntry(item);
		}
	}

	private void CharlieEnding1b()
	{
		charlie.lastSection = true;
		if (cha1)
		{
			return;
		}
		foreach (CharlieAction item in charlieEndEvents1b)
		{
			charlie.AddEntry(item);
		}
	}

	private void CharlieEnding2()
	{
		charlie.lastSection = true;
		foreach (CharlieAction item in charlieEndEvents2)
		{
			charlie.AddEntry(item);
		}
	}

	private void IntroLight1()
	{
		Mawaru_Sprite mawaru_Sprite = mawaru_gantry_lights[0];
		mawaru_Sprite.render.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_Sprite.transform.DOScale(Vector3.one * 1.2f, 0f))
			.Append(mawaru_Sprite.transform.DOScale(Vector3.one, 0.2f * speed));
	}

	private void IntroLight2()
	{
		Mawaru_Sprite mawaru_Sprite = mawaru_gantry_lights[1];
		mawaru_Sprite.render.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_Sprite.transform.DOScale(Vector3.one * 1.2f, 0f))
			.Append(mawaru_Sprite.transform.DOScale(Vector3.one, 0.2f * speed));
	}

	private void IntroLight3()
	{
		Mawaru_Sprite mawaru_Sprite = mawaru_gantry_lights[2];
		mawaru_Sprite.render.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_Sprite.transform.DOScale(Vector3.one * 1.2f, 0f))
			.Append(mawaru_Sprite.transform.DOScale(Vector3.one, 0.2f * speed));
	}

	private void IntroLight4()
	{
		foreach (Mawaru_Sprite mawaru_gantry_light in mawaru_gantry_lights)
		{
			mawaru_gantry_light.SetState(1);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_gantry_light.transform.DOScale(Vector3.one * 1.2f, 0f))
				.Append(mawaru_gantry_light.transform.DOScale(Vector3.one, 0.2f * speed));
		}
		mawaru_gantry_frame.transform.DOLocalMoveY(5f, beats(2f, 170f));
	}

	private void HideGantry()
	{
		mawaru_gantry_frame.transform.DOScale(Vector3.zero, 0f);
	}

	private void MawaruIntro()
	{
		if (slumpo)
		{
			Mawaru2_Stats.Reset();
		}
		else
		{
			Mawaru_Stats.Reset();
		}
		if (ADOBase.controller.currentState == States.PlayerControl)
		{
			mawaru_logo_flash.render.enabled = true;
			mawaru_logo.render.enabled = true;
			mawaru_logo_nope.render.enabled = true;
			subtitle_behind.render.enabled = true;
			fast_stars.render.enabled = true;
			s1 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo_flash.render.DOColor(new Color(1f, 1f, 1f, 0.6f), 0f))
				.Append(mawaru_logo_flash.render.DOColor(whiteClear, beats(4f, 170f)));
			s2 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fast_stars.render.DOColor(new Color(1f, 1f, 1f, 0f), 0f))
				.Append(fast_stars.render.DOColor(new Color(1f, 1f, 1f, 0.3f), beats(2f, 170f)))
				.AppendInterval(beats(5f, 170f))
				.Append(fast_stars.render.DOColor(whiteClear, beats(8f, 170f)).OnComplete(delegate
				{
					fast_stars.render.enabled = false;
				}));
			if (slumpo)
			{
				mawaru_logo_subtitle.text = RDString.Get("worldT3EX.description");
			}
			else
			{
				mawaru_logo_subtitle.text = RDString.Get("mawaru.subtitle");
			}
			s3 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo_subtitle.DOColor(whiteClear, 0f))
				.AppendInterval(beats(4f, 170f))
				.Append(mawaru_logo_subtitle.DOColor(Color.white, beats(1f, 170f)))
				.AppendInterval(beats(7f, 170f))
				.Append(mawaru_logo_subtitle.DOColor(whiteClear, beats(1f, 170f)));
			s4 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo_subtitle.transform.DOScale(Vector3.one * fontScale, 0f))
				.AppendInterval(beats(4f, 170f))
				.Append(mawaru_logo_subtitle.transform.DOScale(Vector3.one * 0.9f * fontScale, beats(9f, 170f)));
			s5 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(subtitle_behind.transform.DOScale(new Vector3(30f, 0f, 1f), 0f))
				.AppendInterval(beats(3.5f, 170f))
				.Append(subtitle_behind.transform.DOScale(new Vector3(30f, 1.2f, 1f), beats(0.5f, 170f)).SetEase(Ease.OutBack))
				.Append(subtitle_behind.transform.DOScale(new Vector3(30f, 1.05f, 1f), beats(8f, 170f)).SetEase(Ease.Linear))
				.Append(subtitle_behind.transform.DOScale(new Vector3(30f, 0f, 1f), beats(0.5f, 170f)).SetEase(Ease.InBack).OnComplete(delegate
				{
					subtitle_behind.render.enabled = false;
				}));
			s6 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo.transform.DOScale(Vector3.one * 1.05f, 0f))
				.Append(mawaru_logo.transform.DOScale(Vector3.one * 0.9f, beats(12f, 170f)).SetEase(Ease.Linear))
				.Append(mawaru_logo.transform.DOScale(Vector3.one * 0.5f, beats(2f, 170f)).SetEase(Ease.InQuad));
			s7 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo.render.DOColor(Color.white, 0f))
				.AppendInterval(beats(12f, 170f))
				.Append(mawaru_logo.render.DOColor(whiteClear, beats(2f, 170f)).OnComplete(delegate
				{
					mawaru_logo.render.enabled = false;
				}));
			s8 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo_nope.render.DOColor(Color.white, 0f))
				.AppendInterval(beats(12f, 170f))
				.Append(mawaru_logo_nope.render.DOColor(whiteClear, beats(2f, 170f)).OnComplete(delegate
				{
					mawaru_logo_nope.render.enabled = false;
				}));
			s9 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_logo_nope.transform.DOLocalMoveX(0.5f, beats(14f, 170f)).SetRelative(isRelative: true).SetEase(Ease.Linear));
			introSeqs.Add(s2);
			introSeqs.Add(s3);
			introSeqs.Add(s4);
			introSeqs.Add(s5);
			introSeqs.Add(s6);
			introSeqs.Add(s7);
			introSeqs.Add(s8);
			introSeqs.Add(s9);
		}
	}

	private void MawaruIntroUpdate()
	{
		if (songBeat > (double)next_sparkle)
		{
			SpawnSparkle();
			next_sparkle += 0.5f;
		}
		foreach (Sequence introSeq in introSeqs)
		{
			if (ADOBase.controller.currentState == States.Fail)
			{
				introSeq.Kill();
			}
		}
	}

	private void SpawnSparkle()
	{
		Mawaru_Sprite n = mawaru_logo_particles[curSparkleSpawn];
		n.render.enabled = true;
		n.enabled = true;
		n.transform.DOScale(Vector3.one * (1.5f + RandF() * 0.6f), 0f);
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(n.render.material.DOColor(whiteClear, 0f))
			.Append(n.render.material.DOColor(fadedParticles, 0.1f * speed).SetEase(Ease.Linear))
			.Append(n.render.material.DOColor(whiteClear, 1f * speed).SetEase(Ease.Linear))
			.OnComplete(delegate
			{
				n.render.enabled = false;
				n.enabled = false;
			});
		n.transform.localScale = Vector3.one * (1f + RandF());
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(n.transform.DOLocalMove(mawaru_logo.transform.localPosition + Vector3.forward * (3f + RandF() * 4f) + Vector3.right * (-5f + RandF() * 10f) + Vector3.up * (-2f + RandF() * 4f), 0f))
			.Append(n.transform.DOLocalMove(Vector3.forward * (-4f + RandF() * -4f), 1.1f * speed).SetEase(Ease.Linear).SetRelative(isRelative: true));
		curSparkleSpawn++;
		if (curSparkleSpawn >= mawaru_logo_particles.Count)
		{
			curSparkleSpawn = 0;
		}
	}

	private void InnervoiceSetup()
	{
		foreach (Mawaru_Sprite innervoice_cloud in innervoice_clouds)
		{
			innervoice_cloud.render.enabled = true;
			innervoice_cloud.render.material.SetColor("_Color", new Color(1f, 0.6f, 0.3f, 0f));
		}
		SystemLanguage language = RDString.language;
		for (int i = 0; i < innervoice_text.Count; i++)
		{
			innervoice_text[i].render.enabled = true;
			innervoice_text[i].render.material.SetColor("_Color", whiteClear);
			switch (language)
			{
			case SystemLanguage.ChineseSimplified:
				innervoice_text[i].SetState(1);
				break;
			case SystemLanguage.ChineseTraditional:
				innervoice_text[i].SetState(2);
				break;
			}
		}
		innervoice_skybg.render.enabled = true;
		innervoice_skybg.render.material.SetColor("_Color", whiteClear);
	}

	private void InnervoiceFadeIn()
	{
		innervoice_skybg.render.material.DOColor(Color.white, beats(4f, 140f));
		foreach (Mawaru_Sprite innervoice_cloud in innervoice_clouds)
		{
			innervoice_cloud.render.material.DOColor(new Color(1f, 0.6f, 0.3f, 1f), beats(4f, 140f));
		}
	}

	private void InnervoiceFadeOut()
	{
		innervoice_skybg.render.material.DOColor(whiteClear, beats(4f, 140f)).OnComplete(delegate
		{
			innervoice_skybg.render.enabled = false;
		});
		foreach (Mawaru_Sprite innervoice_cloud in innervoice_clouds)
		{
			innervoice_cloud.render.material.DOColor(new Color(1f, 0.6f, 0.3f, 0f), beats(4f, 140f));
		}
	}

	private void QuadSetup()
	{
		float num = -10f;
		float num2 = 8f;
		float num3 = 4f;
		float num4 = 2f;
		int num5 = 6;
		int num6 = 0;
		int num7 = 0;
		float num8 = 0f;
		Vector3 localScale = new Vector3(0f, 0f, 1f);
		Color color = new Color(1f, 0.7f, 0f, 0.6f);
		for (int i = 0; i < innervoice_quads.Count; i++)
		{
			innervoice_quads[i].render.enabled = true;
			innervoice_quads[i].render.material.SetColor("_Color", color * whiteClear);
			innervoice_quads[i].miscf = (float)num6 + ((num7 % 2 == 1) ? 0.5f : 0f);
			float num9 = num + (float)num6 * num3 + ((num7 % 2 == 1) ? (num3 * 0.5f) : 0f);
			num8 = num2 - (float)num7 * num4 - num9 * 0.06f;
			innervoice_quads[i].transform.localPosition = Vector3.right * num9 + Vector3.up * num8 - Vector3.forward * 2f;
			innervoice_quads[i].transform.localScale = localScale;
			num6++;
			if (num6 >= num5)
			{
				num6 = 0;
				num7++;
			}
		}
	}

	private void QuadSwipeRight()
	{
		QuadSwipe(1f);
	}

	private void QuadSwipeLeft()
	{
		QuadSwipe(-1f);
	}

	private void QuadSwipe(float dir)
	{
		innervoice_quads_rot.transform.eulerAngles = new Vector3(0f, 0f, 5f * dir);
		Color color = new Color(1f, 0.7f, 0f, 0.6f);
		Vector3 vector = new Vector3(1.6f, 1.6f, 1f);
		for (int i = 0; i < innervoice_quads.Count; i++)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval((dir > 0f) ? (innervoice_quads[i].miscf * 0.02f) : (0.15f - innervoice_quads[i].miscf * 0.02f))
				.Append(innervoice_quads[i].render.material.DOColor(new Color(color.r, color.g, color.b, 0.4f), 0.4f * speed))
				.AppendInterval(0.6f * speed)
				.Append(innervoice_quads[i].render.material.DOColor(new Color(color.r, color.g, color.b, 0f), 1f * speed));
			float y = innervoice_quads[i].transform.position.y;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval((dir > 0f) ? (innervoice_quads[i].miscf * 0.02f) : (0.15f - innervoice_quads[i].miscf * 0.02f))
				.Append(innervoice_quads[i].transform.DOMoveY(y, 0f))
				.Append(innervoice_quads[i].transform.DOMoveY(y + 1f, 1f * speed).SetEase(Ease.OutBack))
				.Append(innervoice_quads[i].transform.DOMoveY(y, 1f * speed).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval((dir > 0f) ? (innervoice_quads[i].miscf * 0.02f) : (0.15f - innervoice_quads[i].miscf * 0.02f))
				.Append(innervoice_quads[i].transform.DORotate(Vector3.zero, 0f))
				.AppendInterval(0.4f * speed)
				.Append(innervoice_quads[i].transform.DORotate(Vector3.forward * 210f * dir, 1f * speed, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval((dir > 0f) ? (innervoice_quads[i].miscf * 0.02f) : (0.15f - innervoice_quads[i].miscf * 0.02f))
				.Append(innervoice_quads[i].transform.DOScale(vector * 0.5f, 0f))
				.Append(innervoice_quads[i].transform.DOScale(vector, 0.5f * speed).SetEase(Ease.OutCubic))
				.AppendInterval(0.6f * speed)
				.Append(innervoice_quads[i].transform.DOScale(Vector3.zero, 1f * speed).SetEase(Ease.InCubic));
		}
	}

	private void InnervoiceText()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(innervoice_text[0].transform.DOMoveX(1f, 0.3f).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(innervoice_text[0].render.material.DOColor(new Color(1f, 1f, 1f, 1f), 0.3f * speed).SetEase(Ease.Linear))
			.AppendInterval(beats(2f, 140f))
			.Append(innervoice_text[0].render.material.DOColor(new Color(1f, 1f, 1f, 0f), 1.5f * speed).SetEase(Ease.Linear));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(9f / 14f / rate)
			.Append(innervoice_text[1].transform.DOMoveX(1f, 0.3f).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(9f / 14f / rate)
			.Append(innervoice_text[1].render.material.DOColor(new Color(1f, 1f, 1f, 1f), 0.3f * speed).SetEase(Ease.Linear))
			.AppendInterval(beats(2f, 140f))
			.Append(innervoice_text[1].render.material.DOColor(new Color(1f, 1f, 1f, 0f), 1.5f * speed).SetEase(Ease.Linear));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(1.0714285f / rate)
			.Append(innervoice_text[2].transform.DOMoveX(1f, 0.3f).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(1.0714285f / rate)
			.Append(innervoice_text[2].render.material.DOColor(new Color(1f, 1f, 1f, 1f), 0.3f * speed).SetEase(Ease.Linear))
			.AppendInterval(beats(2f, 140f))
			.Append(innervoice_text[2].render.material.DOColor(new Color(1f, 1f, 1f, 0f), 1.5f * speed).SetEase(Ease.Linear));
	}

	private void InnervoiceTextFade()
	{
	}

	private void InnervoiceHide()
	{
		innervoice_container.SetActive(value: false);
	}

	private void SolSetup()
	{
		sol_bg.render.material.SetColor("_Color", whiteClear);
		sol_bg.render.enabled = true;
		for (int i = 0; i < sol_stuff.Count; i++)
		{
			sol_stuff[i].render.material.SetColor("_Color", whiteClear);
		}
	}

	private void SolFadeIn()
	{
		sol_bg.render.material.DOColor(Color.white, beats(4f, 140f));
	}

	private void SolFadeOut()
	{
		sol_bg.render.material.DOColor(whiteClear, beats(4f, 140f)).OnComplete(delegate
		{
			sol_bg.render.enabled = false;
		});
	}

	private void SolMoon1()
	{
		SolActivate(sol_stuff[0]);
	}

	private void SolSun1()
	{
		SolActivate(sol_stuff[1]);
	}

	private void SolMoon2()
	{
		SolActivate(sol_stuff[2]);
	}

	private void SolSun2()
	{
		SolActivate(sol_stuff[3]);
	}

	private void SolActivate(Mawaru_Sprite s)
	{
		s.render.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(s.transform.DOScale(Vector3.zero, 0f))
			.Append(s.transform.DOScale(Vector3.one * 1.2f, 0.4f * speed).SetEase(Ease.OutBack));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(s.render.material.DOColor(new Color(1f, 1f, 1f, 0f), 0f))
			.Append(s.render.material.DOColor(new Color(1f, 1f, 1f, 0.6f), 0.3f * speed).SetEase(Ease.Linear));
	}

	private void SolHide()
	{
		sol_container.SetActive(value: false);
		sol_bg.render.enabled = false;
	}

	private void SoccerBootParent()
	{
		shouldBoots = true;
	}

	private void SoccerUpdate()
	{
		_ = songBeat;
		kickRad = kickTranslation.position.x;
		kickRad2 = kickTranslation2.position.x;
		kickPos = kickRad * Mathf.Sin((float)base.redPlanet.angle) * Vector3.right + kickRad * Mathf.Cos((float)base.redPlanet.angle) * Vector3.up;
		kickPos2 = kickRad2 * Mathf.Sin((float)base.bluePlanet.angle) * Vector3.right + kickRad2 * Mathf.Cos((float)base.bluePlanet.angle) * Vector3.up;
		if (shouldBoots)
		{
			soccerBoots[0].transform.position = base.bluePlanet.transform.position + kickPos;
			soccerBoots[1].transform.position = base.redPlanet.transform.position + kickPos2;
		}
		if (!slumpo)
		{
			for (int i = 0; i < 2; i++)
			{
				if (soccerKicked[i] || soccerBalls[i].floor.grade == HitMargin.TooEarly)
				{
					continue;
				}
				soccerKicked[i] = true;
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(kickTranslation.DOMoveX(1f, beats(0.25f)).SetEase(Ease.OutQuad))
					.Append(kickTranslation.DOMoveX(0f, beats(1f, 140f)).SetEase(Ease.InQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBoots[0].transform.DOScale(Vector3.one * 1.5f, 0f))
					.Append(soccerBoots[0].transform.DOScale(Vector3.one, beats(1f, 140f)).SetEase(Ease.OutQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBoots[0].transform.DORotate(Vector3.forward * 60f, 0f))
					.Append(soccerBoots[0].transform.DORotate(Vector3.zero, beats(0.75f, 140f)).SetEase(Ease.InBack));
				if (soccerBalls[i].floor.grade == HitMargin.Perfect)
				{
					mawaru_soccer_scored[i] = 1;
					switch (i)
					{
					case 0:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(Vector3.up * 3.6f, 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(Vector3.up * -0.8f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.3f * speed)
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 210f, 0.4f * speed, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
						break;
					case 1:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(2.5f, 2f, 0f), 0.35f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(0.5f, -0.4f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 250f, 0.4f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -30f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					}
				}
				else if (soccerBalls[i].floor.grade == HitMargin.EarlyPerfect)
				{
					switch (i)
					{
					case 0:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(1.8f, 3.2f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(1.8f, -3.2f, 0f) * 0.2f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 40f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					case 1:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(4.5f, 1.1f, 0f), 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(4.5f, -1.1f, 0f) * 0.2f, 0.25f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 250f, 0.4f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -10f, 0.25f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					}
				}
				else if (soccerBalls[i].floor.grade == HitMargin.LatePerfect)
				{
					switch (i)
					{
					case 0:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(-1.8f, 3.2f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(-1.8f, -3.2f, 0f) * 0.2f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -40f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					case 1:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(1f, 1.2f, 0f), 0.18f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(-2f, 0.2f, 0f) * 0.5f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -120f, 0.18f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 250f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					}
				}
				else if (soccerBalls[i].floor.grade == HitMargin.VeryEarly)
				{
					switch (i)
					{
					case 0:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(2.5f, 2.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(2.5f, 2.5f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 60f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					case 1:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(1f, 0.2f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * 90f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					}
				}
				else if (soccerBalls[i].floor.grade == HitMargin.VeryLate)
				{
					switch (i)
					{
					case 0:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(-2.5f, 2.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(-2.5f, 2.5f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -60f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					case 1:
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].transform.DOMove(new Vector3(-0.5f, 3f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
							.Append(soccerBalls[i].transform.DOMove(new Vector3(-0.5f, 3f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -150f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
							.Append(soccerBalls[i].ball.transform.DORotate(Vector3.forward * -50f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
						break;
					}
				}
			}
			return;
		}
		for (int j = 0; j < 4; j++)
		{
			if (soccerKicked[j] || soccerBalls[j].floor.grade == HitMargin.TooEarly)
			{
				continue;
			}
			soccerKicked[j] = true;
			if (j < 3)
			{
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(kickTranslation.DOMoveX(1f, beats(0.25f)).SetEase(Ease.OutQuad))
					.Append(kickTranslation.DOMoveX(0f, beats(1f, 140f)).SetEase(Ease.InQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBoots[0].transform.DOScale(Vector3.one * 1.5f, 0f))
					.Append(soccerBoots[0].transform.DOScale(Vector3.one, beats(1f, 140f)).SetEase(Ease.OutQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBoots[0].transform.DORotate(Vector3.forward * 60f, 0f))
					.Append(soccerBoots[0].transform.DORotate(Vector3.zero, beats(0.75f, 140f)).SetEase(Ease.InBack));
			}
			else
			{
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(kickTranslation2.DOMoveX(1f, beats(0.25f)).SetEase(Ease.OutQuad))
					.Append(kickTranslation2.DOMoveX(0f, beats(1f, 140f)).SetEase(Ease.InQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBoots[1].transform.DOScale(Vector3.one * 1.5f, 0f))
					.Append(soccerBoots[1].transform.DOScale(Vector3.one, beats(1f, 140f)).SetEase(Ease.OutQuad));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBoots[1].transform.DORotate(Vector3.forward * 60f, 0f))
					.Append(soccerBoots[1].transform.DORotate(Vector3.zero, beats(0.75f, 140f)).SetEase(Ease.InBack));
			}
			if (soccerBalls[j].floor.grade == HitMargin.Perfect)
			{
				mawaru_soccer_scored[j] = 1;
				switch (j)
				{
				case 0:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(4f, 4f, 0f), 0.5f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(2f, -2f, 0f), 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.3f * speed)
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 210f, 0.5f * speed, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
					break;
				case 1:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(Vector3.up * 3.6f, 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(Vector3.up * -0.8f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.3f * speed)
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 210f, 0.4f * speed, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
					break;
				case 2:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(2.5f, 2f, 0f), 0.35f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(0.5f, -0.4f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 250f, 0.4f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -30f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 3:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(Vector3.up * 1.8f, 0.15f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(Vector3.up * -0.8f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.3f * speed)
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 210f, 0.25f * speed, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
					break;
				}
			}
			else if (soccerBalls[j].floor.grade == HitMargin.EarlyPerfect)
			{
				switch (j)
				{
				case 0:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(6f, 3f, 0f), 0.6f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(6f, -3f, 0f) * 0.2f, 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -200f, 0.6f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 40f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 1:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(1.8f, 3.2f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(1.8f, -3.2f, 0f) * 0.2f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 40f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 2:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(4.5f, 1.1f, 0f), 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(4.5f, -1.1f, 0f) * 0.2f, 0.25f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 250f, 0.4f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -10f, 0.25f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 3:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(-1f, 1f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(1.5f, -0.6f, 0f) * 0.7f, 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 40f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				}
			}
			else if (soccerBalls[j].floor.grade == HitMargin.LatePerfect)
			{
				switch (j)
				{
				case 0:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(2.5f, 3.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(-2.5f, -3.5f, 0f) * 0.2f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -40f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 1:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(-1.8f, 3.2f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(-1.8f, -3.2f, 0f) * 0.2f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -40f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 2:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(1f, 1.2f, 0f), 0.18f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(-2f, 0.2f, 0f) * 0.5f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -120f, 0.18f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 250f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 3:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(2f, 1f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(1f, -1f, 0f) * 0.5f, 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 40f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				}
			}
			else if (soccerBalls[j].floor.grade == HitMargin.VeryEarly)
			{
				switch (j)
				{
				case 0:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(3f, -3f, 0f), 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(3f, -3f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 60f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 1:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(2.5f, 2.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(2.5f, 2.5f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 60f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 2:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(1f, 0.2f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 90f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 3:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(3f, 0.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * 90f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				}
			}
			else if (soccerBalls[j].floor.grade == HitMargin.VeryLate)
			{
				switch (j)
				{
				case 0:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(1f, 6f, 0f), 0.6f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -90f, 0.6f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 1:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(-2.5f, 2.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(-2.5f, 2.5f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -200f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -60f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 2:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(-0.5f, 3f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear))
						.Append(soccerBalls[j].transform.DOMove(new Vector3(-0.5f, 3f, 0f) * 0.3f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -150f, 0.3f * speed, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -50f, 0.4f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				case 3:
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].transform.DOMove(new Vector3(-3f, 0.5f, 0f), 0.3f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(soccerBalls[j].ball.transform.DORotate(Vector3.forward * -90f, 0.3f * speed, RotateMode.FastBeyond360).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
					break;
				}
			}
		}
	}

	private void SoccerSetup()
	{
		soccer_field.render.material.SetColor("_Color", whiteClear);
		soccer_field.render.enabled = true;
		soccer_goal2.render.enabled = true;
		SystemLanguage language = RDString.language;
		for (int i = 0; i < mawaru_soccer_text.Count; i++)
		{
			switch (language)
			{
			case SystemLanguage.ChineseSimplified:
				mawaru_soccer_text[i].SetState(1);
				break;
			case SystemLanguage.ChineseTraditional:
				mawaru_soccer_text[i].SetState(2);
				break;
			}
		}
		mawaru_soccer_playing = true;
	}

	private void SoccerFadeIn()
	{
		soccer_field.render.material.DOColor(Color.white, beats(4f, 140f));
	}

	private void SoccerFadeOut()
	{
		soccer_field.render.material.DOColor(whiteClear, beats(4f, 140f)).OnComplete(delegate
		{
			soccer_field.render.enabled = false;
		});
	}

	private void SoccerHide()
	{
		soccer_container.SetActive(value: false);
		soccer_goal2.render.enabled = false;
		for (int i = 0; i < mawaru_soccer_text.Count; i++)
		{
			mawaru_soccer_text[i].render.enabled = false;
			mawaru_soccer_splash[i].render.enabled = false;
		}
	}

	private void SoccerCheckGoal1()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			int index = 0;
			if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 1)
			{
				mawaru_soccer_text[index].transform.DORotate(Vector3.right * 720f, beats(2f, 140f)).SetEase(Ease.Linear);
				mawaru_soccer_text[index].transform.DOScale(Vector3.one, beats(2f, 140f)).SetEase(Ease.OutCubic);
				mawaru_soccer_splash[index].transform.DOScale(Vector3.one * 0.9f, beats(3f, 140f)).SetEase(Ease.OutExpo);
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, 0f))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalRed, beats(0.5f, 140f)))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, beats(0.5f, 140f)))
					.SetLoops(-1, LoopType.Restart)
					.SetUpdate(isIndependentUpdate: true);
			}
		}
	}

	private void SoccerCheckGoal2()
	{
		if (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		int index = 1;
		if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 1)
		{
			mawaru_soccer_text[index].transform.DORotate(Vector3.right * 720f, beats(2f, 140f)).SetEase(Ease.Linear);
			mawaru_soccer_text[index].transform.DOScale(Vector3.one, beats(2f, 140f)).SetEase(Ease.OutCubic);
			mawaru_soccer_splash[index].transform.DOScale(Vector3.one * 0.9f, beats(3f, 140f)).SetEase(Ease.OutExpo);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, 0f))
				.Append(mawaru_soccer_splash[index].render.DOColor(GoalRed, beats(0.5f, 140f)))
				.Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, beats(0.5f, 140f)))
				.SetLoops(-1, LoopType.Restart)
				.SetUpdate(isIndependentUpdate: true);
		}
		int num = 0;
		foreach (int item in mawaru_soccer_scored)
		{
			num += item;
		}
		if (slumpo)
		{
			Mawaru2_Stats.goals = num;
		}
		else
		{
			Mawaru_Stats.goals = num;
		}
		if (mawaru_soccer_playing && ((num < 1 && !slumpo) || (num != mawaru_soccer_scored.Count && slumpo)))
		{
			Fail();
		}
	}

	private void SoccerCheckGoal1S()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			int index = 0;
			if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 1)
			{
				mawaru_soccer_text[index].transform.DORotate(Vector3.right * 720f, beats(2f, 140f)).SetEase(Ease.Linear);
				mawaru_soccer_text[index].transform.DOScale(Vector3.one, beats(2f, 140f)).SetEase(Ease.OutCubic);
				mawaru_soccer_splash[index].transform.DOScale(Vector3.one * 0.9f, beats(3f, 140f)).SetEase(Ease.OutExpo);
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, 0f))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalRed, beats(0.5f, 140f)))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, beats(0.5f, 140f)))
					.SetLoops(-1, LoopType.Restart)
					.SetUpdate(isIndependentUpdate: true);
			}
			else if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 0)
			{
				Fail();
			}
		}
	}

	private void SoccerCheckGoal2S()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			int index = 1;
			if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 1)
			{
				mawaru_soccer_text[index].transform.DORotate(Vector3.right * 720f, beats(2f, 140f)).SetEase(Ease.Linear);
				mawaru_soccer_text[index].transform.DOScale(Vector3.one, beats(2f, 140f)).SetEase(Ease.OutCubic);
				mawaru_soccer_splash[index].transform.DOScale(Vector3.one * 0.9f, beats(3f, 140f)).SetEase(Ease.OutExpo);
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, 0f))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalRed, beats(0.5f, 140f)))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, beats(0.5f, 140f)))
					.SetLoops(-1, LoopType.Restart)
					.SetUpdate(isIndependentUpdate: true);
			}
			else if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 0)
			{
				Fail();
			}
		}
	}

	private void SoccerCheckGoal3S()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			int index = 2;
			if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 1)
			{
				mawaru_soccer_text[index].transform.DORotate(Vector3.right * 720f, beats(2f, 140f)).SetEase(Ease.Linear);
				mawaru_soccer_text[index].transform.DOScale(Vector3.one, beats(2f, 140f)).SetEase(Ease.OutCubic);
				mawaru_soccer_splash[index].transform.DOScale(Vector3.one * 0.9f, beats(3f, 140f)).SetEase(Ease.OutExpo);
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, 0f))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalRed, beats(0.5f, 140f)))
					.Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, beats(0.5f, 140f)))
					.SetLoops(-1, LoopType.Restart)
					.SetUpdate(isIndependentUpdate: true);
			}
			else if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 0)
			{
				Fail();
			}
		}
	}

	private void SoccerCheckGoal4S()
	{
		if (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		int index = 3;
		if (mawaru_soccer_playing && mawaru_soccer_scored[index] == 1)
		{
			mawaru_soccer_text[index].transform.DORotate(Vector3.right * 720f, beats(2f, 140f)).SetEase(Ease.Linear);
			mawaru_soccer_text[index].transform.DOScale(Vector3.one, beats(2f, 140f)).SetEase(Ease.OutCubic);
			mawaru_soccer_splash[index].transform.DOScale(Vector3.one * 0.9f, beats(3f, 140f)).SetEase(Ease.OutExpo);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, 0f))
				.Append(mawaru_soccer_splash[index].render.DOColor(GoalRed, beats(0.5f, 140f)))
				.Append(mawaru_soccer_splash[index].render.DOColor(GoalYellow, beats(0.5f, 140f)))
				.SetLoops(-1, LoopType.Restart)
				.SetUpdate(isIndependentUpdate: true);
		}
		int num = 0;
		foreach (int item in mawaru_soccer_scored)
		{
			num += item;
		}
		if (slumpo)
		{
			Mawaru2_Stats.goals = num;
		}
		else
		{
			Mawaru_Stats.goals = num;
		}
		if (mawaru_soccer_playing && ((num < 1 && !slumpo) || (num != mawaru_soccer_scored.Count && slumpo)))
		{
			Fail();
		}
	}

	private void TrianglesSetup()
	{
		EnableStuff(triangle_bgstuff);
		EnableStuff(triangles);
	}

	private void TrianglesFadeIn()
	{
		FadeStuff(triangle_bgstuff, 1f, beats(4f, 155f));
	}

	private void TrianglesFadeOut()
	{
		FadeStuff(triangle_bgstuff, 0f, beats(4f, 155f));
		FadeStuff(triangles, 0f, beats(4f, 155f));
	}

	private void TriangleAppear()
	{
		Mawaru_Sprite mawaru_Sprite = triangles[current_tri];
		float num = ((current_tri % 2 == 0) ? 1 : (-1));
		Vector3 vector = ((current_tri % 2 == 0) ? base.redPlanet.transform.position : base.bluePlanet.transform.position);
		mawaru_Sprite.transform.position = vector + (3f + RandF() * 2f) * num * Vector3.up + (-3f + RandF() * 4f) * Vector3.forward + (-1f + RandF() * 2f) * Vector3.right;
		FadeStuff(mawaru_Sprite, 0.6f, 0f);
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_Sprite.transform.DOScale(Vector3.zero, 0f))
			.Append(mawaru_Sprite.transform.DORotate(Vector3.forward * RandF() * 360f, 0f))
			.Append(mawaru_Sprite.transform.DOScale(Vector3.one * (1.5f + 1f * RandF()), 0.4f * speed).SetEase(Ease.OutBack));
		current_tri++;
	}

	private void Triangle2Appear()
	{
		Mawaru_Sprite mawaru_Sprite = triangles2[current_tri2];
		float num = ((current_tri2 % 2 == 0) ? 1 : (-1));
		Vector3 vector = ((current_tri2 % 2 == 0) ? base.redPlanet.transform.position : base.bluePlanet.transform.position);
		mawaru_Sprite.transform.position = vector + (3f + RandF() * 2f) * num * Vector3.right + (-3f + RandF() * 4f) * Vector3.forward + (2f + RandF() * 2f) * Vector3.up;
		FadeStuff(mawaru_Sprite, 0.5f, 0f);
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(mawaru_Sprite.transform.DOScale(Vector3.zero, 0f))
			.Append(mawaru_Sprite.transform.DORotate(Vector3.forward * RandF() * 360f, 0f))
			.Append(mawaru_Sprite.transform.DOScale(Vector3.one * (1.5f + 1f * RandF()), 0.4f * speed).SetEase(Ease.OutBack));
		current_tri2++;
	}

	private void Triangle2GhostAppear()
	{
		Mawaru_Sprite t = ghosts[current_ghost];
		float num = ((current_ghost == 3) ? 1 : (-1));
		float num2 = ((current_ghost != 1) ? 1 : (-1));
		Vector3 vector = ((current_ghost % 2 == 0) ? base.redPlanet.transform.position : base.bluePlanet.transform.position);
		vector += num * Vector3.right;
		t.render.enabled = true;
		t.transform.localScale = new Vector3(0f - num, num2, 1f) * 1.3f;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(t.transform.DOMove(vector + Vector3.up * 0.6f, 0f))
			.Append(t.transform.DOMoveX(num * 1.5f, 0.5f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(t.transform.DOMoveY(num2 * 1.5f, 0.6f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(t.render.material.DOColor(whiteClear, 0f))
			.Append(t.render.material.DOColor(Color.white - Color.black * 0.4f, 0.2f * speed).SetEase(Ease.Linear))
			.Append(t.render.material.DOColor(whiteClear, 0.4f * speed).SetEase(Ease.Linear).OnComplete(delegate
			{
				t.render.enabled = false;
			}));
		current_ghost++;
	}

	private void Triangles2Setup()
	{
		EnableStuff(triangle2_bgstuff);
		EnableStuff(triangles2);
	}

	private void Triangles2FadeIn()
	{
		FadeStuff(triangle2_bgstuff, 1f, beats(8f, 215f));
	}

	private void Triangles2FadeOut()
	{
		FadeStuff(triangle2_bgstuff, 0f, beats(8f, 215f));
		FadeStuff(triangles2, 0f, beats(8f, 215f));
	}

	private void HighFiveSetup()
	{
		EnableStuff(h5_bgstuff);
		foreach (Mawaru_Sprite item in h5_bottomrow)
		{
			item.miscf = item.transform.position.y;
		}
		foreach (Mawaru_Sprite item2 in h5_toprow)
		{
			item2.miscf = item2.transform.position.y;
		}
	}

	private void HighFiveFadeIn()
	{
		FadeStuff(h5_bgstuff, 1f, beats(4f, 155f));
	}

	private void HighFiveFadeOut()
	{
		FadeStuff(h5_bgstuff, 0f, beats(4f, 155f));
		Mawaru_Stats.hifive = 0;
		Mawaru2_Stats.hifive = 0;
		foreach (Mawaru_Arm h5_arm in h5_arms)
		{
			if (h5_arm.hitgood)
			{
				if (slumpo)
				{
					Mawaru2_Stats.hifive++;
				}
				else
				{
					Mawaru_Stats.hifive++;
				}
			}
		}
	}

	private void HighFiveUpdate()
	{
		foreach (Mawaru_Arm h5_arm in h5_arms)
		{
			if (h5_arm.floor != null && h5_arm.floor.grade != HitMargin.TooEarly && !h5_arm.slapped)
			{
				if (h5_arm.floor.grade == HitMargin.VeryEarly || h5_arm.floor.grade == HitMargin.VeryLate || h5_arm.floor.grade == HitMargin.EarlyPerfect || h5_arm.floor.grade == HitMargin.LatePerfect)
				{
					h5_arm.HitOK();
				}
				else if (h5_arm.floor.grade == HitMargin.Perfect)
				{
					h5_arm.Hit();
				}
			}
		}
		for (int i = 0; i < 3; i++)
		{
			h5_bottomrow[i].transform.position = h5_bottomrow[i].startpos + Vector3.up * 0.25f * Mathf.Abs(Mathf.Sin(((float)songBeat + (float)i * 0.1f) * (float)Math.PI));
			h5_toprow[i].transform.position = h5_toprow[i].startpos + Vector3.up * -0.25f * Mathf.Abs(Mathf.Sin(((float)songBeat + (float)i * 0.1f) * (float)Math.PI));
		}
	}

	private void FreeroamSetup()
	{
		if (!slumpo)
		{
			for (int i = 0; i < 3; i++)
			{
				collect_coins[i].transform.position += Vector3.right * 1.5f * UnityEngine.Random.Range(-1, 2);
			}
		}
	}

	private void FreeroamUpdate()
	{
		foreach (Mawaru_Coin collect_coin in collect_coins)
		{
			if (collect_coin.collected || ADOBase.controller.currentState != States.PlayerControl)
			{
				continue;
			}
			foreach (scrPlanet planet in ADOBase.controller.planetarySystem.planetList)
			{
				if (Vector3.Distance(planet.transform.position, collect_coin.pos) < 0.3f)
				{
					collect_coin.Collect();
					scrSfx.instance.PlaySfx(SfxSound.MawaruCoin, MixerGroup.SfxParent, 0.4f);
					coins++;
				}
			}
		}
	}

	private void FreeroamCheckCoins()
	{
		if (slumpo)
		{
			Mawaru2_Stats.coins = coins;
		}
		else
		{
			Mawaru_Stats.coins = coins;
		}
		if (coins < 3)
		{
			Fail();
		}
	}

	private void OskariAppear()
	{
		if (ADOBase.controller.currentState == States.PlayerControl)
		{
			remember.Rise();
		}
	}

	private void OskariDisappear()
	{
		if (ADOBase.controller.currentState == States.PlayerControl)
		{
			remember.Fall();
		}
	}

	private void LofiSetup()
	{
		EnableStuff(lofi_bgstuff);
	}

	private void LofiFadeIn()
	{
		FadeStuff(lofi_bgstuff, 1f, beats(8f, 170f));
	}

	private void LofiFadeOut()
	{
		FadeStuff(lofi_bgstuff, 0f, beats(4f, 170f));
	}

	private void LofiUpdate()
	{
		for (int i = 0; i < 9; i++)
		{
			lofi_sub = (float)i / 8f * 0.25f;
			lofi_alp = Mathf.Max(0f, 0.4f - lofi_sub + 0.2f * Mathf.Sin((float)Math.PI * 2f * ((float)i / 9f) + (float)songBeat * 0.33f * (float)Math.PI));
			lofi_origColor = lofi_links[i].render.material.GetColor("_Color");
			lofi_newColor = new Color(lofi_origColor.r, lofi_origColor.g, lofi_origColor.b, lofi_alp);
			lofi_links[i].render.material.SetColor("_Color", lofi_newColor);
		}
	}

	private void LofiWarp()
	{
		if (curLofiWarp < lofi_warptiles.Count)
		{
			lofi_warptiles[curLofiWarp].Trip();
		}
		curLofiWarp++;
	}

	private void MathUpdate()
	{
		for (int i = 0; i < 2; i++)
		{
			if (ADOBase.controller.currFloor.seqID == math1_floors[i] && (ADOBase.controller.playerOne.missesOnCurrFloor.Count > 0 || ADOBase.controller.currentState == States.Fail) && !math1_miss[i])
			{
				math1_miss[i] = true;
				Fail();
				FadeStuff(math1_fakenumber[i], new Color(1f, 0f, 0f, 1f), 0f);
				FadeStuff(math1_fakenumber[i], new Color(1f, 0f, 0f, 0.5f), 0.5f);
				math1_fakenumber[i].transform.DOScale(Vector3.one * 0.5f, 0.5f);
				FadeStuff(math1_fakefloor[i], new Color(1f, 0f, 0f, 1f), 0f);
				FadeStuff(math1_fakefloor[i], new Color(1f, 0f, 0f, 0f), 0.5f);
			}
			if (ADOBase.controller.currFloor.seqID == math1_floors[i] + 3 && !movedCameraMath1[i])
			{
				movedCameraMath1[i] = true;
				ADOBase.controller.MoveCameraToTile(scrLevelMaker.instance.listFloors[math1_floors[i] + 4], scrLevelMaker.instance.listFloors[math1_floors[i] + 3], beats(4f, 170f), Ease.InOutSine);
				scrLevelMaker.instance.listFloors[math1_floors[i] + 3].transform.DOMove(scrLevelMaker.instance.listFloors[math1_floors[i] + 4].transform.position + Vector3.left * 1.5f, beats(4f, 170f)).SetEase(Ease.InOutSine);
			}
		}
	}

	private void RandomizeTiles()
	{
		math_floors.Add(math1_floors[0]);
		math_floors.Add(math1_floors[1]);
		math_floors.Add(math2_floors[0]);
		math_floors.Add(math2_floors[1]);
		for (int i = 0; i < 4; i++)
		{
			bool num = RandF() < 0.5f;
			if (!num)
			{
				_ = math_correctAnswer[i];
				_ = 0f;
			}
			if (num)
			{
				math_movableFloor[i * 3].render.flipY = !math_movableFloor[i * 3].render.flipY;
				math_movableFloor[i * 3].transform.position += Vector3.up * 3f * math_correctAnswer[i];
				math_movableFloor[i * 3 + 1].transform.position += Vector3.up * 3f * math_correctAnswer[i];
				math_movableFloor[i * 3 + 2].transform.position += Vector3.up * 3f * math_correctAnswer[i];
				math_randomize[i * 3 + 1].transform.position += Vector3.up * 2.78f;
				math_randomize[i * 3 + 2].transform.position += Vector3.up * -2.78f;
				scrFloor scrFloor2 = scrLevelMaker.instance.listFloors[math_floors[i]];
				scrFloor scrFloor3 = scrLevelMaker.instance.listFloors[math_floors[i] + 1];
				if (math_correctAnswer[i] > 0f)
				{
					scrFloor2.exitangle = 3.1415927;
					scrFloor2.entryangle = 4.7123889;
					ADOBase.lm.CalculateSingleFloorAngleLength(scrFloor2);
					scrFloor3.exitangle = 1.5707963;
					scrFloor3.entryangle = 0.0;
					ADOBase.lm.CalculateSingleFloorAngleLength(scrFloor3);
					scrFloor3.transform.localScale = new Vector3(1f, -1f, 1f);
				}
				else
				{
					scrFloor2.exitangle = 0.0;
					scrFloor2.entryangle = 4.7123889;
					ADOBase.lm.CalculateSingleFloorAngleLength(scrFloor2);
					scrFloor3.exitangle = 1.5707963;
					scrFloor3.entryangle = 3.1415927;
					ADOBase.lm.CalculateSingleFloorAngleLength(scrFloor3);
					scrFloor3.transform.localScale = new Vector3(1f, -1f, 1f);
				}
				scrLevelMaker.instance.listFloors[math_floors[i] + 1].transform.position -= Vector3.up * 3f * math_correctAnswer[i];
				scrLevelMaker.instance.listFloors[math_floors[i] + 2].transform.position -= Vector3.up * 3f * math_correctAnswer[i];
				scrLevelMaker.instance.listFloors[math_floors[i] + 3].transform.position -= Vector3.up * 3f * math_correctAnswer[i];
			}
		}
	}

	private void MathRandomize()
	{
		float pitch = ADOBase.conductor.song.pitch;
		List<int> list = new List<int>
		{
			UnityEngine.Random.Range(0, 4),
			UnityEngine.Random.Range(0, 4),
			UnityEngine.Random.Range(0, 4),
			UnityEngine.Random.Range(0, 4)
		};
		if (!slumpo)
		{
			if ((double)pitch < 1.3)
			{
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						math_randomize[i * 3 + j].SetState(list[i]);
					}
				}
				math_randomize[9].render.sprite = math_extraquestions3c[list[3]];
				math_randomize[10].render.sprite = math_extraanswers3c[list[3] * 2];
				math_randomize[11].render.sprite = math_extraanswers3c[list[3] * 2 + 1];
			}
			if ((double)pitch >= 1.3 && (double)pitch < 1.6)
			{
				math_randomize[0].SetState(list[0]);
				math_randomize[1].SetState(list[0]);
				math_randomize[2].SetState(list[0]);
				math_randomize[3].SetState(list[1]);
				math_randomize[4].SetState(list[1]);
				math_randomize[5].SetState(list[1]);
				math_randomize[6].render.sprite = math_extraquestions2b[list[2]];
				math_randomize[7].render.sprite = math_extraanswers2b[list[2] * 2 + 1];
				math_randomize[8].render.sprite = math_extraanswers2b[list[2] * 2];
				math_randomize[9].render.sprite = math_extraquestions3[list[3]];
				math_randomize[10].render.sprite = math_extraanswers3[list[3] * 2];
				math_randomize[11].render.sprite = math_extraanswers3[list[3] * 2 + 1];
			}
			else if ((double)pitch >= 1.6 && pitch < 2f)
			{
				math_randomize[0].SetState(list[0]);
				math_randomize[1].SetState(list[0]);
				math_randomize[2].SetState(list[0]);
				math_randomize[3].render.sprite = math_extraquestions1b[list[1]];
				math_randomize[4].render.sprite = math_extraanswers1b[list[1] * 2 + 1];
				math_randomize[5].render.sprite = math_extraanswers1b[list[1] * 2];
				math_randomize[6].render.sprite = math_extraquestions2[list[2]];
				math_randomize[7].render.sprite = math_extraanswers2[list[2] * 2 + 1];
				math_randomize[8].render.sprite = math_extraanswers2[list[2] * 2];
				math_randomize[9].render.sprite = math_extraquestions2b[list[3]];
				math_randomize[10].render.sprite = math_extraanswers2b[list[3] * 2];
				math_randomize[11].render.sprite = math_extraanswers2b[list[3] * 2 + 1];
			}
			else if (pitch >= 2f)
			{
				math_randomize[0].SetState(list[0]);
				math_randomize[1].SetState(list[0]);
				math_randomize[2].SetState(list[0]);
				math_randomize[3].render.sprite = math_extraquestions1b[list[1]];
				math_randomize[4].render.sprite = math_extraanswers1b[list[1] * 2 + 1];
				math_randomize[5].render.sprite = math_extraanswers1b[list[1] * 2];
				math_randomize[6].render.sprite = math_extraquestions1c[list[2]];
				math_randomize[7].render.sprite = math_extraanswers1c[list[2] * 2 + 1];
				math_randomize[8].render.sprite = math_extraanswers1c[list[2] * 2];
				math_randomize[9].render.sprite = math_extraquestions1d[list[3]];
				math_randomize[10].render.sprite = math_extraanswers1d[list[3] * 2];
				math_randomize[11].render.sprite = math_extraanswers1d[list[3] * 2 + 1];
			}
		}
		else
		{
			math_randomize[0].render.sprite = math_extraquestions3[list[0]];
			math_randomize[0].transform.position += Vector3.left * 1f;
			math_randomize[1].render.sprite = math_extraanswers3[list[0] * 2];
			math_randomize[2].render.sprite = math_extraanswers3[list[0] * 2 + 1];
			math_randomize[3].render.sprite = math_extraquestions3c[list[1]];
			math_randomize[4].render.sprite = math_extraanswers3c[list[1] * 2 + 1];
			math_randomize[5].render.sprite = math_extraanswers3c[list[1] * 2];
			math_randomize[6].render.sprite = math_extraquestions4[list[2]];
			math_randomize[7].render.sprite = math_extraanswers4[list[2] * 2 + 1];
			math_randomize[8].render.sprite = math_extraanswers4[list[2] * 2];
			math_randomize[9].render.sprite = math_extraquestions3b[list[3]];
			math_randomize[10].render.sprite = math_extraanswers3b[list[3] * 2];
			math_randomize[11].render.sprite = math_extraanswers3b[list[3] * 2 + 1];
		}
	}

	private void MathTint1()
	{
		FadeStuff(math1_floorstotint, math1_floorcolor, 0f);
	}

	private void MathTint2()
	{
		FadeStuff(math2_floorstotint, math2_floorcolor, 0f);
	}

	private void MathSetup()
	{
		EnableStuff(math1_bgstuff);
	}

	private void MathFadeIn()
	{
		FadeStuff(math1_bgstuff, 1f, beats(8f, 170f));
		FadeUITextColor(Color.black, new Color(1f, 1f, 1f, 0.84f), beats(1f, 170f));
	}

	private void MathFadeOut()
	{
		FadeStuff(math1_bgstuff, 0f, beats(8f, 170f));
		FadeUITextColor(Color.white, new Color(0.19f, 0.19f, 0.19f, 0.84f), beats(1f, 170f));
	}

	private void Math2Update()
	{
		if (songBeat > (double)next_num)
		{
			SpawnNumber();
			next_num += 1f;
		}
		for (int i = 0; i < 2; i++)
		{
			if (ADOBase.controller.currFloor.seqID == math2_floors[i] && (ADOBase.controller.playerOne.missesOnCurrFloor.Count > 0 || ADOBase.controller.currentState == States.Fail) && !math2_miss[i])
			{
				math2_miss[i] = true;
				Fail();
				FadeStuff(math2_fakenumber[i], new Color(1f, 0f, 0f, 1f), 0f);
				FadeStuff(math2_fakenumber[i], new Color(1f, 0f, 0f, 0.5f), 0.5f);
				math2_fakenumber[i].transform.DOScale(Vector3.one * 0.5f, 0.5f);
				FadeStuff(math2_fakefloor[i], new Color(1f, 0f, 0f, 1f), 0f);
				FadeStuff(math2_fakefloor[i], new Color(1f, 0f, 0f, 0f), 0.5f);
			}
			if (ADOBase.controller.currFloor.seqID == math2_floors[i] + 3 && !movedCameraMath2[i])
			{
				movedCameraMath2[i] = true;
				ADOBase.controller.MoveCameraToTile(scrLevelMaker.instance.listFloors[math2_floors[i] + 4], scrLevelMaker.instance.listFloors[math2_floors[i] + 3], beats(4f, 170f), Ease.InOutSine);
				scrLevelMaker.instance.listFloors[math2_floors[i] + 3].transform.DOMove(scrLevelMaker.instance.listFloors[math2_floors[i] + 4].transform.position + Vector3.left * 1.5f, beats(4f, 200f)).SetEase(Ease.InOutSine);
			}
		}
	}

	private void Math2Setup()
	{
		EnableStuff(math2_bgstuff);
	}

	private void Math2FadeIn()
	{
		FadeStuff(math2_bgstuff, 1f, beats(8f, 170f));
	}

	private void Math2FadeOut()
	{
		FadeStuff(math2_bgstuff, 0f, beats(8f, 170f));
	}

	private void SpawnNumber()
	{
		Mawaru_Sprite n = math_numbers[curNumberSpawn];
		n.render.enabled = true;
		n.enabled = true;
		n.SetState(UnityEngine.Random.Range(0, 10));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(n.render.material.DOColor(Color.clear, 0f))
			.Append(n.render.material.DOColor(numberColor, 1f * speed).SetEase(Ease.Linear))
			.Append(n.render.material.DOColor(Color.clear, 1f * speed).SetEase(Ease.Linear))
			.OnComplete(delegate
			{
				n.render.enabled = false;
				n.enabled = false;
			});
		n.transform.localScale = Vector3.one * (1f + RandF());
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(n.transform.DOMove(base.redPlanet.transform.position + Vector3.forward * (3f + RandF() * 4f) + Vector3.right * (-15f + RandF() * 30f) + Vector3.up * (-10f + RandF() * 20f), 0f))
			.Append(n.transform.DOMove(Vector3.forward * (-1f + RandF() * 1f) + Vector3.right * (-1f + RandF() * 1f) + Vector3.up * (-1f + RandF() * 1f), 2f * speed).SetEase(Ease.Linear).SetRelative(isRelative: true));
		curNumberSpawn++;
		if (curNumberSpawn >= math_numbers.Count)
		{
			curNumberSpawn = 0;
		}
	}

	private void HardstyleSetup()
	{
		EnableStuff(hardstyle_bgstuff);
		float num = -40f;
		float num2 = 10f;
		float num3 = 8f;
		float num4 = 8f;
		int num5 = 25;
		int num6 = 0;
		int num7 = 0;
		float num8 = 0f;
		for (int i = 0; i < hardstyle_quads.Count; i++)
		{
			float num9 = num + (float)num6 * num3;
			num8 = num2 - (float)num7 * num4;
			hardstyle_quads[i].transform.localPosition = Vector3.right * num9 + Vector3.up * num8;
			if (lowVfx)
			{
				hardstyle_quads[i].transform.localScale = hsQuadScale * 0.65f;
			}
			else
			{
				hardstyle_quads[i].transform.localScale = hsQuadScale;
			}
			num6++;
			if (num6 >= num5)
			{
				num6 = 0;
				num7++;
			}
		}
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

	private float oc(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t = t / d - 1f;
		return c * (Mathf.Pow(t, 5f) + 1f) + b;
	}

	private float ob(float t, float b = 0f, float c = 1f, float d = 1f, float s = 1.3f)
	{
		t = t / d - 1f;
		return c * (t * t * ((s + 1f) * t + s) + 1f) + b;
	}

	private void EndingBeatVibe()
	{
		if (songBeat > 929.5 && songBeat < 954.5)
		{
			for (int i = endingBeatStartFloor; i <= endingBeatEndFloor; i++)
			{
				scrFloor floor = scrLevelMaker.instance.listFloors[i];
				FloorResetPosition(floor);
				FloorBeatVibe(floor, 0.75f);
			}
		}
	}

	private void HardstyleUpdate()
	{
		if (songBeat > 360.5 && songBeat < 389.5 && !slumpo)
		{
			for (int i = hardstyleStartFloor; i <= hardstyleEndFloor; i++)
			{
				scrFloor floor = scrLevelMaker.instance.listFloors[i];
				FloorResetPosition(floor);
				FloorBeatVibe(floor, slumpo ? 1.1f : 0.75f);
			}
		}
		if (songBeat > 361.0 && !slumpo)
		{
			hspingpong = (float)songBeat - Mathf.Floor((float)songBeat);
			hspos = oc(hspingpong);
			if (songBeat % 2.0 < 1.0)
			{
				hspos = 1f - hspos;
			}
			if (songBeat < 389.0)
			{
				for (int j = 0; j < hardstyle_quads.Count; j++)
				{
					hsmult = ((j % 2 == 0) ? 1 : (-1));
					hardstyle_quads[j].transform.localScale = hsQuadScale * 0.65f + Vector3.one * 3f * ((hspos * 2f - 1f) * hsmult);
				}
			}
			else
			{
				base.camy.transform.eulerAngles = Vector3.zero;
			}
			if ((songBeat > 361.0 && songBeat < 375.0) || (songBeat > 379.0 && songBeat < 389.0))
			{
				hardstyle_sky.transform.localScale = Vector3.one * 10f - Vector3.up * 3.5f * (1f - ((float)songBeat - Mathf.Floor((float)songBeat)));
				base.camy.transform.eulerAngles = Vector3.forward * ((float)(slumpo ? 8 : 5) * Mathf.Sin((float)songBeat * (float)Math.PI));
				base.camy.zoomSize = 1f + Mathf.Abs((slumpo ? 0.4f : 0.25f) * Mathf.Sin((float)songBeat * (float)Math.PI));
			}
		}
		if (!(songBeat > 361.0) || !slumpo)
		{
			return;
		}
		hspingpong = (float)songBeat - Mathf.Floor((float)songBeat);
		hspos = oc(hspingpong);
		if (songBeat % 2.0 < 1.0)
		{
			hspos = 1f - hspos;
		}
		if (songBeat < 389.0)
		{
			for (int k = 0; k < hardstyle_quads.Count; k++)
			{
				hsmult = ((k % 2 == 0) ? 1 : (-1));
				hardstyle_quads[k].transform.localScale = hsQuadScale * 0.65f + Vector3.one * 3f * ((hspos * 2f - 1f) * hsmult);
			}
		}
		else
		{
			base.camy.transform.eulerAngles = Vector3.zero;
		}
		if (songBeat > 361.0 && songBeat < 389.0)
		{
			hardstyle_sky.transform.localScale = Vector3.one * 10f - Vector3.up * 3.5f * (1f - ((float)songBeat - Mathf.Floor((float)songBeat)));
			base.camy.transform.eulerAngles = Vector3.forward * (3f * Mathf.Sin((float)songBeat * 0.5f * (float)Math.PI));
			base.camy.zoomSize = 1f + Mathf.Abs(0.2f * Mathf.Cos((float)songBeat * 0.5f * (float)Math.PI));
		}
	}

	private void HardstyleUpdateLow()
	{
		if (!(songBeat > 361.0))
		{
			return;
		}
		hspingpong = (float)songBeat - Mathf.Floor((float)songBeat);
		hspos = oc(hspingpong);
		if (songBeat % 2.0 < 1.0)
		{
			hspos = 1f - hspos;
		}
		if (songBeat < 389.0)
		{
			for (int i = 0; i < hardstyle_quads.Count; i++)
			{
				hsmult = ((i % 2 == 0) ? 1 : (-1));
				hardstyle_quads[i].transform.localScale = hsQuadScale * 0.65f + Vector3.one * 0.5f * ((hspos * 2f - 1f) * hsmult);
			}
		}
	}

	private void HardstyleKickCam()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(DOTween.To(() => base.camy.zoomSize, delegate(float x)
		{
			base.camy.zoomSize = x;
		}, 1.8f, beats(1.5f, 185f)).SetEase(Ease.OutExpo))
			.Append(DOTween.To(() => base.camy.zoomSize, delegate(float x)
			{
				base.camy.zoomSize = x;
			}, 0.8f, 0f))
			.AppendInterval(beats(0.5f, 185f))
			.Append(DOTween.To(() => base.camy.zoomSize, delegate(float x)
			{
				base.camy.zoomSize = x;
			}, 1.2f, 0f))
			.Append(DOTween.To(() => base.camy.zoomSize, delegate(float x)
			{
				base.camy.zoomSize = x;
			}, 1f, beats(2f, 185f)).SetEase(Ease.OutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(2f, 185f))
			.Append(base.camy.transform.DORotate(Vector3.forward * 20f, 0f))
			.AppendInterval(beats(0.5f, 185f))
			.Append(base.camy.transform.DORotate(Vector3.forward * -15f, 0f))
			.AppendInterval(beats(0.5f, 185f))
			.Append(base.camy.transform.DORotate(Vector3.forward * 10f, 0f))
			.Append(base.camy.transform.DORotate(Vector3.forward * 0f, beats(1f, 185f)).SetEase(Ease.OutCubic));
	}

	private void HardstyleEndQuads()
	{
		if (!lowVfx)
		{
			for (int i = 0; i < hardstyle_quads.Count; i++)
			{
				DOTween.Sequence().Append(hardstyle_quads[i].transform.DOScale(hsQuadScale * 1.3f, beats(1.5f, 185f)).SetEase(Ease.OutCubic)).Append(hardstyle_quads[i].transform.DOScale(Vector3.zero, beats(1.5f, 185f)).SetEase(Ease.InCubic));
			}
		}
		else
		{
			for (int j = 0; j < hardstyle_quads.Count; j++)
			{
				DOTween.Sequence().Append(hardstyle_quads[j].transform.DOScale(hsQuadScale, beats(1.5f, 185f)).SetEase(Ease.OutCubic)).Append(hardstyle_quads[j].transform.DOScale(Vector3.zero, beats(1.5f, 185f)).SetEase(Ease.InCubic));
			}
		}
	}

	private void HardstyleFadeIn()
	{
		FadeStuff(hardstyle_bgstuff, 1f, beats(8f, 185f));
	}

	private void HardstyleFadeOut()
	{
		FadeStuff(hardstyle_sky, 0f, beats(8f, 185f));
		for (int i = 0; i < hardstyle_quads.Count; i++)
		{
			hardstyle_quads[i].render.enabled = false;
		}
	}

	private void DriftSetup()
	{
		EnableStuff(drift_bgstuff);
		base.transform.position = Vector3.zero;
		driftSeq = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(9f, 185f))
			.Append(base.transform.DOMoveX(0.5f, beats(2f, 185f)).SetEase(Ease.OutCubic))
			.AppendInterval(beats(9f, 185f))
			.Append(base.transform.DOMoveX(0f, beats(4.5f, 185f)).SetEase(Ease.InOutCubic))
			.AppendInterval(beats(0.4f, 185f))
			.Append(base.transform.DOMoveX(1f, beats(0.2f, 185f)).SetEase(Ease.Linear))
			.Append(base.transform.DOMoveX(0f, beats(2.4f, 185f)).SetEase(Ease.InQuad))
			.AppendInterval(beats(0.4f, 185f))
			.Append(base.transform.DOMoveX(-1.3f, beats(0.2f, 185f)).SetEase(Ease.Linear))
			.Append(base.transform.DOMoveX(0f, beats(2.4f, 185f)).SetEase(Ease.InQuad))
			.AppendInterval(beats(0.4f, 185f))
			.Append(base.transform.DOMoveX(1.6f, beats(0.2f, 185f)).SetEase(Ease.Linear))
			.Append(base.transform.DOMoveX(0f, beats(2.4f, 185f)).SetEase(Ease.InQuad))
			.AppendInterval(beats(0.4f, 185f))
			.Append(base.transform.DOMoveX(-1.6f, beats(0.2f, 185f)).SetEase(Ease.Linear))
			.Append(base.transform.DOMoveX(0f, beats(6.8f, 185f)).SetEase(Ease.InOutCubic));
	}

	private void DriftFadeIn()
	{
		FadeStuff(drift_bgstuff, 1f, beats(8f, 185f));
	}

	private void DriftFadeOut()
	{
		FadeStuff(drift_bgstuff, 0f, beats(8f, 185f));
	}

	private void DriftUpdate()
	{
		for (int i = driftFloorStart; i <= driftFloorEnd; i++)
		{
			scrFloor floor = scrLevelMaker.instance.listFloors[i];
			FloorResetPosition(floor);
			FloorDrunkVibe(floor, thisTransform.position.x, -1f, 8f, 0f, 2f);
		}
		if (ADOBase.controller.currentState == States.Fail)
		{
			driftSeq.Kill();
			base.transform.DOMoveX(0f, beats(2f, 185f)).SetEase(Ease.InOutCubic);
		}
	}

	private void DrunkSetup()
	{
		EnableStuff(drunk_bgstuff);
		driftSeq.Kill();
		base.transform.position = Vector3.zero;
		drunkSeq = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(9f, 185f))
			.Append(base.transform.DOMoveX(0.4f, beats(16f, 185f)).SetEase(Ease.Linear))
			.AppendInterval(beats(10f, 185f))
			.Append(base.transform.DOMoveX(0f, beats(6f, 185f)).SetEase(Ease.InOutCubic));
		drunk_pig.transform.DOMove(new Vector3(8f, 4f, 0f), beats(50f, 185f)).SetRelative(isRelative: true);
		drunk_pig.transform.DORotate(Vector3.forward * 360f * 2f, beats(50f, 185f), RotateMode.FastBeyond360).SetRelative(isRelative: true);
	}

	private void DrunkFadeIn()
	{
		FadeStuff(drunk_bgstuff, 1f, beats(8f, 185f));
	}

	private void DrunkFadeOut()
	{
		FadeStuff(drunk_bgstuff, 0f, beats(8f, 185f));
	}

	private void DrunkUpdate()
	{
		for (int i = drunkFloorStart; i <= drunkFloorEnd; i++)
		{
			scrFloor floor = scrLevelMaker.instance.listFloors[i];
			FloorResetPosition(floor);
			FloorDrunkVibe(floor, thisTransform.position.x, 0f, 8f, 0f, 2f);
			FloorDrunkVibe(floor, thisTransform.position.x, (float)Math.PI / 2f, 8f, (float)Math.PI / 2f, 2f);
		}
		if (ADOBase.controller.currentState == States.Fail)
		{
			drunkSeq.Kill();
			base.transform.DOMoveX(0f, beats(2f, 185f)).SetEase(Ease.InOutCubic);
		}
	}

	private float GetDefendAngle(double time)
	{
		return (float)(3.1415927 * (time - 185.039) / 0.75) - (float)Math.PI / 2f;
	}

	private void DefendColors()
	{
		defend_colors.Clear();
		defend_colors.Add(new Color(1f, 1f, 1f, 1f));
		defend_colors.Add(new Color(1f, 0f, 0f, 1f));
		defend_colors.Add(new Color(0f, 0f, 1f, 1f));
		defend_colors.Add(new Color(0.7f, 0f, 1f, 1f));
		defend_colors.Add(new Color(1f, 1f, 0f, 1f));
		defend_colors.Add(new Color(1f, 0.5f, 0f, 1f));
	}

	private void DefendSetup()
	{
		EnableStuff(defend_bgstuff);
		drunkSeq.Kill();
		base.transform.position = Vector3.zero;
		for (int i = 0; i < defend_times.Count; i++)
		{
			defend_projectiles[i].angle = GetDefendAngle(defend_times[i]);
			float num = defend_projectiles[i].angle / ((float)Math.PI / 2f);
			float num2 = num - Mathf.Floor(num);
			if (Mathf.Abs(num2) < 0.05f || Mathf.Abs(num2 - 1f) < 0.05f)
			{
				defend_projectiles[i].quant = 1;
			}
			else if (Mathf.Abs(num2 - 0.5f) < 0.05f)
			{
				defend_projectiles[i].quant = 2;
			}
			else if (Mathf.Abs(num2 - 0.333f) < 0.05f || Mathf.Abs(num2 - 0.666f) < 0.05f || Mathf.Abs(num2 - 0.833f) < 0.05f || Mathf.Abs(num2 - 0.166f) < 0.05f)
			{
				defend_projectiles[i].quant = 3;
			}
			else if (Mathf.Abs(num2 - 0.25f) < 0.05f || Mathf.Abs(num2 - 0.75f) < 0.05f)
			{
				defend_projectiles[i].quant = 4;
			}
			else
			{
				defend_projectiles[i].quant = 5;
			}
			defend_projectiles[i].beat = num;
			defend_projectiles[i].fract = num2;
			defend_projectiles[i].UpdateFire();
			defend_projectiles[i].floor = scrLevelMaker.instance.listFloors[defend_first_floor + 2 * i];
		}
		defend_crosshair.render.enabled = true;
		defend_target_text.text = RDString.Get("mawaru.target");
		defend_castle.princess.animate = true;
		defend_castle.princess.SetState(0);
		defend_castle.princess.lastFrame = 3;
		defend_castle.won = false;
	}

	private void DefendFadeIn()
	{
		FadeStuff(defend_bgstuff, 1f, beats(8f, 185f));
	}

	private void DefendFadeOut()
	{
		FadeStuff(defend_bgstuff, 0f, beats(8f, 200f));
	}

	private void DefendUpdate()
	{
		defend_castle.UpdateWorry();
		defend_castle.ring.transform.eulerAngles -= Vector3.forward * 10f * Time.deltaTime;
		if (defend_castle.exploded)
		{
			defend_castle.princess.transform.position += (Vector3.up * defend_castle.princessGrav + Vector3.right * 4f) * Time.deltaTime;
			defend_castle.princessGrav -= 18f * Time.deltaTime;
			defend_castle.princess_hat.transform.eulerAngles += Vector3.forward * 300f * Time.deltaTime;
			defend_castle.princess.transform.eulerAngles += Vector3.forward * -600f * Time.deltaTime;
			defend_castle.princess_hat.transform.position += (Vector3.up * defend_castle.hatGrav + Vector3.right * 4f) * Time.deltaTime;
			defend_castle.hatGrav -= 18f * Time.deltaTime;
		}
		else
		{
			int num = defend_times.Count - 1;
			while (num >= 0 && songTime + 2.407 < defend_times[num] && !defend_projectiles[num].hit)
			{
				nextRock = num;
				num--;
			}
			if (nextRock < defend_times.Count)
			{
				defend_crosshair_parent.transform.position = defend_projectiles[nextRock].transform.position;
			}
		}
		if (defend_castle.exploded || defend_castle.won)
		{
			defend_crosshair.render.enabled = false;
			defend_target_text.text = "";
		}
		bool flag = ADOBase.controller.currentState == States.Checkpoint || ADOBase.controller.currentState == States.PlayerControl;
		if (!defend_castle.exploded && !defend_castle.won && curRock < defend_times.Count && songTime + 2.407 > defend_times[curRock] - (double)defend_approachTime && flag)
		{
			r = defend_projectiles[curRock];
			if (curRock == 0)
			{
				r.transform.localPosition = Vector3.up * defend_approachDist * 1f * Mathf.Cos(r.angle) + Vector3.right * defend_approachDist * 1f * Mathf.Sin(r.angle);
			}
			else
			{
				r.transform.localPosition = Vector3.up * defend_approachDist * 0.85f * Mathf.Cos(r.angle) + Vector3.right * defend_approachDist * 0.85f * Mathf.Sin(r.angle);
			}
			float num2 = Vector3.Magnitude(r.transform.localPosition);
			r.transform.localPosition += Vector3.up * 0.34f;
			float tweenTime = defend_approachTime / ((num2 - 2f) / num2);
			r.tweenTime = tweenTime;
			r.spawnTime = (float)defend_times[curRock] - defend_approachTime;
			r.targetTime = (float)defend_times[curRock];
			r.spawnPos = r.transform.localPosition + Vector3.up * 0.34f;
			r.targetPos = Vector3.up * 0.34f;
			if ((double)(r.targetTime - 1.6f) > ADOBase.lm.listFloors[GCS.checkpointNum].entryTime)
			{
				r.rock.render.enabled = true;
				r.spawned = true;
				r.fire.Play();
				r.rock.render.material.SetColor("_Color", whiteClear);
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(r.rock.render.material.DOColor(Color.white, 0.3f * speed));
				r.rock.transform.eulerAngles = Vector3.forward * RandF() * 360f;
			}
			else
			{
				r.hit = true;
			}
			curRock++;
		}
		if (defend_castle.exploded || defend_castle.won)
		{
			return;
		}
		for (int i = 0; i < defend_times.Count; i++)
		{
			r = defend_projectiles[i];
			if (r.spawned && !r.hit && !r.fadingOut && flag)
			{
				r.rock.transform.eulerAngles += Vector3.forward * 180f * Time.deltaTime;
				perc = (songTime + 2.407 - (double)r.spawnTime) / (double)r.tweenTime;
				r.transform.localPosition = Vector3.Lerp(r.spawnPos, r.targetPos, (float)perc);
				if (!r.fadingIn)
				{
					r.rock.render.material.SetColor("_Color", Color.Lerp(Color.white, defend_colors[r.quant], (float)perc));
				}
			}
			if (!r.hit && songTime + 2.407 < (double)(r.spawnTime + 0.3f))
			{
				r.fadingIn = true;
			}
			else
			{
				r.fadingIn = false;
			}
			if (!r.hit && songTime + 2.407 >= (double)(r.spawnTime + r.tweenTime))
			{
				r.fadingOut = true;
			}
			else
			{
				r.fadingOut = false;
			}
			if (!r.hit && songTime + 2.407 >= (double)(r.spawnTime + r.tweenTime) && songTime + 2.407 < (double)(r.spawnTime + r.tweenTime + 0.5f))
			{
				if (!GCS.practiceMode)
				{
					r.rock.render.enabled = false;
					if (!defend_castle.exploded)
					{
						defend_castle.Explode();
						Fail();
					}
					for (int j = 0; j < defend_projectiles.Count; j++)
					{
						if (defend_projectiles[j].spawned)
						{
							defend_projectiles[j].rock.animate = true;
							defend_projectiles[j].explo.render.enabled = true;
							FadeStuff(defend_projectiles[j].explo, 0f, 0.8f);
							defend_projectiles[j].explo.transform.localScale = Vector3.one * 2.4f;
							defend_projectiles[j].explo.transform.DOScale(Vector3.one * 1.5f, 0.8f).SetEase(Ease.OutCubic);
							defend_projectiles[j].fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
						}
					}
				}
				else
				{
					r.fadingOut = true;
					FadeStuff(r.rock, 0f, 0.3f * speed);
					r.fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				}
			}
			if (defend_projectiles[i].floor.grade != HitMargin.TooEarly && defend_projectiles[i].floor.grade != HitMargin.VeryEarly && defend_projectiles[i].floor.grade != HitMargin.VeryLate && defend_projectiles[i].floor.grade != HitMargin.TooLate && !defend_projectiles[i].hit)
			{
				defend_projectiles[i].hit = true;
				defend_projectiles[i].rock.animate = true;
				defend_projectiles[i].fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				defend_projectiles[i].explo.render.enabled = true;
				FadeStuff(defend_projectiles[i].explo, 0f, 0.8f);
				defend_projectiles[i].explo.transform.localScale = Vector3.one * 2.4f;
				defend_projectiles[i].explo.transform.DOScale(Vector3.one * 1.5f, 0.8f).SetEase(Ease.OutCubic);
				if (i == defend_times.Count - 1 && !defend_castle.won)
				{
					defend_castle.won = true;
					defend_castle.princess.animate = false;
					defend_castle.princess.SetState(4);
					defend_castle.princess.transform.localScale = Vector3.one * 1.9f;
					defend_castle.princess.transform.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.OutCubic);
					defend_castle.heart.render.enabled = true;
					defend_castle.heart.render.material.SetColor("_Color", Color.white);
					defend_castle.heart.transform.DOMoveY(0.8f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.OutCubic);
					FadeStuff(defend_castle.heart, 0f, 0.5f);
				}
			}
		}
	}

	private void FogSetup()
	{
		EnableStuff(fog_bgstuff);
		EnableStuff(fog_overlay);
		EnableStuff(fogp1);
		EnableStuff(fogp2);
	}

	private void FogFadeIn()
	{
		FadeStuff(fog_bgstuff, 1f, beats(8f, 200f));
	}

	private void FogBegin()
	{
		base.transform.position = Vector3.zero;
		if (lowVfx)
		{
			if (slumpo)
			{
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.transform.DOMoveX(0.8f, beats(4f, 200f)).SetEase(Ease.OutCubic))
					.AppendInterval(beats(4f, 200f))
					.Append(base.transform.DOMoveX(1.3f, beats(6f, 200f)).SetEase(Ease.InOutCubic))
					.Append(base.transform.DOMoveX(0.5f, beats(2f, 200f)).SetEase(Ease.InOutCubic))
					.Append(base.transform.DOMoveX(1.3f, beats(2f, 200f)).SetEase(Ease.InOutCubic))
					.Append(base.transform.DOMoveX(1.5f, beats(8f, 200f)).SetEase(Ease.InOutCubic));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(12f, 200f))
					.Append(fog_overlay.render.material.DOColor(Color.white, beats(8f, 200f)));
			}
			else
			{
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.transform.DOMoveX(0.6f, beats(4f, 200f)).SetEase(Ease.OutCubic))
					.AppendInterval(beats(4f, 200f))
					.Append(base.transform.DOMoveX(1.1f, beats(5f, 200f)).SetEase(Ease.InOutCubic))
					.Append(base.transform.DOMoveX(0.4f, beats(3f, 200f)).SetEase(Ease.InOutCubic))
					.Append(base.transform.DOMoveX(1.1f, beats(3f, 200f)).SetEase(Ease.InOutCubic))
					.Append(base.transform.DOMoveX(1.2f, beats(8f, 200f)).SetEase(Ease.InOutCubic));
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(12f, 200f))
					.Append(fog_overlay.render.material.DOColor(Color.white, beats(8f, 200f)));
			}
		}
		else if (slumpo)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.transform.DOMoveX(0.8f, beats(4f, 200f)).SetEase(Ease.OutCubic))
				.AppendInterval(beats(4f, 200f))
				.Append(base.transform.DOMoveX(1.3f, beats(6f, 200f)).SetEase(Ease.InOutCubic))
				.Append(base.transform.DOMoveX(0.8f, beats(2f, 200f)).SetEase(Ease.InOutCubic))
				.Append(base.transform.DOMoveX(1.3f, beats(2f, 200f)).SetEase(Ease.InOutCubic))
				.Append(base.transform.DOMoveX(1.5f, beats(8f, 200f)).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(12f, 200f))
				.Append(fog_overlay.render.material.DOColor(Color.white, beats(8f, 200f)));
		}
		else
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.transform.DOMoveX(0.6f, beats(4f, 200f)).SetEase(Ease.OutCubic))
				.AppendInterval(beats(4f, 200f))
				.Append(base.transform.DOMoveX(1.1f, beats(5f, 200f)).SetEase(Ease.InOutCubic))
				.Append(base.transform.DOMoveX(0.6f, beats(3f, 200f)).SetEase(Ease.InOutCubic))
				.Append(base.transform.DOMoveX(1.1f, beats(3f, 200f)).SetEase(Ease.InOutCubic))
				.Append(base.transform.DOMoveX(1.2f, beats(8f, 200f)).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(12f, 200f))
				.Append(fog_overlay.render.material.DOColor(Color.white, beats(8f, 200f)));
		}
		fogBPM.transform.DOMoveZ(4f, beats(2f, 200f)).SetRelative(isRelative: true).SetEase(Ease.OutSine);
	}

	private void FogFadeOut()
	{
		FadeStuff(fog_bgstuff, 0f, beats(8f, 200f));
		FadeStuff(fog_overlay, 0f, beats(8f, 200f));
		FadeStuff(fogp1, 0f, beats(8f, 200f));
		FadeStuff(fogp2, 0f, beats(8f, 200f));
	}

	private void FogBPMMoveBack()
	{
		fogBPM.transform.DOMoveZ(-4f, beats(2f, 200f)).SetRelative(isRelative: true).SetEase(Ease.InSine);
	}

	private void FogUpdate()
	{
		for (int i = 0; i < fogp1.Count; i++)
		{
			float num = Time.time * 6.28f + (float)i / (float)fogp1.Count * 2f * (float)Math.PI;
			fogAdd1 = Vector3.right * Mathf.Sin(num) + Vector3.up * Mathf.Cos(num);
			fogAdd2 = Vector3.right * Mathf.Sin(num + 3.14f) + Vector3.up * Mathf.Cos(num + 3.14f);
			fogp1[i].transform.position = base.redPlanet.transform.position + fogAdd1 * (1f + base.transform.position.x * 0.3f);
			fogp2[i].transform.position = base.bluePlanet.transform.position + fogAdd2 * (1f + base.transform.position.x * 0.3f);
			fogp1[i].transform.localScale = Vector3.one * (1f + base.transform.position.x * 0.6f);
			fogp2[i].transform.localScale = Vector3.one * (1f + base.transform.position.x * 0.6f);
			if (songBeat < 616.0)
			{
				States currentState = ADOBase.controller.currentState;
				if (currentState == States.PlayerControl || currentState == States.Checkpoint)
				{
					fogp1[i].render.material.SetColor("_Color", whiteClear + Color.black * base.transform.position.x);
					fogp2[i].render.material.SetColor("_Color", whiteClear + Color.black * base.transform.position.x);
				}
				else if (!fogDied)
				{
					FadeStuff(fogp1, 0f, beats(4f, 200f));
					FadeStuff(fogp2, 0f, beats(4f, 200f));
					fogDied = true;
				}
			}
		}
	}

	private void Fog2Setup()
	{
		EnableStuff(fogp1);
		EnableStuff(fogp2);
	}

	private void Fog2Begin()
	{
		base.transform.position = Vector3.zero;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.transform.DOMoveX(0.6f, beats(4f, 200f)).SetEase(Ease.OutCubic))
			.AppendInterval(beats(4f, 200f))
			.Append(base.transform.DOMoveX(1.2f, beats(4f, 200f)).SetEase(Ease.InOutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(fog_overlay.render.material.DOColor(Color.white, beats(8f, 200f)));
	}

	private void Fog2FadeOut()
	{
		FadeStuff(fogp1, 0f, beats(4f, 200f));
		FadeStuff(fogp2, 0f, beats(4f, 200f));
	}

	private void Fog2Update()
	{
		for (int i = 0; i < fogp1.Count; i++)
		{
			float num = Time.time * 6.28f + (float)i / (float)fogp1.Count * 2f * (float)Math.PI;
			fogAdd1 = Vector3.right * Mathf.Sin(num) + Vector3.up * Mathf.Cos(num);
			fogAdd2 = Vector3.right * Mathf.Sin(num + 3.14f) + Vector3.up * Mathf.Cos(num + 3.14f);
			fogp1[i].transform.position = base.redPlanet.transform.position + fogAdd1 * (1f + base.transform.position.x * 0.3f);
			fogp2[i].transform.position = base.bluePlanet.transform.position + fogAdd2 * (1f + base.transform.position.x * 0.3f);
			fogp1[i].transform.localScale = Vector3.one * (1f + base.transform.position.x * 0.6f);
			fogp2[i].transform.localScale = Vector3.one * (1f + base.transform.position.x * 0.6f);
			if (songBeat < 989.0)
			{
				States currentState = ADOBase.controller.currentState;
				if (currentState == States.PlayerControl || currentState == States.Checkpoint)
				{
					fogp1[i].render.material.SetColor("_Color", whiteClear + Color.black * base.transform.position.x);
					fogp2[i].render.material.SetColor("_Color", whiteClear + Color.black * base.transform.position.x);
				}
				else if (!fog2Died)
				{
					FadeStuff(fogp1, 0f, beats(4f, 200f));
					FadeStuff(fogp2, 0f, beats(4f, 200f));
					fog2Died = true;
				}
			}
		}
	}

	private void PsytranceSetup()
	{
		base.transform.position = Vector3.zero;
		EnableStuff(psytrance_bgstuff);
		foreach (Mawaru_Sprite item in psytrance_lightmass)
		{
			item.transform.position += Vector3.forward * (-2f + RandF() * 4f);
		}
	}

	private void PsytranceFadeIn()
	{
		FadeStuff(psytrance_bgstuff, 1f, beats(8f, 200f));
	}

	private void PsytranceFadeOut()
	{
		FadeStuff(psytrance_bgstuff, 0f, beats(8f, 200f));
	}

	private void PsytranceUpdate()
	{
		if (songBeat > 632.5 && songBeat < 664.5 && !lowVfx)
		{
			for (int i = psytranceStartFloor; i <= psytranceEndFloor; i++)
			{
				scrFloor floor = scrLevelMaker.instance.listFloors[i];
				FloorResetPosition(floor);
				FloorBeatVibe(floor, 0.25f);
			}
		}
	}

	private void NishiSetup()
	{
		if (!slumpo)
		{
			for (int i = 0; i < 4; i++)
			{
				goatFloorIcons[i].transform.position = ADOBase.lm.listFloors[goatFloors[i]].transform.position;
			}
		}
		else
		{
			for (int j = 0; j < 4; j++)
			{
				goatFloorIcons[j].transform.position = ADOBase.lm.listFloors[goatFloors[j]].transform.position;
			}
		}
		EnableStuff(nishizabu_bgstuff);
		foreach (Mawaru_Goat nishizabu_goat in nishizabu_goats)
		{
			nishizabu_goat.Enable();
		}
	}

	private void NishiFadeIn()
	{
		FadeStuff(nishizabu_bgstuff, 1f, beats(8f, 215f));
	}

	private void NishiFadeOut()
	{
		nishifading = true;
		FadeStuff(nishizabu_bgstuff, 0f, beats(8f, 215f));
		FadeStuff(nishizabu_goat_stuff, 0f, beats(8f, 215f));
	}

	private void NishiFadeOutQ()
	{
		if (nishifading)
		{
			return;
		}
		foreach (Mawaru_Goat nishizabu_goat in nishizabu_goats)
		{
			nishizabu_goat.gameObject.SetActive(value: false);
		}
	}

	private void NishiGoat1()
	{
		if (slumpo)
		{
			NishiDoGoat(0, 6f);
		}
		else
		{
			NishiDoGoat(0, 4f);
		}
	}

	private void NishiGoat2()
	{
		if (slumpo)
		{
			NishiDoGoat(1, 4f);
		}
		else
		{
			NishiDoGoat(1, 6f);
		}
	}

	private void NishiGoat3()
	{
		if (slumpo)
		{
			NishiDoGoat(2, 2f);
		}
		else
		{
			NishiDoGoat(2, 5f);
		}
	}

	private void NishiGoat4()
	{
		if (slumpo)
		{
			NishiDoGoat(3, 10f);
		}
		else
		{
			NishiDoGoat(3, 5f);
		}
	}

	private void NishiDoGoat(int which, float b)
	{
		if (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		int w;
		for (w = 0; w < 4; w++)
		{
			if (w == 3)
			{
				goatViewAngle = Vector3.forward * -50f + Vector3.up * 180f;
			}
			else
			{
				goatViewAngle = Vector3.forward * -10f + Vector3.up * 180f;
			}
			nishizabu_goats[w].goatEyeLight.render.enabled = true;
			Sequence value = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(nishizabu_goats[w].transform.DORotate(goatViewAngle, beats(0.5f, 215f)))
				.AppendInterval(beats(b + 0.5f, 215f))
				.Append(nishizabu_goats[w].transform.DORotate(Vector3.zero, beats(0.5f, 215f)));
			Sequence value2 = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(0.5f, 215f))
				.Append(nishizabu_goats[w].goatEyeLight.transform.DOScaleY(1f, beats(1f, 215f)).SetEase(Ease.OutBack))
				.AppendInterval(beats(b - 1f, 215f))
				.Append(nishizabu_goats[w].goatEyeLight.transform.DOScaleY(0f, beats(1f, 215f)).SetEase(Ease.InBack))
				.OnComplete(delegate
				{
					nishizabu_goats[w].goatEyeLight.render.enabled = false;
				});
			goatBodyTweens[w] = value;
			goatLightTweens[w] = value2;
		}
	}

	private void NishiBar1()
	{
		if (slumpo)
		{
			NishiDoBar(0, 6f, 4f);
		}
		else
		{
			NishiDoBar(0, 4f, 4f);
		}
	}

	private void NishiBar2()
	{
		if (slumpo)
		{
			NishiDoBar(1, 4f, 4f, 4f);
		}
		else
		{
			NishiDoBar(1, 6f, 6f);
		}
	}

	private void NishiBar3()
	{
		if (slumpo)
		{
			NishiDoBar(2, 2f, 4f, 4f);
		}
		else
		{
			NishiDoBar(2, 5f, 5f);
		}
	}

	private void NishiBar4()
	{
		if (slumpo)
		{
			NishiDoBar(3, 10f, 10f);
		}
		else
		{
			NishiDoBar(3, 5f, 5f);
		}
	}

	private void NishiDoBar(int which, float b, float bs, float force = -1f)
	{
		float numBeats = b / bs;
		if (force > 0f)
		{
			bs = force;
		}
		nishizabu_numbers[which].render.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(b, 215f))
			.Append(nishizabu_numbers[which].transform.DOMoveZ(0f, 0f).SetRelative(isRelative: true).OnComplete(delegate
			{
				nishizabu_numbers[which].render.enabled = false;
			}));
		nishizabu_numbers[which].SetState((int)bs);
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 1f, 0f));
				if ((int)Mathf.Max(bs - 1f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 2f, 0f));
				if ((int)Mathf.Max(bs - 2f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 3f, 0f));
				if ((int)Mathf.Max(bs - 3f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 4f, 0f));
				if ((int)Mathf.Max(bs - 4f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 5f, 0f));
				if ((int)Mathf.Max(bs - 5f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 6f, 0f));
				if ((int)Mathf.Max(bs - 6f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 7f, 0f));
				if ((int)Mathf.Max(bs - 7f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 8f, 0f));
				if ((int)Mathf.Max(bs - 8f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 9f, 0f));
				if ((int)Mathf.Max(bs - 9f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one, 0f))
			.Append(nishizabu_numbers[which].transform.DOScale(Vector3.one * 0.9f, beats(numBeats, 215f)).SetEase(Ease.OutCubic).OnComplete(delegate
			{
				nishizabu_numbers[which].SetState((int)Mathf.Max(bs - 10f, 0f));
				if ((int)Mathf.Max(bs - 10f, 0f) == 0)
				{
					nishizabu_numbers[which].render.enabled = false;
				}
			}));
		if (which == 1 && slumpo)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(3f, 215f))
				.Append(nishizabu_numbers[which].render.DOColor(whiteClear, 0f).SetEase(Ease.Linear));
		}
		if (which == 2 && !slumpo)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(1f, 215f))
				.Append(nishizabu_numbers[which].render.DOColor(whiteClear, beats(4f, 215f)).SetEase(Ease.Linear));
		}
		if (which == 2 && slumpo)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(1f, 215f))
				.Append(nishizabu_numbers[which].render.DOColor(whiteClear, 0f).SetEase(Ease.Linear));
		}
		if (which == 3)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(beats(0f, 215f))
				.Append(nishizabu_numbers[which].render.DOColor(whiteClear, beats(3f, 215f)).SetEase(Ease.Linear));
		}
	}

	private void NishiGoatUpdate()
	{
		hiding = false;
		for (int i = 0; i < 4; i++)
		{
			if (nishizabu_numbers[i].render.enabled)
			{
				hiding = true;
			}
			if (!nishizabu_goats[i].fired)
			{
				nishizabu_goats[i].transform.position = nishizabu_goats[i].initpos + Vector3.up * 0.3f * Mathf.Sin(Time.time * 215f / 60f * (float)Math.PI * 0.25f + (float)i * 0.5f * (float)Math.PI);
			}
		}
		if (!hiding)
		{
			gearBeat += Time.deltaTime * 3.5833333f / speed;
		}
		float num = 20f * (Mathf.Floor(gearBeat) + ob(gearBeat - Mathf.Floor(gearBeat))) % 360f;
		foreach (Mawaru_Sprite item in nishizabu_gear)
		{
			item.transform.eulerAngles = Vector3.forward * ((item.miscf > 0f) ? (0f - num) : num);
		}
		for (int j = 0; j < 4; j++)
		{
			if (ADOBase.controller.currFloor.seqID == goatFloors[j] + 1 && !goatFloorHidden[j])
			{
				int j2 = j;
				goatFloorHidden[j] = true;
				DOTween.To(() => ADOBase.lm.listFloors[goatFloors[j2]].holdOpacity, delegate(float x)
				{
					ADOBase.lm.listFloors[goatFloors[j2]].holdOpacity = x;
				}, 0f, beats(1f, 215f)).SetEase(Ease.Linear);
			}
		}
		if (ADOBase.controller.currentState != States.Fail)
		{
			return;
		}
		for (int num2 = 0; num2 < 4; num2++)
		{
			if (ADOBase.controller.currFloor.seqID != goatFloors[num2] || nishizabu_goats[num2].fired)
			{
				continue;
			}
			scrSfx.instance.PlaySfx(SfxSound.MawaruLaser, MixerGroup.SfxParent, 0.5f);
			nishizabu_goats[num2].Fire();
			int k = num2;
			if (goatBodyTweens.TryGetValue(num2, out var value))
			{
				value.Kill();
			}
			if (goatLightTweens.TryGetValue(num2, out var value2))
			{
				value2.Kill();
			}
			goatViewAngle = Vector3.forward * -10f + Vector3.up * 180f;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(nishizabu_goats[k].transform.DORotate(goatViewAngle, beats(0.5f, 215f)));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(nishizabu_goats[k].goatEyeLight.transform.DOScaleY(0f, beats(1f, 215f)).SetEase(Ease.InBack))
				.OnComplete(delegate
				{
					nishizabu_goats[k].goatEyeLight.render.enabled = false;
				});
			goatFloorIcons[num2].enabled = false;
			for (int num3 = 0; num3 < 3; num3++)
			{
				scrFloor scrFloor2 = ADOBase.lm.listFloors[goatFloors[num2] - 1 + num3];
				if (scrFloor2 != null && scrFloor2.isLandable)
				{
					float f = (float)Math.PI / 180f * nishizabu_goats[num2].transform.eulerAngles.z;
					DOTween.Kill(scrFloor2);
					scrFloor2.isLandable = false;
					scrFloor2.TweenOpacity(0f, (float)ADOBase.conductor.crotchetAtStart / ADOBase.conductor.song.pitch);
					scatter = Vector3.right * 2f * Mathf.Sin(f) + Vector3.down * 2f * Mathf.Cos(f) + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f);
					scrFloor2.transform.DOLocalMove(scatter, (float)ADOBase.conductor.crotchetAtStart / ADOBase.conductor.song.pitch).SetEase(Ease.OutCubic).SetRelative(isRelative: true);
					scrFloor2.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), (float)ADOBase.conductor.crotchetAtStart / ADOBase.conductor.song.pitch, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
				}
			}
		}
	}

	private void VictoryLapSetup()
	{
		myCam.cullingMask = funkyMask;
	}

	private void VictoryLapEnd()
	{
		myCam.cullingMask = defaultMask;
		foreach (scrPlanetCopyCam cloneCam in cloneCams)
		{
			cloneCam.Disable();
		}
	}

	private void EnableCams(int num)
	{
		foreach (scrPlanetCopyCam cloneCam in cloneCams)
		{
			cloneCam.Disable();
		}
		for (int i = 0; i < num; i++)
		{
			cloneCams[i].Enable();
		}
	}

	private void Cams2()
	{
		EnableCams(2);
	}

	private void Cams4()
	{
		EnableCams(4);
	}

	private void Cams6()
	{
		EnableCams(6);
	}

	private void VictoryLapUpdate()
	{
		cang = ((float)songBeat - 850f) * 0.25f * (float)Math.PI;
		cang2 = ((float)songBeat - 850f) * 0.5f * (float)Math.PI;
		camp = Mathf.Sin(cang);
		if (songBeat >= 850.0 && songBeat < 854.0)
		{
			for (int i = 0; i < 4; i++)
			{
				float f = (float)songBeat * 3.14159f * 0.5f + (float)i / 4f * 2f * 3.14159f;
				cloneCams[i].position = camp * 3f * new Vector2(Mathf.Sin(f), Mathf.Cos(f));
			}
		}
		if (songBeat >= 854.0 && songBeat < 858.0)
		{
			for (int j = 0; j < 2; j++)
			{
				cloneCams[j].position = camp * 3f * new Vector2(0f, (j % 2 != 0) ? 1 : (-1));
			}
		}
		if (songBeat >= 858.0 && songBeat < 862.0)
		{
			for (int k = 0; k < 4; k++)
			{
				cloneCams[k].position = camp * 3f * new Vector2((float)k - 1.5f, 0f);
			}
		}
		if (songBeat >= 862.0 && songBeat < 866.0)
		{
			for (int l = 0; l < 4; l++)
			{
				float f2 = (float)songBeat * -3.14159f * 0.5f + (float)l / 4f * 2f * 3.14159f;
				cloneCams[l].position = camp * 3f * new Vector2(Mathf.Sin(f2), Mathf.Cos(f2));
			}
		}
		if (songBeat >= 866.0 && songBeat < 870.0)
		{
			for (int m = 0; m < 2; m++)
			{
				cloneCams[m].position = camp * 3f * new Vector2((m % 2 != 0) ? 1 : (-1), (m % 2 == 0) ? 1 : (-1));
				cloneCams[m].rotationOffset = 30f * camp;
			}
		}
		if (songBeat >= 870.0 && songBeat < 874.0)
		{
			for (int n = 0; n < 4; n++)
			{
				cloneCams[n].position = camp * 3f * new Vector2((n % 2 != 0) ? 1 : (-1), (n != 1 && n != 2) ? 1 : (-1));
				if (slumpo)
				{
					cloneCams[n].rotationOffset = 360f * oc((float)songBeat - 870f, 0f, 1f, 4f);
				}
				else
				{
					cloneCams[n].rotationOffset = 720f * oc((float)songBeat - 870f, 0f, 1f, 4f);
				}
			}
		}
		if (songBeat >= 874.0 && songBeat < 878.0)
		{
			float num = 10f * ioc((float)(songBeat - 874.0), 0f, 1f, 4f);
			for (int num2 = 0; num2 < 2; num2++)
			{
				cloneCams[num2].position = new Vector2(0f, (float)(-10 * num2) + num);
				cloneCams[num2].rotationOffset = -20f * camp;
			}
		}
		if (songBeat >= 878.0 && songBeat < 882.0)
		{
			for (int num3 = 0; num3 < 4; num3++)
			{
				float f3 = (float)songBeat * 3.14159f * 0.5f + (float)num3 / 6f * 2f * 3.14159f;
				cloneCams[num3].position = camp * 3f * new Vector2(Mathf.Sin(f3), Mathf.Cos(f3));
				cloneCams[num3].rotationOffset = 25f * camp * (float)((num3 % 2 == 0) ? 1 : (-1));
			}
		}
	}

	private void VictoryLapUpdateLow()
	{
		cang = ((float)songBeat - 850f) * 0.25f * (float)Math.PI;
		cang2 = ((float)songBeat - 850f) * 0.5f * (float)Math.PI;
		camp = Mathf.Sin(cang);
		if (songBeat >= 850.0 && songBeat < 882.0)
		{
			for (int i = 0; i < 2; i++)
			{
				cloneCams[i].position = camp * 1.5f * new Vector2(0f, (i % 2 != 0) ? 1 : (-1));
			}
		}
	}

	private void SpawnBootlegFloors1()
	{
		SpawnBootlegFloors(0);
	}

	private void SpawnBootlegFloors2()
	{
		SpawnBootlegFloors(1);
	}

	private void SpawnBootlegFloors3()
	{
		SpawnBootlegFloors(2);
	}

	private void SpawnBootlegFloors4()
	{
		SpawnBootlegFloors(3);
	}

	private void SpawnBootlegFloors(int which)
	{
		if (curBootlegFloor >= 4)
		{
			return;
		}
		GameObject gameObject = bootleg_squares[curBootlegFloor];
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(gameObject.transform.DOScale(Vector3.one * 0.8f, 0f))
			.Append(gameObject.transform.DOScale(Vector3.one * 1.05f, beats(2f, 200f)).SetEase(Ease.InQuad))
			.AppendInterval(0.05f * speed)
			.Append(gameObject.transform.DOScale(Vector3.zero, 0f));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(gameObject.transform.DOMove(charlie.transform.position + Vector3.up * 2f, 0f))
			.Append(gameObject.transform.DOMove(charlie.waypoints[11].position + Vector3.left * 3f * curBootlegFloor, beats(2f, 200f)).SetEase(Ease.Linear).OnComplete(delegate
			{
				for (int i = 0; i < bootleg_stuff[which].Item2; i++)
				{
					scrLevelMaker.instance.listFloors[bootleg_stuff[which].Item1 + i].TweenOpacity(1f, 0f);
				}
			}));
		gameObject.transform.DORotate(Vector3.forward * 720f, beats(2f, 200f), RotateMode.FastBeyond360).SetEase(Ease.Linear);
		curBootlegFloor++;
	}

	private float GetDefend2Angle(double time)
	{
		if (slumpo)
		{
			starttime = 328.8;
		}
		return (float)(-3.1415927 * (time - starttime) / 0.631578947368421) - (float)Math.PI / 2f;
	}

	private void Defend2Setup()
	{
		for (int i = 0; i < defend2_times.Count; i++)
		{
			defend2_projectiles[i].angle = GetDefend2Angle(defend2_times[i]);
			float num = defend2_projectiles[i].angle / ((float)Math.PI / 2f);
			float num2 = num - Mathf.Floor(num);
			if (Mathf.Abs(num2) < 0.05f || Mathf.Abs(num2 - 1f) < 0.05f)
			{
				defend2_projectiles[i].quant = 1;
			}
			else if (Mathf.Abs(num2 - 0.5f) < 0.05f)
			{
				defend2_projectiles[i].quant = 2;
			}
			else if (Mathf.Abs(num2 - 0.333f) < 0.05f || Mathf.Abs(num2 - 0.666f) < 0.05f || Mathf.Abs(num2 - 0.833f) < 0.05f || Mathf.Abs(num2 - 0.166f) < 0.05f)
			{
				defend2_projectiles[i].quant = 3;
			}
			else if (Mathf.Abs(num2 - 0.25f) < 0.05f || Mathf.Abs(num2 - 0.75f) < 0.05f)
			{
				defend2_projectiles[i].quant = 4;
			}
			else
			{
				defend2_projectiles[i].quant = 5;
			}
			defend2_projectiles[i].beat = num;
			defend2_projectiles[i].fract = num2;
			defend2_projectiles[i].UpdateFire();
			defend2_projectiles[i].floor = scrLevelMaker.instance.listFloors[defend2_first_floor + 2 * i];
		}
		defend2_castle.princess.animate = true;
		defend2_castle.princess.SetState(0);
		defend2_castle.princess.lastFrame = 3;
		defend2_target_text.text = RDString.Get("mawaru.target");
		defend2_castle.won = false;
	}

	private void Defend2RingOn()
	{
		defend2_castle.ring.render.enabled = true;
		defend2_castle.ring.transform.DOScale(Vector3.one * 2.01f, beats(1f, 190f)).SetEase(Ease.OutSine);
	}

	private void Defend2RingOff()
	{
		FadeStuff(defend2_castle.ring, 0f, beats(2f, 190f));
	}

	private void Defend2Update()
	{
		defend2_castle.UpdateWorry();
		defend2_castle.ring.transform.eulerAngles -= Vector3.forward * 10f * Time.deltaTime;
		if (defend2_castle.exploded)
		{
			defend2_castle.princess.transform.position += (Vector3.up * defend2_castle.princessGrav + Vector3.right * 4f) * Time.deltaTime;
			defend2_castle.princessGrav -= 18f * Time.deltaTime;
			defend2_castle.princess_hat.transform.eulerAngles += Vector3.forward * 300f * Time.deltaTime;
			defend2_castle.princess.transform.eulerAngles += Vector3.forward * -600f * Time.deltaTime;
			defend2_castle.princess_hat.transform.position += (Vector3.up * defend2_castle.hatGrav + Vector3.right * 4f) * Time.deltaTime;
			defend2_castle.hatGrav -= 18f * Time.deltaTime;
		}
		else
		{
			int num = defend2_times.Count - 1;
			while (num >= 0 && songTime + 2.407 < defend2_times[num] && !defend2_projectiles[num].hit)
			{
				nextRock2 = num;
				num--;
			}
			if (nextRock2 < defend2_times.Count)
			{
				defend2_crosshair_parent.transform.position = defend2_projectiles[nextRock2].transform.position;
			}
		}
		if (defend2_castle.exploded || defend2_castle.won)
		{
			defend2_crosshair.render.enabled = false;
			defend2_target_text.text = "";
		}
		bool flag = ADOBase.controller.currentState == States.Checkpoint || ADOBase.controller.currentState == States.PlayerControl;
		if (!defend2_castle.exploded && !defend2_castle.won && curRock2 < defend2_times.Count && songTime + 2.407 > defend2_times[curRock2] - (double)defend2_approachTime && flag)
		{
			r = defend2_projectiles[curRock2];
			r.transform.localPosition = Vector3.up * defend2_approachDist * 0.85f * Mathf.Cos(r.angle) + Vector3.right * defend2_approachDist * 0.85f * Mathf.Sin(r.angle);
			float num2 = Vector3.Magnitude(r.transform.localPosition);
			r.transform.localPosition += Vector3.up * 0.34f;
			float tweenTime = defend2_approachTime / ((num2 - 2f) / num2);
			r.tweenTime = tweenTime;
			r.spawnTime = (float)defend2_times[curRock2] - defend2_approachTime;
			r.targetTime = (float)defend2_times[curRock2];
			r.spawnPos = r.transform.localPosition + Vector3.up * 0.34f;
			r.targetPos = Vector3.up * 0.34f;
			if ((double)(r.targetTime - 1.6f) > ADOBase.lm.listFloors[GCS.checkpointNum].entryTime)
			{
				r.rock.render.enabled = true;
				r.spawned = true;
				r.fire.Play();
				r.rock.render.material.SetColor("_Color", whiteClear);
				DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(r.rock.render.material.DOColor(Color.white, 0.3f * speed));
				r.rock.transform.eulerAngles = Vector3.forward * RandF() * 360f;
			}
			else
			{
				r.hit = true;
			}
			curRock2++;
		}
		if (defend2_castle.exploded || defend2_castle.won)
		{
			return;
		}
		for (int i = 0; i < defend2_times.Count; i++)
		{
			r = defend2_projectiles[i];
			if (r.spawned && !r.hit && !r.fadingOut && flag)
			{
				r.rock.transform.eulerAngles += Vector3.forward * 180f * Time.deltaTime;
				perc = (songTime + 2.407 - (double)r.spawnTime) / (double)r.tweenTime;
				r.transform.localPosition = Vector3.Lerp(r.spawnPos, r.targetPos, (float)perc);
				if (!r.fadingIn)
				{
					r.rock.render.material.SetColor("_Color", Color.Lerp(Color.white, defend_colors[r.quant], (float)perc));
				}
			}
			if (!r.hit && songTime + 2.407 < (double)(r.spawnTime + 0.3f))
			{
				r.fadingIn = true;
			}
			else
			{
				r.fadingIn = false;
			}
			if (!r.hit && songTime + 2.407 >= (double)(r.spawnTime + r.tweenTime))
			{
				r.fadingOut = true;
			}
			else
			{
				r.fadingOut = false;
			}
			if (!r.hit && songTime + 2.407 >= (double)(r.spawnTime + r.tweenTime) && songTime + 2.407 < (double)(r.spawnTime + r.tweenTime + 0.5f))
			{
				if (!GCS.practiceMode)
				{
					r.rock.render.enabled = false;
					if (!defend2_castle.exploded)
					{
						defend2_castle.Explode();
						Fail();
					}
					for (int j = 0; j < defend2_projectiles.Count; j++)
					{
						if (defend2_projectiles[j].spawned)
						{
							defend2_projectiles[j].rock.animate = true;
							defend2_projectiles[j].explo.render.enabled = true;
							FadeStuff(defend2_projectiles[j].explo, 0f, 0.8f);
							defend2_projectiles[j].explo.transform.localScale = Vector3.one * 2.4f;
							defend2_projectiles[j].explo.transform.DOScale(Vector3.one * 1.5f, 0.8f).SetEase(Ease.OutCubic);
							defend2_projectiles[j].fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
						}
					}
				}
				else
				{
					r.fadingOut = true;
					FadeStuff(r.rock, 0f, 0.3f * speed);
					r.fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				}
			}
			if (defend2_projectiles[i].floor.grade != HitMargin.TooEarly && defend2_projectiles[i].floor.grade != HitMargin.VeryEarly && defend2_projectiles[i].floor.grade != HitMargin.VeryLate && defend2_projectiles[i].floor.grade != HitMargin.TooLate && !defend2_projectiles[i].hit)
			{
				defend2_projectiles[i].hit = true;
				defend2_projectiles[i].rock.animate = true;
				defend2_projectiles[i].fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				defend2_projectiles[i].explo.render.enabled = true;
				FadeStuff(defend2_projectiles[i].explo, 0f, 0.8f);
				defend2_projectiles[i].explo.transform.localScale = Vector3.one * 2.4f;
				defend2_projectiles[i].explo.transform.DOScale(Vector3.one * 1.5f, 0.8f).SetEase(Ease.OutCubic);
				if (i == defend2_times.Count - 1 && !defend2_castle.won)
				{
					defend2_castle.won = true;
					defend2_castle.princess.animate = false;
					defend2_castle.princess.SetState(4);
					defend2_castle.princess.transform.localScale = Vector3.one * 1.9f;
					defend2_castle.princess.transform.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.OutCubic);
					defend2_castle.speech.text = RDString.Get("mawaru.youGotThis");
					defend2_castle.speech.DOColor(Color.white, beats(1f));
					defend2_castle.heart.render.enabled = true;
					defend2_castle.heart.render.material.SetColor("_Color", Color.white);
					defend2_castle.heart.transform.DOMoveY(0.8f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.OutCubic);
					FadeStuff(defend2_castle.heart, 0f, 0.5f);
				}
			}
		}
	}

	private void Lofi2Setup()
	{
		for (int i = 1; i <= 2; i++)
		{
			EnableStuff(lofi2_warptiles[i].objectsToFade);
			EnableStuff(lofi2_warptiles[i].shape);
			lofi2_warptiles[i].transform.position += Vector3.left * 4.5f;
			lofi2_links[i].transform.position += Vector3.left * 4.5f;
		}
	}

	private void Lofi2FadeOut0()
	{
		FadeStuff(lofi2_warptiles[0].objectsToFade, 0f, beats(2f, 190f));
		FadeStuff(lofi2_warptiles[0].shape, 0f, beats(2f, 190f));
	}

	private void Lofi2FadeIn12()
	{
		if (ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			for (int i = 1; i <= 2; i++)
			{
				FadeStuff(lofi2_warptiles[i].objectsToFade, 1f, beats(4f, 190f));
				FadeStuff(lofi2_warptiles[i].shape, 0.21f, beats(4f, 190f));
				lofi2_warptiles[i].transform.DOMoveX(4.5f, beats(4f, 190f)).SetEase(Ease.OutSine).SetRelative(isRelative: true);
				lofi2_links[i].transform.DOMoveX(4.5f, beats(4f, 190f)).SetEase(Ease.OutSine).SetRelative(isRelative: true);
			}
		}
	}

	private void Lofi2Update()
	{
		for (int i = 0; i < lofi2_links.Count; i++)
		{
			lofi_sub = (float)i / (float)(lofi2_links.Count - 1) * 0.25f;
			lofi_alp = Mathf.Max(0f, 0.4f - lofi_sub + 0.2f * Mathf.Sin((float)Math.PI * 2f * ((float)i / 9f) + (float)songBeat * 0.33f * (float)Math.PI));
			lofi_origColor = lofi2_links[i].render.material.GetColor("_Color");
			lofi_newColor = new Color(lofi_origColor.r, lofi_origColor.g, lofi_origColor.b, lofi_alp * lofi2_warptiles[i].tile.render.material.GetColor("_Color").a);
			lofi2_links[i].render.material.SetColor("_Color", lofi_newColor);
		}
	}

	private void EndingWarp()
	{
		if (curEndingWarp < lofi2_warptiles.Count)
		{
			lofi2_warptiles[curEndingWarp].Trip();
		}
		curEndingWarp++;
	}

	private void FinaleUpdate()
	{
		if (!gotC2P)
		{
			C2P = defend2_castle.transform.position;
			gotC2P = true;
		}
		if (!GCS.practiceMode && !charlieWon && (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2))
		{
			charlieWon = true;
			charlie.RemoveActionsWithLabel("endingDive");
			charlie.AddEntry(charlieWaitABit);
			charlie.AddEntry(charlieJumpToGoal);
			charlie.AddEntry(charlieVictory);
		}
		if (songBeat > 963.0 && songBeat < 994.0)
		{
			Defend2Update();
		}
		if (songBeat < 994.0)
		{
			Lofi2Update();
			Fog2Update();
		}
		if (endingFloorChecking < endingFloorsToCheck.Count)
		{
			scrFloor scrFloor2 = scrLevelMaker.instance.listFloors[endingFloorsToCheck[endingFloorChecking]];
			if (scrFloor2.grade != HitMargin.TooEarly)
			{
				scrFloor2.thisTransform.DOMoveY(2f, beats(2f, 200f)).SetEase(Ease.InCubic);
				scrFloor2.TweenOpacity(0f, beats(2f, 200f));
				endingFloorChecking++;
			}
		}
		bgAll.transform.position = Vector3.forward * (base.camy.zoomSize - 1f) * 8f;
		defend2_castle.transform.localPosition = C2P - bgAll.transform.position.z * Vector3.forward;
		defend2_castle.transform.localScale = Vector3.one * 1.75f / base.camy.zoomSize;
	}

	private void MoveFinalText()
	{
		scrUIController.instance.txtCongrats.transform.DOLocalMoveY(100f, 0f);
		scrUIController.instance.txtAllStrictClear.transform.DOLocalMoveY(260f, 0f);
	}

	private void CharlieSplat()
	{
		if (!GCS.practiceMode && ADOBase.controller.currentState != States.Fail && ADOBase.controller.currentState != States.Fail2)
		{
			charlie.AddEntry(finalSplat);
			if (!levelNameTextPresent)
			{
				lvlname.DOAnchorPosY(lvlnameAnchorPos.y + 20f, 1f).SetEase(Ease.OutBack);
			}
		}
	}

	private void Fail()
	{
		ADOBase.controller.playerOne.Die();
		Wrong();
	}

	private void ShowFinalResults()
	{
		if (GCS.practiceMode || (GCS.speedTrialMode && (!GCS.speedTrialMode || !(GCS.currentSpeedTrial >= 1f))) || ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2)
		{
			return;
		}
		ADOBase.controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: false);
		int[] array = SaveMedals(slumpo ? "T3EX" : "T3", slumpo ? Mawaru2_Stats.sectionStats : Mawaru_Stats.sectionStats);
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
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.03f * (float)j)
				.Append(endingStuff[j].transform.DOScale(Vector3.one * endingStuff_originalSizes[j], 0.3f).SetEase(Ease.OutBack));
		}
		if (slumpo)
		{
			endingStuffText[0].text = $"{Mawaru2_Stats.goals}/{soccer_floors.Count}";
			endingStuffText[1].text = $"{Mawaru2_Stats.hifive}/{h5_floors.Count}";
			endingStuffText[2].text = $"{Mawaru2_Stats.coins}/{collect_coins.Count}";
			endingStuffText[3].text = $"{Mawaru2_Stats.checkpointsUsed}";
		}
		else
		{
			endingStuffText[0].text = $"{Mawaru_Stats.goals}/{soccer_floors.Count}";
			endingStuffText[1].text = $"{Mawaru_Stats.hifive}/{h5_floors.Count}";
			endingStuffText[2].text = $"{Mawaru_Stats.coins}/{collect_coins.Count}";
			endingStuffText[3].text = $"{Mawaru_Stats.checkpointsUsed}";
		}
		for (int k = 0; k < endingStuffText.Count; k++)
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(endingStuffText[k].DOColor(whiteClear, 0f))
				.AppendInterval(0.75f + 0.1f * (float)k)
				.Append(endingStuffText[k].DOColor(Color.white, 0.5f));
		}
		ADOBase.controller.canExitLevel = true;
	}
}
