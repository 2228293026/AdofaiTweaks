using System;
using System.Collections.Generic;
using DG.Tweening;
using MobileMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene1 : TaroCutsceneScript
{
	private bool triggered;

	[Header("Cutscene 1 Specific Fields")]
	public List<scrFloor> triggerFloors;

	public GameObject cameraFocus;

	public GameObject cameraFocus2;

	public GameObject cover;

	public scrFloor spawnFloor;

	public MobileMenuController mobileMenu;

	private scrFloor hitFloor;

	public scrFloor t1;

	public scrFloor t2;

	public scrFloor t3;

	private Vector3 t1pos;

	private Vector3 t2pos;

	private Vector3 t3pos;

	public scrPortal t1Portal;

	public scrPortal t2Portal;

	public scrPortal t3Portal;

	private Dictionary<scrPortal, Vector3> portalPosition = new Dictionary<scrPortal, Vector3>();

	private Vector3 t2Ppos;

	private Vector3 t3Ppos;

	public MenuArrow arrowT1;

	public MenuArrow arrowT2;

	public MenuArrow arrowT3a;

	public MenuArrow arrowT3b;

	public MenuArrow arrowExit;

	public GameObject nameTileTemplate;

	private List<List<scrFloor>> nameEntryStuff = new List<List<scrFloor>>();

	private List<string> neLetters;

	public TextMeshPro nameEntryTitleText;

	public List<scrFloor> allFloors = new List<scrFloor>();

	public List<scrHoldRenderer> allHolds = new List<scrHoldRenderer>();

	private GameObject neContainer;

	private float floating = 0.08f;

	private bool droppedNameEntry;

	private Sequence keyboardSequence;

	private Sequence CharlieSeq;

	private Sequence OverSeq;

	private bool shouldLeave;

	private bool arrowsToLevels;

	private Dictionary<scrPortal, bool> shouldBeHidden = new Dictionary<scrPortal, bool>();

	private new void Awake()
	{
		base.Awake();
		t1pos = t1.transform.position;
		t2pos = t2.transform.position;
		t3pos = t3.transform.position;
		portalPosition[t1Portal] = t1Portal.transform.position;
		portalPosition[t2Portal] = t2Portal.transform.position;
		portalPosition[t3Portal] = t3Portal.transform.position;
		t1.isLandable = false;
		t2.isLandable = false;
		t3.isLandable = false;
		t1.transform.localScale = Vector3.zero;
		t2.transform.localScale = Vector3.zero;
		t3.transform.localScale = Vector3.zero;
		neLetters = new List<string>();
		for (int i = 0; i < 26; i++)
		{
			neLetters.Add(Convert.ToChar(i + 65).ToString() ?? "");
		}
		neLetters.Add("-");
		neLetters.Add("!");
		neLetters.Add("?");
		neLetters.Add("<color=#00FF00>OK</color>");
		neContainer = new GameObject("Name Entry Container");
		if (Persistence.taroStoryProgress == 0)
		{
			CreateNameEntry();
		}
		characters[0].SetState(2);
		for (int j = 0; j < 2; j++)
		{
			characters[j].render.enabled = false;
			characters[j].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
			characters[j].transform.DOLocalMoveY(-1f, 0f).SetRelative(isRelative: true);
		}
		runnables["Skippable"] = Skippable;
		runnables["Unskippable"] = Unskippable;
	}

	private void CreateNameEntry()
	{
		GameObject gameObject = neContainer;
		double num = scrController.instance.tileSize;
		int num2 = 11;
		int num3 = 5;
		_ = scrLevelMaker.instance;
		Transform transform = new GameObject().transform;
		transform.position = Vector3.right * 12f + Vector3.up * 17f;
		nameEntryTitleText.text = RDString.Get("neoCosmos.nameEntry");
		nameEntryTitleText.SetLocalizedFont();
		nameEntryTitleText.transform.localScale = Vector3.zero;
		nameEntryTitleText.transform.localPosition = Vector3.right * 12f + Vector3.right * 8f + Vector3.up * 18f;
		nameEntryTitleText.transform.parent = gameObject.transform;
		int num4 = 0;
		for (int i = 0; i < num2 * num3; i++)
		{
			int num5 = i % num2;
			int num6 = (int)Mathf.Floor((float)i / (float)num2);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(nameTileTemplate, transform.position + Vector3.right * 12f + Vector3.right * num5 * (float)num + Vector3.down * num6 * (float)num, default(Quaternion));
			gameObject2.name = "Name Entry-" + (i + 1) + "/Floor freeroam x" + num5 + " y" + num6;
			gameObject2.gameObject.transform.parent = gameObject.transform;
			scrFloor component = gameObject2.GetComponent<scrFloor>();
			component.freeroam = true;
			component.freeroamGenerated = true;
			component.freeroamRegion = 0;
			component.transform.localScale = Vector3.zero;
			if (num6 % 2 == 0 && num5 > 0)
			{
				gameObject2.GetComponentInChildren<Text>().text = neLetters[num4];
				num4++;
			}
			if (nameEntryStuff.Count == num6)
			{
				nameEntryStuff.Add(new List<scrFloor>());
			}
			nameEntryStuff[num6].Add(component);
		}
	}

	private void SkewNameEntry()
	{
		keyboardSequence?.Complete();
		Transform obj = neContainer.transform;
		obj.localPosition = new Vector2(4f, 6.75f);
		obj.rotation = Quaternion.Euler(0f, 0f, -7.5f);
		obj.localScale = Vector2.one * 0.75f;
		neContainer.AddComponent<scrGfxFloat>().amplitude = 0.25f;
		float duration = 1f;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				scrFloor obj2 = nameEntryStuff[i][j];
				SpriteRenderer legacyFloorSpriteRenderer = obj2.legacyFloorSpriteRenderer;
				Text componentInChildren = obj2.GetComponentInChildren<Text>();
				legacyFloorSpriteRenderer.DOFade(1f, duration).SetEase(Ease.OutSine);
				componentInChildren.DOFade(1f, duration).SetEase(Ease.OutSine);
			}
		}
	}

	public void DropNameEntry()
	{
		if (!droppedNameEntry)
		{
			droppedNameEntry = true;
			neContainer.transform.DORotate(Vector3.forward * -90f, 7f).SetEase(Ease.InSine);
		}
	}

	private void DisplayNameEntry()
	{
		keyboardSequence = DOTween.Sequence();
		keyboardSequence.Insert(0f, nameEntryTitleText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
		keyboardSequence.Insert(0f, nameEntryTitleText.transform.DOLocalMoveX(-8f, 1f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				scrFloor scrFloor2 = nameEntryStuff[i][j];
				float atPosition = (float)j * 0.05f;
				keyboardSequence.Insert(atPosition, scrFloor2.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
				keyboardSequence.Insert(atPosition, scrFloor2.transform.DOLocalMoveX(-12f, 1f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
			}
		}
		keyboardSequence.Insert(0.4f, neContainer.transform.DOLocalMoveX(-1f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		keyboardSequence.Insert(0.4f, characters[0].transform.DOLocalMoveX(-1f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		keyboardSequence.Insert(0.4f, DOTween.To(() => floating, delegate(float x)
		{
			floating = x;
		}, 0f, 0.5f).SetEase(Ease.OutCubic));
	}

	private void RemoveNameEntry()
	{
		keyboardSequence?.Complete();
		keyboardSequence = DOTween.Sequence();
		keyboardSequence.Insert(1f, nameEntryTitleText.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCubic));
		keyboardSequence.Insert(0f, nameEntryTitleText.transform.DOLocalMoveX(8f, 1f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		float duration = 1f;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				scrFloor scrFloor2 = nameEntryStuff[i][j];
				float atPosition = (float)(10 - j) * 0.05f;
				keyboardSequence.Insert(atPosition, scrFloor2.transform.DOMoveX(8f, duration).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
				SpriteRenderer legacyFloorSpriteRenderer = scrFloor2.legacyFloorSpriteRenderer;
				Text componentInChildren = scrFloor2.GetComponentInChildren<Text>();
				keyboardSequence.Insert(atPosition, legacyFloorSpriteRenderer.DOFade(0f, duration).SetEase(Ease.InSine));
				keyboardSequence.Insert(atPosition, componentInChildren.DOFade(0f, duration).SetEase(Ease.InSine));
			}
		}
		keyboardSequence.Insert(0f, neContainer.transform.DOLocalMoveX(1f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		keyboardSequence.Insert(0f, characters[0].transform.DOLocalMoveX(1f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		keyboardSequence.Insert(0f, DOTween.To(() => floating, delegate(float x)
		{
			floating = x;
		}, 0.08f, 0.5f).SetEase(Ease.OutCubic));
	}

	private void Charlie2On()
	{
		CharlieSeq.Kill(complete: true);
		OverSeq.Kill(complete: true);
		OverSeq = DOTween.Sequence().AppendInterval(0f).Append(characters[0].transform.DOLocalMoveX(-2f, 0.5f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true));
		CharFadeIn(characters[1], 0.5f);
		CharlieSeq = DOTween.Sequence().AppendInterval(0f).Append(characters[1].transform.DOLocalMoveX(1f, 0f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true))
			.Append(characters[1].transform.DOLocalMoveY(1f, 0.5f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true));
	}

	private void Charlie2On2()
	{
		CharlieSeq.Kill(complete: true);
		OverSeq.Kill(complete: true);
		OverSeq = DOTween.Sequence().AppendInterval(0f).Append(characters[0].transform.DOLocalMoveX(-2f, 0.5f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true));
		CharFadeIn(characters[1], 0.5f);
		CharlieSeq = DOTween.Sequence().AppendInterval(0f).Append(characters[1].transform.DOLocalMoveX(-1f, 0f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true))
			.Append(characters[1].transform.DOLocalMoveY(-1f, 0f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true))
			.Append(characters[1].transform.DOLocalMoveY(1f, 0.5f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true));
	}

	private void Charlie2Off()
	{
		CharlieSeq.Kill(complete: true);
		OverSeq.Kill(complete: true);
		OverSeq = DOTween.Sequence().AppendInterval(0f).Append(characters[0].transform.DOLocalMoveX(2f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
		CharFadeOut(characters[1], 0.5f);
		CharlieSeq = DOTween.Sequence().AppendInterval(0f).Append(characters[1].transform.DOLocalMoveX(2f, 0.5f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true));
	}

	private void Skippable()
	{
		canSkip = true;
	}

	private void Unskippable()
	{
		canSkip = false;
	}

	private void DestroyWorld()
	{
		GCS.banished = true;
		scrSfx.instance.PlaySfx(SfxSound.Banish, MixerGroup.SfxParent);
		foreach (scrHoldRenderer allHold in allHolds)
		{
			allHold.transform.position = Vector3.one * 9999f;
		}
		foreach (scrFloor allFloor in allFloors)
		{
			if (allFloor != spawnFloor)
			{
				float num = Vector3.Distance(allFloor.transform.position, spawnFloor.transform.position);
				float f = Mathf.Atan2(allFloor.transform.position.y - spawnFloor.transform.position.y, allFloor.transform.position.x - spawnFloor.transform.position.x);
				if (Mathf.Abs(allFloor.transform.position.x - spawnFloor.transform.position.x) < 1.1f)
				{
					DOTween.Sequence().AppendInterval(num * 0.1f).Append(allFloor.transform.DOLocalMoveY(-0.2f, 0.4f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
					DOTween.Sequence().AppendInterval(num * 0.1f).Append(allFloor.transform.DORotate(Vector3.forward * 90f, 0.4f).SetEase(Ease.InQuad));
					DOTween.Sequence().AppendInterval(num * 0.1f).Append(allFloor.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InQuad));
				}
				else
				{
					Vector3 vector = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
					DOTween.Sequence().AppendInterval(num * 0.1f).Append(allFloor.transform.DOLocalMove(vector * 0.4f, 0.3f).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
						.Append(allFloor.transform.DOLocalMove(vector * -0.4f, 0.7f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true));
				}
			}
		}
		scrFloor scrFloor2 = spawnFloor;
		DOTween.Sequence().Append(scrFloor2.transform.DOLocalMoveY(-0.2f, 0.3f).SetEase(Ease.OutCubic).SetRelative(isRelative: true)).Append(scrFloor2.transform.DOLocalMoveY(0.2f, 0.7f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true))
			.AppendInterval(1f)
			.Append(scrFloor2.transform.DOLocalMoveY(-30f, 3f).SetEase(Ease.InCubic).SetRelative(isRelative: true));
		DOTween.Sequence().AppendInterval(2f).Append(scrFloor2.transform.DORotate(Vector3.forward * 90f, 3f).SetEase(Ease.InQuad));
		DOTween.Sequence().Append(ADOBase.controller.chosenPlanet.transform.DOLocalMoveY(-0.4f, 0.3f).SetEase(Ease.OutCubic).SetRelative(isRelative: true)).Append(ADOBase.controller.chosenPlanet.transform.DOLocalMoveY(0.4f, 0.7f).SetEase(Ease.InOutCubic).SetRelative(isRelative: true))
			.AppendInterval(1f)
			.Append(ADOBase.controller.chosenPlanet.transform.DOLocalMoveY(-30f, 3f).SetEase(Ease.InCubic).SetRelative(isRelative: true));
		GameObject gameObject = new GameObject();
		DOTween.Sequence().AppendInterval(2f).Append(gameObject.transform.DOLocalMoveY(0f, 0f).OnComplete(delegate
		{
			ADOBase.controller.MoveCameraToObject(cameraFocus2, 3f, Ease.InCubic);
		}));
		DOTween.Sequence().AppendInterval(3f).Append(cover.transform.DOLocalMoveY(5f, 2f).SetEase(Ease.Linear));
	}

	private void ToMenu2()
	{
		GCS.sceneToLoad = "scnTaroMenu2";
		ADOBase.controller.StartLoadingScene();
	}

	private void GoToMobileMenu()
	{
		ADOBase.controller.responsive = false;
		ADOBase.controller.isCutscene = true;
		ADOBase.controller.camy.enabled = false;
		mobileMenu.Enable(enable: true);
		mobileMenu.JumpToScreen(mobileMenu.map.mapCenter, instant: false, 2f);
	}

	private void FinishCutscene1()
	{
		CharFadeOut(characters[0], 0.5f);
		characters[0].transform.DOLocalMoveY(-1f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
		if (ADOBase.isMobileMenu)
		{
			neContainer.gameObject.SetActive(value: false);
			GoToMobileMenu();
			return;
		}
		ADOBase.controller.MoveCameraToPlayer(1f, Ease.InOutCubic);
		if (shouldLeave)
		{
			ShowArrowsToExit();
		}
		if (arrowsToLevels)
		{
			ShowArrowsToLevels();
		}
		DOTween.Sequence().AppendInterval(0f).Append(t1.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
		DOTween.Sequence().AppendInterval(0f).Append(t2.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
		t1.isLandable = true;
		t2.isLandable = true;
		ShowPortal(t1Portal, 1f);
		ShowPortal(t2Portal, 1f);
		SkewNameEntry();
	}

	private void FinishCutscene2()
	{
		CharlieSeq.Kill(complete: true);
		OverSeq.Kill(complete: true);
		CharFadeOut(characters[0], 0.5f);
		OverSeq = DOTween.Sequence().Append(characters[0].transform.DOLocalMoveY(-1f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.InCubic));
		CharFadeOut(characters[1], 0.5f);
		CharlieSeq = DOTween.Sequence().Append(characters[1].transform.DOLocalMoveY(-1f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.InCubic));
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.map.SetMapCenter(mobileMenu.map.portalLUT["T3"]);
			GoToMobileMenu();
			return;
		}
		ShowArrowsToMawaru();
		ADOBase.controller.MoveCameraToPlayer(1f, Ease.InOutCubic);
		DOTween.Sequence().AppendInterval(0f).Append(t3.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
		t3.isLandable = true;
		ShowPortal(t1Portal, 1f);
		ShowPortal(t2Portal, 1f);
		ShowPortal(t3Portal, 1f);
	}

	private void FinishCutscene3()
	{
	}

	private void Scene1()
	{
		Persistence.taroStoryProgress = 1;
		Persistence.Save();
		OverSeq.Kill(complete: true);
		runnables["OnComplete"] = FinishCutscene1;
		runnables["NameEntryOn"] = DisplayNameEntry;
		runnables["NameEntryOff"] = RemoveNameEntry;
		ADOBase.controller.MoveCameraToObject(cameraFocus, 0.5f, Ease.Linear);
		ADOBase.controller.isCutscene = true;
		CharFadeIn(characters[0], 0.5f);
		OverSeq = DOTween.Sequence().Append(characters[0].transform.DOLocalMoveY(1f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		dialog.Add(RDString.Get("neoCosmosStory.1.1"));
		dialog.Add(RDString.Get("neoCosmosStory.1.2"));
		dialog.Add(RDString.Get("neoCosmosStory.1.3"));
		dialog.Add(RDString.Get("neoCosmosStory.1.4"));
		dialog.Add(RDString.Get("neoCosmosStory.1.5"));
		dialog.Add(RDString.Get("neoCosmosStory.1.6"));
		dialog.Add(RDString.Get("neoCosmosStory.1.7"));
		dialog.Add(RDString.Get("neoCosmosStory.1.8"));
		dialog.Add(RDString.Get("neoCosmosStory.1.9"));
		if (!Persistence.IsWorldComplete(5))
		{
			dialog.Add(RDString.Get("neoCosmosStory.1.b.1"));
			dialog.Add(RDString.Get("neoCosmosStory.1.b.2"));
			dialog.Add(RDString.Get("neoCosmosStory.1.b.3"));
			dialog.Add(RDString.Get("neoCosmosStory.1.b.4"));
			dialog.Add(RDString.Get("neoCosmosStory.1.b.5"));
			shouldLeave = true;
		}
		else
		{
			dialog.Add(RDString.Get("neoCosmosStory.1.a.1"));
			dialog.Add(RDString.Get("neoCosmosStory.1.a.2"));
			dialog.Add(RDString.Get("neoCosmosStory.1.a.3"));
			arrowsToLevels = true;
		}
		StartScene();
	}

	private void Scene2()
	{
		Persistence.taroStoryProgress = 2;
		runnables["OnComplete"] = FinishCutscene2;
		runnables["Charlie2On"] = Charlie2On;
		runnables["Charlie2On2"] = Charlie2On2;
		runnables["Charlie2Off"] = Charlie2Off;
		ADOBase.controller.MoveCameraToObject(cameraFocus, 0.5f, Ease.Linear);
		ADOBase.controller.isCutscene = true;
		characters[0].SetState(1);
		CharFadeIn(characters[0], 0.5f);
		OverSeq.Kill(complete: true);
		OverSeq = DOTween.Sequence().Append(characters[0].transform.DOLocalMoveY(1f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		dialog.Add(RDString.Get("neoCosmosStory.2.1"));
		dialog.Add(RDString.Get("neoCosmosStory.2.2"));
		dialog.Add(RDString.Get("neoCosmosStory.2.3"));
		dialog.Add(RDString.Get("neoCosmosStory.2.4"));
		dialog.Add(RDString.Get("neoCosmosStory.2.5"));
		dialog.Add(RDString.Get("neoCosmosStory.2.6"));
		dialog.Add(RDString.Get("neoCosmosStory.2.7"));
		dialog.Add(RDString.Get("neoCosmosStory.2.8"));
		dialog.Add(RDString.Get("neoCosmosStory.2.9"));
		dialog.Add(RDString.Get("neoCosmosStory.2.10"));
		dialog.Add(RDString.Get("neoCosmosStory.2.11"));
		dialog.Add(RDString.Get("neoCosmosStory.2.12"));
		dialog.Add(RDString.Get("neoCosmosStory.2.13"));
		dialog.Add(RDString.Get("neoCosmosStory.2.14"));
		dialog.Add(RDString.Get("neoCosmosStory.2.15"));
		dialog.Add(RDString.Get("neoCosmosStory.2.16"));
		dialog.Add(RDString.Get("neoCosmosStory.2.17"));
		dialog.Add(RDString.Get("neoCosmosStory.2.18"));
		curSwitch = 0;
		charaSwitch[4] = 1;
		charaSwitch[5] = 0;
		charaSwitch[12] = 1;
		charaSwitch[13] = 0;
		charaSwitch[14] = 1;
		charaSwitch[15] = 0;
	}

	private void Scene3()
	{
		if (Persistence.taroStoryProgress < 3)
		{
			Persistence.taroStoryProgress = 3;
		}
		runnables["DestroyWorld"] = DestroyWorld;
		runnables["Charlie2On"] = Charlie2On2;
		runnables["Charlie2Off"] = Charlie2Off;
		runnables["OnComplete"] = ToMenu2;
		ADOBase.controller.MoveCameraToObject(cameraFocus, 0.5f, Ease.Linear);
		OverSeq.Kill(complete: true);
		characters[0].SetState(2);
		CharFadeIn(characters[0], 0.5f);
		OverSeq = DOTween.Sequence().Append(characters[0].transform.DOLocalMoveY(1f, 0.5f).SetRelative(isRelative: true).SetEase(Ease.OutCubic));
		dialog.Add(RDString.Get("neoCosmosStory.3.1"));
		dialog.Add(RDString.Get("neoCosmosStory.3.2"));
		dialog.Add(RDString.Get("neoCosmosStory.3.3"));
		dialog.Add(RDString.Get("neoCosmosStory.3.4"));
		dialog.Add(RDString.Get("neoCosmosStory.3.5"));
		dialog.Add(RDString.Get("neoCosmosStory.3.6"));
		dialog.Add(RDString.Get("neoCosmosStory.3.7"));
		dialog.Add(RDString.Get("neoCosmosStory.3.8"));
		dialog.Add(RDString.Get("neoCosmosStory.3.9"));
		dialog.Add(RDString.Get("neoCosmosStory.3.10"));
	}

	private void ShowArrowsToExit()
	{
		arrowExit.FadeIn();
	}

	private void ShowArrowsToLevels()
	{
		arrowT1.FadeIn();
		arrowT2.FadeIn();
	}

	private void ShowArrowsToMawaru()
	{
		arrowT3a.FadeIn();
		arrowT3b.FadeIn();
	}

	private void Start()
	{
		HidePortal(t1Portal, 0f);
		HidePortal(t2Portal, 0f);
		HidePortal(t3Portal, 0f);
		t1.isLandable = true;
		if (!ADOBase.isMobileMenu)
		{
			if (Persistence.taroStoryProgress > 0)
			{
				DOTween.Sequence().AppendInterval(0f).Append(t1.transform.DOScale(Vector3.one, 0f).SetEase(Ease.OutCubic));
				DOTween.Sequence().AppendInterval(0f).Append(t2.transform.DOScale(Vector3.one, 0f).SetEase(Ease.OutCubic));
				t1.isLandable = true;
				t2.isLandable = true;
				ShowPortal(t1Portal, 0f);
				ShowPortal(t2Portal, 0f);
				MovePlanetsToCenter();
			}
			if (Persistence.taroStoryProgress > 1)
			{
				DOTween.Sequence().AppendInterval(0f).Append(t3.transform.DOScale(Vector3.one, 0f).SetEase(Ease.OutCubic));
				ShowPortal(t3Portal, 0f);
				t3.isLandable = true;
			}
		}
		if (Persistence.IsWorldComplete("T1") && Persistence.IsWorldComplete("T2") && Persistence.taroStoryProgress == 1)
		{
			if (ADOBase.isMobileMenu)
			{
				MovePlanetsToCenter();
			}
			ADOBase.controller.isCutscene = true;
			Scene2();
			HidePortal(t1Portal, 0f);
			HidePortal(t2Portal, 0f);
			HidePortal(t3Portal, 0f);
		}
		if (Persistence.IsWorldComplete("T3") && Persistence.taroStoryProgress > 1)
		{
			if (ADOBase.isMobileMenu)
			{
				MovePlanetsToCenter();
			}
			DOTween.Sequence().AppendInterval(0f).Append(t3.transform.DOScale(Vector3.zero, 0f).SetEase(Ease.OutCubic));
			ADOBase.controller.isCutscene = true;
			Scene3();
			HidePortal(t1Portal, 0f);
			HidePortal(t2Portal, 0f);
			HidePortal(t3Portal, 0f);
		}
		if (ADOBase.isMobileMenu)
		{
			bool flag = Persistence.taroStoryProgress == 0;
			bool flag2 = ADOBase.controller.isCutscene || flag;
			mobileMenu.Enable(!flag2, instant: true);
			if (!flag2)
			{
				ADOBase.controller.camy.enabled = false;
				mobileMenu.JumpToMenuEntrance();
			}
		}
		else if (!ADOBase.controller.isCutscene)
		{
			if (Persistence.taroStoryProgress == 1)
			{
				ShowArrowsToLevels();
			}
			if (Persistence.taroStoryProgress >= 1)
			{
				ShowPortal(t1Portal, 0f);
				ShowPortal(t2Portal, 0f);
			}
			if (Persistence.taroStoryProgress >= 2)
			{
				ShowArrowsToMawaru();
				ShowPortal(t3Portal, 0f);
			}
		}
	}

	private void MovePlanetsToCenter()
	{
		ADOBase.controller.chosenPlanet.transform.position = Vector3.right * 9f + Vector3.up * 11f;
		ADOBase.controller.camy.ViewObjectInstant(ADOBase.controller.chosenPlanet.transform);
	}

	private new void Update()
	{
		bool flag = false;
		if (shouldBeHidden[t1Portal])
		{
			HidePortal(t1Portal, 0f);
		}
		if (shouldBeHidden[t2Portal])
		{
			HidePortal(t2Portal, 0f);
		}
		if (shouldBeHidden[t3Portal])
		{
			HidePortal(t3Portal, 0f);
		}
		foreach (scrFloor triggerFloor in triggerFloors)
		{
			if (ADOBase.controller.chosenPlanet.currfloor == triggerFloor)
			{
				flag = true;
				hitFloor = triggerFloor;
			}
		}
		if (!triggered && Persistence.taroStoryProgress == 0 && flag)
		{
			triggered = true;
			Scene1();
		}
		base.transform.position = floating * Vector3.up * Mathf.Sin(Time.time * 0.5f * (float)Math.PI);
		if (ADOBase.controller.isCutscene)
		{
			if (charaSwitch.ContainsKey(curSwitch))
			{
				for (int i = 0; i < 2; i++)
				{
					if (charaSwitch[curSwitch] == i)
					{
						SetSpeaking(characters[i]);
					}
					else if (curSwitch != 5)
					{
						SetNotSpeaking(characters[i]);
					}
				}
			}
			if (curTextString == curSwitch + 1)
			{
				curSwitch++;
			}
		}
		if (ADOBase.controller.isCutscene)
		{
			base.Update();
		}
	}

	private void ShowPortal(scrPortal p, float dur)
	{
		shouldBeHidden[p] = false;
		p.hidden = false;
		p.transform.DOComplete();
		p.transform.DOLocalMove(portalPosition[p], dur).SetEase(Ease.OutCubic);
		DOTween.Sequence().AppendInterval(dur * 0.5f).Append(p.sprPortal.transform.DOScale(Vector3.one, dur).SetEase(Ease.InOutCubic));
		p.gameObject.SetActive(value: true);
	}

	private void HidePortal(scrPortal p, float dur)
	{
		shouldBeHidden[p] = true;
		p.hidden = true;
		p.transform.DOComplete();
		p.transform.DOLocalMove(portalPosition[p] + Vector3.up * 3f, dur).SetEase(Ease.OutCubic).OnComplete(delegate
		{
			p.gameObject.SetActive(value: false);
		});
		p.sprPortal.transform.DOScale(Vector3.zero, dur).SetEase(Ease.InOutCubic);
	}
}
