namespace ADOFAI.Editor.Actions;

public class ToggleAutoEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Other;

	public override void Execute(scnEditor editor)
	{
		if (editor.lockPathEditing || editor.SelectionIsEmpty() || RDInput.holdingAlt)
		{
			editor.ToggleAuto();
		}
	}
}
