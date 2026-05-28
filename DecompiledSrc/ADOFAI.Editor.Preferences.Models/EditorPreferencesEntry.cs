using ADOFAI.Editor.Preferences.Controls;

namespace ADOFAI.Editor.Preferences.Models;

public class EditorPreferencesEntry
{
	public string Name;

	public EditorPreferencesControl Control;

	public EditorPreferencesEntry(string name, EditorPreferencesControl control)
	{
		Name = name;
		Control = control;
		control.Entry = this;
	}

	public void NotifyChange(EditorPreferencesControl control)
	{
	}
}
