namespace ADOFAI.Editor.Actions;

public class CompositeEditorAction : EditorAction
{
	private EditorAction[] actions;

	public override EditorTabKey sectionKey => EditorTabKey.None;

	public CompositeEditorAction(params EditorAction[] actions)
	{
		this.actions = actions;
	}

	public override void Execute(scnEditor editor)
	{
		EditorAction[] array = actions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Execute(editor);
		}
	}
}
