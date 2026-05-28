using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Toggle : PropertyControl
{
	public GameObject enumButtonPrefab;

	public TweakableDropdown dropdown;

	public GameObject dropdownBorder;

	public Image dropdownArrow;

	public GameObject browseSection;

	public BrowseButton browseButton;

	public Sprite selectedToggleBackground;

	public Sprite unselectedToggleBackground;

	public List<string> enumValList = new List<string>();

	public Dictionary<string, Button> buttonsDict;

	[NonSerialized]
	public List<Selectable> selectablesList = new List<Selectable>();

	public string selected;

	public bool settingText;

	private bool useButtons
	{
		get
		{
			if (enumValList.Count >= 3 || propertyInfo.stringDropdown)
			{
				return propertyInfo.type == PropertyType.Tile;
			}
			return true;
		}
	}

	public override List<Selectable> selectables
	{
		get
		{
			if (!useButtons)
			{
				return new List<Selectable> { dropdown.inputField };
			}
			return selectablesList;
		}
	}

	public override string text
	{
		get
		{
			return selected;
		}
		set
		{
			if (useButtons)
			{
				foreach (string key in buttonsDict.Keys)
				{
					SetButtonEnabled(buttonsDict[key], key == value);
				}
				return;
			}
			settingText = true;
			List<TweakableDropdownItem> items = dropdown.items;
			int num = enumValList.IndexOf(value);
			if (num != -1)
			{
				TweakableDropdownItem targetItem = items[num];
				dropdown.SelectItem(targetItem);
			}
			else
			{
				SetDropdownToBrowsedFile(value);
			}
			settingText = false;
		}
	}

	public override void EnumSetup(string enumTypeString, List<string> enumVals, bool localize = true, List<string> customLabels = null, bool enableBrowse = false)
	{
		bool flag = customLabels?.Count == enumVals.Count;
		enumValList = enumVals;
		((RectTransform)dropdown.transform).SetParent(base.transform, worldPositionStays: false);
		dropdown.gameObject.SetActive(value: true);
		dropdownArrow = dropdown.dropdownButton.image;
		dropdown.enumTypeString = enumTypeString;
		dropdown.localizeEnumStrings = localize;
		dropdown.overwriteTextWithNoneSelected = !enableBrowse;
		dropdown.useCustomLabels = !localize && flag;
		dropdown.customLabels = customLabels;
		if (useButtons)
		{
			base.gameObject.AddComponent<HorizontalLayoutGroup>();
			dropdown.gameObject.SetActive(value: false);
			dropdownBorder.SetActive(value: false);
			buttonsDict = new Dictionary<string, Button>();
			float num = 0f;
			int num2 = 250 / enumValList.Count;
			for (int i = 0; i < enumValList.Count; i++)
			{
				string enumVar = enumValList[i];
				GameObject obj = UnityEngine.Object.Instantiate(enumButtonPrefab, base.transform);
				TMP_Text componentInChildren = obj.GetComponentInChildren<TMP_Text>();
				if (localize)
				{
					componentInChildren.text = RDString.GetEnumValue(enumTypeString, enumVar);
				}
				else if (flag)
				{
					componentInChildren.text = customLabels[i];
				}
				else
				{
					componentInChildren.text = enumVar;
				}
				RectTransform component = obj.GetComponent<RectTransform>();
				component.AnchorPosX(num);
				component.sizeDelta = new Vector2(num2, component.sizeDelta.y);
				num += (float)num2;
				Button component2 = obj.GetComponent<Button>();
				buttonsDict.Add(enumVar, component2);
				selectablesList.Add(component2);
				component2.onClick.AddListener(delegate
				{
					SelectVar(enumVar);
				});
			}
		}
		else
		{
			dropdown.itemValues.Clear();
			dropdown.itemValues.AddRange(enumValList);
			dropdown.ReloadList();
			for (int num3 = 0; num3 < dropdown.itemValues.Count; num3++)
			{
				string value = enumVals[num3];
				dropdown.items[num3].value = value;
			}
			dropdown.onValueChanged = delegate(TweakableDropdownItem selectedItem)
			{
				if (selectedItem != null)
				{
					SelectVar(enumVals[selectedItem.index]);
				}
			};
			dropdownBorder.SetActive(value: true);
			RectTransform rectTransform = (RectTransform)dropdown.transform;
			if (enableBrowse)
			{
				rectTransform.offsetMax = new Vector2(-45f, 0f);
				browseSection.SetActive(value: true);
				browseButton.Initialize(this, dropdown.inputField, ProcessFile);
			}
			else
			{
				rectTransform.offsetMax = new Vector2(0f, 0f);
				browseSection.SetActive(value: false);
			}
		}
		ColorBlock colors = dropdown.inputField.colors;
		colors.selectedColor = InspectorPanel.selectionColor;
		dropdown.inputField.colors = colors;
		foreach (Selectable selectables in selectablesList)
		{
			selectables.colors = colors;
		}
	}

	public override void Setup(bool addListener)
	{
	}

	private void Update()
	{
		if (dropdownArrow != null)
		{
			dropdownArrow.color = Color.white.WithAlpha(dropdown.interactable ? 1f : 0.3f);
		}
	}

	private void SetDropdownToBrowsedFile(string browsedFile)
	{
		dropdown.textToOverwrite = browsedFile;
		dropdown.SelectItem(null);
	}

	public void SelectVar(string var)
	{
		if (settingText)
		{
			return;
		}
		using (new SaveStateScope(ADOBase.editor))
		{
			if (buttonsDict != null && buttonsDict.Count > 0)
			{
				foreach (string key in buttonsDict.Keys)
				{
					SetButtonEnabled(buttonsDict[key], key == var);
				}
			}
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			selected = var;
			Type enumType = propertyInfo.enumType;
			if (propertyInfo.type == PropertyType.Tile)
			{
				Tuple<int, TileRelativeTo> tile = selectedEvent.GetTile(propertyInfo.name);
				selectedEvent[propertyInfo.name] = new Tuple<int, TileRelativeTo>(tile.Item1, (TileRelativeTo)Enum.Parse(enumType, var));
			}
			else if (propertyInfo.stringDropdown)
			{
				selectedEvent[propertyInfo.name] = var;
			}
			else
			{
				selectedEvent[propertyInfo.name] = Enum.Parse(enumType, var);
			}
			ToggleOthersEnabled();
			if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
			{
				ADOBase.customLevel.SetBackground();
			}
			else if (selectedEvent.IsDecoration)
			{
				ADOBase.editor.UpdateDecorationObject(selectedEvent);
			}
			else if (selectedEvent.eventType == LevelEventType.SetFilterAdvanced)
			{
				PropertyControl_FilterProperties obj = propertiesPanel.properties["filterProperties"].control as PropertyControl_FilterProperties;
				obj.enableProperties = true;
				obj.ReloadFilterProperties(selectedEvent);
			}
			ApplyTileChanges();
			OnValueChange();
		}
	}

	public void SetButtonEnabled(Button button, bool enabled)
	{
		button.image.sprite = (enabled ? selectedToggleBackground : unselectedToggleBackground);
		button.GetComponentInChildren<TMP_Text>().color = (enabled ? Color.black : Color.white);
	}

	public override void OnSelectedEventChanged(LevelEvent levelEvent)
	{
		if (useButtons || dropdown.arrowSelectedDropdownItems.Count == 0)
		{
			return;
		}
		for (int i = 0; i < dropdown.arrowSelectedDropdownItems.Count; i++)
		{
			TweakableDropdownItem tweakableDropdownItem = dropdown.arrowSelectedDropdownItems[i];
			if (!(tweakableDropdownItem == dropdown.selectedItem) && (bool)tweakableDropdownItem)
			{
				tweakableDropdownItem.OnArrowSelect(selected: false);
			}
		}
	}

	public void OggEncodeCallback(string resultName)
	{
		dropdown.SelectItem(null);
		browseButton.filename = resultName;
		propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.name] = resultName;
		dropdown.inputField.text = resultName;
	}

	public void ProcessFile(string newFilename, FileType fileType)
	{
		if (newFilename.IsNullOrEmpty())
		{
			return;
		}
		if (newFilename != "")
		{
			if (!string.IsNullOrEmpty(ADOBase.levelPath))
			{
				if (!File.Exists(Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), newFilename)))
				{
					return;
				}
			}
			else if (dropdown.enabledItems.Count != 0)
			{
				return;
			}
		}
		if (!browseButton.CheckIfLevelIsSaved())
		{
			return;
		}
		if (fileType == FileType.Audio)
		{
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			browseButton.filename = newFilename;
			if (Path.GetExtension(browseButton.filename).Replace(".", string.Empty) == "mp3")
			{
				ADOBase.editor.soundToConvert = browseButton.filename;
				ADOBase.editor.soundConversionCallback = OggEncodeCallback;
				ADOBase.editor.ShowPopup(show: true, scnEditor.PopupType.OggEncode);
				return;
			}
			selectedEvent[propertyInfo.name] = browseButton.filename;
			SetDropdownToBrowsedFile(browseButton.filename);
		}
		OnValueChange();
	}
}
