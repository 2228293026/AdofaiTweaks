using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ffxSetFilterPlus : ffxPlusBase
{
	public Filter filter;

	public bool enableFilter;

	public bool disableOthers;

	public float intensity;

	public bool dontDisable;

	private Dictionary<Filter, MonoBehaviour> filterToComp => vfx.filterToComp;

	private Dictionary<Filter, float> filterCurrIntensity => vfx.filterCurrIntensity;

	private Dictionary<Filter, Tween> filterTween => vfx.filterTween;

	protected override IEnumerable<Tween> eventTweens
	{
		get
		{
			if (filterTween.ContainsKey(filter))
			{
				return new Tween[1] { filterTween[filter] };
			}
			return null;
		}
	}

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
		disableIfMinFx = !dontDisable;
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		if (filterTween.ContainsKey(filter))
		{
			filterTween[filter].Kill(complete: true);
			filterTween.Remove(filter);
		}
		bool flag = filterToComp[filter].enabled;
		if (duration == 0f)
		{
			filterCurrIntensity[filter] = intensity;
			SetFilter(filter, enableFilter, intensity);
		}
		else
		{
			if (!flag && !ADOBase.controller.legacyTween)
			{
				filterCurrIntensity[filter] = CollectionExtensions.GetValueOrDefault<Filter, float>((IReadOnlyDictionary<Filter, float>)vfx.filterDefaultValues, filter, 0f);
			}
			filterTween[filter] = DOTween.To(() => filterCurrIntensity[filter], delegate(float i)
			{
				filterCurrIntensity[filter] = i;
				SetFilter(filter, enableFilter, filterCurrIntensity[filter]);
			}, intensity, duration).SetEase(ease);
		}
		if (!disableOthers)
		{
			return;
		}
		foreach (Filter key in filterToComp.Keys)
		{
			if (key != filter && filterToComp[key] != null)
			{
				if (filterTween.TryGetValue(key, out var value))
				{
					value.Kill(complete: true);
				}
				filterToComp[key].enabled = false;
			}
		}
	}

	private void SetFilter(Filter f, bool fEnable, float fIntensity)
	{
		if (!filterToComp.ContainsKey(f))
		{
			return;
		}
		MonoBehaviour monoBehaviour = filterToComp[f];
		if (!ADOBase.isMobile || !(monoBehaviour is CameraMotionBlur))
		{
			monoBehaviour.enabled = fEnable;
		}
		if (!fEnable)
		{
			return;
		}
		switch (f)
		{
		case Filter.VHS:
			(monoBehaviour as CameraFilterPackLegacy_Real_VHS).TRACKING = 0.212f * fIntensity;
			break;
		case Filter.LED:
			(monoBehaviour as CameraFilterPackLegacy_TV_LED).Size = Mathf.RoundToInt(5f * fIntensity);
			break;
		case Filter.Drawing:
			(monoBehaviour as CameraFilterPackLegacy_Drawing_Paper).Fade_With_Original = Mathf.Clamp(fIntensity, 0f, 1f);
			break;
		case Filter.Compression:
			(monoBehaviour as CameraFilterPackLegacy_TV_CompressionFX).Parasite = 3f * fIntensity;
			break;
		case Filter.Waves:
			(monoBehaviour as CameraFilterPackLegacy_Distortion_Wave_Horizontal).WaveIntensity = 10f * fIntensity;
			CameraFilterPackLegacy_Distortion_Wave_Horizontal.ChangeWaveIntensity = 10f * fIntensity;
			break;
		case Filter.Pixelate:
			(monoBehaviour as CameraFilterPackLegacy_Pixel_Pixelisation)._Pixelisation = 4f * fIntensity;
			CameraFilterPackLegacy_Pixel_Pixelisation.ChangePixel = 4f * fIntensity;
			break;
		case Filter.Rain:
			(monoBehaviour as CameraFilterPackLegacy_Atmosphere_Rain).Intensity = 0.5f * fIntensity;
			break;
		case Filter.Blizzard:
			(monoBehaviour as CameraFilterPackLegacy_Blizzard)._Speed = fIntensity;
			break;
		case Filter.PixelSnow:
			(monoBehaviour as CameraFilterPackLegacy_Atmosphere_Snow_8bits).Threshold = 0.9f + 0.1f * fIntensity;
			break;
		case Filter.Static:
			(monoBehaviour as CameraFilterPackLegacy_Noise_TV).Fade = fIntensity;
			CameraFilterPackLegacy_Noise_TV.ChangeValue = fIntensity;
			break;
		case Filter.Grain:
			(monoBehaviour as CameraFilterPackLegacy_Film_Grain).Value = 32f * fIntensity;
			CameraFilterPackLegacy_Film_Grain.ChangeValue = 32f * fIntensity;
			break;
		case Filter.MotionBlur:
			if (!ADOBase.isMobile)
			{
				(monoBehaviour as CameraMotionBlur).velocityScale = 0.375f * fIntensity;
			}
			break;
		case Filter.Fisheye:
			(monoBehaviour as CameraFilterPackLegacy_Distortion_FishEye).Distortion = fIntensity;
			CameraFilterPackLegacy_Distortion_FishEye.ChangeDistortion = fIntensity;
			break;
		case Filter.Aberration:
			(monoBehaviour as CameraFilterPackLegacy_Color_Chromatic_Aberration).Offset = fIntensity * 0.04f - 0.02f;
			CameraFilterPackLegacy_Color_Chromatic_Aberration.ChangeOffset = fIntensity * 0.04f - 0.02f;
			break;
		case Filter.Sepia:
			(monoBehaviour as CameraFilterPackLegacy_Color_Sepia).Intensity = fIntensity;
			break;
		case Filter.Grayscale:
			(monoBehaviour as CameraFilterPackLegacy_Color_GrayScale).Intensity = fIntensity;
			break;
		case Filter.HexagonBlack:
			(monoBehaviour as CameraFilterPackLegacy_FX_Hexagon_Black).Value = Mathf.Max(0.2f, fIntensity);
			CameraFilterPackLegacy_FX_Hexagon_Black.ChangeValue = Mathf.Max(0.2f, fIntensity);
			break;
		case Filter.Posterize:
			(monoBehaviour as CameraFilterPackLegacy_TV_Posterize).Posterize = fIntensity * 20f;
			CameraFilterPackLegacy_TV_Posterize.ChangePosterize = fIntensity * 20f;
			break;
		case Filter.Sharpen:
			(monoBehaviour as CameraFilterPackLegacy_Sharpen_Sharpen).Value2 = fIntensity;
			CameraFilterPackLegacy_Sharpen_Sharpen.ChangeValue2 = fIntensity;
			break;
		case Filter.Contrast:
			(monoBehaviour as CameraFilterPackLegacy_Color_Contrast).Contrast = fIntensity + 1f;
			CameraFilterPackLegacy_Color_Contrast.ChangeContrast = fIntensity + 1f;
			break;
		case Filter.OilPaint:
			(monoBehaviour as CameraFilterPackLegacy_Pixelisation_OilPaint).Value = fIntensity;
			CameraFilterPackLegacy_Pixelisation_OilPaint.ChangeValue = fIntensity;
			break;
		case Filter.Blur:
			(monoBehaviour as CameraFilterPackLegacy_Blur_Blurry).Amount = fIntensity * 2f;
			CameraFilterPackLegacy_Blur_Blurry.ChangeAmount = fIntensity * 2f;
			break;
		case Filter.BlurFocus:
			(monoBehaviour as CameraFilterPackLegacy_Blur_Focus)._Size = Mathf.Max(0.10001f, fIntensity);
			CameraFilterPackLegacy_Blur_Focus.ChangeSize = Mathf.Max(0.10001f, fIntensity);
			break;
		case Filter.GaussianBlur:
			(monoBehaviour as CameraFilterPackLegacy_Blur_GaussianBlur).Size = Mathf.Max(0.10001f, fIntensity);
			CameraFilterPackLegacy_Blur_GaussianBlur.ChangeSize = fIntensity;
			break;
		case Filter.WaterDrop:
			(monoBehaviour as CameraFilterPackLegacy_AAA_WaterDrop).Distortion = Mathf.Lerp(64f, 8f, Mathf.Clamp(fIntensity, 0f, 1f));
			CameraFilterPackLegacy_AAA_WaterDrop.ChangeDistortion = Mathf.Lerp(64f, 8f, Mathf.Clamp(fIntensity, 0f, 1f));
			break;
		case Filter.LightWater:
			(monoBehaviour as CameraFilterPackLegacy_Light_Water2).Intensity = fIntensity * 2.4f;
			CameraFilterPackLegacy_Light_Water2.ChangeValue4 = fIntensity * 2.4f;
			break;
		case Filter.Petals:
			(monoBehaviour as FallingPetals).TogglePetals(enabled: true, 6f);
			break;
		case Filter.PetalsInstant:
			(monoBehaviour as FallingPetals).TogglePetals(enabled: true, 28f);
			break;
		case Filter.Invert:
		case Filter.EightiesTV:
		case Filter.FiftiesTV:
		case Filter.Arcade:
		case Filter.Glitch:
		case Filter.Neon:
		case Filter.Handheld:
		case Filter.NightVision:
		case Filter.Funk:
		case Filter.Tunnel:
		case Filter.Weird3D:
		case Filter.EdgeBlackLine:
		case Filter.SuperDot:
			break;
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		filter = (Filter)evnt["filter"];
		enableFilter = evnt.GetBool("enabled");
		intensity = evnt.GetFloat("intensity") / 100f;
		disableOthers = evnt.GetBool("disableOthers");
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
	}
}
