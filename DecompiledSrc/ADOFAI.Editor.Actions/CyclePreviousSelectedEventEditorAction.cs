namespace ADOFAI.Editor.Actions;

public class CyclePreviousSelectedEventEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override string descriptionKey => "CycleSelectedEvent";

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.levelEventsPanel.CycleSelectedEventTab(next: true);
		}
	}
}
