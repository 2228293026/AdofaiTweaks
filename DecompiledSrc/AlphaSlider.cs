using UnityEngine;
using UnityEngine.UI;

public class AlphaSlider : MonoBehaviour
{
	public CUIColorPicker cuiColorPicker;

	public Image result;

	public Slider slider;

	public Image backgroundImage;

	private void OnEnable()
	{
		result = cuiColorPicker.result.GetComponent<Image>();
	}

	private void LateUpdate()
	{
		UpdateAlphaSlider();
	}

	public void UpdateResultAlpha()
	{
		if (result != null)
		{
			cuiColorPicker.Color = new Color(cuiColorPicker.Color.r, cuiColorPicker.Color.g, cuiColorPicker.Color.b, slider.value);
		}
	}

	public void UpdateAlphaSlider()
	{
		slider.value = cuiColorPicker.Color.a;
	}
}
