namespace ADOFAI.Editor.Actions;

public class OpenLogDirectoryEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Other;

	public override void Execute(scnEditor editor)
	{
		RDEditorUtils.OpenLogDirectory();
	}
}
