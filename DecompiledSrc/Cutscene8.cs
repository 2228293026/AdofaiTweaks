using System;
using System.Collections.Generic;
using DG.Tweening;
using MobileMenu;
using TMPro;
using UnityEngine;

public class Cutscene8 : TaroCutsceneScript
{
	private List<Transform> charAdd = new List<Transform>();

	private List<Vector3> charStartPos = new List<Vector3>();

	public TextMeshPro exitText;

	public Crack crack;

	public scrFloor triggerFloor;

	private bool triggered;

	public GameObject scene8Marker;

	public MobileMenuController mobileMenu;

	public MobileMenuTaroRiftScreen mobileCrack;

	private MobileMenuMap originalMap;

	private int exWorldsComplete;

	public RectTransform creditsContent;

	public RectTransform creditsContentCopy;

	private bool animating;

	private float floating = 0.08f;

	private new void Awake()
	{
		base.Awake();
		characters[0].SetState(0);
		characters[0].render.enabled = false;
		characters[0].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		runnables["ShowCrack"] = ShowCrack;
		runnables["BackFromCrack"] = BackFromCrack;
		runnables["CrackState0"] = CrackState0;
		runnables["CrackState1"] = CrackState1;
		runnables["CrackState2"] = CrackState2;
		runnables["CrackState3"] = CrackState3;
		runnables["EndScene"] = FinishCrackScene;
		charAdd.Add(new GameObject().transform);
		charAdd[0].position = Vector3.down;
		charStartPos.Add(characters[0].transform.localPosition);
	}

	private void Start()
	{
		exWorldsComplete = 0;
		if (Persistence.IsWorldComplete("T1EX"))
		{
			exWorldsComplete++;
		}
		if (Persistence.IsWorldComplete("T2EX"))
		{
			exWorldsComplete++;
		}
		if (Persistence.IsWorldComplete("T3EX"))
		{
			exWorldsComplete++;
		}
		if (Persistence.IsWorldComplete("T4EX"))
		{
			exWorldsComplete++;
		}
		int num = (RDC.forceUnlockAllLevels ? 4 : Persistence.taroEXProgress);
		bool flag = exWorldsComplete > num;
		if (ADOBase.isMobileMenu)
		{
			if (exWorldsComplete > 0)
			{
				mobileCrack = mobileMenu.map.groupLUT["riftGroup"][0] as MobileMenuTaroRiftScreen;
				crack = mobileCrack.crack;
				if (flag)
				{
					Timer.Add(delegate
					{
						mobileMenu.Enable(enable: false);
					}, 0.2f);
				}
			}
			if (Persistence.taroStoryProgress == 6)
			{
				originalMap = mobileMenu.map;
				Transform transform = new GameObject().transform;
				transform.position = originalMap.portalLUT["T1EX"].transform.position + Vector3.down * originalMap.rootGroup.GetHeight() * 0.75f;
				mobileMenu.mapContainer = transform;
				mobileMenu.LoadMap("taro0ending");
				MobileMenuScreen screen = mobileMenu.map.rootGroup[0];
				MobileMenuScreen mobileMenuScreen = mobileMenu.map.rootGroup[1];
				mobileMenu.map.SetMapCenter(mobileMenuScreen);
				mobileMenu.JumpToScreen(screen, instant: true);
				scene8Marker.transform.position = mobileMenuScreen.transform.position;
			}
			else
			{
				mobileMenu.JumpToMenuEntrance();
			}
		}
		if (num == 0)
		{
			crack.gameObject.SetActive(value: false);
		}
		if (num > 0)
		{
			crack.SetState(Persistence.taroEXProgress - 1);
		}
		if (num == 4)
		{
			crack.FadeText(0f);
		}
		if (flag)
		{
			CrackScene(exWorldsComplete - 1);
		}
		Persistence.taroEXProgress = exWorldsComplete;
		Persistence.Save();
	}

	private void PlayCutscene()
	{
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.Enable(enable: false);
		}
		ADOBase.controller.isCutscene = true;
		scene_ended = false;
		if (!ADOBase.isMobileMenu)
		{
			ADOBase.controller.MoveCameraToObject(scene8Marker, 1f, Ease.Linear);
		}
		exitText.transform.DOScale(0f, 0f);
		runnables["Scene2"] = Scene2;
		runnables["OnComplete"] = FinishCutscene;
		dialog.Add(RDString.Get("neoCosmosStory.4.1"));
		dialog.Add(RDString.Get("neoCosmosStory.4.2"));
		dialog.Add(RDString.Get("neoCosmosStory.4.3"));
		dialog.Add(RDString.Get("neoCosmosStory.4.4"));
		dialog.Add(RDString.Get("neoCosmosStory.4.5"));
		dialog.Add(RDString.Get("neoCosmosStory.4.6"));
		dialog.Add(RDString.Get("neoCosmosStory.4.7"));
		dialog.Add(RDString.Get("neoCosmosStory.4.8"));
		dialog.Add(RDString.Get("neoCosmosStory.4.9"));
		dialog.Add(RDString.Get("neoCosmosStory.4.10"));
		dialog.Add(RDString.Get("neoCosmosStory.4.11"));
		dialog.Add(RDString.Get("neoCosmosStory.4.12"));
		dialog.Add(RDString.Get("neoCosmosStory.4.13"));
		dialog.Add(RDString.Get("neoCosmosStory.4.14"));
		dialog.Add(RDString.Get("neoCosmosStory.4.15"));
		StartScene();
	}

	private void Scene2()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(9999f, 0f).SetRelative(isRelative: true);
		Persistence.taroStoryProgress = 7;
		Persistence.Save();
		characters[0].transform.parent.gameObject.SetActive(value: true);
		CharFadeIn(characters[0], 0.5f);
		charAdd[0].DOMoveY(0f, 0.5f).SetEase(Ease.OutCubic);
		if (!ADOBase.isMobileMenu)
		{
			creditsContent.transform.DOLocalMoveX(1200f, 1f).SetEase(Ease.Linear);
			creditsContentCopy.transform.DOLocalMoveX(1200f, 1f).SetEase(Ease.Linear);
		}
	}

	private void FinishCutscene()
	{
		ADOBase.controller.MoveCameraToPlayer(1f, Ease.Linear);
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(-9999f, 0f).SetRelative(isRelative: true);
		CharFadeOut(characters[0], 0.5f);
		charAdd[0].DOMoveY(-1f, 0.5f).SetEase(Ease.InCubic);
		exitText.transform.DOScale(1f, 0f);
		if (!ADOBase.isMobileMenu)
		{
			creditsContent.transform.DOLocalMoveX(0f, 1f).SetEase(Ease.Linear);
			creditsContentCopy.transform.DOLocalMoveX(0f, 1f).SetEase(Ease.Linear);
		}
		else
		{
			mobileMenu.map = originalMap;
			mobileMenu.JumpToScreen(originalMap.portalLUT["T1EX"], instant: false, 2f);
			mobileMenu.Enable(enable: true);
		}
	}

	private void FinishCrackScene()
	{
		if (ADOBase.isMobileMenu)
		{
			mobileMenu.Enable(enable: true);
		}
		AdvanceText();
		crack.Localize();
	}

	private void ShowCrack()
	{
		_ = ADOBase.isMobileMenu;
		ADOBase.controller.MoveCameraToObject(crack.gameObject, 1f, Ease.InOutCubic);
	}

	private void CrackState0()
	{
		crack.DoCrack(0);
	}

	private void CrackState1()
	{
		crack.DoCrack(1);
	}

	private void CrackState2()
	{
		crack.DoCrack(2);
	}

	private void CrackState3()
	{
		crack.DoCrack(3);
	}

	private void BackFromCrack()
	{
		if (ADOBase.isMobileMenu)
		{
			ADOBase.controller.MoveCameraToObject(mobileMenu.currentScreen.transform.gameObject, 1f, Ease.InOutCubic);
		}
		else
		{
			ADOBase.controller.MoveCameraToPlayer(1f, Ease.InOutCubic);
		}
	}

	private void CrackScene(int val)
	{
		runnables["OnComplete"] = delegate
		{
		};
		ADOBase.controller.isCutscene = true;
		displayBox = false;
		canSkip = false;
		canAdvance = false;
		bool flag = RDString.language == SystemLanguage.English;
		float num = ((val == 3 && flag) ? 3f : 1f);
		dialog.Add($"..`p,.5;..`f,ShowCrack;..`p,2;..`f,CrackState{val};..`p,{num};..`f,BackFromCrack;..`p,0.1;..`f,EndScene;");
		crack.tbcText.text = "";
		StartScene();
	}

	private new void Update()
	{
		if (!triggered && Persistence.taroStoryProgress == 6 && ((ADOBase.isMobileMenu && mobileMenu.selectedScreenIndex == 1) || ADOBase.controller.chosenPlanet.currfloor == triggerFloor))
		{
			triggered = true;
			PlayCutscene();
		}
		if (animating)
		{
			characters[0].transform.localPosition = charStartPos[0] + floating * Vector3.up * Mathf.Sin(Time.time * 0.5f * (float)Math.PI) + charAdd[0].position;
		}
		base.Update();
	}
}
