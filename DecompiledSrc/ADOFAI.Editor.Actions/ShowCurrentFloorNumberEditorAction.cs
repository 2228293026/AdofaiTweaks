using UnityEngine;

namespace ADOFAI.Editor.Actions;

public class ShowCurrentFloorNumberEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Other;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			editor.ShowNotification($"Currently selected floor is {editor.selectedFloors[0].seqID}!");
			Debug.Log($"THIS IS FLOOR {editor.selectedFloors[0].seqID} - entry time {editor.selectedFloors[0].entryTime} - entry beat {editor.selectedFloors[0].entryBeat}");
		}
	}
}
