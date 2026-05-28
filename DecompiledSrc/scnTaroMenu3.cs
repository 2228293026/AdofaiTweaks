using MobileMenu;
using TMPro;
using UnityEngine;

public class scnTaroMenu3 : LevelSelectBase
{
	private scrCamera camera;

	public TextMeshPro exitText;

	public float fontScale = 1f;

	public MenuArrow toStage;

	public scrCamera mainCamera;

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
			mobileMenu.LoadMap("taro3");
			MobileMenuPortalScreen mapCenter = mobileMenu.map.portalLUT["T5"];
			mobileMenu.map.SetMapCenter(mapCenter);
		}
		base.Awake();
	}

	private void Start()
	{
		if (Persistence.taroStoryProgress == 4)
		{
			Persistence.taroStoryProgress = 5;
			Persistence.Save();
		}
		if (ADOBase.isMobileMenu)
		{
			base.enabled = false;
			camy.enabled = false;
			ADOBase.controller.responsive = false;
			if (GCS.worldEntrance == "TP")
			{
				mobileMenu.JumpToScreen(mobileMenu.map.portalLUT["T5"], instant: true);
			}
			else
			{
				mobileMenu.JumpToMenuEntrance();
			}
			return;
		}
		camera = scrCamera.instance;
		toStage.FadeIn();
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
		if (GCS.worldEntrance != null)
		{
			JumpToWorldPortal(GCS.worldEntrance, instant: true);
		}
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenPlanet.transform.position;
		int num = Mathf.RoundToInt(position.y);
		int num2 = Mathf.RoundToInt(position.x);
		if (!ADOBase.controller.isCutscene)
		{
			if (num == 2)
			{
				camera.positionState = PositionState.TaroMenu3TopLane;
			}
			else if (num == -2 && num2 > 2)
			{
				camera.positionState = PositionState.TaroMenu3BottomLane;
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
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			JumpWithKey(5);
		}
	}

	public void JumpWithKey(int key)
	{
		if (!ADOBase.controller.playerOne.holding && !ADOBase.controller.isCutscene)
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
			case 4:
				JumpToWorldPortal("T4");
				break;
			default:
				JumpToWorldPortal("T5");
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
		else if (world != null)
		{
			Vector2 vector = new Vector2(0f, 0f);
			if (!GCS.puzzle)
			{
				vector = scrPortal.portals[world].jumpPosition;
			}
			else
			{
				GCS.puzzle = false;
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
