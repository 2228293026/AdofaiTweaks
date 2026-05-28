namespace ADOFAI.Editor.Actions;

public class OpenLevelEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.DeselectAnyUIGameObject();
		editor.OpenLevel();
	}
}
