namespace ADOFAI.Editor.Actions;

public class PasteTrackColorSingleTileEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.PasteTrackColorSingleTile(editor.selectedFloors[0].seqID);
		}
	}
}
