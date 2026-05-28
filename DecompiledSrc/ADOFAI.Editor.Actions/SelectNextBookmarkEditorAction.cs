namespace ADOFAI.Editor.Actions;

public class SelectNextBookmarkEditorAction : EditorAction
{
	private bool selectRelative;

	public override EditorTabKey sectionKey => EditorTabKey.Bookmarks;

	public SelectNextBookmarkEditorAction(bool selectRelative)
	{
		this.selectRelative = selectRelative;
	}

	public override void Execute(scnEditor editor)
	{
		editor.SelectBookmark(1, selectRelative);
	}
}
