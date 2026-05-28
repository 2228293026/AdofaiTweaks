namespace ADOFAI.Editor.Actions;

public class RotateFloors90ClockwiseEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override string descriptionKey => "RotateFloors90";

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.RotateFloor(editor.selectedFloors[0]);
			}
			else
			{
				editor.RotateSelection();
			}
		}
	}
}
