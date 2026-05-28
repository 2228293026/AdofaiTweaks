using UnityEngine;
using UnityEngine.UI;

public class ThumbnailMaker_bad : MonoBehaviour
{
	public Texture2D background;

	public Texture2D overlay;

	public Texture2D userImage;

	private Color test = Color.red;

	public Texture2D AlphaBlend(Texture2D aBottom, Texture2D aTop, bool tint = false)
	{
		Color[] pixels = aBottom.GetPixels();
		Color[] pixels2 = aTop.GetPixels();
		int num = pixels.Length;
		Color[] array = new Color[num];
		for (int i = 0; i < num; i++)
		{
			Color color = pixels[i];
			Color color2 = pixels2[i];
			float a = color2.a;
			float num2 = 1f - color2.a;
			float num3 = a + num2 * color.a;
			Color color3 = (color2 * a + color * color.a * num2) / num3;
			if (tint)
			{
				color3.r -= 1f - test.r;
				color3.g -= 1f - test.g;
				color3.b -= 1f - test.b;
			}
			color3.a = num3;
			array[i] = color3;
		}
		Texture2D texture2D = new Texture2D(aTop.width, aTop.height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	private void Start()
	{
		GetComponent<Image>().sprite = Sprite.Create(AlphaBlend(background, overlay), new Rect(0f, 0f, 512f, 512f), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.Tight);
	}
}
