using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Slider : PropertyControl
{
	public PropertyControl_Text inputField;

	public Slider slider;

	private bool isInt;

	private bool draggingSlider;

	public TMP_InputField.SubmitEvent onEndEdit => inputField.onEndEdit;

	public override List<Selectable> selectables => inputField.selectables;

	public override string text
	{
		get
		{
			return inputField.Validate();
		}
		set
		{
			inputField.text = value;
		}
	}

	public override void Setup(bool addListener)
	{
		inputField.propertyInfo = propertyInfo;
		inputField.propertiesPanel = propertiesPanel;
		inputField.parentControl = this;
		inputField.Setup(addListener);
		isInt = propertyInfo.type == PropertyType.Int;
		slider.minValue = (isInt ? ((float)propertyInfo.int_min) : propertyInfo.float_min);
		slider.maxValue = (isInt ? ((float)propertyInfo.int_max) : propertyInfo.float_max);
		slider.onValueChanged.AddListener(delegate(float v)
		{
			v = SliderUtils.GetRoundedValue(v);
			slider.value = v;
			if (draggingSlider)
			{
				inputField.text = $"{v}";
			}
			UpdateSliderColor(v);
		});
	}

	private void UpdateSliderColor(float value)
	{
		bool outOfRange = value < slider.minValue || value > slider.maxValue;
		ColorBlock colors = slider.colors;
		colors.normalColor = colors.normalColor.GetHandleColor(outOfRange);
		colors.disabledColor = colors.disabledColor.GetHandleColor(outOfRange);
		colors.highlightedColor = colors.highlightedColor.GetHandleColor(outOfRange);
		colors.pressedColor = colors.pressedColor.GetHandleColor(outOfRange);
		colors.selectedColor = colors.selectedColor.GetHandleColor(outOfRange);
		slider.colors = colors;
	}

	public void UpdateSliderValue(object parsedValue)
	{
		float value = (isInt ? ((float)(int)parsedValue) : ((float)parsedValue));
		slider.value = value;
		UpdateSliderColor(value);
	}

	public void OnSliderPointerDown()
	{
		draggingSlider = true;
		inputField.text = $"{SliderUtils.GetRoundedValue(slider.value)}";
	}

	public void OnSliderPointerUp()
	{
		draggingSlider = false;
		inputField.onEndEdit.Invoke(inputField.text);
	}
}
