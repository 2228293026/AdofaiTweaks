using JetBrains.Annotations;

namespace ADOFAI.Editor.Preferences.Controls;

public abstract class EditorPreferencesValueControl<T> : EditorPreferencesControl
{
	public delegate T Get();

	public delegate void Set(T value);

	[CanBeNull]
	protected readonly Get Getter;

	[CanBeNull]
	protected readonly Set Setter;

	public EditorPreferencesValueControl(Get getter = null, Set setter = null)
	{
		Getter = getter;
		Setter = setter;
	}

	protected virtual void OnValueChange(T value)
	{
		Setter?.Invoke(value);
	}
}
