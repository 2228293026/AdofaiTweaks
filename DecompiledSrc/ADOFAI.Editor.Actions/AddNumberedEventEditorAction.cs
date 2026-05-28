namespace ADOFAI.Editor.Actions;

public class AddNumberedEventEditorAction : EditorAction
{
	private int number;

	public override EditorTabKey sectionKey => EditorTabKey.None;

	public AddNumberedEventEditorAction(int number)
	{
		this.number = number;
	}

	public override void Execute(scnEditor editor)
	{
		foreach (LevelEventButton item in editor.eventButtons[editor.currentCategory])
		{
			if (item.keyCode == number && item.page == editor.currentPage && item.enableButton)
			{
				editor.AddEventAtSelected(item.type);
			}
		}
	}
}
