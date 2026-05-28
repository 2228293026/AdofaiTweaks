namespace ADOFAI.Editor.Actions;

public class CutEventsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.CutFloor(editor.selectedFloors[0], clearClipboard: true, selectedEventOnly: true);
		}
	}
}
