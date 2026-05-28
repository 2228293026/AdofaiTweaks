namespace ADOFAI.Editor.Actions;

public class CopyFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.CopyFloor(editor.selectedFloors[0]);
			}
			else
			{
				editor.MultiCopyFloors();
			}
		}
		else if (!editor.SelectionDecorationIsEmpty())
		{
			if (editor.SelectionDecorationIsSingle())
			{
				editor.CopyDecoration(editor.selectedDecorations[0]);
			}
			else
			{
				editor.MultiCopyDecorations();
			}
		}
	}
}
