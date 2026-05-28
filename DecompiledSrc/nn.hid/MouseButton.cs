using System;

namespace nn.hid;

[Flags]
public enum MouseButton
{
	Left = 1,
	Right = 2,
	Middle = 4,
	Forward = 8,
	Back = 0x10
}
