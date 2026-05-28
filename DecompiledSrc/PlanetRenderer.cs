using System;
using DG.Tweening;
using UnityEngine;

public class PlanetRenderer : ADOBase
{
	[NonSerialized]
	public PlanetSprite sprite;

	[NonSerialized]
	public scrRing ringComp;

	public ParticleSystem coreParticles;

	public ParticleSystem tailParticles;

	public ParticleSystem sparks;

	public LineRenderer ring;

	public SpriteRenderer glow;

	[Header("Effects")]
	public ParticleSystem deathExplosion;

	public SpriteRenderer faceSprite;

	public SpriteRenderer faceDetails;

	public Transform faceHolder;

	public SpriteRenderer samuraiSprite;

	public GameObject planetGold;

	public GameObject planetOverseer;

	public ParticleSystem tailParticlesCoop;

	public scrSamurai scrSamurai;

	public bool onlyRing;

	private Tween facePulseTween;

	private int faceIndex = -1;

	[NonSerialized]
	public int attachedDummyFloor;

	[NonSerialized]
	public scrObjectDecoration objectDecoration;

	public PlanetColor planetColor { get; private set; }

	public bool samuraiMode { get; private set; }

	public bool emojiMode { get; private set; }

	public bool rainbow { get; private set; }

	public static float rainbowHue => Time.unscaledTime / 5f % 1f;

	public Renderer[] appearanceRenderers => new Renderer[6]
	{
		sprite.meshRenderer,
		coreParticles.GetComponent<Renderer>(),
		tailParticles.GetComponent<Renderer>(),
		sparks.GetComponent<Renderer>(),
		ring,
		glow
	};

	private void Awake()
	{
		ringComp = ring.GetComponent<scrRing>();
		sprite = GetComponent<PlanetSprite>();
		sprite.Init();
		if (scrController.coopMode && tailParticles != tailParticlesCoop)
		{
			tailParticlesCoop.name = tailParticles.gameObject.name;
			tailParticlesCoop.gameObject.SetActive(value: true);
			tailParticles.gameObject.name += "old";
			tailParticles.gameObject.SetActive(value: false);
			tailParticles = tailParticlesCoop;
		}
	}

	private void LateUpdate()
	{
		if (emojiMode)
		{
			faceHolder.transform.rotation = Quaternion.Euler(0f, 0f, ADOBase.controller.camy.transform.rotation.eulerAngles.z);
		}
		if (rainbow && ringComp != null)
		{
			Color color = Color.HSVToRGB(rainbowHue, 1f, 1f);
			sprite.color = color;
			SetRingColor(color);
			SetTailColor(color);
			SetCoreColor(color);
			SetFaceColor(color);
		}
		if (ADOBase.isExpo && sprite.visible)
		{
			bool flag = !ADOBase.controller.playerOne.missCooldownActive || Time.unscaledTime % 0.1f < 0.05f;
			sprite.color = sprite.color.WithAlpha(flag ? 1f : 0.5f);
		}
	}

	public void Revive(bool liteRevive = false)
	{
		if (!liteRevive)
		{
			sprite.visible = true;
		}
		ringComp.enabled = true;
		ring.enabled = true;
		glow.enabled = true;
		tailParticles.Play();
		coreParticles.Play();
		sparks.Play();
		deathExplosion.gameObject.SetActive(value: false);
		faceHolder.gameObject.SetActive(!liteRevive && emojiMode);
	}

	public void SetColor(PlanetColor planetColor, bool isRed = true)
	{
		this.planetColor = planetColor;
		switch (planetColor.preset)
		{
		case PlanetColorPreset.DefaultRed:
			EnableDefaultFireAndIceColor(isRed, 0);
			DisableAllSpecialPlanets();
			break;
		case PlanetColorPreset.DefaultBlue:
			EnableDefaultFireAndIceColor(isRed, 1);
			DisableAllSpecialPlanets();
			break;
		case PlanetColorPreset.Gold:
			EnableDefaultFireAndIceColor(isRed);
			SwitchToGold();
			break;
		case PlanetColorPreset.Rainbow:
			EnableCustomColor();
			SetRainbow(enabled: true);
			break;
		case PlanetColorPreset.Overseer:
			EnableDefaultFireAndIceColor(isRed);
			SwitchToOverseer();
			break;
		default:
			EnableCustomColor();
			SetPlanetColor(planetColor.ToRealColor());
			SetTailColor(planetColor.ToRealColor());
			break;
		}
	}

	public void SetColorAndSave(PlanetColor planetColor, bool isRed = true)
	{
		SetColor(planetColor, isRed);
		Persistence.SetPlayerColor(planetColor, isRed);
	}

	public void SwitchToGold()
	{
		ToggleSpecialPlanet(planetGold, on: true, "Gold");
		SetExplosionColor(ADOBase.gc.goldExplosionColor1, ADOBase.gc.goldExplosionColor2);
		SetFaceColor(ringComp.color.WithAlpha(1f));
		SetRingColor(planetColor.ToRealColor());
	}

	public void RemoveGold()
	{
		ToggleSpecialPlanet(planetGold, on: false);
	}

	public void SwitchToOverseer()
	{
		ToggleSpecialPlanet(planetOverseer, on: true, "Ov");
		SetExplosionColor(ADOBase.gc.overseerExplosionColor1, ADOBase.gc.overseerExplosionColor2);
		sprite.color = new Color(9f / 85f, 0.64705884f, 40f / 51f, 1f);
		SetFaceColor(ringComp.color.WithAlpha(1f));
		SetRingColor(planetColor.ToRealColor());
	}

	public void RemoveOverseer()
	{
		ToggleSpecialPlanet(planetOverseer, on: false);
	}

	public void ToggleSpecialPlanet(GameObject specialP, bool on, string specialSuffix = "")
	{
		if (on)
		{
			DisableAllSpecialPlanets();
		}
		specialP.gameObject.SetActive(on);
		Transform t = (on ? specialP.transform : base.transform);
		if (on)
		{
			SetBasePlanetElementsActive(active: false);
			SetupNewPlanetElements(specialSuffix);
		}
		else
		{
			SetupNewPlanetElements("");
			SetBasePlanetElementsActive(active: true);
		}
		void SetBasePlanetElementsActive(bool active)
		{
			tailParticles.gameObject.SetActive(active);
			sparks.gameObject.SetActive(active);
			coreParticles.gameObject.SetActive(active);
			glow.gameObject.SetActive(active);
			sprite.visible = active;
		}
		void SetupNewPlanetElements(string suffix)
		{
			coreParticles = t.Find("Core" + suffix)?.GetComponent<ParticleSystem>() ?? coreParticles;
			tailParticles = t.Find("Tail" + suffix)?.GetComponent<ParticleSystem>() ?? tailParticles;
			sparks = t.Find("Sparks" + suffix)?.GetComponent<ParticleSystem>() ?? sparks;
			ring = t.Find("Ring" + suffix)?.GetComponent<LineRenderer>() ?? ring;
			glow = t.Find("Glow" + suffix)?.GetComponent<SpriteRenderer>() ?? glow;
			sprite = (on ? t.Find("Planet" + suffix) : base.transform)?.GetComponent<PlanetSprite>() ?? sprite;
		}
	}

	public void DisableAllSpecialPlanets()
	{
		if (planetGold.activeSelf)
		{
			RemoveGold();
		}
		if (planetOverseer.activeSelf)
		{
			RemoveOverseer();
		}
	}

	public void EnableCustomColor()
	{
		DisableAllSpecialPlanets();
		sprite.sprite = ADOBase.gc.tex_planetWhite;
	}

	public void EnableDefaultFireAndIceColor(bool isRed, int defaultColor = -1)
	{
		SetRainbow(enabled: false);
		DisableAllSpecialPlanets();
		sprite.color = Color.white;
		switch (defaultColor)
		{
		case -1:
		{
			sprite.sprite = (isRed ? ADOBase.gc.tex_planetRed : ADOBase.gc.tex_planetBlue);
			Color color = (isRed ? Color.red : Color.blue);
			SetRingColor(color);
			SetTailColor(color);
			SetFaceColor(color);
			break;
		}
		case 0:
			sprite.sprite = ADOBase.gc.tex_planetRed;
			SetRingColor(Color.red);
			SetCoreColor(Color.red);
			SetTailColor(Color.red);
			SetFaceColor(Color.red);
			break;
		case 1:
			sprite.sprite = ADOBase.gc.tex_planetBlue;
			SetRingColor(Color.blue);
			SetCoreColor(Color.blue);
			SetTailColor(Color.blue);
			SetFaceColor(Color.blue);
			break;
		}
	}

	public void SetPlanetColor(Color color)
	{
		if (!(sprite == null))
		{
			SetRainbow(enabled: false);
			sprite.color = color;
			SetRingColor(color);
			SetCoreColor(color);
			SetFaceColor(color);
		}
	}

	private void SetRingColor(Color color)
	{
		ringComp.color = color.WithAlpha(color.a * 0.4f);
	}

	public void SetTailColor(Color color)
	{
		if (!(sprite == null))
		{
			SetParticleSystemColor(tailParticles, color, color * new Color(0.5f, 0.5f, 0.5f));
			SetExplosionColor(color);
		}
	}

	public void SetExplosionColor(Color color, Color color2 = default(Color))
	{
		ParticleSystem.MainModule main = deathExplosion.main;
		Gradient gradient = new Gradient();
		Color col = ((color2 != Color.clear) ? color2 : color);
		gradient.SetKeys(new GradientColorKey[3]
		{
			new GradientColorKey(color, 0f),
			new GradientColorKey(color, 0.5f),
			new GradientColorKey(col, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		});
		gradient.mode = GradientMode.Fixed;
		ParticleSystem.MinMaxGradient startColor = new ParticleSystem.MinMaxGradient(gradient);
		startColor.mode = ParticleSystemGradientMode.RandomColor;
		main.startColor = startColor;
	}

	public Color GetPlanetColor()
	{
		return sprite.color;
	}

	public Color GetTailColor()
	{
		return tailParticles.main.startColor.color;
	}

	public void SetCoreColor(Color color)
	{
		glow.color = color.WithAlpha(glow.color.a * color.a);
		ParticleSystem.MainModule main = coreParticles.main;
		main.startColor = new ParticleSystem.MinMaxGradient(color);
		SetParticleSystemColor(coreParticles, color, color);
	}

	public void SetLayer(string layer)
	{
		int sortingLayerID = SortingLayer.NameToID(layer);
		tailParticles.GetComponent<ParticleSystemRenderer>().sortingLayerID = sortingLayerID;
		coreParticles.GetComponent<ParticleSystemRenderer>().sortingLayerID = sortingLayerID;
		sparks.GetComponent<ParticleSystemRenderer>().sortingLayerID = sortingLayerID;
		ring.sortingLayerID = sortingLayerID;
		sprite.meshRenderer.sortingLayerID = sortingLayerID;
		glow.sortingLayerID = sortingLayerID;
		samuraiSprite.sortingLayerID = sortingLayerID;
		faceDetails.sortingLayerID = sortingLayerID;
		faceSprite.sortingLayerID = sortingLayerID;
	}

	private void SetParticleSystemColor(ParticleSystem particleSystem, Color baseColor, Color startColor)
	{
		ParticleSystem.MainModule main = particleSystem.main;
		main.startColor = new ParticleSystem.MinMaxGradient(startColor);
		Color color = Color.Lerp(baseColor, Color.black, 0.4f);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		Gradient gradient = new Gradient();
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(baseColor, 0f),
			new GradientColorKey(baseColor * new Color(0.6f, 0.6f, 0.6f), 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(0.4f, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		Gradient gradient2 = new Gradient();
		gradient2.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(color, 0f),
			new GradientColorKey(color * new Color(0.6f, 0.6f, 0.6f), 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(0.6f, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		if (particleSystem == tailParticlesCoop)
		{
			gradient2 = gradient;
		}
		ParticleSystem.MinMaxGradient color2 = new ParticleSystem.MinMaxGradient(gradient, gradient2);
		color2.mode = ParticleSystemGradientMode.TwoGradients;
		colorOverLifetime.color = color2;
	}

	private void SetFaceColor(Color color)
	{
		faceSprite.color = color;
	}

	public void SetRainbow(bool enabled)
	{
		rainbow = enabled;
	}

	public void LoadPlanetColor(bool isRed)
	{
		if (objectDecoration != null)
		{
			return;
		}
		SetRainbow(enabled: false);
		if (Persistence.GetSamuraiMode(isRed))
		{
			ToggleSamurai(enabled: true, isRed);
		}
		SetEmojiMode(Persistence.GetEmojiMode(isRed));
		PlanetColor planetColor = (this.planetColor = Persistence.GetPlayerColor(isRed));
		if (planetColor.preset == PlanetColorPreset.Gold || GCS.d_forceGoldPlanets)
		{
			DisableAllSpecialPlanets();
			SwitchToGold();
		}
		else if (planetColor.preset == PlanetColorPreset.Overseer)
		{
			DisableAllSpecialPlanets();
			SwitchToOverseer();
		}
		else if (planetColor.preset == PlanetColorPreset.Rainbow)
		{
			EnableCustomColor();
			SetRainbow(enabled: true);
		}
		else
		{
			PlanetColorPreset preset = planetColor.preset;
			if (preset == PlanetColorPreset.DefaultRed || preset == PlanetColorPreset.DefaultBlue)
			{
				int defaultColor = ((planetColor.preset != PlanetColorPreset.DefaultRed) ? 1 : 0);
				EnableDefaultFireAndIceColor(isRed, defaultColor);
			}
			else
			{
				EnableCustomColor();
				var (color, tailColor) = planetColor.preset switch
				{
					PlanetColorPreset.TransBlue => (new Color(0.3607843f, 67f / 85f, 0.9294118f), Color.white), 
					PlanetColorPreset.TransPink => (new Color(0.9568627f, 0.6431373f, 0.7098039f), Color.white), 
					PlanetColorPreset.NBYellow => (new Color(0.996f, 0.953f, 0.18f), Color.white), 
					PlanetColorPreset.NBPurple => (new Color(0.612f, 0.345f, 0.82f), Color.black), 
					PlanetColorPreset.Custom => (planetColor.customColor.Value, planetColor.customColor.Value), 
					_ => (planetColor.preset.GetColor(), planetColor.preset.GetColor()), 
				};
				SetPlanetColor(color);
				SetTailColor(tailColor);
			}
		}
		if (scrLogoText.instance != null)
		{
			scrLogoText.instance.UpdateColors();
		}
	}

	public void ToggleSamurai(bool enabled, bool isRed)
	{
		samuraiMode = enabled;
		samuraiSprite.gameObject.SetActive(enabled);
		GetComponent<SpriteRenderer>().maskInteraction = (enabled ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None);
		ToggleAllSpecialPlanetsSamuraiMode(enabled);
		SetEmojiMode(!enabled && Persistence.GetEmojiMode(isRed));
	}

	private void ToggleSpecialPlanetSamuraiMode(GameObject specialP, bool on, string specialSuffix = "")
	{
		specialP.transform.Find("Planet" + specialSuffix).GetComponent<SpriteRenderer>().maskInteraction = (on ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None);
	}

	private void ToggleAllSpecialPlanetsSamuraiMode(bool on)
	{
		ToggleSpecialPlanetSamuraiMode(planetGold, on, "Gold");
		ToggleSpecialPlanetSamuraiMode(planetOverseer, on, "Ov");
	}

	public void SetEmojiMode(bool enabled, bool pulseOnEnable = false)
	{
		if (!enabled || !samuraiSprite.gameObject.activeSelf)
		{
			emojiMode = enabled;
			faceHolder.gameObject.SetActive(enabled);
			if (enabled)
			{
				ChangeFace(pulseOnEnable);
			}
		}
	}

	public void ChangeFace(bool pulse, bool ignoreTimescale = false)
	{
		if (emojiMode)
		{
			int num = ADOBase.gc.faceBaseSprites.Length - 1;
			int num2 = 0;
			int num3;
			do
			{
				num3 = ((UnityEngine.Random.Range(0, 1000) == 0) ? num : UnityEngine.Random.Range(0, num));
				num2++;
			}
			while (num2 <= 100 && num3 == faceIndex);
			faceIndex = num3;
			faceSprite.sprite = ADOBase.gc.faceBaseSprites[faceIndex];
			faceDetails.sprite = ADOBase.gc.faceBaseDetailSprites[faceIndex];
			if (pulse)
			{
				facePulseTween?.Kill();
				faceHolder.ScaleXY(1.25f, 1.25f);
				facePulseTween = faceHolder.DOScale(Vector3.one, 0.33f).SetEase(Ease.OutCubic).SetUpdate(ignoreTimescale);
			}
		}
	}

	public void PlayParticles()
	{
		tailParticles.Play();
		coreParticles.Play();
		sparks.Play();
	}

	public void DisableParticles()
	{
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		deathExplosion.gameObject.SetActive(value: false);
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void StopParticles()
	{
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		deathExplosion.gameObject.SetActive(value: false);
	}

	public void ClearParticles()
	{
		tailParticles.Clear();
		coreParticles.Clear();
		sparks.Clear();
	}

	public void Explode(float explosionSize)
	{
		Destroy();
		deathExplosion.gameObject.SetActive(value: true);
		ParticleSystem.MainModule main = deathExplosion.main;
		ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
		startLifetime.constant = explosionSize * 1.2f;
		main.startLifetime = startLifetime;
		deathExplosion.emission.SetBursts(new ParticleSystem.Burst[1]
		{
			new ParticleSystem.Burst(0f, (int)(explosionSize * 2000f))
		});
	}

	public void Destroy(bool keepRing = false)
	{
		sprite.visible = false;
		ring.enabled = keepRing;
		onlyRing = keepRing;
		faceHolder.gameObject.SetActive(value: false);
		samuraiSprite.gameObject.SetActive(value: false);
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		deathExplosion.Clear();
		glow.enabled = false;
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}
}
