using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Text : PropertyControl
{
	public TMP_InputField inputField;

	public TMP_Text unit;

	public Button goToFloorButton;

	public Button linkFloorIDButton;

	public PropertyControl parentControl;

	public TMP_InputField.SubmitEvent onEndEdit => inputField.onEndEdit;

	public override List<Selectable> selectables => new List<Selectable> { inputField };

	public override string text
	{
		get
		{
			return Validate();
		}
		set
		{
			inputField.text = value;
		}
	}

	private void Awake()
	{
		if (!(linkFloorIDButton == null))
		{
			goToFloorButton.onClick.AddListener(delegate
			{
				GoToFloorButton();
			});
			linkFloorIDButton.onClick.AddListener(delegate
			{
				SelectFloorIDToggle();
			});
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
				rectTransform.anchorMax = rectTransform.anchorMax.WithX(0.48f);
			}
			else
			{
				randomControl.gameObject.SetActive(value: false);
				rectTransform.anchorMax = rectTransform.anchorMax.WithX(1f);
			}
		}
	}

	private void Update()
	{
		if (!(linkFloorIDButton == null) && linkFloorIDButton.gameObject.activeSelf)
		{
			Color editingColor = scnEditor.instance.editingColor;
			Color defaultButtonColor = scnEditor.instance.defaultButtonColor;
			linkFloorIDButton.image.color = (scnEditor.selectingFloorID ? editingColor : defaultButtonColor);
		}
	}

	public string Validate()
	{
		if (propertyInfo == null)
		{
			return inputField.text;
		}
		if (propertyInfo.type == PropertyType.Float)
		{
			float result = 1f;
			if (float.TryParse(inputField.text, out result))
			{
				result = propertyInfo.Validate(result);
			}
			else
			{
				DataTable dataTable = new DataTable();
				try
				{
					object dictValue = dataTable.Compute(inputField.text, "");
					result = propertyInfo.Validate(RDEditorUtils.DecodeFloat(dictValue));
				}
				catch
				{
					result = (float)propertyInfo.value_default;
				}
			}
			return result.ToString();
		}
		if (propertyInfo.type == PropertyType.Int || propertyInfo.type == PropertyType.Tile)
		{
			int num = 1;
			if (float.TryParse(inputField.text, out var result2))
			{
				num = Mathf.RoundToInt(result2);
				num = propertyInfo.Validate(num);
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				try
				{
					num = RDEditorUtils.DecodeInt(dataTable2.Compute(inputField.text, ""));
				}
				catch
				{
					num = (int)propertyInfo.value_default;
				}
			}
			return num.ToString();
		}
		return inputField.text;
	}

	public override void ValidateInput()
	{
		inputField.text = Validate();
	}

	public override void Setup(bool addListener)
	{
		ColorBlock colors = inputField.colors;
		colors.selectedColor = InspectorPanel.selectionColor;
		inputField.colors = colors;
		if (addListener)
		{
			if (propertyInfo.name == "artist")
			{
				inputField.onValueChanged.AddListener(delegate(string s)
				{
					ADOBase.editor.settingsPanel.ToggleArtistPopup(s, rectTransform.position.y, this);
					ToggleOthersEnabled();
					OnValueChange();
				});
			}
			inputField.onEndEdit.AddListener(delegate
			{
				if (propertyInfo.name == "artist")
				{
					return;
				}
				using (new SaveStateScope(ADOBase.editor))
				{
					ValidateInput();
					LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
					PropertyType type = propertyInfo.type;
					string text = inputField.text;
					string text2 = propertyInfo.name;
					object obj = null;
					switch (type)
					{
					case PropertyType.Int:
						obj = int.Parse(text);
						break;
					case PropertyType.Float:
						obj = float.Parse(text);
						break;
					case PropertyType.String:
						obj = text;
						break;
					case PropertyType.Tile:
					{
						Tuple<int, TileRelativeTo> tile = selectedEvent.GetTile(text2);
						obj = new Tuple<int, TileRelativeTo>(int.Parse(text), tile.Item2);
						break;
					}
					}
					if (text2 == "floor")
					{
						selectedEvent.floor = (int)obj;
					}
					else if (propertyInfo.name == "angleOffset" && selectedEvent.eventType == LevelEventType.SetSpeed)
					{
						float num = (float)ADOBase.editor.selectedFloors[0].entryangle;
						float num2 = (float)ADOBase.editor.selectedFloors[0].exitangle;
						float num3 = Mathf.Round((float)scrMisc.GetAngleMoved(num, num2, !ADOBase.editor.selectedFloors[0].isCCW) * 57.29578f);
						if (num3 <= Mathf.Pow(10f, -6f) && ADOBase.lm.leveldata[selectedEvent.floor] != '!')
						{
							num3 = 360f;
						}
						obj = Mathf.Clamp((float)obj, 0f, num3);
						selectedEvent[propertyInfo.name] = obj;
						ADOBase.editor.levelEventsPanel.ShowPanelOfEvent(selectedEvent);
					}
					else
					{
						selectedEvent[text2] = obj;
					}
					ToggleOthersEnabled();
					if (propertyInfo.slider)
					{
						((PropertyControl_Slider)parentControl).UpdateSliderValue(obj);
					}
					if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
					{
						ADOBase.customLevel.SetBackground();
					}
					else if (selectedEvent.IsDecoration)
					{
						ADOBase.editor.UpdateDecorationObject(selectedEvent);
					}
					ApplyTileChanges();
					if (ADOBase.editor.SelectionIsSingle())
					{
						ADOBase.editor.ShowEventIndicators(ADOBase.editor.selectedFloors[0]);
					}
					OnValueChange();
				}
			});
		}
		if (!string.IsNullOrEmpty(propertyInfo.unit))
		{
			unit.gameObject.SetActive(value: true);
			unit.text = RDString.Get("editor.unit." + propertyInfo.unit);
		}
	}

	private void GoToFloorButton()
	{
		if (int.TryParse(text, out var result))
		{
			scnEditor.instance.SelectFloor(scnEditor.instance.floors[result]);
		}
	}

	private void SelectFloorIDToggle()
	{
		scnEditor.selectingFloorID = !scnEditor.selectingFloorID;
		scnEditor.instance.selectingFloorIDTextMoving = false;
	}
}
