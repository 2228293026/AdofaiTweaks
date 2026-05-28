namespace ADOFAI.Editor.Actions;

public class ZoomInUiEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.ZoomInUI();
	}
}
