using DG.Tweening;
using UnityEngine;

public class ControllerIcon : MonoBehaviour
{
	public SpriteRenderer background;

	public SpriteRenderer border;

	public SpriteRenderer[] tintableDevices;

	public ControllerIconHand[] hands;

	private int lastHandIndex;

	private void Awake()
	{
		ControllerIconHand[] array = hands;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetPressed(isPressed: false);
		}
	}

	public void Tap(float duration)
	{
		lastHandIndex = (lastHandIndex + 1) % hands.Length;
		ControllerIconHand hand = hands[lastHandIndex];
		hand.SetPressed(isPressed: true);
		DOVirtual.DelayedCall(duration, delegate
		{
			hand.SetPressed(isPressed: false);
		});
	}

	public static ControllerIcon Create(int playerIndex)
	{
		_ = scrPlayerManager.instance.players[playerIndex];
		ControllerType controllerType = ControllerType.None;
		if (controllerType == ControllerType.None)
		{
			return null;
		}
		ControllerIcon component = Object.Instantiate(Resources.Load<GameObject>("ControllerIcon-" + controllerType)).GetComponent<ControllerIcon>();
		for (int i = 0; i < component.tintableDevices.Length; i++)
		{
			_ = component.tintableDevices[i];
		}
		return component;
	}
}
