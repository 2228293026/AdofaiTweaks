namespace ADOFAI.Editor.Actions;

public class CopyTrackColorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.CopyTrackColor(editor.selectedFloors[0].seqID);
		}
	}
}
