namespace ADOFAI.Editor.Actions;

public class DuplicateDecorationsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionDecorationIsEmpty())
		{
			editor.DuplicateDecorations();
		}
	}
}
