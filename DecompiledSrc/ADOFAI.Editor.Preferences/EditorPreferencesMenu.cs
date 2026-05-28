using System;
using System.Collections.Generic;
using ADOFAI.Editor.Preferences.Controls;
using ADOFAI.Editor.Preferences.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.Editor.Preferences;

public class EditorPreferencesMenu : MonoBehaviour
{
	private delegate void AddCategoryDelegate(EditorPreferencesCategory category);

	public RectTransform tabList;

	public RectTransform contentArea;

	public EditorPreferencesTabButton tabButtonTemplate;

	public EditorPreferencesTabContent tabContentTemplate;

	public Button closeButton;

	[NonSerialized]
	public readonly List<(EditorPreferencesTabButton, EditorPreferencesTabContent)> Tabs = new List<(EditorPreferencesTabButton, EditorPreferencesTabContent)>();

	public readonly List<EditorPreferencesCategory> Categories = new List<EditorPreferencesCategory>();

	private void Awake()
	{
		tabButtonTemplate.gameObject.SetActive(value: false);
		tabContentTemplate.gameObject.SetActive(value: false);
		closeButton.onClick.AddListener(delegate
		{
			scnEditor.instance.HidePreferences();
		});
		SetupMenu();
		GenerateUI();
		if (Tabs.Count > 0)
		{
			SelectCategory(0);
		}
	}

	private void Update()
	{
		if (RDInput.cancelPress && !scnEditor.instance.userIsEditingAnInputField)
		{
			scnEditor.instance.HidePreferences();
		}
	}

	private void SetupMenu()
	{
		AddCategory("general", delegate(EditorPreferencesCategory category)
		{
			category.AddEntry(new EditorPreferencesEntry("markFloorWithComment", new EditorPreferencesToggle(() => Persistence.markFloorWithComment, delegate(bool v)
			{
				Persistence.markFloorWithComment = v;
				scnEditor.instance.RemakePath();
			})));
			category.AddEntry(new EditorPreferencesEntry("disableRewindButton", new EditorPreferencesToggle(Persistence.GetDisableRewindButton, delegate(bool disable)
			{
				bool disableRewindButton = Persistence.GetDisableRewindButton();
				Persistence.disableRewindButton = disable;
				scnEditor instance = scnEditor.instance;
				instance.rewind.gameObject.SetActive(!disable);
				if (disableRewindButton && !disable)
				{
					instance.playPause.transform.position += new Vector3(38.5f, 0f);
				}
				else if (!disableRewindButton && disable)
				{
					instance.playPause.transform.position -= new Vector3(38.5f, 0f);
				}
			})));
			category.AddEntry(new EditorPreferencesEntry("useLegacyZoom", new EditorPreferencesToggle(() => Persistence.editorUseLegacyZoom, delegate(bool enable)
			{
				Persistence.editorUseLegacyZoom = enable;
			})));
			category.AddEntry(new EditorPreferencesEntry("disableEventsPageRepeat", new EditorPreferencesToggle(() => Persistence.disableEventsPageRepeat, delegate(bool enable)
			{
				Persistence.disableEventsPageRepeat = enable;
			})));
			category.AddEntry(new EditorPreferencesEntry("disableAutoAngleOffset", new EditorPreferencesToggle(() => Persistence.disableAutoAngleOffset, delegate(bool enable)
			{
				Persistence.disableAutoAngleOffset = enable;
			})));
			category.AddEntry(new EditorPreferencesEntry("disableCameraDecorationFocus", new EditorPreferencesToggle(() => Persistence.disableCameraDecorationFocus, delegate(bool v)
			{
				Persistence.disableCameraDecorationFocus = v;
			})));
		});
	}

	private void AddCategory(string categoryName, AddCategoryDelegate callback)
	{
		EditorPreferencesCategory editorPreferencesCategory = new EditorPreferencesCategory(categoryName);
		callback(editorPreferencesCategory);
		Categories.Add(editorPreferencesCategory);
	}

	private void GenerateUI()
	{
		for (int i = 0; i < Categories.Count; i++)
		{
			EditorPreferencesCategory category = Categories[i];
			EditorPreferencesTabButton editorPreferencesTabButton = GenerateTabButton(category);
			EditorPreferencesTabContent item = GenerateTabContent(category);
			Tabs.Add((editorPreferencesTabButton, item));
			int j = i;
			editorPreferencesTabButton.button.onClick.AddListener(delegate
			{
				SelectCategory(j);
			});
		}
	}

	private EditorPreferencesTabContent GenerateTabContent(EditorPreferencesCategory category)
	{
		EditorPreferencesTabContent editorPreferencesTabContent = UnityEngine.Object.Instantiate(tabContentTemplate, contentArea);
		editorPreferencesTabContent.gameObject.SetActive(value: true);
		category.GenerateSettingsUI(editorPreferencesTabContent);
		return editorPreferencesTabContent;
	}

	private EditorPreferencesTabButton GenerateTabButton(EditorPreferencesCategory category)
	{
		EditorPreferencesTabButton editorPreferencesTabButton = UnityEngine.Object.Instantiate(tabButtonTemplate, tabList);
		editorPreferencesTabButton.gameObject.SetActive(value: true);
		editorPreferencesTabButton.text.text = RDString.Get("editor.prefs.tabs." + category.Name);
		return editorPreferencesTabButton;
	}

	public void SelectCategory(int index)
	{
		for (int i = 0; i < Tabs.Count; i++)
		{
			(EditorPreferencesTabButton, EditorPreferencesTabContent) tuple = Tabs[i];
			EditorPreferencesTabButton item = tuple.Item1;
			EditorPreferencesTabContent item2 = tuple.Item2;
			bool flag = index == i;
			item.SetSelected(flag);
			item2.gameObject.SetActive(flag);
		}
	}
}
