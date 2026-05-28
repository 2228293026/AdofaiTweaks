namespace nn.hid;

public class Mouse
{
	public const int StateCountMax = 16;

	public static MouseHandle GetHandle()
	{
		return default(MouseHandle);
	}

	public static MouseHandle GetDebugMouseHandle()
	{
		return default(MouseHandle);
	}

	public static void Initialize(MouseHandle handle)
	{
	}

	public static void GetState(ref MouseState pOutValue, MouseHandle handle)
	{
	}

	public static int GetStates(MouseState[] outValues, MouseHandle handle)
	{
		return 0;
	}
}
