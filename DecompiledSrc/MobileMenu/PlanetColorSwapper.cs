using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileMenu;

public class PlanetColorSwapper : ADOBase
{
	public GameObject colorCloudPrefab;

	public Transform emojiPaintTransform;

	public PlanetRenderer planetRed;

	public PlanetRenderer planetBlue;

	public MobileMenuGrabbablePlanet grabbablePlanetRed;

	public MobileMenuGrabbablePlanet grabbablePlanetBlue;

	public SpriteRenderer redButtonIcon;

	public SpriteRenderer redButtonGlow;

	public SpriteRenderer redButtonArrows;

	public SpriteRenderer blueButtonIcon;

	public SpriteRenderer blueButtonGlow;

	public SpriteRenderer blueButtonArrows;

	private Transform[] colorClouds;

	private List<PlanetColorPreset> availableColors;

	private PlanetColorPreset previousColorCloud;

	private bool changedEmojiMode;

	private bool changedRainbowMode;

	private int paintCombo;

	private int prevBeat;

	private void Awake()
	{
		UpdateColors();
	}

	private void Start()
	{
		planetRed.SetColor(Persistence.GetPlayerColor(red: true));
		planetBlue.SetColor(Persistence.GetPlayerColor(red: false));
	}

	public void UpdateColors()
	{
		availableColors = new List<PlanetColorPreset>();
		if (Persistence.IsWorldComplete(0))
		{
			availableColors.Add(PlanetColorPreset.DefaultRed, PlanetColorPreset.DefaultBlue);
		}
		if (Persistence.IsWorldComplete(1))
		{
			availableColors.Add(PlanetColorPreset.Orange, PlanetColorPreset.LightBlue);
		}
		if (Persistence.IsWorldComplete(2))
		{
			availableColors.Add(PlanetColorPreset.Pink, PlanetColorPreset.Green);
		}
		if (Persistence.IsWorldComplete(3))
		{
			availableColors.Add(PlanetColorPreset.Purple, PlanetColorPreset.Grass);
		}
		if (Persistence.IsWorldComplete(4))
		{
			availableColors.Add(PlanetColorPreset.PastelBlue, PlanetColorPreset.PastelPink);
		}
		if (Persistence.IsWorldComplete(5))
		{
			availableColors.Add(PlanetColorPreset.Black, PlanetColorPreset.White);
		}
		if (Persistence.IsWorldComplete(6))
		{
			availableColors.Add(PlanetColorPreset.Gold);
		}
		if (Persistence.IsWorldComplete(7))
		{
			availableColors.Add(PlanetColorPreset.Aqua, PlanetColorPreset.Violet);
		}
		if (Persistence.IsWorldComplete(8))
		{
			availableColors.Add(PlanetColorPreset.Jungle, PlanetColorPreset.Vine);
		}
		if (Persistence.IsWorldComplete(9))
		{
			availableColors.Add(PlanetColorPreset.Crimson, PlanetColorPreset.Maroon);
		}
		if (Persistence.IsWorldComplete(10))
		{
			availableColors.Add(PlanetColorPreset.Cyan, PlanetColorPreset.Teal);
		}
		if (Persistence.IsWorldComplete(11))
		{
			availableColors.Add(PlanetColorPreset.Jester, PlanetColorPreset.Stone);
		}
		if (Persistence.IsWorldComplete(12))
		{
			availableColors.Add(PlanetColorPreset.Rust, PlanetColorPreset.Metal);
		}
		if (Persistence.IsWorldComplete("T5"))
		{
			availableColors.Add(PlanetColorPreset.Overseer);
		}
		emojiPaintTransform.gameObject.SetActive(Persistence.IsWorldComplete(ADOBase.worldData["MO"].index));
		if (colorClouds != null)
		{
			Transform[] array = colorClouds;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
		}
		colorClouds = new Transform[availableColors.Count];
		float[] array2 = new float[availableColors.Count];
		float S = 0f;
		float V = 0f;
		for (int j = 0; j < availableColors.Count; j++)
		{
			Color.RGBToHSV(availableColors[j].GetColor(), out array2[j], out S, out V);
		}
		availableColors = availableColors.OrderBy((PlanetColorPreset planetColor) => RDUtils.GetHue(planetColor.GetColor())).ToList();
		for (int num = 0; num < colorClouds.Length; num++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(colorCloudPrefab);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			ParticleSystem.MainModule main = gameObject.GetComponent<ParticleSystem>().main;
			PlanetColorPreset planetColorPreset = availableColors[num];
			main.startColor = planetColorPreset.GetColor();
			ColorCloud component = gameObject.GetComponent<ColorCloud>();
			component.SetSortingOrder(num);
			switch (planetColorPreset)
			{
			case PlanetColorPreset.Gold:
				component.goldSparks.SetActive(value: true);
				break;
			case PlanetColorPreset.Overseer:
				component.overseerParticles.SetActive(value: true);
				break;
			}
			colorClouds[num] = gameObject.transform;
		}
		PositionClouds();
	}

	public void PositionClouds()
	{
		Vector2 vector = new Vector2(4f, 3f);
		for (int i = 0; i < colorClouds.Length; i++)
		{
			Vector2 vector2 = vector;
			float f = (float)i * 1f / (float)colorClouds.Length * 2f * (float)Math.PI;
			float num = ((i % 2 == 0) ? 1f : 1f);
			vector2.x *= num;
			vector2.y *= num;
			Vector3 localPosition = new Vector3(Mathf.Cos(f) * vector2.x, Mathf.Sin(f) * vector2.y, 0f);
			colorClouds[i].transform.localPosition = localPosition;
		}
	}

	private void PaintPlanet(PlanetRenderer planetRenderer, bool isRed, PlanetColorPreset color)
	{
		if (!changedRainbowMode)
		{
			if (!changedEmojiMode)
			{
				planetRenderer.SetEmojiMode(enabled: false);
				Persistence.SetEmojiMode(enabled: false, isRed);
			}
			paintCombo++;
			if (paintCombo >= 20)
			{
				color = PlanetColorPreset.Rainbow;
				changedRainbowMode = true;
				scrSfx.instance.PlaySfx(SfxSound.NotificationTinyText, MixerGroup.InterfaceParent);
			}
			else
			{
				float pitch = Mathf.Pow(1.0594631f, Mathf.Min(paintCombo, 20));
				scrSfx.instance.PlaySfx(SfxSound.PlanetPaint, MixerGroup.InterfaceParent, 1f, pitch);
			}
			planetRenderer.SetColorAndSave(new PlanetColor(color), isRed);
			scrLogoText.instance.UpdateColors();
		}
	}

	private void Update()
	{
		float t = 0.15f;
		float num = Time.timeSinceLevelLoad * 3f % ((float)Math.PI * 2f);
		float f = (num + (float)Math.PI) % ((float)Math.PI * 2f);
		float num2 = 1f;
		MobileMenuController instance = MobileMenuController.instance;
		MobileMenuGrabController grabController = instance.grabController;
		if (instance.currentGroup != null && instance.currentScreen is MobileMenuColorScreen)
		{
			if (RDInput.confirmPress)
			{
				grabController.ToggleGrabObject(grabbablePlanetRed);
			}
			if (RDInput.backPress)
			{
				grabController.ToggleGrabObject(grabbablePlanetBlue);
			}
		}
		MobileMenuGrabbable grabbedObject = grabController.grabbedObject;
		MobileMenuGrabbablePlanet mobileMenuGrabbablePlanet = null;
		if (grabbedObject is MobileMenuGrabbablePlanet)
		{
			mobileMenuGrabbablePlanet = grabbedObject as MobileMenuGrabbablePlanet;
		}
		bool flag = grabbedObject == grabbablePlanetRed;
		bool flag2 = grabbedObject == grabbablePlanetBlue;
		SpriteRenderer spriteRenderer = redButtonGlow;
		bool flag3 = (redButtonArrows.enabled = flag);
		spriteRenderer.enabled = flag3;
		SpriteRenderer spriteRenderer2 = blueButtonGlow;
		flag3 = (blueButtonArrows.enabled = flag2);
		spriteRenderer2.enabled = flag3;
		if ((bool)mobileMenuGrabbablePlanet)
		{
			Vector2 position = RDInput.position;
			Vector3 translation = Time.unscaledDeltaTime * 7f * new Vector3(position.x, position.y, 0f);
			mobileMenuGrabbablePlanet.transform.Translate(translation);
			if (flag)
			{
				redButtonArrows.enabled = Time.unscaledTime % 1f < 0.8f;
			}
			if (flag2)
			{
				blueButtonArrows.enabled = Time.unscaledTime % 1f < 0.8f;
			}
		}
		if (mobileMenuGrabbablePlanet == null || !mobileMenuGrabbablePlanet.isRed)
		{
			Vector3 b = new Vector3(Mathf.Cos(num) * num2, Mathf.Sin(num) * num2, 0f);
			planetRed.transform.parent.localPosition = Vector3.Lerp(planetRed.transform.parent.localPosition, b, t);
		}
		if (mobileMenuGrabbablePlanet == null || mobileMenuGrabbablePlanet.isRed)
		{
			Vector3 b2 = new Vector3(Mathf.Cos(f) * num2, Mathf.Sin(f) * num2, 0f);
			planetBlue.transform.parent.localPosition = Vector3.Lerp(planetBlue.transform.parent.localPosition, b2, t);
		}
		PlanetRenderer planetRenderer;
		if ((bool)mobileMenuGrabbablePlanet)
		{
			planetRenderer = mobileMenuGrabbablePlanet.planet;
			if (emojiPaintTransform.gameObject.activeSelf && !changedEmojiMode && IsPlanetTouching(emojiPaintTransform))
			{
				planetRenderer.SetEmojiMode(enabled: true);
				Persistence.SetEmojiMode(enabled: true, mobileMenuGrabbablePlanet.isRed);
				changedEmojiMode = true;
				scrSfx.instance.PlaySfx(SfxSound.ModifierActivate, MixerGroup.InterfaceParent);
			}
			else
			{
				for (int i = 0; i < colorClouds.Length; i++)
				{
					if (IsPlanetTouching(colorClouds[i].transform))
					{
						PlanetColorPreset planetColorPreset = availableColors[i];
						if (paintCombo == 0)
						{
							previousColorCloud = planetRenderer.planetColor.preset;
						}
						if (previousColorCloud != planetColorPreset)
						{
							PaintPlanet(planetRenderer, mobileMenuGrabbablePlanet.isRed, planetColorPreset);
							previousColorCloud = planetColorPreset;
						}
						break;
					}
				}
			}
		}
		else
		{
			changedEmojiMode = false;
			changedRainbowMode = false;
			paintCombo = 0;
		}
		if (Mathf.Abs(ADOBase.conductor.beatNumber - prevBeat) >= 1)
		{
			((ADOBase.conductor.beatNumber % 2 == 0) ? planetBlue : planetRed).ChangeFace(pulse: true);
			prevBeat = ADOBase.conductor.beatNumber;
		}
		bool IsPlanetTouching(Transform tr)
		{
			return Vector2.Distance(tr.position, planetRenderer.transform.position) < 0.3f;
		}
	}
}
