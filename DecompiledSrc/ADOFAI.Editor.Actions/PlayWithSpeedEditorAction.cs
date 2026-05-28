namespace ADOFAI.Editor.Actions;

public class PlayWithSpeedEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		editor.Play();
	}
}
