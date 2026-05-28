namespace ADOFAI.Editor.Actions;

public class DeleteFloorsEditorAction : EditorAction
{
	private bool backwards;

	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public DeleteFloorsEditorAction(bool backwards)
	{
		this.backwards = backwards;
	}

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.DeleteSingleSelection(backwards);
		}
		else if (!editor.SelectionIsEmpty())
		{
			editor.DeleteMultiSelection(backwards);
		}
	}
}
