using UnityEngine;

public class FloorSpriteRenderer : FloorRenderer
{
	private Sprite iconSprite;

	public SpriteRenderer spriteRenderer => renderer as SpriteRenderer;

	public override Color color
	{
		get
		{
			return spriteRenderer.color;
		}
		set
		{
			_ = GetComponent<scrFloor>().seqID;
			spriteRenderer.color = value;
		}
	}

	public override Sprite sprite
	{
		get
		{
			return spriteRenderer.sprite;
		}
		set
		{
			spriteRenderer.sprite = value;
		}
	}

	public override void SetAngle(float entryAngle, float exitAngle)
	{
	}
}
