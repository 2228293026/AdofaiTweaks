using UnityEngine;

namespace ADOFAI.LevelEditor.Controls;

internal static class SliderUtils
{
	public static float GetRoundedValue(float v)
	{
		if (!RDInput.holdingShift)
		{
			return Mathf.Round(v);
		}
		return Mathf.Round(v * 1000f) / 1000f;
	}

	public static Color GetHandleColor(this Color color, bool outOfRange)
	{
		color.g = (outOfRange ? 0.5f : color.r);
		color.b = (outOfRange ? 0.5f : color.r);
		return color;
	}
}
