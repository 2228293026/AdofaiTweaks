using System.Linq;
using UnityEngine;

public class RDInputType_Mouse : RDInputType
{
	private static readonly KeyCode[] MouseKeys = new KeyCode[7]
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6
	};

	public override int Main(ButtonState state)
	{
		if (!isActive || ADOBase.isMobile || !base.isPlaying || ((bool)scnCLS.instance && scnCLS.instance.optionsPanels.showingAnyPanel))
		{
			return 0;
		}
		MainStateCount stateCount = GetStateCount(state);
		if (stateCount.lastFrameUpdated == Time.frameCount)
		{
			return stateCount.keys.Count;
		}
		stateCount.lastFrameUpdated = Time.frameCount;
		stateCount.keys = (from key in MouseKeys
			where RDInputType_Keyboard.CheckKeyState(key, state)
			select new AnyKeyCode(key)).ToList();
		return stateCount.keys.Count;
	}

	public override bool Restart(ButtonState state)
	{
		return false;
	}

	public override bool Cancel(ButtonState state)
	{
		return false;
	}

	public override bool Back(ButtonState state)
	{
		return false;
	}

	public override bool Quit(ButtonState state)
	{
		return false;
	}

	public override bool Left(ButtonState state)
	{
		return false;
	}

	public override bool Right(ButtonState state)
	{
		return false;
	}

	public override bool Up(ButtonState state)
	{
		return false;
	}

	public override bool Down(ButtonState state)
	{
		return false;
	}

	public override bool LeftAlt(ButtonState state)
	{
		return false;
	}

	public override bool RightAlt(ButtonState state)
	{
		return false;
	}

	public override bool UpAlt(ButtonState state)
	{
		return false;
	}

	public override bool DownAlt(ButtonState state)
	{
		return false;
	}

	public override bool Action1(ButtonState state)
	{
		return false;
	}

	public override bool Action2(ButtonState state)
	{
		return false;
	}

	public override bool Confirm(ButtonState state)
	{
		return false;
	}

	public override bool FaceUp(ButtonState state)
	{
		return false;
	}

	public override bool FaceLeft(ButtonState state)
	{
		return false;
	}
}
