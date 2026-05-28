using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.Editor.Components.Gradients;

public class GradientGenerator : MonoBehaviour
{
	public RawImage image;

	public Gradient gradient;

	private const int Width = 600;

	private const int Height = 60;

	private Texture2D _texture;

	private void Awake()
	{
		UpdateGradient();
	}

	public void UpdateGradient()
	{
		if (!_texture)
		{
			_texture = new Texture2D(600, 60)
			{
				filterMode = FilterMode.Point
			};
			image.texture = _texture;
		}
		for (int i = 0; i < 600; i++)
		{
			Color color = gradient.Evaluate((float)i / 600f);
			for (int j = 0; j < 60; j++)
			{
				_texture.SetPixel(i, j, color);
			}
		}
		_texture.Apply();
	}
}
