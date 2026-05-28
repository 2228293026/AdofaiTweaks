using System;

namespace ADOFAI.Editor.Actions;

public class CreateFloorWithCharOrAngleEditorAction : EditorAction
{
	private float angle;

	private Func<float> angleFunc;

	private char chara;

	private Func<char> charaFunc;

	private bool pulseFloorButtons;

	private bool fullSpin;

	public override EditorTabKey sectionKey => EditorTabKey.None;

	public CreateFloorWithCharOrAngleEditorAction(float angle, char chara, bool pulseFloorButtons = true, bool fullSpin = false)
	{
		this.angle = angle;
		this.chara = chara;
		this.pulseFloorButtons = pulseFloorButtons;
		this.fullSpin = fullSpin;
	}

	public CreateFloorWithCharOrAngleEditorAction(Func<float> angleFunc, Func<char> charaFunc, bool pulseFloorButtons = true, bool fullSpin = false)
	{
		this.angleFunc = angleFunc;
		this.charaFunc = charaFunc;
		this.pulseFloorButtons = pulseFloorButtons;
		this.fullSpin = fullSpin;
	}

	public override void Execute(scnEditor editor)
	{
		if (angleFunc != null)
		{
			editor.CreateFloorWithCharOrAngle(angleFunc(), charaFunc(), pulseFloorButtons, fullSpin);
		}
		else
		{
			editor.CreateFloorWithCharOrAngle(angle, chara, pulseFloorButtons, fullSpin);
		}
	}
}
