using System;
using System.Collections.Generic;
using ADOFAI.Editor.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Vector2Range : PropertyControl_FloatPairBase
{
	public MinMaxControl controlX;

	public MinMaxControl controlY;

	public override List<Selectable> selectables => new List<Selectable>
	{
		controlX.startInput.field,
		controlX.endInput.field,
		controlY.startInput.field,
		controlY.endInput.field
	};

	public override void ValidateInput()
	{
		var (text, text2) = Validate(controlX, clamp: false);
		controlX.startInput.field.text = text;
		controlX.endInput.field.text = text2;
		var (text3, text4) = Validate(controlY, clamp: false);
		controlY.startInput.field.text = text3;
		controlY.endInput.field.text = text4;
	}

	public override void SetEnabled(bool enabled, bool shown = true)
	{
		base.SetEnabled(enabled, shown);
		Color color = (enabled ? Color.gray : Color.gray.WithAlpha(0.5f));
		controlX.startInput.field.placeholder.GetComponent<TMP_Text>().color = color;
		controlX.endInput.field.placeholder.GetComponent<TMP_Text>().color = color;
		controlY.startInput.field.placeholder.GetComponent<TMP_Text>().color = color;
		controlY.endInput.field.placeholder.GetComponent<TMP_Text>().color = color;
	}

	public override void Setup(bool addListener)
	{
		if (addListener)
		{
			controlX.onChange.AddListener(delegate
			{
				SetVectorVals();
			});
			controlX.startInput.onEndEdit.AddListener(delegate
			{
				SetVectorVals();
			});
			controlX.endInput.onEndEdit.AddListener(delegate
			{
				SetVectorVals();
			});
			controlY.onChange.AddListener(delegate
			{
				SetVectorVals();
			});
			controlY.startInput.onEndEdit.AddListener(delegate
			{
				SetVectorVals();
			});
			controlY.endInput.onEndEdit.AddListener(delegate
			{
				SetVectorVals();
			});
		}
		if (!string.IsNullOrEmpty(propertyInfo.unit))
		{
			SetUnit(controlX.startInput.unitText);
			SetUnit(controlX.endInput.unitText);
			SetUnit(controlY.startInput.unitText);
			SetUnit(controlY.endInput.unitText);
		}
		SetColorBlock(controlX);
		SetColorBlock(controlY);
		static void SetColorBlock(MinMaxControl field)
		{
			ColorBlock colors = field.startInput.field.colors;
			colors.selectedColor = InspectorPanel.selectionColor;
			field.startInput.field.colors = colors;
			field.endInput.field.colors = colors;
		}
		void SetUnit(TMP_Text label)
		{
			label.gameObject.SetActive(value: true);
			label.text = RDString.Get("editor.unit." + propertyInfo.unit);
		}
	}

	private void SetVectorVals()
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			ValidateInput();
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			string s = ConvertEmptyToNaN(controlX.startInput.field.text);
			string s2 = ConvertEmptyToNaN(controlY.startInput.field.text);
			string s3 = ConvertEmptyToNaN(controlX.endInput.field.text);
			string s4 = ConvertEmptyToNaN(controlY.endInput.field.text);
			float x = float.Parse(s);
			float y = float.Parse(s2);
			float x2 = float.Parse(s3);
			Tuple<Vector2, Vector2> value = new Tuple<Vector2, Vector2>(item2: new Vector2(x2, float.Parse(s4)), item1: new Vector2(x, y));
			selectedEvent[propertyInfo.name] = value;
			ToggleOthersEnabled();
			OnValueChange();
			if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
			{
				ADOBase.customLevel.SetBackground();
			}
			else if (selectedEvent.IsDecoration)
			{
				ADOBase.editor.UpdateDecorationObject(selectedEvent);
			}
			if (selectedEvent.eventType == LevelEventType.PositionTrack || selectedEvent.eventType == LevelEventType.FreeRoam || selectedEvent.eventType == LevelEventType.FreeRoamTwirl || selectedEvent.eventType == LevelEventType.FreeRoamRemove || selectedEvent.eventType == LevelEventType.FreeRoamWarning)
			{
				ADOBase.editor.ApplyEventsToFloors();
				if (ADOBase.editor.SelectionIsSingle())
				{
					ADOBase.editor.floorButtonCanvas.transform.position = ADOBase.editor.selectedFloors[0].transform.position;
				}
			}
		}
	}

	private string ConvertEmptyToNaN(string s)
	{
		if (s == "")
		{
			return "NaN";
		}
		return s;
	}
}
