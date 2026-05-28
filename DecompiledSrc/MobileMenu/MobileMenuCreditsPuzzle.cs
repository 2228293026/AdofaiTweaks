using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuCreditsPuzzle : ADOBase
{
	private struct GlitchPhase
	{
		public Vector2 localPosition;

		public string groupId;

		public int screenIndex;

		public SfxSound sound;
	}

	public MobileMenuController menuController;

	public GlitchText glitch;

	private Transform grabbedPlanet;

	private int stage;

	private bool interactable;

	private SfxSound soundToPlay;

	private bool wasTouchingEdge;

	private readonly GlitchPhase[] glitchPhases = new GlitchPhase[3]
	{
		new GlitchPhase
		{
			localPosition = new Vector2(5f, 3f),
			groupId = "colorGroup",
			screenIndex = 0,
			sound = SfxSound.MobileMenuXC1
		},
		new GlitchPhase
		{
			localPosition = new Vector2(0f, 3f),
			groupId = "titleGroup",
			screenIndex = 0,
			sound = SfxSound.MobileMenuXC2
		},
		new GlitchPhase
		{
			localPosition = new Vector2(-4.5f, -1.75f),
			groupId = "creditsGroup",
			screenIndex = 0,
			sound = SfxSound.MobileMenuXC3
		}
	};

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		MobileMenuController mobileMenuController = menuController;
		mobileMenuController.onFinishLoading = (Action)Delegate.Combine(mobileMenuController.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		if (!GCS.FOOL_JOKER && !ADOBase.isExpo)
		{
			bool unlockedXC = Persistence.unlockedXC;
			if (Persistence.IsWorldComplete(ADOBase.worldData["XF"].index) && !unlockedXC)
			{
				base.gameObject.SetActive(value: true);
				MobileMenuGrabController grabController = menuController.grabController;
				grabController.onGrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onGrab, new Action<MobileMenuGrabbable>(OnGrab));
				grabController.onUngrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onUngrab, new Action<MobileMenuGrabbable>(OnUngrab));
				AdvanceGlitch();
			}
		}
	}

	private void Update()
	{
		if (!(grabbedPlanet != null) || !interactable)
		{
			return;
		}
		if (Mathf.RoundToInt(Time.timeSinceLevelLoad * 100f) % 4 == 0)
		{
			glitch.transform.rotation = Quaternion.Euler(Vector3.zero.WithZ(UnityEngine.Random.Range(-18f, -24f)));
		}
		if (Vector2.Distance(grabbedPlanet.transform.position, glitch.transform.position) < 1f)
		{
			AdvanceGlitch();
		}
		if (stage <= 1)
		{
			return;
		}
		Camera camobj = scrController.instance.camy.camobj;
		float num = camobj.orthographicSize * camobj.aspect;
		float num2 = grabbedPlanet.transform.position.x - camobj.transform.position.x;
		if (Mathf.Abs(num2) > num - 1f)
		{
			if (!wasTouchingEdge)
			{
				menuController.MoveInDirection((num2 < 0f) ? MoveDirection.Left : MoveDirection.Right);
			}
			wasTouchingEdge = true;
		}
		else
		{
			wasTouchingEdge = false;
		}
	}

	private void AdvanceGlitch()
	{
		Transform transform = glitch.transform;
		interactable = false;
		if (stage != 0)
		{
			MobileMenuController.PlayPuzzleSfx(soundToPlay);
		}
		if (stage < glitchPhases.Length)
		{
			GlitchPhase glitchPhase = glitchPhases[stage];
			MobileMenuScreen mobileMenuScreen = menuController.map.groupLUT[glitchPhase.groupId][glitchPhase.screenIndex];
			transform.parent = mobileMenuScreen.transform;
			TweenerCore<Vector3, Vector3, VectorOptions> t = transform.DOLocalMove(glitchPhase.localPosition, 0.5f).OnComplete(delegate
			{
				interactable = true;
			});
			if (stage == 0)
			{
				t.Complete();
			}
			soundToPlay = glitchPhase.sound;
			stage++;
		}
		else
		{
			glitch.longInterval = 0.05f;
			glitch.shortInterval = 0.05f;
			glitch.transform.DOScale(4f, 2f).SetEase(Ease.InCubic);
			glitch.transform.DOShakePosition(2f, 0.1f, 15, 90f, snapping: false, fadeOut: false);
			DOVirtual.DelayedCall(2f, UnlockXC);
		}
	}

	private void UnlockXC()
	{
		scrFlash.Flash();
		Persistence.unlockedXC = true;
		menuController.map.portalLUT["XC"].visible = true;
		menuController.map.Build(instantiate: true);
		glitch.gameObject.SetActive(value: false);
	}

	private void OnGrab(MobileMenuGrabbable obj)
	{
		if (obj is MobileMenuGrabbablePlanet && interactable)
		{
			grabbedPlanet = obj.transform;
			HighlightGlitch(highlight: true);
		}
	}

	private void OnUngrab(MobileMenuGrabbable obj)
	{
		grabbedPlanet = null;
		if (interactable)
		{
			HighlightGlitch(highlight: false);
		}
	}

	private void HighlightGlitch(bool highlight)
	{
		float num = (highlight ? 1.5f : 1f);
		glitch.transform.DOKill();
		glitch.transform.DOScale(Vector3.one * num, 0.5f).SetEase(Ease.OutExpo);
	}
}
