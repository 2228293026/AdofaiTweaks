using System.Collections.Generic;
using UnityEngine;

namespace ADOFAI.Editor.Preferences.Models;

public class EditorPreferencesCategory
{
	public string Name;

	public readonly List<EditorPreferencesEntry> Entries = new List<EditorPreferencesEntry>();

	public EditorPreferencesCategory(string name)
	{
		Name = name;
	}

	public void AddEntry(EditorPreferencesEntry entry)
	{
		Entries.Add(entry);
	}

	public void GenerateSettingsUI(EditorPreferencesTabContent content)
	{
		foreach (EditorPreferencesEntry entry in Entries)
		{
			EditorPreferencesField editorPreferencesField = Object.Instantiate(entry.Control.controlType switch
			{
				EditorPreferencesControlType.Horizontal => content.horizontalFieldTemplate, 
				EditorPreferencesControlType.Vertical => content.verticalFieldTemplate, 
				_ => null, 
			}, content.rt);
			editorPreferencesField.label.text = RDString.Get("editor.prefs.fields." + entry.Name);
			bool exists;
			string withCheck = RDString.GetWithCheck("editor.prefs.fields." + entry.Name + ".description", out exists);
			editorPreferencesField.description.gameObject.SetActive(exists);
			if (exists)
			{
				editorPreferencesField.description.text = withCheck;
			}
			entry.Control.Instantiate(editorPreferencesField.control);
		}
	}
}
