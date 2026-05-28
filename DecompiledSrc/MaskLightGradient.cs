using UnityEngine;

public class MaskLightGradient : ADOBase
{
	private SpriteRenderer spriteRenderer;

	private SpriteRenderer darkness;

	private SpriteMask spriteMask;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteMask = base.transform.parent.GetComponent<SpriteMask>();
		darkness = Level.FindDecorationComponent<SpriteRenderer>("darkness");
		_ = darkness == null;
		_ = spriteRenderer == null;
		_ = (Object)(object)spriteMask == null;
	}

	private void Update()
	{
		spriteRenderer.color = Color.white.WithAlpha(darkness.color.a);
		((Renderer)(object)spriteMask).enabled = darkness.color.a > 0f;
	}
}
