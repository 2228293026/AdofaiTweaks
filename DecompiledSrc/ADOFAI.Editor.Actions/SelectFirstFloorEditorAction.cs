namespace ADOFAI.Editor.Actions;

public class SelectFirstFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		editor.SelectFirstFloor();
	}
}
