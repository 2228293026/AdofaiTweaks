namespace ADOFAI.Editor.Actions;

public class ToggleFloorNumsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Other;

	public override void Execute(scnEditor editor)
	{
		int num = (editor.SelectionIsSingle() ? editor.selectedFloors[0].seqID : (-1));
		editor.showFloorNums = !editor.showFloorNums;
		editor.RemakePath();
		if (num != -1)
		{
			editor.SelectFloor(editor.floors[num], cameraJump: false);
		}
	}
}
