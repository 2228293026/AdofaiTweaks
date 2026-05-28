using ADOFAI.Editor.Components;
using UnityEngine;

namespace ADOFAI.Editor.Preferences.Controls;

public class EditorPreferencesToggle : EditorPreferencesValueControl<bool>
{
	public Switch Switch;

	public EditorPreferencesToggle(Get getter, Set setter)
		: base(getter, setter)
	{
	}

	public override void Instantiate(RectTransform transform)
	{
		Switch = Object.Instantiate(Resources.Load<GameObject>("LevelEditor/SettingsControls/ToggleControl"), transform).GetComponentInChildren<Switch>();
		if (Getter != null)
		{
			Switch.SetValue(Getter(), immediate: true);
		}
		Switch.onToggle.AddListener(delegate(bool check)
		{
			OnValueChange(check);
			NotifyChange();
		});
	}
}
