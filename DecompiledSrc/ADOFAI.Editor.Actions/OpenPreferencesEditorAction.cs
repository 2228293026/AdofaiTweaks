namespace ADOFAI.Editor.Actions;

public class OpenPreferencesEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.ShowPreferences();
	}
}
