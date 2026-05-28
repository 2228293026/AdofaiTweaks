using UnityEngine.EventSystems;

namespace ADOFAI.Editor.Actions;

public class CreateArbitraryFloorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		if (editor.floorButtonArbitraryContainer.activeSelf)
		{
			editor.CreateArbitraryFloor();
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
