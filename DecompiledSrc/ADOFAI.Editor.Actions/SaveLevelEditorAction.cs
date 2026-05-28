namespace ADOFAI.Editor.Actions;

public class SaveLevelEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.DeselectAnyUIGameObject();
		editor.SaveLevel();
	}
}
