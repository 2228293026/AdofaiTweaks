using System.Linq;

namespace ADOFAI.Editor.Actions;

public class SelectPreviousFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			editor.SelectFloor(editor.PreviousFloor(editor.selectedFloors.First()));
		}
	}
}
