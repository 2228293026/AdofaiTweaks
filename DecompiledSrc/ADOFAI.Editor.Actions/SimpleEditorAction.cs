using System;

namespace ADOFAI.Editor.Actions;

public class SimpleEditorAction : EditorAction
{
	private Action action;

	public override EditorTabKey sectionKey => EditorTabKey.None;

	public SimpleEditorAction(Action action)
	{
		this.action = action;
	}

	public override void Execute(scnEditor editor)
	{
		action();
	}
}
