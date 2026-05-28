namespace ADOFAI.Editor.Actions;

public class DeletePrecedingFloorsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		editor.DeletePrecedingFloors();
	}
}
