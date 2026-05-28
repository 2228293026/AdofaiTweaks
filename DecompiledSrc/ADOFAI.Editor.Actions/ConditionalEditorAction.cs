using System;

namespace ADOFAI.Editor.Actions;

public class ConditionalEditorAction : EditorAction
{
	private Func<bool> evaluator;

	private Func<scnEditor, bool> editorEvaluator;

	private EditorAction trueAction;

	private EditorAction falseAction;

	public override EditorTabKey sectionKey => EditorTabKey.None;

	public ConditionalEditorAction(Func<bool> evaluator, EditorAction trueAction = null, EditorAction falseAction = null)
	{
		this.evaluator = evaluator;
		this.trueAction = trueAction;
		this.falseAction = falseAction;
	}

	public override void Execute(scnEditor editor)
	{
		if (evaluator())
		{
			trueAction?.Execute(editor);
		}
		else
		{
			falseAction?.Execute(editor);
		}
	}
}
