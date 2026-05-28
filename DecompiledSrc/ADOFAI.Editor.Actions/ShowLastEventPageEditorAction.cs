namespace ADOFAI.Editor.Actions;

public class ShowLastEventPageEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.ShowNextPage(moveToLast: true);
	}
}
