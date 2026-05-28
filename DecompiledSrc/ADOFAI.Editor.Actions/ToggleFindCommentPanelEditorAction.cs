namespace ADOFAI.Editor.Actions;

public class ToggleFindCommentPanelEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Bookmarks;

	public override void Execute(scnEditor editor)
	{
		editor.ToggleFindCommentPanel();
	}
}
