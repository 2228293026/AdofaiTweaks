namespace ADOFAI.Editor.Actions;

public class UndoEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		editor.Undo();
	}
}
