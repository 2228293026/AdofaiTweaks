namespace ADOFAI.Editor.Actions;

public class SelectToFirstFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.MultiSelectFloors(editor.selectedFloors[0], editor.floors[0], setSelectPoint: true);
			}
			else
			{
				editor.MultiSelectFloors(editor.floors[0], editor.multiSelectPoint);
			}
		}
	}
}
