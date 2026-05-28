using System.Linq;

namespace ADOFAI.Editor.Actions;

public class MoveSelectionLeftEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override string descriptionKey => "MoveSelection";

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle() && editor.selectedFloors[0].seqID != 0)
			{
				editor.MultiSelectFloors(editor.selectedFloors[0], editor.floors[editor.selectedFloors[0].seqID - 1], setSelectPoint: true);
			}
			else
			{
				editor.MultiSelectFloors((editor.selectedFloors[0].seqID < editor.multiSelectPoint.seqID) ? editor.floors[editor.selectedFloors[0].seqID - ((editor.selectedFloors[0].seqID != 0) ? 1 : 0)] : editor.floors[editor.selectedFloors.Last().seqID - 1], editor.multiSelectPoint);
			}
		}
	}
}
