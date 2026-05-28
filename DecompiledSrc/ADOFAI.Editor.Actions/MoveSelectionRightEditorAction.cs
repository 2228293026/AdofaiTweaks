using System.Linq;

namespace ADOFAI.Editor.Actions;

public class MoveSelectionRightEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override string descriptionKey => "MoveSelection";

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle() && editor.selectedFloors[0].seqID != editor.floors.Count - 1)
			{
				editor.MultiSelectFloors(editor.selectedFloors[0], editor.floors[editor.selectedFloors[0].seqID + 1], setSelectPoint: true);
			}
			else
			{
				editor.MultiSelectFloors((editor.selectedFloors.Last().seqID > editor.multiSelectPoint.seqID) ? editor.floors[editor.selectedFloors.Last().seqID + ((editor.selectedFloors.Last().seqID != editor.floors.Count - 1) ? 1 : 0)] : editor.floors[editor.selectedFloors[0].seqID + 1], editor.multiSelectPoint);
			}
		}
	}
}
