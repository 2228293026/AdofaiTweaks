using MobileMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scnTaroMenu0 : LevelSelectBase
{
	private scrCamera camera;

	public TextMeshPro exitText;

	public TextMeshPro showSceneText;

	public TextMeshPro puzzleRoomText;

	public Transform T5WithCutsceneTextContainerMobile;

	public MobileMenuController mobileMenu;

	public Canvas canvasOverlay;

	private new void Awake()
	{
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.LoadMap("taro0");
		}
		base.Awake();
	}

	private void Start()
	{
		GCS.enableCutsceneT5 = false;
		if (ADOBase.isMobileMenu)
		{
			base.enabled = false;
			ADOBase.controller.camy.enabled = false;
			ADOBase.controller.responsive = false;
			AddT5CutsceneVersionOnMobileMenu();
			canvasOverlay.gameObject.SetActive(value: false);
		}
		else
		{
			camera = scrCamera.instance;
			exitText.text = RDString.Get("neoCosmos.exitWorld");
			exitText.SetLocalizedFont();
			showSceneText.text = RDString.Get("neoCosmos.showCutscene");
			showSceneText.SetLocalizedFont();
			showSceneText.gameObject.SetActive(value: false);
			puzzleRoomText.text = RDString.Get("TP.title");
			puzzleRoomText.SetLocalizedFont();
		}
	}

	public void EnableT5Cutscene()
	{
		GCS.enableCutsceneT5 = true;
	}

	public void ShowSceneText()
	{
		showSceneText.gameObject.SetActive(value: true);
	}

	public void HideSceneText()
	{
		showSceneText.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenPlanet.transform.position;
		int num = Mathf.RoundToInt(position.y);
		int num2 = Mathf.RoundToInt(position.x);
		if (!ADOBase.controller.isCutscene)
		{
			if (num2 == 36)
			{
				camera.positionState = PositionState.NeoCosmosCredits;
			}
			else if ((num == 0 || num == 1) && num2 != 35)
			{
				camera.positionState = PositionState.TaroMenu0TopLane;
			}
			else if (num == -7)
			{
				camera.positionState = PositionState.TaroMenu0BottomLane;
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
	}

	private void AddT5CutsceneVersionOnMobileMenu()
	{
		if (Persistence.IsWorldComplete("T5") && !(T5WithCutsceneTextContainerMobile == null))
		{
			Transform transform = mobileMenu.sublevelBrowser.submenu["T5"].transform;
			Transform transform2 = Object.Instantiate(transform.GetChild(transform.childCount - 1), transform);
			T5WithCutsceneTextContainerMobile.transform.SetParent(transform2, worldPositionStays: false);
			T5WithCutsceneTextContainerMobile.gameObject.SetActive(value: true);
			transform2.GetComponent<Button>().onClick.AddListener(delegate
			{
				EnableT5Cutscene();
				MobileMenuController.EnterLevel("T5-X", speedTrial: false);
			});
		}
	}
}
