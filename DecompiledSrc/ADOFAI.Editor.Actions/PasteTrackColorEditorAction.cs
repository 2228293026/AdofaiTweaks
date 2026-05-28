namespace ADOFAI.Editor.Actions;

public class PasteTrackColorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.PasteTrackColor(editor.selectedFloors[0].seqID);
		}
	}
}
