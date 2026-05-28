namespace ADOFAI.Editor.Actions;

public class ToggleBookmarkEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Bookmarks;

	public override void Execute(scnEditor editor)
	{
		if (!editor.SelectionIsEmpty())
		{
			if (editor.levelData.levelEvents.Exists((LevelEvent e) => e.floor == editor.selectedFloors[0].seqID && e.eventType == LevelEventType.Bookmark))
			{
				editor.RemoveEventAtSelected(LevelEventType.Bookmark);
			}
			else
			{
				editor.AddEventAtSelected(LevelEventType.Bookmark);
			}
		}
	}
}
