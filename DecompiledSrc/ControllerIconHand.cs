using UnityEngine;

public class ControllerIconHand : MonoBehaviour
{
	public SpriteRenderer hand;

	public Sprite unpressed;

	public Sprite pressed;

	public SpriteRenderer effect;

	public void SetPressed(bool isPressed)
	{
		hand.sprite = (isPressed ? pressed : unpressed);
		effect.enabled = isPressed;
	}
}
