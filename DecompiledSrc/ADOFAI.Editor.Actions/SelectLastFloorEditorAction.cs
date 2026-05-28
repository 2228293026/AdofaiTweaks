namespace ADOFAI.Editor.Actions;

public class SelectLastFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		editor.SelectFloor(editor.floors[editor.floors.Count - 1]);
	}
}
