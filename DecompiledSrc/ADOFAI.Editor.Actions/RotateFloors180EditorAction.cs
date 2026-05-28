namespace ADOFAI.Editor.Actions;

public class RotateFloors180EditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.RotateFloor180(editor.selectedFloors[0]);
			}
			else
			{
				editor.RotateSelection180();
			}
		}
	}
}
