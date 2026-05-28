using UnityEngine;

public class PaintingStretch : MonoBehaviour
{
	private void Awake()
	{
		Texture2D texture = GetComponent<SpriteRenderer>().sprite.texture;
		float num = (float)Screen.width * 1f / (float)Screen.height;
		float num2 = (float)texture.width * 1f / (float)texture.height;
		if (num > num2)
		{
			float num3 = num / num2;
			base.transform.ScaleXY(num3 * 10f);
		}
	}
}
