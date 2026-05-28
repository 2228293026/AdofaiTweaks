using System;

namespace ADOFAI.Editor;

[Flags]
public enum KeyModifier
{
	None = 0,
	Shift = 1,
	Control = 2,
	Alt = 4,
	BackQuote = 8
}
