using UnityEngine;

public class ControllerIconTester : MonoBehaviour
{
	public ControllerIcon[] icons;

	public Color[] backgroundColors;

	public Color[] controllerColors;

	private int colorIndex;

	private void Awake()
	{
		RandomizeBackgroundColors();
		RandomizeControllerColors();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			RandomizeBackgroundColors();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			RandomizeControllerColors();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ControllerIcon[] array = icons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Tap(0.15f);
			}
		}
	}

	private void RandomizeBackgroundColors()
	{
		ControllerIcon[] array = icons;
		foreach (ControllerIcon obj in array)
		{
			colorIndex = (colorIndex + 1) % backgroundColors.Length;
			obj.border.color = backgroundColors[colorIndex];
		}
	}

	private void RandomizeControllerColors()
	{
		ControllerIcon[] array = icons;
		for (int i = 0; i < array.Length; i++)
		{
			SpriteRenderer[] tintableDevices = array[i].tintableDevices;
			foreach (SpriteRenderer obj in tintableDevices)
			{
				int num = Random.Range(0, controllerColors.Length);
				obj.color = controllerColors[num];
			}
		}
	}
}
