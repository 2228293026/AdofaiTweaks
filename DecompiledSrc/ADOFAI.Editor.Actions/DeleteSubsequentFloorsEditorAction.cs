namespace ADOFAI.Editor.Actions;

public class DeleteSubsequentFloorsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		editor.DeleteSubsequentFloors();
	}
}
