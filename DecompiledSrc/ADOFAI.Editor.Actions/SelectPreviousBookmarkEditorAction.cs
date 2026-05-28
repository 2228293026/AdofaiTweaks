namespace ADOFAI.Editor.Actions;

public class SelectPreviousBookmarkEditorAction : EditorAction
{
	private bool selectRelative;

	public override EditorTabKey sectionKey => EditorTabKey.Bookmarks;

	public SelectPreviousBookmarkEditorAction(bool selectRelative)
	{
		this.selectRelative = selectRelative;
	}

	public override void Execute(scnEditor editor)
	{
		editor.SelectBookmark(-1, selectRelative);
	}
}
