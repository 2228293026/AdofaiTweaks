using ADOFAI.Editor.Interfaces;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ADOFAI.Editor.Components;

public class ColorField : MonoBehaviour
{
	private class PickerData : IColorPickerData
	{
		public string text
		{
			get
			{
				return inputField.text;
			}
			set
			{
				control.SetValue(value);
				control.onChange.Invoke(control.value);
			}
		}

		public bool usesAlpha { get; }

		public Image sample { get; }

		private TMP_InputField inputField { get; }

		private ColorField control { get; }

		public PickerData(TMP_InputField inputField, ColorField control, bool usesAlpha)
		{
			this.inputField = inputField;
			this.usesAlpha = usesAlpha;
			this.control = control;
			sample = control.sample;
		}

		public void SetPickerPosition(RDColorPickerPopup popup)
		{
			bool flag = sample.canvas.transform.InverseTransformPoint(sample.rectTransform.position).x < 0f;
			if (flag)
			{
				Vector3 position = sample.rectTransform.position;
				popup.rectTransform.position = position;
				popup.rectTransform.anchoredPosition += new Vector2(24f, 0f);
			}
			else
			{
				Vector3 position2 = sample.rectTransform.position;
				position2.y = sample.rectTransform.position.y;
				popup.rectTransform.position = position2;
				popup.rectTransform.anchoredPosition -= new Vector2(48f, 0f);
			}
			popup.arrow.ScaleX(flag ? (-1f) : 1f);
			popup.panel.pivot = popup.panel.pivot.WithX(flag ? 0f : 1f);
			popup.panel.AnchorPosX(flag ? 12 : (-12));
			Vector2 anchoredPosition = popup.rectTransform.anchoredPosition;
			float y = 0f;
			if (anchoredPosition.y < -720f)
			{
				y = -720f - anchoredPosition.y;
			}
			else if (anchoredPosition.y > -190f)
			{
				y = -190f - anchoredPosition.y;
			}
			popup.panel.AnchorPosY(y);
		}

		public void OnHide(string value)
		{
			inputField.text = value;
			control.SetValue(value);
		}
	}

	public TMP_InputField field;

	public Image sample;

	public Button colorButton;

	public RDColorPickerPopup colorPickerPopup;

	public bool usesAlpha;

	public string defaultValue;

	public UnityEvent<string> onChange;

	[UsedImplicitly]
	public string value
	{
		get
		{
			return Validate(field.text);
		}
		set
		{
			SetValue(value);
		}
	}

	private IColorPickerData BuildPickerData()
	{
		return new PickerData(field, this, usesAlpha);
	}

	private void Awake()
	{
		colorButton.onClick.AddListener(delegate
		{
			colorPickerPopup.Show(BuildPickerData());
		});
		field.onEndEdit.AddListener(delegate(string newValue)
		{
			SetValue(newValue);
			onChange.Invoke(value);
		});
		value = defaultValue;
	}

	private void SetValue(string newValue)
	{
		newValue = Validate(newValue);
		field.text = newValue;
		sample.color = newValue.HexToColor();
	}

	private string Validate(string newValue)
	{
		if (!ColorUtility.TryParseHtmlString("#" + newValue, out var color))
		{
			color = defaultValue.HexToColor();
		}
		return color.ToHex(usesAlpha, hash: false).ToLower();
	}
}
