using MobileMenu;
using TMPro;
using UnityEngine;

public class scnTaroMenu1 : LevelSelectBase
{
	private scrCamera camera;

	public GameObject menuExit;

	public TextMeshPro exitText;

	public MobileMenuController mobileMenu;

	protected override void Awake()
	{
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.LoadMap("taro1");
			MobileMenuPortalScreen mapCenter = mobileMenu.map.portalLUT["T1"];
			mobileMenu.map.SetMapCenter(mapCenter);
		}
		base.Awake();
	}

	private void Start()
	{
		camera = scrCamera.instance;
		if (ADOBase.isMobileMenu)
		{
			menuExit.SetActive(value: false);
			return;
		}
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
	}

	private void Update()
	{
		int num = Mathf.RoundToInt(ADOBase.controller.chosenPlanet.transform.position.y);
		if (!ADOBase.controller.isCutscene)
		{
			if (num == 12)
			{
				camera.positionState = PositionState.TaroMenu1TopLane;
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
}
