using TMPro;
using UnityEngine;

public class scnLevelSelectTaro : LevelSelectBase
{
	public scrCreditsText credits;

	public Cutscene8 scene;

	public TextMeshPro showSceneText;

	public scrCamera mainCamera;

	public GameObject bgAsset;

	private Vector3 bgOrigScale;

	private bool creditsJumped;

	private int lastKey = -1;

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
			base.enabled = false;
			return;
		}
		base.Awake();
		if (bgAsset != null)
		{
			bgOrigScale = bgAsset.transform.localScale;
		}
	}

	private void Start()
	{
		mainCamera.setCamSizeInstant(mainCamera.camsizenormal);
		JumpToWorldPortal(GCS.worldEntrance, instant: true);
		scene.creditsContent = credits.content;
		scene.creditsContentCopy = credits.contentCopy;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
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
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			JumpToPosition(new Vector2(36f, 0f), instant: false);
		}
	}

	private void LateUpdate()
	{
		UpdateCameraOrthoSize();
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
		if (bgAsset != null)
		{
			bgAsset.transform.localScale = bgOrigScale * (tosize / 5f);
		}
	}

	public void JumpWithKey(int key)
	{
		if (ADOBase.isMobileMenu || ADOBase.controller.playerOne.holding || ADOBase.controller.isCutscene)
		{
			return;
		}
		if (showSceneText != null)
		{
			showSceneText.gameObject.SetActive(value: false);
		}
		if (chosenplanet.transform.position.y > -3.5f)
		{
			if (lastKey == key && key != 5)
			{
				JumpToWorldPortal($"T{key}EX");
			}
			else
			{
				JumpToWorldPortal($"T{key}");
			}
		}
		else if (lastKey == key || key == 5)
		{
			JumpToWorldPortal($"T{key}");
		}
		else if (key != 5)
		{
			JumpToWorldPortal($"T{key}EX");
		}
		lastKey = key;
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
			return;
		}
		Vector2 position = Vector2.zero;
		if (world != null && scrPortal.portals.ContainsKey(world) && scrPortal.portals[world] != null)
		{
			position = scrPortal.portals[world].jumpPosition;
		}
		if (Persistence.taroStoryProgress == 6 && !creditsJumped)
		{
			creditsJumped = true;
			position = new Vector2(36f, 0f);
		}
		JumpToPosition(position, instant);
	}

	private void JumpToPosition(Vector2 position, bool instant)
	{
		chosenplanet.transform.MoveXY(position);
		base.menuPhase = 1;
		if (instant)
		{
			camy.ViewObjectInstant(chosenplanet.transform, includeOffset: true);
		}
		else
		{
			camy.Refocus(chosenplanet.transform);
		}
		camy.isMoveTweening = true;
	}
}
