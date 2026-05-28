using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;
using UnityStandardAssets.ImageEffects;

public class scrVfxPlus : ADOBase
{
	public VideoPlayer videoBG;

	public float vidOffset;

	[NonSerialized]
	public List<ffxPlusBase> effects;

	[NonSerialized]
	public scrVolumeTrackerFloat vTrackerFloat;

	public Dictionary<Filter, MonoBehaviour> filterToComp = new Dictionary<Filter, MonoBehaviour>();

	public Dictionary<Filter, float> filterCurrIntensity = new Dictionary<Filter, float>();

	public Dictionary<Filter, Tween> filterTween = new Dictionary<Filter, Tween>();

	public List<Tween> pausedTweens = new List<Tween>();

	public readonly Dictionary<Filter, float> filterDefaultValues = new Dictionary<Filter, float>
	{
		{
			Filter.Aberration,
			0.5f
		},
		{
			Filter.Blizzard,
			1f
		},
		{
			Filter.Fisheye,
			0.5f
		},
		{
			Filter.LED,
			1f
		},
		{
			Filter.Pixelate,
			0.01f
		}
	};

	private scrConductor cond;

	private scrController ctrl;

	private scrCamera cam;

	private int currentVfxIndex;

	[NonSerialized]
	public bool hasPlayed;

	private float _camAngle;

	private static scrVfxPlus _instance;

	public float camAngle
	{
		get
		{
			return _camAngle;
		}
		set
		{
			_camAngle = value;
			cam.transform.rotation = Quaternion.Euler(0f, 0f, value);
		}
	}

	private bool shouldPlayVideo => Persistence.visualEffects == VisualEffects.Full;

	public static scrVfxPlus instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindAnyObjectByType<scrVfxPlus>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		cond = scrConductor.instance;
		ctrl = scrController.instance;
		cam = scrCamera.instance;
		vTrackerFloat = GetComponent<scrVolumeTrackerFloat>();
		effects = new List<ffxPlusBase>();
		if (ADOBase.customLevel != null)
		{
			videoBG = ADOBase.customLevel.videoBG;
		}
		else if ((UnityEngine.Object)(object)videoBG != null && (UnityEngine.Object)(object)videoBG.clip != null && shouldPlayVideo)
		{
			((Component)(object)videoBG).gameObject.SetActive(value: true);
			videoBG.Stop();
			videoBG.Prepare();
		}
		MakeNewFilterDictionary();
	}

	public void MakeNewFilterDictionary()
	{
		filterToComp.Add(Filter.Grayscale, GetFilter<CameraFilterPackLegacy_Color_GrayScale>());
		filterToComp.Add(Filter.Sepia, GetFilter<CameraFilterPackLegacy_Color_Sepia>());
		filterToComp.Add(Filter.Invert, GetFilter<CameraFilterPackLegacy_Color_Invert>());
		filterToComp.Add(Filter.VHS, GetFilter<CameraFilterPackLegacy_Real_VHS>());
		filterToComp.Add(Filter.EightiesTV, GetFilter<CameraFilterPackLegacy_TV_80>());
		filterToComp.Add(Filter.FiftiesTV, GetFilter<CameraFilterPackLegacy_TV_50>());
		filterToComp.Add(Filter.Arcade, GetFilter<CameraFilterPackLegacy_TV_ARCADE>());
		filterToComp.Add(Filter.LED, GetFilter<CameraFilterPackLegacy_TV_LED>());
		filterToComp.Add(Filter.Rain, GetFilter<CameraFilterPackLegacy_Atmosphere_Rain>());
		filterToComp.Add(Filter.Blizzard, GetFilter<CameraFilterPackLegacy_Blizzard>());
		filterToComp.Add(Filter.PixelSnow, GetFilter<CameraFilterPackLegacy_Atmosphere_Snow_8bits>());
		filterToComp.Add(Filter.Compression, GetFilter<CameraFilterPackLegacy_TV_CompressionFX>());
		filterToComp.Add(Filter.Glitch, GetFilter<CameraFilterPackLegacy_FX_Glitch1>());
		filterToComp.Add(Filter.Pixelate, GetFilter<CameraFilterPackLegacy_Pixel_Pixelisation>());
		filterToComp.Add(Filter.Waves, GetFilter<CameraFilterPackLegacy_Distortion_Wave_Horizontal>());
		filterToComp.Add(Filter.Static, GetFilter<CameraFilterPackLegacy_Noise_TV>());
		filterToComp.Add(Filter.Grain, GetFilter<CameraFilterPackLegacy_Film_Grain>());
		filterToComp.Add(Filter.MotionBlur, GetFilter<CameraMotionBlur>());
		filterToComp.Add(Filter.Blur, GetFilter<CameraFilterPackLegacy_Blur_Blurry>());
		filterToComp.Add(Filter.BlurFocus, GetFilter<CameraFilterPackLegacy_Blur_Focus>());
		filterToComp.Add(Filter.GaussianBlur, GetFilter<CameraFilterPackLegacy_Blur_GaussianBlur>());
		filterToComp.Add(Filter.Fisheye, GetFilter<CameraFilterPackLegacy_Distortion_FishEye>());
		filterToComp.Add(Filter.Aberration, GetFilter<CameraFilterPackLegacy_Color_Chromatic_Aberration>());
		filterToComp.Add(Filter.Drawing, GetFilter<CameraFilterPackLegacy_Drawing_Paper>());
		filterToComp.Add(Filter.Neon, GetFilter<CameraFilterPackLegacy_Edge_Neon>());
		filterToComp.Add(Filter.HexagonBlack, GetFilter<CameraFilterPackLegacy_FX_Hexagon_Black>());
		filterToComp.Add(Filter.Posterize, GetFilter<CameraFilterPackLegacy_TV_Posterize>());
		filterToComp.Add(Filter.Sharpen, GetFilter<CameraFilterPackLegacy_Sharpen_Sharpen>());
		filterToComp.Add(Filter.Contrast, GetFilter<CameraFilterPackLegacy_Color_Contrast>());
		filterToComp.Add(Filter.EdgeBlackLine, GetFilter<CameraFilterPackLegacy_Edge_BlackLine>());
		filterToComp.Add(Filter.OilPaint, GetFilter<CameraFilterPackLegacy_Pixelisation_OilPaint>());
		filterToComp.Add(Filter.SuperDot, GetFilter<CameraFilterPackLegacy_FX_superDot>());
		filterToComp.Add(Filter.WaterDrop, GetFilter<CameraFilterPackLegacy_AAA_WaterDrop>());
		filterToComp.Add(Filter.LightWater, GetFilter<CameraFilterPackLegacy_Light_Water2>());
		filterToComp.Add(Filter.Handheld, GetFilter<CameraFilterPackLegacy_FX_8bits_gb>());
		filterToComp.Add(Filter.NightVision, GetFilter<CameraFilterPackLegacy_Oculus_NightVision1>());
		filterToComp.Add(Filter.Funk, GetFilter<CameraFilterPackLegacy_FX_Funk>());
		filterToComp.Add(Filter.Tunnel, GetFilter<PolarScreen>());
		filterToComp.Add(Filter.Weird3D, GetFilter<CameraFilterPackLegacy_TV_Video3D>());
		filterToComp.Add(Filter.Petals, GetFilter<FallingPetals>());
		filterToComp.Add(Filter.PetalsInstant, GetFilter<FallingPetals>());
		static T GetFilter<T>()
		{
			return scrCamera.instance.GetComponent<T>();
		}
	}

	public void Reset()
	{
		currentVfxIndex = 0;
		camAngle = 0f;
		effects.Clear();
		hasPlayed = false;
		filterTween.Clear();
		ResetFilterIntensityDefaults();
	}

	private void ResetFilterIntensityDefaults()
	{
		foreach (Filter value in Enum.GetValues(typeof(Filter)))
		{
			filterCurrIntensity[value] = CollectionExtensions.GetValueOrDefault<Filter, float>((IReadOnlyDictionary<Filter, float>)filterDefaultValues, value, 0f);
		}
	}

	private void Update()
	{
		if (ctrl.paused || !cond.hasSongStarted)
		{
			return;
		}
		int num;
		int num2;
		if (!ADOBase.isOfficialLevel)
		{
			num = ((!Persistence.forceVisualSettings) ? 1 : 0);
			if (num != 0)
			{
				num2 = 20;
				goto IL_003e;
			}
		}
		else
		{
			num = 0;
		}
		num2 = (int)ADOBase.controller.visualQuality;
		goto IL_003e;
		IL_003e:
		VisualQuality visualQuality = (VisualQuality)num2;
		VisualEffects visualEffects = ((num != 0) ? VisualEffects.Full : ADOBase.controller.visualEffects);
		int num3 = 0;
		while (currentVfxIndex < effects.Count)
		{
			ffxPlusBase ffxPlusBase2 = effects[currentVfxIndex];
			if (ffxPlusBase2 == null)
			{
				break;
			}
			if (!ffxPlusBase2.triggered)
			{
				if (cond.songposition_minusi < ffxPlusBase2.startTime - ffxPlusBase2.startEffectOffset)
				{
					break;
				}
				if ((!GCS.practiceMode || ADOBase.controller.currentState < States.Fail) && ffxPlusBase2.IsAllowedByVisualSettings(visualQuality, visualEffects))
				{
					ffxPlusBase2.StartEffect();
				}
				ffxPlusBase2.triggered = true;
			}
			currentVfxIndex++;
			num3++;
			if (num3 > 1000000)
			{
				break;
			}
		}
		if (!((UnityEngine.Object)(object)videoBG != null) || !((Component)(object)videoBG).gameObject.activeSelf || videoBG.isPlaying || hasPlayed || !videoBG.isPrepared)
		{
			return;
		}
		double num4 = (cond.separateCountdownTime ? (cond.crotchetAtStart * (double)ADOBase.conductor.adjustedCountdownTicks) : 0.0);
		if (cond.songposition_minusi >= num4 - (double)vidOffset)
		{
			if (shouldPlayVideo)
			{
				videoBG.Play();
			}
			else
			{
				videoBG.Stop();
			}
			hasPlayed = true;
			videoBG.playbackSpeed = cond.song.pitch;
			double time = cond.songposition_minusi - num4 + (double)vidOffset;
			videoBG.time = time;
			if ((bool)ADOBase.customLevel)
			{
				ADOBase.customLevel.ShowTutorialBackground(visible: false);
			}
		}
	}

	public void ScrubToTime(float t)
	{
		VisualQuality visualQuality = ((!ADOBase.isOfficialLevel && !Persistence.forceVisualSettings) ? VisualQuality.High : ADOBase.controller.visualQuality);
		VisualEffects visualEffects = (((bool)ADOBase.customLevel && !Persistence.forceVisualSettings) ? VisualEffects.Full : ADOBase.controller.visualEffects);
		List<ffxPlusBase> list = new List<ffxPlusBase>();
		foreach (ffxPlusBase effect in effects)
		{
			if (!effect.triggered && effect.IsAllowedByVisualSettings(visualQuality, visualEffects))
			{
				effect.ScrubToTime(t);
			}
			if (effect.triggered)
			{
				list.Add(effect);
			}
		}
		foreach (ffxPlusBase item in list)
		{
			effects.Remove(item);
		}
	}

	public void PrintEffects()
	{
		int num = 0;
		foreach (ffxPlusBase effect in effects)
		{
			_ = effect == null;
			num++;
		}
	}
}
