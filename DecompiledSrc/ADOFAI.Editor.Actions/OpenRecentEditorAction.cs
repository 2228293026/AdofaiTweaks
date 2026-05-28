namespace ADOFAI.Editor.Actions;

public class OpenRecentEditorAction : EditorAction
{
	private bool checkCtrl;

	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public OpenRecentEditorAction(bool checkCtrl = false)
	{
		this.checkCtrl = checkCtrl;
	}

	public override void Execute(scnEditor editor)
	{
		editor.DeselectAnyUIGameObject();
		editor.OpenRecent(checkCtrl);
	}
}
