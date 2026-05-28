using System;
using ADOFAI.Editor.Interfaces;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ADOFAI;

public class RDColorPickerPopup : ADOBase
{
	public const float MaxY = -190f;

	public const float MinY = -720f;

	public GameObject alphaSlider;

	public GameObject alphaInputField;

	public RectTransform panel;

	public Ease showEase;

	public Ease hideEase;

	public float animDuration;

	public RectTransform arrow;

	public CUIColorPicker cuiColorPicker;

	public TMP_InputField hexColor;

	public TMP_InputField rInput;

	public TMP_InputField gInput;

	public TMP_InputField bInput;

	public TMP_InputField aInput;

	public string lastValidColor = "ffddee";

	private IColorPickerData colorPC;

	private bool isAnyRGBAInputFocused;

	public RectTransform rectTransform { get; private set; }

	private TMP_InputField[] _colorInputs => new TMP_InputField[4] { rInput, gInput, bInput, aInput };

	private bool UsesAlpha => colorPC.usesAlpha;

	private string Color
	{
		get
		{
			if (!hexColor.text.IsValidHexColor(UsesAlpha))
			{
				return lastValidColor;
			}
			return hexColor.text;
		}
		set
		{
			if (value.IsValidHexColor(UsesAlpha))
			{
				hexColor.text = value;
			}
		}
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		hexColor.onValueChanged.AddListener(delegate
		{
			if (hexColor.text.IsValidHexColor(UsesAlpha) && hexColor.isFocused)
			{
				lastValidColor = hexColor.text;
				cuiColorPicker.Color = lastValidColor.HexToColor();
			}
		});
		TMP_InputField[] colorInputs = _colorInputs;
		foreach (TMP_InputField field in colorInputs)
		{
			field.onEndEdit.AddListener(delegate
			{
				SetResultColorFromRGBAInputs(field);
			});
		}
	}

	private void Update()
	{
		isAnyRGBAInputFocused = rInput.isFocused || gInput.isFocused || bInput.isFocused || aInput.isFocused;
		if (!isAnyRGBAInputFocused && hexColor.text.IsValidHexColor(UsesAlpha))
		{
			Color color = hexColor.text.HexToColor();
			rInput.text = Convert.ToInt32(color.r * 255f).ToString();
			gInput.text = Convert.ToInt32(color.g * 255f).ToString();
			bInput.text = Convert.ToInt32(color.b * 255f).ToString();
			aInput.text = Convert.ToInt32(color.a * 255f).ToString();
		}
		if (!hexColor.isFocused)
		{
			UpdateHexInputColor();
		}
	}

	public void SetResultColorFromRGBAInputs(TMP_InputField currentRGBAInput)
	{
		if (isAnyRGBAInputFocused)
		{
			if (string.IsNullOrEmpty(currentRGBAInput.text))
			{
				currentRGBAInput.text = "0";
			}
			else if (Convert.ToInt32(currentRGBAInput.text) < 0)
			{
				currentRGBAInput.text = "0";
			}
			else if (Convert.ToInt32(currentRGBAInput.text) > 255)
			{
				currentRGBAInput.text = "255";
			}
			Color color = new Color((float)Convert.ToInt32(rInput.text) / 255f, (float)Convert.ToInt32(gInput.text) / 255f, (float)Convert.ToInt32(bInput.text) / 255f, (float)Convert.ToInt32(aInput.text) / 255f);
			cuiColorPicker.Color = color;
		}
	}

	private void UpdateHexInputColor()
	{
		hexColor.text = cuiColorPicker.Color.ToHex(UsesAlpha, hash: false);
	}

	public void Show(IColorPickerData propertyControl_Color)
	{
		colorPC = propertyControl_Color;
		RectTransform component = hexColor.GetComponent<RectTransform>();
		if (UsesAlpha)
		{
			alphaSlider.SetActive(value: true);
			alphaInputField.SetActive(value: true);
			panel.sizeDelta = new Vector2(300f, 332f);
			hexColor.characterLimit = 8;
			component.sizeDelta = component.sizeDelta.WithX(96f);
		}
		else
		{
			alphaSlider.SetActive(value: false);
			alphaInputField.SetActive(value: false);
			panel.sizeDelta = new Vector2(300f, 297f);
			hexColor.characterLimit = 6;
			component.sizeDelta = component.sizeDelta.WithX(80f);
		}
		alphaSlider.GetComponent<AlphaSlider>().result = colorPC.sample;
		cuiColorPicker.startColor = colorPC.text.HexToColor();
		cuiColorPicker.result = colorPC.sample.gameObject;
		hexColor.text = ColorUtility.ToHtmlStringRGBA(colorPC.sample.color);
		base.gameObject.SetActive(value: true);
		colorPC.SetPickerPosition(this);
		if ((bool)ADOBase.editor)
		{
			GetComponent<Canvas>().sortingOrder = ADOBase.editor.PushPopupBlocker(Hide);
		}
		Animate(show: true);
	}

	public void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			colorPC.text = Color;
			colorPC.OnHide(Color);
			Animate(show: false);
		}
	}

	public void Animate(bool show)
	{
		Vector3 localScale = (show ? Vector3.zero : Vector3.one);
		Vector3 endValue = (show ? Vector3.one : Vector3.zero);
		Ease ease = (show ? showEase : hideEase);
		rectTransform.DOKill();
		rectTransform.localScale = localScale;
		rectTransform.DOScale(endValue, animDuration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				base.gameObject.SetActive(show);
			});
	}
}
