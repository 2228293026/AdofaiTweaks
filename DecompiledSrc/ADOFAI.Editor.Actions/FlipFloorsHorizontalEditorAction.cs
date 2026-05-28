namespace ADOFAI.Editor.Actions;

public class FlipFloorsHorizontalEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.SelectionIsSingle())
			{
				editor.FlipFloor(editor.selectedFloors[0]);
			}
			else
			{
				editor.FlipSelection();
			}
		}
	}
}
