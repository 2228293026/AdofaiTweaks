namespace ADOFAI.Editor.Actions;

public class TryQuitToMenuEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.TryQuitToMenu();
	}
}
