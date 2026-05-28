namespace ADOFAI.Editor.Actions;

public class CreateUpsideDownPentagonEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.FlippingAndRotation;

	public override void Execute(scnEditor editor)
	{
		if (editor.SelectionIsSingle())
		{
			char chara = '6';
			for (int i = 0; i < 5; i++)
			{
				editor.CreateFloorWithCharOrAngle(editor.selectedFloors[0].floatDirection - 72f, chara);
			}
		}
	}
}
