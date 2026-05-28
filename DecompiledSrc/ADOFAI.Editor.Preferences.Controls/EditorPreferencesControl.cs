using ADOFAI.Editor.Preferences.Models;
using UnityEngine;

namespace ADOFAI.Editor.Preferences.Controls;

public abstract class EditorPreferencesControl
{
	public EditorPreferencesEntry Entry;

	public virtual EditorPreferencesControlType controlType => EditorPreferencesControlType.Horizontal;

	protected void NotifyChange()
	{
		Entry.NotifyChange(this);
	}

	public abstract void Instantiate(RectTransform transform);
}
