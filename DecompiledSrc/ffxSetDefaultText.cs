using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

public class ffxSetDefaultText : ffxPlusBase
{
	public Color defaultTextColor;

	public bool defaultTextColorUsed;

	public Color defaultTextShadowColor;

	public bool defaultTextShadowColorUsed;

	public Vector2 levelTitlePosition;

	public bool levelTitlePositionUsed;

	public string levelTitleText;

	public bool levelTitleTextUsed;

	public string congratsText;

	public bool congratsTextUsed;

	public string perfectText;

	public bool perfectTextUsed;

	private static Tween _defaultTextColorTween;

	private static Tween _defaultTextShadowColorTween;

	private static Tween _levelTitlePositionTween;

	private static Shadow _shadow;

	private static List<scrHUDText> _hudTexts;

	private static Shadow shadow
	{
		get
		{
			if (!_shadow)
			{
				return _shadow = ADOBase.controller.txtLevelName.GetComponent<Shadow>();
			}
			return _shadow;
		}
	}

	private static List<scrHUDText> hudTexts => _hudTexts ?? (_hudTexts = new List<scrHUDText>(ADOBase.controller.txtLevelName.transform.parent.GetComponentsInChildren<scrHUDText>()));

	protected override IEnumerable<Tween> eventTweens => new Tween[3] { _defaultTextColorTween, _defaultTextShadowColorTween, _levelTitlePositionTween };

	private static void CreateTweenOrSetValue(ref Tween tween, float duration, Ease ease, DOGetter<float> getter, DOSetter<float> setter, float endValue)
	{
		if (duration == 0f)
		{
			setter(endValue);
			return;
		}
		tween?.Kill(complete: true);
		tween = DOTween.To(getter, setter, endValue, duration).SetEase(ease);
	}

	private static void CreateTweenOrSetValue(ref Tween tween, float duration, Ease ease, DOGetter<Vector2> getter, DOSetter<Vector2> setter, Vector2 endValue)
	{
		if (duration == 0f)
		{
			setter(endValue);
			return;
		}
		tween?.Kill(complete: true);
		tween = DOTween.To(getter, setter, endValue, duration).SetEase(ease);
	}

	private static void CreateTweenOrSetValue(ref Tween tween, float duration, Ease ease, DOGetter<Color> getter, DOSetter<Color> setter, Color endValue)
	{
		if (duration == 0f)
		{
			setter(endValue);
			return;
		}
		tween?.Kill(complete: true);
		tween = DOTween.To(getter, setter, endValue, duration).SetEase(ease);
	}

	public static void UpdateHudTexts()
	{
		if (!hudTexts[0])
		{
			_hudTexts = null;
		}
		foreach (scrHUDText hudText in hudTexts)
		{
			hudText.changeColor = true;
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		ColourScheme currentColourScheme = scrVfx.instance.currentColourScheme;
		Text txtLevelName = ADOBase.controller.txtLevelName;
		if (defaultTextColorUsed)
		{
			CreateTweenOrSetValue(ref _defaultTextColorTween, duration, ease, () => txtLevelName.color, delegate(Color v)
			{
				ColourScheme colourScheme = currentColourScheme;
				Color colourText = (txtLevelName.color = v);
				colourScheme.colourText = colourText;
			}, defaultTextColor);
		}
		if (defaultTextShadowColorUsed)
		{
			CreateTweenOrSetValue(ref _defaultTextShadowColorTween, duration, ease, () => shadow.effectColor, delegate(Color v)
			{
				ColourScheme colourScheme = currentColourScheme;
				Color colourTextShadow = (shadow.effectColor = v);
				colourScheme.colourTextShadow = colourTextShadow;
			}, defaultTextShadowColor);
		}
		if (levelTitlePositionUsed)
		{
			RectTransform rectTransform = txtLevelName.rectTransform;
			CreateTweenOrSetValue(ref _levelTitlePositionTween, duration, ease, () => rectTransform.anchoredPosition - ADOBase.controller.txtLevelNameOriginalPosition.Value, delegate(Vector2 v)
			{
				rectTransform.anchoredPosition = ADOBase.controller.txtLevelNameOriginalPosition.Value + v;
			}, levelTitlePosition);
		}
		if (levelTitleTextUsed)
		{
			ADOBase.controller.txtLevelName.text = levelTitleText;
		}
		if (congratsTextUsed || perfectTextUsed)
		{
			if (congratsTextUsed)
			{
				ADOBase.controller.customTxtCongrats = congratsText;
			}
			else
			{
				ADOBase.controller.customTxtPurePerfect = perfectText;
			}
			if ((States)(object)ADOBase.controller.stateMachine.GetState() == States.Won)
			{
				ADOBase.controller.txtCongrats.text = (ADOBase.controller.mistakesManager.IsAllPurePerfect() ? perfectText : congratsText);
			}
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
		defaultTextColor = evnt.GetColor("defaultTextColor");
		defaultTextColorUsed = !evnt.disabled["defaultTextColor"];
		defaultTextShadowColor = evnt.GetColor("defaultTextShadowColor");
		defaultTextShadowColorUsed = !evnt.disabled["defaultTextShadowColor"];
		levelTitlePosition = (Vector2)evnt["levelTitlePosition"];
		levelTitlePositionUsed = !evnt.disabled["levelTitlePosition"];
		levelTitleText = evnt.GetStringLocalized("levelTitleText");
		levelTitleTextUsed = !evnt.disabled["levelTitleText"];
		congratsText = evnt.GetStringLocalized("congratsText");
		congratsTextUsed = !evnt.disabled["congratsText"];
		perfectText = evnt.GetStringLocalized("perfectText");
		perfectTextUsed = !evnt.disabled["perfectText"];
	}
}
