namespace ADOFAI.Editor.Actions;

public class ToggleShortcutsPanelEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Other;

	public override void Execute(scnEditor editor)
	{
		editor.ToggleShortcutsPanel();
	}
}
