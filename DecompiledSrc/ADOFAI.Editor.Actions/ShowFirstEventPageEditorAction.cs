namespace ADOFAI.Editor.Actions;

public class ShowFirstEventPageEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.ShowPrevPage(moveToFirst: true);
	}
}
