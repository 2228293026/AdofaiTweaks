namespace ADOFAI.Editor.Actions;

public class CycleNextSelectedEventEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override string descriptionKey => "CycleSelectedEvent";

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.levelEventsPanel.CycleSelectedEventTab(next: false);
		}
	}
}
