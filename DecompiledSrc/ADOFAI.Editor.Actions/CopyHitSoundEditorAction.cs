namespace ADOFAI.Editor.Actions;

public class CopyHitSoundEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.CopyHitSound(editor.selectedFloors[0].seqID);
		}
	}
}
