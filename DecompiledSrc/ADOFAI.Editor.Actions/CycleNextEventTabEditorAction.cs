namespace ADOFAI.Editor.Actions;

public class CycleNextEventTabEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override string descriptionKey => "CycleEventTab";

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.levelEventsPanel.CycleTabs(selectNext: true);
		}
	}
}
