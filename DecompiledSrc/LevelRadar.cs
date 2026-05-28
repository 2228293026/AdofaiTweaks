using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class LevelRadar : ADOBase
{
	private enum LevelTrait
	{
		Stamina,
		Tech,
		Speed,
		Rhythm
	}

	private const int TraitCount = 4;

	public Vector2Int gridSize = new Vector2Int(16, 16);

	public Vector2 pixelSize = new Vector2(10f, 10f);

	public Vector2 padding = new Vector2(1f, 1f);

	public Texture2D iconsTexture;

	public Texture2D traitTexture;

	public GameObject pixelPrefab;

	public Color[] traitBaseColors;

	public Gradient rainbowGradient;

	public Gradient[] traitColorGradients;

	public Text difficultyText;

	public Texture2D[] iconAnimationTextures;

	private LevelTrait testTrait;

	private Color[] traitTexturePixels;

	private float[] levelTraitValues = new float[4];

	private float[] currentTraitValues = new float[4];

	private Image[,] pixels;

	private float timer;

	private bool animWentToZero;

	private void Awake()
	{
		traitTexturePixels = traitTexture.GetPixels();
		pixels = new Image[gridSize.x, gridSize.y];
		int num = 0;
		for (int i = 0; i < gridSize.y; i++)
		{
			for (int j = 0; j < gridSize.x; j++)
			{
				GameObject obj = Object.Instantiate(pixelPrefab, pixelPrefab.transform.parent);
				obj.name = $"pixel {num} ({j}, {i})";
				RectTransform component = obj.GetComponent<RectTransform>();
				Image component2 = obj.GetComponent<Image>();
				Vector2 vector = padding + pixelSize;
				component.anchoredPosition = new Vector2(vector.x * (float)j, vector.y * (float)i);
				component2.color = Color.clear;
				pixels[j, i] = component2;
				num++;
			}
		}
		pixelPrefab.SetActive(value: false);
		RandomizeLevelTraits();
	}

	public void SetLevelData(Vector4 traits, int difficulty)
	{
		timer = 0f;
		for (int i = 0; i < 4; i++)
		{
			levelTraitValues[i] = traits[i];
			SetTraitAnimated((LevelTrait)i, traits[i]);
		}
		difficultyText.text = difficulty.ToString();
	}

	private void Update()
	{
		timer += Time.unscaledDeltaTime;
		if (timer > 12f)
		{
			timer %= 12f;
			animWentToZero = false;
			ShowLevelTraits();
		}
		else if (timer > 7f)
		{
			float num = iconAnimationTextures.Length;
			int num2 = Mathf.FloorToInt((timer - 7f) % (0.25f * num) / 0.25f);
			SetTexture(iconAnimationTextures[num2]);
		}
		else if (timer > 9.7f && !animWentToZero)
		{
			animWentToZero = true;
			for (int i = 0; i < 4; i++)
			{
				SetTraitAnimated((LevelTrait)i, 0f);
			}
		}
	}

	private void RandomizeLevelTraits()
	{
		for (int i = 0; i < 4; i++)
		{
			levelTraitValues[i] = Random.Range(0f, 0.7f);
		}
	}

	private void ShowLevelTraits()
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			float num2 = levelTraitValues[i];
			SetTraitAnimated((LevelTrait)i, num2);
			num += num2;
		}
	}

	private void SetLights(bool on)
	{
		float duration = 0.2f;
		float endValue = (on ? 1f : 0f);
		Image[,] array = pixels;
		int upperBound = array.GetUpperBound(0);
		int upperBound2 = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
			{
				array[i, j].DOFade(endValue, duration);
			}
		}
	}

	private void SetTexture(Texture2D texture)
	{
		Color[] array = texture.GetPixels();
		int num = 0;
		for (int i = 0; i < gridSize.y; i++)
		{
			for (int j = 0; j < gridSize.x; j++)
			{
				Image image = pixels[j, i];
				Color color = array[num++];
				image.color = color;
			}
		}
	}

	private void TestTraitAnimation(LevelTrait trait, float level)
	{
		levelTraitValues[(int)trait] = level;
		SetTraitAnimated(trait, level);
	}

	private void SetTraitAnimated(LevelTrait trait, float level, float? from = null)
	{
		int traitIndex = (int)trait;
		TweenerCore<float, float, FloatOptions> t = DOTween.To(() => currentTraitValues[traitIndex], delegate(float x)
		{
			SetTrait(trait, x);
		}, level, 0.2f);
		if (from.HasValue)
		{
			t.From(from.Value);
		}
	}

	private void SetTrait(LevelTrait trait, float level)
	{
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		Gradient gradient = rainbowGradient;
		Color b = traitBaseColors[(int)trait];
		switch (trait)
		{
		case LevelTrait.Stamina:
			flag = false;
			flag2 = false;
			break;
		case LevelTrait.Tech:
			flag = true;
			flag2 = false;
			break;
		case LevelTrait.Speed:
			flag = false;
			flag2 = true;
			break;
		case LevelTrait.Rhythm:
			flag = true;
			flag2 = true;
			break;
		}
		for (int i = 0; i < gridSize.y; i++)
		{
			for (int j = 0; j < gridSize.x; j++)
			{
				int num2 = (flag ? (gridSize.x - 1 - j) : j);
				int num3 = (flag2 ? (gridSize.y - 1 - i) : i);
				Image image = pixels[num2, num3];
				Color color = traitTexturePixels[num++];
				if (!(color.a < 1f))
				{
					image.color = ((color.r <= level) ? Color.Lerp(gradient.Evaluate(color.r), b, 0.03f) : Color.clear);
				}
			}
		}
		currentTraitValues[(int)trait] = level;
	}
}
