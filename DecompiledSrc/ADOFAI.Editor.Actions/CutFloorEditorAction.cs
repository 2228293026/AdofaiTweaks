namespace ADOFAI.Editor.Actions;

public class CutFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.CutFloor(editor.selectedFloors[0]);
			}
			else
			{
				editor.MultiCutFloors();
			}
		}
		else if (!editor.SelectionDecorationIsEmpty())
		{
			if (editor.SelectionDecorationIsSingle())
			{
				editor.CutDecoration(editor.selectedDecorations[0]);
			}
			else
			{
				editor.MultiCutDecorations();
			}
		}
	}
}
