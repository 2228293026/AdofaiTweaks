namespace ADOFAI.Editor.Actions;

public class DeselectAllEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty() || !editor.SelectionDecorationIsEmpty())
		{
			editor.DeselectFloors();
			editor.DeselectAllDecorations();
		}
	}
}
