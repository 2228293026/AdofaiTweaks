namespace ADOFAI.Editor.Actions;

public class Create360FloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			float rotDirection = ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(editor.selectedFloors[0].floatDirection, CW: true), CW: true);
			char rotDirection2 = ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(editor.selectedFloors[0].stringDirection, CW: true), CW: true);
			new CreateFloorWithCharOrAngleEditorAction(rotDirection, rotDirection2, pulseFloorButtons: true, fullSpin: true).Execute(editor);
		}
	}
}
