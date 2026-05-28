using TMPro;
using UnityEngine;

namespace ADOFAI.Editor.Preferences.Controls;

public class EditorsPreferencesText : EditorPreferencesValueControl<string>
{
	public TMP_InputField Input;

	public override void Instantiate(RectTransform transform)
	{
		Input = Object.Instantiate(Resources.Load<TMP_InputField>("LevelEditor/SettingsControls/TextControl"), transform);
		Input.onEndEdit.AddListener(delegate(string value)
		{
			OnValueChange(value);
			NotifyChange();
		});
	}

	public EditorsPreferencesText()
		: base((Get)null, (Set)null)
	{
	}
}
