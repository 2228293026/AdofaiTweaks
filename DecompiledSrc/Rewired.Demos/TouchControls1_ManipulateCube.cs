using System;
using UnityEngine;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class TouchControls1_ManipulateCube : MonoBehaviour
{
	public float rotateSpeed = 1f;

	public float moveSpeed = 1f;

	private int currentColorIndex;

	private Color[] colors = new Color[4]
	{
		Color.white,
		Color.red,
		Color.green,
		Color.blue
	};

	private void OnEnable()
	{
		if (ReInput.isReady)
		{
			Player player = ReInput.players.GetPlayer(0);
			if (player != null)
			{
				player.AddInputEventDelegate((Action<InputActionEventData>)OnMoveReceivedX, (UpdateLoopType)0, (InputActionEventType)33, "Horizontal");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnMoveReceivedX, (UpdateLoopType)0, (InputActionEventType)34, "Horizontal");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnMoveReceivedY, (UpdateLoopType)0, (InputActionEventType)33, "Vertical");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnMoveReceivedY, (UpdateLoopType)0, (InputActionEventType)34, "Vertical");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnCycleColor, (UpdateLoopType)0, (InputActionEventType)3, "CycleColor");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnCycleColorReverse, (UpdateLoopType)0, (InputActionEventType)3, "CycleColorReverse");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnRotationReceivedX, (UpdateLoopType)0, (InputActionEventType)33, "RotateHorizontal");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnRotationReceivedX, (UpdateLoopType)0, (InputActionEventType)34, "RotateHorizontal");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnRotationReceivedY, (UpdateLoopType)0, (InputActionEventType)33, "RotateVertical");
				player.AddInputEventDelegate((Action<InputActionEventData>)OnRotationReceivedY, (UpdateLoopType)0, (InputActionEventType)34, "RotateVertical");
			}
		}
	}

	private void OnDisable()
	{
		if (ReInput.isReady)
		{
			Player player = ReInput.players.GetPlayer(0);
			if (player != null)
			{
				player.RemoveInputEventDelegate((Action<InputActionEventData>)OnMoveReceivedX);
				player.RemoveInputEventDelegate((Action<InputActionEventData>)OnMoveReceivedY);
				player.RemoveInputEventDelegate((Action<InputActionEventData>)OnCycleColor);
				player.RemoveInputEventDelegate((Action<InputActionEventData>)OnCycleColorReverse);
				player.RemoveInputEventDelegate((Action<InputActionEventData>)OnRotationReceivedX);
				player.RemoveInputEventDelegate((Action<InputActionEventData>)OnRotationReceivedY);
			}
		}
	}

	private void OnMoveReceivedX(InputActionEventData data)
	{
		OnMoveReceived(new Vector2(((InputActionEventData)(ref data)).GetAxis(), 0f));
	}

	private void OnMoveReceivedY(InputActionEventData data)
	{
		OnMoveReceived(new Vector2(0f, ((InputActionEventData)(ref data)).GetAxis()));
	}

	private void OnRotationReceivedX(InputActionEventData data)
	{
		OnRotationReceived(new Vector2(((InputActionEventData)(ref data)).GetAxis(), 0f));
	}

	private void OnRotationReceivedY(InputActionEventData data)
	{
		OnRotationReceived(new Vector2(0f, ((InputActionEventData)(ref data)).GetAxis()));
	}

	private void OnCycleColor(InputActionEventData data)
	{
		OnCycleColor();
	}

	private void OnCycleColorReverse(InputActionEventData data)
	{
		OnCycleColorReverse();
	}

	private void OnMoveReceived(Vector2 move)
	{
		base.transform.Translate((Vector3)move * Time.deltaTime * moveSpeed, Space.World);
	}

	private void OnRotationReceived(Vector2 rotate)
	{
		rotate *= rotateSpeed;
		base.transform.Rotate(Vector3.up, 0f - rotate.x, Space.World);
		base.transform.Rotate(Vector3.right, rotate.y, Space.World);
	}

	private void OnCycleColor()
	{
		if (colors.Length != 0)
		{
			currentColorIndex++;
			if (currentColorIndex >= colors.Length)
			{
				currentColorIndex = 0;
			}
			GetComponent<Renderer>().material.color = colors[currentColorIndex];
		}
	}

	private void OnCycleColorReverse()
	{
		if (colors.Length != 0)
		{
			currentColorIndex--;
			if (currentColorIndex < 0)
			{
				currentColorIndex = colors.Length - 1;
			}
			GetComponent<Renderer>().material.color = colors[currentColorIndex];
		}
	}
}
