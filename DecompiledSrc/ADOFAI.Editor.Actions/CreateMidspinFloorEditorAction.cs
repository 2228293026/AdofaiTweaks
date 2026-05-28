using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.Editor.Actions;

public class CreateMidspinFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override string descriptionKey => "Midspin";

	public override void Execute(scnEditor editor)
	{
		GameObject currentSelectedGameObject = editor.eventSystem.currentSelectedGameObject;
		if (!(currentSelectedGameObject != null) || !(currentSelectedGameObject.GetComponent<Selectable>() != null))
		{
			new CreateFloorWithCharOrAngleEditorAction(999f, '!', pulseFloorButtons: true, fullSpin: true).Execute(editor);
		}
	}
}
