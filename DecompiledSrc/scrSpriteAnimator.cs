using UnityEngine;

public class scrSpriteAnimator : MonoBehaviour
{
	public float fps;

	public Sprite[] sprites;

	private SpriteRenderer spriteRenderer;

	private double frame;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (sprites.Length != 0)
		{
			frame = (frame + (double)(Time.deltaTime * fps)) % (double)((float)sprites.Length * 1f);
			int num = Mathf.FloorToInt((float)frame);
			if (num >= 0 && num < sprites.Length)
			{
				spriteRenderer.sprite = sprites[num];
			}
		}
	}
}
