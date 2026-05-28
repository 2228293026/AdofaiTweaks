namespace ADOFAI.Editor.Actions;

public class CreateUpsideDownHeptagonEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			char chara = '8';
			for (int i = 0; i < 7; i++)
			{
				editor.CreateFloorWithCharOrAngle(editor.selectedFloors[0].floatDirection - 51.42857f, chara);
			}
		}
	}
}
