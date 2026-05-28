namespace ADOFAI.Editor.Actions;

public class ZoomOutCameraEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.ZoomCamera(-0.5f, anchorAtPointer: false);
	}
}
