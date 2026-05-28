namespace ADOFAI.Editor.Actions;

public class RedoEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		editor.Redo();
	}
}
