namespace ADOFAI.Editor.Actions;

public class PasteHitSoundSingleTileEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.PasteHitsoundSingleTile(editor.selectedFloors[0].seqID);
		}
	}
}
