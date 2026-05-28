namespace ADOFAI.Editor.Actions;

public class ShowNextEventPageEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override string descriptionKey => "CycleEventsPage";

	public override void Execute(scnEditor editor)
	{
		editor.ShowNextPage();
	}
}
