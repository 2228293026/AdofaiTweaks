using System;
using System.Collections.Generic;
using ADOFAI.Editor.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_FloatPair : PropertyControl_FloatPairBase
{
	public MinMaxControl control;

	public override List<Selectable> selectables => new List<Selectable>
	{
		control.startInput.field,
		control.endInput.field
	};

	public override string text
	{
		get
		{
			return Validate(control).ToString();
		}
		set
		{
			Tuple<float, float> tuple = RDUtils.StringToFloatPair(value);
			control.startInput.field.text = tuple.Item1.ToString("0.######");
			control.endInput.field.text = tuple.Item2.ToString("0.######");
		}
	}

	public override void ValidateInput()
	{
		var (text, text2) = Validate(control);
		control.startInput.field.text = text;
		control.endInput.field.text = text2;
	}

	public override void SetEnabled(bool enabled, bool shown = true)
	{
		base.SetEnabled(enabled, shown);
		Color color = (enabled ? Color.gray : Color.gray.WithAlpha(0.5f));
		control.startInput.field.placeholder.GetComponent<TMP_Text>().color = color;
		control.endInput.field.placeholder.GetComponent<TMP_Text>().color = color;
	}

	public override void Setup(bool addListener)
	{
		if (addListener)
		{
			control.onChange.AddListener(delegate(Tuple<float, float> value)
			{
				SaveWithoutRecording(value);
			});
			control.startInput.onEndEdit.AddListener(delegate
			{
				Save(new Tuple<float, float>(control.start, control.end));
			});
			control.endInput.onEndEdit.AddListener(delegate
			{
				Save(new Tuple<float, float>(control.start, control.end));
			});
		}
		if (!string.IsNullOrEmpty(propertyInfo.unit))
		{
			TMP_Text unitText = control.startInput.unitText;
			unitText.gameObject.SetActive(value: true);
			unitText.text = RDString.Get("editor.unit." + propertyInfo.unit);
			TMP_Text unitText2 = control.endInput.unitText;
			unitText2.gameObject.SetActive(value: true);
			unitText2.text = RDString.Get("editor.unit." + propertyInfo.unit);
		}
	}

	private void Save(Tuple<float, float> value)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			SaveWithoutRecording(value);
		}
	}

	private void SaveWithoutRecording(Tuple<float, float> value)
	{
		ValidateInput();
		LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
		selectedEvent[propertyInfo.name] = value;
		ToggleOthersEnabled();
		OnValueChange();
		if (selectedEvent.IsDecoration)
		{
			ADOBase.editor.UpdateDecorationObject(selectedEvent);
		}
	}
}
