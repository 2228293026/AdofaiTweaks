using System.Linq;

namespace ADOFAI.Editor.Actions;

public class SelectNextFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.SelectionAndDeletion;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			editor.SelectFloor(editor.NextFloor(editor.selectedFloors.Last()));
		}
	}
}
