using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Vector2 : PropertyControl
{
	public TMP_InputField inputX;

	public TMP_InputField inputY;

	public TMP_Text unitX;

	public TMP_Text unitY;

	private Rect startRect;

	private Vector2 lastValue = Vector2.zero;

	public override List<Selectable> selectables => new List<Selectable> { inputX, inputY };

	public override string text
	{
		get
		{
			return Validate(inputX, inputY).ToString();
		}
		set
		{
			Vector2 vector = (lastValue = RDUtils.StringToVector2(value));
			inputX.text = ConvertNaNToEmpty(vector.x.ToString("0.######"));
			inputY.text = ConvertNaNToEmpty(vector.y.ToString("0.######"));
		}
	}

	public void Awake()
	{
		if (rectTransform != null)
		{
			startRect = rectTransform.rect;
		}
	}

	public override void SetRandomLayout()
	{
		RandomMode randomMode = (RandomMode)propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.randModeKey];
		if (randomControl != null && rectTransform != null)
		{
			if (randomMode != RandomMode.None)
			{
				randomControl.gameObject.SetActive(value: true);
				rectTransform.anchorMin = rectTransform.anchorMin.WithY(0.4f);
				rectTransform.rect.Set(startRect.x, startRect.y + 2.4f, startRect.width, startRect.height);
			}
			else
			{
				randomControl.gameObject.SetActive(value: false);
				rectTransform.anchorMin = rectTransform.anchorMin.WithY(0f);
				rectTransform.rect.Set(startRect.x, startRect.y, startRect.width, startRect.height);
			}
		}
	}

	public (string, string) Validate(TMP_InputField x, TMP_InputField y)
	{
		Vector2 vector = new Vector2(lastValue.x, lastValue.y);
		string text = ConvertEmptyToNaN(x.text);
		string text2 = ConvertEmptyToNaN(y.text);
		if (float.TryParse(text, out var result) && float.TryParse(text2, out var result2))
		{
			vector = new Vector2(result, result2);
			vector = propertyInfo.Validate(vector, propertiesPanel.inspectorPanel.selectedEvent.isFake);
		}
		else
		{
			DataTable dataTable = new DataTable();
			try
			{
				object dictValue = dataTable.Compute(text, "");
				vector.x = RDEditorUtils.DecodeFloat(dictValue);
			}
			catch
			{
			}
			try
			{
				object dictValue2 = dataTable.Compute(text2, "");
				vector.y = RDEditorUtils.DecodeFloat(dictValue2);
			}
			catch
			{
			}
		}
		if (propertiesPanel.inspectorPanel.selectedEvent.eventType == LevelEventType.AddDecoration && propertyInfo.name == "tile")
		{
			vector.x = Mathf.RoundToInt(vector.x);
			vector.y = Mathf.RoundToInt(vector.y);
		}
		string item = ConvertNaNToEmpty(vector.x.ToString("0.######"));
		string item2 = ConvertNaNToEmpty(vector.y.ToString("0.######"));
		return (item, item2);
	}

	public override void ValidateInput()
	{
		(inputX.text, inputY.text) = Validate(inputX, inputY);
	}

	public override void SetEnabled(bool enabled, bool shown = true)
	{
		base.SetEnabled(enabled, shown);
		Color color = (enabled ? Color.gray : Color.gray.WithAlpha(0.5f));
		inputX.placeholder.GetComponent<TMP_Text>().color = color;
		inputY.placeholder.GetComponent<TMP_Text>().color = color;
	}

	public override void Setup(bool addListener)
	{
		if (addListener)
		{
			inputX.onEndEdit.AddListener(delegate
			{
				SetVectorVals();
			});
			inputY.onEndEdit.AddListener(delegate
			{
				SetVectorVals();
			});
		}
		if (!string.IsNullOrEmpty(propertyInfo.unit))
		{
			unitX.gameObject.SetActive(value: true);
			unitX.text = RDString.Get("editor.unit." + propertyInfo.unit);
			unitY.gameObject.SetActive(value: true);
			unitY.text = RDString.Get("editor.unit." + propertyInfo.unit);
		}
		ColorBlock colors = inputX.colors;
		colors.selectedColor = InspectorPanel.selectionColor;
		inputX.colors = colors;
		inputY.colors = colors;
	}

	public void SetVectorVals()
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			ValidateInput();
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			string s = ConvertEmptyToNaN(inputX.text);
			string s2 = ConvertEmptyToNaN(inputY.text);
			float x = float.Parse(s);
			float y = float.Parse(s2);
			Vector2 vector = new Vector2(x, y);
			selectedEvent[propertyInfo.name] = vector;
			ToggleOthersEnabled();
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
			OnValueChange();
		}
	}

	private string ConvertNaNToEmpty(string s)
	{
		if (s == "NaN")
		{
			return "";
		}
		return s;
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
