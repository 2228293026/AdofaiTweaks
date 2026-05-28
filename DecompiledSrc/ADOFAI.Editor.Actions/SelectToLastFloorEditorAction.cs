namespace ADOFAI.Editor.Actions;

public class SelectToLastFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.MultiSelectFloors(editor.selectedFloors[0], editor.floors[editor.floors.Count - 1], setSelectPoint: true);
			}
			else
			{
				editor.MultiSelectFloors(editor.floors[editor.floors.Count - 1], editor.multiSelectPoint);
			}
		}
	}
}
