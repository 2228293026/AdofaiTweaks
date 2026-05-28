namespace ADOFAI.Editor.Actions;

public class SaveLevelAsEditorAction : EditorAction
{
	private bool newLevel;

	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public SaveLevelAsEditorAction(bool newLevel = false)
	{
		this.newLevel = newLevel;
	}

	public override void Execute(scnEditor editor)
	{
		editor.DeselectAnyUIGameObject();
		editor.SaveLevelAs(newLevel);
	}
}
