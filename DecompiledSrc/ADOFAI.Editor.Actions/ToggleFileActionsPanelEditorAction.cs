namespace ADOFAI.Editor.Actions;

public class ToggleFileActionsPanelEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.None;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsEmpty() && editor.SelectionDecorationIsEmpty())
		{
			editor.ToggleFileActionsPanel();
		}
	}
}
