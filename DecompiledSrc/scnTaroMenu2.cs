using DG.Tweening;
using MobileMenu;
using TMPro;
using UnityEngine;

public class scnTaroMenu2 : LevelSelectBase
{
	private scrCamera camera;

	public MenuArrow toPuzzle;

	public MenuArrow toStage;

	public scrFloor puzzle;

	public TextMeshPro exitText;

	public scrCamera mainCamera;

	[Header("Mobile")]
	public Transform puzzleFloorsContainer;

	public Transform mobileIslandCenter;

	public GameObject mobileIslandContainer;

	public MobileMenuController mobileMenu;

	private scrPlanet chosenplanet => ADOBase.controller.chosenPlanet;

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

	protected override void Awake()
	{
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.LoadMap("taro2");
			MobileMenuPortalScreen mapCenter = mobileMenu.map.portalLUT["T4"];
			mobileMenu.map.SetMapCenter(mapCenter);
		}
		base.Awake();
	}

	private void Start()
	{
		bool flag = Persistence.IsWorldComplete("T4");
		int worldAttempts = Persistence.GetWorldAttempts(GCNS.worldData["T4"].index);
		if (!GCS.banished && !Persistence.banishmentPuzzleComplete && worldAttempts == 0 && !flag)
		{
			GCS.banished = true;
		}
		camera = scrCamera.instance;
		Persistence.Save();
		if (Persistence.IsWorldComplete("T4"))
		{
			toPuzzle.FadeIn();
		}
		else
		{
			toStage.FadeIn();
			puzzle.isLandable = false;
			puzzle.transform.localScale = Vector3.zero;
		}
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
		if (GCS.banished)
		{
			DoMaze();
		}
		else if (ADOBase.isMobileMenu)
		{
			camy.enabled = false;
			ADOBase.controller.responsive = false;
			MobileMenuScreen mobilPuzzleRoomsScreen = GetMobilPuzzleRoomsScreen();
			if (GCS.worldEntrance == "TP")
			{
				mobileMenu.JumpToScreen(mobilPuzzleRoomsScreen, instant: true);
			}
			else
			{
				mobileMenu.JumpToMenuEntrance();
			}
			mobilPuzzleRoomsScreen?.transform.Find("Gradient").gameObject.SetActive(value: true);
		}
		else if (GCS.worldEntrance != null)
		{
			JumpToWorldPortal(GCS.worldEntrance, instant: true);
		}
		if (ADOBase.isMobileMenu)
		{
			GetMobilPuzzleRoomsScreen()?.transform.Find("Gradient").gameObject.SetActive(value: true);
		}
	}

	private MobileMenuScreen GetMobilPuzzleRoomsScreen()
	{
		foreach (MobileMenuGroup value in mobileMenu.map.groupLUT.Values)
		{
			foreach (MobileMenuScreen visibleScreen in value.visibleScreens)
			{
				if (visibleScreen is PuzzleRoomsScreen)
				{
					return visibleScreen;
				}
			}
		}
		return null;
	}

	private void DoMaze()
	{
		Vector2 vector = new Vector2(-18f, -4f);
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.Enable(enable: false, instant: true);
			Vector2 vector2 = Vector2.left * 80f;
			vector += vector2;
			foreach (Transform item in puzzleFloorsContainer)
			{
				item.position += (Vector3)vector2;
			}
			mobileMenu.mapContainer.position = mobileIslandCenter.position + Vector3.up * 5f;
			mobileIslandContainer.SetActive(value: true);
			mobileMenu.map.portalLUT["T4"].portal.FadePortal(1f, instant: true);
			HideAllButT4();
		}
		else
		{
			GCS.banished = false;
		}
		chosenplanet.transform.position = new Vector3(vector.x, vector.y, chosenplanet.transform.position.z);
		base.menuPhase = 1;
		camy.ViewObjectInstant(chosenplanet.transform, includeOffset: true);
		camera.timer = 0f;
		camera.positionState = PositionState.None;
		camy.isMoveTweening = true;
	}

	private void HideAllButT4(bool hide = true)
	{
		_ = mobileMenu.map.rootGroup;
		foreach (MobileMenuGroup value in mobileMenu.map.groupLUT.Values)
		{
			if (value.visibleScreens == null)
			{
				continue;
			}
			foreach (MobileMenuScreen item in value)
			{
				if (item is MobileMenuPortalScreen)
				{
					MobileMenuPortalScreen mobileMenuPortalScreen = item as MobileMenuPortalScreen;
					if (mobileMenuPortalScreen.world != "T4")
					{
						mobileMenuPortalScreen.portal.FadePortal(hide ? 0f : 0.2f, hide);
					}
				}
				else
				{
					item.transform.gameObject.SetActive(!hide);
				}
			}
		}
	}

	private void GoToMobileMenu()
	{
		ADOBase.controller.responsive = false;
		ADOBase.controller.isCutscene = true;
		ADOBase.controller.camy.enabled = false;
		mobileMenu.Enable(enable: true);
		mobileMenu.JumpToScreen(mobileMenu.map.mapCenter, instant: false, 2f);
	}

	public void FinishMaze()
	{
		Persistence.banishmentPuzzleComplete = true;
		Persistence.Save();
	}

	private void FinishMazeMobile()
	{
		GCS.banished = false;
		GoToMobileMenu();
		HideAllButT4(hide: false);
		Vector3 endValue = Vector3.down * 20f;
		ADOBase.controller.chosenPlanet.transform.DOMove(endValue, 5f).SetRelative(isRelative: true);
		puzzleFloorsContainer.DOMove(endValue, 5f).SetRelative(isRelative: true);
		Persistence.banishmentPuzzleComplete = true;
		Persistence.Save();
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenPlanet.transform.position;
		int num = Mathf.RoundToInt(position.y);
		int num2 = Mathf.RoundToInt(position.x);
		if (ADOBase.isMobileMenu && GCS.banished)
		{
			Vector2 b = (Vector2)mobileIslandCenter.position + Vector2.up * 2f;
			if (Vector2.Distance(position, b) < 2f)
			{
				FinishMazeMobile();
			}
			return;
		}
		if (!ADOBase.controller.isCutscene)
		{
			if (num == 2 && num2 > -5)
			{
				camera.positionState = PositionState.TaroMenu2TopLane;
			}
			else if (num == -8 && num2 > 2)
			{
				camera.positionState = PositionState.TaroMenu2BottomLane;
			}
			else
			{
				camera.positionState = PositionState.None;
			}
		}
		else
		{
			camera.timer = 0f;
			camera.positionState = PositionState.None;
		}
		if (num2 > -5)
		{
			if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Alpha0))
			{
				JumpWithKey(0);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				JumpWithKey(1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				JumpWithKey(2);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				JumpWithKey(3);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				JumpWithKey(4);
			}
		}
	}

	public void JumpWithKey(int key)
	{
		if (!ADOBase.isMobileMenu && !ADOBase.controller.playerOne.holding && !ADOBase.controller.isCutscene)
		{
			switch (key)
			{
			case 1:
				JumpToWorldPortal("T1");
				break;
			case 2:
				JumpToWorldPortal("T2");
				break;
			case 3:
				JumpToWorldPortal("T3");
				break;
			default:
				JumpToWorldPortal("T4");
				break;
			}
		}
	}

	public void JumpToWorldPortal(string world, bool instant = false, bool wipeFirst = false)
	{
		if (!responsive)
		{
			return;
		}
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpToWorldPortal(world, instant: true);
				scrUIController.instance.WipeFromBlack();
			}, delegate
			{
				responsive = true;
			});
		}
		else
		{
			if (world == null)
			{
				return;
			}
			Vector2 vector = new Vector2(0f, 0f);
			if (!GCS.banished)
			{
				if (scrPortal.portals.ContainsKey(world) && scrPortal.portals[world] != null)
				{
					vector = scrPortal.portals[world].jumpPosition;
				}
			}
			else
			{
				vector = new Vector2(-18f, -4f);
				GCS.banished = false;
			}
			chosenplanet.transform.position = new Vector3(vector.x, vector.y, chosenplanet.transform.position.z);
			base.menuPhase = 1;
			if (instant)
			{
				camy.ViewObjectInstant(chosenplanet.transform, includeOffset: true);
			}
			else
			{
				camy.Refocus(chosenplanet.transform);
			}
			camera.timer = 0f;
			camera.positionState = PositionState.None;
			camy.isMoveTweening = true;
		}
	}
}
