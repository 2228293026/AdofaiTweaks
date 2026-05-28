namespace ADOFAI.Editor.Actions;

public class CopyAllSameTypeEventsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.CopyFloor(editor.selectedFloors[0], clearClipboard: true, cut: false, selectedEventOnly: true, allSameTypeEvents: true);
		}
	}
}
