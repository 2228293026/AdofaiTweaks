namespace ADOFAI.Editor.Actions;

public class FlipFloorsVerticalEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.FlipFloor(editor.selectedFloors[0], horizontal: false);
			}
			else
			{
				editor.FlipSelection(horizontal: false);
			}
		}
	}
}
