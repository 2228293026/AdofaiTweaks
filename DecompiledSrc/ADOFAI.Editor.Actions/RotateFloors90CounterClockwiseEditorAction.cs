namespace ADOFAI.Editor.Actions;

public class RotateFloors90CounterClockwiseEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override string descriptionKey => "RotateFloors90";

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.RotateFloor(editor.selectedFloors[0], CW: false);
			}
			else
			{
				editor.RotateSelection(CW: false);
			}
		}
	}
}
