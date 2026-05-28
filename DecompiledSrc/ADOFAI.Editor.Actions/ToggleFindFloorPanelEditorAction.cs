namespace ADOFAI.Editor.Actions;

public class ToggleFindFloorPanelEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Bookmarks;

	public override void Execute(scnEditor editor)
	{
		editor.ToggleFindFloorPanel();
	}
}
