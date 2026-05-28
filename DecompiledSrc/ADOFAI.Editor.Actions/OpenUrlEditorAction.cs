namespace ADOFAI.Editor.Actions;

public class OpenUrlEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.EditorWorkflow;

	public override void Execute(scnEditor editor)
	{
		editor.CheckUnsavedChanges(delegate
		{
			editor.DeselectAnyUIGameObject();
			editor.ShowPopup(show: true, scnEditor.PopupType.OpenURL);
		}, skipCloseAnim: true);
	}
}
