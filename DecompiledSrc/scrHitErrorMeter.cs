using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrHitErrorMeter : MonoBehaviour
{
	private readonly string TWEEN_ID = "adofai.hit_error_meter";

	public int tickCacheSize = 60;

	public GameObject curvedMeter;

	public GameObject straightMeter;

	public GameObject tickPrefab;

	public CanvasScaler scaler;

	public RectTransform wrapperRectTransform;

	public Image handImage;

	public Vector2 pos = new Vector2(0.5f, 0.03f);

	[Range(0.25f, 4f)]
	public float meterScale = 1f;

	[Range(0f, 1f)]
	public float sensitivity = 0.2f;

	[Range(0f, 10f)]
	public float tickLife = 3f;

	private ErrorMeterShape meterShape;

	private float averageAngle;

	private Image[] cachedTickImages;

	private string[] cachedTweenIds;

	private int tickIndex;

	private float tickLifeMultiplier
	{
		get
		{
			if (!scrController.coopMode)
			{
				return 1f;
			}
			return 0.25f;
		}
	}

	private void Awake()
	{
		cachedTickImages = new Image[tickCacheSize];
		cachedTweenIds = new string[tickCacheSize];
		for (int i = 0; i < tickCacheSize; i++)
		{
			GameObject gameObject = Object.Instantiate(tickPrefab, wrapperRectTransform.gameObject.transform);
			cachedTickImages[i] = gameObject.GetComponent<Image>();
			cachedTweenIds[i] = TWEEN_ID + "_tick_" + i;
		}
		UpdateLayout();
	}

	public void UpdateLayout(ErrorMeterSize size = ErrorMeterSize.Normal, ErrorMeterShape shape = ErrorMeterShape.Straight)
	{
		switch (size)
		{
		case ErrorMeterSize.Small:
			wrapperRectTransform.anchoredPosition = new Vector2(0f, -30f);
			meterScale = 0.75f;
			break;
		case ErrorMeterSize.Normal:
			wrapperRectTransform.anchoredPosition = new Vector2(0f, -48f);
			meterScale = 1f;
			break;
		case ErrorMeterSize.Large:
			wrapperRectTransform.anchoredPosition = new Vector2(0f, -71f);
			meterScale = 1.5f;
			break;
		case ErrorMeterSize.ExtraLarge:
			wrapperRectTransform.anchoredPosition = new Vector2(0f, -94f);
			meterScale = 2f;
			break;
		}
		if (meterShape != shape)
		{
			meterShape = shape;
			Reset();
			curvedMeter.SetActive(value: false);
			straightMeter.SetActive(value: false);
			switch (shape)
			{
			case ErrorMeterShape.Curved:
				curvedMeter.SetActive(value: true);
				break;
			case ErrorMeterShape.Straight:
				straightMeter.SetActive(value: true);
				break;
			}
		}
		wrapperRectTransform.anchorMin = pos;
		wrapperRectTransform.anchorMax = pos;
		wrapperRectTransform.pivot = pos;
		wrapperRectTransform.localScale = new Vector3(1f, 1f, 1f) * meterScale;
	}

	public void AddHit(float angleDiff, float marginScale = 1f, scrPlanet planet = null)
	{
		angleDiff *= -57.29578f;
		if (scrConductor.instance == null)
		{
			Debug.Log("Conductor was null!");
			return;
		}
		float? obj = planet?.player.currFloor.prevfloor?.speed;
		double bpmTimesSpeed = scrConductor.instance.bpm * (obj ?? 1f);
		double conductorPitch = scrConductor.instance.song.pitch;
		double adjustedAngleBoundaryInDeg = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch, marginScale);
		angleDiff *= (float)(60.0 / adjustedAngleBoundaryInDeg);
		if (angleDiff < -60f)
		{
			angleDiff = -60.0001f - Random.value * 3f;
		}
		if (angleDiff > 60f)
		{
			angleDiff = 60.0001f + Random.value * 3f;
		}
		if (angleDiff >= -60f && angleDiff <= 60f)
		{
			averageAngle = Mathf.Lerp(averageAngle, angleDiff, sensitivity);
		}
		if (meterShape == ErrorMeterShape.Curved)
		{
			handImage.rectTransform.DORotateQuaternion(Quaternion.Euler(0f, 0f, averageAngle), 0.25f).SetEase(Ease.OutCubic).SetId(TWEEN_ID);
		}
		else if (meterShape == ErrorMeterShape.Straight)
		{
			handImage.rectTransform.DOAnchorPos(new Vector2((0f - averageAngle) * 2.5f, -62f), 0.25f).SetEase(Ease.OutCubic).SetId(TWEEN_ID);
		}
		if (!scrController.coopMode)
		{
			planet = null;
		}
		if (meterShape == ErrorMeterShape.Curved)
		{
			DrawCurvedTick(angleDiff, marginScale, planet);
		}
		else if (meterShape == ErrorMeterShape.Straight)
		{
			DrawStraightTick(angleDiff, marginScale, planet);
		}
	}

	public void Reset()
	{
		DOTween.Kill(TWEEN_ID);
		string[] array = cachedTweenIds;
		for (int i = 0; i < array.Length; i++)
		{
			DOTween.Kill(array[i]);
		}
		Image[] array2 = cachedTickImages;
		foreach (Image obj in array2)
		{
			obj.rectTransform.rotation = Quaternion.identity;
			obj.rectTransform.anchoredPosition = Vector2.zero;
		}
		averageAngle = 0f;
		if ((bool)handImage)
		{
			handImage.rectTransform.rotation = Quaternion.identity;
			if (meterShape == ErrorMeterShape.Curved)
			{
				handImage.rectTransform.anchoredPosition = Vector2.zero;
			}
			else if (meterShape == ErrorMeterShape.Straight)
			{
				handImage.rectTransform.anchoredPosition = new Vector2(0f, -62f);
			}
		}
	}

	private void DrawCurvedTick(float angle, float marginScale = 1f, scrPlanet planet = null)
	{
		Color color = planet?.planetRenderer.planetColor.ToRealColor() ?? CalculateTickColor(angle);
		Image tickImage = cachedTickImages[tickIndex];
		string text = cachedTweenIds[tickIndex];
		tickImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
		DOTween.Kill(text);
		tickImage.color = color;
		tickImage.DOColor(color.WithAlpha(0f), tickLife * tickLifeMultiplier).SetEase(Ease.InQuad).SetId(text)
			.OnKill(delegate
			{
				tickImage.color = Color.clear;
			});
		tickIndex = (tickIndex + 1) % tickCacheSize;
	}

	private void DrawStraightTick(float angle, float marginScale = 1f, scrPlanet planet = null)
	{
		Color color = planet?.planetRenderer.planetColor.ToRealColor() ?? CalculateTickColor(angle);
		Image tickImage = cachedTickImages[tickIndex];
		string text = cachedTweenIds[tickIndex];
		tickImage.rectTransform.anchoredPosition = new Vector2((0f - angle) * 2.5f, -62f);
		DOTween.Kill(text);
		tickImage.color = color;
		tickImage.DOColor(color.WithAlpha(0f), tickLife * tickLifeMultiplier).SetEase(Ease.InQuad).SetId(text)
			.OnKill(delegate
			{
				tickImage.color = Color.clear;
			});
		tickIndex = (tickIndex + 1) % tickCacheSize;
	}

	private Color CalculateTickColor(float angle, float marginScale = 1f)
	{
		double bpmTimesSpeed = (double)scrConductor.instance.bpm * scrController.instance.playerOne.planetarySystem.speed;
		double conductorPitch = scrConductor.instance.song.pitch;
		double adjustedAngleBoundaryInDeg = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch, marginScale);
		double adjustedAngleBoundaryInDeg2 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch, marginScale);
		double adjustedAngleBoundaryInDeg3 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch, marginScale);
		double num = 60.0 / adjustedAngleBoundaryInDeg;
		adjustedAngleBoundaryInDeg *= num;
		adjustedAngleBoundaryInDeg2 *= num;
		adjustedAngleBoundaryInDeg3 *= num;
		if ((double)angle < 0.0 - adjustedAngleBoundaryInDeg)
		{
			return RDConstants.data.hitMarginColoursUI.colourTooEarly;
		}
		if ((double)angle < 0.0 - adjustedAngleBoundaryInDeg2)
		{
			return RDConstants.data.hitMarginColoursUI.colourVeryEarly;
		}
		if ((double)angle < 0.0 - adjustedAngleBoundaryInDeg3)
		{
			return RDConstants.data.hitMarginColoursUI.colourLittleEarly;
		}
		if ((double)angle <= adjustedAngleBoundaryInDeg3)
		{
			return RDConstants.data.hitMarginColoursUI.colourPerfect;
		}
		if ((double)angle <= adjustedAngleBoundaryInDeg2)
		{
			return RDConstants.data.hitMarginColoursUI.colourLittleLate;
		}
		if ((double)angle <= adjustedAngleBoundaryInDeg)
		{
			return RDConstants.data.hitMarginColoursUI.colourVeryLate;
		}
		return RDConstants.data.hitMarginColoursUI.colourTooLate;
	}
}
