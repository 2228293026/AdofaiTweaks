using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnLevelSelect : LevelSelectBase
{
	public const int CrownEntrance = 50;

	public const int CrownExit = 51;

	public const int MuseDashEntrance = 52;

	public const int MuseDashExit = 53;

	private const int CR2024Entrance = 54;

	private const int CR2024Exit = 55;

	private readonly Vector2 originPosition = new Vector2(0f, 0f);

	private readonly Vector2 xtraIslandPosition = new Vector2(0f, 9f);

	private readonly Vector2 crownIslandPosition = new Vector2(0f, 23f);

	private readonly Vector2 changingRoomPosition = new Vector2(0f, -6f);

	private readonly Vector2 changingRoom2Position = new Vector2(0f, -19f);

	private readonly Vector2 museDashIslandPosition = new Vector2(-25f, 27f);

	private readonly Vector2 cr2024IslandPosition = new Vector2(20f, 25f);

	private readonly RDCheatCode cheatIslandCheatCode = new RDCheatCode("imacheater");

	private readonly RDCheatCode qureeCheatCode = new RDCheatCode("quree");

	private readonly RDCheatCode star4thCheatCode = new RDCheatCode("star4th");

	private readonly RDCheatCode paradiddleCheatCode = new RDCheatCode("→←→→←→←←→←→→←→←←");

	private readonly RDCheatCode bonusLevelCheatCode = new RDCheatCode("thanks");

	private readonly RDCheatCode unlockAllCheatCode = new RDCheatCode("idkfa");

	private readonly RDCheatCode heysoraCheatCode = new RDCheatCode("heysora");

	private readonly RDCheatCode transCheatCode = new RDCheatCode("transrights");

	private readonly RDCheatCode minesweeperCheatCode = new RDCheatCode("kaboom");

	private readonly RDCheatCode nbCheatCode = new RDCheatCode("nbrights");

	private readonly RDCheatCode rainbowCheatCode = new RDCheatCode("↑↑↓↓←→←→ba⏎");

	private readonly RDCheatCode faceCheatCode = new RDCheatCode("thonk");

	private readonly RDCheatCode samuraiCheatCode = new RDCheatCode("samurai");

	private readonly RDCheatCode aprilFoolsCheatCode = new RDCheatCode("aprilfools");

	private readonly RDCheatCode firestixCheatCode = new RDCheatCode("firestix");

	public scrCamera mainCamera;

	public SpriteRenderer tv;

	public SpriteRenderer tvChick;

	public Sprite tvChickSprite0;

	public Sprite tvChickSprite1;

	public CanvasGroup textGroup;

	public Text rdOfferText;

	public Text rdOfferDismiss;

	public scrFloor rdFloor;

	public LevelSelectCutsceneController cutsceneController;

	public scrFloor crownEntranceGem;

	public scrFloor crownExitGem;

	public GameObject changingRoomGem;

	public GameObject changingRoomSmall;

	public GameObject changingRoomBig;

	public GameObject changingRoomCoop;

	public readonly Dictionary<GCNS.WorldData.IslandType, LevelSelectIsland> islands = new Dictionary<GCNS.WorldData.IslandType, LevelSelectIsland>();

	public GameObject editorFloor;

	public GameObject editorText;

	[NonSerialized]
	public string lastVisitedWorld;

	private bool showRDOffer;

	private bool showingRDOfferText;

	private uint frameUsed;

	private float lastPlanetX;

	private int verticalDirection;

	public bool playingCutscene;

	private bool shouldReunifyPlanets;

	private const int MAX_WORLD_KEYS = 10;

	private Dictionary<string, Dictionary<int, string>> numberWorldMap = new Dictionary<string, Dictionary<int, string>>
	{
		{
			"Xtra",
			new Dictionary<int, string>
			{
				{ 1, "XF" },
				{ 2, "XC" },
				{ 3, "XH" },
				{ 4, "PA" },
				{ 5, "XS" },
				{ 6, "XR" },
				{ 7, "RJ" },
				{ 8, "XN" },
				{ 9, "XM" },
				{ 10, "AR" }
			}
		},
		{
			"Crown",
			new Dictionary<int, string>
			{
				{ 1, "XO" },
				{ 2, "XI" },
				{ 3, "XT" }
			}
		},
		{
			"MuseDash",
			new Dictionary<int, string>
			{
				{ 1, "MN" },
				{ 2, "ML" },
				{ 3, "MO" }
			}
		},
		{
			"CR2024",
			new Dictionary<int, string> { { 1, "CE" } }
		},
		{
			"Main1",
			new Dictionary<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
				{ 6, "6" },
				{ 7, "B" }
			}
		},
		{
			"Main2",
			new Dictionary<int, string>
			{
				{ 1, "7" },
				{ 2, "8" },
				{ 3, "9" },
				{ 4, "10" },
				{ 5, "11" },
				{ 6, "12" }
			}
		}
	};

	private double _audioBufferCheckTime;

	private double _audioBufferCheckDspTime;

	private bool _audioBufferCheckedSkippedLastFrame;

	private static bool _shouldShowAudioBufferChangeNotification;

	public new static scnLevelSelect instance => LevelSelectBase.instance as scnLevelSelect;

	private scrPlanet chosenplanet => ADOBase.controller.chosenPlanet;

	private Transform planetTransform => chosenplanet.transform;

	private bool responsive
	{
		get
		{
			return ADOBase.controller.responsive;
		}
		set
		{
			ADOBase.controller.responsive = value;
		}
	}

	private scrCamera camy => ADOBase.controller.camy;

	public bool inCrownIsland
	{
		get
		{
			Vector3 position = planetTransform.position;
			double num = Math.Round(Mathf.Abs(position.x - crownIslandPosition.x));
			double num2 = Math.Round(position.y - crownIslandPosition.y);
			if (num <= 5.0)
			{
				if (num2 >= -1.0)
				{
					return num2 <= 6.0;
				}
				return false;
			}
			return false;
		}
	}

	public bool inExtraIsland
	{
		get
		{
			Vector3 position = planetTransform.position;
			double num = Math.Round(Mathf.Abs(position.x - xtraIslandPosition.x));
			double num2 = Math.Round(position.y - xtraIslandPosition.y);
			if (num <= 3.0)
			{
				if (num2 >= -1.0)
				{
					return num2 <= 6.0;
				}
				return false;
			}
			return false;
		}
	}

	public bool inMuseDashIsland
	{
		get
		{
			Vector3 position = planetTransform.position;
			double num = Math.Round(Mathf.Abs(position.x - museDashIslandPosition.x));
			double num2 = Math.Round(position.y - museDashIslandPosition.y);
			if (num <= 4.0)
			{
				if (num2 >= -7.0)
				{
					return num2 <= 0.0;
				}
				return false;
			}
			return false;
		}
	}

	public bool inCR2024Island
	{
		get
		{
			Vector3 position = planetTransform.position;
			if (position.x >= cr2024IslandPosition.x && position.y >= cr2024IslandPosition.y - 1f)
			{
				return position.y <= cr2024IslandPosition.y + 1f;
			}
			return false;
		}
	}

	public bool inCrownIslandPosition => planetTransform.position == new Vector3(crownIslandPosition.x, crownIslandPosition.y, planetTransform.position.z);

	public bool inExtraIslandPosition => planetTransform.position == new Vector3(xtraIslandPosition.x, xtraIslandPosition.y, planetTransform.position.z);

	public bool inMuseDashIslandPosition => planetTransform.position == new Vector3(museDashIslandPosition.x, museDashIslandPosition.y, planetTransform.position.z);

	public bool inCR2024IslandPosition => planetTransform.position == new Vector3(cr2024IslandPosition.x, cr2024IslandPosition.y, planetTransform.position.z);

	protected override void Awake()
	{
		base.Awake();
		RDC.forceMobile = false;
		Application.logMessageReceived -= ADOStartup.LogMessageReceived;
		scnCLS.DeactivateCustomLevelModifiers();
		camy.isMoveTweening = false;
		camy.positionState = PositionState.Origin;
	}

	private void Start()
	{
		RDInput.SetMapping("LevelSelect");
		showRDOffer = false;
		ShowRDOffer(showRDOffer);
		ShowRDOfferText(show: false);
		UpdateCameraOrthoSize();
		mainCamera.setCamSizeInstant(mainCamera.camsizenormal);
		if (GCS.worldEntrance != null && !GCS.FOOL_JOKER)
		{
			JumpToWorldPortal(GCS.worldEntrance, instant: true);
		}
		playingCutscene = cutsceneController.CheckForCutscene() != null;
		if (crownEntranceGem != null)
		{
			float animDuration = 8f;
			float s = 0.5f;
			Color color = Color.HSVToRGB(0f, s, 1f);
			crownEntranceGem.ColorFloor(TrackColorType.Rainbow, color, Color.white, animDuration, TrackColorPulse.None, 1f);
			crownExitGem.ColorFloor(TrackColorType.Rainbow, color, Color.white, animDuration, TrackColorPulse.None, 1f);
		}
		if (_shouldShowAudioBufferChangeNotification)
		{
			_shouldShowAudioBufferChangeNotification = false;
			Notification.instance.ShowAudioBufferChange(Persistence.audioBufferSize);
		}
		ADOBase.conductor.onBeats.Add(this);
		bool coopMode = scrController.coopMode;
		int overallProgressStage = Persistence.GetOverallProgressStage();
		changingRoomSmall.SetActive(value: false);
		changingRoomBig.SetActive(value: false);
		changingRoomCoop.SetActive(value: false);
		GameObject gameObject = (coopMode ? changingRoomCoop : ((overallProgressStage >= 3) ? changingRoomBig : ((overallProgressStage > 0) ? changingRoomSmall : null)));
		if ((bool)gameObject)
		{
			gameObject.SetActive(value: true);
		}
		changingRoomGem.SetActive(coopMode || Persistence.GetOverallProgressStage() > 0);
	}

	private void Update()
	{
		CheckAudioBreak();
		if (showRDOffer)
		{
			float x = ADOBase.controller.chosenPlanet.transform.position.x;
			if (x != lastPlanetX)
			{
				if (x - float.Epsilon > 6f && x + float.Epsilon < 10f)
				{
					if (!showingRDOfferText)
					{
						ShowRDOfferText(show: true);
					}
				}
				else if (showingRDOfferText)
				{
					ShowRDOfferText(show: false);
				}
				lastPlanetX = x;
			}
		}
		if (!GCS.FOOL_JOKER)
		{
			if (cheatIslandCheatCode.CheckCheatCode())
			{
				if (ADOBase.sceneName != GCNS.sceneLevelSelect || chosenplanet.currfloor.GetComponent<scrMenuMovingFloor>() != null)
				{
					return;
				}
				if (chosenplanet.transform.position.x < -5f)
				{
					JumpAndWipeWithKey(52);
				}
				else if (chosenplanet.transform.position.y > 20f)
				{
					JumpAndWipeWithKey(50);
				}
				else
				{
					JumpToWorldPortal("XO", instant: false, wipeFirst: true);
				}
			}
			if (qureeCheatCode.CheckCheatCode())
			{
				ADOBase.controller.EnterLevel("XO-1");
			}
			if (star4thCheatCode.CheckCheatCode())
			{
				ADOBase.controller.EnterLevel("XO-X", speedTrial: true);
			}
			if (RDC.debug && bonusLevelCheatCode.CheckCheatCode())
			{
				ADOBase.controller.EnterLevel("B-X");
			}
		}
		if (unlockAllCheatCode.CheckCheatCode() && GCS.allowDebug)
		{
			Persistence.Complete100();
			ADOBase.GoToLevelSelect();
			scrFlash.Flash(Color.white);
		}
		if (transCheatCode.CheckCheatCode())
		{
			ADOBase.controller.playerOne.planetarySystem.TransMode();
			scrFlash.Flash(Color.white);
		}
		if (nbCheatCode.CheckCheatCode())
		{
			ADOBase.controller.playerOne.planetarySystem.EnbyMode();
			scrFlash.Flash(Color.white);
		}
		if (rainbowCheatCode.CheckCheatCode())
		{
			ADOBase.controller.playerOne.planetarySystem.RainbowMode();
			scrFlash.Flash(Color.white);
		}
		if (faceCheatCode.CheckCheatCode())
		{
			ADOBase.controller.playerOne.planetarySystem.ToggleEmojiMode();
			scrFlash.Flash(Color.white);
		}
		if (minesweeperCheatCode.CheckCheatCode())
		{
			scnMinesweeper.EnterScene();
		}
		if (samuraiCheatCode.CheckCheatCode())
		{
			ADOBase.controller.playerOne.planetarySystem.SamuraiMode();
			scrFlash.Flash(Color.white);
		}
		if (aprilFoolsCheatCode.CheckCheatCode())
		{
			ffxMenuFoolSwirl[] array = UnityEngine.Object.FindObjectsByType<ffxMenuFoolSwirl>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
			if (array.Length != 0)
			{
				ffxMenuFoolSwirl obj = array[0];
				obj.gameObject.SetActive(value: true);
				obj.Start();
				obj.StartEffect(ADOBase.controller.playerOne.planetarySystem.chosenPlanet);
			}
			else
			{
				GCS.FOOL_SWIRL = true;
			}
		}
		if (firestixCheatCode.CheckCheatCode())
		{
			ffxMenuFoolJoker ffxMenuFoolJoker2 = UnityEngine.Object.FindAnyObjectByType<ffxMenuFoolJoker>(FindObjectsInactive.Include);
			if (ffxMenuFoolJoker2 != null)
			{
				ffxMenuFoolJoker2.gameObject.SetActive(value: true);
			}
		}
		if (RDEditorUtils.CheckPlayerLogKeyCombo())
		{
			RDEditorUtils.OpenLogDirectory();
		}
		if (ADOBase.controller.paused)
		{
			return;
		}
		if (GCS.d_drumcontroller && Input.GetKeyDown(KeyCode.End))
		{
			JumpAndWipeWithKey(0);
		}
		if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Alpha0))
		{
			JumpAndWipeWithKey(0);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			JumpAndWipeWithKey(1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			JumpAndWipeWithKey(2);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			JumpAndWipeWithKey(3);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			JumpAndWipeWithKey(4);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			JumpAndWipeWithKey(5);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			JumpAndWipeWithKey(6);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			JumpAndWipeWithKey(7);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			JumpAndWipeWithKey(8);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			JumpAndWipeWithKey(9);
		}
		if (!ADOBase.isSwitch && (!(ADOBase.controller?.creditsText) || !ADOBase.controller.creditsText.planetOnPosition))
		{
			if (RDInput.leftPress)
			{
				JumpHorizontal(-1);
			}
			else if (RDInput.rightPress)
			{
				JumpHorizontal(1);
			}
			else if (RDInput.upPress)
			{
				JumpVertical(1);
			}
			else if (RDInput.downPress)
			{
				JumpVertical(-1);
			}
		}
		if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.C))
		{
			ADOBase.controller.PortalTravelAction(Portal.CustomLevelsScene);
		}
		else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.E))
		{
			ADOBase.controller.PortalTravelAction(Portal.EditorScene);
		}
		else if (ADOBase.IsAprilFools() && RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.J))
		{
			ADOBase.controller.PortalTravelAction(Portal.FoolJoker);
		}
		else if (!GCS.FOOL_JOKER)
		{
			if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.N) && base.neoCosmosManager.installed)
			{
				ADOBase.controller.PortalTravelAction(Portal.TaroDLCMap);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.V) && base.vegaDLCManager.installed)
			{
				ADOBase.controller.PortalTravelAction(Portal.VegaDLCMap);
			}
		}
	}

	private void LateUpdate()
	{
		if (shouldReunifyPlanets)
		{
			ADOBase.playerManager.ReunifyPlanets(lastPlanetLanded.player);
			shouldReunifyPlanets = false;
		}
		UpdateCameraOrthoSize();
	}

	public void DeferReunifyPlanets()
	{
		shouldReunifyPlanets = true;
	}

	private void UpdateCameraOrthoSize(bool print = false)
	{
		float value = (float)Screen.width * 1f / (float)Screen.height;
		float t = Mathf.InverseLerp(1.7777778f, 1.3333334f, value);
		mainCamera.tosize = Mathf.Lerp(5f, 6.7f, t);
		float tosize = mainCamera.tosize;
		mainCamera.camsizenormal = tosize;
		mainCamera.Bgcamstatic.orthographicSize = tosize;
		mainCamera.BGcam.orthographicSize = tosize;
	}

	public void DismissRDOffer()
	{
		Persistence.showRDOffer = false;
		ShowRDOffer(show: false);
	}

	public void ShowRDOffer(bool show)
	{
		tv.gameObject.SetActive(show);
		rdFloor.gameObject.SetActive(show);
	}

	public void ShowRDOfferText(bool show)
	{
		showingRDOfferText = show;
		float endValue = (show ? 1f : 0f);
		float endValue2 = (show ? 0.3f : 1f);
		tv.DOFade(endValue2, 0.3f);
		tvChick.DOFade(endValue2, 0.3f);
		textGroup.DOFade(endValue, 0.3f);
	}

	public override void OnBeat()
	{
		frameUsed++;
		tvChick.sprite = ((frameUsed % 2 == 0) ? tvChickSprite0 : tvChickSprite1);
	}

	public void JumpVertical(int direction)
	{
		if (responsive)
		{
			string areaByWorld = GetAreaByWorld(lastVisitedWorld);
			if ((areaByWorld == "Main1" && direction == -1) || (areaByWorld == "Main2" && direction == 1))
			{
				JumpAndWipeWithKey(GetNumberByWorld(lastVisitedWorld));
			}
			else
			{
				JumpAndWipeWithKey(0, wipeFirst: false, direction == 1);
			}
		}
	}

	public void JumpHorizontal(int direction)
	{
		if (!responsive)
		{
			return;
		}
		int num = GetNumberByWorld(lastVisitedWorld);
		int num2 = 0;
		while (!JumpAndWipeWithKey(num += direction) && ++num2 <= 10)
		{
			if (num < 0)
			{
				num = 11;
			}
			else if (num > 10)
			{
				num = -1;
			}
		}
	}

	public void JumpWithKey(int key)
	{
		JumpAndWipeWithKey(key, wipeFirst: true);
	}

	private int GetNumberByWorld(string world)
	{
		string text = default(string);
		Dictionary<int, string> dictionary = default(Dictionary<int, string>);
		int num = default(int);
		foreach (KeyValuePair<string, Dictionary<int, string>> item in numberWorldMap)
		{
			item.Deconstruct(ref text, ref dictionary);
			foreach (KeyValuePair<int, string> item2 in dictionary)
			{
				item2.Deconstruct(ref num, ref text);
				int num2 = num;
				if (text == world)
				{
					num = num2;
					return num;
				}
			}
		}
		return 0;
	}

	private string GetAreaByWorld(string world)
	{
		string text = default(string);
		Dictionary<int, string> dictionary = default(Dictionary<int, string>);
		int num = default(int);
		foreach (KeyValuePair<string, Dictionary<int, string>> item in numberWorldMap)
		{
			item.Deconstruct(ref text, ref dictionary);
			string text2 = text;
			foreach (KeyValuePair<int, string> item2 in dictionary)
			{
				item2.Deconstruct(ref num, ref text);
				if (text == world)
				{
					text = text2;
					return text;
				}
			}
		}
		return null;
	}

	public bool JumpAndWipeWithKey(int key, bool wipeFirst = false, bool reverse = false)
	{
		if (chosenplanet.currfloor.TryGetComponent<scrMenuMovingFloor>(out var component) && component.moving && key < 50)
		{
			return false;
		}
		bool flag = chosenplanet.transform.position.y > 20f;
		bool wipeFirst2 = wipeFirst || flag;
		switch (key)
		{
		case 50:
			return JumpToWorldPortal(50.ToString(), instant: false, wipeFirst: true);
		case 51:
			return JumpToWorldPortal(51.ToString(), instant: false, wipeFirst: true);
		case 52:
			return JumpToWorldPortal(52.ToString(), instant: false, wipeFirst: true);
		case 53:
			return JumpToWorldPortal(53.ToString(), instant: false, wipeFirst: true);
		case 54:
			return JumpToWorldPortal(54.ToString(), instant: false, wipeFirst: true);
		case 55:
			return JumpToWorldPortal(55.ToString(), instant: false, wipeFirst: true);
		case 0:
		{
			if (chosenplanet.currfloor.TryGetComponent<WorldSelectorTile>(out var component2))
			{
				Vector3 position = component2.island.portalFloor.transform.position;
				ADOBase.playerManager.ReunifyPlanets(chosenplanet.player, new Vector2(position.x, position.y - 1f));
				lastVisitedWorld = "0";
				return true;
			}
			verticalDirection = (reverse ? 1 : (-1));
			return JumpToWorldPortal(null, instant: false, wipeFirst);
		}
		default:
			if (inExtraIsland)
			{
				string valueOrDefault = CollectionExtensions.GetValueOrDefault<int, string>((IReadOnlyDictionary<int, string>)numberWorldMap["Xtra"], key);
				LevelSelectIsland island = islands[GCNS.WorldData.IslandType.Xtra];
				if (valueOrDefault != null)
				{
					JumpToIslandFloor(island, valueOrDefault);
				}
				return true;
			}
			if (inCrownIsland)
			{
				string valueOrDefault2 = CollectionExtensions.GetValueOrDefault<int, string>((IReadOnlyDictionary<int, string>)numberWorldMap["Crown"], key);
				LevelSelectIsland island2 = islands[GCNS.WorldData.IslandType.Crown];
				if (valueOrDefault2 != null)
				{
					JumpToIslandFloor(island2, valueOrDefault2);
				}
				return true;
			}
			if (inMuseDashIsland)
			{
				string valueOrDefault3 = CollectionExtensions.GetValueOrDefault<int, string>((IReadOnlyDictionary<int, string>)numberWorldMap["MuseDash"], key);
				LevelSelectIsland island3 = islands[GCNS.WorldData.IslandType.MuseDash];
				if (valueOrDefault3 != null)
				{
					JumpToIslandFloor(island3, valueOrDefault3);
				}
				return true;
			}
			if (inCR2024Island)
			{
				string text = ((key != 1) ? null : "CE");
				string text2 = text;
				if (text2 != null)
				{
					return JumpToWorldPortal(text2);
				}
			}
			else if (chosenplanet.transform.position.y < -3.5f && chosenplanet.transform.position.x > 2f)
			{
				string valueOrDefault4 = CollectionExtensions.GetValueOrDefault<int, string>((IReadOnlyDictionary<int, string>)numberWorldMap["Main2"], key);
				if (valueOrDefault4 != null)
				{
					return JumpToWorldPortal(valueOrDefault4, instant: false, wipeFirst2);
				}
			}
			else
			{
				string valueOrDefault5 = CollectionExtensions.GetValueOrDefault<int, string>((IReadOnlyDictionary<int, string>)numberWorldMap["Main1"], key);
				if (valueOrDefault5 != null)
				{
					return JumpToWorldPortal(valueOrDefault5, instant: false, wipeFirst2);
				}
			}
			return false;
		}
	}

	public bool JumpToWorldPortal(string world, bool instant = false, bool wipeFirst = false)
	{
		if (!responsive)
		{
			return false;
		}
		if (world != null && base.dlcManagers.Any((DLCManager x) => x.IsDLCLevel(world)))
		{
			return false;
		}
		if (GCS.FOOL_JOKER && world != null && world.EndsWith("J"))
		{
			string text = world;
			world = text.Substring(0, text.Length - 1);
		}
		Transform transform = chosenplanet.transform;
		float planetX = transform.position.x;
		float planetY = transform.position.y;
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpToWorldPortal(world, instant: true);
				scrUIController.instance.WipeFromBlack();
				if (int.TryParse(world, out var result3) && (result3 == 50 || result3 == 51 || result3 == 52 || result3 == 53 || result3 == 54 || result3 == 55))
				{
					Collider2D[] array = Physics2D.OverlapPointAll(new Vector2(planetX, planetY), 1 << LayerMask.NameToLayer("Floor"));
					for (int i = 0; i < array.Length; i++)
					{
						scrFloor component = ((Component)(object)array[i]).GetComponent<scrFloor>();
						if (component.isLandable)
						{
							chosenplanet.currfloor = component;
						}
					}
				}
			}, delegate
			{
				responsive = true;
			});
			return false;
		}
		bool flag = world == "7" || world == "8" || world == "9" || world == "10" || world == "11" || world == "12" || world == "B";
		bool flag2 = world != null && world.IsCrownWorld();
		_ = world == 50.ToString();
		bool flag3 = world == 51.ToString();
		bool flag4 = world != null && world.IsMuseDashWorld();
		_ = world == 52.ToString();
		bool flag5 = world == 53.ToString();
		_ = world == 55.ToString();
		int overallProgressStage = Persistence.GetOverallProgressStage();
		if (overallProgressStage < 3 && world == "6")
		{
			return false;
		}
		if (overallProgressStage < 5 && flag)
		{
			return false;
		}
		bool flag6 = world != null && (world.IsXtra() || flag4);
		if (overallProgressStage < 5 && flag6 && !flag2 && !flag4)
		{
			return false;
		}
		if (world != null && int.TryParse(world, out var result) && result >= 1 && result <= 12 && scrPortal.portals[world].jumpPosition == new Vector2Int((int)planetX, (int)planetY) && !GCS.FOOL_JOKER)
		{
			int num = ((result <= 6) ? 6 : (-6));
			JumpToWorldPortal((result + num).ToString());
			return false;
		}
		camy.SetXOffset(0f);
		camy.SetYOffset((world == null || flag6 || flag3 || flag5) ? 0f : 3f);
		if (world == null)
		{
			if (GCS.FOOL_JOKER)
			{
				JumpTo(PositionState.Origin, instant: false);
				return false;
			}
			_ = transform.position == new Vector3(originPosition.x, originPosition.y, transform.position.z);
			bool flag7 = transform.position == new Vector3(changingRoomPosition.x, changingRoomPosition.y, transform.position.z);
			bool flag8 = transform.position == new Vector3(changingRoom2Position.x, changingRoom2Position.y, transform.position.z);
			List<PositionState> list = new List<PositionState>();
			List<PositionState> list2 = new List<PositionState>
			{
				PositionState.CR2024Island,
				PositionState.CrownIsland,
				PositionState.MuseDashIsland
			};
			if (overallProgressStage >= 3)
			{
				list.Add(PositionState.CR2024Island);
			}
			if (overallProgressStage >= 8)
			{
				list.Add(PositionState.CrownIsland);
			}
			if (overallProgressStage >= 3)
			{
				list.Add(PositionState.MuseDashIsland);
			}
			if (overallProgressStage >= 3)
			{
				list.Add(PositionState.XtraIsland);
			}
			list.Add(PositionState.Origin);
			if (overallProgressStage >= 1)
			{
				list.Add(PositionState.ChangingRoom);
				if (!scrController.coopMode)
				{
					list.Add(PositionState.ChangingRoom2);
				}
			}
			PositionState positionState = PositionState.Origin;
			if (inCR2024IslandPosition)
			{
				positionState = PositionState.CR2024Island;
			}
			else if (inCrownIslandPosition)
			{
				positionState = PositionState.CrownIsland;
			}
			else if (inMuseDashIslandPosition)
			{
				positionState = PositionState.MuseDashIsland;
			}
			else if (inExtraIslandPosition)
			{
				positionState = PositionState.XtraIsland;
			}
			else if (flag7)
			{
				positionState = PositionState.ChangingRoom;
			}
			else if (flag8)
			{
				positionState = PositionState.ChangingRoom2;
			}
			int num2 = list.IndexOf(positionState) - ((positionState != PositionState.Origin || camy.positionState == PositionState.Origin) ? verticalDirection : 0);
			if (num2 > list.Count - 1)
			{
				num2 = 0;
			}
			if (num2 < 0)
			{
				num2 = list.Count - 1;
			}
			PositionState positionState2 = list[num2];
			bool flag9 = list2.Contains(positionState) || list2.Contains(positionState2);
			JumpTo(positionState2, flag9, flag9);
		}
		else
		{
			int result2;
			Vector2Int vector2Int = (int.TryParse(world, out result2) ? result2 : (-999)) switch
			{
				50 => new Vector2Int(10, -2), 
				51 => new Vector2Int(0, 21), 
				52 => new Vector2Int(8, -2), 
				53 => new Vector2Int(-25, 27), 
				54 => new Vector2Int(6, -2), 
				55 => new Vector2Int(20, 25), 
				_ => scrPortal.portals[world].jumpPosition, 
			};
			if (world.IsTaro())
			{
				vector2Int = Vector2Int.zero;
			}
			scrPlayer toPlayer = (lastPlanetLanded ? lastPlanetLanded.player : chosenplanet.player);
			ADOBase.playerManager.ReunifyPlanets(toPlayer, vector2Int, instant);
			base.menuPhase = 1;
			if (instant)
			{
				camy.ViewVectorInstant(vector2Int, includeOffset: true);
			}
			else
			{
				camy.Refocus(vector2Int);
			}
			bool num3 = (float)vector2Int.y <= -5f;
			camy.isMoveTweening = true;
			if (num3)
			{
				camy.positionState = PositionState.DLC;
			}
			else if (flag2 || flag3)
			{
				camy.positionState = PositionState.CrownIsland;
				camy.ViewVectorInstant(new Vector2(0f, 21f), includeOffset: true);
			}
			else if (flag4 || flag5)
			{
				camy.positionState = PositionState.MuseDashIsland;
				camy.ViewVectorInstant(new Vector2(-25f, 23f), includeOffset: true);
			}
			else if (flag6)
			{
				camy.positionState = PositionState.XtraIsland;
			}
			else if (Mathf.Approximately(vector2Int.y, cr2024IslandPosition.y))
			{
				camy.positionState = PositionState.CR2024Island;
				camy.SetYOffset(0f);
			}
			else
			{
				camy.positionState = PositionState.Levels;
			}
		}
		return true;
	}

	private void JumpTo(PositionState positionState, bool instant, bool wipeFirst = false)
	{
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpTo(positionState, instant);
				scrUIController.instance.WipeFromBlack();
			}, delegate
			{
				responsive = true;
			});
		}
		else
		{
			JumpTo(positionState, instant);
		}
	}

	private void JumpTo(PositionState positionState, bool instant)
	{
		if (positionState == PositionState.Origin)
		{
			lastVisitedWorld = "0";
		}
		Vector2 vector = positionState switch
		{
			PositionState.XtraIsland => xtraIslandPosition, 
			PositionState.CrownIsland => crownIslandPosition, 
			PositionState.ChangingRoom => changingRoomPosition, 
			PositionState.ChangingRoom2 => changingRoom2Position, 
			PositionState.MuseDashIsland => museDashIslandPosition, 
			PositionState.CR2024Island => cr2024IslandPosition, 
			_ => originPosition, 
		};
		ADOBase.playerManager.ReunifyPlanets(chosenplanet.player, vector, instant: true);
		base.menuPhase = ((positionState != PositionState.Origin) ? 1 : 0);
		camy.positionState = positionState;
		if (instant)
		{
			camy.ViewVectorInstant(vector);
			return;
		}
		camy.Refocus(vector);
		camy.isMoveTweening = true;
	}

	public void JumpToIslandFloor(LevelSelectIsland island, string world, bool instant = false)
	{
		if (!responsive)
		{
			return;
		}
		Vector3 position = island.worldTiles[world].transform.position;
		if (island.speedTrialToggle != null)
		{
			if (planetTransform.position == position)
			{
				island.ToggleSpeedTrial();
			}
		}
		else if (planetTransform.position == position)
		{
			position = island.speedTrialTiles[world].transform.position;
		}
		if (RDUtils.GetFloorAtPosition(position).TryGetComponent<WorldSelectorTile>(out var component) && component.enabled)
		{
			component.StartEffect();
			ADOBase.playerManager.ReunifyPlanets(chosenplanet.player, position, instant);
		}
	}

	public void GoToCheatIsland()
	{
		JumpAndWipeWithKey(51);
	}

	public void GoToMuseDashIsland()
	{
		JumpAndWipeWithKey(53);
	}

	private void CheckAudioBreak()
	{
		if (AudioListener.pause)
		{
			return;
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if ((double)unscaledDeltaTime >= 0.2)
		{
			Debug.Log($"Skipping audio break checking. Detected delta time: {unscaledDeltaTime}", this);
			_audioBufferCheckTime = 0.0;
			_audioBufferCheckedSkippedLastFrame = true;
			return;
		}
		if (_audioBufferCheckedSkippedLastFrame)
		{
			_audioBufferCheckedSkippedLastFrame = false;
			return;
		}
		if (_audioBufferCheckTime == 0.0)
		{
			_audioBufferCheckDspTime = AudioSettings.dspTime;
			_audioBufferCheckTime += unscaledDeltaTime;
			return;
		}
		_audioBufferCheckTime += unscaledDeltaTime;
		if (_audioBufferCheckTime >= 1.0)
		{
			_audioBufferCheckTime = 0.0;
			if (AudioSettings.dspTime - _audioBufferCheckDspTime < 0.75 && Persistence.audioBufferSize < 2048)
			{
				Persistence.audioBufferSize *= 2;
				_shouldShowAudioBufferChangeNotification = true;
				ADOBase.controller.Restart();
			}
		}
	}

	public override void PlanetLandedOnFloor(scrPlanet planet, scrFloor floor)
	{
		Vector3 position = floor.transform.position;
		Camera camobj = camy.camobj;
		float num = 2f * camobj.orthographicSize;
		float num2 = num * camobj.aspect;
		Vector3 position2 = camobj.transform.position;
		float num3 = position2.x - num2 / 2f;
		float num4 = position2.y - num / 2f;
		bool flag = position.x < num3 + 1f || position.x > num3 + num2 - 1f || position.y < num4 + 1f || position.y > num4 + num - 1f;
		bool flag2 = false;
		scrPlayer[] players = ADOBase.controller.playerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			scrPlanet chosenPlanet = players[i].planetarySystem.chosenPlanet;
			if (!(chosenPlanet == planet) && (Vector3.Distance(position, chosenPlanet.transform.position) > 8f || flag))
			{
				flag2 = true;
				break;
			}
		}
		lastPlanetLanded = planet;
		if (flag2)
		{
			ADOBase.playerManager.ReunifyPlanets(planet.player, position);
		}
	}

	public void SetPositionState(int positionState)
	{
		if (camy.positionStateInt != positionState)
		{
			camy.positionStateInt = positionState;
			shouldReunifyPlanets = true;
		}
	}
}
