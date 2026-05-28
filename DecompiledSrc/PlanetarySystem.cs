using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PlanetarySystem : ADOBase
{
	public enum DeathAnimation
	{
		Explode,
		CrumbleAndExplode,
		Crumble
	}

	[NonSerialized]
	public int maxPlanetColors = 2;

	public scrPlanet planetRed;

	public scrPlanet planetBlue;

	[NonSerialized]
	public scrPlanet planetGreen;

	[NonSerialized]
	public scrPlanet planetYellow;

	[NonSerialized]
	public scrPlanet planetPurple;

	[NonSerialized]
	public scrPlanet planetPink;

	[NonSerialized]
	public scrPlanet planetOrange;

	[NonSerialized]
	public scrPlanet planetCyan;

	[NonSerialized]
	public List<scrPlanet> planetList;

	[NonSerialized]
	public List<scrPlanet> allPlanets;

	[NonSerialized]
	public List<scrPlanet> availablePlanets;

	[NonSerialized]
	public double speed = 1.0;

	[NonSerialized]
	public bool isCW = true;

	[NonSerialized]
	public scrPlanet chosenPlanet;

	public Tween explodeTween;

	public void Init()
	{
		planetList = new List<scrPlanet> { planetRed, planetBlue };
		GameObject planetContainer = planetRed.transform.parent.gameObject;
		for (int i = 0; i < planetList.Count; i++)
		{
			scrPlanet obj = planetList[i];
			obj.planetIndex = i;
			obj.next = GetMultiPlanet(i, 1);
			obj.prev = GetMultiPlanet(i, -1);
		}
		availablePlanets = new List<scrPlanet>
		{
			(planetGreen = InitPlanet("PlanetGreen", new Color(0.3f, 0.7f, 0f, 1f))),
			(planetYellow = InitPlanet("PlanetYellow", new Color(1f, 0.8f, 0f, 1f))),
			(planetPurple = InitPlanet("PlanetPurple", new Color(0.7f, 0.1f, 1f, 1f))),
			(planetPink = InitPlanet("PlanetPink", new Color(1f, 0.1f, 0.7f, 1f))),
			(planetOrange = InitPlanet("PlanetOrange", new Color(1f, 0.4f, 0.1f, 1f))),
			(planetCyan = InitPlanet("PlanetCyan", new Color(0.1f, 0.8f, 0.9f, 1f)))
		};
		foreach (scrPlanet availablePlanet in availablePlanets)
		{
			availablePlanet.Destroy();
		}
		allPlanets = new List<scrPlanet> { planetRed, planetBlue };
		allPlanets.AddRange(availablePlanets);
		planetRed.planetRenderer.samuraiSprite.GetComponent<scrSamurai>().Setup();
		planetBlue.planetRenderer.samuraiSprite.GetComponent<scrSamurai>().Setup();
		scrPlanet InitPlanet(string name, Color c)
		{
			scrPlanet obj2 = UnityEngine.Object.Instantiate(planetBlue, planetContainer.transform);
			obj2.name = name;
			obj2.planetRenderer.SetColor(new PlanetColor(c));
			obj2.isExtra = true;
			return obj2;
		}
	}

	private void Start()
	{
		if (ADOBase.sceneName.IsTaro())
		{
			maxPlanetColors = 3;
		}
	}

	public void ResetPlanets()
	{
		ResetNumPlanets();
		for (int i = 0; i < planetList.Count; i++)
		{
			scrPlanet scrPlanet2 = planetList[i];
			scrPlanet2.Destroy();
			if (i > 1)
			{
				scrPlanet2.transform.localPosition = Vector3.right * (1.5f * (float)i);
			}
			scrPlanet2.planetRenderer.samuraiSprite.gameObject.SetActive(value: false);
			scrPlanet2.planetRenderer.faceHolder.gameObject.SetActive(value: false);
			scrPlanet2.planetScale = 1f;
		}
		explodeTween?.Kill();
	}

	public scrPlanet GetMultiPlanet(int index, int dir)
	{
		return planetList[scrMisc.ModInt(index + dir, planetList.Count)];
	}

	public void ResetNumPlanets()
	{
		planetList.Clear();
		availablePlanets.Clear();
		planetList.Add(planetRed);
		planetList.Add(planetBlue);
		availablePlanets.Add(planetGreen);
		availablePlanets.Add(planetYellow);
		availablePlanets.Add(planetPurple);
		availablePlanets.Add(planetPink);
		availablePlanets.Add(planetOrange);
		availablePlanets.Add(planetCyan);
		for (int i = 0; i < planetList.Count; i++)
		{
			planetList[i].planetIndex = i;
			planetList[i].next = GetMultiPlanet(i, 1);
			planetList[i].prev = GetMultiPlanet(i, -1);
		}
		for (int j = 0; j < availablePlanets.Count; j++)
		{
			availablePlanets[j].transform.position = planetList[0].transform.position;
			availablePlanets[j].Destroy();
		}
	}

	public void ScrubToFloorNumber(int floorNum, float? windbackTime = null, bool movePos = true, bool doingRevive = false)
	{
		scrPlanet scrPlanet2 = planetList[0];
		int num = 2;
		SetNumPlanets(2, scrPlanet2, 0);
		for (int i = 1; i <= floorNum; i++)
		{
			scrFloor scrFloor2 = scrLevelMaker.instance.listFloors[i];
			if (scrFloor2.numPlanets != num)
			{
				num = scrFloor2.numPlanets;
				SetNumPlanets(num, scrPlanet2, i);
			}
			scrPlanet2 = (scrFloor2.midSpin ? scrPlanet2.prev : scrPlanet2.next);
		}
		chosenPlanet = scrPlanet2;
		scrPlanet2.ScrubToFloorNumber(floorNum, windbackTime, movePos);
	}

	public void SetNumPlanets(int numPlanets, scrPlanet scrubChosen = null, int scrubbingfloor = -1)
	{
		bool flag = scrubChosen != null;
		int count = planetList.Count;
		if (numPlanets < 2 || numPlanets > 8)
		{
			return;
		}
		if (numPlanets < count)
		{
			for (int i = 0; i < planetList.Count - numPlanets; i++)
			{
				int planetIndex = GetMultiPlanet(flag ? scrubChosen.planetIndex : chosenPlanet.planetIndex, -(i + 1)).planetIndex;
				scrPlanet scrPlanet2 = planetList[planetIndex];
				if (!flag)
				{
					scrPlanet2.Die(0.3f);
				}
				else
				{
					scrPlanet2.Destroy();
				}
				scrPlanet2.toDelete = true;
			}
			int num = 0;
			for (int num2 = planetList.Count - 1; num2 >= 0; num2--)
			{
				if (planetList[num2].toDelete)
				{
					availablePlanets.Insert(num, planetList[num2]);
					planetList[num2].toDelete = false;
					planetList.RemoveAt(num2);
					num++;
				}
			}
		}
		PlanetRenderer planetRenderer = planetRed.planetRenderer;
		PlanetRenderer planetRenderer2 = planetBlue.planetRenderer;
		if (numPlanets > count)
		{
			int num3 = 0;
			int num4 = (flag ? scrubChosen.planetIndex : chosenPlanet.planetIndex);
			for (int j = count; j < numPlanets; j++)
			{
				int index = num4 + num3;
				planetList.Insert(index, availablePlanets[0]);
				planetList[index].Rewind();
				planetList[index].transform.position = chosenPlanet.transform.position;
				planetList[index].planetRenderer.ClearParticles();
				if (!GCS.staticPlanetColors)
				{
					if (planetRenderer.emojiMode && planetList[index] == planetRed)
					{
						planetRed.planetRenderer.SetEmojiMode(enabled: true);
					}
					else if (planetRenderer2.emojiMode && planetList[index] == planetBlue)
					{
						planetBlue.planetRenderer.SetEmojiMode(enabled: true);
					}
					else if (planetRenderer.emojiMode && planetRenderer2.emojiMode)
					{
						planetList[index].planetRenderer.SetEmojiMode(enabled: true);
					}
					if (planetRenderer.samuraiMode && planetList[index] == planetRed)
					{
						planetRed.planetRenderer.ToggleSamurai(enabled: true, isRed: true);
					}
					else if (planetRenderer2.samuraiMode && planetList[index] == planetBlue)
					{
						planetBlue.planetRenderer.ToggleSamurai(enabled: true, isRed: false);
					}
					else if (planetRenderer.samuraiMode && planetRenderer2.samuraiMode)
					{
						planetList[index].planetRenderer.ToggleSamurai(enabled: true, isRed: false);
					}
				}
				availablePlanets.RemoveAt(0);
				num3++;
			}
		}
		for (int k = 0; k < planetList.Count; k++)
		{
			planetList[k].planetIndex = k;
			planetList[k].next = GetMultiPlanet(k, 1);
			planetList[k].prev = GetMultiPlanet(k, -1);
		}
		ApplyMultiplanetColors();
	}

	public void Die(DeathAnimation deathAnimation, Action callback = null)
	{
		bool num = deathAnimation != DeathAnimation.Explode;
		bool isLastPlayer = ADOBase.controller.playerManager.GetActivePlayers().Count == 1;
		if (num && GCS.playDeathSound)
		{
			SfxSound sfxSound = (isLastPlayer ? SfxSound.PlanetPreExplosion : SfxSound.PlanetPreExplosionCoop);
			scrSfx.instance.PlaySfx(sfxSound, MixerGroup.SfxParent, 0.5f);
		}
		if (!num)
		{
			Explode(isLastPlayer);
		}
		else
		{
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => chosenPlanet.cosmeticRadius, delegate(float x)
			{
				chosenPlanet.cosmeticRadius = x;
			}, 0f, 0.5f);
			if (deathAnimation == DeathAnimation.CrumbleAndExplode)
			{
				tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, (TweenCallback)delegate
				{
					Explode(isLastPlayer);
				});
			}
		}
		if (callback != null)
		{
			explodeTween = DOVirtual.DelayedCall(0.5f, delegate
			{
				callback();
			});
		}
	}

	public void Explode(bool isLastPlayer = true)
	{
		if (isLastPlayer)
		{
			scrFlash.Flash(Color.white.WithAlpha(0.3f));
		}
		SfxSound sfxSound = SfxSound.PlanetExplosion;
		if (!isLastPlayer)
		{
			sfxSound = SfxSound.PlanetExplosionCoop;
		}
		else if (ADOBase.controller.endLevelInfo.newBestType == NewBestType.Jingle)
		{
			sfxSound = SfxSound.PlanetExplosionHighscore;
		}
		if (GCS.playDeathSound)
		{
			scrSfx.instance.PlaySfx(sfxSound, MixerGroup.SfxParent, 0.5f);
		}
		if (GCS.playWilhelm)
		{
			scrSfx.instance.PlaySfx(SfxSound.Wilhelm, MixerGroup.SfxParent, 0.6f);
		}
		for (int i = 0; i < planetList.Count; i++)
		{
			planetList[i].Die();
		}
	}

	public void LoadPlanetColors(int playerID)
	{
		GCS.staticPlanetColors = false;
		if (scrController.coopMode)
		{
			PlanetColor planetColor = scrPlayerManager.playerColors[playerID];
			int num = scrPlayerManager.playerOrder[playerID];
			bool flag = scrPlayerManager.playerEmoji[playerID];
			foreach (scrPlanet allPlanet in allPlanets)
			{
				allPlanet.planetRenderer.SetColor(planetColor);
			}
			planetRed.planetRenderer.SetEmojiMode(flag);
			planetBlue.planetRenderer.SetEmojiMode(flag);
			for (int i = 0; i < allPlanets.Count; i++)
			{
				allPlanets[i].planetScale = 1f - 0.25f * (float)num;
				base.transform.localPosition = base.transform.localPosition.WithZ(-num);
			}
		}
		else
		{
			planetRed.planetRenderer.LoadPlanetColor(isRed: true);
			planetBlue.planetRenderer.LoadPlanetColor(isRed: false);
		}
		for (int j = 0; j < planetList.Count; j++)
		{
			scrPlanet obj = planetList[j];
			obj.planetIndex = j;
			obj.next = GetMultiPlanet(j, 1);
			obj.prev = GetMultiPlanet(j, -1);
		}
	}

	public void ApplyMultiplanetColors()
	{
		if (GCS.staticPlanetColors)
		{
			return;
		}
		PlanetColor planetColor = planetRed.planetRenderer.planetColor;
		PlanetColor planetColor2 = planetBlue.planetRenderer.planetColor;
		if (planetColor.preset == PlanetColorPreset.DefaultRed && planetColor2.preset == PlanetColorPreset.DefaultBlue)
		{
			planetGreen.planetRenderer.SetColor(new PlanetColor(new Color(0.3f, 0.7f, 0f, 1f)));
			planetYellow.planetRenderer.SetColor(new PlanetColor(new Color(1f, 0.8f, 0f, 1f)));
			planetPurple.planetRenderer.SetColor(new PlanetColor(new Color(0.7f, 0.1f, 1f, 1f)));
			planetPink.planetRenderer.SetColor(new PlanetColor(new Color(1f, 0.1f, 0.7f, 1f)));
			planetOrange.planetRenderer.SetColor(new PlanetColor(new Color(1f, 0.4f, 0.1f, 1f)));
			planetCyan.planetRenderer.SetColor(new PlanetColor(new Color(0.1f, 0.8f, 0.9f, 1f)));
		}
		else if (planetColor == planetColor2)
		{
			for (int i = 2; i <= maxPlanetColors - 1; i++)
			{
				PlanetRenderer planetRenderer = allPlanets[i].planetRenderer;
				planetRenderer.SetColor(planetColor);
				if (planetColor.preset == PlanetColorPreset.Rainbow)
				{
					planetRenderer.SetRainbow(enabled: true);
				}
				PlanetColorPreset preset = planetColor.preset;
				if (preset == PlanetColorPreset.TransPink || preset == PlanetColorPreset.TransBlue || preset == PlanetColorPreset.NBYellow)
				{
					planetRenderer.SetTailColor(Color.white);
				}
				if (planetColor.preset == PlanetColorPreset.NBPurple)
				{
					planetRenderer.SetTailColor(Color.black);
				}
			}
		}
		else if (GCS.isTrans() || GCS.isNB())
		{
			Color item = new Color(0.9568627f, 0.6431373f, 0.7098039f);
			Color item2 = new Color(0.3607843f, 67f / 85f, 0.9294118f);
			Color item3 = Color.white;
			Color white = Color.white;
			Color item4 = Color.white;
			Color white2 = Color.white;
			Color item5 = Color.white;
			Color item6 = Color.white;
			if (GCS.isNB())
			{
				item = new Color(0.612f, 0.345f, 0.82f);
				item2 = new Color(0.996f, 0.953f, 0.18f);
				item3 = new Color(0.5f, 0.5f, 0.5f);
				white = Color.white;
				item4 = Color.black;
				white2 = Color.white;
				item5 = new Color(0.996f, 0.953f, 0.18f);
				item6 = new Color(0.612f, 0.345f, 0.82f);
			}
			List<Color> list = new List<Color> { item, item2, item3, white };
			List<Color> list2 = new List<Color> { item4, white2, item5, item6 };
			for (int j = 2; j <= maxPlanetColors - 1; j++)
			{
				PlanetRenderer planetRenderer2 = allPlanets[j].planetRenderer;
				int num = j;
				if (GCS.isTrans())
				{
					if (j == 4)
					{
						num = 5;
					}
					if (j == 5)
					{
						num = 4;
					}
				}
				planetRenderer2.SetPlanetColor(list[num % list.Count]);
				planetRenderer2.SetTailColor(list2[num % list2.Count]);
			}
		}
		else if (maxPlanetColors > 2)
		{
			Color rgbColor = planetColor.ToRealColor();
			Color rgbColor2 = planetColor2.ToRealColor();
			Color.RGBToHSV(rgbColor, out var H, out var S, out var V);
			Color.RGBToHSV(rgbColor2, out var H2, out var S2, out var V2);
			Mathf.Max(Mathf.Abs(H2 - H), 1f - Mathf.Abs(H2 - H));
			float num2 = ((1f - Mathf.Abs(H2 - H) > Mathf.Abs(H2 - H)) ? Mathf.Max(H, H2) : Mathf.Min(H, H2));
			float num3 = ((1f - Mathf.Abs(H2 - H) > Mathf.Abs(H2 - H)) ? Mathf.Min(H, H2) : Mathf.Max(H, H2));
			float num4 = ((num2 == H) ? S : S2);
			float num5 = ((num2 == H) ? V : V2);
			float num6 = ((num2 == H) ? S2 : S);
			float num7 = ((num2 == H) ? V2 : V);
			if (num3 < num2)
			{
				num3 += 1f;
			}
			for (int k = 2; k <= maxPlanetColors - 1; k++)
			{
				PlanetRenderer planetRenderer3 = allPlanets[k].planetRenderer;
				float num8 = (float)(k - 2 + 1) / (float)(maxPlanetColors - 1);
				float h = (num2 + (num3 - num2) * num8) % 1f;
				float s = num4 + (num6 - num4) * num8;
				float v = num5 + (num7 - num5) * num8;
				planetRenderer3.SetColor(new PlanetColor(Color.HSVToRGB(h, s, v)));
			}
		}
	}

	public void ResetMode()
	{
		scrPlanet obj = ADOBase.controller.planetRed;
		scrPlanet scrPlanet2 = ADOBase.controller.planetBlue;
		obj.planetRenderer.EnableDefaultFireAndIceColor(isRed: true);
		scrPlanet2.planetRenderer.EnableDefaultFireAndIceColor(isRed: false);
		Persistence.SetPlayerColor(PlanetColorPreset.DefaultRed, red: true);
		Persistence.SetPlayerColor(PlanetColorPreset.DefaultBlue, red: false);
		Persistence.SetEmojiMode(enabled: false, red: true);
		Persistence.SetEmojiMode(enabled: false, red: false);
		obj.planetRenderer.SetEmojiMode(enabled: false);
		scrPlanet2.planetRenderer.SetEmojiMode(enabled: false);
		scrLogoText.instance.UpdateColors();
	}

	public void TransMode()
	{
		scrPlanet obj = ADOBase.controller.planetRed;
		scrPlanet scrPlanet2 = ADOBase.controller.planetBlue;
		Color planetColor = new Color(0.3607843f, 67f / 85f, 0.9294118f);
		Color planetColor2 = new Color(0.9568627f, 0.6431373f, 0.7098039f);
		obj.planetRenderer.EnableCustomColor();
		scrPlanet2.planetRenderer.EnableCustomColor();
		obj.planetRenderer.SetPlanetColor(planetColor);
		scrPlanet2.planetRenderer.SetPlanetColor(planetColor2);
		obj.planetRenderer.SetTailColor(Color.white);
		scrPlanet2.planetRenderer.SetTailColor(Color.white);
		Persistence.SetPlayerColor(PlanetColorPreset.TransBlue, red: true);
		Persistence.SetPlayerColor(PlanetColorPreset.TransPink, red: false);
		scrLogoText.instance.UpdateColors();
	}

	public void EnbyMode()
	{
		Color planetColor = new Color(0.996f, 0.953f, 0.18f);
		Color planetColor2 = new Color(0.612f, 0.345f, 0.82f);
		planetRed.planetRenderer.EnableCustomColor();
		planetBlue.planetRenderer.EnableCustomColor();
		planetRed.planetRenderer.SetPlanetColor(planetColor);
		planetBlue.planetRenderer.SetPlanetColor(planetColor2);
		planetRed.planetRenderer.SetTailColor(Color.white);
		planetBlue.planetRenderer.SetTailColor(Color.black);
		Persistence.SetPlayerColor(PlanetColorPreset.NBYellow, red: true);
		Persistence.SetPlayerColor(PlanetColorPreset.NBPurple, red: false);
		scrLogoText.instance.UpdateColors();
	}

	public void RainbowMode()
	{
		scrPlanet scrPlanet2 = ADOBase.controller.planetRed;
		scrPlanet obj = ADOBase.controller.planetBlue;
		scrPlanet2.planetRenderer.EnableCustomColor();
		obj.planetRenderer.EnableCustomColor();
		scrPlanet2.planetRenderer.SetRainbow(enabled: true);
		obj.planetRenderer.SetRainbow(enabled: true);
		Persistence.SetPlayerColor(PlanetColorPreset.Rainbow, red: true);
		Persistence.SetPlayerColor(PlanetColorPreset.Rainbow, red: false);
		scrLogoText.instance.UpdateColors();
	}

	public void SamuraiMode()
	{
		bool samuraiMode = Persistence.GetSamuraiMode(red: true);
		bool samuraiMode2 = Persistence.GetSamuraiMode(red: false);
		Persistence.SetSamuraiMode(!samuraiMode, red: true);
		Persistence.SetSamuraiMode(!samuraiMode2, red: false);
		planetRed.planetRenderer.ToggleSamurai(!samuraiMode, isRed: true);
		planetBlue.planetRenderer.ToggleSamurai(!samuraiMode2, isRed: false);
	}

	public void ToggleEmojiMode()
	{
		bool emojiMode = Persistence.GetEmojiMode(red: true);
		bool emojiMode2 = Persistence.GetEmojiMode(red: false);
		Persistence.SetEmojiMode(!emojiMode, red: true);
		Persistence.SetEmojiMode(!emojiMode2, red: false);
		planetRed.planetRenderer.SetEmojiMode(!emojiMode);
		planetBlue.planetRenderer.SetEmojiMode(!emojiMode2);
	}
}
