namespace ADOFAI.Editor.Actions;

public class ShowPreviousEventPageEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override string descriptionKey => "CycleEventsPage";

	public override void Execute(scnEditor editor)
	{
		editor.ShowPrevPage();
	}
}
