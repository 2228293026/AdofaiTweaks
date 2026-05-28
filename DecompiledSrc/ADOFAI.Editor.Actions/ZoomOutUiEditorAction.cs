namespace ADOFAI.Editor.Actions;

public class ZoomOutUiEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.ZoomOutUI();
	}
}
