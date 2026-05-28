using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.Editor.Preferences;

public class EditorPreferencesTabButton : MonoBehaviour
{
	public Button button;

	public Image image;

	public TMP_Text text;

	public Color inactiveBackgroundColor;

	public Color inactiveForegroundColor;

	public Color activeBackgroundColor;

	public Color activeForegroundColor;

	public void SetSelected(bool selected)
	{
		var (color, color2) = GetColors(selected);
		image.color = color;
		text.color = color2;
	}

	private (Color, Color) GetColors(bool selected)
	{
		if (!selected)
		{
			return (inactiveBackgroundColor, inactiveForegroundColor);
		}
		return (activeBackgroundColor, activeForegroundColor);
	}
}
