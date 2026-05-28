using ADOFAI.LevelEditor.Controls;

namespace ADOFAI.Editor.Actions;

public class LockSelectedDecorationsEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	private PropertyControl_DecorationsList propertyControlDecorationsList => scnEditor.instance.propertyControlDecorationsList;

	public override void Execute(scnEditor editor)
	{
		if (editor.userIsEditingAnInputField || editor.SelectionDecorationIsEmpty() || editor.userIsEditingAnInputField)
		{
			return;
		}
		bool locked = false;
		foreach (LevelEvent selectedDecoration in editor.selectedDecorations)
		{
			if (!selectedDecoration.locked)
			{
				locked = true;
			}
		}
		foreach (LevelEvent selectedDecoration2 in editor.selectedDecorations)
		{
			selectedDecoration2.locked = locked;
		}
		foreach (ListItem_Decoration shownItem in editor.propertyControlDecorationsList.ShownItems)
		{
			shownItem.ShowSideButtons();
		}
		if ((bool)propertyControlDecorationsList.CachedItemOnMouse)
		{
			((ListItem_Decoration)propertyControlDecorationsList.CachedItemOnMouse).ShowSideButtons(forceShow: true);
		}
	}
}
