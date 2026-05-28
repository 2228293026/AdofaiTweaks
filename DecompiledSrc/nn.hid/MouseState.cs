namespace nn.hid;

public readonly struct MouseState
{
	public readonly long samplingNumber;

	public readonly int x;

	public readonly int y;

	public readonly int deltaX;

	public readonly int deltaY;

	public readonly int wheelDelta;

	public readonly int sideWheelDelta;

	public readonly MouseButton buttons;

	public readonly MouseAttribute attributes;

	public override string ToString()
	{
		return $"Position({x},{y}) Delta({deltaX},{deltaY}) Wheel({wheelDelta},{sideWheelDelta}) [{buttons}] {attributes} {samplingNumber}";
	}
}
