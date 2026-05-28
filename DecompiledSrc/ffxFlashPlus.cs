using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class ffxFlashPlus : ffxPlusBase
{
	private const int fgStartLayer = 0;

	private const int bgStartLayer = 10;

	public Color startColor = Color.white;

	public Color endColor = Color.clear;

	public bool FG;

	public static bool legacyFlash;

	private Renderer flashRenderer;

	private Material material;

	private static Tween fgFlashTween;

	private static Tween bgFlashTween;

	protected override IEnumerable<Tween> eventTweens => new Tween[1] { FG ? fgFlashTween : bgFlashTween };

	public override void Awake()
	{
		base.Awake();
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualEffects != VisualEffects.Minimum)
		{
			AdjustDurationForHardbake();
			if (material == null)
			{
				FlashSetup();
			}
			DoFlash();
		}
	}

	public void FlashSetup()
	{
		if (!legacyFlash)
		{
			flashRenderer = (FG ? cam.flashPlusRendererFg : cam.flashPlusRendererBg);
			UpdateFlashLayer();
		}
		else
		{
			flashRenderer = cam.flashPlusRendererFg;
		}
		material = flashRenderer.material;
	}

	private void DoFlash()
	{
		if (legacyFlash)
		{
			UpdateFlashLayer();
		}
		DOTween.Kill(material, complete: true);
		material.color = startColor;
		TweenerCore<Color, Color, ColorOptions> t = material.DOColor(endColor, duration).SetEase(ease);
		t.OnComplete(delegate
		{
			material.color = endColor;
		});
		if (FG)
		{
			fgFlashTween = t;
		}
		else
		{
			bgFlashTween = t;
		}
	}

	private void UpdateFlashLayer()
	{
		flashRenderer.gameObject.layer = ((!FG) ? 10 : 0);
		flashRenderer.sortingLayerName = (FG ? "FgFlash" : "Default");
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = evnt.GetFloat("duration") * crotchet;
		startColor = evnt.GetColor("startColor");
		endColor = evnt.GetColor("endColor");
		startColor.a = evnt.GetFloat("startOpacity") / 100f;
		endColor.a = evnt.GetFloat("endOpacity") / 100f;
		ease = (Ease)evnt["ease"];
		FlashPlane flashPlane = (FlashPlane)evnt["plane"];
		FG = flashPlane == FlashPlane.Foreground;
		if (Application.isPlaying)
		{
			FlashSetup();
		}
	}
}
