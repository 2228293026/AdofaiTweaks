namespace ADOFAI.Editor.Actions;

public class ShowCopyrightPopupEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.Other;

	public override void Execute(scnEditor editor)
	{
		editor.ShowPopup(show: true, scnEditor.PopupType.CopyrightWarning, skipAnim: true);
	}
}
