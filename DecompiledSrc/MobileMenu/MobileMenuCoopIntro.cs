using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuCoopIntro : ADOBase
{
	public static bool didCoopTutorial;

	public MobileMenuController menuController;

	[Header("Tutorial")]
	public SpriteRenderer backgroundFader;

	public GameObject instructions;

	public AudioClip tutorialCompleteSound;

	public Transform[] tracks;

	public Transform[] controlIconContainers;

	[NonSerialized]
	public ControllerIcon[] controlIcons;

	[Header("Ready Area")]
	public CoopPainter painter;

	public Transform readyArea;

	public Transform[] readyAreaRows;

	public Transform[] startFloors;

	public Transform[] confirmFloors;

	public Text[] confirmTexts;

	public AudioClip[] mergeAnimationSounds;

	private scrPlayer[] confirmationSlots = new scrPlayer[4];

	private int playerCount;

	private float tutorialCompleteSoundCooldown;

	private float readyTextSoundCooldown;

	private int playersReady;

	private static bool isCoop => scrPlayerManager.playerCount > 1;

	private static bool showReadyArea => isCoop;

	public static IntroType GetIntroType()
	{
		if (!ADOBase.isExpo)
		{
			if (!isCoop)
			{
				if (!Persistence.passedMobileMenuTutorial)
				{
					return IntroType.Tutorial;
				}
				return IntroType.NoIntro;
			}
			if (!didCoopTutorial)
			{
				return IntroType.Both;
			}
			if (!showReadyArea)
			{
				return IntroType.Tutorial;
			}
			return IntroType.ColorSelect;
		}
		return IntroType.NoIntro;
	}

	private void Awake()
	{
		Transform[] array = tracks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		readyArea.gameObject.SetActive(value: false);
		instructions.SetActive(value: false);
	}

	private void Update()
	{
		TutorialUpdate();
		ReadyUpdate();
		if (tutorialCompleteSoundCooldown > 0f)
		{
			tutorialCompleteSoundCooldown -= Time.deltaTime;
		}
		if (readyTextSoundCooldown > 0f)
		{
			readyTextSoundCooldown -= Time.deltaTime;
		}
	}

	public void Run()
	{
		playerCount = scrPlayerManager.playerCount;
		base.gameObject.SetActive(value: true);
		ADOBase.controller.responsive = true;
		menuController.Enable(!base.enabled);
		menuController.enabled = !base.enabled;
		HideAllButTitleScreen(base.enabled);
		scrLogoText.instance.Enable(enabled: true);
		PrepareFloors();
		painter.Init();
		controlIcons = new ControllerIcon[playerCount];
		for (int i = 0; i < playerCount; i++)
		{
			ControllerIcon controllerIcon = ControllerIcon.Create(i);
			if (!(controllerIcon == null))
			{
				controllerIcon.transform.ScaleXY(0.01f);
				controllerIcon.transform.SetParent(controlIconContainers[i], worldPositionStays: false);
				if (playerCount == 1)
				{
					controllerIcon.border.enabled = false;
				}
				else
				{
					Color color = ADOBase.playerManager.players[i].planetarySystem.chosenPlanet.planetRenderer.planetColor.ToRealColor();
					controllerIcon.border.color = (color * 0.85f).WithAlpha(0.5f);
				}
				controlIcons[i] = controllerIcon;
			}
		}
		ADOBase.controller.independentPlayers = true;
		foreach (scrPlayer player in ADOBase.controller.playerManager)
		{
			foreach (scrPlanet allPlanet in player.planetarySystem.allPlanets)
			{
				allPlanet.planetScale = 1f;
			}
			scrPlayer obj = player;
			obj.onHit = (Action<scrFloor>)Delegate.Combine(obj.onHit, (Action<scrFloor>)delegate(scrFloor floor)
			{
				OnHit(player, floor);
			});
		}
		float orthographicSize = ((playerCount >= 3) ? 5.5f : 4.5f);
		ADOBase.controller.camy.camobj.orthographicSize = orthographicSize;
		switch (GetIntroType())
		{
		case IntroType.Tutorial:
		case IntroType.Both:
			ToggleTutorial(enabled: true);
			break;
		case IntroType.ColorSelect:
			ToggleReadyArea(enabled: true);
			break;
		}
		ADOBase.conductor.song3.volume = 0f;
	}

	private void HideAllButTitleScreen(bool hide)
	{
		MobileMenuGroup rootGroup = menuController.map.rootGroup;
		foreach (MobileMenuGroup value in menuController.map.groupLUT.Values)
		{
			if (value.visibleScreens == null || value == rootGroup)
			{
				continue;
			}
			foreach (MobileMenuScreen item in value)
			{
				item.transform?.gameObject.SetActive(!hide);
			}
		}
	}

	private void PrepareFloors()
	{
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		readyArea.gameObject.SetActive(showReadyArea);
		int num = playerCount * 2 - 1;
		for (int i = 0; i < tracks.Length; i++)
		{
			Transform transform = tracks[i];
			bool flag = i < playerCount;
			transform.gameObject.SetActive(flag);
			startFloors[i].gameObject.SetActive(flag);
			confirmFloors[i].gameObject.SetActive(flag);
			if (flag)
			{
				if (!showReadyArea)
				{
					Transform child = transform.GetChild(transform.childCount - 1);
					transform.transform.MoveX(0f - child.transform.localPosition.x);
				}
				float y = (float)(-i * 2) + (float)num / 2f - 0.5f;
				transform.LocalMoveY(y);
				startFloors[i].LocalMoveY(y);
				confirmFloors[i].LocalMoveY(y);
			}
		}
		Transform transform2 = tracks[0].GetChild(0).transform;
		instructions.transform.position = transform2.position + Vector3.up * 1.8f;
		if (playerCount == 1)
		{
			Color color = new Color(0.78039217f, 0.78039217f, 0.8862745f, 1f);
			foreach (Transform item in tracks[0])
			{
				item.GetComponent<SpriteRenderer>().color = color;
			}
		}
		if (playerCount == 2)
		{
			int num2 = readyAreaRows.Length / 2;
			int num3 = 1;
			Transform[] array = readyAreaRows;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].gameObject.SetActive(value: false);
			}
			readyAreaRows = readyAreaRows[Range.EndAt(Index.op_Implicit(num2 - num3))].Union(readyAreaRows[Range.StartAt(Index.op_Implicit(num2 + num3))]).ToArray();
			int num4 = readyAreaRows.Length;
			for (int k = 0; k < readyAreaRows.Length; k++)
			{
				float y2 = (float)(-k) + (float)num4 / 2f - 0.5f;
				readyAreaRows[k].LocalMoveY(y2);
				readyAreaRows[k].gameObject.SetActive(value: true);
			}
		}
	}

	private void ToggleTutorial(bool enabled)
	{
		backgroundFader.gameObject.SetActive(enabled);
		instructions.SetActive(enabled);
		if (enabled)
		{
			ADOBase.conductor.song2.volume = 0f;
			scnMobileMenu.introPhase = IntroPhase.Tutorial;
			for (int i = 0; i < playerCount; i++)
			{
				scrPlayer obj = ADOBase.controller.playerManager.players[i];
				Transform child = tracks[i].GetChild(0);
				obj.planetarySystem.chosenPlanet.currfloor = child.GetComponent<scrFloor>();
				obj.planetarySystem.chosenPlanet.transform.transform.position = child.position;
			}
			Transform transform = tracks[0].GetChild(0).transform;
			ADOBase.controller.camy.transform.position = new Vector3(transform.position.x, base.transform.position.y, ADOBase.controller.camy.transform.position.z);
		}
	}

	private void DoCompleteTutorial(int playerIndex)
	{
		scrPlayer player = ADOBase.controller.playerManager.players[playerIndex];
		player.alive = false;
		Sequence s = DOTween.Sequence();
		Transform transform = tracks[playerIndex];
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			s.Insert(1.5f / (float)transform.childCount * (float)i, child.DOScale(0f, 0.25f).SetEase(Ease.OutSine));
		}
		if (tutorialCompleteSoundCooldown <= 0f)
		{
			scrSfx.instance.PlaySfx(tutorialCompleteSound, MixerGroup.SfxParent);
			tutorialCompleteSoundCooldown = 0.15f;
		}
		if (showReadyArea)
		{
			scrPlanet planet = player.planetarySystem.chosenPlanet;
			player.planetarySystem.transform.SetParent(planet.currfloor.transform);
			DOTween.To(() => planet.cosmeticRadius, delegate(float x)
			{
				planet.cosmeticRadius = x;
			}, 0f, 0.5f);
			planet.planetRenderer.ringComp.DOFade(0f, 0.5f);
		}
		bool flag = true;
		foreach (scrPlayer item in ADOBase.controller.playerManager)
		{
			if (item.alive)
			{
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		if (isCoop)
		{
			didCoopTutorial = true;
		}
		if (!Persistence.passedMobileMenuTutorial)
		{
			Persistence.passedMobileMenuTutorial = true;
			Persistence.Save();
		}
		Timer.Add(delegate
		{
			ToggleTutorial(enabled: false);
			if (showReadyArea)
			{
				ToggleReadyArea(enabled: true, instant: false);
			}
			else
			{
				confirmationSlots[0] = player;
				DoEveryoneReady();
			}
		}, 1.5f);
	}

	private void ToggleReadyArea(bool enabled, bool instant = true)
	{
		float duration = (instant ? 0f : 1f);
		Transform transform = ADOBase.controller.camy.transform;
		transform.DOMove(readyArea.position.WithZ(transform.position.z), duration).SetEase(Ease.OutSine).Done();
		if (!enabled)
		{
			return;
		}
		ADOBase.conductor.song2.DOFade(1f, 0.5f);
		scnMobileMenu.introPhase = IntroPhase.ColorSelect;
		for (int i = 0; i < playerCount; i++)
		{
			scrPlayer scrPlayer2 = ADOBase.controller.playerManager.players[i];
			Transform transform2 = startFloors[i];
			scrPlanet planet = scrPlayer2.planetarySystem.chosenPlanet;
			scrPlayer2.planetarySystem.transform.SetParent(planet.player.transform, worldPositionStays: false);
			planet.transform.position = transform2.position;
			planet.currfloor = transform2.GetComponent<scrFloor>();
			planet.planetRenderer.ClearParticles();
			planet.other.planetRenderer.ClearParticles();
			scrPlayer2.alive = true;
			confirmTexts[i].gameObject.SetActive(value: false);
			DOTween.To(() => planet.cosmeticRadius, delegate(float x)
			{
				planet.cosmeticRadius = x;
			}, 1f, 0.5f);
			planet.planetRenderer.ringComp.DOFade(planet.other.planetRenderer.ringComp.color.a, 0.5f);
		}
	}

	private void TutorialUpdate()
	{
		if (scnMobileMenu.introPhase != IntroPhase.Tutorial)
		{
			return;
		}
		Transform obj = tracks[0];
		float x = obj.position.x;
		float x2 = obj.GetChild(obj.childCount - 1).position.x;
		float num = x2;
		float num2 = 0f;
		for (int i = 0; i < playerCount; i++)
		{
			scrPlayer scrPlayer2 = ADOBase.controller.playerManager.players[i];
			if (scrPlayer2.alive)
			{
				float x3 = scrPlayer2.planetarySystem.chosenPlanet.currfloor.transform.position.x;
				num = Mathf.Min(num, x3);
				num2 += x3;
				if (x3 == x2)
				{
					DoCompleteTutorial(i);
				}
			}
		}
		scrCamera camy = ADOBase.controller.camy;
		float x4 = Mathf.Lerp(camy.transform.position.x, num, 4f * Time.deltaTime);
		camy.transform.position = camy.transform.position.WithX(x4);
		float value = num2 / (float)playerCount;
		float num3 = Mathf.InverseLerp(x, x2, value);
		backgroundFader.color = backgroundFader.color.WithAlpha(1f - num3);
		ADOBase.conductor.song2.volume = num3;
		if (!showReadyArea)
		{
			ADOBase.conductor.song3.volume = num3;
		}
		if (ADOBase.conductor.onBeatHappened)
		{
			for (int j = 0; j < playerCount; j++)
			{
				controlIcons[j].Tap((float)ADOBase.conductor.crotchetAtStart / 4f);
			}
		}
	}

	private void ReadyUpdate()
	{
		_ = scnMobileMenu.introPhase;
		_ = 2;
	}

	private void OnHit(scrPlayer player, scrFloor floor)
	{
		if (scnMobileMenu.introPhase != IntroPhase.ColorSelect)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < playerCount; i++)
		{
			if (floor.transform == confirmFloors[i])
			{
				Ready(player, i);
			}
			else if (player == confirmationSlots[i])
			{
				Unready(i);
			}
			if (confirmationSlots[i] == null)
			{
				flag = false;
			}
		}
		if (flag)
		{
			Timer.Add(delegate
			{
				DoEveryoneReady();
			}, 0.6f);
		}
	}

	private void Ready(scrPlayer player, int slot)
	{
		playersReady++;
		confirmationSlots[slot] = player;
		Text obj = confirmTexts[slot];
		obj.gameObject.SetActive(value: true);
		obj.transform.parent.DOKill();
		obj.transform.parent.DOScale(1f, 0.5f).SetEase(Ease.OutBack).From(0f);
		obj.color = player.planetarySystem.chosenPlanet.planetRenderer.planetColor.ToRealColor();
		if (readyTextSoundCooldown <= 0f)
		{
			scrSfx.instance.PlaySfx(SfxSound.NotificationTinyText, MixerGroup.InterfaceParent);
			readyTextSoundCooldown = 0.1f;
		}
		ADOBase.conductor.song3.DOKill();
		ADOBase.conductor.song3.DOFade((float)playersReady / (float)playerCount, 0.25f);
	}

	private void Unready(int slot)
	{
		playersReady--;
		confirmationSlots[slot] = null;
		confirmTexts[slot].gameObject.SetActive(value: false);
		ADOBase.conductor.song3.DOKill();
		ADOBase.conductor.song3.DOFade((float)playersReady / (float)playerCount, 0.25f);
	}

	private void DoEveryoneReady()
	{
		scnMobileMenu.introPhase = IntroPhase.Finished;
		GCS.worldEntrance = null;
		ADOBase.controller.responsive = false;
		for (int i = 0; i < playerCount; i++)
		{
			scrPlayer scrPlayer2 = confirmationSlots[i];
			scrPlayerManager.playerOrder[scrPlayer2.playerID] = i;
		}
		float duration = ((playerCount > 1) ? 1.2f : 0.6f);
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			componentsInChildren[j].DOFade(0f, 0.25f);
		}
		Text[] array = confirmTexts;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].DOFade(0f, 0.25f);
		}
		array = painter.colorTexts;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].DOFade(0f, 0.25f);
		}
		Camera camobj = ADOBase.controller.camy.camobj;
		for (int k = 0; k < playerCount; k++)
		{
			scrPlayer scrPlayer3 = confirmationSlots[k];
			PlanetarySystem planetarySystem = scrPlayer3.planetarySystem;
			scrPlayer3.transform.MoveZ(-k);
			Transform transform = planetarySystem.chosenPlanet.transform;
			Vector3 position = camobj.transform.position;
			transform.DOMove(position.WithZ(transform.position.z), duration).SetEase(Ease.InOutSine);
			planetarySystem.chosenPlanet.planetRenderer.ringComp.DOFade(0f, 0.25f);
			DOTween.To(() => planetarySystem.planetRed.planetScale, delegate(float x)
			{
				planetarySystem.planetRed.planetScale = x;
			}, 1f - (float)k * 0.25f, duration).SetEase(Ease.InOutSine);
			DOTween.To(() => planetarySystem.planetBlue.planetScale, delegate(float x)
			{
				planetarySystem.planetBlue.planetScale = x;
			}, 1f - (float)k * 0.25f, duration).SetEase(Ease.InOutSine);
			planetarySystem.chosenPlanet.planetRenderer.StopParticles();
			planetarySystem.chosenPlanet.other.planetRenderer.StopParticles();
		}
		scrSfx.instance.PlaySfx(mergeAnimationSounds[0], MixerGroup.SfxParent);
		float mergePauseDuration = ((playerCount > 1) ? 0.6f : 0f);
		DOZoom(4f, duration).OnComplete(delegate
		{
			if (playerCount > 1)
			{
				scrSfx.instance.PlaySfx(mergeAnimationSounds[1], MixerGroup.SfxParent);
				scrFlash.Flash();
			}
			Timer.Add(delegate
			{
				foreach (scrPlayer player in ADOBase.controller.playerManager)
				{
					player.planetarySystem.chosenPlanet.transform.DOMoveY(-10f, 0.6f).SetRelative(isRelative: true).SetEase(Ease.InBack)
						.OnComplete(delegate
						{
							player.gameObject.SetActive(value: false);
						});
				}
				scrSfx.instance.PlaySfx(mergeAnimationSounds[2], MixerGroup.SfxParent);
			}, mergePauseDuration);
			Timer.Add(delegate
			{
				if (scnMobileMenu.returnToLevelAfterIntroFinished)
				{
					scnMobileMenu.returnToLevelAfterIntroFinished = false;
					MobileMenuController.EnterLevel(Persistence.savedCurrentLevel, GCS.speedTrialMode);
				}
				else
				{
					menuController.Enable(enable: true);
					menuController.enabled = true;
					menuController.JumpToMenuEntrance();
					Timer.Add(delegate
					{
						HideAllButTitleScreen(hide: false);
					}, 0.25f);
				}
			}, mergePauseDuration + 0.48000002f);
		});
	}

	private Tween DOZoom(float zoom, float duration)
	{
		return DOTween.To(() => ADOBase.controller.camy.camobj.orthographicSize, delegate(float x)
		{
			ADOBase.controller.camy.camobj.orthographicSize = x;
		}, zoom, duration);
	}
}
